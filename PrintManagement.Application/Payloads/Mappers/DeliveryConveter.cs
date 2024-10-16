using PrintManagement.Application.Payloads.ResponseModels.DataDelivery;
using PrintManagement.Application.Payloads.ResponseModels.DataDesign;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Mappers
{
    public class DeliveryConveter
    {
        public DataResponseDelivery EntitytoDTO(Delivery delivery)
        {
            return new DataResponseDelivery()
            {
                Id = delivery.Id,
                ShippingMethodId = delivery.ShippingMethodId,
                CustomerId = delivery.CustomerId,
                DeliverId = delivery.DeliverId,
                ProjectId = delivery.ProjectId,
                DeliveryAddress = delivery.DeliveryAddress,
                EstimateDeliveryTime = delivery.EstimateDeliveryTime,
                ActualDeliveryTime = delivery.ActualDeliveryTime,
                DeliveryStatus = delivery.DeliveryStatus,
            };
        }
    }
}
