using CleanMadeira.Application.Contract;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Enums;
using CleanMadeira.Web.ViewModels;
using CleanMadeira.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanMadeira.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailService _emailService;
    private readonly ICleanerNumberGenerator _cleanerNumberGenerator;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService,
        ICleanerNumberGenerator cleanerNumberGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _cleanerNumberGenerator = cleanerNumberGenerator;
    }

    [HttpGet]
    public async Task<IActionResult> LoginAsync(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (user.Role == UserRole.Limpador ||
                user.Role == UserRole.GestorELimpador)
            {
                return RedirectToAction(
                    "MyTasks",
                    "CleaningTask"
                );
            }

            return RedirectToAction(
                "Index",
                "Dashboard"
            );
        }

        var model = new LoginVM
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAsync(
    LoginVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result =
            await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

        if (!result.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                "Email ou palavra-passe inválidos."
            );

            return View(model);
        }

        var invitationToken =
        HttpContext.Session.GetString(
            "PendingCompanyInvitationToken"
        );

        if (!string.IsNullOrWhiteSpace(invitationToken))
        {
            HttpContext.Session.Remove(
                "PendingCompanyInvitationToken"
            );

            return RedirectToAction(
                "AcceptInvitation",
                "Company",
                new
                {
                    token = invitationToken
                }
            );
        }

        // fluxo normal
        var user = await _userManager.FindByEmailAsync(
            model.Email
        );

        if (user == null)
            return Unauthorized();

        if (user.Role == UserRole.Limpador ||
            user.Role == UserRole.GestorELimpador)
        {
            return RedirectToAction(
                "MyTasks",
                "CleaningTask"
            );
        }

        return RedirectToAction(
            "Index",
            "Dashboard"
        );
    }

    public IActionResult Register()
    {
        return View(new RegisterVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (vm.Role == UserRole.Dono && vm.Type == null)
        {
            ModelState.AddModelError(nameof(vm.Type),
                "Selecione se é pessoa singular ou empresa.");
        }

        if (vm.Role == UserRole.Dono &&
            vm.Type == OwnerType.Empresa &&
            string.IsNullOrWhiteSpace(vm.EmpresaNome))
        {
            ModelState.AddModelError(nameof(vm.EmpresaNome),
                "O nome da empresa é obrigatório.");
        }

        var existingUser = await _userManager.FindByEmailAsync(vm.Email);

        if (existingUser != null)
        {
            ModelState.AddModelError(nameof(vm.Email),
                "Já existe uma conta com este email.");

            return View(vm);
        }

        if (!ModelState.IsValid)
            return View(vm);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = vm.Email,
            Email = vm.Email,
            PhoneNumber = vm.Telemovel,
            PrimeiroNome = vm.PrimeiroNome,
            UltimoNome = vm.UltimoNome,
            Role = vm.Role,
            Active = true,
            CreatedAt = DateTime.UtcNow
        };

        if (user.Role == UserRole.Limpador)
        {
            user.CleanerNumber =
                await _cleanerNumberGenerator.GenerateAsync();
        }

        var result = await _userManager.CreateAsync(user, vm.Password);

        if (result.Succeeded && (user.Role == UserRole.Limpador || user.Role == UserRole.GestorELimpador))
        {
            //user.LimpadorCodigo = $"LMP-{user.SequentialNumber:D6}";
            await _userManager.UpdateAsync(user);
        }

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(vm);
        }
        else
        {
            var token = await _userManager
        .GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.Action(
                nameof(ConfirmEmail),
                "Account",
                new
                {
                    userId = user.Id,
                    token
                },
                protocol: Request.Scheme);

            await _emailService.SendConfirmationEmailAsync(
                user,
                confirmationLink);

            return RedirectToAction(nameof(RegisterConfirmation));
        }

        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToAction("Index", "Dashboard");
    }

    /*   [HttpPost]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> Logout()
       {
           await _signInManager.SignOutAsync();

           return RedirectToAction(nameof(Login));
       }

       public IActionResult AccessDenied()
       {
           return View();
       }*/

    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);

        if (user == null)
            return RedirectToAction(nameof(ForgotPasswordConfirmation));

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink = Url.Action(
            "ResetPassword",
            "Account",
            new { email = vm.Email, token = token },
            Request.Scheme);

        // Aqui depois envias por email
        Console.WriteLine(resetLink);
        try
        {
            await _emailService.SendResetPasswordEmailAsync(user, resetLink);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Erro ao enviar email: " + ex.Message);
            return View(vm);
        }

        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }

    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    public IActionResult ResetPassword(string email, string token)
    {
        return View(new ResetPasswordVM
        {
            Email = email,
            Token = token
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);

        if (user == null)
            return RedirectToAction(
                "Login",
                "Account"
            );

        var result = await _userManager.ResetPasswordAsync(
            user,
            vm.Token,
            vm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(vm);
        }

        return RedirectToAction(
            "Login",
            "Account"
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(
            "Login",
            "Account"
        );
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Perfil()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var vm = new EditPerfilVM
        {
            PrimeiroNome = user.PrimeiroNome,
            UltimoNome = user.UltimoNome,
            Email = user.Email ?? string.Empty,
            Telemovel = user.PhoneNumber,
            LimpadorCodigo = user.CleanerCode
        };

        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(EditPerfilVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        user.PrimeiroNome = vm.PrimeiroNome.Trim();
        user.UltimoNome = vm.UltimoNome.Trim();
        user.PhoneNumber = vm.Telemovel?.Trim();

        /*if (user.Type == OwnerType.Empresa)
        {
            user.CompanyName = vm.EmpresaNome?.Trim();
        }*/

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(vm);
        }

        await _signInManager.RefreshSignInAsync(user);

        TempData["Success"] = "Perfil atualizado com sucesso.";

        return RedirectToAction(nameof(Perfil));
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(
    Guid userId,
    string token)
    {
        if (userId == Guid.Empty || string.IsNullOrWhiteSpace(token))
        {
            return View("ConfirmEmailError");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return View("ConfirmEmailError");
        }

        if (await _userManager.IsEmailConfirmedAsync(user))
        {
            return View("ConfirmEmailSuccess");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            var loginLink = Url.Action(
                "Login",
                "Account",
                values: null,
                protocol: Request.Scheme)!;
            await _emailService.SendWelcomeEmailAsync(user, loginLink);
            return View("ConfirmEmailSuccess");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View("ConfirmEmailError");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }


    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToAction(
                "Login",
                "Account"
            );
        }

        var result = await _userManager.ChangePasswordAsync(
            user,
            model.CurrentPassword,
            model.NewPassword);

        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);

            TempData["Success"] = "Palavra-passe alterada com sucesso.";

            return RedirectToAction(nameof(Index), "Dashboard");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }
}