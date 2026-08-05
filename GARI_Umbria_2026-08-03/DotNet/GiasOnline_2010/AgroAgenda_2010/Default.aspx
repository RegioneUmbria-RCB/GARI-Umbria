<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="AgroAgenda_2010._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="<%= ResolveUrl("~/Styles/jquery-ui-1.8.4.custom.css") %>" rel="Stylesheet" type="text/css" />
    <link href="<%= ResolveUrl("~/Styles/Site.css") %>" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="FORM1" runat="server">
    <!--Bottone per invocare il postback dal client-->
    <asp:Button ID="BTN_PostBack" runat="server" Style="display: none;" />
    <asp:HiddenField ID="ID_Elimina" runat="server" Value="none" />
    <!--per invoare direttamente degli script-->
    <asp:Literal ID="controllo_script" runat="server"></asp:Literal>
    <asp:ScriptManager ID="ScriptManager1"  runat="server" AsyncPostBackTimeout="36000" >
    </asp:ScriptManager>
    <div class="page">
        <!--TOP-->
        <div style="text-align: center; height: 80px;">
            <table width="100%" aria-hidden="true">
                <tr>
                    <td style="width: 100px">
                        <asp:Image ID="Logo" Height="73px" Width="96px" ImageUrl="~/AB_Immagini/logo/Logo_GiasOnline_Mini.jpg"
                            runat="server" />
                    </td>
                    <td>
                        <div style="width: 100%; height: 32px;" class="ui-widget-header">
                            <div style="float: left; margin-left: 10px; font-size: 15px; margin-top: 5px;">
                                Agronica Agenda 2010
                            </div>
                            <div style="float: right; height: 32px; margin-top: 2px; width: 32px; margin-right: 17px">
                            </div>
                        </div>
                        <div style="text-align: left; width: 100%; margin-top: 15px; height: 15px;" class="ui-widget-header">
                            <div style="float: left">
                               
                                <asp:Label ID="lblUA" runat="server"></asp:Label>
                                <div style="float: right">

                                </div>
                            </div>
                        </div>
                    </td>
                    <td style="width: 50px">
                    </td>
                </tr>
            </table>
        </div>
        <div class="clear">
        </div>
        <div class="clear">
        </div>
    </div>
    </form>
</body>
</html>
