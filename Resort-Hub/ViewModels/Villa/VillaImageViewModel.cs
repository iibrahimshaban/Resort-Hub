namespace Resort_Hub.ViewModels.Villa
{
    public class VillaImageViewModel
    {
        public required string ImageUrl { get; set; }
        public bool IsMain { get; set; } = false;
        public int DispalayOrder { get; set; }
    }
}
