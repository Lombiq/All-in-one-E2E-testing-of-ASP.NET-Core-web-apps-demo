using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Models;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;

namespace E2ETestingDemo.Tests.UI.Helpers;

public static class SetupHelpers
{
    public const string RecipeId = "E2ETestingDemo.Tests";
    public const string SiteName = "E2E Testing Demo";

    public static async Task<Uri> RunSetupAsync(UITestContext context)
    {
        var homepageUri = await context.GoToSetupAndSetupOrchardCoreAsync(
            new OrchardCoreSetupParameters(context)
            {
                SiteName = SiteName,
                RecipeId = RecipeId,
            });

        context.Exists(By.Id("mainNav"));

        return homepageUri;
    }
}
