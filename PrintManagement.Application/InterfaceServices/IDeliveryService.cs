using PrintManagement.Application.Payloads.RequestModels.DeliveryRequest;
using PrintManagement.Application.Payloads.ResponseModels;
using PrintManagement.Application.Payloads.ResponseModels.DataDelivery;
using PrintManagement.Application.Payloads.ResponseModels.DataProject;
using PrintManagement.Application.Payloads.ResponseModels.DataShippingMethod;
using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.InterfaceServices
{
    public interface IDeliveryService
    {
        Task<ResponseObject<DataResponseDelivery>> CreateDelivery(int creatorId, Request_CreateDelivery request);
        Task<ResponsePagedResult<DataResponseDeliveryFull>> GetDeliveryByStatus(DStatus? dStatus, int pageNumber, int pageSize);
        Task<ResponseObject<DataResponseDelivery>> UpdateDelivery(int creatorId, int id, Request_CreateDelivery request);
        Task<ResponseObject<string>> UpdateDeliveryStatusById(int id, int shipperId, DStatus dStatus);
        Task<ResponseObject<string>>ConfirmSuccessfulDelivery(int id, int shipperId);
        Task<int?> GetShipperIdByDeliveryAddressAsync(string deliveryAddress);
        Task<ResponseObject<IEnumerable<DataResponseProject>>> GetCompletedProjectsNotInDeliveryAsync();
        Task<ResponseObject<IEnumerable<DataResponseUser>>> GetEmployeesInDeliveryTeamAsync();
        Task<ResponseObject<IEnumerable<DataResponseShippingMethod>>> GetAllShippingMethod();
        Task<ResponseObject<DataResponseShippingMethod>> GetByIdShippingMethod(int id);
        Task<ResponseObject<string>> GetAddressCustomerByProjectId(int projectid);
        Task<ResponsePagedResult<DataResponseDeliveryFull>> GetDeliveryByShipperIdStatus(int shipperId,DStatus? dStatus, int pageNumber, int pageSize);
    }
}
