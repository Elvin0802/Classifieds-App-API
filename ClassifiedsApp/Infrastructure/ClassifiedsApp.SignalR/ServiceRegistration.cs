using ClassifiedsApp.Application.Interfaces.Services.SignalR;
using ClassifiedsApp.SignalR.HubServices;
using Microsoft.Extensions.DependencyInjection;

namespace ClassifiedsApp.SignalR;

public static class ServiceRegistration
{
	public static void AddSignalRServices(this IServiceCollection services)
	{
		services.AddScoped<IChatHub, ChatHubService>();

		services.AddSignalR();

	}
}
