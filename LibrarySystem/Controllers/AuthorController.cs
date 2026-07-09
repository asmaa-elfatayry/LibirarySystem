using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using LibrarySystem.web.ViewModels.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.web.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class AuthorController(IGenericService<Author> _authorService, IMapper _mapper) : Controller
{
    public async Task<IActionResult> Index()
    {
        var authors = await _authorService.GetAllAsync();
        var viewModels = _mapper.Map<List<AuthorViewModel>>(authors);
        return View(viewModels);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuthorViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var author = _mapper.Map<Author>(vm);
        await _authorService.CreateAsync(author);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var author = await _authorService.GetByIdAsync(id);
        if (author == null) return NotFound();

        var vm = _mapper.Map<AuthorViewModel>(author);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AuthorViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var author = _mapper.Map<Author>(vm);
        var success = await _authorService.UpdateAsync(author);
        if (!success) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAjax(Guid id)
    {
        var success = await _authorService.DeleteAsync(id);
        if (!success)
            return Json(new { success = false, message = "المؤلف مش موجود" });

        return Json(new { success = true });
    }
}
