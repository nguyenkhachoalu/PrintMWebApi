using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.RequestModels.DesignRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataDesign;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Constants;
using PrintManagement.Domain.Entities;

namespace PrintManagement.Controllers
{
    [Route(Constant.DefaultValue.DEFAULTCONTROLLER_ROUTE)]
    [ApiController]
    [Authorize]
    public class DesignController : Controller
    {
        private readonly IDesignService _designService;

        public DesignController(IDesignService designService)
        {
            _designService = designService;
        }

        [HttpGet("design/{Id}")]
        public async Task<IActionResult> GetDesignById(int Id)
        {
            return Ok(await _designService.GetDesignById(Id));
        }
        [HttpGet]
        public async Task<IActionResult> GetDesignByProjectAsyc(DeStatus? deStatus,int projectId)
        {
            return Ok(await _designService.GetDesignByProjectAsyc(deStatus, projectId));
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ApprovedDesign([FromBody] Request_Approve request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _designService.ApprovedDesign(id, request));
        }
        [HttpPost]
        public async Task<IActionResult> UploadFileDesig([FromForm] Request_CreateDesign request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _designService.UploadFileDesig(id, request));
        }
    }
}
