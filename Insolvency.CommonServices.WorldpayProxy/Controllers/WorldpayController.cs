using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Insolvency.CommonServices.WorldpayProxy.Helpers;
using Insolvency.CommonServices.WorldpayProxy.Implementations;
using Insolvency.CommonServices.WorldpayProxy.Interface;
using Insolvency.CommonServices.WorldpayProxy.Models;
using System;
using INSS.ODS.Common.Utilities.Logging;

namespace Insolvency.CommonServices.WorldpayProxy.Controllers
{
    public class WorldpayController : ApiController
    {
        private readonly IPaymentService _paymentService;

        public WorldpayController()
        {
            _paymentService = new WorldpayPaymentService();
        }

        public WorldpayController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Route("worldpay/{installationId}")]
        public async Task<HttpResponseMessage> Post([FromUri] string installationId, WorldpayOrder worldpayOrder)
        {
            Logger.LogInfo($"Creating order for order code: {worldpayOrder.OrderCode}");

            try
            {
                var xml = Xml.CreateOrderXml(installationId, worldpayOrder);

                var response = _paymentService.PostOrder(xml);

                if (response.IsSuccessStatusCode)
                {
                    var responseContentXml = await response.Content.ReadAsStringAsync();
                    Logger.LogDebug(responseContentXml);

                    var worldPayResponse = Xml.ParseReplyXml(responseContentXml);
                    return Request.CreateResponse(HttpStatusCode.OK, worldPayResponse);
                }

                var errordetail = response.StatusCode + " - " + response.Content.ReadAsStringAsync();
                var worldpayResponse = new WorldpayResponse {Error = errordetail};

                return Request.CreateResponse(HttpStatusCode.OK, worldpayResponse);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An exception was thrown", ex);
            }
        }


        [HttpPost]
        [Route("worldpay/{installationId}/refund")]
        public async Task<HttpResponseMessage> PostRefund([FromUri] string installationId,
            WorldpayRefundRequest refundRequest)
        {
            Logger.LogInfo($"Creating refund for order code: {refundRequest.OrderCode}");

            try
            {
                var xml = Xml.CreateRefundXml(installationId, refundRequest);

                var response = _paymentService.PostOrder(xml);

                if (response.IsSuccessStatusCode)
                {
                    var responseContentXml = await response.Content.ReadAsStringAsync();
                    Logger.LogDebug(responseContentXml);

                    var worldPayResponse = Xml.ParseRefundRequestReplyXml(responseContentXml);

                    if (!worldPayResponse)
                    {
                        Logger.LogError("Worldpay response error");
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, worldPayResponse);
                }

                var errordetail = response.StatusCode + " - " + response.Content.ReadAsStringAsync();
                var worldpayResponse = new WorldpayResponse {Error = errordetail};

                return Request.CreateResponse(HttpStatusCode.OK, worldpayResponse);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An exception was thrown", ex);
            }
        }


        [HttpPost]
        [Route("worldpay/{installationId}/cancel/{orderCode}")]
        public async Task<HttpResponseMessage> PostCancelOrder([FromUri] string installationId, [FromUri] string orderCode)
        {
            Logger.LogInfo($"Creating cancellation for order code: {orderCode}");

            try
            {
                var xml = Xml.CreateCancelOrRefundXml(installationId, orderCode);

                var response = _paymentService.PostOrder(xml);

                if (response.IsSuccessStatusCode)
                {
                    var responseContentXml = await response.Content.ReadAsStringAsync();
                    Logger.LogDebug(responseContentXml);

                    var worldPayResponse = Xml.ParseCancelOrRefundRequestReplyXml(responseContentXml);

                    if (!worldPayResponse)
                    {
                        Logger.LogError("Worldpay response error");
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, worldPayResponse);
                }

                var errordetail = response.StatusCode + " - " + response.Content.ReadAsStringAsync();
                var worldpayResponse = new WorldpayResponse { Error = errordetail };

                return Request.CreateResponse(HttpStatusCode.OK, worldpayResponse);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An exception was thrown", ex);
            }

        }


    }
}
