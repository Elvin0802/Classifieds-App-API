using ClassifiedsApp.Application.Common.Results;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Users.ChangeName;

public class ChangeNameCommand : IRequest<Result>
{
	public string Name { get; set; }
}
