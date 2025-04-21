using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedsApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
	private readonly IMediator _mediator;

	public ChatController(IMediator mediator)
	{
		_mediator = mediator;
	}

	//[HttpPost("rooms")]
	//public async Task<IActionResult> CreateChatRoom(CreateChatRoomCommand command)
	//{
	//	var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
	//	if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
	//		return Unauthorized();

	//	command.BuyerId = userGuid;
	//	var result = await _mediator.Send(command);

	//	if (!result.IsSuccess)
	//		return BadRequest(result.Error);

	//	return Ok(result.Value);


	//}


}