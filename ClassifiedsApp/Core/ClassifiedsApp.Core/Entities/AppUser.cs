﻿using Microsoft.AspNetCore.Identity;

namespace ClassifiedsApp.Core.Entities;

public class AppUser : IdentityUser<Guid>
{
	public string Name { get; set; }

	public string? RefreshToken { get; set; }
	public DateTimeOffset? RefreshTokenExpiresAt { get; set; }

	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }
	public DateTimeOffset ArchivedAt { get; set; }

	public bool IsBlacklisted { get; set; } = false;
	public DateTimeOffset? BlacklistedAt { get; set; }
	public string? BlacklistReason { get; set; }

	public IList<Ad> Ads { get; set; }
	public IList<UserAdSelection> SelectedAds { get; set; }
	public IList<FeaturedAdTransaction> FeatureTransactions { get; set; }

	public AppUser()
	{
		EmailConfirmed = false;
		RefreshTokenExpiresAt = DateTimeOffset.MinValue;
		ArchivedAt = DateTimeOffset.MinValue;
		CreatedAt = DateTimeOffset.UtcNow;
		UpdatedAt = CreatedAt;
	}
}
