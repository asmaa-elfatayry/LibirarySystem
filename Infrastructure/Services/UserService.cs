using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<ApplicationUser>> RegisterAsync(string email, string password, string fullName)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            MembershipDate = DateTime.UtcNow
            // لاحظ: EmailConfirmed بتفضل false افتراضيًا (القيمة الأصلية في IdentityUser)
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return Result<ApplicationUser>.Failure(
                Domain.Enums.eResultStatus.ValidationError,
                string.Join("; ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Member");

        return Result<ApplicationUser>.SuccessWithData(user, "تم إنشاء الحساب بنجاح");
    }
}
