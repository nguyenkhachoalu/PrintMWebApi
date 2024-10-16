using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.ResponseModels.DataDesign
{
    public class DataResponseDesign : DataResponseBase
    {
        public int ProjectId { get; set; }
        public int DesignerId { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public DateTime DesignTime { get; set; }
        public DeStatus DesignStatus { get; set; }
        public int? ApproverId { get; set; } // Người phê duyệt thiết kế
    }
}
