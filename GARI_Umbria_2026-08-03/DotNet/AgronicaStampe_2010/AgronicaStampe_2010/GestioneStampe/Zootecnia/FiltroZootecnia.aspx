<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Master/StampeBootstrap.Master"
    CodeBehind="FiltroZootecnia.aspx.vb" Inherits="AgronicaStampe_2010.FiltroZootecnia"
    EnableEventValidation="false" %>

<%@ Import Namespace="AgronicaCoreDataProvider" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row" id="pulsanti_in_alto">
        <div class="col-md-12" style="margin-bottom: 15px;">
            <div style="float: right;">
                 <div class="btn btn-warning" id="btn_filtri" style="float: left; margin-right: 5px;">
                     <i class="fa fa-filter"></i>Pulisci filtri
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
                        <div id="Rbl_ReportZootecnia">
                            <div class="radio">
                                <label>
                                    <input type="checkbox" name="chk_group[]" value="259" />Sintesi Partite</label>
                            </div>
                            <div class="radio">
                                <label>
                                    <input type="checkbox" name="chk_group[]" value="260" checked="checked" />Dettaglio Partita</label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <hr />
        <!--  FILTRI DATE -->
        <div class="row">
            <h4 id="FiltroDateText">Filtro periodo macellazione</h4>
            <div id="scheda_intervallo_temp">
                <div id="data_inizio" class="col-lg-2 col-md-4 col-sm-12">
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
                <div class="col-lg-4 col-md-2"></div>
                <div id="data_fine" class="col-lg-2 col-md-4 col-sm-12">
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
                <div id="stato_partite" class="col-lg-2 col-md-4 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_stato_partite" for="Cmb_statoPartite">
                                    <i class="fa fa-calendar"></i>Filtro stato partite </span>
                                <asp:DropDownList ID="Cmb_statoPartite" runat="server" CssClass="form-control" data-live-search="true"
                                    aria-describedby="lbl_stato_partite">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--  FILTRI CAPI -->
        <div id="FiltroCapi" class="row">
            <h4 id="FiltroCapoText">Filtro Capi</h4>
            <div class="col-lg-6 col-md-6 col-sm-12">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon alert-info" id="lbl_Razza" for="Cmb_Razza">Razza
                            </span>
                            <asp:DropDownList ID="Cmb_Razza" runat="server" CssClass="form-control" data-live-search="true"
                                aria-describedby="lbl_Razza">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-2 col-md-2 col-sm-4">
                <div class="form-horizontal">
                    <div class="form-group">
                        <div class="input-group">
                            <span class="input-group-addon alert-info" id="lbl_Sesso" for="Cmb_Sesso">Sesso
                            </span>
                            <asp:DropDownList ID="Cmb_Sesso" runat="server" CssClass="form-control" data-live-search="true"
                                aria-describedby="lbl_Sesso">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--  FILTRI IMPRESA -->
        <div id="ricerca_azienda">
            <div class="row">
                <h4>Filtro Impresa, Centro e Stalla</h4>
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
            </div>
            <div class="row">
               <div class="col-lg-2 col-md-6 col-sm-12">
                   <div class="btn btn-info" id="btn_CercaImpresa">
                       <i class="fa fa-search"></i>Cerca Impresa
                   </div>
                   <asp:ImageButton ID="ImgBtn_CercaImpresa" runat="server" ImageUrl="../../AB_Immagini/icone32/lente.ico"
                       Style="display: none;"></asp:ImageButton>
               </div>
            </div>
            <div id="risultato_azienda" class="row">
                <div class="col-lg-6 col-md-6 col-sm-12">
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
                <div id="FiltroCentro" class="col-lg-4 col-md-6 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_Centro" for="Cmb_Centro">Centro
                                </span>
                                <asp:DropDownList ID="Cmb_Centro" runat="server" CssClass="form-control" data-live-search="true"
                                    aria-describedby="lbl_Centro">
                                </asp:DropDownList>
                                <span class="input-group-addon alert-info">
                                    <asp:Label ID="Lbl_NumCentro" runat="server">0</asp:Label>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="FiltroStalla" class="col-lg-4 col-md-6 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_Stalla" for="Cmb_Stalla">Stalla
                                </span>
                                <asp:DropDownList ID="Cmb_Stalla" runat="server" CssClass="form-control" data-live-search="true"
                                    aria-describedby="lbl_Stalla">
                                </asp:DropDownList>
                                <span class="input-group-addon alert-info">
                                    <asp:Label ID="Lbl_NumStalle" runat="server">0</asp:Label>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--  FILTRI FORNITORI -->
        <div id="FiltroFornitori">
            <div class="row">
                <h4 id="FiltroFornitoriText">Filtro Fornitori</h4>
                <div class="col-lg-4 col-md-6 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_Txt_ForFat" for="Txt_ForFat">Ricerca
                                    Fornitore Fatturazione </span>
                                <asp:TextBox ID="Txt_ForFat" runat="server" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-2"></div>
                <div class="col-lg-4 col-md-6 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_Txt_ForPro" for="Txt_ForPro">Ricerca
                                    Fornitore Provenienza </span>
                                <asp:TextBox ID="Txt_ForPro" runat="server" CssClass="form-control">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
               <div class="col-lg-2 col-md-6 col-sm-12">
                   <div class="btn btn-info" id="btn_CercaForFat">
                       <i class="fa fa-search"></i>Cerca Fornitore Fatturazione
                   </div>
                   <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../../AB_Immagini/icone32/lente.ico"
                       Style="display: none;"></asp:ImageButton>
               </div>
                <div class="col-lg-4"></div>
                <div class="col-lg-2 col-md-6 col-sm-12">
                    <div class="btn btn-info" id="btn_CercaForPro">
                        <i class="fa fa-search"></i>Cerca Fornitore Provenienza
                    </div>
                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="../../AB_Immagini/icone32/lente.ico"
                        Style="display: none;"></asp:ImageButton>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_FornitoreFatt" for="Cmb_FornitoreFatt">Fornitore Fatturazione
                                </span>
                                <asp:DropDownList ID="cmb_FornitoreFatt" runat="server" CssClass="form-control" data-live-search="true"
                                    aria-describedby="lbl_FornitoreFatt">
                                </asp:DropDownList>
                                <span class="input-group-addon alert-info">
                                    <asp:Label ID="lbl_numeroFornFatt" runat="server">0</asp:Label>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_FornitoreProv" for="Cmb_FornitoreProv">Fornitore Provenienza
                                </span>
                                <asp:DropDownList ID="cmb_FornitoreProv" runat="server" CssClass="form-control" data-live-search="true"
                                    aria-describedby="lbl_FornitoreProv">
                                </asp:DropDownList>
                                <span class="input-group-addon alert-info">
                                    <asp:Label ID="lbl_numeroFornProv" runat="server">0</asp:Label>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--  FILTRI LOTTO -->
        <div id="lotti" class="row">
            <h4>Filtro Partita</h4>
            <div class="row" id="div_txtlotto">
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <div class="form-horizontal">
                        <div class="form-group">
                            <div class="input-group">
                                <span class="input-group-addon alert-info" id="lbl_Lotto" for="Cmb_Lotto">Partita Da
                                </span>
                                <asp:DropDownList ID="Cmb_Lotto" runat="server" CssClass="form-control" data-live-search="true"
                                    aria-describedby="lbl_Lotto">
                                </asp:DropDownList>
                            </div>
                            <div class="btn btn-info" id="btn_CercaLotti">
                                <i class="fa fa-search"></i>Cerca
                            </div>
                            <asp:ImageButton ID="ImgBtn_CercaLotti" Style="display: none;" runat="server"
                                ImageUrl="../../AB_Immagini/icone32/lente.ico"></asp:ImageButton>
                            
                            <div class="btn btn-info" id="btn_TuttiLotti">
                                <i class="fa fa-filter"></i>Seleziona Tutti
                            </div>
                            <asp:ImageButton ID="ImageButton3" Style="display: none;" runat="server"
                                ImageUrl="../../AB_Immagini/icone32/lente.ico"></asp:ImageButton>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-12">
                    <span class="input-group-addon alert-info" id="lbl_Lotto2" for="Cmb_Lotto2">Partita A
                    </span>
                    <asp:DropDownList ID="Cmb_Lotto2" runat="server" CssClass="form-control" data-live-search="true"
                        aria-describedby="lbl_Lotto2">
                    </asp:DropDownList>
                </div>
            </div>
        </div>
    </div>
    <hr />
    <!-- JUMBOTRON-->

    <asp:HiddenField ID="hdDataDa" runat="server" />
    <asp:HiddenField ID="hdDataA" runat="server" />
    <asp:HiddenField ID="hdStatoPartite" runat="server" />
    <asp:HiddenField ID="hdImpresaSelezionata" runat="server" />
    <asp:HiddenField ID="hdStallaSelezionata" runat="server" />
    <asp:HiddenField ID="hdLottoSelezionato" runat="server" />
    <asp:HiddenField ID="hdLotto2Selezionato" runat="server" />
    <asp:HiddenField ID="hdRazzaSelezionato" runat="server" />
    <asp:HiddenField ID="hdSessoSelezionato" runat="server" />
    <asp:HiddenField ID="hdFornitoreFatturazioneSelezionato" runat="server" />
    <asp:HiddenField ID="hdFornitoreProvenienzaSelezionato" runat="server" />
    <asp:HiddenField ID="hdCentroSelezionato" runat="server" />
    

    <input type="hidden" id="hdTipo_Scheda" runat="server" />
    <input type="hidden" id="hdReportSelezionato" runat="server" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentScript" runat="server">
    <script type="text/javascript">

        var cIdTipo_Scheda = "#<%=hdTipo_Scheda.ClientID() %>";
        var cIdReportSelezionato = "#<%=hdReportSelezionato.ClientID() %>";
        var primoGiroImprese = true;
        var primoGiroCentri = true;
        var primoGiroStalla = true;
        var primoGiroRazze = true;
        var primoGiroSesso = true;
        var primoGiroLotti = true;
        var primoGiroFornitoreFatt = true;
        var primoGiroFornitoreProv = true;

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

        function carica_centri() {

            var param = kendo.stringify({
                Piva: $('#<%=Cmb_Impresa.ClientID %>').val()
            });

            $.ajax({
                type: 'POST',
                url: 'FiltroZootecnia.aspx/Carica_Centri',
                data: param,
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {

                    $('#FiltroStalla').hide();

                    if ($('#<%=Lbl_NumImprese.ClientID %>').text() == 0) {
                        $('#<%=Cmb_Centro.ClientID %>').empty();
                        $('#<%=Lbl_NumCentro.ClientID %>').empty();
                        $('#<%=Lbl_NumCentro.ClientID %>').append(0);
                    } else {

                        $('#<%=Cmb_Centro.ClientID %>').empty();
                        $('#<%=Cmb_Centro.ClientID %>').append(r.d[0]);

                        $('#<%=Lbl_NumCentro.ClientID %>').empty();
                        $('#<%=Lbl_NumCentro.ClientID %>').append(r.d[1]);

                        if (primoGiroCentri) {
                            primoGiroCentri = false;
                            if ($('#<%=hdCentroSelezionato.ClientID %>').val() != "") {
                                setSelectFromHidden('<%=Cmb_Centro.ClientID %>', '<%=hdCentroSelezionato.ClientID %>');
                                if ($('#<%=hdStallaSelezionata.ClientID %>').val() == "" && $('#<%=hdLottoSelezionato.ClientID %>').val() != "") {
                                    Carica_Lista_Lotti();
                                }
                            }
                        } 
                        if (parseInt(r.d[1]) == 1) {
                            var secondOption = $('#<%=Cmb_Centro.ClientID %>').find('option').eq(1);
                            if (secondOption.length) {
                                $('#<%=Cmb_Centro.ClientID %>').val(secondOption.val()).trigger('change'); 
                                $('#<%=hdCentroSelezionato.ClientID %>').val(secondOption.val()); 
                            }
                        }
                    }

                }
            });

        }

        function carica_stalle() {

            var param = kendo.stringify({
                Piva: $('#<%=Cmb_Impresa.ClientID %>').val(),
                Sa_Cod: $('#<%=Cmb_Centro.ClientID %>').val()
            });

            $.ajax({
                type: 'POST',
                url: 'FiltroZootecnia.aspx/Carica_Stalle',
                data: param,
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {

                    if ($('#<%=Lbl_NumCentro.ClientID %>').text() == 0) {
                        $('#<%=Cmb_Stalla.ClientID %>').empty();
                        $('#<%=Lbl_NumStalle.ClientID %>').empty();
                        $('#<%=Lbl_NumStalle.ClientID %>').append(0);
                    } else {

                        $('#<%=Cmb_Stalla.ClientID %>').empty();
                        $('#<%=Cmb_Stalla.ClientID %>').append(r.d[0]);

                        $('#<%=Lbl_NumStalle.ClientID %>').empty();
                        $('#<%=Lbl_NumStalle.ClientID %>').append(r.d[1]);

                        if (primoGiroStalla) {
                            primoGiroStalla = false;
                            if ($('#<%=hdStallaSelezionata.ClientID %>').val() != "") {
                                setSelectFromHidden('<%=Cmb_Stalla.ClientID %>', '<%=hdStallaSelezionata.ClientID %>');
                                if ($('#<%=hdLottoSelezionato.ClientID %>').val() != "") {
                                    Carica_Lista_Lotti();
                                }
                            }
                        } 
                        if (parseInt(r.d[1]) == 1) {
                            var secondOption = $('#<%=Cmb_Stalla.ClientID %>').find('option').eq(1);
                            if (secondOption.length) {
                                $('#<%=Cmb_Stalla.ClientID %>').val(secondOption.val()).trigger('change'); 
                                $('#<%=hdStallaSelezionata.ClientID %>').val(secondOption.val()); 
                            }
                        }
                    }

                }
            });

        }

        $("#btn_filtri").click(clearAllFields);

        // svuota campi testuali, ddl e hidden specifici della pagina
        function clearAllFields() {
            // campi testuali generici con classe form-control (esclude hidden)
            $('input.form-control[type="text"], textarea.form-control').not(':hidden').val('');

            // campi testuali espliciti (date, ricerche)
            $('#<%=TxtDataDa.ClientID %>').val('');
            $('#<%=TxtDataA.ClientID %>').val('');
            $('#<%=Txt_Impresa.ClientID %>').val('');
            $('#<%=Txt_ForFat.ClientID %>').val('');
            $('#<%=Txt_ForPro.ClientID %>').val('');

            // array delle DropDownList da resettare (imposta il primo option e scatta change)
            var ddlIds = [
                '<%=Cmb_statoPartite.ClientID %>',
                '<%=Cmb_Razza.ClientID %>',
                '<%=Cmb_Sesso.ClientID %>',
                '<%=Cmb_Impresa.ClientID %>',
                '<%=Cmb_Centro.ClientID %>',
                '<%=Cmb_Stalla.ClientID %>',
                '<%=cmb_FornitoreFatt.ClientID %>',
                '<%=cmb_FornitoreProv.ClientID %>',
                '<%=Cmb_Lotto.ClientID %>',
                '<%=Cmb_Lotto2.ClientID %>'
            ];

            ddlIds.forEach(function(id) {
                var $ddl = $('#' + id);
                if ($ddl.length) {
                    // se esiste almeno un option lo seleziono (indice 0) altrimenti lo svuoto
                    if ($ddl.find('option').length > 0) {
                        $ddl.prop('selectedIndex', 0);
                    } else {
                        $ddl.empty();
                    }
                    // scateno change per aggiornare eventuali handler collegati
                    $ddl.trigger('change');
                }
            });

            // svuota gli hidden usati per la sincronizzazione
            $('#<%=hdDataDa.ClientID %>').val('');
            $('#<%=hdDataA.ClientID %>').val('');
            $('#<%=hdImpresaSelezionata.ClientID %>').val('');
            $('#<%=hdStallaSelezionata.ClientID %>').val('');
            $('#<%=hdLottoSelezionato.ClientID %>').val('');
            $('#<%=hdLotto2Selezionato.ClientID %>').val('');
            $('#<%=hdRazzaSelezionato.ClientID %>').val('');
            $('#<%=hdSessoSelezionato.ClientID %>').val('');
            $('#<%=hdStatoPartite.ClientID %>').val('');
            $('#<%=hdFornitoreFatturazioneSelezionato.ClientID %>').val('');
            $('#<%=hdFornitoreProvenienzaSelezionato.ClientID %>').val('');
            $('#<%=hdCentroSelezionato.ClientID %>').val('');
            $('#<%=Cmb_Lotto.ClientID %>').empty();
            $('#<%=Cmb_Lotto2.ClientID %>').empty();
            $('#FiltroCentro').hide();
            $('#FiltroStalla').hide();
            Carica_Lista_Imprese();
            Carica_Lista_Fornitore_Fatturazione();
            Carica_Lista_Fornitore_Provenienza();
        }

        function Carica_Lista_Imprese() {
            $.ajax({
                type: 'POST',
                url: 'FiltroZootecnia.aspx/Carica_ImpreseWS',
                data: "{testo:'" + $('#<%=Txt_Impresa.ClientID %>').val() + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {

                    $('#FiltroCentro').hide();
                    $('#FiltroStalla').hide();

                    $('#<%=Cmb_Impresa.ClientID %>').empty();
                    $('#<%=Cmb_Impresa.ClientID %>').append(r.d[0]);

                    $('#<%=Lbl_NumImprese.ClientID %>').empty();
                    $('#<%=Lbl_NumImprese.ClientID %>').append(r.d[1]);
                                        
                    if (primoGiroImprese) {
                        primoGiroImprese = false;
                        if ($('#<%=hdImpresaSelezionata.ClientID %>').val() != "") {
                            setSelectFromHidden('<%=Cmb_Impresa.ClientID %>', '<%=hdImpresaSelezionata.ClientID %>');
                            if ($('#<%=hdCentroSelezionato.ClientID %>').val() == "" && $('#<%=hdLottoSelezionato.ClientID %>').val() != "") {
                                Carica_Lista_Lotti();
                            }
                        }
                    }
                      
                    if (parseInt(r.d[1]) == 1) {
                        var secondOption = $('#<%=Cmb_Impresa.ClientID %>').find('option').eq(1);
                        if (secondOption.length && secondOption.val() != $('#<%=hdImpresaSelezionata.ClientID %>').val()) {
                            $('#<%=Cmb_Impresa.ClientID %>').val(secondOption.val()).trigger('change');
                            $('#<%=hdImpresaSelezionata.ClientID %>').val(secondOption.val());
                        }
                    }
                }
            });
        }

        function Carica_Lista_Fornitore_Fatturazione() {
            $.ajax({
                type: 'POST',
                url: 'FiltroZootecnia.aspx/Carica_Fornitore_FatturazioneWS',
                data: "{testo:'" + $('#<%=Txt_ForFat.ClientID %>').val() + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {
                    $('#<%=cmb_FornitoreFatt.ClientID %>').empty();
                    $('#<%=cmb_FornitoreFatt.ClientID %>').append(r.d[0]);
                    $('#<%=lbl_numeroFornFatt.ClientID %>').empty();
                    $('#<%=lbl_numeroFornFatt.ClientID %>').append(r.d[1]);
                    if (primoGiroFornitoreFatt) {
                        primoGiroFornitoreFatt = false;
                        if ($('#<%=hdFornitoreFatturazioneSelezionato.ClientID %>').val() != "") {
                            setSelectFromHidden('<%=cmb_FornitoreFatt.ClientID %>', '<%=hdFornitoreFatturazioneSelezionato.ClientID %>');
                        }
                    }   
                }
            });
        }

        function Carica_Lista_Fornitore_Provenienza() {
            $.ajax({
                type: 'POST',
                url: 'FiltroZootecnia.aspx/Carica_Fornitore_ProvenienzaWS',
                data: "{testo:'" + $('#<%=Txt_ForPro.ClientID %>').val() + "'}",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {
                    $('#<%=cmb_FornitoreProv.ClientID %>').empty();
                    $('#<%=cmb_FornitoreProv.ClientID %>').append(r.d[0]);
                    $('#<%=lbl_numeroFornProv.ClientID %>').empty();
                    $('#<%=lbl_numeroFornProv.ClientID %>').append(r.d[1]);
                    if (primoGiroFornitoreProv) {
                        primoGiroFornitoreProv = false;
                        if ($('#<%=hdFornitoreProvenienzaSelezionato.ClientID %>').val() != "") {
                            setSelectFromHidden('<%=cmb_FornitoreProv.ClientID %>', '<%=hdFornitoreProvenienzaSelezionato.ClientID %>');
                        }
                    }   
                }
            });
        }

        function Carica_Lista_Lotti() {
            param = kendo.stringify({
                gruppo_date: $('#<%=TxtDataDa.ClientID %>').val() + "," + $('#<%=TxtDataA.ClientID %>').val(),
                piva: $('#<%=Cmb_Impresa.ClientID %>').val(),
                centro: $('#<%=hdCentroSelezionato.ClientID %>').val(),
                stalla: $('#<%=hdStallaSelezionata.ClientID %>').val(),
                Dettaglio: $('#<%=hdReportSelezionato.ClientID %>').val() == 260
            });
            WaitFrame.show();
            $.ajax({
                type: 'POST',
                url: 'FiltroZootecnia.aspx/Carica_LottiWS',
                data: param,
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: true,
                success: function (r) {
                    WaitFrame.hide();
                    $('#<%=Cmb_Lotto.ClientID %>').empty();
                    $('#<%=Cmb_Lotto.ClientID %>').append(r.d[0]);
                    $('#<%=Cmb_Lotto2.ClientID %>').empty();
                    $('#<%=Cmb_Lotto2.ClientID %>').append(r.d[0]);
                    if (primoGiroLotti) {
                        primoGiroLotti = false;
                        temp = "";
                        if ($('#<%=hdLotto2Selezionato.ClientID %>').val() != "") {
                            temp = $('#<%=hdLotto2Selezionato.ClientID %>').val();
                            setSelectFromHidden('<%=Cmb_Lotto2.ClientID %>', '<%=hdLotto2Selezionato.ClientID %>');
                        } else {
                            $('#<%=hdLotto2Selezionato.ClientID %>').val($('#<%=Cmb_Lotto.ClientID %>').val());
                        }
                        if ($('#<%=hdLottoSelezionato.ClientID %>').val() != "") {
                            setSelectFromHidden('<%=Cmb_Lotto.ClientID %>', '<%=hdLottoSelezionato.ClientID %>');
                            if (temp != "") {
                                $('#<%=hdLotto2Selezionato.ClientID %>').val(temp)
                                setSelectFromHidden('<%=Cmb_Lotto2.ClientID %>', '<%=hdLotto2Selezionato.ClientID %>');
                            }                            
                        } else {
                            $('#<%=hdLottoSelezionato.ClientID %>').val($('#<%=Cmb_Lotto.ClientID %>').val());
                        }
                    } else {
                        $('#<%=hdLottoSelezionato.ClientID %>').val($('#<%=Cmb_Lotto.ClientID %>').val());
                        $('#<%=hdLotto2Selezionato.ClientID %>').val($('#<%=Cmb_Lotto.ClientID %>').val());
                    }
                    $('#btn_TuttiLotti').kendoButton().data("kendoButton").enable(true);
                }
            });
        }

        function setSelectFromHidden(ddlClientId, hiddenClientId) {
            var hv = $('#' + hiddenClientId).val();
            if (hv !== undefined && hv !== null && hv !== '') {
                var $ddl = $('#' + ddlClientId);
                // se l'opzione esiste la seleziono, altrimenti la aggiungo come fallback (testo = valore)
                if ($ddl.find('option[value="' + hv + '"]').length) {
                    $ddl.val(hv).trigger('change');
                } else {
                    $ddl.prepend($('<option>').val(hv).text(hv));
                    $ddl.val(hv).trigger('change');
                }
            }
        }

        function setInputFromHidden(inputClientId, hiddenClientId) {
            var hv = $('#' + hiddenClientId).val();
            if (hv !== undefined && hv !== null && hv !== '') {
                $('#' + inputClientId).val(hv);
            }
        }

        //*** READY ***/
        $(document).ready(async function () {

            // Aspetta che il datepicker sia completamente inizializzato
            setTimeout(function () {

                // Intercetta TUTTI gli eventi possibili del datepicker
                $('#<%=TxtDataDa.ClientID %>, #<%=TxtDataA.ClientID %>').on('change changeDate input blur dp.change', function () {
                    var $input = $(this);
                    var fieldId = $input.attr('id');

                    // Aspetta un attimo per permettere al datepicker di aggiornare il valore
                    setTimeout(function () {
                        var valore = $input.val();

                        if (valore) {
                            if (fieldId === '<%=TxtDataDa.ClientID %>') {
                                $('#<%=hdDataDa.ClientID %>').val(valore);
                            } else if (fieldId === '<%=TxtDataA.ClientID %>') {
                                $('#<%=hdDataA.ClientID %>').val(valore);
                            }
                        }
                    }, 100);
                });

            }, 500);

            //clearAllFields();

            $('#FiltroCentro').hide();
            $('#FiltroStalla').hide();
            $('#btn_TuttiLotti').kendoButton().data("kendoButton").enable(false);

            var tipoScheda = 0;

            // Leggo la tipologia di scheda selezionata da menu
            tipoScheda = parseInt($(cIdReportSelezionato).val());
            if (tipoScheda == 260) {
                $("#FiltroDateText").text("Filtro periodo apertura partita")
                $('#lotti').show();
                $('#stato_partite').hide();
            } else {
                $("#FiltroDateText").text("Filtro periodo macellazione")
                $('#lotti').hide();
                $('#stato_partite').show();
            }
            $('#Rbl_ReportZootecnia input[type="checkbox"]').each(function () {
                if (parseInt($(this).val()) === tipoScheda) {
                    $(this).prop('checked', true);
                }
                else {
                    $(this).prop('checked', false);
                }
            });

            if ($('#<%=hdDataDa.ClientID %>').val() != "") {
                setInputFromHidden('<%=TxtDataDa.ClientID %>', '<%=hdDataDa.ClientID %>');
            }
            if ($('#<%=hdDataA.ClientID %>').val() != "") {
                setInputFromHidden('<%=TxtDataA.ClientID %>', '<%=hdDataA.ClientID %>');
            }

            Carica_Lista_Imprese()

            Carica_Lista_Fornitore_Fatturazione();

            Carica_Lista_Fornitore_Provenienza();

            $.ajax({
                type: 'POST',
                url: 'FiltroZootecnia.aspx/Carica_ListaBoviniWS',
                data: "",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {
                    $('#<%=Cmb_Razza.ClientID %>').empty();
                    $('#<%=Cmb_Razza.ClientID %>').append(r.d[0]);

                    if (primoGiroRazze) {
                        primoGiroRazze = false;
                        if ($('#<%=hdRazzaSelezionato.ClientID %>').val() != "") {
                            setSelectFromHidden('<%=Cmb_Razza.ClientID %>', '<%=hdRazzaSelezionato.ClientID %>');
                        }
                    }                    
                }
            }); 

            $.ajax({
                type: 'POST',
                url: 'FiltroZootecnia.aspx/Carica_Stato_Partite',
                data: "",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {
                    $('#<%=Cmb_statoPartite.ClientID %>').empty();
                    $('#<%=Cmb_statoPartite.ClientID %>').append(r.d[0]);
                    if ($('#<%=hdStatoPartite.ClientID %>').val() != "") {
                        setSelectFromHidden('<%=Cmb_statoPartite.ClientID %>', '<%=hdStatoPartite.ClientID %>');
                    }
                }
            });

            $.ajax({
                type: 'POST',
                url: 'FiltroZootecnia.aspx/Carica_Sesso',
                data: "",
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'json', async: false,
                success: function (r) {
                    $('#<%=Cmb_Sesso.ClientID %>').empty();
                    $('#<%=Cmb_Sesso.ClientID %>').append(r.d[0]);
                    if (primoGiroSesso) {
                        primoGiroSesso = false;
                        if ($('#<%=hdSessoSelezionato.ClientID %>').val() != "") {
                            setSelectFromHidden('<%=Cmb_Sesso.ClientID %>', '<%=hdSessoSelezionato.ClientID %>');
                        }
                    }   
                }
            });

            //restoreFieldsFromHidden();

        });

        function AzzeraLotti() {
            $('#<%=Cmb_Lotto.ClientID %>').empty();
            $('#<%=Cmb_Lotto2.ClientID %>').empty();
        }

        $('#btn_CercaImpresa').click(function () {
            //$('#<=ImgBtn_CercaImpresa.ClientID %>').click();

            if ($('#<%=Txt_Impresa.ClientID %>').val() == '') {
                alert('Applicare un filtro per la ricerca Impresa');
            } else {
                Carica_Lista_Imprese();
            }
        });

        $('#btn_CercaForFat').click(function () {

                    if ($('#<%=Txt_ForFat.ClientID %>').val() == '') {
                alert('Applicare un filtro per la ricerca Fornitore Fatturazione');
            } else {
                // riempio le tendine
                Carica_Lista_Fornitore_Fatturazione();
            }
        });

        $('#btn_CercaForPro').click(function () {

            if ($('#<%=Txt_ForPro.ClientID %>').val() == '') {
                alert('Applicare un filtro per la ricerca Fornitore Fatturazione');
            } else {
                Carica_Lista_Fornitore_Provenienza();
            }
        });

        // AgronicaStampe_2010\GestioneStampe\Magazzino\Filtro_SchedeMagazzino_new.aspx

        $('#btn_CercaLotti').click(function () {
            if ($('#<%=Cmb_Impresa.ClientID %>').val() == "") {
                alert('Applicare un filtro per azienda');
                return;

            } else {
                Carica_Lista_Lotti();
            }
        });

        $('#btn_TuttiLotti').click(function () {
            let lastCmb2 = $('#MainContent_Cmb_Lotto2')[0].length - 1;
            $('#<%=Cmb_Lotto.ClientID %>').prop('selectedIndex', 0).trigger('change');
            $('#<%=Cmb_Lotto2.ClientID %>').prop('selectedIndex', lastCmb2).trigger('change');
        });

        $('#<%=TxtDataDa.ClientID %>').on('change blur', function () {
            var valore = $(this).val();
            $('#<%=hdDataDa.ClientID %>').val(valore);
        });

        $('#<%=TxtDataA.ClientID %>').change(function () {
            var valore = $(this).val();
            $('#<%=hdDataA.ClientID %>').val(valore);
        });

        $('#<%=Cmb_Lotto.ClientID %>').change(function () {
            var valore = $(this).val();
            $('#<%=hdLottoSelezionato.ClientID %>').val(valore);
            $('#<%=hdLotto2Selezionato.ClientID %>').val(valore);

            var selectedIndex = $(this)[0].selectedIndex;
            var cmbLotto2 = $('#<%=Cmb_Lotto2.ClientID %>');
            var options = $(this).find('option');

            // Svuota e ripopola Cmb_Lotto2 solo con le opzioni dalla selezionata in poi
            cmbLotto2.empty();
            options.each(function (i, opt) {
                if (i >= selectedIndex) {
                    cmbLotto2.append($(opt).clone());
                }
            });

            // Imposta come selezionato il valore uguale a quello scelto in Cmb_Lotto
            var selectedValue = $(this).val();
            cmbLotto2.val(selectedValue);
        });
        

        $('#<%=Cmb_Impresa.ClientID %>').change(function () {
            var valore = $(this).val();
            $('#<%=hdImpresaSelezionata.ClientID %>').val(valore);
            AzzeraLotti();
            if(valore == "") {
                $('#FiltroCentro').hide();
                $('#FiltroStalla').hide();
                $('#<%=Cmb_Centro.ClientID %>').empty();
                $('#<%=Lbl_NumCentro.ClientID %>').empty();
                $('#<%=Lbl_NumCentro.ClientID %>').append(0);
                $('#<%=Cmb_Stalla.ClientID %>').empty();
                $('#<%=Lbl_NumStalle.ClientID %>').empty();
                $('#<%=Lbl_NumStalle.ClientID %>').append(0);
                $('#<%=hdCentroSelezionato.ClientID %>').val(valore);
                $('#<%=hdStallaSelezionata.ClientID %>').val(valore);
            } else {
                $('#FiltroCentro').show();
                carica_centri();
            }
        });

        $('#<%=Cmb_Centro.ClientID %>').change(function () {
            var valore = $(this).val();
            $('#<%=hdCentroSelezionato.ClientID %>').val(valore);
            AzzeraLotti();
            if (valore == "") {
                $('#FiltroStalla').hide();
                $('#<%=Cmb_Stalla.ClientID %>').empty();
                $('#<%=Lbl_NumStalle.ClientID %>').empty();
                $('#<%=Lbl_NumStalle.ClientID %>').append(0);
                $('#<%=hdStallaSelezionata.ClientID %>').val(valore);
            } else {
                $('#FiltroStalla').show();
                carica_stalle();
            }
        });

        $('#<%=Cmb_Stalla.ClientID %>').change(function () {
            var valore = $(this).val();
            $('#<%=hdStallaSelezionata.ClientID %>').val(valore);
            AzzeraLotti();
        });

        $('#<%=Cmb_Sesso.ClientID %>').change(function () {
            var valore = $(this).val();
            $('#<%=hdSessoSelezionato.ClientID %>').val(valore);
        });

        $('#<%=Cmb_statoPartite.ClientID %>').change(function () {
            var valore = $(this).val();
            $('#<%=hdStatoPartite.ClientID %>').val(valore);
        });

        $('#<%=cmb_FornitoreFatt.ClientID %>').change(function () {
            var valore = $(this).val();
            $('#<%=hdFornitoreFatturazioneSelezionato.ClientID %>').val(valore);
        });

        $('#<%=cmb_FornitoreProv.ClientID %>').change(function () {
            var valore = $(this).val();
            $('#<%=hdFornitoreProvenienzaSelezionato.ClientID %>').val(valore);
        });

        $('#<%=Cmb_Razza.ClientID %>').change(function () {
            var valore = $(this).val();
            $('#<%=hdRazzaSelezionato.ClientID %>').val(valore);
        });

        $('#<%=Cmb_Lotto2.ClientID %>').change(function () {
            var valore = $(this).val();
            $('#<%=hdLotto2Selezionato.ClientID %>').val(valore);
        });

        //... on change
        $('#Rbl_ReportZootecnia input[type="checkbox"]').change(function () {
            //alert();
            $('input[name="chk_group[]"]').not(this).prop('checked', false);

            var tipoScheda = parseInt($(this).val());

            if (tipoScheda == 260) {
                $("#FiltroDateText").text("Filtro periodo apertura partita")
                $('#lotti').show();
                $('#stato_partite').hide();
            } else {
                $("#FiltroDateText").text("Filtro periodo macellazione")
                $('#lotti').hide();
                $('#stato_partite').show();
            }

            $(cIdTipo_Scheda).val(tipoScheda);
            $(cIdReportSelezionato).val(tipoScheda);
            $('#<%=hdReportSelezionato.ClientID %>').val(tipoScheda);
           
        });


        $('#btn_CercaImpresa').click(function () {
            
        });

        $('#btn_stampa_excel').click(btn_stampa_excel_click);

        function btn_stampa_excel_click() {
            
        }

        $("#btn_stampa").click(btn_stampa_click);

        function btn_stampa_click() {

            if ($('#<%=TxtDataDa.ClientID %>').val() == '' || $('#<%=TxtDataA.ClientID %>').val() == '') {
                alert('Applicare entrambi i filtri per la data');
            } else if ($('#<%=hdImpresaSelezionata.ClientID %>').val() == '') {
                alert('Applicare un filtro per azienda');
            } else if ($(cIdReportSelezionato).val() == '260' && $('#<%=Cmb_Lotto2.ClientID %>').val() == null) {
                alert("Selezionare l'intervallo di partite");
            } else {


                var flag_ok = true;

                var val_check;
                $('#Rbl_ReportZootecnia input[name="chk_group[]"]:checked').each(function () {
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

                if (flag_ok) {

                    // proseguo con la stampa crystal
                    $('#<%=ImgBtn_Stampa.ClientID %>').click();

                    var secondOption = $('#<%=Cmb_Impresa.ClientID %>').find('option').eq(0);
                    $('#<%=Cmb_Impresa.ClientID %>').val(secondOption.val()).trigger('change'); 
                    $('#<%=hdImpresaSelezionata.ClientID %>').val(secondOption.val());

                }
                
            }
        }
    </script>
</asp:Content>
