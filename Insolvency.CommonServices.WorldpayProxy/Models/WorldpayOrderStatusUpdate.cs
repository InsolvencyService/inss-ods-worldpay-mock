using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Insolvency.CommonServices.WorldpayProxy.Models
{
    public class WorldpayOrderStatusUpdate
    {
        [DataMember]
        public string OrderCode { get; set; }

        [DataMember]
        public string Status { get; set; }
    }
}