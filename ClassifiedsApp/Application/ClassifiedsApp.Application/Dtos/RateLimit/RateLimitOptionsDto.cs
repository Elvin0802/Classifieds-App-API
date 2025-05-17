using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Application.Dtos.RateLimit;

public class RateLimitOptionsDto
{
	public int MaxRequests { get; set; } = 100;
	public int WindowSeconds { get; set; } = 60;
}
