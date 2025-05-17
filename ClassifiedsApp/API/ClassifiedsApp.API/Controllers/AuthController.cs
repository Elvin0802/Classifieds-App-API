using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Auth.Token;
using ClassifiedsApp.Application.Features.Commands.Auth.ConfirmResetToken;
using ClassifiedsApp.Application.Features.Commands.Auth.Login;
using ClassifiedsApp.Application.Features.Commands.Auth.PasswordReset;
using ClassifiedsApp.Application.Features.Commands.Auth.RefreshTokenLogin;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedsApp.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
	readonly IMediator _mediator;

	public AuthController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	public async Task<ActionResult<Result<string>>> Login(LoginCommand command)
	{
		command.Response = Response;

		AuthTokenDto authToken = (await _mediator.Send(command)).Data;

		return Ok(Result.Success(authToken.AccessToken, "Login successfully completed."));
		//return Ok(new { token = authToken.AccessToken });
		//return Ok(); // use this line for production.
	}

	[HttpPost]
	public async Task<ActionResult<Result<string>>> RefreshTokenLogin()
	{
		AuthTokenDto authToken = (await _mediator.Send(
			new RefreshTokenLoginCommand
			{
				Request = Request,
				Response = Response
			})).Data;

		return Ok(Result.Success(authToken.AccessToken, "Refresh Token Login successfully completed."));
		//return Ok(new { token = authToken.AccessToken });
	}

	[HttpPost]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User")]
	public async Task<ActionResult<Result>> Logout()
	{
		Response.Cookies.Delete("refreshToken", new CookieOptions
		{
			HttpOnly = true,
			Secure = false,
			SameSite = SameSiteMode.Lax,
		});

		Response.Headers.Append("Authorization", "");

		return Ok(Result.Success("Log Out successfully completed."));
		//return Ok(new { Message = "Log out success." });
	}

	[HttpPost]
	public async Task<ActionResult<Result>> PasswordReset([FromBody] PasswordResetCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpPost]
	public async Task<ActionResult<Result>> ConfirmResetToken([FromBody] ConfirmResetTokenCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

}

