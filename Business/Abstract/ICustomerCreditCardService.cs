using System.Collections.Generic;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Models;

namespace Business.Abstract
{
    public interface ICustomerCreditCardService
    {
        IDataResult<List<CreditCard>> GetSavedCreditCardsByCustomerId(int customerId);
        IResult SaveCustomerCreditCard(CustomerCreditCardModel customerCreditCardModel);
        IResult DeleteCustomerCreditCard(CustomerCreditCardModel customerCreditCardModel);
    }
}
