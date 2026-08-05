var rigaDuplicataKendoGrid = false;
var rigaDaCopiareKendoGrid;
var dataItemMovimenti = null;

function popolaDocContabileRighe(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: kReadMovimenti_rows,
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };

    
    if (flagSceltaImputazione) {
        funzioniCRUD.checkBoxFunction = SelezionaRigaRiepilogo;
    }
    else {
        funzioniCRUD.checkBoxFunction = null;
    }

    var idModel = "key_mov_dett";

    var campiKendoModel = kReadMovimenti_mod();
    var colonneKendoGrid = kReadMovimenti_col();
    var parametriPerLettura = null;
    var parametriDataSource = {
        pagesize: 50,
        aggregate: [
            { field: "Ordine_Det", aggregate: "max" },
            { field: "NrImballaggi", aggregate: "sum" },  // TODO Questo e altri solo se gestiti
            { field: "NrContenitori", aggregate: "sum" },
            { field: "NrConfezioni", aggregate: "sum" },
            { field: "KgLordi", aggregate: "sum" },
            { field: "KgNetti", aggregate: "sum" },
            { field: "Tara", aggregate: "sum" },
            { field: "Qta", aggregate: "sum" },
            { field: "Degrado_Calcolato", aggregate: "sum" },
            { field: "Quantita_Escluso_Degrado", aggregate: "sum" },
            //{ field: "Num_Imballi_Riscontrati", aggregate: "sum" },
            //{ field: "Num_Colli_Riscontrati", aggregate: "sum" },
            //{ field: "Num_Conf_Riscontrate", aggregate: "sum" },
            { field: "Peso_Lordo_Riscontrato", aggregate: "sum" },
            { field: "Peso_Netto_Riscontrato", aggregate: "sum" },
            { field: "Tara_Totale_Riscontrata", aggregate: "sum" },
            { field: "Importo_Totale", aggregate: "sum" },
            { field: "Importo_Unitario", aggregate: "sum" },
            { field: "Iva", aggregate: "sum" },
            { field: "Imponibile", aggregate: "sum" },
            { field: "Imponibile_Netto", aggregate: "sum" }
        ]
    };

    let arrayCommand = [];
    if (UteAbilitatoInsMod && cIdTipoOp !== enum_TipoOperazioneDB.Lettura.value) {

        arrayCommand = [
            {
                template: "<span class='fa fa-pencil-square-o fa-2x edit_elem' title='" + TraduzioneMultiResx(resxContabileDettagliUC, "Modifica", "Modifica") + "' onclick=ApriModificaRiga(this.closest('tr'),this.closest('.k-grid'),2)></span>",
                visible: function (dataItem) {

                    let statiOrdineModificabili = [
                        enum_StatoOrdine.indefinito,
                        enum_StatoOrdine.non_pronto,
                        enum_StatoOrdine.inevaso
                    ];

                    return dataItem.StatoEvasione_Cod === undefined || statiOrdineModificabili.includes(dataItem.StatoEvasione_Cod);
                }
            },
            {
                template: "<span class='fa fa-trash-o fa-2x del_elem' title='" + TraduzioneMultiResx(resxContabileDettagliUC, "Elimina", "Elimina") + "' onclick=EliminaRiga(this.closest('tr'),this.closest('.k-grid'),3)></span>",
                visible: function (dataItem) {

                    let statiOrdineModificabili = [
                        enum_StatoOrdine.indefinito,
                        enum_StatoOrdine.non_pronto,
                        enum_StatoOrdine.inevaso
                    ];

                    return dataItem.StatoEvasione_Cod === undefined || statiOrdineModificabili.includes(dataItem.StatoEvasione_Cod);
                }
            }
            //{ template: "<span class='fa fa-info fa-2x info_elem' title='Info' onclick=ApriModificaRiga(this.closest('tr'),this.closest('.k-grid'),0)></span>" },
        ];

        if (impedisciCreazioneCarichiMultiriga == false) {
            arrayCommand.push({ template: "<span class='fa fa-files-o fa-2x copia_elem' style='cursor: pointer;' title='" + TraduzioneMultiResx(resxContabileDettagliUC, "Copia", "copia") + "' onclick=DuplicaRiga(this.closest('tr'),this.closest('.k-grid'),10)></span>" });
        }

        if (lavCodAccettazione) {
            arrayCommand.push(
                {
                    template: "<span class='fa fa-barcode fa-2x print_elem' style='cursor: pointer;' title='" + TraduzioneMultiResx(resxContabileDettagliUC, "StampaBarCode", "Stampa BarCode") + "' onclick=stampaBarCode(this.closest('tr'),this.closest('.k-grid'),true)></span>",
                    visible: function (dataItem) {
                        return (true);
                    }
                }
            );
        }

        if (lavCodOrdine) {
            arrayCommand.push(
                {
                    template: "<span class='fa fa-truck fa-2x ' style='cursor: pointer;' title='" + TraduzioneMultiResx(resxContabileDettagliUC, "ForzaEvasioneDettaglio", "Forza Evasione Dettaglio") + "' onclick=ForzaEvasioneDettaglio(this.closest('tr'),this.closest('.k-grid'),true)></span>",
                    visible: function (dataItem) {
                        if (dataItem.Cat_Cod === RIGA_DESCRIZIONE_LIBERA || [enum_StatoOrdine.evaso, enum_StatoOrdine.evaso_forzatamente].includes(dataItem.StatoEvasione_Cod)) return false;
                        else return true;
                    }
                }
            );
            arrayCommand.push(
                {
                    //template: "<span class='fa-stack fa-2x crossed-out' style='cursor: pointer;' title='Annulla Evasione forzata Dettaglio' onclick=ForzaEvasioneDettaglio(this.closest('tr'),this.closest('.k-grid'),false)>" +
                    //    "<i class='fa fa-lg fa-stack-1x fa-truck'></i>" +
                    //    "</span>",
                    template: "<span class='fa fa-stack' style='cursor: pointer;vertical-align: baseline;' title='" + TraduzioneMultiResx(resxContabileDettagliUC, "AnnullaEvasioneForzataDettaglio", "Annulla Evasione Forzata Dettaglio") + "' onclick=ForzaEvasioneDettaglio(this.closest('tr'),this.closest('.k-grid'),false)>" +
                        "<i class='fa fa-truck fa-stack-2x' style='margin: 0;'></i>" +
                        "<i class='fa fa-times fa-stack-2x' style='margin: 0; color:red;'></i>" +
                        "</span>",
                    visible: function (dataItem) {
                        if (dataItem.Cat_Cod !== RIGA_DESCRIZIONE_LIBERA && dataItem.StatoEvasione_Cod === enum_StatoOrdine.evaso_forzatamente) return true;
                        else return false;
                    }
                }
            );
        }


    } else {
        arrayCommand = [
            { template: "<span class='fa fa-info fa-2x info_elem' title='" + TraduzioneMultiResx(resxContabileDettagliUC, "Info", "Info") + "' onclick=ApriModificaRiga(this.closest('tr'),this.closest('.k-grid'),0)></span>" }
        ];
    }

    colonneKendoGrid.unshift({
        command: arrayCommand,
        locked: true, title: TraduzioneMultiResx(resxContabileDettagliUC, "Azioni", "Azioni"), width: "130px"
    });
    
    // Se l'utente non è abilitato in modifica non mostro il pulsante di duplicazione
    // Le colonne modifica / cancellazione e inserimento nuova riga sono già gestite nel GiasBase
    //if (UteAbilitatoInsMod)
    //{
    //    colCustKendoGrid[0].command.push(
    //        {
    //            iconClass: "fa fa-files-o fa-xs", className: "blockDuplica", name: "duplica", text: "", click: duplicaRigaKendoGrid
    //        });
    //}

    var parametriKendoGrid = {
        pdf: false, groupable: false, 
        editable: {
            mode: "inline"
        },
        //pageable: { pageSizes: [5, 10, 20, 50, 100, "all"] },
        //colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
    };

    // Impostazione barra dei comandi

    parametriKendoGrid.toolbarCommands = [];

    if (cIdTipoOp !== enum_TipoOperazioneDB.Lettura.value && impedisciCreazioneCarichiMultiriga == false) {
        parametriKendoGrid.toolbarCommands.push("templatePulsanteNuovaRiga");
    }

    if (!isContrattoAffitto()) {
        parametriKendoGrid.toolbarCommands.push("templateChkSceltaColonne");
    }

    if (flagSceltaImputazione) {
        parametriKendoGrid.toolbarCommands.push("contabDettagliUC_tmplAssociaCDG");
    }

    // Impostazione funzioni prima/dopo eventi

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: onDataBoundRigheMovimenti,
        funzioneDaChiamareDopoEdit: onEditRigheMovimenti,
        //funzioneDaChiamareDopoSave: onSaveRigheMovimenti,
        funzioneDaChiamareDopoChange: onChangeRigheMovimenti
    };

    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    var kGridRighe = creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

    kGridRighe.find(".menuSceltaColonne").kendoMenu();

    //se l'utente non ha permesso in lettura di gestione prezzi, nascondo il check di gruppo
    if ($("input[name$='hf_UtenteAbilitatoGestionePrezziLettura']").val() !== "True") {
        $("#groupChk_Mostra_Economico").hide();
    }

    if (flagSceltaImputazione) {
        var tipologiaProgetto = getTipologiaProgetto();

        var spnAssociaRigaCdgProgetti = document.getElementById("spnAssociaRigaCdgProgetti");
        if (spnAssociaRigaCdgProgetti !== undefined && spnAssociaRigaCdgProgetti !== null) {
            spnAssociaRigaCdgProgetti.innerHTML = TraduzioneMultiResx(resxContabileDettagliUC, "Associa", "Associa") + " " + tipologiaProgetto;
        }
    }
}

function kReadMovimenti_rows(options) {

    var data = $('input[name$="hdKendo_RigheDoc"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    //Disabilito la modifica di alcuni elementi della testata se c'è già almeno una riga di dettaglio
    if (parseInt($(cIdAgenda).val()) !== 0 && jSonParsed_Kendo.kendo_rows.length > 0) {

        AbilitaModificaDatiMinimiTestata(false);

        if (cIdTipoOp === enum_TipoOperazioneDB.Lettura.value) {
            PulsantiSalvataggioSolaLettura();
        } else {
            PulsantiSalvataggioModifica();
        }

    } else {
        //se ancora non ho inserito righe, nascondo i pulsanti di salvataggio dalla testata
        VisualizzaPulsantiSalvataggio(false);

        $("#btnStampaDoc").hide();
        $("#btnStampaDoc2").hide();

        $("#btnEsciDocTxt").text(TraduzioneMultiResx(resxContabileDettagliUC, "EsciSenzaSalvare", "Esci senza salvare"));
        $("#btnEsciDocTxt2").text(TraduzioneMultiResx(resxContabileDettagliUC, "EsciSenzaSalvare", "Esci senza salvare"));
    }

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadMovimenti_col() {

    var data = $('input[name$="hdKendo_RigheDoc"]').val();
    var jSonParsed_Kendo = JSON.parse(data);
    var mostraSigla = true;

    // ------------   INIZIO CREAZIONE DDL KENDO -----------------

    // --------------------------------------------------------------------
    // DDL PRODOTTO, PARAMETRI QUALITATIVI (FRA CUI IMBALLAGGI) E CELLA
    // --------------------------------------------------------------------
    var gruppoColonne = "Prodotto";

    //Cella
    //////var k = 0;
    //////kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "key_Dest", "Mag. o Cella", k, "Ubic_Des", celle_Template, null, null, filterable_celle_Template, true, gruppoColonne);

    //Prodotto
    //////k++;
    //////kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "Mat_Cod", "Prodotto", k, "Mat_Des", materie_prime_Template, null, null, filterable_materie_primeTemplate, true, gruppoColonne);
    
    // Cerco l'elenco dei parametri qualitativi gestiti
    //////var filterTemplate; 
    //////for (var ipar = 0; ipar < paramQualGestiti_FF.length; ipar++) {
    //////    if (paramQualGestiti_FF[ipar].Tabella_ID !== 0 && data.aContains("FF_" + paramQualGestiti_FF[ipar].Tabella_Cod_Des + "_Tipo_Cod"))
    //////    {
    //////        if (paramQualGestiti_FF[ipar].Tabella_ID !== "4" &&
    //////            paramQualGestiti_FF[ipar].Tabella_ID !== "5" &&
    //////            paramQualGestiti_FF[ipar].Tabella_ID !== "8") {

    //////            k++;
    //////            mostraSigla = true;
    //////            filterTemplate = getfilterable_paramQualTemplate($(cIdPiva).val(), paramQualGestiti_FF[ipar].Tabella_ID, mostraSigla);
    //////            kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "FF_" + paramQualGestiti_FF[ipar].Tabella_Cod_Des + "_Tipo_Cod", paramQualGestiti_FF[ipar].Tabella_Des, k, "FF_" + paramQualGestiti_FF[ipar].Tabella_Cod_Des + "_Sigla", paramQual_Template, null, null, filterTemplate, true, gruppoColonne);
    //////        }
    //////    }
    //////}
    
    //////if (jSonParsed_Kendo.kendo_model.FF_imballaggio_Tipo_Cod !== undefined) {
    //////    k++;
    //////    mostraSigla = false;
    //////    filterTemplate = getfilterable_paramQualTemplate($(cIdPiva).val(), 4, mostraSigla);
    //////    kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "FF_imballaggio_Tipo_Cod", "Imb.", k, "FF_imballaggio_Descrizione", paramQual_Template, null, null, filterTemplate, true, gruppoColonne);
        
    //////}

    //////if (jSonParsed_Kendo.kendo_model.FF_contenitore_Tipo_Cod !== undefined) {
    //////    k++;
    //////    mostraSigla = false;
    //////    filterTemplate = getfilterable_paramQualTemplate($(cIdPiva).val(), 8, mostraSigla);
    //////    kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "FF_contenitore_Tipo_Cod", "Conten.", k, "FF_contenitore_Descrizione", paramQual_Template, null, null, filterTemplate, true, gruppoColonne);
    //////}

    //////if (jSonParsed_Kendo.kendo_model.FF_confezione_Tipo_Cod !== undefined) {
    //////    k++;
    //////    mostraSigla = false;
    //////    filterTemplate = getfilterable_paramQualTemplate($(cIdPiva).val(), 5, mostraSigla);
    //////    kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "FF_confezione_Tipo_Cod", "Conf.", k, "FF_confezione_Descrizione", paramQual_Template, null, null, filterTemplate, true, gruppoColonne);
    //////}

    // ----------------------------------------------
    // DDL DATI Economici
    // ----------------------------------------------
    gruppoColonne = "Economico";

    //Sconto Modalità
    //TODO RIABILITARE
    //k++;
    //kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "Sconto_Modalita", "Modalità Sconto", k, "Sconto_Modalita_Descr", sconto_Modalita_Template, null, null, filterable_sconto_Modalita_Template, true, gruppoColonne);

    //Tempo Carenza (ovvero indica il campo da tenere fisso per calcolo prezzo, iva e importo)
    //TODO RIABILITARE
    //k++;
    //kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "TempoCarenza", "Valore riferimento", k, "TempoCarenza_Descr", tempocarenza_Template, null, null, filterable_tempocarenza_Template, true, gruppoColonne);

    // Listino
    //////k++;
    //////var commandListino = [{
    //////    template: "<a class='k-button k-button-icontext CercaPrezzoDaListini' onclick=CercaPrezziDaListino(this.closest('tr'),this.closest('.k-grid'))><span class='k-icon k-i-paste-plain-text'></span>Listino</a>"
    //////}];
    //////kendo_Colonne_estendi(jSonParsed_Kendo, "CercaPrezzoDaListini", "Listino", k, "CercaPrezzoDaListini", undefined, undefined, commandListino, gruppoColonne)
     
    //Prezzo Livello
    //TODO RIABILITARE
    //k++;
    //kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "Prezzo_Livello", "Prezzo riferito a", k, "Prezzo_Livello_Descr", prezzo_Livello_Template, null, null, filterable_prezzo_Livello_Template, true, gruppoColonne);

    //Sconto - Maggiorazione
    //TODO RIABILITARE
    //k++;
    //kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "ScontoMaggiorazione", "Sconto / Magg.", k, "ScontoMaggiorazione_Descr", scontomaggiorazione_Template, null, null, filterable_scontomaggiorazione_Template, true, gruppoColonne);

    //IVA
    //TODO RIABILITARE
    //k++;
    //kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "Cod_IVA", "IVA", k, "Sigla_IVA", IVA_Aliquote_Template, null, null, filterable_IVA_Aliquote_Template, true, gruppoColonne);

    // ----------------------------------------------
    // DDL DATI IMPUTAZIONI
    // ----------------------------------------------
    gruppoColonne = "Imputazioni";

    ////////Anno
    //////k++;
    //////kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "Anno_Cod", "Anno", k, "Anno", anniApertiConti_Template, null, null, filterable_anniApertiConti_Template, true, gruppoColonne);

    // Conto Economico
    //TODO RIABILITARE
    //k++;
    //kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "Cod_Conto_Economico", "Conto Economico", k, "Descr_Conto_Economico", conto_economico_Template, null, null, filterable_conto_economico_Template, true, gruppoColonne);

    // Conto Patrimoniale
    //TODO RIABILITARE
    //k++;
    //kendo_Colonne_estendi_DDL(jSonParsed_Kendo, "Cod_Conto_Patrimoniale", "Conto Patrimoniale", k, "Descr_Conto_Patrimoniale", conto_patrimoniale_Template, null, null, filterable_conto_patrimoniale_Template, true, gruppoColonne);


    // ------------   FINE CREAZIONE DDL KENDO -----------------

    for (var i = 0; i < jSonParsed_Kendo.kendo_columns.length; i++) {

        // TODO INIZIO da togliere, è solo un esperimento per ricerca su griglia
        //if (jSonParsed_Kendo.kendo_columns[i].field === "Lotto") {
        //    jSonParsed_Kendo.kendo_columns[i].editor = editorLotto;
        //}
        // TODO FINE da togliere, è solo un esperimento per ricerca su griglia

        if (jSonParsed_Kendo.kendo_columns[i].format === "{0:n0}" ||
            jSonParsed_Kendo.kendo_columns[i].format === "{0:n1}" ||
            jSonParsed_Kendo.kendo_columns[i].format === "{0:n2}" ||
            jSonParsed_Kendo.kendo_columns[i].format === "{0:n3}" ||
            jSonParsed_Kendo.kendo_columns[i].format === "{0:n4}" ||
            jSonParsed_Kendo.kendo_columns[i].format === "{0:n5}" ||
            jSonParsed_Kendo.kendo_columns[i].format === "{0:n6}") {
            jSonParsed_Kendo.kendo_columns[i].editor = editKendoNumericTextBoxForGridInline;
        }
    }

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

////////////$('.FF_imballaggio_Tipo_Cod_Carico').on('change', function () {
////////////    var grid = $("#tab_elenco_movimenti").getKendoGrid();
////////////    var row = $(this).closest("tr");
////////////    dataItem = grid.dataItem(row);
////////////    var data = grid.dataSource.data();
////////////    for (var i = 0; i < gridResult.dataSource.data().length; i++) {
////////////        alert(gridResult.dataSource.data()[i].ToString());
////////////        //if (parseInt(data[i].Code) == parseInt($(this).attr('name'))) {
////////////        //    gridResult.dataSource.data()[i].Quantity.Set($(this).val());
////////////        //}
////////////    }
////////////    gridResult.refresh();
////////////});

function kReadMovimenti_mod() {

    var data = $('input[name$="hdKendo_RigheDoc"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}

function SelezionaRigaRiepilogo() {
    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = row.parents(".k-grid.k-widget").eq(0).data("kendoGrid");

    var dataItem = grid.dataItem(row);
    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)

    //if (grid.select().length > 0)
    //    $("#btn_avvia_fatturazione_dettaglio").show();
    //else
    //    $("#btn_avvia_fatturazione_dettaglio").hide();
}

function contabDettagliUC_associaRigaCdgProgetti() {
    var rowCheckd = rowIsSelected();
    var tipologiaProgetto = getTipologiaProgetto();
    if (rowCheckd) {
        creaKendoDropDownList("listaCdcWbs", { read: WS_LeggiListaCdgProgetti }, "Imputazione_Nome", "Imputazione_Cod", null, null, null, true);
        //forzo la deselezione, per evitare che mi imposti cmq il primo
        KendoDDL("listaCdcWbs").select(-1);

        document.getElementById('titelModal').innerHTML = TraduzioneMultiResx(resxContabileDettagliUC, "SelezionaUn", "Seleziona un") + " " + tipologiaProgetto;

        $('#CD_UC_window_aggiunta_automatica1').modal('show');
    }
    else {
        kendo.alert(TraduzioneMultiResx(resxContabileDettagliUC, "SelezionaRigaDaAssociare", "Seleziona la riga da associare al") + " " + tipologiaProgetto);
    }
}

function rowIsSelected() {
    var cheked = false;
    var grid = $("#tab_elenco_movimenti").data("kendoGrid");

    grid.select().each(function () {
        cheked = true;
    });
    return cheked;
}

function contabDettagliUC_associaRigaCdgToProgetti() {
   
    var imputazione_Cod = KendoDDL("listaCdcWbs").value();
    var msgErr = "";
    var resposta = "";

    if (imputazione_Cod !== "-1" && imputazione_Cod !== "" && imputazione_Cod !== null) {
        //riprendo i dati che avevo letto, così ho già molte cose valorizzate come idAgenda e dataOraUltimaLettura
        let testataDoc = getDatiContabTestata();
        var descLib = testataDoc.DescrizioneAgenda;
        var dataEmissione = kendo.parseDate($("#inDataEmissione").val());
        var odaOddt = getODAoDDT(); //'0 per ODA e (1 per ddt emesso?


        var grid = $("#tab_elenco_movimenti").data("kendoGrid");
        grid.select().each(function () {

            let item = grid.dataItem(this).toJSON();
            var piva = item.Piva;
            var saCod = item.Sa_Cod;
            var idAgenda = item.Id_Agenda;
            var idMov = item.Id_Mov;
            var idMovDet = item.Id_Mov_Det;
            var lavCod = item.Lav_Cod;
            resposta = WS_AggionaCDGImputazioniDocContabile(piva, saCod, idAgenda, idMov, idMovDet, lavCod, dataEmissione, descLib, imputazione_Cod, odaOddt);
            if (resposta != "") {
                msgErr = msgErr + "<br/>" + resposta;
            }
        });

        if (msgErr === "") {
            MessaggioTuttoOK_Bootstrap(TraduzioneMultiResx(resxContabileDettagliUC, "AssociazioneRiuscita", "Associazione riuscita"), "DIV_Messaggi");
        }
        else {
            MessaggioErrore_Bootstrap(msgErr, "DIV_Messaggi");
        }

        //Ricarico Griglia dettagli movimento
        DocContabileRicercaMovimenti(indirizzoHttp_DocContabile_WS, "tab_elenco_movimenti", $(cIdPiva).val(), $(cIdAgenda).val(), cIdLavCod);
        $('#CD_UC_window_aggiunta_automatica1').modal('hide');

      }
    else {
        var tipologiaProgetto = getTipologiaProgetto();
        alert(TraduzioneMultiResx(resxContabileDettagliUC, "SelezionaUn", "Seleziona un") + " " + tipologiaProgetto);
    }
}


function getTipologiaProgetto() {
    var tipologia = "";
    if (cIdLavCod === enum_LavCod.DDT_Emesso.value || cIdLavCod === enum_LavCod.DDT_Contabilizzato_Emesso.value) {
        tipologia = "CDC";
    }
    else if (cIdLavCod === enum_LavCod.Ordine_Acquisto.value) {
        tipologia = "CDC / WBS";
    }
    return tipologia;
}


function getODAoDDT() {
    //'0 per ODA e (1 per ddt emesso?
    var odaOddt = null;
    if (cIdLavCod === enum_LavCod.DDT_Emesso.value || cIdLavCod === enum_LavCod.DDT_Contabilizzato_Emesso.value) {
        //odaOddt = 1;

        let causaleTrasporto = Get_KendoDDLValue("inCausaleTrasporto", 0);
        if (parseInt(causaleTrasporto) === 15) {
            //sono in un ddt emesso di reso e quindi devo proporre i progetti come se fossi in un ordine di acquisto
            odaOddt = 0;
        } else {
            odaOddt = 1;
        }
    }
    else if (cIdLavCod === enum_LavCod.Ordine_Acquisto.value) {
        odaOddt = 0;
    }
    return odaOddt;
}


function onDataBoundRigheMovimenti(e) {
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");

    if (grid.dataSource._data.length > 0) {
        kendo_AggiustaDimensioneColonne("#" + gridId);
        grid.resize(); //sono costretta a rifarlo, perché sennò si vede dello spazio bianco in fondo alla griglia
        //for (var i = 0; i < grid.columns.length; i++) {
        //grid.autoFitColumn(i);
        //}

        //var wrapperMovimenti = grid.wrapper;
        //var headerMovimenti = wrapperMovimenti.find(".k-grid-header");

        //function resizeFixedMovimenti() {
        //    var paddingRight = parseInt(headerMovimenti.css("padding-right"));
        //    headerMovimenti.css("width", wrapperMovimenti.width() - paddingRight);
        //}

        //function scrollFixedMovimenti() {
        //    var offset = $(this).scrollTop(),
        //        tableOffsetTop = wrapperMovimenti.offset().top,
        //        tableOffsetBottom = tableOffsetTop + wrapperMovimenti.height() - headerMovimenti.height();
        //    if (offset < tableOffsetTop || offset > tableOffsetBottom) {
        //        headerMovimenti.removeClass("fixed-header");
        //    } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && !headerMovimenti.hasClass("fixed")) {
        //        headerMovimenti.addClass("fixed-header");
        //    }
        //}

        //resizeFixedMovimenti();
        //$(window).resize(resizeFixedMovimenti);
        //$(window).scroll(scrollFixedMovimenti);
    }
   

}


// Click modifica riga documento
function ApriModificaRiga(tr_elem, grid_elem, operazione) {

    var dataItem = $(grid_elem).data("kendoGrid").dataItem(tr_elem);

    ApriModificaDettaglioRiga(dataItem, operazione);

}

// Apri modifica dettaglio riga
function ApriModificaDettaglioRiga(dataItem, operazione) {

    var Modalita_Protetta = 0;    

    if (operazione === enum_TipoOperazioneDB.Modifica.value) {

        let Piva = dataItem.key_mov_dett.split("_")[0];

        let Id_Agenda = parseInt(dataItem.key_mov_dett.split("_")[2]);
        let Id_Mov = parseInt(dataItem.key_mov_dett.split("_")[3]);
        let Id_Mov_Det = parseInt(dataItem.key_mov_dett.split("_")[4]);
        let Data_Movimento = dataItem.Data_Movimento;

        if (Data_Movimento == undefined || Data_Movimento == null) {
            Data_Movimento = AGRODATAINIZIO
        }

        var paramcheck = kendo.stringify({
            piva: $(cIdPiva).val(),
            Id_Agenda: Id_Agenda,
            Id_Mov: Id_Mov,
            Id_Mov_Det: Id_Mov_Det,
            Lav_Cod: cIdLavCod,
            Tipo_Operazione: operazione,
            IgnoraAvvisoWarning: false,
            ModuloGiasLicenziato: 0,
            flagAggiornaConteggi: false,
            dataMov: Data_Movimento
        });

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/VerificaEliminaModificaDocumento",
            paramcheck,
            false,
            function (risposta) {
                let risp = JSON.parse(risposta.RispostaStringa);
                Modalita_Protetta = risp.ModalitaProtetta;
                $(hf_ModalitaProtettaRiga).val(Modalita_Protetta);
                Modifica_Riga_Doc(dataItem, operazione);
            },
            function (risposta) {
                if (risposta.RispostaConferma === true) {

                    let risp = JSON.parse(risposta.RispostaStringa);

                    Modalita_Protetta = risp.ModalitaProtetta;

                    $(hf_ModalitaProtettaRiga).val(Modalita_Protetta);

                    $("#confermaEliminazioneDialog").kendoDialog({
                        width: "400px",
                        title: TraduzioneMultiResx(resxContabileDettagliUC, "GestioneModificaDocumenti", "Gestione Modifica Documenti"),
                        closable: false,
                        modal: true,
                        visible: false,
                        content: "<p>" + risp.RispostaStringa + "<p>",
                        actions: [
                            { text: TraduzioneMultiResx(resxContabileDettagliUC, "Conferma", "Conferma"), action: function (e) { Modifica_Riga_Doc(dataItem, operazione); } },
                            { text: TraduzioneMultiResx(resxContabileDettagliUC, "Annulla", "Annulla"), primary: true }
                        ]
                    });
                    $("#confermaEliminazioneDialog").data("kendoDialog").open();
                }
                else {
                    kendo.alert(risposta.Errore);
                }
            }
        );
    }
    else {
    //essendo in sola lettura, me lo posso risparmiare
        $(hf_ModalitaProtettaRiga).val(Modalita_Protetta);
        Modifica_Riga_Doc(dataItem, operazione);
    }


    /*TODO Implementare blocchi logici
    var Blocco_Flag = dataItem.Blocco_Flag;
    
    if (Blocco_Flag == true && operazione == 2) {
        var kendoConfirm = $("<div></div>").kendoConfirm({
            title: "Attenzione",
            messages: { okText: "Sì", cancel: "No" },
            content: "La riga non è modificabile in quanto esistono righe collegate. Aprirla in sola consultazione ?"
        }).data("kendoConfirm");
        kendoConfirm.result.done(function () {
            aprimodifica_riga(dataItem, 0);
        });

        kendoConfirm.open();
    }
    else {*/
    
    /*} */

}

// Click elimina documento
function EliminaRiga(tr_elem, grid_elem, operazione) {

    let grid = $(grid_elem).data("kendoGrid");
    let dataItem = grid.dataItem(tr_elem);
    let cancellaInteroDoc = false;
    let msgConferma = TraduzioneMultiResx(resxContabileDettagliUC, "MessElim_ConfermaRiga", "<p>Eliminare definitivamente questa riga?<p>");

    if (grid.dataSource.data().length === 1) {
        //Ho una sola riga, quindi se la elimino, devo eliminare l'intero documento
        cancellaInteroDoc = true;
        msgConferma = TraduzioneMultiResx(resxContabileDettagliUC, "MessElim_AvvisoUnicaRiga",
            "<p>Questa è l'unica riga del documento, pertanto eliminandola <strong>verrà eliminato l'intero documento.</strong><br>");

        let msgAggiuntivo = "";

        if ($("#tab_griglia_carico").data("kendoGrid") !== undefined &&
            $("#tab_griglia_carico").data("kendoGrid").dataSource.data().length > 0) {
            msgAggiuntivo = TraduzioneMultiResx(resxContabileDettagliUC, "MessElim_ImballiVuotiEntrata", "beni di confezionamento vuoti in entrata");
        }

        if ($("#tab_griglia_scarico").data("kendoGrid") !== undefined &&
            $("#tab_griglia_scarico").data("kendoGrid").dataSource.data().length > 0) {
            if (msgAggiuntivo !== "")
                msgAggiuntivo += ", ";
            msgAggiuntivo += TraduzioneMultiResx(resxContabileDettagliUC, "MessElim_ImballiResi", "beni di confezionamento resi");
        }

        if (msgAggiuntivo !== "") {
            msgConferma += TraduzioneMultiResx(resxContabileDettagliUC, "MessElim_AttenzioneImballi", "<br><strong>ATTENZIONE:</strong> Sono presenti anche") + " " +
                msgAggiuntivo + TraduzioneMultiResx(resxContabileDettagliUC, "MessElim_ImballiElim", "; proseguendo verranno eliminati anche questi.<br>");
        }

        msgConferma += TraduzioneMultiResx(resxContabileDettagliUC, "MessElim_ConfermaDoc",
            "<br>Eliminare definitivamente <strong>questa riga e l'intero documento?</strong><p>");

    }

    //TODO: if griglia carico imballi o griglia reso imballi hanno qualcosa, cambia il msg per far capire che verranno eliminati anche quelli

    //let msgConfermaFinale = "<p>Eliminare definitivamente questa riga?<p>";
    //let msgConfermaFinale = "<p>Questa è l'unica riga del documento, pertanto eliminandola <strong>verrà eliminato l'intero documento.</strong><br>" +
    //    "<br><strong>ATTENZIONE:</strong> Sono presenti anche " + // message empha
    //    "beni di confezionamento vuoti in entrata " +
    //    " e " +
    //    "beni di confezionamento resi" +
    //    "; proseguendo verranno eliminati anche questi.<br>" + // message action
    //    "<br>Eliminare definitivamente <strong>questa riga e l'intero documento?</strong><p>"; // message confirm

    $("#confermaEliminazioneRigaDialog").kendoDialog({
        width: "400px",
        title: TraduzioneMultiResx(resxContabileDettagliUC, "GestioneCancellazioneRiga", "Gestione Cancellazione Riga"),
        closable: false,
        modal: true,
        visible: false,
        content: msgConferma,
        actions: [
            {
                text: TraduzioneMultiResx(resxContabileDettagliUC, "Conferma", "Conferma"), action: function (e) {
                    let Lav_Cod = cIdLavCod;
                    var Data_Movimento = dataItem.Data_Movimento;

                    if (cancellaInteroDoc === true) {

                        Elimina_Intero_Doc(parseInt($(cIdAgenda).val()), Lav_Cod, false, Data_Movimento);

                    } else {

                        let Piva = dataItem.key_mov_dett.split("_")[0];
                        let Id_Agenda = parseInt(dataItem.key_mov_dett.split("_")[2]);
                        let Id_Mov = parseInt(dataItem.key_mov_dett.split("_")[3]);
                        let Id_Mov_Det = parseInt(dataItem.key_mov_dett.split("_")[4]);

                        Elimina_Riga_Doc(Piva, Id_Agenda, Id_Mov, Id_Mov_Det, Lav_Cod, dataItem.Cat_Cod, false, Data_Movimento);
                    }
                }
            },
            { text: TraduzioneMultiResx(resxContabileDettagliUC, "Annulla", "Annulla"), primary: true }
        ]
    });

    $("#confermaEliminazioneRigaDialog").data("kendoDialog").open();

}


// Click modifica documento
function DuplicaRiga(tr_elem, grid_elem, operazione) {

    // TODO
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);
    
    Duplica_Riga_Doc(dataItem, operazione);

}


function changedMateriePrime(e) {
    var grid = $("#tab_elenco_movimenti").data("kendoGrid");
    if (grid.editable) {
        grid.editable.validatable.validateInput(e.sender.element);
        aggiornaQtaMovimento(e, "tab_elenco_movimenti", "prodotto", 0);
    }
}

function changedCelle(e) {
    var grid = $("#tab_elenco_movimenti").data("kendoGrid");
    if (grid.editable) {
        grid.editable.validatable.validateInput(e.sender.element);
    }
}

function changedMagazzini(e) {
    var grid = $("#tab_elenco_movimenti").data("kendoGrid");
    if (grid.editable) {
        grid.editable.validatable.validateInput(e.sender.element);
    }
}

function changedCelleMagazzini(e) {
    var grid = $("#tab_elenco_movimenti").data("kendoGrid");
    if (grid.editable) {
        grid.editable.validatable.validateInput(e.sender.element);
    }
}

//function onSaveRigheMovimenti(e) {

//    // Prendo il sa_cod dal magazzino
//    e.model.Sa_Cod = e.model.key_Dest.substr(e.model.key_Dest.indexOf("_") + 1, e.model.key_Dest.lastIndexOf("_") - e.model.key_Dest.indexOf("_") - 1);
//    for (var eleProd = 0; eleProd < elencoTuttiProdotti.length; eleProd++)
//    {
//        if (elencoTuttiProdotti[eleProd].Mat_Cod === e.model.Mat_Cod)
//        {
//            e.model.Cat_Cod = elencoTuttiProdotti[eleProd].Elem_Cod;
//            e.model.Mat_Des = elencoTuttiProdotti[eleProd].Mat_Des;
//            break;
//        }
//    }
//}

function onEditRigheMovimenti(e) {

    // Gestione della duplicazione di una riga
    //      Vengono copiati solo i valori delle colonne marcate come "Da duplicare""
    //      Questa funzione viaggia in coppia con la funzione Duplica
    if (rigaDuplicataKendoGrid && rigaDaCopiareKendoGrid != null && e.model.isNew() && !e.model.dirty) {

        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");

        rigaDaCopiareKendoGrid.forEach(function (valore, campo) {
            //console.log('[' + campo + '] ' + valore);
            var duplicazioneEffettuata = false;
            for (let r = 0; r < grid.columns.length; r++) {
                let col = grid.columns[r];
                if (col.daDuplicare && col.field !== undefined && col.field === campo) {
                    if (e.model.get(campo) !== valore) {
                        e.container.find("input[name=" + campo + "]").val(valore).change();
                        e.model.set(campo, valore);
                    }
                    duplicazioneEffettuata = true;
                }
            }

            if (!duplicazioneEffettuata)
            {
                // Se non ho trovato il campo fra le colonne della griglia cerco se è una DropDownList (i campi 
                // codice in questo caso non fanno parte delle colonne quindi non li trova)
                if (e.container.find("input[name=" + campo + "]").data("kendoDropDownList") !== undefined) {
                    var keyCampo = e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.dataValueField;
                    var textCampo = null;

                    // nomeCampoEffettivo è un campo valorizzato solo per i parametri qualitativi
                    if (e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.nomeCampoEffettivo !== undefined)
                        textCampo = e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.nomeCampoEffettivo;
                    else
                        textCampo = e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.dataTextField;

                    if (textCampo != null) {
                        for (let r = 0; r < grid.columns.length; r++) {
                            let col = grid.columns[r];
                            if (col.daDuplicare && col.field !== undefined && col.field === textCampo) {
                                if (e.model.get(campo) !== valore) {
                                    e.container.find("input[name=" + keyCampo + "]").val(valore).change();
                                    e.model.set(campo, valore);
                                }
                                duplicazioneEffettuata = true;
                            }
                        }
                    }
                }
            }

            if (!duplicazioneEffettuata)
            {
                //Gestione dei campi che non voglio duplicare per evitare che vengano impostati con i default
                for (var f in grid.dataSource.options.schema.model.fields) {
                    if (grid.dataSource.options.schema.model.fields.hasOwnProperty(f) &&
                        f === campo && 
                        grid.dataSource.options.schema.model.fields[f].defaultValue !== undefined) {
                        //console.log(f + " -> " + grid.dataSource.options.schema.model.fields[f]);
                        if (grid.dataSource.options.schema.model.fields[f].type === "string")
                        {
                            e.container.find("input[name=" + f + "]").val("").change();
                            e.model.set(f, "");
                        }
                        if (grid.dataSource.options.schema.model.fields[f].type === "date") {
                            e.container.find("input[name=" + f + "]").val("").change();
                            e.model.set(f, "");
                        }
                        if (grid.dataSource.options.schema.model.fields[f].type === "number") {
                            e.container.find("input[name=" + f + "]").val(0).change();
                            e.model.set(f, 0);
                        }
                        if (grid.dataSource.options.schema.model.fields[f].type === "boolean") {
                            e.container.find("input[name=" + f + "]").val(false).change();
                            e.model.set(f, false);
                        }
                    }
                }
            }
        });

        rigaDuplicataKendoGrid = false;
        rigaDaCopiareKendoGrid = null;

    }

    var input = e.container.find(".k-input");
    var value = input.val();
    input.keyup(function () {
        value = input.val();
    });

    input.change(function (e) {
        aggiornaQtaMovimento(e, "tab_elenco_movimenti", "", 0);
    });
}

function aggiornaQtaMovimento(e, IDControllo, CampoModificato, ValoreCampoModificato) {

    // N.B.  I campi CampoModificato, ValoreCampoModificato arrivano valorizzati in caso di cambio imballo / contenitore / confezione
    // TODO Non gestito per il momento il cambio del prodotto

    var grid = $("#" + IDControllo).getKendoGrid();
    var row = null;
    if (e.target !== undefined) {
        row = $(e.target).closest("tr");
        CampoModificato = "";
    }
    else
        if (e.sender !== undefined) {
            
            row = $(e.sender.element).closest("tr");
        }
            

    var dataItem = grid.dataItem(row);

    var NrImbMask = dataItem.NrImballaggi === undefined ? 0 : dataItem.NrImballaggi;
    var NrContenitoriMask = dataItem.NrContenitori === undefined ? 0 : dataItem.NrContenitori;
    var NrConfezioniMask = dataItem.NrConfezioni === undefined ? 0 : dataItem.NrConfezioni;
    var KgLordiMask = dataItem.KgLordi === undefined ? 0.0 : dataItem.KgLordi;
    var KgNettiMask = dataItem.KgNetti === undefined ? 0.0 : dataItem.KgNetti;

    var TaraUnitImballiMask = dataItem.FF_imballaggio_Tara_Campionatura === undefined ? 0.0 : dataItem.FF_imballaggio_Tara_Campionatura;
    var TaraUnitContenitoriMask = dataItem.FF_contenitore_Tara_Campionatura === undefined ? 0.0 : dataItem.FF_contenitore_Tara_Campionatura;
    var TaraUnitConfezioniMask = dataItem.FF_confezione_Tara_Campionatura === undefined ? 0.0 : dataItem.FF_confezione_Tara_Campionatura;

    if (e.target !== undefined) {

        var campoMod = $(e.target).data().bind;

        if (e.target.value == "")
            e.target.value = 0;

        if (campoMod === "value:NrImballaggi") {
            CampoModificato = "NrImballaggi";
            NrImbMask = kendo.parseInt(e.target.value);
        }

        if (campoMod === "value:NrContenitori") {
            CampoModificato = "NrContenitori";
            NrContenitoriMask = kendo.parseInt(e.target.value);
        }

        if (campoMod === "value:NrConfezioni") {
            CampoModificato = "NrConfezioni";
            NrConfezioniMask = kendo.parseInt(e.target.value);
        }

        if (campoMod === "value:KgLordi") {
            CampoModificato = "KgLordi";
            KgLordiMask = kendo.parseFloat(e.target.value);
        }

        if (campoMod === "value:KgNetti") {
            CampoModificato = "KgNetti";
            KgNettiMask = kendo.parseFloat(e.target.value);
        }

        if (campoMod === "value:FF_imballaggio_Tara_Campionatura") {
            CampoModificato = "taraImballo";
            TaraUnitImballiMask = kendo.parseFloat(e.target.value);
            dataItem.FF_imballaggio_Tara_Campionatura = TaraUnitImballiMask;
        }

        if (campoMod === "value:FF_contenitore_Tara_Campionatura") {
            CampoModificato = "taraContenitore";
            TaraUnitContenitoriMask = kendo.parseFloat(e.target.value);
            dataItem.FF_contenitore_Tara_Campionatura = TaraUnitContenitoriMask;
        }

        if (campoMod === "value:FF_confezione_Tara_Campionatura") {
            CampoModificato = "taraConfezione";
            TaraUnitConfezioniMask = kendo.parseFloat(e.target.value);
            dataItem.FF_confezione_Tara_Campionatura = TaraUnitConfezioniMask;
        }

    }
    else
        if (e.sender !== undefined) {

            if (CampoModificato == "tipoImballo")
                TaraUnitImballiMask = ValoreCampoModificato;

            if (CampoModificato == "tipoContenitore")
                TaraUnitContenitoriMask = ValoreCampoModificato;

            if (CampoModificato == "tipoConfezione")
                TaraUnitConfezioniMask = ValoreCampoModificato;

        }

    var ImballoCod = dataItem.FF_imballaggio_Tipo_Cod === undefined ? 0 : TrovaMatCodPerBeneConfezionamento(4, $(cIdPiva).val(), dataItem.FF_imballaggio_Tipo_Cod);
    var ContenitoreCod = dataItem.FF_contenitore_Tipo_Cod === undefined ? 0 : TrovaMatCodPerBeneConfezionamento(8, $(cIdPiva).val(), dataItem.FF_contenitore_Tipo_Cod);
    var ConfezioneCod = dataItem.FF_confezione_Tipo_Cod === undefined ? 0 : TrovaMatCodPerBeneConfezionamento(5, $(cIdPiva).val(), dataItem.FF_confezione_Tipo_Cod);

    var NrImbTotRiferimento = dataItem.NrImballaggi === undefined ? 0 : dataItem.NrImballaggi;
    var NrContenitoriTotRiferimento = dataItem.NrContenitori === undefined ? 0 : dataItem.NrContenitori;
    var NrConfezioniTotRiferimento = dataItem.NrConfezioni === undefined ? 0 : dataItem.NrConfezioni;
    var KgLordiTotRiferimento = dataItem.KgLordi === undefined ? 0.0 : dataItem.KgLordi;
    var KgNettiTotRiferimento = dataItem.KgNetti === undefined ? 0.0 : dataItem.KgNetti;

    if (CampoModificato !== "")
    {

        var risultato = calcolaQuantitaInCascata(
            $(cIdPiva).val(), dataItem.Mat_Cod, ImballoCod, ContenitoreCod, ConfezioneCod,
            NrImbMask, NrContenitoriMask, NrConfezioniMask, KgLordiMask, KgNettiMask,
            TaraUnitImballiMask, TaraUnitContenitoriMask, TaraUnitConfezioniMask,
            NrImbTotRiferimento, NrContenitoriTotRiferimento, NrConfezioniTotRiferimento, KgLordiTotRiferimento, KgNettiTotRiferimento,
            CampoModificato, true);

        dataItem.NrImballaggi = risultato.NrImbMask;
        dataItem.NrContenitori = risultato.NrContenitoriMask;
        dataItem.NrConfezioni = risultato.NrConfezioniMask;
        dataItem.KgLordi = risultato.KgLordiMask;
        dataItem.KgNetti = risultato.KgNettiMask;
        if (KendoNumTB("NrImballaggi") !== undefined)
            KendoNumTB("NrImballaggi").value(risultato.NrImbMask);
        if (KendoNumTB("NrContenitori") !== undefined)
            KendoNumTB("NrContenitori").value(risultato.NrContenitoriMask);
        if (KendoNumTB("NrConfezioni") !== undefined)
            KendoNumTB("NrConfezioni").value(risultato.NrConfezioniMask);
        if (KendoNumTB("KgLordi") !== undefined)
            KendoNumTB("KgLordi").value(risultato.KgLordiMask);
        if (KendoNumTB("KgNetti") !== undefined)
            KendoNumTB("KgNetti").value(risultato.KgNettiMask);

        dataItem.dirty = true;
    }
}

// -------   INIZIO AGGIORNAMENTO DB  -------------//
function CreaOggettoCancellazione(daCancellare, utilizzaDestinazione) {

    var lavorazione = new Object();

    if (utilizzaDestinazione === true) {
        lavorazione.piva = daCancellare.key_mov_dett.split("_")[0];
        lavorazione.saCod = parseInt(daCancellare.key_mov_dett.split("_")[1]);
        lavorazione.idAgenda = parseInt(daCancellare.key_mov_dett.split("_")[2]);
        lavorazione.idMov = parseInt(daCancellare.key_mov_dett.split("_")[3]);
        lavorazione.idMovDet = parseInt(daCancellare.key_mov_dett.split("_")[4]);
    } else {
        lavorazione.piva = daCancellare.key_mov_dett.split("_")[0];
        lavorazione.idAgenda = parseInt(daCancellare.key_mov_dett.split("_")[1]);
        lavorazione.idMov = parseInt(daCancellare.key_mov_dett.split("_")[2]);
        lavorazione.idMovDet = parseInt(daCancellare.key_mov_dett.split("_")[3]);
    }

    return lavorazione;
}

function CreaOggettoModifica(daModificare, utilizzaDestinazione) {

    var lavorazione = new Object();

    if (utilizzaDestinazione === true) {
        lavorazione.piva = daModificare.key_mov_dett.split("_")[0];
        lavorazione.saCod = parseInt(daModificare.key_mov_dett.split("_")[1]);
        lavorazione.idAgenda = parseInt(daModificare.key_mov_dett.split("_")[2]);
        lavorazione.idMov = parseInt(daModificare.key_mov_dett.split("_")[3]);
        lavorazione.idMovDet = parseInt(daModificare.key_mov_dett.split("_")[4]);
        lavorazione.tipoDestinazione = parseInt(daModificare.key_mov_dett.split("_")[6]);
        lavorazione.idDestinazione = parseInt(daModificare.key_mov_dett.split("_")[8]);

        //TODO: Per permettere la modifica della cella è necessario passare key_Dest in un nuovo campo
        //lavorazione.tipoDestinazione = parseInt(daModificare.key_Dest.split("_")[0]);
        //lavorazione.saCod = parseInt(daModificare.key_Dest.split("_")[1]);
        //lavorazione.idDestinazione = parseInt(daModificare.key_Dest.split("_")[2]);
    } else {
        lavorazione.piva = daModificare.key_mov_dett.split("_")[0];
        lavorazione.idAgenda = parseInt(daModificare.key_mov_dett.split("_")[1]);
        lavorazione.idMov = parseInt(daModificare.key_mov_dett.split("_")[2]);
        lavorazione.idMovDet = parseInt(daModificare.key_mov_dett.split("_")[3]);
    }

    var numTotImballi = parseInt(daModificare.NrImballaggi);
    var numTotContenitori = parseInt(daModificare.NrContenitori);
    var numTotConfezioni = parseInt(daModificare.NrConfezioni);
    
    var pesoNetto = kendo.parseFloat(daModificare.KgNetti);
    var pesoLordo = kendo.parseFloat(daModificare.KgLordi);
    var taraTotale = pesoLordo - pesoNetto;
    
    if (numTotConfezioni > 0) {
        //38 = numero
        lavorazione.udmCod = 38;
        lavorazione.qta = numTotConfezioni;
        lavorazione.qtaExtra = pesoNetto / numTotConfezioni;
    } else {
        //2 = kg
        lavorazione.udmCod = 2;
        lavorazione.qta = pesoNetto;
        lavorazione.qtaExtra = 1;
    }

    lavorazione.numContenitori = numTotContenitori;
    lavorazione.numImballaggi = numTotImballi;
    
    lavorazione.qtaExtraTotale = pesoNetto;
    lavorazione.tara = taraTotale;

    lavorazione.dataLettura = daModificare.DataOraUltimaLettura;

    return lavorazione;
}

function CreaOggettoModificaCarico(daModificare, utilizzaDestinazione) {

    var lavorazione = CreaOggettoModifica(daModificare, utilizzaDestinazione);

    if (lavorazione !== undefined && lavorazione !== null) {

        lavorazione.elemCod = parseInt(daModificare.Cat_Cod);
        lavorazione.matCod = parseInt(daModificare.Mat_Cod);
        lavorazione.lotto = daModificare.Lotto;
        lavorazione.calCod = parseInt(daModificare.Cal_Cod);

        var paramQualGestiti = RicercaParametriQualitativi(true, $(cIdPiva).val());
        var arrParams = creaArrayCampionature(daModificare, paramQualGestiti);
        if (arrParams !== undefined && arrParams !== null && arrParams.length > 0) {
            lavorazione.listMatPriCampValor = kendoEscapeOggetto(arrParams);
        }
    }

    return lavorazione;
}

//TODO
/*
function CreaOggettoInserimento(daInserire, utilizzaDestinazione) {
    var lavorazione = new Object();

    lavorazione.piva = $(cIdPiva).val();
    lavorazione.idAgenda = parseInt($(cIdAgenda).val());
    //TODO: valorizzare Tipo Lavorazione Des
    lavorazione.tipoLavorazioneDes = "";

    //TODO: per il momento non c'è, quindi imposto secco il 210
    if (daInserire.Cat_Cod === "")
        daInserire.Cat_Cod = 210;
    lavorazione.elemCod = parseInt(daInserire.Cat_Cod);
    
    lavorazione.matCod = daInserire.Mat_Cod;
    //lavorazione.matDes = daInserire.Mat_Des;
    lavorazione.lotto = daInserire.Lotto;
    lavorazione.calCod = 0;

    var numTotImballi = parseInt(daInserire.NrImballaggi);
    var numTotContenitori = parseInt(daInserire.NrContenitori);
    var numTotConfezioni = parseInt(daInserire.NrConfezioni);

    var pesoNetto = kendo.parseFloat(daInserire.KgNetti);
    var pesoLordo = kendo.parseFloat(daInserire.KgLordi);
    var taraTotale = pesoLordo - pesoNetto;

    if (numTotConfezioni > 0) {
        //38 = numero
        lavorazione.udmCod = 38;
        lavorazione.qta = numTotConfezioni;
        lavorazione.qtaExtra = pesoNetto / numTotConfezioni;
    } else {
        //2 = kg
        lavorazione.udmCod = 2;
        lavorazione.qta = pesoNetto;
        lavorazione.qtaExtra = 1;
    }

    lavorazione.qtaExtraTotale = pesoNetto;
    lavorazione.tara = taraTotale;
    lavorazione.numContenitori = numTotContenitori;
    lavorazione.numImballaggi = numTotImballi;

    if (utilizzaDestinazione === true) {
        //TODO: sono da prendere dalla chiave oppure dai campi slegati?!?
        lavorazione.tipoDestinazione = parseInt(daInserire.key_Dest.split("_")[0]);
        lavorazione.saCod = parseInt(daInserire.key_Dest.split("_")[1]);
        lavorazione.idDestinazione = parseInt(daInserire.key_Dest.split("_")[2]);
    }

    var paramQualGestiti = RicercaParametriQualitativi(true, $(cIdPiva).val());
    var arrParams = creaArrayCampionature(daInserire, paramQualGestiti);
    if (arrParams !== undefined && arrParams !== null && arrParams.length > 0) {
        lavorazione.listMatPriCampValor = kendoEscapeOggetto(arrParams);
    }
    
    return lavorazione;
}


function SubmitAggiornaMovimenti(options) {

    var grid = $("#tab_elenco_movimenti").data("kendoGrid");
    var allOk = true;
    var currentData = grid.dataSource.data();

    var righeDaControllare = [];
    for (var d = 0; d < currentData.length; d++) {
        if (currentData[d].isNew())
            righeDaControllare.push(currentData[d].toJSON());
        else if (currentData[d].dirty)
            righeDaControllare.push(currentData[d].toJSON());
    }
    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var errMessage = controllaTipoNrTaraImballi(righeDaControllare, "carico");

    if (errMessage !== "") {
        $("<div></div>").kendoAlert({
            title: "Operazione non consentita",
            content: errMessage
        }).data("kendoAlert").open();
    }
    else {

        // Non ci sono errori, procedo con aggiornamenti

        var updatedRecords = [];
        var newRecords = [];
        var deletedRecords = [];

        for (var c = 0; c < grid.dataSource._destroyed.length; c++)
            deletedRecords.push(grid.dataSource._destroyed[c].toJSON());

        for (var d = 0; d < currentData.length; d++) {
            if (currentData[d].isNew())
                newRecords.push(currentData[d].toJSON());
            else if (currentData[d].dirty)
                updatedRecords.push(currentData[d].toJSON());
        }

        if (deletedRecords.length > 0) {
            var righeCancellate = kendoEscapeOggetto(deletedRecords);

            for (var i = 0; i < deletedRecords.length; i++) {

                var lavorazione = CreaOggettoCancellazione(deletedRecords[i], true);

                if (lavorazione !== undefined && lavorazione !== null) {
                    var lav = kendoEscapeOggetto(lavorazione);

                    var risCanc = CancellaCaricoLavorazione(lavorazione.piva, lav, true);

                    if (risCanc !== undefined && risCanc.RispostaOK === true) {
                        MessaggioTuttoOK_Bootstrap("Eliminazione effettuata correttamente", "DIV_Messaggi");
                    } else if (risCanc !== undefined && risCanc.RispostaOK === false) {
                        allOk = false;
                        MessaggioErrore_Bootstrap(risCanc.RispostaStringa + "<br/>" + "Errore: " + risCanc.Errore,
                            "DIV_Messaggi");
                    } else {
                        allOk = false;
                        MessaggioErrore_Bootstrap("Errore eliminazione", "DIV_Messaggi");
                    }
                }
            }
        }

        if (updatedRecords.length > 0) {
            var righeModificate = kendoEscapeOggetto(updatedRecords);

            for (var j = 0; j < updatedRecords.length; j++) {

                var lavorazioneM = CreaOggettoModificaCarico(updatedRecords[j], true);

                if (lavorazioneM !== undefined && lavorazioneM !== null) {

                    var lavM = kendoEscapeOggetto(lavorazioneM);

                    var risMod = ModificaCaricoLavorazione(lavorazioneM.piva, lavM, true);

                    if (risMod !== undefined && risMod.RispostaOK === true) {
                        MessaggioTuttoOK_Bootstrap("Modifica effettuata correttamente", "DIV_Messaggi");
                    } else if (risMod !== undefined && risMod.RispostaOK === false) {
                        allOk = false;
                        MessaggioErrore_Bootstrap(risMod.RispostaStringa + "<br/>" + "Errore: " + risMod.Errore,
                            "DIV_Messaggi");
                    } else {
                        allOk = false;
                        MessaggioErrore_Bootstrap("Errore modifica", "DIV_Messaggi");
                    }
                }
            }
        }

        if (newRecords.length > 0) {
            var righeInserite = kendoEscapeOggetto(newRecords);

            for (var l = 0; l < newRecords.length; l++) {

                var lavorazioneI = CreaOggettoInserimento(newRecords[l], true);

                if (lavorazioneI !== undefined && lavorazioneI !== null) {

                    var lavI = kendoEscapeOggetto(lavorazioneI);

                    var risIns = InserisciCaricoLavorazione(lavorazioneI.piva, lavI, true);

                    if (risIns !== undefined && risIns.RispostaOK === true) {
                        MessaggioTuttoOK_Bootstrap("Inserimento effettuato correttamente", "DIV_Messaggi");

                    } else if (risIns !== undefined && risIns.RispostaOK === false) {
                        allOk = false;
                        MessaggioErrore_Bootstrap(risIns.RispostaStringa + "<br/>" + "Errore: " + risIns.Errore,
                            "DIV_Messaggi");
                    } else {
                        allOk = false;
                        MessaggioErrore_Bootstrap("Errore inserimento", "DIV_Messaggi");
                    }
                }
            }
        }

        if (allOk) {
            //options.success([]);
            //grid.dataSource._destroyed = [];
            //grid.refresh();

            RicercaMovimenti(indirizzoHttp_DocContabile_WS, null, $(cIdPiva).val(), parseInt($(cIdAgenda).val()), options);
            grid.dataSource.read();
        }
    }
}
*/

// -------   FINE AGGIORNAMENTO DB  -------------//

function controllaTipoNrTaraImballi(righe, tipoOper) {

    // tipoOper può valere "carico" o "scarico"

    var errMessage = "";

    for (var x = 0; x < righe.length; x++) {

        var item = righe[x];
        
        // Imballaggio
        if (item.NrImballaggi != 0 && (item.FF_imballaggio_Tipo_Cod == undefined || item.FF_imballaggio_Tipo_Cod == 0)  ||
            item.NrImballaggi == 0 && item.FF_imballaggio_Tipo_Cod != undefined && item.FF_imballaggio_Tipo_Cod != 0 && tipoOper == "carico")
        {
            errMessage += TraduzioneMultiResx(resxContabileDettagliUC, "TipoENumeroImballaggioDevonoEssereValorizzati", "Tipo e nr imballaggio devono essere entrambi valorizzati") + "<br/>";
        }
        if (item.FF_imballaggio_Tara_Campionatura != undefined && item.FF_imballaggio_Tara_Campionatura != 0 && (item.FF_imballaggio_Tipo_Cod == undefined || item.FF_imballaggio_Tipo_Cod == 0) ||
            (item.FF_imballaggio_Tara_Campionatura == undefined || item.FF_imballaggio_Tara_Campionatura == 0) && item.FF_imballaggio_Tipo_Cod != undefined && item.FF_imballaggio_Tipo_Cod != 0 && tipoOper == "carico")
        {
            errMessage += TraduzioneMultiResx(resxContabileDettagliUC, "TipoETaraImballaggioDevonoEssereValorizzati", "Tipo e tara imballaggio devono essere entrambi valorizzati") + "<br/>";
        }

        // Contenitore
        if (item.NrContenitori != 0 && (item.FF_contenitore_Tipo_Cod == undefined  || item.FF_contenitore_Tipo_Cod == 0) ||
            item.NrContenitori == 0 && item.FF_contenitore_Tipo_Cod != undefined  && item.FF_contenitore_Tipo_Cod != 0 && tipoOper == "carico")
        {
            errMessage += TraduzioneMultiResx(resxContabileDettagliUC, "TipoContenitoreENumeroDevonoEssereValorizzati", "Tipo contenitore e nr devono essere entrambi valorizzati") + "<br/>";
        }
        if (item.FF_contenitore_Tara_Campionatura != undefined && item.FF_contenitore_Tara_Campionatura != 0 && (item.FF_contenitore_Tipo_Cod == undefined || item.FF_contenitore_Tipo_Cod == 0) ||
            (item.FF_contenitore_Tara_Campionatura == undefined || item.FF_contenitore_Tara_Campionatura == 0) && item.FF_contenitore_Tipo_Cod != undefined && item.FF_contenitore_Tipo_Cod != 0 && tipoOper == "carico")
        {
            errMessage += TraduzioneMultiResx(resxContabileDettagliUC, "TipoETaraContenitoreDevonoEssereValorizzati", "Tipo e tara contenitore devono essere entrambi valorizzati") + "<br/>";
        }

        // Confezione
        if (item.NrConfezioni != 0 && (item.FF_confezione_Tipo_Cod == undefined || item.FF_confezione_Tipo_Cod == 0) ||
            item.NrConfezioni == 0 && item.FF_confezione_Tipo_Cod != undefined && item.FF_confezione_Tipo_Cod != 0 && tipoOper == "carico")
        {
            errMessage += TraduzioneMultiResx(resxContabileDettagliUC, "TipoConfezioneENumeroDevonoEssereValorizzati", "Tipo confezione e nr devono essere entrambi valorizzati") + "<br/>";
        }
        //if (item.FF_confezione_Tara_Campionatura != undefined && item.FF_confezione_Tara_Campionatura != 0 && (item.FF_confezione_Tipo_Cod == undefined || item.FF_confezione_Tipo_Cod == 0) ||
        //    (item.FF_confezione_Tara_Campionatura == undefined || item.FF_confezione_Tara_Campionatura == 0) && item.FF_confezione_Tipo_Cod != undefined && item.FF_confezione_Tipo_Cod != 0 && tipoOper == "carico")
        //{
        //    errMessage += "Tipo e tara confezione devono essere entrambi valorizzati" + "<br/>";
        //}
        if (!FF_gest_materiale_vivaistico && (item.KgLordi === undefined || item.KgLordi == 0)) {
            errMessage += TraduzioneMultiResx(resxContabileDettagliUC, "ChilogrammiLordiObbligatori", "Kg Lordi obbligatori") + "<br/>";
        }
        if (item.KgNetti === undefined || item.KgNetti == 0) {
            if (FF_gest_materiale_vivaistico)
                errMessage += TraduzioneMultiResx(resxContabileDettagliUC, "NumeroObbligatorio", "Numero obbligatorio") + "<br/>";
            else
                errMessage += TraduzioneMultiResx(resxContabileDettagliUC, "ChilogrammiNettiObbligatori", "Kg Netti obbligatori") + "<br/>";
        }
    }

    return errMessage;
}

function duplicaRigaKendoGrid(e) {

    var grid = $("#tab_elenco_movimenti").data("kendoGrid");
    var row = $(e.target).closest("tr");

    var hasChanges = grid.dataSource.hasChanges();

    if (!hasChanges) {

        e.preventDefault();

        rigaDaCopiareKendoGrid = grid.dataItem(row);
        rigaDuplicataKendoGrid = true;
        grid.addRow();

    }
    else {
        alert(TraduzioneMultiResx(resxContabileDettagliUC, "SonoPresentiRigheNonSalvateProcederePrimaConIlSalvataggio",
            "Sono presenti righe non salvate: procedere prima con il salvataggio"));
    }
}


/* Cancellare se utilizziamo i check  
function MostraSoloQuantita() {

    var grid = $("#tab_elenco_movimenti").data("kendoGrid");
   
     var data = $('input[name$="hdKendo_RigheDoc"]').val();
     var jSonParsed_Kendo = JSON.parse(data);
     for (var i = 0; i < grid.columns.length; i++) {
         if (grid.columns[i].field === "NrImballaggi" ||
             grid.columns[i].field === "NrContenitori" ||
             grid.columns[i].field === "NrConfezioni" ||
             grid.columns[i].field === "FF_imballaggio_Tara_Campionatura" ||
             grid.columns[i].field === "FF_contenitore_Tara_Campionatura" ||
             grid.columns[i].field === "FF_confezione_Tara_Campionatura" ||
             grid.columns[i].field === "KgLordi" ||
             grid.columns[i].field === "KgNetti")
         {
             grid.showColumn(grid.columns[i].field); 
         }
         else
         {
             grid.hideColumn(grid.columns[i].field);
         }
    }
}

function MostraSoloAnagrafiche() {

    var grid = $("#tab_elenco_movimenti").data("kendoGrid");

    var data = $('input[name$="hdKendo_RigheDoc"]').val();
    var jSonParsed_Kendo = JSON.parse(data);
    for (var i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].field !== "NrImballaggi" &&
            grid.columns[i].field !== "NrContenitori" &&
            grid.columns[i].field !== "NrConfezioni" &&
            grid.columns[i].field !== "FF_imballaggio_Tara_Campionatura" &&
            grid.columns[i].field !== "FF_contenitore_Tara_Campionatura" &&
            grid.columns[i].field !== "FF_confezione_Tara_Campionatura" &&
            grid.columns[i].field !== "KgLordi" &&
            grid.columns[i].field !== "KgNetti") {
            grid.showColumn(grid.columns[i].field);
        }
        else {
            grid.hideColumn(grid.columns[i].field);
        }
    }
}

function MostraTutto() {
    var grid = $("#tab_elenco_movimenti").data("kendoGrid");

    var data = $('input[name$="hdKendo_RigheDoc"]').val();
    var jSonParsed_Kendo = JSON.parse(data);
    for (var i = 0; i < grid.columns.length; i++) {
        
            grid.showColumn(grid.columns[i].field);
    }
}
*/

function SceltaColonne(chk) {

    var grid = $("#tab_elenco_movimenti").data("kendoGrid");

    var data = $('input[name$="hdKendo_RigheDoc"]').val();
    var jSonParsed_Kendo = JSON.parse(data);
    for (var i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].gruppoColonne !== undefined &&
            "chk_Mostra_" + grid.columns[i].gruppoColonne === chk.id) {
            if (chk.checked)
                grid.showColumn(grid.columns[i].field);
            else
                grid.hideColumn(grid.columns[i].field);
        }
    }

    kendo_AggiustaDimensioneColonne("#tab_elenco_movimenti");
    //for (var i = 0; i < grid.columns.length; i++) {
        //grid.autoFitColumn(i);
    //}
}

function ricercaProdotti(e) {
    var grid = $("#tab_elenco_movimenti").getKendoGrid();
    dataItemMovimenti = grid.dataItem(e.closest("tr"));
    //dataItemMovimenti = this.dataItem($(e.currentTarget).closest("tr"));

    popolaAnagProdotti(dataItemMovimenti);
     
}

// TODO INIZIO Ricerca sui prodotti
function popolaAnagProdotti(dataItemMovimenti) {

    var title = TraduzioneMultiResx(resxContabileDettagliUC, "Ricerca", "Ricerca");

    var dialog = $("#tab_ricercaArea").data("kendoWindow");
    //dialog.title(title);
    $('#tab_ricercaArea').parent().find('.k-window-title').html('<b>' + title + '</b>');
    dialog.center().open();
    $("#tab_ricercaArea .container").scrollTop(0);

    var funzioniCRUD = {
        funzioneRead: RicercaProdottiGrid,
        funzioneInsert: null,
        funzioneUpdate: null,
        funzioneDelete: null
    };
    var idModel = "Mat_Cod";
    var campiKendoModel = {
        Mat_Cod: { editable: false, type: "number" },
        Elem_Cod: { editable: false, type: "number" },
        Mat_Des: { editable: false, type: "string" }
    };

    var colonneKendoGrid = [
        {
            command: [{
                click: Scegli, iconClass: "fa fa-check fa-xs", name: "Scegli", text: "" }], width: "40px" //i18n la proprietà name non è da tradurre
        },
        {
            field: "Elem_Cod", title: TraduzioneMultiResx(resxContabileDettagliUC, "Categoria", "Categoria"), width: "300px",
            filterable: {
                cell: {
                    showOperators: false
                }
            }
        },
        {
            field: "Mat_Des", title: TraduzioneMultiResx(resxContabileDettagliUC, "Descrizione", "Descrizione"), width: "300px",
            filterable: {
                cell: {
                    showOperators: false,
                    operator: "contains" ,
                    suggestionOperator: "contains"
                }
            }
        }
    ];
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        columnMenu: false,
        reorderable: true,
        excel: false,
        pdf: false,
        groupable: false
        ,
         filterable: {
             mode: "row"
         }

    };
    var funzioniPrimaDopoEventi = {};
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid("tab_ricerca", // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
     
}

function onChangeRigheMovimenti(arg) {
    var selected = $.map(this.select(), function (item) {
        return $(item).text();
    });

   //  alert("Selected: " + selected.length + " item(s), [" + selected.join(", ") + "]");
   kendoConsole.log("Selected: " + selected.length + " item(s), [" + selected.join(", ") + "]");
}

function Scegli(e) {

    var grid = $("#tab_ricerca").getKendoGrid();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    
    var dialog = $("#tab_ricercaArea").data("kendoWindow");
    dialog.close();

    dataItemMovimenti.Mat_Cod = dataItem.Mat_Cod;
    dataItemMovimenti.Mat_Des = dataItem.Mat_Des;
    var gridMovimenti = $("#tab_elenco_movimenti").getKendoGrid();
    var rowMovimenti = gridMovimenti.tbody.find("tr[data-uid='" + dataItemMovimenti.uid + "']");
    kendoFastRedrawRow(gridMovimenti, rowMovimenti);

   // dataItemMovimenti = null;

}

function RicercaProdottiGrid(options) {
    options.success(elencoTuttiProdotti);
}

function editorLotto(container, options) {

    $('<input type="textbox" value= "#: Lotto #" onClick="ricercaProdotti(this)" name="' + options.field + '"/>')
        .appendTo(container);

    //$('<input data-type="string"' + setValidation(container, options) + ' data-bind="value:' + options.field + '" name="' + options.field + '"/>')
    //    .appendTo(container)
    //    .kendoNumericTextBox({
    //        spinners: false
    //    }).off("keydown");
    
}

 
function KendoGrid_Imposta_Visibilita_ColonneEconomiche(colonneEconomicheVisibili) {

    var grid = $("#tab_elenco_movimenti").data("kendoGrid");

    //var data = $('input[name$="hdKendo_RigheDoc"]').val();

    //var jSonParsed_Kendo = JSON.parse(data);
    if (grid !== undefined) {
        for (var i = 0; i < grid.columns.length; i++) {
            if (grid.columns[i].gruppoColonne !== undefined &&
                (grid.columns[i].gruppoColonne === "Economico" ||
                    grid.columns[i].gruppoColonne === "Imputazioni")) {
                if (colonneEconomicheVisibili)
                    grid.showColumn(grid.columns[i].field);
                else
                    grid.hideColumn(grid.columns[i].field);
            }
        }
    }
}

function stampaBarCode(tr_elem, grid_elem, dettaglio) {
    var dataItem = $(grid_elem).data("kendoGrid").dataItem(tr_elem);
    Stampa_BarCode(dataItem, dettaglio);
}

function ForzaEvasioneDettaglio(tr_elem, grid_elem, forzaEvasione) {
    var dataItem = $(grid_elem).data("kendoGrid").dataItem(tr_elem);

    let msg = "";
    if (forzaEvasione) {
        msg = TraduzioneMultiResx(resxContabileDettagliUC, "EvasioneForzataRigaConferma", "Confermare l'evasione forzata di questa riga?");
    } else {
        msg = TraduzioneMultiResx(resxContabileDettagliUC, "EvasioneForzataRigaAnnulla", "Annullare l'evasione forzata di questa riga?");
    }

    let kendoConfirm = $("<div></div>").kendoConfirm({
        title: TraduzioneMultiResx(resxContabileDettagliUC, "Attenzione", "Attenzione"),
        messages: { okText: TraduzioneMultiResx(resxContabileDettagliUC, "Si", "Sì"), cancel: TraduzioneMultiResx(resxContabileDettagliUC, "No", "No") },
        content: msg
    }).data("kendoConfirm");

    kendoConfirm.result.done(function () {

        let idMovDet = dataItem.Id_Mov_Det;
        let listDet = [idMovDet];

        ForzaEvasioneRigheOrdine(listDet, forzaEvasione);
    });

    kendoConfirm.open();
}

//************************  FINE FORM PRODOTTO  *******************
