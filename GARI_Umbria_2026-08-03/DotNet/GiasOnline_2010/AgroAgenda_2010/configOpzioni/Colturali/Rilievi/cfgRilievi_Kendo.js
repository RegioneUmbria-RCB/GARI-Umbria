

var indirizzohttp = "CfgRilievi_WS.aspx";




function inizializzazionePagina() {
    
    LetturaDatiCfgAnagraficheVarie();
}


function LetturaDatiCfgAnagraficheVarie() {

    var param = "{ }";

    ajaxAgronica(indirizzohttp + "/LetturaDatiCfgAnagraficheVarie",
        param,
        function (risposta) {

            $("#hdAnagrafiche").val(risposta.RispostaStringa);
            letturaDatiCfgRilieviKendo();
        }, null);

}

function letturaDatiCfgRilieviKendo() {

    var param = "{ }";

    ajaxAgronica(indirizzohttp + "/letturaDatiCfgRilieviKendo",
        param,
        function (risposta) {

            $("#hdCfgRilievi").val(risposta.RispostaStringa);
            CfgRilieviKendo("divCfgRilievi");

        }, null);
}


function onDataBindingRighe() {

}

function kReadCfgRilievi_rows(options) {

    var data = $('#hdCfgRilievi').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadCfgRilievi_col() {

    var data = $('#hdCfgRilievi').val();
    jSonParsed_Kendo = JSON.parse(data);

    kendo_Colonne_estendi(jSonParsed_Kendo, "Veg_Cod", "Specie", 1, "Veg_Des", MisureSpecie_Template);
    kendo_Colonne_estendi(jSonParsed_Kendo, "Av_Cod", "Avversità", 2, "Av_Des_Vol", MisureAvversita_Template);
    kendo_Colonne_estendi(jSonParsed_Kendo, "Udm_Cod", "Unità di Misura", 3, "Udm_Des", MisureUdm_Template);
    kendo_Colonne_estendi(jSonParsed_Kendo, "FF_Cod", "Fase Fenologica", 4, "FF_Des", MisureFF_Template);

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadCfgRilievi_mod() {

    var data = $('#hdCfgRilievi').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}


/**
 * Aggiornamento dei dati sul server
 */
function kReadCfgRilievi_dati() {

    //DEBUG_GABRIELE
    // effettuate modifiche dopo l'implementazione della funzioneCRUD -> funzioneSubmit

    var grid = $("#divCfgRilievi").data("kendoGrid");
    var currentData = grid.dataSource.data();

    //effettuare verifiche lato client alla ricerca di errori.. e valorizzare foundErr di conseguenza

    // Non ci sono errori, procedo con aggiornamenti

    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    for (var i = 0; i < currentData.length; i++) {

        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }

    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }


    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        var righeInserite = JSON.stringify(newRecords).replace(/'/g, "\\'");;
        var righeModificate = JSON.stringify(updatedRecords).replace(/'/g, "\\'");;
        var righeCancellate = JSON.stringify(deletedRecords).replace(/'/g, "\\'");;
        var allOk = false;
        var param = "{ righeInserite: '" + righeInserite + "', righeModificate: '" + righeModificate + "', righeCancellate: '" + righeCancellate + "' }"

        ajaxAgronicaSync(indirizzohttp + "/kReadCfgRilievi_dati", param, false,
                    function (risposta) {
                        allOk = true;
                        grid.dataSource._destroyed = [];

                        var pj = JSON.parse($("#hdCfgRilievi").val());
                        pj.kendo_rows = currentData;
                        $("#hdCfgRilievi").val(JSON.stringify(pj));

                        grid.dataSource.read();
                        grid.refresh();
                        MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
                    }, null);

        if (!allOk) {

            if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
                var dsSort = [];
                //dsSort.push({ field: "Tabella_Des", dir: "asc" });
                //dsSort.push({ field: "val_des", dir: "asc" });

                // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
                // che non si vedono più
                ripristinaRigheCancellateKendoGrid(grid, dsSort);
            }
        }
    }
}

function CfgRilieviKendo(divCfgRilievi) {

    var funzioniCRUD = {
        funzioneRead: kReadCfgRilievi_rows,
        funzioneSubmit: { funzione: kReadCfgRilievi_dati }
    };
    var idModel = "COD";
    var campiKendoModel = kReadCfgRilievi_mod();
    var colonneKendoGrid = kReadCfgRilievi_col();
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        pagesize: 20,
        groupable: false,
        scrollable: false,
        sortable: true,
        resizable: false,
        filterable: true,
        pageable: true,
        pdf: false,
        excel: false,
        colonneCustomKendoGrid: [
            {
                command: {
                    template: "<div class='btn btn-info' style='display:block;width:150px;border:0px;' onclick=valoriSelezioneAttiva(this.closest('tr'),this.closest('.k-grid'))>Valori per Selezione</div>"
                }, title: "Azioni", width: "150px"//, widthfisso: true //width: "135px" - no style in span
            }
        ]

    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamarePrimaDelDataBinding: onDataBindingRighe };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["COD"];

    creaKendoGrid(divCfgRilievi, // rappresenta l'ID del div a cui si associa la griglia
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


/*------------------------*/

/**
 * Aggiornamento dei dati sul server
 */
function cfgRilieviAnagAggiornaDatiSrv() {

    //DEBUG_GABRIELE
    // effettuate modifiche dopo l'implementazione della funzioneCRUD -> funzioneSubmit

    var grid = $("#divcfgRilieviAnag").data("kendoGrid");
    var currentData = grid.dataSource.data();

    //effettuare verifiche lato client alla ricerca di errori.. e valorizzare foundErr di conseguenza

    for (var i = 0; i < currentData.length; i++) {
        currentData[i].kendoKey = $("#hdCurrentCOD").val() + '-' + currentData[i].Anag_Valore;
    }

    // Non ci sono errori, procedo con aggiornamenti

    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    for (var i = 0; i < currentData.length; i++) {

        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        var righeInserite = JSON.stringify(newRecords).replace(/'/g, "\\'");;
        var righeModificate = JSON.stringify(updatedRecords).replace(/'/g, "\\'");;
        var righeCancellate = JSON.stringify(deletedRecords).replace(/'/g, "\\'");;
        var allOk = false;

        var param = "{ righeInserite: '" + righeInserite + "', righeModificate: '" + righeModificate + "', righeCancellate: '" + righeCancellate + "' }"

        ajaxAgronicaSync(indirizzohttp + "/cfgRilieviAnagAggiornaDatiSrv", param, false,
            function (risposta) {
                allOk = true;
                grid.dataSource._destroyed = [];

                var pj = JSON.parse($("#hdcfgRilieviAnag").val());
                pj.kendo_rows = currentData;
                $("#hdcfgRilieviAnag").val(JSON.stringify(pj));

                grid.dataSource.read();
                grid.refresh();
                MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
            }, null);

        if (!allOk) {
            if (grid.dataSource._destroyed != null && grid.dataSource._destroyed.length > 0) {
                var dsSort = [];
                //dsSort.push({ field: "Tabella_Des", dir: "asc" });
                //dsSort.push({ field: "val_des", dir: "asc" });

                // chiamo la funzione in funzioniComuniKendoGrid che permette di mostrare le righe cancellate 
                // che non si vedono più
                ripristinaRigheCancellateKendoGrid(grid, dsSort);
            }
        }
    }
}

function letturaDaticfgRilieviAnagKendo(cod) {

    var param = "{ cod:'" + cod + "' }";

    ajaxAgronica(indirizzohttp + "/letturaDaticfgRilieviAnagKendo",
        param,
        function (risposta) {

            $("#hdcfgRilieviAnag").val(risposta.RispostaStringa);
            cfgRilieviAnagKendo("divcfgRilieviAnag");

        }, null);
}

function kReadcfgRilieviAnag_rows(options) {
    
    var data = $('#hdcfgRilieviAnag').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
       

}

function kReadcfgRilieviAnag_col() {

    var data = $('#hdcfgRilieviAnag').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadcfgRilieviAnag_mod() {

    var data = $('#hdcfgRilieviAnag').val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}

function cfgRilieviAnagKendo(divcfgRilieviAnag) {

    var funzioniCRUD = {
        funzioneRead: kReadcfgRilieviAnag_rows,
        funzioneSubmit: { funzione: cfgRilieviAnagAggiornaDatiSrv }
    };
    var idModel = "kendoKey";
    var campiKendoModel = kReadcfgRilieviAnag_mod();
    var colonneKendoGrid = kReadcfgRilieviAnag_col();
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        pagesize: 0,
        groupable: false,
        scrollable: false,
        sortable: false,
        resizable: false,
        filterable: false,
        pageable: false,
        pdf: false,
        excel: false
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamarePrimaDelDataBinding: onDataBindingRighe };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["kendoKey"];

    creaKendoGrid(divcfgRilieviAnag, // rappresenta l'ID del div a cui si associa la griglia
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