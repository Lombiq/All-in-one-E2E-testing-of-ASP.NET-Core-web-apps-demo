using E2ETestingDemo.Tests.UI.Helpers;
using Lombiq.Tests.UI.Constants;
using Lombiq.Tests.UI.Extensions;
using OpenQA.Selenium;
using Shouldly;

namespace E2ETestingDemo.Tests.UI.Tests;

public class BasicTests(ITestOutputHelper testOutputHelper) : UITestBase(testOutputHelper)
{
    [Fact]
    public Task AnonymousHomePageShouldExist() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                // This makes the test fail.
                await context.SignInDirectlyAsync();

                await context.GoToHomePageAsync(onlyIfNotAlreadyThere: false);

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

                // This makes the test fail.
                context.ExecuteScript("console.error('Something went wrong on the login page.');");

                await context.ClickReliablyOnSubmitAsync();

                (await context.GetCurrentUserNameAsync()).ShouldBe(DefaultUser.UserName);
            });

    [Fact]
    public Task ShortcutsShouldWork() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                await context.SignInDirectlyAsync();

                (await context.GetCurrentUserNameAsync()).ShouldBe(DefaultUser.UserName);

                await context.EnableFeatureDirectlyAsync("OrchardCore.HealthChecks");

                // This makes the test fail.
                await context.GoToRelativeUrlAsync("/Lombiq.Tests.UI.Shortcuts/Error/Index");
            });
}
