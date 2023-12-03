using Bongo.Areas.TimetableArea.Infrastructure;
using Bongo.Areas.TimetableArea.Models;
using Bongo.Areas.TimetableArea.Models.ViewModels;
using Bongo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace Bongo.Areas.TimetableArea.Controllers
{
    [Area("TimetableArea")]
    public class SessionController : Controller
    {
        private IEndpointWrapper wrapper;

        public SessionController(IEndpointWrapper _wrapper)
        {
            wrapper = _wrapper;
        }

        [MyAuthorize]
        public async Task<IActionResult> Manage()
        {
            bool isForFirstSemester = Request.Cookies["isForFirstSemester"] == "true";
            TempData["isForFirstSemester"] = isForFirstSemester;

            var response = await wrapper.Session.GetClashes();
            if (response.IsSuccessStatusCode)
            {
                var clashes = await response.Content.ReadFromJsonAsync<List<List<Session>>>();
                if (clashes.Count > 0)
                    return View("Clashes", clashes);
            }


            response = await wrapper.Session.GetGroups();
            if (response.IsSuccessStatusCode)
            {
                var groups = await response.Content.ReadFromJsonAsync<List<Lecture>>();
                if (groups.Count > 0)
                    return View("Groups", groups);
            }

            return RedirectToAction("Display", "Timetable");
        }

        [MyAuthorize]
        [HttpPost]
        public async Task<IActionResult> Clashes(string[] Sessions)
        {
            var response = await wrapper.Session.HandleClashes(Sessions);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Message"] = await response.Content.ReadAsStringAsync();
            }

            return RedirectToAction("Manage");
        }

        [MyAuthorize]
        [HttpPost]
        public async Task<IActionResult> Groups(GroupsViewModel model)
        {
            var response = await wrapper.Session.HandleGroups(model);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Message"] = await response.Content.ReadAsStringAsync();
            }

            return RedirectToAction("Manage");
        }

        public async Task<IActionResult> ManageModules()
        {
            await PopulateColorDLL();

            return View("EditColors", await (await wrapper.Color.GetModulesWithColors()).
                Content.ReadFromJsonAsync<ModulesColorsViewModel>());
        }

        [MyAuthorize]
        [HttpPost]
        public async Task<IActionResult> DeleteModule(string ModuleCode)
        {
            var response = await wrapper.Session.DeleteModule(ModuleCode);
            TempData["Message"] = "Module removed successfuly.";
            return RedirectToAction("ManageModules");
        }

        [MyAuthorize]
        public async Task<IActionResult> EditClashes()
        {
            var response = await wrapper.Session.GetClashes();
            var clashes = await response.Content.ReadFromJsonAsync<List<List<Session>>>();

            return View("Clashes", clashes);

        }

        [MyAuthorize]
        public async Task<IActionResult> EditGroups(bool firstSemester)
        {
            var response = await wrapper.Session.GetGroups();
            var groups = await response.Content.ReadFromJsonAsync<List<Lecture>>();

            return View("Groups", groups);
        }

        [MyAuthorize]
        [HttpPost]
        public async Task<IActionResult> DeleteSession(string session)
        {
            var response = await wrapper.Session.DeleteSession(session);
            TempData["Message"] = "Session removed successfuly.";
            return RedirectToAction("Manage");
        }

        [MyAuthorize]
        [HttpGet]
        public async Task<IActionResult> AddSession(string day, string time, string moduleCode = "", string Venue = "")
        {
            PopulateEndTimeDLL(time, await getAvailablePeriodCount(time, day));
            return View(new AddSessionViewModel { ModuleCode = moduleCode, Venue = Venue, Day = day, startTime = time });
        }

        [MyAuthorize]
        [HttpPost]
        public async Task<IActionResult> AddSession(AddSessionViewModel model)
        {
            var response = await wrapper.Session.AddSession(model);
            switch ((int)response.StatusCode)
            {
                case 200:
                    TempData["Message"] = "Session successfully added";
                    return RedirectToAction("Manage");
                case 409:
                    return View("ConfirmGroup", model);
                default:
                    ModelState.AddModelError("", "Unable to add the session.");
                    PopulateEndTimeDLL(model.startTime, await getAvailablePeriodCount(model.startTime, model.Day));
                    return View(model);
            }
        }

        [MyAuthorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmGroup(AddSessionViewModel model)
        {
            var response = await wrapper.Session.ConfirmGroup(model);
            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Session successfully added";
            }
            PopulateEndTimeDLL(model.startTime, await getAvailablePeriodCount(model.startTime, model.Day));
            return View("AddSession", model);
        }

        [MyAuthorize]
        [HttpGet]
        public async Task<IActionResult> SessionDetails(string session)
        {
            var response = await wrapper.Session.GetSessionDetails(session);
            if (response.IsSuccessStatusCode)
                return View(await response.Content.ReadFromJsonAsync<SessionModuleColorViewModel>());

            TempData["Message"] = "Session not found";
            return RedirectToAction("Display", "Timetable");
        }

        [MyAuthorize]
        [HttpPost]
        public async Task<IActionResult> UpdateModuleColor(SessionModuleColorsUpdate model)
        {
            var response = await wrapper.Session.UpdateModuleColor(model);
            TempData["Message"] = await response.Content.ReadAsStringAsync();
            return RedirectToAction("Display", "Timetable");
        }

        [MyAuthorize]
        [HttpGet]
        public async Task<IActionResult> EditColors()
        {
            await PopulateColorDLL();

            var response = await wrapper.Color.GetAllColors();
            var colors = await response.Content.ReadFromJsonAsync<List<Color>>();

            response = await wrapper.Color.GetModulesWithColors();
            var moduleColors = await response.Content.ReadFromJsonAsync<List<ModuleColor>>();

            return View(new ModulesColorsViewModel()
            {
                ModuleColors = moduleColors.Where(m => Request.Cookies["isForFirstSemester"] == "true" ? 
                 (int.Parse(m.ModuleCode.Substring(6, 1)) == 0 || int.Parse(m.ModuleCode.Substring(6, 1)) % 2 == 1)
                                : int.Parse(m.ModuleCode.Substring(6, 1)) % 2 == 0),
                Colors = colors
            });
        }

        [MyAuthorize]
        [HttpGet]
        public async Task<IActionResult> RandomColorEdit(string activeAction)
        {
            await wrapper.Session.SetColorsRandomly();
            return RedirectToAction(activeAction == "EditColors" ? "EditColors" : "ManageModules");
        }
        private void PopulateEndTimeDLL(string startTime, int periodCount)
        {
            List<SelectListItem> endTimes = new List<SelectListItem>();
            int start = int.Parse(startTime.Substring(0, 2)) + 1;
            for (int i = 0; i <= periodCount; i++)
            {
                string value = start < 10 ? $"0{start}:00" : $"{start}:00";
                endTimes.Add(new SelectListItem { Value = value, Text = value });
                start++;
            }
            ViewBag.endTimes = endTimes;
        }
        private async Task<int> getAvailablePeriodCount(string startTime, string Day)
        {
            int day = 0;

            switch (Day)
            {
                case "Monday": day = 0; break;
                case "Tuesday": day = 1; break;
                case "Wednesday": day = 2; break;
                case "Thursday": day = 3; break;
                case "Friday": day = 4; break;
            }


            var response = await wrapper.Session.GetTimetableSessions(Request.Cookies["isForFirstSemester"] == "true");
            var data = SessionControlHelpers.Get2DArray(await response.Content.ReadFromJsonAsync<List<Session>>());

            int period = int.Parse(startTime.Substring(0, 2)) - 6;
            int count = 0;
            while (period < 16 && data[day, period] is null)
            {
                count++;
                period++;
            }

            return count;
        }
        private async Task PopulateColorDLL(object selectedColor = null)
        {
            var colorsResponse = await wrapper.Color.GetAllColors();
            var colors = await colorsResponse.Content.ReadFromJsonAsync<List<Color>>();
            ViewBag.Colors = new SelectList(colors.OrderBy(g => g.ColorName),
                "ColorId", "ColorName", selectedColor);
        }
        private void SetCookie(string key, string value)
        {
            CookieOptions cookieOptions = new CookieOptions { Expires = DateTime.Now.AddDays(90) };
            Response.Cookies.Append(key, value == null ? "" : value, cookieOptions);
        }
    }
}
