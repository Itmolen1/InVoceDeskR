using CrystalDecisions.CrystalReports.Engine;
using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InvoiceDiskLast.WebForms
{
    public partial class QuotationDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

           
          DBEntities entity = new DBEntities();

          List<Comp> info = entity.ComapnyInfoes.Where(x=>x.CompanyID == 54).Select(c => new Comp
            {
                // Company Information   
                CompanyID = c.CompanyID ,
                CompanyTRN = c.CompanyTRN,
                CompanyName = c.CompanyName,
                CompanyAddress = c.CompanyAddress,
                CompanyPhone = c.CompanyPhone,
                CompanyCell = c.CompanyCell,
                CompanyEmail = c.CompanyEmail,
                CompanyLogo = "~/images/6.jpg",
                CompanyCity = c.CompanyCity,
                CompanyCountry = c.CompanyCountry,
                StreetNumber = c.StreetNumber,
                PostalCode = c.PostalCode,
                IBANNumber = c.IBANNumber,
                Website = c.Website,
                BIC = c.BIC,
                KVK = c.KVK,
                BTW = c.BTW,
                BankName = c.BankName,
                UserName = c.UserName,             

            }).ToList();

            List<Contacts> Contact = entity.ContactsTables.Where(x => x.ContactsId == 66).Select(c => new Contacts
            {
                ContactName = c.ContactName,
                ContactAddress = c.ContactAddress,
                ContactCity  = c.City,
                ContactLand = c.Land,
                ContactPostalCode = c.PostalCode,
                contactMobile = c.Mobile,
                Contacttelephone = c.telephone,
                ContactStreetNumber = c.StreetNumber,
            }).ToList();

            List<GoodsTable> goodsTable = entity.QutationDetailsTables.Where(x => x.QutationID == 46 && x.Type == "Goods").Select(x => new GoodsTable {
                 ProductName = x.ProductTable.ProductName,
                 Quantity = x.Quantity ??0,
                 Rate = x.Rate ??0,
                 Total = x.Total??0,
                 Vat = x.Vat ??0, 
                 RowSubTotal = x.RowSubTotal ?? 0,

           }).ToList();

            DateTime dt = DateTime.Today;

            List<ServicesTables> servicesTables = entity.QutationDetailsTables.Where(x => x.QutationID == 46 && x.Type == "Service").Select(x => new ServicesTables
            {             

                //Date = (x.ServiceDate!=null ? Convert.ToDateTime(x.ServiceDate).ToShortDateString(): DateTime.Today.ToShortDateString()),

                Date= "12/02/20118",
                ProductNames = x.ProductTable.ProductName,  
                Descriptions = x.Description,
                Quantitys = x.Quantity ?? 0,
                Rates = x.Rate ?? 0,
                Totals = x.Total ?? 0,
                Vats = x.Vat ?? 0,
                RowSubTotals = x.RowSubTotal ?? 0,

            }).ToList();

            List<QuotationReportModel> quotationReportModel = entity.QutationTables.Where(x => x.QutationID == 46).Select(x => new QuotationReportModel {

                QutationID = x.QutationID,
                 Qutation_ID =x.Qutation_ID,
                 RefNumber =x.RefNumber,
                 QutationDate = x.QutationDate,
                 DueDate = x.DueDate,
                 SubTotal = x.SubTotal ??0,
                 TotalVat6 = x.TotalVat6 ?? 0,
                 TotalVat21 = x.TotalVat21 ?? 0,
                 DiscountAmount = x.DiscountAmount ?? 0,
                 TotalAmount  = x.TotalAmount ?? 0,
                CustomerNote = x.CustomerNote,
                Status = x.Status

               }).ToList();

            ReportDocument Report = new ReportDocument();
            Report.Load(Server.MapPath("~/CrystalReport/QuotationDetails.rpt"));

           
            Report.Database.Tables[0].SetDataSource(info);
            Report.Database.Tables[1].SetDataSource(Contact);
            Report.Database.Tables[2].SetDataSource(goodsTable);
            Report.Database.Tables[3].SetDataSource(servicesTables);
            Report.Database.Tables[4].SetDataSource(quotationReportModel);

           

            CrystalReportViewer1.ReportSource = Report;
            CrystalReportViewer1.RefreshReport();





        }

    }
}
