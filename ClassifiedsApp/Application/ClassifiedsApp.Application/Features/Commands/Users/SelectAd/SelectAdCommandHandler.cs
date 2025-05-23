﻿using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Repositories.Ads;
using ClassifiedsApp.Application.Interfaces.Repositories.Users;
using ClassifiedsApp.Application.Interfaces.Services.Users;
using ClassifiedsApp.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedsApp.Application.Features.Commands.Users.SelectAdCommand;

public class SelectAdCommandHandler : IRequestHandler<SelectAdCommand, Result>
{
	readonly IUserAdSelectionWriteRepository _writeRepository;
	readonly IAdReadRepository _readRepository;
	readonly ICurrentUserService _currentUserService;

	public SelectAdCommandHandler(IUserAdSelectionWriteRepository writeRepository,
								  ICurrentUserService currentUserService,
								  IAdReadRepository readRepository)
	{
		_writeRepository = writeRepository;
		_currentUserService = currentUserService;
		_readRepository = readRepository;
	}

	public async Task<Result> Handle(SelectAdCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var item = await _writeRepository.Table
								.FirstOrDefaultAsync(
									uas => uas.AppUserId == _currentUserService.UserId!.Value && uas.AdId == request.SelectAdId,
									cancellationToken: cancellationToken);

			if (item is not null)
				throw new Exception("Ad selection found.");

			var ad = await _readRepository.Table.Where(ad => ad.Id == request.SelectAdId).FirstOrDefaultAsync() ??
					throw new Exception("Ad not found.");

			if (ad.AppUserId == _currentUserService.UserId)
				throw new NotSupportedException("Ad not selectable by creator user.");

			await _writeRepository.AddAsync(new UserAdSelection()
			{
				AppUserId = _currentUserService.UserId!.Value,
				AdId = request.SelectAdId
			});

			await _writeRepository.SaveAsync();

			return Result.Success("Ad selected by user.");
		}
		catch (Exception ex)
		{
			return Result.Failure($"Error occoured. {ex.Message}");
		}
	}
}

