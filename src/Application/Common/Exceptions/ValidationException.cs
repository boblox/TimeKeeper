using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace TimeKeeper.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        private const string GeneralPropertyName = "General";
        public IDictionary<string, string[]> Errors { get; }
        
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(params string[] errors)
            : this()
        {
            AddErrors(errors.Select(failure => (PropertyName: GeneralPropertyName, failure)));
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            AddErrors(failures.Select(failure => (failure.PropertyName, failure.ErrorMessage)));
        }

        private void AddErrors(IEnumerable<(string PropertyName, string ErrorMessage)> failures)
        {
            var failureGroups = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage);

            foreach (var failureGroup in failureGroups)
            {
                var propertyName = failureGroup.Key;
                var propertyFailures = failureGroup.ToArray();

                Errors.Add(propertyName, propertyFailures);
            }
        }
    }
}