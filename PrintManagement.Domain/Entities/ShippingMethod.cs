using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public class ShippingMethod : BaseEntity
    {
        public string ShippingMethodName { get; set; } = string.Empty;
    }
}
