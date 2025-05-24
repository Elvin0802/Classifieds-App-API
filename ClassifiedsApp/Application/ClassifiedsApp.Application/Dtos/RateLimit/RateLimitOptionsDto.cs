namespace ClassifiedsApp.Application.Dtos.RateLimit;

public class RateLimitOptionsDto
{
	public int MaxRequests { get; set; } = 100;
	public int WindowSeconds { get; set; } = 60;
}
