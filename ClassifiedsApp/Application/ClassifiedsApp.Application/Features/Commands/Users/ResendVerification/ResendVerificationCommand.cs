using ClassifiedsApp.Application.Common.Results;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Users.ResendVerification;

public class ResendVerificationCommand : IRequest<Result>
{
	public string Email { get; set; }
}
