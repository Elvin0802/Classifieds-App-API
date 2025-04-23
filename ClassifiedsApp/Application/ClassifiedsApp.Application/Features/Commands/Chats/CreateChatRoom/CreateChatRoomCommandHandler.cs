using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Chats;
using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using ClassifiedsApp.Application.Interfaces.Repositories.Chats;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using ClassifiedsApp.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClassifiedsApp.Application.Features.Commands.Chats.CreateChatRoom;

public class CreateChatRoomCommandHandler : IRequestHandler<CreateChatRoomCommand, Result<ChatRoomDto>>
{
	readonly IChatRoomReadRepository _chatRoomReadRepository;
	readonly IChatRoomWriteRepository _chatRoomWriteRepository;
	readonly IAdReadRepository _adReadRepository;
	readonly ICurrentUserService _currentUserService;
	readonly UserManager<AppUser> _userManager;

	public CreateChatRoomCommandHandler(IChatRoomReadRepository chatRoomReadRepository,
										IChatRoomWriteRepository chatRoomWriteRepository,
										IAdReadRepository adReadRepository,
										UserManager<AppUser> userManager,
										ICurrentUserService currentUserService)
	{
		_chatRoomReadRepository = chatRoomReadRepository;
		_chatRoomWriteRepository = chatRoomWriteRepository;
		_adReadRepository = adReadRepository;
		_userManager = userManager;
		_currentUserService = currentUserService;
	}

	public async Task<Result<ChatRoomDto>> Handle(CreateChatRoomCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var ad = await _adReadRepository.GetAdByIdWithIncludesAsync(request.AdId, false)
					 ?? throw new KeyNotFoundException("Ad not found.");

			if (ad.AppUserId == _currentUserService.UserId)
				throw new Exception("User cannot chat with yourself.");

			// Check if chat room already exists
			var existingChatRoom = _chatRoomReadRepository
									.GetWhere(x => x.AdId == request.AdId && x.BuyerId == _currentUserService.UserId && x.SellerId == ad.AppUserId)
									.ToList();

			if (existingChatRoom.Count != 0)
				throw new Exception("Chat room already exists");

			var buyer = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString()!)
						?? throw new KeyNotFoundException("User not found.");

			var chatRoom = new ChatRoom()
			{
				BuyerId = _currentUserService.UserId!.Value,
				SellerId = ad.AppUserId,
				AdId = request.AdId
			};

			await _chatRoomWriteRepository.AddAsync(chatRoom);
			await _chatRoomWriteRepository.SaveAsync();

			var mainImage = ad.Images.FirstOrDefault(i => i.SortOrder == 0);
			string imageUrl = mainImage?.Url ?? "";

			var chatRoomDto = new ChatRoomDto
			{
				Id = chatRoom.Id,
				BuyerId = chatRoom.BuyerId,
				BuyerName = buyer.Name,
				SellerId = chatRoom.SellerId,
				SellerName = ad.AppUser.Name,
				AdId = chatRoom.AdId,
				AdTitle = ad.Title,
				AdImageUrl = imageUrl,
				AdPrice = ad.Price,
				LastMessageAt = chatRoom.CreatedAt,
				UnreadCount = 0
			};

			return Result.Success(chatRoomDto, "Chat room successfully created.");
		}
		catch (Exception ex)
		{
			return Result.Failure<ChatRoomDto>($"Error occoured. {ex.Message}");
		}
	}
}
