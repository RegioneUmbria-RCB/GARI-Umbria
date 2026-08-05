var chkstategrid = ["[]", "", null, undefined];

function EsciSenzaSalvare() {
    $("#confermaUscitaDialog").kendoDialog({
        width: "400px",
        title: "",
        closable: false,
        modal: true,
        visible: false,
        content: "Uscire senza salvare?",
        actions: [
            {
                text: "Conferma", action: function (e) {
                    window.history.go(-1);
                }
            },
            { text: "Annulla", primary: true }
        ]
    });
    $("#confermaUscitaDialog").data("kendoDialog").open();
}

function LeggiDatiCaricaGrigliaCoeffXSpecie() {
    LeggiCoefficientiXSpecie();
    if (chkstategrid.includes($("#" + hdKendoCoeffXSpecieRowsClientID).val())) {
        $("#btnAddData").show();
        $("#btnSaveData").hide();
        $("#divCoeffSpecie").hide();
    } else {
        $("#btnAddData").hide();
        $("#btnSaveData").show();
        $("#divCoeffSpecie").show();
        CaricaDatiCoefficienti("divCoeffSpecie");
    }
}

function onChange_Specie(e) {
    if ($('#ddlSpecie').val() !== "" && $('#ddlSpecie').val() !== undefined) {
        LeggiDatiCaricaGrigliaCoeffXSpecie();
    } else {
        $("#btnAddData").hide();
        $("#btnSaveData").hide();
        $("#divCoeffSpecie").hide();
    }
}
//function onChange_Year(e) {
//    if ($('#ddlYear').val() !== "" && $('#ddlYear').val() !== undefined) {
//        PopolaGrigliaParametriGenerali();
//        KendoDDL("ddlSpecie").select(0);
//        KendoDDL("ddlSpecie").trigger("change");
//        //if ($('#ddlSpecie').val() !== "" && $('#ddlSpecie').val() !== undefined) {
//        //    LeggiDatiCaricaGrigliaCoeffXSpecie();
//        //}
//    }
//}



function PopolaGrigliaParametriGenerali() {
    LeggiParametriGenerali();
    if (chkstategrid.includes($("#" + hdKendoParsRowsClientID).val())) {
        $("#btnInitGeneralParameters").show();
        $("#divKendoGridDataPars").hide();
    } else {
        $("#btnInitGeneralParameters").hide();
        $("#divKendoGridDataPars").show();
        CaricaDati("divKendoGridDataPars");
    }
}

function ReadRows(options) {
    var data = $('#' + hdKendoParsRowsClientID).val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo);
}

function SetModify(e) {
    SetModifyState(true);
}

function CaricaDati(IDControllo) {
    var funzioniCRUD = {
        funzioneRead: ReadRows
    };

    var dataModel = $('#' + hdKendoParsModelClientID).val();
    var dataColumns = $('#' + hdKendoParsColumnsClientID).val();


    var idModel = "";
    var campiKendoModel = JSON.parse(dataModel); // KendoDataSet.kendo_model;
    var colonneKendoGrid = JSON.parse(dataColumns); // KendoDataSet.kendo_columns;

    var toolbars = [];

    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        editable: true,
        groupable: false,
        reorderable: false,
        columnMenu: false,
        selectable: false,
        pdf: false,
        excel: true,
        scrollable: true,
        resizable: true,
        pageable: false,
        btnEliminaTuttiFiltri: false
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: SetModify};
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

function ReadRowsCoeff(options) {
    var data = $('#' + hdKendoCoeffXSpecieRowsClientID).val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo);
}

function CaricaDatiCoefficienti(IDControllo) {
    var funzioniCRUD = {
        funzioneRead: ReadRowsCoeff
    };

    var dataModel = $('#' + hdKendoCoeffXSpecieModelClientID).val();
    var dataColumns = $('#' + hdKendoCoeffXSpecieColumnsClientID).val();

    var cols = JSON.parse(dataColumns);
    cols.forEach(function (elem) {
        if (elem.format === "{0:n3}") {
            elem.editor =  customNumberEditor ;
        }
    });

    var idModel = "";
    var campiKendoModel = JSON.parse(dataModel); // KendoDataSet.kendo_model;
    var colonneKendoGrid = cols; // KendoDataSet.kendo_columns;

    var toolbars = [];

    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        editable: true,
        groupable: false,
        reorderable: false,
        columnMenu: false,
        selectable: false,
        pdf: false,
        excel: true,
        scrollable: true,
        resizable: true,
        pageable: false,
        btnEliminaTuttiFiltri: false
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: SetModify };
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

function SetModifyState(state) {
    modify = state;
    if (state === false) {
        $("#btnSaveAll").hide();
        $("#btnCancel").hide();
        //KendoDDL("ddlYear").enable(true);
    } else {
        $("#btnSaveAll").show();
        $("#btnCancel").show();
        //KendoDDL("ddlYear").enable(false);
    }
}

function customNumberEditor(container, options) {
    $('<input id="' + kendo.guid() + '" name="' + options.field + '" data-bind="value:' + options.field +'">')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 3,
            format: 'n3'
        });
}