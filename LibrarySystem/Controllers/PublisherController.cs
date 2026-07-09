using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using LibrarySystem.web.ViewModels.Publisher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.web.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class PublisherController(IPublisherService _publisherService, IMapper _mapper) : Controller
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

        var dto = _mapper.Map<Application.DTOs.PublisherDto>(vm);
        var res = await _publisherService.CreateAsync(dto);
        if (!res.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, res.Message);
            return View(vm);
        }

        TempData["Success"] = res.Message;
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

        var dto = _mapper.Map<Application.DTOs.PublisherDto>(vm);
        var res = await _publisherService.UpdateAsync(dto);
        if (!res.IsSuccess)
        {
            if (res.Status == Domain.Enums.eResultStatus.NotFound) return NotFound();
            ModelState.AddModelError(string.Empty, res.Message);
            return View(vm);
        }

        TempData["Success"] = res.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAjax(Guid id)
    {
        var res = await _publisherService.DeleteAsync(id);
        if (!res.IsSuccess)
            return Json(new { success = false, message = res.Message });

        return Json(new { success = true });
    }
}
