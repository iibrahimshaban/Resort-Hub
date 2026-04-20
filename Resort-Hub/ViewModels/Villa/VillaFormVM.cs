using Resort_Hub.Entities;
using System.ComponentModel.DataAnnotations;

namespace Resort_Hub.ViewModels.Villa;

public class VillaFormVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public double PricePerNight { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sqft must be greater than 0")]
    public int Sqft { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
    public int Capacity { get; set; }

    public bool IsAvilable { get; set; } = true;

    public List<int> SelectedAmenityIds { get; set; } = [];
    public List<Amenity> AvailableAmenities { get; set; } = [];

    public List<IFormFile>? NewImages { get; set; }
    public List<VillaImage> ExistingImages { get; set; } = [];
}
