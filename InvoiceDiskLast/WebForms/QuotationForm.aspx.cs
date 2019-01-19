using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;
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

            DBEntities _dbentities = new DBEntities();

            List<QuatationReportViewModel> QutationReportViewModel = new List<QuatationReportViewModel>();

            QutationReportViewModel = (from q in _dbentities.QutationTables
                                       join qt in _dbentities.QutationDetailsTables on q.QutationID equals qt.QutationID
                                       join c in _dbentities.ContactsTables on q.ContactId equals c.ContactsId
                                       join comp in _dbentities.ComapnyInfoes on q.CompanyId equals comp.CompanyID
                                       join p in _dbentities.ProductTables on qt.ItemId equals p.ProductId
                                       where q.QutationID == 46

                                       select new QuatationReportViewModel
                                       {
                                           // Contact Information
                                           ContactName = c.ContactName,
                                           ContactAddress = c.ContactAddress,
                                           City = c.City,
                                           Land = c.Land,
                                           PostalCode = c.PostalCode,
                                           Mobile = c.Mobile,
                                           telephone = c.telephone,
                                           ConatctStreetNumber = c.StreetNumber,
                                           // Company Information                           
                                           CompanyName = comp.CompanyName,
                                           CompanyAddress = comp.CompanyAddress,
                                           CompanyPhone = comp.CompanyPhone,
                                           CompanyCell = comp.CompanyCell,
                                           CompanyEmail = comp.CompanyEmail,
                                           CompanyLogo = "../images/" + comp.CompanyLogo,
                                           CompanyCity = comp.CompanyCity,
                                           CompanyCountry = comp.CompanyCountry,
                                           CompnayStreetNumber = c.StreetNumber,
                                           CompnayPostalCode = c.PostalCode,
                                           IBANNumber = comp.IBANNumber,
                                           WebSite = comp.Website,
                                           BIC = comp.BIC,
                                           KVK = comp.KVK,
                                           BTW = comp.BTW,
                                           BankName = comp.BankName,
                                           //qutation Tabel
                                           QutationID = q.QutationID,
                                           Qutation_ID = q.Qutation_ID,
                                           RefNumber = q.RefNumber,
                                           QutationDate = q.QutationDate,
                                           DueDate = q.DueDate,
                                           SubTotal = q.SubTotal ?? 0,
                                           TotalVat6 = q.TotalVat6 ?? 0,
                                           TotalVat21 = q.TotalVat21 ?? 0,

                                           TotalAmount = q.TotalAmount ?? 0,
                                           CustomerNote = q.CustomerNote,
                                           //Qutation Detail table
                                           Rate = qt.Rate ?? 0,
                                           Quantity = qt.Quantity ?? 0,
                                           Vat = qt.Vat ?? 0,
                                           Type = qt.Type,
                                           ItemName = p.ProductName,
                                           Total = qt.Total ?? 0,
                                           RowSubTotal = qt.RowSubTotal ?? 0,
                                           Description = qt.Description,

                                       }).ToList();


            //ReportDocument rd = new ReportDocument();

            //rd.Load(Path.Combine(Server.MapPath(")));

            //rd.SetDataSource(QutationReportViewModel);


            // return new FileStreamResult(stram, "application/pdf");

            ReportDocument Report = new ReportDocument();
            Report.Load(Server.MapPath("~/CrystalReport/Quotation.rpt"));
            Report.SetDataSource(QutationReportViewModel);
            //Report.SetDatabaseLogon("sa", "sa123", "Rakesh-PC", "RakeshData");
            CrystalReportViewer1.ReportSource = Report;




            //  CrystalReportViewer1.ReportSource = QutationReportViewModel;
        }

    }
}
