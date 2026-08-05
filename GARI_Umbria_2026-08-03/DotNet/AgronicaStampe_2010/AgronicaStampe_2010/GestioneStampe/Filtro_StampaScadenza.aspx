
<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Stampe.Master" CodeBehind="Filtro_StampaScadenza.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_StampaScadenza" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="<%= ResolveClientUrl("../App_Scripts/fancybox/jquery.mousewheel-3.0.4.pack.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>
    <script src="<%= ResolveClientUrl("../App_Scripts/fancybox/jquery.fancybox-1.3.4.pack.js?" & Application("GiasVersioneCorrente").ToString) %>" type="text/javascript"></script>
    <link rel="stylesheet" href="<%= ResolveClientUrl("../App_Scripts/fancybox/jquery.fancybox-1.3.4.css") %>" />
    <link rel="stylesheet" href="<%= ResolveClientUrl("~/App_Styles/jquery-ui-1.8.16.custom.css") %>" />
    <script>
        function PostBackSlave() {
        }

    </script> 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentStampeContenuti" runat="server">
        <table aria-hidden="true" class="boxColore" >
            <tr>
                <td style=" width:300px;" >
                    <b style="margin-left: 5px;">Selezionare l'opzione di stampa</b>:
                    <br />
                    <asp:RadioButtonList ID="RblOpzioni" runat="server" CssClass="txtui">
                        <asp:ListItem Value="1" Selected="True">Stampa di prova</asp:ListItem>
                        <asp:ListItem Value="2">Stampa definitiva con scadenza</asp:ListItem>
                    </asp:RadioButtonList>                  
                </td>
                <td style=" width:300px; " align="center">
                    <asp:ImageButton ID="ImgBtn_Stampa" ImageUrl="~/AB_Immagini/Icone32/Stampa.ico" 
                        runat="server" />
                    
                </td>                                  
            </tr>
                  			                   
		</table>
    <div>
        <a id="LinkxIframe" href="" style="display: none">LinkxIframe</a>
    </div>
</asp:Content>