using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using PrintManagement.Application.Handle.HandleFile;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.Mappers;
using PrintManagement.Application.Payloads.RequestModels.ResourcesRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataResources;
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
    public class ResourcesService : IResourcesService
    {
        private readonly IBaseRepository<Resources> _baseResourcesRepository;
        private readonly IResourcesRepository _resourcesRepository;
        private readonly ResourcesConveter _resourcesConveter;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResourcesService(IBaseRepository<Resources> baseResourcesRepository, IResourcesRepository resourcesRepository, ResourcesConveter resourcesConveter, IHttpContextAccessor httpContextAccessor)
        {
            _baseResourcesRepository = baseResourcesRepository;
            _resourcesRepository = resourcesRepository;
            _resourcesConveter = resourcesConveter;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseObject<DataResponseResources>> CreateResourcesAsync(Request_Resources request)
        {
            try
            {
                if (await _resourcesRepository.GetResourcesByName(request.ResourceName) != null)
                {
                    return new ResponseObject<DataResponseResources>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Tài nguyên này đã có",
                        Data = null,
                    };
                }
                var resources = new Resources
                {
                    ResourceName = request.ResourceName,
                    Image = await HandleUploadFile.WirteFileResources(request.Image),
                    ResourceType = request.ResourceType,
                    AvailableQuantity = request.AvailableQuantity,
                    ResourceStatus = request.ResourceStatus,
                };
                await _baseResourcesRepository.CreateAsync(resources);
                return new ResponseObject<DataResponseResources>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Tạo tài nguyên thành công",
                    Data = _resourcesConveter.EntitytoDTO(resources),
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseResources>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: "+ex.Message,
                    Data = null,
                };
            }
        }

        public async Task<ResponseObject<bool>> DeteleteResourcesAsync(int id)
        {
            try
            {
                var resources = await _baseResourcesRepository.GetByIdAsync(id);
                if (resources == null)
                {
                    return new ResponseObject<bool>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Tài nguyên không tồn tại",
                        Data = false,
                    };
                }
                await _baseResourcesRepository.DeleteAsync(id);
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài nguyên đã được xóa",
                    Data = true,
                };

            }
            catch(Exception ex)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: "+ex.Message,
                    Data = false,
                };
            }
        }

        public async Task<ResponseObject<IEnumerable<DataResponseResources>>> GetAllResourceAsync()
        {
            try
            {
                var resources = await _baseResourcesRepository.GetAllAsync();
                var hostUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                var response = resources.Select(x => new DataResponseResources
                {
                    Id = x.Id,
                    ResourceName = x.ResourceName,
                    Image = $"{hostUrl}/images/resources/{x.Image}",
                    ResourceType = x.ResourceType,
                    AvailableQuantity = x.AvailableQuantity,
                    ResourceStatus = x.ResourceStatus,
                });
                return new ResponseObject<IEnumerable<DataResponseResources>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Lấy danh sách tài nguyên thành công",
                    Data = response.ToList(),
                };

            }
            catch (Exception ex)
            {
                return new ResponseObject<IEnumerable<DataResponseResources>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: "+ex.Message,
                    Data = null,
                };

            }
        }

        public async Task<ResponseObject<DataResponseResources>> UpdateResourcesAsync(int id, Request_Resources request)
        {
            try
            {
                var resources = await _baseResourcesRepository.GetByIdAsync(id);
                if (resources == null)
                {
                    return new ResponseObject<DataResponseResources>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Tài nguyên không tồn tại",
                        Data = null,
                    };
                }
                resources.ResourceName = request.ResourceName;
                resources.Image = await HandleUploadFile.WirteFileResources(request.Image);
                resources.ResourceType = request.ResourceType;
                resources.AvailableQuantity = request.AvailableQuantity;
                resources.ResourceStatus = request.ResourceStatus;
                await _baseResourcesRepository.UpdateAsync(resources);
                return new ResponseObject<DataResponseResources>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài nguyên đã được cập nhật",
                    Data = _resourcesConveter.EntitytoDTO(resources),
                };

            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseResources>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null,
                };
            }
        }
    }
}
