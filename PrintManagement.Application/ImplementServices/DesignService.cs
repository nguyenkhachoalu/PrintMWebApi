using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Utilities.IO;
using PrintManagement.Application.Handle.HandleFile;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.Mappers;
using PrintManagement.Application.Payloads.RequestModels.DesignRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataDesign;
using PrintManagement.Application.Payloads.ResponseModels.DataTeams;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.ImplementServices
{
    public class DesignService : IDesignService
    {
        private readonly IBaseRepository<Design> _baseDesignRepository;
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IBaseRepository<Project> _baseProjectRepository;
        private readonly IBaseRepository<PrintJobs> _basePrintJobRepository;
        private readonly IBaseRepository<Notification> _baseNotificationRepository;
        private readonly IDesignRepository _designRepository;
        private readonly IUserRepository _userRepository;
        private readonly DesignConveter _designConveter;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DesignService(IBaseRepository<Design> baseDesignRepository, IBaseRepository<User> baseUserRepository, IBaseRepository<Project> baseProjectRepository, IBaseRepository<PrintJobs> basePrintJobRepository, IBaseRepository<Notification> baseNotificationRepository, IDesignRepository designRepository, IUserRepository userRepository, DesignConveter designConveter, IHttpContextAccessor httpContextAccessor)
        {
            _baseDesignRepository = baseDesignRepository;
            _baseUserRepository = baseUserRepository;
            _baseProjectRepository = baseProjectRepository;
            _basePrintJobRepository = basePrintJobRepository;
            _baseNotificationRepository = baseNotificationRepository;
            _designRepository = designRepository;
            _userRepository = userRepository;
            _designConveter = designConveter;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseObject<DataResponseDesign>> GetDesignById(int Id)
        {
            var design = await _baseDesignRepository.GetByIdAsync(Id);
            if (design == null)
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Không có thiết kế nào",
                    Data = null,

                };
            }
            return new ResponseObject<DataResponseDesign>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy bản thiết kế thành công",
                Data = _designConveter.EntitytoDTO(design),

            };
        }
        public async Task<ResponseObject<DataResponseDesign>> ApprovedDesign(int approveId, Request_Approve request)
        {
            var design = await _baseDesignRepository.GetByIdAsync(request.DesignId);
            if (design == null)
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Không có thiết kế nào",
                    Data = null,

                };
            }
            var leader = await _userRepository.IsUserInDesignTeamAsync(approveId);
            if (!leader)
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Bạn không phải thuộc phòng ban thiết kế",
                    Data = null
                };
            }
            var user = await _baseUserRepository.GetByIdAsync(approveId);
            var role = await _userRepository.GetRolesOfUserAsync(user);
            if (!role.Contains("Leader"))
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Bạn không có quyền Leader",
                    Data = null
                };
            }
            if (request.DesignStatus == DeStatus.Awaiting)
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "nếu bạn chờ thì không cần chọn nữa",
                    Data = null,

                };
            }
            var project = await _baseProjectRepository.GetByIdAsync(design.ProjectId);

            project.ProjectStatus = PStatus.ResourcePreparation;
            design.DesignStatus = request.DesignStatus;

            await _baseDesignRepository.UpdateAsync(design);
            
            var message = "";
            if (design.DesignStatus == DeStatus.Refuse)
            {
                
                message = $"Thiết kế của dự án {project.ProjectName} đã bị từ chối";
            }
            else
            {
                var printJob = new PrintJobs
                {
                    DesignId = design.Id,
                    PrintJobStatus = PjStatus.Pending
                };
                await _baseProjectRepository.UpdateAsync(project);
                await _basePrintJobRepository.CreateAsync(printJob);
                message = $"Thiết kế của dự án {project.ProjectName} đã được duyệt";
            }
            

            var notification = new Notification
            {
                UserId = design.DesignerId,
                Context = message,
                Link = request.Link,
                CreateTime = DateTime.Now,
                IsSeen = false,
            };
            await _baseNotificationRepository.CreateAsync(notification);
            return new ResponseObject<DataResponseDesign>
            {
                Status = StatusCodes.Status200OK,
                Message = "Thiết kế đã được xử lý thành công",
                Data = _designConveter.EntitytoDTO(design),

            };
        }


        public async Task<ResponseObject<IEnumerable<DataResponseDesign>>> GetDesignByProjectAsyc(DeStatus? deStatus, int projectId)
        {
            var designs = await _designRepository.GetDesignWithStatusByProjectId(deStatus,projectId);
            var hostUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            if (designs == null)
            {
                return new ResponseObject<IEnumerable<DataResponseDesign>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Không có thiết kế nào cho project này",
                    Data = null,

                };
            }
            var response = designs.Select(t => new DataResponseDesign
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                DesignerId = t.DesignerId,
                FilePath = $"{hostUrl}/filePaths/designs/{t.FilePath}",
                DesignTime = t.DesignTime,
                DesignStatus = t.DesignStatus,
                ApproverId = t.ApproverId,
            });
            return new ResponseObject<IEnumerable<DataResponseDesign>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy ra các thiết kế thành công",
                Data = response,

            };
        }

        public async Task<ResponseObject<DataResponseDesign>> UploadFileDesig(int userId, Request_CreateDesign request)
        {
            var project = await _baseProjectRepository.GetByIdAsync(request.ProjectId);
            
            if (project == null)
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Không có project này",
                    Data = null
                };

            }

            if (project.ProjectStatus != PStatus.Designing)
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Dự án đã hoàn thành hoặc đang trong quá trình in",
                    Data = null
                };
            }
            var designer = await _userRepository.IsUserInDesignTeamAsync(userId);
            if (!designer)
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Bạn không phải thuộc phòng ban thiết kế",
                    Data = null
                };
            }
            var user = await _baseUserRepository.GetByIdAsync(userId);
            var role = await _userRepository.GetRolesOfUserAsync(user);
            if (!role.Contains("Designer"))
            {
                return new ResponseObject<DataResponseDesign>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Bạn không có quyền Thiết kế",
                    Data = null
                };
            }
            var design = new Design
            {
                ProjectId = project.Id,
                DesignerId = userId,
                FilePath = await HandleUploadFile.WirteFileDesign(request.FilePath),
                DesignTime = DateTime.Now,
                DesignStatus = DeStatus.Awaiting,
                ApproverId = null,
            };
            await _baseDesignRepository.CreateAsync(design);
            var response = _designConveter.EntitytoDTO(design);
            return new ResponseObject<DataResponseDesign>
            {
                Status = StatusCodes.Status201Created,
                Message = "Gửi tệp tin thành công",
                Data = response
            };

        }


    }
}