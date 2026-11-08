using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.MonkeyTesting;

namespace E2ETestingDemo.Tests.UI.Tests;

public class MonkeyTests(ITestOutputHelper testOutputHelper) : UITestBase(testOutputHelper)
{
    [Fact]
    public Task LoginShouldSurviveMonkeyTesting() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                await context.GoToRelativeUrlAsync("/Login");

                var monkeyTestingOptions = new MonkeyTestingOptions { PageTestTime = TimeSpan.FromSeconds(5) };
                await context.TestCurrentPageAsMonkeyAsync(monkeyTestingOptions);
            });
}
