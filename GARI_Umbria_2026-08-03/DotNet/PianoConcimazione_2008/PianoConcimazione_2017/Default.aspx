<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Default.aspx.vb" Inherits="PianoConcimazione_2017._Default" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xml:lang="en">
	<head>
		<title>Piano Concimazione</title>
        <link href="/GiasBase/App_CSS/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
        <link rel="stylesheet" href="/GiasBase/agronica/Styles/Site.css" type="text/css" />
        <link rel="stylesheet" href="/GiasBase/agronica/Styles/bootstrap_AGRONICA.css" media="screen" />
	</head>
	<body>
    <form id="FORM1" runat="server">
    <table width="100%" aria-hidden="true">
        <tr>
            <td style="width: 100px">
                <asp:Image ID="Logo" Height="73px" Width="96px" ImageUrl="/GiasBase/AB_Immagini/logo/Logo_GiasOnline_Mini.jpg"
                    runat="server" />
            </td>
            <td>
                <div style="width: 100%; height: 32px;" class="ui-widget-header">
                    <div style="float: left; margin-left: 10px; font-size: 15px; margin-top: 5px;">
                        Piano Concimazione
                    </div>
                    <div style="float: right; height: 32px; margin-top: 2px; width: 32px; margin-right: 17px">
                    </div>
                </div>
                <div style="text-align: left; width: 100%; margin-top: 15px; height: 15px;" class="ui-widget-header">
                    <div style="float: left">
                        <span style="color: black;">Ultimo Aggiornamento&nbsp;:&nbsp;&nbsp;&nbsp;<asp:Label ID="lblUA" runat="server"></asp:Label></span>
                    </div>
                </div>
            </td>
            <td style="width: 50px">
            </td>
        </tr>
    </table>
    </form>
</body>	
</html>
