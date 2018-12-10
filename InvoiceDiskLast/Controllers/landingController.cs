using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.Controllers
{
    public class landingController : Controller
    {
        List<AttakmentList> _attacklist = new List<AttakmentList>();

        // GET: landing
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult FeedBack(Feedback feedBackModel)
        {
            try
            {
                EmailModel emailmodel = new EmailModel();
                // emailmodel.ToEmail = feedBackModel;
                DateTime message = DateTime.Now;
                emailmodel.ToEmail = "infouurtjefactuur@gmail.com";
                emailmodel.From = "infouurtjefactuur@gmail.com";
                emailmodel.Subject = feedBackModel.Subject;
                string htmlString = @"<html>
                      <body>
                      <p>Dear Ms. Susan,</p>
                       <p>" + feedBackModel.Name + "</p>" +
                       "<p>" + feedBackModel.Email + "</p>" +
                       "<p>" + feedBackModel.Message + "</p>" +
                      "</body>" +
                      "</html>";
                emailmodel.EmailBody = htmlString;
                EmailController.email(emailmodel, _attacklist);

                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)

            {
                throw ex;
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
        }
    }
}