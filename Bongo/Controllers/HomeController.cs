using Bongo.Data;
using Bongo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<BongoUser> _userManager;
        private readonly IEndpointWrapper endpoint;

        public HomeController(IEndpointWrapper _endpoint, UserManager<BongoUser> userManager)
        {
            endpoint = _endpoint;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var cookie = Request.Cookies["userTokenExpDate"];
                var tokenExpDate = cookie is not null ? DateTime.Parse(cookie) : default;
                if (tokenExpDate < DateTime.UtcNow)
                    RedirectToAction("Logout", "Account");

                var response = await endpoint.Timetable.GetUserTimetable();
                TempData["HasTimetable"] = response.IsSuccessStatusCode;
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user.MergeKey == default)
                    return RedirectToAction("MergeKey", "Account");
            }
            else
                TempData["HasTimetable"] = false;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Notice()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            user.Notified = true;
            await _userManager.UpdateAsync(user);
            Response.Cookies.Append("Notified", user.Notified.ToString().ToLower(),
                            new CookieOptions { Expires = DateTime.Now.AddDays(90) }
                            );
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return View(user);
        }
        [HttpPost]
        [ActionName("Profile")]
        public async Task<IActionResult> UpdateProfile(string id, string action, string newValue)
        {
            BongoUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            switch (id)
            {
                case "username": user.UserName = newValue; break;
                case "email": user.Email = newValue; break;
                case "mergeKey": user.MergeKey = newValue; break;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                TempData["Message"] = "Your account detail(s) have successfully been updated";
            else
                TempData["Message"] = "Failed to update account detail(s)";

            return action == "mergeKey" ? RedirectToAction("Index") : RedirectToAction("Profile");
        }
    }
}
