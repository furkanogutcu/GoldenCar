using Core.Entities;

namespace Entities.Concrete
{
    public class CustomerCreditCard : IEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int CreditCardId { get; set; }
    }
}
