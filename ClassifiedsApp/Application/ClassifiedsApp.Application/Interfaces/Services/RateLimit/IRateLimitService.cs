using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Application.Interfaces.Services.RateLimit;

public interface IRateLimitService
{
	bool IsLimitExceeded(string userKey, out TimeSpan retryAfter);
	void ResetAll();
}