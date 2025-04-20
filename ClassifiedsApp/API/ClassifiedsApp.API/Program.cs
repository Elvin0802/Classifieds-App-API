using ClassifiedsApp.API.Config;
using ClassifiedsApp.API.Middlewares;
using ClassifiedsApp.Application;
//using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
//using ClassifiedsApp.Application.Interfaces.Repositories.Categories;
//using ClassifiedsApp.Application.Interfaces.Repositories.Locations;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Infrastructure;
using ClassifiedsApp.SignalR;
using ClassifiedsApp.SignalR.Hubs;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Host.UseSerilog((context, config) =>
{
	config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AuthenticationAndAuthorization(builder.Configuration);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSignalRServices();

builder.Services.AddSwagger();

builder.Services.AddBackgroundJobs();

var client = builder.Configuration["ClientUrl"];

builder.Services.AddCors(options => options.AddPolicy("CORSPolicy", builder =>
{
	builder.WithOrigins(client!)
		   .AllowAnyMethod()
		   .AllowAnyHeader()
		   .AllowCredentials();
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("CORSPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");

// Adding seed data.
using (var scope = app.Services.CreateScope())
{
	await SeedData.AddSeedRolesAndUsersAsync(scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>(),
											 scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>(),
											 builder.Configuration);
	/*
		await SeedData.AddSeedDataAsync(scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>(),
										builder.Configuration,
										scope.ServiceProvider.GetRequiredService<ICategoryReadRepository>(),
										scope.ServiceProvider.GetRequiredService<ICategoryWriteRepository>(),
										scope.ServiceProvider.GetRequiredService<IMainCategoryReadRepository>(),
										scope.ServiceProvider.GetRequiredService<IMainCategoryWriteRepository>(),
										scope.ServiceProvider.GetRequiredService<ISubCategoryReadRepository>(),
										scope.ServiceProvider.GetRequiredService<ISubCategoryWriteRepository>(),
										scope.ServiceProvider.GetRequiredService<ISubCategoryOptionReadRepository>(),
										scope.ServiceProvider.GetRequiredService<ISubCategoryOptionWriteRepository>(),
										scope.ServiceProvider.GetRequiredService<IAdReadRepository>(),
										scope.ServiceProvider.GetRequiredService<IAdWriteRepository>(),
										scope.ServiceProvider.GetRequiredService<ILocationReadRepository>(),
										scope.ServiceProvider.GetRequiredService<ILocationWriteRepository>());*/
}

app.Run();


/*

public class GetChatMessagesByChatRoomQueryHandler : IRequestHandler<GetChatMessagesByChatRoomQuery, Result<List<ChatMessageDto>>>
{
    private readonly IRepository<ChatRoom> _chatRoomRepository;
    private readonly IRepository<ChatMessage> _chatMessageRepository;
    private readonly IRepository<AppUser> _userRepository;

    public GetChatMessagesByChatRoomQueryHandler(
        IRepository<ChatRoom> chatRoomRepository,
        IRepository<ChatMessage> chatMessageRepository,
        IRepository<AppUser> userRepository)
    {
        _chatRoomRepository = chatRoomRepository;
        _chatMessageRepository = chatMessageRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<ChatMessageDto>>> Handle(GetChatMessagesByChatRoomQuery request, CancellationToken cancellationToken)
    {
        var chatRoom = await _chatRoomRepository.GetByIdAsync(request.ChatRoomId);
        if (chatRoom == null)
            return Result<List<ChatMessageDto>>.Failure("Chat room not found");

        if (chatRoom.BuyerId != request.UserId && chatRoom.SellerId != request.UserId)
            return Result<List<ChatMessageDto>>.Failure("User is not part of this chat room");

        var messages = await _chatMessageRepository.ListWithIncludeAsync(
            m => m.AdId == chatRoom.AdId && 
                (m.SenderId == chatRoom.BuyerId || m.SenderId == chatRoom.SellerId) &&
                (m.ReceiverId == chatRoom.BuyerId || m.ReceiverId == chatRoom.SellerId),
            m => m.Sender);

        // Sort by creation time
        messages = messages.OrderBy(m => m.CreatedAt).ToList();

        var messageDtos = new List<ChatMessageDto>();
        
        foreach (var message in messages)
        {
            messageDtos.Add(new ChatMessageDto
            {
                Id = message.Id,
                Content = message.Content,
                SenderId = message.SenderId,
                SenderName = message.Sender.Name,
                CreatedAt = message.CreatedAt,
                IsRead = message.IsRead
            });
        }

        return Result<List<ChatMessageDto>>.Success(messageDtos);
    }
}

public class GetAdChatInfoQueryHandler : IRequestHandler<GetAdChatInfoQuery, Result<AdChatInfoDto>>
{
    private readonly IRepository<Ad> _adRepository;

    public GetAdChatInfoQueryHandler(IRepository<Ad> adRepository)
    {
        _adRepository = adRepository;
    }

    public async Task<Result<AdChatInfoDto>> Handle(GetAdChatInfoQuery request, CancellationToken cancellationToken)
    {
        var ad = await _adRepository.GetByIdWithIncludeAsync(request.AdId, x => x.AppUser, x => x.Images);
        if (ad == null)
            return Result<AdChatInfoDto>.Failure("Ad not found");

        var mainImage = ad.Images.FirstOrDefault(i => i.SortOrder == 0);
        string imageUrl = mainImage?.Url ?? "";

        var adChatInfoDto = new AdChatInfoDto
        {
            Id = ad.Id,
            Title = ad.Title,
            ImageUrl = imageUrl,
            Price = ad.Price,
            SellerName = ad.AppUser.Name
        };

        return Result<AdChatInfoDto>.Success(adChatInfoDto);
    }
}

*/