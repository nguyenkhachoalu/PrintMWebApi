using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; } = string.Empty;
        public string Password{ get; set; } = string.Empty ;
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public  string Avatar { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime{ get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public int? TeamId { get; set; }
        public bool IsActive { get; set; }


        public Team? Team { get; set; }
        public ICollection<Team>? Teams { get; set; }
        public ICollection<Permissions>? Permissions { get; set; }
        public ICollection<Delivery>? Deliverys { get; set; }
    }
}
