<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="Documentazione.aspx.vb" Inherits="AgroAgenda_2010.Documentazione" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron" style="padding-top: 30px; margin-right: 15px; margin-left: 15px;">
        <asp:Panel runat="server" ID="pnlManuali"></asp:Panel>
        <asp:Panel runat="server" ID="pnlVideoCorsi"></asp:Panel>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
</asp:Content>
