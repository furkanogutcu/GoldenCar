using System.Collections.Generic;
using FluentValidation.Results;

namespace Core.Extensions.Exception
{
    public class ValidationErrorDetails : ErrorDetails
    {
        public IEnumerable<ValidationFailure> ValidationErrors { get; set; }
    }
}
