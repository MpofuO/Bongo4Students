using Bongo.MockAPI.Bongo.Areas.TimetableArea.Controllers;
using Bongo.MockAPI.Bongo.Areas.TimetableArea.Models.ViewModels;
using Bongo.MockAPI.Bongo.Controllers;
using Bongo.MockAPI.Bongo.Data;
using Bongo.MockAPI.Bongo.Models;
using Bongo.MockAPI.Bongo.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Bongo.MockAPI
{
    public static class Global
    {
        public static IApplicationBuilder app;
    }

    public static class UserIdentity
    {
        public static string Name { get; set; }
    }

    public class MockHttpClient
    {
        private MockAPIAccountController account;
        private MockAPIUserController user;
        private MockAPIColorController color;
        private MockAPIMergerController merger;
        private MockAPISessionController session;
        private MockAPITimetableController timetable;

        private Task<IActionResult> result;

        public MockHttpClient()
        {
            AppDbContext context = Global.app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
            UserManager<BongoUser> userManager = Global.app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<UserManager<BongoUser>>();
            SignInManager<BongoUser> signInManager = Global.app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<SignInManager<BongoUser>>();
            IConfiguration config = Global.app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();

            IRepositoryWrapper repo = new RepositoryWrapper(context);

            account = new MockAPIAccountController(userManager, signInManager, config);
            user = new MockAPIUserController(userManager);
            color = new MockAPIColorController(repo);
            merger = new MockAPIMergerController(repo, userManager);
            session = new MockAPISessionController(repo);
            timetable = new MockAPITimetableController(repo, userManager);
        }

        public async Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            return await HandleRequest(uri, default);
        }
        public async Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
        {
            return await HandleRequest(uri, content);
        }

        private async Task<HttpResponseMessage> HandleRequest(Uri uri, HttpContent content)
        {
            string url = uri.ToString();
            if (url.Contains("/authorization/"))
                HandleOnAccount(url, content);
            else if (url.Contains("/User/"))
                HandleOnUser(url, content);
            else if (url.Contains("/Color/"))
                HandleOnColor(url, content);
            else if (url.Contains("/Merger/"))
                HandleOnMerger(url, content);
            else if (url.Contains("/Session/"))
                HandleOnSession(url, content);
            else
                HandleOnTimetable(url, content);

            var objectResult = (ObjectResult)await result;

            HttpResponseMessage response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)objectResult.StatusCode,
                Content = JsonContent.Create(objectResult.Value)
            };
            return response;
        }

        #region Private methods for handling different requests
        private async void HandleOnAccount(string url, HttpContent content)
        {
            if (url.Contains("ChangePassword"))
                result = account.ChangePassword(url.Substring(url.LastIndexOf('/')));
            else if (url.Contains("ForgotPassword"))
            {
                result = account.ForgotPassword(await content.ReadFromJsonAsync<AnswerSecurityQuestionViewModel>());
            }
            else if (url.Contains("login"))
            {
                result = account.Login(await content.ReadFromJsonAsync<LoginViewModel>());
            }
            else if (url.Contains("logout"))
            {
                result = account.Logout();
            }
            else if (url.Contains("register"))
            {
                result = account.Register(await content.ReadFromJsonAsync<RegisterModel>());
            }
            else if (url.Contains("ResetPassword"))
            {
                result = account.ResetPassword(await content.ReadFromJsonAsync<ResetPassword>());
            }
            else if (url.Contains("UpdateSecurityQuestion"))
            {
                result = account.UpdateSecurityQuestion(await content.ReadFromJsonAsync<SecurityQuestionViewModel>());
            }
            else if (url.Contains("VerifyEmail"))
                result = account.VerifyEmail(url.Substring(url.LastIndexOf('/')));
        }
        private async void HandleOnUser(string url, HttpContent content)
        {
            if (url.Contains("Create"))
            {
                result = user.Create(await content.ReadFromJsonAsync<BongoUser>());
            }
            else if (url.Contains("Delete"))
            {
                result = user.Delete(await content.ReadFromJsonAsync<BongoUser>());
            }
            else
            {
                result = user.Update(await content.ReadFromJsonAsync<BongoUser>());
            }
        }
        private void HandleOnColor(string url, HttpContent content)
        {
            if (url.Contains("GetModuleColorWithColorDetails"))
                result = color.GetModuleColorWithColorDetails(url.Substring(url.LastIndexOf('/')));
            else if (url.Contains("GetModulesWithColors"))
                result = color.GetModulesWithColors();
            else
                result = color.GetAllColors();
        }
        private void HandleOnMerger(string url, HttpContent content)
        {
            if (url.Contains("InitialiseMerger"))
                result = merger.InitialiseMerger(url.Substring(url.LastIndexOf('/')).ToLower() == "true");
            else if (url.Contains("RemoveUserTimetable"))
                result = merger.RemoveUserTimetable(url.Substring(url.LastIndexOf('/')));
            else
                result = merger.AddUserTimetable(url.Substring(url.LastIndexOf('/')));
        }
        private async void HandleOnSession(string url, HttpContent content)
        {
            if (url.Contains("AddSession"))
                result = session.AddSession(await content.ReadFromJsonAsync<AddSessionViewModel>());
            else if (url.Contains("ConfirmGroup"))
                result = session.ConfirmGroup(await content.ReadFromJsonAsync<AddSessionViewModel>());
            else if (url.Contains("DeleteModule"))
                result = session.DeleteModule(url.Substring(url.LastIndexOf('/')));
            else if (url.Contains("DeleteSession"))
                result = session.DeleteSession(url.Substring(url.LastIndexOf('/')));
            else if (url.Contains("GetClashes"))
                result = session.GetClashes();
            else if (url.Contains("GetGroups"))
                result = session.GetGroups();
            else if (url.Contains("GetSessionDetails"))
                result = session.GetSessionDetails(url.Substring(url.LastIndexOf('/')));
            else if (url.Contains("GetTimetableSessions"))
                result = session.GetTimetableSessions(url.Substring(url.LastIndexOf('/')).ToLower() == "true");
            else if (url.Contains("HandleClashes"))
                result = session.HandleClashes(await content.ReadFromJsonAsync<string[]>());
            else if (url.Contains("HandleGroups"))
                result = session.HandleGroups(await content.ReadFromJsonAsync<GroupsViewModel>());
            else if (url.Contains("SetColorsRandomly"))
                result = session.SetColorsRandomly();
            else
                result = session.UpdateModuleColor(await content.ReadFromJsonAsync<SessionModuleColorsUpdate>());
        }
        private async void HandleOnTimetable(string url, HttpContent content)
        {
            if (url.Contains("GetUserTimetable"))
                result = timetable.GetUserTimetable();
            else if (url.Contains("ClearUserTable"))
                result = timetable.ClearUserTable(int.Parse(url.Substring(url.LastIndexOf('/'))));
            else
                result = timetable.UpdateOrCreate(await content.ReadAsStringAsync());
        }

        #endregion
    }

}
