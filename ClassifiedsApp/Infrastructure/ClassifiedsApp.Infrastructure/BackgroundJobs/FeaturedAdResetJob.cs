using ClassifiedsApp.Infrastructure.Services.BackgroundJobs;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class FeaturedAdResetJob : QuartzJobRunner<FeaturedAdResetJobService>
{
	public FeaturedAdResetJob(FeaturedAdResetJobService job) : base(job)
	{
	}
}
