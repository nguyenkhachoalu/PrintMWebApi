﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.RequestModels.UserRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataNotification;
using PrintManagement.Application.Payloads.ResponseModels.DataRole;
using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Constants;
using PrintManagement.Domain.Entities;

namespace PrintManagement.Controllers
{
    [Route(Constant.DefaultValue.DEFAULTCONTROLLER_ROUTE)]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(string? userName, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return  Ok(await _userService.GetAllUsers(userName,pageNumber, pageSize));
        }
        [HttpGet]
        public async Task<IActionResult> GetUsersWithRoleManagers()
        {
            return Ok(await _userService.GetUsersWithRoleManagers());
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // xac thuc bang json web token
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromForm] Request_UpdateUser request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _userService.UpdateProfile(id,request));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // xac thuc bang json web token
        [HttpGet]
        public async Task<IActionResult> GetProfileById()
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _userService.MyProfileGetById(id));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetNotificationByUserId()
        {
            int userId = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _userService.GetNotificationByUserId(userId));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateIsSeenNotification(int id)
        {
            return Ok(await _userService.UpdateIsSeenNotification(id));
        }
        [HttpPut]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateRoleOfUser(int id,[FromBody]List<string> roleCode)
        {
            return Ok(await _userService.UpdateRoleOfUser(id, roleCode));
        }
        [HttpGet]
        public async Task<IActionResult> GetRoleOfUser(int idUser)
        {
            return Ok(await _userService.GetRoleOfUser(idUser));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRole()
        {
            return Ok(await _userService.GetAllRole());
        }
    }
}
