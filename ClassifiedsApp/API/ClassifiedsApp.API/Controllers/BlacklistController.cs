using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Features.Commands.Users.AddUserToBlackList;
using ClassifiedsApp.Application.Features.Commands.Users.RemoveUserFromBlackList;
using ClassifiedsApp.Application.Features.Queries.Users.GetUsersFromBlackList;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedsApp.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public class BlacklistController : ControllerBase
{
	private readonly IMediator _mediator;

	public BlacklistController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	public async Task<ActionResult<Result>> BlacklistUser(AddUserToBlackListCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpPost]
	public async Task<ActionResult<Result>> UnblacklistUser(RemoveUserFromBlackListCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpGet]
	public async Task<ActionResult<Result<GetUsersFromBlackListQueryResponse>>> GetBlacklistedUsers()
	{
		return Ok(await _mediator.Send(new GetUsersFromBlackListQuery()));
	}

}
