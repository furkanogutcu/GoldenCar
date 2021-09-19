using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IPaymentService
    {
        IDataResult<int> Pay(CreditCard creditCard, int customerId, decimal amount);
    }
}
