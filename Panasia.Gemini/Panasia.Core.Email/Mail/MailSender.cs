using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Panasia.Core;

namespace Panasia.Core.Email
{
    public static class MailSender
    { 
        public static void Send(this SendItem item)
        {
            switch (item.Host.Type)
            {
                case HostType.Local:
                    SendLocal(item);
                    break;
                case HostType.Smtp:
                    SendSmtpServer(item);
                    break;
                case HostType.Exchange:
                default:
                    SendExchange(item);
                    break;
            }
        } 

        public static IEnumerable<EmailItem> CreateItems(this string email)
        {
            foreach (var item in email.Split(','))
            {
                yield return new EmailItem { Email = item.Trim() };
            }
        }

        public static void SendExchange(this SendItem item)
        {
            System.Net.Mail.MailMessage msg = GetMailMessage(item);
            SmtpClient client = new SmtpClient(item.Host.Address);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials =(item.Credentials.UserID.Length > 0)?( new System.Net.NetworkCredential(item.Credentials.UserID, item.Credentials.Password)):
            System.Net.CredentialCache.DefaultNetworkCredentials;
            client.EnableSsl = item.Host.EnableSsl;
            client.Send(msg);
        }

        static System.Net.Mail.MailMessage GetMailMessage(this SendItem item)
        {            
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            foreach (EmailItem email in item.To)
            {
                msg.To.Add(email.Email);
            }
            foreach (EmailItem email in item.Cc)
            {
                msg.CC.Add(email.Email);
            }
            foreach (EmailItem email in item.BCc)
            {
                msg.Bcc.Add(email.Email);
            }

            if (item.From.Count > 0)
            {
                msg.From = new System.Net.Mail.MailAddress(item.From[0].Email, item.From[0].Name, System.Text.Encoding.GetEncoding(item.From[0].Encoding));
            }
            msg.Subject = item.Subject.Content;
            msg.SubjectEncoding = System.Text.Encoding.GetEncoding(item.Subject.Encoding);
            msg.Body = item.Body.Content;
            msg.BodyEncoding =System.Text.Encoding.GetEncoding( item.Body.Encoding);
            msg.IsBodyHtml = item.Body.IsBodyHtml;
            msg.Priority = item.Priority;
            return msg;
        }

        public static void SendLocal(this SendItem item)
        {
            System.Net.Mail.MailMessage msg = GetMailMessage(item);

            SmtpClient client = new SmtpClient();
            client.Host = item.Host.Address;
            object userstate = msg;

            client.Send(msg);
        }

        public static void SendSmtpServer(this SendItem item)
        {
            System.Net.Mail.MailMessage msg = GetMailMessage(item);

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(item.Credentials.UserID, item.Credentials.Password);
            client.Host = item.Host.Address;
            client.Port = Convert.ToInt32(item.Host.Port);
            client.EnableSsl = item.Host.EnableSsl;
            object userstate = msg;
            client.Send(msg);
        }
    }
}