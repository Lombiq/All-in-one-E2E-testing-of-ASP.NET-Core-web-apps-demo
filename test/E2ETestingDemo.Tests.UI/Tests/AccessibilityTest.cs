using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using System.Threading.Tasks;
using Xunit;

namespace E2ETestingDemo.Tests.UI.Tests;

public class AccessibilityTest : UITestBase
{
    public AccessibilityTest(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public Task FrontendPagesShouldBeAccessible() =>
        ExecuteTestAfterSetupAsync(
            context => context.GoToRelativeUrlAsync("/categories/travel"),
            configuration =>
            {
                configuration.AccessibilityCheckingConfiguration.RunAccessibilityCheckingAssertionOnAllPageChanges = true;

                configuration.AccessibilityCheckingConfiguration.AxeBuilderConfigurator += axeBuilder =>
                    AccessibilityCheckingConfiguration.ConfigureWcag22aa(axeBuilder)
                        .DisableRules("color-contrast", "link-name");
            });
}
