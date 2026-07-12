using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.web.Controllers;

[Authorize]
public class FineController(
   IFineService _fineService,
   UserManager<ApplicationUser> _userManager) : Controller
{
    public async Task<IActionResult> MyFines()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var fines = await _fineService.GetMyFinesAsync(userId);
        return View(fines);
    }

    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Index()
    {
        var fines = await _fineService.GetAllAsync();
        return View(fines);
    }

    [Authorize(Roles = "Admin,Librarian")]
    [HttpPost]
    public async Task<IActionResult> MarkAsPaidAjax(Guid id)
    {
        var result = await _fineService.MarkAsPaidAsync(id);
        return Json(new { success = result.IsSuccess, message = result.Message });
    }
}
