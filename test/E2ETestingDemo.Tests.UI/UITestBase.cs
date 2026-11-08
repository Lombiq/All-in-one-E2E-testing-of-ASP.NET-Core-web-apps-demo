using E2ETestingDemo.Tests.UI.Helpers;
using Lombiq.Tests.UI;
using Lombiq.Tests.UI.Constants;
using Lombiq.Tests.UI.Services;

namespace E2ETestingDemo.Tests.UI;

public abstract class UITestBase(ITestOutputHelper testOutputHelper) : OrchardCoreUITestBase<Program>(testOutputHelper)
{
    protected override Task ExecuteTestAfterSetupAsync(
        Func<UITestContext, Task> testAsync,
        Browser browser,
        Func<OrchardCoreUITestExecutorConfiguration, Task> changeConfigurationAsync) =>
        ExecuteTestAsync(testAsync, browser, SetupHelpers.RunSetupAsync, changeConfigurationAsync);

    protected override Task ExecuteTestAsync(
        Func<UITestContext, Task> testAsync,
        Browser browser,
        Func<UITestContext, Task<Uri>> setupOperation,
        Func<OrchardCoreUITestExecutorConfiguration, Task> changeConfigurationAsync) =>
        base.ExecuteTestAsync(
            testAsync,
            browser,
            setupOperation,
            async configuration =>
            {
                configuration.BrowserConfiguration.DefaultBrowserSize = CommonDisplayResolutions.HdPlus;

                configuration.BrowserConfiguration.Headless =
                    TestConfigurationManager.GetBoolConfiguration("BrowserConfiguration:Headless", defaultValue: false);

                // Slowing down visible browser runs so the audience can follow the steps.
                if (!configuration.BrowserConfiguration.Headless)
                {
                    var stepDelay = TimeSpan.FromSeconds(1);
                    configuration.Events.AfterNavigation += (_, _) => Task.Delay(stepDelay);
                    configuration.Events.BeforeClick += (_, _) => Task.Delay(stepDelay);
                }

                if (changeConfigurationAsync != null) await changeConfigurationAsync(configuration);
            });
}
