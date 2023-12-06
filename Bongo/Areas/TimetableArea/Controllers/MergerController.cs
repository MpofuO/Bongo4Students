using Bongo.Areas.TimetableArea.Infrastructure;
using Bongo.Areas.TimetableArea.Models.ViewModels;
using Bongo.Data;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Areas.TimetableArea.Controllers
{
    [Area("TimetableArea")]
    public class MergerController : Controller
    {
        private static IEndpointWrapper wrapper;
        private static MergerIndexViewModel viewModel;
        private static bool _isForFirstSemester;
        public MergerController(IEndpointWrapper _wrapper)
        {
            wrapper = _wrapper;
        }

        [MyAuthorize]
        [HttpGet]
        public IActionResult ChooseSemester()
        {
            return View();
        }

        [MyAuthorize]
        [HttpGet]
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
            var apiViewModel = await response.Content.ReadFromJsonAsync<APIMergerIndexViewModel>();
            viewModel = new MergerIndexViewModel
            {
                MergedUsers = apiViewModel.MergedUsers,
                Users = apiViewModel.Users,
                Sessions = SessionControlHelpers.Get2DArray(apiViewModel.Sessions),
                isFirstSemester = _isForFirstSemester
            };
            return RedirectToAction("Index");
        }

        [MyAuthorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View(viewModel);
        }

        [MyAuthorize]
        [HttpGet]
        public async Task<IActionResult> AddUserTimetable(string username)
        {
            var response = await wrapper.Merger.AddUserTimetable(username);
            if ((int)response.StatusCode == 202)
            {
                var apiViewModel = await response.Content.ReadFromJsonAsync<APIMergerIndexViewModel>();
                viewModel = new MergerIndexViewModel
                {
                    MergedUsers = apiViewModel.MergedUsers,
                    Users = apiViewModel.Users,
                    Sessions = SessionControlHelpers.Get2DArray(apiViewModel.Sessions),
                    isFirstSemester = _isForFirstSemester
                };
            }
            else
            {
                TempData["Message"] = (await response.Content.ReadAsStringAsync()).Replace("\"", "");
                if ((int)response.StatusCode == 204)
                    viewModel.MergedUsers.Add(username);
            }

            return RedirectToAction("Index");
        }

        [MyAuthorize]
        [HttpGet]
        public async Task<IActionResult> RemoveUserTimetable(string username)
        {
            var response = await wrapper.Merger.RemoveUserTimetable(username);
            if (response.IsSuccessStatusCode)
            {
                var apiViewModel = await response.Content.ReadFromJsonAsync<APIMergerIndexViewModel>();
                viewModel = new MergerIndexViewModel
                {
                    MergedUsers = apiViewModel.MergedUsers,
                    Users = apiViewModel.Users,
                    Sessions = SessionControlHelpers.Get2DArray(apiViewModel.Sessions),
                    isFirstSemester = _isForFirstSemester
                };
            }
            else
                TempData["Message"] = await response.Content.ReadAsStringAsync();

            return RedirectToAction("Index");
        }
    }
}
