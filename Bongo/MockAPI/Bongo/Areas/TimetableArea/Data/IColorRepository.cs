using Bongo.MockAPI.Bongo.Areas.TimetableArea.Models;
using Bongo.MockAPI.Bongo.Data;

namespace Bongo.MockAPI.Bongo.Areas.TimetableArea.Data
{
    public interface IColorRepository : IRepositoryBase<Color>
    {
        public Color GetByName(string name);
    }
}
