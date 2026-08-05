//import { setTimeout } from "timers";

function CaricaGrigliaZoo(deferred) {

    //Leggo da db solo se non ho mai letto
    if ($("#divKendoZoo").html() === '') {
        var data_inizio = $(data_inizio_ClientID).val();
        var data_fine = $(data_fine_ClientID).val();
        var sa_cod = $('#ddlCentri').val();
        //var veg_cod = $('#ddlSpecie').val();

        KendoZoo_leggi(undefined, data_inizio, data_fine, deferred, sa_cod);
    }

}

function KendoZoo_inizializza(divKendoZoo) {

    var funzioniCRUD = {
        funzioneRead: kZooReadValorizzazione_rows,
        checkBoxFunction: KendoOperazioniZoo_checked
    };

    var idModel = "id_agenda";

    var campiKendoModel = kZooReadValorizzazione_mod();
    var colonneKendoGrid = kZooReadValorizzazione_col();

    var pulsanteCopiaZoo = "<div class='btn btn-warning btnCopia' style='display:block;width:70px;border:0px;margin-bottom:0px;' onclick=duplicaZoo(this.closest('tr'),this.closest('.k-grid'), " + enumZoo + ")>Copia</div>";
    if (Request_QueryString("gis") === "true") {
        pulsanteCopiaZoo = "";
    }

    var jsLblInfo = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblInfo", "Info");
    var jsLblModifica = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblModifica", "Modifica");
    var jsLblCancella = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblCancella", "Cancella");
    var jsLblGeneraModello4 = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblGeneraModello4", "Genera Modello 4");
    var jsLblRegistraUscita = TraduzioneMultiResx(MenuBS_Agenda_NuovoResx, "jsLblRegistraUscita", "Registra Uscita");

    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };
    var parametriKendoGrid = {
        columnMenu: true,
        impostaColonneKendoGridDaCookie: false,
        toolbarCommands: ["templateLegendaMenuAgendaOperazioniTutte"],
        excel: true,
        pdf: false,
        sortable: true,
        groupable: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        scrollable: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: { mode: "row" },
        selectable: "row",
        checkSelezioneRiga: { filterable: false, field: null, width: "35px" },
        colonneCustomKendoGrid: [
            {
                command: {
                    template: "<div class='btn btn-info btnInfo' style='display:block;width:125px;border:0px;' onclick=infoZoo(this.closest('tr'),this.closest('.k-grid'))>" + jsLblInfo + "</div>" +
                        "<div class='btn btn-success btnModifica' style='display:block;width:125px;border:0px;' onclick=modificaZoo(this.closest('tr'),this.closest('.k-grid'))>" + jsLblModifica + "</div>" +
                        "<div class='btn btn-danger btnCancella' style='display:block;width:125px;border:0px;' onclick=eliminaZoo(this.closest('tr'),this.closest('.k-grid'))>" + jsLblCancella + "</div>" +
                        "<div class='btn btn-info btnModello4' style='display:block;width:125px;border:0px;' onclick=inviaModello4(this.closest('tr'),this.closest('.k-grid'))>" + jsLblGeneraModello4 + "</div>" +
                        "<div class='btn btn-info btnRegModello4' style='display:block;width:125px;border:0px;' onclick=registraModello4(this.closest('tr'),this.closest('.k-grid'))>" + jsLblRegistraUscita + "</div>" +
                        pulsanteCopiaZoo
                }, title: "Azioni", width: "150px"
            }
        ]
    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: kendo_Zoo_onDataBoundedRighe
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    var KendoZoo = creaKendoGrid(divKendoZoo, // rappresenta l'ID del div a cui si associa la griglia
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

    CreaToolBarZoo();

    var griglia = $('#' + divKendoZoo).data("kendoGrid");
    griglia.autoFitColumn("Data2");
}

function kendo_Zoo_onDataBoundedRighe(e) {
    coloraRigheOperazioni(e);
    nascondiPulsantiOperazioniAgenda(e);
    mostraColonnaUnicaSeInMobile(e, 2);
    riduciAltezzaRighe(e, 2);
}

function kZooReadValorizzazione_rows(options) {

    var data = $('#hdKendoZoo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    options.success(jSonParsed_Kendo.kendo_rows);
}

function kZooReadValorizzazione_col() {

    var data = $('#hdKendoZoo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_columns;
}

function kZooReadValorizzazione_mod() {

    var data = $('#hdKendoZoo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function infoZoo(tr_elem, grid_elem) {
    infoElemento(tr_elem, grid_elem, enumZoo)
}

function modificaZoo(tr_elem, grid_elem) {
    modificaElemento(tr_elem, grid_elem, enumZoo)
}

function eliminaZoo(tr_elem, grid_elem) {
    eliminaElemento(tr_elem, grid_elem, enumZoo)
}

function duplicaZoo(tr_elem, grid_elem) {
    duplicaElemento(tr_elem, grid_elem, enumZoo)    
}

function CreaToolBarZoo(){
    var gridTB = $("#divKendoZoo").find(".k-grid-toolbar");

    if (permessoModello4) {
        gridTB.append();

        $("#inviaModello4").click(function (a, b) {
            inviaModello4(a, b);
        });
    }
}

function KendoOperazioniZoo_checked(e) {

    let checked = this.checked;
    let row = $(this).parents("tr");
    let grid = $("#divKendoZoo").data("kendoGrid");
    let dataItem = grid.dataItem(row);

    dataItem.Selected = checked;
    dataItem.dirty = true;

    rowKendoGridSelected(row, checked)
}