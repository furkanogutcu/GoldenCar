using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.CrossCuttingConcerns.Validation.FluentValidation;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Concrete
{
    public class RentalManager:IRentalService
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

        public IResult Add(Rental rental)
        {
            ValidationTool.Validate(new RentalValidator(), rental);
            if (!IsCarAvailable(rental))
            {
                return new ErrorResult(Messages.RentalCarNotAvailable);
            }
            _rentalDal.Add(rental);
            return new SuccessResult(Messages.RentalAdded);
        }

        public IResult Delete(Rental rental)
        {
            ValidationTool.Validate(new RentalValidator(), rental);
            _rentalDal.Delete(rental);
            return new SuccessResult(Messages.RentalDeleted);
        }

        public IResult Update(Rental rental)
        {
            ValidationTool.Validate(new RentalValidator(), rental);
            _rentalDal.Update(rental);
            return new SuccessResult(Messages.RentalUpdated);
        }

        private bool IsCarAvailable(Rental rental)
        {
            var carRentalHistory = _rentalDal.GetAll(r => r.CarId == rental.CarId);
            if (carRentalHistory.Any(c => c.ReturnDate == null))
            {
                return false;
            }
            return true;
        }
    }
}
