using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insolvency.CommonServices.MockWorldpay.Data
{
    internal interface IWorldpayRequest
    {
        string OrderCode { get; }
    }
}
