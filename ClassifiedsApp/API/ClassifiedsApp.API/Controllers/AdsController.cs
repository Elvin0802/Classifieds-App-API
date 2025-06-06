﻿using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Features.Commands.Ads.ChangeAdStatus;
using ClassifiedsApp.Application.Features.Commands.Ads.CreateAd;
using ClassifiedsApp.Application.Features.Commands.Ads.DeleteAd;
using ClassifiedsApp.Application.Features.Commands.Ads.FeatureAd;
using ClassifiedsApp.Application.Features.Commands.Ads.UpdateAd;
using ClassifiedsApp.Application.Features.Commands.Users.SelectAdCommand;
using ClassifiedsApp.Application.Features.Commands.Users.UnselectAd;
using ClassifiedsApp.Application.Features.Queries.Ads.GetAdById;
using ClassifiedsApp.Application.Features.Queries.Ads.GetAllAds;
using ClassifiedsApp.Application.Features.Queries.Ads.GetFeaturedPricing;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ClassifiedsApp.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AdsController : ControllerBase
{
	readonly IMediator _mediator;

	public AdsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User")]
	public async Task<ActionResult<Result>> Create([FromForm] CreateAdCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpPost]
	[DisableRateLimiting]
	public async Task<ActionResult<Result<GetAllAdsQueryResponse>>> GetAll([FromBody] GetAllAdsQuery? query)
	{
		return Ok(await _mediator.Send(query!));
	}

	[HttpGet]
	[DisableRateLimiting]
	public async Task<ActionResult<Result<GetAdByIdQueryResponse>>> GetById([FromQuery] GetAdByIdQuery query)
	{
		return Ok(await _mediator.Send(query));
	}

	[HttpDelete]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User")]
	public async Task<ActionResult<Result>> Delete([FromQuery] DeleteAdCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpPost]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User")]
	public async Task<ActionResult<Result>> SelectAd([FromBody] SelectAdCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpPost]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User")]
	public async Task<ActionResult<Result>> UnselectAd([FromBody] UnselectAdCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpPost]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User")]
	public async Task<ActionResult<Result>> Update([FromForm] UpdateAdCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpGet]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User")]
	public async Task<ActionResult<Result<GetFeaturedPricingQueryResponse>>> GetPricingOptions()
	{
		return Ok(await _mediator.Send(new GetFeaturedPricingQuery()));
	}

	[HttpPost]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User")]
	public async Task<ActionResult<Result>> FeatureAd(FeatureAdCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpPost]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
	public async Task<ActionResult<Result>> ChangeAdStatus([FromBody] ChangeAdStatusCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

}
