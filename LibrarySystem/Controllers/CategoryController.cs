using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using LibrarySystem.Web.ViewModels;
using LibrarySystem.Web.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class CategoryController(IGenericService<Category> _categoryService, IMapper _mapper) : Controller
{
    // GET: Category
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        var viewModels = _mapper.Map<List<CategoryViewModel>>(categories);
        return View(viewModels);
    }

    // GET: Category/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var category = _mapper.Map<Category>(vm);
        await _categoryService.CreateAsync(category);
        return RedirectToAction(nameof(Index));
    }

    // GET: Category/Edit/{id}
    public async Task<IActionResult> Edit(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();

        var vm = _mapper.Map<CategoryViewModel>(category);
        return View(vm);
    }

    // POST: Category/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CategoryViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var category = _mapper.Map<Category>(vm);
        var success = await _categoryService.UpdateAsync(category);
        if (!success) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    // POST: Category/DeleteAjax/{id}
    [HttpPost]
    public async Task<IActionResult> DeleteAjax(Guid id)
    {
        var success = await _categoryService.DeleteAsync(id);

        if (!success)
            return Json(new { success = false, message = "التصنيف مش موجود" });

        return Json(new { success = true });
    }
}