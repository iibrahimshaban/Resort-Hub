using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Services.Amenity;
using Resort_Hub.Services.FontAwesome;

namespace Resort_Hub.Controllers;

public class AmenityController : Controller
{
    private readonly IAmenityService _amenityService;
    private readonly IFontAwesomeService _fontAwesomeService;

    public AmenityController(IAmenityService amenityService, IFontAwesomeService fontAwesomeService)
    {
        _amenityService = amenityService;
        _fontAwesomeService = fontAwesomeService;
    }


    public IActionResult Index()
    {
        return View(_amenityService.GetAll());
    }

    public IActionResult Create()
    {
        return View(new Amenity());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Amenity amenity)
    {
        if (!ModelState.IsValid)
            return View(amenity);

        if (!string.IsNullOrEmpty(amenity.Icon))
            amenity.Icon = amenity.Icon.Replace("bi bi-", "fa-solid fa-");

        _amenityService.Add(amenity);
        await _amenityService.SaveChangesAsync();

        TempData.SetMessageModal(new ModalMessage("Success", "Amenity created successfully!", ModalType.Success));

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var amenity = _amenityService.GetById(id);

        if (amenity is null) 
            return NotFound();

        return View(amenity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Amenity amenity)
    {
        if (!ModelState.IsValid)
            return View(amenity);

        _amenityService.Update(amenity);
        await _amenityService.SaveChangesAsync();

        TempData.SetMessageModal(new ModalMessage("Success", "Amenity updated successfully!", ModalType.Success));

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var amenity = _amenityService.GetById(id);

        if (amenity is null) 
            return NotFound();

        _amenityService.Delete(amenity);
        await _amenityService.SaveChangesAsync();

        TempData.SetMessageModal(new ModalMessage("Success", "Amenity deleted successfully!", ModalType.Success));


        return RedirectToAction(nameof(Index));
    }



    public async Task<IActionResult> SearchIcons(string q)
    {
        var icons = await _fontAwesomeService.SearchIconsAsync(q);
        return Json(icons);
    }
}
