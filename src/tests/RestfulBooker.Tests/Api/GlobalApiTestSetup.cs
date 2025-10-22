using RestfulBooker.Tests.Utils;

namespace RestfulBooker.Tests.Api;

[SetUpFixture]
public class GlobalApiTestSetup
{
    [OneTimeSetUp]
    public void BeforeAllTests()
    {
        var extent = ExtentReportManager.Instance;
        extent.AddSystemInfo("Environment", "QA");
        extent.AddSystemInfo("Platform", "GitHub Actions");
    }

    [OneTimeTearDown]
    public void AfterAllTests()
    {
        ExtentReportManager.Instance.Flush();
    }
}
