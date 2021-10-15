using System;
using Business.Constants;
using Entities.Models;
using FluentValidation;

namespace Business.ValidationRules.FluentValidation
{
    public class RentPaymentRequestValidator : AbstractValidator<RentPaymentRequestModel>
    {
        public RentPaymentRequestValidator()
        {
            RuleFor(r => r.CardNumber).NotNull();
            RuleFor(r => r.CardNumber).NotEmpty();
            RuleFor(r => r.CardNumber).Length(16);
            RuleFor(r => r.CardNumber).Must(CheckIfNumberString).WithMessage(Messages.StringMustConsistOfNumbersOnly);

            RuleFor(r => r.ExpireYear).NotNull();
            RuleFor(r => r.ExpireYear).NotEmpty();
            RuleFor(r => r.ExpireYear).Length(4);
            RuleFor(r => r.ExpireYear).Must(CheckIfNumberString).WithMessage(Messages.StringMustConsistOfNumbersOnly);

            RuleFor(r => r.ExpireMonth).NotNull();
            RuleFor(r => r.ExpireMonth).NotEmpty();
            RuleFor(r => r.ExpireMonth).Length(2);
            RuleFor(r => r.ExpireMonth).Must(CheckIfNumberString).WithMessage(Messages.StringMustConsistOfNumbersOnly);

            RuleFor(r => r.Cvc).NotNull();
            RuleFor(r => r.Cvc).NotEmpty();
            RuleFor(r => r.Cvc).Length(3);
            RuleFor(r => r.Cvc).Must(CheckIfNumberString).WithMessage(Messages.StringMustConsistOfNumbersOnly);

            RuleFor(r => r.CardHolderFullName).NotNull();
            RuleFor(r => r.CardHolderFullName).NotEmpty();
            RuleFor(r => r.CardHolderFullName).MinimumLength(5);
            RuleFor(r => r.CardHolderFullName).MaximumLength(50);

            RuleFor(r => r.CustomerId).NotEmpty();
            RuleFor(r => r.CustomerId).NotNull();
            RuleFor(r => r.CustomerId).GreaterThan(0);

            RuleFor(r => r.Amount).NotNull();
            RuleFor(r => r.Amount).NotEmpty();
            RuleFor(r => r.Amount).GreaterThanOrEqualTo(0);
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
