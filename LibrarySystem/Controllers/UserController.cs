using LibrarySystem.web.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController(
      UserManager<ApplicationUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var viewModels = new List<UserListItemViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                viewModels.Add(new UserListItemViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    CurrentRole = roles.FirstOrDefault() ?? "بدون دور",
                    MembershipDate = user.MembershipDate
                });
            }

            return View(viewModels.OrderBy(u => u.FullName).ToList());
        }

        public IActionResult Create() => View(new CreateUserViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var existing = await _userManager.FindByEmailAsync(vm.Email);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(vm.Email), "البريد الإلكتروني مستخدم بالفعل");
                return View(vm);
            }

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                FullName = vm.FullName,
                EmailConfirmed = true,
                MembershipDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(vm);
            }

            await _userManager.AddToRoleAsync(user, vm.Role);
            TempData["Success"] = $"تم إنشاء المستخدم {vm.FullName} بدور {vm.Role}";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRoleAjax(Guid userId, string newRole)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (userId.ToString() == currentUserId)
                return Json(new { success = false, message = "لا يمكنك تغيير دورك الخاص" });

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Json(new { success = false, message = "المستخدم غير موجود" });

            var currentRoles = await _userManager.GetRolesAsync(user);

            // حماية: منع إزالة آخر Admin في النظام
            if (currentRoles.Contains("Admin") && newRole != "Admin")
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                if (adminUsers.Count <= 1)
                    return Json(new { success = false, message = "لا يمكن إزالة آخر Admin في النظام" });
            }

            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);

            return Json(new { success = true, message = $"تم تحديث الدور إلى {newRole}" });
        }
    }
}
