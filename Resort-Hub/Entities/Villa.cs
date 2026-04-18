namespace Resort_Hub.Entities;

public class Villa
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public double PricePerNight { get; set; }
    public int Sqft { get; set; }
    public int Capacity { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsAvilable { get; set; } = true;
    public virtual ICollection<VillaAmenity> VillaAmenity { get; set; } = new HashSet<VillaAmenity>();
    public virtual ICollection<VillaImage> VillaImages { get; set; } = new HashSet<VillaImage>();
   
}
