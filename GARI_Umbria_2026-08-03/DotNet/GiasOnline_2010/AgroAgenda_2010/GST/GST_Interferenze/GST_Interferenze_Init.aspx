<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/GSTBootstrap.master" CodeBehind="GST_Interferenze_Init.aspx.vb" Inherits="AgroAgenda_2010.GST_Interferenze_Init" %>

<%@ MasterType VirtualPath="~/Master/GSTBootstrap.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="GST_HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="GST_MainContent" runat="server">

    <div style="display: grid; grid-template-columns: 1fr 1fr 1fr; margin-top: 8px">

        <div id="gstInterferenzeImprese" style="margin-right: 4px">
            <asp:Label ID="Label1" runat="server" CssClass="Testo_08_Nero_Bold" Text="Distanze di interferenza :"></asp:Label>
            <br />
            <asp:Panel ID="Pannello_xImpreseReferenti" runat="server" BackColor="#FFE0E0" BorderColor="Gray"
                BorderStyle="Solid" BorderWidth="1px" Height="200px" Style="padding: 5px" ScrollBars="Vertical">
                <asp:Label ID="LABEL103" runat="server" CssClass="Testo_08_Nero" Height="33px" Style="width: 409px">Nelle operazioni di ricerca, verranno utilizzate le distanze minime di Legge moltiplicate per il valore inserito.</asp:Label>
                <br />
                <asp:Label ID="LABEL12" runat="server" CssClass="Testo_08_Nero_Bold" Style="width: 216px">Moltiplicatore distanze minime :</asp:Label>
                <asp:TextBox ID="Txt_Moltiplicatore" runat="server" CssClass="Testo_08_Blue_Bold"
                    MaxLength="4" Style="width: 41px">1</asp:TextBox>
            </asp:Panel>
        </div>

        <div id="gstInterferenzeTipo" style="margin-right: 4px">
            <asp:Label ID="Label13" runat="server" CssClass="Testo_08_Nero_Bold" Style="width: 364px"
                Text="Tipo di interferenze da ricercare :"></asp:Label>
            <br />
            <asp:Panel ID="Panel7" runat="server" BackColor="#E0FFE0" BorderColor="Gray" BorderStyle="Solid"
                BorderWidth="1px" Height="200px" Style="width: 100%; padding: 5px" ScrollBars="Vertical">
                <asp:Label ID="Lbl_InterferenzeTipo" runat="server" CssClass="Testo_08_Nero">
                In funzione dell'utente attualmente collegato, (###) e' possibile limitare la ricerca ad alcuni tipi di interferenze.
                </asp:Label>
                <br />
                <br />
                <asp:RadioButton ID="Rbl_FlagInterferenze_Interne" runat="server" GroupName="Tipologia_Interferenza" Style="float: left;" CssClass="Testo_08_Nero"
                    Height="16px" Text="Interferenze <b>INTERNE</b> della <b>PROPRIA</b> Azienda." Checked="true" />

                <asp:Label ID="Lbl_AA" runat="server" CssClass="Testo_08_Nero_Bold" ForeColor="Green"
                    Style="width: 41px; float: right;" Text="{AA}"></asp:Label>

                <asp:RadioButton ID="Rbl_FlagInterferenze_Esterne" runat="server" GroupName="Tipologia_Interferenza" Style="float: left;" CssClass="Testo_08_Nero"
                    Height="16px" Text="Interferenze tra la <b>PROPRIA</b> Azienda e le <b>ALTRE</b>." />

                <asp:Label ID="Lbl_AX" runat="server" CssClass="Testo_08_Nero_Bold" ForeColor="Green"
                    Style="width: 41px; float: right;" Text="{Ax}"></asp:Label>

                <br />


                <asp:RadioButton ID="Rbl_FlagInterferenze_AB" runat="server" GroupName="Tipologia_Interferenza" Style="float: left;" CssClass="Testo_08_Nero"
                    Height="16px" Text="Interferenze <b>TRA</b> le Aziende." />

                <br />


                <asp:Label ID="Lbl_XY" runat="server" CssClass="Testo_08_Nero_Bold" ForeColor="Green"
                    Style="width: 41px; float: right;" Text="{xy}"></asp:Label>

                <br />

                <asp:RadioButton ID="Rbl_FlagInterferenze_TUTTI_TUTTI" runat="server" GroupName="Tipologia_Interferenza" Style="float: left;" CssClass="Testo_08_Nero"
                    Height="16px" Text="<b>Tutte</b> le Interferenze." />


            </asp:Panel>
        </div>



        <div id="gstInterferenzeImp" style="margin-right: 4px">
            <asp:Label ID="Label10" runat="server" CssClass="Testo_08_Nero_Bold" Text="Insiemi sui quali effettuare la ricerca :"></asp:Label>
            <asp:Panel ID="panelLabel10" runat="server" BackColor="LightYellow" BorderColor="Gray"
                BorderStyle="Solid" BorderWidth="1px" Style="padding: 5px; height: 200px">
                <asp:Label ID="Label9" runat="server" CssClass="Testo_08_Nero">Selezionare l'insieme di impianti sui quali si vuole effettuare l'operazione di ricerca 
                                 delle possibili interferenze :</asp:Label>
                <br />
                <asp:RadioButton ID="Opt_Ricerca_Tutti" runat="server" Checked="True" CssClass="Testo_08_Blue"
                    GroupName="AreaRicercaInterferenze" Text="Ricerca su TUTTI gli impianti selezionati" /><br />

                <br />

                <br />

                <asp:Button ID="btn_Elabora" runat="server" Text="Lancia Elaborazione" Style="float: right" />

                <br />

            </asp:Panel>
        </div>

    </div>

    <asp:Panel ID="Pannello_Risultato" runat="server" BackColor="WhiteSmoke" Style="margin-top: 14px"
        BorderStyle="None" BorderWidth="0px">

        <div class="row" style="margin-top:14px; margin-bottom: 14px; display:none" id="rowBtnEstrazioni">
            <div class="col-lg-3">
                <div style="display: grid; gap: 10px;">
                    <div id="btnSalvaInterferenze" class="k-button"><span class="fa fa-table fa-2x"></span>Consolida Interferenze</div>
                </div>
            </div>
            <div class="col-lg-9">&nbsp;</div>
        </div>

        <div style="margin-bottom: 14px">

            <asp:ImageButton ID="ImgBtn_Impianti_SelezionaTutti" runat="server" ImageUrl="<%=PATH_GIASBASE %>agronica/AB_Immagini/Icone24/ValidazioneSI_24.ico"
                Style="width: 24px" ToolTip="Seleziona tutti" />
            <asp:ImageButton ID="ImgBtn_Impianti_DeselezionaTutti" runat="server" ImageUrl="<%=PATH_GIASBASE %>agronica/AB_Immagini/Icone24/ValidazioneNO_24.ico"
                Style="width: 24px" ToolTip="Deseleziona tutti" />
            <asp:Label ID="Label11" runat="server" CssClass="Testo_08_Nero_Bold" Style="width: 122px"
                Text="Ordinamento per  :"></asp:Label>
            <asp:DropDownList ID="Cmb_Ordinamento" runat="server" CssClass="Testo_08_Blue" Style="width: 200px">
            </asp:DropDownList>
            <asp:ImageButton ID="ImgBtn_Crescente" runat="server" ImageUrl="<%=PATH_GIASBASE %>agronica/AB_Immagini/Icone16/FrecciaVerde_S.ico"
                Style="width: 16px" ToolTip="Ordine Crescente" />
            <asp:ImageButton ID="ImgBtn_Decrescente" runat="server" ImageUrl="<%=PATH_GIASBASE %>agronica/AB_Immagini/Icone16/FrecciaRossa_N.ico"
                Style="width: 16px; height: 16px;" ToolTip="Ordine Decrescente" />
            <asp:Label ID="Lbl_NumeroInterferenze" runat="server" CssClass="Testo_07_Nero" ForeColor="Fuchsia"
                Style="width: 200px" Text="N. Righe :  <b>1234</b>"></asp:Label>
        </div>
        <asp:GridView ID="GridViewInterferenze" runat="server" AllowSorting="True" AutoGenerateColumns="False"
            CellPadding="4" CellSpacing="1" ForeColor="#000000" GridLines="None" ShowFooter="True"
            Style="width: 100%" CaptionAlign="Top">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <Columns>
                <asp:TemplateField>
                    <HeaderStyle Width="20px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:CheckBox ID="ChkSeleziona" runat="server" AutoPostBack="false" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="Impianto_A" HeaderText="Impianto 1" HtmlEncode="False" />
                <asp:BoundField DataField="Impianto_B" HeaderText="Impianto 2" HtmlEncode="False" />
                <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" HtmlEncode="False" />
                <asp:BoundField DataField="Distanza" HeaderText="Distanze  [m.]" HtmlEncode="False">
                    <ItemStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField DataField="Distanza_di_legge" HeaderText="Distanza di Legge [m.]" HtmlEncode="False" />
                <asp:BoundField DataField="Distanza_di_legge_moltiplicata" HeaderText="Distanza di Legge<br> Moltiplicata[m.]" HtmlEncode="False" />

            </Columns>
            <RowStyle BackColor="#EFF3FB" Font-Size="7pt" />
            <EditRowStyle BackColor="#2461BF" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" Font-Size="8pt" Font-Underline="False"
                ForeColor="White" Height="25px" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
    </asp:Panel>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="GST_ScriptContent" runat="server">
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_Interferenze_Init.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_Interferenze_InitJQueryDocReady.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("GST_Interferenze_Init_ws_client.js") %>"></script>
</asp:Content>
