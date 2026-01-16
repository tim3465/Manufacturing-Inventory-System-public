using System.Text;
using CncApp.Api.Middleware;
using CncApp.Application;
using CncApp.Infrastructure;
using CncApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Optional local-only configuration for secrets not committed to source control
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add ProblemDetails support and register global exception handler
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Register Application and Infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Register ASP.NET Core Identity
builder.Services.AddIdentityCore<IdentityUser<int>>(options =>
{
    // Identity options can be configured here if needed
})
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable exception handling middleware
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ==========================================================
// DEV-ONLY START: AUTH SEEDING (REMOVE BEFORE PRODUCTION)
// ==========================================================


//if (app.Environment.IsDevelopment())
//{
//    // Seed Identity roles (Admin and User)
//    await SeedRolesAsync(app.Services);

//    //  DEV ONLY - Seed dev admin user (remove/disable in production)
//    await SeedDevAdminAsync(app.Services, app.Configuration, app.Logger);
//}


//// Role seeding helper - ensures Admin and User roles exist on startup
//static async Task SeedRolesAsync(IServiceProvider services)
//{
//    using var scope = services.CreateScope();
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

//    var roles = new[] { "Admin", "User" };

//    foreach (var roleName in roles)
//    {
//        var roleExists = await roleManager.RoleExistsAsync(roleName);
//        if (!roleExists)
//        {
//            var result = await roleManager.CreateAsync(new IdentityRole<int> { Name = roleName });
//            if (result.Succeeded)
//            {
//                Console.WriteLine($"Created role: {roleName}");
//            }
//            else
//            {
//                Console.WriteLine($"Failed to create role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
//            }
//        }
//    }
//}



////  DEV ONLY - Seed dev admin user (remove/disable in production)
//// Creates admin@local.test with password Admin123! and assigns Admin role
//static async Task SeedDevAdminAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
//{
//    var devAdminEmail = configuration["DevAdmin:Email"];
//    var devAdminPassword = configuration["DevAdmin:Password"];

//    if (string.IsNullOrWhiteSpace(devAdminEmail) || string.IsNullOrWhiteSpace(devAdminPassword))
//    {
//        logger.LogInformation("Dev admin seeding skipped (DevAdmin credentials not configured)");
//        return;
//    }

//    using var scope = services.CreateScope();
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<int>>>();

//    // Check if user already exists
//    var existingUser = await userManager.FindByEmailAsync(devAdminEmail);
//    if (existingUser != null)
//    {
//        // User exists - ensure they have Admin role
//        var isInAdminRole = await userManager.IsInRoleAsync(existingUser, "Admin");
//        if (!isInAdminRole)
//        {
//            var addToRoleResult = await userManager.AddToRoleAsync(existingUser, "Admin");
//            if (addToRoleResult.Succeeded)
//            {
//                Console.WriteLine($"(DEV ONLY): Added Admin role to existing user: {devAdminEmail}");
//            }
//        }
//        return;
//    }

//    // Create new dev admin user
//    var adminUser = new IdentityUser<int>
//    {
//        UserName = devAdminEmail,
//        Email = devAdminEmail,
//        EmailConfirmed = true // Skip email confirmation for dev user
//    };

//    var createResult = await userManager.CreateAsync(adminUser, devAdminPassword);
//    if (createResult.Succeeded)
//    {
//        // Assign Admin role
//        var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
//        if (addToRoleResult.Succeeded)
//        {
//            Console.WriteLine($"(DEV ONLY): Created admin user: {devAdminEmail}");
//        }
//        else
//        {
//            Console.WriteLine($"(DEV ONLY): Created user but failed to assign Admin role: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
//        }
//    }
//    else
//    {
//        Console.WriteLine($" (DEV ONLY): Failed to create admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
//    }


//}
    // ==========================================================
    // DEV-ONLY END: AUTH SEEDING
    // ==========================================================
app.Run();