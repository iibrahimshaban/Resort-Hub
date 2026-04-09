namespace Resort_Hub.Persistence.EntityConfiguration;

public class VillaImageConfiguration : IEntityTypeConfiguration<VillaImage>
{
    public void Configure(EntityTypeBuilder<VillaImage> builder)
    {
        builder.Property(vi => vi.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();
    }
}
