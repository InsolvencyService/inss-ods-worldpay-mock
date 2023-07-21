using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using Insolvency.CommonServices.MockWorldpay.Data;

namespace Insolvency.CommonServices.MockWorldpay.Controllers
{
    public class PaymentUpdateController : Controller
    {
        // GET: PaymentUpdate
        public ActionResult Index()
        {
            var model = OrderDataStore.GetAll();
            return View(model);
        }

        public async Task<ActionResult> Update(int update, string[] orderCode, string[] updatedStatus)
        {
            var code = orderCode[update];
            var newstatus = updatedStatus[update];

            var key = $"INSSDRO^{code}";

            var order = OrderDataStore.Get(key).Clone();
            order.Status = newstatus; 

            //send the request to the proxy api
            var xml = Helpers.GeneratePaymentStatusUpdateXml(order);

            var result = await Helpers.SendOrderStatusUpdate(xml);

            //update in local session store if successfully changed
            if (result)
            {
                OrderDataStore.Get(key).Status = newstatus;
            }

            return RedirectToAction("Index");
        }



    }
}