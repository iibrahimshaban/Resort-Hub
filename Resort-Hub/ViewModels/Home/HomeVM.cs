namespace Resort_Hub.ViewModels.Home;

public class HomeVM
{
    public IEnumerable<VillaVM> MostPickedVillas { get; set; }
    public IEnumerable<VillaVM> PopularVillas { get; set; }

    public DateOnly? CheckInDate { get; set; }
    public int PersonCount { get; set; }
    public string? Location { get; set; }

    public int TotalUsers { get; set; }
    public int TotalProperties { get; set; }
    public int TotalVillas { get; set; }
}
