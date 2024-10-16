using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.ResponseModels.DataProject
{
    public class DataResponseProject : DataResponseBase
    {
        public string ProjectName { get; set; } = string.Empty;
        public string RequestDescriptionFromCustomer { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Image { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public int CustomerId { get; set; }
        public PStatus ProjectStatus { get; set; }
    }
}
