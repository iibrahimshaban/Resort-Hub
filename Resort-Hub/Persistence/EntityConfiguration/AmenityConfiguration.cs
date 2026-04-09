namespace Resort_Hub.Persistence.EntityConfiguration;

public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
{
    public void Configure(EntityTypeBuilder<Amenity> builder)
    {
        builder.Property(a => a.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(300);

        builder.Property(a => a.Icon)
            .HasMaxLength(100);
    }
}
