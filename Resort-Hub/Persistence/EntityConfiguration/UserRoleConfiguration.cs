using Resort_Hub.Abstraction.Consts;

namespace Resort_Hub.Persistence.EntityConfiguration;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(new IdentityUserRole<string>
        {
            RoleId = DefaultRoles.Admin.Id,
            UserId = DefaultUsers.AdminId
        });
    }
}
