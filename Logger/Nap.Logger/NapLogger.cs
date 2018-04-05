using System;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Nap.Logger.Log4NetAppenders.Appenders;
using Nap.Logger.Log4NetAppenders.Layout;

namespace Nap.Logger
{
    public class NapLogger : INapLogger
    {
        private static ILog _logger;
        private static Level _logLevel;
        private static readonly string EntryAssemblyName;
        private static readonly string EntryAssemblyVersion;

        /// <summary>
        /// Logger logs all messages to GELF server
        /// Also writes to console with colored messages
        /// </summary>

        static NapLogger()
        {
            // A web application
            var myEntryAssembly = (System.Web.HttpContext.Current != null)
                ? System.Web.HttpContext.Current.ApplicationInstance.GetType().BaseType?.Assembly 
                : Assembly.GetEntryAssembly();

            //If we didn't pick up an assembly name then use Nap.Logger as default
            if (myEntryAssembly == null)
            {
                EntryAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                EntryAssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            else
            {
                EntryAssemblyName = myEntryAssembly.GetName().Name;
                EntryAssemblyVersion = myEntryAssembly.GetName().Version.ToString();
            }
        }

        public NapLogger(string host, LogEnvironmentType environment, string logLevel) : this(host, EntryAssemblyName, environment, logLevel)
        { }

        public NapLogger(string host, string logName, LogEnvironmentType environment, string logLevel)
        {
            _logger = LogManager.GetLogger(logName);
            _logLevel = _logger.Logger.Repository.LevelMap[logLevel];

            #region add NapGelfAppender

            var napGelfAppender = new NapGelfAppender(host)
            {
                Name = "NapGelfAppender",
                Threshold = _logLevel,                
                Layout = new GelfLayout
                {
                    Facility = "NapGelfAppender",
                    AdditionalFields = "AppVersion:" + EntryAssemblyVersion + "," +
                                        "Environment:" + Enum.GetName(typeof(LogEnvironmentType), environment) + "," +
                                        "Level:%level",
                    ConversionPattern = "[%t] - %m",
                    IncludeLocationInformation = true
                }
            };
            napGelfAppender.ActivateOptions();
            BasicConfigurator.Configure(napGelfAppender);

            #endregion add NapGelfAppender

            #region add gelf4net GelfUdpAppender

            //Get parts from host
            //var myUri = new Uri(host);
            //var ipAddress = GetIpAddressFromHost(myUri);
            //var myPort = GetPortFromHost(myUri);

            ////Only set up appender if we have a valid ip and port
            //if (ipAddress != null && myPort != null) 
            //{

            //    var gelfudpappender = new GelfUdpAppender
            //    {
            //        RemoteAddress = ipAddress,
            //        RemotePort = (int)myPort,
            //        Layout = new GelfLayout
            //        {
            //            AdditionalFields = "AppVersion:" + EntryAssemblyVersion + "," +
            //                                "Environment:" + Enum.GetName(typeof(LogEnvironmentType), environment) + "," +
            //                                "Level:%level",
            //            ConversionPattern = "[%t] - %m",
            //            IncludeLocationInformation = true
            //        },
            //        Name = "GelfUdpAppender"
            //    };

            //    gelfudpappender.ActivateOptions();
            //    BasicConfigurator.Configure(gelfudpappender);
            //}

            #endregion add gel4net GelfUdpAppender

            #region add gel4net GelfHttpAppender

            //var gelfappender = new GelfHttpAppender
            //{
            //    Url = host,
            //    Layout = new GelfLayout
            //    {
            //        AdditionalFields = "AppVersion:" + EntryAssemblyVersion + "," +
            //                            "Environment:" + Enum.GetName(typeof(LogEnvironmentType), environment) + "," +
            //                            "Level:%level",
            //        ConversionPattern = "[%t] - %m",
            //        IncludeLocationInformation = true
            //    },
            //    Name = "GelfHttpAppender"
            //};
            //gelfappender.ActivateOptions();

            //BasicConfigurator.Configure(gelfappender);

            #endregion add gel4net GelfHttpAppender

            #region add ColoredConsoleAppender

            var coloredConsoleAppender = new ColoredConsoleAppender
            {
                Threshold = _logLevel,
                Layout = new PatternLayout("%date [%thread] %-5level %logger - %message%newline")
            };

            coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Debug,
                ForeColor = ColoredConsoleAppender.Colors.Cyan
                    | ColoredConsoleAppender.Colors.HighIntensity
            });
            coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Info,
                ForeColor = ColoredConsoleAppender.Colors.Green
                    | ColoredConsoleAppender.Colors.HighIntensity
            });
            coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Warn,
                ForeColor = ColoredConsoleAppender.Colors.Purple
                    | ColoredConsoleAppender.Colors.HighIntensity
            });
            coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Error,
                ForeColor = ColoredConsoleAppender.Colors.Red
                    | ColoredConsoleAppender.Colors.HighIntensity
            });
            coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Fatal,
                ForeColor = ColoredConsoleAppender.Colors.White
                    | ColoredConsoleAppender.Colors.HighIntensity,
                BackColor = ColoredConsoleAppender.Colors.Red
            });
            coloredConsoleAppender.ActivateOptions();

            BasicConfigurator.Configure(coloredConsoleAppender);

            #endregion add ColoredConsoleAppender

            #region add file appender

            //var fileAppender = new FileAppender
            //{
            //    AppendToFile = true,
            //    LockingModel = new FileAppender.MinimalLock(),
            //    Layout = new PatternLayout("%date [%thread] %-5level %logger - %message%newline"),
            //    File = @"C:\temp\log_batchsize500.txt"
            //};

            //fileAppender.ActivateOptions();
            //BasicConfigurator.Configure(fileAppender);

            #endregion add file appender

            #region add  BufferingForwardingAppender

            //var bfAppender = new BufferingForwardingAppender
            //{
            //    BufferSize = 100
            //};

            ////bfAppender.AddAppender(fileAppender);
            //bfAppender.AddAppender(napGelfAppender);

            //bfAppender.ActivateOptions();
            //BasicConfigurator.Configure(bfAppender);

            #endregion add  BufferingForwardingAppender
        }

        //private static int? GetPortFromHost(Uri myUri)
        //{
        //    if (myUri.IsDefaultPort)
        //        return null;
        //    else
        //        return myUri.Port;
        //}

        //private static IPAddress GetIpAddressFromHost(Uri myUri)
        //{
        //    var myHost = myUri.Host;

        //    //If myhost is IP then ok
        //    IPAddress ipAddress;
        //    var ipOk = IPAddress.TryParse(myUri.Host, out ipAddress);

        //    //otherwise get ip from host name
        //    if (ipOk == false)
        //    {
        //        try
        //        {
        //            ipAddress = Dns.GetHostEntry(myHost).AddressList[0];
        //            ipOk = true;
        //        }
        //        catch (Exception)
        //        {
        //            //Can't set up UDP Appender.  Just leave ipOk == false
        //        }
        //    }

        //    return ipOk ? ipAddress : null;
        //}

        //string only methods

        public void Debug(string message)
        {
            if (!_logger.IsDebugEnabled) return;
            _logger.Debug(message);
        }
        public void Information(string message)
        {
            if (!_logger.IsInfoEnabled) return;
            _logger.Info(message);
        }
        public void Warning(string message)
        {
            if (!_logger.IsWarnEnabled) return;
            _logger.Warn(message);
        }
        public void Error(string message)
        {
            if (!_logger.IsErrorEnabled) return;
            _logger.Error(message);
        }
        public void Fatal(string message)
        {
            if (!_logger.IsFatalEnabled) return;
            _logger.Fatal(message);
        }

        //exception+string methods
        public void Debug(string message, Exception exception)
        {
            if (!_logger.IsDebugEnabled) return;
            _logger.Debug(message, exception);
        }
        public void Information(string message, Exception exception)
        {
            if (!_logger.IsInfoEnabled) return;
            _logger.Info(message, exception);
        }
        public void Warning(string message, Exception exception)
        {
            if (!_logger.IsWarnEnabled) return;
            _logger.Warn(message, exception);
        }
        public void Error(string message, Exception exception)
        {
            if (!_logger.IsErrorEnabled) return;
            _logger.Error(message, exception);
        }
        public void Fatal(string message, Exception exception)
        {
            if (!_logger.IsFatalEnabled) return;
            _logger.Fatal(message, exception);
        }

        //IFormatProvider + string + args
        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (!_logger.IsDebugEnabled) return;
            _logger.DebugFormat(provider, format, args);
        }
        public void Information(IFormatProvider provider, string format, params object[] args)
        {
            if (!_logger.IsInfoEnabled) return;
            _logger.InfoFormat(provider, format, args);
        }
        public void Warning(IFormatProvider provider, string format, params object[] args)
        {
            if (!_logger.IsWarnEnabled) return;
            _logger.WarnFormat(provider, format, args);
        }
        public void Error(IFormatProvider provider, string format, params object[] args)
        {
            if (!_logger.IsErrorEnabled) return;
            _logger.ErrorFormat(provider, format, args);
        }
        public void Fatal(IFormatProvider provider, string format, params object[] args)
        {
            if (!_logger.IsFatalEnabled) return;
            _logger.FatalFormat(provider, format, args);
        }
    }
}
