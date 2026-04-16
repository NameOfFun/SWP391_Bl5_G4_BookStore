using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers;

[Authorize(Roles = "Shipper")]
public class ShipperController : Controller
{
    private readonly IShipperService _shipperService;
    private readonly UserManager<IdentityUser> _userManager;

    public ShipperController(IShipperService shipperService, UserManager<IdentityUser> userManager)
    {
        _shipperService = shipperService;
        _userManager    = userManager;
    }

    // GET /Shipper/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var vm = await _shipperService.GetDashboardAsync(user.Id);

        ViewData["Title"]              = "Dashboard Shipper";
        ViewData["BreadcrumbParent"]   = "Shipper";
        ViewData["BreadcrumbParentUrl"] = "#";

        return View(vm);
    }
}
