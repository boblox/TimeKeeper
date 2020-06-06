using FluentValidation;
using TimeKeeper.Application.Identity.Login;

namespace TimeKeeper.Application.Identity.Register
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            //TODO: it would be handled by IdentityService itself
            //RuleFor(v => v.UserName)
            //    .NotEmpty();
            //RuleFor(v => v.Password)
            //    .NotEmpty();
        }
    }
}
