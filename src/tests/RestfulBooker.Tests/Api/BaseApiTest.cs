using AventStack.ExtentReports;
using RestfulBooker.Tests.Utils;

namespace RestfulBooker.Tests.Api;

public class BaseApiTest
{
    private ExtentReports _extent;
    protected ExtentTest _test;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _extent = ExtentReportManager.Instance;
    }

    [SetUp]
    public void Setup()
    {
        _test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
    }

    [TearDown]
    public void TearDown()
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        var stacktrace = TestContext.CurrentContext.Result.StackTrace;
        var message = TestContext.CurrentContext.Result.Message;

        switch (status)
        {
            case NUnit.Framework.Interfaces.TestStatus.Passed:
                _test.Pass("Test passed");
                break;

            case NUnit.Framework.Interfaces.TestStatus.Failed:
                _test.Fail("Test failed: " + message);
                if (!string.IsNullOrEmpty(stacktrace))
                    _test.Fail(stacktrace);
                break;

            case NUnit.Framework.Interfaces.TestStatus.Skipped:
                _test.Skip("Test skipped");
                break;

            default:
                _test.Warning("Test status: " + status);
                break;
        }
    }
}
