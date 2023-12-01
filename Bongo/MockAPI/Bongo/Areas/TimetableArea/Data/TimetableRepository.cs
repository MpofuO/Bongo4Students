using Bongo.MockAPI.Bongo.Areas.TimetableArea.Models;
using Bongo.MockAPI.Bongo.Data;

namespace Bongo.MockAPI.Bongo.Areas.TimetableArea.Data
{
    public class TimetableRepository : RepositoryBase<Timetable>, ITimetableRepository
    {
        public TimetableRepository(AppDbContext appDbContext) : base(appDbContext)
        { }

        public Timetable GetUserTimetable(string username)
        {
            return _appDbContext.Timetables.FirstOrDefault(t => t.Username == username);
        }
    }
}
