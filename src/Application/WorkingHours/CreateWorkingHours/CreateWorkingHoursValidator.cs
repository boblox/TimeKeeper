using System;
using FluentValidation;
using TimeKeeper.Application.Common.Models;

namespace TimeKeeper.Application.WorkingHours.CreateWorkingHours
{
    public class CreateWorkingHoursValidator : AbstractValidator<CreateWorkingHoursCommand>
    {
        public CreateWorkingHoursValidator()
        {
            RuleFor(v => v.Description).NotEmpty().MaximumLength(200);
            RuleFor(v => v.Duration).Must(span => span <= DurationConstants.MaxDuration)
                .WithMessage($"Duration should be less or equal than {DurationConstants.MaxDuration.TotalHours} hours");
        }
    }
}
