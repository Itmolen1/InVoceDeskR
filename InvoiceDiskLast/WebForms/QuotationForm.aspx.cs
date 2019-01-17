using CrystalDecisions.CrystalReports.Engine;
using InvoiceDiskLast.CrystalReport;
using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InvoiceDiskLast.WebForms
{
    public partial class QuotationForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
            Quotation Report = new Quotation();
            DBEntities entities = new DBEntities();


            string str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            SqlConnection con = new SqlConnection(str);
            SqlDataAdapter da = new SqlDataAdapter("SP_GetQutationDetails",con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

           da.SelectCommand.Parameters.AddWithValue("@QuatationID",44);

            DataSet ds = new DataSet();
            da.Fill(ds);

            //cmd.ExecuteNonQuery();



            Report.SetDataSource(ds);



         //   entities.QutationDetails(45).ToList();



            //Report.SetDataSource(entities.ComapnyInfoes.Select(c => new
            //{
            //    CompanyID = c.CompanyID,
            //    CompanyName = c.CompanyName,
            //    CompanyPhone = c.CompanyPhone,
            //    CompanyCell = c.CompanyCell,
            //    CompanyEmail = c.CompanyEmail,
            //    Website = c.Website,
            //    CompanyTRN = c.CompanyTRN,
            //    BankName = c.BankName,
            //    IBANNumber = c.IBANNumber,
            //    BIC = c.BIC,
            //    KVK = c.KVK,
            //    BTW = c.BTW,
            //    CompanyAddress = c.CompanyAddress,
            //    StreetNumber = c.StreetNumber,
            //    PostalCode = c.PostalCode,
            //    CompanyCity = c.CompanyCity,
            //    CompanyCountry = c.CompanyCountry,
            //    CompanyLogo = c.CompanyLogo,

            //}).ToList());


            //Report.SetDataSource(entities.ContactsTables.Select(c => new 
            //{
            //    ContactsId = "1",
            //    ContactName = "fda",
            //    ContactAddress = "fda",
            //    Company_Id = "fda",
            //    UserId = "fda",
            //    Type = "fda",
            //    StreetNumber = "fda",
            //    PostalCode = "fda",
            //    City = "fda",              
            //    Land = "fda",
            //    LandLine = "fda",
            //    telephone = "fda",
            //    Mobile = "fda",
            //    BillingEmail = "fda",
            //    Website = "fda",
            //    PersonCompany = "fda",
            //    Remarks = "fda",
            //    Addeddate = "fda",
            //    Status = "fda",
            //    PaymentTerm = "fda",

            //}).ToList());
            //Report.Load(Path.Combine(Server.MapPath("~/CrystalReport/Quotation.rpt")));

            CrystalReportViewer1.ReportSource = Report;
           // Report.SetParameterValue("ReportHeading", "Quotation");
            CrystalReportViewer1.RefreshReport();

        }
    }
}