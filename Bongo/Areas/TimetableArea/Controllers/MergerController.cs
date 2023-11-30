using Bongo.Areas.TimetableArea.Infrastructure;
using Bongo.Areas.TimetableArea.Models;
using Bongo.Areas.TimetableArea.Models.ViewModels;
using Bongo.Data;
using Bongo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Areas.TimetableArea.Controllers
{
    [Authorize]
    [Area("TimetableArea")]
    public class MergerController : Controller
    {
        private static IEndpointWrapper wrapper;
        private static UserManager<BongoUser> userManager;
        private static MergerIndexViewModel viewModel;
        private static bool _isForFirstSemester;
        public MergerController(IEndpointWrapper _wrapper, UserManager<BongoUser> _userManager)
        {
            wrapper = _wrapper;
            userManager = _userManager;
        }
        public IActionResult ChooseSemester()
        {
            return View();
        }
        public async Task<IActionResult> SetSemester(bool isForFirstSemester)
        {
            _isForFirstSemester = isForFirstSemester;

            var response = await wrapper.Merger.InitialiseMerger(_isForFirstSemester);
            switch ((int)response.StatusCode)
            {
                case 202: goto valid;
                case 204:
                    TempData["Message"] = "Note that your timetable has no sessions for the selected semester.";
                    goto valid;
                case 400:
                    TempData["Message"] = await response.Content.ReadAsStringAsync();
                    return RedirectToAction("Manage", "Session");
                default:
                    TempData["Message"] = "Please create your timetable before attempting to merge with anyone.";
                    return RedirectToAction("Upload", "Timetable");
            }

        valid:
            viewModel = await response.Content.ReadFromJsonAsync<MergerIndexViewModel>();
            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            return View(viewModel);
        }
        public async Task<IActionResult> AddUserTimetable(string username)
        {
            var response = await wrapper.Merger.AddUserTimetable(username);
            if (response.IsSuccessStatusCode)
                viewModel = await response.Content.ReadFromJsonAsync<MergerIndexViewModel>();
            else
                TempData["Message"] = await response.Content.ReadAsStringAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> RemoveUserTimetable(string username)
        {
            var response = await wrapper.Merger.RemoveUserTimetable(username);
            if (response.IsSuccessStatusCode)
                viewModel = await response.Content.ReadFromJsonAsync<MergerIndexViewModel>();
            else
                TempData["Message"] = await response.Content.ReadAsStringAsync();

            return RedirectToAction("Index");
        }
    }
}
