using Microsoft.AspNetCore.Mvc;
using Resort_Hub.Interfaces;
using Resort_Hub.Services;
using Resort_Hub.ViewModels.Villa;

namespace Resort_Hub.Controllers;

public class VillaController(IUnitOfWork unitOfWork,IVillaService villaService, ICloudinaryService cloudinaryService) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IVillaService _villaService = villaService;
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
    
    public IActionResult Index()
    {
        var villas = _unitOfWork.Villas.GetAll();
        return View(villas);
    }

    public IActionResult Create()
    {
        var vm = new VillaFormVM
        {
            AvailableAmenities = _unitOfWork.Amenities.GetAll()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VillaFormVM vm)
    {
        if (!ModelState.IsValid)
        {
            vm.AvailableAmenities = _unitOfWork.Amenities.GetAll();
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

        // save villa 
        _unitOfWork.Villas.Add(villa);
        await unitOfWork.SaveAsync();

        // upload img to cloude
        if (vm.NewImages != null && vm.NewImages.Count > 0)
        {
            int order = 1;
            foreach (var img in vm.NewImages)
            {
                var publicId = $"resort-hub/villas/villa_{villa.Id}_{order}";
                var imageUrl = await _cloudinaryService.UploadImageAsync(img, "resort-hub/villas", publicId);

                villa.VillaImages.Add(new VillaImage
                {
                    VillaId = villa.Id,
                    ImageUrl = imageUrl,
                    IsMain = (order - 1) == vm.MainImageIndex,
                    DispalayOrder = order++
                });
            }

            _unitOfWork.Villas.Update(villa);
            await _unitOfWork.SaveAsync();
        }

        TempData["Success"] = "Villa created successfully!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _villaService.ValidateVilla(id);
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
            AvailableAmenities = _unitOfWork.Amenities.GetAll(),
            SelectedAmenityIds = villa.VillaAmenity.Select(va => va.AmenityId).ToList(),
            ExistingImages = villa.VillaImages.OrderBy(i => i.DispalayOrder).ToList(),
            MainExistingImageId = villa.VillaImages.FirstOrDefault(i => i.IsMain)?.Id
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VillaFormVM vm)
    {
        if (!ModelState.IsValid)
        {
            vm.AvailableAmenities = _unitOfWork.Amenities.GetAll();
            vm.ExistingImages = _unitOfWork.Villas.GetById(id)?.VillaImages
                                        .OrderBy(i => i.DispalayOrder).ToList() ?? [];
            return View(vm);
        }

        var result = await _villaService.GetVillaForEdit(id);
        if (result.IsFailur) return NotFound();

        var villa = result.Value;
        villa.Name = vm.Name;
        villa.Description = vm.Description;
        villa.PricePerNight = vm.PricePerNight;
        villa.Sqft = vm.Sqft;
        villa.Capacity = vm.Capacity;
        villa.IsAvilable = vm.IsAvilable;
        villa.UpdatedDate = DateTime.Now;

        // Amenities
        villa.VillaAmenity.Clear();
        foreach (var amenityId in vm.SelectedAmenityIds)
            villa.VillaAmenity.Add(new VillaAmenity { VillaId = id, AmenityId = amenityId });

        // delete img - Cloudinary & DB
        if (vm.DeletedImageIds.Any())
        {
            foreach (var imageId in vm.DeletedImageIds)
            {
                var image = villa.VillaImages.FirstOrDefault(i => i.Id == imageId);
                if (image != null)
                {
                    // delete from Cloudinary
                    var order = image.DispalayOrder;
                    var publicId = $"resort-hub/villas/villa_{id}_{order}";
                    await cloudinaryService.DeleteImageAsync(publicId);

                    // delete from DB
                    villa.VillaImages.Remove(image);
                }
            }
        }

        // update main
        if (vm.MainExistingImageId.HasValue)
        {
            foreach (var image in villa.VillaImages)
                image.IsMain = image.Id == vm.MainExistingImageId.Value;
        }

        // uploade new img - Cloudinary
        if (vm.NewImages != null && vm.NewImages.Count > 0)
        {
            int order = villa.VillaImages.Count + 1;
            foreach (var img in vm.NewImages)
            {
                var publicId = $"resort-hub/villas/villa_{id}_{order}";
                var imageUrl = await cloudinaryService.UploadImageAsync(img, "resort-hub/villas", publicId);

                villa.VillaImages.Add(new VillaImage
                {
                    VillaId = id,
                    ImageUrl = imageUrl,
                    IsMain = false,
                    DispalayOrder = order++
                });
            }
        }

        _unitOfWork.Villas.Update(villa);
        await _unitOfWork.SaveAsync();

        TempData["Success"] = "Villa updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await _villaService.ValidateVilla(id);
        if (result.IsFailur) return NotFound();
        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _villaService.ValidateVilla(id);
        if (result.IsFailur) return NotFound();

        var villa = result.Value;

        // delete from Cloudinary
        int order = 1;
        foreach (var image in villa.VillaImages)
        {
            var publicId = $"resort-hub/villas/villa_{id}_{order++}";
            await cloudinaryService.DeleteImageAsync(publicId);
        }

        _unitOfWork.Villas.Delete(villa);
        await _unitOfWork.SaveAsync();

        TempData["Success"] = "Villa deleted successfully!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _villaService.ValidateVilla(id);
        if (result.IsFailur) return NotFound();
        return View(result.Value);
    }


    public async Task<IActionResult> Info([FromRoute] int id)
    {
            var villaResult = await _villaService.GetAllVillaData(id);

            if (!villaResult.IsSuccess)
            {
                TempData.SetError(villaResult.Error);
                return RedirectToAction("Index", "Home");
            }

            return View(villaResult.Value.Adapt<VillaDetailsViewModel>());    
    }
}