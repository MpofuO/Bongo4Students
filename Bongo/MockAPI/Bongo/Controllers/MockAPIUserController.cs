using Bongo.MockAPI.Bongo.Infrastructure;
using Bongo.MockAPI.Bongo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.MockAPI.Bongo.Controllers
{
    [Authorize]
    public class MockAPIUserController : Controller
    {
        private readonly UserManager<BongoUser> userManager;

        public MockAPIUserController(UserManager<BongoUser> _userManager)
        {
            userManager = _userManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] BongoUser user)
        {
            var result = await userManager.CreateAsync(user.EncryptUser());
            if (result.Succeeded)
                return Ok();
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] BongoUser user)
        {
            var result = await userManager.UpdateAsync(user.EncryptUser());
            if (result.Succeeded)
                return Ok();
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] BongoUser user)
        {
            var result = await userManager.DeleteAsync(user.EncryptUser());
            if (result.Succeeded)
                return Ok();
            return BadRequest();
        }
    }
}
