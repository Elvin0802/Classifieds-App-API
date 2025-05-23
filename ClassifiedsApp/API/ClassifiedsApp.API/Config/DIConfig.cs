﻿using ClassifiedsApp.Application.Dtos.Auth.Token;
using ClassifiedsApp.Application.Interfaces.Services.Auth;
using ClassifiedsApp.Application.Services;
using ClassifiedsApp.Infrastructure.BackgroundJobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace ClassifiedsApp.API.Config;

public static class DIConfig
{
	public static IServiceCollection AddSwagger(this IServiceCollection services)
	{
		services.AddSwaggerGen(setup =>
		{
			setup.SwaggerDoc("v1",
				new OpenApiInfo
				{
					Title = "ClassifiedsApp API",
					Version = "v1.0"
				});

			setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
							 "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
							 "Example: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
			});

			setup.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					Array.Empty<string>()
				}
			});
		});

		return services;
	}

	public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<IdentityOptions>(options =>
		{
			options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
		});

		services.AddScoped<ITokenService, TokenService>();

		var jwtConfig = new JwtConfigDto();
		configuration.Bind("JWT", jwtConfig);
		services.AddSingleton(jwtConfig);

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.SaveToken = true;
			options.RequireHttpsMetadata = false;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ClockSkew = TimeSpan.Zero,
				ValidIssuer = jwtConfig.Issuer,
				ValidAudience = jwtConfig.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
				RoleClaimType = ClaimTypes.Role
			};

			options.Events = new JwtBearerEvents
			{
				OnMessageReceived = context =>
				{
					// Extract token from Authorization header for regular HTTP requests
					if (context.Request.Headers.ContainsKey("Authorization"))
					{
						var header = context.Request.Headers["Authorization"].ToString();

						if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
							context.Token = header.Substring("Bearer ".Length).Trim();
					}

					// Extract token from query string for SignalR WebSocket connections
					var accessToken = context.Request.Query["access_token"];

					// If the request is for the hub
					var path = context.HttpContext.Request.Path;

					Log.Warning($"SignalR context path , request path = {path}");
					Log.Warning($"Query for SignalR , context token = {accessToken}");

					if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/chatHub"))
					{
						context.Token = accessToken;
						Log.Warning($"SignalR context token changed. new token = {context.Token}");
					}

					return Task.CompletedTask;
				},
				OnAuthenticationFailed = context =>
				{
					if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
						context.Response.Headers.Append("Token-Expired", "true");

					return Task.CompletedTask;
				}
			};
		});

		services.AddAuthorization();

		return services;
	}

	public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddQuartz(q =>
		{
			#region Test Job

			// Configure Hourly Job (every 1 hour)

			var hourlyJobKey = new JobKey("HourlyJob");

			q.AddJob<TestJob>(opts => opts.WithIdentity(hourlyJobKey));

			q.AddTrigger(opts => opts
				.ForJob(hourlyJobKey)
				.WithIdentity("HourlyJob-trigger")
				.WithSimpleSchedule(schedule => schedule
					.WithIntervalInHours(1)
					.RepeatForever()));

			#endregion

			#region Rate Limit Reset Job

			var rateLimitResetJobKey = new JobKey("RateLimitResetJob");

			q.AddJob<RateLimitResetJob>(opts => opts.WithIdentity(rateLimitResetJobKey));

			var windowSeconds = int.Parse(configuration["RateLimit:WindowSeconds"] ?? "120");

			q.AddTrigger(opts => opts
				.ForJob(rateLimitResetJobKey)
				.WithIdentity("RateLimitResetJob-trigger")
				.WithSimpleSchedule(x => x
					.WithIntervalInSeconds(windowSeconds)
					.RepeatForever()));

			#endregion

			#region Ad Feature Reset Job

			var adFeatureResetJobKey = new JobKey("AdFeatureResetJob");

			q.AddJob<FeaturedAdResetJob>(opts => opts.WithIdentity(adFeatureResetJobKey));

			q.AddTrigger(opts => opts
				.ForJob(adFeatureResetJobKey)
				.WithIdentity("AdFeatureResetJob-trigger")
				.WithSimpleSchedule(schedule => schedule
					.WithIntervalInHours(24)
					.RepeatForever()));

			#endregion


			// Configure Weekly Job (Monday at 12:00)
			/*
			var weeklyJobKey = new JobKey("WeeklyJob");

			q.AddJob<TestJob>(opts => opts.WithIdentity(weeklyJobKey));

			q.AddTrigger(opts => opts
				.ForJob(weeklyJobKey)
				.WithIdentity("WeeklyJob-trigger")
				.WithCronSchedule("0 0 12 ? * MON *")); // Every Monday at 12:00
			*/

			//----

			// Configure Monthly Job (first day of each month at 00:00)
			/*
			var monthlyJobKey = new JobKey("MonthlyJob");

			q.AddJob<TestJob>(opts => opts.WithIdentity(monthlyJobKey));
			
			q.AddTrigger(opts => opts
				.ForJob(monthlyJobKey)
				.WithIdentity("MonthlyJob-trigger")
				.WithCronSchedule("0 0 0 1 * ?")); // First day of each month at midnight
			*/
		});

		services.AddQuartzHostedService(options =>
		{
			options.WaitForJobsToComplete = true;
		});

		return services;

	}

}
