using FluentValidation;

namespace TimeKeeper.Application.Identity.Login
{
    public class GetTokenCommandValidator : AbstractValidator<GetTokenCommand>
    {
        public GetTokenCommandValidator()
        {
            RuleFor(v => v.UserName)
                .NotEmpty();
            RuleFor(v => v.Password)
                .NotEmpty();
        }
    }
}
