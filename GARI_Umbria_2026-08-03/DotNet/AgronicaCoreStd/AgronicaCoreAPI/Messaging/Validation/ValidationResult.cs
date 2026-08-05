using System.Collections.Generic;
using System.Linq;

namespace AgronicaCoreAPI.Messaging.Validation
{
    public class ValidationResult
    {
        public static readonly ValidationResult Ok = new(new List<ValidationError>());

        public bool IsValid => !_errors.Any();
        public IReadOnlyList<ValidationError> Errors => _errors;

        private readonly List<ValidationError> _errors;

        private ValidationResult(List<ValidationError> errors) => _errors = errors;

        public static ValidationResult Failure(IEnumerable<ValidationError> errors) =>
            new(new List<ValidationError>(errors));

        public static ValidationResult Failure(string field, string code, string message) =>
            new(new List<ValidationError> { new(field, code, message) });
    }
}
