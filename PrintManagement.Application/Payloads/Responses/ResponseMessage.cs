﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Responses
{
    public static class ResponseMessage
    {
        public static string GetEmailSuccessMessage(string email)
        {
            return $"Email đã được gửi đến: {email}";
        }
    }
}
