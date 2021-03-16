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
            string error = string.Empty;
            TDI.Wrapper.DeleteAllBLECmds(() => { }, (err) => { error = err; });
            Assert.AreEqual(string.Empty, error, "MAKE SURE TO CLOSE ANY STORAGE FILES");
        }

        #endregion

        #region Create

        [Test]
        public void T001_Create01_Success() {
            this.log.InfoEntry("T001_Create_Success");
            TestHelpers.CatchUnexpected(() => {
                List<ScriptItem> items = new List<ScriptItem>();
                items.Add(new ScriptItem() { Display = "Close door", Command = "0" });
                items.Add(new ScriptItem() { Display = "Open door", Command = "1" });
                items.Add(new ScriptItem() { Display = "Lock door", Command = "2" });
                var ndx = this.CreateItem(items, "6195", BLE_DataType.UInt_8bit, "Demo uint 8 bit open close", "");
                Assert.NotNull(ndx, "Did not return index");
            });
        }


        [Test]
        public void T001_Create02_SingleItemOutOfRange() {
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
        public void T001_Create03_ItemInListOutOfRange() {
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
        public void T001_Create04_ValidateCount() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData();
                List<IIndexItem<BLECmdIndexExtraInfo>> index = this.GetCommandList(4);
                Assert.AreEqual(4, index.Count);
                Assert.AreEqual("6194", index[0].ExtraInfoObj.CharacteristicName);
                Assert.AreEqual("6195", index[1].ExtraInfoObj.CharacteristicName);
                Assert.AreEqual("6196", index[2].ExtraInfoObj.CharacteristicName);
                Assert.AreEqual("6197", index[3].ExtraInfoObj.CharacteristicName);

            });
        }


        [Test]
        public void T001_05CreateDemoSet() {
            this.log.InfoEntry("T004_04CreateDemoSet");
            TestHelpers.CatchUnexpected(() => {
                this.GetCommandList(0);
                W.CreateBLEDemoCmdsBool(this.OnSuccessDummy, this.AssertErr);
                W.CreateBLEDemoCmdsUint8(this.OnSuccessDummy, this.AssertErr);
                this.GetCommandList(2);
            });
        }

        #endregion

        #region Delete

        [Test]
        public void T002_Delete01_Second() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData();
                List<IIndexItem<BLECmdIndexExtraInfo>> index = this.GetCommandList(4);
                bool ok = false;
                string error = string.Empty;
                TDI.Wrapper.DeleteBLECmdSet(
                    index[1], (tf) => { ok = tf; }, (err) => { error = err; });
                Assert.AreEqual(string.Empty, error);
                Assert.True(ok, "Success but with false");
                index = GetCommandList(3);
                // Middle is gone
                Assert.AreEqual("6194", index[0].ExtraInfoObj.CharacteristicName);
                Assert.AreEqual("6196", index[1].ExtraInfoObj.CharacteristicName);
                Assert.AreEqual("6197", index[2].ExtraInfoObj.CharacteristicName);

            });
        }


        [Test]
        public void T002_Delet02_AllFiles() {
            this.log.InfoEntry("T001_Create_Success");
            TestHelpers.CatchUnexpected(() => {
                // First create 3
                this.SetupData();
                this.GetCommandList(4);
                W.DeleteAllBLECmds(this.OnSuccessDummy, this.AssertErr);
                this.AssertCompleteFired();
                this.GetCommandList(0);
            });
        }

        #endregion

        #region Add

        [Test]
        public void T003_Add01_Success() {
            TestHelpers.CatchUnexpected(() => {
                // Create list of 3 and validate
                this.SetupData();
                List<IIndexItem<BLECmdIndexExtraInfo>> index = this.GetCommandList(4);

                List<ScriptItem> items = new List<ScriptItem>();
                items.Add(new ScriptItem() { Display = "Close door", Command = "0" });
                items.Add(new ScriptItem() { Display = "Open door", Command = "1" });
                items.Add(new ScriptItem() { Display = "Lock door", Command = "2" });
                var ndx = this.CreateItem(items, "9999", BLE_DataType.UInt_16bit, "Fifth at 16 bits", "");
                Assert.NotNull(ndx, "Null returned when creating fourth");

                index = this.GetCommandList(5);


                Assert.AreEqual("Fifth at 16 bits", index[4].Display);
                Assert.AreEqual("9999", index[4].ExtraInfoObj.CharacteristicName);
                Assert.AreEqual(BLE_DataType.UInt_16bit, index[4].ExtraInfoObj.DataType);
                Assert.AreEqual(BLE_DataType.UInt_16bit.ToStr(), index[4].ExtraInfoObj.DataTypeDisplay);

            });
        }

        #endregion

        #region Save

        [Test]
        public void T004_EditSave01_Success() {
            TestHelpers.CatchUnexpected(() => {
                // Create list of 4 and validate
                this.SetupData();
                List<IIndexItem<BLECmdIndexExtraInfo>> index = this.GetCommandList(4);
                IIndexItem<BLECmdIndexExtraInfo> item = index[1];

                BLECommandSetDataModel dataModel = null;
                string error = string.Empty;
                TDI.Wrapper.RetrieveBLECmdSet(item,
                    (dm) => { dataModel = dm; },
                    (err) => { error = err; });

                Assert.AreEqual(string.Empty, error);
                Assert.NotNull(dataModel, "Data model");
                // Validate existing before edit
                Assert.AreEqual("6195", dataModel.CharacteristicName);
                Assert.AreEqual(BLE_DataType.UInt_8bit, dataModel.DataType);
                Assert.AreEqual("Demo uint 8 bit", dataModel.Display);
                Assert.AreEqual(3, dataModel.Items.Count, "Command item count in data model");
                Assert.AreEqual("Close door", dataModel.Items[0].Display);
                Assert.AreEqual("0", dataModel.Items[0].Command);

                // Change values
                dataModel.CharacteristicName = "9999";
                dataModel.DataType = BLE_DataType.UInt_64bit;
                dataModel.Display = "Demo uint 64 bit";
                dataModel.Items[0].Command = "10000";

                error = string.Empty;
                TDI.Wrapper.SaveBLECmdSet(item, dataModel, () => { }, (err) => { error = err; });
                Assert.AreEqual(string.Empty, error, "After save");


                // TODO index extra info not being updated. The actual object file is OK
                // need to retrieve the proper one. Not in previous order
                index = this.GetCommandList(4);
                // Iterate to find the correct data model
                item = null;
                for (int i = 0; i< index.Count; i++) {
                    if (index[i].ExtraInfoObj.CharacteristicName == "9999") {
                        item = index[i];
                        break;
                    }
                }
                Assert.NotNull(item, "Retrieve after save - index for 9999 not found");
                dataModel = null;
                error = string.Empty;
                TDI.Wrapper.RetrieveBLECmdSet(item,
                    (dm) => { dataModel = dm; },
                    (err) => { error = err; });

                Assert.AreEqual(string.Empty, error, "Retrieve after save");
                Assert.NotNull(dataModel, "Data model");
                Assert.AreEqual("9999", dataModel.CharacteristicName, "Retrieve after edit");
                Assert.AreEqual(BLE_DataType.UInt_64bit, dataModel.DataType, "Retrieve after edit");
                Assert.AreEqual("Demo uint 64 bit", dataModel.Display, "Retrieve after edit");
                Assert.AreEqual(3, dataModel.Items.Count, "Retrieve after edit");
                Assert.AreEqual("Close door", dataModel.Items[0].Display, "Retrieve after edit");
                Assert.AreEqual("10000", dataModel.Items[0].Command, "Retrieve after edit");
            });
        }


        [Test]
        public void T004_EditSave02_OutOfRange() {
            TestHelpers.CatchUnexpected(() => {
                // Create list of 3 and validate
                this.SetupData();
                List<IIndexItem<BLECmdIndexExtraInfo>> index = this.GetCommandList(4);
                IIndexItem<BLECmdIndexExtraInfo> item = index[1];

                BLECommandSetDataModel dataModel = null;
                string error = string.Empty;
                TDI.Wrapper.RetrieveBLECmdSet(item,
                    (dm) => { dataModel = dm; },
                    (err) => { error = err; });

                Assert.AreEqual(string.Empty, error);
                Assert.NotNull(dataModel, "Data model");
                // Validate existing before edit
                Assert.AreEqual("6195", dataModel.CharacteristicName);
                Assert.AreEqual(BLE_DataType.UInt_8bit, dataModel.DataType);
                Assert.AreEqual(3, dataModel.Items.Count);
                Assert.AreEqual("Close door", dataModel.Items[0].Display);
                Assert.AreEqual("0", dataModel.Items[0].Command);

                // Change value to force out of range for 8 bit
                dataModel.Items[0].Command = "10000";
                error = string.Empty;
                TDI.Wrapper.SaveBLECmdSet(item, dataModel, () => { }, (err) => { error = err; });
                Assert.AreEqual("Invalid Input", error, "After save");
            });
        }

        #endregion


        #region Retrieve lists

        [Test]
        public void T005_Filter01_AllNoNames() {
            TestHelpers.CatchUnexpected(() => {
                // Create list of 4 and validate
                this.SetupDataNoNames();
                var 
                t = this.GetFilteredCommandList("", BLE_DataType.Bool, 1, 0);
                t = this.GetFilteredCommandList("", BLE_DataType.UInt_8bit, 1, 0);
                t = this.GetFilteredCommandList("", BLE_DataType.UInt_16bit, 1, 0);
                t = this.GetFilteredCommandList("", BLE_DataType.UInt_32bit, 1, 0);
            });
        }

        [Test]
        public void T005_Filter02_AllWithNames() {
            TestHelpers.CatchUnexpected(() => {
                // Create list of 4 and validate
                this.SetupData();
                var
                t = this.GetFilteredCommandList("", BLE_DataType.Bool, 1, 0);
                t = this.GetFilteredCommandList("", BLE_DataType.UInt_8bit, 1, 0);
                t = this.GetFilteredCommandList("", BLE_DataType.UInt_16bit, 1, 0);
                t = this.GetFilteredCommandList("", BLE_DataType.UInt_32bit, 1, 0);
            });
        }


        [Test]
        public void T005_Filter03_AllWithNamesNameFilterOn() {
            TestHelpers.CatchUnexpected(() => {
                // Create list of 4 and validate
                this.SetupData();
                var
                t = this.GetFilteredCommandList("6194", BLE_DataType.Bool, 1, 1);
                t = this.GetFilteredCommandList("6195", BLE_DataType.UInt_8bit, 1, 1);
                t = this.GetFilteredCommandList("6196", BLE_DataType.UInt_16bit, 1, 1);
                t = this.GetFilteredCommandList("6197", BLE_DataType.UInt_32bit, 1, 1);
            });
        }

        [Test]
        public void T005_Filter04_AllWithNamesNameFilterOn_NoMatcth() {
            TestHelpers.CatchUnexpected(() => {
                // Create list of 4 and validate
                this.SetupData();
                var
                t = this.GetFilteredCommandList("X6194", BLE_DataType.Bool, 1, 0);
                t = this.GetFilteredCommandList("Y6195", BLE_DataType.UInt_8bit, 1, 0);
                t = this.GetFilteredCommandList("Z6196", BLE_DataType.UInt_16bit, 1,0);
                t = this.GetFilteredCommandList("W6197", BLE_DataType.UInt_32bit, 1, 0);
            });
        }


        [Test]
        public void T005_Filter04_MultiOfSameTypeNameFilterOn() {
            TestHelpers.CatchUnexpected(() => {
                // Create list of 4 and validate
                // The uint8 named one is 6195
                this.SetupData();
                this.CreateItem(this.CreateCmdList(0, 4), "", BLE_DataType.UInt_8bit, "Demo Uint8 - 1", "");
                this.CreateItem(this.CreateCmdList(0, 4), "", BLE_DataType.UInt_8bit, "Demo Uint8 - 2", "");
                this.CreateItem(this.CreateCmdList(0, 4), "", BLE_DataType.UInt_8bit, "Demo Uint8 - 3", "");
                this.CreateItem(this.CreateCmdList(0, 4), "", BLE_DataType.UInt_8bit, "Demo Uint8 - 4", "");
                this.CreateItem(this.CreateCmdList(0, 4), "", BLE_DataType.UInt_8bit, "Demo Uint8 - 5", "");
                this.CreateItem(this.CreateCmdList(0, 4), "", BLE_DataType.UInt_8bit, "Demo Uint8 - 6", "");
                this.CreateItem(this.CreateCmdList(0, 4), "", BLE_DataType.UInt_8bit, "Demo Uint8 - 7", "");
                this.CreateItem(this.CreateCmdList(0, 4), "", BLE_DataType.UInt_8bit, "Demo Uint8 - 8", "");

                var
                t = this.GetFilteredCommandList("6195", BLE_DataType.UInt_8bit, 9, 1);
                t = this.GetFilteredCommandList("", BLE_DataType.UInt_8bit, 9, 0);
            });
        }





        #endregion


        private List<IIndexItem<BLECmdIndexExtraInfo>> GetCommandList(int expectedCount) {
            List<IIndexItem<BLECmdIndexExtraInfo>> index = null;
            string error = string.Empty;
            TDI.Wrapper.GetBLECmdList(
                (ndx) => { index = ndx; },
                (err) => { error = err; });
            Assert.AreEqual(string.Empty, error);
            Assert.NotNull(index);
            Assert.AreEqual(expectedCount, index.Count, "GetCommandList index count");
            return index;
        }


        private Tuple<List<IIndexItem<BLECmdIndexExtraInfo>>, List<IIndexItem<BLECmdIndexExtraInfo>>>
            GetFilteredCommandList(string name, BLE_DataType dataType, int generalCount, int specificCount) {
            List<IIndexItem<BLECmdIndexExtraInfo>> general = null;
            List<IIndexItem<BLECmdIndexExtraInfo>> specific = null;

            string error = string.Empty;
            TDI.Wrapper.GetFilteredBLECmdList(
                dataType,
                name,
                (ndxG, ndxS) => { 
                    general = ndxG;
                    specific = ndxS;
                },
                (err) => { error = err; });
            Assert.AreEqual(string.Empty, error);
            Assert.NotNull(general);
            Assert.NotNull(specific);
            Assert.AreEqual(generalCount, general.Count, 
                string.Format("{0} GetFilteredCommandList general index count", dataType));
            Assert.AreEqual(specificCount, specific.Count, 
                string.Format("{0} GetFilteredCommandList Specific index count", dataType));
            return new Tuple<List<IIndexItem<BLECmdIndexExtraInfo>>, List<IIndexItem<BLECmdIndexExtraInfo>>>(general, specific);
        }





        private void SetupData() {
            List<ScriptItem> items = new List<ScriptItem>();
            items.Add(new ScriptItem() { Display = "Close door", Command = "0" });
            items.Add(new ScriptItem() { Display = "Open door", Command = "1" });
            var ndx = this.CreateItem(items, "6194", BLE_DataType.Bool, "Demo bool", "");
            Assert.NotNull(ndx, "Did not return index 0");

            items = new List<ScriptItem>();
            items.Add(new ScriptItem() { Display = "Close door", Command = "0" });
            items.Add(new ScriptItem() { Display = "Open door", Command = "1" });
            items.Add(new ScriptItem() { Display = "Lock door", Command = "2" });
            ndx = this.CreateItem(items, "6195", BLE_DataType.UInt_8bit, "Demo uint 8 bit", "");
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


        private void SetupDataNoNames() {
            List<ScriptItem> items = new List<ScriptItem>();
            items.Add(new ScriptItem() { Display = "Close door", Command = "0" });
            items.Add(new ScriptItem() { Display = "Open door", Command = "1" });
            var ndx = this.CreateItem(items, "", BLE_DataType.Bool, "Demo bool", "");
            Assert.NotNull(ndx, "Did not return index 0");

            items = new List<ScriptItem>();
            items.Add(new ScriptItem() { Display = "Close door", Command = "0" });
            items.Add(new ScriptItem() { Display = "Open door", Command = "1" });
            items.Add(new ScriptItem() { Display = "Lock door", Command = "2" });
            ndx = this.CreateItem(items, "", BLE_DataType.UInt_8bit, "Demo uint 8 bit", "");
            Assert.NotNull(ndx, "Did not return index 1");

            items = new List<ScriptItem>();
            items.Add(new ScriptItem() { Display = "Close sessame", Command = "10000" });
            items.Add(new ScriptItem() { Display = "Open sessame", Command = "10001" });
            ndx = this.CreateItem(items, "", BLE_DataType.UInt_16bit, "Demo uint 16 bit", "");
            Assert.NotNull(ndx, "Did not return index 2");

            items = new List<ScriptItem>();
            items.Add(new ScriptItem() { Display = "Close 32 bits", Command = "999999" });
            items.Add(new ScriptItem() { Display = "Open 32 bits", Command = "888888" });
            ndx = this.CreateItem(items, "", BLE_DataType.UInt_32bit, "Demo uint 32 bit", "");
            Assert.NotNull(ndx, "Did not return index 3");
        }


        private List<ScriptItem> CreateCmdList(int startCmd, int number) {
            List<ScriptItem> items = new List<ScriptItem>();
            for (int i = 0; i < number; i++) {
                items.Add(new ScriptItem() { Display =  string.Format("Command {0}", i), Command = startCmd.ToString() }); ;
                startCmd++;
            }
            return items;
        }



        private IIndexItem<BLECmdIndexExtraInfo> CreateItem(List<ScriptItem> items, string characteristicName, BLE_DataType dataType, string display, string errExpected) {
            IIndexItem<BLECmdIndexExtraInfo> index = null;
            string error = string.Empty;
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
