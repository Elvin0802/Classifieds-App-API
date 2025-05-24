using ClassifiedsApp.Application.Common.Consts;
using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Ads;
using ClassifiedsApp.Application.Interfaces.Repositories.AdImages;
using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using ClassifiedsApp.Application.Interfaces.Services.Ads;
using ClassifiedsApp.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedsApp.Application.Features.Commands.Ads.UpdateAd;

/*

public class UpdateAdCommandHandler : IRequestHandler<UpdateAdCommand, Result>
{
	readonly IAdWriteRepository _writeRepository;
	readonly IAdReadRepository _readRepository;
	readonly IAdImageService _adImageService;
	readonly IAdImageWriteRepository _adImageWriteRepository;

	public UpdateAdCommandHandler(IAdWriteRepository writeRepository,
								  IAdReadRepository readRepository,
								  IAdImageService adImageService,
								  IAdImageWriteRepository adImageWriteRepository)
	{
		_writeRepository = writeRepository;
		_readRepository = readRepository;
		_adImageService = adImageService;
		_adImageWriteRepository = adImageWriteRepository;
	}

	public async Task<Result> Handle(UpdateAdCommand request, CancellationToken cancellationToken)
	{
		try
		{
			Ad ad = await _readRepository.Table.Include(ad => ad.Images)
											   .FirstOrDefaultAsync(ad => ad.Id == request.Id, cancellationToken: cancellationToken)
					?? throw new KeyNotFoundException("Ad Not Found.");

			ad.Description = request.Description;
			ad.Price = request.Price;
			ad.IsNew = request.IsNew;
			ad.UpdatedAt = DateTimeOffset.UtcNow;

			if (request.ImagesToDelete != null && request.ImagesToDelete.Any())
			{
				foreach (var imageIdToDelete in request.ImagesToDelete)
				{
					var imageToDelete = ad.Images.FirstOrDefault(i => i.Id == imageIdToDelete);
					if (imageToDelete != null)
					{
						if (!string.IsNullOrEmpty(imageToDelete.BlobName))
						{
							await _adImageService.DeleteAsync(imageToDelete.BlobName, AzureContainerNames.ContainerName);
						}

						await _adImageWriteRepository.RemoveAsync(imageToDelete.Id);
					}
				}
				await _adImageWriteRepository.SaveAsync();
			}

			int sortOrder = 0;

			if (request.ExistingImages != null && request.ExistingImages.Any())
			{
				foreach (var updatedImageId in request.ExistingImages)
				{
					var existingImage = ad.Images.FirstOrDefault(i => i.Id == updatedImageId);
					if (existingImage != null)
					{
						existingImage.SortOrder = sortOrder++;
						_adImageWriteRepository.Update(existingImage);
					}
				}
				await _adImageWriteRepository.SaveAsync();
			}

			if (request.NewImages != null && request.NewImages.Any())
			{
				foreach (var newImage in request.NewImages)
				{
					var uploadResult = await _adImageService.ResizeAndUploadAsync(newImage, AzureContainerNames.ContainerName);

					if (!uploadResult.Error)
					{
						await _adImageWriteRepository.AddAsync(new AdImage
						{
							AdId = ad.Id,
							Url = uploadResult.Url!,
							BlobName = uploadResult.BlobName!,
							SortOrder = sortOrder++
						});
					}
				}
			}

			_writeRepository.Update(ad);
			await _writeRepository.SaveAsync();

			return Result.Success("Ad updated successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure($"Error occurred: {ex.Message}");
		}
	}

}



*/


public class UpdateAdCommandHandler : IRequestHandler<UpdateAdCommand, Result>
{
	readonly IAdWriteRepository _writeRepository;
	readonly IAdReadRepository _readRepository;
	readonly IAdImageService _adImageService;
	readonly IAdImageWriteRepository _adImageWriteRepository;
	readonly IAdSubCategoryValueWriteRepository _adSubCategoryWriteRepository;
	readonly IAdSubCategoryValueReadRepository _adSubCategoryReadRepository;

	public UpdateAdCommandHandler(IAdWriteRepository writeRepository,
								  IAdReadRepository readRepository,
								  IAdImageService adImageService,
								  IAdImageWriteRepository adImageWriteRepository,
								  IAdSubCategoryValueWriteRepository adSubCategoryWriteRepository,
								  IAdSubCategoryValueReadRepository adSubCategoryReadRepository)
	{
		_writeRepository = writeRepository;
		_readRepository = readRepository;
		_adImageService = adImageService;
		_adImageWriteRepository = adImageWriteRepository;
		_adSubCategoryWriteRepository = adSubCategoryWriteRepository;
		_adSubCategoryReadRepository = adSubCategoryReadRepository;
	}

	public async Task<Result> Handle(UpdateAdCommand request, CancellationToken cancellationToken)
	{
		try
		{
			// Load ad with all related data
			Ad ad = await _readRepository.Table
						.Include(ad => ad.Images)
						.Include(ad => ad.SubCategoryValues)
						.FirstOrDefaultAsync(ad => ad.Id == request.Id, cancellationToken: cancellationToken)
					?? throw new KeyNotFoundException("Ad Not Found.");

			// Update basic properties
			ad.Description = request.Description;
			ad.Price = request.Price;
			ad.IsNew = request.IsNew;
			ad.UpdatedAt = DateTimeOffset.UtcNow;

			if (request.LocationId.HasValue)
			{
				ad.LocationId = request.LocationId.Value;
			}

			// Update categories if provided
			if (request.CategoryId.HasValue)
			{
				ad.CategoryId = request.CategoryId.Value;
			}

			if (request.MainCategoryId.HasValue)
			{
				ad.MainCategoryId = request.MainCategoryId.Value;
			}

			// Handle subcategory values update
			await UpdateSubCategoryValues(ad, request.SubCategoryValues, cancellationToken);

			// Handle image deletion
			await HandleImageDeletion(ad, request.ImagesToDelete);

			// Handle image reordering
			await HandleImageReordering(ad, request.ExistingImages);

			// Handle new image uploads
			int sortOrder = ad.Images?.Count ?? 0;
			await HandleNewImageUploads(ad, request.NewImages, sortOrder);

			// Save the ad changes
			_writeRepository.Update(ad);
			await _writeRepository.SaveAsync();

			return Result.Success("Ad updated successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure($"Error occurred: {ex.Message}");
		}
	}

	private async Task UpdateSubCategoryValues(Ad ad, IList<CreateAdSubCategoryValueDto> newSubCategoryValues, CancellationToken cancellationToken)
	{
		if (newSubCategoryValues == null || !newSubCategoryValues.Any())
			return;

		// Get existing subcategory values for this ad
		var existingValues = await _adSubCategoryReadRepository.Table
			.Where(x => x.AdId == ad.Id)
			.ToListAsync(cancellationToken);

		// Remove all existing subcategory values
		if (existingValues.Any())
		{
			foreach (var existingValue in existingValues)
			{
				await _adSubCategoryWriteRepository.RemoveAsync(existingValue.Id);
			}
			await _adSubCategoryWriteRepository.SaveAsync();
		}

		// Add new subcategory values
		var newValues = new List<AdSubCategoryValue>();
		foreach (var item in newSubCategoryValues)
		{
			newValues.Add(new AdSubCategoryValue
			{
				AdId = ad.Id,
				SubCategoryId = item.SubCategoryId,
				Value = item.Value
			});
		}

		if (newValues.Any())
		{
			await _adSubCategoryWriteRepository.AddRangeAsync(newValues);
			await _adSubCategoryWriteRepository.SaveAsync();
		}
	}

	private async Task HandleImageDeletion(Ad ad, IList<Guid>? imagesToDelete)
	{
		if (imagesToDelete == null || !imagesToDelete.Any())
			return;

		foreach (var imageIdToDelete in imagesToDelete)
		{
			var imageToDelete = ad.Images.FirstOrDefault(i => i.Id == imageIdToDelete);
			if (imageToDelete != null)
			{
				if (!string.IsNullOrEmpty(imageToDelete.BlobName))
				{
					await _adImageService.DeleteAsync(imageToDelete.BlobName, AzureContainerNames.ContainerName);
				}

				await _adImageWriteRepository.RemoveAsync(imageToDelete.Id);
			}
		}
		await _adImageWriteRepository.SaveAsync();
	}

	private async Task HandleImageReordering(Ad ad, IList<Guid>? existingImages)
	{
		if (existingImages == null || !existingImages.Any())
			return;

		int sortOrder = 0;
		foreach (var updatedImageId in existingImages)
		{
			var existingImage = ad.Images.FirstOrDefault(i => i.Id == updatedImageId);
			if (existingImage != null)
			{
				existingImage.SortOrder = sortOrder++;
				_adImageWriteRepository.Update(existingImage);
			}
		}
		await _adImageWriteRepository.SaveAsync();
	}

	private async Task HandleNewImageUploads(Ad ad, IList<IFormFile>? newImages, int startingSortOrder)
	{
		if (newImages == null || !newImages.Any())
			return;

		int sortOrder = startingSortOrder;
		foreach (var newImage in newImages)
		{
			var uploadResult = await _adImageService.ResizeAndUploadAsync(newImage, AzureContainerNames.ContainerName);

			if (!uploadResult.Error)
			{
				await _adImageWriteRepository.AddAsync(new AdImage
				{
					AdId = ad.Id,
					Url = uploadResult.Url!,
					BlobName = uploadResult.BlobName!,
					SortOrder = sortOrder++
				});
			}
		}
	}
}