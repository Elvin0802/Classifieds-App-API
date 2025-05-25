using ClassifiedsApp.API.Config;
using ClassifiedsApp.API.Middlewares;
using ClassifiedsApp.Application;
using ClassifiedsApp.Application.Interfaces.Repositories.Categories;
using ClassifiedsApp.Application.Interfaces.Repositories.Locations;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Infrastructure;
using ClassifiedsApp.SignalR;
using ClassifiedsApp.SignalR.Hubs;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Host.UseSerilog((context, config) =>
{
	config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSignalRServices();

builder.Services.AddSwagger();

builder.Services.AddHealthChecks()
	.AddCheck("self", () => HealthCheckResult.Healthy()) // Liveness
	.AddSqlServer(builder.Configuration["ConnectionStrings:Default"]!, name: "sql", failureStatus: HealthStatus.Unhealthy); // Readiness

builder.Services.AddBackgroundJobs(builder.Configuration);

var client = builder.Configuration["ClientUrl"];

builder.Services.AddCors(options => options.AddPolicy("CORSPolicy", builder =>
{
	builder.WithOrigins(client!)
		   .AllowAnyMethod()
		   .AllowAnyHeader()
		   .AllowCredentials();
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("CORSPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RateLimitingMiddleware>();
app.UseMiddleware<BlackListProtectionMiddleware>();

app.MapControllers();

app.MapHub<ChatHub>("api/chatHub");

// Adding seed data.
using (var scope = app.Services.CreateScope())
{
	await SeedData.AddSeedRolesAndUsersAsync(scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>(),
											 scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>(),
											 builder.Configuration);

	await SeedData.AddSeedDataAsync(scope.ServiceProvider.GetRequiredService<ICategoryReadRepository>(),
									scope.ServiceProvider.GetRequiredService<ICategoryWriteRepository>(),
									scope.ServiceProvider.GetRequiredService<IMainCategoryReadRepository>(),
									scope.ServiceProvider.GetRequiredService<IMainCategoryWriteRepository>(),
									scope.ServiceProvider.GetRequiredService<ISubCategoryReadRepository>(),
									scope.ServiceProvider.GetRequiredService<ISubCategoryWriteRepository>(),
									scope.ServiceProvider.GetRequiredService<ILocationReadRepository>(),
									scope.ServiceProvider.GetRequiredService<ILocationWriteRepository>());
}


// LIVENESS
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
	Predicate = check => check.Name == "self"
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
	Predicate = _ => true,
	ResponseWriter = async (context, report) =>
	{
		context.Response.ContentType = "application/json";

		var result = JsonSerializer.Serialize(new
		{
			status = report.Status.ToString(),
			checks = report.Entries.Select(entry => new
			{
				name = entry.Key,
				status = entry.Value.Status.ToString(),
				error = entry.Value.Exception?.Message
			})
		});

		await context.Response.WriteAsync(result);
	}
});

app.MapGet("/", () => "The API is working successfully.");

app.Run();

