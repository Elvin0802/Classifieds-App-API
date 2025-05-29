using FluentValidation;

namespace ClassifiedsApp.Application.Features.Commands.Ads.CreateAd;

public class CreateAdCommandValidator : AbstractValidator<CreateAdCommand>
{
	public CreateAdCommandValidator()
	{
		RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Title is required")
				.MinimumLength(4).WithMessage("A valid Title is required");

		RuleFor(x => x.Description)
		.NotEmpty().WithMessage("Description is required")
		.MinimumLength(4).WithMessage("A valid Description is required");
	}
}
