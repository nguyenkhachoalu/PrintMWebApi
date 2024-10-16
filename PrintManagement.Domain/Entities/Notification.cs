using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public string Context { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public bool IsSeen{ get; set; }

        // Navigation Properties
        public User? User { get; set; }
    }
}
