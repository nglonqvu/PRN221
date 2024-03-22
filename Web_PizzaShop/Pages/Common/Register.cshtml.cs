using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Mail;
using Web_PizzaShop.Models;


namespace Web_PizzaShop.Pages.Common
{
    public class RegisterModel : PageModel
    {
        private readonly PRN221_PRJContext _context;
        private const string sendingEmail = "longvu131102@gmail.com";
        private const string sendingEmailPassword = "ehcy ilad zkyt zhpm";
        private const string smtpServer = "smtp.gmail.com";

        public RegisterModel(PRN221_PRJContext context)
        {
            _context = context;
        }

        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync(string Email, string PhoneNumber, string Username, string PasswordHash)
        {
            var confirmToken = Guid.NewGuid().ToString();
            TempData["ConfirmToken"] = confirmToken;

            TempData["Email"] = Email;
            TempData["Username"] = Username;
            TempData["PhoneNumber"] = PhoneNumber;
            // TempData["PasswordHash"] = BCrypt.Net.BCrypt.HashPassword(PasswordHash);

            var confirmLink = $"{this.Request.Scheme}://{this.Request.Host}/Confirm?token={confirmToken}";

            try
            {
                await SendConfirmEmailAsync(Email, confirmLink);
                TempData["Message"] = "Đã gửi email xác nhận. Vui lòng kiểm tra hộp thư và xác nhận.";
                return RedirectToPage("/Confirm");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi email kích hoạt. Vui lòng thử lại.";
                return RedirectToPage("/Error");
            }
        }

        private async Task SendConfirmEmailAsync(string toEmail, string confirmLink)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(sendingEmail);
                mail.To.Add("somebody@domain.com");
                mail.Subject = "Hello World";
                mail.Body = "<h1>Hello</h1>";
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("email@gmail.com", "password");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }

            using (var client = new SmtpClient(smtpServer))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential(sendingEmail, sendingEmailPassword);
                client.EnableSsl = true;

                var mail = new MailMessage
                {
                    From = new MailAddress(sendingEmail),
                    Subject = "Kích hoạt tài khoản",
                    Body = $"Nhấn vào liên kết sau để kích hoạt tài khoản: {confirmLink}",
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);

                await client.SendMailAsync(mail);
            }
        }
    }
}
