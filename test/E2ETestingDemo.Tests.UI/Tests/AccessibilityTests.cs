using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;

namespace E2ETestingDemo.Tests.UI.Tests;

public class AccessibilityTests(ITestOutputHelper testOutputHelper) : UITestBase(testOutputHelper)
{
    [Fact]
    public Task FrontendPagesShouldBeAccessible() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                await context.GoToRelativeUrlAsync("/categories/travel");

                // This makes the test fail.
                context.ExecuteScript("document.documentElement.removeAttribute('lang');");

                context.AssertAccessibility();
            },
            configuration =>
            {
                configuration.AccessibilityCheckingConfiguration.RunAccessibilityCheckingAssertionOnAllPageChanges = true;

                configuration.AccessibilityCheckingConfiguration.AxeBuilderConfigurator += axeBuilder =>
                    AccessibilityCheckingConfiguration.ConfigureWcag22aa(axeBuilder)
                        .DisableRules("color-contrast", "link-name");
            });
}
