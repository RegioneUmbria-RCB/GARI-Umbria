<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ImputazioneImpianti.aspx.vb"
    Inherits="AgroAgenda_2010.ImputazioneImpianti" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<%@ Register TagPrefix="uc1" TagName="ImputazioneImpiantiUC" Src="./UserControl/ImputazioneImpiantiUC.ascx" %>
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

    <div id="panelArea" class="panel-group searchArea" style="opacity: 1;">
        <div class="panel-group preArea">
            <div class="panel-body" style="padding-top: 30px;">
                <div class="row">
                    <div class="btn btn-success" id="btn_salva">
                        <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva</span>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%--<div class="btn btn-success" id="saveChanges">
        <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva</span>
    </div>--%>

    <!--dialogs varie-->
    <div id="confermaEliminazioneDialog"></div>

    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdPiva_Codificata" runat="server" />
    <input type="hidden" id="hdPiva_Scarico" runat="server" />    
    <input type="hidden" id="hdIdAgenda" runat="server" />    
    <input type="hidden" id="hdCod_Risum" runat="server" />
    
    <input type="hidden" id="hdKendo_risultatiLettura" runat="server" />
    <input type="hidden" id="hdPaginaRedirect" runat="server" />
    <input type="hidden" id="hdPaginaRedirect_Codificata" runat="server" />

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("ImputazioneImpianti.js") %>" ></script>

    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cPiva = "#<%=hdPiva.ClientID() %>";     
        var cIdAgenda = "#<%=hdIdAgenda.ClientID() %>";     
        var cPiva_Scarico = "#<%=hdPiva_Scarico.ClientID() %>";     
        var cKendo_risultatiLettura = "#<%=hdKendo_risultatiLettura.ClientID() %>";
        var paginaRedirect = "#<%=hdPaginaRedirect.ClientID() %>";
    </script>

</asp:Content>
