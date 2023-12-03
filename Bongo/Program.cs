using Bongo.Data;
using Bongo.MockAPI;
using Bongo.MockAPI.Bongo.Data;
using Bongo.MockAPI.Bongo.Models;
using Bongo.MockAPI.Bongo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.AreaViewLocationFormats.Clear();
    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
    options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(typeof(MyAuthorize));
});
builder.Services.AddScoped<IEndpointWrapper, EndpointWrapper>();
builder.Services.AddTransient<IMailService, MailService>();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

#region In API
builder.Services.AddIdentity<BongoUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>().AddTokenProvider<DataProtectorTokenProvider<BongoUser>>(TokenOptions.DefaultProvider);
#endregion

builder.Services.AddScoped<IPasswordValidator<BongoUser>, CustomPasswordValidator>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();    
app.UseAuthorization();

app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Merger}/{action=Display}/{id?}"
    );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//In API
SeedData.EnsurePopulated(app);
Global.app = app;


Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(app.Configuration.GetValue<string>("SyncfusionKey:Key"));

app.Run();
