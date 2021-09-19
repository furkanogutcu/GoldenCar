using System;
using Business.Constants;
using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class CreditCardValidator : AbstractValidator<CreditCard>
    {
        public CreditCardValidator()
        {
            RuleFor(c => c.CardNumber).NotNull();
            RuleFor(c => c.CardNumber).NotEmpty();
            RuleFor(c => c.CardNumber).Length(16);
            RuleFor(c => c.CardNumber).Must(CheckIfNumberString).WithMessage(Messages.StringMustConsistOfNumbersOnly);

            RuleFor(c => c.ExpireYear).NotNull();
            RuleFor(c => c.ExpireYear).NotEmpty();
            RuleFor(c => c.ExpireYear).Length(4);
            RuleFor(c => c.ExpireYear).Must(CheckIfNumberString).WithMessage(Messages.StringMustConsistOfNumbersOnly);

            RuleFor(c => c.ExpireMonth).NotNull();
            RuleFor(c => c.ExpireMonth).NotEmpty();
            RuleFor(c => c.ExpireMonth).Length(2);
            RuleFor(c => c.ExpireMonth).Must(CheckIfNumberString).WithMessage(Messages.StringMustConsistOfNumbersOnly);

            RuleFor(c => c.Cvc).NotNull();
            RuleFor(c => c.Cvc).NotEmpty();
            RuleFor(c => c.Cvc).Length(3);
            RuleFor(c => c.Cvc).Must(CheckIfNumberString).WithMessage(Messages.StringMustConsistOfNumbersOnly);

            RuleFor(c => c.CardHolderFullName).NotNull();
            RuleFor(c => c.CardHolderFullName).NotEmpty();
            RuleFor(c => c.CardHolderFullName).MinimumLength(5);
            RuleFor(c => c.CardHolderFullName).MaximumLength(50);
        }
        private bool CheckIfNumberString(string input)
        {
            foreach (var chr in input)
            {
                if (!Char.IsNumber(chr))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
