using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Features.Commands.Auth.Register;
using ClassifiedsApp.Application.Features.Commands.Users.ChangePassword;
using ClassifiedsApp.Application.Features.Commands.Users.UpdatePassword;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedsApp.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UsersController : ControllerBase
{
	readonly IMediator _mediator;

	public UsersController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	public async Task<ActionResult<Result>> Register(RegisterCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpPost]
	public async Task<ActionResult<Result>> UpdatePassword([FromBody] UpdatePasswordCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpPost]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User")]
	public async Task<ActionResult<Result>> ChangePassword([FromBody] ChangePasswordCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

}
