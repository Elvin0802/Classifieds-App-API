namespace ClassifiedsApp.Application.Interfaces.Services.Ads;

public interface IAiService
{
	Task<bool> CheckAdContent(string title, string description, List<string> categories);

}
