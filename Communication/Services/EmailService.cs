using Communication.Interfaces;
using MailKit.Security;
using MimeKit;
using Models.ConfigModels;
using Models.ServiceRequest;
using MailKit.Net.Smtp;

namespace Communication.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        public EmailService(EmailConfig emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task<bool> SendEmailAsync(MailRequest mailRequest)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_emailConfig.Mail);

                if (!string.IsNullOrEmpty(mailRequest.ToEmail) && mailRequest.ToEmail.Contains(","))
                {
                    var mailList = mailRequest.ToEmail.Split(",");
                    var list = new InternetAddressList();
                    foreach (var item in mailList)
                    {
                        list.Add(MailboxAddress.Parse(item));
                    }
                    email.To.AddRange(list);
                }
                else
                {
                    email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                }
                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder();
                if (mailRequest.Attachments != null)
                {
                    byte[] fileBytes;
                    foreach (var file in mailRequest.Attachments)
                    {
                        if (file.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                fileBytes = ms.ToArray();
                            }
                            builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                        }
                    }
                }
                builder.HtmlBody = mailRequest.Body;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_emailConfig.Host, _emailConfig.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailConfig.Mail, _emailConfig.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
