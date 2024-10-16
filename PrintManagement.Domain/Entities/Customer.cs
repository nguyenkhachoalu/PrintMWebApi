using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public ICollection<Delivery>? Permissions { get; set; }
        public ICollection<CustomerFeedback>? CustomerFeedbacks { get; set; }
        public ICollection<Bill>? Bills { get; set; }
        public ICollection<Project>? Projects { get; set; }
    }
}
