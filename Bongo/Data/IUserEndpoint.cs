using Bongo.Models;
using Bongo.Models.ViewModels;

namespace Bongo.Data
{
    public interface IUserEndpoint : IBaseEndpoint
    {
        public Task<HttpResponseMessage> Create(BongoUser user);
        public Task<HttpResponseMessage> Update(BongoUser user);
        public Task<HttpResponseMessage> Delete(BongoUser user);
    }
}
