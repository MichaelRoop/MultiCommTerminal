using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTestCases.Core.Wrapper.Utils;
using NUnit.Framework;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.Enumerations;
using SerialCommon.Net.StorageIndexExtraInfo;
using StorageFactory.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using TestCaseSupport.Core;

namespace MultiCommTestCases.Core.Wrapper.Storage {

    [TestFixture]
    public class SerialStorageTests : WrapperTestBase {

        #region Data

        ClassLog log = new ClassLog("SerialStorageTests");
        private ScriptDataModel changedScript = null;
        private ScriptDataModel changedScriptUSB = null;

        #endregion

        #region Setup

        [OneTimeSetUp]
        public void TestSetSetup() { this.OneTimeSetup(); }

        [OneTimeTearDown]
        public void TestSetTeardown() { this.OneTimeTeardown(); }

        [SetUp]
        public void SetupEachTest() {
            TDI.Wrapper.DeleteAllSerialCfg(this.OnSuccessDummy, this.AssertOnDeleteAllErrMsg);
            this.PerTestSetup();
        }

        #endregion

        #region Creation T01

        [Test]
        public void T01_Create01_New() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(1);
                List<IIndexItem<SerialIndexExtraInfo>> list = null;
                TDI.Wrapper.GetSerialCfgList((ndx) => list = ndx, this.AssertErr);
                Assert.NotNull(list, "Nothing retrieved");
                Assert.AreEqual(1, list.Count, "List count");
                this.RetrieveAndValidate(list[0], "SerialCfg 0", 115200, 8, 
                    SerialStopBits.OnePointFive, SerialParityType.Even, 
                    SerialFlowControlHandshake.XonXoff);
            });
        }


        [Test]
        public void T01_Create02_CreateOrSave_Create() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(2);
                this.RetrieveList(2);
                var cfg = this.CreateDevice("Blipo 1");
                TDI.Wrapper.CreateOrSaveSerialCfg(cfg.Display, cfg, this.OnSuccessDummy, this.AssertErr);
                this.AssertCompleteFired();
                this.RetrieveList(3);
            });
        }


        [Test]
        public void T01_Create03_CreateOrSave_Save() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(2);
                IIndexItem<SerialIndexExtraInfo> ndx = this.RetrieveList(2)[0];
                var cfg = this.RetrieveData(ndx);
                cfg.Display = "BLAH BLAH BLAH";
                cfg.Baud = 55000;
                cfg.DataBits = 1;
                cfg.USB_VendorId = 44;
                cfg.USB_VendorIdDisplay = "blah";
                TDI.Wrapper.CreateOrSaveSerialCfg(cfg.Display, cfg, this.OnSuccessDummy, this.AssertErr);
                this.AssertCompleteFired();
                var list = this.RetrieveList(2);
                bool found = false;
                for (int i = 0; i< 2; i++) {
                    if (list[i].UId_Object == ndx.UId_Object) {
                        found = true;
                        this.RetrieveAndValidate(list[i], "BLAH BLAH BLAH", 55000, 1, cfg.StopBits, cfg.Parity, cfg.FlowHandshake);
                    }
                }
                Assert.True(found, "Did not find original index");



            });
        }


        [Test]
        public void T01_Create04_New_WithIndex() {
            TestHelpers.CatchUnexpected(() => {
                IIndexItem<SerialIndexExtraInfo> ndx = null;
                var data = CreateDevice("Biff boy");
                TDI.Wrapper.CreateNewSerialCfg(data.Display, data, x => ndx = x, this.AssertErr);
                Assert.NotNull(ndx, "Did not return index");
                this.RetrieveAndValidate(ndx, data.Display, data.Baud, data.DataBits, data.StopBits, data.Parity, data.FlowHandshake);
            });
        }

        #endregion

        #region Retrieval T02

        [Test]
        public void T02_RetrieveItem01_OK() {
            TestHelpers.CatchUnexpected(() => {
                var index = this.SetupAndRetrieveList(2);
                SerialDeviceInfo p = this.RetrieveData(index[0]);
                Assert.AreEqual("SerialCfg 0", p.Display);
            });
        }

        #endregion

        #region Delete T03

        [Test]
        public void T03_Delete01_NoQuestion() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(3);
                IIndexItem<SerialIndexExtraInfo> ndx = this.RetrieveList(3)[1];
                TDI.Wrapper.DeleteSerialCfg(ndx, this.OnSuccessAssertTrue, this.AssertErr);
                this.AssertCompleteFired();
                var list = this.RetrieveList(2);
                Assert.AreEqual("SerialCfg 0", list[0].Display);
                Assert.AreEqual("SerialCfg 2", list[1].Display);
            });
        }


        [Test]
        public void T03_Delete02_Yes_WithIndex() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(3);
                IIndexItem<SerialIndexExtraInfo> ndx = this.RetrieveList(3)[1];
                TDI.Wrapper.DeleteSerialCfg(ndx, "Bliff", this.AreYouSureYes, this.OnSuccessAssertTrue, this.AssertErr);
                this.AssertCompleteFired();
                var list = this.RetrieveList(2);
                Assert.AreEqual("SerialCfg 0", list[0].Display);
                Assert.AreEqual("SerialCfg 2", list[1].Display);
            });
        }


        [Test]
        public void T03_Delete03_No_WithIndex() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(3);
                IIndexItem<SerialIndexExtraInfo> ndx = this.RetrieveList(3)[1];
                TDI.Wrapper.DeleteSerialCfg(ndx, "Bliff", this.AreYouSureNo, this.OnSuccessAssertTrue, this.AssertErr);
                this.AssertCompleteDidNotFire();
                var list = this.RetrieveList(3);
            });
        }


        [Test]
        public void T03_Delete04_Yes_WithData() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(3);
                var data = this.RetrieveData(this.RetrieveList(3)[1]);
                Assert.AreEqual("SerialCfg 1", data.Display, "The Display from retrieved object");
                TDI.Wrapper.DeleteSerialCfg(data, this.AreYouSureYes, this.OnSuccessAssertTrue, this.AssertErr);
                this.AssertCompleteFired();
                var list = this.RetrieveList(2);
                Assert.AreEqual("SerialCfg 0", list[0].Display, "index 0");
                Assert.AreEqual("SerialCfg 2", list[1].Display, "index 1");
            });
        }


        [Test]
        public void T03_Delete05_No_WithData() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(3);
                var data = this.RetrieveData(this.RetrieveList(3)[1]);
                TDI.Wrapper.DeleteSerialCfg(data, this.AreYouSureNo, this.OnSuccessAssertTrue, this.AssertErr);
                this.AssertCompleteDidNotFire();
                var list = this.RetrieveList(3);
            });
        }

        #endregion

        #region Edit and save T04

        [Test]
        public void T04_Create01_EditAndSave() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(3);
                IIndexItem<SerialIndexExtraInfo> ndx = this.RetrieveList(3)[1];
                var cfg = this.RetrieveData(ndx);
                cfg.Display = "BLAH BLAH BLAH";
                cfg.Baud = 2;
                cfg.DataBits = 1;
                cfg.StopBits = SerialStopBits.Two;
                cfg.Parity = SerialParityType.Odd;
                cfg.FlowHandshake = SerialFlowControlHandshake.RequestToSendXonXoff;
                TDI.Wrapper.SaveSerialCfg(ndx, cfg, this.OnSuccessDummy, this.AssertErr);
                this.AssertCompleteFired();
                this.RetrieveAndValidate(ndx, "BLAH BLAH BLAH", 2, 1, SerialStopBits.Two, 
                    SerialParityType.Odd, SerialFlowControlHandshake.RequestToSendXonXoff);
            });
        }

        #endregion




        #region Private Tools

        #region Events

        private void SubscribeToChange() {
            this.changedScript = null;
            this.changedScriptUSB = null;
            TDI.Wrapper.CurrentScriptChangedUSB += scriptChangedUSB;
            TDI.Wrapper.CurrentScriptChanged += scriptChanged;
        }

        private void UnsubscribeToChange() {
            TDI.Wrapper.CurrentScriptChangedUSB -= scriptChangedUSB;
            TDI.Wrapper.CurrentScriptChanged -= scriptChanged;
            this.changedScript = null;
            this.changedScriptUSB = null;
            // TODO - check other test cases and code. when does specific device script change fire
        }


        private void scriptChanged(object sender, ScriptDataModel e) {
            this.changedScript = e;
        }

        private void scriptChangedUSB(object sender, ScriptDataModel e) {
            this.changedScriptUSB = e;
        }


        private void AssertScriptChangeFired() {
            Assert.NotNull(this.changedScript, "The change event not fired");
        }


        private void AssertUSBScriptChangeFired() {
            Assert.NotNull(this.changedScriptUSB, "The USB change event not fired");
        }

        #endregion


        private SerialDeviceInfo CreateDevice(string name) {
            return new SerialDeviceInfo() {
                Display = name,
                Baud = 115200,
                DataBits = 8,
                StopBits = SerialStopBits.OnePointFive,
                Parity = SerialParityType.Even,
                FlowHandshake = SerialFlowControlHandshake.XonXoff,
            };
        }


        private void SetupData(uint count) {
            for (uint i = 0; i < count; i++) {
                SerialDeviceInfo info = this.CreateDevice(string.Format("SerialCfg {0}", i));
                //SerialDeviceInfo info = new SerialDeviceInfo() {
                //    Display = string.Format("SerialCfg {0}", i),
                //    Baud = 115200,
                //    DataBits = 8,
                //    StopBits = SerialStopBits.OnePointFive,
                //    Parity = SerialParityType.Even,
                //    FlowHandshake = SerialFlowControlHandshake.XonXoff,
                //};
                this.SubscribeToChange();
                TDI.Wrapper.CreateNewSerialCfg(info.Display, info, this.OnSuccessDummy, this.AssertErr);
                this.AssertCompleteFired();
            }
        }


        private List<IIndexItem<SerialIndexExtraInfo>> RetrieveList(uint count) {
            List<IIndexItem<SerialIndexExtraInfo>> list = null;
            TDI.Wrapper.GetSerialCfgList(lst => {
                list = lst;
            }, this.AssertErr);
            Assert.NotNull(list, "Nothing retrieved");
            Assert.AreEqual(count, list.Count, "List count");
            return list;
        }


        private List<IIndexItem<SerialIndexExtraInfo>> SetupAndRetrieveList(uint count) {
            this.SetupData(count);
            return this.RetrieveList(count);
        }


        private SerialDeviceInfo RetrieveData(IIndexItem<SerialIndexExtraInfo> index) {
            SerialDeviceInfo data = null;
            TDI.Wrapper.RetrieveSerialCfg(index, p => { data = p; }, this.AssertErr);
            Assert.NotNull(data, "Failed to retrieve");
            return data;
        }


        private SerialDeviceInfo RetrieveAndValidate(
            IIndexItem<SerialIndexExtraInfo> index, string display, uint baud, ushort dataBits, SerialStopBits sbits, SerialParityType parity, SerialFlowControlHandshake hs) {
            SerialDeviceInfo p = this.RetrieveData(index);
            Assert.AreEqual(display, p.Display, "Param Data");
            Assert.AreEqual(baud, p.Baud, "Baud");
            Assert.AreEqual(dataBits, p.DataBits, "Data bits");
            Assert.AreEqual(sbits, p.StopBits, "Stop bits");
            Assert.AreEqual(parity, p.Parity, "Parity");
            Assert.AreEqual(hs, p.FlowHandshake, "Flow");
            // Extra info
            Assert.AreEqual(p.PortName, index.ExtraInfoObj.PortName, "Extra Info Port");
            Assert.AreEqual(p.USB_ProductIdDisplay, index.ExtraInfoObj.USBProduct, "Extra Info Product");
            Assert.AreEqual(p.USB_ProductId, index.ExtraInfoObj.USBProductId, "Extra Info Product id");
            Assert.AreEqual(p.USB_VendorIdDisplay, index.ExtraInfoObj.USBVendor, "Extra Info vendor");
            Assert.AreEqual(p.USB_VendorId, index.ExtraInfoObj.USBVendorId, "Extra Info vendor id");
            return p;
        }



        #endregion

    }

}
