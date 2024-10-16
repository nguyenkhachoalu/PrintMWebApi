using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.RequestModels.ResourcePropertyRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataResourceProperty;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Constants;

namespace PrintManagement.Controllers
{
    [Route(Constant.DefaultValue.DEFAULTCONTROLLER_ROUTE)]
    [ApiController]
    [Authorize]
    public class ResourcePropertyController : Controller
    {
        private readonly IResourcePropertyServices _resourcePropertyServices;

        public ResourcePropertyController(IResourcePropertyServices resourcePropertyServices)
        {
            _resourcePropertyServices = resourcePropertyServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllResourcePropertiesAsync()
        {
            return Ok(await _resourcePropertyServices.GetAllResourcePropertiesAsync());
        }
        [HttpPost]
        public async Task<IActionResult> CreateResourcePropertyAsync(Request_ResourceProperty request)
        {
            if (request == null)
            {
                return BadRequest("Request object is null");
            }
            return Ok(await _resourcePropertyServices.CreateResourcePropertyAsync(request));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateResourcePropertyAsync(int id, Request_ResourceProperty request)
        {
            return Ok(await _resourcePropertyServices.UpdateResourcePropertyAsync(id,request));
        }
        [HttpDelete]
        public async Task<IActionResult> DeteleteResourcePropertyAsync(int id)
        {
            return Ok(await _resourcePropertyServices.DeteleteResourcePropertyAsync(id));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateQuantityPropertyAsync(int id, int quantity)
        {
            return Ok(await _resourcePropertyServices.UpdateQuantityPropertyAsync(id,quantity));
        }
        [HttpGet]
        public async Task<IActionResult> GetResourcePropertyByResourceId(int resourceId)
        {
            return Ok(await _resourcePropertyServices.GetResourcePropertyByResourceId(resourceId));
        }
    }
}
