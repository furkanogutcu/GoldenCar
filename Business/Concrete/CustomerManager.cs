using System.Collections.Generic;
using System.Linq;
using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Concrete
{
    public class CustomerManager : ICustomerService
    {
        private readonly ICustomerDal _customerDal;
        private readonly IUserService _userService;

        public CustomerManager(ICustomerDal customerDal, IUserService userService)
        {
            _customerDal = customerDal;
            _userService = userService;
        }

        [SecuredOperation("admin,customer.all,customer.list")]
        [CacheAspect(10)]
        public IDataResult<List<Customer>> GetAll()
        {
            return new SuccessDataResult<List<Customer>>(_customerDal.GetAll(), Messages.CustomersListed);
        }

        [SecuredOperation("admin,customer.all,customer.list")]
        [CacheAspect(10)]
        public IDataResult<Customer> GetCustomerById(int customerId)
        {
            var result = _customerDal.Get(c => c.Id == customerId);
            if (result != null)
            {
                return new SuccessDataResult<Customer>(result, Messages.CustomerListed);
            }

            return new ErrorDataResult<Customer>(null, Messages.CustomerNotExist);
        }

        //[SecuredOperation("admin,customer.all,customer.list")]
        [CacheAspect(10)]
        public IDataResult<Customer> GetCustomerByUserId(int userId)
        {
            var result = _customerDal.Get(c => c.UserId == userId);
            if (result != null)
            {
                return new SuccessDataResult<Customer>(result, Messages.CustomerListed);
            }

            return new ErrorDataResult<Customer>(null, Messages.CustomerNotExist);
        }

        [SecuredOperation("admin,customer.all,customer.list")]
        [CacheAspect(10)]
        public IDataResult<List<CustomerDetailDto>> GetCustomersDetails()
        {
            return new SuccessDataResult<List<CustomerDetailDto>>(_customerDal.GetCustomersDetail(), Messages.CustomersListed);
        }

        [SecuredOperation("admin,customer.all,customer.add")]
        [ValidationAspect(typeof(CustomerValidator))]
        [CacheRemoveAspect("ICustomerService.Get")]
        public IDataResult<int> Add(Customer customer)
        {
            var rulesResult = BusinessRules.Run(CheckIfUserIdValid(customer.UserId), CheckIfUserIdExist(customer.UserId));
            if (rulesResult != null)
            {
                return new ErrorDataResult<int>(-1, rulesResult.Message);
            }

            _customerDal.Add(customer);
            var result = _customerDal.Get(c => c.UserId == customer.UserId && c.CompanyName == customer.CompanyName);
            if (result != null)
            {
                return new SuccessDataResult<int>(result.Id, Messages.CustomerAdded);
            }

            return new ErrorDataResult<int>(-1, Messages.NotAddedCustomer);
        }

        [SecuredOperation("admin,customer.all,customer.update")]
        [ValidationAspect(typeof(CustomerValidator))]
        [CacheRemoveAspect("ICustomerService.Get")]
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
        [CacheRemoveAspect("ICustomerService.Get")]
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

        private IResult CheckIfUserIdValid(int userId)
        {
            var result = _userService.GetUserById(userId);
            if (!result.Success)
            {
                return new ErrorResult(Messages.UserNotExist);
            }

            return new SuccessResult();
        }
    }
}
