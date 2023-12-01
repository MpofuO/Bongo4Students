using Bongo.Data;
using Bongo.Models;
using Bongo.Models.ViewModels;
using Bongo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Controllers
{
    [MyAuthorize]
    public class AccountController : Controller
    {
        private readonly IEndpointWrapper _wrapper;

        public AccountController(IEndpointWrapper wrapper)
        {
            _wrapper = wrapper;
        }
        [TempData]
        public string Message { get; set; }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return RedirectToAction("SignIn", new { returnUrl = returnUrl });
        }

        [AllowAnonymous]
        public IActionResult SignIn(string returnUrl)
        {
            return View(new LoginViewModel
            { ReturnUrl = returnUrl }
                        );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                //login to the API
                var response = await _wrapper.Authorization.Login(loginModel);
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<BongoUser>();
                    Response.Cookies.Append("Notified", user.Notified.ToString().ToLower(),
                        new CookieOptions { Expires = DateTime.Now.AddDays(90) });
                    Current.User = user;

                    if (Current.User.SecurityQuestion != default)
                        return RedirectToAction("Index", "Home");
                    else
                        return RedirectToAction("SecurityQuestion", new { username = Current.User.UserName, sendingAction = "LogIn" });
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong. Please try again and if the problem persists contact us.");
                    goto OnError;
                }

            }
            ModelState.AddModelError("", "Invalid username or password");
        OnError:
            return View("SignIn", loginModel);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _wrapper.Authorization.Register(registerModel);
            }
            return View(registerModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmEmail(string userId, string token)
        {
            return View(new ConfirmEmail { UserId = userId, Token = token });
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ConfirmEmail(ConfirmEmail model)
        {
            //Still needs to be implemented correctly
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyUsername(string username)
        {
            if (ModelState.IsValid)
            {
                var result = await _wrapper.Authorization.VerifyUsername(username);
            }
            return View("ForgotPassword", new ForgotPassword { Username = username });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string username)
        {
            return View(new ForgotPassword { Username = username });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(AnswerSecurityQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _wrapper.Authorization.ForgotPassword(model);
            }
            ModelState.AddModelError("", $"Something went wrong with email {model.Email}. Please try again, if the problem persists contact us.");
            return View(new { email = model.Email });
        }

        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(string userId)
        {
            var result = await _wrapper.Authorization.ChangePassword(userId);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string token)
        {
            return View(new ResetPassword { Token = token, UserId = userId });
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (ModelState.IsValid)
            {
                var result = await _wrapper.Authorization.ResetPassword(model);
            }
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult SecurityQuestion(string username, string sendingAction)
        {
            return View(new SecurityQuestionViewModel { UserName = username, SendingAction = sendingAction });
        }
        [HttpPost]
        [AllowAnonymous]
        [ActionName("SecurityQuestion")]
        public async Task<IActionResult> UpdateSecurityQuestion(SecurityQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _wrapper.Authorization.UpdateSecurityQuestion(model);


                bool fromRegister = model.SendingAction == "Register";
                if (fromRegister)
                {
                    return RedirectToAction("SignIn", "Account");
                }
                else if (model.SendingAction == "Profile")
                {
                    return RedirectToAction("Profile", "Home");
                }
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            //Logout from the API
            await _wrapper.Authorization.Logout();
            Current.User = null;

            return RedirectToAction("Index", "Home");
        }

        public IActionResult MergeKey()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
