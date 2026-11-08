using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton(builder.Configuration)
    .AddOrchardCms(orchardCoreBuilder => orchardCoreBuilder
        .ConfigureSecurityDefaultsWithStaticFiles(new ContentSecurityPolicyHeaderConfiguration { AllowInlineStyle = true }));

var app = builder.Build();

app.UseOrchardCore();

await app.RunAsync();

public partial class Program;
