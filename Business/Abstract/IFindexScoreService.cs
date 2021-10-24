using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IFindexScoreService
    {
        IDataResult<int> GetCustomerFindexScore(int customerId);
        IDataResult<int> GetCarMinFindexScore(int carId);
    }
}
