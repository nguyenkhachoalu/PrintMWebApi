using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public enum DeStatus
    {
        Awaiting,// Đang phác thảo
        Refuse,     // Từ chối
        Approved,     // Đã phê duyệt
    }
    public class Design : BaseEntity
    {
        public int ProjectId { get; set; }
        public int DesignerId { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public DateTime DesignTime { get; set; }
        public DeStatus DesignStatus { get; set; }
        public int? ApproverId { get; set; } // Người phê duyệt thiết kế

        public Project? Project { get; set; }
        public User? Designer { get; set; }
        public User? Approver { get; set; }
    }
}
