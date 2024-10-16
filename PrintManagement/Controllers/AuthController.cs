using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.RequestModels.AuthRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Constants;

namespace PrintManagement.Controllers
{
    [Route(Constant.DefaultValue.DEFAULTCONTROLLER_ROUTE)]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromForm]Request_Register request)
        {
            return Ok(await _authService.Register(request));
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmRegisterAccount(string confirmCode)
        {
            return Ok(await _authService.ConfirmRegisterAccount(confirmCode));
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]Request_Login request)
        {
            return Ok(await _authService.Login(request));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // xac thuc bang json web token
        public async Task<IActionResult> ChangePassword([FromBody]Request_ChangePassword request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _authService.ChangePassword(id, request));
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string userName)
        {
            return Ok(await _authService.ForgotPassword(userName));
        }
        [HttpPut]
        public async Task<IActionResult> ConfirmForgotPassword(string confirmCode, [FromBody]Request_ForgotPassword request)
        {
            return Ok(await _authService.ConfirmForgotPassword(confirmCode,request));
        }
        [HttpPost]
        public async Task<IActionResult> LogoutAsync(string token)
        {
            return Ok(await _authService.LogoutAsync(token));
        }
    }
}
