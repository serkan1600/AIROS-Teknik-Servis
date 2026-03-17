using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AIROSWEB.Helpers
{
    public class MailHelper
    {
        public static bool SendMail(string toEmail, string subject, string body)
        {
            try
            {
                // SMTP Ayarları Web.config üzerinden okunacak (system.net/mailSettings)
                using (var smtp = new SmtpClient())
                {
                    using (var message = new MailMessage())
                    {
                        message.To.Add(toEmail);
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = true;
                        message.BodyEncoding = Encoding.UTF8;
                        
                        smtp.Send(message);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir
                return false;
            }
        }
    }
}
