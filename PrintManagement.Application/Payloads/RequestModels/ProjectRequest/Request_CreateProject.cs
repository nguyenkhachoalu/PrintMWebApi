using System;
using PrintManagement.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PrintManagement.Application.Payloads.RequestModels.ProjectRequest
{
    public class Request_CreateProject
    {
        public string ProjectName { get; set; } = string.Empty;
        public string RequestDescriptionFromCustomer { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public IFormFile Image { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public int CustomerId { get; set; }
    }
}
