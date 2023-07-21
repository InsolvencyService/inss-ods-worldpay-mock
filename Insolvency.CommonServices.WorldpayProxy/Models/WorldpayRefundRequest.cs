using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Insolvency.CommonServices.WorldpayProxy.Models
{
    [DataContract]
    public class WorldpayRefundRequest
    {
        [DataMember]
        public string OrderCode { get; set; }

        [DataMember]
        public decimal RefundValue { get; set; }

        [DataMember]
        public string CurrencyCode { get; set; }
    }
}