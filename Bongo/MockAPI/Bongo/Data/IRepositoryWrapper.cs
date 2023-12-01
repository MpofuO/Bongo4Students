using Bongo.MockAPI.Bongo.Areas.TimetableArea.Data;

namespace Bongo.MockAPI.Bongo.Data
{
    public interface IRepositoryWrapper
    {
        ITimetableRepository Timetable { get; }
        IModuleColorRepository ModuleColor { get; }
        IColorRepository Color { get; }
        IUserReviewRepository UserReview { get; }
        void SaveChanges();
    }
}