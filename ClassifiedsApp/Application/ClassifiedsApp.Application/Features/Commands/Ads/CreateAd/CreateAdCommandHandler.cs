﻿using ClassifiedsApp.Application.Common.Consts;
using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using ClassifiedsApp.Application.Interfaces.Services.Ads;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Core.Enums;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Ads.CreateAd;

public class CreateAdCommandHandler : IRequestHandler<CreateAdCommand, Result>
{
	readonly IAdWriteRepository _writeRepository;
	readonly IAdSubCategoryValueWriteRepository _adSubCategoryWriteRepository;
	readonly ICurrentUserService _currentUserService;
	readonly IAdImageService _adImageService;

	public CreateAdCommandHandler(IAdWriteRepository writeRepository,
								  IAdSubCategoryValueWriteRepository adSubCategoryWriteRepository,
								  ICurrentUserService currentUserService,
								  IAdImageService adImageService)
	{
		_writeRepository = writeRepository;
		_adSubCategoryWriteRepository = adSubCategoryWriteRepository;
		_currentUserService = currentUserService;
		_adImageService = adImageService;
	}

	public async Task<Result> Handle(CreateAdCommand request, CancellationToken cancellationToken)
	{
		try
		{
			Ad newAd = new()
			{
				Title = request.Title.Trim(),
				Description = request.Description.Trim(),
				Price = request.Price,
				IsNew = request.IsNew,
				CategoryId = request.CategoryId,
				MainCategoryId = request.MainCategoryId,
				LocationId = request.LocationId,
				AppUserId = _currentUserService.UserId!.Value,
				ExpiresAt = DateTimeOffset.MinValue,
				Status = AdStatus.Pending,
				SubCategoryValues = new List<AdSubCategoryValue>(),
				Images = new List<AdImage>()
			};

			foreach (var item in request.SubCategoryValues)
			{
				newAd.SubCategoryValues.Add(new()
				{
					AdId = newAd.Id,
					SubCategoryId = item.SubCategoryId,
					Value = item.Value
				});
			}

			if (request.Images is null || request.Images.Count < 1)
				throw new ArgumentNullException(nameof(request.Images), "Ad must have images.");

			int imageSortOrder = 0;

			foreach (var imageFile in request.Images)
			{
				var uploadResult = await _adImageService.ResizeAndUploadAsync(imageFile, AzureContainerNames.ContainerName);

				if (!uploadResult.Error)
				{
					newAd.Images.Add(new AdImage
					{
						AdId = newAd.Id,
						Url = uploadResult.Url!,
						BlobName = uploadResult.BlobName!,
						SortOrder = imageSortOrder++
					});
				}
			}

			await _writeRepository.AddAsync(newAd);
			await _adSubCategoryWriteRepository.AddRangeAsync(newAd.SubCategoryValues.ToList());
			await _writeRepository.SaveAsync();

			return Result.Success("Ad created successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure($"Error occoured. {ex.Message}");
		}
	}
}
