using CleanMadeira.Domain.Entities.Enums;
using CleanMadeira.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminCompanyController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminCompanyController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var companies = await _context.Companies
            .OrderBy(c => c.Status)
            .ThenBy(c => c.Name)
            .ToListAsync();

        return View(companies);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
            return NotFound();

        company.Status = CompanyStatus.Ativa;

        await _context.SaveChangesAsync();

        TempData["Success"] =
            $"{company.Name} foi aprovada.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
            return NotFound();

        company.Status = CompanyStatus.Rejeitada;

        await _context.SaveChangesAsync();

        TempData["Success"] =
            $"{company.Name} foi rejeitada.";

        return RedirectToAction(nameof(Index));
    }
}