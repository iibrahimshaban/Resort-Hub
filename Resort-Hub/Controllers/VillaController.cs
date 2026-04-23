using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Interfaces;
using Resort_Hub.Services;
using Resort_Hub.ViewModels.Villa;

namespace Resort_Hub.Controllers;

public class VillaController(
    IUnitOfWork unitOfWork,
    IVillaService villaService,
    IWebHostEnvironment env) : Controller
{
    public IActionResult Index()
    {
        var villas = unitOfWork.Villas.GetAll();
        return View(villas);
    }

    public IActionResult Create()
    {
        var vm = new VillaFormVM
        {
            AvailableAmenities = unitOfWork.Amenities.GetAll()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VillaFormVM vm)
    {
        if (!ModelState.IsValid)
        {
            vm.AvailableAmenities = unitOfWork.Amenities.GetAll();
            return View(vm);
        }

        var villa = new Villa
        {
            Name = vm.Name,
            Description = vm.Description,
            PricePerNight = vm.PricePerNight,
            Sqft = vm.Sqft,
            Capacity = vm.Capacity,
            IsAvilable = vm.IsAvilable,
            CreatedDate = DateTime.Now
        };

        // Amenities
        foreach (var amenityId in vm.SelectedAmenityIds)
            villa.VillaAmenity.Add(new VillaAmenity { AmenityId = amenityId });

        // Images
        if (vm.NewImages != null && vm.NewImages.Count > 0)
        {
            int order = 1;
            foreach (var img in vm.NewImages)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(img.FileName)}";
                var folder = Path.Combine(env.WebRootPath, "images", "villas");
                Directory.CreateDirectory(folder);

                using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
                await img.CopyToAsync(stream);

                villa.VillaImages.Add(new VillaImage
                {
                    ImageUrl = $"/images/villas/{fileName}",
                    IsMain = order == 1,
                    DispalayOrder = order++
                });
            }
        }

        unitOfWork.Villas.Add(villa);
        await unitOfWork.SaveAsync();

        TempData["Success"] = "Villa created successfully!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await villaService.ValidateVilla(id);
        if (result.IsFailur) return NotFound();

        var villa = result.Value;
        var vm = new VillaFormVM
        {
            Id = villa.Id,
            Name = villa.Name,
            Description = villa.Description,
            PricePerNight = villa.PricePerNight,
            Sqft = villa.Sqft,
            Capacity = villa.Capacity,
            IsAvilable = villa.IsAvilable,
            AvailableAmenities = unitOfWork.Amenities.GetAll(),
            SelectedAmenityIds = villa.VillaAmenity.Select(va => va.AmenityId).ToList(),
            ExistingImages = villa.VillaImages.ToList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VillaFormVM vm)
    {
        if (!ModelState.IsValid)
        {
            vm.AvailableAmenities = unitOfWork.Amenities.GetAll();
            vm.ExistingImages = unitOfWork.Villas.GetById(id)?.VillaImages.ToList() ?? [];
            return View(vm);
        }

        var result = await villaService.GetVillaForEdit(id);
        if (result.IsFailur) return NotFound();

        var villa = result.Value;
        villa.Name = vm.Name;
        villa.Description = vm.Description;
        villa.PricePerNight = vm.PricePerNight;
        villa.Sqft = vm.Sqft;
        villa.Capacity = vm.Capacity;
        villa.IsAvilable = vm.IsAvilable;
        villa.UpdatedDate = DateTime.Now;


        villa.VillaAmenity.Clear();
        foreach (var amenityId in vm.SelectedAmenityIds)
            villa.VillaAmenity.Add(new VillaAmenity { VillaId = id, AmenityId = amenityId });

       
        if (vm.NewImages != null && vm.NewImages.Count > 0)
        {
            int order = villa.VillaImages.Count + 1;
            foreach (var img in vm.NewImages)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(img.FileName)}";
                var folder = Path.Combine(env.WebRootPath, "images", "villas");
                Directory.CreateDirectory(folder);

                using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
                await img.CopyToAsync(stream);

                villa.VillaImages.Add(new VillaImage
                {
                    VillaId = id,
                    ImageUrl = $"/images/villas/{fileName}",
                    IsMain = !villa.VillaImages.Any(i => i.IsMain),
                    DispalayOrder = order++
                });
            }
        }

        unitOfWork.Villas.Update(villa);
        await unitOfWork.SaveAsync();

        TempData["Success"] = "Villa updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await villaService.ValidateVilla(id);
        if (result.IsFailur) return NotFound();
        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await villaService.ValidateVilla(id);
        if (result.IsFailur) return NotFound();

        unitOfWork.Villas.Delete(result.Value);
        await unitOfWork.SaveAsync();

        TempData["Success"] = "Villa deleted successfully!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await villaService.ValidateVilla(id);
        if (result.IsFailur) return NotFound();
        return View(result.Value);
    }
}
