namespace Resort_Hub.Entities;

public class Amenity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public virtual ICollection<VillaAmenity> VillaAmenities { get; set; } = new HashSet<VillaAmenity>();

}
