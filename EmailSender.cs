using MailKit.Net.Smtp;
using MimeKit;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using ContentType = MimeKit.ContentType;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);

            Send(emailMessage);
        }

        public async Task SendEmailAsync(Message message)
        {
            var mailMessage = CreateEmailMessage(message);

            await SendAsync(mailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            //Add Header
            StreamReader strHeader = new StreamReader(Directory.GetCurrentDirectory() +_emailConfig.TemplatePath + _emailConfig.HeaderFile);
            string headerString = strHeader.ReadToEnd();
            strHeader.Close();

          

            //Add Content
            StreamReader strContent = new StreamReader(Directory.GetCurrentDirectory() + _emailConfig.TemplatePath + message.Template); 
            string contentString = strContent.ReadToEnd();
            strContent.Close();
            //Add Footer
            StreamReader strFooter = new StreamReader(Directory.GetCurrentDirectory() + _emailConfig.TemplatePath + _emailConfig.FooterFile);
            string MailText = headerString + contentString + strFooter.ReadToEnd();
            strFooter.Close();

            // Fill Template with Content 
            foreach ( var placeholder in message.Placeholder)
            {
                MailText = MailText.Replace("{{"+placeholder.Key+"}}", placeholder.Value);
            }

            var bodyBuilder = new BodyBuilder { HtmlBody = MailText };

            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }

                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception, or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception, or both.
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}