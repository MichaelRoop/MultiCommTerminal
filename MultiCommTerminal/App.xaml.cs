#define SEND_LOG_TO_DEBUG
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs.Utils;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MultiCommTerminal.NetCore {

    /// <summary>Interaction logic for App.xaml</summary>
    public partial class App : Application {

        #region Data

        private log4net.ILog loggerImpl = null;
        private ClassLog log = new ClassLog("App");
        private DateTime currentDate = DateTime.Now;
        private static App staticApp = null;
        LogHelper logHelper = new LogHelper();


        #endregion

        #region Properties

        public static App STATIC_APP {
            get {
                return staticApp;
            }
            private set {
                staticApp = value;
            }
        }

        /// <summary>Build number to display</summary>
        public static string Build {
            get {
                return "2021.01.14.05";
            }
        }

        #endregion

        #region Events

        public event EventHandler<string> LogMsgEvent;

        #endregion

        #region Constructors

        public App() {
            STATIC_APP = this;
            this.SetupExceptionHandlers();
            this.SetupLogging();
            this.SetupDI();
        }

        #endregion

        #region Setup methods

        private void SetupExceptionHandlers() {
            //// TODO - implement after next release
            this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += this.TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
        }


        private void SetupLogging() {
            this.loggerImpl = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            this.SetupLog4Net();

            this.logHelper.InfoMsgEvent += this.LogHelper_InfoMsgEvent;
            this.logHelper.DebugMsgEvent += this.LogHelper_DebugMsgEvent;
            this.logHelper.WarningMsgEvent += this.LogHelper_WarningMsgEvent;
            this.logHelper.ErrorMsgEvent += this.LogHelper_ErrorMsgEvent;
            this.logHelper.ExceptionMsgEvent += this.LogHelper_ExceptionMsgEvent;
            this.logHelper.EveryMsgEvent += this.LogHelper_EveryMsgEvent;
            this.logHelper.Setup(App.Build, MsgLevel.Info, true, 5);
        }


        private void SetupDI() {
            // Start it here for first load and retrieve stored language
            ErrReport err;
            WrapErr.ToErrReport(out err, 9999, () => {
                DI.Wrapper.CurrentStoredLanguage();
            });
            if (err.Code != 0) {
                MessageBox.Show(err.Msg, "Critical Error loading DI container");
                Application.Current.Shutdown();
            }

        }

        #endregion

        #region Unhandled Exception capture


        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            this.log.Error(9999, "CurrentDomain_UnhandledException", () => string.Format(""));
            // Cant do anything
            //App.ShowMsg("CurrentDomain_UnhandledException");
            //if (!e.IsTerminating) {
                this.ProcessException(e.ExceptionObject as Exception);
            //}

            // TODO - flag is exeption isTerminating but no other handlers has posted report

            // You can also retrieve the IsTerminating to find if thrown during termination
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
            this.log.Error(9999, "TaskScheduler_UnobservedTaskException", () => string.Format(""));
            // Gets exceptions from a Task
            
            //App.ShowMsg("TaskScheduler_UnobservedTaskException");
            this.ProcessException(e.Exception);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            this.log.Error(9999, "App_DispatcherUnhandledException", () => string.Format(""));
            // Gets errors out of the UI. Need to decide what to do
            //App.ShowMsg("App_DispatcherUnhandledException");
            this.ProcessException(e.Exception as Exception);
        }


        private void ProcessException(Exception e) {
            App.ShowMsg("Process Exception");

            CrashReport.ShowBox(e);
            //if (e != null) {
            //    ErrReportException erex = e as ErrReportException;
            //    if (erex != null) {
            //        App.ShowMsg(erex.Report.Msg + erex.Report.StackTrace);
            //    }
            //    else {
            //        ErrReport report = WrapErr.GetErrReport(0, e.Message, e);
            //        App.ShowMsg(report.Msg + report.StackTrace);
            //    }

            //}
            //else {
            //    App.ShowMsg("Null Exception");
            //}

            Application.Current.Shutdown();
        }



        #endregion

        #region Log Event Handlers

        private void LogHelper_EveryMsgEvent(object sender, string msg) {
            STATIC_APP.DispatchProxy(() => this.LogMsgEvent?.Invoke(this, msg));
        }


        private void LogHelper_InfoMsgEvent(object sender, string msg) {
            this.loggerImpl.Info(msg);
        }


        private void LogHelper_DebugMsgEvent(object sender, string msg) {
            this.loggerImpl.Debug(msg);
        }


        private void LogHelper_WarningMsgEvent(object sender, string msg) {
            this.loggerImpl.Warn(msg);
        }


        private void LogHelper_ErrorMsgEvent(object sender, string msg) {
            this.loggerImpl.Error(msg);
        }


        private void LogHelper_ExceptionMsgEvent(object sender, string msg) {
            this.loggerImpl.Error(msg);
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
            //MultiCommSerialTerminal/Settings
            roller.File =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.None),
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

        #region Static helpers that provide Windows dispatch

        private void DispatchProxy(Action action) {
            this.Dispatcher.Invoke(new Action(() => {
                WrapErr.ToErrReport(9999, action.Invoke);
            }));
        }


        public static void ShowMsg(string msg) {
            STATIC_APP.DispatchProxy(()=> WindowHelpers.ShowMsg(msg));
        }


        public static void ShowMsgTitle(string title, string msg) {
            STATIC_APP.DispatchProxy(() => WindowHelpers.ShowMsgTitle(title, msg));
        }

        #endregion

    }
}
