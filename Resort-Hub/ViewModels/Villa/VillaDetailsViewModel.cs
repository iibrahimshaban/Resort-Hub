namespace Resort_Hub.ViewModels.Villa
{
    public class VillaDetailsViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public double PricePerNight { get; set; }
        public List<VillaAmenityViewModel> VillaAmenities { get; set; } = new();
        public List<VillaImageViewModel> VillaImages { get; set; } = new();
        public bool IsAvilable { get; set; }
    }
}
