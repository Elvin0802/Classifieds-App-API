using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Chats;
using ClassifiedsApp.Application.Features.Commands.Chats.CreateChatRoom;
using ClassifiedsApp.Application.Features.Commands.Chats.MarkMessagesAsRead;
using ClassifiedsApp.Application.Features.Commands.Chats.SendMessage;
using ClassifiedsApp.Application.Features.Queries.Chats.GetAdChatInfo;
using ClassifiedsApp.Application.Features.Queries.Chats.GetChatMessagesByChatRoom;
using ClassifiedsApp.Application.Features.Queries.Chats.GetChatRoom;
using ClassifiedsApp.Application.Features.Queries.Chats.GetChatRoomsByUser;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedsApp.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User")]
public class ChatController : ControllerBase
{
	readonly IMediator _mediator;

	public ChatController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	public async Task<ActionResult<Result<ChatRoomDto>>> CreateChatRoom(CreateChatRoomCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpPost]
	public async Task<ActionResult<Result<GetChatRoomsByUserQueryResponse>>> GetChatRooms()
	{
		return Ok(await _mediator.Send(new GetChatRoomsByUserQuery()));
	}

	[HttpPost]
	public async Task<ActionResult<Result<GetChatRoomQueryResponse>>> GetChatRoom([FromBody] GetChatRoomQuery query)
	{
		return Ok(await _mediator.Send(query));
	}

	[HttpPost]
	public async Task<ActionResult<Result<GetChatMessagesByChatRoomQueryResponse>>> GetChatMessages([FromBody] GetChatMessagesByChatRoomQuery query)
	{
		return Ok(await _mediator.Send(query));
	}

	[HttpPost]
	public async Task<ActionResult<Result<GetAdChatInfoQueryResponse>>> GetAdChatInfo([FromBody] GetAdChatInfoQuery query)
	{
		return Ok(await _mediator.Send(query));
	}

	[HttpPost]
	public async Task<ActionResult<Result<ChatMessageDto>>> SendMessage([FromBody] SendMessageCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

	[HttpPost]
	public async Task<ActionResult<Result>> MarkMessagesAsRead([FromBody] MarkMessagesAsReadCommand command)
	{
		return Ok(await _mediator.Send(command));
	}

}
