using System;
using FluentValidation;

namespace TimeKeeper.Application.PreferredWorkingHours.UpdatePreferredWorkingHours
{
    public class UpdatePreferredWorkingHoursValidator : AbstractValidator<UpdatePreferredWorkingHoursCommand>
    {
        static readonly TimeSpan MaxDuration = new TimeSpan(24, 00, 00);

        public UpdatePreferredWorkingHoursValidator()
        {
            RuleFor(v => v.Duration).Must(span => span <= MaxDuration)
                .WithMessage("Duration should be less or equal than 24 hours");
        }
    }
}
