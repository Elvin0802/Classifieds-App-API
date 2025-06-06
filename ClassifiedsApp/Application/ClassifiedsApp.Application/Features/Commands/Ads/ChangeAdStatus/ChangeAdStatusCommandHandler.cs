﻿using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using ClassifiedsApp.Core.Entities;
using ClassifiedsApp.Core.Enums;
using MediatR;

namespace ClassifiedsApp.Application.Features.Commands.Ads.ChangeAdStatus;

public class ChangeAdStatusCommandHandler : IRequestHandler<ChangeAdStatusCommand, Result>
{
	readonly IAdReadRepository _adReadRepository;
	readonly IAdWriteRepository _adWriteRepository;

	public ChangeAdStatusCommandHandler(IAdReadRepository adReadRepository,
										IAdWriteRepository adWriteRepository)
	{
		_adReadRepository = adReadRepository;
		_adWriteRepository = adWriteRepository;
	}

	public async Task<Result> Handle(ChangeAdStatusCommand request, CancellationToken cancellationToken)
	{
		try
		{
			Ad ad = await _adReadRepository.GetAdByIdWithIncludesAsync(request.AdId)
					?? throw new KeyNotFoundException("Ad Not Found.");

			ad.Status = request.NewAdStatus;
			ad.UpdatedAt = DateTimeOffset.UtcNow;

			if (ad.Status == AdStatus.Active)
				ad.ExpiresAt = ad.UpdatedAt.AddDays(30);

			_adWriteRepository.Update(ad);
			await _adWriteRepository.SaveAsync();

			return Result.Success("Ad Status Changed.");
		}
		catch (Exception ex)
		{
			return Result.Failure($"Error occoured. {ex.Message}");
		}
	}
}
