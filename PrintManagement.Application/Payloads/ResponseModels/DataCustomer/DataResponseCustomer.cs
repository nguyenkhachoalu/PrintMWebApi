using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.ResponseModels.DataCustomer
{
    public class DataResponseCustomer : DataResponseBase
    {
        public string FullName { get; set; } = string.Empty;
    }
}
