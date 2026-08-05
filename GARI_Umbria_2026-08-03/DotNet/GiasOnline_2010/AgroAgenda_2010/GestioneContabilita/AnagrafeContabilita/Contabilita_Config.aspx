<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="Contabilita_Config.aspx.vb" 
    Inherits="AgroAgenda_2010.Contabilita_Config" %>

<%@ Register TagPrefix="ucGestioneContabilita" TagName="ModalitaPagamentoUC" Src="~/GestioneContabilita/AnagrafeContabilita/ModalitaPagamento/ModalitaPagamentoUC.ascx" %>
<%@ Register TagPrefix="ucGestioneContabilita" TagName="CausaleTrasportoUC" Src="~/GestioneContabilita/AnagrafeContabilita/CausaleTrasporto/CausaleTrasportoUC.ascx" %>
<%@ Register TagPrefix="ucGestioneContabilita" TagName="IstitutiCreditoUC" Src="~/GestioneContabilita/AnagrafeContabilita/IstitutiCredito/IstitutiCreditoUC.ascx" %>
<%@ Register TagPrefix="ucGestioneContabilita" TagName="SezionaliUC" Src="~/GestioneContabilita/AnagrafeContabilita/Sezionali/SezionaliUC.ascx" %>
<%@ Register TagPrefix="ucGestioneContabilita" TagName="LiquiditaUC" Src="~/GestioneContabilita/AnagrafeContabilita/Liquidita/LiquiditaUC.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />

    <div class="row">
        <div class="col-lg-12 col-md-12 col-sm-12">
            <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">
                <ul class="nav nav-tabs" role="tablist" id="tabs_Anagrafiche_Contabilita">
                    <li class="active"><a href="#tabModalitaPagamento" data-toggle="tab" id="a_tabModalitaPagamento">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, ModalitaPagamento %>" runat="server">
                            Modalita Pagamento
                        </asp:Localize></a>
                    </li>
                    <li><a href="#tabCausaleTrasporto" data-toggle="tab" id="a_tabCausaleTrasporto">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, CausaleTrasporto %>" runat="server">
                            Causale Trasporto
                        </asp:Localize></a>
                    </li>
                    <li><a href="#tabSezionali" data-toggle="tab" id="a_tabSezionali">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, Sezionali %>" runat="server">
                            Sezionali
                        </asp:Localize></a>
                    </li>
                    <li><a href="#tabIstitutiCredito" data-toggle="tab" id="a_tabIstitutiCredito">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, IstitutiCredito %>" runat="server">
                            Istituti Credito
                        </asp:Localize></a>
                    </li>
                    <li><a href="#tabLiquidita" data-toggle="tab" id="a_tabLiquidita">
                        <asp:Localize Text="<%$ Resources: AgronicaAgenda_2010, GestioneContiCorrenti %>" runat="server">
                            Gestione Conti Correnti
                        </asp:Localize></a>
                    </li>
                </ul>
                
                <div class="tab-content">
                    <!-- tab Modalità Pagamento -->
                    <div class="tab-pane fade in" id="tabModalitaPagamento" style="overflow: auto; margin-bottom: 70px;">
                        <div class="row">
                            <ucGestioneContabilita:ModalitaPagamentoUC id="ModalitaPagamentoUC" runat="server" />
                        </div>
                    </div>
                </div>
                <div class="tab-content">
                    <!-- tab Istituti Credito -->
                    <div class="tab-pane fade in" id="tabIstitutiCredito" style="overflow: auto; margin-bottom: 70px;">
                        <div class="row">
                            <ucGestioneContabilita:IstitutiCreditoUC id="IstitutiCreditoUC" runat="server" />
                        </div>
                    </div>
                </div>
                <div class="tab-content">
                    <!-- tab Sezionali -->
                    <div class="tab-pane fade in" id="tabSezionali" style="overflow: auto; margin-bottom: 70px;">
                        <div class="row">
                            <ucGestioneContabilita:SezionaliUC id="SezionaliUC" runat="server" />
                        </div>
                    </div>
                </div>
                <div class="tab-content">
                    <!-- tab Causale Trasporto -->
                    <div class="tab-pane fade in" id="tabCausaleTrasporto" style="overflow: auto; margin-bottom: 70px;">
                        <div class="row">
                            <ucGestioneContabilita:CausaleTrasportoUC id="CausaleTrasportoUC" runat="server" />
                        </div>
                    </div>
                </div>
                <div class="tab-content">
                    <!-- tab Liquidita -->
                    <div class="tab-pane fade in" id="tabLiquidita" style="overflow: auto; margin-bottom: 70px;">
                        <div class="row">
                            <ucGestioneContabilita:LiquiditaUC id="LiquiditaUC" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>  

    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="PathCoreWS" name="PathCoreWS"  runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/template_comuni_dropdown.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/FreshAndFood/leggi_tabelle_FF_ws_client.js")) %>"></script>
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Contabilita_Config_jQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Contabilita_Config_ws_client.js") %>"></script> 

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
