using System;

namespace Insolvency.CommonServices.MockWorldpay.Data
{
    public class OrderData: IWorldpayRequest
    {
        public string OrderCode { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string MerchantCode { get; set; }
        public string Currency { get; set; }

        public string ValueInPounds
        {
            get
            {
                var amount = Decimal.Parse(Value);
                amount /= 100;
                return amount.ToString("C");
            }
        }

        public string Status { get; set; }

        public OrderData Clone()
        {
            return new OrderData
            {
                OrderCode = OrderCode,
                Description = Description,
                Value = Value,
                MerchantCode =  MerchantCode,
                Currency =  Currency               
            };
        }
    }
}