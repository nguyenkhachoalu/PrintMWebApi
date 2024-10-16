using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using PrintManagement.Application.Handle.HandleEmail;
using PrintManagement.Application.Handle.HandleFile;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.Mappers;
using PrintManagement.Application.Payloads.RequestModels.ResourceForPrintJobRequest;
using PrintManagement.Application.Payloads.RequestModels.ResourcePropertyDetailRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataPrintJob;
using PrintManagement.Application.Payloads.ResponseModels.DataResourceForPrintJob;
using PrintManagement.Application.Payloads.ResponseModels.DataResourcePropertyDetail;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PrintManagement.Application.ImplementServices
{
    public class PrintJobService : IPrintJobService
    {
        private readonly IBaseRepository<ResourceForPrintJob> _baseResourceForPrintJobRepository;
        private readonly IBaseRepository<ResourcePropertyDetail> _baseResourcePropertyDetailRepository;
        private readonly IBaseRepository<ResourceProperty> _baseResourcePropertyRepository;
        private readonly IBaseRepository<Resources> _baseResourcesRepository;
        private readonly IBaseRepository<PrintJobs> _basePrintJobRepository;
        private readonly IBaseRepository<Project> _baseProjectRepository;
        private readonly IBaseRepository<Design> _baseDesignRepository;
        private readonly IBaseRepository<Customer> _baseCustomerRepository;
        private readonly ResourceForPrintJobConveter _resourceForPrintJobConveter;
        private readonly IPrintJobsRepository _printJobsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResourcePropertyServices _resourcePropertyService;
        private readonly IEmailService _emailService;

        public PrintJobService(IBaseRepository<ResourceForPrintJob> baseResourceForPrintJobRepository, IBaseRepository<ResourcePropertyDetail> baseResourcePropertyDetailRepository, IBaseRepository<ResourceProperty> baseResourcePropertyRepository, IBaseRepository<Resources> baseResourcesRepository, IBaseRepository<PrintJobs> basePrintJobRepository, IBaseRepository<Project> baseProjectRepository, IBaseRepository<Design> baseDesignRepository, IBaseRepository<Customer> baseCustomerRepository, ResourceForPrintJobConveter resourceForPrintJobConveter, IPrintJobsRepository printJobsRepository, IHttpContextAccessor httpContextAccessor, IResourcePropertyServices resourcePropertyService, IEmailService emailService)
        {
            _baseResourceForPrintJobRepository = baseResourceForPrintJobRepository;
            _baseResourcePropertyDetailRepository = baseResourcePropertyDetailRepository;
            _baseResourcePropertyRepository = baseResourcePropertyRepository;
            _baseResourcesRepository = baseResourcesRepository;
            _basePrintJobRepository = basePrintJobRepository;
            _baseProjectRepository = baseProjectRepository;
            _baseDesignRepository = baseDesignRepository;
            _baseCustomerRepository = baseCustomerRepository;
            _resourceForPrintJobConveter = resourceForPrintJobConveter;
            _printJobsRepository = printJobsRepository;
            _httpContextAccessor = httpContextAccessor;
            _resourcePropertyService = resourcePropertyService;
            _emailService = emailService;
        }

        public async Task<ResponseObject<IEnumerable<DataResponseResourceForPrintJob>>> CreateResourceForPrintJobs(Request_CreateResourceForPrintJob request)
        {
            var printJob = await _basePrintJobRepository.GetByIdAsync(request.PrintJobId);
            if (printJob == null)
            {
                return new ResponseObject<IEnumerable<DataResponseResourceForPrintJob>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Thiết kế chưa được duyệt tiến trình in",
                    Data = null
                };
            }

            if (printJob.PrintJobStatus != PjStatus.Pending && printJob.PrintJobStatus != PjStatus.Printing)
            {
                return new ResponseObject<IEnumerable<DataResponseResourceForPrintJob>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Thiết kế này chưa sẵn sàng để in hoặc đã in xong",
                    Data = null
                };
            }

            // Kiểm tra nếu không có tài nguyên nào được gửi
            if (request.ResourceDetails == null || !request.ResourceDetails.Any())
            {
                return new ResponseObject<IEnumerable<DataResponseResourceForPrintJob>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Danh sách tài nguyên không hợp lệ!",
                    Data = null
                };
            }

            // Bước 1: Tạo các ResourcePropertyDetails và lưu vào cơ sở dữ liệu
            var resourcePropertyDetailsResponse = await CreateResourcePropertyDetailsAsync(new Request_CreateResourcePropertyDetails
            {
                ResourceDetails = request.ResourceDetails,
            });

            if (resourcePropertyDetailsResponse.Status != StatusCodes.Status201Created)
            {
                return new ResponseObject<IEnumerable<DataResponseResourceForPrintJob>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = resourcePropertyDetailsResponse.Message,
                    Data = null
                };
            }
            printJob.PrintJobStatus = PjStatus.Printing;
            await _basePrintJobRepository.UpdateAsync(printJob);
            // Lấy danh sách ID từ các chi tiết tài nguyên đã tạo
            var createdPropertyDetailIds = resourcePropertyDetailsResponse.Data.Select(x => x.Id).ToList();

            // Bước 2: Cập nhật số lượng ResourceProperty
            foreach (var detail in request.ResourceDetails)
            {
                var currentResource = await _baseResourcePropertyRepository.GetByIdAsync(detail.PropertyId);
                var resouceType = await _baseResourcesRepository.GetByIdAsync(currentResource.ResourceId);
                if (currentResource != null && resouceType.ResourceType != RType.NonConsumable)
                {
                    // Trừ số lượng tài nguyên đã được sử dụng
                    var newQuantity = currentResource.Quantity - detail.Quantity;
                    await _resourcePropertyService.UpdateQuantityPropertyAsync(currentResource.Id, newQuantity);
                }
            }
            // Bước 3: Sử dụng danh sách ID đã tạo để tạo các ResourceForPrintJob
            var resourceForPrintJobs = createdPropertyDetailIds
                .Select(detailId => new ResourceForPrintJob
                {
                    PrintJobId = printJob.Id,
                    ResourcePropertyDetailId = detailId
                })
                .ToList();
            var design = await _baseDesignRepository.GetByIdAsync(printJob.DesignId);
            var project = await _baseProjectRepository.GetByIdAsync(design.ProjectId);
            await _baseResourceForPrintJobRepository.CreateAsync(resourceForPrintJobs);
            project.ProjectStatus = PStatus.Printing;
            await _baseProjectRepository.UpdateAsync(project);
            // Chuyển đổi các bản ghi thành DTO để trả về
            var responseData = resourceForPrintJobs
                .Select(resourceForPrintJob => _resourceForPrintJobConveter.EntitytoDTO(resourceForPrintJob))
                .ToList();

            return new ResponseObject<IEnumerable<DataResponseResourceForPrintJob>>
            {
                Status = StatusCodes.Status201Created,
                Message = "Tạo thành công các tài nguyên cho công việc in!",
                Data = responseData
            };
        }

        // Phương thức hỗ trợ: Tạo ResourcePropertyDetails
        public async Task<ResponseObject<IEnumerable<DataResponseResourcePropertyDetail>>> CreateResourcePropertyDetailsAsync(Request_CreateResourcePropertyDetails request)
        {
            var hostUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

            // Kiểm tra xem request có tài nguyên không
            if (request.ResourceDetails == null || !request.ResourceDetails.Any())
            {
                return new ResponseObject<IEnumerable<DataResponseResourcePropertyDetail>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Danh sách tài nguyên không hợp lệ!",
                    Data = null
                };
            }

            var invalidProperties = new List<int>();
            var invalidQuantity = new List<string>();
            // Kiểm tra từng PropertyId xem có tồn tại trong bảng Resources hay không
            foreach (var detail in request.ResourceDetails)
            {
                var exists = await _baseResourcePropertyRepository.GetByIdAsync(detail.PropertyId);
                if (exists == null)
                {
                    invalidProperties.Add(detail.PropertyId);
                }
                else if (exists.Quantity < detail.Quantity)
                {
                    invalidQuantity.Add(exists.ResourcePropertyName);
                }

            }

            // Xử lý trường hợp Property không tồn tại hoặc không đủ số lượng
            if (invalidProperties.Any())
            {
                return new ResponseObject<IEnumerable<DataResponseResourcePropertyDetail>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Các Tài nguyên không tồn tại: {string.Join(", ", invalidProperties)}",
                    Data = null
                };
            }

            if (invalidQuantity.Any())
            {
                return new ResponseObject<IEnumerable<DataResponseResourcePropertyDetail>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Các Tài nguyên không đủ số lượng: {string.Join(", ", invalidQuantity)}",
                    Data = null
                };
            }

            // Tạo danh sách ResourcePropertyDetail
            var resourcePropertyDetails = new List<ResourcePropertyDetail>();

            foreach (var detail in request.ResourceDetails)
            {
                var fileName = await HandleUploadFile.WirteFileResourcePropertyDetail(detail.Image);
                var resourceDetail = new ResourcePropertyDetail
                {
                    PropertyId = detail.PropertyId,
                    PropertyDetailName = detail.PropertyDetailName,
                    Image = fileName,
                    Price = detail.Price,
                    Quantity = detail.Quantity
                };

                resourcePropertyDetails.Add(resourceDetail);
            }

            var createdDetails = await _baseResourcePropertyDetailRepository.CreateAsync(resourcePropertyDetails);

            // Trả về kết quả
            var responseDetails = createdDetails
                .Select(detail => new DataResponseResourcePropertyDetail
                {
                    Id = detail.Id,
                    PropertyId = detail.PropertyId,
                    PropertyDetailName = detail.PropertyDetailName,
                    Image = $"{hostUrl}/images/resourceDetails/{detail.Image}",
                    Price = detail.Price,
                    Quantity = detail.Quantity
                }).ToList();

            return new ResponseObject<IEnumerable<DataResponseResourcePropertyDetail>>
            {
                Status = StatusCodes.Status201Created,
                Message = "Tạo tài nguyên thành công!",
                Data = responseDetails
            };
        }
        public async Task<ResponseObject<DataResponseResourcePropertyDetail>> UpdateResourcePropertyDetailAsync(int id, Request_UpdateResourcePropertyDetail request)
        {
            // Lấy thông tin ResourcePropertyDetail từ CSDL
            var resourceDetail = await _baseResourcePropertyDetailRepository.GetByIdAsync(id);
            if (resourceDetail == null)
            {
                return new ResponseObject<DataResponseResourcePropertyDetail>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Chi tiết tài nguyên không tồn tại!",
                    Data = null
                };
            }

            // Lấy thông tin ResourceProperty để kiểm tra số lượng tồn kho
            var resourceProperty = await _baseResourcePropertyRepository.GetByIdAsync(resourceDetail.PropertyId);
            if (resourceProperty == null)
            {
                return new ResponseObject<DataResponseResourcePropertyDetail>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Tài nguyên không tồn tại!",
                    Data = null
                };
            }
            var resourceType = await _baseResourcesRepository.GetByIdAsync(resourceProperty.ResourceId);
            if (resourceType != null && resourceType.ResourceType != RType.NonConsumable)
            {
                // Bước 1: Hoàn lại số lượng cũ vào ResourceProperty
                resourceProperty.Quantity += resourceDetail.Quantity;
                await _baseResourcePropertyRepository.UpdateAsync(resourceProperty);
                // Bước 2: Kiểm tra xem số lượng mới có hợp lệ không (sau khi đã hoàn lại số lượng cũ)
                if (request.Quantity > resourceProperty.Quantity)
                {
                    return new ResponseObject<DataResponseResourcePropertyDetail>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = $"Số lượng yêu cầu ({request.Quantity}) vượt quá số lượng trong kho ({resourceProperty.Quantity})!",
                        Data = null
                    };
                }

                // Bước 3: Cập nhật số lượng mới
                resourceProperty.Quantity -= request.Quantity;
                
            }
            if (request.Quantity > resourceProperty.Quantity)
            {
                return new ResponseObject<DataResponseResourcePropertyDetail>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = $"Số lượng yêu cầu ({request.Quantity}) vượt quá số lượng trong kho ({resourceProperty.Quantity})!",
                    Data = null
                };
            }
            resourceDetail.Quantity = request.Quantity;
            // Bước 4: Cập nhật các trường khác không phải là PropertyId
            resourceDetail.PropertyDetailName = request.PropertyDetailName;
            resourceDetail.Price = request.Price;
            resourceDetail.Image = await HandleUploadFile.WirteFileResourcePropertyDetail(request.Image);

            // Bước 5: Cập nhật lại vào database
            await _baseResourcePropertyDetailRepository.UpdateAsync(resourceDetail);
            await _baseResourcePropertyRepository.UpdateAsync(resourceProperty);

            // Bước 6: Trả về kết quả sau khi cập nhật
            var responseDetail = new DataResponseResourcePropertyDetail
            {
                Id = resourceDetail.Id,
                PropertyId = resourceDetail.PropertyId,  // Không được thay đổi PropertyId
                PropertyDetailName = resourceDetail.PropertyDetailName,
                Image = resourceDetail.Image,
                Price = resourceDetail.Price,
                Quantity = resourceDetail.Quantity
            };

            return new ResponseObject<DataResponseResourcePropertyDetail>
            {
                Status = StatusCodes.Status200OK,
                Message = "Cập nhật thành công!",
                Data = responseDetail
            };
        }

        public async Task<ResponseObject<bool>> DeleteResourcePropertyDetailAsync(int id)
        {
            // Lấy thông tin ResourcePropertyDetail từ cơ sở dữ liệu
            var resourceDetail = await _baseResourcePropertyDetailRepository.GetByIdAsync(id);
            if (resourceDetail == null)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Chi tiết tài nguyên không tồn tại!",
                    Data = false
                };
            }

            // Lấy thông tin ResourceProperty liên quan
            var resourceProperty = await _baseResourcePropertyRepository.GetByIdAsync(resourceDetail.PropertyId);
            if (resourceProperty == null)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Tài nguyên liên quan không tồn tại!",
                    Data = false
                };
            }
            var resourceType = await _baseResourcesRepository.GetByIdAsync(resourceProperty.ResourceId);
            if (resourceType != null && resourceType.ResourceType != RType.NonConsumable)
            {
                // Bước 1: Hoàn lại số lượng cho ResourceProperty
                resourceProperty.Quantity += resourceDetail.Quantity;

                // Cập nhật lại ResourceProperty trong cơ sở dữ liệu
                await _baseResourcePropertyRepository.UpdateAsync(resourceProperty);
            }

            // Bước 2: Xóa ResourcePropertyDetail
            await _baseResourcePropertyDetailRepository.DeleteAsync(id);

            return new ResponseObject<bool>
            {
                Status = StatusCodes.Status200OK,
                Message = "Xóa thành công và số lượng tài nguyên đã được hoàn lại!",
                Data = true
            };
        }

        public async Task<ResponseObject<bool>> UpdatePrintJobStatus(int printJobId, PjStatus newStatus)
        {
            // Bước 1: Tìm PrintJob dựa trên DesignId
            var printJob = await _basePrintJobRepository.GetByIdAsync(printJobId);
            if (printJob == null)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy công việc in với DesignId đã cung cấp.",
                    Data = false
                };
            }

            // Bước 2: Cập nhật trạng thái cho PrintJob
            printJob.PrintJobStatus = newStatus;
            await _basePrintJobRepository.UpdateAsync(printJob);

            // Bước 3: Tìm Design và Project liên quan
            var design = await _baseDesignRepository.GetByIdAsync(printJob.DesignId);
            if (design == null)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy thiết kế liên quan đến công việc in.",
                    Data = false
                };
            }

            var project = await _baseProjectRepository.GetByIdAsync(design.ProjectId);
            if (project == null)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy dự án liên quan đến thiết kế.",
                    Data = false
                };
            }

            // Bước 4: Cập nhật trạng thái Project dựa trên trạng thái của PrintJob

            if (newStatus == PjStatus.Completed)
            {
                var printJobs = await _printJobsRepository.GetPrintJobsByDesignid(printJob.DesignId);

                // Kiểm tra nếu tất cả các PrintJob đều có trạng thái Completed
                bool allPrintJobsCompleted = printJobs.All(pj => pj.PrintJobStatus == PjStatus.Completed);

                if (allPrintJobsCompleted)
                {
                    project.ProjectStatus = PStatus.Completed; // Cập nhật ProjectStatus thành Completed nếu tất cả các PrintJob đã hoàn thành

                    // Gửi email thông báo hoàn thành dự án
                    var customer = await _baseCustomerRepository.GetByIdAsync(project.CustomerId);
                    var emailContent = _emailService.GenerateProjectCompletionEmail(project.ProjectName);
                    var message = new EmailMessage(new string[] { customer.Email }, $"Dự án của {customer.FullName} đã hoàn tất", emailContent);
                    var responseMessage = _emailService.SendEmail(message);

                    // Cập nhật project
                    await _baseProjectRepository.UpdateAsync(project);
                }
            }

            // Bước 5: Cập nhật Project


            return new ResponseObject<bool>
            {
                Status = StatusCodes.Status200OK,
                Message = "Cập nhật trạng thái thành công.",
                Data = true
            };
        }

        public async Task<ResponseObject<IEnumerable<DataResponsePrintJob>>> GetResourcePrintJobByDesign(int designId)
        {
            // Lấy PrintJobs dựa trên designId
            var printJobs = await _printJobsRepository.GetPrintJobsByDesignid(designId);
            // Kiểm tra nếu không có printJob nào
            if (printJobs == null || !printJobs.Any())
            {
                return new ResponseObject<IEnumerable<DataResponsePrintJob>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Thiết kế này chưa được duyệt in",
                    Data = null,
                };
            }
            // Ánh xạ dữ liệu sang đối tượng DataResponsePrintJobCustom
            var response = printJobs.Select(x => new DataResponsePrintJob
            {
                Id = x.Id,
                DesignId = designId,
                PrintJobStatus = x.PrintJobStatus,
            }).ToList();
            // Trả về kết quả cuối cùng
            return new ResponseObject<IEnumerable<DataResponsePrintJob>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Dữ liệu đã được lấy thành công",
                Data = response
            };
        }

        public async Task<ResponseObject<IEnumerable<DataResponseResourcePropertyDetail>>> GetResourceDetailByPrintJob(int printJobId)
        {
            var printJob = await _basePrintJobRepository.GetByIdAsync(printJobId);
            var hostUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            if (printJob == null)
            {
                return new ResponseObject<IEnumerable<DataResponseResourcePropertyDetail>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Thiết kế này chưa được duyệt in",
                    Data = null,
                };
            }
            var resourceDetail = await _printJobsRepository.GetResourceDetailsByPrintJob(printJob);
            if (resourceDetail == null)
            {
                return new ResponseObject<IEnumerable<DataResponseResourcePropertyDetail>>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Chưa có tài nguyên nào được sử dụng",
                    Data = null,
                };
            }
            var response = resourceDetail.Select(x => new DataResponseResourcePropertyDetail
            {
                Id = x.Id,
                PropertyId = x.PropertyId,
                PropertyDetailName = x.PropertyDetailName,
                Image = $"{hostUrl}/images/resourceDetails/{x.Image}",
                Price = x.Price,
                Quantity = x.Quantity,
            }).ToList();

            return new ResponseObject<IEnumerable<DataResponseResourcePropertyDetail>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy thành công tài nguyên sử dụng cho tác vụ in",
                Data = response,
            };
        }
    }
}