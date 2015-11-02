using NLog;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.ServiceConfigurators;

namespace Cyrus.MicroServices
{
    internal static class WindowsServiceConfiguration
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Configure(HostConfigurator config)
        {
            // http://docs.topshelf-project.com/en/latest/
            Logger.Debug("Configuring windows service");
            config.Service<WindowsService>(ConfigureServiceClass);

            // prompt for credentials - Sys Admins will need to type password for SchTasks.Admin user
            config.RunAsPrompt();

            config.SetDescription("Cyrus MicroServices Background Processing (Multiple Mini-Apps)");
            config.SetDisplayName("Cyrus MicroServices");
            config.SetServiceName("Cyrus.MicroServices");

            //TODO: windows service settings
            //config.SetDescription(AppSettings.Default.WindowsService.Description);
            //config.SetDisplayName(AppSettings.Default.WindowsService.DisplayName);
            //config.SetServiceName(AppSettings.Default.WindowsService.Name);

            config.StartAutomatically();
            config.UseNLog();

            config.EnableServiceRecovery(rc =>
            {
                rc.RestartService(delayInMinutes: 2);
                //rc.RestartService(AppSettings.Default.WindowsService.RecoverRestartAfterMins);
            });

            Logger.Debug("Windows service configured");
        }

        private static void ConfigureServiceClass(ServiceConfigurator<WindowsService> svc)
        {
            svc.ConstructUsing(name => new WindowsService());
            svc.WhenStarted(es => es.Start());
            svc.WhenStopped(es => es.Stop());

            // use this later if/when needed
            // svc.WhenCustomCommandReceived()
        }
    }
}
