using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resort_Hub.Entities;
using Resort_Hub.Persistence;
using System.Reflection;

namespace Resort_Hub;

public static class DependacyInjection
{
    public static IServiceCollection AddAllDependacies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();
        services.AddRazorPages();

        services
            .AddDbContextConfiguration(configuration)
            .AddMapsterConfig();

        return services;
    }
    public static IServiceCollection AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


        services.AddIdentity<ApplicationUser,IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var MappingConfig = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(MappingConfig));

        return services;
    }
}
