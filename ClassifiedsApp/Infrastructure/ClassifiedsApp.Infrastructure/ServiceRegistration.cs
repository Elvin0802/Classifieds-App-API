﻿using Azure.Storage.Blobs;
using ClassifiedsApp.Application.Dtos.Cache;
using ClassifiedsApp.Application.Dtos.Encryption;
using ClassifiedsApp.Application.Dtos.RateLimit;
using ClassifiedsApp.Application.Interfaces.Repositories.AdImages;
using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using ClassifiedsApp.Application.Interfaces.Repositories.Categories;
using ClassifiedsApp.Application.Interfaces.Repositories.Chats;
using ClassifiedsApp.Application.Interfaces.Repositories.Locations;
using ClassifiedsApp.Application.Interfaces.Repositories.Reports;
using ClassifiedsApp.Application.Interfaces.Repositories.Users;
using ClassifiedsApp.Application.Interfaces.Services.Ads;
using ClassifiedsApp.Application.Interfaces.Services.Auth;
using ClassifiedsApp.Application.Interfaces.Services.BlackList;
using ClassifiedsApp.Application.Interfaces.Services.Cache;
using ClassifiedsApp.Application.Interfaces.Services.Common;
using ClassifiedsApp.Application.Interfaces.Services.Mail;
using ClassifiedsApp.Application.Interfaces.Services.RateLimit;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Infrastructure.Persistence.Context;
using ClassifiedsApp.Infrastructure.Persistence.Repositories.AdImages;
using ClassifiedsApp.Infrastructure.Persistence.Repositories.Ads;
using ClassifiedsApp.Infrastructure.Persistence.Repositories.Categories;
using ClassifiedsApp.Infrastructure.Persistence.Repositories.Chats;
using ClassifiedsApp.Infrastructure.Persistence.Repositories.Locations;
using ClassifiedsApp.Infrastructure.Persistence.Repositories.Reports;
using ClassifiedsApp.Infrastructure.Persistence.Repositories.Users;
using ClassifiedsApp.Infrastructure.Services.Ads;
using ClassifiedsApp.Infrastructure.Services.Auth;
using ClassifiedsApp.Infrastructure.Services.BackgroundJobs;
using ClassifiedsApp.Infrastructure.Services.BlackList;
using ClassifiedsApp.Infrastructure.Services.Cache;
using ClassifiedsApp.Infrastructure.Services.Encryption;
using ClassifiedsApp.Infrastructure.Services.Mail;
using ClassifiedsApp.Infrastructure.Services.RateLimit;
using ClassifiedsApp.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ClassifiedsApp.Infrastructure;

public static class ServiceRegistration
{
	public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ApplicationDbContext>(options =>
		{
			options.UseSqlServer(Configuration.ConnectionString);
		});

		services.AddIdentity<AppUser, AppRole>(options =>
		{
			options.Password.RequireDigit = true;
			options.Password.RequireLowercase = true;
			options.Password.RequireUppercase = true;
			options.Password.RequireNonAlphanumeric = true;
			options.Password.RequiredLength = 6;

			options.User.RequireUniqueEmail = true;
		})
		.AddEntityFrameworkStores<ApplicationDbContext>()
		.AddDefaultTokenProviders();

		services.AddSingleton(x => new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage")));

		services.Configure<RateLimitOptionsDto>(configuration.GetSection("RateLimit"));

		// caching usage

		var cacheConfig = new CacheConfigDto();

		configuration.Bind("RedisCache", cacheConfig);

		services.AddSingleton(cacheConfig);

		if (configuration.GetValue<bool>("RedisEnabled"))
		{
			services.AddSingleton<IConnectionMultiplexer>(sp =>
					ConnectionMultiplexer.Connect(configuration["RedisCache:Configuration"]!));

			services.AddStackExchangeRedisCache(options =>
			{
				options.Configuration = configuration["RedisCache:Configuration"];
				options.InstanceName = configuration["RedisCache:InstanceName"];
			});
		}
		else
		{
			// If Redis is disabled, use in-memory cache
			services.AddDistributedMemoryCache();
		}

		services.AddScoped<ICacheService, RedisCacheService>();

		services.AddScoped<IBlacklistService, BlacklistService>();

		services.Configure<EncryptionSettingsDto>(configuration.GetSection("Encryption"));
		services.AddScoped<IEncryptionService, EncryptionService>();

		services.AddScoped<IAdReadRepository, AdReadRepository>();
		services.AddScoped<IAdWriteRepository, AdWriteRepository>();

		services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
		services.AddScoped<ICategoryWriteRepository, CategoryWriteRepository>();

		services.AddScoped<ILocationReadRepository, LocationReadRepository>();
		services.AddScoped<ILocationWriteRepository, LocationWriteRepository>();

		services.AddScoped<IAdImageReadRepository, AdImageReadRepository>();
		services.AddScoped<IAdImageWriteRepository, AdImageWriteRepository>();

		services.AddScoped<IMainCategoryReadRepository, MainCategoryReadRepository>();
		services.AddScoped<IMainCategoryWriteRepository, MainCategoryWriteRepository>();

		services.AddScoped<IAdSubCategoryValueReadRepository, AdSubCategoryValueReadRepository>();
		services.AddScoped<IAdSubCategoryValueWriteRepository, AdSubCategoryValueWriteRepository>();

		services.AddScoped<IMainCategoryReadRepository, MainCategoryReadRepository>();
		services.AddScoped<IMainCategoryWriteRepository, MainCategoryWriteRepository>();

		services.AddScoped<ISubCategoryReadRepository, SubCategoryReadRepository>();
		services.AddScoped<ISubCategoryWriteRepository, SubCategoryWriteRepository>();

		services.AddScoped<ISubCategoryOptionReadRepository, SubCategoryOptionReadRepository>();
		services.AddScoped<ISubCategoryOptionWriteRepository, SubCategoryOptionWriteRepository>();

		services.AddScoped<IMainCategoryReadRepository, MainCategoryReadRepository>();
		services.AddScoped<IMainCategoryWriteRepository, MainCategoryWriteRepository>();

		services.AddScoped<IUserAdSelectionWriteRepository, UserAdSelectionWriteRepository>();
		services.AddScoped<IUserAdSelectionReadRepository, UserAdSelectionReadRepository>();

		services.AddScoped<IFeaturedAdTransactionReadRepository, FeaturedAdTransactionReadRepository>();
		services.AddScoped<IFeaturedAdTransactionWriteRepository, FeaturedAdTransactionWriteRepository>();

		services.AddScoped<IMailService, MailService>();

		services.AddScoped<IFeaturedAdService, FeaturedAdService>();

		services.AddScoped<IReportReadRepository, ReportReadRepository>();
		services.AddScoped<IReportWriteRepository, ReportWriteRepository>();

		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<ICurrentUserService, CurrentUserService>();

		services.AddScoped<IChatMessageReadRepository, ChatMessageReadRepository>();
		services.AddScoped<IChatMessageWriteRepository, ChatMessageWriteRepository>();

		services.AddScoped<IChatRoomReadRepository, ChatRoomReadRepository>();
		services.AddScoped<IChatRoomWriteRepository, ChatRoomWriteRepository>();

		services.AddScoped<IAdImageService, AdImageService>();

		services.AddSingleton<IRateLimitService, RateLimitService>(); // rate limit.

		services.AddScoped<TestJobService>(); // add test job service.
		services.AddScoped<RateLimitResetJobService>(); // add rate limit job service.
		services.AddScoped<FeaturedAdResetJobService>(); // add reset ad feature job service.


	}

}
