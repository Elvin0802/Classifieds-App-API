using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Application.Dtos.Auth.Users;

public class EmailVerificationDto
{
	public string Code { get; set; }
	public string Token { get; set; }
	public DateTime Timestamp { get; set; }
}
