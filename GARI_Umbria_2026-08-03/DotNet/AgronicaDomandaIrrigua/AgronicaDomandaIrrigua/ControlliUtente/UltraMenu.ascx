<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="UltraMenu.ascx.vb"
    Inherits="AgronicaDomandaIrrigua.UltraMenu" %>
<asp:Menu ID="UltraMenu" runat="server" BackColor="#E3EAEB" 
    CssClass="UltraMenu" DynamicHorizontalOffset="2" Font-Names="Verdana" 
    Font-Size="0.8em" ForeColor="#666666" StaticSubMenuIndent="10px" 
    Orientation="Horizontal" >
    <DynamicHoverStyle BackColor="#666666" ForeColor="White" />
    <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
    <DynamicMenuStyle CssClass="IE8Fix" BackColor="#E3EAEB" />
       <DynamicSelectedStyle BackColor="#1C5E55" />
       <Items>
        <asp:MenuItem ImageUrl="~/AB_Immagini/icone24/pagewarp24.ico">
            <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/impresa.ico" Text="Anagrafica e Catasto Aziendale"
                Value="Anagrafica e Catasto Aziendale"></asp:MenuItem>
            <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Stalla02.ico" Text="Gestione Impianti Zootecnici"
                Value="Gestione Impianti Zootecnici"></asp:MenuItem>
            <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Euro_01_32.ico" Text="Commercio Elettronico"
                Value="Commercio Elettronico"></asp:MenuItem>
            <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Agenda01.ico" Text="Agenda Op. Colturali e Zoo"
                Value="Agenda Op. Colturali e Zoo"></asp:MenuItem>
        </asp:MenuItem>
    </Items>
    <StaticHoverStyle BackColor="#666666" ForeColor="White" />
    <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
    <StaticSelectedStyle BackColor="#1C5E55" />
</asp:Menu>
