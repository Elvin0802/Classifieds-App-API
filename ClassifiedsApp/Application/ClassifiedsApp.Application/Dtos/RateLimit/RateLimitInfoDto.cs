namespace ClassifiedsApp.Application.Dtos.RateLimit;

public class RateLimitInfoDto
{
	public int Count { get; set; }
	public DateTime WindowStart { get; set; }
}
