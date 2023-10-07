namespace ECommerceApp.Models
{
    public class CreditCardModel
    {
        public long creditCardNumber { get; set; }
        public DateTime expiryDate { get; set; }
        public int cvv { get; set; }
    }
}
