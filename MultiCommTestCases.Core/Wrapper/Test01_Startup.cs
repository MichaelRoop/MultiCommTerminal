using LogUtils.Net;
using MultiCommTestCases.Core.Wrapper.Utils;
using NUnit.Framework;
using TestCaseSupport.Core;

namespace MultiCommTestCases.Core.Wrapper {

    [TestFixture]
    public class Test01_Startup : WrapperTestBase {

        ClassLog log = new ClassLog("Test01_Startup");

        #region Setup

        [OneTimeSetUp]
        public void TestSetSetup() {
            this.OneTimeSetup();
        }

        [OneTimeTearDown]
        public void TestSetTeardown() {
            this.OneTimeTeardown();
        }

        [SetUp]
        public void SetupEachTest() {
        }

        #endregion


        [Test]
        public void FirstTest() {
            TestHelpers.CatchUnexpected(() => {
                //Assert.True(true, "Test");
            });
        }






    }
}
