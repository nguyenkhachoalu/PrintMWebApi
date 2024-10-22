using Microsoft.AspNetCore.Http;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.Mappers;
using PrintManagement.Application.Payloads.RequestModels.TeamRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataTeams;
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
    public class TeamServices : ITeamService
    {
        private readonly IBaseRepository<Team> _baseTeamRepository;
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly TeamConverter _teamConverter;

        public TeamServices(IBaseRepository<Team> baseTeamRepository, IBaseRepository<User> baseUserRepository, ITeamRepository teamRepository, IUserRepository userRepository, TeamConverter teamConverter)
        {
            _baseTeamRepository = baseTeamRepository;
            _baseUserRepository = baseUserRepository;
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _teamConverter = teamConverter;
        }

        public async Task<ResponseObject<DataResponseTeam>> CreateTeam(Request_Team request)
        {

            try
            {
                if (await _teamRepository.GetTeamByName(request.Name) != null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status409Conflict,
                        Message = "Team này đã có trong hệ thống",
                        Data = null,
                    };
                }
                var user = await _baseUserRepository.GetByIdAsync(request.ManagerId);
                if (user == null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status409Conflict,
                        Message = "Người này chưa có trong hệ thống",
                        Data = null,
                    };
                }
                var role = await _userRepository.GetRolesOfUserAsync(user);
                if (!role.Contains("Manager"))
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người này không có quyền",
                        Data = null
                    };
                }
                var checkTeam = await _teamRepository.GetTeamByManager(user.Id);
                if (checkTeam != null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người này đang là trưởng phòng của phòng ban khác",
                        Data = null
                    };
                }
                var team = new Team
                {
                    Name = request.Name,
                    Description = request.Description,
                    NumberOfMember = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = null,
                    ManagerId = request.ManagerId,
                };

                await _baseTeamRepository.CreateAsync(team);
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Tạo team thành công",
                    Data = _teamConverter.EntitytoDTO(team),
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null,
                };
            }
        }

        public async Task<ResponseObject<bool>> DeleteTeam(int id)
        {
            try
            {
                if (await _baseTeamRepository.GetByIdAsync(id) == null)
                {
                    return new ResponseObject<bool>
                    {
                        Status = StatusCodes.Status409Conflict,
                        Message = "Id Không tồn tại trong hệ thống",
                        Data = false,
                    };
                }
                await _baseTeamRepository.DeleteAsync(id);
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Xóa team thành công",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = false
                };
            }

        }

        public async Task<ResponsePagedResult<DataResponseTeam>> GetAllTeams(string? teamName, int pageNumber, int pageSize)
        {
            try
            {
                var teams = await _teamRepository.GetAllTeamsAsync(teamName);
                var responseTeam = teams.Select(t => _teamConverter.EntitytoDTO(t)).ToPagedList(pageNumber, pageSize);
                return new ResponsePagedResult<DataResponseTeam>
                {
                    Items = responseTeam.ToList(),
                    TotalPages = responseTeam.PageCount,
                    TotalItems = responseTeam.TotalItemCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                return new ResponsePagedResult<DataResponseTeam>
                {
                    Items = null,
                    TotalPages = 0,
                    TotalItems = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }
        public async Task<ResponseObject<IEnumerable<ResponseDepartments>>> getDepartments()
        {
            try
            {
                var teams = await _baseTeamRepository.GetAllAsync();
                var responseTeam = teams.Select(t => new ResponseDepartments
                {
                    Id = t.Id,
                    Name = t.Name,
                }).ToList();
                return new ResponseObject<IEnumerable<ResponseDepartments>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "lấy thành công",
                    Data = responseTeam,
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<IEnumerable<ResponseDepartments>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null
                };
            }
        }
        public async Task<ResponseObject<DataResponseTeam>> GetTeamById(int id)
        {
            try
            {
                var team = await _baseTeamRepository.GetByIdAsync(id);
                if (team == null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status409Conflict,
                        Message = "Team Không tồn tại trong hệ thống",
                        Data = null,
                    };
                }
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Lấy thành công thông tin team theo Id",
                    Data = _teamConverter.EntitytoDTO(team),
                };

            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseTeam>> UpdateTeam(int id, Request_Team request)
        {
            try
            {
                var team = await _baseTeamRepository.GetByIdAsync(id);
                if (team == null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status409Conflict,
                        Message = "Id Không tồn tại trong hệ thống",
                        Data = null,
                    };
                }
                var checkName = await _teamRepository.GetTeamByName(request.Name);
                if (checkName != null && team.Name != request.Name)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status409Conflict,
                        Message = "Team này đã có trong hệ thống",
                        Data = null,
                    };
                }
                var checkTeam = _teamRepository.GetTeamByManager(request.ManagerId);
                if (checkTeam != null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người này đang là trưởng phòng của phòng ban khác",
                        Data = null

                    };
                }
                var numberOfmember = await _teamRepository.GetNumberOfMembersInTeamAsync(team.Id);
                team.Name = request.Name;
                team.Description = request.Description;
                team.NumberOfMember = numberOfmember;
                team.UpdateTime = DateTime.Now;
                team.ManagerId = request.ManagerId;
                await _baseTeamRepository.UpdateAsync(team);
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Cập nhật team thành công",
                    Data = _teamConverter.EntitytoDTO(team),
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseTeam>> UpdateTeamManager(int id, int userId)
        {
            try
            {
                var team = await _baseTeamRepository.GetByIdAsync(id);
                var user = await _baseUserRepository.GetByIdAsync(userId);
                if (team == null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Team không tồn tại trong hệ thống",
                        Data = null
                    };
                }

                if (user == null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "User không tồn tại trong hệ thống",
                        Data = null
                    };
                }

                var role = await _userRepository.GetRolesOfUserAsync(user);
                if (!role.Contains("Manager"))
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người này không có quyền",
                        Data = null
                    };
                }
                var teamManager = _teamRepository.GetTeamByManager(user.Id);
                if (teamManager != null)
                {
                    return new ResponseObject<DataResponseTeam>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người này đang là trưởng phòng của phòng ban khác",
                        Data = null
                    };
                }


                var oldTeamId = user?.TeamId;

                //update team mới
                team.ManagerId = user.Id;
                team.NumberOfMember = await _teamRepository.GetNumberOfMembersInTeamAsync(team.Id);
                team.UpdateTime = DateTime.Now;
                await _baseTeamRepository.UpdateAsync(team);

                //update teamId của user
                user.TeamId = team.Id;
                user.UpdateTime = DateTime.Now;
                await _baseUserRepository.UpdateAsync(user);

                //update team cũ
                if (oldTeamId != null)
                {
                    // Nếu TeamId không null thì thực hiện cập nhật số lượng thành viên của team cũ
                    var oldTeam = await _baseTeamRepository.GetByIdAsync(oldTeamId.Value);
                    if (oldTeam != null)
                    {
                        oldTeam.NumberOfMember = await _teamRepository.GetNumberOfMembersInTeamAsync(oldTeam.Id);
                        oldTeam.UpdateTime = DateTime.Now;
                        await _baseTeamRepository.UpdateAsync(oldTeam);
                    }
                }
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Cập nhật Trưởng phòng thành công",
                    Data = new DataResponseTeam
                    {
                        Id = team.Id,
                        ManagerId = team.ManagerId
                    }
                };



            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseTeam>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null,
                };
            }
        }
    }
}
