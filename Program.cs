using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// 1. Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. ASP.NET Core Identity Configuration
// Do NOT use AddDefaultIdentity() so we can avoid the pre-built Microsoft UI
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    // Strict password policies for your admin portal
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 16;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.User.RequireUniqueEmail = true;

    // Lockout configuration for brute-force protection
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 3. Custom Cookie Configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Portfolio.AdminAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.LoginPath = "/Admin/Auth/Login";
    options.AccessDeniedPath = "/Admin/Auth/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// 4. Seed the Admin role and default admin user on first run
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Ensure the "Admin" role exists
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<int> { Name = "Admin" });
        }

        // Seed a default admin if no admin user exists yet
        var adminEmail = builder.Configuration["AdminSeed:Email"] ?? "admin@portfol.io";
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                FullName = "Portfolio Admin",
                Bio = "Portfolio owner. Update this bio from the admin dashboard.",
                AvailabilityStatus = EAvailabilityStatus.Building,
                EmailConfirmed = true // Skip email confirmation for the seed user
            };

            // Use a strong default password — CHANGE THIS after first login
            var seedPassword = builder.Configuration["AdminSeed:Password"] ?? "Admin!Portfol.io2026";
            var result = await userManager.CreateAsync(adminUser, seedPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        else if (existingAdmin.UserName == adminEmail)
        {
            existingAdmin.UserName = "admin";
            await userManager.UpdateAsync(existingAdmin);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
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
app.UseResponseCompression();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24 * 365; // 1 year
        ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] =
            "public,max-age=" + durationInSeconds;
    }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 5. Map routing to support the Admin Area boundary alongside public routes
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "user_portfolio",
    pattern: "{username}",
    defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();