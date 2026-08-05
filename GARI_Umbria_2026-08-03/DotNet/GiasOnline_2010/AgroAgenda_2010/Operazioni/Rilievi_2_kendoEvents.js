var indirizzohttp = "Rilievi_2.aspx";

function letturaTabella_Kendo() {
    WaitFrame.show();

    var param = "{ param: '' }";

    ajaxAgronica(indirizzohttp + "/CaricaTabella_Kendo",
        param,
        function (risposta) {
            if (risposta.RispostaOK) {
                $("#" + hdRilievi_clientID).val(risposta.RispostaStringa);
                popolaGriglia("divRilievi");
                WaitFrame.hide();
            }
            else {
                WaitFrame.hide();
                alert(risposta.Errore);
            }
        }, null);
}


//-----------------------------------------------------------------------------------------------------------------------------------
//KENDO
//-----------------------------------------------------------------------------------------------------------------------------------

function popolaGriglia(IDControllo) {

    var funzioniCRUD = { funzioneRead: kReadValorizzazione_rows, funzioneInsert: null, funzioneUpdate: null, funzioneDelete: null };
    var idModel = "Codice";
    var campiKendoModel = kReadValorizzazione_mod();
    var colonneKendoGrid = kReadValorizzazione_col();
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        columnMenu: false,
        sortable: false,
        pdf: false,
        excel: false,
        groupable: false,
        filterable: false,
        pageable: false
    };
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoSave: onSaveDataFF, funzioneDaChiamareDopoDataBound: onDataBoundRighe };
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

function kReadValorizzazione_rows(options) {

    var data = $('#' + hdRilievi_clientID).val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazione_col() {

    var data = $('#' + hdRilievi_clientID).val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadValorizzazione_mod() {

    var data = $('#' + hdRilievi_clientID).val();
    jSonParsed_Kendo = JSON.parse(data);

    //console.log(data);
    return jSonParsed_Kendo.kendo_model;
}

function AggiornaRiga() {



}

function onSaveDataFF(e) {

    if (e.values.Default !== undefined) {
        var nuovaData = e.values.Default;

        //copio la data default su tutta la riga
        var grid = $("#divRilievi").data("kendoGrid");
        for (var i = 0; i < grid.columns.length; i++) {
            if (i >= 2) {
                e.model["App_" + (i - 1)] = nuovaData;
            }
        }

        grid.refresh();
    }
    //var colIndex = kendo_indiceColonna_DatoNomeCampo("#divRilievi", Object.keys(e.values)[0]);

    //var rows = e.sender.tbody.children();

    //var DateValorizzate = new Array();


    //for (var i = 0; i < rows.length; i++) {

    //    var row = $(rows[i]);

    //    var dataCorrente = e.sender.dataItem(row)[colIndex];

    //    if (dataCorrente != undefined) {

    //        DateValorizzate.push(dataItem);
    //        //var dataSuccessiva = e.sender.dataItem($(rows[i + 1]))[colIndex];

    //        //if (dataSuccessiva != '') {

    //        //    if (dataCorrente > dataSuccessiva) {
    //        //        alert('errore');
    //        //        return;
    //        //    }

    //       }

    //    }

    }



function onDataBoundRighe(e) {    

    var rows = e.sender.tbody.children();

    for (var i = 0; i < rows.length; i++) {

        var row = $(rows[i]);
        var dataItem = e.sender.dataItem(row);

        var Fioritura = dataItem.get("Fioritura");

        if (Fioritura == '1') {
            row.addClass("Fioritura");
        }

    }
    
}