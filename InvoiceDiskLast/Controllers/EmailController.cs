using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        public ActionResult Index()
        {
            return View();
        }


        public static bool email(EmailModel emailmodel)
        {
            bool emailstatus = false;
            try
            {
                string bodyhtml = emailmodel.EmailBody;
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress(emailmodel.From);
                mail.To.Add(emailmodel.ToEmail);
                mail.Subject = "IT Molen Offer Letter";//emailModel.Subject;
                mail.Body = bodyhtml;
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(emailmodel.Attachment);
                mail.Attachments.Add(attachment);
                //Gmail Port
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("samarbudhni@gmail.com", "samar1234567");
                //You can specifiy below line of code either in web.config file or as below.
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                emailstatus = true;

            }

            catch (Exception)
            {

                emailstatus = false;

                throw;
            }

            return emailstatus;
        }
    }
}