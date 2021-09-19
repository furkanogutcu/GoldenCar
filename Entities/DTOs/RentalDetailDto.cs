using System;
using Core.Entities;

namespace Entities.DTOs
{
    public class RentalDetailDto:IDto
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string ModelFullName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public decimal DailyPrice { get; set; }
        public DateTime RentDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool? DeliveryStatus { get; set; }
    }
}
