using EvidencePrihlasekWeb.Models;
using EvidencePrihlasekWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvidencePrihlasekWeb.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Dashboard()
        {
            if (User.Identity?.Name == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string username = User.Identity.Name;
            DashboardModel model = await _dashboardService.GetDashboardDataAsync(username);

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
