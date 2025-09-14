using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Application.Dtos.Validators
{
    public class MemberUpdateDtoValidator : AbstractValidator<MemberUpdateDto>
    {
        public MemberUpdateDtoValidator()
        {
            // Rule for FirstName: Optional, but if provided, must be between 2 and 100 characters
            RuleFor(x => x.FirstName)
                .NotEmpty().When(x => x.FirstName != null) // Validate if it's not null
                .WithMessage("First name must not be empty.")
                .Length(2, 100).When(x => x.FirstName != null)
                .WithMessage("First name must be between 2 and 100 characters.");

            // Rule for LastName: Optional, but if provided, must be between 2 and 100 characters
            RuleFor(x => x.LastName)
                .NotEmpty().When(x => x.LastName != null)
                .WithMessage("Last name must not be empty.")
                .Length(2, 100).When(x => x.LastName != null)
                .WithMessage("Last name must be between 2 and 100 characters.");

            // Rule for Email: Optional, but if provided, must be a valid email format
            RuleFor(x => x.Email)
                .NotEmpty().When(x => x.Email != null)
                .WithMessage("Email must not be empty.")
                .EmailAddress().When(x => x.Email != null)
                .WithMessage("A valid email address is required.");

            // Rule for DateOfBirth: Optional, but if provided, must be a valid date in the past
            RuleFor(x => x.DateOfBirth)
                .Must(BeAValidDateInThePast).When(x => x.DateOfBirth.HasValue)
                .WithMessage("Date of birth must be a valid date in the past.");
        }

        // Custom validation method
        private bool BeAValidDateInThePast(DateTime? date)
        {
            if (!date.HasValue) return true; // If no date provided, it's valid (it's optional)
            return date.Value < DateTime.UtcNow;
        }
    }
}
