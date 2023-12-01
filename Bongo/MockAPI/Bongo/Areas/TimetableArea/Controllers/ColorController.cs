using Bongo.MockAPI.Bongo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.MockAPI.Bongo.Areas.TimetableArea.Controllers
{
    [Authorize]
    public class ColorController : Controller
    {
        private static IRepositoryWrapper repository;
        public ColorController(IRepositoryWrapper _repository)
        {
            repository = _repository;
        }

        [HttpGet]
        public IActionResult GetAllColors()
        {
            return Ok(repository.Color.FindAll());
        }

        [HttpGet("{moduleCode}")]
        public IActionResult GetModuleColorWithColorDetails(string moduleCode)
        {
            var moduleColor = repository.ModuleColor.GetModuleColorWithColorDetails(User.Identity.Name, moduleCode);
            return Ok(moduleColor);
        }

        [HttpGet]
        public IActionResult GetModulesWithColors()
        {
            return Ok(repository.ModuleColor.GetByCondition(mc => mc.Username == User.Identity.Name));
        }
    }
}
