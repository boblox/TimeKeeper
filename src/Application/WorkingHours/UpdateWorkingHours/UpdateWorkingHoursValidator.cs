using FluentValidation;
using TimeKeeper.Application.Common.Models;

namespace TimeKeeper.Application.WorkingHours.UpdateWorkingHours
{
    public class UpdateWorkingHoursValidator : AbstractValidator<UpdateWorkingHoursCommand>
    {
        public UpdateWorkingHoursValidator()
        {
            RuleFor(v => v.Description).NotEmpty().MaximumLength(200);
            RuleFor(v => v.Duration).Must(span => span <= DurationConstants.MaxDuration)
                .WithMessage($"Duration should be less or equal than {DurationConstants.MaxDuration.TotalHours} hours");
        }
    }
}
