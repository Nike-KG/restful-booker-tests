using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;

namespace RestfulBooker.Tests.Utils;

public class ExtentReportManager
{
    private static ExtentReports _extent;
    private static readonly object _lock = new object();

    public static ExtentReports Instance
    {
        get
        {
            if (_extent == null)
            {
                lock (_lock)
                {
                    if (_extent == null)
                    {
                        string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", ".."));
                        var reportPath = Path.Combine(projectRoot, "Reports", "ExtentReport.html");
                        var spark = new ExtentSparkReporter(reportPath);
                        spark.Config.DocumentTitle = "API Automation Report";
                        spark.Config.ReportName = "Booking API Tests";
                        spark.Config.Theme = Theme.Standard;

                        _extent = new ExtentReports();
                        _extent.AttachReporter(spark);
                    }
                }
            }

            return _extent;
        }
    }
}
