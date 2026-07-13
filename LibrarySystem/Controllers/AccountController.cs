using Domain.Entities;
using LibrarySystem.web.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibrarySystem.Controllers;

public class AccountController(
 UserManager<ApplicationUser> _userManager,
 SignInManager<ApplicationUser> _signInManager,
 IUserService _userService,
 IEmailSender _emailSender) : Controller   // ← ضفنا IEmailSender
{

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _userService.RegisterAsync(model.Email, model.Password, model.FullName);

        if (!result.IsSuccess || result.Data == null)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        var user = result.Data;

        // ولّد Token تأكيد الإيميل وابعته
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmLink = Url.Action(
            "ConfirmEmail", "Account",
            new { userId = user.Id, token },
            protocol: HttpContext.Request.Scheme);

        await _emailSender.SendEmailAsync(
            user.Email!,
            "تأكيد البريد الإلكتروني - LibrarySystem",
                            $@"
                    <div style='font-family: Arial, sans-serif; line-height:1.8; color:#333;'>
                        <h2 style='color:#2c3e50;'>مرحبًا {user.FullName} 👋</h2>

                        <p>شكرًا لتسجيلك في <strong>LibrarySystem</strong>.</p>

                        <p>لإكمال إنشاء حسابك، يُرجى تأكيد عنوان بريدك الإلكتروني بالضغط على الزر التالي:</p>

                        <p style='margin: 30px 0;'>
                            <a href='{confirmLink}'
                               style='background-color:#2563eb;
                                      color:#ffffff;
                                      padding:12px 24px;
                                      text-decoration:none;
                                      border-radius:6px;
                                      display:inline-block;
                                      font-weight:bold;'>
                                تأكيد البريد الإلكتروني
                            </a>
                        </p>

                        <p>إذا لم يعمل الزر، يمكنك نسخ الرابط التالي ولصقه في المتصفح:</p>

                        <p>
                            <a href='{confirmLink}'>{confirmLink}</a>
                        </p>

                        <hr style='border:none; border-top:1px solid #ddd; margin:30px 0;' />

                        <p style='font-size:14px; color:#666;'>
                            إذا لم تقم بإنشاء هذا الحساب، يمكنك تجاهل هذه الرسالة بأمان.
                        </p>

                        <p style='font-size:14px; color:#666;'>
                            مع أطيب التحيات،<br />
                            <strong>فريق LibrarySystem</strong>
                        </p>
                    </div>");

        return RedirectToAction(nameof(RegisterConfirmation));
    }

    public IActionResult RegisterConfirmation() => View();

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            await _userManager.AddClaimAsync(user, new Claim("FullName", user.FullName));
            await _signInManager.SignInAsync(user, isPersistent: false);
            return View("ConfirmEmailSuccess");
        }

        return View("ConfirmEmailFailed");
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



}