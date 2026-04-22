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
        //
        public string Password { get; set; }

        /// <summary>
        /// Создает и отправляет сообщение с кодом для проверки пользователя
        /// </summary>
        /// <param name="email">Email на который будет отправляться сообщение</param>
        /// <param name="code">Код, который используется для проверки пользователя</param>
        /// <param name="mailGoal">Цель сообщения, указыватся для темы письма</param>

        public static void CreateMail(string email, int code, string mailGoal)
        {
            var config = JsonSerializer.Deserialize<SendMail>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "config.json")));

            string nameMail = "";
            string mainSubject = "";

            switch (mailGoal)
            {
                case "forPass": nameMail = "Смена пароля"; mainSubject = "Код проверки для смены пароля";  break;
                case "forAny": nameMail = "Вход"; mainSubject = "Код проверки"; break;
                default: 
                    break;
            }

            MailAddress from = new MailAddress("kostyhomyakov0807@gmail.com", $"{nameMail}");
            MailAddress to = new MailAddress($"{email}");
            MailMessage mail = new MailMessage(from, to);

            mail.Subject = $"{mainSubject}";
            mail.Body = $"Код подтверждения: {code}";
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            
            smtp.Credentials = new NetworkCredential("kostyhomyakov0807@gmail.com", $"{config.Password}");
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}
