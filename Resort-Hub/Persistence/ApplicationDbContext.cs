using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Resort_Hub.Entities;
using System.Reflection;

namespace Resort_Hub.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
    public virtual DbSet<Villa> Villas { get; set; } 
    public virtual DbSet<Amenity> Amenities { get; set; }
    public virtual DbSet<VillaAmenity> VillaAmenities { get; set; }
    public virtual DbSet<Booking> Bookings { get; set; }
    public virtual DbSet<VillaImage> VillaImages { get; set; }
    public virtual DbSet<OtpEntry> OtpEntries { get; set; }
}
