﻿using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Features.Commands.Locations.CreateLocation;
using ClassifiedsApp.Application.Features.Commands.Locations.DeleteLocation;
using ClassifiedsApp.Application.Features.Queries.Locations.GetAllLocations;
using ClassifiedsApp.Application.Features.Queries.Locations.GetLocationById;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ClassifiedsApp.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]

public class LocationsController : ControllerBase
{
	readonly IMediator _mediator;

	public LocationsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
	public async Task<ActionResult<Result>> Create([FromBody] CreateLocationCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpGet]
	[DisableRateLimiting]
	public async Task<ActionResult<Result<GetAllLocationsQueryResponse>>> GetAll([FromQuery] GetAllLocationsQuery query)
	{
		return Ok(await _mediator.Send(query));
	}

	[HttpGet]
	[DisableRateLimiting]
	public async Task<ActionResult<Result<GetLocationByIdQueryResponse>>> GetById([FromQuery] GetLocationByIdQuery query)
	{
		return Ok(await _mediator.Send(query));
	}

	[HttpPost]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
	public async Task<ActionResult<Result>> Delete([FromBody] DeleteLocationCommand command)
	{
		return Ok(await _mediator.Send(command));
	}
}
