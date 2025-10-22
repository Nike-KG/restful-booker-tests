using AventStack.ExtentReports;
using RestfulBooker.Tests.Utils;

namespace RestfulBooker.Tests.Api;

public abstract class BaseApiTest
{
    protected static ExtentReports Extent => ExtentReportManager.Instance;

    // Thread-safe test node
    protected ThreadLocal<ExtentTest> _test = new ThreadLocal<ExtentTest>();

    [SetUp]
    public void Setup()
    {
        // Create a test node per test method
        _test.Value = Extent.CreateTest(TestContext.CurrentContext.Test.Name);
    }

    [TearDown]
    public void TearDown()
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        var message = TestContext.CurrentContext.Result.Message;
        var stacktrace = TestContext.CurrentContext.Result.StackTrace;

        switch (status)
        {
            case NUnit.Framework.Interfaces.TestStatus.Passed:
                _test?.Value?.Pass("Test passed");
                break;

            case NUnit.Framework.Interfaces.TestStatus.Failed:
                _test?.Value?.Fail("Test failed: " + message);
                if (!string.IsNullOrEmpty(stacktrace))
                    _test?.Value?.Fail(stacktrace);
                break;

            case NUnit.Framework.Interfaces.TestStatus.Skipped:
                _test?.Value?.Skip("Test skipped");
                break;

            default:
                _test?.Value?.Warning("Test status: " + status);
                break;
        }
    }
}
