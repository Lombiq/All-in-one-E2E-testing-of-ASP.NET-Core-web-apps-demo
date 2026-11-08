using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.MonkeyTesting;
using Lombiq.Tests.UI.MonkeyTesting.UrlFilters;

namespace E2ETestingDemo.Tests.UI.Tests;

public class MonkeyTests(ITestOutputHelper testOutputHelper) : UITestBase(testOutputHelper)
{
    [Fact]
    public Task AdminDashboardShouldSurviveMonkeyTesting() =>
        ExecuteTestAfterSetupAsync(
            context =>
            {
                var monkeyTestingOptions = new MonkeyTestingOptions { PageTestTime = TimeSpan.FromSeconds(5) };
                monkeyTestingOptions.UrlFilters.Add(new MatchesRegexMonkeyTestingUrlFilter("/Admin$"));

                return context.TestAdminAsMonkeyRecursivelyAsync(monkeyTestingOptions);
            });
}
