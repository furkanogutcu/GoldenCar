using Entities.Concrete;

namespace Entities.Models
{
    public class CustomerCreditCardModel
    {
        public int CustomerId { get; set; }
        public CreditCard CreditCard { get; set; }
    }
}
