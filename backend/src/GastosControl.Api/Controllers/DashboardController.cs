using GastosControl.Api.Extensions;
using GastosControl.Application.Dashboard;
using GastosControl.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosControl.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get(
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] TransactionType? type,
        [FromQuery] Guid? categoryId,
        CancellationToken cancellationToken)
    {
        var request = new DashboardRequest(year, month, type, categoryId);
        return Ok(await _dashboardService.GetAsync(User.GetUserId(), request, cancellationToken));
    }
}
