<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Versione.aspx.vb" Inherits="AgronicaDomandaIrrigua.DomandaIrriguaVersione.Versione" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
    <head id="Head1" runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <title></title>
        <!--JS-->
        <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
        <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery-ui-1.8.4.custom.min.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
        <!-- localizzazione italiana per il datapicker -->
        <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.ui.datepicker-it.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
        <!-- script gestione delle date -->
        <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/confrontaDate.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
        <!-- widget personalizzato per le combo UI -->
        <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery-ui.combobox.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
        <!-- bubble popup -->
        <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.bubblepopup.v2.3.1.min.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
        <!-- funzioni prototipizzate per le stringhe -->
        <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/proto.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
        <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/Funzioni.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
        <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/FunzioniMie.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
        <!--STYLE-->
        <link href="./Styles/jquery-ui-1.8.4.custom.css" rel="Stylesheet" type="text/css" />
        <link href="./Styles/Site.css" rel="stylesheet" type="text/css" />
        <link href="./Styles/jquery.bubblepopup.v2.3.1.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="FORM1" runat="server">
            <!--Bottone per invocare il postback dal client-->
            <asp:Button ID="BTN_PostBack" runat="server" Style="display: none;" />
            <asp:HiddenField ID="ID_Elimina" runat="server" Value="none" />
            <!--per invocare direttamente degli script-->
            <asp:Literal ID="controllo_script" runat="server"></asp:Literal>
            <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="36000">
            </asp:ScriptManager>
            <div class="page">
                <!--TOP-->
                <div style="text-align: center; height: 80px;">
                    <table width="100%">
                        <tr>
                            <td style="width: 100px">
                                <asp:Image ID="Logo" Height="73px" Width="96px" ImageUrl="~/AB_Immagini/logo/Logo_GiasOnline_Mini.jpg"
                                    runat="server" />
                            </td>
                            <td>
                                <div style="width: 100%; height: 32px;" class="ui-widget-header">
                                    <div style="float: left; margin-left: 10px; font-size: 15px; margin-top: 5px;">
                                        Agronica Domanda Irrigua
                                    </div>
                                    <div style="float: right; height: 32px; margin-top: 2px; width: 32px; margin-right: 17px">
                                    </div>
                                </div>
                                <div style="text-align: left; width: 100%; margin-top: 15px; height: 15px;" class="ui-widget-header">
                                    <div style="float: left">
                                        Aggiornamenti ed implementazioni
                                        <div style="float: right">
                                        </div>
                                    </div>
                                </div>
                            </td>
                            <td style="width: 50px">
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="Pannello_Chiave" runat="server"></asp:Panel>
                    <asp:Label ID="lblChiaveAccesso" runat="server" Text="password:"></asp:Label>
                    <asp:TextBox ID="Txt_ChiaveAccesso" runat="server" TextMode="Password"></asp:TextBox>
                    <asp:Button ID="Btn_Codice_Ins" runat="server" Text="ok" />
                    <asp:Panel ID="Pannello_Versione" runat="server">
                        <table id="TabellaVersione" runat="server">
                        </table>
                    </asp:Panel>
                </div>
                <div class="clear">
                </div>
                <div class="clear">
                </div>
            </div>
        </form>
    </body>
</html>
