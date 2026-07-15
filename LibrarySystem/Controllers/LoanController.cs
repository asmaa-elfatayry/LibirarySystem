using LibrarySystem.web.ViewModels.Loan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibrarySystem.web.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class LoanController(
    ILoanService _loanService,
    IBookService _bookService,
    UserManager<ApplicationUser> _userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var loans = await _loanService.GetAllAsync();
        return View(loans);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new LoanViewModel();
        await PopulateDropdownsAsync(vm);
        return View(vm);
    }

    [Authorize] 
    public async Task<IActionResult> MyLoans()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var loans = await _loanService.GetActiveLoansForMemberAsync(userId);
        return View(loans);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LoanViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }

        var result = await _loanService.BorrowAsync(vm.MemberId, vm.BookId);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ReturnAjax(Guid id)
    {
        var result = await _loanService.ReturnAsync(id);
        return Json(new { success = result.IsSuccess, message = result.Message });
    }

    private async Task PopulateDropdownsAsync(LoanViewModel vm)
    {
        var members = await _userManager.GetUsersInRoleAsync("Member");
        var books = await _bookService.GetAllAsync();

        vm.Members = members.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.FullName });
        vm.Books = books.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Title });
    }
}
