using Bongo.MockAPI.Bongo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.MockAPI.Bongo.Areas.TutoringArea.Controllers
{
    [AllowAnonymous]
    public class HowItWorksController : Controller
    {
        private readonly IRepositoryWrapper _repo;

        public HowItWorksController(IRepositoryWrapper repo)
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