using ClassifiedsApp.API.Config;
using ClassifiedsApp.API.Middlewares;
using ClassifiedsApp.Application;
using ClassifiedsApp.Application.Interfaces.Repositories.Categories;
using ClassifiedsApp.Application.Interfaces.Repositories.Locations;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Infrastructure;
using ClassifiedsApp.SignalR;
using ClassifiedsApp.SignalR.Hubs;
using Microsoft.AspNetCore.Identity;
using Serilog;

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

app.Run();

