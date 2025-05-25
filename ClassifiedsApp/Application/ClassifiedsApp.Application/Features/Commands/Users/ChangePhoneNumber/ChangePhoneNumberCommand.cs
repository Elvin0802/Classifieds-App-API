using ClassifiedsApp.Application.Common.Results;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Users.ChangePhoneNumber;

public class ChangePhoneNumberCommand : IRequest<Result>
{
	public string PhoneNumber { get; set; }
}
