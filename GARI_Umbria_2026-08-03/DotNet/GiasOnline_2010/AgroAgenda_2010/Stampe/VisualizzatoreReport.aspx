<%@ Page Language="vb" AutoEventWireup="false" ValidateRequest="false"  CodeBehind="VisualizzatoreReport.aspx.vb" Inherits="AgroAgenda_2010.VisualizzatoreReport" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <CR:CrystalReportViewer ID="CrystalReportViewer1" HasToggleParameterPanelButton="false" runat="server" AutoDataBind="true" PrintMode="Pdf" />
    </div>
    </form>
</body>
</html>
