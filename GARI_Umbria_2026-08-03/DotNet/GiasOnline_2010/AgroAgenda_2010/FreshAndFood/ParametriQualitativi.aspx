<%@ Page Title="ParametriQualitativi" Language="vb" AutoEventWireup="false" CodeBehind="ParametriQualitativi.aspx.vb"
    Inherits="AgroAgenda_2010.ParametriQualitativi" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorClass {
            border-color: #D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />                              

   <div class="row">
       <h5 style="color: #428bca;">Moduli Disponibili</h5>
   </div> 

    <div class="row">
        
        <div class="jumbotron col-lg-2 col-md-2 col-sm-12">           
            <div id="treeview_ParametriQualitativi">
            </div>
        </div>

        <div class="col-lg-1 col-md-1 col-sm-12">
            &nbsp;
        </div>

        <div id="grid_Area_ParametriQualitativi" class="jumbotron col-lg-9 col-md-9 col-sm-12" style="display:none;">
            <div id="grid_Testata_ParametriQualitativi">
            </div>
        </div>
    </div>

    <input type="hidden" id="hdPiva" runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/template_comuni_dropdown.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/FreshAndFood/leggi_tabelle_FF_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ParametriQualitativi_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ParametriQualitativi.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ParametriQualitativi_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ParametriQualitativi_ws_client.js") %>"></script>
    <script type="text/javascript">
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
    </script>
</asp:Content>
