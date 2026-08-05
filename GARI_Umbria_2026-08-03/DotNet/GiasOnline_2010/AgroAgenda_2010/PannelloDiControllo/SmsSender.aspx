<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="SmsSender.aspx.vb" Inherits="AgroAgenda_2010.SmsSender" %>

<%@ MasterType VirtualPath="~/Master/AgendaBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <h2>SMS Sender</h2>

    <asp:Panel ID="panCredenziali" Visible="false" runat="server">
        <p align="center" style="border: 1px solid black" title="Login">
            <br>
            <asp:Label ID="Label1" runat="server" Text="User ID"></asp:Label>
            <asp:TextBox ID="textUserID" runat="server"></asp:TextBox>
            <asp:Label ID="Label2" runat="server" Text="Password"></asp:Label>
            <asp:TextBox TextMode="Password" ID="textUserPassword" runat="server"></asp:TextBox>            
            <br>
            <br>
        </p>
    </asp:Panel>

    <asp:Panel ID="panSMS" Visible="false" runat="server">



        <p align="center" style="border: 1px solid black" title="Invio SMS">
            <br>
            <asp:Label ID="Labe20" runat="server" Text="Per l'invio multiplo i numeri devono essere separati da ';' e senza spazi"></asp:Label>
            <br>
            <br>
            <asp:Label ID="Label3" runat="server" Text="Destinatario/i"></asp:Label>
            <asp:TextBox ID="textPhoneNumber" runat="server">numero</asp:TextBox>
            <asp:Label ID="Label4" runat="server" Text="Mittente"></asp:Label>
            <asp:TextBox ID="textSenderNum" runat="server" MaxLength="11">nome</asp:TextBox>
            <br>
            <br>

            <asp:Label ID="Label5" runat="server" Text="Testo SMS"></asp:Label>
            <asp:TextBox ID="textSMS" runat="server" Height="64px" MaxLength="160"
                TextMode="MultiLine" Width="325px" Style="resize: none;"></asp:TextBox>
            <br>
            <br>

            <asp:Button ID="btnInviaSMS" runat="server" Text="Invia SMS" />
            <br>
            <br>
        </p>


    </asp:Panel>

    <asp:Panel ID="panSMScredito" runat="server" Visible="false">
        <p align="center" style="border: 1px solid black" title="Residui">
            <br>
            <asp:Button ID="btnVerificaCreditoResiduo" runat="server" Text="SMS Residui" />
            <asp:TextBox ID="textRemCredit" runat="server" ReadOnly="True" Width="150px"
                BackColor="#CCCCCC"></asp:TextBox>
            <br>
            <br>
        </p>
    </asp:Panel>

    <asp:Panel ID="panRisultato" runat="server" Visible="false">
        <asp:Label ID="Label6" runat="server" Text="Esito:"></asp:Label>
        <asp:TextBox ID="resultBox" runat="server" ReadOnly="True" Width="374px"
            BackColor="#CCCCCC"></asp:TextBox>
        <br>
        <br>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
</asp:Content>
