using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class BrandValidator : AbstractValidator<Brand>
    {
        public BrandValidator()
        {
            RuleFor(b => b.Name).NotEmpty();
            RuleFor(b => b.Name).NotNull();
            RuleFor(b => b.Name).MinimumLength(2);
            RuleFor(b => b.Name).MaximumLength(10);
        }
    }
}
