using PrintManagement.Application.Payloads.ResponseModels.DataProject;
using PrintManagement.Application.Payloads.ResponseModels.DataResourceForPrintJob;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Mappers
{
    public class ResourceForPrintJobConveter
    {
        public DataResponseResourceForPrintJob EntitytoDTO(ResourceForPrintJob resourceForPrintJob)
        {
            return new DataResponseResourceForPrintJob()
            {
                Id = resourceForPrintJob.Id,
                ResourcePropertyDetailId = resourceForPrintJob.ResourcePropertyDetailId,
                PrintJobId = resourceForPrintJob.PrintJobId,
            };
        }
    }
}
