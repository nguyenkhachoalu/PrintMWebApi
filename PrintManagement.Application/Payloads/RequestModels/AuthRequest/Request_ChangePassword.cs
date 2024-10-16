using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.RequestModels.AuthRequest
{
    public class Request_ChangePassword
    {
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string OldPassword { get; set; } = string.Empty;
        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        public string NewPassword { get; set; } = string.Empty;
        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        public string ConfirmPassword { get; set;} = string.Empty;
    }
}
