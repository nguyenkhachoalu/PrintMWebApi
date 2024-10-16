using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.ResponseModels.DataShippingMethod
{
    public class DataResponseShippingMethod : DataResponseBase
    {
        public string ShippingMethodName { get; set; } = string.Empty;
    }
}
