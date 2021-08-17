using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;

namespace Business.Abstract
{
    public interface ICarImageService
    {
        IDataResult<List<CarImage>> GetAll();
        IDataResult<List<CarImage>> GetCarImages(int carId);
        IDataResult<CarImage> GetById(int imageId);
        IResult Add(CarImage carImage,IFormFile file);
        IResult Update(CarImage carImage, IFormFile file);
        IResult Delete(int imageId);
    }
}
