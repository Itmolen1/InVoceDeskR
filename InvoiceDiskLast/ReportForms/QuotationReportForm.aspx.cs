using InvoiceDiskLast.CrystalReport;
using InvoiceDiskLast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InvoiceDiskLast.ReportForms
{
    public partial class QuotationReportForm : System.Web.UI.Page
    {
        protected void Page_Load(int Id)
        {
            CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
          //  CrystalReport2 _ob = new CrystalReport2();
            DBEntities entities = new DBEntities();

            var Query = entities.QutationTables.Where(Q => Q.QutationID == Id).Select(c => new QuatationReportViewModel
            {
                QutationID = c.QutationID,
                Qutation_ID = c.Qutation_ID,
                QutationDate = c.QutationDate,
                DueDate = c.DueDate,
                RefNumber = c.RefNumber,
                CustomerNote = c.CustomerNote,
                Type = c.Type
                
            }).ToList();

        }
    }
}