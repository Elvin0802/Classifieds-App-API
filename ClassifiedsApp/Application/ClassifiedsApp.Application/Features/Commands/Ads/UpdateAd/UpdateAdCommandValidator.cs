using FluentValidation;

namespace ClassifiedsApp.Application.Features.Commands.Ads.UpdateAd;

public class UpdateAdCommandValidator : AbstractValidator<UpdateAdCommand>
{
	public UpdateAdCommandValidator()
	{
		RuleFor(x => x.Description)
		.NotEmpty().WithMessage("Description is required")
		.MinimumLength(4).WithMessage("A valid Description is required");

	}
}
