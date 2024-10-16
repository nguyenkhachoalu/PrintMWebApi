using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Org.BouncyCastle.Asn1.Ocsp;
using PrintManagement.Application.Handle.HandleEmail;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.Mappers;
using PrintManagement.Application.Payloads.RequestModels.DeliveryRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataDelivery;
using PrintManagement.Application.Payloads.ResponseModels.DataProject;
using PrintManagement.Application.Payloads.ResponseModels.DataShippingMethod;
using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace PrintManagement.Application.ImplementServices
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IBaseRepository<Delivery> _baseDeliveryRepository;
        private readonly IBaseRepository<Project> _baseProjectRepository;
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IBaseRepository<Team> _baseTeamRepository;
        private readonly IBaseRepository<Customer> _baseCustomerRepository;
        private readonly IBaseRepository<ShippingMethod> _baseShippingMethodRepository;
        private readonly IBaseRepository<Notification> _baseNotificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITeamRepository _teamtRepository;
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IEmailService _emailService;
        private readonly DeliveryConveter _deliveryConveter;
        private readonly ProjectConverter _projectConveter;
        private readonly UserConverter _userConveter;

        public DeliveryService(IBaseRepository<Delivery> baseDeliveryRepository, IBaseRepository<Project> baseProjectRepository, IBaseRepository<User> baseUserRepository, IBaseRepository<Team> baseTeamRepository, IBaseRepository<Customer> baseCustomerRepository, IBaseRepository<ShippingMethod> baseShippingMethodRepository, IBaseRepository<Notification> baseNotificationRepository, IUserRepository userRepository, ITeamRepository teamtRepository, IDeliveryRepository deliveryRepository, IEmailService emailService, DeliveryConveter deliveryConveter, ProjectConverter projectConveter, UserConverter userConveter)
        {
            _baseDeliveryRepository = baseDeliveryRepository;
            _baseProjectRepository = baseProjectRepository;
            _baseUserRepository = baseUserRepository;
            _baseTeamRepository = baseTeamRepository;
            _baseCustomerRepository = baseCustomerRepository;
            _baseShippingMethodRepository = baseShippingMethodRepository;
            _baseNotificationRepository = baseNotificationRepository;
            _userRepository = userRepository;
            _teamtRepository = teamtRepository;
            _deliveryRepository = deliveryRepository;
            _emailService = emailService;
            _deliveryConveter = deliveryConveter;
            _projectConveter = projectConveter;
            _userConveter = userConveter;
        }

        public async Task<ResponseObject<DataResponseDelivery>> CreateDelivery(int creatorId, Request_CreateDelivery request)
        {
            try
            {
                #region valid
                var creator = await _baseUserRepository.GetByIdAsync(creatorId);  
                var team = await _baseTeamRepository.GetByIdAsync((int)creator.TeamId);
                if (team.Name.ToLower() != "delivery")
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Phòng ban của bạn không có quyền sử lý tác vụ này",
                        Data = null,
                    };
                }
                var roleCreator = await _userRepository.GetRolesOfUserAsync(creator);
                if (!roleCreator.Contains("Manager"))
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Bạn không có quyền sử lý tác vụ này",
                        Data = null,
                    };
                }
                var project = await _baseProjectRepository.GetByIdAsync(request.ProjectId);
                if (project == null)
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Project không tồn tại",
                        Data = null,
                    };
                }
                if (project.ProjectStatus != PStatus.Completed)
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Chưa sẵn sàng để giao hàng",
                        Data = null,
                    };
                }
                var customer = await _baseCustomerRepository.GetByIdAsync(project.CustomerId);
                if (customer == null)
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Có thể khách hàng đã bị xóa khỏi hệ thống",
                        Data = null,
                    };
                }
                var shipper = await _baseUserRepository.GetByIdAsync(request.DeliverId);
                if (shipper == null)
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người giao hàng không tồn tại",
                        Data = null,
                    };
                }
                var teamShipper = await _baseTeamRepository.GetByIdAsync((int)shipper.TeamId);
                if (team.Name.ToLower() != "delivery")
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người này không thuộc phòng ban giao hàng",
                        Data = null,
                    };
                }
                #endregion
                var delivery = new Delivery
                {
                    ShippingMethodId = request.ShippingMethodId,
                    CustomerId = customer.Id,
                    DeliverId = shipper.Id,
                    ProjectId = project.Id,
                    DeliveryAddress = customer.Address,
                    EstimateDeliveryTime = DateTime.Now,
                    ActualDeliveryTime = DateTime.Now.AddDays(7),
                    DeliveryStatus = DStatus.Processing
                };
                await _baseDeliveryRepository.CreateAsync(delivery);
                var response = _deliveryConveter.EntitytoDTO(delivery);
                return new ResponseObject<DataResponseDelivery>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Đã xác nhận chờ xử lý đơn hàng",
                    Data = response,
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseDelivery>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: "+ex.Message,
                    Data = null,
                };
            }
        }

        public async Task<ResponsePagedResult<DataResponseDeliveryFull>> GetDeliveryByStatus(DStatus? dStatus, int pageNumber, int pageSize)
        {
            try
            {
                // Gọi repository để lấy dữ liệu dựa trên dStatus (có hoặc không có lọc)
                var deliveries = await _deliveryRepository.GetAllDeliveryByStatus(dStatus);

                // Thực hiện mapping từ entity sang DTO, thêm URL đầy đủ cho các hình ảnh nếu cần
                var responseDeliveries = deliveries.Select(d => new DataResponseDeliveryFull
                {
                    Id = d.Id,
                    ShippingMethodId = d.ShippingMethodId,
                    CustomerId = d.CustomerId,
                    DeliverId = d.DeliverId,
                    ProjectId = d.ProjectId,
                    DeliveryAddress = d.DeliveryAddress,
                    EstimateDeliveryTime = d.EstimateDeliveryTime,
                    ActualDeliveryTime = d.ActualDeliveryTime,
                    DeliveryStatus = d.DeliveryStatus,
                    ShippingMethodName = d.ShippingMethodName,
                    ProjectName = d.ProjectName,
                    CustomeName = d.CustomeName,
                    DeliverName = d.DeliverName
                }).ToPagedList(pageNumber, pageSize); // Phân trang với ToPagedList

                // Trả về kết quả phân trang
                return new ResponsePagedResult<DataResponseDeliveryFull>
                {
                    Items = responseDeliveries.ToList(),
                    TotalPages = responseDeliveries.PageCount,
                    TotalItems = responseDeliveries.TotalItemCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return new ResponsePagedResult<DataResponseDeliveryFull>
                {
                    Items = null,
                    TotalPages = 0,
                    TotalItems = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }


        public async Task<ResponseObject<DataResponseDelivery>> UpdateDelivery(int creatorId, int id, Request_CreateDelivery request)
        {
            try
            {
                #region valid
                var delivery = await _baseDeliveryRepository.GetByIdAsync(id);
                if(delivery == null)
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Đơn vận chuyển không tồn tại",
                        Data = null,
                    };
                }
                var creator = await _baseUserRepository.GetByIdAsync(creatorId);
                var team = await _baseTeamRepository.GetByIdAsync((int)creator.TeamId);
                if (team.Name.ToLower() != "delivery")
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Phòng ban của bạn không có quyền sử lý tác vụ này",
                        Data = null,
                    };
                }
                var roleCreator = await _userRepository.GetRolesOfUserAsync(creator);
                if (!roleCreator.Contains("Manager"))
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Bạn không có quyền sử lý tác vụ này",
                        Data = null,
                    };
                }
                var project = await _baseProjectRepository.GetByIdAsync(request.ProjectId);
                if (project == null)
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Project không tồn tại",
                        Data = null,
                    };
                }
                if (project.ProjectStatus != PStatus.Completed)
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Chưa sẵn sàng để giao hàng",
                        Data = null,
                    };
                }
                var customer = await _baseCustomerRepository.GetByIdAsync(project.CustomerId);
                if (customer == null)
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Có thể khách hàng đã bị xóa khỏi hệ thống",
                        Data = null,
                    };
                }
                var shipper = await _baseUserRepository.GetByIdAsync(request.DeliverId);
                if (shipper == null)
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người giao hàng không tồn tại",
                        Data = null,
                    };
                }
                var teamShipper = await _baseTeamRepository.GetByIdAsync((int)shipper.TeamId);
                if (team.Name.ToLower() != "delivery")
                {
                    return new ResponseObject<DataResponseDelivery>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người này không thuộc phòng ban giao hàng",
                        Data = null,
                    };
                }
                #endregion

                delivery.ShippingMethodId = request.ShippingMethodId;
                delivery.CustomerId = customer.Id;
                delivery.DeliverId = shipper.Id;
                delivery.ProjectId = project.Id;
                delivery.DeliveryAddress = customer.Address;
                delivery.DeliveryStatus = DStatus.Processing;
                await _baseDeliveryRepository.UpdateAsync(delivery);
                var response = _deliveryConveter.EntitytoDTO(delivery);
                return new ResponseObject<DataResponseDelivery>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Sửa thông tin đơn hàng thành công",
                    Data = response,
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseDelivery>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null,
                };
            }
        }

        public async Task<ResponseObject<string>> UpdateDeliveryStatusById(int id,int shipperId, DStatus dStatus)
        {
            try
            {
                #region valid
                var delivery = await _baseDeliveryRepository.GetByIdAsync(id);
                if (delivery == null)
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Đơn vận chuyển không tồn tại",
                        Data = null,
                    };
                }
                if (delivery.DeliveryStatus == dStatus)
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Trạng thái này đã cập nhật trước đây",
                        Data = null,
                    };
                }
                var shipper = await _baseUserRepository.GetByIdAsync(shipperId);
                if (shipper == null)
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người giao hàng không tồn tại",
                        Data = null,
                    };
                }
                var teamShipper = await _baseTeamRepository.GetByIdAsync((int)shipper.TeamId);
                if (teamShipper.Name.ToLower() != "delivery")
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người này không thuộc phòng ban giao hàng",
                        Data = null,
                    };
                }
               
                var roleCreator = await _userRepository.GetRolesOfUserAsync(shipper);
                if (!roleCreator.Contains("Employee"))
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Bạn không có quyền sử lý tác vụ này",
                        Data = null,
                    };
                }
                if(dStatus == DStatus.Delivered)
                {
                    var project = await _baseProjectRepository.GetByIdAsync(delivery.ProjectId);
                    var customer = await _baseCustomerRepository.GetByIdAsync(delivery.CustomerId);
                    var teamDelivery = await _teamtRepository.GetTeamByName("delivery");
                    var notifycationManagerDelivery = new Notification
                    {
                        UserId = teamDelivery.ManagerId,
                        Context = $"Dự án {project.ProjectName} hoàn tất giao hàng",
                        Link = id.ToString(),
                        CreateTime = DateTime.Now,
                        IsSeen = false,
                    };
                    await _baseNotificationRepository.CreateAsync(notifycationManagerDelivery);

                    var emailContent = _emailService.GenerateDeliveryCompletionEmail(project.ProjectName);
                    var message = new EmailMessage(new string[] { customer.Email }, $"Giao hàng thành công dự án của {customer.FullName}", emailContent);
                    var responseMessage = _emailService.SendEmail(message);

                }    
                #endregion

                delivery.DeliveryStatus = dStatus;  
                await _baseDeliveryRepository.UpdateAsync(delivery);
                var response = _deliveryConveter.EntitytoDTO(delivery);
                
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Cập nhật trạng thái đơn hàng thành công",
                    Data = null,
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null,
                };
            }
        }
        public async Task<ResponseObject<string>> ConfirmSuccessfulDelivery(int id, int shipperId)
        {
            try
            {
                #region valid
                var delivery = await _baseDeliveryRepository.GetByIdAsync(id);
                if (delivery == null)
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Đơn vận chuyển không tồn tại",
                        Data = null,
                    };
                }
                if (delivery.DeliveryStatus == DStatus.Delivered)
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Trạng thái này đã cập nhật trước đây",
                        Data = null,
                    };
                }
                var shipper = await _baseUserRepository.GetByIdAsync(shipperId);
                if (shipper == null)
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người giao hàng không tồn tại",
                        Data = null,
                    };
                }
                var teamShipper = await _baseTeamRepository.GetByIdAsync((int)shipper.TeamId);
                if (teamShipper.Name.ToLower() != "delivery")
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Người này không thuộc phòng ban giao hàng",
                        Data = null,
                    };
                }
                
                var roleCreator = await _userRepository.GetRolesOfUserAsync(shipper);
                if (!roleCreator.Contains("Employee"))
                {
                    return new ResponseObject<string>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Bạn không có quyền sử lý tác vụ này",
                        Data = null,
                    };
                }

                #endregion

                delivery.DeliveryStatus = DStatus.Delivered;

                await _baseDeliveryRepository.UpdateAsync(delivery);
                var project = await _baseProjectRepository.GetByIdAsync(delivery.ProjectId);
                var customer = await _baseCustomerRepository.GetByIdAsync(delivery.CustomerId);
                var teamDelivery = await _teamtRepository.GetTeamByName("Delivery");

                //tạo thông báo
                var notifycationManagerDelivery = new Notification
                {
                    UserId = teamDelivery.ManagerId,
                    Context = $"Dự án {project.ProjectName} hoàn tất giao hàng",
                    Link = id.ToString(),
                    CreateTime = DateTime.Now,
                    IsSeen = false,
                };
                await _baseNotificationRepository.CreateAsync(notifycationManagerDelivery);

                //gửi email tới khách hàng
                var emailContent = _emailService.GenerateDeliveryCompletionEmail(project.ProjectName);
                var message = new EmailMessage(new string[] { customer.Email }, $"Giao hàng thành công dự án  của {customer.FullName}", emailContent);
                var responseMessage = _emailService.SendEmail(message);

                //mapper ghi note vào cho vui
                var response = _deliveryConveter.EntitytoDTO(delivery);
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Cập nhật trạng thái đơn hàng thành công",
                    Data = teamDelivery.ManagerId.ToString(),
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null,
                };
            }
        }
        public async Task<int?> GetShipperIdByDeliveryAddressAsync(string deliveryAddress)
        {
            return await _deliveryRepository.GetShipperIdByDeliveryAddressAsync(deliveryAddress);
            
        }
        public async Task<ResponseObject<IEnumerable<DataResponseProject>>> GetCompletedProjectsNotInDeliveryAsync()
        {
            var projects = await _deliveryRepository.GetCompletedProjectsNotInDeliveryAsync();
            if(projects == null)
            {
                return new ResponseObject<IEnumerable<DataResponseProject>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Đơn vận chuyển không tồn tại",
                    Data = null,
                };
            }
            var response = projects.Select(x => _projectConveter.EntitytoDTO(x)).ToList();
            return new ResponseObject<IEnumerable<DataResponseProject>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Danh sách dự án hoàn thành mà chưa được giao hàng",
                Data = response,
            };

        }

        public async Task<ResponseObject<IEnumerable<DataResponseUser>>> GetEmployeesInDeliveryTeamAsync()
        {
            var users = await _deliveryRepository.GetEmployeesInDeliveryTeamAsync();
            if (users == null)
            {
                return new ResponseObject<IEnumerable<DataResponseUser>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "không có nhân viên nào tại phòng ban giao hàng",
                    Data = null,
                };
            }
            var response = users.Select(x => _userConveter.EntitytoDTO(x)).ToList();
            return new ResponseObject<IEnumerable<DataResponseUser>>
            {
                Status = StatusCodes.Status200OK,
                Message = "lấy danh sách nhân viên phòng giao hàng thành công",
                Data = response,
            };
        }

        public async Task<ResponseObject<IEnumerable<DataResponseShippingMethod>>> GetAllShippingMethod()
        {
            var shippingMethods = await _baseShippingMethodRepository.GetAllAsync();
            var response = shippingMethods.Select(x => new DataResponseShippingMethod
            {
                Id = x.Id,
                ShippingMethodName = x.ShippingMethodName,
            }).ToList();
            return new ResponseObject<IEnumerable<DataResponseShippingMethod>>
            {
                Status = StatusCodes.Status200OK,
                Message = "lấy danh sách phương thức giao hàng thành công",
                Data = response,
            };
        }

        public async Task<ResponseObject<DataResponseShippingMethod>> GetByIdShippingMethod(int id)
        {
            var shippingMethod = await _baseShippingMethodRepository.GetByIdAsync(id);
            var response = new DataResponseShippingMethod
            {
                Id = shippingMethod.Id,
                ShippingMethodName = shippingMethod.ShippingMethodName,
            };
            return new ResponseObject<DataResponseShippingMethod>
            {
                Status = StatusCodes.Status200OK,
                Message = "lấy danh sách phương thức giao hàng thành công",
                Data = response,
            };
        }

        public async Task<ResponseObject<string>> GetAddressCustomerByProjectId(int projectid)
        {
            var project = await _baseProjectRepository.GetByIdAsync(projectid);
            if(project == null)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Project Không tồn tại",
                    Data = null,
                };
            }
            var customer = await _baseCustomerRepository.GetByIdAsync(project.CustomerId);
            if (customer == null)
            {
                return new ResponseObject<string>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Customer Không tồn tại",
                    Data = null,
                };
            }
            return new ResponseObject<string>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy thành công",
                Data = customer.Address,
            };
        }

        public async Task<ResponsePagedResult<DataResponseDeliveryFull>> GetDeliveryByShipperIdStatus(int shipperId, DStatus? dStatus, int pageNumber, int pageSize)
        {
            try
            {
                // Gọi repository để lấy dữ liệu dựa trên dStatus (có hoặc không có lọc)
                var deliveries = await _deliveryRepository.GetAllDeliveryByDeliverIdAndStatus(shipperId, dStatus);
                
                // Thực hiện mapping từ entity sang DTO, thêm URL đầy đủ cho các hình ảnh nếu cần
                var responseDeliveries = deliveries.Select(d => new DataResponseDeliveryFull
                {
                    Id = d.Id,
                    ShippingMethodId = d.ShippingMethodId,
                    CustomerId = d.CustomerId,
                    DeliverId = d.DeliverId,
                    ProjectId = d.ProjectId,
                    DeliveryAddress = d.DeliveryAddress,
                    EstimateDeliveryTime = d.EstimateDeliveryTime,
                    ActualDeliveryTime = d.ActualDeliveryTime,
                    DeliveryStatus = d.DeliveryStatus,
                    ShippingMethodName = d.ShippingMethodName,
                    ProjectName = d.ProjectName,
                    CustomeName = d.CustomeName,
                    DeliverName = d.DeliverName

                }).ToPagedList(pageNumber, pageSize); // Phân trang với ToPagedList

                // Trả về kết quả phân trang
                return new ResponsePagedResult<DataResponseDeliveryFull>
                {
                    Items = responseDeliveries.ToList(),
                    TotalPages = responseDeliveries.PageCount,
                    TotalItems = responseDeliveries.TotalItemCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return new ResponsePagedResult<DataResponseDeliveryFull>
                {
                    Items = null,
                    TotalPages = 0,
                    TotalItems = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }
    }
}
