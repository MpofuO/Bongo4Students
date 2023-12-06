using Bongo.Data;
using Bongo.Models;
using Bongo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto;

namespace Bongo.Controllers
{
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
                    Response.Cookies.Append("Username", user.UserName,
                        new CookieOptions { Expires = DateTime.Now.AddDays(loginModel.RememberMe ? 3 : 1) });

                    if (user.SecurityQuestion != default)
                        return RedirectToAction("Index", "Home");
                    else
                        return RedirectToAction("SecurityQuestion", new { sendingAction = "LogIn" });
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong. Please try again and if the problem persists contact us.");
                    goto OnError;
                }

            }
            ModelState.AddModelError("", "Invalid email or password");
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
                switch ((int)result.StatusCode)
                {
                    case 200:
                        TempData[Message] = "Registered successfully";
                        return RedirectToAction("Index", "Home");
                    case 409:
                        ModelState.AddModelError("", "Email used by another user. Please use a unique email"); break;
                    case 406:
                        foreach (var error in await result.Content.ReadFromJsonAsync<IEnumerable<IdentityError>>())
                            ModelState.AddModelError("", error.Description);
                        break;
                    case 500:
                        ModelState.AddModelError("", "Something went wrong. Please try again."); break;
                    default:
                        break;
                }
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            if (ModelState.IsValid)
            {
                var result = await _wrapper.Authorization.VerifyEmail(email);
                if (result.IsSuccessStatusCode)
                    return View("AskSecurityQuestion", new AnswerSecurityQuestionViewModel { Email = email });

                ModelState.AddModelError("", $"User with email {email} does not exist. Please enter a valid email.");
            }
            return View(new ForgotPassword { Email = email });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(AnswerSecurityQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _wrapper.Authorization.ForgotPassword(model);
                switch ((int)result.StatusCode)
                {
                    case 200:
                        string[] arr = await result.Content.ReadFromJsonAsync<string[]>();
                        return RedirectToAction("ResetPassword", new { userId = arr[1], token = arr[0] });
                    case 406:
                        ModelState.AddModelError("", "Incorrect answer. Please answer the question correctly to continue.");
                        break;
                    default:
                        ModelState.AddModelError("", $"Something went wrong with email {model.Email}. Please try again, if the problem persists contact us.");
                        break;
                }
            }
            return View("AskSecurityQuestion", model);
        }

        [MyAuthorize]
        public async Task<IActionResult> ChangePassword()
        {
            var userResponse = await _wrapper.User.GetUserByName(Request.Cookies["Username"]);
            var user = await userResponse.Content.ReadFromJsonAsync<BongoUser>();

            var result = await _wrapper.Authorization.ForgotPassword(new AnswerSecurityQuestionViewModel
            {
                SecurityAnswer = user.SecurityAnswer,
                SecurityQuestion = user.SecurityQuestion,
                Email = user.Email
            });
            switch ((int)result.StatusCode)
            {
                case 200:
                    string[] arr = await result.Content.ReadFromJsonAsync<string[]>();
                    return RedirectToAction("ResetPassword", new { userId = arr[1], token = arr[0] });
                case 400:
                    TempData["Message"] = $"Something went wrong.";
                    break;
            }
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
                switch ((int)result.StatusCode)
                {
                    case 200:
                        TempData["Message"] = "Successfully changed password";
                        return RedirectToAction("Index", "Home");
                    case 404:
                        ModelState.AddModelError("", "User not found"); break;
                    case 406:
                        foreach (var error in await result.Content.ReadFromJsonAsync<IEnumerable<IdentityError>>())
                            ModelState.AddModelError("", error.Description); break;
                    default: break;
                }
            }
            return View(model);
        }

        [MyAuthorize]
        public IActionResult SecurityQuestion(string sendingAction)
        {
            return View(new SecurityQuestionViewModel { SendingAction = sendingAction });
        }
        [HttpPost]
        [ActionName("SecurityQuestion")]
        public async Task<IActionResult> UpdateSecurityQuestion(SecurityQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _wrapper.Authorization.UpdateSecurityQuestion(model);
                switch ((int)result.StatusCode)
                {
                    case 200:
                        TempData["Message"] = "Successfully updated security question and/or answer."; break;
                    case 404:
                        ModelState.AddModelError("", "User not found");
                        goto onModelError;
                    default:
                        foreach (var error in await result.Content.ReadFromJsonAsync<IEnumerable<IdentityError>>())
                            ModelState.AddModelError("", error.Description);
                        goto onModelError;
                }

                if (model.SendingAction == "Profile")
                {
                    return RedirectToAction("Profile", "Home");
                }
                return RedirectToAction("Index", "Home");
            }
        onModelError:
            return View(model);
        }

        [MyAuthorize]
        public async Task<IActionResult> Logout()
        {
            //Logout from the API
            await _wrapper.Authorization.Logout();
            Response.Cookies.Delete("Username");

            return RedirectToAction("Index", "Home");
        }

        [MyAuthorize]
        public IActionResult MergeKey()
        {
            return View();
        }

    }
}
