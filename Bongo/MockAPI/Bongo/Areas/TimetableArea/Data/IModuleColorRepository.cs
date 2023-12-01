using Bongo.MockAPI.Bongo.Areas.TimetableArea.Models;
using Bongo.MockAPI.Bongo.Data;

namespace Bongo.MockAPI.Bongo.Areas.TimetableArea.Data
{
    public interface IModuleColorRepository : IRepositoryBase<ModuleColor>
    {
        ModuleColor GetModuleColorWithColorDetails(string username, string moduleCode);
    }
}
