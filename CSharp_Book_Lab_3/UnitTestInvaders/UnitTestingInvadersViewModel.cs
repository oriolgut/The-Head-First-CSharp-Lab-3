using InvadersWindowsStoreApp.ViewModel;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer;

namespace UnitTestInvaders
{
    [TestClass]
    public class UnitTestingInvadersViewModel
    {
        [UITestMethod]
        public void TestRecreateScanLines()
        {
            InvadersViewModel viewModel = new InvadersViewModel();
            int oldScanLines = viewModel.ScanLines.Count;
            viewModel.RecreateScanLines();
            int newScanLines = viewModel.ScanLines.Count;

            Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.AreNotEqual(oldScanLines, newScanLines, "Should not be equal. \"newScanLines\" should contain Scan Lines.");
        }

    }
}