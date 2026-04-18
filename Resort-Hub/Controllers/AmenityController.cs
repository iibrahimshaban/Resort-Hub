using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Entities;
using Resort_Hub.Interfaces;

namespace Resort_Hub.Controllers;

public class AmenityController(IUnitOfWork unitOfWork) : Controller
{
    public IActionResult Index()
    {
        return View(unitOfWork.Amenities.GetAll());
    }

    public IActionResult Create() => View(new Amenity());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Amenity amenity)
    {
        if (!ModelState.IsValid) return View(amenity);

        unitOfWork.Amenities.Add(amenity);
        await unitOfWork.SaveAsync();

        TempData["Success"] = "Amenity created!";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var amenity = unitOfWork.Amenities.GetById(id);
        if (amenity is null) return NotFound();
        return View(amenity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Amenity amenity)
    {
        if (!ModelState.IsValid) return View(amenity);

        unitOfWork.Amenities.Update(amenity);
        await unitOfWork.SaveAsync();

        TempData["Success"] = "Amenity updated!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var amenity = unitOfWork.Amenities.GetById(id);
        if (amenity is null) return NotFound();

        unitOfWork.Amenities.Delete(amenity);
        await unitOfWork.SaveAsync();

        TempData["Success"] = "Amenity deleted!";
        return RedirectToAction(nameof(Index));
    }
}