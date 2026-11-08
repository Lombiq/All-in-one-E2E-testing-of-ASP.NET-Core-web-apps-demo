using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.SecurityScanning;
using Shouldly;

namespace E2ETestingDemo.Tests.UI.Tests;

public class SecurityScanningTests(ITestOutputHelper testOutputHelper) : UITestBase(testOutputHelper)
{
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
                    .DisableSubresourceIntegrityAttributeMissingRuleForGoogleFonts()
                    .SignIn(),
                sarifLog => sarifLog.Runs[0].Results.Count.ShouldBe(0)),
            changeConfiguration: configuration => configuration.UseAssertAppLogsForSecurityScan());
}
