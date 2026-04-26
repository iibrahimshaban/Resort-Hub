namespace Resort_Hub.Entities;

public class VillaImage
{
    public int Id { get; set; }
    public int VillaId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; } = false;
    public int DispalayOrder { get; set; }
    public Villa Villa { get; set; } = null!;
    
}
