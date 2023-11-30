using Bongo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Areas.TutoringArea.Controllers
{
    [AllowAnonymous]
    [Area("TutoringArea")]
    public class HowItWorksController : Controller
    {
        private readonly IEndpointWrapper _repo;

        public HowItWorksController(IEndpointWrapper repo)
        {
            _repo = repo; 
        }
        public IActionResult FindTutor()
        {
            return View();
        }
        public IActionResult BeTutor()
        {
            return View();
        }
    }
}