using FluentValidation;

namespace ClassifiedsApp.Application.Features.Commands.Users.VerifyEmail;

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
	public VerifyEmailCommandValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required")
			.EmailAddress().WithMessage("A valid email address is required");

	}
}
