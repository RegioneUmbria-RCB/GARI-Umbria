<%@ Page Title="Assegnazione lotti per fornitore, specie, varietà, qualità, certificazione" Language="vb" AutoEventWireup="false" CodeBehind="Lotto_AssegnaxRisumSpeVarQualCert.aspx.vb" Inherits="AgroAgenda_2010.Lotto_AssegnaxRisumSpeVarQualCert" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<%@ Register TagPrefix="ucLotto_Assegna" TagName="Lotto_AssegnazioneUC" Src="~/FreshAndFood/Lotti/UserControl/Lotto_AssegnazioneUC.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">
    .errorClass {
        border-color:#D41E1A;
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
        <ucLotto_Assegna:Lotto_AssegnazioneUC id="Lotto_AssegnazioneUC" Tipo_Lotto="E" runat="server" />
    </div>
 
    <!-- fine container -->
          
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hd_ID_fattore_variazione" runat="server" />
    <input type="hidden" id="hd_Id_listino_fattore_variaz" runat="server" />
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/template_comuni_dropdown.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("lotto_AssegnaxRisumSpeVarQualCert_jQueryDocReady.js") %>" ></script>

    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIDfattorevariazione = "#<%=hd_ID_fattore_variazione.ClientID() %>";
        var cIDlistinoFattoreVariaz = "#<%=hd_Id_listino_fattore_variaz.ClientID() %>";
    </script>
</asp:Content>