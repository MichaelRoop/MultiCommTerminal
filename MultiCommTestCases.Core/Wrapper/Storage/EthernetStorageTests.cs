using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommTestCases.Core.Wrapper.Utils;
using NUnit.Framework;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TestCaseSupport.Core;

namespace MultiCommTestCases.Core.Wrapper.Storage {

    [TestFixture]
    public class EthernetStorageTests : WrapperTestBase {

        ClassLog log = new ClassLog("EthernetStorageTests");

        #region Setup

        [OneTimeSetUp]
        public void TestSetSetup() { this.OneTimeSetup(); }

        [OneTimeTearDown]
        public void TestSetTeardown() { this.OneTimeTeardown(); }

        [SetUp]
        public void SetupEachTest() {
            TDI.Wrapper.DeleteAllEthernetData(this.DummyOk, this.AssertOnDeleteAllErrMsg);
        }

        #endregion

        #region Creation

        [Test]
        public void T01_CreateNew() {
            TestHelpers.CatchUnexpected(() => {
                this.SetupData(1);
                List<IIndexItem<EthernetExtraInfo>> list = null;
                TDI.Wrapper.GetEthernetDataList(lst => {
                    list = lst;
                }, this.AssertErr);
                Assert.NotNull(list, "Nothing retrieved");
                Assert.AreEqual(1, list.Count, "List count");
            });
        }

        [Test]
        public void T02_CreateNewWithIndex() {
            TestHelpers.CatchUnexpected(() => {
                EthernetParams data = new EthernetParams() {
                    Display = "Extra param",
                    EthernetAddress = "192.168.1.33",
                    EthernetServiceName = "10000",
                };
                IIndexItem<EthernetExtraInfo> idx = null;
                TDI.Wrapper.CreateNewEthernetData(data, 
                    (x) => {
                        idx = x;

                    }, AssertErr);
                Assert.NotNull(idx, "Bad index item");
                this.SetupData(3);
                EthernetParams retrieved = this.RetrieveData(idx);
                Assert.NotNull(retrieved, "Bad retrieved params");
                Assert.AreEqual(data.UId, retrieved.UId);
                Assert.AreEqual(data.Display, retrieved.Display, "retrieved");
                Assert.AreEqual(data.EthernetAddress, retrieved.EthernetAddress, "retrieved");
                Assert.AreEqual(data.EthernetServiceName, retrieved.EthernetServiceName, "retrieved");
                Assert.AreEqual(data.Display, idx.Display, "idx");
                Assert.AreEqual(data.EthernetAddress, idx.ExtraInfoObj.Address, "idx");
                Assert.AreEqual(data.EthernetServiceName, idx.ExtraInfoObj.Port, "idx");
            });
        }



        [Test]
        public void T03_ValidateIndexExtraInfo() {
            TestHelpers.CatchUnexpected(() => {
                var index = this.SetupAndRetrieveList(1);
                Assert.AreEqual("192.168.1.0", index[0].ExtraInfoObj.Address);
                Assert.AreEqual("0", index[0].ExtraInfoObj.Port);
            });
        }

        #endregion

        #region Retrieval

        [Test]
        public void T04_RetrieveItem() {
            TestHelpers.CatchUnexpected(() => {
                var index = this.SetupAndRetrieveList(2);
                EthernetParams p = this.RetrieveData(index[0]);
                Assert.AreEqual("EthernetParam 0", p.Display);
                Assert.AreEqual("192.168.1.0", p.EthernetAddress);
                Assert.AreEqual("0", p.EthernetServiceName);
            });
        }

        #endregion

        #region Delete

        [Test]
        public void T05_DeleteItemYes() {
            TestHelpers.CatchUnexpected(() => {
                var index = this.SetupAndRetrieveList(3);
                // Delete middle one
                TDI.Wrapper.DeleteEthernetData(
                    index[1], index[1].Display, this.AreYouSureYes, this.DummyOk, AssertErr);
                index = this.RetrieveList(2);
                RetrieveAndValidate(index[0], "EthernetParam 0", "192.168.1.0", "0");
                RetrieveAndValidate(index[1], "EthernetParam 2", "192.168.1.2", "2");
            });
        }

        [Test]
        public void T06_DeleteItemNo() {
            TestHelpers.CatchUnexpected(() => {
                var index = this.SetupAndRetrieveList(3);
                // Delete middle one
                TDI.Wrapper.DeleteEthernetData(
                    index[1], index[1].Display, this.AreYouSureNo, this.DummyOk, AssertErr);
                index = this.RetrieveList(3);
            });
        }

        [Test]
        public void T07_DeleteTwice() {
            TestHelpers.CatchUnexpected(() => {
                var List = this.SetupAndRetrieveList(3);
                IIndexItem<EthernetExtraInfo> idx = List[0];
                bool ok = false;
                TDI.Wrapper.DeleteEthernetData(
                    idx, idx.Display, this.AreYouSureYes, (tf)=> { ok = tf; }, AssertErr);
                Assert.True(ok, "First delete");
                TDI.Wrapper.DeleteEthernetData(
                    idx, idx.Display, this.AreYouSureYes, (tf) => { ok = tf; }, AssertErr);
                // Base line. If it does not exist it still returns true. False is only for exceptions on actual delete
                Assert.True(ok, "Second delete");
            });
        }

        [Test]
        public void T08_DeleteNull() {
            TestHelpers.CatchUnexpected(() => {
                var List = this.SetupAndRetrieveList(3);
                IIndexItem<EthernetExtraInfo> idx = List[0];
                string err = string.Empty;
                TDI.Wrapper.DeleteEthernetData(
                    null, "dummy msg", 
                    this.AreYouSureYes, (tf) => {},  
                    (e) => { err = e; });
                Assert.AreEqual("Nothing Selected", err);
            });
        }


        [Test]
        public void T09_DeleteNullNoFiles() {
            TestHelpers.CatchUnexpected(() => {
                string err = string.Empty;
                TDI.Wrapper.DeleteEthernetData(
                    null, "dummy msg",
                    this.AreYouSureYes, (tf) => { },
                    (e) => { err = e; });
                Assert.AreEqual("Nothing Selected", err);
            });
        }


        [Test]
        public void T10_DeleteNoFileValidFormatIndex() {
            TestHelpers.CatchUnexpected(() => {
                IIndexItem<EthernetExtraInfo> idx = new IndexItem<EthernetExtraInfo>(Guid.NewGuid().ToString());
                string err = string.Empty;
                bool ok = true;
                TDI.Wrapper.DeleteEthernetData(
                    idx, "dummy msg",
                    this.AreYouSureYes, (tf) => { ok = tf; },
                    (e) => { err = e; });
                Assert.AreEqual("", err);
                // Base line. If it does not exist it still returns true. False is only for exceptions on actual delete
                Assert.True(ok, "is OK");
            });
        }

        #endregion

        #region Edit save

        [Test]
        public void T11_EditAndSave() {
            TestHelpers.CatchUnexpected(() => {
                List<IIndexItem<EthernetExtraInfo>> list = this.SetupAndRetrieveList(2);
                IIndexItem<EthernetExtraInfo> idx = list[0];
                EthernetParams p = this.RetrieveData(idx);
                string display = "BLAH PHUT";
                string address = "221.221.221.1";
                string port = "99";
                p.Display = display;
                p.EthernetAddress = address;
                p.EthernetServiceName = port;
                TDI.Wrapper.SaveEthernetData(idx, p, this.DummyOk, AssertErr);
                this.RetrieveAndValidate(idx, display, address, port);
            });
        }

        #endregion

        #region Others

        [Test]
        public void T12_MultiCreateDelete() {
            TestHelpers.CatchUnexpected(() => {
                // With 200 it was: 
                // Time to Create 200 - 672
                // Time to Delete 200 - 438
                Stopwatch sw = new Stopwatch();
                sw.Start();
                List<IIndexItem<EthernetExtraInfo>> list = this.SetupAndRetrieveList(20);
                sw.Stop();
                this.log.Info("", () => string.Format("Time to Create {0} - {1}", list.Count, sw.ElapsedMilliseconds));
                TestContext.WriteLine(string.Format("Time to Create {0} - {1}", list.Count, sw.ElapsedMilliseconds));
                sw.Restart();
                //int count = list.Count;


                // Need copy of list since the one loaded is being shrunk on each delete
                List<IIndexItem<EthernetExtraInfo>> list2 = new List<IIndexItem<EthernetExtraInfo>>();
                foreach (var item in list) {
                    list2.Add(item);
                }

                for (int i = 0; i < list2.Count; i++) {
                    TDI.Wrapper.DeleteEthernetData(list2[i], "msg", AreYouSureYes, this.DummyOk, AssertErr);
                }
                sw.Stop();
                this.log.Info("", () => string.Format("Time to Delete {0} - {1}", list2.Count, sw.ElapsedMilliseconds));
                TestContext.WriteLine(string.Format("Time to Delete {0} - {1}", list2.Count, sw.ElapsedMilliseconds));

                list = this.RetrieveList(0);

            });
        }

        #endregion

        #region Private

        private EthernetParams RetrieveAndValidate(IIndexItem<EthernetExtraInfo> index, string display, string address, string port) {
            EthernetParams p = this.RetrieveData(index);
            Assert.AreEqual(display, p.Display, "Param Data");
            Assert.AreEqual(address, p.EthernetAddress, "Param Data");
            Assert.AreEqual(port, p.EthernetServiceName, "Param Data");
            Assert.AreEqual(p.Display, index.Display, "index Data");
            Assert.AreEqual(p.EthernetAddress, index.ExtraInfoObj.Address, "index Data");
            Assert.AreEqual(p.EthernetServiceName, index.ExtraInfoObj.Port, "index Data");
            return p;
        }


        private EthernetParams RetrieveData(IIndexItem<EthernetExtraInfo> index) {
            EthernetParams data = null;
            TDI.Wrapper.RetrieveEthernetData(index, p => { data = p; }, this.AssertErr);
            Assert.NotNull(data, "Failed to retrieve");
            return data;
        }


        private List<IIndexItem<EthernetExtraInfo>> SetupAndRetrieveList(uint count) {
            this.SetupData(count);
            return this.RetrieveList(count);
        }


        private List<IIndexItem<EthernetExtraInfo>> RetrieveList(uint count) {
            List<IIndexItem<EthernetExtraInfo>> list = null;
            TDI.Wrapper.GetEthernetDataList(lst => {
                list = lst;
            }, this.AssertErr);
            Assert.NotNull(list, "Nothing retrieved");
            Assert.AreEqual(count, list.Count, "List count");
            return list;
        }



        private void SetupData(uint count) {
            for (uint i = 0; i < count; i++) {
                EthernetParams item = new EthernetParams() {
                    Display = string.Format("EthernetParam {0}", i),
                    EthernetAddress = string.Format("192.168.1.{0}", i),
                    EthernetServiceName = i.ToString(),
                };
                TDI.Wrapper.CreateNewEthernetData(item, this.DummyOk, this.AssertErr);
            }
        }


        private void AssertErr(string err) {
            Assert.AreEqual(string.Empty, err);
        }


        private void AssertOnDeleteAllErrMsg(string msg) {
            Assert.AreEqual(string.Empty, msg, "MAKE SURE TO CLOSE ANY STORAGE FILES");
        }

        private void DummyOk() {}

        private void DummyOk(bool ok) { Assert.True(ok); }

        private bool AreYouSureYes(string name) {
            return true;
        }

        private bool AreYouSureNo(string name) {
            return false;
        }

        #endregion

    }
}
