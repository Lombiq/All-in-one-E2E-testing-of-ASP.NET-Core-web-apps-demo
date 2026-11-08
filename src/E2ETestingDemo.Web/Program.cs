using Lombiq.HelpfulLibraries.AspNetCore.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton(builder.Configuration)
    .AddOrchardCms(orchardCoreBuilder => orchardCoreBuilder
        // This makes security scanning fail. Uncomment the next line to add the security headers.
        ////.ConfigureSecurityDefaultsWithStaticFiles(new ContentSecurityPolicyHeaderConfiguration { AllowInlineStyle = true })
        .EnableAutoSetupIfNotUITesting(builder.Configuration));

var app = builder.Build();

app.UseOrchardCore();

await app.RunAsync();

public partial class Program;
