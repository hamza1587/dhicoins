using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Dhicoin.Areas.Identity.Data;
using Dhicoin.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Dhicoin.Utility.Repositories;
using EmailSender = Dhicoin.Utility.Repositories.EmailSender;
using AspNetCoreHero.ToastNotification;
using Shyjus.BrowserDetection;
using AspNetCoreHero.ToastNotification.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");


builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(connectionString));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options=>
    {
    options.SignIn.RequireConfirmedAccount = false;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
    options.Lockout.MaxFailedAccessAttempts = 3;
        }) .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
   
});
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});

//builder.Services.AddAuthentication().AddFacebook(options =>
//{
//    options.AppId = builder.Configuration["Facebook:AppId"];
//    options.AppSecret = builder.Configuration["Facebook:AppSecret"];

//});
//builder.Services.AddAuthentication().AddGoogle(options =>
//{
//    options.ClientId = builder.Configuration["Google:ClientId"];
//    options.ClientSecret = builder.Configuration["Google:ClientSecret"];

//});

builder.Services.AddTransient<CurrencyConverter>(); // Register CurrencyConverter as a transient service

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddTransient<IMailSender, EmailSender>();
builder.Services.AddScoped<BrowserDetector>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var LoggerFactory = service.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = service.GetRequiredService<ApplicationDbContext>();
        var UserManeger = service.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManeger = service.GetRequiredService<RoleManager<IdentityRole>>();
        //Add defafule Three roles
        await ContextSeed.seedRolesAsync(service.GetRequiredService<RoleManager<IdentityRole>>());
        //Add default Admin who manage web application
        await ContextSeed.SeedSuperAdminAsync(UserManeger, roleManeger);

    }
    catch (Exception ex)
    {
        var logger = LoggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }

}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");
app.UseNotyf();
app.Run();

