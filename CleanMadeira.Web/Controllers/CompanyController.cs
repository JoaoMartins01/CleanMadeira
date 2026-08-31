using CleanMadeira.Application.Common.DTO.Company;
using CleanMadeira.Application.Contract;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Enums;
using CleanMadeira.Infrastructure.Data;
using CleanMadeira.Web.ViewModels.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleanMadeira.Web.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompanyService _companyService;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyController(
            ApplicationDbContext context,
            ICompanyService companyService,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _companyService = companyService;
            _emailService = emailService;
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

        [HttpGet]
        public async Task<IActionResult> MyCompany()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            // Ainda não pertence a nenhuma empresa
            if (!user.CompanyId.HasValue)
            {
                return RedirectToAction(
                    "CreateAccommodationManagement"
                );
            }

            return RedirectToAction(
                "Details",
                new { id = user.CompanyId.Value }
            );
        }


        // =====================================================
        // CRIAR EMPRESA
        // =====================================================
        [HttpGet]
        public IActionResult CreateCleaning()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateAccommodationManagement()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccommodationManagement(
    CreateAccommodationManagementCompanyVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.CompanyId.HasValue)
            {
                TempData["Error"] = "Já pertence a uma empresa.";
                return RedirectToAction("MyCompany");
            }

            var company = new Company
            {
                Id = Guid.NewGuid(),

                Name = model.Name.Trim(),
                NIF = model.NIF.Trim(),
                Email = model.Email.Trim(),
                PhoneNumber = model.Phone.Trim(),
                Phone = model.Phone.Trim(),
                Address = model.Address.Trim(),

                Type = CompanyType.AlojamentoLocal,

                CreatedByUserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _companyService.CreateAsync(company);

            user.CompanyId = company.Id;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            TempData["Success"] = "Empresa criada com sucesso.";

            return RedirectToAction("MyCompany");
        }

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
                .AnyAsync(c => c.NIF == vm.Nif);

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
                NIF = vm.Nif.Trim(),
                Email = vm.Email.Trim(),
                Phone = vm.Phone?.Trim(),
                PhoneNumber = vm.Phone?.Trim(),
                Address = vm.Endereço,
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
                Nif = company.NIF,
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
                    c.NIF == vm.Nif &&
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
            company.NIF = vm.Nif.Trim();
            company.Email = vm.Email.Trim();
         //   company.Phone = vm.Phone?.Trim();

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Dados da empresa atualizados.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.CompanyId != id)
                return Forbid();

            var company = await _companyService
                .GetByIdWithMembersAsync(id);

            if (company == null)
                return NotFound();

            var model = new CompanyDetailsVM
            {
                Id = company.Id,
                Name = company.Name,
                NIF = company.NIF,
                Email = company.Email,
                Phone = company.Phone,
                Address = company.Address,
                CreatedAt = company.CreatedAt,

                PropertyCount = company.Properties.Count,
                MemberCount = company.Users.Count,

                Members = company.Users
                    .Select(x => new CompanyMemberVM
                    {
                        Id = x.Id,

                        Name = $"{x.PrimeiroNome} {x.UltimoNome}",

                        Email = x.Email ?? "",

                        Role = x.CompanyRole == CompanyRole.Admin
                            ? "Admin"
                            : "Manager"
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> InviteManager(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.CompanyId != id)
                return Forbid();

            var company = await _companyService.GetByIdAsync(id);

            if (company == null)
                return NotFound();

            var model = new InviteManagerVM
            {
                CompanyId = company.Id,
                CompanyName = company.Name
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InviteManager(
    InviteManagerVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.CompanyId != model.CompanyId)
                return Forbid();

            var company = await _companyService
                .GetByIdAsync(model.CompanyId);

            if (company == null)
                return NotFound();

            var email = model.Email
                .Trim()
                .ToLowerInvariant();

            var existingUser =
                await _userManager.FindByEmailAsync(email);

            if (existingUser?.CompanyId != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Este utilizador já pertence a uma empresa.");

                model.CompanyName = company.Name;

                return View(model);
            }

            var pendingInvitation =
                await _companyService
                    .HasPendingInvitationAsync(
                        company.Id,
                        email);

            if (pendingInvitation)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Já existe um convite pendente para este email.");

                model.CompanyName = company.Name;

                return View(model);
            }

            var invitation = new CompanyInvitation
            {
                Id = Guid.NewGuid(),

                CompanyId = company.Id,

                Email = email,

                Token = Guid.NewGuid(),

                CreatedAt = DateTime.UtcNow,

                ExpiresAt = DateTime.UtcNow
                    .AddDays(7),

                Accepted = false
            };

            await _companyService
                .AddInvitationAsync(invitation);

            var inviteUrl =
                Url.Action(
                    "AcceptInvitation",
                    "Company",
                    new
                    {
                        token = invitation.Token
                    },
                    Request.Scheme
                );

            var subject =
                $"Convite para {company.Name}";

            var body = $"""
        <h2>Convite para integrar {company.Name}</h2>

        <p>
            Foi convidado para integrar a equipa de gestão
            da empresa <strong>{company.Name}</strong>
            no CleanMadeira.
        </p>

        <p>
            Clique no botão abaixo para aceitar o convite:
        </p>

        <p>
            <a href="{inviteUrl}"
               style="
                    display:inline-block;
                    padding:12px 18px;
                    background:#0d6efd;
                    color:white;
                    text-decoration:none;
                    border-radius:6px;
               ">
                Aceitar convite
            </a>
        </p>

        <p>
            Este convite expira dentro de 7 dias.
        </p>
    """;

            await _emailService.SendEmailAsync(
                email,
                subject,
                body);

            TempData["Success"] =
                "Convite enviado com sucesso.";

            return RedirectToAction(
                "Details",
                new { id = company.Id });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AcceptInvitation(Guid token)
        {
            var invitation = await _companyService
        .GetInvitationByTokenAsync(token);

            if (invitation == null)
                return NotFound();

            if (invitation.Accepted)
            {
                TempData["Error"] = "Este convite já foi utilizado.";
                return RedirectToAction("Login", "Account");
            }

            if (invitation.ExpiresAt < DateTime.UtcNow)
            {
                TempData["Error"] = "Este convite expirou.";
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                HttpContext.Session.SetString(
                    "PendingCompanyInvitationToken",
                    token.ToString()
                );

                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }


            // Está autenticado com outro email
            if (!string.Equals(
                user.Email,
                invitation.Email,
                StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] =
                    $"Este convite foi enviado para {invitation.Email}.";

                return RedirectToAction(
                    "Index",
                    "Dashboard");
            }

            // Já pertence a uma empresa
            if (user.CompanyId.HasValue)
            {
                TempData["Error"] =
                    "Já pertence a uma empresa.";

                return RedirectToAction(
                    "MyCompany",
                    "Company");
            }

            var model = new AcceptInvitationVM
            {
                Token = invitation.Token,
                CompanyName = invitation.Company.Name,
                Email = invitation.Email,
                ExpiresAt = invitation.ExpiresAt
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmInvitation(
    Guid token)
        {
            var invitation = await _companyService
                .GetInvitationByTokenAsync(token);

            if (invitation == null)
                return NotFound();

            if (invitation.Accepted)
            {
                TempData["Error"] =
                    "Este convite já foi utilizado.";

                return RedirectToAction(
                    "Index",
                    "Dashboard");
            }

            if (invitation.ExpiresAt < DateTime.UtcNow)
            {
                TempData["Error"] =
                    "Este convite expirou.";

                return RedirectToAction(
                    "Index",
                    "Dashboard");
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            // Segurança: o convite só pode ser aceite
            // pela conta correspondente ao email
            if (!string.Equals(
                user.Email,
                invitation.Email,
                StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] =
                    "Este convite pertence a outro utilizador.";

                return RedirectToAction(
                    "Index",
                    "Dashboard");
            }

            if (user.CompanyId.HasValue)
            {
                TempData["Error"] =
                    "Já pertence a uma empresa.";

                return RedirectToAction(
                    "MyCompany",
                    "Company");
            }

            // Associa o utilizador à empresa
            user.CompanyId = invitation.CompanyId;

            user.CompanyRole = CompanyRole.Manager;

            var result = await _userManager
                .UpdateAsync(user);

            if (!result.Succeeded)
            {
                TempData["Error"] =
                    "Não foi possível aceitar o convite.";

                return RedirectToAction(
                    nameof(AcceptInvitation),
                    new { token });
            }

            // Marca convite como utilizado
            invitation.Accepted = true;

            await _companyService
                .UpdateInvitationAsync(invitation);

            TempData["Success"] =
                $"Entrou na empresa {invitation.Company.Name}.";

            return RedirectToAction(
                "MyCompany",
                "Company");
        }
    }
}