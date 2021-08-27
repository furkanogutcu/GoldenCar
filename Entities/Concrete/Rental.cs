using System;
using Core.Entities;

namespace Entities.Concrete
{
    public class Rental:IEntity
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int CustomerId { get; set; }
        public DateTime? RentDate { get; set; } //Datetime = Values cannot be null -- Datetime? = Nullable Datetime (Values can be null)
        public DateTime? ReturnDate { get; set; }
    }
}
