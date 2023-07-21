using System.Web.Mvc;

namespace Insolvency.CommonServices.MockWorldpay.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Mock Worldpay";
            return View();
        }
    }
}
