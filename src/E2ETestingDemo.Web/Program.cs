using Lombiq.HelpfulLibraries.AspNetCore.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton(builder.Configuration)
    .AddOrchardCms(orchardCoreBuilder => orchardCoreBuilder
        // This makes the test fail. Uncomment the next line to add the security headers.
        ////.ConfigureSecurityDefaultsWithStaticFiles(new ContentSecurityPolicyHeaderConfiguration { AllowInlineStyle = true })
        .EnableAutoSetupIfNotUITesting(builder.Configuration));

var app = builder.Build();

// This makes the test fail.
app.Use((context, next) => context.Request.Path == "/Admin" ? throw new InvalidOperationException("The dashboard is broken.") : next());

app.UseOrchardCore();

await app.RunAsync();

public partial class Program;
