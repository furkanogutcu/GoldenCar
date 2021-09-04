using System.Collections.Generic;
using System.Linq;
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfRentalDal:EfEntityRepositoryBase<Rental,RentACarContext>,IRentalDal
    {
        public List<RentalDetailDto> GetRentalsDto()
        {
            using (RentACarContext context = new RentACarContext())
            {
                var result = from r in context.Rentals
                    join c in context.Cars
                        on r.CarId equals c.Id
                    join cu in context.Customers
                        on r.CustomerId equals cu.Id
                    join u in context.Users
                        on cu.UserId equals u.Id
                    join b in context.Brands
                        on c.BrandId equals b.Id
                    select new RentalDetailDto
                    {
                        Id = r.Id,
                        BrandName = b.Name,
                        CustomerFullName = $"{u.FirstName} {u.LastName}",
                        ReturnDate = r.ReturnDate.Value,
                        DailyPrice = c.DailyPrice,
                        RentDate = r.RentDate.Value
                    };
                return result.ToList();
            }
        }
    }
}
