using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.ResponseModels.DataResources
{
    public class DataResponseResources : DataResponseBase
    {
        public string ResourceName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public RType ResourceType { get; set; }
        public int AvailableQuantity { get; set; }
        public RStatus ResourceStatus { get; set; }
    }
}
