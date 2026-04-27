namespace Resort_Hub.ViewModels.Home;

public class VillaVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public double PricePerNight { get; set; }
    public int Capacity { get; set; }         // for Person filter
    public bool IsAvailable { get; set; }
    public bool IsPopularChoice { get; set; } // for the "Popular Choice" badge
    public string? Location { get; set; }

    public string? MainImageUrl { get; set; }
}
