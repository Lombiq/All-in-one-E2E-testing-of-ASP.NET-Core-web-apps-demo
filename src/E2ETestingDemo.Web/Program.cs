using Lombiq.HelpfulLibraries.AspNetCore.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton(builder.Configuration)
    .AddOrchardCms(orchardCoreBuilder => orchardCoreBuilder
        .ConfigureSecurityDefaultsWithStaticFiles(new ContentSecurityPolicyHeaderConfiguration { AllowInlineStyle = true })
        .EnableAutoSetupIfNotUITesting(builder.Configuration));

var app = builder.Build();

app.UseOrchardCore();

await app.RunAsync();

public partial class Program;
