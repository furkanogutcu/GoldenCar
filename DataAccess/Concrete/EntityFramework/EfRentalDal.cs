using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    select new RentalDetailDto
                    {
                        Id = r.Id,
                        ReturnDate = r.ReturnDate.Value,
                        DailyPrice = c.DailyPrice,
                        RentDate = r.RentDate.Value
                    };
                return result.ToList();
            }
        }
    }
}
