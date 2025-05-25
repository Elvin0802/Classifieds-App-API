using ClassifiedsApp.Application.Common.Results;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Users.RemoveUserFromBlackList;

public class RemoveUserFromBlackListCommand : IRequest<Result>
{
	public string Email { get; set; }
}
