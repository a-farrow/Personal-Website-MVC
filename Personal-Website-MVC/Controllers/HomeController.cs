using Microsoft.AspNetCore.Mvc;
using Personal_Website_MVC.Models;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using MimeKit; //Added for access to MimeMessage class
using MailKit.Net.Smtp; //Added for access to SmtpClient class

namespace Personal_Website_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CredentialSettings _credentials;

        public HomeController(ILogger<HomeController> logger, IOptions<CredentialSettings> credentials)
        {
            _logger = logger;
            _credentials = credentials.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel cvm)
        {
            string message = $"You have received a new email from your site's contact form!<br/>" +
                $"Sender: {cvm.Name}<br/>Email: {cvm.Email}<br/>Subject: {cvm.Subject}<br/>" +
                $"Message: {cvm.Message}";

            var mm = new MimeMessage();

            mm.From.Add(new MailboxAddress("Sender", _credentials.Email.Username));
            mm.To.Add(new MailboxAddress("Personal", _credentials.Email.Recipient));
            mm.Subject = cvm.Subject;
            mm.Body = new TextPart("HTML") { Text = message };
            mm.Priority = MessagePriority.Urgent;
            mm.ReplyTo.Add(new MailboxAddress("User", cvm.Email));


            using (var client = new SmtpClient())
            {
                client.Connect(_credentials.Email.Server, 8889);
                client.Authenticate(_credentials.Email.Username, _credentials.Email.Password);
                try
                {
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"There was an error processing your request. Please " +
                        $"try again later.<br/>Error Message: {ex.StackTrace}";
                    return View(cvm);
                }
            }

            return View("EmailConfirmation", cvm);
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult PortfolioOverview()
        {
            return View();
        }


    }
}
