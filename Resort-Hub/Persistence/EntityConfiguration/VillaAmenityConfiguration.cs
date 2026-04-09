namespace Resort_Hub.Persistence.EntityConfiguration;

public class VillaAmenityConfiguration : IEntityTypeConfiguration<VillaAmenity>
{
    public void Configure(EntityTypeBuilder<VillaAmenity> builder)
    {
        builder.HasKey(va => new { va.VillaId, va.AmenityId });
    }
}
