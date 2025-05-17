using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Auth.Token;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ClassifiedsApp.Application.Features.Commands.Auth.Login;

public class LoginCommand : IRequest<Result<AuthTokenDto>>
{
	public string Email { get; set; }
	public string Password { get; set; }
}
