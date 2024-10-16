using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public class ConfirmEmail : BaseEntity
    {
        public int UserId { get; set; }
        public string ConfirmCode { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsConfirm { get; set; }
        public User? User { get; set; }
    }
}
