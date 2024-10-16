using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.RequestModels.AuthRequest
{
    public class Request_ForgotPassword
    {
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPasword { get; set; } = string.Empty;
    }
}
