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
            if (Request.Cookies["Username"] is not null)
            {
                var response = await wrapper.Timetable.GetUserTimetable();
                TempData["HasTimetable"] = response.IsSuccessStatusCode;

                var userResponse = await wrapper.User.GetUserByName(Request.Cookies["Username"]);
                var user = await userResponse.Content.ReadFromJsonAsync<BongoUser>();
                if (user.MergeKey == default)
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
            var userResponse = await wrapper.User.GetUserByName(Request.Cookies["Username"]);
            var user = await userResponse.Content.ReadFromJsonAsync<BongoUser>();
            user.Notified = true;
            await wrapper.User.Update(user);
            Response.Cookies.Append("Notified", user.Notified.ToString().ToLower(),
                            new CookieOptions { Expires = DateTime.Now.AddDays(90) }
                            );
            return RedirectToAction("Index");
        }

        [MyAuthorize]
        public async Task<IActionResult> Profile()
        {
            var userResponse = await wrapper.User.GetUserByName(Request.Cookies["Username"]);
            var user = await userResponse.Content.ReadFromJsonAsync<BongoUser>();
            return View(user);
        }

        [MyAuthorize]
        [HttpPost]
        [ActionName("Profile")]
        public async Task<IActionResult> UpdateProfile(string id, string action, string newValue)
        {
            var userResponse = await wrapper.User.GetUserByName(Request.Cookies["Username"]);
            var user = await userResponse.Content.ReadFromJsonAsync<BongoUser>();

            switch (id)
            {
                case "username": user.UserName = newValue; break;
                case "email": user.Email = newValue; break;
                case "mergeKey": user.MergeKey = newValue; break;
            }

            var result = await wrapper.User.Update(user);
            if (result.IsSuccessStatusCode)
            {
                if (id == "username")
                {
                    wrapper.Clear();
                    Response.Cookies.Append("Username", user.UserName);
                }
                TempData["Message"] = "Your account detail(s) have successfully been updated";
            }
            else
                TempData["Message"] = "Failed to update account detail(s)";

            return action == "mergeKey" ? RedirectToAction("Index") : RedirectToAction("Profile");
        }
    }
}
