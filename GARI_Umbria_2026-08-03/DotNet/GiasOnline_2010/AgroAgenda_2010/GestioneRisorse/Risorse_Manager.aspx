<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Agenda.Master"
    CodeBehind="Risorse_Manager.aspx.vb" Inherits="AgroAgenda_2010.Risorse_Manager" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentAgendaHead" runat="server">
    <script type="text/javascript">


        function apriFormDialog(url, w, h) {
            //alert("ciao");
            $.logThis("apriFormDialog(w: " + w + ", h: " + h + ")");

            if (w != 0) {
                $('#dialog').dialog({
                    autoOpen: false,
                    height: h,
                    width: w,
                    maxHeight: $(window).height() - 30,
                    maxWidth: $(window).width() - 60,
                    modal: true
                });
            }

            $("#dialog").dialog("open");

            $("#xiframe").html("<iframe frameBorder='0' src='" + url + "' style='width:100%; height:" + ($("#dialog").height()) + "px'><iframe>");
        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <asp:UpdatePanel ID="Update_Risorse" runat="server">
        <ContentTemplate>
            <div class="main  box" id="Div3" style="min-width: 1000px; margin-left: 15px; margin-right: 15px;">
                <asp:UpdatePanel ID="UpdatePanel_Menu" runat="server" style="position: relative;">
                    <ContentTemplate>
                        <div style="width: 100%;">
                            <div style="width: 100%; margin-bottom: 10px; background-color: #E6F4FF;">
                                <asp:Menu ID="MenuAgenda" SkipLinkText="" runat="server" StaticSubMenuIndent="10px"
                                    BackColor="#E6F4FF" DynamicHorizontalOffset="2" Font-Names="Verdana" Font-Size="1.5em"
                                    ForeColor="#666666" Orientation="Horizontal" CssClass="btn_per_load">
                                    <DynamicHoverStyle BackColor="#BFE4FF" ForeColor="White" CssClass="btn_per_load" />
                                    <DynamicMenuItemStyle HorizontalPadding="3px" VerticalPadding="10px" Font-Size="14px"
                                        CssClass="btn_per_load bordomenu" />
                                    <DynamicMenuStyle BackColor="#E6F4FF" CssClass="btn_per_load" />
                                    <DynamicSelectedStyle BackColor="#99D3FF" ForeColor="#444444" Font-Bold="True" CssClass="btn_per_load" />
                                    <Items>
                                        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Informazioni.ico" Value="0" ToolTip="Informazioni sul Movimento Selezionato"
                                            Text=""></asp:MenuItem>
                                        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Nuovo.ico" Value="1" ToolTip="Carico"
                                            Text=""></asp:MenuItem>
                                        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Modifica.ico" Value="2" ToolTip="Modifica Movimento"
                                            Text=""></asp:MenuItem>
                                        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Cestino.ico" Value="3" ToolTip="Elimina Movimento"
                                            Text=""></asp:MenuItem>
                                        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Stampa.ico" Value="7" ToolTip="Stampa"
                                            meta:resourcekey="MenuItemResource17" Text="">
                                        <asp:MenuItem Text="Scheda Giacenze" Value="7t" ToolTip="Esportazione Excel Anagrafica Prodotti">
                                            </asp:MenuItem>
                                            <asp:MenuItem Text="Scheda Giacenze" Value="7p" ToolTip="Stampa la Scheda Giacenze">
                                            </asp:MenuItem>
                                            <asp:MenuItem Text="Movimenti Magazzino" Value="7q" ToolTip="Stampa la Scheda dei Movimenti Magazzino">
                                            </asp:MenuItem>
                                            <asp:MenuItem Text="Scheda dei Fertilizzanti" Value="7r" ToolTip="Stampa la Scheda dei Fertilizzanti">
                                            </asp:MenuItem>
                                            <asp:MenuItem Text="Scheda dei Prodotti Fitosanitari" Value="7s" ToolTip="Stampa la Scheda dei Prodotti Fitosanitari">
                                            </asp:MenuItem>
                                        </asp:MenuItem>
                                        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Agenda01.ico" Value="60" ToolTip="Agenda"
                                            Text=""></asp:MenuItem>
                                        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Impresa.ico" Value="16" ToolTip="Anagrafica azienda"
                                            meta:resourcekey="MenuItemResource25" Text=""></asp:MenuItem>
                                        <%--                        agosto 2014 tolto link contabilità         <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Contabilita.ico" Value="17" ToolTip="Contabilità"
                                    Text=""></asp:MenuItem>--%>
                                        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/lente.ico" Value="20" ToolTip="Cambia Impresa"
                                            meta:resourcekey="MenuItemResource30" Text=""></asp:MenuItem>
                                        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Impostazioni01.ico" Value="21" ToolTip="Impostazioni Utente"
                                            meta:resourcekey="MenuItemResource26" Text=""></asp:MenuItem>
                                        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Check32a.ico" Value="22" ToolTip="Profilazione Impresa"
                                            meta:resourcekey="MenuItemResource27" Text=""></asp:MenuItem>
                                        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Contatti.ico" Value="23" ToolTip="Gestione Contatti"
                                            meta:resourcekey="MenuItemResource28" Text=""></asp:MenuItem>
                                        <asp:MenuItem ImageUrl="~/AB_Immagini/icone32/Formulati32.ico" Value="13" ToolTip="GIAS ProFitoSan"
                                            meta:resourcekey="MenuItemResource19" Text=""></asp:MenuItem>
                                    </Items>
                                    <StaticHoverStyle BackColor="#666666" ForeColor="White" />
                                    <StaticMenuItemStyle HorizontalPadding="3px" VerticalPadding="2px" />
                                    <StaticSelectedStyle BackColor="#1C5E55" />
                                </asp:Menu>
                            </div>
                            <div class="clear ">
                            </div>
                            <div style="min-width: 700px; width: 100%; float: left;">
                                <!-- Riga 1 -->
                                <div class="box50" style="min-width: 350px" runat="server" id="PannelloCategoria">
                                    <div class="descrizione" style="width: 80px">
                                        <asp:Label ID="lblCategoria" runat="server">Categoria Prodotto</asp:Label>
                                    </div>
                                    <div class="valoriinput">
                                        <asp:DropDownList ID="cmb_Categoria" runat="server" Width="100%" CssClass="txtUI"
                                            AutoPostBack="True">
                                        </asp:DropDownList>
                                        <asp:Button ID="BTN_ComboCategoria" runat="server" Text="Button" Style="display: none" />
                                    </div>
                                </div>
            <div class="box50" style="min-width: 350px" runat="server" id="Div_Data">
                            <div class="box50" runat="server" id="Div_Data_Da">
                                <div class="descrizione" style="width: 80px">
                                    <asp:Label ID="lblData1" runat="server" meta:resourcekey="lblData1Resource1">Dal</asp:Label></div>
                                <div class="valoriinput" style="width: 74px">
                                    <asp:TextBox ID="txt_DataOperazioneDa" runat="server" CssClass="txtUI datepicker"></asp:TextBox>
                                    <asp:Button ID="BTN_ChangeData_Da" runat="server" Text="Button" Style="display: none" />
                                </div>
                            </div>
                            <div class="box50" runat="server" id="Div_Data_A">
                                <div class="descrizione" style="width: 80px">
                                    <asp:Label ID="Label3" runat="server" meta:resourcekey="lblData1Resource1">Al</asp:Label></div>
                                <div class="valoriinput" style="width: 74px">
                                    <asp:TextBox ID="txt_DataOperazioneA" runat="server" CssClass="txtUI datepicker"></asp:TextBox>
                                    <asp:Button ID="BTN_ChangeData_A" runat="server" Text="Button" Style="display: none" />
                                </div>
                            </div>
                           
                        </div>
                                <div class="clear">
                                </div>
                                <!-- Riga 3 -->
                                <div class="box50" style="min-width: 350px" runat="server" id="Div1">
                                    <div class="descrizione" style="width: 80px">
                                        <asp:Label ID="LabelProdotto" runat="server">Nome Prodotto</asp:Label>
                                    </div>
                                    <div class="valoriinput">
                                        <asp:TextBox ID="TxtProdotto" runat="server" Width="100%"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="box50" style="min-width: 350px" runat="server" id="Div2">
                                    <div class="descrizione" style="width: 80px">
                                        <asp:Label ID="Label1" runat="server">Codice</asp:Label>
                                    </div>
                                    <div class="valoriinput">
                                        <asp:TextBox ID="TxtCodice" runat="server" Width="100%"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="clear">
                                </div>
                                <div class="box33" style="min-width: 350px" runat="server" id="Div4">
                                    <div class="descrizione" style="width: 80px">
                                        <asp:Label ID="Label5" runat="server">Visualizza: </asp:Label>
                                    </div>
                                    <div class="valoriinput">
                                        <asp:RadioButtonList ID="RadioButtonList1" runat="server">
                                            <asp:ListItem Value="1" Text="Risorse Aziendali">
                                            </asp:ListItem>
                                            <asp:ListItem Value="2" Text="Risorse Pubbliche">
                                            </asp:ListItem>
                                            <asp:ListItem Value="3" Text="Tutte le Risorse">
                                            </asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                                <div class="box33" style="min-width: 250px" runat="server" id="Div5">
                                   
                                    <div class="valoriinput" >
                                        <asp:CheckBox ID="CheckBox1" runat="server" Text="Visualizza solo i Prodotti Prezzati" />
                                    </div>
                                </div>
                                <div class="box33" style="" runat="server" id="Div7">
                                    <div class="descrizione" style="width: 80px">
                                        <asp:Label ID="Label2" runat="server">Cerca: </asp:Label>
                                    </div>
                                    <div class="valoriinput" style="width: 80px">
                                        <asp:ImageButton ID="ImgBtn_Cerca" CssClass="btn_per_load" runat="server" ImageUrl="../AB_Immagini/icone32/lente.ico"
                                            Style="width: 42px" />
                                    </div>
                                                                        <div class="descrizione" style="width: 84px">
                                        <asp:Label ID="Label4" runat="server">Prodotti Trovati: </asp:Label></div>
                                    <div class="valoriinput" style="width: 40px;">
                                        <asp:Label ID="Label6" runat="server" CssClass="txtUI" Style="min-width: 40px"
                                            Width="40px"></asp:Label>
                                    </div>
                                </div>
                                <div class="clear">
                                </div>
                            </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="clear">
            </div>
            <div style="width: 100%;">
                <asp:UpdatePanel ID="UpdatePanelGiacenze" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="GridView_Giacenze" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                            Width="100%" CellPadding="5" CssClass="ui-widget-content" Caption="Riepilogo delle Risorse Registrate"
                            meta:resourcekey="GridView_DosiResource1">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="ChkSelezionaMovimento" runat="server" CssClass="ChkSelezionaMovimento" />
                                    </ItemTemplate>
                                    <HeaderStyle Width="20px" />
                                    <ItemStyle Width="20px" />
                                    <FooterStyle Width="20px" />
                                    <ControlStyle Width="20px" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="Piva" HeaderText="Piva" HtmlEncode="False">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Pubblico" HeaderText="Pubblico" HtmlEncode="False"></asp:BoundField>
                                <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod" HtmlEncode="False">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Sa_Nome" HeaderText="Centro" HtmlEncode="False"></asp:BoundField>
                                <asp:BoundField DataField="Cat_Cod" HeaderText="Cat_Cod" HtmlEncode="False">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Cat_Des" HeaderText="Categoria" HtmlEncode="False"></asp:BoundField>
                                <asp:BoundField DataField="Pro_Cod" HeaderText="Codice Prodotto" HtmlEncode="False">
                                </asp:BoundField>
                                <asp:BoundField DataField="Pro_Des" HeaderText="Prodotto" HtmlEncode="False"></asp:BoundField>
                                <asp:BoundField DataField="Mat_Cod" HeaderText="Mat_Cod" HtmlEncode="False">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Prezzo_Unitario" HeaderText="Prezzo_Unitario" HtmlEncode="False">
                                    <ItemStyle CssClass="displaynone" />
                                    <HeaderStyle CssClass="displaynone" />
                                </asp:BoundField>
                            </Columns>
                            <HeaderStyle CssClass="ui-widget-header" />
                            <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                            <RowStyle CssClass="rigaImpianti" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <!--------------------- DIALOG --->
                <div id="dialog" style="overflow: hidden">
                    <div id="xiframe" style="height: 100%">
                    </div>
                </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
