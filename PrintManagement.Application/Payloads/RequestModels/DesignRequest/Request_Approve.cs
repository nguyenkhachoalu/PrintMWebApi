using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.RequestModels.DesignRequest
{
    public class Request_Approve
    {
        public int DesignId { get; set; }
        public DeStatus DesignStatus { get; set; }
        public string Link { get; set; } = string.Empty;
    }
}
