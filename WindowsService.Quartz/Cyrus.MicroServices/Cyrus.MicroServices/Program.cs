using System;
using System.Diagnostics;
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
                HostFactory.Run(WindowsServiceConfiguration.Configure);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Critical error in app setup: {ex}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
