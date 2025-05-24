using ClassifiedsApp.Application.Common.Results;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Auth.CheckEmail;

public class CheckEmailCommand : IRequest<Result>
{
	public string Email { get; set; }
}
