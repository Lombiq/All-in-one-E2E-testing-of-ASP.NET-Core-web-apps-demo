using Lombiq.Tests.UI.Extensions;
using OpenQA.Selenium;

namespace E2ETestingDemo.Tests.UI.Tests;

public class EmailTests(ITestOutputHelper testOutputHelper) : UITestBase(testOutputHelper)
{
    [Fact]
    public Task SendingTestEmailShouldWork() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                await context.SignInDirectlyAsync();

                await context.GoToEmailTestAsync();
                await context.FillEmailTestFormAsync("Test message");
                context.ShouldBeSuccess();

                await context.GoToSmtpWebUIAsync();

                await context.ClickReliablyOnSmtpInboxRowAndSwitchToFrame0WithRetriesAsync("Test message");

                context.Exists(By.XPath("//pre[contains(.,'Hi, this is a test.')]"));

                // This makes the test fail.
                context.Exists(By.XPath("//pre[contains(.,'Unsubscribe')]"));
            },
            configuration => configuration.UseSmtpService = true);
}
