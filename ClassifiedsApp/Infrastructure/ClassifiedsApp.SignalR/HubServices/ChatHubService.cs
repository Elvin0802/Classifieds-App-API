using ClassifiedsApp.Application.Interfaces.Services.SignalR;
using ClassifiedsApp.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ClassifiedsApp.SignalR.HubServices;

public class ChatHubService : IChatHub
{
	private readonly IHubContext<ChatHub> _hubContext;

	public ChatHubService(IHubContext<ChatHub> hubContext)
	{
		_hubContext = hubContext;
	}

	public async Task SendMessageAsync(string userId, string method, object message)
	{
		await _hubContext.Clients.Group(userId).SendAsync(method, message);
	}
}
