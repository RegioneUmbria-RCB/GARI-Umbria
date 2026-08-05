//REPORT ELAS

var esporta_carica_elas = 0;
var anno_elas = 0;
var bimestre_elas = 0;
var gestioneCarbResx = [{}];
var resxArrPath = [];
var setup = {};
var tipologia_ELAS = 0;
var cittaTab = "";
const larghezzaStdCampoNumerico = 120;

async function ConfiguraGrigliaElas(IDControllo) {
    var omettiAnnulla = true;
    var funzioneSubmitDaUsare = null;

    funzioneSubmitDaUsare = { /*funzione: SubmitGrid_Dettagli_Impianti,*/ flagInsert: false, flagUpdate: false, flagDelete: false };

    var funzioniCRUD = {
        funzioneRead: CercaReportELAS,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: omettiAnnulla
    };
    var idModel = "ID";
    var campiKendoModel = null;

    campiKendoModel = {
        ID: { editable: false, type: "number", validation: { required: true } },
        CUAA: { editable: false, type: "string", validation: { required: true } },
        azienda: { editable: false, type: "string", validation: { required: true } },
        citta: { editable: false, type: "string", validation: { required: true } },
        Prov: { editable: false, type: "string", validation: { required: true } },
        via: { editable: false, type: "string", validation: { required: true } },
        assegnato_gasolio: { editable: false, type: "number", validation: { required: true } },
        assegnato_benzina: { editable: false, type: "number", validation: { required: true } },
        assegnato_gasolio_serra: { editable: false, type: "number", validation: { required: true } },
        nRichiesta: { editable: false, type: "number", validation: { required: true } },
        annoRichiesta: { editable: false, type: "number", validation: { required: true } },
        ultimo_Avanzamento: { editable: false, type: "date", validation: { required: true } },
        tipo_richiesta: { editable: false, type: "string", validation: { required: true } },        
    };

    var styleElen = /*"background-color: #C4C4EF; */"text-align: center; vertical-align: top";

    var colonneIndirizzo = [];
    colonneIndirizzo.push({ field: "Prov", title: TraduzioneMultiResx(gestioneCarbResx, "Prov", "Prov."), width: 95, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    colonneIndirizzo.push({ field: "citta", title: TraduzioneMultiResx(gestioneCarbResx, "citta", "Citta'"), width: 95, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    colonneIndirizzo.push({ field: "via", title: TraduzioneMultiResx(gestioneCarbResx, "via", "Via"), width: 125, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    
    var colonneKendoGrid = [
        { field: "nRichiesta", title: TraduzioneMultiResx(gestioneCarbResx, "nRichiesta", "Numero Richiesta"), width: 100, headerAttributes: { style: styleElen } },
        { field: "annoRichiesta", title: TraduzioneMultiResx(gestioneCarbResx, "annoRichiesta", "Anno Richiesta"), width: 100, headerAttributes: { style: styleElen } },
        { field: "tipo_richiesta", title: TraduzioneMultiResx(gestioneCarbResx, "tipo_richiesta", "Tipo Richiesta"), width: 110, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "CUAA", title: TraduzioneMultiResx(gestioneCarbResx, "CUAA", "CUAA"), width: 150, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "azienda", title: TraduzioneMultiResx(gestioneCarbResx, "azienda", "Azienda"), width: 250, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "assegnato_gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato_gasolio", "Carb. Ass. Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "assegnato_benzina", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato_benzina", "Carb. Ass. Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "assegnato_gasolio_serra", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato_gasolio_serra", "Carb. Ass. Gas. Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "ultimo_Avanzamento", title: TraduzioneMultiResx(gestioneCarbResx, "ultimo_Avanzamento", "Data Ultimo Avanz."), format: "{0:dd/MM/yyyy}", width: 100, headerAttributes: { style: styleElen } },
        { field: "Prov", title: TraduzioneMultiResx(gestioneCarbResx, "Prov", "Prov."), width: 70, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "citta", title: TraduzioneMultiResx(gestioneCarbResx, "citta", "Citta'"), width: 120, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "via", title: TraduzioneMultiResx(gestioneCarbResx, "via", "Via"), width: 150, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
    ];
    
    var parametriPerLettura = [];
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        //columnMenu: false,
        pdf: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        colonneCustomKendoGrid: []
    };

    var funzioniPrimaDopoEventi = { /*funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti, funzioneDaChiamareDopoDataBound: elencoOnDataBound, funzioneDaChiamareDopoDelete: HideTabDettagli*/ };

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
    var grid = $("#tab_griglia_reportELAS").data("kendoGrid");

    setup = await LeggiSetup(isNaN(parseInt($('#anno')[0].value)) ? 0 : parseInt($('#anno')[0].value))

    $("#btn_esporta_carica")[0].setAttribute("disabled", "")
    $("#btn_elenco_report_elas")[0].setAttribute("disabled", "")

    if (setup !== undefined && setup.length > 0) {
        if (setup[0].Tipologia_Report_Elas > 0) {
            tipologia_ELAS = setup[0].Tipologia_Report_Elas
            $("#btn_esporta_carica")[0].removeAttribute("disabled")
            $("#btn_elenco_report_elas")[0].removeAttribute("disabled")
        }
    }

    grid.bind("excelExport", function (e) {
        var nomeFile = "ReportELAS_" + anno_elas.toString() + "_" + bimestre_elas.toString();
        var estensioneFile = "xlsx";
        e.workbook.fileName = nomeFile + "." + estensioneFile;
        e.preventDefault();
        var worko = new kendo.ooxml.Workbook(e.workbook).toDataURL();
        if (seEsportaCaricaElas()) {
            resetEsportaCaricaElas();
            var fileVero = dataURLtoFile(worko, "Report_ELAS");
            if (fileVero) {
                var reader = new FileReader();
                reader.onload = function (readerEvt) {
                    var binaryString = readerEvt.target.result;
                    SalvaDocumento(btoa(binaryString), nomeFile, estensioneFile, tipologia_ELAS, "Report ELAS");
                };
                reader.readAsBinaryString(fileVero);
            }
        } else {
            kendo.saveAs({
                dataURI: worko,
                fileName: e.workbook.fileName
            });
        }
    });
}

function EsportaECaricaAllegato() {
    var grid = $("#tab_griglia_reportELAS").data("kendoGrid");
    setEsportaCaricaElas();
    var dato = grid.saveAsExcel();
    grid.refresh();
}

function setEsportaCaricaElas() {
    esporta_carica_elas = 1;
}

function resetEsportaCaricaElas() {
    esporta_carica_elas = 0;
}

function seEsportaCaricaElas() {
    return (esporta_carica_elas === 1);
}

//Elenco Inadempienti

var anno_elenco_inadempienti = 0
var tipologia_Inadempienti = 0
var esporta_carica_inadempienti = 0

async function ConfiguraGrigliaElencoInadempienti(IDControllo) {
    var omettiAnnulla = true;
    var funzioneSubmitDaUsare = null;

    funzioneSubmitDaUsare = { /*funzione: SubmitGrid_Dettagli_Impianti,*/ flagInsert: false, flagUpdate: false, flagDelete: false };

    var funzioniCRUD = {
        funzioneRead: CercaElencoInadempienti,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: omettiAnnulla
    };
    var idModel = "Numero";
    var campiKendoModel = null;

    campiKendoModel = {
        Numero: { editable: false, type: "number", validation: { required: true } },
        Pratica: { editable: false, type: "string", validation: { required: true } },
        Anno: { editable: false, type: "number", validation: { required: true } },
        Tipo: { editable: false, type: "string", validation: { required: true } },
        CUAA: { editable: false, type: "string", validation: { required: true } },
        Azienda: { editable: false, type: "string", validation: { required: true } },
        Data_Comp_Rend: { editable: false, type: "date", validation: { required: true } },
        Inadempienza: { editable: false, type: "string", validation: { required: true } },
        //assegnato_gasolio: { editable: false, type: "number", validation: { required: true } },
        //assegnato_benzina: { editable: false, type: "number", validation: { required: true } },
        //assegnato_gasolio_serra: { editable: false, type: "number", validation: { required: true } },
        Acquistato_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Acquistato_Benzina: { editable: false, type: "number", validation: { required: true } },
        Acquistato_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },
        Rimanenza_Iniziale_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Rimanenza_Iniziale_Benzina: { editable: false, type: "number", validation: { required: true } },
        Rimanenza_Iniziale_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },
        Rimanenza_Finale_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Rimanenza_Finale_Benzina: { editable: false, type: "number", validation: { required: true } },
        Rimanenza_Finale_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },
        citta: { editable: false, type: "string", validation: { required: true } },
        Prov: { editable: false, type: "string", validation: { required: true } },
        via: { editable: false, type: "string", validation: { required: true } },
    };

    var styleElen = /*"background-color: #C4C4EF; */"text-align: center; vertical-align: top";

    var colonneIndirizzo = [];
    /*colonneIndirizzo.push({ field: "Prov", title: TraduzioneMultiResx(gestioneCarbResx, "Prov", "Prov."), width: 95, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    colonneIndirizzo.push({ field: "citta", title: TraduzioneMultiResx(gestioneCarbResx, "citta", "Citta'"), width: 95, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    colonneIndirizzo.push({ field: "via", title: TraduzioneMultiResx(gestioneCarbResx, "via", "Via"), width: 125, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } });
    */
    var colonneKendoGrid = [
        { field: "Pratica", title: TraduzioneMultiResx(gestioneCarbResx, "Pratica", "Pratica"), width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Numero", title: TraduzioneMultiResx(gestioneCarbResx, "Numero", "Numero"), width: 100, headerAttributes: { style: styleElen } },
        { field: "Anno", title: TraduzioneMultiResx(gestioneCarbResx, "Anno", "Anno"), width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Tipo", title: TraduzioneMultiResx(gestioneCarbResx, "Tipo", "Tipo"), width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "CUAA", title: TraduzioneMultiResx(gestioneCarbResx, "CUAA", "CUAA"), width: 150, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Azienda", title: TraduzioneMultiResx(gestioneCarbResx, "Azienda", "Azienda"), width: 300, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Inadempienza", title: TraduzioneMultiResx(gestioneCarbResx, "Inadempienza", "Inadempienza"), width: 250, filterable: { multi: true, search: true }, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Data_Comp_Rend", title: TraduzioneMultiResx(gestioneCarbResx, "Data_Comp_Rend", "Data Compilazione Rendicontazione"), width: 140, format: "{0:dd/MM/yyyy}", headerAttributes: { style: styleElen } },
        //{ field: "assegnato_gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato_gasolio", "Assegnato Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        //{ field: "assegnato_benzina", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato_benzina", "Assegnato Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        //{ field: "assegnato_gasolio_serra", title: TraduzioneMultiResx(gestioneCarbResx, "assegnato_gasolio_serra", "Assegnato Gas. Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Acquistato_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Acquistato_Gasolio", "Acquistato Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Acquistato_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Acquistato_Benzina", "Acquistato Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Acquistato_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Acquistato_Gasolio_Serra", "Acquistato Gas. Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rimanenza_Iniziale_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Rimanenza_Iniziale_Gasolio", "Rimanenza Iniziale Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rimanenza_Iniziale_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Rimanenza_Iniziale_Benzina", "Rimanenza Iniziale Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rimanenza_Iniziale_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Rimanenza_Iniziale_Gasolio_Serra", "Rimanenza Iniziale Gas. Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rimanenza_Finale_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Rimanenza_Finale_Gasolio", "Rimanenza Finale Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rimanenza_Finale_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Rimanenza_Finale_Benzina", "Rimanenza Finale Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Rimanenza_Finale_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Rimanenza_Finale_Gasolio_Serra", "Rimanenza Finale Gas. Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Prov", title: TraduzioneMultiResx(gestioneCarbResx, "Prov", "Prov."), width: 70, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "citta", title: TraduzioneMultiResx(gestioneCarbResx, "citta", "Citta'"), width: 120, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "via", title: TraduzioneMultiResx(gestioneCarbResx, "via", "Via"), width: 150, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
    ];

    var parametriPerLettura = [];
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        //columnMenu: false,
        pdf: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        colonneCustomKendoGrid: []
    };

    var funzioniPrimaDopoEventi = { /*funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti, funzioneDaChiamareDopoDataBound: elencoOnDataBound, funzioneDaChiamareDopoDelete: HideTabDettagli*/ };

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
    var grid = $("#" + IDControllo + "").data("kendoGrid");

    setup = await LeggiSetup(isNaN(parseInt($('#annoIna')[0].value)) ? 0 : parseInt($('#annoIna')[0].value))

    $("#btn_esporta_Elenco")[0].setAttribute("disabled", "")
    $("#btn_elenco_report_inadempienti")[0].setAttribute("disabled", "")

    if (setup !== undefined && setup.length > 0) {
        if (setup[0].Tipologia_Elenco_Inadempienti > 0) {
            tipologia_Inadempienti = setup[0].Tipologia_Elenco_Inadempienti
            $("#btn_esporta_Elenco")[0].removeAttribute("disabled")
            $("#btn_elenco_report_inadempienti")[0].removeAttribute("disabled")            
        }
    }

    grid.bind("excelExport", function (e) {
        var nomeFile = "Elenco_Inadempienti_" + anno_elenco_inadempienti.toString();
        var estensioneFile = "xlsx";
        e.workbook.fileName = nomeFile + "." + estensioneFile;
        e.preventDefault();
        var worko = new kendo.ooxml.Workbook(e.workbook).toDataURL();
        if (seEsportaCaricaInadempienti()) {
            resetEsportaCaricaInadempienti();
            var fileVero = dataURLtoFile(worko, "Elenco_Inadempienti");
            if (fileVero) {
                var reader = new FileReader();
                reader.onload = function (readerEvt) {
                    var binaryString = readerEvt.target.result;
                    SalvaDocumento(btoa(binaryString), nomeFile, estensioneFile, tipologia_Inadempienti, "Elenco Inadempienti");
                };
                reader.readAsBinaryString(fileVero);
            }
        } else {
            kendo.saveAs({
                dataURI: worko,
                fileName: e.workbook.fileName
            });
        }
    });
}


function EsportaECaricaAllegatoInadempienti() {
    var grid = $("#tab_griglia_ElencoInadempienti").data("kendoGrid");
    setEsportaCaricaInadempienti();
    var dato = grid.saveAsExcel();
    grid.refresh();
}

function setEsportaCaricaInadempienti() {
    esporta_carica_inadempienti = 1;
}

function resetEsportaCaricaInadempienti() {
    esporta_carica_inadempienti = 0;
}

function seEsportaCaricaInadempienti() {
    return (esporta_carica_inadempienti === 1);
}

//Elenco Trasferimenti

async function ConfiguraGrigliaElencoTrasferimenti(IDControllo) {
    var omettiAnnulla = true;
    var funzioneSubmitDaUsare = null;

    funzioneSubmitDaUsare = { /*funzione: SubmitGrid_Dettagli_Impianti,*/ flagInsert: false, flagUpdate: false, flagDelete: false };

    var funzioniCRUD = {
        funzioneRead: CercaElencoTrasferimenti,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: omettiAnnulla
    };
    var idModel = "CUAA";
    var campiKendoModel = null;

    campiKendoModel = {
        CUAA: { editable: false, type: "string", validation: { required: true } },
        rag_soc: { editable: false, type: "string", validation: { required: true } },
        Tipo_Richiesta: { editable: false, type: "string", validation: { required: true } },
        Confermato_Gasolio: { editable: false, type: "number", validation: { required: true } },
        Confermato_Benzina: { editable: false, type: "number", validation: { required: true } },
        Confermato_Gasolio_Serra: { editable: false, type: "number", validation: { required: true } },
        Data_Trasferimento: { editable: false, type: "date", validation: { required: true } },
        Azienda_Trasferente: { editable: false, type: "string", validation: { required: true } },
        Avanzamento_Richiesta: { editable: false, type: "string", validation: { required: true } },
        Anno: { editable: false, type: "number", validation: { required: true } },
        Numero: { editable: false, type: "number", validation: { required: true } }
    };

    var styleElen = /*"background-color: #C4C4EF; */"text-align: center; vertical-align: top";

    var colonneKendoGrid = [
        { field: "CUAA", title: TraduzioneMultiResx(gestioneCarbResx, "CUAA", "CUAA Ricevente"), width: 130, headerAttributes: { style: styleElen } },
        { field: "rag_soc", title: TraduzioneMultiResx(gestioneCarbResx, "rag_soc", "Ragione Sociale Ricevente"), width: 200, headerAttributes: { style: styleElen } },
        { field: "Tipo_Richiesta", title: TraduzioneMultiResx(gestioneCarbResx, "Tipo_Richiesta", "Tipo"), width: 110, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Confermato_Gasolio", title: TraduzioneMultiResx(gestioneCarbResx, "Confermato_Gasolio", "Carb. Ricevuto Gasolio"), width: larghezzaStdCampoNumerico, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Confermato_Benzina", title: TraduzioneMultiResx(gestioneCarbResx, "Confermato_Benzina", "Carb. Ricevuto Benzina"), width: larghezzaStdCampoNumerico, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Confermato_Gasolio_Serra", title: TraduzioneMultiResx(gestioneCarbResx, "Confermato_Gasolio_Serra", "Carb. Ricevuto Gasolio Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "Data_Trasferimento", title: TraduzioneMultiResx(gestioneCarbResx, "Data_Trasferimento", "Data Trasferimento"), width: 120, format: "{0:dd/MM/yyyy}", headerAttributes: { style: styleElen } },
        { field: "Azienda_Trasferente", title: TraduzioneMultiResx(gestioneCarbResx, "Azienda_Trasferente", "Azienda Trasferente"), width: 120, headerAttributes: { style: styleElen } },
        { field: "Avanzamento_Richiesta", title: TraduzioneMultiResx(gestioneCarbResx, "Avanzamento_Richiesta", "Tipo Pratica"), width: 120, headerAttributes: { style: styleElen } },
        { field: "Anno", title: TraduzioneMultiResx(gestioneCarbResx, "Anno", "Anno Pratica"), width: 120, format: "{0:0}", headerAttributes: { style: styleElen } },
        { field: "Numero", title: TraduzioneMultiResx(gestioneCarbResx, "Numero", "Numero Pratica"), width: 120, format: "{0:n0}", headerAttributes: { style: styleElen } },
    ];

    var parametriPerLettura = [];
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        //columnMenu: false,
        pdf: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        colonneCustomKendoGrid: []
    };

    var funzioniPrimaDopoEventi = { /*funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti, funzioneDaChiamareDopoDataBound: elencoOnDataBound, funzioneDaChiamareDopoDelete: HideTabDettagli*/ };

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

//Segnalazione Accise

var tipologia_Segnalazioni = 0;
var anno_elenco_segnalazioni = 0;
var esporta_carica_segnalazioni_accise = 0;
var rimanenze = 0;
var filtriSegnalatiVisibili = false;
var rendInseribili = false;

async function ElencoSegnalazioneAccise(IDControllo) {
    var omettiAnnulla = true;
    var funzioneSubmitDaUsare = null;

    setup = await LeggiSetup(parseInt($('#annoAcc')[0].value))

    $("#btn_esporta_Accise")[0].setAttribute("disabled", "")
    $("#btn_elenco_report_accise")[0].setAttribute("disabled", "")

    if (setup !== undefined && setup.length > 0) {
        if (setup[0].Tipologia_Report_SegnalazioneAccise > 0) {
            tipologia_Segnalazioni = setup[0].Tipologia_Report_SegnalazioneAccise
            rimanenze = setup[0].Gestione_Rimanenze
            $("#btn_esporta_Accise")[0].removeAttribute("disabled")
            $("#btn_elenco_report_accise")[0].removeAttribute("disabled")
        }
    }

    funzioneSubmitDaUsare = { /*funzione: SubmitGrid_Dettagli_Impianti,*/ flagInsert: false, flagUpdate: false, flagDelete: false };

    var funzioniCRUD = {
        funzioneRead: CercaElencoSegnalazioni,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: omettiAnnulla
    };
    var idModel = "Numero";
    var campiKendoModel = null;

    if (!rendInseribili) {
        $("#btn_segnalazione")[0].removeAttribute("disabled", "");
        $("#btn_annulla_segnalazione")[0].removeAttribute("disabled", "");
        $(".dataSegn").show();
        funzioniCRUD.checkBoxFunction = kEventoSelezionaRigaAccise;
    } else {
        $(".dataSegn").hide();
        $("#btn_segnalazione")[0].setAttribute("disabled", "")
        $("#btn_annulla_segnalazione")[0].setAttribute("disabled", "");
    }

    campiKendoModel = {
        Caso: { editable: false, type: "string", validation: { required: true } },
        Pratica: { editable: false, type: "string", validation: { required: true } },
        Numero: { editable: false, type: "string", validation: { required: true } },
        Anno: { editable: false, type: "number", validation: { required: true } },
        Tipo: { editable: false, type: "string", validation: { required: true } },
        CUAA: { editable: false, type: "string", validation: { required: true } },
        Richiesta_Cod: { editable: false, type: "number", validation: { required: true } },
        Piva: { editable: false, type: "string", validation: { required: true } },
        Azienda: { editable: false, type: "string", validation: { required: true } },
        citta: { editable: false, type: "string", validation: { required: true } },
        Prov: { editable: false, type: "string", validation: { required: true } },
        via: { editable: false, type: "string", validation: { required: true } },
        AcqGas: { editable: false, type: "number", validation: { required: true } },
        AcqBenz: { editable: false, type: "number", validation: { required: true } },
        AcqSerra: { editable: false, type: "number", validation: { required: true } },
        RimGas: { editable: false, type: "number", validation: { required: true } },
        RimBenz: { editable: false, type: "number", validation: { required: true } },
        RimSerra: { editable: false, type: "number", validation: { required: true } },
        RendGas: { editable: false, type: "number", validation: { required: true } },
        RendBenz: { editable: false, type: "number", validation: { required: true } },
        RendSerra: { editable: false, type: "number", validation: { required: true } },
        RimFinGas: { editable: false, type: "number", validation: { required: true } },
        RimFinBenz: { editable: false, type: "number", validation: { required: true } },
        RimFinSerra: { editable: false, type: "number", validation: { required: true } },
        RecAccGas: { editable: false, type: "number", validation: { required: true } },
        RecAccBenz: { editable: false, type: "number", validation: { required: true } },
        RecAccSerra: { editable: false, type: "number", validation: { required: true } },
        Causale: { editable: false, type: "number", validation: { required: true } },
        CausaleDescr: { editable: false, type: "string", validation: { required: true } },
        Data_Segnalazione: { editable: false, type: "date", validation: { required: true } },
        Utente_Segnalazione: { editable: false, type: "string", validation: { required: true } },        
    };

    var styleElen = /*"background-color: #C4C4EF; */"text-align: center; vertical-align: top";

    var colonneKendoGrid = [
        //{ field: "Caso", title: TraduzioneMultiResx(gestioneCarbResx, "Caso", "Caso"), width: 70, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Pratica", title: TraduzioneMultiResx(gestioneCarbResx, "Pratica", "Pratica"), width: 110, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Numero", title: TraduzioneMultiResx(gestioneCarbResx, "Numero", "Numero"), width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Anno", title: TraduzioneMultiResx(gestioneCarbResx, "Anno", "Anno"), width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Tipo", title: TraduzioneMultiResx(gestioneCarbResx, "Tipo", "Tipo"), width: 100, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "CUAA", title: TraduzioneMultiResx(gestioneCarbResx, "CUAA", "CUAA"), width: 150, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Azienda", title: TraduzioneMultiResx(gestioneCarbResx, "Azienda", "Azienda"), width: 300, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },

        { field: "Prov", title: TraduzioneMultiResx(gestioneCarbResx, "Prov", "Prov."), width: 70, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "citta", title: TraduzioneMultiResx(gestioneCarbResx, "citta", "Citta'"), width: 120, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "via", title: TraduzioneMultiResx(gestioneCarbResx, "via", "Via"), width: 150, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },

        { field: "RimGas", title: TraduzioneMultiResx(gestioneCarbResx, "RimInizGas", "Rimanenza Iniziale Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "RimBenz", title: TraduzioneMultiResx(gestioneCarbResx, "RimInizBenz", "Rimanenza Iniziale Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "RimSerra", title: TraduzioneMultiResx(gestioneCarbResx, "RimInizSerra", "Rimanenza Iniziale Gasolio Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "AcqGas", title: TraduzioneMultiResx(gestioneCarbResx, "AcqGas", "Acquistato Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "AcqBenz", title: TraduzioneMultiResx(gestioneCarbResx, "AcqBenz", "Acquistato Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "AcqSerra", title: TraduzioneMultiResx(gestioneCarbResx, "AcqSerra", "Acquistato Gasolio Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "RendGas", title: TraduzioneMultiResx(gestioneCarbResx, "RendGas", "Rendicontato Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "RendBenz", title: TraduzioneMultiResx(gestioneCarbResx, "RendBenz", "Rendicontato Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "RendSerra", title: TraduzioneMultiResx(gestioneCarbResx, "RendSerra", "Rendicontato Gasolio Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "RimFinGas", title: TraduzioneMultiResx(gestioneCarbResx, "AcqGas", "Rimanenza Finale Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "RimFinBenz", title: TraduzioneMultiResx(gestioneCarbResx, "AcqBenz", "Rimanenza Finale Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "RimFinSerra", title: TraduzioneMultiResx(gestioneCarbResx, "AcqSerra", "Rimanenza Finale Gasolio Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        { field: "RecAccGas", title: TraduzioneMultiResx(gestioneCarbResx, "RecAccGas", "Recupero Accise Gasolio"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "RecAccBenz", title: TraduzioneMultiResx(gestioneCarbResx, "RecAccBenz", "Recupero Accise Benzina"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },
        { field: "RecAccSerra", title: TraduzioneMultiResx(gestioneCarbResx, "RecAccSerra", "Recupero Accise Gasolio Serra"), width: larghezzaStdCampoNumerico, format: "{0:n0}", headerAttributes: { style: styleElen } },

        //{ field: "Causale", title: TraduzioneMultiResx(gestioneCarbResx, "Cod. Caus.", "Cod. Caus."), width: 70, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "CausaleDescr", title: TraduzioneMultiResx(gestioneCarbResx, "Causale", "Causale"), width: 250, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Data_Segnalazione", title: TraduzioneMultiResx(gestioneCarbResx, "Data_Segnalazione", "Data Segnalazione"), width: 110, format: "{0:dd/MM/yyyy}", filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },
        { field: "Utente_Segnalazione", title: TraduzioneMultiResx(gestioneCarbResx, "Utente_Segnalazione", "Utente Segnalazione"), width: 150, filterable: { multi: true, search: true }, headerAttributes: { style: styleElen } },

    ];

    var parametriPerLettura = [];
    var parametriDataSource = {};

    var parametriKendoGrid = {
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        //columnMenu: false,
        pdf: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        colonneCustomKendoGrid: []
    };

    var funzioniPrimaDopoEventi = { /*funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti, funzioneDaChiamareDopoDataBound: elencoOnDataBound, funzioneDaChiamareDopoDelete: HideTabDettagli*/ };

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
    var grid = $("#" + IDControllo + "").data("kendoGrid");

    grid.bind("excelExport", function (e) {
        var nomeFile = "Segnalazioni_recupero_accise_" + anno_elenco_segnalazioni.toString();
        var estensioneFile = "xlsx";
        e.workbook.fileName = nomeFile + "." + estensioneFile;
        e.preventDefault();
        var worko = new kendo.ooxml.Workbook(e.workbook).toDataURL();
        if (seEsportaCaricaSegnalazioniAccise()) {
            resetEsportaCaricaSegnalazioniAccise();
            var fileVero = dataURLtoFile(worko, "Segnalazioni_recupero_accise_");
            if (fileVero) {
                var reader = new FileReader();
                reader.onload = function (readerEvt) {
                    var binaryString = readerEvt.target.result;
                    SalvaDocumento(btoa(binaryString), nomeFile, estensioneFile, tipologia_Segnalazioni, "Segnalazioni Recupero Accise");
                };
                reader.readAsBinaryString(fileVero);
            }
        } else {
            kendo.saveAs({
                dataURI: worko,
                fileName: e.workbook.fileName
            });
        }
    });
}

function kEventoSelezionaRigaAccise(e) {

    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#tab_griglia_SegnalazioniAccise").data("kendoGrid"),
        dataItem = grid.dataItem(row);


    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
}

async function Segnala() {
    var grid = $("#tab_griglia_SegnalazioniAccise").data("kendoGrid");
    var dati = grid.dataSource.data();
    let selected = dati.filter((el) => { return el.Selected == true })
    await SegnalaSelezionati(selected);
    ElencoSegnalazioneAccise("tab_griglia_SegnalazioniAccise");
}

async function AnnullaSelezionati() {
    var grid = $("#tab_griglia_SegnalazioniAccise").data("kendoGrid");
    var dati = grid.dataSource.data();
    let selected = dati.filter((el) => { return el.Selected == true })
    WaitFrame.show();
    await AnnullaSegnalazioneSelezionati(selected);
    ElencoSegnalazioneAccise("tab_griglia_SegnalazioniAccise");
}

var exportExcelSenzaColonneIndesiderate = true
function EsportaECaricaAllegatoSegnalazioniAccise() {
    var grid = $("#tab_griglia_SegnalazioniAccise").data("kendoGrid");
    if (!rendInseribili && exportExcelSenzaColonneIndesiderate) {
        exportExcelSenzaColonneIndesiderate = false;
        grid.hideColumn(0);
        setTimeout(function () {
            EsportaECaricaAllegatoSegnalazioniAccise();
        });
    }
    else {
        exportExcelSenzaColonneIndesiderate = true;
        setEsportaCaricaSegnalazioniAccise();
        var dato = grid.saveAsExcel();
        grid.refresh();
    }
}

function setEsportaCaricaSegnalazioniAccise() {
    esporta_carica_segnalazioni_accise = 1;
}

function resetEsportaCaricaSegnalazioniAccise() {
    esporta_carica_segnalazioni_accise = 0;
}

function seEsportaCaricaSegnalazioniAccise() {
    return (esporta_carica_segnalazioni_accise === 1);
}


//Funzioni Utility

function StatiPratiche_Load() {

    $('#statoPratica').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiStatiPratiche } },
        dataTextField: "stato",
        dataValueField: "cod",
        optionLabel: { "stato": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + " TUTTI", "cod": "-1" },
        autoWidth: true,
        dataBound: ddlStatiPratiche_OnDataBound
    });

}

function ddlStatiPratiche_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        //ddlStatiPratiche.onchange(); //forzo l'evento di onchange
    }
}

function Citta_Load(tab) {
    WaitFrame.show();
    cittaTab = tab;
    $(tab).kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiCitta } },
        dataTextField: "citta",
        dataValueField: "proCom",
        //optionLabel: { "citta": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + " TUTTE", "proCom": -1 },
        autoWidth: true,
        dataBound: ddlCitta_OnDataBound
    });

}

function ddlCitta_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(0); //seleziono l'elemento 
        //ddlStatiPratiche.onchange(); //forzo l'evento di onchange
    } else if (KendoDDL("prov").value() == -1) {
        this.select(0);
    } else if (KendoDDL("provAcc").value() == -1) {
        this.select(0);
    }
}

function Prov_Load(tab) {

    $(tab).kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiProv } },
        dataTextField: "PROVINCIA",
        dataValueField: "PROV",
        //optionLabel: { "PROVINCIA": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + "...", "PROV": "" },
        autoWidth: true,
        dataBound: ddlProv_OnDataBound
    });

}

function ddlProv_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        ddlProv.onchange(); //forzo l'evento di onchange
    }
}

function ddlProv_Change(e) {
    Citta_Load('#citta');
    KendoDDL("citta").value("-1");
    //$("#citta").show();
}

function ddlProvAcc_Change(e) {
    Citta_Load('#cittaAcc');
    KendoDDL("cittaAcc").value("-1");
    //$("#citta").show();
}

async function checkTermineUltimo(e) {
    let anno = $('#annoAcc')[0].value;
    let oltreUltimaData = await LeggiTermineUltimoRendicontazione(anno)
    if (!oltreUltimaData) {
        rendInseribili = true;
        $(".divAlertAnno").show();
    } else {
        $(".divAlertAnno").hide();
        rendInseribili = false;
    }
}

function dataURLtoFile(dataurl, filename) {
    var arr = dataurl.split(","),
        mime = arr[0].match(/:(.*?);/)[1],
        bstr = atob(arr[arr.length - 1]),
        n = bstr.length,
        u8arr = new Uint8Array(n);
    while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
    }
    return new File([u8arr], filename, { type: mime });
}

function getYear() {
    if ($.cookie("UMA.Anno") != null && !isNaN($.cookie("UMA.Anno"))) {
        return parseInt($.cookie("UMA.Anno"));
    } else {
        var date = new Date();
        let anno = new Date().getFullYear();
        date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
        $.cookie("UMA.Anno", anno, { expires: date, path: '/' });
        return anno;
    }
}

function setYear(anno) {
    var date = new Date();
    date.setTime(date.getTime() + (20 * 365 * 24 * 60 * 60 * 1000));
    $.cookie("UMA.Anno", anno, { expires: date, path: '/' });
}

function ElencoReportControlloElas(e) {
    const DOCUMENTALE = 1;
    var url = GetUrlDocAgenda2010(cIdPivaSuperUser, tipologia_ELAS, 0, 0, 0, "Read", DOCUMENTALE)
    apriKendoWindowTestataGriglia(url, "Elenco Report ELAS U.M.A. Carburanti");
}

function ElencoReportControlloInadempienti(e) {
    const DOCUMENTALE = 1;
    var url = GetUrlDocAgenda2010(cIdPivaSuperUser, tipologia_Inadempienti, 0, 0, 0, "Read", DOCUMENTALE)
    apriKendoWindowTestataGriglia(url, "Elenco Report Inadempienti U.M.A. Carburanti");
}

function ElencoReportControlloSegnalazioneAccise(e) {
    const DOCUMENTALE = 1;
    var url = GetUrlDocAgenda2010(cIdPivaSuperUser, tipologia_Segnalazioni, 0, 0, 0, "Read", DOCUMENTALE)
    apriKendoWindowTestataGriglia(url, "Elenco Report Segnalazione Recupero Accise U.M.A. Carburanti");
}
function apriKendoWindowTestataGriglia(url, title) {

    $(document.body).append('<div id="tab_documentale"></div>');
    $('#tab_documentale').kendoWindow({
        title: title,
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            setTimeout(function () {
                $('#tab_documentale').kendoWindow('destroy');
            }, 200);
        }
    }).data('kendoWindow').center().maximize();
}

function mostraFiltriSegnalati(e) {
    filtriSegnalatiVisibili = !filtriSegnalatiVisibili;
    if (filtriSegnalatiVisibili) {
        $(".sagnalatiFiltri").show();
    } else {
        $(".sagnalatiFiltri").hide();
    }
}