<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master"
    CodeBehind="Filtro_SchedeMagazzino_new.aspx.vb" Inherits="AgronicaStampe_2010.Filtro_SchedeMagazzino_new"
    EnableEventValidation="false" %>

<%@ Import Namespace="AgronicaCoreDataProvider" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row" id="pulsanti_in_alto">
        <div class="col-md-12" style="margin-bottom: 15px;">
            <div style="float: right;">
                <asp:CheckBox ID="CB_BloccaOperazioni" Style="float: left;" runat="server" CssClass="Testo_07_Blue"
                    Text="Blocca Operazioni"></asp:CheckBox>
                <div class="btn btn-default" id="btn_stampa_excel" style="float: left; margin-right: 5px;">
                    <i class="fa fa-file-excel-o"></i>Esporta in Excel
                </div>
                <div class="btn btn-info" id="btn_stampa" style="float: left; margin-right: 5px;">
                    <i class="fa fa-print"></i>Stampa
                </div>
                <asp:ImageButton Style="display: none;" ID="ImgBtn_Stampa" runat="server" ImageUrl="../../AB_Immagini/icone32/stampa.ico"></asp:ImageButton>
                <asp:ImageButton ID="ImgBtn_StampaExcel" Style="display: none;" runat="server" ImageUrl="../../AB_Immagini/icone32/XLS_01.ico"></asp:ImageButton>
            </div>
        </div>
    </div>
    <div class="jumbotron" style="margin-bottom: 100px;">
        <div class="row">
            <div id="selezione_stampa" class="col-md-6">
                <div class="row">
                    <div class="col-md-12">
                        <h5 style="color: #052747; margin-top: 10px;">Seleziona il tipo di report:</h5>
                    </div>
                    <div class="col-md-12">
                        <%--      <ASP:RADIOBUTTONLIST id="Rbl_SchedaMagazzino" runat="server" CssClass="Testo_08_Blue" RepeatLayout="Flow">
				            <asp:ListItem Value="9" Selected="True">Scheda Movimenti di Magazzino</asp:ListItem>
				            <asp:ListItem Value="10">Scheda Giacenze di Magazzino</asp:ListItem>
				            <asp:ListItem Value="11">Scheda Fertilizzanti in Magazzino</asp:ListItem>
				            <asp:ListItem Value="12">Scheda Prodotti Fitosanitari in Magazzino</asp:ListItem>
			            </ASP:RADIOBUTTONLIST>   --%>
                        <div id="Rbl_SchedaMagazzino">
                            <div class="radio">
                                <label>
                                    <input type="checkbox" name="chk_group[]" value="9" />Scheda Movimenti di Magazzino</label>
                                <asp:RadioButtonList ID="caricoscarico" runat="server" CssClass="radio">
                                    <asp:ListItem Value="0" Selected="True">Carichi e Scarichi</asp:ListItem>
                                    <asp:ListItem Value="1">Solo Carichi</asp:ListItem>
                                    <asp:ListItem Value="2">Solo Scarichi</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                            <div class="radio">
                                <label>
                                    <input type="checkbox" name="chk_group[]" value="10" />Scheda Giacenze di Magazzino</label>
                            </div>
                            <div class="radio">
                                <label>
                                    <input type="checkbox" name="chk_group[]" value="11" checked="checked" />Scheda
                                    Fertilizzanti in Magazzino</label>
                            </div>
                            <div class="radio">
                                <label>
                                    <input type="checkbox" name="chk_group[]" value="12" />Scheda Prodotti Fitosanitari
                                    in Magazzino</label>
                            </div>
                            <div class="radio">
                                <label>
                                    <input type="checkbox" name="chk_group[]" value="213" />Scheda Riepilogo Prodotti
                                    Utilizzati</label>
                                <asp:RadioButtonList ID="rbl_caricoscarico2" runat="server" CssClass="radio">
                                    <asp:ListItem Value="7300" Selected="True">Solo Carichi</asp:ListItem>
                                    <asp:ListItem Value="7350">Solo Scarichi</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-6" id="arrotondamento">
                <div class="row">
                    <div class="col-md-12">
                        <h5 style="color: #052747; margin-top: 10px;">Seleziona il tipo di arrotondamento</h5>
                    </div>
                    <div class="col-md-12">
                        <asp:RadioButtonList ID="Rbl_Arrotondamento" runat="server" CssClass="Testo_08_Blue">
                            <asp:ListItem Value="0">Nessuno</asp:ListItem>
                            <asp:ListItem Value="1">Unit&#224;</asp:ListItem>
                            <asp:ListItem Value="2">1 Decimale</asp:ListItem>
                            <asp:ListItem Value="3">2 Decimali</asp:ListItem>
                            <asp:ListItem Value="4" Selected="True">3 Decimali</asp:ListItem>
                            <asp:ListItem Value="5">4 Decimali</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </div>
        <hr />
        <!--  FILTRI DATE -->
        <div class="row">
            <%-- <div id="titolo_annata_agraria">
                <div class="col-md-12">
                    <h5 style="color: #052747; margin-top: 10px;">
                        Annata Agraria</h5>
                </div>
            </div>
            <div id="titolo_intervallo_temp">
                <div class="col-md-12">
                    <h5 style="color: #052747; margin-top: 10px;">
                        Intervallo temporale</h5>
                </div>
            </div>
            <div id="titolo_stampa_sing">
                <div class="col-md-12">
                    <h5 style="color: #052747; margin-top: 10px;">
                        Data Stampa</h5>
                </div>
            </div>--%>
            <div id="scheda_intervallo_temp">
                <div id="data_inizio" class="col-lg-4 col-md-4 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_validita_inizio" for="TxtValiditaInizio">
                                    <i class="fa fa-calendar"></i>Data Inizio </span>
                                <asp:TextBox ID="TxtDataDa" runat="server" CssClass="form-control datepicker">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="data_fine" class="col-lg-4 col-md-4 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_validita_fine" for="TxtValiditaFine">
                                    <i class="fa fa-calendar"></i>Data Fine </span>
                                <asp:TextBox ID="TxtDataA" runat="server" CssClass="form-control datepicker">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="scorri" class="col-lg-4 col-md-4 col-sm-12">
                    <div class="btn btn-info" id="btn_AnnataPrecedente" runat="server" style="float: left; margin-right: 5px;">
                        <i class="fa fa-arrow-left"></i>Precedente
                        <asp:ImageButton ID="ImgBtn_AnnataPrecedente" Style="display: none" runat="server"
                            ImageUrl="../../AB_Immagini/icone32/frecciasx.ico" ToolTip="Annata Precedente"></asp:ImageButton>
                    </div>
                    <div class="btn btn-info" id="btn_AnnataSuccessiva" runat="server" style="float: left; margin-right: 5px;">
                        Successiva <i class="fa fa-arrow-right"></i>
                        <asp:ImageButton ID="ImgBtn_AnnataSuccessiva" Style="display: none" runat="server"
                            ImageUrl="../../AB_Immagini/icone32/frecciadx.ico" ToolTip="Annata Successiva"></asp:ImageButton>
                    </div>
                </div>
            </div>
            <div id="scheda_stampa_sing">
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_Stampa" for="TxtStampa"><i class="fa fa-calendar"></i>Data Stampa </span>
                                <asp:TextBox ID="TxtStampa" runat="server" CssClass="form-control datepicker">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <%--            <div id="scheda_annata_agraria">
                <div class="col-md-12">
                    <h5 style="color: #052747; margin-top: 10px;">
                        Naviga Annate Agrarie</h5>
                </div>
                <div class="col-lg-4 col-md-4 col-sm-12">
                    <div class="btn btn-info" id="btn_AnnataPrecedente" runat="server" style="float: left;
                        margin-right: 5px;">
                        <i class="fa fa-arrow-left"></i>Precedente
                        <asp:ImageButton ID="ImgBtn_AnnataPrecedente" Style="display: none" runat="server"
                            ImageUrl="../../AB_Immagini/icone32/frecciasx.ico" ToolTip="Annata Precedente">
                        </asp:ImageButton>
                    </div>
                    <div class="btn btn-info" id="btn_AnnataSuccessiva" runat="server" style="float: left;
                        margin-right: 5px;">
                        Successiva <i class="fa fa-arrow-right"></i>
                        <asp:ImageButton ID="ImgBtn_AnnataSuccessiva" Style="display: none" runat="server"
                            ImageUrl="../../AB_Immagini/icone32/frecciadx.ico" ToolTip="Annata Successiva">
                        </asp:ImageButton>
                    </div>
                </div>
            </div>--%>
        </div>
        <!-- FILTRI MENU' A TENDINA-->
        <div id="ricerca_azienda" class="row">
            <div class="col-lg-4 col-md-6 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon alert-info" id="lbl_Txt_Impresa" for="Txt_Impresa">Ricerca
                                Impresa </span>
                            <asp:TextBox ID="Txt_Impresa" runat="server" CssClass="form-control">
                            </asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-2 col-md-6 col-sm-12">
                <div class="btn btn-success xonne-btn-primary" id="btn_CercaImpresa">
                    <i class="fa fa-search"></i>Cerca
                </div>
                <asp:ImageButton ID="ImgBtn_CercaImpresa" runat="server" ImageUrl="../../AB_Immagini/icone32/lente.ico"
                    Style="display: none;"></asp:ImageButton>
            </div>
            <div class="col-lg-3 col-md-6 col-sm-12">
                <asp:CheckBox ID="Chk_LogoRegione" runat="server" CssClass="Testo_07_Blue" Text="Stampa il logo della regione"></asp:CheckBox>
            </div>
            <div class="col-lg-3 col-md-6 col-sm-12">
                <%--                <ASP:DROPDOWNLIST tabIndex="7" id="Cmb_Regioni" style="POSITION: absolute; LEFT: 664px; Z-INDEX: 129; TOP: 240px"
					runat="server" Width="152px" Height="18px" CssClass="Testo_08_Nero" AutoPostBack="True"></ASP:DROPDOWNLIST>--%>
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon alert-info" id="lbl_Regioni" for="Cmb_Regioni">Regione
                            </span>
                            <asp:DropDownList ID="Cmb_Regioni" runat="server" CssClass="form-control" data-live-search="true"
                                aria-describedby="lbl_Regioni">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div id="risultato_azienda" class="row">
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon alert-info" id="lbl_Impresa" for="Cmb_Impresa">Impresa
                            </span>
                            <%--                           <ASP:DROPDOWNLIST id="Cmb_Impresa" style="POSITION: absolute; LEFT: 152px; Z-INDEX: 108; TOP: 280px"
					runat="server" Width="496px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>--%>
                            <asp:DropDownList ID="Cmb_Impresa" runat="server" CssClass="form-control" data-live-search="true"
                                aria-describedby="lbl_Impresa">
                            </asp:DropDownList>
                            <span class="input-group-addon alert-info">
                                <asp:Label ID="Lbl_NumImprese" runat="server">0</asp:Label>
                            </span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon alert-info" id="lbl_CentroAziendale" for="Cmb_CentroAziendale">Centro Aziendale </span>
                            <%--                           <ASP:DROPDOWNLIST id="Cmb_Impresa" style="POSITION: absolute; LEFT: 152px; Z-INDEX: 108; TOP: 280px"
					runat="server" Width="496px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>--%>
                            <asp:DropDownList ID="Cmb_CentroAziendale" runat="server" CssClass="form-control"
                                data-live-search="true" aria-describedby="lbl_CentroAziendale">
                            </asp:DropDownList>
                            <span class="input-group-addon alert-info">
                                <asp:Label ID="Lbl_NumCentri" runat="server">0</asp:Label>
                            </span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-12 col-md-12 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon alert-info" id="lbl_Magazzino" for="Cmb_Magazzino">Magazzino
                            </span>
                            <%--                           <ASP:DROPDOWNLIST id="Cmb_Impresa" style="POSITION: absolute; LEFT: 152px; Z-INDEX: 108; TOP: 280px"
					runat="server" Width="496px" Height="18px" CssClass="Testo_08_Blue" AutoPostBack="True"></ASP:DROPDOWNLIST>--%>
                            <asp:DropDownList ID="Cmb_Magazzino" runat="server" CssClass="form-control" data-live-search="true"
                                aria-describedby="lbl_Magazzino">
                            </asp:DropDownList>
                            <span class="input-group-addon alert-info">
                                <asp:Label ID="Lbl_NumMagazzini" runat="server">0</asp:Label>
                            </span>
                        </div>
                    </div>
                </div>
            </div>
            <div id="scheda_fitosanitari">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <asp:CheckBox ID="Chk_Composizione" runat="server" CssClass="Testo_08_Blue" Text="Stampa la composizione dei prodotti fitosanitari"></asp:CheckBox>
                </div>
            </div>
        </div>
        <div id="categoria_prodotto_lotto">
            <div class="row" id="div_elemcod">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_CatProdotto" for="cmb_CatProdotto">Categoria Prodotto </span>
                                <asp:DropDownList ID="cmb_CatProdotto" runat="server" CssClass="form-control" data-live-search="true"
                                    aria-describedby="lbl_CatProdotto">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row" id="div_ricercaprod">
                <div class="col-lg-4 col-md-6 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_ProdottoCerca" for="Txt_ProdottoCerca">Ricerca Prodotto </span>
                                <asp:TextBox ID="Txt_ProdottoCerca" runat="server" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-2 col-md-6 col-sm-12">
                    <div class="btn btn-success xonne-btn-primary" id="btn_CercaProdotto">
                        <i class="fa fa-search"></i>Cerca
                    </div>
                    <asp:ImageButton ID="ImgBtn_ProdottiCerca" Style="display: none;" runat="server"
                        ImageUrl="../../AB_Immagini/icone32/lente.ico"></asp:ImageButton>
                </div>
            </div>
            <div class="row" id="div_cmbprod">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_Prodotti" for="cmb_Prodotti">Prodotti
                                </span>
                                <asp:DropDownList ID="cmb_Prodotti" runat="server" CssClass="form-control" data-live-search="true"
                                    aria-describedby="lbl_Prodotti">
                                </asp:DropDownList>
                                <span class="input-group-addon alert-info">
                                    <asp:Label ID="Lbl_NumProdotti" runat="server">0</asp:Label>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row" id="div_txtlotto">
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_Lotto" for="Txt_Lotto">Lotto
                                </span>
                                <asp:TextBox ID="Txt_Lotto" runat="server" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <hr />
        <div id="opzioni" class="row">
            <%-- <ASP:PANEL id="Pannello_CodiciArticolo"	MS_POSITIONING="GridLayout" runat="server" Visible="False">--%>
            <div class="col-md-6" id="div_codarticolo_categ">
                <div class="row">
                    <div class="col-md-12">
                        <h5 style="color: #052747; margin-top: 10px;">Seleziona le categoria di cui si vuole stampare il codice articolo</h5>
                    </div>
                    <div class="col-md-12">
                        <asp:CheckBoxList ID="ChkList_Categorie" runat="server" CssClass="Testo_08_Blue">
                        </asp:CheckBoxList>
                    </div>
                </div>
            </div>
            <%-- </ASP:PANEL>--%>
            <div class="col-md-6" id="div_altro">
                <div class="row" id="div_ordinamento">
                    <div class="col-md-12">
                        <h5 style="color: #052747; margin-top: 10px;">Seleziona il tipo di ordinamento (x Scheda Mov. Magazzino)</h5>
                    </div>
                    <div class="col-md-12">
                        <asp:RadioButtonList ID="Rbl_Ordinamento" runat="server" CssClass="Testo_08_Blue">
                            <asp:ListItem Value="0" Selected="True">Data</asp:ListItem>
                            <asp:ListItem Value="1">Prodotto</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
                <div class="row" id="div_stampalotto">
                    <div class="col-md-12">
                        <h5 style="color: #052747; margin-top: 10px;">Seleziona la modalità di stampa del lotto</h5>
                    </div>
                    <div class="col-md-12">
                        <asp:RadioButtonList ID="Rbl_StampaLotto" runat="server" CssClass="Testo_08_Blue">
                            <asp:ListItem Value="0">Stampa sempre il lotto</asp:ListItem>
                            <asp:ListItem Value="1" Selected="True">Stampa in base alla configurazione del prodotto</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
                <div class="row" id="div_raggruppamento">
                    <div class="col-md-12">
                        <h5 style="color: #052747; margin-top: 10px;">Raggruppamento (x Scheda Giacenze Magazzino):
                        </h5>
                    </div>
                    <div class="col-md-12">
                        <asp:RadioButtonList ID="Rbl_Raggruppamento" runat="server" CssClass="Testo_08_Blue">
                            <asp:ListItem Value="0">Stampa raggruppata</asp:ListItem>
                            <asp:ListItem Value="1" Selected="True">Stampa standard</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- JUMBOTRON-->

    <!-- Questi arrivano dalla Query String -->
    <input type="hidden" id="hdQs_Sa_Cod" runat="server" />
    <input type="hidden" id="hdQs_Fabbricato_Cod" runat="server" />

    <input type="hidden" id="hdQS_DataStampa" runat="server" />
    <input type="hidden" id="hdQS_DataInizio" runat="server" />
    <input type="hidden" id="hdQS_DataFine" runat="server" />

    <!-- Questi sostituiscono l'uso della Session all'interno della pagina per passaggi client/servers -->
    <input type="hidden" id="hds_piva" runat="server" />
    <input type="hidden" id="hds_sa_cod" runat="server" />
    <input type="hidden" id="hds_fabbricato_cod" runat="server" />
    <input type="hidden" id="hds_cat_prodotto" runat="server" />
    <input type="hidden" id="hds_prodotto" runat="server" />

    <input type="hidden" id="hdTipo_Scheda" runat="server" />
    <input type="hidden" id="hdReportSelezionato" runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">

        var cIdQs_Sa_Cod = "#<%=hdQs_Sa_Cod.ClientID() %>";
        var cIdQs_Fabbricato_Cod = "#<%=hdQs_Fabbricato_Cod.ClientID() %>";

        var cIdQS_DataStampa = "#<%=hdQS_DataStampa.ClientID() %>";
        var cIdQS_DataInizio = "#<%=hdQS_DataInizio.ClientID() %>";
        var cIdQS_DataFine = "#<%=hdQS_DataFine.ClientID() %>";

        var cIds_piva = "#<%=hds_piva.ClientID() %>";
        var cIds_sa_cod = "#<%=hds_sa_cod.ClientID() %>";
        var cIds_fabbricato_cod = "#<%=hds_fabbricato_cod.ClientID() %>";
        var cIds_cat_prodotto = "#<%=hds_cat_prodotto.ClientID() %>";
        var cIds_prodotto = "#<%=hds_prodotto.ClientID() %>";

        var cIdTipo_Scheda = "#<%=hdTipo_Scheda.ClientID() %>";
        var cIdReportSelezionato = "#<%=hdReportSelezionato.ClientID() %>";

        // funzione di lettura del url e querystring
        function getParameterByName(name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        }

        function scheda_visibilita(tipo) {

            switch (tipo) {

                //MOVIMENTI
                case 9:

                    $('#selezione_stampa').show();
                    $('#<%=caricoscarico.ClientID %>').show();
                    $('#<%=rbl_caricoscarico2.ClientID %>').hide();

                    //$('#scheda_annata_agraria').show();
                    $('#scheda_intervallo_temp').show();
                    $('#scheda_stampa_sing').hide();

                    $('#categoria_prodotto_lotto').show();
                    $('#scheda_fitosanitari').hide();

                    $('#div_codarticolo_categ').show();
                    $('#div_txtlotto').show();

                    $('#div_altro').show();
                    $('#div_ordinamento').show();
                    $('#div_stampalotto').show();

                    $('#div_raggruppamento').hide();

                    <%If (permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.Stampa_MovimentiMagazziniExcel).Lettura = True) Then%>
                    $('#btn_stampa_excel').show();
                    <%Else%>
                    $('#btn_stampa_excel').hide();
                    <%End If%>

                    <%If (Permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.Agenda_Operazioni_Blocco).Scrittura = True) Then%>
                    $('#<%=CB_BloccaOperazioni.ClientID %>').show();
                    $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').show();
                    <%Else%>
                    $('#<%=CB_BloccaOperazioni.ClientID %>').hide();
                    $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').hide();
                    <%End If%>


                    break;

                //GIACENZE
                case 10:

                    $('#selezione_stampa').show();
                    $('#<%=caricoscarico.ClientID %>').hide();
                    $('#<%=rbl_caricoscarico2.ClientID %>').hide();

                    //$('#scheda_annata_agraria').hide();
                    $('#scheda_intervallo_temp').hide();
                    $('#scheda_stampa_sing').show();

                    $('#categoria_prodotto_lotto').show();
                    $('#scheda_fitosanitari').show();

                    $('#div_codarticolo_categ').show();
                    $('#div_txtlotto').show();

                    $('#div_altro').show();
                    $('#div_ordinamento').hide();
                    $('#div_stampalotto').show();
                    $('#div_raggruppamento').show();


                   // $('#btn_stampa_excel').hide();
                   // $('#<%=CB_BloccaOperazioni.ClientID %>').hide();
                   // $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').hide();


                    <%If (permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.Stampa_MovimentiMagazziniExcel).Lettura = True) Then%>
                    $('#btn_stampa_excel').show();
                    <%Else%>
                    $('#btn_stampa_excel').hide();
                    <%End If%>

                    <%If (Permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.Agenda_Operazioni_Blocco).Scrittura = True) Then%>
                    $('#<%=CB_BloccaOperazioni.ClientID %>').show();
                    $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').show();
                    <%Else%>
                    $('#<%=CB_BloccaOperazioni.ClientID %>').hide();
                    $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').hide();
                    <%End If%>

                    break;

                //CONCIMI
                case 11:

                    $('#selezione_stampa').show();
                    $('#<%=caricoscarico.ClientID %>').hide();
                    $('#<%=rbl_caricoscarico2.ClientID %>').hide();

                    //$('#scheda_annata_agraria').show();
                    $('#scheda_intervallo_temp').show();
                    $('#scheda_stampa_sing').hide();

                    $('#categoria_prodotto_lotto').hide();
                    $('#scheda_fitosanitari').hide();

                    $('#div_codarticolo_categ').hide();
                    $('#div_txtlotto').hide();
                    $('#div_altro').hide();
                    $('#div_ordinamento').hide();
                    $('#div_stampalotto').hide();
                    $('#div_raggruppamento').hide();

                    //$('#btn_stampa_excel').hide();
                    //$('#<%=CB_BloccaOperazioni.ClientID %>').hide();
                    //$('#<%=CB_BloccaOperazioni.ClientID %>').next('label').hide();


                    <%If (permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.Stampa_MovimentiMagazziniExcel).Lettura = True) Then%>
                    $('#btn_stampa_excel').show();
                    <%Else%>
                    $('#btn_stampa_excel').hide();
                    <%End If%>

                    <%If (Permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.Agenda_Operazioni_Blocco).Scrittura = True) Then%>
                    $('#<%=CB_BloccaOperazioni.ClientID %>').show();
                    $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').show();
                    <%Else%>
                    $('#<%=CB_BloccaOperazioni.ClientID %>').hide();
                    $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').hide();
                    <%End If%>

                    break;

                //FITO
                case 12:

                    $('#selezione_stampa').show();
                    $('#<%=caricoscarico.ClientID %>').hide();
                    $('#<%=rbl_caricoscarico2.ClientID %>').hide();

                    //$('#scheda_annata_agraria').show();
                    $('#scheda_intervallo_temp').show();
                    $('#scheda_stampa_sing').hide();

                    $('#categoria_prodotto_lotto').hide();
                    $('#scheda_fitosanitari').show();

                    $('#div_codarticolo_categ').hide();
                    $('#div_txtlotto').hide();
                    $('#div_altro').hide();
                    $('#div_ordinamento').hide();
                    $('#div_stampalotto').hide();
                    $('#div_raggruppamento').hide();

                    //$('#btn_stampa_excel').hide();
                    //$('#<%=CB_BloccaOperazioni.ClientID %>').hide();
                    //$('#<%=CB_BloccaOperazioni.ClientID %>').next('label').hide();


                     <%If (permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.Stampa_MovimentiMagazziniExcel).Lettura = True) Then%>
                    $('#btn_stampa_excel').show();
                    <%Else%>
                    $('#btn_stampa_excel').hide();
                    <%End If%>

                    <%If (Permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.Agenda_Operazioni_Blocco).Scrittura = True) Then%>
                    $('#<%=CB_BloccaOperazioni.ClientID %>').show();
                    $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').show();
                    <%Else%>
                    $('#<%=CB_BloccaOperazioni.ClientID %>').hide();
                    $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').hide();
                    <%End If%>

                    break;

                //RIEPILOGO PRODOTTI
                case 213:

                    $('#selezione_stampa').show();
                    $('#<%=caricoscarico.ClientID %>').hide();
                    $('#<%=rbl_caricoscarico2.ClientID %>').show();

                    //$('#scheda_annata_agraria').hide();
                    $('#scheda_intervallo_temp').show();
                    $('#scheda_stampa_sing').hide();

                    $('#categoria_prodotto_lotto').show();
                    $('#scheda_fitosanitari').show();

                    $('#div_codarticolo_categ').show();
                    $('#div_txtlotto').show();

                    $('#div_altro').show();
                    $('#div_ordinamento').hide();
                    $('#div_stampalotto').show();
                    $('#div_raggruppamento').hide();


                    $('#<%=CB_BloccaOperazioni.ClientID %>').hide();
                    $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').hide();


                     <%If (permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.Stampa_MovimentiMagazziniExcel).Lettura = True) Then%>
                    $('#btn_stampa_excel').show();
                    <%Else%>
                    $('#btn_stampa_excel').hide();
                    <%End If%>

                    <%If (Permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.Agenda_Operazioni_Blocco).Scrittura = True) Then%>
                    $('#<%=CB_BloccaOperazioni.ClientID %>').show();
                    $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').show();
                    <%Else%>
                    $('#<%=CB_BloccaOperazioni.ClientID %>').hide();
                    $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').hide();
                    <%End If%>

                    break;

            }

        }

        function caricaDate() {
            // recuperava queste date dalla sessione ==> ora dovrebbe prenderle dai campi hidden

            var QS_DataStampa = $(cIdQS_DataStampa).val();
            var QS_DataInizio = $(cIdQS_DataInizio).val();
            var QS_DataFine = $(cIdQS_DataFine).val();

            $('#<%=TxtStampa.ClientID %>').val(QS_DataStampa);
            $('#<%=TxtDataDa.ClientID %>').val(QS_DataInizio);
            $('#<%=TxtDataA.ClientID %>').val(QS_DataFine);

            $('#<%=TxtStampa.ClientID %>').datepicker('update');
            $('#<%=TxtDataDa.ClientID %>').datepicker('update');
            $('#<%=TxtDataA.ClientID %>').datepicker('update');

<%--            // riempio le date tramite querystring
            // var QS_DataStampa = getParameterByName('ds');
            // var QS_DataInizio = getParameterByName('di');
            // var QS_DataFine = getParameterByName('df');
            
            // date
            $.ajax({
                type: 'POST',
                url: 'Filtro_SchedeMagazzino_new.aspx/Carica_date',
                data: "{}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    var QS_DataStampa = r.d[0];
                    var QS_DataInizio = r.d[1];
                    var QS_DataFine = r.d[2];

                    $('#<%=TxtStampa.ClientID %>').val(QS_DataStampa);
                    $('#<%=TxtDataDa.ClientID %>').val(QS_DataInizio);
                    $('#<%=TxtDataA.ClientID %>').val(QS_DataFine);

                    $('#<%=TxtStampa.ClientID %>').datepicker('update');
                    $('#<%=TxtDataDa.ClientID %>').datepicker('update');
                    $('#<%=TxtDataA.ClientID %>').datepicker('update');

                    //$('.datepicker').datepicker('update');
                }
            });
--%>
        }

        function salvaDate() {

            // salvava queste date in sessione ==> ora dovrebbe metterle nei campi hidden

            $(cIdQS_DataStampa).val(elimina_null($('#<%=TxtStampa.ClientID %>').val()));
            $(cIdQS_DataInizio).val(elimina_null($('#<%=TxtDataDa.ClientID %>').val()));
            $(cIdQS_DataFine).val(elimina_null($('#<%=TxtDataA.ClientID %>').val()));

<%--            var param = kendo.stringify({
                dataStampa: elimina_null($('#<%=TxtStampa.ClientID %>').val()),
                dataInizio: elimina_null($('#<%=TxtDataDa.ClientID %>').val()),
                dataFine: elimina_null($('#<%=TxtDataA.ClientID %>').val())
            });

            $.ajax({
                type: 'POST',
                url: 'Filtro_SchedeMagazzino_new.aspx/Salva_Date',
                data: param,
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {}
            });
--%>
        }

        function salvaVariabiliSession() {

            // salvava queste date in sessione ==> ora dovrebbe metterle nei campi hidden

            $(cIds_piva).val(elimina_null($('#<%=Cmb_Impresa.ClientID %>').val()));
            $(cIds_sa_cod).val(elimina_null($('#<%=Cmb_CentroAziendale.ClientID %>').val()));
            $(cIds_fabbricato_cod).val(elimina_null($('#<%=Cmb_Magazzino.ClientID %>').val()));
            $(cIds_cat_prodotto).val(elimina_null($('#<%=cmb_CatProdotto.ClientID %>').val()));
            $(cIds_prodotto).val(elimina_null($('#<%=cmb_Prodotti.ClientID %>').val()));

<%--            var param = kendo.stringify({
                piva: elimina_null($('#<%=Cmb_Impresa.ClientID %>').val()),
                sa_cod: elimina_null($('#<%=Cmb_CentroAziendale.ClientID %>').val()),
                fabbricato_cod: elimina_null($('#<%=Cmb_Magazzino.ClientID %>').val()),
                cat_prodotto: elimina_null($('#<%=cmb_CatProdotto.ClientID %>').val()),
                prodotto: elimina_null($('#<%=cmb_Prodotti.ClientID %>').val())
            });

            $.ajax({
                type: 'POST',
                url: 'Filtro_SchedeMagazzino_new.aspx/Salva_Variabili_InSession',
                data: param,
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {
                }
            });
--%>
        }


        function carica_centro_az(centro_cod) {

            var piva = $('#<%=Cmb_Impresa.ClientID %>').val();

            //visto che non posso usare i controlli hidden dentro a Carica_Centro_Aziendale (sostitutivi della sessione), 
            //sono costretta a passarli da fuori e risalvarli sul successo

            //non è esattamente ben chiaro perché serva la doppia gestione di QS_Sa_Cod e s_sa_cod, ma per il momento la lascio così

            var param = kendo.stringify({
                Piva: piva,
                sa_cod: centro_cod,
                QS_Sa_Cod: $(cIdQs_Sa_Cod).val(),
                QS_Fabbricato_Cod: $(cIdQs_Fabbricato_Cod).val()
            });

            $.ajax({
                type: 'POST',
                url: 'Filtro_SchedeMagazzino_new.aspx/Carica_Centro_Aziendale',
                data: param,
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {

                    $(cIdQs_Sa_Cod).val(r.d[3]);
                    $(cIdQs_Fabbricato_Cod).val(r.d[4]);

                    $('#<%=Cmb_CentroAziendale.ClientID %>').empty();
                    $('#<%=Cmb_CentroAziendale.ClientID %>').append(r.d[0]);

                    var saCod = $('#<%=Cmb_CentroAziendale.ClientID %>').val();

                    $('#<%=Lbl_NumCentri.ClientID %>').empty();
                    $('#<%=Lbl_NumCentri.ClientID %>').append(r.d[1]);

                    carica_magazzini(piva, saCod, r.d[2]);

                }
            });

        }


        function carica_magazzini(piva, sa_cod, fabbricato_cod) {

            $.ajax({
                type: 'POST',
                url: 'Filtro_SchedeMagazzino_new.aspx/Carica_Magazzini',
                data: kendo.stringify({ piva: piva, sa_cod: sa_cod, fabbricato_cod: fabbricato_cod}),
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {

                    $('#<%=Cmb_Magazzino.ClientID %>').empty();
                    $('#<%=Cmb_Magazzino.ClientID %>').append(r.d[0]);

                    $('#<%=Lbl_NumMagazzini.ClientID %>').empty();
                    $('#<%=Lbl_NumMagazzini.ClientID %>').append(r.d[1]);

                    $('#<%=Cmb_Regioni.ClientID %>').val(r.d[2]);
                    if (r.d[0] == 0) {
                        $('#<%=Cmb_Magazzino.ClientID %>').val("");
                    }

                }
            });

        }

        function elimina_null(valore) {

            if (valore == null) {
                valore = "";
            }

            return valore;
        }


        $('#<%=Cmb_Impresa.ClientID %>').change(function () {
            carica_centro_az(0);
        });


        $('#<%=Cmb_CentroAziendale.ClientID %>').change(function () {
            carica_centro_az($('#<%=Cmb_CentroAziendale.ClientID %>').val());
        });


        //*** READY ***/
        $(document).ready(function () {

            $('#MainContent_ChkList_Categorie td').each(function () {
                $(this).addClass('radio');
            });

            $('#MainContent_Rbl_Arrotondamento td').each(function () {
                $(this).addClass('radio');
            });

            $('#MainContent_Rbl_Ordinamento td').each(function () {
                $(this).addClass('radio');
            });

            $('#MainContent_Rbl_StampaLotto td').each(function () {
                $(this).addClass('radio');
            });

            // gestione visibilità pannelli
            // inizializzazione...

            var tipoScheda = 0;

            // Leggo la tipologia di scheda selezionata da menu
            tipoScheda = parseInt($(cIdReportSelezionato).val());
            scheda_visibilita(tipoScheda);
            caricaDate();

            $('#Rbl_SchedaMagazzino input[type="checkbox"]').each(function () {
                if (parseInt($(this).val()) === tipoScheda) {
                    $(this).prop('checked', true);
                }
                else {
                    $(this).prop('checked', false);
                }
            });

            //$.ajax({
            //    type: 'POST',
            //    url: 'Filtro_SchedeMagazzino_new.aspx/Tipo_Scheda',
            //    data: "{}",
            //    contentType: 'application/json; charset=utf-8',
            //    cache: false,
            //    dataType: 'json', async: true,
            //    success: function (r) {
            //        tipoScheda = parseInt(r.d);
            //        scheda_visibilita(tipoScheda);
            //        caricaDate();

            //        $('#Rbl_SchedaMagazzino input[type="checkbox"]').each(function () {
            //            if (parseInt($(this).val()) === tipoScheda) {
            //                $(this).prop('checked', true);
            //            }
            //            else {
            //                $(this).prop('checked', false);
            //            }
            //        });
            //    }
            //});

//            <%If (Permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.Stampa_MovimentiMagazziniExcel).Lettura = True) Then%>
//                $('#btn_stampa_excel').show();
//            <%Else%>
//                $('#btn_stampa_excel').hide();
//            <%End If%>

//            <%If (Permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.Agenda_Operazioni_Blocco).Scrittura = True) Then%>
//                $('#<%=CB_BloccaOperazioni.ClientID %>').show();
//                $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').show();
//            <%Else%>
//                $('#<%=CB_BloccaOperazioni.ClientID %>').hide();
//                $('#<%=CB_BloccaOperazioni.ClientID %>').next('label').hide();
//            <%End If%>
        });


        //... on change
        $('#Rbl_SchedaMagazzino input[type="checkbox"]').change(function () {
            //alert();
            $('input[name="chk_group[]"]').not(this).prop('checked', false);

            var tipoScheda = parseInt($(this).val());
            scheda_visibilita(tipoScheda);

            $(cIdTipo_Scheda).val(tipoScheda);
            $(cIdReportSelezionato).val(tipoScheda);

            $.ajax({
                type: 'POST',
                url: 'Filtro_SchedeMagazzino_new.aspx/Salva_tipo_scheda',
                data: kendo.stringify({ value: tipoScheda }),
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {}
            });

        });


        $('#btn_CercaImpresa').click(function () {
            //$('#<=ImgBtn_CercaImpresa.ClientID %>').click();

            if ($('#<%=Txt_Impresa.ClientID %>').val() == '') {
                alert('Applicare un filtro per la ricerca Impresa');
            } else {
                // riempio le tendine
                $.ajax({
                    type: 'POST',
                    url: 'Filtro_SchedeMagazzino_new.aspx/Carica_ImpreseWS',
                    data: "{testo:'" + $('#<%=Txt_Impresa.ClientID %>').val() + "'}",
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: false,
                    success: function (r) {
                        $('#<%=Cmb_Impresa.ClientID %>').empty();
                        $('#<%=Cmb_Impresa.ClientID %>').append(r.d[0]);

                        $('#<%=Lbl_NumImprese.ClientID %>').empty();
                        $('#<%=Lbl_NumImprese.ClientID %>').append(r.d[1]);

                        carica_centro_az(0);
                        //$('.selectpicker').selectpicker('refresh');
                    }
                });
            }
        });


        $('#btn_CercaProdotto').click(function () {
            var flag = true;

            if ($('#<%=cmb_CatProdotto.ClientID %>').val() == '') {
                alert('Selezionare una categoria di magazzino');
                flag = false;
            }

            if (flag) {

                //passo le date
                var gruppo_date = "";
                gruppo_date = $('#<%=TxtDataDa.ClientID %>').val() + "," + $('#<%=TxtStampa.ClientID %>').val() + "," + $('#<%=TxtDataA.ClientID %>').val();

                var param = kendo.stringify({
                    testo: $('#<%=Txt_ProdottoCerca.ClientID %>').val(),
                    gruppo_date: gruppo_date,
                    cat_prod: $('#<%=cmb_CatProdotto.ClientID %>').val(),
                    impresa: $('#<%=Cmb_Impresa.ClientID %>').val(),
                    sa_cod: $('#<%=Cmb_CentroAziendale.ClientID %>').val(),
                    fabbricato_cod: $('#<%=Cmb_Magazzino.ClientID %>').val(),
                    tipo_scheda: $(cIdTipo_Scheda).val()
                });

                // riempio le tendine
                $.ajax({
                    type: 'POST',
                    url: 'Filtro_SchedeMagazzino_new.aspx/Carica_ProdottiWS',
                    data: param,
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: true,
                    success: function (r) {
                        $('#<%=cmb_Prodotti.ClientID %>').empty();
                        $('#<%=cmb_Prodotti.ClientID %>').append(r.d[0]);

                        $('#<%=Lbl_NumProdotti.ClientID %>').empty();
                        $('#<%=Lbl_NumProdotti.ClientID %>').append(r.d[1]);
                        if (r.d[1] === 0) {
                            alert('Non ci sono prodotti della categoria selezionata nel Magazzino selezionato.');
                        }
                    }
                });
            }

        });

        //onclick = "$('#<=ImgBtn_ProdottiCerca.ClientID %>').click();"


        $('#btn_stampa_excel').click(btn_stampa_excel_click);

        function btn_stampa_excel_click() {
            var flag_ok = true;

            // Azzero tutte le label custom_val
            $("input").each(function (i, obj) {
                $(this).css('border', '1px solid #ccc');
                $(this).parent().children().css('border-color', '#ccc');
                $(this).parent().children('label.error2').remove();
            });

            $('label.error2').each(function (i, obj) {
                $(this).remove();
            });

            if (jQuery.isEmptyObject($('#<%=Cmb_Impresa.ClientID %>').val()) || $('#<%=Cmb_Impresa.ClientID %>').val() == "") {
//		        $('#<%=Cmb_Impresa.ClientID %>').parent().append('<label id="<%=Cmb_Impresa.ClientID %>-error" class="custom_val error2" for="<%=Cmb_Impresa.ClientID %>">Selezionare una Impresa</label>');
//		        $('#<%=Cmb_Impresa.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                alert("Selezionare un'impresa");
                flag_ok = false;
            }

            var val_check;
            $('#Rbl_SchedaMagazzino input[name="chk_group[]"]:checked').each(function () {
                val_check = $(this).val();
            });

            if (val_check == 9) {

                if ($('#<%=TxtDataDa.ClientID %>').val() == "" || $('#<%=TxtDataA.ClientID %>').val() == "") {
//		            $('#<%=TxtDataDa.ClientID %>').parent().append('<label id="<%=TxtDataDa.ClientID %>-error" class="custom_val error2" for="<%=TxtDataDa.ClientID %>">E\' necessario specificare l\'intervallo temporale in cui stampare i Movimenti di Magazzino</label>');
//		            $('#<%=TxtDataDa.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                    alert("E\' necessario specificare l\'intervallo temporale in cui stampare i Movimenti di Magazzino");
                    flag_ok = false;
                }

                // Controllo se le date sono corrette temporalmente
                var TxtValiditaInizio = $('#<%=TxtDataDa.ClientID %>').val().split("/");
                var TxtValiditaFine = $('#<%=TxtDataA.ClientID %>').val().split("/");

                ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
                fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);

                if (ini > fin) {
//		            $('#<%=TxtDataDa.ClientID %>').parent().append('<label id="<%=TxtDataDa.ClientID %>-error" class="custom_val error2" for="<%=TxtDataDa.ClientID %>">La data di inizio dell\'intervallo non può essere superiore alla data di fine</label>');
//		            $('#<%=TxtDataDa.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                    alert("La data di inizio dell\'intervallo non può essere superiore alla data di fine");
                    flag_ok = false;
                }

                var fl = false;
                // ci deve essere almeno un check nella lista tipo magazzino
                $('input[id^=MainContent_ChkList_Categorie]').each(function () {
                    if ($(this).prop('checked'))
                        fl = true;
                });

                if (!fl) {
                    //$('#div_codarticolo_categ').append('<label id="div_codarticolo_categ-error" class="custom_val error2" for="div_codarticolo_categ">E\' necessario selezionare almeno una categoria di magazzino</label>');
                    alert("E\' necessario selezionare almeno una categoria di magazzino");
                    flag_ok = false;
                }

            }


            if (flag_ok) {

                salvaVariabiliSession();

                salvaDate();

                // valido l'esportazione in excel
                $.ajax({
                    type: 'POST',
                    url: 'Filtro_SchedeMagazzino_new.aspx/Validazione_Excel',
                    data: kendo.stringify({ tipo_scheda: $(cIdTipo_Scheda).val() }),
                    contentType: 'application/json; charset=utf-8',
                    cache: false,
                    dataType: 'json', async: false,
                    success: function (r) {

                        if (r.d.RispostaOK === true) {
                            $('#<%=ImgBtn_StampaExcel.ClientID %>').click();
                        } else {
                            //alert(r.d.Errore);
                            $('#btn_stampa_excel').hide();
                        }

                    }
                });
            }
        }

        $("#btn_stampa").click(btn_stampa_click);

        function btn_stampa_click() {

            var flag_ok = true;

            var val_check;
            $('#Rbl_SchedaMagazzino input[name="chk_group[]"]:checked').each(function () {
                val_check = $(this).val();
            });

            // Azzero tutte le label custom_val
            $("input").each(function (i, obj) {
                $(this).css('border', '1px solid #ccc');
                $(this).parent().children().css('border-color', '#ccc');
                $(this).parent().children('label.error2').remove();
            });

            $('label.error2').each(function (i, obj) {
                $(this).remove();
            });

            if (jQuery.isEmptyObject($('#<%=Cmb_Impresa.ClientID %>').val()) || $('#<%=Cmb_Impresa.ClientID %>').val() == "") {
                alert("Selezionare un'impresa");
                flag_ok = false;
            }

                if (val_check != 213) {

                    if (jQuery.isEmptyObject($('#<%=Cmb_CentroAziendale.ClientID %>').val()) || $('#<%=Cmb_CentroAziendale.ClientID %>').val() == "" || $('#<%=Cmb_CentroAziendale.ClientID %>').val() == "0") {
                        alert("Selezionare un centro aziendale");
                        flag_ok = false;
                    }

                    if (jQuery.isEmptyObject($('#<%=Cmb_Magazzino.ClientID %>').val()) || $('#<%=Cmb_Magazzino.ClientID %>').val() == "" || $('#<%=Cmb_Magazzino.ClientID %>').val() == "0") {
                        alert("Selezionare un magazzino");
                        flag_ok = false;
                    }

                }



                switch (parseInt(val_check)) {
                    case 9:

                        if ($('#<%=TxtDataDa.ClientID %>').val() == "" || $('#<%=TxtDataA.ClientID %>').val() == "") {
		                    //		                    $('#<%=TxtDataDa.ClientID %>').parent().append('<label id="<%=TxtDataDa.ClientID %>-error" class="custom_val error2" for="<%=TxtDataDa.ClientID %>">E\' necessario specificare l\'intervallo temporale in cui stampare i Movimenti di Magazzino</label>');
		                    //		                    $('#<%=TxtDataDa.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                            alert("E\' necessario specificare l\'intervallo temporale in cui stampare i Movimenti di Magazzino");
                            flag_ok = false;
                        }

                        // Controllo se le date sono corrette temporalmente
                        var TxtValiditaInizio = $('#<%=TxtDataDa.ClientID %>').val().split("/");
                        var TxtValiditaFine = $('#<%=TxtDataA.ClientID %>').val().split("/");

                        ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
                        fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);

                        if (ini > fin) {
		                    //$('#<%=TxtDataDa.ClientID %>').parent().append('<label id="<%=TxtDataDa.ClientID %>-error" class="custom_val error2" for="<%=TxtDataDa.ClientID %>">La data di inizio dell\'intervallo non può essere superiore alla data di fine</label>');
		                    //$('#<%=TxtDataDa.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                            alert("La data di inizio dell\'intervallo non può essere superiore alla data di fine");
                            flag_ok = false;
                        }

                        break;


                    case 10:

                        if ($('#<%=TxtStampa.ClientID %>').val() == "") {
		                    //$('#<%=TxtStampa.ClientID %>').parent().append('<label id="<%=TxtStampa.ClientID %>-error" class="custom_val error2" for="<%=TxtStampa.ClientID %>">Il campo non può essere vuoto</label>');
		                    //$('#<%=TxtStampa.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                            alert("Il campo data non può essere vuoto");
                            flag_ok = false;
                        }

                        break;

                    case 11:

                        if ($('#<%=TxtDataDa.ClientID %>').val() == "" || $('#<%=TxtDataA.ClientID %>').val() == "") {
		                    //$('#<%=TxtDataDa.ClientID %>').parent().append('<label id="<%=TxtDataDa.ClientID %>-error" class="custom_val error2" for="<%=TxtDataDa.ClientID %>">E\' necessario specificare l\'intervallo temporale in cui stampare i Movimenti di Magazzino</label>');
		                    //$('#<%=TxtDataDa.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                            alert("E\' necessario specificare l\'intervallo temporale in cui stampare i Movimenti di Magazzino");
                            flag_ok = false;
                        }

                        // Controllo se le date sono corrette temporalmente
                        var TxtValiditaInizio = $('#<%=TxtDataDa.ClientID %>').val().split("/");
                        var TxtValiditaFine = $('#<%=TxtDataA.ClientID %>').val().split("/");

                        ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
                        fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);

                        if (ini > fin) {
		                    //$('#<%=TxtDataDa.ClientID %>').parent().append('<label id="<%=TxtDataDa.ClientID %>-error" class="custom_val error2" for="<%=TxtDataDa.ClientID %>">La data di inizio dell\'intervallo non può essere superiore alla data di fine</label>');
		                    //$('#<%=TxtDataDa.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                            alert("La data di inizio dell\'intervallo non può essere superiore alla data di fine");
                            flag_ok = false;
                        }

                        break;



                    case 12:

                        if ($('#<%=TxtDataDa.ClientID %>').val() == "" || $('#<%=TxtDataA.ClientID %>').val() == "") {
		                    //$('#<%=TxtDataDa.ClientID %>').parent().append('<label id="<%=TxtDataDa.ClientID %>-error" class="custom_val error2" for="<%=TxtDataDa.ClientID %>">E\' necessario specificare l\'intervallo temporale in cui stampare i Movimenti di Magazzino</label>');
		                    //$('#<%=TxtDataDa.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                            alert("E\' necessario specificare l\'intervallo temporale in cui stampare i Movimenti di Magazzino");
                            flag_ok = false;
                        }

                        // Controllo se le date sono corrette temporalmente
                        var TxtValiditaInizio = $('#<%=TxtDataDa.ClientID %>').val().split("/");
                        var TxtValiditaFine = $('#<%=TxtDataA.ClientID %>').val().split("/");

                        ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
                        fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);

                        if (ini > fin) {
		                    //$('#<%=TxtDataDa.ClientID %>').parent().append('<label id="<%=TxtDataDa.ClientID %>-error" class="custom_val error2" for="<%=TxtDataDa.ClientID %>">La data di inizio dell\'intervallo non può essere superiore alla data di fine</label>');
		                    //$('#<%=TxtDataDa.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                            alert("La data di inizio dell\'intervallo non può essere superiore alla data di fine");
                            flag_ok = false;
                        }

                        break;

                    case 213:

                        if ($('#<%=TxtDataDa.ClientID %>').val() == "" || $('#<%=TxtDataA.ClientID %>').val() == "") {
		                    //$('#<%=TxtDataDa.ClientID %>').parent().append('<label id="<%=TxtDataDa.ClientID %>-error" class="custom_val error2" for="<%=TxtDataDa.ClientID %>">E\' necessario specificare l\'intervallo temporale in cui stampare i Movimenti di Magazzino</label>');
		                    //$('#<%=TxtDataDa.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                            alert("E\' necessario specificare l\'intervallo temporale in cui stampare i Movimenti di Magazzino");
                            flag_ok = false;
                        }

                        // Controllo se le date sono corrette temporalmente
                        var TxtValiditaInizio = $('#<%=TxtDataDa.ClientID %>').val().split("/");
                        var TxtValiditaFine = $('#<%=TxtDataA.ClientID %>').val().split("/");

                        ini = new Date(TxtValiditaInizio[2], TxtValiditaInizio[1] - 1, TxtValiditaInizio[0]);
                        fin = new Date(TxtValiditaFine[2], TxtValiditaFine[1] - 1, TxtValiditaFine[0]);

                        if (ini > fin) {
		                    //$('#<%=TxtDataDa.ClientID %>').parent().append('<label id="<%=TxtDataDa.ClientID %>-error" class="custom_val error2" for="<%=TxtDataDa.ClientID %>">La data di inizio dell\'intervallo non può essere superiore alla data di fine</label>');
		                    //$('#<%=TxtDataDa.ClientID %>').closest("input").css('border', '1px solid #D41E1A');
                            alert("La data di inizio dell\'intervallo non può essere superiore alla data di fine");
                            flag_ok = false;
                        }

                        break;
                }

                /*
		        var fl = false;
		        // ci deve essere almeno un check nella lista tipo magazzino
		        $('input[id^=MainContent_ChkList_Categorie]').each(function () {
		            if ($(this).prop('checked'))
		                fl = true;
		        });

		        if (!fl) {
		            $('#div_codarticolo_categ').append('<label id="div_codarticolo_categ-error" class="custom_val error2" for="div_codarticolo_categ">E\' necessario selezionare almeno una categoria di magazzino</label>');
		            alert("E\' necessario selezionare almeno una tipologia di Stampa di magazzino");
		            flag_ok = false;
		        }
                */

            if (flag_ok) {

                // salvo le variabili in session
                salvaVariabiliSession();

                salvaDate();

                // proseguo con la stampa crystal
                $('#<%=ImgBtn_Stampa.ClientID %>').click();
            }

        }

        $('#<%=btn_AnnataPrecedente.ClientID %>').click(function () {
            var dataInizio = $('#<%=TxtDataDa.ClientID %>').val();
            var dataFine = $('#<%=TxtDataA.ClientID %>').val();

            if (dataInizio !== "") {
                var dataNewI = cambiaAnnata(dataInizio, -1);
                $('#<%=TxtDataDa.ClientID %>').val(dataNewI);
            }

            if (dataFine !== "") {
                var dataNewF = cambiaAnnata(dataFine, -1);
                $('#<%=TxtDataA.ClientID %>').val(dataNewF);
            }
        });

        $('#<%=btn_AnnataSuccessiva.ClientID %>').click(function () {
            var dataInizio = $('#<%=TxtDataDa.ClientID %>').val();
            var dataFine = $('#<%=TxtDataA.ClientID %>').val();

            if (dataInizio !== "") {
                var dataNewI = cambiaAnnata(dataInizio, 1);
                $('#<%=TxtDataDa.ClientID %>').val(dataNewI);
            }

            if (dataFine !== "") {
                var dataNewF = cambiaAnnata(dataFine, 1);
                $('#<%=TxtDataA.ClientID %>').val(dataNewF);
            }
        });

        function cambiaAnnata(dataInput, anniDaAggiungere) {
            // se anniDaAggiungere è negativo di fatto sottrae anni
            var separator = "/";
            var aoDate = dataInput.split(separator);

            var day = aoDate[0];
            var month = aoDate[1];
            var year = aoDate[2];

            var yearNew = parseInt(year) + parseInt(anniDaAggiungere);
            var dataNew = [day, month, yearNew].join(separator);

            return dataNew;
        }



    </script>
</asp:Content>
