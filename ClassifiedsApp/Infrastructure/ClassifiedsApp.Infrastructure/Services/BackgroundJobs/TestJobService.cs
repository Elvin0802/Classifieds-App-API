using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using ClassifiedsApp.Application.Interfaces.Services.BackgroundJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClassifiedsApp.Infrastructure.Services.BackgroundJobs;

public class TestJobService : IBackgroundJob
{
	private readonly ILogger<TestJobService> _logger;
	private readonly IAdReadRepository _repository;

	public TestJobService(ILogger<TestJobService> logger, IAdReadRepository repository)
	{
		_logger = logger;
		_repository = repository;
	}

	public async Task Execute(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Job executing at: {time}", DateTime.UtcNow);

		try
		{
			var itemsCount = (await _repository.GetAll(false).ToListAsync()).Count;

			_logger.LogInformation("Total item count = {itemsCount}", itemsCount);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error executing in job.");
		}
	}
}