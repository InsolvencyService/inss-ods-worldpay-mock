using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Insolvency.CommonServices.WorldpayProxy.Models
{
    [DataContract]
    public class WorldpayOrder
   {
        [DataMember]
        public string OrderCode { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public decimal OrderValue { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public bool IncludeAddress { get; set; }
        [DataMember]
        public string Address1 { get; set; }
        [DataMember]
        public string Address2 { get; set; }
        [DataMember]
        public string Address3 { get; set; }
        [DataMember]
        public string PostCode { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string TelephoneNumber { get; set; }
        [DataMember]
        public string CurrencyCode { get; set; }
    }
}