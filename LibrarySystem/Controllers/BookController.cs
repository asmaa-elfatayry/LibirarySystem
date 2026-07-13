using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using LibrarySystem.web.ViewModels.Book;
using LibrarySystem.Web.ViewModels.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibrarySystem.Web.Controllers
{
    [Authorize]
    public class BookController(
      IBookService _bookService,
      IAuthorService _authorService,
      ICategoryService _categoryService,
      IPublisherService _publisherService,
      IGenericService<BookCopy> _bookCopyService,
      IMapper _mapper) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllAsync();
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

            var bookDto = _mapper.Map<Application.DTOs.BookDto>(vm);
            bookDto.CoverImageUrl = await SaveCoverImageAsync(vm.CoverImageFile);

            var result = await _bookService.CreateAsync(bookDto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await PopulateDropdownsAsync(vm);
                return View(vm);
            }
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

            var dto = _mapper.Map<Application.DTOs.BookDto>(vm);

            if (vm.CoverImageFile != null)
                dto.CoverImageUrl = await SaveCoverImageAsync(vm.CoverImageFile);
            else
                dto.CoverImageUrl = vm.CoverImageUrl;

            var res = await _bookService.UpdateAsync(dto);
            if (!res.IsSuccess)
            {
                if (res.Status == Domain.Enums.eResultStatus.NotFound) return NotFound();
                ModelState.AddModelError(string.Empty, res.Message);
                await PopulateDropdownsAsync(vm);
                return View(vm);
            }

            TempData["Success"] = res.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAjax(Guid id)
        {
            var res = await _bookService.DeleteAsync(id);
            if (!res.IsSuccess)
                return Json(new { success = false, message = res.Message });

            return Json(new { success = true });
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();

            var vm = _mapper.Map<BookViewModel>(book);

            var allCopies = await _bookCopyService.GetAllAsync();
            var bookCopies = allCopies.Where(c => c.BookId == id).ToList();
            vm.Copies = _mapper.Map<List<BookCopyViewModel>>(bookCopies);

            vm.HasAvailableCopy = bookCopies.Any(c => c.Status == eCopyStatus.Available);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCopy(Guid bookId, string copyNumber)
        {
            if (string.IsNullOrWhiteSpace(copyNumber))
                return Json(new { success = false, message = "رقم النسخة مطلوب" });

            var allCopies = await _bookCopyService.GetAllAsync();
            var isDuplicate = allCopies.Any(c =>
                c.BookId == bookId &&
                c.CopyNumber.Trim().Equals(copyNumber.Trim(), StringComparison.OrdinalIgnoreCase));

            if (isDuplicate)
                return Json(new { success = false, message = $"رقم النسخة \"{copyNumber}\" موجود بالفعل لهذا الكتاب" });

            var copy = new Domain.Entities.BookCopy
            {
                BookId = bookId,
                CopyNumber = copyNumber.Trim(),
                Status = eCopyStatus.Available
            };

            var success = await _bookCopyService.CreateAsync(copy);
            return Json(new { success });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCopyStatusAjax(Guid id, Domain.Enums.eCopyStatus status)
        {
            var copy = await _bookCopyService.GetByIdAsync(id);
            if (copy == null) return Json(new { success = false, message = "النسخة مش موجودة" });

            copy.Status = status;
            var success = await _bookCopyService.UpdateAsync(copy);
            return Json(new { success });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCopyAjax(Guid id)
        {
            var success = await _bookCopyService.DeleteAsync(id);
            return Json(new { success });
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