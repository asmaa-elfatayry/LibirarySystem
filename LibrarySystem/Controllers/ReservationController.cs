using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.web.Controllers;

[Authorize] // أي مستخدم مسجل دخول (مش بس Admin/Librarian) - العضو نفسه هيستخدمها
public class ReservationController(
    IReservationService _reservationService,
    UserManager<ApplicationUser> _userManager) : Controller
{
    public async Task<IActionResult> MyReservations()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var reservations = await _reservationService.GetMyReservationsAsync(userId);
        return View(reservations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax(Guid bookId)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var result = await _reservationService.CreateAsync(userId, bookId);

        return Json(new { success = result.IsSuccess, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> CancelAjax(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var isStaff = User.IsInRole("Admin") || User.IsInRole("Librarian");

        var result = await _reservationService.CancelAsync(id, userId, isStaff);
        return Json(new { success = result.IsSuccess, message = result.Message });
    }

    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Index()
    {
        var reservations = await _reservationService.GetAllAsync();
        return View(reservations);
    }
}
