using PrintManagement.Application.Payloads.ResponseModels.DataDesign;
using PrintManagement.Application.Payloads.ResponseModels.DataProject;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Mappers
{
    public class DesignConveter
    {
        public DataResponseDesign EntitytoDTO(Design design)
        {
            return new DataResponseDesign()
            {
                Id = design.Id,
                ProjectId = design.ProjectId,
                DesignerId = design.DesignerId,
                FilePath = design.FilePath,
                DesignTime = design.DesignTime,
                DesignStatus = design.DesignStatus,
                ApproverId = design.ApproverId,
            };
        }
    }
}
