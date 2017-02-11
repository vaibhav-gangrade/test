using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Millionlights.Models
{
    public class Orders
    {
        private MillionlightsContext db = new MillionlightsContext();
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int OrderID{ get; set; }
        public string OrderNumber { get; set; }
        public int UserID { get; set; }
        
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity{ get; set; }
        public string ShippingState{ get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingZipCode { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2{ get; set; }
        public string BillingCity{ get; set; }
        public string BillingState { get; set; }
        public string BillingCountry{ get; set; }
        public string BillingZipCode{ get; set; }
        public int OrderStatusID { get; set; }
        public string OrderStatus { get; set;}
        
        public DateTime OrderedDatetime{ get; set; }
        public string OrderedDatetimeString
        {
            get
            {
                if (OrderedDatetime != null)
                {
                    return ((DateTime)OrderedDatetime).ToString(@"dd/MM/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public DateTime OrderModifiedOn{ get; set; }

        public bool IsActive   { get; set; }

    }
}