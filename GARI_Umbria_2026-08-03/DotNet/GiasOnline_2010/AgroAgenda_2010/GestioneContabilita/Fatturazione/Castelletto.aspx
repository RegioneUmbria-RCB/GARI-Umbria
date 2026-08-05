<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Castelletto.aspx.vb"
    Inherits="AgroAgenda_2010.Castelletto" MasterPageFile="~/Master/AgendaBootstrap.Master" %>

<%@ Register TagPrefix="uc1" TagName="CastellettoUC" Src="./CastellettoUC.ascx" %>

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

            </div>

        </div>

        <uc1:CastellettoUC id="CastellettoUC1" runat="server" />

    </div>

    <%--<div class="btn btn-success" id="saveChanges">
        <span class="fa fa-save lampeggiante"></span><span class="lampeggiante">Salva</span>
    </div>--%>

    <!--dialogs varie-->

    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdPiva_Codificata" runat="server" />
    <input type="hidden" id="hdIdAgenda" runat="server" />
    <input type="hidden" id="hdLavCod" runat="server" />
    

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("Castelletto_jQueryDocReady.js") %>" ></script>

    <script type="text/javascript">

        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cPiva_Codificata = "#<%=hdPiva_Codificata.ClientID() %>";
        var cIdLavCod = parseInt($("#<%=hdLavCod.ClientID() %>").val());        
        var cIdAgenda = "#<%=hdIdAgenda.ClientID() %>";
        
    </script>

</asp:Content>
