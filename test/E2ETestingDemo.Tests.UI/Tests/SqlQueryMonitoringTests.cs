using E2ETestingDemo.TestingModule.Controllers;
using Lombiq.HelpfulLibraries.OrchardCore.Mvc;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.SqlQueryMonitoring.Extensions;
using OpenQA.Selenium;
using Shouldly;

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
                configuration.SqlQueryMonitoringConfiguration.EnableSqlQueryMonitoringCollection = true);

    [Fact]
    public Task SqlQueryMonitoringPageLoadFollowingAsyncRequests() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                var asyncApiPath =
                    context.GetRelativeUrlOfAction<SqlQueryMonitoringScenarioController>(controller => controller.AsyncQuery());

                await context.GoToAsync<SqlQueryMonitoringScenarioController>(controller => controller.Index());
                var pagePath = context.GetCurrentUri().AbsolutePath;

                context.DoWithRetriesOrFail(() => context.GetText(By.Id("async-query-status")).EqualsOrdinalIgnoreCase("Completed"));

                await context.AssertSqlQueryMonitoringForRequestAsync(
                    pagePath,
                    HttpMethod.Get.Method,
                    summary =>
                    {
                        summary.RequestPath.ShouldBe(pagePath);
                        summary.Executions.ShouldNotBeEmpty(
                            "The initial page request should have its own SQL monitoring summary.");
                        return Task.CompletedTask;
                    });

                await context.AssertSqlQueryMonitoringForRequestAsync(
                    asyncApiPath,
                    HttpMethod.Get.Method,
                    summary =>
                    {
                        summary.RequestPath.ShouldBe(asyncApiPath);
                        summary.Executions.ShouldNotBeEmpty(
                            "The async API request should also have its own SQL monitoring summary.");
                        return Task.CompletedTask;
                    });

                await context.GoToAsync<SqlQueryMonitoringScenarioController>(controller => controller.Index());

                await context.AssertSqlQueryMonitoringIncludingFollowUpRequestsAsync(summary =>
                {
                    summary.RequestMethod.ShouldBe("MULTI");
                    summary.RequestPath.ShouldContain("combined");
                    summary.Executions.Count.ShouldBeGreaterThanOrEqualTo(
                        2,
                        "The combined assertion should contain executions from both the page request and the async API call.");
                    return Task.CompletedTask;
                });
            },
            configuration => configuration.SqlQueryMonitoringConfiguration.EnableSqlQueryMonitoringCollection = true);
}
