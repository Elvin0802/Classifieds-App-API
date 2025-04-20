namespace ClassifiedsApp.Application.Interfaces.Services.SignalR;

public interface IChatHub
{
	Task SendMessageAsync(string userId, string method, object message);
}
