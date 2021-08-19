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
        IDataResult<List<CustomerDetailDto>> GetCustomersDetails();
        IResult Add(Customer customer);
        IResult Delete(int customerId);
        IResult Update(Customer customer);
    }
}
