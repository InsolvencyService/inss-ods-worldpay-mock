using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Insolvency.CommonServices.WorldpayProxy.Controllers
{
    public class HealthController : ApiController
    {
        [HttpGet]
        [Route("Health/Ping")]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
