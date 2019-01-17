using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace InvoiceDiskLast.CrystalReport
{
    public partial class WebForm1 : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            //byte[] photo;

            //string path = HttpContext.Current.Server.MapPath("~/images/6.jpg");
            //photo = File.ReadAllBytes(path);

            try
            {
                CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                CustomerReport crystalReport = new CustomerReport();
                DBEntities entities = new DBEntities();



                //crystalReport.SetDatabaseLogon("ITMOLEN", "1234", @"CLIENT1\Database", "InvoiceDisk");
                crystalReport.SetDataSource(entities.ComapnyInfoes.Select(c => new
                {

                    CompanyID = c.CompanyID,
                    CompanyName = c.CompanyName,
                    CompanyPhone = c.CompanyPhone,
                    CompanyCell = c.CompanyCell,
                    CompanyEmail = c.CompanyEmail,
                    Website = c.Website,
                    CompanyTRN = c.CompanyTRN,
                    BankName = c.BankName,
                    IBANNumber = c.IBANNumber,
                    BIC = c.BIC,
                    KVK = c.KVK,
                    BTW = c.BTW,
                    CompanyAddress = c.CompanyAddress,
                    StreetNumber = c.StreetNumber,
                    PostalCode = c.PostalCode,
                    CompanyCity = c.CompanyCity,
                    CompanyCountry = c.CompanyCountry,
                    CompanyLogo = c.CompanyLogo,
                    

                }).ToList());

                crystalReport.SetDataSource(entities.ContactsTables.Select(d => new
                {
                    ContactName = d.ContactName,
                    ContactAddress = d.ContactAddress,
                    City = d.City,
                    PostalCode = d.PostalCode,
                    Land = d.Land,
                    StreetNumber = d.StreetNumber,

                }).ToList());

                // crystalReport.SetParameterValue("ReportHeading", "Qutation");
               
                CrystalReportViewer1.ReportSource = crystalReport;
                crystalReport.SetParameterValue("ReportHeading", "Quotation");
                CrystalReportViewer1.RefreshReport();
            }
            catch(Exception ex)
            {

            }
        }

      

    }

    
}

