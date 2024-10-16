using Microsoft.AspNetCore.Http;
using PrintManagement.Application.Payloads.RequestModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.RequestModels.ResourcePropertyDetailRequest
{
    public class Request_CreateResourcePropertyDetails
    {
        public List<Request_ResourcePropertyDetail> ResourceDetails { get; set; }
    }


}
