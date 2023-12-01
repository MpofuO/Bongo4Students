//using Bongo.Areas.TimetableArea.Models;
//using Bongo.Areas.TimetableArea.Models.User;
//using Bongo.Data;
//using Bongo.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;

//namespace Bongo.Areas.TimetableArea.Controllers
//{
//    [MyAuthorize]
//    [Area("TimetableArea")]
//    public class UserReviewController : Controller
//    {
//        private readonly IEndpointWrapper wrapper;
//        private readonly UserManager<BongoUser> _userManager;

//        public UserReviewController(IEndpointWrapper repo, UserManager<BongoUser> userManager)
//        {
//            wrapper = repo;
//            _userManager = userManager;
//        }

//        [HttpGet]
//        public IActionResult AddReview()
//        {
//            UserReview model = wrapper.UserReview.FindAll().FirstOrDefault(r => r.Username == User.Identity.Name);
//            return View(model ?? new UserReview { Username = User.Identity.Name});
//        }
//        [HttpPost]
//        public async Task<IActionResult> AddReview(UserReview model)
//        {
//            try
//            {
//                var user = await _userManager.FindByNameAsync(model.Username);
//                model.ReviewDate = DateTime.Now.ToLocalTime();
//                model.Email = user.Email;
//                if (model.ReviewId != 0)
//                {
//                    wrapper.UserReview.Update(model);
//                    TempData["Message"] = "Review updated successfully. Thank you";
//                }
//                else
//                {
//                    wrapper.UserReview.Create(model);
//                    TempData["Message"] = "Review submited successfully. Thank you";
//                }
//                wrapper.SaveChanges();
//            }
//            catch (Exception)
//            {
//                ModelState.AddModelError("", "Someting went wrong while saving. Please try again later.😔");
//            }
//            var newModel = wrapper.UserReview.FindAll().FirstOrDefault(r => r.Username.Equals(model.Username));   
//            return View(newModel ?? model);
//        }
//        [HttpGet]
//        [Authorize(Roles = "Admin")]
//        public ActionResult ListReviews()
//        {
//            return View(wrapper.UserReview.FindAll().OrderBy(u => u.Username));
//        }
//        [HttpGet]
//        [Authorize(Roles = "Admin")]
//        public ActionResult ListUsers()
//        {
//            return View(_userManager.Users.OrderBy(u => u.UserName));
//        }

//        [HttpPost]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Delete(string id)
//        {
//            var user = _userManager.Users.FirstOrDefault(u => u.Id == id);
//            if (user != null)
//            {
//                try
//                {
//                    await _userManager.DeleteAsync(user);
//                    IEnumerable<ModuleColor> moduleColors = wrapper.Color.GetByCondition(mc => mc.Username == user.UserName);
//                    foreach (var module in moduleColors)
//                        wrapper.Color.Delete(module);
//                    var table = wrapper.Timetable.FindAll().FirstOrDefault(t => t.Username == user.UserName);
//                    if (table != null)
//                        wrapper.Timetable.Delete(table);

//                    wrapper.SaveChanges();
//                    TempData["Message"] = $"{user.UserName} has been deleted successfully!";
//                }
//                catch (Exception e)
//                {
//                    TempData["Message"] = $"Something went wrong!\nMessage: {e.ToString()}";
//                }
//            }
//            return RedirectToAction("ListUsers");
//        }
//        [HttpPost]
//        [Authorize(Roles = "Admin")]
//        public IActionResult DeleteReview(string username)
//        {
//            var userReview = wrapper.UserReview.FindAll().FirstOrDefault(ur => ur.Username == username);
//            if (userReview != null)
//            {
//                try
//                {
//                    wrapper.UserReview.Delete(userReview);

//                    wrapper.SaveChanges();
//                    TempData["Message"] = $"{username}'s review has been deleted successfully!";
//                }
//                catch (Exception e)
//                {
//                    TempData["Message"] = $"Something went wrong!\nMessage: {e.ToString()}";
//                }
//            }
//            return RedirectToAction("ListReviews");
//        }
//    }
//}
