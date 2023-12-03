using Bongo.MockAPI.Bongo.Infrastructure;
using Bongo.MockAPI.Bongo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.MockAPI.Bongo.Controllers
{
    [Authorize]
    public class MockAPIUserController : ControllerBase
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
                return Ok("User created successfully.");
            return BadRequest("User not created successfully.");
        }

        [MyAuthorize]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] BongoUser user)
        {
            var result = await userManager.UpdateAsync(user.EncryptUser());
            if (result.Succeeded)
                return Ok("User updated successfully.");
            return BadRequest("User not updated successfully.");
        }

        [MyAuthorize]
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] BongoUser user)
        {
            var result = await userManager.DeleteAsync(user.EncryptUser());
            if (result.Succeeded)
                return Ok("User deleted successfully.");
            return BadRequest("User not deleted successfully.");
        }
    }
}
