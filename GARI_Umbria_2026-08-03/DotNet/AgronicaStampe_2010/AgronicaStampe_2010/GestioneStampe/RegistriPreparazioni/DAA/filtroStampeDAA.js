
function popolaGrigliaRiepilogoGaranzie(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: RicercaRiepilogoGaranzieCircolazione,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True"
    };
    var idModel = "ID_GaranziaCircolazione";
    var campiKendoModel = {
        ID_GaranziaCircolazione: { editable: false, type: "number" },
        Importo_Impegnato: { editable: false, type: "number" },
        Importo_Svincolato: { editable: false, type: "number" },
        Tipo: { editable: false, type: "string" },
        RiportoFinePeriodo: { editable: false, type: "number" },
        RiportoPeriodoPrecedente: { editable: false, type: "number" },
        data_emis_documento_AAAAMMGG: { editable: false, type: "date" },
        SaldoProgressivo: { editable: false, type: "number" },

    };
    var colonneKendoGrid = [
        { field: "data_emis_documento_AAAAMMGG", title: "Data", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;"}, template: '#=templateData(data.data_emis_documento_AAAAMMGG)#' },    
        { field: "Tipo", title: "Tipo", headerAttributes: { style: "text-align: center" }, attributes: { style: "text-align:center;"} },
        { field: "RiportoPeriodoPrecedente", title: "Riporto Periodo Prec.", format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;"} },
        { field: "Importo_Impegnato",  title: "Imp. Impegnato", format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;"} },
        { field: "Importo_Svincolato", title: "Imp. Svincolato", format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;"} },
        { field: "SaldoProgressivo", title: "Saldo Progr..", format: "{0:n2}", headerAttributes: { style: "text-align: right" }, attributes: { style: "text-align:right;"} }
    ];

    
    var parametriKendoGrid = {
        pdf: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
    };

    var parametriPerLettura = null;
    var parametriDataSource = { pagesize: 25 };
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: postDataBoundRiepilogoGaranzie };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

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

function templateData(data) {
    if (kendo.toString(data, "dd/MM/yyyy") === "01/01/1900") return "";
    return kendo.toString(data, "dd/MM/yyyy HH:mm:ss");
}

function postDataBoundRiepilogoGaranzie(e) {
//    var gridId = e.sender.element[0].id;
//    var grid = $("#" + gridId).data("kendoGrid");
//    for (var i = 0; i < grid.columns.length; i++) {
//        grid.autoFitColumn(i);
//    }
}
