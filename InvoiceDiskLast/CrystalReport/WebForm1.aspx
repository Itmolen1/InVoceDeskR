<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="InvoiceDiskLast.CrystalReport.WebForm1" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>
<html>

<head runat="server">
    <title></title>
    <%--<script src="~/crystalreportviewers13/js/crviewer/crv.js"></script>
    --%>
</head>

<body style="background-color:darkgray">
    <form id="form1" runat="server">
        <div class="row">
            <div class="col-md-12">
                <table>
                    <tr>
                        <td>
                            <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="true"  ></CR:CrystalReportViewer>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
