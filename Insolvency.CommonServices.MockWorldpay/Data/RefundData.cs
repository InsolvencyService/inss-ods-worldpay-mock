using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Insolvency.CommonServices.MockWorldpay.Data
{
    public class RefundData: IWorldpayRequest
    {
        public string OrderCode { get; set; }
        public string RefundValue { get; set; }
        public string Currency { get; set; }
    }
}