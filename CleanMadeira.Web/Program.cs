using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Application.Contract;
using CleanMadeira.Application.Interfaces;
using CleanMadeira.Application.Interfaces.Repositories;
using CleanMadeira.Application.Interfaces.Services;
using CleanMadeira.Application.Services;
using CleanMadeira.Application.Services.Implementation;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Interfaces;
using CleanMadeira.Infrastructure.Data;
using CleanMadeira.Infrastructure.Repositories;
using CleanMadeira.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Mail;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.ReferenceHandler =
             System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
     });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
           errorNumbersToAdd: null
        )
    );
});

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IUtilizadorRepositorio, UtilizadorRepository>();
builder.Services.AddScoped<ICleaningTaskRepository, CleaningTaskRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ITaskPhotoRepository, TaskPhotoRepository>();
builder.Services.AddScoped<ICalendarIntegrationRepository, CalendarIntegrationRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IMaintenanceProviderRepository, MaintenanceProviderRepository>();
builder.Services.AddScoped<IMaintenanceReportRepository, MaintenanceReportRepository>();


builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IUtilizadorService, UtilizadorService>();
builder.Services.AddScoped<ICleaningTaskService, CleaningTaskService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<ICalendarIntegrationService, CalendarIntegrationService>();
builder.Services.AddScoped<ICleanerNumberGenerator, CleanerNumberGenerator>();
builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
builder.Services.AddScoped<ICalendarIntegrationService, CalendarIntegrationService>();
builder.Services.AddScoped<IMaintenanceProviderService, MaintenanceProviderService>();
builder.Services.AddScoped<IMaintenanceReportService, MaintenanceReportService>();
builder.Services.AddHttpClient<
    ICalendarSyncService,
    CalendarSyncService>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    });

builder.Services.Configure<IConfiguration>(
    builder.Configuration.GetSection("EmailSettings"));


builder.Services.AddAuthentication()
    .AddCookie("SiteAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    })
    .AddJwtBearer("ApiAuth", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),

            NameClaimType = ClaimTypes.NameIdentifier
        };
    });





builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();

    var userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

   await DbInitializer.SeedAdminUserAsync(userManager);

}

app.Run();