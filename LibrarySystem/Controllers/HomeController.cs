using LibrarySystem.Models;
using LibrarySystem.web.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibrarySystem.Controllers
{
    public class HomeController(IGenericService<Category> _categoryService, IGenericService<Book> _bookService, UserManager<ApplicationUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var activeUsersCount = _userManager.Users.Count();
            var vm = new HomeVM()
            {
                BookCount=books.Count(),
                CategoriesCount=categories.Count(),
                ActiveUsersCount=activeUsersCount
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public IActionResult StatusCode(int code)
        {
            ViewBag.StatusCode = code;
            return View();
        }
    }
}
