
namespace Millionlights.Models
{
    public class Transaction
    {
        public long transactionId { get; set; }
        public string status { get; set; }
        public string country { get; set; }
        public string currency { get; set; }
       public string orderNumber { get; set; }
       public decimal price { get; set; }
       public string courseName { get; set; }
    }
}