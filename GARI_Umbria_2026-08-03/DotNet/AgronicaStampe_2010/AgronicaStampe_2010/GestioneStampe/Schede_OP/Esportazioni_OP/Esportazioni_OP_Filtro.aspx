<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Esportazioni_OP_Filtro.aspx.vb"
    Inherits="AgronicaStampe_2010.Esportazioni_OP_Filtro" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../../App_Styles/AgronicaStyle.css" rel="stylesheet" />
    <link href="../../../App_Styles/Site.min.css" rel="stylesheet" type="text/css" />
    <link href="../../../App_Styles/jquery-ui-1.8.16.custom.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="../../../APP_Scripts/jquery-1.4.1.min.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <script type="text/javascript" src="../../../APP_Scripts/min/jquery-ui-1.8.16.custom.min.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <script type="text/javascript">

        $(document).ready(function () {

            $('#<%=TxtValiditaInizio.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });

            $('.datepicker').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });
            $.datepicker.regional['it'];

        });

        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <table aria-hidden="true">
        <tr id="RigaIntestazione" runat="server">
            <td>
                <img alt="" src="../../../AB_Immagini/Logo/Logo_GiasOnline_Mini.jpg" />
            </td>
            <td style="width: 100%;">
                <table aria-hidden="true" id="TableTitolo" style="width: 100%;">
                    <tr>
                        <td>
                            <div style="width: 100%; height: 32px;" class="ui-widget-header">
                                <div style="float: left; margin-left: 10px; font-size: 15px; margin-top: 5px;">
                                    <asp:Label ID="LblTitolo" runat="server">Filtro Esportazioni OP</asp:Label>
                                </div>
                            </div>
                            <div style="text-align: left; width: 100%; margin-top: 15px; height: 15px;" class="ui-widget-header">
                                <div style="float: left">
                                    <asp:Label ID="Lbl_RagSoc" runat="server"></asp:Label></div>
                            </div>
                        </td>
                        <td style="width: 50px">
                            <asp:ImageButton ID="ImgBtnAnnullaTutto" runat="server" Height="32px" ImageUrl="../../../AB_Immagini/Icone32/Esci.bmp"
                                Width="32px" Style="margin-left: 10px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table aria-hidden="true">
        <tr id="riga_cod_unione" runat="server">
            <td>
                Codice Unione :
            </td>
            <td>
                <asp:TextBox ID="Txt_CodiceUnione" runat="server" CssClass="txtui" MaxLength="2"
                    Width="150px">
                </asp:TextBox>
            </td>
        </tr>
         <tr id="riga_istat" runat="server">
            <td>
                Codice ISTAT Regione OP :
            </td>
            <td>
                <asp:TextBox ID="Txt_Istat" runat="server" CssClass="txtui" MaxLength="3"
                    Width="150px">
                </asp:TextBox>
            </td>
        </tr >
           <tr id="riga_cuaa" runat="server">
            <td>
                CUAA OP :
            </td>
            <td>
                <asp:TextBox ID="Txt_Cuaa" runat="server" CssClass="txtui" MaxLength="16"
                    Width="150px">
                </asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                Data Riferimento Stampa :
            </td>
            <td>
                <asp:TextBox ID="TxtValiditaInizio" runat="server" CssClass="txtui datepicker" MaxLength="10"
                    Width="150px">
                </asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="Btn_Stampa" Style="margin-top: 5px; margin-left: 20px" runat="server"
                    Text="STAMPA" Width="80px" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
