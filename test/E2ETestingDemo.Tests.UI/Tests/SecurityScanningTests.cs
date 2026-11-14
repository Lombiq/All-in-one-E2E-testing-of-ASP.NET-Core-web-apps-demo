using Lombiq.Tests.UI.SecurityScanning;

namespace E2ETestingDemo.Tests.UI.Tests;

public class SecurityScanningTests(ITestOutputHelper testOutputHelper) : UITestBase(testOutputHelper)
{
    [Fact]
    public Task BasicSecurityScanShouldPass() =>
        ExecuteTestAfterBrowserSetupWithoutBrowserAsync(
            context => context.RunAndAssertBaselineSecurityScanAsync(
                configuration => configuration.DisableSubresourceIntegrityAttributeMissingRuleForGoogleFonts()),
            changeConfiguration: configuration => configuration.UseAssertAppLogsForSecurityScan());
}
