using PrintManagement.Application.Payloads.RequestModels.ResourceForPrintJobRequest;
using PrintManagement.Application.Payloads.RequestModels.ResourcePropertyDetailRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataPrintJob;
using PrintManagement.Application.Payloads.ResponseModels.DataResourceForPrintJob;
using PrintManagement.Application.Payloads.ResponseModels.DataResourcePropertyDetail;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.InterfaceServices
{
    public interface IPrintJobService
    {
        Task<ResponseObject<IEnumerable<DataResponseResourceForPrintJob>>> CreateResourceForPrintJobs(Request_CreateResourceForPrintJob request);
        Task<ResponseObject<DataResponseResourcePropertyDetail>> UpdateResourcePropertyDetailAsync(int id, Request_UpdateResourcePropertyDetail request);
        Task<ResponseObject<bool>> DeleteResourcePropertyDetailAsync(int id);
        Task<ResponseObject<bool>> UpdatePrintJobStatus(int designId, PjStatus newStatus);
        Task<ResponseObject<IEnumerable<DataResponsePrintJob>>> GetResourcePrintJobByDesign(int designId);
        Task<ResponseObject<IEnumerable<DataResponseResourcePropertyDetail>>> GetResourceDetailByPrintJob(int id);
    }
}
