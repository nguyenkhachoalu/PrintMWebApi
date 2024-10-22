using Microsoft.AspNetCore.Http;
using PrintManagement.Application.Handle.HandleFile;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.Mappers;
using PrintManagement.Application.Payloads.RequestModels.ProjectRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataCustomer;
using PrintManagement.Application.Payloads.ResponseModels.DataProject;
using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace PrintManagement.Application.ImplementServices
{
    public class ProjectService : IProjectService
    {
        private readonly IBaseRepository<Project> _baseProjectRepository;
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IBaseRepository<Team> _baseTeamRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ProjectConverter _projectConverter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IBaseRepository<Project> baseProjectRepository, IBaseRepository<User> baseUserRepository, IBaseRepository<Team> baseTeamRepository, IUserRepository userRepository, ICustomerRepository customerRepository, ProjectConverter projectConverter, IHttpContextAccessor httpContextAccessor, IProjectRepository projectRepository)
        {
            _baseProjectRepository = baseProjectRepository;
            _baseUserRepository = baseUserRepository;
            _baseTeamRepository = baseTeamRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _projectConverter = projectConverter;
            _httpContextAccessor = httpContextAccessor;
            _projectRepository = projectRepository;
        }

        public async Task<ResponseObject<DataResponseProject>> CreateProject(int idUser, Request_CreateProject request)
        {
            try
            {
                var user = await _baseUserRepository.GetByIdAsync(idUser);
                
                var roles = await _userRepository.GetRolesOfUserAsync(user);
                #region Valid
                //kiểm tra quyền hạn
                if (user == null)
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người này không tồn tại",
                        Data = null
                    };
                }
                if (!roles.Contains("Employee"))
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Bạn không có quyền tạo project",
                        Data = null
                    };
                }
                if (user.TeamId == null) 
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người này chưa nằm trong team nào",
                        Data = null
                    };
                }
                var team = await _baseTeamRepository.GetByIdAsync((int)user.TeamId);
                if (team.Name.ToLower() != "sales")
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "Team Không có quyền tạo project",
                        Data = null
                    };
                }

                //kiểm tra nhập liệu
                if (request.StartDate < DateTime.Now)
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Ngày bắt đầu phải lớn hơn ngày hiện tại",
                        Data = null
                    };
                }
                if (request.ExpectedEndDate <= request.StartDate)
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Ngày kết thúc dự kiến phải lớn hơn ngày bắt đầu",
                        Data = null
                    };
                }

                #endregion

                var project = new Project
                {
                    ProjectName = request.ProjectName,
                    RequestDescriptionFromCustomer = request.RequestDescriptionFromCustomer,
                    StartDate = request.StartDate,
                    Image = await HandleUploadFileProject.WirteFile(request.Image),
                    EmployeeId = idUser,
                    ExpectedEndDate = request.ExpectedEndDate,
                    CustomerId = request.CustomerId,
                    ProjectStatus = 0
                };
                await _baseProjectRepository.CreateAsync(project);
                return new ResponseObject<DataResponseProject>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Đã tạo thành công project",
                    Data = _projectConverter.EntitytoDTO(project)
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseProject>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null
                };
            }

        }

        public async Task<IEnumerable<DataResponseCustomer>> GetAllIdNameCustomer()
        {
            var customers = await _customerRepository.GetIdNameCustomer();
            var response = customers.Select(c => new DataResponseCustomer
            {
                Id = c.Id,
                FullName = c.FullName
            }).ToList();

            return response;
        }

        public async Task<ResponsePagedResult<DataResponseProject>> GetAllProject(string? projectName, PStatus? status, int pageNumber, int pageSize)
        {
            try
            {
                var projects = await _projectRepository.GetAllProjectAsync(projectName, status);
                var hostUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

                // Thay đổi quá trình mapping để thêm URL đầy đủ cho avatar
                var responseProject = projects.Select(t => new DataResponseProject
                {
                    Id = t.Id,
                    ProjectName = t.ProjectName,
                    RequestDescriptionFromCustomer = t.RequestDescriptionFromCustomer,
                    StartDate = t.StartDate,
                    Image = $"{hostUrl}/images/projects/{t.Image}",
                    EmployeeId = t.EmployeeId,
                    ExpectedEndDate = t.ExpectedEndDate,
                    CustomerId = t.CustomerId,
                    ProjectStatus = t.ProjectStatus
                
                }).ToPagedList(pageNumber, pageSize);

                return new ResponsePagedResult<DataResponseProject>
                {
                    Items = responseProject.ToList(),
                    TotalPages = responseProject.PageCount,
                    TotalItems = responseProject.TotalItemCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                return new ResponsePagedResult<DataResponseProject>
                {
                    Items = null,
                    TotalPages = 0,
                    TotalItems = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }

        public async Task<ResponseObject<DataResponseProject>> GetProjectById(int id)
        {
            try
            {
                var project = await _baseProjectRepository.GetByIdAsync(id);
                var hostUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                if (project == null)
                {
                    return new ResponseObject<DataResponseProject>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Dự án không tồn tại",
                        Data = null
                    };
                }
                var responseProject = new DataResponseProject
                {
                    ProjectName = project.ProjectName,
                    RequestDescriptionFromCustomer = project.RequestDescriptionFromCustomer,
                    StartDate = project.StartDate,
                    Image = $"{hostUrl}/images/projects/{project.Image}",
                    EmployeeId = project.EmployeeId,
                    ExpectedEndDate = project.ExpectedEndDate,
                    CustomerId = project.CustomerId,
                    ProjectStatus = project.ProjectStatus

                };
                return new ResponseObject<DataResponseProject>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Lấy thành công dự án",
                    Data = responseProject
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseProject>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: "+ex.Message,
                    Data = null
                };
            }
        }
    }
}
