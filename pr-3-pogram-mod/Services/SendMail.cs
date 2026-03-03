using DotNetEnv;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace pr_3_pogram_mod.Services
{
    public class SendMail
    {
        public string Password { get; set; }

        public static void CreateMail(string email ,int code)
        {
            var config = JsonSerializer.Deserialize<SendMail>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "config.json")));

            MailAddress from = new MailAddress("kostyhomyakov0807@gmail.com", "Смена пароля");
            MailAddress to = new MailAddress($"{email}");
            MailMessage mail = new MailMessage(from,to);

            mail.Subject = "Код проверки для смены пароля";
            mail.Body = $"Код подтверждения:{code}";
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            
            smtp.Credentials = new NetworkCredential("kostyhomyakov0807@gmail.com", $"{config.Password}");
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}
