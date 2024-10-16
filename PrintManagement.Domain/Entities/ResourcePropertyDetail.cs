using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public class ResourcePropertyDetail : BaseEntity
    {
        public int PropertyId { get; set; }
        public string PropertyDetailName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public ResourceProperty? ResourceProperty { get; set; }
    }
}
