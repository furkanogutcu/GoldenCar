using System;
using Business.Concrete;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            BrandManager brandManager = new BrandManager(new EfBrandDal());
            BrandManagerAddTest(brandManager);

            ColorManager colorManager = new ColorManager(new EfColorDal());
            ColorManagerAddTest(colorManager);

            CarManager carManager = new CarManager(new EfCarDal());
            CarManagerAddTest(carManager);
            ListCars(carManager);

            UserManager userManager = new UserManager(new EfUserDal());
            UserManagerAddTest(userManager);

            CustomerManager customerManager = new CustomerManager(new EfCustomerDal());
            CustomerManagerAddTest(customerManager);

            RentalManager rentalManager = new RentalManager(new EfRentalDal());
            RentACarTest(rentalManager);
            RentACarTest(rentalManager);

            Console.ReadLine();
        }

        private static void RentACarTest(RentalManager rentalManager)
        {
            var x = rentalManager.Add(new Rental {CarId = 4, CustomerId = 1, RentDate = DateTime.Now});
            Console.WriteLine("{0} ---> {1}",x.Success,x.Message);
        }

        private static void CustomerManagerAddTest(CustomerManager customerManager)
        {
            customerManager.Add(new Customer {UserId = 1, CompanyName = "example1 Company"});
            customerManager.Add(new Customer {UserId = 3, CompanyName = "example2 Company"});
        }

        private static void UserManagerAddTest(UserManager userManager)
        {
            userManager.Add(new User {FirstName = "Furkan", LastName = "Ogutcu", Email = "example@", Password = "123456"});
            userManager.Add(new User {FirstName = "test1", LastName = "test2", Email = "example2@", Password = "654321"});
            userManager.Add(new User {FirstName = "test3", LastName = "test4", Email = "example2@", Password = "654321"});
        }

        private static void ListCars(CarManager carManager)
        {
            foreach (var car in carManager.GetCarDetails().Data)
            {
                Console.WriteLine("ID: {0} , Brand: {1} , Color: {2} , ModelYear: {3} , DailyPrice {4} , Description: {5}",
                    car.Id, car.BrandName, car.ColorName,
                    car.ModelYear.Year, car.DailyPrice, car.Description);
            }
        }

        private static void CarManagerAddTest(CarManager carManager)
        {
            carManager.Add(new Car{BrandId = 1, ColorId = 3, ModelYear = new DateTime(2010, 1, 1), DailyPrice = 500, Description = "1.2 liter engine"});
            carManager.Add(new Car{BrandId = 1, ColorId = 2, ModelYear = new DateTime(2015, 1, 1), DailyPrice = 750, Description = "1.4 liter engine"});
            carManager.Add(new Car{BrandId = 2, ColorId = 7, ModelYear = new DateTime(2012, 1, 1), DailyPrice = 900, Description = "1.4 liter engine"});
            carManager.Add(new Car{BrandId = 3, ColorId = 4, ModelYear = new DateTime(2021, 1, 1), DailyPrice = 1500, Description = "1.6 liter engine"});
            carManager.Add(new Car{BrandId = 5, ColorId = 4, ModelYear = new DateTime(2017, 1, 1), DailyPrice = 600, Description = "1.6 liter engine"});
            carManager.Add(new Car{BrandId = 3, ColorId = 5, ModelYear = new DateTime(2018, 1, 1), DailyPrice = 900, Description = "1.2 liter engine"});
            carManager.Add(new Car{BrandId = 3, ColorId = 2, ModelYear = new DateTime(2021, 1, 1), DailyPrice = 1000, Description = "1.4 liter engine"});
            carManager.Add(new Car{BrandId = 1, ColorId = 2, ModelYear = new DateTime(2014, 1, 1), DailyPrice = 1500, Description = "1.6 liter engine"});
            carManager.Add(new Car{BrandId = 2, ColorId = 3, ModelYear = new DateTime(2021, 1, 1), DailyPrice = 3000, Description = "1.6 liter engine"});
            carManager.Add(new Car{BrandId = 1, ColorId = 6, ModelYear = new DateTime(2015, 1, 1), DailyPrice = 1200, Description = "1.4 liter engine"});
            carManager.Add(new Car{BrandId = 4, ColorId = 6, ModelYear = new DateTime(2000, 1, 1), DailyPrice = 1000, Description = "1.2 liter engine"});
        }

        private static void ColorManagerAddTest(ColorManager colorManager)
        {
            colorManager.Add(new Color {Name = "Black"});
            colorManager.Add(new Color {Name = "Red"});
            colorManager.Add(new Color {Name = "White"});
            colorManager.Add(new Color {Name = "Yellow"});
            colorManager.Add(new Color {Name = "Blue"});
            colorManager.Add(new Color {Name = "Green"});
            colorManager.Add(new Color {Name = "Metallic"});
        }

        private static void BrandManagerAddTest(BrandManager brandManager)
        {
            brandManager.Add(new Brand {Name = "BMW"});
            brandManager.Add(new Brand {Name = "Mercedes"});
            brandManager.Add(new Brand {Name = "Volkswagen"});
            brandManager.Add(new Brand {Name = "Audi"});
            brandManager.Add(new Brand {Name = "FIAT"});
        }
    }
}
