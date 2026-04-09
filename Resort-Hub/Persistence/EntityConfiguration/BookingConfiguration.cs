namespace Resort_Hub.Persistence.EntityConfiguration;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(b => b.TotalCost)
            .HasColumnType("decimal(18,2)");

        builder.Ignore(b => b.Nights);
    }
}
