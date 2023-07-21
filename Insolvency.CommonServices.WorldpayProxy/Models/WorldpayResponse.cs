using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Insolvency.CommonServices.WorldpayProxy.Models
{
    [DataContract]
    public class WorldpayResponse
    {
        [DataMember]
        public string OrderCode { get; set; }
        [DataMember]
        public string RedirectUrl { get; set; }
        [DataMember]
        public string Error { get; set; }
    }
}