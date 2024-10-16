using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public enum DStatus
    {
        Processing,      // Đơn hàng đang được xử lý
        Shipped,         // Đơn hàng đã được gửi đi
        Delivered,       // Đơn hàng đã được giao
        Cancelled,       // Đơn hàng đã bị hủy
        Returned         // Đơn hàng đã bị trả lại
    }
    public class Delivery : BaseEntity
    {
        public int ShippingMethodId { get; set; }
        public int CustomerId { get; set; }
        public int DeliverId { get; set; }
        public int ProjectId { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public DateTime EstimateDeliveryTime { get; set; }
        public DateTime ActualDeliveryTime { get; set; }
        public DStatus DeliveryStatus { get; set; }


        public ShippingMethod? ShippingMethod { get; set; }
        public Customer? Customer { get; set; }
        public User? Deliver { get; set; }
        public Project? Project { get; set; }
    }
}
