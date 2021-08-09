using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Entities.Concrete;

namespace DataAccess.Concrete.InMemory
{
    public class InMemoryCarDal:ICarDal
    {
        private List<Car> _carList;

        public InMemoryCarDal()
        {
            _carList = new List<Car>();
        }

        public Car GetById(int id)
        {
            if (_carList.Count > 0)
            {
                return _carList.SingleOrDefault(c => c.Id == id);
            }
            else
            {
                throw new Exception("The car list is empty."); 
            }
        }

        public List<Car> GetAll()
        {
            return _carList;
        }

        public void Add(Car car)
        {
            _carList.Add(car);
        }

        public void Update(Car car)
        {
            Car carToUpdate = _carList.SingleOrDefault(c => c.Id == car.Id);
            carToUpdate.BrandId = car.BrandId;
            carToUpdate.ColorId = car.ColorId;
            carToUpdate.DailyPrice = car.DailyPrice;
            carToUpdate.Description = car.Description;
            carToUpdate.ModelYear = car.ModelYear;
        }

        public void Delete(Car car)
        {
            _carList.Remove(_carList.SingleOrDefault(c => c.Id == car.Id));
        }
    }
}
