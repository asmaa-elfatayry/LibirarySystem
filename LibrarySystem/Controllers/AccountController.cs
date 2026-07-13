using Domain.Entities;
using LibrarySystem.web.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibrarySystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            IUserService userService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userService.RegisterAsync(model.Email, model.Password, model.FullName);

            if (result.IsSuccess)
            {
                // Get the registered user and sign in with FullName claim
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await _userManager.AddClaimAsync(user, new Claim("FullName", user.FullName));
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", "Category");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Add FullName claim to user
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    var existingClaim = (await _userManager.GetClaimsAsync(user))
                        .FirstOrDefault(c => c.Type == "FullName");

                    if (existingClaim != null)
                    {
                        await _userManager.RemoveClaimAsync(user, existingClaim);
                    }

                    await _userManager.AddClaimAsync(user, new Claim("FullName", user.FullName));
                }
                return RedirectToAction("Index", "Book");
            }

            ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

    }
}