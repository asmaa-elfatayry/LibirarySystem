using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using LibrarySystem.web.ViewModels.Publisher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.web.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class PublisherController(IGenericService<Publisher> _publisherService, IMapper _mapper) : Controller
{
    public async Task<IActionResult> Index()
    {
        var publishers = await _publisherService.GetAllAsync();
        var viewModels = _mapper.Map<List<PublisherViewModel>>(publishers);
        return View(viewModels);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PublisherViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var publisher = _mapper.Map<Publisher>(vm);
        await _publisherService.CreateAsync(publisher);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var publisher = await _publisherService.GetByIdAsync(id);
        if (publisher == null) return NotFound();

        var vm = _mapper.Map<PublisherViewModel>(publisher);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, PublisherViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var publisher = _mapper.Map<Publisher>(vm);
        var success = await _publisherService.UpdateAsync(publisher);
        if (!success) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAjax(Guid id)
    {
        var success = await _publisherService.DeleteAsync(id);
        if (!success)
            return Json(new { success = false, message = "الناشر مش موجود" });

        return Json(new { success = true });
    }
}
