using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Application.Dtos.RateLimit;

public class RateLimitInfoDto
{
	public int Count { get; set; }
	public DateTime WindowStart { get; set; }
}
