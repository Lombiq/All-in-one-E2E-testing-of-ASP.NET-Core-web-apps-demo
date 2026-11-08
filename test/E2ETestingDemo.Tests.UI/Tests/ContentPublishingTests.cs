using Lombiq.Tests.UI.Extensions;
using OpenQA.Selenium;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace E2ETestingDemo.Tests.UI.Tests;

public class ContentPublishingTests : UITestBase
{
    public ContentPublishingTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public Task PublishedArticleShouldBeVisibleToVisitors() =>
        ExecuteTestAfterSetupAsync(
            async context =>
            {
                const string title = "Hello .NET Conf";

                await context.SignInDirectlyAsync();

                await context.CreateNewContentItemAsync("Article");
                await context.FillContentItemTitleAsync(title);
                await context.ClickPublishAsync();
                context.ShouldBeSuccess();

                await context.SignOutDirectlyAsync();
                (await context.GetCurrentUserNameAsync()).ShouldBeNullOrEmpty();

                await context.GoToRelativeUrlAsync("/hello-net-conf");
                context.Get(By.TagName("h1")).Text.ShouldBe(title);
            });
}
