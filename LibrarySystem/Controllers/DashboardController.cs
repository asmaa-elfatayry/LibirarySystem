using LibrarySystem.web.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.web.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class DashboardController(
        IBookService _bookService,
        ILoanService _loanService,
        IReservationService _reservationService,
        IFineService _fineService,
        UserManager<ApplicationUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllAsync();
            var loans = await _loanService.GetAllAsync();
            var reservations = await _reservationService.GetAllAsync();
            var fines = await _fineService.GetAllAsync();
            var members = await _userManager.GetUsersInRoleAsync("Member");

            var vm = new DashboardViewModel
            {
                TotalBooks = books.Count,
                TotalMembers = members.Count,
                ActiveLoans = loans.Count(l => l.Status == "Active"),
                OverdueLoans = loans.Count(l => l.Status == "Active" && l.DueDate < DateTime.UtcNow),
                PendingReservations = reservations.Count(r => r.Status == "Pending"),
                UnpaidFinesTotal = fines.Where(f => !f.IsPaid).Sum(f => f.Amount)
            };

            // ===== أكتر 5 كتب استعارة =====
            var topBooks = loans
                .Where(l => !string.IsNullOrEmpty(l.BookTitle))
                .GroupBy(l => l.BookTitle!)
                .Select(g => new { Title = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            vm.TopBooksLabels = topBooks.Select(x => x.Title).ToList();
            vm.TopBooksCounts = topBooks.Select(x => x.Count).ToList();

            // ===== الاستعارات آخر 14 يوم =====
            var last14Days = Enumerable.Range(0, 14)
                .Select(i => DateTime.UtcNow.Date.AddDays(-i))
                .OrderBy(d => d)
                .ToList();

            foreach (var day in last14Days)
            {
                vm.LoansTimelineLabels.Add(day.ToString("dd/MM"));
                vm.LoansTimelineCounts.Add(loans.Count(l => l.BorrowDate.Date == day));
            }

            // ===== توزيع الكتب حسب التصنيف =====
            var categoryGroups = books
                .Where(b => !string.IsNullOrEmpty(b.CategoryName))
                .GroupBy(b => b.CategoryName!)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            vm.CategoryLabels = categoryGroups.Select(x => x.Category).ToList();
            vm.CategoryCounts = categoryGroups.Select(x => x.Count).ToList();

            return View(vm);
        }
    }
}
