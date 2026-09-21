using System.Globalization;
using FoodDispatchSystem.Web.Data;
using Microsoft.EntityFrameworkCore;
using FoodDispatchSystem.Web.Models;
using Microsoft.AspNetCore.Identity;
using FoodDispatchSystem.Web.Services;


var builder = WebApplication.CreateBuilder(args);
var cultureInfo = new CultureInfo("en-US");

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<BusinessService>();
builder.Services.AddScoped<OrderItemService>();


builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();



var app = builder.Build();
await IdentitySeeder.SeedRolesAsync(app.Services);

var seedAdminEmail =
    app.Configuration["SeedAdmin:Email"];

var seedAdminPassword =
    app.Configuration["SeedAdmin:Password"];

if (!string.IsNullOrWhiteSpace(seedAdminEmail) &&
    !string.IsNullOrWhiteSpace(seedAdminPassword))
{
    await IdentitySeeder.SeedAdminAsync(app.Services);
}

if (app.Environment.IsDevelopment())
{
    await IdentitySeeder.SeedCajeroAsync(app.Services);
    await IdentitySeeder.SeedCocinaAsync(app.Services);
    await IdentitySeeder.SeedDespachoAsync(app.Services);
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
app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
