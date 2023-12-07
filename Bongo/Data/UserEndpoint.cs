using Bongo.Models;

namespace Bongo.Data
{
    public class UserEndpoint : BaseEndpoint, IUserEndpoint
    {
        public UserEndpoint(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            URI += "User";
        }

        public async Task<HttpResponseMessage> Create(BongoUser user)
        {
            using (HttpContent content = JsonContent.Create(user))
            {
                return await Client.PostAsync(new Uri($"{URI}/Create"), content);
            }
        }

        public async Task<HttpResponseMessage> Delete(BongoUser user)
        {
            using (HttpContent content = JsonContent.Create(user))
            {
                return await Client.PostAsync(new Uri($"{URI}/Delete"), content);
            }
        }

        public async Task<HttpResponseMessage> GetUserByName(string username)
        {
            return await Client.GetAsync(new Uri($"{URI}/GetUserByName/{username}"));
        }
        public async Task<HttpResponseMessage> GetUserByEmail(string email)
        {
            return await Client.GetAsync(new Uri($"{URI}/GetUserByEmail/{email}"));
        }

        public async Task<HttpResponseMessage> Update(BongoUser user)
        {
            using (HttpContent content = JsonContent.Create(user))
            {
                return await Client.PostAsync(new Uri($"{URI}/Update"), content);
            }
        }
    }
}
