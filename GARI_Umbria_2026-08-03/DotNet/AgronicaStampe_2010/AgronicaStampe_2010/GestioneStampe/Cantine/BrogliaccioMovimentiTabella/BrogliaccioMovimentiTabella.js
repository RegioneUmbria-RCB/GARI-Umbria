function AggiornaKendo(){
    var dataDa=$('#Txt_DataInizio').val();
    var dataA = $('#Txt_DataFine').val();
    var txtMese = $('#Txt_Mese').val();
    var mese = 0;
    var anno = 0;
    var chiamaFunzione = false;
    switch ($('input[name=finestraTemporale]:checked').val()) {
        case "0":
            dataDa = "";
            dataA = "";
            var meseOk = false
            if (txtMese != '') {
                mese = txtMese.split("/")[0]
                anno = txtMese.split("/")[1]
                if ((mese > 0 && mese < 13) && (anno > 1900 && anno < 2100)) {
                    meseOk = true;
                }
            } else {
                alert("E' stata selezionata la stampa per mese: selezionare il mese desiderato!")
            }
            if (meseOk) {
                chiamaFunzione = true;
            }
            break;
        case "1":
            if (dataDa == "" || dataA == "") {
                alert('Impostare Data Inizio e Data Fine')
                chiamaFunzione = false;
            } else {
                chiamaFunzione = true;
            }
            break;
    }
    if (chiamaFunzione) {
        var qs_piva = getParameterByName("p")
        CaricaKendo(qs_piva, dataDa, dataA, mese, anno, function (r) {
            if (r.RispostaOK == true) {
                var dd = jQuery.parseJSON(r.RispostaStringa);
                jSonParsed_Kendo_Brogliaccio = dd;
                GrigliaKendoBrogliaccio('kTable');
                var gridId = 'kTable';
                var grid = $("#" + gridId).data("kendoGrid");
                for (var i = 0; i < grid.columns.length; i++) {
                    grid.autoFitColumn(i);
                }
            }
        });
    }
}

function GrigliaKendoBrogliaccio(div) {

    var funzioniCRUD = {
        funzioneRead: kendo_Brogliaccio_Leggi
    };
    var idModel = "Id_Agenda";
    var campiKendoModel = kReadBrogliaccio_mod(); //kendo_model
    var colonneKendoGrid = kReadBrogliaccio_col(); //kendo_columns
    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        columnMenu: false,
        impostaColonneKendoGridDaCookie: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        toolbarCommands: [],
        excel: true,
        pdf: true,
        sortable: true,
        groupable: true,
        pageable: true,
        reorderable: true,
        filterable: {mode: "row"},
        colonneCustomKendoGrid: []
    };
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBindingRigheBrogliaccio };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];
    try {

        creaKendoGrid(div, // rappresenta l'ID del div a cui si associa la griglia
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

    } catch (e) {

    console.log(e);
    
    }
}

function kendo_Brogliaccio_Leggi(options) {
    options.success(jSonParsed_Kendo_Brogliaccio.kendo_rows);
}

function kReadBrogliaccio_mod() {
    return jSonParsed_Kendo_Brogliaccio.kendo_model;
}

function kReadBrogliaccio_col() {
    var columns = jSonParsed_Kendo_Brogliaccio.kendo_columns;
    return columns;
}

function onDataBindingRigheBrogliaccio(e) {
//    if ($('#kTable > .k-grid-content').height() > 400) {
//        $('#kTable > .k-grid-content').height('450px');
//    }
    //var gridId = e.sender.element[0].id;
//    var grid = $("#" + gridId).data("kendoGrid");
//    for (var i = 0; i < grid.columns.length; i++) {
//        grid.autoFitColumn(i);
//    }
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}