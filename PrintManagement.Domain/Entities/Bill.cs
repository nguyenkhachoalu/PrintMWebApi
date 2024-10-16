using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public enum Status
    {
        Pending,        // Chờ thanh toán
        Paid,           // Đã thanh toán
        Cancelled,      // Hóa đơn đã bị hủy
        Processing,     // Đang xử lý thanh toán
        Overdue,        // Quá hạn thanh toán
        Refunded        // Đã hoàn tiền
    }
    public class Bill : BaseEntity
    {
        public string BillName { get; set; } = string.Empty;
        public Status BillStatus { get; set; }
        public decimal TotalMoney { get; set; }
        public int ProjectId { get; set; }
        public int CustomerId { get; set; }
        public string TradingCode { get; set; } = string.Empty ;
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int EmployeeId { get; set; }

        public Project? Project { get; set; }
        public Customer? Customer { get; set; }
        public User? Employee { get; set; }
    }
}
