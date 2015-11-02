using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using CyberCoders.Background.Diagnostics;
using CyberCoders.Core.System;
using Cyrus.Plugin.Common;
using Hangfire;
using Microsoft.Owin.Hosting;
using NLog;

namespace Cyrus.MicroServices
{
    internal class WindowsService : DisposableObject
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IDisposable _webApp;
        private FileSystemWatcher _pluginWatcher;

        private BackgroundJobServer _server;


        public void Start()
        {
            if (Environment.UserInteractive)
                Logger.ConsoleLog(LogLevel.Info, "Service is starting. Press Ctrl+C to cancel/stop at any time...");
            else
                Logger.Info("Service is starting; creating job scheduler");

            ConfigureJobManagement();
            SetupPluginFolderWatcher();
        }

        private void SetupPluginFolderWatcher()
        {
            var pluginFolder = PluginFolder();
            _pluginWatcher = new FileSystemWatcher(pluginFolder)
            {
                Filter = "*.dll",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size,
                EnableRaisingEvents = true
            };
            _pluginWatcher.Changed += _pluginWatcher_Changed;
        }

        private static string PluginFolder()
        {
            var pluginFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\PluginsTemp");
            return pluginFolder;
        }

        private void _pluginWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            var fi = new FileInfo(e.FullPath);
            if (e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Changed
                && e.Name.StartsWith("Cyrus.") && fi.Extension == ".dll" && e.Name != "Cyrus.Plugin.Common.dll")
            {
                LoadPlugins();
            }
        }

        private void ConfigureJobManagement()
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(
                @"Server=.\sqlexpress; Database=CyrusMicroSvcHangfire; Integrated Security=SSPI;");

            const string baseAddress = "http://localhost:9000/";
            // hangire storage config must be done before spinning up web instance
            _webApp = WebApp.Start<Startup>(baseAddress);

            _server = new BackgroundJobServer();
        }

        private void LoadPlugins()
        {
            var components = new DirectoryCatalog(PluginFolder());
            var container = new CompositionContainer(components);
            container.ComposeParts(this);

            var plugins = container.GetExportedValues<IPlugin>().ToList();

            foreach (var plugin in plugins)
            {
                plugin.ScheduleJobs();
            }

            //var monitoring = JobStorage.Current.GetMonitoringApi();
            // reschedule existing jobs?
        }

        public void Stop()
        {
            Logger.Info("Service is stopping");
            Teardown();
        }

        private bool TeardownPerformed { get; set; }

        private void Teardown()
        {
            if (TeardownPerformed) return;

            _server?.Dispose();
            _webApp?.Dispose();

            if (_pluginWatcher != null)
            {
                _pluginWatcher.Changed -= _pluginWatcher_Changed;
                _pluginWatcher.Dispose();
            }

            TeardownPerformed = true;
        }

        protected override void DisposeManagedResources()
        {
            Teardown();
        }
    }
}
