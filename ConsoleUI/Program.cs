using System;
using Business.Concrete;
using DataAccess.Concrete.EntityFramework;
using DataAccess.Concrete.InMemory;
using Entities.Concrete;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            BrandManager brandManager = new BrandManager(new EfBrandDal());
            brandManager.Add(new Brand { Id = 1, Name = "BMW"});
            brandManager.Add(new Brand { Id = 2, Name = "Mercedes" });
            brandManager.Add(new Brand { Id = 3, Name = "Volkswagen" });
            brandManager.Add(new Brand { Id = 4, Name = "Audi" });
            brandManager.Add(new Brand { Id = 5, Name = "FIAT" });

            ColorManager colorManager = new ColorManager(new EfColorDal());
            colorManager.Add(new Color { Id = 1, Name = "Black"});
            colorManager.Add(new Color { Id = 2, Name = "Red" });
            colorManager.Add(new Color { Id = 3, Name = "White" });
            colorManager.Add(new Color { Id = 4, Name = "Yellow" });
            colorManager.Add(new Color { Id = 5, Name = "Blue" });
            colorManager.Add(new Color { Id = 6, Name = "Green" });
            colorManager.Add(new Color { Id = 7, Name = "Metallic" });

            CarManager carManager = new CarManager(new EfCarDal());
            //Add
            carManager.Add(new Car {Id = 1, BrandId = 1, ColorId = 3, ModelYear = new DateTime(2010, 1, 1), DailyPrice = 500, Description = "1.2 liter engine"});
            carManager.Add(new Car {Id = 2, BrandId = 1, ColorId = 2, ModelYear = new DateTime(2015, 1, 1), DailyPrice = 750, Description = "1.4 liter engine"});
            carManager.Add(new Car {Id = 3, BrandId = 2, ColorId = 7, ModelYear = new DateTime(2012, 1, 1), DailyPrice = 900, Description = "1.4 liter engine"});
            carManager.Add(new Car {Id = 4, BrandId = 3, ColorId = 4, ModelYear = new DateTime(2021, 1, 1), DailyPrice = 1500, Description = "1.6 liter engine"});
            carManager.Add(new Car { Id = 5, BrandId = 5, ColorId = 4, ModelYear = new DateTime(2017, 1, 1), DailyPrice = 600, Description = "1.6 liter engine" });
            carManager.Add(new Car { Id = 6, BrandId = 3, ColorId = 5, ModelYear = new DateTime(2018, 1, 1), DailyPrice = 900, Description = "1.2 liter engine" });
            carManager.Add(new Car { Id = 7, BrandId = 3, ColorId = 2, ModelYear = new DateTime(2021, 1, 1), DailyPrice = 1000, Description = "1.4 liter engine" });
            carManager.Add(new Car { Id = 8, BrandId = 1, ColorId = 2, ModelYear = new DateTime(2014, 1, 1), DailyPrice = 1500, Description = "1.6 liter engine" });
            carManager.Add(new Car { Id = 9, BrandId = 2, ColorId = 3, ModelYear = new DateTime(2021, 1, 1), DailyPrice = 3000, Description = "1.6 liter engine" });
            carManager.Add(new Car { Id = 10, BrandId = 1, ColorId = 6, ModelYear = new DateTime(2015, 1, 1), DailyPrice = 1200, Description = "1.4 liter engine" });
            carManager.Add(new Car { Id = 11, BrandId = 4, ColorId = 6, ModelYear = new DateTime(2000, 1, 1), DailyPrice = 1000, Description = "1.2 liter engine" });
            //GetAll
            foreach (var car in carManager.GetAll())
            {
                Console.WriteLine("ID: {0} , Brand: {1} , Color: {2} , ModelYear: {3} , DailyPrice {4} , Description: {5}",car.Id,brandManager.GetBrandById(car.BrandId).Name,colorManager.GetColorById(car.ColorId).Name,car.ModelYear.Year,car.DailyPrice,car.Description);
            }
            //GetById
            Console.WriteLine(carManager.GetById(3).DailyPrice + " (Daily price of the car with id=3)");
            //Update
            carManager.Update(new Car
            {
                Id = 3,
                BrandId = 2,
                ColorId = 3,
                ModelYear = new DateTime(2012, 1, 1),
                DailyPrice = 1800,
                Description = "1.4 liter engine"
            });
            Console.WriteLine(carManager.GetById(3).DailyPrice + " (New daily price of the car with id=3)");
            //Delete
            carManager.Delete(carManager.GetById(2));
            foreach (var car in carManager.GetAll())
            {
                Console.WriteLine("ID: {0} , Brand: {1} , Color: {2} , ModelYear: {3} , DailyPrice {4} , Description: {5}", car.Id, brandManager.GetBrandById(car.BrandId).Name, colorManager.GetColorById(car.ColorId).Name, car.ModelYear.Year, car.DailyPrice, car.Description);
            }

            Console.ReadLine();
        }
    }
}
