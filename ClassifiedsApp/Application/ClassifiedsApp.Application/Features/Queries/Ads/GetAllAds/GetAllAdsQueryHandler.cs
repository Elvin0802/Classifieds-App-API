﻿using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Ads;
using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedsApp.Application.Features.Queries.Ads.GetAllAds;

public class GetAllAdsQueryHandler : IRequestHandler<GetAllAdsQuery, Result<GetAllAdsQueryResponse>>
{
	private readonly IAdReadRepository _repository;

	public GetAllAdsQueryHandler(IAdReadRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<GetAllAdsQueryResponse>> Handle(GetAllAdsQuery request, CancellationToken cancellationToken)
	{
		try
		{
			// Start with base query
			var query = _repository.GetAll(false)
									.Include(ad => ad.Images)
									.Include(ad => ad.AppUser)
									.Include(ad => ad.Location)
									.Include(ad => ad.Category)
									.Include(ad => ad.MainCategory)
									.Include(ad => ad.SubCategoryValues)
										.ThenInclude(scv => scv.SubCategory)
									.Include(ad => ad.SelectorUsers)
										.ThenInclude(su => su.AppUser)
									.AsQueryable();

			if (!(request.AdStatus.HasValue) || request.AdStatus == AdStatus.Active)
				query = query.Where(ad => ad.Status == AdStatus.Active && ad.ExpiresAt > DateTimeOffset.UtcNow);
			else
				query = query.Where(ad => ad.Status == request.AdStatus!.Value);

			if (request.SearchedAppUserId.HasValue)
			{
				query = query.Where(ad => ad.AppUserId == request.SearchedAppUserId.Value);

				request.PageSize = query.Count();
			}

			if (!(request.SearchedAppUserId.HasValue))
			{
				// get vip ads.
				if (request.IsFeatured.HasValue)
				{
					if (request.IsFeatured!.Value)
						query = query.Where(ad => ad.IsFeatured && ad.FeatureEndDate > DateTimeOffset.UtcNow)
									 .OrderByDescending(ad => ad.FeaturePriority)
									 .ThenByDescending(ad => ad.FeatureStartDate);
					else if (request.IsFeatured!.Value == false)
						query = query.Where(ad => ad.IsFeatured == false);
				}
			}

			if (request.IsNew.HasValue)
			{
				if (request.IsNew.Value)
					query = query.Where(ad => ad.IsNew == true);
				else
					query = query.Where(ad => ad.IsNew == false);
			}

			// Apply search filter
			var searchTerm = request.SearchTitle?.Trim().ToLower();

			if (!string.IsNullOrWhiteSpace(searchTerm))
			{
				var newQuery = query.Where(ad => ad.Title.ToLower().Contains(searchTerm));

				if (newQuery.Count() < 1)
					query = query.Where(ad => ad.Description.ToLower().Contains(searchTerm));
				else
					query = newQuery;
			}

			// Apply price filters
			if (request.MinPrice.HasValue)
				query = query.Where(ad => ad.Price >= request.MinPrice.Value);

			if (request.MaxPrice.HasValue)
				query = query.Where(ad => ad.Price <= request.MaxPrice.Value);

			// Apply category filters
			if (request.CategoryId.HasValue)
				query = query.Where(ad => ad.CategoryId == request.CategoryId.Value);

			if (request.MainCategoryId.HasValue)
				query = query.Where(ad => ad.MainCategoryId == request.MainCategoryId.Value);

			// Apply location filters
			if (request.LocationId.HasValue)
				query = query.Where(ad => ad.LocationId == request.LocationId.Value);

			if (request.SubCategoryValues is not null && request.SubCategoryValues.Count > 0)
				foreach (var v in request.SubCategoryValues)
					query = query.Where(ad => ad.SubCategoryValues.Any(scv => scv.SubCategoryId == v.Key && scv.Value == v.Value));

			// Apply sorting
			query = ApplySorting(query, request.SortBy, request.IsDescending);

			// Get total count before pagination
			var totalCount = await query.CountAsync(cancellationToken);

			// Apply pagination
			var paginatedQuery = query
				.Skip((request.PageNumber - 1) * request.PageSize)
				.Take(request.PageSize);

			// Get results
			var list = await paginatedQuery
				.Select(p => new AdPreviewDto
				{
					Id = p.Id,
					Title = p.Title,
					Price = p.Price,
					LocationCityName = p.Location.City,
					IsNew = p.IsNew,
					IsFeatured = p.IsFeatured,
					MainImageUrl = p.Images.FirstOrDefault(img => img.SortOrder == 0)!.Url,
					IsSelected = request.CurrentAppUserId.HasValue && p.SelectorUsers.Any(su => su.AppUserId == request.CurrentAppUserId.Value),
					IsOwner = p.AppUserId == request.CurrentAppUserId,
					CreatedAt = p.CreatedAt,
					UpdatedAt = p.UpdatedAt,
					Status = p.Status,
					ExpiresAt = p.ExpiresAt
				})
				.ToListAsync(cancellationToken);

			var data = new GetAllAdsQueryResponse()
			{
				Items = list,
				PageNumber = request.PageNumber,
				PageSize = request.PageSize,
				TotalCount = totalCount
			};

			return Result.Success(data, "Ads retrieved successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure<GetAllAdsQueryResponse>($"Failed to retrieve Ads: {ex.Message}");
		}
	}

	private IQueryable<Ad> ApplySorting(IQueryable<Ad> query, string? sortBy, bool isDescending)
	{
		if (string.IsNullOrWhiteSpace(sortBy))
		{
			return isDescending
				? query.OrderByDescending(p => p.CreatedAt)
				: query.OrderBy(p => p.CreatedAt);
		}

		switch (sortBy.ToLower())
		{
			case "price":
				return isDescending
					? query.OrderByDescending(p => p.Price)
					: query.OrderBy(p => p.Price);

			case "title":
				return isDescending
					? query.OrderByDescending(p => p.Title)
					: query.OrderBy(p => p.Title);

			case "updatedat":
				return isDescending
					? query.OrderByDescending(p => p.UpdatedAt)
					: query.OrderBy(p => p.UpdatedAt);

			case "viewcount":
				return isDescending
					? query.OrderByDescending(p => p.ViewCount)
					: query.OrderBy(p => p.ViewCount);

			default:
				return isDescending
					? query.OrderByDescending(p => p.CreatedAt)
					: query.OrderBy(p => p.CreatedAt);
		}
	}
}