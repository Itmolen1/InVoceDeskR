using CrystalDecisions.CrystalReports.Engine;
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
    public partial class QuotationDetails : System.Web.UI.Page
    {


        public Byte[] GetImageBytes(string image_name)
        {
            Byte[] bytes = null;
            if (!string.IsNullOrEmpty(image_name))
            {


                //
                if (System.IO.File.Exists(image_name))
                {
                    FileStream fs = new FileStream(image_name, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    bytes = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
                }
            }
            return bytes;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            string d = HttpContext.Current.Server.MapPath("/images/" + "6.jpg");
            DataSet Ds = new DataSet();
            DataTable dt = new DataTable();
            // object of data row 
            DataRow drow;
            // add the column in table to store the image of Byte array type 
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("CompanyLogo", typeof(byte[]));
            
            drow = dt.NewRow();
            // define the filestream object to read the image 
            FileStream fs;
            // define te binary reader to read the bytes of image 
            BinaryReader br;
            // check the existance of image 

            // open image in file stream 
            fs = new FileStream(d, FileMode.Open);

            // initialise the binary reader from file streamobject 
            br = new BinaryReader(fs);
            // define the byte array of filelength 
            byte[] imgbyte = new byte[fs.Length + 1];
            // read the bytes from the binary reader 
            imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
            drow[0] = 122;
            drow[1] = imgbyte;
            // add the image in bytearray 
            dt.Rows.Add(drow);
            // add row into the datatable 
            br.Close();
            // close the binary reader 
            fs.Close();
            // close the file stream 
           
           
            DBEntities entity = new DBEntities();

            List<Comp> info = entity.ComapnyInfoes.Where(x => x.CompanyID == 54).Select(c => new Comp
            {
                // Company Information   
                CompanyID = c.CompanyID,
                CompanyTRN = c.CompanyTRN,
                CompanyName = c.CompanyName,
                CompanyAddress = c.CompanyAddress,
                CompanyPhone = c.CompanyPhone,
                CompanyCell = c.CompanyCell,
                CompanyEmail = c.CompanyEmail,
                CompanyLogo = d.ToString(),
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

            List<ImageModel> Model = new List<ImageModel>();

            string Pag = HttpContext.Current.Server.MapPath("/images/" + "8.jpg");

            Model.Add(new ImageModel { imgdata = GetImageBytes(Pag) });



            List<Contacts> Contact = entity.ContactsTables.Where(x => x.ContactsId == 66).Select(c => new Contacts
            {
                ContactName = c.ContactName,
                ContactAddress = c.ContactAddress,
                ContactCity = c.City,
                ContactLand = c.Land,
                ContactPostalCode = c.PostalCode,
                contactMobile = c.Mobile,
                Contacttelephone = c.telephone,
                ContactStreetNumber = c.StreetNumber,
            }).ToList();

            List<GoodsTable> goodsTable = entity.QutationDetailsTables.Where(x => x.QutationID == 46 && x.Type == "Goods").Select(x => new GoodsTable
            {
                ProductName = x.ProductTable.ProductName,
                Quantity = x.Quantity ?? 0,
                Rate = x.Rate ?? 0,
                Total = x.Total ?? 0,
                Vat = x.Vat ?? 0,
                RowSubTotal = x.RowSubTotal ?? 0,

            }).ToList();

            DateTime dt22 = DateTime.Today;


            List<ServicesTables> servicesTabless = entity.QutationDetailsTables.Where(x => x.QutationID == 46 && x.Type == "Service").Select(x => new ServicesTables
            {

                Date = x.ServiceDate.ToString(),
                ProductNames = x.ProductTable.ProductName,
                Descriptions = x.Description,
                Quantitys = x.Quantity ?? 0,
                Rates = x.Rate ?? 0,
                Totals = x.Total ?? 0,
                Vats = x.Vat ?? 0,
                RowSubTotals = x.RowSubTotal ?? 0,

            }).ToList();


            List<ServicesTables> servicesTables = new List<ServicesTables>();

            foreach (var x in servicesTabless)
            {
                ServicesTables Serv = new ServicesTables();

                DateTime dtt = Convert.ToDateTime(x.Date);

                Serv.Date = dtt.ToShortDateString();
                Serv.ProductNames = x.ProductNames;
                Serv.Descriptions = x.Descriptions;
                Serv.Quantitys = x.Quantitys;
                Serv.Rates = x.Rates;
                Serv.Totals = x.Totals;
                Serv.Vats = x.Vats;
                Serv.RowSubTotals = x.RowSubTotals;

                servicesTables.Add(Serv);

            }


            List<QuotationReportModel> quotationReportModels = entity.QutationTables.Where(x => x.QutationID == 46).Select(x => new QuotationReportModel
            {

                QutationID = x.QutationID,
                Qutation_ID = x.Qutation_ID,
                RefNumber = x.RefNumber,
                QutationDate = x.QutationDate.ToString(),
                DueDate = x.DueDate.ToString(),
                SubTotal = x.SubTotal ?? 0,
                TotalVat6 = x.TotalVat6 ?? 0,
                TotalVat21 = x.TotalVat21 ?? 0,
                DiscountAmount = x.DiscountAmount ?? 0,
                TotalAmount = x.TotalAmount ?? 0,
                CustomerNote = x.CustomerNote,
                Status = x.Status

            }).ToList();


            List<QuotationReportModel> quotationReportModel = new List<QuotationReportModel>();

            foreach (var x in quotationReportModels)
            {
                QuotationReportModel qut = new QuotationReportModel();


                //DateTime QT = Convert.ToDateTime(x.QutationDate);
                //  DateTime DQT = Convert.ToDateTime(x.DueDate);

                qut.QutationID = x.QutationID;
                qut.Qutation_ID = x.Qutation_ID;
                qut.RefNumber = x.RefNumber;
                //qut.QutationDate = QT.ToShortDateString();
                //qut.DueDate = DQT.ToShortDateString();
                qut.SubTotal = x.SubTotal;
                qut.TotalVat6 = x.TotalVat6;
                qut.TotalVat21 = x.TotalVat21;
                qut.DiscountAmount = x.DiscountAmount;
                qut.TotalAmount = x.TotalAmount;
                qut.CustomerNote = x.CustomerNote;
                qut.Status = x.Status;
                quotationReportModel.Add(qut);
            }


            ReportDocument Report = new ReportDocument();
            Report.Load(Server.MapPath("~/CrystalReport/CrystalReport1.rpt"));

            //   Report.Database.Tables[0].SetDataSource(Model);

            Report.SetDataSource(dt);
            // set the datasource of crystalreport object 
            CrystalReportViewer1.ReportSource = Report;
            //set the report source 
            //Report.Database.Tables[1].SetDataSource(Contact);
            //Report.Database.Tables[2].SetDataSource(goodsTable);
            //Report.Database.Tables[3].SetDataSource(servicesTables);
            //Report.Database.Tables[4].SetDataSource(quotationReportModel);
            // Report.Database.Tables[5].SetDataSource(Model);
            CrystalReportViewer1.ReportSource = Report;
            CrystalReportViewer1.RefreshReport();
        }
    }
}
