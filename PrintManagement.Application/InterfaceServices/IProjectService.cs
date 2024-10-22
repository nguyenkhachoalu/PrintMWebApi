using PrintManagement.Application.Payloads.RequestModels.ProjectRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataCustomer;
using PrintManagement.Application.Payloads.ResponseModels.DataProject;
using PrintManagement.Application.Payloads.ResponseModels.DataTeams;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.InterfaceServices
{
    public interface IProjectService
    {
        Task<ResponseObject<DataResponseProject>> CreateProject(int idUser, Request_CreateProject request);
        Task<ResponsePagedResult<DataResponseProject>> GetAllProject(string? projectName, PStatus? status, int pageNumber, int pageSize);
        Task<IEnumerable<DataResponseCustomer>> GetAllIdNameCustomer();
        Task<ResponseObject<DataResponseProject>> GetProjectById(int id);
    }
}
