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


    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);

        // لاحظ: بنرجّع نفس الرسالة سواء الإيميل موجود أو لأ - هنشرح ليه تحت
        if (user != null && await _userManager.IsEmailConfirmedAsync(user))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action(
                "ResetPassword", "Account",
                new { userId = user.Id, token },
                protocol: HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.Email!,
                "إعادة تعيين كلمة المرور - LibrarySystem",
                $@"
                    <div style='font-family: Arial, sans-serif; color:#333;'>
                        <h3>مرحبًا {user.FullName} 👋</h3>

                        <p>لقد تلقينا طلبًا لإعادة تعيين كلمة المرور الخاصة بحسابك.</p>

                        <p>
                            اضغط على الرابط التالي لإعادة تعيين كلمة المرور:
                        </p>

                        <p>
                            <a href='{resetLink}'>إعادة تعيين كلمة المرور</a>
                        </p>

                        <p style='color:#666; font-size:14px;'>
                            إذا لم تطلب هذا الإجراء، يمكنك تجاهل هذه الرسالة.
                        </p>

                        <p>
                            مع تحيات فريق <strong>LibrarySystem</strong>
                        </p>
                    </div>");

         
        }
        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }

    public IActionResult ForgotPasswordConfirmation() => View();

    [HttpGet]
    public IActionResult ResetPassword(Guid userId, string token)
    {
        var model = new ResetPasswordViewModel { UserId = userId, Token = token };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByIdAsync(model.UserId.ToString());
        if (user == null)
            return RedirectToAction(nameof(ResetPasswordConfirmation));

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

        if (result.Succeeded)
            return RedirectToAction(nameof(ResetPasswordConfirmation));

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    public IActionResult ResetPasswordConfirmation() => View();


    [HttpGet]
    public IActionResult AccessDenied() => View();

    #region external login 

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            TempData["Error"] = "حصلت مشكلة أثناء تسجيل الدخول بجوجل";
            return RedirectToAction(nameof(Login));
        }

        // الخطوة 1: هل الحساب ده سجّل بجوجل قبل كده؟
        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false);

        if (signInResult.Succeeded)
            return RedirectToAction("Index", "Home");

        // الخطوة 2: لو أول مرة، نجيب الإيميل من Google ونشوف لو عنده حساب أصلاً بنفس الإيميل
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var fullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email ?? "مستخدم جديد";

        if (string.IsNullOrEmpty(email))
        {
            TempData["Error"] = "مقدرناش نجيب بريدك الإلكتروني من جوجل";
            return RedirectToAction(nameof(Login));
        }

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            // المستخدم عنده حساب بالإيميل بتاعه أصلاً (سجّل عادي قبل كده) - نربط جوجل بيه
            await _userManager.AddLoginAsync(existingUser, info);
            await _signInManager.SignInAsync(existingUser, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        // الخطوة 3: مستخدم جديد تمامًا - نعمله حساب
        var newUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true,   // جوجل أصلاً أكّد الإيميل ده، فمينفعش نطلب تأكيد تاني
            MembershipDate = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
        {
            TempData["Error"] = "مقدرناش ننشئ حساب جديد";
            return RedirectToAction(nameof(Login));
        }

        await _userManager.AddToRoleAsync(newUser, "Member");
        await _userManager.AddLoginAsync(newUser, info);
        await _signInManager.SignInAsync(newUser, isPersistent: false);

        return RedirectToAction("Index", "Home");
    }
    #endregion


}