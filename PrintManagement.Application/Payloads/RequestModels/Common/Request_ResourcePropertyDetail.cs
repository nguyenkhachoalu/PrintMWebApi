using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.RequestModels.Common
{
    public class Request_ResourcePropertyDetail
    {
        public int PropertyId { get; set; }
        public string PropertyDetailName { get; set; } = string.Empty;
        public IFormFile Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
