using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Entities.Enums;
using CleanMadeira.Web.ViewModels.Team;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleanMadeira.Web.Controllers
{
    public class TeamController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TeamController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser =
                await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized();

            if (currentUser.Role != UserRole.Gestor &&
                currentUser.Role != UserRole.GestorELimpador)
            {
                return Forbid();
            }

            if (currentUser.CompanyId == null)
                return RedirectToAction("Create", "Company");

            var members = await _userManager.Users
                .Where(u => u.CompanyId == currentUser.CompanyId)
                .OrderBy(u => u.PrimeiroNome)
                .ToListAsync();

            return View(members);
        }


        [HttpGet]
        public IActionResult AddMember()
        {
            return View(new AddTeamMemberVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMember(AddTeamMemberVM vm)
        {
            var gestor = await _userManager.GetUserAsync(User);

            if (gestor == null)
                return Unauthorized();

            // Só um gestor de limpeza pode adicionar membros
            if (gestor.Role != UserRole.GestorELimpador)
                return Forbid();

            // O gestor tem de pertencer a uma empresa
            if (gestor.CompanyId == null)
            {
                ModelState.AddModelError(
                    "",
                    "Ainda não tem uma empresa associada."
                );

                return View(vm);
            }

            if (!ModelState.IsValid)
                return View(vm);

            // Procurar utilizador pelo email
            var member = await _userManager.FindByEmailAsync(
                vm.Email.Trim()
            );

            if (member == null)
            {
                ModelState.AddModelError(
                    nameof(vm.Email),
                    "Não existe nenhum utilizador registado com este email."
                );

                return View(vm);
            }

            // Não pode adicionar-se a si próprio
            if (member.Id == gestor.Id)
            {
                ModelState.AddModelError(
                    nameof(vm.Email),
                    "Já pertence a esta empresa."
                );

                return View(vm);
            }

            // O utilizador tem de estar registado como limpador
            if (member.Role != UserRole.Limpador)
            {
                ModelState.AddModelError(
                    nameof(vm.Email),
                    "Este utilizador não está registado como limpador."
                );

                return View(vm);
            }

            // Já pertence a alguma empresa
            if (member.CompanyId != null)
            {
                if (member.CompanyId == gestor.CompanyId)
                {
                    ModelState.AddModelError(
                        nameof(vm.Email),
                        "Este utilizador já pertence à sua equipa."
                    );
                }
                else
                {
                    ModelState.AddModelError(
                        nameof(vm.Email),
                        "Este utilizador já pertence a outra empresa."
                    );
                }

                return View(vm);
            }

            // ASSOCIAÇÃO DO LIMPADOR À EMPRESA
            member.CompanyId = gestor.CompanyId;

            var result = await _userManager.UpdateAsync(member);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error.Description
                    );
                }

                return View(vm);
            }

            TempData["Success"] =
                $"{member.PrimeiroNome} foi adicionado à equipa.";

            return RedirectToAction(nameof(Index));
        }
    }
}
