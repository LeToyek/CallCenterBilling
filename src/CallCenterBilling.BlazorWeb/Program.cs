using CallCenterBilling.BlazorWeb.Components;
using CallCenterBilling.Application.Mappings;
using CallCenterBilling.Infrastructure;
using CallCenterBilling.Domain.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Add Infrastructure services (includes DbContext, Identity, and all repositories/services)
builder.Services.AddInfrastructure(builder.Configuration);

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add authorization policies (optional - customize as needed)
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
    .AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Security middleware order is important!
app.UseRouting();
app.UseAuthentication();  // Must come before UseAuthorization
app.UseAuthorization();
app.UseAntiforgery();

// Map authentication routes
app.MapPost("/Account/Logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

// Map Razor Components and Pages
// Don't require auth for entire app - handle it per component/page instead
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

// Ensure database is created and seeded (optional)
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        // Seed default roles and admin user if needed
        await SeedDataAsync(serviceProvider);
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();

// Helper method for seeding initial data
static async Task SeedDataAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Create roles if they don't exist
    string[] roleNames = { "Admin", "User", "Operator", "Manager" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Create default admin user
    var adminEmail = "admin@callcenter.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "System",
            LastName = "Administrator",
            IsActive = true
        };
        
        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}