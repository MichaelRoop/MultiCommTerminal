using BluetoothCommon.Net;
using LogUtils.Net;
using MultiCommTestCases.Core.Wrapper.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TestCaseSupport.Core;

namespace MultiCommTestCases.Core.Wrapper.Bluetooth {

    [TestFixture]
    public class Test01_BluetoothClassicTests : WrapperTestBase {

        #region Data

        private ClassLog log = new ClassLog("BluetoothClassicTests");
        private List<BTDeviceInfo> devices = new List<BTDeviceInfo>();
        private AutoResetEvent discoveryComplet = new AutoResetEvent(false);
        private AutoResetEvent connectComplet = new AutoResetEvent(false);

        #endregion

        #region Setup

        [OneTimeSetUp]
        public void TestSetSetup() {
            this.OneTimeSetup();
            TDI.Wrapper.BT_DiscoveryComplete += this.Wrapper_BT_DiscoveryComplete;
            TDI.Wrapper.BT_DeviceDiscovered += this.Wrapper_BT_DeviceDiscovered;
            TDI.Wrapper.BTClassicDiscoverAsync(true);
            Assert.True(this.discoveryComplet.WaitOne(5000));
            Assert.True(this.devices.Count > 0, "Failed to find Paired BT devices");
        }


        [OneTimeTearDown]
        public void TestSetTeardown() {
            this.OneTimeTeardown();
            TDI.Wrapper.BT_DiscoveryComplete -= this.Wrapper_BT_DiscoveryComplete;
            TDI.Wrapper.BT_DeviceDiscovered -= this.Wrapper_BT_DeviceDiscovered;
        }

        [SetUp]
        public void SetupEachTest() {
        }

        #endregion


        [Test]
        public void Test01_ConnectDisconnect() {
            this.connectComplet.Reset();
            TDI.Wrapper.BT_ConnectionCompleted += Wrapper_BT_ConnectionCompleted;
            TestHelpers.CatchUnexpected(() => {
                TDI.Wrapper.BTClassicConnectAsync(this.devices[0]);
                Assert.True(this.connectComplet.WaitOne(5000), "Connect timed out");
                TDI.Wrapper.BTClassicDisconnect();
            });
            TDI.Wrapper.BT_ConnectionCompleted -= Wrapper_BT_ConnectionCompleted;
        }


        #region Event handlers

        private void Wrapper_BT_DeviceDiscovered(object sender, BTDeviceInfo e) {
            this.devices.Add(e);
        }

        private void Wrapper_BT_DiscoveryComplete(object sender, bool e) {
            this.discoveryComplet.Set();
        }

        private void Wrapper_BT_ConnectionCompleted(object sender, bool e) {
            this.connectComplet.Set();
        }

        #endregion

    }
}
