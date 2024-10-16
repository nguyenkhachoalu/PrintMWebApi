using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.ResponseModels.DataResourceProperty
{
    public class DataResponseResourceProperty : DataResponseBase
    {
        public string ResourcePropertyName { get; set; } = string.Empty;
        public int ResourceId { get; set; }
        public int Quantity { get; set; }

    }
}
