using Bongo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bongo.Data
{
    public interface IAuthorizationEndpoint:IBaseEndpoint
    {
        public Task<HttpResponseMessage> Login(LoginViewModel model);
        public Task<HttpResponseMessage> Logout();
        public Task<HttpResponseMessage> Register(RegisterModel registerModel);
        public Task<HttpResponseMessage> VerifyUsername(string username);
        public Task<HttpResponseMessage> ForgotPassword(AnswerSecurityQuestionViewModel model);
        public Task<HttpResponseMessage> ChangePassword(string userId);
        public Task<HttpResponseMessage> ResetPassword(ResetPassword model);
        public Task<HttpResponseMessage> UpdateSecurityQuestion(SecurityQuestionViewModel model);
    }
}
