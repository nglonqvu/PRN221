using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Mail;
using Web_PizzaShop.Interface.Public;
using Web_PizzaShop.Models;


namespace Web_PizzaShop.Pages.Common
{
    public class RegisterModel : PageModel
    {
        private readonly PRN221_PRJContext _context;
<<<<<<< Updated upstream
        private const string sendingEmail = "longvu131102@gmail.com";
        private const string sendingEmailPassword = "ehcy ilad zkyt zhpm";
=======
        private readonly IUserService _userService;
        private const string sendingEmail = "fcmonlineservice.noreply@gmail.com";
        private const string sendingEmailPassword = "lorywvszvjdjcefy";
>>>>>>> Stashed changes
        private const string smtpServer = "smtp.gmail.com";
        [BindProperty]
        public User user { get; set; }
        [BindProperty]
        public String message { get; set; }

        [BindProperty]
        public String passwordConfirm { get; set; }
        public RegisterModel(PRN221_PRJContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

<<<<<<< Updated upstream
        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync(string Email, string PhoneNumber, string Username, string PasswordHash)
=======
        public IActionResult OnGet()
>>>>>>> Stashed changes
        {
            return Page();
        }

<<<<<<< Updated upstream
            TempData["Email"] = Email;
            TempData["Username"] = Username;
            TempData["PhoneNumber"] = PhoneNumber;
            // TempData["PasswordHash"] = BCrypt.Net.BCrypt.HashPassword(PasswordHash);

            var confirmLink = $"{this.Request.Scheme}://{this.Request.Host}/Confirm?token={confirmToken}";

            try
            {
                await SendConfirmEmailAsync(Email, confirmLink);
=======
        public async Task<IActionResult> OnPostAsync()
        {
            if (!passwordConfirm.Equals(user.PasswordHash))
            {
                message = "Please enter password again";
                Console.WriteLine("dmmmm");
                return Page();
            }
            var check = _userService.Regis(user).Result;
            if (!check.Item1)
            {
                message = "Username or email has been existed";
                return Page();
            }

            //var confirmToken = Guid.NewGuid().ToString();
            //TempData["ConfirmToken"] = confirmToken;

            TempData["Email"] = user.Email;
            TempData["Username"] = user.UserName;
            TempData["PhoneNumber"] = user.PhoneNumber;
            //  TempData["PasswordHash"] = BCrypt.Net.BCrypt.HashPassword(PasswordHash);

            var confirmLink = $"https://localhost:7277/Common/MailConfirm?userId={check.Item2}";
            try
            {
                await SendConfirmEmail(user.Email, confirmLink);
>>>>>>> Stashed changes
                TempData["Message"] = "Đã gửi email xác nhận. Vui lòng kiểm tra hộp thư và xác nhận.";
                return RedirectToPage("/Confirm");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi email kích hoạt. Vui lòng thử lại.";
                return RedirectToPage("/Error");
            }

        }

<<<<<<< Updated upstream
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
=======
        private async Task SendConfirmEmail(string toEmail, string confirmLink)
        {
            try
            {
                using (var emailMessage = new MimeMessage())
                {
                    MailboxAddress emailFrom = new MailboxAddress(sendingEmail, sendingEmailPassword);
                    emailMessage.From.Add(emailFrom);
                    MailboxAddress mailTo = new MailboxAddress("Receiver", toEmail);
                    emailMessage.To.Add(mailTo);
                    emailMessage.Subject = "Confirm mail";
                    BodyBuilder bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = "Click this link to verify the account: " + confirmLink;
                    emailMessage.Body = bodyBuilder.ToMessageBody();
                    using (var mailClient = new MailKit.Net.Smtp.SmtpClient())
                    {
                        await mailClient.ConnectAsync(smtpServer, 587, MailKit.Security.SecureSocketOptions.StartTls);
                        await mailClient.AuthenticateAsync(sendingEmail, sendingEmailPassword);
                        await mailClient.SendAsync(emailMessage);
                        await mailClient.DisconnectAsync(true);
                    }
                }
                //using (var client = new SmtpClient(smtpServer))
                //{
                //    client.Port = 587;
                //    client.Credentials = new NetworkCredential(sendingEmail, sendingEmailPassword);
                //    client.EnableSsl = true;
                //    var mail = new MailMessage
                //    {
                //        From = new MailAddress(sendingEmail),
                //        Subject = "Kích ho?t tài kho?n",
                //        Body = $"Nh?n vào liên k?t sau ?? kích ho?t tài kho?n: {confirmLink}",
                //        IsBodyHtml = true
                //    };
                //    mail.To.Add(toEmail);
                //    client.Send(mail);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
>>>>>>> Stashed changes
            }
        }
    }
}
