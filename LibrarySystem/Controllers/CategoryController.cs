using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class CategoryController(IGenericService<Category> _categoryService) : Controller
{
    // GET: Category
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }

    // GET: Category/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid)
            return View(category);

        await _categoryService.CreateAsync(category);
        return RedirectToAction(nameof(Index));
    }

    // GET: Category/Edit/{id}
    public async Task<IActionResult> Edit(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();

        return View(category);
    }

    // POST: Category/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Category category)
    {
        if (id != category.Id) return BadRequest();
        if (!ModelState.IsValid) return View(category);

        var success = await _categoryService.UpdateAsync(category);
        if (!success) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    // GET: Category/Delete/{id}
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();

        return View(category);
    }

    // POST: Category/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _categoryService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}