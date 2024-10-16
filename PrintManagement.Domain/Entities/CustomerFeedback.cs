using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public class CustomerFeedback : BaseEntity
    {
        public int ProjectId { get; set; }
        public int CustomerId { get; set; }
        public string FeedbackContent { get; set; } = string.Empty;
        public string ResponseByCompany { get; set; } = string.Empty ;
        public int UserFeedbackId { get; set; }
        public DateTime FeedBackTime { get; set; }
        public DateTime ResponseTime { get; set; }

        public Project? Project { get; set; }
        public Customer? Customer { get; set; }
        public User? UserFeedback { get; set; }
    }
}
