using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Entities.Enums;

//using CleanMadeira.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CleanMadeira.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        await context.Database.MigrateAsync();

       // await SeedRolesAsync(roleManager);

        var company = await SeedCompanyAsync(context);

        await SeedAdminUserAsync(userManager, company.Id);

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        var roles = new[]
        {
            "Admin",
            "Supervisor",
            "Cleaner"
        };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };

                await roleManager.CreateAsync(role);
            }
        }
    }

   private static async Task<Company> SeedCompanyAsync(
        ApplicationDbContext context)
    {
        var company = await context.Companies
            .FirstOrDefaultAsync(x => x.Name == "CleanMadeira Demo");

        if (company != null)
            return company;

        company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "CleanMadeira Demo",
            Nif = "000000000",
            Email = "demo@cleanmadeira.pt",
            PhoneNumber= "900000000",
            Active = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Companies.Add(company);

        await context.SaveChangesAsync();

        return company;
    }

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        Guid companyId)
    {
        var email = "admin@cleanmadeira.pt";

        var user = await userManager.FindByEmailAsync(email);

        if (user != null)
            return;

        user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            PrimeiroNome = "Admin",
            UltimoNome = "CleanMadeira",
            Role = UserRole.Admin,
            Active = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(
            user,
            "Admin123!");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}