using Entities.Concrete;

namespace Entities.Models
{
    public class RentPaymentRequestModel
    {
        public string CardNumber { get; set; }
        public string ExpireYear { get; set; }
        public string ExpireMonth { get; set; }
        public string Cvc { get; set; }
        public string CardHolderFullName { get; set; }
        public int CustomerId { get; set; }
        public Rental[] Rentals { get; set; }
        public decimal Amount { get; set; }
    }
}
