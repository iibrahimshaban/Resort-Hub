namespace Resort_Hub.Persistence.EntityConfiguration;

public class VillaConfiguration : IEntityTypeConfiguration<Villa>
{
    public void Configure(EntityTypeBuilder<Villa> builder)
    {
        builder.Property(v => v.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(v => v.Description)
            .HasMaxLength(500);

        builder.Property(v => v.PricePerNight)
            .HasColumnType("decimal(18,2)");
    }
}
