using PrintManagement.Application.Handle.HandleEmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.InterfaceServices
{
    public interface IEmailService
    {
        string SendEmail(EmailMessage emailMessage);
        string GenerateConfirmationCodeEmail(string confirmationCode);
        string GenerateProjectCompletionEmail(string projectName);
        string GenerateDeliveryCompletionEmail(string projectName);
    }
}
