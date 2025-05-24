using ClassifiedsApp.Application.Interfaces.Services.BackgroundJobs;
using ClassifiedsApp.Application.Interfaces.Services.RateLimit;
using Microsoft.Extensions.Logging;

namespace ClassifiedsApp.Infrastructure.Services.BackgroundJobs;

public class RateLimitResetJobService : IBackgroundJob
{
	private readonly ILogger<RateLimitResetJobService> _logger;
	private readonly IRateLimitService _rateLimitService;

	public RateLimitResetJobService(ILogger<RateLimitResetJobService> logger, IRateLimitService rateLimitService)
	{
		_logger = logger;
		_rateLimitService = rateLimitService;
	}

	public async Task Execute(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Job executing at: {time}", DateTime.UtcNow);

		try
		{
			_rateLimitService.ResetAll();

			_logger.LogInformation("Rate limit reseted.");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error executing in job.");
		}
	}

}
