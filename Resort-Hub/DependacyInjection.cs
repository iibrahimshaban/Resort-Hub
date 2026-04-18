using CloudinaryDotNet;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resort_Hub.Entities;
using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;
using Resort_Hub.Repositories;
using Resort_Hub.Services;
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
            .AddMapsterConfig()
            .AddRepositoryServices()
            .AddGoogleAuthentication(configuration)
            .AddCloudinaryImageHosting(configuration);

        return services;
    }
    public static IServiceCollection AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


        services.AddIdentity<ApplicationUser,IdentityRole>(opt =>
        {
            opt.Password.RequireDigit = false;
            opt.Password.RequiredLength = 6;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireNonAlphanumeric = false;
        })
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
    private static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IVillaService,VillaService>();
        services.AddScoped<IAuthService,AuthService>();
        services.AddScoped<IAccountService, AccountService>();

        return services;
    }
    public static IServiceCollection AddGoogleAuthentication(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"];
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"];

                options.Scope.Add("profile");
                options.Scope.Add("email");
            });
        return services;
    }
    public static IServiceCollection AddCloudinaryImageHosting(this IServiceCollection services, IConfiguration configuration)
    {
        var cloudinarySettings = configuration.GetSection("Cloudinary");

        var account = new Account(
            cloudinarySettings["CloudName"],
            cloudinarySettings["ApiKey"],
            cloudinarySettings["ApiSecret"]
        );
        services.AddSingleton(new Cloudinary(account));

        services.AddScoped<ICloudinaryService, CloudinaryService>();
        return services;
    }
}
