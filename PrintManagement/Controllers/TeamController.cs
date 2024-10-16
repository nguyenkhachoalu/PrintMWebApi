using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.RequestModels.TeamRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataTeams;
using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Constants;
namespace PrintManagement.Controllers
{
    [Route(Constant.DefaultValue.DEFAULTCONTROLLER_ROUTE)]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;

        public TeamController(ITeamService teamService, IUserService userService)
        {
            _teamService = teamService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeams([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _teamService.GetAllTeams(pageNumber, pageSize));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamById(int id)
        {
            return Ok(await _teamService.GetTeamById(id));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, [FromBody] Request_Team request)
        {
            return Ok(await _teamService.UpdateTeam(id, request));
        }
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] Request_Team request)
        {
            return Ok(await _teamService.CreateTeam(request));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            return Ok(await _teamService.DeleteTeam(id));
        }
        [HttpPut("update_team/{id}")]
        public async Task<IActionResult> UpdateTeamUser(int id, int teamId)
        {
            return Ok(await _userService.UpdateTeamUser(id,teamId));
        }
        [HttpPut("update_leader/{id}")]
        public async Task<IActionResult> UpdateTeamManager(int id, int userId)
        {
            return Ok(await _teamService.UpdateTeamManager(id, userId));
        }
        [HttpGet]
        public async Task<IActionResult> getDepartments()
        {
            return Ok(await _teamService.getDepartments());
        }
    }
}
