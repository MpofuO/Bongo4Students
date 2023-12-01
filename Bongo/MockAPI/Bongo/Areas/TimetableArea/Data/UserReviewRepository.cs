using Bongo.MockAPI.Bongo.Areas.TimetableArea.Models.User;
using Bongo.MockAPI.Bongo.Data;

namespace Bongo.MockAPI.Bongo.Areas.TimetableArea.Data
{
    public class UserReviewRepository : RepositoryBase<UserReview>, IUserReviewRepository
    {
        public UserReviewRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
