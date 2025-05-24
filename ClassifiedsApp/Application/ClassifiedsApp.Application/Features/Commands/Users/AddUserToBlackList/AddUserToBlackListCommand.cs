using ClassifiedsApp.Application.Common.Results;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Users.AddUserToBlackList;

public class AddUserToBlackListCommand : IRequest<Result>
{
	public string Email { get; set; }
	public string Reason { get; set; }
}
