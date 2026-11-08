using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.SqlQueryMonitoring.Extensions;

namespace E2ETestingDemo.Tests.UI.Tests;

public class SqlQueryMonitoringTests(ITestOutputHelper testOutputHelper) : UITestBase(testOutputHelper)
{
    [Fact]
    public Task SqlQueryMonitoringExplicitAssertions() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                await context.AssertSqlQueryMonitoringAsync();

                await context.GoToHomePageAsync(onlyIfNotAlreadyThere: false);
                await context.AssertSqlQueryMonitoringForRequestAsync("/", HttpMethod.Get.Method);

                await context.GoToHomePageAsync(onlyIfNotAlreadyThere: false);
                await context.AssertSqlQueryMonitoringIncludingFollowUpRequestsAsync();
            },
            changeConfiguration: configuration =>
            {
                configuration.SqlQueryMonitoringConfiguration.EnableSqlQueryMonitoringCollection = true;
                // This makes the test fail.
                configuration.SqlQueryMonitoringConfiguration.DuplicateCommandThreshold = 1;
            });
}
