using ClassifiedsApp.Application.Common.Consts;
using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Repositories.AdImages;
using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using ClassifiedsApp.Application.Interfaces.Services.Ads;
using ClassifiedsApp.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedsApp.Application.Features.Commands.Ads.UpdateAd;

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

			// İlan bilgilerini güncelle
			ad.Description = request.Description;
			ad.Price = request.Price;
			ad.IsNew = request.IsNew;
			ad.UpdatedAt = DateTimeOffset.UtcNow;

			/*
			// 1. Silinecek resimleri işle
			if (request.ImagesToDelete != null && request.ImagesToDelete.Any())
			{
				foreach (var imageIdToDelete in request.ImagesToDelete)
				{
					var imageToDelete = ad.Images.FirstOrDefault(i => i.Id == imageIdToDelete);
					if (imageToDelete != null)
					{
						// Azure Blob Storage'dan sil
						if (!string.IsNullOrEmpty(imageToDelete.BlobName))
						{
							await _adImageService.DeleteAsync(imageToDelete.BlobName, AzureContainerNames.ContainerName);
						}

						// DB'den sil
						await _adImageWriteRepository.RemoveAsync(imageToDelete.Id);
					}
				}
				await _adImageWriteRepository.SaveAsync();
			}

			int sortOrder = 0;

			// 2. Mevcut resimlerin sıralamasını güncelle
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

			// 3. Yeni resimleri ekle
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
			*/

			// İlanı güncelle
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
