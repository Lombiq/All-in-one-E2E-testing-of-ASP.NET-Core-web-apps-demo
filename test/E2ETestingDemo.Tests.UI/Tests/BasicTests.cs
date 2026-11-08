using E2ETestingDemo.Tests.UI.Helpers;
using Lombiq.Tests.UI.Constants;
using Lombiq.Tests.UI.Extensions;
using OpenQA.Selenium;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace E2ETestingDemo.Tests.UI.Tests;

public class BasicTests : UITestBase
{
    public BasicTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public Task AnonymousHomePageShouldExist() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                context
                    .Get(By.ClassName("navbar-brand"))
                    .Text
                    .ShouldBe(SetupHelpers.SiteName);

                (await context.GetCurrentUserNameAsync()).ShouldBeNullOrEmpty();
            });

    [Fact]
    public Task LoginShouldWork() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                await context.GoToRelativeUrlAsync("/Login");

                await context.FillInWithRetriesAsync(By.Id("LoginForm_UserName"), DefaultUser.UserName);
                await context.FillInWithRetriesAsync(By.Id("LoginForm_Password"), DefaultUser.Password);

                await context.ClickReliablyOnSubmitAsync();

                (await context.GetCurrentUserNameAsync()).ShouldBe(DefaultUser.UserName);
            });

    [Fact]
    public Task TogglingFeaturesShouldWork() =>
        ExecuteTestAfterSetupAsync(
            context => context.ExecuteAndAssertTestFeatureToggleAsync(),
            configuration =>
                configuration.WithIgnoreExpectedNotFoundResponseFilter(ShortcutsUITestContextExtensions.FeatureToggleTestBenchUrl));

    [Fact]
    public Task ShortcutsShouldWork() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                await context.SignInDirectlyAsync();

                (await context.GetCurrentUserNameAsync()).ShouldBe(DefaultUser.UserName);

                await context.ExecuteRecipeDirectlyAsync("E2ETestingDemo.Sample");

                var info = await context.GetApplicationInfoAsync();
                _testOutputHelper.WriteLineTimestampedAndDebug("App root: " + info.AppRoot);

                await context.EnableFeatureDirectlyAsync("OrchardCore.HealthChecks");
            });
}
