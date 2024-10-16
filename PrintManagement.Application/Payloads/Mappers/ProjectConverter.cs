using Org.BouncyCastle.Asn1.Ocsp;
using PrintManagement.Application.Payloads.ResponseModels.DataProject;
using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Mappers
{
    public class ProjectConverter
    {
        public DataResponseProject EntitytoDTO(Project project)
        {
            return new DataResponseProject()
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                RequestDescriptionFromCustomer = project.RequestDescriptionFromCustomer,
                StartDate = project.StartDate,
                Image = project.Image,
                EmployeeId = project.EmployeeId,
                ExpectedEndDate = project.ExpectedEndDate,
                CustomerId = project.CustomerId,
                ProjectStatus = project.ProjectStatus
            };
        }
    }
}
