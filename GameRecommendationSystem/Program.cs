
using GameRecommendationSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// DATABASE CONNECTION
// ==========================================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// ==========================================
// IDENTITY
// ==========================================

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();


// ==========================================
// MVC AND RAZOR PAGES
// ==========================================

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();


// ==========================================
// BUILD APPLICATION
// ==========================================

var app = builder.Build();


// ==========================================
// HTTP PIPELINE
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// IMPORTANT: Enables CSS, JavaScript and images from wwwroot
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


// ==========================================
// ROUTES
// ==========================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


// ==========================================
// START APPLICATION
// ==========================================

app.Run();
