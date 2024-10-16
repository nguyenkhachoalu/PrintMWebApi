using PrintManagement.Application.Payloads.ResponseModels.DataProject;
using PrintManagement.Application.Payloads.ResponseModels.DataResources;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Mappers
{
    public class ResourcesConveter
    {
        public DataResponseResources EntitytoDTO(Resources resources)
        {
            return new DataResponseResources()
            {
                Id = resources.Id,
                ResourceName = resources.ResourceName,
                Image = resources.Image,
                ResourceType = resources.ResourceType,
                AvailableQuantity = resources.AvailableQuantity,
                ResourceStatus = resources.ResourceStatus,
            };
        }
    }
}
