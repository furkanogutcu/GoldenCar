using System;
using System.Collections.Generic;
using System.Linq;
using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using Entities.Models;

namespace Business.Concrete
{
    public class RentalManager : IRentalService
    {
        private readonly IRentalDal _rentalDal;
        private readonly IPaymentService _paymentService;
        private readonly ICreditCardService _creditCardService;
        private readonly ICarService _carService;
        private readonly IFindexScoreService _findexScoreService;

        public RentalManager(IRentalDal rentalDal, IPaymentService paymentService, ICreditCardService creditCardService, ICarService carService, IFindexScoreService findexScoreService)
        {
            _rentalDal = rentalDal;
            _paymentService = paymentService;
            _creditCardService = creditCardService;
            _carService = carService;
            _findexScoreService = findexScoreService;
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

        //[SecuredOperation("admin,rental.all,rental.list")]
        public IDataResult<bool> CheckIfCanCarBeRentedNow(int carId)
        {
            var rulesResult = BusinessRules.Run(CheckIfCarAvailableNow(carId));
            if (rulesResult != null)
            {
                return new ErrorDataResult<bool>(false, rulesResult.Message);
            }
            return new SuccessDataResult<bool>(true);
        }

        //[SecuredOperation("admin,rental.all,rental.list")]
        public IDataResult<bool> CheckIfAnyRentalBetweenSelectedDates(int carId, DateTime rentDate, DateTime returnDate)
        {
            if (rentDate > DateTime.Now) //Reservation
            {
                return CheckIfCarAvailableBetweenSelectedDatesForReservations(carId, rentDate, returnDate);
            }   //Rental
            return CheckIfCarAvailableBetweenSelectedDatesForCurrentRentals(carId, rentDate, returnDate);
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

        [SecuredOperation("admin,rental.all,rental.rent,customer")]
        [ValidationAspect(typeof(RentPaymentRequestValidator))]
        [TransactionScopeAspect]
        public IDataResult<int> Rent(RentPaymentRequestModel rentPaymentRequest)
        {
            //Get Customer Findex Score
            var customerFindexScoreResult = _findexScoreService.GetCustomerFindexScore(rentPaymentRequest.CustomerId);
            if (!customerFindexScoreResult.Success)
            {
                return new ErrorDataResult<int>(-1, customerFindexScoreResult.Message);
            }

            //Get CreditCard
            var creditCardResult = _creditCardService.Get(rentPaymentRequest.CardNumber, rentPaymentRequest.ExpireYear, rentPaymentRequest.ExpireMonth, rentPaymentRequest.Cvc, rentPaymentRequest.CardHolderFullName.ToUpper());

            List<Rental> verifiedRentals = new List<Rental>();
            decimal totalAmount = 0;

            if (creditCardResult.Success)
            {
                //Verify Rentals
                foreach (var rental in rentPaymentRequest.Rentals)
                {
                    IResult rulesResult;
                    bool? deliveryStatus = false;
                    if (rental.RentDate > DateTime.Now) //Reservation
                    {
                        rulesResult = BusinessRules.Run(CheckIfCarAvailableBetweenSelectedDatesForReservations(rental.CarId, rental.RentDate, rental.ReturnDate), CheckIfReturnDateGreaterThanRentDate(rental.RentDate, rental.ReturnDate), CheckIfCustomerIdsAreEqual(rentPaymentRequest.CustomerId, rental.CustomerId));

                        if (!CheckIfCarAvailableNow(rental.CarId).Success && rental.RentDate < GetReturnDateOfRentedCar(rental.CarId).Data)
                        {
                            return new ErrorDataResult<int>(-1, Messages.CarAlreadyRentedByTheReservationDate);
                        }

                        deliveryStatus = null;
                    }
                    else //Rent now
                    {
                        rulesResult = BusinessRules.Run(CheckIfCarAvailableNow(rental.CarId), CheckIfReturnDateGreaterThanRentDate(rental.RentDate, rental.ReturnDate), CheckIfCustomerIdsAreEqual(rentPaymentRequest.CustomerId, rental.CustomerId));
                    }

                    if (rulesResult != null)
                    {
                        return new ErrorDataResult<int>(-1, rulesResult.Message);
                    }

                    var carMinFindexScore = _findexScoreService.GetCarMinFindexScore(rental.CarId);
                    if (!carMinFindexScore.Success)
                    {
                        return new ErrorDataResult<int>(-1, carMinFindexScore.Message);
                    }

                    if (customerFindexScoreResult.Data < carMinFindexScore.Data)
                    {
                        return new ErrorDataResult<int>(-1, Messages.InsufficientFindexScore);
                    }

                    rental.DeliveryStatus = deliveryStatus;
                    verifiedRentals.Add(rental);

                    //Get Amount
                    var carDailyPrice = _carService.GetById(rental.CarId).Data.DailyPrice;
                    var rentalPeriod = GetRentalPeriod(rental.RentDate, rental.ReturnDate);
                    var amount = carDailyPrice * rentalPeriod;
                    totalAmount += amount;
                }

                //Check Total Amount
                if (totalAmount != rentPaymentRequest.Amount)
                {
                    return new ErrorDataResult<int>(-1, Messages.TotalAmountNotMatch);
                }

                //Pay
                var creditCard = creditCardResult.Data;
                var paymentResult = _paymentService.Pay(creditCard, rentPaymentRequest.CustomerId, rentPaymentRequest.Amount);

                //Verify payment
                if (paymentResult.Success && paymentResult.Data != -1)
                {
                    //Add rentals on db
                    foreach (var verifiedRental in verifiedRentals)
                    {
                        verifiedRental.PaymentId = paymentResult.Data;

                        //Add Rental
                        var rentalAddResult = Add(verifiedRental);

                        //Check Rental
                        if (!rentalAddResult.Success)
                        {
                            return new ErrorDataResult<int>(-1, rentalAddResult.Message);
                        }
                    }
                    return new SuccessDataResult<int>(paymentResult.Data, Messages.RentalSuccessful);
                }
                return new ErrorDataResult<int>(-1, paymentResult.Message);
            }
            return new ErrorDataResult<int>(-1, creditCardResult.Message);
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

        private IResult CheckIfReturnDateGreaterThanRentDate(DateTime rentDate, DateTime returnDate)
        {
            if (returnDate > rentDate)
            {
                return new SuccessResult();
            }

            return new ErrorResult(Messages.RentDateMustBeGreaterThanReturnDate);
        }

        private IResult CheckIfCustomerIdsAreEqual(int customerIdInPayment, int customerIdInRental)
        {
            if (customerIdInPayment == customerIdInRental)
            {
                return new SuccessResult();
            }

            return new ErrorResult(Messages.LeastOneCustomerIdDoesNotMatch);
        }

        private int GetRentalPeriod(DateTime rentDate, DateTime returnDate)
        {
            return (Convert.ToInt32((returnDate - rentDate).TotalDays));
        }
    }
}
