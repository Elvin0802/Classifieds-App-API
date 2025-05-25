using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using ClassifiedsApp.Application.Interfaces.Services.BackgroundJobs;
using ClassifiedsApp.Application.Interfaces.Services.RateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Infrastructure.Services.BackgroundJobs;

public class FeaturedAdResetJobService : IBackgroundJob
{
	readonly ILogger<FeaturedAdResetJobService> _logger;
	readonly IAdReadRepository _adReadRepository;
	readonly IAdWriteRepository _adWriteRepository;

	public FeaturedAdResetJobService(ILogger<FeaturedAdResetJobService> logger,
									 IAdReadRepository adReadRepository,
									 IAdWriteRepository adWriteRepository)
	{
		_logger = logger;
		_adReadRepository = adReadRepository;
		_adWriteRepository = adWriteRepository;
	}

	public async Task Execute(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Featured Ad Reset Job executing at: {time}", DateTime.UtcNow);

		try
		{
			var ads = await _adReadRepository.Table
							.Where(ad => ad.IsFeatured == true && ad.FeatureEndDate <= DateTimeOffset.UtcNow)
							.ToListAsync(cancellationToken);

			if (ads.Count > 0)
			{
				foreach (var ad in ads)
				{
					ad.IsFeatured = false;
					ad.FeaturePriority = 0;
					ad.FeatureStartDate = DateTimeOffset.MinValue;
					ad.FeatureEndDate = DateTimeOffset.MinValue;
				}

				await _adWriteRepository.SaveAsync();
			}

			_logger.LogInformation("Expired ad features reset job completed successfully..");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error executing in job.");
		}
	}

}