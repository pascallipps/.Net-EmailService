using Microsoft.AspNetCore.Http;
using MimeKit;
using System.Collections.Generic;
using System.Linq;

namespace EmailService
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Template { get; set; }
        public Dictionary<string, string> Placeholder { get; set; }

        public IFormFileCollection Attachments { get; set; }

        public Message(IEnumerable<string> to, string subject, string template = null, Dictionary<string, string> placeholder = null, IFormFileCollection attachments = null)
        {
            To = new List<MailboxAddress>();

            To.AddRange(to.Select(x => new MailboxAddress("email", x)));
            Subject = subject;
            Template = template;
            Placeholder = placeholder;
            //Content = content;
            Attachments = attachments;
        }
    }
}