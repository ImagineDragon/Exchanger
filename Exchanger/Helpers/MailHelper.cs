using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Web;

namespace Exchanger.Helpers
{
    public static class MailHelper
    {
        public static void SendMail(string email, string subject, string messageText)
        {
            var fromAddress = new MailAddress("Exchanger.confirmation@gmail.com", subject);
            var toAddress = new MailAddress(email);

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = messageText,
                IsBodyHtml = true
            };

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 20000,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("Exchanger.confirmation@gmail.com",
                    "049c4e2ce3cd4597b847ba472a147894")
            };

            smtp.Send(message);
        }
    }
}