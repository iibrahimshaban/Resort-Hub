using Resort_Hub.Abstraction.Consts;

namespace Resort_Hub.Persistence.EntityConfiguration;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(new List<IdentityRole> {
            new IdentityRole {
                Id = DefaultRoles.Admin.Id,
                Name =  DefaultRoles.Admin.Name,
                NormalizedName = DefaultRoles.Admin.Name.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Admin.ConcurrencyStamp
                },
            new IdentityRole {
                Id = DefaultRoles.Customer.Id,
                Name =  DefaultRoles.Customer.Name,
                NormalizedName = DefaultRoles.Customer.Name.ToUpper(),
                ConcurrencyStamp = DefaultRoles.Customer.ConcurrencyStamp
                }
        });
    }
}
