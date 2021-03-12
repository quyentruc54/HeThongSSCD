using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace NovaAlert.Common
{
    public static class LogService
    {
        #region Members
        private static readonly ILog logger = LogManager.GetLogger(typeof(LogService));

        public static ILog Logger
        {
            get { return logger; }
        }

        #endregion

        public static string LogFile { get; set; }

        #region Constructors
        static LogService()
        {
            LoadLogService(@"Log\");
        }
        public static void LogServiceConfig(string logPath)
        {
            if (logPath != string.Empty)
            {
                if (logPath.EndsWith(@"\") == false)
                {
                    logPath = logPath + @"\";
                }
                LoadLogService(logPath);
            }
        }

        private static void LoadLogService(string logPath)
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders(); /*Remove any other appenders*/

            RollingFileAppender fileAppender = new RollingFileAppender();
            fileAppender.AppendToFile = true;
            fileAppender.LockingModel = new FileAppender.MinimalLock();
            fileAppender.StaticLogFileName = false;
            fileAppender.RollingStyle = RollingFileAppender.RollingMode.Date;
            fileAppender.DatePattern = "yyyyMMdd";
            fileAppender.File = logPath;
            PatternLayout pl = new PatternLayout();
            pl.ConversionPattern = @"%date [%thread] %-5level %logger %ndc - %message%newline"; // "%d [%2%t] %-5p [%-10c] %m%n%n";
            pl.ActivateOptions();
            fileAppender.Layout = pl;
            fileAppender.ActivateOptions();
            log4net.Config.BasicConfigurator.Configure(fileAppender);

            LogFile = fileAppender.File;
        }

        #endregion
    }
}
