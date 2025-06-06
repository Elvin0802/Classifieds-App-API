﻿using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Features.Commands.Categories.CreateCategory;
using ClassifiedsApp.Application.Features.Commands.Categories.CreateMainCategory;
using ClassifiedsApp.Application.Features.Commands.Categories.CreateSubCategory;
using ClassifiedsApp.Application.Features.Commands.Categories.DeleteCategory;
using ClassifiedsApp.Application.Features.Commands.Categories.DeleteMainCategory;
using ClassifiedsApp.Application.Features.Commands.Categories.DeleteSubCategory;
using ClassifiedsApp.Application.Features.Queries.Categories.GetAllCategories;
using ClassifiedsApp.Application.Features.Queries.Categories.GetAllMainCategories;
using ClassifiedsApp.Application.Features.Queries.Categories.GetAllSubCategories;
using ClassifiedsApp.Application.Features.Queries.Categories.GetCategoryById;
using ClassifiedsApp.Application.Features.Queries.Categories.GetMainCategoryById;
using ClassifiedsApp.Application.Features.Queries.Categories.GetSubCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ClassifiedsApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
	readonly IMediator _mediator;

	public CategoriesController(IMediator mediator)
	{
		_mediator = mediator;
	}

	#region Category Section

	[HttpPost("create/category")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
	public async Task<ActionResult<Result>> Create([FromBody] CreateCategoryCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpGet("all/category")]
	[DisableRateLimiting]
	public async Task<ActionResult<Result<GetAllCategoriesQueryResponse>>> GetAll([FromQuery] GetAllCategoriesQuery query)
	{
		return Ok(await _mediator.Send(query));
	}

	[HttpGet("byId/category")]
	[DisableRateLimiting]
	public async Task<ActionResult<Result<GetCategoryByIdQueryResponse>>> GetById([FromQuery] GetCategoryByIdQuery query)
	{
		return Ok(await _mediator.Send(query));
	}

	[HttpDelete("delete/category")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
	public async Task<ActionResult<Result>> Delete([FromQuery] DeleteCategoryCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	#endregion

	#region Main Category Section

	[HttpPost("create/main-category")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
	public async Task<ActionResult<Result>> Create([FromBody] CreateMainCategoryCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpGet("all/main-category")]
	[DisableRateLimiting]
	public async Task<ActionResult<Result<GetAllMainCategoriesQueryResponse>>> GetAll([FromQuery] GetAllMainCategoriesQuery query)
	{
		return Ok(await _mediator.Send(query));
	}

	[HttpGet("byId/main-category")]
	[DisableRateLimiting]
	public async Task<ActionResult<Result<GetMainCategoryByIdQueryResponse>>> GetById([FromQuery] GetMainCategoryByIdQuery query)
	{
		return Ok(await _mediator.Send(query));
	}

	[HttpDelete("delete/main-category")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
	public async Task<ActionResult<Result>> Delete([FromQuery] DeleteMainCategoryCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	#endregion

	#region Sub Category Section

	[HttpPost("create/sub-category")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
	public async Task<ActionResult<Result>> Create([FromBody] CreateSubCategoryCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpGet("all/sub-category")]
	[DisableRateLimiting]
	public async Task<ActionResult<Result<GetAllSubCategoriesQueryResponse>>> GetAll([FromQuery] GetAllSubCategoriesQuery query)
	{
		return Ok(await _mediator.Send(query));
	}

	[HttpGet("byId/sub-category")]
	[DisableRateLimiting]
	public async Task<ActionResult<Result<GetSubCategoryByIdQueryResponse>>> GetById([FromQuery] GetSubCategoryByIdQuery query)
	{
		return Ok(await _mediator.Send(query));
	}

	[HttpDelete("delete/sub-category")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
	public async Task<ActionResult<Result>> Delete([FromQuery] DeleteSubCategoryCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	#endregion

}
