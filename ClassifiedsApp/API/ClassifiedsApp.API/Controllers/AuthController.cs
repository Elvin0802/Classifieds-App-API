﻿using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Auth.Token;
using ClassifiedsApp.Application.Features.Commands.Auth.ConfirmResetToken;
using ClassifiedsApp.Application.Features.Commands.Auth.Login;
using ClassifiedsApp.Application.Features.Commands.Auth.PasswordReset;
using ClassifiedsApp.Application.Features.Commands.Auth.RefreshTokenLogin;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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
	[DisableRateLimiting]
	public async Task<ActionResult<Result<string>>> Login(LoginCommand command)
	{
		AuthTokenDto authToken = (await _mediator.Send(command)).Data ??
								throw new ArgumentNullException("Login failed , mail or password is wrong.");

		return Ok(Result.Success(authToken.AccessToken, "Login successfully completed."));
	}

	[HttpPost]
	[DisableRateLimiting]
	public async Task<ActionResult<Result<string>>> RefreshTokenLogin()
	{
		AuthTokenDto authToken = (await _mediator.Send(new RefreshTokenLoginCommand())).Data;

		return Ok(Result.Success(authToken.AccessToken, "Refresh Token Login successfully completed."));
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
	}

	[HttpPost]
	[DisableRateLimiting]
	public async Task<ActionResult<Result>> PasswordReset([FromBody] PasswordResetCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpPost]
	[DisableRateLimiting]
	public async Task<ActionResult<Result>> ConfirmResetToken([FromBody] ConfirmResetTokenCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

}

