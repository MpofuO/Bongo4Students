using Bongo.Models.ViewModels;

namespace Bongo.Data
{
    public class AuthorizationEndpoint : BaseEndpoint, IAuthorizationEndpoint
    {
        public AuthorizationEndpoint(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            URI += "authorization";
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
    }
}
