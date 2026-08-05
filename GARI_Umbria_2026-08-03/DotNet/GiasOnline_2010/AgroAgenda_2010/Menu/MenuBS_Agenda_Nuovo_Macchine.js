
function CaricaGrigliaMacchine(deferred) {

    //Leggo da db solo se non ho mai letto
    if ($("#divKendoMacchine").html() === '') {
        var data_inizio = $(data_inizio_ClientID).val();
        var data_fine = $(data_fine_ClientID).val();
        //var sa_cod = $('#ddlCentri').val();
        //var veg_cod = $('#ddlSpecie').val();

        KendoMacchine_leggi(undefined, data_inizio, data_fine, deferred);
    }

}

function KendoMacchine_inizializza(divKendoMacchine) {

    var funzioniCRUD = {
        funzioneRead: kMacchineReadValorizzazione_rows,
        checkBoxFunction: KendoMacchine_checked
    };

    var idModel = "id_agenda";

    var campiKendoModel = kMacchineReadValorizzazione_mod();
    var colonneKendoGrid = kMacchineReadValorizzazione_col();

    var pulsanteCopiaMacchine = "<div class='btn btn-warning btnCopia' style='display:block;width:70px;border:0px;margin-bottom:0px;' onclick=duplicaElemento(this.closest('tr'),this.closest('.k-grid'))>Copia</div>";
    if (Request_QueryString("gis") === "true") {
        pulsanteCopiaMacchine = "";
    }

    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };
    var parametriKendoGrid = {
        columnMenu: true,
        impostaColonneKendoGridDaCookie: false,
        toolbarCommands: ["templateLegendaMenuAgendaOperazioniTutte"],
        excel: false,
        pdf: false,
        sortable: true,
        groupable: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        scrollable: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: { mode: "row" },
        checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        colonneCustomKendoGrid: [
            {
                command: {
                    template: "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoElemento(this.closest('tr'),this.closest('.k-grid'))>Info</div>" +
                        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaElemento(this.closest('tr'),this.closest('.k-grid'))>Modifica</div>" +
                        "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaElemento(this.closest('tr'),this.closest('.k-grid'))>Cancella</div>" +
                        pulsanteCopiaMacchine
                }, title: "Azioni", width: "97px"
            }
        ]

    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: kendo_Operazioni_onDataBoundedRighe
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    var KendoMacchine = creaKendoGrid(divKendoMacchine, // rappresenta l'ID del div a cui si associa la griglia
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

    var griglia = $('#' + divKendoMacchine).data("kendoGrid");
    griglia.autoFitColumn("Data2");

}

function kMacchineReadValorizzazione_rows(options) {

    var data = $('#hdKendoMacchine_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    options.success(jSonParsed_Kendo.kendo_rows);
}

function kMacchineReadValorizzazione_col() {

    var data = $('#hdKendoMacchine_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_columns;
}

function kMacchineReadValorizzazione_mod() {

    var data = $('#hdKendoMacchine_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function KendoMacchine_checked(e) {

    let checked = this.checked;
    let row = $(this).parents("tr");
    let grid = $("#divKendoMacchine").data("kendoGrid");
    let dataItem = grid.dataItem(row);

    dataItem.Selected = checked;
    dataItem.dirty = true;

    rowKendoGridSelected(row, checked)
}