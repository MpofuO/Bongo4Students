using Bongo.Areas.TimetableArea.Models;
using Bongo.Data;

namespace Bongo.Areas.TimetableArea.Data
{
    public interface IColorEndpoint : IBaseEndpoint
    {
        public Task<HttpResponseMessage> GetAllColors();
        public Task<HttpResponseMessage> GetModuleColorWithColorDetails(string moduleCode);
        public Task<HttpResponseMessage> GetModulesWithColors();
    }
}
