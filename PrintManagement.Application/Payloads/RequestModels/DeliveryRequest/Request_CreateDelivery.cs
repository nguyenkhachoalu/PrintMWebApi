using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.RequestModels.DeliveryRequest
{
    public class Request_CreateDelivery
    {
        public int ShippingMethodId { get; set; }
        public int DeliverId { get; set; }
        public int ProjectId { get; set; }
    }
}
