using Bongo.MockAPI.Bongo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.MockAPI.Bongo.Areas.TimetableArea.Controllers
{
    [Authorize]
    public class MockAPIColorController : ControllerBase
    {
        private static IRepositoryWrapper repository;
        private static string Username;
        public MockAPIColorController(IRepositoryWrapper _repository, string username)
        {
            repository = _repository;
            Username = username;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllColors()
        {
            return Ok(repository.Color.FindAll());
        }

        [HttpGet("{moduleCode}")]
        public async Task<IActionResult> GetModuleColorWithColorDetails(string moduleCode)
        {
            var moduleColor = repository.ModuleColor.GetModuleColorWithColorDetails(Username, moduleCode);
            return Ok(moduleColor);
        }

        [HttpGet]
        public async Task<IActionResult> GetModulesWithColors()
        {
            return Ok(repository.ModuleColor.GetByCondition(mc => mc.Username == Username));
        }
    }
}
