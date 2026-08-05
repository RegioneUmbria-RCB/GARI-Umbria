
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////     GRIGLIE    ///////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////



function ConfiguraGrigliaFasi(IDControllo, Key_Padre) {

    var omettiAnnulla = true;
    
    var funzioneSubmitDaUsare = null;
    
    funzioneSubmitDaUsare = { funzione: SubmitGridFasi, flagInsert: true, flagUpdate: false, flagDelete: true };
    //}
    var funzioniCRUD = {
        funzioneRead: RicercaFasi,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false
    }; 

    var idModel = "Fase_Cod";
    var campiKendoModel = null;
    
    campiKendoModel = {
        Fase_Cod: { editable: false, type: "number" },
        veg_cod: { editable: false, type: "number", validation: { required: true } },
        veg_des: { editable: true, type: "string", validation: { required: true } },
        Mat_Cod: { editable: false, type: "number", validation: { required: true } },
        Mat_Des_Esteso: { editable: true, type: "string", validation: { required: true } },
        Valore9: { editable: true, type: "number", validation: { required: true } },
        Superficie: { editable: true, type: "number", validation: { required: false, min: 0} },
        QtaPrevista: { editable: true, type: "number", validation: { required: false, min: 0 } },
        ResaPrevista: { editable: true, type: "number", validation: { required: false, min: 0 } },
    };
    
    colonneKendoGrid = [
        { field: "veg_des", title: "Specie Vegetale", editor: Specie_DropDownEditor },
        { field: "Mat_Des_Esteso", title: "Prodotto", editor: Prodotto_DropDownEditor },
        { field: "Valore9", title: "Maggiorazione"},
        { field: "Superficie", title: "Superficie [Ha]"},
        { field: "QtaPrevista", title: "Qt.a Prevista [Kg]"},
        { field: "ResaPrevista", title: "Resa Contrattata Prevista [Kg/Ha]"}
    ]
   
    
    var parametriPerLettura = [];
    var parametriDataSource = {};


    var parametriKendoGrid = {
        selectable: "row"
        //columnMenu: false,
        // colonneCustomKendoGrid: colCustKendoGrid
    }; // { salvaRipristinaPersonalizzazioni: { url: pathCoreWS }};


    funzioniPrimaDopoEventi = {};
   


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










function ConfiguraGrigliaClausole(IDControllo, Key_Padre) {

    var omettiAnnulla = true;

    var funzioneSubmitDaUsare = null;
    //if (getKendoSwitch("cbDettDistinta")) {
    //    funzioneSubmitDaUsare = { funzione: null, flagInsert: false, flagUpdate: false, flagDelete: false };
    //}
    //else {
    funzioneSubmitDaUsare = { funzione: SubmitGridClausole, flagInsert: true, flagUpdate: false, flagDelete: true };
    //}
    var funzioniCRUD = {
        funzioneRead: RicercaClausole,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false
    };

    var idModel = "Clausola_Cod";
    var campiKendoModel = null;

    campiKendoModel = {
        Clausola_Cod: { editable: false, type: "number" },
        Clausola_Nome_Esteso: { editable: true, type: "string", validation: { required: true } },
        Data: { editable: true, type: "date", validation: { required: true } }
    };

    colonneKendoGrid = [
        { field: "Clausola_Nome_Esteso", title: "Clasuola Contrattuale", editor: Clausola_DropDownEditor },
        { field: "Data", title: "Data", format: "{0:dd/MM/yyyy}" } 
    ]


    var parametriPerLettura = [];
    var parametriDataSource = {};



    //var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoDelete: HideTabDettagli };
    //if (getKendoSwitch("cbDettDistinta")) {
    //funzioniPrimaDopoEventi = { funzioneDaChiamareDopoChange: rigaSplitSelezionata, funzioneDaChiamareDopoDataBound: CheckDistinteChiuse};
    //}

    var parametriKendoGrid = {
        selectable: "row"
        //columnMenu: false,
        // colonneCustomKendoGrid: colCustKendoGrid
    }; // { salvaRipristinaPersonalizzazioni: { url: pathCoreWS }};


    funzioniPrimaDopoEventi = {};
    //funzioniPrimaDopoEventi = { funzioneDaChiamareDopoChange: rigaSplitSelezionata };

    //}


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





function Clausola_DropDownEditor(container, options) {

    creaDropDownEditor(container, "Clausola_Nome", "Clausola_Cod", Elenco_Clausola, changeClausola);

}

function changeClausola(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_griglia_clausole").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Clausola_Cod = dataItem.Clausola_Cod;
    model.Clausola_Nome_Esteso = dataItem.Clausola_Nome_Esteso;


    kendoFastRedrawRow(grid, row);
}





function Specie_DropDownEditor(container, options) {

    creaDropDownEditor(container, "veg_des", "veg_cod", Elenco_Specie, changeSpecie);

}

function changeSpecie(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_griglia_fasi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.veg_cod = dataItem.veg_cod;
    model.veg_des = dataItem.veg_des;
    model.mat_cod = 0;
    model.Mat_Des_Esteso = "";


    kendoFastRedrawRow(grid, row);
}




function Prodotto_DropDownEditor(container, options) {

    var veg_cod = 0;
    if (options.model.veg_cod != 0 && options.model.veg_cod != undefined) {
        veg_cod = options.model.veg_cod;
    }
    
    var Elenco_Prodotto = Elenco_Prodotto_Riempi(true, veg_cod)
    creaDropDownEditor(container, "Mat_Des_Esteso", "Mat_Cod", Elenco_Prodotto, changeProdotto);

}

function changeProdotto(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_griglia_fasi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Mat_Cod = dataItem.Mat_Cod;
    model.Mat_Des_Esteso = dataItem.Mat_Des_Esteso;
    model.veg_cod = dataItem.Veg_Cod;
    model.veg_des = dataItem.Veg_Des;


    kendoFastRedrawRow(grid, row);
}




function RiempicmbCausale(options) {
    options.success(Elenco_Causale);
}


function RiempicmbConferente(options) {
    options.success(Elenco_Conferente);
}

function RiempicmbCentri(options) {
    options.success(Elenco_Centri);
}

function RiempicmbFabbricati(options) {
    options.success(Elenco_Fabbricati);
}


function RiempicmbListino(options) {
    options.success(Elenco_Listino);
}



function AggiornaDati(flagEsci) {

    var messErrore = "";

    //Controllo Impostazione Data Stipulazione  
    var date = kendo.parseExactDate($('input[name$="txt_Data"]').val(), "dd/MM/yyyy");
    if (date == null)
        messErrore = "Data stipulazione non corretta. <br/>";

    //Controllo Impostazione Anno  
    date = kendo.parseExactDate("01/01/" + $('input[name$="txt_anno"]').val(), "dd/MM/yyyy");
    if (date == null)
        messErrore = "Anno non corretto. <br/>";

    //Controllo Impostazione Numero
    if ($("#TxtNumero").val() == undefined || $("#TxtNumero").val() == "" )
        messErrore = "Numero non corretto. <br/>";



    //Controllo Impostazione Causale    
    var cau_contratto = "";
    if (KendoDDL("cmbCausale") !== undefined) {
        if (KendoDDL("cmbCausale").dataItem().Cau_Contratto !== undefined) {
            cau_contratto = KendoDDL("cmbCausale").dataItem().Cau_Contratto;
        }
    }
    if (cau_contratto == "")
        messErrore = "Causale non corretta. <br/>";

    //Controllo Impostazione Conferente    
    var cod_risum = 0;
    if (KendoDDL("cmbConferente") !== undefined) {
        if (KendoDDL("cmbConferente").dataItem().Cod_RisUm !== undefined) {
            cod_risum = KendoDDL("cmbConferente").dataItem().Cod_RisUm;
        }
    }
    if (cod_risum == 0)
        messErrore = "Conferente non impostato. <br/>";




    //---------------------------------------------------------------------------------------
    if (messErrore !== "")
        MessaggioErrore_Bootstrap(messErrore, "DIV_Messaggi");
    else {

        righeInseriteGrid_Fasi = "";
        righeInseriteGrid_Clausole = "";                      

        bDatiNecessariInseriti = true;

        var griglia_fasi = $("#tab_griglia_fasi").data("kendoGrid");
        var griglia_clausole = $("#tab_griglia_clausole").data("kendoGrid");


        griglia_fasi.saveChanges();
        griglia_clausole.saveChanges();

        if (bDatiNecessariInseriti == true) {

            // Aggiornamento effettivo
            AggiornaEffettivo(flagEsci);

        }

     
    }
}




function SubmitGridFasi(options) {

    let errMess = "";
    let found = false;
    
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var grid = $("#tab_griglia_fasi").data("kendoGrid");

    var currentData = grid.dataSource.data();
    
        for (var i = 0; i < currentData.length; i++) {            

            errMess = controllaRigheValidePerSubmitGrid_Fasi(options.data.created, "");
            errMess = controllaRigheValidePerSubmitGrid_Fasi(options.data.updated, errMess);

        }

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");

    }
    else {

        currentData = grid.dataSource.data();

        for (var i = 0; i < currentData.length; i++) {

            if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
            //} else if (currentData[i].dirty) {
            } else {
                updatedRecords.push(currentData[i].toJSON());
            }
        }

        for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
            deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
        }

        if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

            // Variabili globali
            righeInseriteGrid_Fasi = kendoEscapeOggetto(newRecords);
            righeModificateGrid_Fasi = kendoEscapeOggetto(updatedRecords);
            righeCancellateGrid_Fasi = kendoEscapeOggetto(deletedRecords);

        }

    }
    
}




function SubmitGridClausole(options) {

    let errMess = "";
    let found = false;
    
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var grid = $("#tab_griglia_clausole").data("kendoGrid");

    var currentData = grid.dataSource.data();


    for (var i = 0; i < currentData.length; i++) {

        errMess = controllaRigheValidePerSubmitGrid_Clausole(options.data.created, "");
        errMess = controllaRigheValidePerSubmitGrid_Clausole(options.data.updated, errMess);

    }

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");

    }
    else {
        
        currentData = grid.dataSource.data();

        for (let i = 0; i < currentData.length; i++) {
            newRecords.push(currentData[i].toJSON());
        }

        // Variabili globali
        righeInseriteGrid_Clausole = kendoEscapeOggetto(newRecords);

    }
}


function cmbConferente_change(options) {

}

function cmbCentri_change(options) {
    Elenco_Fabbricati = Elenco_Fabbricati_Riempi(true);

    var ddl = KendoDDL("cmbFabbricati");
    ddl.dataSource.read();
    ddl.refresh();
}

function cmbFabbricati_change(options) {

}


function cmbListino_change(options) {

}


function cmbCausale_change(options) {

}