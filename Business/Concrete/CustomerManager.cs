using System.Collections.Generic;
using System.Linq;
using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Concrete
{
    public class CustomerManager:ICustomerService
    {
        private readonly ICustomerDal _customerDal;

        public CustomerManager(ICustomerDal customerDal)
        {
            _customerDal = customerDal;
        }

        [SecuredOperation("admin,customer.all,customer.list")]
        public IDataResult<List<Customer>> GetAll()
        {
            return new SuccessDataResult<List<Customer>>(_customerDal.GetAll(), Messages.CustomersListed);
        }

        [SecuredOperation("admin,customer.all,customer.list")]
        public IDataResult<Customer> GetCustomerById(int customerId)
        {
            return new SuccessDataResult<Customer>(_customerDal.Get(c=>c.Id == customerId), Messages.CustomerListed);
        }

        [SecuredOperation("admin,customer.all,customer.list")]
        public IDataResult<List<CustomerDetailDto>> GetCustomersDetails()
        {
            return new SuccessDataResult<List<CustomerDetailDto>>(_customerDal.GetCustomersDetail(), Messages.CustomersListed);
        }

        [SecuredOperation("admin,customer.all,customer.add")]
        [ValidationAspect(typeof(CustomerValidator))]
        public IResult Add(Customer customer)
        {
            var rulesResult = BusinessRules.Run(CheckIfUserIdExist(customer.UserId));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            _customerDal.Add(customer);
            return new SuccessResult(Messages.CustomerAdded);
        }

        [SecuredOperation("admin,customer.all,customer.update")]
        [ValidationAspect(typeof(CustomerValidator))]
        public IResult Update(Customer customer)
        {
            var rulesResult = BusinessRules.Run(CheckIfCustomerIdExist(customer.Id));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            _customerDal.Update(customer);
            return new SuccessResult(Messages.CustomerUpdated);
        }

        [SecuredOperation("admin,customer.all,customer.delete")]
        public IResult Delete(int customerId)
        {
            var rulesResult = BusinessRules.Run(CheckIfCustomerIdExist(customerId));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            var deletedCustomer = _customerDal.Get(c => c.Id == customerId);
            _customerDal.Delete(deletedCustomer);
            return new SuccessResult(Messages.CustomerDeleted);
        }

        //Business Rules

        private IResult CheckIfCustomerIdExist(int customerId)
        {
            var result = _customerDal.GetAll(c => c.Id == customerId).Any();
            if (!result)
            {
                return new ErrorResult(Messages.CustomerNotExist);
            }
            return new SuccessResult();
        }

        private IResult CheckIfUserIdExist(int userId)
        {
            var result = _customerDal.GetAll(c => c.UserId == userId).Any();
            if (result)
            {
                return new ErrorResult(Messages.UserAlreadyCustomer);
            }
            return new SuccessResult();
        }
    }
}
