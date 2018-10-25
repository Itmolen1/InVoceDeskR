
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc.Attributes;


namespace InvoiceDiskLast.Controllers
{
    public class CaptchaController : Controller
    {       

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string empty)
        {

            // Code for validating the CAPTCHA 
            if (this.IsCaptchaValid("Captcha is not valid"))
            {

                ViewBag.ErrMessage = "Success valid";
                return View();

            }

            ViewBag.ErrMessage = "Error: captcha is not valid.";
            return View();

        }

        public string ThankYouPage()
        {
            return "Valid";

        }
    }
    
       
    }
