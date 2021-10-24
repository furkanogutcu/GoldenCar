using System.Collections.Generic;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface ICustomerService
    {
        IDataResult<List<Customer>> GetAll();
        IDataResult<Customer> GetCustomerById(int customerId);
        IDataResult<Customer> GetCustomerByUserId(int userId);
        IDataResult<List<CustomerDetailDto>> GetCustomersDetails();
        IDataResult<int> Add(Customer customer);
        IResult Delete(int customerId);
        IResult Update(Customer customer);
    }
}
