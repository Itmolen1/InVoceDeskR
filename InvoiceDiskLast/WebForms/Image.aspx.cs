using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace InvoiceDiskLast.WebForms
{
    public partial class Image : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
            
            
            //Report.SetDatabaseLogon("sa", "sa123", "Rakesh-PC", "RakeshData");
            //CrystalReportViewer1.ReportSource = Report;


            ReportDocument reportDocument = new ReportDocument();
            ParameterField paramField = new ParameterField();
            ParameterFields paramFields = new ParameterFields();
            ParameterDiscreteValue paramDiscreteValue = new ParameterDiscreteValue();

            paramField.Name = "ImageURL";
            var s =  Server.MapPath("~/images/6.jpg");
            paramDiscreteValue.Value = s;
            paramField.CurrentValues.Add(paramDiscreteValue);
            paramFields.Add(paramField);

           

            CrystalReportViewer1.ParameterFieldInfo = paramFields;
            reportDocument.Load(Server.MapPath("~/CrystalReport/Image.rpt"));
            CrystalReportViewer1.ReportSource = reportDocument;
            //reportDocument.SetDatabaseLogon("sa", "sa", "OPWFMS-7KYGZ7SB", "test");
        }
    }
}