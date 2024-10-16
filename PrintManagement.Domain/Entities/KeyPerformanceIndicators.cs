using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Entities
{
    public enum Period
    {
        Monthly,  // Tháng
        Quarterly,  // Quý
        Yearly  // Năm
    }
    public class KeyPerformanceIndicators : BaseEntity
    {
        public int EmployeeId { get; set; }
        public string IndicatorName { get; set; } = string.Empty;
        public int Target { get; set; }
        public int ActuallyAchieved { get; set; }
        public Period TimePeriod { get; set; }
        public bool AchieveKPI { get; set; }

        public User? Employee { get; set; }
    }
}
