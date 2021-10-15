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

namespace Business.Concrete
{
    public class BrandManager : IBrandService
    {
        private readonly IBrandDal _brandDal;

        public BrandManager(IBrandDal brandDal)
        {
            _brandDal = brandDal;
        }

        //[SecuredOperation("admin,brand.all,brand.list")]
        [CacheAspect(10)]
        public IDataResult<Brand> GetBrandById(int id)
        {
            return new SuccessDataResult<Brand>(_brandDal.Get(b => b.Id == id), Messages.BrandListed);
        }

        //[SecuredOperation("admin,brand.all,brand.list")]
        [CacheAspect(10)]
        public IDataResult<List<Brand>> GetAll()
        {
            return new SuccessDataResult<List<Brand>>(_brandDal.GetAll(), Messages.BrandsListed);
        }

        [SecuredOperation("admin,brand.all,brand.add")]
        [ValidationAspect(typeof(BrandValidator))]
        [CacheRemoveAspect("IBrandService.Get")]
        public IResult Add(Brand brand)
        {
            var rulesResult = BusinessRules.Run(CheckIfBrandNameExist(brand.Name));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            _brandDal.Add(brand);
            return new SuccessResult(Messages.BrandAdded);
        }

        [SecuredOperation("admin,brand.all,brand.update")]
        [ValidationAspect(typeof(BrandValidator))]
        [CacheRemoveAspect("IBrandService.Get")]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Update(Brand brand)
        {
            var rulesResult = BusinessRules.Run(CheckIfBrandNameExist(brand.Name),
                CheckIfBrandIdExist(brand.Id));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            _brandDal.Update(brand);
            return new SuccessResult(Messages.BrandUpdated);
        }

        [SecuredOperation("admin,brand.all,brand.delete")]
        [CacheRemoveAspect("IBrandService.Get")]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Delete(Brand brand)
        {
            var rulesResult = BusinessRules.Run(CheckIfBrandIdExist(brand.Id));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            var deletedBrand = _brandDal.Get(b => b.Id == brand.Id);
            _brandDal.Delete(deletedBrand);
            return new SuccessResult(Messages.BrandDeleted);
        }

        //Business Rules
        private IResult CheckIfBrandIdExist(int brandId)
        {
            var result = _brandDal.GetAll(b => b.Id == brandId).Any();
            if (!result)
            {
                return new ErrorResult(Messages.BrandNotExist);
            }
            return new SuccessResult();
        }

        private IResult CheckIfBrandNameExist(string brandName)
        {
            var result = _brandDal.GetAll(b => Equals(b.Name, brandName)).Any();
            if (result)
            {
                return new ErrorResult(Messages.BrandExist);
            }
            return new SuccessResult();
        }
    }
}
