using Atata;
using Lombiq.Tests.UI.Extensions;
using Lombiq.Tests.UI.Models;
using Lombiq.Tests.UI.Services;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                TablePrefix = "E2E",
                SiteTimeZoneValue = "Europe/Budapest",
            });

        AssertSetupSuccessful(context);

        return homepageUri;
    }

    private static void AssertSetupSuccessful(UITestContext context)
    {
        try
        {
            context.Exists(By.Id("mainNav"));
        }
        catch (NoSuchElementException exception)
        {
            var validationErrors = context.GetAll(By.ClassName("field-validation-error"));

            if (validationErrors.Count == 0) throw;

            var errors = "\n- " + validationErrors.Select(element => element.Text.Trim()).Join("\n- ");
            throw new AssertionException($"Setup has failed with the following validation errors:{errors}", exception);
        }
    }
}
