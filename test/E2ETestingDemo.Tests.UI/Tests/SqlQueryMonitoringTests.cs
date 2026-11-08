using E2ETestingDemo.TestingModule.Controllers;
using Lombiq.HelpfulLibraries.OrchardCore.Mvc;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.SqlQueryMonitoring;
using Lombiq.Tests.UI.SqlQueryMonitoring.Extensions;
using Lombiq.Tests.UI.SqlQueryMonitoring.Helpers;
using OpenQA.Selenium;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using static Lombiq.Tests.UI.SqlQueryMonitoring.Services.SqlQueryMonitoringConfiguration;

namespace E2ETestingDemo.Tests.UI.Tests;

public class SqlQueryMonitoringTests : UITestBase
{
    public SqlQueryMonitoringTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

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
    public Task SqlQueryMonitoringAutomaticAssertsOnPageChangeWithFilter()
    {
        var automaticallyAssertedSummaries = new List<SqlQueryMonitoringSummary>();

        return ExecuteTestAfterSetupAsync(
            async context =>
            {
                await context.GoToRelativeUrlAsync("/about");

                automaticallyAssertedSummaries.Count(summary => !summary.RequestPath.Contains("/about"))
                    .ShouldBe(0, "Only the page changes that match the configured rule should be in the summaries");
                automaticallyAssertedSummaries[0].RequestMethod.ShouldBe(HttpMethod.Get.Method);
                automaticallyAssertedSummaries[0].RequestPath.ShouldContain("/about");
                automaticallyAssertedSummaries[0].Executions.ShouldNotBeEmpty(
                    "Automatic page-change assertions should receive the monitored SQL summary too.");

                const string requestPathWithQuery = "/categories/travel?sqlMonitoringSample=before-navigation";
                await context.GoToRelativeUrlAsync(requestPathWithQuery);

                await context.AssertSqlQueryMonitoringForRequestAsync(
                    requestPathWithQuery,
                    HttpMethod.Get.Method,
                    summary =>
                    {
                        summary.RequestPath.ShouldStartWith(
                            requestPathWithQuery,
                            Case.Insensitive,
                            "Request-specific assertions should match the exact path and query string.");
                        summary.Executions.ShouldNotBeEmpty(
                            "The request-specific assertion should locate the SQL summary for this exact request.");
                        return Task.CompletedTask;
                    });
            },
            changeConfiguration: configuration =>
            {
                configuration.SqlQueryMonitoringConfiguration.EnableSqlQueryMonitoringCollection = true;

                configuration.SqlQueryMonitoringConfiguration.RunSqlQueryMonitoringAssertionOnAllPageChanges = true;
                configuration.SqlQueryMonitoringConfiguration.SqlQueryMonitoringAndAssertionOnPageChangeRule =
                    context => context.GetCurrentUri().AbsolutePath.EqualsOrdinalIgnoreCase("/about");

                configuration.SetUpSqlQueryMonitoringAssertionOnPageChange(summary =>
                {
                    automaticallyAssertedSummaries.Add(summary);
                    summary.Executions.ShouldNotBeEmpty(
                        "Automatically asserted summaries should also expose the monitored SQL executions.");
                    return Task.CompletedTask;
                });

                configuration.SqlQueryMonitoringConfiguration.ExecutionFilter =
                    SqlQueryMonitoringHelpers.BuildIgnoreCommandTextPatternFilter(
                        @"FROM\s+\[Document\].*RolesDocument");

                configuration.Events.BeforeNavigation += (_, targetUri) =>
                {
                    var thresholds = configuration.SqlQueryMonitoringConfiguration;

                    if (targetUri.AbsolutePath.ContainsOrdinalIgnoreCase("/categories"))
                    {
                        thresholds.DuplicateCommandThreshold = 20;
                        thresholds.DuplicateCommandWithParametersThreshold = 10;
                        thresholds.ResultSetRowCountThreshold = 100;
                    }
                    else
                    {
                        thresholds.DuplicateCommandThreshold = 30;
                        thresholds.DuplicateCommandWithParametersThreshold = 15;
                        thresholds.ResultSetRowCountThreshold = 200;
                    }

                    return Task.CompletedTask;
                };
            });
    }

    [Fact]
    public Task SqlQueryMonitoringAutomaticAssertsOnPageChangeWithRegexThresholdSettings() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                await context.GoToRelativeUrlAsync("/categories/travel");
                await context.AssertSqlQueryMonitoringAsync(summary =>
                {
                    summary.RequestPath.ShouldContain("/categories/travel");
                    summary.Executions.ShouldNotBeEmpty(
                        "The current-page assertion still works after changing thresholds with the regex helper.");
                    return Task.CompletedTask;
                });
            },
            changeConfiguration: configuration =>
            {
                configuration.SqlQueryMonitoringConfiguration.EnableSqlQueryMonitoringCollection = true;

                configuration.ConfigureSqlQueryMonitoringThresholdsForPages(
                    new SqlQueryMonitoringThresholds(
                        DuplicateCommandThreshold: 30,
                        DuplicateCommandWithParametersThreshold: 15,
                        ResultSetRowCountThreshold: 200),
                    (Pattern: @"^/categories/.*", Thresholds: new SqlQueryMonitoringThresholds(
                        DuplicateCommandThreshold: 20,
                        DuplicateCommandWithParametersThreshold: 10,
                        ResultSetRowCountThreshold: 100)),
                    (Pattern: @"^/about$", Thresholds: new SqlQueryMonitoringThresholds(
                        DuplicateCommandThreshold: 25,
                        DuplicateCommandWithParametersThreshold: 12,
                        ResultSetRowCountThreshold: 150)));
            });

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
