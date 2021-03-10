using CommunicationStack.Net.Stacks;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTestCases.Core.Wrapper.Utils;
using NUnit.Framework;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Text;
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

        #endregion

    }
}
