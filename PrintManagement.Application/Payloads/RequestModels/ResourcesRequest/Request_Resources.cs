using Microsoft.AspNetCore.Http;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.RequestModels.ResourcesRequest
{
    public class Request_Resources
    {
        public string ResourceName { get; set; } = string.Empty;
        public IFormFile Image { get; set; }
        public RType ResourceType { get; set; }
        public int AvailableQuantity { get; set; }
        public RStatus ResourceStatus { get; set; }
    }
}
