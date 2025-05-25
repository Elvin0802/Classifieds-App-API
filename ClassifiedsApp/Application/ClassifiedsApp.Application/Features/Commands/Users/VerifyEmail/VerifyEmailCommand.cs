using ClassifiedsApp.Application.Common.Results;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Users.VerifyEmail;

public class VerifyEmailCommand : IRequest<Result>
{
	public string Email { get; set; }
	public string Code { get; set; }
}
