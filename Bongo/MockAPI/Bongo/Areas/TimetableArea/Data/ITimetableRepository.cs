using Bongo.MockAPI.Bongo.Areas.TimetableArea.Models;
using Bongo.MockAPI.Bongo.Data;

namespace Bongo.MockAPI.Bongo.Areas.TimetableArea.Data
{
    public interface ITimetableRepository : IRepositoryBase<Timetable>
    {
        public Timetable GetUserTimetable(string username);
    }
}
