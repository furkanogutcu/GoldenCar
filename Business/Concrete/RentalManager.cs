using System;
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
    public class RentalManager : IRentalService
    {
        private readonly IRentalDal _rentalDal;

        public RentalManager(IRentalDal rentalDal)
        {
            _rentalDal = rentalDal;
        }

        [SecuredOperation("admin,rental.all,rental.list")]
        [CacheAspect(10)]
        public IDataResult<List<Rental>> GetAll()
        {
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(), Messages.RentalsListed);
        }

        [SecuredOperation("admin,rental.all,rental.list")]
        [CacheAspect(10)]
        public IDataResult<Rental> GetRentalById(int rentalId)
        {
            return new SuccessDataResult<Rental>(_rentalDal.Get(r => r.Id == rentalId), Messages.RentalListed);
        }

        [SecuredOperation("admin,rental.all,rental.list")]
        public IDataResult<bool> CheckIfCanCarBeRentedNow(int carId)
        {
            var rulesResult = BusinessRules.Run(CheckIfCarAvailableNow(carId));
            if (rulesResult != null)
            {
                return new ErrorDataResult<bool>(false, rulesResult.Message);
            }
            return new SuccessDataResult<bool>(true);
        }

        [SecuredOperation("admin,rental.all,rental.list")]
        public IDataResult<bool> CheckIfAnyReservationsBetweenSelectedDates(int carId, DateTime rentDate, DateTime returnDate)
        {
            return CheckIfCarAvailableBetweenSelectedDatesForReservations(carId, rentDate, returnDate);
        }

        [SecuredOperation("admin,rental.all,rental.list")]
        [CacheAspect(10)]
        public IDataResult<List<RentalDetailDto>> GetRentalsDetails()
        {
            return new SuccessDataResult<List<RentalDetailDto>>(_rentalDal.GetRentalsDetails(), Messages.RentalsListed);
        }

        [SecuredOperation("admin,rental.all,rental.list")]
        [CacheAspect(10)]
        public IDataResult<List<RentalDetailDto>> GetRentalsByCustomerIdWithDetails(int customerId)
        {
            return new SuccessDataResult<List<RentalDetailDto>>(_rentalDal.GetRentalsDetails(r => r.CustomerId == customerId), Messages.RentalsListed);
        }

        [SecuredOperation("admin,rental.all,rental.add")]
        [ValidationAspect(typeof(RentalValidator))]
        [CacheRemoveAspect("IRentalService.Get")]
        public IResult Add(Rental rental)
        {
            IResult rulesResult;

            if (rental.RentDate > DateTime.Now) //Reservation
            {
                rulesResult = BusinessRules.Run(CheckIfCarAvailableBetweenSelectedDatesForReservations(rental.CarId, rental.RentDate, rental.ReturnDate),
                    DeliveryStatusShieldForAddOperation(rental));

                if (!CheckIfCarAvailableNow(rental.CarId).Success && rental.RentDate < GetReturnDateOfRentedCar(rental.CarId).Data)
                {
                    return new ErrorResult(Messages.CarAlreadyRentedByTheReservationDate);
                }
            }
            else //Rent now
            {
                rulesResult = BusinessRules.Run(CheckIfCarAvailableNow(rental.CarId),
                    DeliveryStatusShieldForAddOperation(rental));
            }
            if (rulesResult != null)
            {
                return rulesResult;
            }
            _rentalDal.Add(rental);
            return new SuccessResult(Messages.RentalAdded);
        }

        [SecuredOperation("admin,rental.all,rental.update")]
        [ValidationAspect(typeof(RentalValidator))]
        [CacheRemoveAspect("IRentalService.Get")]
        public IResult Update(Rental rental)
        {
            IResult rulesResult;

            if (rental.RentDate > DateTime.Now) //Reservation update
            {
                rulesResult = BusinessRules.Run(CheckIfCarAvailableBetweenSelectedDatesForCurrentRentals(rental.CarId, rental.RentDate, rental.ReturnDate),
                    DeliveryStatusShieldForUpdateOperation(rental));

                if (!CheckIfCarAvailableNow(rental.CarId).Success && rental.RentDate < GetReturnDateOfRentedCar(rental.CarId).Data)
                {
                    return new ErrorResult(Messages.CarAlreadyRentedByTheReservationDate);
                }
            }
            else //Normal Update
            {
                rulesResult = BusinessRules.Run(CheckIfCarAvailableBetweenSelectedDatesForReservations(rental.CarId, rental.RentDate, rental.ReturnDate),
                    DeliveryStatusShieldForUpdateOperation(rental));
            }


            if (rulesResult != null)
            {
                return rulesResult;
            }

            _rentalDal.Update(rental);
            return new SuccessResult(Messages.RentalUpdated);
        }

        [SecuredOperation("admin,rental.all,rental.delete")]
        [CacheRemoveAspect("IRentalService.Get")]
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

        private IResult CheckIfCarAvailableNow(int carId)
        {
            var result = (_rentalDal.GetAll(r => r.CarId == carId && r.RentDate <= DateTime.Now && r.ReturnDate >= DateTime.Now && r.DeliveryStatus == false).Any());
            if (result)
            {
                return new ErrorResult(Messages.RentalCarNotAvailable);
            }

            return new SuccessResult();
        }

        private IDataResult<DateTime> GetReturnDateOfRentedCar(int carId)
        {
            return new SuccessDataResult<DateTime>(_rentalDal.Get(r => r.CarId == carId && r.RentDate <= DateTime.Now && r.ReturnDate >= DateTime.Now && r.DeliveryStatus == false).ReturnDate);
        }

        private IDataResult<bool> CheckIfCarAvailableBetweenSelectedDatesForReservations(int carId, DateTime rentDate, DateTime returnDate)
        {
            return BaseCheckIfCarAvailableBetweenSelectedDates(carId, rentDate, returnDate, null);
        }

        private IDataResult<bool> CheckIfCarAvailableBetweenSelectedDatesForCurrentRentals(int carId, DateTime rentDate, DateTime returnDate)
        {
            return BaseCheckIfCarAvailableBetweenSelectedDates(carId, rentDate, returnDate, false);
        }

        private IDataResult<bool> BaseCheckIfCarAvailableBetweenSelectedDates(int carId, DateTime rentDate, DateTime returnDate, bool? deliveryStatus)
        {
            var allRentals = _rentalDal.GetAll(r => r.CarId == carId && r.DeliveryStatus == deliveryStatus);

            foreach (var reservation in allRentals)
            {
                if ((rentDate >= reservation.RentDate && rentDate <= reservation.ReturnDate) ||
                    (returnDate >= reservation.RentDate && returnDate <= reservation.ReturnDate) ||
                    (reservation.RentDate >= rentDate && reservation.RentDate <= returnDate) ||
                    (reservation.ReturnDate >= rentDate && reservation.ReturnDate <= returnDate))
                {
                    return new ErrorDataResult<bool>(false, Messages.ReservationBetweenSelectedDatesExist);
                }
            }
            return new SuccessDataResult<bool>(true, Messages.CarCanBeRentedBetweenSelectedDates);
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

        private IResult DeliveryStatusShieldForAddOperation(Rental rental)
        {
            if (rental.RentDate <= DateTime.Now && rental.ReturnDate <= DateTime.Now)
            {
                if (rental.DeliveryStatus == null)
                {
                    return new ErrorResult(Messages.DeliveryStatusCanNotBeNull);
                }

                return new SuccessResult();
            }
            else if (rental.RentDate <= DateTime.Now && rental.ReturnDate >= DateTime.Now) //Normal
            {
                if (rental.DeliveryStatus != false)
                {
                    return new ErrorResult(Messages.DeliveryStatusMustBeFalse);
                }

                return new SuccessResult();
            }
            else //Reservation
            {
                if (rental.DeliveryStatus != null)
                {
                    return new ErrorResult(Messages.DeliveryStatusMustBeNull);
                }

                return new SuccessResult();
            }
        }

        private IResult DeliveryStatusShieldForUpdateOperation(Rental rental)
        {
            if (rental.RentDate <= DateTime.Now && rental.ReturnDate <= DateTime.Now)
            {
                if (rental.DeliveryStatus == null)
                {
                    return new ErrorResult(Messages.DeliveryStatusCanNotBeNull);
                }

                return new SuccessResult();
            }
            else if (rental.RentDate <= DateTime.Now && rental.ReturnDate >= DateTime.Now) //Normal
            {
                if (rental.DeliveryStatus == null)
                {
                    return new ErrorResult(Messages.DeliveryStatusCanNotBeNull);
                }

                return new SuccessResult();
            }
            else //Reservation
            {
                if (rental.DeliveryStatus != null)
                {
                    return new ErrorResult(Messages.DeliveryStatusMustBeNull);
                }

                return new SuccessResult();
            }
        }
    }
}
