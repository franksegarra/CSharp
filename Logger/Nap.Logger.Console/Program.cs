using System;
using System.Configuration;
using System.Globalization;
using System.Threading;

namespace Nap.Logger.Console
{
    public static class Program
    {
        public static void Main()
        {
            //var msg = "configured in code with ColoredConsoleAppender";
            //var msg = "from local Repo";

            var naplogger = new NapLogger(
                                            ConfigurationManager.AppSettings["Host"], 
                                            (LogEnvironmentType)Enum.Parse(typeof(LogEnvironmentType), ConfigurationManager.AppSettings["LogEnvironment"]),
                                            ConfigurationManager.AppSettings["LogLevel"]
                                         );

            naplogger.Debug("Test string only Debug with NapLogger");

            //for (var i = 0; i < 20000; i++)
            //{
            //    naplogger.Debug("Debug Message " + i + " from Console");
            //    naplogger.Information("Info Message " + i + " from Console");
            //    naplogger.Warning("Warning Message " + i + " from Console");
            //    naplogger.Error("Error Message " + i + " from Console");
            //    naplogger.Fatal("Fatal Message " + i + " from Console");
            //}

            //naplogger.Debug("Test string only Debug with NapLogger " + msg);
            //naplogger.Information("Test string only Information with NapLogger " + msg);
            //naplogger.Warning("Test string only Warning with NapLogger " + msg);
            //naplogger.Error("Test string only Error with NapLogger " + msg);
            //naplogger.Fatal("Test string only Fatal with NapLogger " + msg);

            //msg = "Final with exception";
            //var ex = new Exception("Testing exception passed to graylog");
            //naplogger.Debug("Test string and exception Debug with NapLogger " + msg, ex);
            //naplogger.Information("Test string and exception Information with NapLogger " + msg, ex);
            //naplogger.Warning("Test string and exception Warning with NapLogger " + msg, ex);
            //naplogger.Error("Test string and exception Error with NapLogger " + msg, ex);
            //naplogger.Fatal("Test string and exception Fatal with NapLogger " + msg, ex);

            //IFormatProvider formatProvider = new DateTimeFormatInfo();
            //const string format = "Writing to log at {0} on {1}";
            //naplogger.DebugFormat(formatProvider, format, DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortDateString());
            //naplogger.Information(formatProvider, format, DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortDateString());
            //naplogger.Warning(formatProvider, format, DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortDateString());
            //naplogger.Error(formatProvider, format, DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortDateString());
            //naplogger.Fatal(formatProvider, format, DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortDateString());

            System.Console.ReadKey();
        }
    }
}
