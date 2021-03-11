using CommunicationStack.Net.Stacks;
using LanguageFactory.Net.data;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTestCases.Core.Wrapper.Utils;
using NUnit.Framework;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Collections.Generic;
using TestCaseSupport.Core;

namespace MultiCommTestCases.Core.Wrapper.Storage {

    [TestFixture]
    public class TerminatorsStorageTests : WrapperTestBase {

        #region Data

        ClassLog log = new ClassLog("TerminatorsStorageTests");
        private TerminatorDataModel changed = null;
        //private ScriptDataModel changedScriptUSB = null;

        #endregion

        #region Setup

        [OneTimeSetUp]
        public void TestSetSetup() { this.OneTimeSetup(); }

        [OneTimeTearDown]
        public void TestSetTeardown() { this.OneTimeTeardown(); }

        [SetUp]
        public void SetupEachTest() {
            TDI.Wrapper.DeleteAllTerminators(this.OnSuccessDummy, this.AssertOnDeleteAllErrMsg);
            this.PerTestSetup();
        }

        #endregion

        #region Create

        [Test]
        public void T01_Create01() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(1);
                List<IIndexItem<DefaultFileExtraInfo>> list = null;
                TDI.Wrapper.GetTerminatorList((ndx) => list = ndx, this.AssertErr);
                Assert.NotNull(list, "Terminators list not retrieved");
                Assert.AreEqual(1, list.Count, "List count");
                this.RetrieveAndValidate(list[0], "Terminators 0", this.CreateInfos(5));
            });
        }


        [Test]
        public void T01_Create02_IndexReturn() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(2);
                var t = this.CreateTerminatorDm("TestTerminator", 5);
                IIndexItem<DefaultFileExtraInfo> ndx = null;
                TDI.Wrapper.CreateNewTerminator(t.Display, t, x => ndx = x, this.AssertErr);
                Assert.NotNull(ndx, "Did not return terminator index");
                List<IIndexItem<DefaultFileExtraInfo>> list = null;
                TDI.Wrapper.GetTerminatorList((ndx) => list = ndx, this.AssertErr);
                Assert.NotNull(list, "Terminators list not retrieved");
                Assert.AreEqual(3, list.Count, "List count did not increase");
                this.RetrieveAndValidate(ndx, t.Display, this.CreateInfos(5));
            });
        }


        [Test]
        public void T01_Create03_NoName() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(1);
                string err = "";
                var t = this.CreateTerminatorDm("TestTerminator", 5);
                TDI.Wrapper.CreateNewTerminator("", t, this.OnSuccessDummy, e => err = e);
                AssertCompleteDidNotFire();
                Assert.AreEqual(this.GetMsg(MsgCode.EmptyName), err);
            });
        }


        [Test]
        public void T01_Create04_NullDataModel() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(1);
                string err = "";
                TDI.Wrapper.CreateNewTerminator("Blippo", null, this.OnSuccessDummy, e => err = e);
                AssertCompleteDidNotFire();
                Assert.AreEqual(this.GetMsg(MsgCode.UnknownError), err);
            });
        }

        #endregion

        #region Retrieve Data model

        [Test]
        public void T02_Retrieve01_NullIndex() {
            TestHelpers.CatchUnexpected(() => {
                string err = "";
                TerminatorDataModel dm = null;
                TDI.Wrapper.RetrieveTerminatorData(null, x => dm = x, e => err = e);
                Assert.Null(dm, "Should not have returned data model");
                Assert.AreEqual(this.GetMsg(MsgCode.NothingSelected), err);
            });
        }


        [Test]
        public void T02_Retrieve02_InvalidIndex() {
            TestHelpers.CatchUnexpected(() => {
                string err = "";
                TerminatorDataModel dm = null;
                TDI.Wrapper.RetrieveTerminatorData(new IndexItem<DefaultFileExtraInfo>(), x => dm = x, e => err = e);
                Assert.Null(dm, "Should not have returned data model");
                Assert.AreEqual(this.GetMsg(MsgCode.NotFound), err);
            });
        }


        [Test]
        public void T02_Retrieve03_ValidIndex() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(2);
                IIndexItem<DefaultFileExtraInfo> ndx = this.RetrieveList(2)[0];
                TerminatorDataModel data = null;
                TDI.Wrapper.RetrieveTerminatorData(ndx, d => { data = d; }, this.AssertErr);
                Assert.NotNull(data, "Failed to retrieve data");
                this.Validate(data, data.Display, this.CreateInfos(5));
            });
        }

        #endregion

        #region Save


        [Test]
        public void T03_Save01_Valid() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(3);
                var ndx = this.RetrieveList(3)[1];
                var data = this.RetrieveData(ndx);
                data.Display = "Fat Farmer";
                var infos = data.TerminatorInfos;
                infos.Clear();
                infos.Add(new TerminatorInfo(Terminator.ACK));
                infos.Add(new TerminatorInfo(Terminator.NUL));
                data.Init(infos);
                W.SaveTerminator(ndx, data, OnSuccessDummy, AssertErr);
                AssertCompleteFired();
                this.RetrieveAndValidate(ndx, "Fat Farmer", infos);
            });
        }


        [Test]
        public void T03_Save01_NullIndex() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(3);
                var dm = this.CreateTerminatorDm("Flip", 5);
                string err = "";
                W.SaveTerminator(null, dm, OnSuccessDummy, e => err = e);
                this.AssertCompleteDidNotFire();
                Assert.AreEqual(GetMsg(MsgCode.NothingSelected), err);
            });
        }


        [Test]
        public void T03_Save01_NullData() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(3);
                var ndx = this.RetrieveList(3)[1];
                string err = "";
                W.SaveTerminator(ndx, null, OnSuccessDummy, e => err = e);
                this.AssertCompleteDidNotFire();
                Assert.AreEqual(GetMsg(MsgCode.SaveFailed), err);
            });
        }

        #endregion

        #region Delete

        [Test]
        public void T04_Delete01_Valid() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(3);
                var x = this.RetrieveList(3)[1];
                W.DeleteTerminatorData(x, this.OnSuccessAssertTrue, this.AssertErr);
                this.AssertCompleteFired();
                this.RetrieveList(2);
                string err = "";
                W.RetrieveTerminatorData(x, dm => { }, e => err = e);
                Assert.AreEqual(this.GetMsg(MsgCode.NotFound), err);
            });
        }


        [Test]
        public void T04_Delete02_NullIndex() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(3);
                var x = this.RetrieveList(3)[1];
                string err = "";
                W.DeleteTerminatorData(null, this.OnSuccessAssertTrue, e => err = e);
                this.AssertCompleteDidNotFire();
                Assert.AreEqual(this.GetMsg(MsgCode.NothingSelected), err);
            });
        }


        [Test]
        public void T04_Delete03_NotLast() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(1);
                var x = this.RetrieveList(1)[0];
                string err = "";
                W.DeleteTerminatorData(x, this.OnSuccessAssertTrue, e => err = e);
                this.AssertCompleteDidNotFire();
                Assert.AreEqual(this.GetMsg(MsgCode.CannotDeleteLast), err);
            });
        }


        [Test]
        public void T04_Delete01_ValidCurrentTeminatorChanged() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(5);

                // When current deleted it always sets new current to 0 pos in list
                var list = this.RetrieveList(5);
                var x = list[4];
                var x0 = list[0];
                var dm = this.RetrieveData(x);
                
                W.SetCurrentTerminators(dm, this.AssertErr);
                TerminatorDataModel current = null;
                W.GetCurrentTerminator(d => current = d, this.AssertErr);
                Assert.NotNull(current, "Failed to retrieve current terminator");
                Assert.AreEqual(dm.Display, current.Display, "Current not correctly set");

                W.DeleteTerminatorData(x, this.OnSuccessAssertTrue, this.AssertErr);
                this.AssertCompleteFired();

                dm = this.RetrieveData(x0);
                W.GetCurrentTerminator(d => current = d, this.AssertErr);
                Assert.NotNull(current, "Failed to retrieve current terminator");
                Assert.AreEqual(dm.Display, current.Display, "Current not correctly after previous current deleted");
            });
        }





        #endregion


        #region Private

        #region Events

        private void Subscribe() {
            this.changed = null;
            TDI.Wrapper.CurrentTerminatorChanged += changedOther;
            // TODO - look at the BT, BLE, etc terminator changes
        }

        private void Unsubscribe() {
            TDI.Wrapper.CurrentTerminatorChanged -= changedOther;
        }

        private void changedOther(object sender, TerminatorDataModel e) {
            this.changed = e;
        }

        private void AssertChanged() {
            Assert.NotNull(this.changed, "Did not receive terminator changed event");
            this.changed = null;
        }

        #endregion

        private TerminatorDataModel CreateTerminatorDm(string name, int count) {
            return new TerminatorDataModel(this.CreateInfos(count)) {
                Display = name,
            };
        }



        private void SetupData(uint count) {
            try {
                //this.Subscribe();
                for (uint i = 0; i < count; i++) {
                    TerminatorDataModel dm = this.CreateTerminatorDm(string.Format("Terminators {0}", i), 5);
                    TDI.Wrapper.CreateNewTerminator(dm.Display, dm, this.OnSuccessDummy, this.AssertErr);
                    this.AssertCompleteFired();
                    //this.AssertChanged();
                }
            }
            finally {
                //this.Unsubscribe();
            }
        }



        List<TerminatorInfo> CreateInfos(int count) {
            List<TerminatorInfo> list = new List<TerminatorInfo>();
            for (byte i = 0; i < count; i++) {
                TerminatorInfo ti = new TerminatorInfo((Terminator)i);
            }
            return list;
        }


        private TerminatorDataModel RetrieveData(IIndexItem<DefaultFileExtraInfo> index) {
            TerminatorDataModel data = null;
            TDI.Wrapper.RetrieveTerminatorData(index, d => { data = d; }, this.AssertErr);
            Assert.NotNull(data, "Failed to retrieve data");
            return data;
        }


        private List<IIndexItem<DefaultFileExtraInfo>> RetrieveList(int count) {
            List<IIndexItem<DefaultFileExtraInfo>> list = null;
            W.GetTerminatorList(l => list = l, AssertErr);
            Assert.NotNull(list, "Failed to get list");
            Assert.AreEqual(count, list.Count, "Invalid list count");
            return list;
        }



        private TerminatorDataModel RetrieveAndValidate(
            IIndexItem<DefaultFileExtraInfo> index, string display, List<TerminatorInfo> infos) {

            TerminatorDataModel data = this.RetrieveData(index);
            Assert.AreEqual(display, data.Display);
            Assert.AreEqual(infos.Count, data.TerminatorInfos.Count, "Terminator infos");
            Assert.AreEqual(infos.Count, data.TerminatorBlock.Length, "Terminator block");
            for (int i = 0; i < infos.Count; i++) {
                Assert.AreEqual(infos[i].Code, data.TerminatorInfos[i].Code);
                Assert.AreEqual(infos[i].Description, data.TerminatorInfos[i].Description);
                Assert.AreEqual(infos[i].Display, data.TerminatorInfos[i].Display);
                Assert.AreEqual(infos[i].HexDisplay, data.TerminatorInfos[i].HexDisplay);
                Assert.AreEqual(infos[i].Value, data.TerminatorInfos[i].Value);
            }
            return data;
        }



        private TerminatorDataModel Validate(TerminatorDataModel data, string display, List<TerminatorInfo> infos) {
            Assert.AreEqual(display, data.Display);
            Assert.AreEqual(infos.Count, data.TerminatorInfos.Count, "Terminator infos");
            Assert.AreEqual(infos.Count, data.TerminatorBlock.Length, "Terminator block");
            for (int i = 0; i < infos.Count; i++) {
                Assert.AreEqual(infos[i].Code, data.TerminatorInfos[i].Code);
                Assert.AreEqual(infos[i].Description, data.TerminatorInfos[i].Description);
                Assert.AreEqual(infos[i].Display, data.TerminatorInfos[i].Display);
                Assert.AreEqual(infos[i].HexDisplay, data.TerminatorInfos[i].HexDisplay);
                Assert.AreEqual(infos[i].Value, data.TerminatorInfos[i].Value);
            }
            return data;
        }



        #endregion

    }
}
