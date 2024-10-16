using PrintManagement.Application.Payloads.ResponseModels.DataResourceProperty;
using PrintManagement.Application.Payloads.ResponseModels.DataResources;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Mappers
{
    public class ResourcePropertyConveter
    {
        public DataResponseResourceProperty EntitytoDTO(ResourceProperty resourceProperty)
        {
            return new DataResponseResourceProperty()
            {
                Id = resourceProperty.Id,
                ResourcePropertyName = resourceProperty.ResourcePropertyName,
                ResourceId = resourceProperty.ResourceId,
                Quantity = resourceProperty.Quantity,
                
            };
        }
    }
}
