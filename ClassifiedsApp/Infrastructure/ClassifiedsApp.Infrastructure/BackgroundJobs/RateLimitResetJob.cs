using ClassifiedsApp.Infrastructure.Services.BackgroundJobs;
using Quartz;

namespace ClassifiedsApp.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class RateLimitResetJob : QuartzJobRunner<RateLimitResetJobService>
{
	public RateLimitResetJob(RateLimitResetJobService job) : base(job)
	{
	}
}
