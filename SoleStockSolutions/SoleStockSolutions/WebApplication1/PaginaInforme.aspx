<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaginaInforme.aspx.cs" Inherits="VisualNet.PaginaInforme" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="robots" content="noindex,nofollow"/>
    <title>Informe</title>
</head>
<body style="height:100%">
    <div id="divError" runat="server" visible="false">
        <h1>Error:</h1>
        <h2 id="txtError" runat="server"></h2>
        <h3 id="txtError2" runat="server"></h3>
    </div>
    <form id="form1" runat="server" style="height:100%">
        <div id="div1" style="height:100vh">
            <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeOut="3600"></asp:ScriptManager>
            <rsweb:ReportViewer runat="server" ID="ReportViewer" ShowExportControls="true" ShowPrintButton="true" Width="100%" Heigth="100%" SizeToReportContent="true"></rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
