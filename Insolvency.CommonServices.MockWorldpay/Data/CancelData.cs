using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Insolvency.CommonServices.MockWorldpay.Data
{
    public class CancelData : IWorldpayRequest
    {
        public string OrderCode { get; set; }
    }
}