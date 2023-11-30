using Bongo.Models.ViewModels;

namespace Bongo.Data
{
    public interface IAuthorizationEndpoint:IBaseEndpoint
    {
        public Task<HttpResponseMessage> Login(LoginViewModel model);
        public Task<HttpResponseMessage> Logout();
    }
}
