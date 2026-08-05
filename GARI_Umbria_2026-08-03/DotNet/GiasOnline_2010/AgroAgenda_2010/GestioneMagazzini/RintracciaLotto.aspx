<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/Agenda.Master"
    CodeBehind="RintracciaLotto.aspx.vb" Inherits="AgroAgenda_2010.RintracciaLotto" %>

<%@ Register Assembly="AgronicaControlli_2010" Namespace="AgronicaControlli_2010"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentAgendaHead" runat="server">
    <!--include per l'albero-->
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.cookie.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.hotkeys.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="<%= ResolveClientUrl("~/Scripts/jquery.jstree.js?" & Application("GiasVersioneCorrente").ToString) %>"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui.combobox.js?<% =Application("GiasVersioneCorrente")%>"></script>
    <style type="text/css">
        .bordomenu
        {
            border-bottom-color: #444444;
            border-bottom-width: 1px;
            border-bottom-style: solid;
        }
    </style>
    <script type="text/javascript">


        function pageLoad() {
            //al caricamento della pagina, ma prima di $(window).load
            clicktab();
        }

        //$(window).load(function () {
        $(window).on('load', function () {

        });




        function DoPostBack_EliminazioneControlli(key) {

            if (key == 'DEL') {

            }
        }

        $(document).ready(function () {

            //metto a visibile la classe main
            $('#main').show();
            //nascondo il load
            $('#load_main').hide();

            $('input:submit').button();

            $("#<%= txt_DataOperazioneAlGiorno.ClientID %>").datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });

        });




        //Per invocare il postback sulle combo
        function DoPostBack_Combo($_combo, valoreOpt) {
            //postBack CentroAziendale
            if ($_combo.attr("id").endsWith("ComboCentroAziendale")) {
                $("#<%=BTN_ComboCentroAziendale.ClientID %>").click();
                $('#WaitFrame').show();
                return;
            }

            //postBack Magazzini
            if ($_combo.attr("id").endsWith("ComboMagazzini")) {
                $("#<%=BTN_Magazzini.ClientID %>").click();
                $('#WaitFrame').show();
                return;
            }

            //postBack Categoria
            if ($_combo.attr("id").endsWith("cmb_Categoria")) {
                $("#<%=BTN_ComboCategoria.ClientID %>").click();
                $('#WaitFrame').show();
                return;
            }


        }





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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentAgendaContenuti" runat="server">
    <asp:UpdatePanel ID="UpdatePanelscript" runat="server">
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Bottone per l'eliminazione di un operazione -->
    <asp:Button ID="btn_Elimina_Operazione" runat="server" Style="display: none;" />
    <div class="main  box" id="Div3" style="min-width: 1000px; margin-left: 15px; margin-right: 15px;">
        <asp:UpdatePanel ID="UpdatePanel_Menu" runat="server" style="position: relative;">
            <ContentTemplate>
                <div style="width: 100%;">
                    <div style="min-width: 900px; width: 100%; float: left;">
                        <!-- Riga 1 -->
                        <div class="box50" style="min-width: 350px">
                            <div class="descrizione" style="width: 80px">
                                <asp:Label ID="lblCentroAziendale" runat="server" meta:resourcekey="lblCentroAziendaleResource1">Centro Aziendale</asp:Label>
                            </div>
                            <div class="valoriinput">
                                <cc1:ComboCentroAziendale ID="ComboCentroAziendale" runat="server" 
                                    meta:resourcekey="ComboCentroAziendaleResource1" visible="False" />
                                <asp:Button ID="BTN_ComboCentroAziendale" runat="server" Text="Button" Style="display: none"
                                    meta:resourcekey="BTN_ComboCentroAziendaleResource1" />
                            </div>
                        </div>
                        <div class="box50" style="min-width: 350px" runat="server" id="ProvenienzaRisorse">
                            <div class="descrizione" style="width: 80px">
                                <asp:Label ID="lblProvenienzaRisorse" runat="server">Magazzino</asp:Label>
                            </div>
                            <div class="valoriinput">
                                <cc1:ComboMagazzini ID="ComboMagazzini" runat="server" Fabbricato_Cod="0" Flag_CodCentroFabbricato="True"
                                    Flag_GestioneMagazziniImpresaPadre="False" meta:resourcekey="ComboMagazziniResource1"
                                    Sa_Cod="0" TipoMagazzino="20" visible="False" />
                                <asp:Button ID="BTN_Magazzini" runat="server" Text="Button" Style="display: none"
                                    meta:resourcekey="BTN_MagazziniResource1" />
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                        <!-- Riga 2 -->
                        <div class="box50" style="min-width: 350px" runat="server" id="PannelloCategoria">
                            <div class="descrizione" style="width: 80px">
                                <asp:Label ID="lblCategoria" runat="server">Categoria Prodotto</asp:Label>
                            </div>
                            <div class="valoriinput">
                                <asp:DropDownList ID="cmb_Categoria" runat="server" Width="100%" CssClass="txtUI"
                                    AutoPostBack="True" visible="False">
                                </asp:DropDownList>
                                <asp:Button ID="BTN_ComboCategoria" runat="server" Text="Button" Style="display: none" />
                            </div>
                        </div>
                        <div class="box50" style="min-width: 350px" runat="server" id="Div_Data">
                            <div class="Box100" runat="server" id="Div_Data_AlGiorno">
                                <div class="descrizione" style="width: 140px">
                                    <asp:Label ID="Label6" runat="server">Ricerca al giorno:</asp:Label></div>
                                <div class="valoriinput" style="width: 80px">
                                    <asp:TextBox ID="txt_DataOperazioneAlGiorno" runat="server" 
                                        CssClass="txtUI datepicker" Enabled="False"></asp:TextBox>
                                    <asp:Button ID="BTN_ChangeData_AlGiorno" runat="server" Text="Button" Style="display: none" />
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
                                <asp:TextBox ID="TxtProdotto" runat="server" Width="100%" Enabled="False"></asp:TextBox>
                            </div>
                        </div>
                        <div class="box50" style="min-width: 350px" runat="server" id="Div2">
                            <div class="descrizione" style="width: 80px">
                                <asp:Label ID="Label1" runat="server">Lotto</asp:Label>
                            </div>
                            <div class="valoriinput">
                                <asp:TextBox ID="TxtLotto" runat="server" Width="100%"></asp:TextBox>
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                        <div class="box50" style="min-width: 350px" runat="server" id="Div4">
                            <div class="descrizione" style="width: 80px">
                                <asp:Label ID="Label4" runat="server">Codice Prodotto</asp:Label>
                            </div>
                            <div class="valoriinput">
                                <asp:TextBox ID="TxtCodProdotto" runat="server" Width="100%" Enabled="False"></asp:TextBox>
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                        <div style="min-width: 200px; float: left; width: 30%" id="opzioni">
                            <div>
                                <div class="" style="width: 100%; text-align: center; padding-right: 0px; float: left;
                                    height: 44px;">
                                    <div id="DivCercaMovimenti">
                                        <asp:ImageButton ID="ImgBtn_Cerca_Movimenti" CssClass="btn_per_load" runat="server"
                                            ImageUrl="../AB_Immagini/icone32/movimenti.png" Style="width: 42px" />
                                        Cerca i Movimenti
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="clear">
    </div>
    <div style="width: 100%;">
        <div style="width: 100%;">
            <asp:UpdatePanel ID="UpdatePanelMovimenti" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="GridView_Movimenti" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                        Width="100%" CellPadding="5" CssClass="ui-widget-content" Caption="Riepilogo dei Movimenti">
                        <Columns>
                            <asp:ButtonField ItemStyle-HorizontalAlign="Center" Text="&lt;img src='../AB_Immagini/icone24/movimenti.png' border='0'&gt; "
                                HeaderText="Info" CommandName="Seleziona">
                                <ItemStyle CssClass="btn_per_load" />
                            </asp:ButtonField>
                            <asp:BoundField DataField="DescrizioneMovimento" HeaderText="Descrizione Movimento"
                                HtmlEncode="False"></asp:BoundField>
                            <asp:BoundField DataField="Data" HeaderText="Data" HtmlEncode="False"></asp:BoundField>
                             <asp:BoundField DataField="Ora" HeaderText="Ora" HtmlEncode="False"></asp:BoundField>
                            <asp:BoundField DataField="Lav_Cod" HeaderText="Lav_Cod" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Lav_Des" HeaderText="Descrizione Attivit&#224;" HtmlEncode="False">
                                <HeaderStyle />
                                <ItemStyle />
                            </asp:BoundField>
                            <asp:BoundField DataField="Dettagli" HeaderText="Dettagli" HtmlEncode="False">
                                <HeaderStyle CssClass="dettagli" />
                                <ItemStyle CssClass="dettagli" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cau_Mov" HeaderText="Cau_Mov" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cau_Des" HeaderText="Causale" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Piva" HeaderText="Piva" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Sa_Cod" HeaderText="Sa_Cod" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Id_Agenda" HeaderText="Id_Agenda" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Sa_Nome" HeaderText="Centro" HtmlEncode="False"></asp:BoundField>
                            <asp:BoundField DataField="Fabbricato_Cod" HeaderText="Fabbricato_Cod" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Fabbricato_Des" HeaderText="Fabbricato" HtmlEncode="False">
                            </asp:BoundField>
                            <asp:BoundField DataField="Cat_Cod" HeaderText="Cat_Cod" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cat_Des" HeaderText="Categoria" HtmlEncode="False"></asp:BoundField>
                            <asp:BoundField DataField="Pro_Cod" HeaderText="Codice Prodotto" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Pro_Des" HeaderText="Prodotto" HtmlEncode="False"></asp:BoundField>
                            <asp:BoundField DataField="Mat_Cod" HeaderText="Mat_Cod" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Lotto_Int" HeaderText="Lotto Int." HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Lotto_Acc" HeaderText="Lotto Acc." HtmlEncode="False">
                            </asp:BoundField>
                            <asp:BoundField DataField="Param_Des" HeaderText="Parametro Qualitativo" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cal_Cod" HeaderText="Cal_Cod" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cal_Des" HeaderText="Qualità" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cod_Progetto" HeaderText="Cod_Progetto" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Fase_Cod" HeaderText="Fase_Cod" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Udm_Cod" HeaderText="Udm_Cod" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Udm_Des" HeaderText="Unità di Misura" HtmlEncode="False">
                            </asp:BoundField>
                            <asp:BoundField DataField="Qta" HeaderText="Qta" HtmlEncode="False"></asp:BoundField>
                            <asp:BoundField DataField="Qta_Dest" HeaderText="Qta" HtmlEncode="False">
                                <ItemStyle CssClass="displaynone" />
                                <HeaderStyle CssClass="displaynone" />
                            </asp:BoundField>
                        </Columns>
                        <HeaderStyle CssClass="ui-widget-header" />
                        <PagerStyle CssClass="ui-widget-header" HorizontalAlign="Center" />
                        <RowStyle CssClass="rigaImpianti" />
                    </asp:GridView>

                     <asp:Label ID="LabelRes" runat="server"></asp:Label>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
