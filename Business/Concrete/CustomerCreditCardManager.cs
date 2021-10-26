using System;
using System.Collections.Generic;
using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac.Transaction;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Models;

namespace Business.Concrete
{
    public class CustomerCreditCardManager : ICustomerCreditCardService
    {
        private readonly ICustomerCreditCardDal _customerCreditCardDal;
        private readonly ICreditCardService _creditCardService;

        public CustomerCreditCardManager(ICustomerCreditCardDal customerCreditCardDal, ICreditCardService creditCardService)
        {
            _customerCreditCardDal = customerCreditCardDal;
            _creditCardService = creditCardService;
        }

        public IDataResult<List<CreditCard>> GetSavedCreditCardsByCustomerId(int customerId)
        {
            var userCreditCardsResult = _customerCreditCardDal.GetAll(ucc => ucc.CustomerId == customerId);
            List<CreditCard> userCreditCards = new List<CreditCard>();
            foreach (var result in userCreditCardsResult)
            {
                var creditCard = _creditCardService.GetById(result.CreditCardId);
                if (creditCard.Success)
                {
                    userCreditCards.Add(creditCard.Data);
                }
                else
                {
                    return new ErrorDataResult<List<CreditCard>>(null, creditCard.Message);
                }
            }

            return new SuccessDataResult<List<CreditCard>>(userCreditCards, Messages.CustomersCreditCardsListed);
        }

        [TransactionScopeAspect]
        public IResult SaveCustomerCreditCard(CustomerCreditCardModel customerCreditCardModel)
        {
            var creditCardResult = _creditCardService.Get(customerCreditCardModel.CreditCard.CardNumber,
                                                    customerCreditCardModel.CreditCard.ExpireYear,
                                                    customerCreditCardModel.CreditCard.ExpireMonth,
                                                    customerCreditCardModel.CreditCard.Cvc,
                                                    customerCreditCardModel.CreditCard.CardHolderFullName.ToUpperInvariant());
            if (!creditCardResult.Success)
            {
                return new ErrorResult(creditCardResult.Message);
            }

            CustomerCreditCard customerCreditCard = new CustomerCreditCard
            {
                CustomerId = customerCreditCardModel.CustomerId,
                CreditCardId = creditCardResult.Data.Id
            };

            var customerCreditCardExist = _customerCreditCardDal.GetAll(ccc =>
                ccc.CustomerId == customerCreditCard.CustomerId && ccc.CreditCardId == customerCreditCard.CreditCardId);

            if (customerCreditCardExist.Count > 0) //If the customer has already saved the credit card
            {
                return new ErrorResult(Messages.CustomerCreditCardAlreadySaved);
            }

            _customerCreditCardDal.Add(customerCreditCard);

            var result = GetCustomerCreditCard(customerCreditCard);
            if (result.Success)
            {
                return new SuccessResult(Messages.CustomerCreditCardSaved);
            }

            return new ErrorResult(Messages.CustomerCreditCardFailedToSave);
        }

        [TransactionScopeAspect]
        public IResult DeleteCustomerCreditCard(CustomerCreditCardModel customerCreditCardModel)
        {
            var creditCardResult = _creditCardService.Get(customerCreditCardModel.CreditCard.CardNumber,
                                        customerCreditCardModel.CreditCard.ExpireYear,
                                        customerCreditCardModel.CreditCard.ExpireMonth,
                                        customerCreditCardModel.CreditCard.Cvc,
                                        customerCreditCardModel.CreditCard.CardHolderFullName.ToUpperInvariant());
            if (!creditCardResult.Success)
            {
                return new ErrorResult(Messages.CreditCardNotFound);
            }

            CustomerCreditCard customerCreditCard = new CustomerCreditCard
            {
                CustomerId = customerCreditCardModel.CustomerId,
                CreditCardId = creditCardResult.Data.Id
            };

            var result = GetCustomerCreditCard(customerCreditCard);
            if (result.Success)
            {
                _customerCreditCardDal.Delete(result.Data);
                var newResult = GetCustomerCreditCard(customerCreditCard);
                if (!newResult.Success)
                {
                    return new SuccessResult(Messages.CustomerCreditCardDeleted);
                }

                return new ErrorResult(Messages.CustomerCreditCardNotDeleted);
            }

            return new ErrorResult(Messages.CustomerCreditCardNotFound);
        }

        private IDataResult<CustomerCreditCard> GetCustomerCreditCard(CustomerCreditCard customerCreditCard)
        {
            var result =
                _customerCreditCardDal.Get(ccc => ccc.CustomerId == customerCreditCard.CustomerId &&
                                                  ccc.CreditCardId == customerCreditCard.CreditCardId);
            if (result != null)
            {
                return new SuccessDataResult<CustomerCreditCard>(result);
            }

            return new ErrorDataResult<CustomerCreditCard>();
        }
    }
}
