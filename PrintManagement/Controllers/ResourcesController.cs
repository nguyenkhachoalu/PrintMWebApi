using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.RequestModels.ResourcesRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataResources;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Constants;

namespace PrintManagement.Controllers
{
    [Route(Constant.DefaultValue.DEFAULTCONTROLLER_ROUTE)]
    [ApiController]
    [Authorize]
    public class ResourcesController : Controller
    {
        private readonly IResourcesService _resourcesService;

        public ResourcesController(IResourcesService resourcesService)
        {
            _resourcesService = resourcesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllResourceAsync()
        {
            return Ok(await _resourcesService.GetAllResourceAsync());
        }
        [HttpPost]
        public async Task<IActionResult> CreateResourcesAsync([FromForm]Request_Resources request)
        {
            return Ok(await _resourcesService.CreateResourcesAsync(request));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateResourcesAsync(int id, [FromBody] Request_Resources request)
        {
            return Ok(await _resourcesService.UpdateResourcesAsync(id,request));
        }
        [HttpDelete]
        public async Task<IActionResult> DeteleteResourcesAsync(int id)
        {
            return Ok(await _resourcesService.DeteleteResourcesAsync(id));
        }
    }
}
