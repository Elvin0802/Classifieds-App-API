using ClassifiedsApp.Infrastructure.Services.BackgroundJobs;
using Quartz;

namespace ClassifiedsApp.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class FeaturedAdResetJob : QuartzJobRunner<FeaturedAdResetJobService>
{
	public FeaturedAdResetJob(FeaturedAdResetJobService job) : base(job)
	{
	}
}
