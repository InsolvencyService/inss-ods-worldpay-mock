using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Insolvency.CommonServices.WorldpayProxy.Interface
{
    public interface IPaymentReceiptService
    {
        Task<HttpResponseMessage> UpdatePaymentReceipt(string transactionId, string status);
    }
}