using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Insolvency.CommonServices.WorldpayProxy.Implementations;
using Insolvency.CommonServices.WorldpayProxy.Interface;
using INSS.ODS.Common.Utilities.Logging;

namespace Insolvency.CommonServices.WorldpayProxy.Controllers
{
    public class OrderUpdateController : ApiController
    {
        private readonly IPaymentReceiptService _paymentService;

        public OrderUpdateController()
        {
            _paymentService = new PaymentReceiptService();
        }

        public OrderUpdateController(IPaymentReceiptService paymentService)
        {
            _paymentService = paymentService;
        }


        [HttpPost]
        [Route("orderupdate")]
        public async Task<HttpResponseMessage> Post(HttpRequestMessage requestMessage)
        {
            var xml = await requestMessage.Content.ReadAsStringAsync();

            var update = Helpers.Xml.ParseOrderUpdate(xml);

            Logger.LogInfo($"Received Order Status Update for Order: {update.OrderCode} to status: {update.Status}");
           
            //parse this xml into an object and update payment receipt accordingly
            var result = await _paymentService.UpdatePaymentReceipt(update.OrderCode, update.Status);

            if (result.IsSuccessStatusCode)
            {
                Logger.LogInfo($"Success updating payment receipt for {update.OrderCode}");
                return new HttpResponseMessage {Content = new StringContent("[OK]")};
            }
            else
            {
                var errorcontent = await result.Content.ReadAsStringAsync();
                Logger.LogInfo($"Error updating payment receipt for {update.OrderCode}: {result.StatusCode} {errorcontent}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
           
        }

    }
}
