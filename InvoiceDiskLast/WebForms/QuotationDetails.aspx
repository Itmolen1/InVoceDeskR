<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuotationDetails.aspx.cs" Inherits="InvoiceDiskLast.WebForms.QuotationDetails" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="true" />
        <img width="320" height="76" alt="Imagem" src="CrystalImageHandler.aspx?dynamicimage=cr_tmp_image_e47fba99-96fc-471b-ab11-06fd2212bbdd.png" border="0"/>
    </div>
    </form>
</body>
</html>
