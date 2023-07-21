using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Insolvency.CommonServices.WorldpayProxy.Implementations;
using Insolvency.CommonServices.WorldpayProxy.Interface;

namespace Insolvency.CommonServices.WorldpayProxy.Controllers
{
    public class ProxyController : ApiController
    {
        private readonly IPaymentService _paymentService;

        public ProxyController()
        {
            _paymentService = new WorldpayPaymentService();
        }

        public ProxyController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }


        [HttpPost]
        [Route("proxy")]
        public async Task<HttpResponseMessage> Post(HttpRequestMessage requestMessage)
        {
            //Read the request body
            var requestBody = await requestMessage.Content.ReadAsStringAsync();

            //Pass the request body to the payment service implementation, and return its result
            return _paymentService.PostOrder(requestBody);
        }
    }
}
