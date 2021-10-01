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
using Core.Utilities.Helpers;
using Core.Utilities.Helpers.FileHelper;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;

namespace Business.Concrete
{
    public class CarImageManager : ICarImageService
    {
        private readonly ICarImageDal _carImageDal;

        public CarImageManager(ICarImageDal carImageDal)
        {
            _carImageDal = carImageDal;
        }

        [SecuredOperation("admin,carimage.all,carimage.list")]
        [CacheAspect(10)]
        public IDataResult<List<CarImage>> GetAll()
        {
            return new SuccessDataResult<List<CarImage>>(_carImageDal.GetAll(), Messages.CarsImagesListed);
        }

        [SecuredOperation("admin,carimage.all,carimage.list")]
        [CacheAspect(10)]
        public IDataResult<List<CarImage>> GetCarImages(int carId)
        {
            var checkIfCarImage = CheckIfCarHasImage(carId);
            var images = checkIfCarImage.Success
                ? checkIfCarImage.Data
                : _carImageDal.GetAll(c => c.CarId == carId);
            return new SuccessDataResult<List<CarImage>>(images, checkIfCarImage.Message);
        }

        [SecuredOperation("admin,carimage.all,carimage.list")]
        [CacheAspect(10)]
        public IDataResult<CarImage> GetById(int imageId)
        {
            return new SuccessDataResult<CarImage>(_carImageDal.Get(c => c.Id == imageId), Messages.CarImageListed);
        }

        [SecuredOperation("admin,carimage.all,carimage.add")]
        [ValidationAspect(typeof(CarImageValidator))]
        [CacheRemoveAspect("ICarImageService.Get")]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Add(IFormFile file, int carId)
        {
            IResult rulesResult = BusinessRules.Run(CheckIfCarImageLimitExceeded(carId));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            var imageResult = FileHelper.Upload(file);
            if (!imageResult.Success)
            {
                return new ErrorResult(imageResult.Message);
            }

            CarImage carImage = new CarImage
            {
                ImagePath = imageResult.Message,
                CarId = carId,
                Date = DateTime.Now
            };
            _carImageDal.Add(carImage);
            return new SuccessResult(Messages.CarImageAdded);
        }

        [SecuredOperation("admin,carimage.all,carimage.update")]
        [ValidationAspect(typeof(CarImageValidator))]
        [CacheRemoveAspect("ICarImageService.Get")]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Update(CarImage carImage, IFormFile file)
        {
            IResult rulesResult = BusinessRules.Run(CheckIfCarImageIdExist(carImage.Id),
                CheckIfCarImageLimitExceeded(carImage.CarId));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            var updatedImage = _carImageDal.Get(c => c.Id == carImage.Id);
            var result = FileHelper.Update(file, updatedImage.ImagePath);
            if (!result.Success)
            {
                return new ErrorResult(Messages.ErrorUpdatingImage);
            }
            carImage.ImagePath = result.Message;
            carImage.Date = DateTime.Now;
            _carImageDal.Update(carImage);
            return new SuccessResult(Messages.CarImageUpdated);
        }

        [SecuredOperation("admin,carimage.all,carimage.delete")]
        [CacheRemoveAspect("ICarImageService.Get")]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Delete(CarImage carImage)
        {
            IResult rulesResult = BusinessRules.Run(CheckIfCarImageIdExist(carImage.Id));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            var deletedImage = _carImageDal.Get(c => c.Id == carImage.Id);
            var result = FileHelper.Delete(deletedImage.ImagePath);
            if (!result.Success)
            {
                return new ErrorResult(Messages.ErrorDeletingImage);
            }
            _carImageDal.Delete(deletedImage);
            return new SuccessResult(Messages.CarImageDeleted);
        }

        [SecuredOperation("admin,carimage.all,carimage.delete")]
        [CacheRemoveAspect("ICarImageService.Get")]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult DeleteAllImagesOfCarByCarId(int carId)
        {
            var deletedImages = _carImageDal.GetAll(c => c.CarId == carId);
            if (deletedImages == null)
            {
                return new ErrorResult(Messages.NoPictureOfTheCar);
            }
            foreach (var deletedImage in deletedImages)
            {
                _carImageDal.Delete(deletedImage);
                FileHelper.Delete(deletedImage.ImagePath);
            }
            return new SuccessResult(Messages.CarImageDeleted);
        }

        //Business Rules

        private IResult CheckIfCarImageLimitExceeded(int carId)
        {
            int result = _carImageDal.GetAll(c => c.CarId == carId).Count;
            if (result >= 5)
            {
                return new ErrorResult(Messages.CarImageLimitExceeded);
            }
            return new SuccessResult();
        }

        private IDataResult<List<CarImage>> CheckIfCarHasImage(int carId)
        {
            string logoPath = "/images/default.jpg";
            bool result = _carImageDal.GetAll(c => c.CarId == carId).Any();
            if (!result)
            {
                List<CarImage> imageList = new List<CarImage>
                {
                    new CarImage
                    {
                        ImagePath = logoPath,
                        CarId = carId,
                        Date = DateTime.Now
                    }
                };
                return new SuccessDataResult<List<CarImage>>(imageList, Messages.GetDefaultImage);
            }
            return new ErrorDataResult<List<CarImage>>(new List<CarImage>(), Messages.CarImagesListed);
        }

        private IResult CheckIfCarImageIdExist(int imageId)
        {
            var result = _carImageDal.GetAll(c => c.Id == imageId).Any();
            if (!result)
            {
                return new ErrorResult(Messages.CarImageIdNotExist);
            }
            return new SuccessResult();
        }
    }
}
