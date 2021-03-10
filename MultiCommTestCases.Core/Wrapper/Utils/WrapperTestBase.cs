using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.Net.data;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using LogUtils.Net;
using MultiCommWrapper.Net.interfaces;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using TestCaseSupport.Core;

namespace MultiCommTestCases.Core.Wrapper.Utils {

    public class WrapperTestBase {

        #region Data

        private log4net.ILog loggerImpl = null;
        private LogHelper logHelper = new LogHelper();
        protected HelperLogReader logReader = new HelperLogReader();
        protected bool completeFired = false;

        protected readonly string DIR_SCRIPTS = "Scripts";
        protected readonly string DIR_BLE_CMDS = "BleCommands";
        protected readonly string DIR_ETHERNET = "EthernetData";
        protected readonly string DIR_SERIAL = "SerialConfigurations";
        protected readonly string DIR_SETTINGS = "Settings";
        protected readonly string DIR_TERMINATORS = "Terminators";
        protected readonly string DIR_WIFI = "WifiCredentials";



        #endregion

        #region Properties

        protected static ICommWrapper W { get { return TDI.Wrapper; } }

        #endregion


        #region Setup

        public void OneTimeSetup() {
            this.SetupLogger();
            try {
                this.logReader.StartLogging();
            }
            catch (Exception e) {
                Debug.WriteLine("Start logging exception:{0}", e.Message);
            }


            // Just evoke language to load the entire container
            ErrReport err;
            WrapErr.ToErrReport(out err, 9999, () => {
                TDI.Wrapper.CurrentStoredLanguage();
            });
            if (err.Code != 0) {
                Assert.Fail("Failed to load the DI container");
            }
        }


        public void OneTimeTeardown() {
            TDI.Wrapper.Teardown();
            Thread.Sleep(500);
            this.logReader.StopLogging();
            this.logHelper.InfoMsgEvent -= this.LogHelper_InfoMsgEvent;
            this.logHelper.DebugMsgEvent -= this.LogHelper_DebugMsgEvent;
            this.logHelper.WarningMsgEvent -= this.LogHelper_WarningMsgEvent;
            this.logHelper.ErrorMsgEvent -= this.LogHelper_ErrorMsgEvent;
            this.logHelper.ExceptionMsgEvent -= this.LogHelper_ExceptionMsgEvent;
        }

        protected void PerTestSetup() {
            this.completeFired = false;
        }


        #endregion

        #region File Logger setup

        private void SetupLogger() {
            this.loggerImpl = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            this.SetupLog4Net();
            this.logHelper.InfoMsgEvent += this.LogHelper_InfoMsgEvent;
            this.logHelper.DebugMsgEvent += this.LogHelper_DebugMsgEvent;
            this.logHelper.WarningMsgEvent += this.LogHelper_WarningMsgEvent;
            this.logHelper.ErrorMsgEvent += this.LogHelper_ErrorMsgEvent;
            this.logHelper.ExceptionMsgEvent += this.LogHelper_ExceptionMsgEvent;
            //this.logHelper.EveryMsgEvent += this.LogHelper_EveryMsgEvent;
            this.logHelper.Setup(this.GetType().Name, MsgLevel.Info, false, 5);
        }

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
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.None),
                        @"MR_TestCases\MultiCommSerialTerminal\Logs\log.txt");

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

        #region Protected common tools

        protected void AssertErr(string err) {
            Assert.AreEqual(string.Empty, err);
        }


        protected void AssertOnDeleteAllErrMsg(string msg) {
            Assert.AreEqual(string.Empty, msg, "MAKE SURE TO CLOSE ANY STORAGE FILES");
        }

        protected void OnSuccessDummy() {
            completeFired = true;
        }

        protected void OnSuccessAssertTrue(bool ok) {
            completeFired = true;
            Assert.True(ok, "OnSuccess or OnComplete did not have true param");
        }

        protected void AssertCompleteFired() {
            Assert.True(this.completeFired, "OnComplete or OnSuccess did not fire");
            this.ResetCompleteFired();
        }

        protected void AssertCompleteDidNotFire() {
            Assert.False(this.completeFired, "OnComplete or OnSuccess should not have fired");
        }


        protected void ResetCompleteFired() {
            this.completeFired = false;
        }


        protected bool AreYouSureYes(string name) {
            return true;
        }

        protected bool AreYouSureNo(string name) {
            return false;
        }

        protected string March2021DataDir(string subDir) {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                "PreMarch10_2021_StoredData",
                "MultiCommSerialTerminal",
                subDir);


        }


        protected string TestDataDir(string subDir) {
            return Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.None),
                    "MR_TestCases",
                    "MultiCommSerialTerminal",
                    subDir);
        }

        protected void CopyTestDataMarch2021(string subDir) {
            string targetDir = this.TestDataDir(subDir);

            if (!Directory.Exists(targetDir)) {
                Directory.CreateDirectory(targetDir);
            }
            foreach (var file in Directory.GetFiles(this.March2021DataDir(subDir))) {

                string target = Path.Combine(targetDir, Path.GetFileName(file));
                Log.Info("", "", () => string.Format("Copy {0} to {1}", file, target));


                File.Copy(file, target);
            }

        }

        protected string GetMsg(MsgCode code) {
            return TDI.Wrapper.GetText(code);
        }

        #endregion

    }
}
