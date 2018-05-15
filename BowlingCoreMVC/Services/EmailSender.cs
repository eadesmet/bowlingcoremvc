using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace BowlingCoreMVC.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public IConfiguration Configuration { get; set; }

        public EmailSender(IConfiguration config)
        {
            Configuration = config;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            //write my send email code here
            SmtpClient client = new SmtpClient();
            MailMessage MailMessage;// = new MailMessage();

            client.UseDefaultCredentials = false;

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                client.Port = Convert.ToInt32(Environment.GetEnvironmentVariable("GmailPort"));
                client.Host = Environment.GetEnvironmentVariable("GmailHost");
                client.Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("GmailUser"), Environment.GetEnvironmentVariable("GmailPass"));
                MailMessage = new MailMessage(Environment.GetEnvironmentVariable("GmailUser"), email, subject, message);
            }
            else
            {
                client.Port = Convert.ToInt32(Configuration["GmailPort"]);
                client.Host = Configuration["GmailHost"];
                client.Credentials = new NetworkCredential(Configuration["GmailUser"], Configuration["GmailPass"]);
                MailMessage = new MailMessage(Configuration["GmailUser"], email, subject, message);
            }
            
            client.EnableSsl = true;
            client.Timeout = 30000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            
            

            //var MailMessage = new MailMessage(Configuration["GmailUser"], email, subject, message);
            MailMessage.IsBodyHtml = true;
            
            //return Task.CompletedTask;
            return client.SendMailAsync(MailMessage);
        }
    }
}
