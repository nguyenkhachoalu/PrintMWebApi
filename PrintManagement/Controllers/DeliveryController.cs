using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.RequestModels.DeliveryRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataDelivery;
using PrintManagement.Application.Payloads.ResponseModels.DataProject;
using PrintManagement.Application.Payloads.ResponseModels.DataShippingMethod;
using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Constants;
using PrintManagement.Domain.Entities;
using System.Net.NetworkInformation;

namespace PrintManagement.Controllers
{
    [Route(Constant.DefaultValue.DEFAULTCONTROLLER_ROUTE)]
    [ApiController]
    [Authorize]
    public class DeliveryController : Controller
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveryController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> CreateDelivery( Request_CreateDelivery request)
        {
            int creatorId = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _deliveryService.CreateDelivery(creatorId, request));
        }
        [HttpGet]
        public async Task<IActionResult> GetDeliveryByStatus(DStatus? dStatus, int pageNumber, int pageSize)
        {
            return Ok(await _deliveryService.GetDeliveryByStatus(dStatus, pageNumber, pageSize));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetDeliveryByShipperIdStatus( DStatus? dStatus, int pageNumber, int pageSize)
        {
            int shipperId = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _deliveryService.GetDeliveryByShipperIdStatus(shipperId, dStatus, pageNumber, pageSize));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult> UpdateDelivery( int id, Request_CreateDelivery request)
        {
            int creatorId = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _deliveryService.UpdateDelivery(creatorId,id,request));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult> UpdateDeliveryStatusById(int id, DStatus dStatus)
        {
            int shipperId = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _deliveryService.UpdateDeliveryStatusById(id, shipperId, dStatus));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult> ConfirmSuccessfulDelivery(int id)
        {
            int shipperId = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _deliveryService.ConfirmSuccessfulDelivery(id, shipperId));

        }
        [HttpGet]
        public async Task<IActionResult> GetShipperIdByDeliveryAddressAsync(string deliveryAddress)
        {
            return Ok(await _deliveryService.GetShipperIdByDeliveryAddressAsync(deliveryAddress));
        }
        [HttpGet]
        public async Task<IActionResult> GetCompletedProjectsNotInDeliveryAsync()
        {
            return Ok(await _deliveryService.GetCompletedProjectsNotInDeliveryAsync());
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeesInDeliveryTeamAsync()
        {
            return Ok(await _deliveryService.GetEmployeesInDeliveryTeamAsync());
        }
        [HttpGet]
        public async Task<IActionResult> GetAllShippingMethod()
        {
            return Ok(await _deliveryService.GetAllShippingMethod());
        }
        [HttpGet]
        public async Task<IActionResult> GetByIdShippingMethod(int id)
        {
            return Ok(await _deliveryService.GetByIdShippingMethod(id));
        }
        [HttpGet]
        public async Task<IActionResult> GetAddressCustomerByProjectId(int projectid)
        {
            return Ok(await _deliveryService.GetAddressCustomerByProjectId(projectid));
        }
    }
}
