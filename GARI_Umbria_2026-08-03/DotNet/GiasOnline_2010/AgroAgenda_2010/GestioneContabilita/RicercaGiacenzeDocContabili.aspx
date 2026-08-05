<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/AgendaBootstrap.Master" CodeBehind="RicercaGiacenzeDocContabili.aspx.vb" Inherits="AgroAgenda_2010.RicercaGiacenzeDocContabili" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .errorClass {
            border-color: #D41E1A;
            border-width: 1px;
            border-style: dotted;
            background-color: Yellow;
        }

        .buttonClass {
            margin: 0 0 10px 1px;
        }

        .jumbotron {
            margin-bottom: 0 !important;
        }

        .fixed-header {
            top: 0;
            position: fixed;
            width: auto;
            z-index: 1;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoLettura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoScrittura" />
    <asp:HiddenField runat="server" ID="hf_UtenteAbilitatoPomodoro" />

    <div id="searchArea" class="panel-group searchArea" style="display: none;">
        <ul id="filterPanel">
            <li>
                <span class="k-link k-state-selected k-selected">Filtri Ricerca</span>
                <div class="panel-body" style="padding-top: 5px;">

                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="form-horizontal" style="margin-top: 5px; margin-bottom: 5px;">

                                <div id="tabstrip_Filtri">
                                    <ul class="nav nav-tabs" role="tablist" id="tabs">
                                        <li class="k-state-active k-active"  id="tab1">Filtri Generali
                                        </li>
                                        <li id="tab2">Filtro Contatti
                                        </li>
                                        <li id="tab3">Filtro Prodotti
                                        </li>
                                    </ul>

                                    <!-- tab Filtro Documenti -->
                                    <div class="tab-pane fade active in" id="tabFiltroDocumento" style="overflow: auto">
                                        <div class="jumbotron">

                                            <div class="row" id="rowCentroAziendale">
                                                <div class="col-lg-9 col-sm-9">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <label class="input-group-addon lbl_required" id="lbl_centro_aziendale" for="id_multiselCentroAziendale">Centro Aziendale:</label>
                                                                <select name="multiselCentroAziendale" multiple="multiple" id="id_multiselCentroAziendale" class="form-control" data-placeholder="Tutti"></select>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" style="display: none;">
                                                <div class="col-lg-2 col-sm-4">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon lbl_required" id="lbl_Nr" for="TxtDocNumeroSin">Numero:</span>
                                                                <asp:TextBox ID="TxtDocNumeroSin" runat="server" CssClass="form-control" MaxLength="255">
                                                                </asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-1 col-sm-3">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <asp:TextBox ID="TxtDocNumero" runat="server" type="number" min="0" step="1" CssClass="form-control" MaxLength="8">
                                                                </asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-1 col-sm-3">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <asp:TextBox ID="TxtDocNumeroDes" runat="server" CssClass="form-control" MaxLength="255">
                                                                </asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-2 col-sm-6">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon lbl_required" id="lbl_NrRiga" for="txt_NrRiga">Numero riga:</span>
                                                                <asp:TextBox ID="TxtNrRiga" runat="server" CssClass="form-control" MaxLength="255">
                                                                </asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-6 col-sm-6">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                &nbsp;
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-4 col-md-4 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon lbl_required" id="lbl_DataRifDal" for="Txt_DataRegDal">Da data documento:</span>
                                                                <input id="Txt_DataRegDal" name="Txt_DataRegDal" class="kendoCalendar" style="width: 100%;" maxlength="10" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-4 col-md-4 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon lbl_required" id="lbl_DataRifAl" for="Txt_DataRegAl">A data documento:</span>
                                                                <input id="Txt_DataRegAl" name="Txt_DataRegAl" class="kendoCalendar" style="width: 100%;" maxlength="10" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row" id="rowDescrizioneDocumento">
                                                <div class="col-lg-9 col-sm-9">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon lbl_required" id="lbl_operazione_des" for="txt_operazione">Descrizione:</span>
                                                                <asp:TextBox ID="txt_operazione" ClientIDMode="Static" runat="server" CssClass="form-control">  </asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-lg-4 col-md-12 col-sm-12" id="div_chk_ddt_non_fatturati">
                                                    <div class="form-group">
                                                        <label class="lbl_required" id="lbl_ddt_non_fatturati" for="cb_ddt_non_fatturati">Solo DDT non fatturati</label>
                                                        <input type="checkbox" id="cb_ddt_non_fatturati" name="cb_ddt_non_fatturati" class="kendoSwitch" />
                                                    </div>
                                                </div>
                                                <div class="col-lg-4 col-md-12 col-sm-12" id="div_chk_ordini_non_spediti">
                                                    <div class="form-group">
                                                        <label class="lbl_required" id="lbl_ordini_non_spediti" for="cb_ordini_non_Spediti">Solo Ordini non evasi</label>
                                                        <input type="checkbox" id="cb_ordini_non_Spediti" name="cb_ordini_non_Spediti" class="kendoSwitch" />
                                                    </div>
                                                </div>
                                            </div>


                                        </div>
                                    </div>

                                    <!-- tab Filtro cliente -->
                                    <div class="tab-pane fade in" id="tabFiltroClienti" style="overflow: auto">
                                        <div class="jumbotron">
                                            <div class="row">
                                                <div class="col-lg-12 col-md-12 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <label class="input-group-addon lbl_required" for="multiselContatti">Contatti:</label>
                                                                <select name="multiselContatti" multiple="multiple" id="multiselContatti" class="form-control"></select>
                                                                <label class="input-group-addon ">(Inserire 3 caratteri della descrizione)</label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- tab Filtro Prodotti -->
                                    <div class="tab-pane fade in tabFiltroProdotti" id="tabFiltroProdotti" style="overflow: auto">
                                        <div class="jumbotron">
                                            <div class="row" id="row_filtro_categorie">
                                                <div class="col-lg-6 col-md-6 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <label class="input-group-addon lbl_required" for="id_multiselCategorie">Categorie:</label>
                                                                <select name="multiselCategorie" multiple="multiple" id="id_multiselCategorie" class="form-control"></select>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-12 col-md-12 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <label class="input-group-addon lbl_required" for="id_multiselProdotti">Prodotti:</label>
                                                                <select name="multiselProdotti" multiple="multiple" id="id_multiselProdotti" class="form-control"></select>
                                                                <label class="input-group-addon ">(Inserire 3 caratteri della descrizione)</label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-3 col-md-3 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <label class="input-group-addon lbl_required" for="id_multiselSpecie">Specie:</label>
                                                                <select name="multiselSpecie" multiple="multiple" id="id_multiselSpecie" class="form-control"></select>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-lg-3 col-md-3 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <label class="input-group-addon lbl_required" for="id_multiselVarieta">Varietà:</label>
                                                                <select name="multiselVarieta" multiple="multiple" id="id_multiselVarieta" class="form-control"></select>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-4 col-md-4 col-sm-12">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <span class="input-group-addon lbl_required" id="lbl_DataGiacenza" for="Txt_DataGiacenza">Data Giacenza:</span>
                                                                <input id="Txt_DataGiacenza" name="Txt_DataGiacenza" class="kendoCalendar" style="width: 100%;" maxlength="10" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="boxFiltroLotto col-md-4">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <label class="input-group-addon" id="lbl_Lotto" for="Txt_Lotto">Lotto</label>
                                                                <input type="text" name="Txt_Lotto" id="Txt_Lotto" class="form-control kendoTextBox" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-lg-4 col-md-12 col-sm-12" id="div_chk_prod_giacenza">
                                                    <div class="form-group">
                                                        <label class="lbl_required" id="lbl_prod_giacenza" for="cb_prod_giacenza">Mostra solo prodotti in giacenza</label>
                                                        <input type="checkbox" id="cb_prod_giacenza" name="cb_prod_giacenza" class="kendoSwitch" />
                                                    </div>
                                                </div>
                                                <div class="col-lg-8 col-sm-8">
                                                    <div class="form-horizontal">
                                                        <div class="form-group">
                                                            <div class="input-group">
                                                                <label class="input-group-addon lbl_required" id="lbl_fabbricato" for="id_ddlFabbricato">Magazzino</label>
                                                                <select name="ddlFabbricato" multiple="multiple" id="id_ddlFabbricato" class="form-control" data-placeholder="Tutti"></select>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="btn btn-success buttonClass" id="btn_ricerca" style="margin-left: 10px;">
                                <span class="fa fa-search"></span>Ricerca
                            </div>
                            <div class="btn btn-warning buttonClass" style="margin-left: 2%" id="btn_pulisci_filtri">
                                <span class="fa fa-eraser"></span>Pulisci Filtri
                            </div>
                        </div>
                    </div>
                </div>
            </li>
        </ul>

        <div id="girdAreaDettaglio" class="panel-group gridAreaDettaglio" style="display: none;">
            <div class="panel-body" style="overflow: auto; margin-top: 10px; margin-bottom: 70px;">
                <div id="tab_dettaglio_griglia_report_vendite"></div>
                <div id="tab_dettaglio_griglia_report_acquisti"></div>
                <div id="tab_dettaglio_griglia_report_conferimenti"></div>
            </div>
        </div>

        <!-- fine container -->
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <!--dialogs varie-->
    <div id="confermaEliminazioneDialog"></div>
    <div id="confermaSbloccoDialog"></div>


    <!-- Hidden Fields -->
    <input type="hidden" id="hdPiva" runat="server" />
    <input type="hidden" id="hdType" runat="server" />
    <input type="hidden" id="hdDocType" runat="server" />
    <input type="hidden" id="hdMod" runat="server" />
    <input type="hidden" id="hdUtilizzo" runat="server" />
    <input type="hidden" id="hdUtenteAbilitatoBlocco" runat="server" />
    <input type="hidden" id="hdUtenteAbilitatoSblocco" runat="server" />
    <input type="hidden" id="hdContattiAcc4ConGerarchia" runat="server" />

    <input type="hidden" id="hdSaCod" runat="server" />
    <input type="hidden" id="hdDataGiacenza" runat="server" />
    <input type="hidden" id="hdInGiacenza" runat="server" />
    <input type="hidden" id="hdContatto" runat="server" />
    <input type="hidden" id="hdCategProdotto" runat="server" />
    <input type="hidden" id="hdSpecieVeg" runat="server" />
    <input type="hidden" id="hdVarieta" runat="server" />
    <input type="hidden" id="hdFabbricatoCod" runat="server" />
    <input type="hidden" id="hdLotto" runat="server" />
    <input type="hidden" id="hdDataInizioRangeDDT" runat="server" />
    <input type="hidden" id="hdDataFineRangeDDT" runat="server" />

    <script type="text/javascript">
        var objP_server = '<%=objparametri_server_string %>';
        var cIdPiva = "#<%=hdPiva.ClientID() %>";
        var cType = "#<%=hdType.ClientID() %>";
        var cDocType = "#<%=hdDocType.ClientID() %>";
        var cIdMod = "#<%=hdMod.ClientID() %>";
        var cIdUtilizzo = "#<%=hdUtilizzo.ClientID() %>";
        var cUtenteAbilitatoBlocco = "#<%=hdUtenteAbilitatoBlocco.ClientID() %>";
        var cUtenteAbilitatoSblocco = "#<%=hdUtenteAbilitatoSblocco.ClientID() %>";
        var ccontattiAcc4ConGerarchia = "#<%=hdContattiAcc4ConGerarchia.ClientID() %>";

        var cIdSaCod = "#<%=hdSaCod.ClientID() %>";
        var cIdDataGiacenza = "#<%=hdDataGiacenza.ClientID() %>";
        var cIdInGiacenza = "#<%=hdInGiacenza.ClientID() %>";
        var cIdContatto = "#<%=hdContatto.ClientID() %>";
        var cIdCategProdotto = "#<%=hdCategProdotto.ClientID() %>";
        var cIdSpecieVeg = "#<%=hdSpecieVeg.ClientID() %>";
        var cIdVarieta = "#<%=hdVarieta.ClientID() %>";
        var cIdFabbricatoCod = "#<%=hdFabbricatoCod.ClientID() %>";
        var cIdLotto = "#<%=hdLotto.ClientID() %>";
        var cIdDataInizioRangeDDT = "#<%=hdDataInizioRangeDDT.ClientID() %>";
        var cIdDataFineRangeDDT = "#<%=hdDataFineRangeDDT.ClientID() %>";

    </script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/funzioni_comuni.js")) %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin(ResolveClientUrl("~/ScriptsGestionali/leggitabelle_ws_client.js")) %>"></script>

    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RicercaGiacenzeDocContabili_ws_client.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RicercaGiacenzeDocContabili_globali.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RicercaGiacenzeDocContabili.js") %>"></script>
    <script type="text/javascript" src="<%= AgronicaControlli_2010.Minify.LinkJsMin("RicercaGiacenzeDocContabili_jQueryDocReady.js") %>"></script>

    <script id="templateBtnConfermaRigheScelte" type="text/x-kendo-template">
        <div class="btn btn-success" id="BtnConfermaRigheScelte" style="margin-right: 3px;" onclick="InviaRigheScelte()">
            CONFERMA SELEZIONE E PROCEDI
        </div>
    </script>

</asp:Content>
