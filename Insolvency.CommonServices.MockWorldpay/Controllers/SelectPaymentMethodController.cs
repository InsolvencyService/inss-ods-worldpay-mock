using System;
using System.Net;
using System.Web.Http.Results;
using System.Web.Mvc;
using Insolvency.CommonServices.MockWorldpay.Data;

namespace Insolvency.CommonServices.MockWorldpay.Controllers
{
    //Mocks Worldpay's user interaction pages
    public class SelectPaymentMethodController : Controller
    {
        // GET: SelectPaymentMethod
        public ActionResult Index(string orderKey, string successURL, string pendingURL, string failureURL, string cancelURL)
        {
            try
            {
                var data = OrderDataStore.Get(orderKey);

                if (data == null)
                {
                    return new HttpNotFoundResult("Order not found");
                }

                ViewBag.OrderKey = orderKey;
                ViewBag.SuccessURL = successURL;
                ViewBag.PendingURL = pendingURL;
                ViewBag.FailureURL = failureURL;
                ViewBag.CancelURL = cancelURL;
                return View(data);

            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "An error occurred retrieving specified order");
            }
        }

        [HttpPost]
        public ActionResult Submit(string orderKey, string successURL, string pendingURL, string failureURL, string cancelURL, string result)
        {
            var data = OrderDataStore.Get(orderKey);

            if (data == null)
            {
                return new HttpNotFoundResult("Order not found");
            }

            var url = "";
            var paymentStatus = "";

            switch (result)
            {
                case "success":
                    url = successURL;
                    paymentStatus = "AUTHORISED";
                    break;
                case "refuse":
                    url = failureURL;
                    paymentStatus = "REFUSED";
                    break;
                default:
                    url += cancelURL;
                    paymentStatus = "";
                    break;
            }

            //Update our cache
            data.Status = paymentStatus;

            //Add the AdminCode to the key for purposes of Mac
            orderKey = Constants.AdminCode + "^" + orderKey;

            //Hash the mac
            var mac = Helpers.CreateMacHash(orderKey, data.Value, data.Currency, paymentStatus);

            //Add the required values to the query string
            url = $"{url}?orderKey={orderKey}";

            //When a shopper cancels a payment, the redirect URL will not contain the parameter paymentStatus.
            if (!String.IsNullOrWhiteSpace(paymentStatus))
            {
                url = $"{url}&paymentStatus={paymentStatus}";
            }

            url = $"{url}&paymentAmount={data.Value}&paymentCurrency={data.Currency}&mac={mac}";

            return Redirect(url);
        }

    }
}