using Lombiq.Tests.UI.Extensions;
using OpenQA.Selenium;
using System.Threading.Tasks;
using Xunit;

namespace E2ETestingDemo.Tests.UI.Tests;

public class EmailTests : UITestBase
{
    public EmailTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

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
            },
            configuration => configuration.UseSmtpService = true);
}
