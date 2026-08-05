<%@ Page Title="Assegnazione prefissi e suffissi di numerazione" Language="vb" AutoEventWireup="false" CodeBehind="Numeratore_Tipo.aspx.vb"
    Inherits="AgroAgenda_2010.Numeratore_Tipo" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<%@ Register TagPrefix="uc1" TagName="NumeratoreTipo" Src="~/GestioneContabilita/UserControl/Numeratore_Tipo_UC.ascx" %>
<%@ Register TagPrefix="uc2" TagName="Numeratore_PrefissoSuffisso" Src="~/GestioneContabilita/UserControl/Numeratore_PrefissoSuffisso_UC.ascx" %>
<%@ Register TagPrefix="uc3" TagName="Numeratore_Defaults" Src="~/GestioneContabilita/UserControl/Numeratore_Default_UC.ascx" %>

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
        <div class="col-lg-12 col-md-12 col-sm-12">
            <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">

                <ul class="nav nav-tabs" role="tablist" id="tabs">
                    <li class="active"><a href="#tabNumeratoriTipo" data-toggle="tab" id="a_tabNumeratoriTipo">Numeratori Tipi</a></li>
                    <li><a href="#tabNumeratoriPS" data-toggle="tab" id="a_tabNumeratoriPS">Prefissi / Suffissi</a></li>
                    <li><a href="#tabNumeratoriDefault" data-toggle="tab" id="a_tabNumeratoriDefault">Defaults</a></li>
                </ul>

                <div class="tab-content">

                    <!-- tab Numeratori Tipo -->
                    <div class="tab-pane fade in active" id="tabNumeratoriTipo" style="overflow: auto; margin-bottom: 70px;">
                        <uc1:NumeratoreTipo id="numeratoreTipoUC" runat="server" />
                    </div>

                    <!-- tab Prefissi Suffissi -->
                    <div class="tab-pane fade in" id="tabNumeratoriPS" style="overflow: auto; margin-bottom: 70px;">
                        <uc2:Numeratore_PrefissoSuffisso id="numeratorePrefissoSuffisso" runat="server" />
                    </div>

                    <!-- tab Defaults -->
                    <div class="tab-pane fade in" id="tabNumeratoriDefault" style="overflow: auto; margin-bottom: 70px;">
                        <uc3:Numeratore_Defaults id="numeratoreDefaults" runat="server" />
                    </div>

                </div>

            </div>
        </div>
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
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("numeratore_Tipo_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("numeratore_Tipo.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("numeratore_Tipo_jQueryDocReady.js") %>"></script>

    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cIDfattorevariazione = "#<%=hd_ID_fattore_variazione.ClientID() %>";
        var cIDlistinoFattoreVariaz = "#<%=hd_Id_listino_fattore_variaz.ClientID() %>";
    </script>
</asp:Content>
