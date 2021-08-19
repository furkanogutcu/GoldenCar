using System;
using System.Collections.Generic;
using System.Linq;
using Business.Abstract;
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
    public class RentalManager : IRentalService
    {
        private readonly IRentalDal _rentalDal;

        public RentalManager(IRentalDal rentalDal)
        {
            _rentalDal = rentalDal;
        }

        public IDataResult<List<Rental>> GetAll()
        {
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(), Messages.RentalsListed);
        }

        public IDataResult<Rental> GetRentalById(int rentalId)
        {
            return new SuccessDataResult<Rental>(_rentalDal.Get(r => r.Id == rentalId), Messages.RentalListed);
        }

        public IDataResult<List<Rental>> GetCanBeRented()
        {
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(r => r.ReturnDate < DateTime.Now.Date),
                Messages.RentalsListed);
        }

        public IDataResult<List<RentalDetailDto>> GetRentalsDetails()
        {
            return new SuccessDataResult<List<RentalDetailDto>>(_rentalDal.GetRentalsDto(), Messages.RentalsListed);
        }

        [ValidationAspect(typeof(RentalValidator))]
        public IResult Add(Rental rental)
        {
            if (!IsCarAvailable(rental))
            {
                return new ErrorResult(Messages.RentalCarNotAvailable);
            }
            _rentalDal.Add(rental);
            return new SuccessResult(Messages.RentalAdded);
        }

        [ValidationAspect(typeof(RentalValidator))]
        public IResult Update(Rental rental)
        {
            var rulesResult = BusinessRules.Run(CheckIfRentalIdExist(rental.Id));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            _rentalDal.Update(rental);
            return new SuccessResult(Messages.RentalUpdated);
        }


        public IResult Delete(int rentalId)
        {
            var rulesResult = BusinessRules.Run(CheckIfRentalIdExist(rentalId));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            var deletedRental = _rentalDal.Get(r => r.Id == rentalId);
            _rentalDal.Delete(deletedRental);
            return new SuccessResult(Messages.RentalDeleted);
        }

        //Business Rules

        private bool IsCarAvailable(Rental rental)
        {
            return !(_rentalDal.GetAll(r => r.CarId == rental.CarId && r.ReturnDate == null).Any());
        }

        private IResult CheckIfRentalIdExist(int rentalId)
        {
            var result = _rentalDal.GetAll(r => r.Id == rentalId).Any();
            if (!result)
            {
                return new ErrorResult(Messages.RentalNotExist);
            }
            return new SuccessResult();
        }
    }
}
