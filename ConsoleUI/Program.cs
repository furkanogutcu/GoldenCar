using System;
using System.Collections.Generic;
using System.Linq;
using Business.Concrete;
using Core.Entities.Concrete;
using Core.Utilities.Security.Hashing;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {

            //BrandManagerAddTest();


            //ColorManagerAddTest();


            //CarManagerAddTest();


            //UserManagerAddTest(17);


            //CustomerManagerAddTest(250);


            //RentACarTest(50);

            Console.WriteLine("All tests successful!");
            Console.ReadLine();
        }

        private static void RentACarTest(int numberOfRentalsToAdd)
        {
            RentalManager rentalManager = new RentalManager(new EfRentalDal(), new PaymentManager(new EfPaymentDal(), new CreditCardManager(new EfCreditCardDal())), new CreditCardManager(new EfCreditCardDal()), new CarManager(new EfCarDal()));
            CarManager carManager = new CarManager(new EfCarDal());
            CustomerManager customerManager = new CustomerManager(new EfCustomerDal());

            var carIds = carManager.GetAll().Data.Select(c => c.Id).ToList();
            var customerIds = customerManager.GetAll().Data.Select(c => c.Id).ToList();
            int loopSize = carIds.Count > numberOfRentalsToAdd ? numberOfRentalsToAdd : carIds.Count;
            for (int i = 0; i < loopSize; i++)
            {
                var randomCarNum = new Random().Next(0, carIds.Count);
                var randomCustomerNum = new Random().Next(0, customerIds.Count);
                var randomCarId = carIds[randomCarNum];
                var randomCustomerId = customerIds[randomCustomerNum];

                Rental addedRental = new Rental
                {
                    CarId = randomCarId,
                    CustomerId = randomCustomerId,
                    RentDate = DateTime.Now.AddHours(-randomCarId).AddMinutes(-randomCustomerId),
                    ReturnDate = DateTime.Now.AddHours(randomCustomerId).AddMinutes(randomCarId),
                    DeliveryStatus = false
                };
                rentalManager.Add(addedRental);
                carIds.Remove(randomCarId);
            }
            Console.WriteLine("+Add rentals test successful");
        }

        private static void CustomerManagerAddTest(int numberOfCustomersToAdd)
        {
            CustomerManager customerManager = new CustomerManager(new EfCustomerDal());
            UserManager userManager = new UserManager(new EfUserDal());

            int[] userIds = userManager.GetAll().Data.Select(u => u.Id).ToArray();
            int[] customerIds = customerManager.GetAll().Data.Select(c => c.UserId).ToArray();
            var userIdsThatCanBeAdded = userIds.Except(customerIds).ToList();
            int loopSize = userIdsThatCanBeAdded.Count > numberOfCustomersToAdd
                ? numberOfCustomersToAdd
                : userIdsThatCanBeAdded.Count;

            for (int i = 0; i < loopSize; i++)
            {
                var randomNum = new Random().Next(0, userIdsThatCanBeAdded.Count);
                var randomUserId = userIdsThatCanBeAdded[randomNum];
                customerManager.Add(new Customer
                {
                    UserId = randomUserId,
                    CompanyName = "Test Company " + randomUserId
                });
                userIdsThatCanBeAdded.Remove(randomUserId);
            }
            Console.WriteLine("+Add customers test successful");
        }

        private static void UserManagerAddTest(int numberOfUsersToAdd)
        {
            UserManager userManager = new UserManager(new EfUserDal());
            for (int i = 1; i < numberOfUsersToAdd + 1; i++)
            {
                byte[] p, s = new byte[] { };
                var password = "123456789" + i;
                HashingHelper.CreatePasswordHash(password, out p, out s);
                var addedUser = new User
                {
                    FirstName = "TestName" + i,
                    LastName = "TestSurname" + i,
                    Email = "test" + i + "@.com",
                    PasswordHash = p,
                    PasswordSalt = s,
                    Status = true
                };
                userManager.Add(addedUser);
            }
            Console.WriteLine("+Add users test successful");
        }

        private static void CarManagerAddTest()
        {
            CarManager carManager = new CarManager(new EfCarDal());
            carManager.Add(new Car { BrandId = 1, ColorId = 3, ModelName = "X5 xDrive45E", ModelYear = 2010, DailyPrice = 500, Description = "1.2 liter engine" });
            carManager.Add(new Car { BrandId = 6, ColorId = 2, ModelName = "S60", ModelYear = 2015, DailyPrice = 750, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 2, ColorId = 7, ModelName = "Benz E300", ModelYear = 2012, DailyPrice = 900, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 3, ColorId = 4, ModelName = "T-Roc", ModelYear = 2021, DailyPrice = 1500, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 5, ColorId = 4, ModelName = "500L", ModelYear = 2017, DailyPrice = 600, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 4, ColorId = 5, ModelName = "A3", ModelYear = 2018, DailyPrice = 900, Description = "1.2 liter engine" });
            carManager.Add(new Car { BrandId = 8, ColorId = 8, ModelName = "Focus", ModelYear = 2021, DailyPrice = 1000, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 9, ColorId = 6, ModelName = "C3", ModelYear = 2014, DailyPrice = 1500, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 2, ColorId = 3, ModelName = "AMG S65", ModelYear = 2021, DailyPrice = 3000, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 1, ColorId = 6, ModelName = "M4", ModelYear = 2015, DailyPrice = 1200, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 4, ColorId = 10, ModelName = "TT Sedan", ModelYear = 2000, DailyPrice = 1000, Description = "1.2 liter engine" });
            carManager.Add(new Car { BrandId = 6, ColorId = 9, ModelName = "XC60", ModelYear = 2016, DailyPrice = 1700, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 8, ColorId = 3, ModelName = "Fiesta", ModelYear = 2010, DailyPrice = 500, Description = "1.2 liter engine" });
            carManager.Add(new Car { BrandId = 7, ColorId = 8, ModelName = "City", ModelYear = 2015, DailyPrice = 750, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 5, ColorId = 7, ModelName = "Egea", ModelYear = 2012, DailyPrice = 900, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 3, ColorId = 4, ModelName = "Beetle", ModelYear = 2021, DailyPrice = 1500, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 8, ColorId = 4, ModelName = "Fiesta ST", ModelYear = 2017, DailyPrice = 600, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 9, ColorId = 12, ModelName = "C3 Aircross", ModelYear = 2018, DailyPrice = 900, Description = "1.2 liter engine" });
            carManager.Add(new Car { BrandId = 10, ColorId = 2, ModelName = "Clio", ModelYear = 2021, DailyPrice = 1000, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 5, ColorId = 2, ModelName = "500L", ModelYear = 2014, DailyPrice = 1500, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 3, ColorId = 3, ModelName = "Golf", ModelYear = 2021, DailyPrice = 3000, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 4, ColorId = 6, ModelName = "RS6", ModelYear = 2015, DailyPrice = 1200, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 8, ColorId = 4, ModelName = "Fiesta ST", ModelYear = 2000, DailyPrice = 1000, Description = "1.2 liter engine" });
            carManager.Add(new Car { BrandId = 9, ColorId = 11, ModelName = "DS High Rider", ModelYear = 2016, DailyPrice = 1700, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 2, ColorId = 9, ModelName = "AMG CLS63", ModelYear = 2010, DailyPrice = 500, Description = "1.2 liter engine" });
            carManager.Add(new Car { BrandId = 3, ColorId = 5, ModelName = "T Prime GTE", ModelYear = 2015, DailyPrice = 750, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 3, ColorId = 12, ModelName = "Beetle A5", ModelYear = 2012, DailyPrice = 900, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 4, ColorId = 7, ModelName = "A7", ModelYear = 2021, DailyPrice = 1500, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 10, ColorId = 6, ModelName = "Captur", ModelYear = 2017, DailyPrice = 600, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 8, ColorId = 5, ModelName = "Fiesta", ModelYear = 2018, DailyPrice = 900, Description = "1.2 liter engine" });
            carManager.Add(new Car { BrandId = 7, ColorId = 6, ModelName = "Civic", ModelYear = 2021, DailyPrice = 1000, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 6, ColorId = 1, ModelName = "XC90", ModelYear = 2014, DailyPrice = 1500, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 4, ColorId = 5, ModelName = "TT Shooting Brake", ModelYear = 2021, DailyPrice = 3000, Description = "1.6 liter engine" });
            carManager.Add(new Car { BrandId = 5, ColorId = 6, ModelName = "Argo", ModelYear = 2015, DailyPrice = 1200, Description = "1.4 liter engine" });
            carManager.Add(new Car { BrandId = 1, ColorId = 8, ModelName = "X5 25d", ModelYear = 2000, DailyPrice = 1000, Description = "1.2 liter engine" });
            carManager.Add(new Car { BrandId = 4, ColorId = 4, ModelName = "Q7", ModelYear = 2016, DailyPrice = 1700, Description = "1.4 liter engine" });
            Console.WriteLine("+Add cars test successful");
        }

        private static void ColorManagerAddTest()
        {
            var colorList = new string[] { "Black", "Red", "White", "Yellow", "Blue", "Green", "Metallic", "Brown", "Gray", "Pink", "Gold", "Orange" };
            ColorManager colorManager = new ColorManager(new EfColorDal());
            foreach (var colorName in colorList)
            {
                colorManager.Add(new Color { Name = colorName });
            }
            Console.WriteLine("+Add colors test successful");
        }

        private static void BrandManagerAddTest()
        {
            var brandList = new string[] { "BMW", "Mercedes", "Volkswagen", "Audi", "FIAT", "Volvo", "Honda", "Ford", "Citroen", "Renault" };
            BrandManager brandManager = new BrandManager(new EfBrandDal());
            foreach (var brandName in brandList)
            {
                brandManager.Add(new Brand { Name = brandName });
            };
            Console.WriteLine("+Add brands test successful");
        }
    }
}
