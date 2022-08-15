﻿namespace EmailService
{
    public class EmailConfiguration
    {
        public string FromName { get; set; }
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TemplatePath { get; set; }
        public string HeaderFile { get; set; }
        public string FooterFile { get; set; }
    }
}