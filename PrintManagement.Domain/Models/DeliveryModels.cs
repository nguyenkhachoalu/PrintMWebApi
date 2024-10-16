using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Models
{
    public class DeliveryModels
    {
        public int Id { get; set; }
        public int ShippingMethodId { get; set; }
        public int CustomerId { get; set; }
        public int DeliverId { get; set; }
        public int ProjectId { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public DateTime EstimateDeliveryTime { get; set; }
        public DateTime ActualDeliveryTime { get; set; }
        public DStatus DeliveryStatus { get; set; }
        public string ShippingMethodName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string CustomeName { get; set; } = string.Empty;
        public string DeliverName { get; set; } = string.Empty;
    }
}
