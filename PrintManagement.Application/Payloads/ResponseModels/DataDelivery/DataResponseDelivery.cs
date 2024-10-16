﻿using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.ResponseModels.DataDelivery
{
    public class DataResponseDelivery : DataResponseBase
    {
        public int ShippingMethodId { get; set; }
        public int CustomerId { get; set; }
        public int DeliverId { get; set; }
        public int ProjectId { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public DateTime EstimateDeliveryTime { get; set; }
        public DateTime ActualDeliveryTime { get; set; }
        public DStatus DeliveryStatus { get; set; }
    }
}