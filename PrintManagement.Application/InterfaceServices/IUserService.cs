using Org.BouncyCastle.Pqc.Crypto.Lms;
using PrintManagement.Application.Payloads.RequestModels.UserRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataNotification;
using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.InterfaceServices
{
    public interface IUserService
    {
        Task<ResponseObject<DataResponseUser>> UpdateProfile(int id, Request_UpdateUser request);
        Task<ResponseObject<DataResponseUser>> MyProfileGetById(int id);
        Task<ResponseObject<DataResponseUser>> UpdateTeamUser(int id,int teamId);
        Task<ResponsePagedResult<DataResponseUser>> GetAllUsers(int pageNumber, int pageSize);
        Task<ResponseObject<DataResponseUser>> GetUserById(int id);
        Task<ResponseObject<IEnumerable<DataResponseUser>>> GetUsersWithRoleManagers();
        Task<ResponseObject<IEnumerable<DataResponseNotification>>> GetNotificationByUserId(int userId);
        Task<ResponseObject<string>> UpdateIsSeenNotification(int id);
        Task<ResponseObject<string>> UpdateRoleOfUser(int id, List<string> roleCode);
    }
}
