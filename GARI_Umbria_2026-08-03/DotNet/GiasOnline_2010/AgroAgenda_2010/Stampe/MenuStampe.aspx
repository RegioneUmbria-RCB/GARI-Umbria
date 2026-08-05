<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Agenda.Master"
    CodeBehind="MenuStampe.aspx.vb" Inherits="AgroAgenda_2010.MenuStampe" %>

<asp:Content ID="Head" ContentPlaceHolderID="ContentAgendaHead" runat="server">
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.cookie.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.hotkeys.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.jstree.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <style type="text/css">
        .bordomenu
        {
            border-bottom-color: #444444;
            border-bottom-width: 1px;
            border-bottom-style: solid;
        }
    </style>
    <script type="text/javascript">


        //Per invocare il postback sulle combo
        function DoPostBack_Combo($_combo, valoreOpt) {

            //postBack Categoria
            if ($_combo.attr("id").endsWith("cmb_Stampe")) {

                $("#<%=BTN_ComboStampe.ClientID %>").click();
                $('#WaitFrame').show();
                return;
            }


        }


        $(document).ready(function () {

            $('input:submit').button();

            $('.menuASPX').find('a').addClass("btn_per_load");
        });


    </script>
    <style type="text/css">
        .txtUI
        {
            border: 1px solid #8DB9DB;
            padding: 0.1 0 0 0.1em;
            font-size: 1em;
            margin-bottom: 5px;
            margin-left: 0;
            margin-right: 0;
            margin-top: 0;
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="Contenuti" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <div class="main" id="main" style="overflow: auto; width: 100%; height: 100%">
        <!--Toolbar -->
        <div style="width: 100%; height: 100%">
            <asp:UpdatePanel runat="server" ID="UpdatePanel_Filtri" UpdateMode="Conditional"
                style="width: 100%; height: 100%">
                <ContentTemplate>
                    <div style="overflow: auto; width: 100%; height: 100%">
                        <!--BOX SX-->
                        <div class="" style="float: left; width: 380px; padding-left: 20px; height: 100%">
                            <div style="width: 100%; padding: 5px; height: 100%">
                                <div style="min-width: 100%" runat="server">
                                    <div class="descrizione">
                                        <asp:Label ID="lblStampa" runat="server">Ricerca Rapida:</asp:Label>
                                    </div>
                                    <div class="valoriinput">
                                        <asp:DropDownList ID="cmb_Stampe" runat="server" CssClass="txtUI" AutoPostBack="True">
                                        </asp:DropDownList>
                                        <asp:Button ID="BTN_ComboStampe" runat="server" Text="Button" Style="display: none" />
                                    </div>
                                </div>
                                <div class="clear2" style="width: 100%;">
                                </div>
                                <div style="width: 100%; padding-top: 10px; padding-bottom: 15px;">
                                    <asp:Label ID="LabelStampa" Visible="true" runat="server" Text="Seleziona una Stampa dal menu a tendina"
                                        Width="100%" Style="text-align: center"></asp:Label>
                                </div>
                                <div class="clear2" style="width: 100%;">
                                </div>
                                <div id="DivAggiungi" class="box50">
                                    <div class="descrizione">
                                        Aggiungi ai preferiti:
                                    </div>
                                    <div>
                                        <asp:ImageButton ID="ImageButton_Inserisci" CssClass="btn_per_load" runat="server"
                                            ImageUrl="../AB_Immagini/icone32/FrecciaDN.ico" Style="width: 40px" />
                                    </div>
                                </div>
                                <div id="DivStampa" class="box50">
                                    <div class="descrizione">
                                        Avvia la Stampa:
                                    </div>
                                    <div>
                                        <asp:ImageButton ID="ImgBtn_Stampa" CssClass="btn_per_load" runat="server" ImageUrl="../AB_Immagini/icone32/stampa.ico"
                                            Style="width: 40px" />
                                    </div>
                                </div>
                                <div class="clear2" style="width: 100%;">
                                </div>
                                <div style="width: 100%; padding-top: 15px; padding-bottom: 5px;">
                                    <asp:Label ID="LabelOperazionePref" Visible="True" runat="server" Text="Stampe Preferite:"
                                        Width="100%" Style="text-align: center"></asp:Label>
                                </div>
                            </div>
                            <div style="width: 100%; padding: 0px; height: 100%">
                                <asp:Button ID="BtnOperazionePref1" ToolTip="Lancia una nuova stampa" runat="server"
                                    Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                    Width="100%" />
                                <asp:Button ID="BtnOperazionePref2" ToolTip="Lancia una nuova stampa" runat="server"
                                    Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                    Width="100%" />
                                <asp:Button ID="BtnOperazionePref3" ToolTip="Lancia una nuova stampa" runat="server"
                                    Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                    Width="100%" />
                                <asp:Button ID="BtnOperazionePref4" ToolTip="Lancia una nuova stampa" runat="server"
                                    Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                    Width="100%" />
                                <asp:Button ID="BtnOperazionePref5" ToolTip="Lancia una nuova stampa" runat="server"
                                    Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                    Width="100%" />
                                <asp:Button ID="BtnOperazionePref6" ToolTip="Lancia una nuova stampa" runat="server"
                                    Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                    Width="100%" />
                                <asp:Button ID="BtnOperazionePref7" ToolTip="Lancia una nuova stampa" runat="server"
                                    Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                    Width="100%" />
                                <asp:Button ID="BtnOperazionePref8" ToolTip="Lancia una nuova stampa" runat="server"
                                    Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                    Width="100%" />
                                <asp:Button ID="BtnOperazionePref9" ToolTip="Lancia una nuova stampa" runat="server"
                                    Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                    Width="100%" />
                                <asp:Button ID="BtnOperazionePref10" ToolTip="Lancia una nuova stampa" runat="server"
                                    Visible="False" Text="Nuova1" CssClass="style2" Style="background-color: #E6F4FF"
                                    Width="100%" />
                            </div>
                            <div style="width: 100%; padding: 5px; height: 100%">
                            <div>
                            <asp:Button id="MenuStampe_OLD" Text="VAI AL MENU' STAMPE VS. PRECEDENTE"  runat="server" CssClass="btn_per_load" />
                                        
                                    </div>
                            </div>
                        </div>
                        <div class="" id="Div1" style="float: left; width: 700px; padding-left: 20px; height: 700px">
                            <div style="width: 100%; height: 100%">
                                <asp:UpdatePanel ID="UpdatePanel_Menu" runat="server" style="position: relative;
                                    padding: 10px;">
                                    <ContentTemplate>
                                        <asp:Menu ID="MenuStampe" runat="server" CssClass="menuASPX" StaticSubMenuIndent="10px" BackColor="#E6F4FF"
                                            DynamicHorizontalOffset="2" Font-Names="Verdana" Font-Size="12px" ForeColor="#666666"
                                            Orientation="Vertical"  >
                                            
                                            <DynamicHoverStyle BackColor="#BFE4FF" ForeColor="White" CssClass="menuASPX" />
                                            <DynamicMenuItemStyle HorizontalPadding="3px" VerticalPadding="10px" Font-Size="14px"
                                                CssClass="menuASPX bordomenu" />
                                            <DynamicMenuStyle BackColor="#E6F4FF" CssClass="menuASPX" />
                                            <DynamicSelectedStyle BackColor="#99D3FF" ForeColor="#444444" Font-Bold="True" CssClass="menuASPX" />
                                            <Items>
                                                <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Progetto_Interventi32.ico" Text="Schede di Campagna"
                                                    Value="A2" ToolTip="Schede di Campagna">
                                                    <asp:MenuItem Text="Scheda di Campagna Multicentro" Value="150" ToolTip="Scheda di Campagna Multicentro">
                                                    </asp:MenuItem>
                                                </asp:MenuItem>
                                            </Items>
                                            <StaticHoverStyle BackColor="#666666" ForeColor="White" />
                                            <StaticMenuItemStyle HorizontalPadding="3px" VerticalPadding="2px" />
                                            <StaticSelectedStyle BackColor="#1C5E55" />
                                        </asp:Menu>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div class="clear2" style="width: 100%;">
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="clear2" style="width: 100%;">
        </div>
    </div>
</asp:Content>
