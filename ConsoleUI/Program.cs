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

            UserManager userManager = new UserManager(new EfUserDal());
            UserManagerAddTest(userManager);

            CustomerManager customerManager = new CustomerManager(new EfCustomerDal());
            CustomerManagerAddTest(customerManager);

            RentalManager rentalManager = new RentalManager(new EfRentalDal());
            RentACarTest(rentalManager);

            Console.WriteLine("Test successful!");
            Console.ReadLine();
        }

        private static void RentACarTest(RentalManager rentalManager)
        {
            rentalManager.Add(new Rental { CarId = 4, CustomerId = 6, RentDate = DateTime.Now });
            rentalManager.Add(new Rental { CarId = 6, CustomerId = 2, RentDate = DateTime.Now });
            rentalManager.Add(new Rental { CarId = 1, CustomerId = 3, RentDate = DateTime.Now });
            rentalManager.Add(new Rental { CarId = 3, CustomerId = 1, RentDate = DateTime.Now });
            rentalManager.Add(new Rental { CarId = 5, CustomerId = 2, RentDate = DateTime.Now });
        }

        private static void CustomerManagerAddTest(CustomerManager customerManager)
        {
            customerManager.Add(new Customer { UserId = 1, CompanyName = "Test Company 1" });
            customerManager.Add(new Customer { UserId = 4, CompanyName = "Test Company 2" });
            customerManager.Add(new Customer { UserId = 2, CompanyName = "Test Company 3" });
            customerManager.Add(new Customer { UserId = 7, CompanyName = "Test Company 4" });
            customerManager.Add(new Customer { UserId = 6, CompanyName = "Test Company 5" });
            customerManager.Add(new Customer { UserId = 10, CompanyName = "Test Company 6" });
        }

        private static void UserManagerAddTest(UserManager userManager)
        {
            userManager.Add(new User { FirstName = "TestName1", LastName = "TestSurname1", Email = "test1@.com", Password = "123456789" });
            userManager.Add(new User { FirstName = "TestName2", LastName = "TestSurname2", Email = "test2@.com", Password = "987654321" });
            userManager.Add(new User { FirstName = "TestName3", LastName = "TestSurname3", Email = "test3@.com", Password = "123456789" });
            userManager.Add(new User { FirstName = "TestName4", LastName = "TestSurname4", Email = "test4@.com", Password = "987654321" });
            userManager.Add(new User { FirstName = "TestName5", LastName = "TestSurname5", Email = "test5@.com", Password = "123456789" });
            userManager.Add(new User { FirstName = "TestName6", LastName = "TestSurname6", Email = "test6@.com", Password = "987654321" });
            userManager.Add(new User { FirstName = "TestName7", LastName = "TestSurname7", Email = "test7@.com", Password = "123456789" });
            userManager.Add(new User { FirstName = "TestName8", LastName = "TestSurname8", Email = "test8@.com", Password = "987654321" });
            userManager.Add(new User { FirstName = "TestName9", LastName = "TestSurname9", Email = "test9@.com", Password = "123456789" });
            userManager.Add(new User { FirstName = "TestName10", LastName = "TestSurname10", Email = "test10@.com", Password = "987654321" });
        }

        private static void CarManagerAddTest(CarManager carManager)
        {
            carManager.Add(new Car { BrandId = 1, ColorId = 3, ModelYear = new DateTime(2010, 1, 1), DailyPrice = 500, Description = "1.2 liter engine" });
            carManager.Add(new Car { BrandId = 1, ColorId = 2, ModelYear = new DateTime(2015, 1, 1), DailyPrice = 750, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 2, ColorId = 7, ModelYear = new DateTime(2012, 1, 1), DailyPrice = 900, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 3, ColorId = 4, ModelYear = new DateTime(2021, 1, 1), DailyPrice = 1500, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 5, ColorId = 4, ModelYear = new DateTime(2017, 1, 1), DailyPrice = 600, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 3, ColorId = 5, ModelYear = new DateTime(2018, 1, 1), DailyPrice = 900, Description = "1.2 liter engine" });
            carManager.Add(new Car { BrandId = 3, ColorId = 2, ModelYear = new DateTime(2021, 1, 1), DailyPrice = 1000, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 1, ColorId = 2, ModelYear = new DateTime(2014, 1, 1), DailyPrice = 1500, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 2, ColorId = 3, ModelYear = new DateTime(2021, 1, 1), DailyPrice = 3000, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 1, ColorId = 6, ModelYear = new DateTime(2015, 1, 1), DailyPrice = 1200, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 4, ColorId = 1, ModelYear = new DateTime(2000, 1, 1), DailyPrice = 1000, Description = "1.2 liter engine" });
            carManager.Add(new Car { BrandId = 2, ColorId = 1, ModelYear = new DateTime(2016, 1, 1), DailyPrice = 1700, Description = "1.4 liter engine" });
        }

        private static void ColorManagerAddTest(ColorManager colorManager)
        {
            colorManager.Add(new Color { Name = "Black" });
            colorManager.Add(new Color { Name = "Red" });
            colorManager.Add(new Color { Name = "White" });
            colorManager.Add(new Color { Name = "Yellow" });
            colorManager.Add(new Color { Name = "Blue" });
            colorManager.Add(new Color { Name = "Green" });
            colorManager.Add(new Color { Name = "Metallic" });
        }

        private static void BrandManagerAddTest(BrandManager brandManager)
        {
            brandManager.Add(new Brand { Name = "BMW" });
            brandManager.Add(new Brand { Name = "Mercedes" });
            brandManager.Add(new Brand { Name = "Volkswagen" });
            brandManager.Add(new Brand { Name = "Audi" });
            brandManager.Add(new Brand { Name = "FIAT" });
        }
    }
}
