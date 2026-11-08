using E2ETestingDemo.Tests.UI.Constants;
using Lombiq.Tests.UI.BasicOrchardFeaturesTesting;

namespace E2ETestingDemo.Tests.UI.Tests;

public class BasicOrchardFeaturesTests(ITestOutputHelper testOutputHelper) : UITestBase(testOutputHelper)
{
    [Fact]
    public Task BasicOrchardFeaturesShouldWork() =>
        ExecuteTestAsync(context => context.TestBasicOrchardFeaturesAsync(RecipeIds.BasicOrchardFeaturesTests));
}
