using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using WatchNotes.Models;

namespace WatchNotes.Controllers
{
    public class HomeController : Controller
    {
        private readonly MailSettings _settings;

        public HomeController(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        

        [HttpPost]
        public ActionResult NoteForm(Note note)
        {
           SendMail(note.Content);
           return View("Index");
        }

        public  void SendMail(string message)
        {
            var smtpClient = new SmtpClient(_settings.Host)
            {
                Port = _settings.Port,
                Credentials = new NetworkCredential(_settings.Login, _settings.Password),
                EnableSsl = true,
            };

            smtpClient.Send(_settings.Login, _settings.TickTickMail, $"{message} {_settings.DateSurfix}", message);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
