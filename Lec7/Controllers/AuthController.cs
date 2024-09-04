using Lec7.Models;
using Lec7.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lec7.Controllers;

public class AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) : Controller
{
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser() { Language = vm.Language, Email = vm.Email, UserName = vm.Email };
            var result = await userManager.CreateAsync(user, vm.Password);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, true);
                return Redirect("/");
            }
            foreach (var error in result.Errors)
            {
                //Model state - eadd errors:
                //Model - Database entity
                //State - 

                ModelState.AddModelError("Register Failed", error.Description);
            }
        }

        return View(vm);
    }

    [HttpGet]
    public IActionResult Login(string ReturnUrl = "/")
    {
        ViewBag.ReturnUrl = ReturnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm, string ReturnUrl = "/")
    {
        if (ModelState.IsValid)
        {
            var result = await signInManager.PasswordSignInAsync(vm.Email, vm.Password, true, false);

            if (result.Succeeded)
            {
                return Redirect(ReturnUrl);
            }
            ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
        }

        return View(vm);
    }

    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return Redirect("/");
    }

    [HttpGet]
    public async Task<IActionResult> Manage()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Redirect("/");

        ViewBag.UserName = user.UserName;

        var vm = new ManageViewModel()
        {
            Language = user.Language
        };

        return View(vm);
    }
    [HttpPost]
    public async Task<IActionResult> Manage(ManageViewModel vm)
    {
        var user = await userManager.GetUserAsync(User);
        bool changed = false;
        if (user is null) return Redirect("/");

        if (vm.Password is not null && vm.NewPassword is not null)
        {
            var result = await userManager.ChangePasswordAsync(user, vm.Password, vm.NewPassword);

            if (result.Succeeded) { changed = true; }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }

        if (vm.Language is not null)
        {
            user.Language = vm.Language;
            await userManager.UpdateAsync(user);
            changed = true;
        }

        if (changed) Redirect("/");
        return View(vm);
    }
}
