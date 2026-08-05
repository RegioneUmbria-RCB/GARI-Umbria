<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" 
    CodeBehind="ConferimentoParametriQualitativi.aspx.vb" Inherits="AgroAgenda_2010.ConferimentoParametriQualitativi" %>


<%@ Register TagPrefix="ucParametriQualitativi" TagName="GruppiReferenzeUC" Src="~/GestioneContabilita/AnagrafeContabilita/ConferimentoParametriQualitativi/GruppiReferenzeUC.ascx" %>
<%@ Register TagPrefix="ucParametriQualitativi" TagName="ParametriQualitativiUC" Src="~/GestioneContabilita/AnagrafeContabilita/ConferimentoParametriQualitativi/ParametriQualitativiUC.ascx" %>
<%@ Register TagPrefix="ucParametriQualitativi" TagName="ParametriQualitativiXReferenzaUC" Src="~/GestioneContabilita/AnagrafeContabilita/ConferimentoParametriQualitativi/ParametriQualitativiXReferenzaUC.ascx" %>
<%@ Register TagPrefix="ucParametriQualitativi" TagName="ElencoValoriParametriQualitativiUC" Src="~/GestioneContabilita/AnagrafeContabilita/ConferimentoParametriQualitativi/ElencoValoriParametriQualitativiUC.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorClass {
            border-color: #D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;

       .blockModifica, .blockCancella, .blockDuplica {
        /* display: block; con questo non è possibile ridimensionare la colonna dei pulsanti */
        margin-top: 10px !important;
        margin-bottom: 10px !important;
    
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

<div class="row">
    <div class="col-lg-12 col-md-12 col-sm-12">
        <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
            <ul class="nav nav-tabs" role="tablist" id="tabs_Conferimento_ParametriQualitativi">
                <li class="active"><a href="#tabGruppiReferenze" data-toggle="tab" id="a_tabGruppiReferenze">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GruppiReferenze %>" runat="server">
                        Gruppi Referenze
                    </asp:Localize></a>
                </li>
                <li><a href="#tabParametriQualitativi" data-toggle="tab" id="a_tabParametriQualitativi">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ParametriQualitativi %>" runat="server">
                        Parametri Qualitativi
                    </asp:Localize></a>
                </li>
                <li><a href="#tabParametriQualitativiXReferenza" data-toggle="tab" id="a_tabParametriQualitativiXReferenza">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ParametriQualitativiXReferenza %>" runat="server">
                        Parametri Qualitativi Per Referenza
                    </asp:Localize></a>
                </li>
                <li><a href="#tabElencoValoriParametriQualitativi" data-toggle="tab" id="a_tabElencoValoriParametriQualitativi">
                    <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ElencoValoriParametriQualitativi %>" runat="server">
                        Elenco Valori Parametri Qualitativi
                    </asp:Localize></a>
                </li>
            </ul>
            
            <div class="tab-content">
                <!-- tab Gruppi Referenze -->
                <div class="tab-pane fade in" id="tabGruppiReferenze" style="overflow: auto; margin-bottom: 70px;">
                    <div class="row">
                        <ucParametriQualitativi:GruppiReferenzeUC id="GruppiReferenze" runat="server" />
                    </div>
                </div>
            </div>

            <div class="tab-content">
                <!-- tab Parametri Qualitativi -->
                <div class="tab-pane fade in active" id="tabParametriQualitativi" style="overflow: auto; margin-bottom: 70px;">
                    <div class="row">
                        <ucParametriQualitativi:ParametriQualitativiUC id="ParametriQualitativiUC" runat="server" />
                    </div>
                </div>
            </div>

            <div class="tab-content">
                <!-- tab ParametriQualitativiXReferenza -->
                <div class="tab-pane fade in" id="tabParametriQualitativiXReferenza" style="overflow: auto; margin-bottom: 70px;">
                    <div class="row">
                        <ucParametriQualitativi:ParametriQualitativiXReferenzaUC id="ParametriQualitativiXReferenzaUC" runat="server" />
                    </div>
                </div>
            </div>

            <div class="tab-content">
                <!-- tab Parametri -->
                <div class="tab-pane fade in" id="tabElencoValoriParametriQualitativi" style="overflow: auto; margin-bottom: 70px;">
                    <div class="row">
                        <ucParametriQualitativi:ElencoValoriParametriQualitativiUC id="ElencoValoriParametriQualitativiUC" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>  

<!-- fine container -->

<input type="hidden" id="hdPiva" runat="server" />
<input type="hidden" id="PathCoreWS" name="PathCoreWS"  runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/template_comuni_dropdown.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/FreshAndFood/leggi_tabelle_FF_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ConferimentoParametriQualitativi_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ConferimentoParametriQualitativi_ws_client.js") %>"></script> 
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../../UserControl/criteri_AggregazioneUC.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../../UserControl/criteri_Aggregazione_ws_clientUC.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("../../ScriptsGestionali/leggiTabelle_ws_client.js") %>"></script>
        <script type="text/javascript">
        var cIdPiva = "#<%=hdPiva.ClientID %>";
        var objP_server = '<%=objparametri_server_string %>';
        var objP_utenti = '<%=objparametri_utenti_string %>';
        var pathCoreWS = '<%=PathCoreWS.Value %>';
        </script>
</asp:Content>
