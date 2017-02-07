using Elders.Pandora;

namespace PushNotifications.WS
{
    public static class LogStartup
    {
        public static void Boot(Pandora pandora)
        {
            log4net.Config.XmlConfigurator.Configure();
            log4net.LogManager.GetRepository().AddElasticSearchAppender(pandora);
            log4net.LogManager.GetRepository().AddConsoleAppender(pandora);
            log4net.LogManager.GetRepository().AddRollingFileAppender(pandora);
        }

        public static void AddAppender(this log4net.Repository.ILoggerRepository loggerRepo, log4net.Appender.IAppender appender)
        {
            ((log4net.Repository.Hierarchy.Hierarchy)loggerRepo).Root.AddAppender(appender);
        }

        /// <summary>
        /// https://github.com/jptoto/log4net.ElasticSearch/wiki/01-Project-Setup
        /// </summary>
        /// <param name="loggerRepo"></param>
        /// <param name="pandora"></param>
        public static void AddElasticSearchAppender(this log4net.Repository.ILoggerRepository loggerRepo, Pandora pandora)
        {
            bool canConfigureAppender = pandora.Get<bool>("log_elasticsearch_enabled");

            if (canConfigureAppender)
            {
                var server = new System.Uri(pandora.Get("log_elasticsearch_server"));

                var appender = new log4net.ElasticSearch.ElasticSearchAppender();
                appender.Name = "ElasticSearchAppender";
                var port = server.Port == -1 || server.Port == 80 || server.Port == 443 ? 9200 : server.Port;
                var appName = pandora.ApplicationContext.ApplicationName.ToLowerInvariant();
                var appCluster = pandora.ApplicationContext.Cluster.ToLowerInvariant();
                var appMachine = pandora.ApplicationContext.Machine.ToLowerInvariant();
                appender.ConnectionString = $"Server={server.Host};Index=log-{appCluster}-{appMachine}-{appName};Port={port.ToString()};rolling=true";
                appender.Evaluator = new log4net.Core.LevelEvaluator(log4net.Core.Level.Error);
                appender.BufferSize = pandora.Get<int>("log_elasticsearch_buffer");
                appender.Threshold = log4net.Core.Level.All;
                appender.Fix = log4net.Core.FixFlags.None;
                appender.ActivateOptions();

                loggerRepo.AddAppender(appender);
            }
        }


        // <appender name="console" type="log4net.Appender.ColoredConsoleAppender">
        //      <mapping>
        //        <level value="ERROR" />
        //        <foreColor value="White" />
        //        <backColor value="Red, HighIntensity" />
        //      </mapping>
        //      <mapping>
        //        <level value="WARN" />
        //        <foreColor value="Yellow, HighIntensity" />
        //      </mapping>
        //      <mapping>
        //        <level value="INFO" />
        //        <foreColor value="Green" />
        //      </mapping>
        //      <layout type="log4net.Layout.PatternLayout">
        //        <conversionPattern value="%date %newline%message%newline%newline" />
        //      </layout>
        //  </appender>
        public static void AddConsoleAppender(this log4net.Repository.ILoggerRepository loggerRepo, Pandora pandora)
        {
            bool canConfigureAppender = pandora.Get<bool>("log_console_enabled");

            if (canConfigureAppender)
            {
                var server = new System.Uri(pandora.Get("log_elasticsearch_server"));

                var appender = new log4net.Appender.ColoredConsoleAppender();
                appender.Name = "console";
                appender.Layout = new log4net.Layout.PatternLayout("%date %newline%message%newline%newline");

                var errorMapping = new log4net.Appender.ColoredConsoleAppender.LevelColors();
                errorMapping.ForeColor = log4net.Appender.ColoredConsoleAppender.Colors.White;
                errorMapping.BackColor = log4net.Appender.ColoredConsoleAppender.Colors.Red & log4net.Appender.ColoredConsoleAppender.Colors.HighIntensity;
                errorMapping.Level = log4net.Core.Level.Error;
                appender.AddMapping(errorMapping);

                var warnMapping = new log4net.Appender.ColoredConsoleAppender.LevelColors();
                warnMapping.BackColor = log4net.Appender.ColoredConsoleAppender.Colors.Yellow & log4net.Appender.ColoredConsoleAppender.Colors.HighIntensity;
                warnMapping.Level = log4net.Core.Level.Warn;
                appender.AddMapping(warnMapping);

                var infoMapping = new log4net.Appender.ColoredConsoleAppender.LevelColors();
                infoMapping.BackColor = log4net.Appender.ColoredConsoleAppender.Colors.Green;
                infoMapping.Level = log4net.Core.Level.Info;
                appender.AddMapping(infoMapping);

                appender.ActivateOptions();

                loggerRepo.AddAppender(appender);
            }
        }

        // <remarks>
        // <appender name="rollingFile" type="log4net.Appender.RollingFileAppender,log4net">
        //     <threshold value="ALL" />
        //     <param name="File" value="${ProgramData}\OneBigSplash\Vapt\Vapt.IdentityAndAccess.WS.log.xml" />
        //     <param name="AppendToFile" value="true" />
        //     <param name="RollingStyle" value="Size" />
        //     <param name="DatePattern" value="yyyy.MM.dd" />
        //     <param name="StaticLogFileName" value="true" />
        //     <param name="maximumFileSize" value="100MB" />
        //     <param name="MaxSizeRollBackups" value="10" />
        //     <layout type="log4net.Layout.XmlLayoutSchemaLog4j">
        //         <locationInfo value="true" />
        //     </layout>
        // </appender>
        // </remarks>
        public static void AddRollingFileAppender(this log4net.Repository.ILoggerRepository loggerRepo, Pandora pandora)
        {
            bool canConfigureAppender = pandora.Get<bool>("log_file_enabled");

            if (canConfigureAppender)
            {
                var appender = new log4net.Appender.RollingFileAppender();

                appender.Name = "rollingFile";
                appender.File = System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData),
                    "OneBigSplash",
                    "Vapt",
                    "log",
                    pandora.ApplicationContext.ApplicationName + ".log.xml");

                appender.Threshold = log4net.Core.Level.All;
                appender.AppendToFile = true;
                appender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
                appender.MaximumFileSize = "100MB";
                appender.MaxSizeRollBackups = 25;
                appender.StaticLogFileName = true;
                appender.Layout = new log4net.Layout.XmlLayoutSchemaLog4j(true);
                appender.ActivateOptions();

                loggerRepo.AddAppender(appender);
            }
        }
    }
}
