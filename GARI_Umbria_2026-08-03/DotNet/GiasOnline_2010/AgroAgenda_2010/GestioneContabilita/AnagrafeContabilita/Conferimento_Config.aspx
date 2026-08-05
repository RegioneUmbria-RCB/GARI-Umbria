<%@ Page Title="Configurazione Conferimenti" Language="vb" AutoEventWireup="false" CodeBehind="Conferimento_Config.aspx.vb" 
    Inherits="AgroAgenda_2010.Conferimento_Config" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<%@ Register TagPrefix="ucGestioneLotti" TagName="AttivazioneModuliUC" Src="~/GestioneContabilita/AnagrafeContabilita/ConferimentoConfig/AttivazioneModuliUC.ascx" %>
<%@ Register TagPrefix="ucGestioneLotti" TagName="ConfigurazioneUC" Src="~/GestioneContabilita/AnagrafeContabilita/ConferimentoConfig/ParamEntrataXSpecieVarietaUC.ascx" %>
<%@ Register TagPrefix="ucGestioneLotti" TagName="AssegnaLottiUC" Src="~/GestioneContabilita/AnagrafeContabilita/ConferimentoConfig/AssegnazioneLottoConferimentoUC.ascx" %>
<%@ Register TagPrefix="ucGestioneLotti" TagName="CriteriAggregazioneUC" Src="~/GestioneContabilita/AnagrafeContabilita/ConferimentoConfig/Lotto_AssegnaxRisumSpeVarQualCertUC.ascx" %>

<%@ Register TagPrefix="ucGestioneCelle" TagName="GestioneRepartiPianiUC" Src="~/GestioneContabilita/AnagrafeContabilita/ConferimentoCelle/GestioneRepartiPianiUC.ascx" %>
<%@ Register TagPrefix="ucGestioneCelle" TagName="GestioneCelleUC" Src="~/GestioneContabilita/AnagrafeContabilita/ConferimentoCelle/GestioneCelleUC.ascx" %>

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
                <ul class="nav nav-tabs" role="tablist" id="tabs_Conferimento_Config">
                    <li class="active"><a href="#tabAttivazioneModuli" data-toggle="tab" id="a_tabAttivazioneModuli">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AttivazioneModuli %>" runat="server">
                            Attivazione Moduli
                        </asp:Localize></a>
                    </li>
                    <li><a href="#tabConfigurazioneConferimenti" data-toggle="tab" id="a_tabConfigurazioneConferimenti">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ConfigurazioneConferimenti %>" runat="server">
                            Configurazione Conferimenti
                        </asp:Localize></a>
                    </li>
                    <li><a href="#tabAssegna_Lotti" data-toggle="tab" id="a_tabAssegna_Lotti">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, AssegnazioneLotto %>" runat="server">
                            Assegnazione Lotto
                        </asp:Localize></a>
                    </li>
                    <li><a href="#tabCriteri_Aggregazione" data-toggle="tab" id="a_tabCriteri_Aggregazione">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, LottoXFornitoreProdotto %>" runat="server">
                            Lotto per fornitore-prodotto
                        </asp:Localize></a>
                    </li>
                    <li><a href="#tabGestione_Reparti_Piani" data-toggle="tab" id="a_tabGestione_Reparti_Piani">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GestioneRepartiPiani %>" runat="server">
                            Gestione Reparti e Piani
                        </asp:Localize></a>
                    </li>
                    <li><a href="#tabGestione_Celle" data-toggle="tab" id="a_tabGestione_Celle">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GestioneCelle %>" runat="server">
                            Gestione Celle
                        </asp:Localize></a>
                    </li>
                </ul>
                
                <div class="tab-content">
                    <!-- tab Attivazione Moduli -->
                    <div class="tab-pane fade in" id="tabAttivazioneModuli" style="overflow: auto; margin-bottom: 70px;">
                        <div class="row">
                            <ucGestioneLotti:AttivazioneModuliUC id="AttivazioneModuliUC" runat="server" />
                        </div>
                    </div>
                </div>

                <div class="tab-content">
                    <!-- tab Configurazione Conferimenti -->
                    <div class="tab-pane fade in active" id="tabConfigurazioneConferimenti" style="overflow: auto; margin-bottom: 70px;">
                        <div class="row">
                            <ucGestioneLotti:ConfigurazioneUC id="Configurazione" runat="server" />
                        </div>
                    </div>
                </div>

                <div class="tab-content">
                    <!-- tab Assegnazione Lotto -->
                    <div class="tab-pane fade in" id="tabAssegna_Lotti" style="overflow: auto; margin-bottom: 70px;">
                        <div class="row">
                            <ucGestioneLotti:AssegnaLottiUC id="AssegnaLotti" runat="server" />
                        </div>
                    </div>
                </div>

                <div class="tab-content">
                    <!-- tab Criteri Aggregazione -->
                    <div class="tab-pane fade in" id="tabCriteri_Aggregazione" style="overflow: auto; margin-bottom: 70px;">
                        <div class="row">
                            <ucGestioneLotti:CriteriAggregazioneUC id="CriteriAggregazione" runat="server" />
                        </div>
                    </div>
                </div>

                <div class="tab-content">
                    <!-- tab Gestione Reparti Piani -->
                    <div class="tab-pane fade in" id="tabGestione_Reparti_Piani" style="overflow: auto; margin-bottom: 70px;">
                        <div class="row">
                            <ucGestioneCelle:GestioneRepartiPianiUC id="GestioneRepartiPiani" runat="server" />
                        </div>
                    </div>
                </div>
                
                <div class="tab-content">
                    <!-- tab Gestione Celle -->
                    <div class="tab-pane fade in" id="tabGestione_Celle" style="overflow: auto; margin-bottom: 70px;">
                        <div class="row">
                            <ucGestioneCelle:GestioneCelleUC id="GestioneCelle" runat="server" />
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

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Conferimento_Config_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Conferimento_Config_ws_client.js") %>"></script> 

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
