using System;
using Business.Abstract;
using Core.Utilities.Results;

namespace Business.Concrete
{
    public class FindexScoreManager : IFindexScoreService
    {
        private readonly ICustomerService _customerService;
        private readonly ICarService _carService;

        public FindexScoreManager(ICustomerService customerService, ICarService carService)
        {
            _customerService = customerService;
            _carService = carService;
        }

        public IDataResult<int> GetCustomerFindexScore(int customerId)
        {
            var customerResult = IsCustomerIdExist(customerId);
            if (customerResult.Success)
            {
                //Simulated
                Random random = new Random();
                int randomFindexScore = Convert.ToInt16(random.Next(0, 1900));
                return new SuccessDataResult<int>(randomFindexScore);
            }

            return new ErrorDataResult<int>(-1, customerResult.Message);
        }

        public IDataResult<int> GetCarMinFindexScore(int carId)
        {
            var carResult = _carService.GetById(carId);
            if (!carResult.Success)
            {
                return new ErrorDataResult<int>(-1, carResult.Message);
            }

            return new SuccessDataResult<int>(carResult.Data.MinFindexScore);
        }

        private IResult IsCustomerIdExist(int customerId)
        {
            var result = _customerService.GetCustomerById(customerId);
            if (result.Success)
            {
                return new SuccessResult();
            }

            return new ErrorResult(result.Message);
        }
    }
}
