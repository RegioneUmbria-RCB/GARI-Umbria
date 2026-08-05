
//MenuBS_Anagrafica_Meteo.js

function caricaGrigliaStazioniMeteo(keys) {

    var objAgenda = JSON.parse(objP_agenda);
    var parametri = { piva: objAgenda.Piva, sa_cod: 0 };


    ajaxAgronica(indirizzohttp + "/CaricaStazioniMeteo", JSON.stringify(parametri), function (risposta) {
        $('#hdKendoStazioniMeteo_Valorizzazione').val(risposta.RispostaStringa);
        kendoMeteo_inizializza("divKendoStazioniMeteo", keys);
    }, null)

}

function DoPostBack_ControlliSiNo(key) {
    ajaxAgronica(indirizzohttp + "/EliminaStazioniMeteo", JSON.stringify({ ChiaveStazione: key }),
            function (risposta) {
                kendo.alert(risposta.RispostaStringa);
                let tab = ImpostaSelezione(13, 1);
                tabStrip.select(tab);
            }, null);
}

function eliminaAnagMeteo(tr_elem, grid_elem) {

    

    let datiGriglia = $(grid_elem).data('kendoGrid');
    let datiRiga = datiGriglia.dataItem(tr_elem);

    var chiave = datiRiga.chiave;
    
    let msg = Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_eliminaAnagMeteo", "Confermi l'eliminazione dell'associazione di questa sorgente dati?");

    ConfermaControlliSiNo(msg, chiave);
    
}



function kReadValorizzazioneMeteo_rows(options) {

    var data = $('#hdKendoStazioniMeteo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneMeteo_mod() {

    var data = $('#hdKendoStazioniMeteo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function kReadValorizzazioneMeteo_col() {
    var data = $('#hdKendoStazioniMeteo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);

    //kendo_Colonne_estendi(jSonParsed_Kendo, "TipoSorgente_Cod", "TipoSorgente_Des", 1, "TipoSorgente_Des", SorgenteDati_Template);

    return jSonParsed_Kendo.kendo_columns;
}

function RicaricaTipoSorgente() {

    return [
        { "TipoSorgente_Cod": "0", "TipoSorgente_Des": Traduzione(menuBSAnagraficaResx, "SorgenteMeteoStazioniER", "Gias (Stazioni Regione E.R.)") },
        { "TipoSorgente_Cod": "3", "TipoSorgente_Des": Traduzione(menuBSAnagraficaResx, "SorgenteMeteoQuadrantiER", "Gias (Quadranti Regione E.R.)") },
        { "TipoSorgente_Cod": "1", "TipoSorgente_Des": Traduzione(menuBSAnagraficaResx, "SorgenteMeteoTutteLeStazioni", "Tutte le stazioni in visibilità") },
        { "TipoSorgente_Cod": "2", "TipoSorgente_Des": Traduzione(menuBSAnagraficaResx, "SorgenteMeteoStazioniAziendali", "Solo le stazioni aziendali") }
    ];
}


/*
 * Template kendo
 */

//#Region template per colonne con combo
function SorgenteDati_Template(container, options) {

    $('<input required data-text-field="TipoSorgente_Des" data-value-field="TipoSorgente_Cod" data-bind="value:' + options.field + '"/>')
    .appendTo(container)
    .kendoDropDownList({
        autoBind: true,
        filter: false,
        dataSource: RicaricaTipoSorgente()        
    });

}

function kendoMeteo_inizializza(divKendo, keys) {

    var funzioniCRUD = { funzioneRead: kReadValorizzazioneMeteo_rows };

    var idModel = "chiave";

    var campiKendoModel = kReadValorizzazioneMeteo_mod();
    var colonneKendoGrid = kReadValorizzazioneMeteo_col();

    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };

    var templateCommands = "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaAnagMeteo(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + "</div>";
    var widthAzioni = "97px";

    if (GiasVersioneMaster === "2022") {
        templateCommands = '<button type="button" class="btn-Cancella k-grid-Cancella k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=eliminaAnagMeteo(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>';
        widthAzioni = "90px";
    }

    var parametriKendoGrid = {
        columnMenu: true,
        impostaColonneKendoGridDaCookie: false,
        excel: true,
        pdf: false,
        sortable: true,
        groupable: false,
        reorderable: true,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        scrollable: true,
        selectable: "row",
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: true,
        btnEliminaTuttiFiltri: false,
        //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        colonneCustomKendoGrid: [
            {
                command: {
                    template: templateCommands
                }, title: Traduzione(menuBSAnagraficaResx, "Azioni", "Azioni"), width: widthAzioni
            }
        ]

    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: function (e) {
            //Se in mobile mostro solo la colonna unica
            mostraColonnaUnicaSeInMobile(e, 1);

            //Accorcio l'altezza delle righe
            riduciAltezzaRighe(e, 1);

            var arrKeys = new Array();
            var objParametri_Agenda = JSON.parse(objP_agenda);
            arrKeys.push(objParametri_Agenda.Piva + '_' + objParametri_Agenda.Sa_Cod + '_' + objParametri_Agenda.Fabbricato)
            var grid = $("#" + divKendo).data("kendoGrid");
            var data = grid.dataSource.data();
            for (var i = 0; i < arrKeys.length; i++) {
                for (var j = 0; j < data.length; j++) {
                    if (data[j].chiave == arrKeys[i]) {
                        var rowUid = data[j].uid;
                        var row = grid.table.find("[data-uid=" + rowUid + "]");
                        grid.select(row);
                    }
                }
            }            
            //NascondiBottoni(grid, e);
        },
        funzioneDaChiamareDopoChange: function (e) {
            var grid = $("#" + divKendo).data("kendoGrid");
            var selectedRow = grid.select();
            var dataItem = grid.dataItem(selectedRow);
            //ImpostaObjP_Agenda(7, dataItem.chiave, false, function () {                
            //});
        }
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    KendoOperazioni = creaKendoGrid(divKendo, // rappresenta l'ID del div a cui si associa la griglia
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