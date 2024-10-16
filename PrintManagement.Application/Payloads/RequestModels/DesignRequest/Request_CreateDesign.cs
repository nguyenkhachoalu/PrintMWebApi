using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.RequestModels.DesignRequest
{
    public class Request_CreateDesign
    {
        public int ProjectId { get; set; }
        public IFormFile FilePath { get; set; }
        
    }
}
