namespace ClassifiedsApp.Application.Interfaces.Services.RateLimit;

public interface IRateLimitService
{
	bool IsLimitExceeded(string userKey, out TimeSpan retryAfter);
	void ResetAll();
}