using Bongo.MockAPI.Bongo.Areas.TimetableArea.Models;
using Bongo.Areas.TimetableArea.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bongo.MockAPI.Bongo.Models;

namespace Bongo.MockAPI.Bongo.Data
{
    public class AppDbContext : IdentityDbContext<BongoUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ModuleColor> ModuleColors { get; set; }
        public DbSet<UserReview> UserReviews { get; set; }  

    }
}
