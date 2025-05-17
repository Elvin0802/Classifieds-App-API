using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Auth.Token;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ClassifiedsApp.Application.Features.Commands.Auth.RefreshTokenLogin;

public class RefreshTokenLoginCommand : IRequest<Result<AuthTokenDto>>
{

}
