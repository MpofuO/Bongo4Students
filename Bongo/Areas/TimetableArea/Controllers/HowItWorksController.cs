using Bongo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Areas.TimetableArea.Controllers
{
    [AllowAnonymous]
    [Area("TimetableArea")]
    public class HowItWorksController : Controller
    {
        private readonly IEndpointWrapper _repo;

        public HowItWorksController(IEndpointWrapper repo)
        {
            _repo = repo; 
        }
        public IActionResult CreateTimetable()
        {
            return View();
        }
        public IActionResult MergeTimetables()
        {
            return View();
        }
    }
}