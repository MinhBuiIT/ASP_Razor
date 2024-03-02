using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Services;

class MailSenderService: IEmailSender {

    MailSetting _mailSetting;
    public MailSenderService(IOptions<MailSetting> mailSetting) {
        _mailSetting = mailSetting.Value;
    }
    public async Task SendEmailAsync (string email, string subject, string htmlMessage) {
        var message = new MimeMessage();

        message.Sender = new MailboxAddress(_mailSetting.DisplayName,_mailSetting.Mail);
        message.From.Add(new MailboxAddress(_mailSetting.DisplayName,_mailSetting.Mail));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;

        var builder = new BodyBuilder();
        //cấu hình loại body gửi mail
        builder.HtmlBody = htmlMessage;
        message.Body = builder.ToMessageBody();

        //Dùng smtp của MailKit
        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        try
        {
            smtp.Connect(_mailSetting.Host,_mailSetting.Port,SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSetting.Mail,_mailSetting.AppPassword);
            await smtp.SendAsync(message);
        }
        catch (System.Exception e)
        {
            //lưu lại nội dung gửi bị lỗi vào folder mailservice
            //Guid.NewGuid() sinh ra id
            if(!Directory.Exists("mailservice")) Directory.CreateDirectory("mailservice");
            var emailsavefile = string.Format("mailservice/{0}.eml",Guid.NewGuid());
            await message.WriteToAsync(emailsavefile);
            Console.WriteLine(e.Message);
        }
        
        await smtp.DisconnectAsync(true);
    }
}