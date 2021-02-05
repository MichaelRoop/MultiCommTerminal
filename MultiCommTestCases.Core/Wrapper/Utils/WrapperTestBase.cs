using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using NUnit.Framework;
using TestCaseSupport.Core;

namespace MultiCommTestCases.Core.Wrapper.Utils {

    public class WrapperTestBase : TestCaseBase {

        public override void OneTimeSetup() {
            base.OneTimeSetup();

            // Just evoke language to load the entire container
            ErrReport err;
            WrapErr.ToErrReport(out err, 9999, () => {
                TDI.Wrapper.CurrentStoredLanguage();
            });
            if (err.Code != 0) {
                Assert.Fail("Failed to load the DI container");
            }
        }


        public override void OneTimeTeardown() {
            base.OneTimeTeardown();
            TDI.Wrapper.Teardown();
        }

    }
}
