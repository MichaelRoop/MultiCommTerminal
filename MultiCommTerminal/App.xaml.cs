﻿using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.interfaces;
using LanguageFactory.Messaging;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using LogUtils.Net;
using MultiCommTerminal.DependencyInjection;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace MultiCommTerminal {

    /// <summary>Interaction logic for App.xaml</summary>
    public partial class App : Application {

        #region Data

        private log4net.ILog loggerImpl = null;
        private ClassLog log = new ClassLog("App");

        #endregion

        #region Constructors

        public App() {
            this.SetupLogging();
            this.SetupDI();
        }

        #endregion

        private void SetupLogging() {
            this.loggerImpl = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            this.SetupLog4Net();

            Log.SetStackTools(new StackTools());
            WrapErr.SetStackTools(new StackTools());
            Log.SetVerbosity(MsgLevel.Info);
            Log.SetMsgNumberThreshold(5);
            Log.OnLogMsgEvent += new LogingMsgEventDelegate(this.Log_OnLogMsgEvent);
        }

        private void SetupDI() {
            // Just start it up so that it is loaded in this one place
            ErrReport err;
            WrapErr.ToErrReport(out err, 9999, () => {
                ICommWrapper w = DI.Wrapper();
            });
            if (err.Code != 0) {
                MessageBox.Show(err.Msg, "Critical Error loading DI container");
                Application.Current.Shutdown();
            }

        }


        #region Log Event Handlers

        /// <summary>Safely pass Log message to the logger implementation</summary>
        /// <param name="level">The loging level of the message</param>
        /// <param name="err">The error report object with the information</param>
        void Log_OnLogMsgEvent(MsgLevel level, ErrReport err) {
            if (this.loggerImpl != null) {
                try {
                    string msg = Log.GetMsgFormat1(level, err);
                    switch (level) {
                        case MsgLevel.Info:
                            loggerImpl.Info(msg);
                            break;
                        case MsgLevel.Debug:
                            loggerImpl.Debug(msg);
                            break;
                        case MsgLevel.Warning:
                            loggerImpl.Warn(msg);
                            break;
                        case MsgLevel.Error:
                        case MsgLevel.Exception:
                            loggerImpl.Error(msg);
                            break;
                    }

                    #if !SEND_LOG_TO_DEBUG
                        Debug.WriteLine(msg);
                    #endif
                }
                catch (Exception e) {
                    WrapErr.SafeAction(() => Debug.WriteLine(String.Format("Exception on logging out message:{0}", e.Message)));
                }
            }
        }


        /// <summary>Create a new header entry everytime the app starts up</summary>
        private void DumpLogHeader() {
            this.log.Warning(0, "");
            this.log.Warning(0, "************** New Session ****************");
            this.log.Warning(15664, string.Format("*  Day {0}", DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss")));
            //this.log.Warning(0, string.Format("*  {0}", ""));
            this.log.Warning(0, "**************************************");
        }

        #endregion

        #region Manual Log4Net configuration

        private void SetupLog4Net() {
            // Replaces the configuration loaded from file
            //ILoggerRepository repository = log4net.LogManager.GetRepository(Assembly.GetCallingAssembly());
            //FileInfo fileInfo = new FileInfo(@"log4net.config");
            //log4net.Config.XmlConfigurator.Configure(repository, fileInfo);

            //https://stackoverflow.com/questions/16336917/can-you-configure-log4net-in-code-instead-of-using-a-config-file
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository(Assembly.GetCallingAssembly());
            PatternLayout patternLayout = new PatternLayout();
            //patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ConversionPattern = "%message%newline";
            patternLayout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            // I had to use the manual configuration because the %env for special folders no longer working
            roller.File =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.None),
                    @"MultiCommSerialTerminal\Logs\log.txt");
            //roller.File = @"Logs\EventLog.txt";
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "3GB";
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.StaticLogFileName = true;
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            MemoryAppender memory = new MemoryAppender();
            memory.ActivateOptions();
            hierarchy.Root.AddAppender(memory);

            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;
        }

        #endregion

    }
}
