using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.ResponseModels.DataResourceForPrintJob
{
    public class DataResponseResourceForPrintJob : DataResponseBase
    {
        public int ResourcePropertyDetailId { get; set; }
        public int PrintJobId { get; set; }

    }
}
