using System;
using CyberCoders.Background.Diagnostics;
using CyberCoders.Core.Diagnostics;
using NLog;
using Topshelf;

namespace Cyrus.MicroServices
{
    // http://stackoverflow.com/questions/835182/choosing-between-mef-and-maf-system-addin
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                WindowsEventLog.FailureAction = (exception, type)
                    => Logger.Error($"Failed to write message of type {type} to Windows event log: {exception}");
                HostFactory.Run(WindowsServiceConfiguration.Configure);
            }
            catch (Exception ex)
            {
                WindowsEventLog.TryWriteError($"Critical error in app setup: {ex}");
                Logger.ConsoleFatal(ex);
                throw;
            }
        }
    }
}
