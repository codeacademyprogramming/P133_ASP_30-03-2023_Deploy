using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using P133Allup.DataAccessLayer;
using P133Allup.Helpers;
using P133Allup.Hubs;
using P133Allup.Interfaces;
using P133Allup.Models;
using P133Allup.Services;
using P133Allup.ViewModels;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;

    options.User.RequireUniqueEmail = true;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 3;
}).AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddErrorDescriber<IdentityErrorDescriberAz>();

builder.Services.Configure<SmtpSetting>(builder.Configuration.GetSection("SmtpSetting"));
builder.Services.AddScoped<ILayoutService, LayoutService>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

//AddScopped(),AddTransient(),AddSingelton()
var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{

//}
//else
//{

//}

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
          );
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapHub<NotificationHub>("/semedHub");

app.Run();
