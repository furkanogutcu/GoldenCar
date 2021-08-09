using System;
using Business.Concrete;
using DataAccess.Concrete.InMemory;
using Entities.Concrete;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            CarManager manager = new CarManager(new InMemoryCarDal());
            //Add
            manager.Add(new Car
            {
                Id = 1,
                BrandId = 1,
                ColorId = 3,
                ModelYear = new DateTime(2010, 1, 1),
                DailyPrice = 500,
                Description = "1.2 litre engine"
            });
            manager.Add(new Car
            {
                Id = 2,
                BrandId = 1,
                ColorId = 2,
                ModelYear = new DateTime(2015, 1, 1),
                DailyPrice = 750,
                Description = "1.4 litre engine"
            });
            manager.Add(new Car
            {
                Id = 3,
                BrandId = 2,
                ColorId = 3,
                ModelYear = new DateTime(2012, 1, 1),
                DailyPrice = 900,
                Description = "1.4 litre engine"
            });
            manager.Add(new Car
            {
                Id = 4,
                BrandId = 3,
                ColorId = 1,
                ModelYear = new DateTime(2021, 1, 1),
                DailyPrice = 1500,
                Description = "1.6 litre engine"
            });
            //GetAll
            foreach (var car in manager.GetAll())
            {
                Console.WriteLine("Id: " + car.Id + " / Daily Price: " + car.DailyPrice);
            }
            //GetById
            Console.WriteLine(manager.GetById(3).DailyPrice + " (Daily price of the car with id=3)");
            //Update
            manager.Update(new Car
            {
                Id = 3,
                BrandId = 2,
                ColorId = 3,
                ModelYear = new DateTime(2012, 1, 1),
                DailyPrice = 1800,
                Description = "1.4 litre engine"
            });
            Console.WriteLine(manager.GetById(3).DailyPrice + " (New daily price of the car with id=3)");
            //Delete
            manager.Delete(manager.GetById(2));
            foreach (var car in manager.GetAll())
            {
                Console.WriteLine("Id: " + car.Id + " / Daily Price: " + car.DailyPrice);
            }
            Console.ReadLine();
        }
    }
}
