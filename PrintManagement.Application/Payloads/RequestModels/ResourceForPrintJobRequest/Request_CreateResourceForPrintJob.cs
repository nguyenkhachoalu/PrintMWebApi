using Microsoft.AspNetCore.Http;
using PrintManagement.Application.Payloads.RequestModels.Common;
using PrintManagement.Application.Payloads.RequestModels.ResourcePropertyDetailRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.RequestModels.ResourceForPrintJobRequest
{
    public class Request_CreateResourceForPrintJob
    {
        public int PrintJobId { get; set; }
        public List<Request_ResourcePropertyDetail> ResourceDetails { get; set; }
    }
}
