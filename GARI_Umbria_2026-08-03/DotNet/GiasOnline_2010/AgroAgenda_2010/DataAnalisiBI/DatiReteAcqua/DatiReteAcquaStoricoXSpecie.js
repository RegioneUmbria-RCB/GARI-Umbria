var chkstategrid = ["[]", "", null, undefined];

function ReadRows(options) {
    var data = $('#' + hdKendoRowsClientID).val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo);
}

function CaricaDati(IDControllo) {
    var funzioniCRUD = {
        funzioneRead: ReadRows
    };

    var dataModel = $('#' + hdKendoModelClientID).val();
    var dataColumns = $('#' + hdKendoColumsClientID).val();


    var idModel = "";
    var campiKendoModel = JSON.parse(dataModel); 
    var colonneKendoGrid = JSON.parse(dataColumns); 

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

function onChange_Specie(e) {
    if ($('#ddlSpecie').val() !== "" && $('#ddlSpecie').val() !== undefined
        && $('#ddlYear').val() !== "" && $('#ddlYear').val() !== undefined
        && $('#dllGruppoConsegna').val() !== "" && $('#dllGruppoConsegna').val() !== undefined
    ) {
        SetModifyState(false);
        LeggiDatiCaricaGriglia();
    } else {
        $("#divKendoGridData").hide();
    }
}

function onChange_Year(e) {
    if ($('#ddlSpecie').val() !== "" && $('#ddlSpecie').val() !== undefined
        && $('#ddlYear').val() !== "" && $('#ddlYear').val() !== undefined
        && $('#dllGruppoConsegna').val() !== "" && $('#dllGruppoConsegna').val() !== undefined
    ) {
        SetModifyState(false);
        LeggiDatiCaricaGriglia();
    } else {
        $("#divKendoGridData").hide();
    }
}

function onChange_GruppoConsegna(e) {
    if ($('#ddlSpecie').val() !== "" && $('#ddlSpecie').val() !== undefined        
        && $('#ddlYear').val() !== "" && $('#ddlYear').val() !== undefined
        && $('#dllGruppoConsegna').val() !== "" && $('#dllGruppoConsegna').val() !== undefined
        ) {
        SetModifyState(false);
        LeggiDatiCaricaGriglia();
    } else {
        $("#divKendoGridData").hide();
    }
}

function LeggiDatiCaricaGriglia() {
    LeggiDatiStorici();
    if (chkstategrid.includes($("#" + hdKendoRowsClientID).val())) {
        $("#btnAddData").show();
        $("#btnSaveData").hide();
        $("#btnCancel").hide();
        $("#divKendoGridData").hide();
    } else {
        $("#btnAddData").hide();
        $("#divKendoGridData").show();
        CaricaDati("divKendoGridData");
    }
}

function SetModify(e) {
    SetModifyState(true);
}

function SetModifyState(state) {
    modify = state;
    $("#btnAddData").hide();
    KendoDDL("ddlSpecie").enable(!state);
    KendoDDL("ddlYear").enable(!state);
    KendoDDL("dllGruppoConsegna").enable(!state);
    if (state === false) {
        $("#btnSaveData").hide();
        $("#btnCancel").hide();

    } else {
        $("#btnSaveData").show();
        $("#btnCancel").show();
    }
}