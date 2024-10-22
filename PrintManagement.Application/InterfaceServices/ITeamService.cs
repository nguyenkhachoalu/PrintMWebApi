using PrintManagement.Application.Payloads.RequestModels.TeamRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataTeams;
using PrintManagement.Application.Payloads.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.InterfaceServices
{
    public interface ITeamService
    {
        Task<ResponsePagedResult<DataResponseTeam>> GetAllTeams(string? teamName, int pageNumber, int pageSize);
        Task<ResponseObject<IEnumerable<ResponseDepartments>>> getDepartments();
        Task<ResponseObject<DataResponseTeam>> GetTeamById(int id);
        Task<ResponseObject<DataResponseTeam>> UpdateTeam(int id, Request_Team request);
        Task<ResponseObject<DataResponseTeam>> CreateTeam(Request_Team request);
        Task<ResponseObject<bool>> DeleteTeam(int id);
        Task<ResponseObject<DataResponseTeam>> UpdateTeamManager(int id, int userId);


    }
}
