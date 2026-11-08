using Lombiq.Tests.UI;
using Lombiq.Tests.UI.Extensions;
using OpenQA.Selenium;
using Shouldly;

namespace E2ETestingDemo.Tests.UI.Tests;

public class RemoteTests(ITestOutputHelper testOutputHelper) : RemoteUITestBase(testOutputHelper)
{
    [Fact]
    public Task OrchardCoreWebsiteShouldWork() =>
        ExecuteTestAsync(
            new Uri("https://orchardcore.net/"),
            context =>
            {
                context.Get(By.CssSelector("h1")).Text.ShouldBe("The open-source .NET CMS and application framework.");
                context.Exists(By.LinkText("Try the live demo"));
            },
            configuration => configuration.HtmlValidationConfiguration.RunHtmlValidationAssertionOnAllPageChanges = false);
}
