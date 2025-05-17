using ClassifiedsApp.Application.Features.Commands.Auth.Login;
using FluentValidation;

namespace ClassifiedsApp.Application.Features.Commands.Auth.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
	public RegisterCommandValidator()
	{
		RuleFor(x => x.CreateAppUserDto.Email)
			.NotEmpty().WithMessage("Email is required")
			.EmailAddress().WithMessage("A valid email address is required");

		RuleFor(x => x.CreateAppUserDto.Name)
			.NotEmpty().WithMessage("Name is required")
			.MinimumLength(2).WithMessage("Name must be at least 2 characters");

		RuleFor(x => x.CreateAppUserDto.PhoneNumber)
			.NotEmpty().WithMessage("Phone Number is required")
			.MinimumLength(9).WithMessage("Phone Number must be at least 6 characters")
			.Matches("^(?!0000000000$)(?:070|077|012|055|050|099|010|051)\\d{7}$").WithMessage("Correct Phone Number Sample: 0501234567");

		// shifreni identity yoxlayir.
		RuleFor(x => x.CreateAppUserDto.Password)
			.NotEmpty().WithMessage("Password is required")
			.MinimumLength(6).WithMessage("Password must be at least 6 characters");
	}
}
