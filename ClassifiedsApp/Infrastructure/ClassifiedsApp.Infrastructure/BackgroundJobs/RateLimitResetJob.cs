using ClassifiedsApp.Application.Interfaces.Services.RateLimit;
using ClassifiedsApp.Infrastructure.Services.BackgroundJobs;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class RateLimitResetJob : QuartzJobRunner<RateLimitResetJobService>
{
	public RateLimitResetJob(RateLimitResetJobService job) : base(job)
	{
	}
}
