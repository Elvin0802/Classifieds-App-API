using ClassifiedsApp.Application.Dtos.RateLimit;
using ClassifiedsApp.Application.Interfaces.Services.RateLimit;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace ClassifiedsApp.Infrastructure.Services.RateLimit;

public class RateLimitService : IRateLimitService
{
	private readonly RateLimitOptionsDto _options;
	private readonly ConcurrentDictionary<string, RateLimitInfoDto> _counters = new();

	public RateLimitService(IOptions<RateLimitOptionsDto> options)
	{
		_options = options.Value;
	}

	public bool IsLimitExceeded(string userKey, out TimeSpan retryAfter)
	{
		var now = DateTime.UtcNow;

		var counter = _counters.GetOrAdd(userKey, _ => new RateLimitInfoDto
		{
			Count = 0,
			WindowStart = now
		});

		lock (counter)
		{
			if (now - counter.WindowStart >= TimeSpan.FromSeconds(_options.WindowSeconds))
			{
				counter.Count = 0;
				counter.WindowStart = now;
			}

			if (counter.Count < _options.MaxRequests)
			{
				counter.Count++;
				retryAfter = TimeSpan.Zero;
				return false;
			}

			retryAfter = counter.WindowStart.AddSeconds(_options.WindowSeconds) - now;
			return true;
		}
	}

	public void ResetAll()
	{
		_counters.Clear();
	}
}
