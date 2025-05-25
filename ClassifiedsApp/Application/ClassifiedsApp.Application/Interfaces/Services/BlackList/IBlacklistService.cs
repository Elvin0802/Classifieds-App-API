using ClassifiedsApp.Application.Dtos.Auth.Users;

namespace ClassifiedsApp.Application.Interfaces.Services.BlackList;

public interface IBlacklistService
{
	Task<bool> IsUserBlacklistedAsync(string email);
	Task<bool> IsUserBlacklistedAsync(Guid userId);
	Task BlacklistUserAsync(string email, string reason);
	Task UnblacklistUserAsync(string email);
	Task<IEnumerable<BlacklistUserDto>> GetBlacklistedUsersAsync();
}
