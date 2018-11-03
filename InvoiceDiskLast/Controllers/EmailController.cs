using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    [SessionExpireAttribute]
    public class EmailController : Controller
    {
        // GET: Email
        public ActionResult Index()
        {
            return View();
        }

        public static void DeleteFiles(string path)
        {
            //string filePath = s.MapPath("~/Content/attachments");
            //Array.ForEach(Directory.GetFiles("~/PDF/10161 - My Company.pdf"), System.IO.File.Delete);
        }



        public static bool email(EmailModel emailmodel)
        {
            bool emailstatus = false;
            try
            {
                string bodyhtml = emailmodel.EmailBody;
                MailMessage mail = new MailMessage();
                //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                SmtpClient SmtpServer = new SmtpClient("smtp.uurtjefactuur.nl");               
                mail.From = new MailAddress(emailmodel.From);
                mail.To.Add(emailmodel.ToEmail);
                mail.Subject = "IT Molen";//emailModel.Subject;
                mail.Body = bodyhtml;
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(emailmodel.Attachment);
                mail.Attachments.Add(attachment);
                //Gmail Port
                SmtpServer.Port = 8889;
                SmtpServer.Credentials = new System.Net.NetworkCredential("info@uurtjefactuur.nl", "Itmolen@123");
                //You can specifiy below line of code either in web.config file or as below.
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                emailstatus = true;
                mail.Dispose();
               // DeleteFiles(emailmodel.Attachment);
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