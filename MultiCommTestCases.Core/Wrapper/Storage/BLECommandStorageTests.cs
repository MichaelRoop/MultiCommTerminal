using BluetoothLE.Net.Enumerations;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommTestCases.Core.Wrapper.Utils;
using MultiCommWrapper.Net.interfaces;
using NUnit.Framework;
using StorageFactory.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using TestCaseSupport.Core;

namespace MultiCommTestCases.Core.Wrapper.Storage {


    [TestFixture]
    public class BLECommandStorageTests : WrapperTestBase {

        ClassLog log = new ClassLog("BLECommandStorageTests");

        #region Setup

        [OneTimeSetUp]
        public void TestSetSetup() { this.OneTimeSetup(); }

        [OneTimeTearDown]
        public void TestSetTeardown() { this.OneTimeTeardown(); }

        [SetUp]
        public void SetupEachTest() {
            TDI.Wrapper.DeleteAllBLECmds(() => { }, (err) => {});
        }

        #endregion

        int tstNumber = 0;

        [Test]
        public void T001_Create_Success() {
            this.tstNumber++;
            this.log.InfoEntry("T001_Create_Success");
            TestHelpers.CatchUnexpected(() => {
                List<ScriptItem> items = new List<ScriptItem>();
                items.Add(new ScriptItem() { Display = "Close door", Command = "0" });
                items.Add(new ScriptItem() { Display = "Open door", Command = "1" });
                items.Add(new ScriptItem() { Display = "Lock door", Command = "2" });
                var ndx =this.CreateItem(items, "6195", BLE_DataType.UInt_8bit, "Demo uint 8 bit open close", "");
                Assert.NotNull(ndx, "Did not return index");
            });
        }


        [Test]
        public void T002_Create_SingletemOutOfRange() {
            this.tstNumber++;
            TestHelpers.CatchUnexpected(() => {
                ScriptItem item = new ScriptItem() { Display = "Close door", Command = "3000" };
                string error = string.Empty;
                TDI.Wrapper.ValidateBLECmdItem(BLE_DataType.UInt_8bit, item,
                    () => { },
                    (err) => {
                        error = err;
                    });
                Assert.AreEqual("Invalid Input", error);
            });
        }


        [Test]
        public void T003_Create_ItemInListOutOfRange() {
            this.tstNumber++;
            TestHelpers.CatchUnexpected(() => {
                List<ScriptItem> items = new List<ScriptItem>();
                items.Add(new ScriptItem() { Display = "Close door", Command = "3000" });
                items.Add(new ScriptItem() { Display = "Open door", Command = "1" });
                items.Add(new ScriptItem() { Display = "Lock door", Command = "2" });
                var ndx = this.CreateItem(items, "6195", BLE_DataType.UInt_8bit, 
                    "Demo uint 8 bit open close", "Invalid Input");
                Assert.IsNull(ndx, "Should not return index");
            });
        }


        [Test]
        public void T004_Create_ValidateCount() {
            this.tstNumber++;
            TestHelpers.CatchUnexpected(() => {
                this.SetupData();
                //return;

                List<IIndexItem<BLECmdIndexExtraInfo>> index = null;
                string error = string.Empty;
                TDI.Wrapper.GetBLECmdList(
                    (ndx) => {
                        index = ndx;        
                    }, 
                    (err) => {
                        error = err;
                });
                Assert.AreEqual(string.Empty, error);
                Assert.NotNull(index);
                Assert.AreEqual(3, index.Count);
                Assert.AreEqual("6195", index[0].ExtraInfoObj.CharacteristicName);
                Assert.AreEqual("6196", index[1].ExtraInfoObj.CharacteristicName);
                Assert.AreEqual("6197", index[2].ExtraInfoObj.CharacteristicName);

            });
        }




        private void SetupData() {
            List<ScriptItem> items = new List<ScriptItem>();
            items.Add(new ScriptItem() { Display = "Close door", Command = "0" });
            items.Add(new ScriptItem() { Display = "Open door", Command = "1" });
            items.Add(new ScriptItem() { Display = "Lock door", Command = "2" });
            var ndx = this.CreateItem(items, "6195", BLE_DataType.UInt_8bit, "Demo uint 8 bit", "");
            Assert.NotNull(ndx, "Did not return index 1");

            items = new List<ScriptItem>();
            items.Add(new ScriptItem() { Display = "Close sessame", Command = "10000" });
            items.Add(new ScriptItem() { Display = "Open sessame", Command = "10001" });
            ndx = this.CreateItem(items, "6196", BLE_DataType.UInt_16bit, "Demo uint 16 bit", "");
            Assert.NotNull(ndx, "Did not return index 2");

            items = new List<ScriptItem>();
            items.Add(new ScriptItem() { Display = "Close 32 bits", Command = "999999" });
            items.Add(new ScriptItem() { Display = "Open 32 bits", Command =  "888888" });
            ndx = this.CreateItem(items, "6197", BLE_DataType.UInt_32bit, "Demo uint 32 bit", "");
            Assert.NotNull(ndx, "Did not return index 3");
        }


        private IIndexItem<BLECmdIndexExtraInfo> CreateItem(List<ScriptItem> items, string characteristicName, BLE_DataType dataType, string display, string errExpected) {
            IIndexItem<BLECmdIndexExtraInfo> index = null;
            string error = string.Empty;
            display = string.Format("{0}:{1}", display, this.tstNumber.ToString());

            BLECommandSetDataModel dm = new BLECommandSetDataModel(
                items, characteristicName, dataType, display);
            TDI.Wrapper.CreateBLECmdSet(dm.Display, dm, new BLECmdIndexExtraInfo(dm), (ndx) => {
                index = ndx;
            },
            (err) => {
                error = err;
            });
            Assert.AreEqual(errExpected, error);
            //Assert.NotNull(index, "Did not return index");
            return index;
        }




    }
}
