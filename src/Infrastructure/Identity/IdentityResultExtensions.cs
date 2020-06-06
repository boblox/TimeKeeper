using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using TimeKeeper.Application.Common.Exceptions;

namespace TimeKeeper.Infrastructure.Identity
{
    public static class IdentityResultExtensions
    {
        public static ValidationException ToValidationException(this IEnumerable<IdentityError> errors)
        {
            return new ValidationException(errors.Select(e => e.Description).ToArray());
        }
    }
}