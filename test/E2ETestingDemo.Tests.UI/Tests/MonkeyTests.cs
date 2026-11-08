using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.MonkeyTesting;
using Lombiq.Tests.UI.MonkeyTesting.UrlFilters;
using System.Net;

namespace E2ETestingDemo.Tests.UI.Tests;

public class MonkeyTests(ITestOutputHelper testOutputHelper) : UITestBase(testOutputHelper)
{
    [Fact]
    public Task TestCurrentPageAsMonkeyRecursivelyShouldWorkWithAnonymousUser() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                await context.GoToHomePageAsync();
                await context.TestCurrentPageAsMonkeyRecursivelyAsync(CreateMonkeyTestingOptions());
            });

    [Fact]
    public Task TestAdminPagesAsMonkeyRecursivelyShouldWorkWithAdminUser() =>
        ExecuteTestAfterSetupAsync(
            context =>
            {
                var monkeyTestingOptions = CreateMonkeyTestingOptions();

                monkeyTestingOptions.UrlFilters.Add(new MatchesRegexMonkeyTestingUrlFilter("/Admin$"));

                return context.TestAdminAsMonkeyRecursivelyAsync(monkeyTestingOptions);
            },
            configuration => configuration.WithIgnoreExpectedStatusResponseFilter("/api/graphql", HttpStatusCode.BadRequest));

    private static MonkeyTestingOptions CreateMonkeyTestingOptions() =>
        new()
        {
            PageTestTime = TimeSpan.FromSeconds(5),
        };
}
