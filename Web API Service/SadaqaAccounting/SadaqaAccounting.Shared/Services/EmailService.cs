namespace SadaqaAccounting.Shared.Services
{
    public static class EmailService
    {
        public static async Task<bool> SendEmailAsync(EmailRequest emailRequest, IConfiguration config)
        {
            var smtpHost = config["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(config["EmailSettings:SmtpPort"]!);
            var fromEmail = config["EmailSettings:FromEmail"];
            var userName = config["EmailSettings:UserName"];
            var password = config["EmailSettings:Password"];

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(userName, password)
            };

            var mail = new MailMessage(fromEmail!, emailRequest.ToEmail)
            {
                Subject = emailRequest.Subject,
                Body = emailRequest.Body,
                IsBodyHtml = true
            };

            //Added the attachment
            if (emailRequest.Attachments != null)
            {
                foreach (var formFile in emailRequest.Attachments)
                {
                    if (formFile == null || formFile.Length == 0) continue;

                    var stream = new MemoryStream();
                    await formFile.CopyToAsync(stream);
                    stream.Position = 0;

                    string contentType = string.IsNullOrEmpty(formFile.ContentType)
                        ? "application/octet-stream"
                        : formFile.ContentType;

                    var attachment = new Attachment(stream, formFile.FileName, contentType);
                    mail.Attachments.Add(attachment);
                }
            }

            try
            {
                await client.SendMailAsync(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}