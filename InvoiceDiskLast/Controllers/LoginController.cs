using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class LoginController : Controller
    {

       
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModel loginmodel)
        {

           //GlobalVeriables.WebApiClient.PostAsJsonAsync("api/token",loginmodel).Result;

            return View();
        }


        public ActionResult RegisterSuccess()
        {
            return View();
        }

        public ActionResult  Verifiy(string Code)
        {
            TempData["Success"] = null;

            GlobalVeriables.WebApiClient.DefaultRequestHeaders.Clear();
            GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("Code", Code.ToString());
            AspNetUser productTable = new AspNetUser();
            HttpResponseMessage response = GlobalVeriables.WebApiClient.PutAsJsonAsync("ConfirmEmail/" + 122, productTable).Result;

            if(response.StatusCode== System.Net.HttpStatusCode.OK)
            {
                TempData["Success"] = "verified";
            }
            else
            {
                TempData["Success"] = "faild to verified";
            }

         

            return RedirectToAction("Index","Login");
           
        }

       
       [HttpGet]
        public ActionResult VerifyEmail(string Email)
        {
            try
            {
                string id = Email;


                MvcUserModel mvcuserModel = new MvcUserModel();

                GlobalVeriables.WebApiClient.DefaultRequestHeaders.Add("Email",Email.ToString());
                HttpResponseMessage response = GlobalVeriables.WebApiClient.GetAsync("ConfirmEmail/" + "ss").Result;
                mvcuserModel=response.Content.ReadAsAsync<MvcUserModel>().Result;

                string url = System.Configuration.ConfigurationManager.AppSettings["url"];
          
                string link = url + mvcuserModel.Id;

                sendMail(Email, mvcuserModel.Id, link);

                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
           

        }


        void sendMail(string ToEmail, string Code ,  string link )
        {
            #region formatter
            string text = string.Format("Please click on this link to {0}: {1}","", "Please verify your email to loogin");
            string html = "Please confirm your account by clicking this link: <a href=\"" + link + "\">link</a><br/>";

            html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + link);
            #endregion

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("joe@contoso.com");
            msg.To.Add(new MailAddress(ToEmail));
            msg.Subject = "Email Verification";
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("samarbudhni@gmail.com", "samar1234567");
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);
        }



        public ActionResult RegisterNew()
        {
            var id = Session["id"];
            return View();
        }

        [HttpPost]
        public ActionResult Logut()
        {
            Session.Clear();
            Session.Abandon();

            //return RedirectToAction("Index","Login");
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}