using MapsterMapper;
﻿using CloudinaryDotNet;
using Hangfire;
using Resort_Hub.Interfaces;
using Resort_Hub.Persistence;
using Resort_Hub.Repositories;
using Resort_Hub.Services;
using Resort_Hub.Services.Book;
using Resort_Hub.Settings;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Resort_Hub;

public static class DependacyInjection
{
    public static IServiceCollection AddAllDependacies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();
        services.AddRazorPages();

        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

        services
            .AddDbContextConfiguration(configuration)
            .AddMapsterConfig()
            .AddRepositoryServices()
            .AddGoogleAuthentication(configuration)
            .AddSession()     
            .AddCloudinaryImageHosting(configuration)
            .AddHangfireBGJobs(configuration);

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
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/Login";
        });

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
        services.AddScoped<IBookingService,BookingService>();
        services.AddScoped<IAuthService,AuthService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddTransient<ICustomEmailService, EmailService>();

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

    public static IServiceCollection AddSession(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSession().AddSession(options=>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
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
  
    public static IServiceCollection AddHangfireBGJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        services.AddHangfireServer();

        return services;
    }
}
