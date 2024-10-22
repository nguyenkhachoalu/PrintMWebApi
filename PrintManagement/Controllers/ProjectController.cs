using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.ImplementServices;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.RequestModels.ProjectRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataCustomer;
using PrintManagement.Application.Payloads.ResponseModels.DataProject;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Constants;
using PrintManagement.Domain.Entities;

namespace PrintManagement.Controllers
{
    [Route(Constant.DefaultValue.DEFAULTCONTROLLER_ROUTE)]
    [ApiController]
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateProject([FromForm] Request_CreateProject request)
        {
            int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _projectService.CreateProject(id, request));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProject(string? projectName, PStatus? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _projectService.GetAllProject(projectName, status, pageNumber, pageSize));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllIdNameCustomer()
        {
            return Ok(await _projectService.GetAllIdNameCustomer());
        }
        [HttpGet("Id={id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            return Ok(await _projectService.GetProjectById(id));
        }

    }
}
