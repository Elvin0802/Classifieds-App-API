using ClassifiedsApp.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Application.Features.Commands.Auth.CheckEmail;

public class CheckEmailCommand : IRequest<Result>
{
	public string Email { get; set; }
}
