<%@ Page Title="" Language="vb" AutoEventWireup="false" 
        MasterPageFile="~/Master/AgendaBootstrap_CtrlPnl.Master" 
        CodeBehind="PannelloDiControllo_Impostazioni.aspx.vb" 
        Inherits="AgroAgenda_2010.PannelloDiControllo_Impostazioni" %>
<%@ MasterType VirtualPath="~/Master/AgendaBootstrap_CtrlPnl.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent_CtrlPnl" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent_CtrlPnl" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript_CtrlPnl" runat="server">
    <script type="text/javascript">
        window.onload = function () {
            $('#TabMenu a[id="Timpostazioni"]').tab('show')
        };
    </script>
</asp:Content>
