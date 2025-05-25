using FluentValidation;

namespace ClassifiedsApp.Application.Features.Commands.Auth.PasswordReset;

public class PasswordResetCommandValidator : AbstractValidator<PasswordResetCommand>
{
	public PasswordResetCommandValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required")
			.EmailAddress().WithMessage("A valid email address is required");
	}
}