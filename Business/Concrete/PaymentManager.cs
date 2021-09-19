using System;
using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Core.Aspects.Autofac.Transaction;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class PaymentManager : IPaymentService
    {
        private readonly IPaymentDal _paymentDal;
        private readonly ICreditCardService _creditCardService;

        public PaymentManager(IPaymentDal paymentDal, ICreditCardService creditCardService)
        {
            _paymentDal = paymentDal;
            _creditCardService = creditCardService;
        }

        //[SecuredOperation("admin,payment.all,payment.pay,customer")]
        [TransactionScopeAspect]
        public IDataResult<int> Pay(CreditCard creditCard, int customerId, decimal amount)
        {
            //Validate credit card
            var result = _creditCardService.Validate(creditCard);

            if (result.Success)
            {
                if (creditCard.Balance < amount)
                {
                    return new ErrorDataResult<int>(-1, Messages.InsufficientCardBalance);
                }
                creditCard.Balance -= amount;
                _creditCardService.Update(creditCard);
                DateTime paymentDate = DateTime.Now;
                _paymentDal.Add(new Payment
                {
                    CustomerId = customerId,
                    CreditCardId = creditCard.Id,
                    Amount = amount,
                    PaymentDate = paymentDate
                });
                var paymentId = _paymentDal.Get(p => p.CustomerId == customerId && p.Amount == amount && p.CreditCardId == creditCard.Id && (p.PaymentDate.Date == paymentDate.Date && p.PaymentDate.Hour == paymentDate.Hour && p.PaymentDate.Second == paymentDate.Second)).Id;
                return new SuccessDataResult<int>(paymentId, Messages.PaymentSuccessful);
            }

            return new ErrorDataResult<int>(-1, result.Message);
        }
    }
}
