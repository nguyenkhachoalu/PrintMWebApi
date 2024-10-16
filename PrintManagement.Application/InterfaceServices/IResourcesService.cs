using PrintManagement.Application.Payloads.RequestModels.ResourcesRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataResources;
using PrintManagement.Application.Payloads.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.InterfaceServices
{
    public interface IResourcesService
    {
        Task<ResponseObject<IEnumerable<DataResponseResources>>> GetAllResourceAsync();
        Task<ResponseObject<DataResponseResources>> CreateResourcesAsync(Request_Resources request);
        Task<ResponseObject<DataResponseResources>> UpdateResourcesAsync(int id, Request_Resources request);
        Task<ResponseObject<bool>> DeteleteResourcesAsync(int id);
    }
}
