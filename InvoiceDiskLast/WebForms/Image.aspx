<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Image.aspx.cs" Inherits="InvoiceDiskLast.WebForms.Image" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       
      
        <cr:crystalreportviewer ID="CrystalReportViewer1" runat="server" autodatabind="true"></cr:crystalreportviewer>
    </div>
    </form>
</body>
</html>
