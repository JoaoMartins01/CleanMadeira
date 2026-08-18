using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Enums;
using CleanMadeira.Infrastructure.Data;
using CleanMadeira.Web.ViewModels.Company;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleanMadeira.Web.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =====================================================
        // EMPRESA DO UTILIZADOR
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.CompanyId == null)
            {
                return RedirectToAction(nameof(Create));
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == user.CompanyId);

            if (company == null)
                return NotFound();

            return View(company);
        }

        // =====================================================
        // CRIAR EMPRESA
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            // Se já pertence a uma empresa,
            // não pode criar outra.
            if (user.CompanyId != null)
            {
                return RedirectToAction(nameof(Index));
            }

            var vm = new CreateCompanyVM();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCompanyVM vm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.CompanyId != null)
            {
                ModelState.AddModelError(
                    "",
                    "Já pertence a uma empresa."
                );
            }

            if (!ModelState.IsValid)
                return View(vm);

            // Evitar NIF duplicado
            var nifExists = await _context.Companies
                .AnyAsync(c => c.Nif == vm.Nif);

            if (nifExists)
            {
                ModelState.AddModelError(
                    nameof(vm.Nif),
                    "Já existe uma empresa registada com este NIF."
                );

                return View(vm);
            }

            var company = new Company
            {
                Id = Guid.NewGuid(),

                Name = vm.Name.Trim(),
                Nif = vm.Nif.Trim(),
                Email = vm.Email.Trim(),
                Phone = vm.Phone?.Trim(),
                PhoneNumber = vm.Phone?.Trim(),
                Adress = vm.Endereço,
                Type = vm.Type,

                // Todas as empresas começam por validar
                Status = CompanyStatus.Pendente
            };

            _context.Companies.Add(company);

            await _context.SaveChangesAsync();

            // O utilizador que criou a empresa
            // passa a pertencer à empresa.
            user.CompanyId = company.Id;

            var updateResult =
                await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                // Evita deixar empresa criada sem ligação
                _context.Companies.Remove(company);

                await _context.SaveChangesAsync();

                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error.Description
                    );
                }

                return View(vm);
            }

            TempData["Success"] =
                "Empresa criada com sucesso. Aguarda validação.";

            return RedirectToAction(nameof(Pending));
        }

        // =====================================================
        // AGUARDAR VALIDAÇÃO
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.CompanyId == null)
                return RedirectToAction(nameof(Create));

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == user.CompanyId);

            if (company == null)
                return NotFound();

            // Se já foi aprovada, vai para a empresa
            if (company.Status == CompanyStatus.Ativa)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }

        // =====================================================
        // EDITAR
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.CompanyId == null)
                return RedirectToAction(nameof(Create));

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == user.CompanyId);

            if (company == null)
                return NotFound();

            var vm = new EditCompanyVM
            {
                Id = company.Id,
                Name = company.Name,
                Nif = company.Nif,
                Email = company.Email,
                Phone = company.Phone
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCompanyVM vm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.CompanyId == null)
                return Forbid();

            if (vm.Id != user.CompanyId)
                return Forbid();

            if (!ModelState.IsValid)
                return View(vm);

            var company = await _context.Companies
                .FirstOrDefaultAsync(c =>
                    c.Id == user.CompanyId);

            if (company == null)
                return NotFound();

            var duplicateNif =
                await _context.Companies.AnyAsync(c =>
                    c.Nif == vm.Nif &&
                    c.Id != company.Id);

            if (duplicateNif)
            {
                ModelState.AddModelError(
                    nameof(vm.Nif),
                    "Já existe outra empresa com este NIF."
                );

                return View(vm);
            }

            company.Name = vm.Name.Trim();
            company.Nif = vm.Nif.Trim();
            company.Email = vm.Email.Trim();
         //   company.Phone = vm.Phone?.Trim();

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Dados da empresa atualizados.";

            return RedirectToAction(nameof(Index));
        }
    }
}