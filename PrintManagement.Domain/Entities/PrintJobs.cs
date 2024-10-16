using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public enum PjStatus
    {
        Pending,       // Đang chờ xử lý
        Printing,      // Đang in
        Completed,     // Đã in xong
        Cancelled,     // Bị hủy
        Error          // Lỗi khi in
    }
    public class PrintJobs : BaseEntity
    {
        public int DesignId { get; set; }
        public PjStatus PrintJobStatus { get; set; }

        public Design? Design { get; set; }
    }
}
