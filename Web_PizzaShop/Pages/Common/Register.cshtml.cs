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
        private const string sendingEmail = "namtqhe173167@fpt.edu.vn";
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
          //  TempData["PasswordHash"] = BCrypt.Net.BCrypt.HashPassword(PasswordHash);

            var confirmLink = $"{this.Request.Scheme}://{this.Request.Host}/Confirm?token={confirmToken}";
            try
            {
                SendConfirmEmail(Email, confirmLink);
                TempData["Message"] = "?� g?i email x�c nh?n. Vui l�ng ki?m tra h?p th? v� x�c nh?n.";
                return RedirectToPage("/Confirm");
            }catch (Exception ex)
            {
                TempData["ErrorMessage"] = "C� l?i x?y ra khi g?i email k�ch ho?t. Vui l�ng th? l?i.";
                return RedirectToPage("/Error");
            }
        }

        private void SendConfirmEmail(string toEmail, string confirmLink)
        {
            using(var client = new SmtpClient(smtpServer))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential(sendingEmail, sendingEmailPassword);
                client.EnableSsl = true;
                var mail = new MailMessage
                {
                    From = new MailAddress(sendingEmail),
                    Subject = "K�ch ho?t t�i kho?n",
                    Body = $"Nh?n v�o li�n k?t sau ?? k�ch ho?t t�i kho?n: {confirmLink}",
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);
                client.Send(mail);
            }
        }
    }
}
