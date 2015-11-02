using System;
using System.Collections.Generic;
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

        private BackgroundJobServer _server;

        public void Start()
        {
            if (Environment.UserInteractive)
                Logger.ConsoleLog(LogLevel.Info, "Service is starting. Press Ctrl+C to cancel/stop at any time...");
            else
                Logger.Info("Service is starting; creating job scheduler");

            ConfigureJobManagement();
            LoadPlugins();
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
            var plugins = new List<IPlugin> {new ClientMergeEmails.Plugin()};

            foreach (var plugin in plugins)
            {
                plugin.ScheduleJobs();
            }
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

            TeardownPerformed = true;
        }

        protected override void DisposeManagedResources()
        {
            Teardown();
        }
    }
}
