using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public enum PStatus
    {
        Designing,   // Đang thiết kế
        ResourcePreparation,// Đang chuẩn bị tài nguyên
        Printing,    // Đang in
        Completed,   // Đã hoàn thành
        Cancelled    // Bị hủy
    }
    public class Project : BaseEntity
    {
        public string ProjectName { get; set; } = string.Empty;
        public string RequestDescriptionFromCustomer { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Image { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public int CustomerId { get; set; }
        public PStatus ProjectStatus  { get; set; }

        public User? Employee { get; set; }
        public Customer? Customer { get; set; }
    }
}
