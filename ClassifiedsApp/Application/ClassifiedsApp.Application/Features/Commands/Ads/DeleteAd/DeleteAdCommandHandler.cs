using Azure;
using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using ClassifiedsApp.Application.Interfaces.Services.Ads;
using ClassifiedsApp.Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedsApp.Application.Features.Commands.Ads.DeleteAd;

public class DeleteAdCommandHandler : IRequestHandler<DeleteAdCommand, Result>
{
	readonly IAdWriteRepository _adWriteRepository;
	readonly IAdReadRepository _adReadRepository;
	readonly IAdImageService _adImageService;

	public DeleteAdCommandHandler(IAdWriteRepository adWriteRepository,
								  IAdReadRepository adReadRepository,
								  IAdImageService adImageService)
	{
		_adWriteRepository = adWriteRepository;
		_adReadRepository = adReadRepository;
		_adImageService = adImageService;
	}

	public async Task<Result> Handle(DeleteAdCommand request, CancellationToken cancellationToken)
	{
		try
		{
			//if (!await _adWriteRepository.RemoveAsync(request.Id))
			//	throw new KeyNotFoundException($"Ad not found.");

			//await _adWriteRepository.SaveAsync();

			//return Result.Success("Ad deleted successfully.");

			var adQuery = _adReadRepository.Table
										   .Where(ad => ad.Id == request.Id)
										   .Include(ad => ad.Images);

			var ad = await adQuery.FirstOrDefaultAsync(cancellationToken: cancellationToken)
					 ?? throw new KeyNotFoundException($"Ad not found.");

			// delete images from azure container
			//foreach (var image in ad.Images)
			//{
			//	if (!string.IsNullOrEmpty(image.BlobName))
			//		await _adImageService.DeleteAsync(image.BlobName, AzureContainerNames.ContainerName);
			//}

			// hard delete
			/*
			if (!await _adWriteRepository.RemoveAsync(ad.Id))
				throw new RequestFailedException($"Ad not deleted.");
				*/


			// soft delete
			ad.Status = AdStatus.Archived;
			ad.ArchivedAt = DateTime.UtcNow;

			if (!_adWriteRepository.Update(ad))
				throw new RequestFailedException($"Ad not deleted.");

			await _adWriteRepository.SaveAsync();

			return Result.Success("Ad deleted successfully.");
		}
		catch (Exception ex)
		{
			return Result.Failure($"Error occoured. {ex.Message}");
		}
	}
}
