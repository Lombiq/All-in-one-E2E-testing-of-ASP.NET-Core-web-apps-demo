using Lombiq.HelpfulLibraries.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement.Records;
using YesSql;

namespace E2ETestingDemo.TestingModule.Controllers;

[AllowAnonymous]
[DevelopmentAndLocalhostOnly]
public sealed class SqlQueryMonitoringScenarioController(ISession session) : Controller
{
    public async Task<IActionResult> Index() =>
        View(model: await session.QueryIndex<ContentItemIndex>().CountAsync(HttpContext.RequestAborted));

    public async Task<IActionResult> AsyncQuery() =>
        Ok(await session.QueryIndex<ContentItemIndex>().CountAsync(HttpContext.RequestAborted));
}
