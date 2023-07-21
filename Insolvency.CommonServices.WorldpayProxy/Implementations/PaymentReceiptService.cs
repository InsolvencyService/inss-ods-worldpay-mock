using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Insolvency.CommonServices.WorldpayProxy.Interface;

namespace Insolvency.CommonServices.WorldpayProxy.Implementations
{
    public class PaymentReceiptService : IPaymentReceiptService
    {
        private HttpClient GetHttpClient()
        {
            var client = new HttpClient { BaseAddress = new Uri(ConfigurationManager.AppSettings["PaymentReceiptServiceUrl"]) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public async Task<HttpResponseMessage> UpdatePaymentReceipt(string transactionId, string status)
        {
            using (var client = GetHttpClient())
            {
                var uri = $"paymentreceipt/{transactionId}/updatestatus/{status}";
                var result = await client.PostAsync(uri, new StringContent(""));
                return result;
            }
        }
    }
}