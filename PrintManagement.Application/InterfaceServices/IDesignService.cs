using PrintManagement.Application.Payloads.RequestModels.DesignRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataDesign;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.InterfaceServices
{
    public interface IDesignService
    {
        Task<ResponseObject<IEnumerable<DataResponseDesign>>> GetDesignByProjectAsyc (DeStatus? deStatus,int projectId);
        Task<ResponseObject<DataResponseDesign>> GetDesignById(int Id);
        Task<ResponseObject<DataResponseDesign>> UploadFileDesig(int userId, Request_CreateDesign request);
        Task<ResponseObject<DataResponseDesign>> ApprovedDesign(int approveId, Request_Approve request);
    }
}
