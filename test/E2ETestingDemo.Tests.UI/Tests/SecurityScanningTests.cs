using E2ETestingDemo.Tests.UI.Helpers;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Models;
using Lombiq.Tests.UI.SecurityScanning;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;
using YamlDotNet.RepresentationModel;

namespace E2ETestingDemo.Tests.UI.Tests;

public class SecurityScanningTests : UITestBase
{
    public SecurityScanningTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public Task BasicSecurityScanShouldPass() =>
        ExecuteTestAfterBrowserSetupWithoutBrowserAsync(
            context => context.RunAndAssertBaselineSecurityScanAsync(
                configuration => configuration.DisableSubresourceIntegrityAttributeMissingRuleForGoogleFonts()),
            changeConfiguration: configuration => configuration.UseAssertAppLogsForSecurityScan());

    [Fact]
    public Task SecurityScanWithCustomConfigurationShouldPass() =>
        ExecuteTestAfterBrowserSetupWithoutBrowserAsync(
            context => context.RunAndAssertBaselineSecurityScanAsync(
                configuration => configuration
                    .ExcludeUrlWithRegex(".*blog.*")
                    .DisablePassiveScanRule(10020, "The response does not include either Content-Security-Policy with 'frame-ancestors' directive.")
                    .DisableSubresourceIntegrityAttributeMissingRuleForGoogleFonts()
                    .DisableScanRuleForUrlWithRegex(".*/about", 10038, "Content Security Policy (CSP) Header Not Set")
                    .AddFalsePositiveRuleFilterForEvidence(
                        10202,
                        "Absence of Anti-CSRF Tokens",
                        "The search form doesn't alter the state of the application so anti-CSRF tokens are not needed.",
                        ".*my-search-form.*")
                    .SignIn(),
                sarifLog => sarifLog.Runs[0].Results.Count.ShouldBe(0)),
            changeConfiguration: configuration => configuration.UseAssertAppLogsForSecurityScan());

    [Fact]
    public Task SecurityScanWithCustomAutomationFrameworkPlanShouldPass() =>
        ExecuteTestAfterBrowserSetupWithoutBrowserAsync(
            context => context.RunAndAssertSecurityScanAsync(
                "Tests/CustomZapAutomationFrameworkPlan.yml",
                configuration => configuration
                    .DisableSubresourceIntegrityAttributeMissingRuleForGoogleFonts()
                    .ModifyZapPlan(plan =>
                    {
                        var spiderJob = plan.GetSpiderJob();
                        var spiderParameters = (YamlMappingNode)spiderJob["parameters"];
                        spiderParameters.Add("maxDepth", "8");
                    }),
                sarifLog => SecurityScanningConfiguration.AssertSecurityScanHasNoAlerts(context, sarifLog)),
            changeConfiguration: configuration => configuration.UseAssertAppLogsForSecurityScan());

    protected override Task ExecuteTestAfterSetupAsync(
        Func<UITestContext, Task> testAsync,
        Browser browser,
        Func<OrchardCoreUITestExecutorConfiguration, Task> changeConfigurationAsync) =>
        ExecuteTestAsync(
            testAsync,
            browser,
            async context =>
            {
                var homepageUri = await context.GoToSetupAndSetupOrchardCoreAsync(
                    new OrchardCoreSetupParameters(context)
                    {
                        SiteName = SetupHelpers.SiteName,
                        RecipeId = "Blog",
                        TablePrefix = "E2E",
                        SiteTimeZoneValue = "Europe/Budapest",
                    });

                context.Exists(By.ClassName("site-heading"));

                return homepageUri;
            },
            async configuration =>
            {
                configuration.HtmlValidationConfiguration.RunHtmlValidationAssertionOnAllPageChanges = false;

                await changeConfigurationAsync(configuration);
            });
}
