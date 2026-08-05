<%@ Page Title="Giacenze Magazzino" Language="vb" AutoEventWireup="false" CodeBehind="Giacenze_Magazzino.aspx.vb"
    Inherits="AgroAgenda_2010.Giacenze_Magazzino" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<%--<%@ Register TagPrefix="uc" TagName="LavorazioneMenuUC" Src="~/GestioneLavorazioni/LavorazioneMenuUC.ascx" %>--%>
<%@ Register TagPrefix="uc" TagName="Giacenze_MagazzinoUC" Src="./Giacenze_MagazzinoUC.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
     
    .errorClass {
        border-color:#D41E1A;
        border-width: 1px;
        border-style: dotted;
        background-color: Yellow;
    }

    /* if this class is applied to a Kendo UI widget, its layout may change */
.form-control, 
.container,
.container-fluid,
.row,
.col-xs-1, .col-sm-1, .col-md-1, .col-lg-1,
.col-xs-2, .col-sm-2, .col-md-2, .col-lg-2,
.col-xs-3, .col-sm-3, .col-md-3, .col-lg-3,
.col-xs-4, .col-sm-4, .col-md-4, .col-lg-4,
.col-xs-5, .col-sm-5, .col-md-5, .col-lg-5,
.col-xs-6, .col-sm-6, .col-md-6, .col-lg-6,
.col-xs-7, .col-sm-7, .col-md-7, .col-lg-7,
.col-xs-8, .col-sm-8, .col-md-8, .col-lg-8,
.col-xs-9, .col-sm-9, .col-md-9, .col-lg-9,
.col-xs-10, .col-sm-10, .col-md-10, .col-lg-10,
.col-xs-11, .col-sm-11, .col-md-11, .col-lg-11,
.col-xs-12, .col-sm-12, .col-md-12, .col-lg-12
{
    -webkit-box-sizing: border-box;
    -moz-box-sizing: border-box;
    box-sizing: border-box;
}
 
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
    <%--<uc:LavorazioneMenuUC id="LavorazioneMenuUC1" runat="server" />--%>

     <div id="panelArea" class="panel-group searchArea" style="opacity: 0; margin-top: 5px; margin-bottom: 70px;">
        <div class="row">                
            <div class="col-lg-12 col-md-12 col-sm-12">  
                <uc:Giacenze_MagazzinoUC id="Giacenze_MagazzinoUC" runat="server" />
            </div>
        </div>     
    </div>
    <!-- fine container -->
  
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdId_Agenda" runat="server" />
        <input type="hidden" id="hdKendo_Prodotti" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIdAgenda = "#<%=hdId_Agenda.ClientID() %>";
    </script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/template_comuni_dropdown.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("giacenze_magazzino_ws_client.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("giacenze_magazzino.js") %>" ></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("giacenze_magazzino_jQueryDocReady.js") %>" ></script>
       
</asp:Content>