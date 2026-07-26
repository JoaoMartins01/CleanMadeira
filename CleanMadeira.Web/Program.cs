using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Application.Contract;
using CleanMadeira.Application.Interfaces.Repositories;
using CleanMadeira.Application.Interfaces.Services;
using CleanMadeira.Application.Services;
using CleanMadeira.Application.Services.Implementation;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Infrastructure.Data;
using CleanMadeira.Infrastructure.Repositories;
using CleanMadeira.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
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
builder.Services.AddHttpClient<
    ICalendarSyncService,
    CalendarSyncService>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    });


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

/*using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();

    var userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    await DbInitializer.SeedAsync(
        context,
        userManager,
        roleManager);
}*/

app.Run();