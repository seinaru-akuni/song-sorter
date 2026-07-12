using FluentValidation;
using SongSorterWebAPI.DTOs;

namespace SongSorterWebAPI.Validators
{
    public class VerifyEmailDtoValidator : AbstractValidator<VerifyEmailDto>
    {
        public VerifyEmailDtoValidator() 
        {

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Confirm Password є обов'язковим.");
                

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Confirm Password є обов'язковим.")
                .Equal(x => x.NewPassword).WithMessage("Паролі не співпадають.");

        }
    }
}
