using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Utilities.IO;
using PrintManagement.Application.Handle.HandleFile;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.Mappers;
using PrintManagement.Application.Payloads.RequestModels.UserRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataNotification;
using PrintManagement.Application.Payloads.ResponseModels.DataRole;
using PrintManagement.Application.Payloads.ResponseModels.DataTeams;
using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PrintManagement.Application.ImplementServices
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IBaseRepository<Team> _baseTeamRepository;
        private readonly IBaseRepository<Notification> _baseNotificationRepository;
        private readonly IBaseRepository<Permissions> _basePermissionRepository;
        private readonly IBaseRepository<Role> _baseRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly UserConverter _userConverter;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IBaseRepository<User> baseUserRepository, IBaseRepository<Team> baseTeamRepository, IBaseRepository<Notification> baseNotificationRepository, IBaseRepository<Permissions> basePermissionRepository, IBaseRepository<Role> baseRoleRepository, IUserRepository userRepository, ITeamRepository teamRepository, UserConverter userConverter, IHttpContextAccessor httpContextAccessor)
        {
            _baseUserRepository = baseUserRepository;
            _baseTeamRepository = baseTeamRepository;
            _baseNotificationRepository = baseNotificationRepository;
            _basePermissionRepository = basePermissionRepository;
            _baseRoleRepository = baseRoleRepository;
            _userRepository = userRepository;
            _teamRepository = teamRepository;
            _userConverter = userConverter;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponsePagedResult<DataResponseUser>> GetAllUsers(int pageNumber, int pageSize)
        {
            try
            {
                var users = await _baseUserRepository.GetAllAsync();
                var hostUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

                // Thay đổi quá trình mapping để thêm URL đầy đủ cho avatar
                var responseUser = users.Select(t => new DataResponseUser
                {
                    Id = t.Id,
                    UserName = t.UserName,
                    FullName = t.FullName,
                    DateOfBirth = t.DateOfBirth,
                    Avatar = $"{hostUrl}/images/avatars/{t.Avatar}",
                    Email = t.Email,
                    CreateTime = t.CreateTime,
                    UpdateTime = t.UpdateTime,
                    PhoneNumber = t.PhoneNumber,
                    TeamId = t.TeamId,
                    IsActive = t.IsActive,
                }).ToPagedList(pageNumber, pageSize);

                return new ResponsePagedResult<DataResponseUser>
                {
                    Items = responseUser.ToList(),
                    TotalPages = responseUser.PageCount,
                    TotalItems = responseUser.TotalItemCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                return new ResponsePagedResult<DataResponseUser>
                {
                    Items = null,
                    TotalPages = 0,
                    TotalItems = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }

        public async Task<ResponseObject<DataResponseUser>> GetUserById(int id)
        {
            try
            {
                var user = await _baseUserRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status409Conflict,
                        Message = "Id Không tồn tại trong hệ thống",
                        Data = null,
                    };
                }
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Lấy thành công thông tin user theo Id",
                    Data = _userConverter.EntitytoDTO(user),
                };

            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<IEnumerable<DataResponseUser>>> GetUsersWithRoleManagers()
        {
            var users = await _userRepository.GetUsersWithRoleManagerAsync();
            var responseUsers = users.Select(user => _userConverter.EntitytoDTO(user)).ToList(); ;
            return new ResponseObject<IEnumerable<DataResponseUser>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy thông tin thành công",
                Data = responseUsers,
            };
        }

        public async Task<ResponseObject<DataResponseUser>> MyProfileGetById(int id)
        {
            var hostUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var user = await _baseUserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Người dùng không tồn tại",
                    Data = null,
                };
            }
            var response = new DataResponseUser
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                Avatar = $"{hostUrl}/images/avatars/{user.Avatar}",
                Email = user.Email,
                CreateTime = user.CreateTime,
                UpdateTime = user.UpdateTime,
                PhoneNumber = user.PhoneNumber,
                TeamId = user.TeamId,
                IsActive = user.IsActive,
            };
            return new ResponseObject<DataResponseUser>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy thông tin cá nhân thành công",
                Data = response,
            };
        }

        public Task<ResponseObject<DataResponseUser>> UpdateProfile(int id, Request_UpdateUser request)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseObject<DataResponseUser>> UpdateTeamUser(int id, int teamId)
        {
            try
            {
                var user = await _baseUserRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người dùng không tồn tại",
                        Data = null,
                    };
                }
                var team = await _baseTeamRepository.GetByIdAsync(teamId);
                if (team == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Team không tồn tại",
                        Data = null,
                    };
                }
                user.TeamId = teamId;
                team.NumberOfMember = await _teamRepository.GetNumberOfMembersInTeamAsync(teamId);
                await _baseUserRepository.UpdateAsync(user);
                await _baseTeamRepository.UpdateAsync(team);
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Thay đổi team cho người dùng thành công",
                    Data = _userConverter.EntitytoDTO(user),
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<DataResponseUser>> UpdateUser(int id, Request_UpdateUser request)
        {
            try
            {
                var user = await _baseUserRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return new ResponseObject<DataResponseUser>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người dùng không tồn tại",
                        Data = null,
                    };
                }
                user.FullName = request.FullName;
                user.Avatar = await HandleUploadFile.WirteFile(request.Avatar);
                user.Email = request.Email;
                user.PhoneNumber = request.PhoneNumber;
                user.DateOfBirth = request.DateOfBirth;
                await _baseUserRepository.UpdateAsync(user);
                var response = _userConverter.EntitytoDTO(user);
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Đổi thông tin thành công",
                    Data = response,
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseUser>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex,
                    Data = null,
                };
            }
        }
        public async Task<ResponseObject<IEnumerable<DataResponseNotification>>> GetNotificationByUserId(int userId)
        {
            var notifications = await _userRepository.GetNotificationByUserId(userId);
            var response = notifications.Select(x => new DataResponseNotification
            {
                Id = x.Id,
                UserId = x.UserId,
                Context = x.Context,
                Link = x.Link,
                CreateTime = x.CreateTime,
                IsSeen = x.IsSeen,
            }).ToList();
            return new ResponseObject<IEnumerable<DataResponseNotification>>
            {
                Status = StatusCodes.Status200OK,
                Message = "lấy thành công thông báo",
                Data = response,
            };
        }
        public async Task<ResponseObject<string>> UpdateIsSeenNotification(int id)
        {
            var notification = await _baseNotificationRepository.GetByIdAsync(id);
            if (notification == null)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Không tìm thấy id này",
                    Data = null,
                };
            }
            if (notification.IsSeen == true)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "thông báo đã được xem trước đây",
                    Data = null,
                };
            }
            notification.IsSeen = true;
            await _baseNotificationRepository.UpdateAsync(notification);
            return new ResponseObject<string>
            {
                Status = StatusCodes.Status200OK,
                Message = "Người dùng đã xem thông báo",
                Data = null,
            };
        }

        public async Task<ResponseObject<string>> UpdateRoleOfUser(int id, List<string> roleCode)
        {
            try
            {
                var user = await _baseUserRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người dùng không tồn tại",
                        Data = null,
                    };
                }
                var permissions = await _userRepository.GetPermissionsByUserId(user.Id);

                if (permissions != null && permissions.Any())
                {
                    await _basePermissionRepository.DeleteAsync(p => permissions.Select(x => x.Id).Contains(p.Id));
                }
                await _userRepository.AddRolesToUserAsync(user, roleCode);
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Thêm quyền thành công",
                    Data = null,
                };
            }

            catch (Exception ex)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Lỗi: " + ex.Message,
                    Data = null,
                };

            }
        }
        public async Task<ResponseObject<IEnumerable<DataResponseRole>>> GetAllRole()
        {
            var roles = await _baseRoleRepository.GetAllAsync();
            var response = roles.Select(x => new DataResponseRole
            {
                Id = x.Id,
                RoleCode = x.RoleCode,
                RoleName = x.RoleName,
                IsActive = x.IsActive,
            });
            return new ResponseObject<IEnumerable<DataResponseRole>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy danh sách thành công",
                Data = response,
            };
        }
    }
}
