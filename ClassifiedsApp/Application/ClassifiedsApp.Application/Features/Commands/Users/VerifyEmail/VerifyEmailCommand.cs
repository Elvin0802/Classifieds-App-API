using ClassifiedsApp.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Application.Features.Commands.Users.VerifyEmail;

public class VerifyEmailCommand : IRequest<Result>
{
	public string Email { get; set; }
	public string Code { get; set; }
}
