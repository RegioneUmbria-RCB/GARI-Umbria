<%@ Page Language="vb" AutoEventWireup="false"  MasterPageFile="~/Master/MasterConcimazione.Master" 
CodeBehind="Filtro_StampaScadenza.aspx.vb" Inherits="PianoConcimazione_2017.Filtro_StampaScadenza" %>

<%@ MasterType VirtualPath="~/Master/MasterConcimazione.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>Stampa</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <!-- TESTATA -->
        <div class="row" >
            <table class="boxColore" aria-hidden="true">
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
                        <asp:button ID="Btn_Stampa" CssClass="btn btn-primary" text="Stampa" runat="server" />
                    
                    </td>                                  
                </tr>
                  			                   
	        </table>
        </div>
        <div>
            <a id="LinkxIframe" href="" style="display: none">LinkxIframe</a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentScript" runat="server">
    <script>
    </script>
</asp:Content>