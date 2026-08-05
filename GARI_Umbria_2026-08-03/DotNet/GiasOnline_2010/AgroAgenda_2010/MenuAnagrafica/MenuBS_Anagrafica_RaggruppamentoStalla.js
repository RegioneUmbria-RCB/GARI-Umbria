function kendoRaggruppamentoStalla_onDataBoundedRighe(e) {

    //Se in mobile mostro solo la colonna unica
    //mostraColonnaUnicaSeInMobile(e, 1);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe("#divKendoAzienda", 1);

}

function kendoRaggruppamentoStalla_inizializza(divKendo, keys) {

    var funzioniCRUD = { funzioneRead: kReadValorizzazioneRaggruppamentoStalla_rows };

    var idModel = "chiave";

    var campiKendoModel = kReadValorizzazioneRaggruppamentoStalla_mod();
    var colonneKendoGrid = kReadValorizzazioneRaggruppamentoStalla_col();

    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };

    var templateCommands = "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoRaggruppamentoStalla(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaRaggruppamentoStalla(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + "</div>" +
        "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaRaggruppamentoStalla(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + "</div>";
    var widthAzioni = "97px";

    if (GiasVersioneMaster === "2022") {
        templateCommands = '<button type="button" data-title="' + Traduzione(menuBSAnagraficaResx, "Info", "Info") + '" class="btn-Info k-grid-Info k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=infoRaggruppamentoStalla(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" data-title="' + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + '" class="btn-Modifica k-grid-Modifica k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=modificaRaggruppamentoStalla(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" data-title="' + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + '" class="btn-Cancella k-grid-Cancella k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=eliminaRaggruppamentoStalla(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>';
        widthAzioni = "140px";
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
            //mostraColonnaUnicaSeInMobile(e, 1);
            autoFitSeMobile(e);

            //Accorcio l'altezza delle righe
            riduciAltezzaRighe(e, 1);
            var objParametri_Agenda = JSON.parse(objP_agenda);
            var arrKeys = new Array();
            var doSelect = false;
            arrKeys.push(objParametri_Agenda.Piva)
            var grid = $("#" + divKendo).data("kendoGrid");
            var data = grid.dataSource.data();
            for (var i = 0; i < arrKeys.length; i++) {
                for (var j = 0; j < data.length; j++) {
                    if (data[j].chiave == arrKeys[i]) {
                        var rowUid = data[j].uid;
                        var row = grid.table.find("[data-uid=" + rowUid + "]");
                        grid.select(row);
                        doSelect = true;
                    }
                }
            }
            if (doSelect == false && data.length == 1) {
                var rowUid = data[0].uid;
                var row = grid.table.find("[data-uid=" + rowUid + "]");
                grid.select(row);
                grid.trigger("change");
            }
            Dati_Relativi_Percorso_Selezione2(8);
        },
        funzioneDaChiamareDopoChange: function (e) {
            var grid = $("#" + divKendo).data("kendoGrid");
            var selectedRow = grid.select();
            var dataItem = grid.dataItem(selectedRow);
            if (dataItem != undefined) {
                ImpostaObjP_Agenda(8, dataItem.chiave, false, function () {
                    Dati_Relativi_Percorso_Selezione2(8);
                });
            }
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

function kReadValorizzazioneRaggruppamentoStalla_rows(options) {

    var data = $('#hdKendoRaggruppamentiStalle_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneRaggruppamentoStalla_col() {

    var data = $('#hdKendoRaggruppamentiStalle_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadValorizzazioneRaggruppamentoStalla_mod() {

    var data = $('#hdKendoRaggruppamentiStalle_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaRaggruppamentoStalla(keys) {

    var parametri = {}

    ajaxAgronica(indirizzohttp + "/CaricaRaggruppamentiStalla", JSON.stringify(parametri), function (risposta) {
        $('#hdKendoRaggruppamentiStalle_Valorizzazione').val(risposta.RispostaStringa);
        kendoRaggruppamentoStalla_inizializza("divKendoRaggruppamentiStalle", keys);
    }, null);

}

function infoRaggruppamentoStalla(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/InfoRaggruppamentoStalla',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Stalla_Raggruppamenti_Edit.aspx" + riportaParametroVisibilita();
        }
    });

}

function modificaRaggruppamentoStalla(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditRaggruppamentoStalla',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Stalla_Raggruppamenti_Edit.aspx" + riportaParametroVisibilita();
        }
    });
}

function eliminaRaggruppamentoStalla(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var xTipoNodo = 7;
    var chiave = datiRiga.chiave;

    var streelemento = Traduzione(menuBSAnagraficaResx, "Raggruppamento", "Raggruppamento") + ": <b>" + datiRiga.raggruppamento_Des + "</b>";

    Popup_delete(streelemento, xTipoNodo, chiave);
}
