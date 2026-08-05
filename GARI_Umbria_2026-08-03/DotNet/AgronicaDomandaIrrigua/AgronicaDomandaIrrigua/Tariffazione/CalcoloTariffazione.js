function VisualizzaFinestra(flag) {
    if (flag === false) {
        $("#id_MainContainer").addClass("transparent");
    } else {
        $("#id_MainContainer").removeClass("transparent");
    }
}

function ShowTab(idtab, flag) {
    let tabstrip = $("#tabstrip").getKendoTabStrip();

    let tabToShow = tabstrip.tabGroup.children().eq(idtab);

    tabstrip.enable(tabToShow, flag);

    if (!flag) {
        $(tabstrip.items()[idtab]).addClass("hidden");
    } else {
        $(tabstrip.items()[idtab]).removeClass("hidden");
        tabstrip.select(tabToShow);
    }
}

function VisualizzaFinestra(flag) {
    if (flag === false) {
        $("#id_MainContainer").addClass("transparent");
    } else {
        $("#id_MainContainer").removeClass("transparent");
    }
}

function ReadDati(options) {
    var data = JSON.parse($(cIdDatiCalcoloTariffazione).val());
    options.success(data.tariffazioneAzienda);
}

function popolaGrigliaRisultati(IDControllo) {
    var funzioniCRUD = {
        funzioneRead: ReadDati
    };

    var idModel = "idIdDomanda";
    var campiKendoModel = {
        IdDomanda: { editable: false, type: "number" },
        RagioneSociale: { editable: false, type: "string" },
        piva: { editable: false, type: "string" },
        pivaReale: { editable: false, type: "string" },
        cuaa: { editable: false, type: "string" },
        CodiceSDI: { editable: false, type: "string" },
        SuperficieTotale: { editable: false, type: "number" },
        VolumeTotale: { editable: false, type: "number" },
        Incidenza: { editable: false, type: "number" },
        IncidenzaFasciaT1: { editable: false, type: "number" },
        IncidenzaFasciaT2: { editable: false, type: "number" },
        IncidenzaFasciaT3: { editable: false, type: "number" },
        VolumeFasciaT1: { editable: false, type: "number" },
        VolumeFasciaT2: { editable: false, type: "number" },
        VolumeFasciaT3: { editable: false, type: "number" },
        QuotaFissa: { editable: false, type: "number" },
        QuotaVariabileT1: { editable: false, type: "number" },
        QuotaVariabileT2: { editable: false, type: "number" },
        QuotaVariabileT3: { editable: false, type: "number" },
        ImponibileQuotaFissa: { editable: false, type: "number" },
        ImponibileQuotaVariabileT1: { editable: false, type: "number" },
        ImponibileQuotaVariabileT2: { editable: false, type: "number" },
        ImponibileQuotaVariabileT3: { editable: false, type: "number" },
        TotaleImponibile: { editable: false, type: "number" }
    };

    var colonneKendoGrid = [
        { field: "IdDomanda", title: "Id domanda", hidden: true },
        { field: "RagioneSociale", title: "Ragione Sociale", filterable: { multi: true, search: true }, width: 200 },
        { field: "pivaReale", title: "Partita Iva", filterable: { multi: true, search: true }, width: 150 },
        { field: "cuaa", title: "CUAA", filterable: { multi: true, search: true }, hidden: true },
        { field: "CodiceSDI", title: "Codice SDI", hidden: true },
        { field: "SuperficieTotale", title: "Sup. Tot. (Ha)", format: '{0:0.0000}', filterable: { multi: true, search: true }, width: 150, attributes: { style: "text-align:right;" } },
        { field: "VolumeTotale", title: "Vol. Tot. (mc)", format: '{0:##,#.00000}', filterable: { multi: true, search: true }, width: 150, attributes: { style: "text-align:right;" } },
        { field: "Incidenza", title: "Incidenza (mc-Ha)", format: '{0:##,#.00000}', filterable: { multi: true, search: true }, width: 150, attributes: { style: "text-align:right;" } },
        { field: "IncidenzaFasciaT1", title: "Incidenza fascia 1 (mc-Ha)", format: '{0:##,#.00000}', filterable: { multi: true, search: true }, width: 200, attributes:{style:"text-align:right;"}},
        { field: "IncidenzaFasciaT2", title: "Incidenza fascia 2 (mc-Ha)", format: '{0:##,#.00000}', filterable: { multi: true, search: true }, width: 200, attributes:{style:"text-align:right;"}},
        { field: "IncidenzaFasciaT3", title: "Incidenza fascia 3 (mc-Ha)", format: '{0:##,#.00000}', filterable: { multi: true, search: true }, width: 200, attributes: { style: "text-align:right;" }},
        { field: "VolumeFasciaT1", title: "Vol. fascia 1 (mc)", format: '{0:##,#.00000}', filterable: { multi: true, search: true }, width: 150 , attributes:{style:"text-align:right;"}},
        { field: "VolumeFasciaT2", title: "Vol. fascia 2 (mc)", format: '{0:##,#.00000}', filterable: { multi: true, search: true }, width: 150 , attributes:{style:"text-align:right;"}},
        { field: "VolumeFasciaT3", title: "Vol. fascia 3 (mc)", format: '{0:##,#.00000}', filterable: { multi: true, search: true }, width: 150, attributes: { style: "text-align:right;" }},
        { field: "QuotaFissa", title: "Quota fissa x Ha", format: '{0:c2}', filterable: { multi: true, search: true }, width: 150, attributes: { style: "text-align:right;" }},
        { field: "QuotaVariabileT1", title: "Quota var. fascia 1", format: '{0:c5}', filterable: { multi: true, search: true }, width: 150, attributes:{style:"text-align:right;"}},
        { field: "QuotaVariabileT2", title: "Quota var. fascia 2", format: '{0:c5}', filterable: { multi: true, search: true }, width: 150, attributes:{style:"text-align:right;"}},
        { field: "QuotaVariabileT3", title: "Quota var. fascia 3", format: '{0:c5}', filterable: { multi: true, search: true }, width: 150, attributes: { style: "text-align:right;" }},
        { field: "ImponibileQuotaFissa", title: "Imp. quota fissa", format: '{0:c2}', filterable: { multi: true, search: true }, width: 150, attributes: { style: "text-align:right;" }},
        { field: "ImponibileQuotaVariabileT1", title: "Imp. quota var fascia 1", format: '{0:c2}', filterable: { multi: true, search: true }, width: 200, attributes:{style:"text-align:right;"} },
        { field: "ImponibileQuotaVariabileT2", title: "Imp. quota var fascia 2", format: '{0:c2}', filterable: { multi: true, search: true }, width: 200, attributes:{style:"text-align:right;"} },
        { field: "ImponibileQuotaVariabileT3", title: "Imp. quota var fascia 3", format: '{0:c2}', filterable: { multi: true, search: true }, width: 200, attributes: { style: "text-align:right;" } },
        { field: "TotaleImponibile", title: "Totale imponibile", format: '{0:c2}', filterable: { multi: true, search: true }, width: 150, attributes: { style: "text-align:right;" } }
    ];

    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        editable: true,
        groupable: false,
        reorderable: false,
        columnMenu: true,
        selectable: false,
        pdf: false,
        scrollable: true,
        resizable: true,
        pageable: false,
        btnEliminaTuttiFiltri: false
    };

    var funzioniPrimaDopoEventi = {  };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

function onDataBoundRighe(e) {
    var grid = $("#divKendoOut").data('kendoGrid');
    for (var i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].width === undefined) {
            grid.autoFitColumn(i);
        }
    }
}