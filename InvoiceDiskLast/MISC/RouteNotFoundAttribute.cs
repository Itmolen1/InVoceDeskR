using InvoiceDiskLast.Models;
using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace InvoiceDiskLast.MISC
{
    public class RouteNotFoundAttribute : HandleErrorAttribute
    {
        Log lg  = new Log();
        public override void OnException(ExceptionContext filterContext)
        {

            ExceptionLogger logger = new ExceptionLogger()
            {
                ExceptionMessage = filterContext.Exception.Message,
                ControllerName = filterContext.RouteData.Values["controller"].ToString(),
                MethodName = filterContext.RouteData.Values["action"].ToString(),
                DateTime = DateTime.Now.ToShortDateString(),
                ExceptionStackTrace = filterContext.Exception.StackTrace.ToString().Substring(0, 400)

             };
                try
                {
                    HttpResponseMessage response = GlobalVeriables.WebApiClient.PostAsJsonAsync("PostErrorLog", logger ).Result;
                    ExceptionLogger exceptionLogger = response.Content.ReadAsAsync<ExceptionLogger>().Result;


                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        filterContext.ExceptionHandled = true;

                        filterContext.Result = new ViewResult()
                        {
                            ViewName = "Error"
                        };
                    }                   
                }
                catch (Exception ex)
                {
                    string Maaasge = "ExceptionInExceptionLog =" + ex.ToString() + "\n ExceptionMessage =" + logger.ExceptionMessage +"  " + " \n ExceptionStackTrace=" + logger.ExceptionStackTrace + "\n ControllerName " + logger.ControllerName + " \n MethodName " + logger.MethodName + "\n DateTime "+ logger.DateTime;

                    lg.LogException(Maaasge);

                    filterContext.Result = new ViewResult()
                    {
                        ViewName = "Error"
                    };

                }
           }
       }     
    }

