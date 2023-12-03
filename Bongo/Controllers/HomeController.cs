using Bongo.Data;
using Bongo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEndpointWrapper wrapper;

        public HomeController(IEndpointWrapper _wrapper)
        {
            wrapper = _wrapper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (Current.IsAuthenticated)
            {
                var response = await wrapper.Timetable.GetUserTimetable();
                TempData["HasTimetable"] = response.IsSuccessStatusCode;
                if (Current.User.MergeKey == default)
                    return RedirectToAction("MergeKey", "Account");
            }
            else
                TempData["HasTimetable"] = false;

            return View();
        }

        [MyAuthorize]
        [HttpPost]
        public async Task<IActionResult> Notice()
        {
            var user = Current.User;
            user.Notified = true;
            await wrapper.User.Update(user);
            Response.Cookies.Append("Notified", user.Notified.ToString().ToLower(),
                            new CookieOptions { Expires = DateTime.Now.AddDays(90) }
                            );
            return RedirectToAction("Index");
        }

        [MyAuthorize]
        public IActionResult Profile()
        {
            return View(Current.User);
        }

        [MyAuthorize]
        [HttpPost]
        [ActionName("Profile")]
        public async Task<IActionResult> UpdateProfile(string id, string action, string newValue)
        {
            var user = Current.User;

            switch (id)
            {
                case "username": user.UserName = newValue; break;
                case "email": user.Email = newValue; break;
                case "mergeKey": user.MergeKey = newValue; break;
            }

            var result = await wrapper.User.Update(user);
            if (result.IsSuccessStatusCode)
                TempData["Message"] = "Your account detail(s) have successfully been updated";
            else
                TempData["Message"] = "Failed to update account detail(s)";

            return action == "mergeKey" ? RedirectToAction("Index") : RedirectToAction("Profile");
        }
    }
}
