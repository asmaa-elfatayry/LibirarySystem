using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using LibrarySystem.Web.ViewModels.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibrarySystem.Web.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class BookController(
      IGenericService<Book> _bookService,
      IGenericService<Author> _authorService,
      IGenericService<Category> _categoryService,
      IGenericService<Publisher> _publisherService,
      IMapper _mapper) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllAsync(b => b.Author, b => b.Category, b => b.Publisher);
            var viewModels = _mapper.Map<List<BookViewModel>>(books);
            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new BookViewModel();
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(vm);
                return View(vm);
            }

            var book = _mapper.Map<Book>(vm);
            book.CoverImageUrl = await SaveCoverImageAsync(vm.CoverImageFile);

            await _bookService.CreateAsync(book);
            return RedirectToAction(nameof(Index));
        }





        public async Task<IActionResult> Edit(Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();

            var vm = _mapper.Map<BookViewModel>(book);
            await PopulateDropdownsAsync(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BookViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(vm);
                return View(vm);
            }

            var book = _mapper.Map<Book>(vm);

            if (vm.CoverImageFile != null)
                book.CoverImageUrl = await SaveCoverImageAsync(vm.CoverImageFile);
            else
                book.CoverImageUrl = vm.CoverImageUrl; // مفيش صورة جديدة اترفعت، خلي القديمة زي ما هي

            var success = await _bookService.UpdateAsync(book);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAjax(Guid id)
        {
            var success = await _bookService.DeleteAsync(id);
            if (!success)
                return Json(new { success = false, message = "الكتاب مش موجود" });

            return Json(new { success = true });
        }

        private async Task PopulateDropdownsAsync(BookViewModel vm)
        {
            var authors = await _authorService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var publishers = await _publisherService.GetAllAsync();

            vm.Authors = authors.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName });
            vm.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
            vm.Publishers = publishers.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name });
        }


        // دالة مساعدة خاصة، بتحفظ أي صورة مرفوعة وترجّع مسارها
        private async Task<string?> SaveCoverImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("نوع الصورة غير مدعوم");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "covers");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/covers/{fileName}";
        }
    }
}