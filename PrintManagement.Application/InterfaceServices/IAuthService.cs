using PrintManagement.Application.Payloads.RequestModels.AuthRequest;
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
    public interface IAuthService
    {
        Task<ResponseObject<DataResponseUser>> Register(Request_Register request);
        Task<string> ConfirmRegisterAccount(string confirmCode);
        Task<ResponseObject<DataResponseLogin>> Login(Request_Login request);
        Task<ResponseObject<DataResponseLogin>> GetJwtTokenAsync(User user);
        Task<(string accessToken, string refreshToken)> RefreshTokenAsync(Request_Token request);

        Task<ResponseObject<DataResponseUser>> ChangePassword(int userId,Request_ChangePassword request);
        Task<string> ForgotPassword(string userName);
        Task<string> ConfirmForgotPassword(string confirmCode,Request_ForgotPassword request);
        Task<ResponseObject<string>> LogoutAsync(string token);
    }
}
