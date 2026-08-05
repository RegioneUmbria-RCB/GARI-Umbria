<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="MenuAgenda_Menu.ascx.vb"
    Inherits="AgroAgenda_2010.MenuAgenda_Menu" %>
    

<asp:Menu ID="Menu" runat="server" StaticSubMenuIndent="10px" BackColor="#E3EAEB"
    DynamicHorizontalOffset="2" Font-Names="Verdana" Font-Size="1.2em" ForeColor="#666666"
    Orientation="Horizontal">
    <DynamicHoverStyle BackColor="#666666" ForeColor="White" />
    <DynamicMenuItemStyle HorizontalPadding="3px" VerticalPadding="2px" />
    <DynamicMenuStyle BackColor="#E3EAEB" />
    <DynamicSelectedStyle BackColor="#1C5E55" />
    <Items>
        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Informazioni.ico" Text="" Value="0"
            ToolTip="Informazioni"></asp:MenuItem>
        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Nuovo.ico" Text="" Value="1" ToolTip="Nuovo">
        </asp:MenuItem>
        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Modifica.ico" Text="" Value="2" ToolTip="Modifica">
        </asp:MenuItem>
        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Copia32.ico" Text="" Value="4" ToolTip="Copia">
        </asp:MenuItem>
        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Cestino.ico" Text="" Value="3" ToolTip="Elimina">
        </asp:MenuItem>
        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Agenda01.ico" Text="" Value="5" ToolTip="">
            <asp:MenuItem Text="Operazione Multi-Aziendale" Value="5a" ToolTip="Operazione Multi-Aziendale">
            </asp:MenuItem>
            <asp:MenuItem Text="Operazione Multi-Aziendale" Value="5b" ToolTip="Modifica le operazioni selezionate">
            </asp:MenuItem>
            <asp:MenuItem Text="Filtra e Modifica più operazioni contemporaneamente" Value="5c"
                ToolTip="Filtra e Modifica più operazioni contemporaneamente"></asp:MenuItem>
        </asp:MenuItem>
        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Esclamativo.ico" Text="" Value="6"
            ToolTip="">
            <asp:MenuItem Text="Visualizza Pianificazioni" Value="6a" ToolTip="Visualizza Pianificazioni">
            </asp:MenuItem>
            <asp:MenuItem Text="Crea Ricetta" Value="6b" ToolTip="Crea Ricetta"></asp:MenuItem>
        </asp:MenuItem>
        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Stampa.ico" Text="" Value="7" ToolTip="Stampa">
            <asp:MenuItem Text="Scheda Campagna" Value="7a" ToolTip="Scheda Campagna"></asp:MenuItem>
            <asp:MenuItem Text="Crea Ricetta" Value="7b" ToolTip="Global Gap"></asp:MenuItem>
            <asp:MenuItem Text="Crea Ricetta" Value="7c" ToolTip="Registro Fertilizzazioni">
            </asp:MenuItem>
        </asp:MenuItem>
        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/TrovaImprese.ico" Text="" Value="8"
            ToolTip="Trova Imprese"></asp:MenuItem>
        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/ImpresaPrecedente.ico" Text="" Value="9"
            ToolTip="Impresa Precedente"></asp:MenuItem>
        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Calendario04.ico" Text="" Value="10"
            ToolTip="Calendario">
            <asp:MenuItem Text="Visualizzazione Annuale" Value="10a" ToolTip="Visualizzazione Annuale">
            </asp:MenuItem>
            <asp:MenuItem Text="Visualizzazione Settimanale" Value="10b" ToolTip="Visualizzazione Settimanale">
            </asp:MenuItem>
        </asp:MenuItem>
    </Items>
    <StaticHoverStyle BackColor="#666666" ForeColor="White" />
    <StaticMenuItemStyle HorizontalPadding="3px" VerticalPadding="2px" />
    <StaticSelectedStyle BackColor="#1C5E55" />
</asp:Menu>
