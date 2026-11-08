using Lombiq.Tests.UI;
using Lombiq.Tests.UI.Extensions;
using OpenQA.Selenium;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace E2ETestingDemo.Tests.UI.Tests;

public class RemoteTests : RemoteUITestBase
{
    public RemoteTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public Task SeleniumWebFormShouldWork() =>
        ExecuteTestAsync(
            new Uri("https://www.selenium.dev/selenium/web/web-form.html"),
            context =>
            {
                context.Get(By.CssSelector("h1")).Text.ShouldBe("Web form");
                context.Exists(By.LinkText("Return to index"));
            },
            configuration => configuration.HtmlValidationConfiguration.RunHtmlValidationAssertionOnAllPageChanges = false);
}
