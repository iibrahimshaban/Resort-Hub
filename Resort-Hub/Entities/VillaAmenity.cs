namespace Resort_Hub.Entities;

public class VillaAmenity
{
    public int VillaId { get; set; }
    public int AmenityId { get; set; }
    public Villa Villa { get; set; } = default!;
    public Amenity Amenity { get; set; } = default!;
}
