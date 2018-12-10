using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

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

        // E:\InvoiceDeskR\InvoiceDiskLast\PDF\Journal-2018-11-19-2018-11-21

        public static bool email(EmailModel emailmodel, List<AttakmentList> _StringList)
        {
            bool emailstatus = false;
            try
            {

                string bodyhtml = emailmodel.EmailBody;
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
                mail.From = new MailAddress("infouurtjefactuur@gmail.com");
                mail.From = new MailAddress(emailmodel.From);
                mail.To.Add(emailmodel.ToEmail);

                if (mail.Subject == null || mail.Subject == "")
                {
                    mail.Subject = "IT Molen";//emailModel.Subject;
                }
                mail.IsBodyHtml = true;

                mail.Body = bodyhtml;


                if (_StringList != null && _StringList.Count() > 0)
                {
                    foreach (var item in _StringList)
                    {
                        string attt = item.Attckment;

                        mail.Attachments.Add(new System.Net.Mail.Attachment(item.Attckment.ToString()));

                    }
                }
                else
                {

                    if (emailmodel.Attachment != null)
                    {
                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(emailmodel.Attachment);
                        mail.Attachments.Add(attachment);

                    }
                }
                //Gmail Port
                //SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("infouurtjefactuur@gmail.com", "samar12345");
                //You can specifiy below line of code either in web.config file or as below.
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                emailstatus = true;
                mail.Dispose();
                //DeleteFiles(emailmodel.Attachment);
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