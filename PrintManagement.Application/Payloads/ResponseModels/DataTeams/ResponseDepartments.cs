using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.ResponseModels.DataTeams
{
    public class ResponseDepartments : DataResponseBase
    {
        public string Name { get; set; } = string.Empty;
    }
}
