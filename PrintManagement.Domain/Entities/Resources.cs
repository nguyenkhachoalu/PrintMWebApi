using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public enum RType
    {
        Consumable,   // Tiêu hao (ví dụ: giấy, mực in)
        NonConsumable
    }
    public enum RStatus
    {
        Available,    // Sẵn sàng sử dụng
        Maintenance   // Cần bảo trì
    }

    public class Resources : BaseEntity
    {
        public string ResourceName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public RType ResourceType { get; set; }
        public int AvailableQuantity { get; set; }
        public RStatus ResourceStatus { get; set; }

        public ICollection<ResourceProperty>? ResourceProperties { get; set; }
    }
}
