﻿using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTestCases.Core.Wrapper.Utils;
using NUnit.Framework;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using TestCaseSupport.Core;

namespace MultiCommTestCases.Core.Wrapper.Storage {

    [TestFixture]
    public class ScriptStorageTests : WrapperTestBase {

        private ClassLog log = new ClassLog("ScriptStorageTests");

        #region Setup

        [OneTimeSetUp]
        public void TestSetSetup() { this.OneTimeSetup(); }

        [OneTimeTearDown]
        public void TestSetTeardown() { this.OneTimeTeardown(); }

        [SetUp]
        public void SetupEachTest() {
            TDI.Wrapper.DeleteAllScriptData(this.OnSuccessDummy, this.AssertOnDeleteAllErrMsg);
            this.PerTestSetup();
        }

        #endregion

        #region Back compatibility

        // Explicit has stopped working so it will run every time
        [Test, Explicit]
        public void T0_BackCompatible01_March2021() {
            TestHelpers.CatchUnexpected(() => {
                // Copy over pre-March 10, 2021 data
                this.CopyTestDataMarch2021(DIR_SCRIPTS);
                List<string> names = new List<string>();
                names.Add("Demo open close commands");
                names.Add("HC-05");
                // Only for first
                List<ScriptItem> scriptItems = new List<ScriptItem>();
                scriptItems.Add(new ScriptItem("Open door cmd", "OpenDoor"));
                scriptItems.Add(new ScriptItem("Close door cmd", "CloseDoor"));

                var list = this.RetrieveList();
                Assert.True(list.Count == 2, "List count");
                for (int i = 0; i < list.Count; i++) {
                    var item = this.RetrieveData(list[i]);
                    Assert.NotNull(item, "ScriptDataModel");
                    Assert.AreEqual(names[i], item.Display);
                    if (i == 0) {
                        Assert.IsTrue(item.Items.Count == 2, "Command items");
                        for (int j = 0; j < item.Items.Count; j++) {
                            Assert.AreEqual(scriptItems[i].Display, item.Items[i].Display);
                            Assert.AreEqual(scriptItems[i].Command, item.Items[i].Command);
                        }
                    }
                    //for (int j = 0; j < item.Items.Count; j++) {
                    //    TestContext.WriteLine("    {0}:{1}", item.Items[j].Display, item.Items[j].Command);
                    //}
                }
            });

        }

        #endregion

        #region Create

        [Test]
        public void T01_Create01_NoIndex() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(2);
                var list = this.RetrieveList(2);
            });
        }


        [Test]
        public void T01_Create02_WithIndex() {
            TestHelpers.CatchUnexpected(() => {
                var set1 = this.CreateScriptSet("Set1", 2);
                IIndexItem<DefaultFileExtraInfo> idx1 = null;
                TDI.Wrapper.CreateNewScript(
                    set1.Display, set1, (x) => { idx1 = x; }, this.AssertErr);

                var set2 = this.CreateScriptSet("Set2", 2);
                IIndexItem<DefaultFileExtraInfo> idx2 = null;
                TDI.Wrapper.CreateNewScript(
                    set2.Display, set2, (x) => { idx2 = x; }, this.AssertErr);
                this.RetrieveList(2);
            });
        }

        #endregion

        #region Retrieve

        [Test]
        public void T02_Retrieve01_Good() {
            TestHelpers.CatchUnexpected(() => {
                var list = this.SetupAndRetrieveData(3);
                List<ScriptItem> items = this.CreateScriptItems(4);
                for (int i = 0; i < 3; i++) {
                    this.RetrieveAndValidate(list[i], string.Format("Script Set {0}", i), items);
                }
            });
        }

        [Test]
        public void T02_Retrieve02_BadIndex() {
            TestHelpers.CatchUnexpected(() => {
                var list = this.SetupAndRetrieveData(3);
                string error = string.Empty;
                ScriptDataModel dataModel = null;
                TDI.Wrapper.RetrieveScriptData(
                    new IndexItem<DefaultFileExtraInfo>(Guid.NewGuid().ToString()),
                    (dm) => {
                        dataModel = dm;
                    },
                    (err) => {
                        error = err;
                    });
                Assert.AreEqual("Not Found", error);
                Assert.Null(dataModel);
            });
        }

        [Test]
        public void T02_Retrieve03_NullIndex() {
            TestHelpers.CatchUnexpected(() => {
                var list = this.SetupAndRetrieveData(3);
                string error = string.Empty;
                ScriptDataModel dataModel = null;
                TDI.Wrapper.RetrieveScriptData(null,
                    (dm) => {
                        dataModel = dm;
                    },
                    (err) => {
                        error = err;
                    });
                Assert.AreEqual("Nothing Selected", error);
                Assert.Null(dataModel);
            });
        }

        #endregion

        #region Delete

        [Test]
        public void T03_Delete00_Item() {
            TestHelpers.CatchUnexpected(() => {
                IIndexItem<DefaultFileExtraInfo> idx = this.SetupAndRetrieveData(3, 4)[1];
                // Delete middle one
                TDI.Wrapper.DeleteScriptData(idx, this.OnSuccessAssertTrue, AssertErr);
                var indexList = this.RetrieveList(2);
                List<ScriptItem> scriptItems = this.CreateScriptItems(4);
                RetrieveAndValidate(indexList[0], "Script Set 0", scriptItems);
                RetrieveAndValidate(indexList[1], "Script Set 2", scriptItems);
                this.AssertCompleteFired();
            });
        }


        [Test]
        public void T03_Delete01_ItemYes() {
            TestHelpers.CatchUnexpected(() => {
                IIndexItem<DefaultFileExtraInfo> idx = this.SetupAndRetrieveData(3,4)[1];
                // Delete middle one
                TDI.Wrapper.DeleteScriptData(
                    idx, idx.Display, this.AreYouSureYes, this.OnSuccessAssertTrue, AssertErr);
                var indexList = this.RetrieveList(2);
                List<ScriptItem> scriptItems = this.CreateScriptItems(4);
                RetrieveAndValidate(indexList[0], "Script Set 0", scriptItems);
                RetrieveAndValidate(indexList[1], "Script Set 2", scriptItems);
                this.AssertCompleteFired();
            });
        }


        [Test]
        public void T03_Delete02_ItemNo() {
            TestHelpers.CatchUnexpected(() => {
                TestHelpers.CatchUnexpected(() => {
                    IIndexItem<DefaultFileExtraInfo> idx = this.SetupAndRetrieveData(3, 4)[1];
                    // Delete middle one but answer No so it is not deleted
                    TDI.Wrapper.DeleteScriptData(
                        idx, idx.Display, this.AreYouSureNo, this.OnSuccessAssertTrue, AssertErr);
                    this.AssertCompleteDidNotFire();
                    var indexList = this.RetrieveList(3);
                    List<ScriptItem> scriptItems = this.CreateScriptItems(4);
                    RetrieveAndValidate(indexList[0], "Script Set 0", scriptItems);
                    RetrieveAndValidate(indexList[1], "Script Set 1", scriptItems);
                    RetrieveAndValidate(indexList[2], "Script Set 2", scriptItems);
                });
            });
        }


        [Test]
        public void T03_Delete03_Twice() {
            TestHelpers.CatchUnexpected(() => {
                IIndexItem<DefaultFileExtraInfo> idx = this.SetupAndRetrieveData(3, 4)[1];
                TDI.Wrapper.DeleteScriptData(
                    idx, idx.Display, this.AreYouSureYes, this.OnSuccessAssertTrue, AssertErr);
                this.AssertCompleteFired();
                // Base line. If it does not exist it still returns true. False is only for exceptions on actual delete
                TDI.Wrapper.DeleteScriptData(
                    idx, idx.Display, this.AreYouSureYes, this.OnSuccessAssertTrue, AssertErr);
                this.AssertCompleteFired();
            });
        }


        [Test]
        public void T03_Delete04_Null() {
            TestHelpers.CatchUnexpected(() => {
                Assert.False(this.completeFired, "TEST BEFORE - IS FIRED");
                this.SetupAndRetrieveData(3);
                string error = string.Empty;
                TDI.Wrapper.DeleteScriptData(
                    null, "Blipo", this.AreYouSureYes, this.OnSuccessAssertTrue, err => error = err);
                Assert.AreEqual("Nothing Selected", error);
                this.AssertCompleteDidNotFire();
            });
        }


        [Test]
        public void T03_Delete05_NullNoFiles() {
            TestHelpers.CatchUnexpected(() => {
                string error = string.Empty;
                TDI.Wrapper.DeleteScriptData(
                    null, "Blipo", this.AreYouSureYes, this.OnSuccessAssertTrue,
                    err => error = err);
                Assert.AreEqual("Nothing Selected", error);
                this.AssertCompleteDidNotFire();
            });
        }


        [Test]
        public void T03_Delete06_NoFileValidFormatIndex() {
            TestHelpers.CatchUnexpected(() => {
                // Need to create some valid entries to avoid tripping the "Cannot delete last"
                this.SetupData(2);
                // Dummy index item create that is not in index 
                IIndexItem<DefaultFileExtraInfo> idx = new IndexItem<DefaultFileExtraInfo>(Guid.NewGuid().ToString());
                // Base line. If it does not exist it still returns true. False is only for exceptions on actual delete
                TDI.Wrapper.DeleteScriptData(
                    idx, "Blipo", this.AreYouSureYes, this.OnSuccessAssertTrue, this.AssertErr);
                this.AssertCompleteFired();
            });
        }


        [Test]
        public void T03_Delete07_Yes_DeleteLast() {
            TestHelpers.CatchUnexpected(() => {
                TestHelpers.CatchUnexpected(() => {
                    IIndexItem<DefaultFileExtraInfo> idx = this.SetupAndRetrieveData(1, 4)[0];
                    string error = "";
                    bool ok = false;
                    TDI.Wrapper.DeleteScriptData(
                        idx, idx.Display, this.AreYouSureYes, (tf) => ok = tf, e => error = e);
                    Assert.AreEqual("Cannot delete last entry", error);
                    this.AssertCompleteDidNotFire();
                });
            });
        }


        [Test]
        public void T03_Delete08_DeleteLast() {
            TestHelpers.CatchUnexpected(() => {
                TestHelpers.CatchUnexpected(() => {
                    IIndexItem<DefaultFileExtraInfo> idx = this.SetupAndRetrieveData(1, 4)[0];
                    string error = "";
                    TDI.Wrapper.DeleteScriptData(
                        idx, this.OnSuccessAssertTrue, e => error = e);
                    Assert.AreEqual("Cannot delete last entry", error);
                    this.AssertCompleteDidNotFire();
                });
            });
        }



        // TODO - set new current on delete of current

        #endregion

        #region Edit and save

        [Test]
        public void T04_EditSave01_validateDisplayAndIndex() {
            TestHelpers.CatchUnexpected(() => {
                //Make su both name and index change
                var ndx = this.SetupAndRetrieveData(1)[0];
                var dm = this.RetrieveData(ndx);
                dm.Display = "Blipo";
                TDI.Wrapper.SaveScript(ndx, dm, this.OnSuccessDummy, this.AssertErr);
                this.AssertCompleteFired();

                var dm2 = this.RetrieveData(ndx);
                Assert.AreEqual("Blipo", dm2.Display, "Data model");
                Assert.AreEqual("Blipo", ndx.Display, "Index");
                var ndx2 = this.RetrieveList(1)[0];
                Assert.AreEqual("Blipo", ndx2.Display, "Index retrieved");
            });
        }


        #endregion

        #region Private

        private void SetupData(uint count, uint itemCount = 4) {
            for (uint i = 0; i < count; i++) {
                string name = string.Format("Script Set {0}", i);
                TDI.Wrapper.CreateNewScript(
                    name, this.CreateScriptSet(name, itemCount), this.OnSuccessDummy, this.AssertErr);
                this.AssertCompleteFired();
            }
        }


        private List<IIndexItem<DefaultFileExtraInfo>> SetupAndRetrieveData(uint count, uint itemCount = 4) {
            for (uint i = 0; i < count; i++) {
                string name = string.Format("Script Set {0}", i);
                TDI.Wrapper.CreateNewScript(
                    name, this.CreateScriptSet(name, itemCount), this.OnSuccessDummy, this.AssertErr);
                this.AssertCompleteFired();
            }
            return this.RetrieveList(count);
        }



        private ScriptDataModel CreateScriptSet(string name, uint itemCount) {
            return new ScriptDataModel(this.CreateScriptItems(itemCount)) {
                Display = name,
            };
        }


        private List<ScriptItem> CreateScriptItems(uint count) {
            List<ScriptItem> list = new List<ScriptItem>();
            for (uint i = 0; i < count; i++) {
                list.Add(new ScriptItem(
                    string.Format("Name{0}", i), 
                    string.Format("Cmd{0}", i)));
            }
            return list;
        }


        private List<IIndexItem<DefaultFileExtraInfo>> RetrieveList() {
            List<IIndexItem<DefaultFileExtraInfo>> list = null;
            TDI.Wrapper.GetScriptList(lst => { list = lst; }, this.AssertErr);
            return list;
        }


        private List<IIndexItem<DefaultFileExtraInfo>> RetrieveList(uint count) {
            List<IIndexItem<DefaultFileExtraInfo>> list = this.RetrieveList();
            Assert.NotNull(list, "Nothing retrieved");
            Assert.AreEqual(count, list.Count, "List count");
            return list;
        }



        private ScriptDataModel RetrieveData(IIndexItem<DefaultFileExtraInfo> index) {
            ScriptDataModel data = null;
            TDI.Wrapper.RetrieveScriptData(index, (d) => { data = d; }, this.AssertErr);
            Assert.NotNull(data);
            return data;
        }


        private ScriptDataModel RetrieveAndValidate(IIndexItem<DefaultFileExtraInfo> index, string display, List<ScriptItem> expected) {
            ScriptDataModel s = this.RetrieveData(index);
            Assert.AreEqual(display, s.Display, "Param Data");
            Assert.AreEqual(expected.Count, s.Items.Count, "Item count");
            for (int i = 0; i < s.Items.Count; i++) {
                this.ValidateScriptItem(expected[i], s.Items[i]);
            }
            return s;
        }


        private void ValidateScriptItem(ScriptItem expected, ScriptItem actual) {
            Assert.AreEqual(expected.Display, actual.Display);
            Assert.AreEqual(expected.Command, actual.Command);
        }


        #endregion

    }

}
