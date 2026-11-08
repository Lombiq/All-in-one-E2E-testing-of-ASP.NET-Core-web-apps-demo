using E2ETestingDemo.Tests.UI.Helpers;
using Lombiq.Tests.UI;
using Lombiq.Tests.UI.Constants;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace E2ETestingDemo.Tests.UI;

public abstract class UITestBase : OrchardCoreUITestBase<Program>
{
    protected UITestBase(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

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

                configuration.OrchardCoreConfiguration.BeforeAppStart +=
                    (_, argumentsBuilder) =>
                    {
                        argumentsBuilder
                            .AddWithValue("OrchardCore:OrchardCore_Admin:AdminUrlPrefix", "Admin");

                        return Task.CompletedTask;
                    };

                configuration.OrchardCoreConfiguration.BeforeAppStart +=
                    (_, argumentsBuilder) =>
                    {
                        argumentsBuilder
                            .AddWithValue("Logging:LogLevel:Default", "Error")
                            .AddWithValue("Logging:LogLevel:System", "Error")
                            .AddWithValue("Logging:LogLevel:Microsoft", "Error")
                            .AddWithValue("Logging:LogLevel:OrchardCore.Environment.Shell.ShellHost", "Information");

                        return Task.CompletedTask;
                    };

                configuration.OrchardCoreConfiguration.AfterFakeLoggingConfiguration =
                    (_, fakeLogCollectorOptions) => fakeLogCollectorOptions.FilteredLevels.Add(LogLevel.Information);

                configuration.AssertAppLogsAsync = webApplicationInstance =>
                    webApplicationInstance.LogsShouldNotContainAsync(
                        logEntry => logEntry.Message != "My permitted message." && logEntry.Level != LogLevel.Information,
                        configuration.TestCancellationToken);

                configuration.SetupConfiguration.SetupOperationIdentifierCalculator = setupOperation =>
                    setupOperation == SetupHelpers.RunSetupAsync
                        ? "Sample Setup"
                        : OrchardCoreSetupConfiguration.DefaultSetupOperationIdentifierCalculator(setupOperation);

                if (changeConfigurationAsync != null) await changeConfigurationAsync(configuration);
            });
}
