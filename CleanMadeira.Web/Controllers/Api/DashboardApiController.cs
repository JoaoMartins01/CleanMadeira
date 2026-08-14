using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Web.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Treasury;
using System.Security.Claims;

namespace CleanMadeira.Web.Controllers;

[Authorize(AuthenticationSchemes = "ApiAuth")]
[ApiController]
[Route("api/[controller]")]
public class DashboardApiController : ControllerBase
{
    private readonly IDashboardService _service;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardApiController(IDashboardService service,
        UserManager<ApplicationUser> userManager)
    {
        _service = service;
        _userManager = userManager;
    }   

    [HttpGet]
    public async Task<ActionResult<DashboardVM>> GetDashboard()
    {

        Console.WriteLine("AUTH HEADER NA API: " + Request.Headers["Authorization"]);
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.GetUserAsync(User);

        var userIdS = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


        var vm = await _service.GetDashboardAsync(user.Id);

        return Ok(vm);
    }
}
