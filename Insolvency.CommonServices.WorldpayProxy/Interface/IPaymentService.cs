using System.Net.Http;
using System.Threading.Tasks;

namespace Insolvency.CommonServices.WorldpayProxy.Interface
{
    public interface IPaymentService
    {
        HttpResponseMessage PostOrder(string payload);
    }
}
