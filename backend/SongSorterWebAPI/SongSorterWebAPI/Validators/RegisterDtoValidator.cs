using FluentValidation;
using SongSorterWebAPI.DTOs;

namespace SongSorterWebAPI.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email є обов'язковим.")
                .EmailAddress().WithMessage("Невірний формат Email.");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username є обов'язковим.")
                .MinimumLength(3).WithMessage("Username має містити мінімум 3 символи.")
                .MaximumLength(20).WithMessage("Username занадто довгий.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Паролі не співпадають.");
        }
    }
}
