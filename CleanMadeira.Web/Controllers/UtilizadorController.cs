using CleanMadeira.Domain.Entities;
using CleanMadeira.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleanMadeira.Web.Controllers;

public class UtilizadorController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UtilizadorController(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var loggedUser = await _userManager.GetUserAsync(User);

        if (loggedUser == null)
            return RedirectToAction("Login", "Account");

       var users = await _userManager.Users
            .ToListAsync();

        var vm = users.Select(u => new UtilizadorVM
        {
            Id = u.Id,
            PrimeiroNome = u.PrimeiroNome,
            UltimoNome = u.UltimoNome,
            Email = u.Email ?? "",
            Telemovel = u.PhoneNumber,
            Role = u.Role,
            Active = u.Active
        }).ToList();

        return View(vm);
    }

 /*   public IActionResult Create()
    {
        return View(new CreateUserViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var loggedUser = await _userManager.GetUserAsync(User);

        if (loggedUser == null)
            return RedirectToAction("Login", "Account");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = vm.Email,
            Email = vm.Email,
            PrimeiroNome = vm.FirstName,
            UltimoNome = vm.LastName,
            PhoneNumber = vm.Phone,
            Role = vm.Role,
            Ativo = true,
            CreatedAt = DateTime.UtcNow,
            //OwnerId = loggedUser.Id
        };

        var result = await _userManager.CreateAsync(user, vm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(vm);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return NotFound();

        var vm = new UtilizadorVM
        {
            Id = user.Id,
            PrimeiroNome = user.PrimeiroNome,
            UltimoNome = user.UltimoNome,
            Email = user.Email ?? "",
            Telemovel = user.PhoneNumber,
            Role = user.Role,
            Ativo = user.Ativo
        };

        return View(vm);
    }*/

    public async Task<IActionResult> Edit(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return NotFound();

        var vm = new UtilizadorVM
        {
            Id = user.Id,
            PrimeiroNome = user.PrimeiroNome,
            UltimoNome = user.UltimoNome,
            Email = user.Email ?? "",
            Telemovel = user.PhoneNumber,
            Role = user.Role,
            Active = user.Active
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UtilizadorVM vm)
    {
        if (id != vm.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return NotFound();

        user.PrimeiroNome = vm.PrimeiroNome;
        user.UltimoNome = vm.UltimoNome;
        user.Email = vm.Email;
        user.UserName = vm.Email;
        user.PhoneNumber = vm.Telemovel;
        user.Role = vm.Role;
        user.Active = vm.Active;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(vm);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return NotFound();

        var vm = new UtilizadorVM
        {
            Id = user.Id,
            PrimeiroNome = user.PrimeiroNome,
            UltimoNome = user.UltimoNome,
            Email = user.Email ?? "",
            Telemovel = user.PhoneNumber,
            Role = user.Role,
            Active = user.Active
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
            return NotFound();

        user.Active = false;

        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Index));
    }
}