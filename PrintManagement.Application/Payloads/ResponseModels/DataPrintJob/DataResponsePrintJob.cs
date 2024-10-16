using Microsoft.AspNetCore.Http;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.ResponseModels.DataPrintJob
{
    public class DataResponsePrintJob : DataResponseBase
    {
        public int DesignId { get; set; }
        public PjStatus PrintJobStatus { get; set; }
        
    }
}
