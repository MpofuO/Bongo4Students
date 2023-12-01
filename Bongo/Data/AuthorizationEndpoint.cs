using Bongo.Models.ViewModels;

namespace Bongo.Data
{
    public class AuthorizationEndpoint : BaseEndpoint, IAuthorizationEndpoint
    {
        public AuthorizationEndpoint(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            URI += "authorization";
        }

        public async Task<HttpResponseMessage> ChangePassword(string userId)
        {
            return await Client.GetAsync(new Uri($"{URI}/ChangePassword/{userId}"));
        }

        public async Task<HttpResponseMessage> ForgotPassword(AnswerSecurityQuestionViewModel model)
        {
            using (HttpContent content = JsonContent.Create(model))
            {
                return await Client.PostAsync(new Uri($"{URI}/ForgotPassword"), content);
            }
        }

        public async Task<HttpResponseMessage> Login(LoginViewModel model)
        {
            using (HttpContent content = JsonContent.Create(model))
            {
                return await Client.PostAsync(new Uri($"{URI}/login"), content);
            }
        }

        public async Task<HttpResponseMessage> Logout()
        {
            return await Client.GetAsync(new Uri($"{URI}/logout"));
        }

        public async Task<HttpResponseMessage> Register(RegisterModel registerModel)
        {
            using (HttpContent content = JsonContent.Create(registerModel))
            {
                return await Client.PostAsync(new Uri($"{URI}/register"), content);
            }
        }

        public async Task<HttpResponseMessage> ResetPassword(ResetPassword model)
        {
            using (HttpContent content = JsonContent.Create(model))
            {
                return await Client.PostAsync(new Uri($"{URI}/ResetPassword"), content);
            }
        }

        public async Task<HttpResponseMessage> UpdateSecurityQuestion(SecurityQuestionViewModel model)
        {
            using (HttpContent content = JsonContent.Create(model))
            {
                return await Client.PostAsync(new Uri($"{URI}/UpdateSecurityQuestion"), content);
            }
        }

        public async Task<HttpResponseMessage> VerifyEmail(string email)
        {
            return await Client.GetAsync(new Uri($"{URI}/VerifyEmail/{email}"));
        }
    }
}
