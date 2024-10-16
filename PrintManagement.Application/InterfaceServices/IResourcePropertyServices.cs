using PrintManagement.Application.Payloads.RequestModels.ResourcePropertyRequest;
using PrintManagement.Application.Payloads.RequestModels.ResourcesRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataResourceProperty;
using PrintManagement.Application.Payloads.ResponseModels.DataResources;
using PrintManagement.Application.Payloads.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.InterfaceServices
{
    public interface IResourcePropertyServices
    {
        Task<ResponseObject<IEnumerable<DataResponseResourceProperty>>> GetAllResourcePropertiesAsync();
        Task<ResponseObject<DataResponseResourceProperty>> CreateResourcePropertyAsync(Request_ResourceProperty request);
        Task<ResponseObject<DataResponseResourceProperty>> UpdateResourcePropertyAsync(int id, Request_ResourceProperty request);
        Task<ResponseObject<bool>> DeteleteResourcePropertyAsync(int id);
        Task<ResponseObject<DataResponseResourceProperty>> UpdateQuantityPropertyAsync(int id, int quantity);
        Task<ResponseObject<IEnumerable<DataResponseResourceProperty>>> GetResourcePropertyByResourceId(int resourceId);
    }
}
