using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Org.BouncyCastle.Asn1.Ocsp;
using PrintManagement.Application.Handle.HandleFile;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.Mappers;
using PrintManagement.Application.Payloads.RequestModels.ResourcePropertyRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataResourceProperty;
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
    public class ResourcePropertyService : IResourcePropertyServices
    {
        private readonly IBaseRepository<ResourceProperty> _baseResourcePropertyRepository;
        private readonly IBaseRepository<Resources> _baseResourceRepository;
        private readonly ResourcePropertyConveter _resourcePropertyConveter;
        private readonly IResourcesRepository _resourcesRepository;
        private readonly IResourcePropertyRepository _resourcePropertyRepository;

        public ResourcePropertyService(IBaseRepository<ResourceProperty> baseResourcePropertyRepository, IBaseRepository<Resources> baseResourceRepository, ResourcePropertyConveter resourcePropertyConveter, IResourcesRepository resourcesRepository, IResourcePropertyRepository resourcePropertyRepository)
        {
            _baseResourcePropertyRepository = baseResourcePropertyRepository;
            _baseResourceRepository = baseResourceRepository;
            _resourcePropertyConveter = resourcePropertyConveter;
            _resourcesRepository = resourcesRepository;
            _resourcePropertyRepository = resourcePropertyRepository;
        }

        public async Task<ResponseObject<DataResponseResourceProperty>> CreateResourcePropertyAsync(Request_ResourceProperty request)
        {
            try
            {
                var resources = await _baseResourceRepository.GetByIdAsync(request.ResourceId);
                if (resources == null)
                {
                    return new ResponseObject<DataResponseResourceProperty>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Tài nguyên không tồn tại",
                        Data = null,
                    };
                }

                var resourceProperty = new ResourceProperty
                {
                    ResourcePropertyName = request.ResourcePropertyName,
                    ResourceId = request.ResourceId,
                    Quantity = request.Quantity,
                };
                
                await _baseResourcePropertyRepository.CreateAsync(resourceProperty);
                await _resourcesRepository.UpdateAvailableQuantityAsync(request.ResourceId);
                var response = _resourcePropertyConveter.EntitytoDTO(resourceProperty);

                return new ResponseObject<DataResponseResourceProperty>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Tạo chi tiết tài nguyên thành công",
                    Data = response,
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseResourceProperty>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null,
                };
            }

        }

        public async Task<ResponseObject<bool>> DeteleteResourcePropertyAsync(int id)
        {
            try
            {
                var resourceProperty = await _baseResourcePropertyRepository.GetByIdAsync(id);
                if (resourceProperty == null)
                {
                    return new ResponseObject<bool>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Tài nguyên không tồn tại",
                        Data = false,
                    };
                }
                await _baseResourcePropertyRepository.DeleteAsync(id);
                await _resourcesRepository.UpdateAvailableQuantityAsync(resourceProperty.ResourceId);
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Tài nguyên đã được xóa",
                    Data = true,
                };

            }
            catch (Exception ex)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = false,
                };
            }
        }

        public async Task<ResponseObject<IEnumerable<DataResponseResourceProperty>>> GetAllResourcePropertiesAsync()
        {
            try
            {
                var resourcesProperty = await _baseResourcePropertyRepository.GetAllAsync();
                var response = resourcesProperty.Select(x => _resourcePropertyConveter.EntitytoDTO(x));
                return new ResponseObject<IEnumerable<DataResponseResourceProperty>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Lấy thông tin thành công",
                    Data = response.ToList(),
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<IEnumerable<DataResponseResourceProperty>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null,
                };
            }
        }

        public async Task<ResponseObject<DataResponseResourceProperty>> UpdateResourcePropertyAsync(int id, Request_ResourceProperty request)
        {
            try
            {
                var resources = await _baseResourcePropertyRepository.GetByIdAsync(id);
                if (resources == null)
                {
                    return new ResponseObject<DataResponseResourceProperty>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Tài nguyên không tồn tại",
                        Data = null,
                    };
                }
                resources.ResourcePropertyName = request.ResourcePropertyName;
                resources.ResourceId = request.ResourceId;
                resources.Quantity = request.Quantity;
                await _baseResourcePropertyRepository.UpdateAsync(resources);
                await _resourcesRepository.UpdateAvailableQuantityAsync(request.ResourceId);
                var response = _resourcePropertyConveter.EntitytoDTO(resources);
                return new ResponseObject<DataResponseResourceProperty>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Chi tiết tài nguyên đã được cập nhật",
                    Data = response,
                };

            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseResourceProperty>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null,
                };
            }
        }
        public async Task<ResponseObject<DataResponseResourceProperty>> UpdateQuantityPropertyAsync(int id, int quantity)
        {
            try
            {
                var resourceProperty = await _baseResourcePropertyRepository.GetByIdAsync(id);
                if (resourceProperty == null)
                {
                    return new ResponseObject<DataResponseResourceProperty>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Tài nguyên không tồn tại",
                        Data = null,
                    };
                }
                resourceProperty.Quantity = quantity;
                await _baseResourcePropertyRepository.UpdateAsync(resourceProperty);
                await _resourcesRepository.UpdateAvailableQuantityAsync(resourceProperty.ResourceId);

                var response = _resourcePropertyConveter.EntitytoDTO(resourceProperty);
                return new ResponseObject<DataResponseResourceProperty>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Số lượng tài nguyên đã được cập nhật",
                    Data = response,
                };

            }
            catch (Exception ex)
            {
                return new ResponseObject<DataResponseResourceProperty>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi: " + ex.Message,
                    Data = null,
                };
            }
        }

        public async Task<ResponseObject<IEnumerable<DataResponseResourceProperty>>> GetResourcePropertyByResourceId(int resourceId)
        {
            var resourceP = await _resourcePropertyRepository.GetResourcePropertiesByResourceId(resourceId);
            var response = resourceP.Select(x => _resourcePropertyConveter.EntitytoDTO(x)).ToList();
            return new ResponseObject<IEnumerable<DataResponseResourceProperty>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy danh sách tài nguyên thành công",
                Data = response,
            };
        }
    }
}
