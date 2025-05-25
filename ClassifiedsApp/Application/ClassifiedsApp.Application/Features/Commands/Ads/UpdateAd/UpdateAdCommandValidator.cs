using ClassifiedsApp.Application.Features.Commands.Ads.CreateAd;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Application.Features.Commands.Ads.UpdateAd;

public class UpdateAdCommandValidator : AbstractValidator<UpdateAdCommand>
{
	public UpdateAdCommandValidator()
	{
		RuleFor(x => x.Description)
		.NotEmpty().WithMessage("Description is required")
		.MinimumLength(4).WithMessage("A valid Description is required");

		RuleFor(x => x.Price).GreaterThan(0);

	}
}
