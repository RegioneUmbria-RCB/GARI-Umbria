function kendoAzienda_onDataBoundedRighe(e) {

    //Se in mobile mostro solo la colonna unica
    mostraColonnaUnicaSeInMobile(e, 1);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe("#divKendoAzienda", 1);

}

function kendoAzienda_inizializza(divKendo) {

    var funzioniCRUD = { funzioneRead: kReadValorizzazioneAzienda_rows };

    var idModel = "chiave";

    var campiKendoModel = kReadValorizzazioneAzienda_mod();
    var colonneKendoGrid = kReadValorizzazioneAzienda_col();

    var qsVisibilita = Request_QueryString("visibilita");
    var colonneCustomKendoGrid = [];

    if (qsVisibilita != "2") {
    
        var templateCommands = "<div class='btn-group-vertical'>" +
            "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoAzienda(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
            "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaAzienda(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + "</div>" +
            "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaAzienda(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + "</div>" +
            "</div>";
        var widthAzioni = "97px";

        if (GiasVersioneMaster === "2022") {
            templateCommands = '<button type="button" class="btn-Info k-grid-Info k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=infoAzienda(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span>&nbsp;</button>' +
                '<button type="button" class="btn-Modifica k-grid-Modifica k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=modificaAzienda(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span>&nbsp;</button>' +
                '<button type="button" class="btn-Cancella k-grid-Cancella k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=eliminaAzienda(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span>&nbsp;</button>';
            widthAzioni = "140px";
        }

        colonneCustomKendoGrid = [
            {
                command: {
                    template: templateCommands
                }, title: Traduzione(menuBSAnagraficaResx, "Azioni", "Azioni"), width: widthAzioni
            }
        ];
    }


    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };
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
        btnEliminaTuttiFiltri: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: false,
        //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        colonneCustomKendoGrid: colonneCustomKendoGrid

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
            if (doSelect == false && data.length == 0) {
                var rowUid = data[0].uid;
                var row = grid.table.find("[data-uid=" + rowUid + "]");
                grid.selec(row);
                grid.trigger("change");
            }
            Dati_Relativi_Percorso_Selezione2(1);
            nascondiBottoniAzienda();
        },
        funzioneDaChiamareDopoChange: function (e) {
            var grid = $("#" + divKendo).data("kendoGrid");
            var selectedRow = grid.select();
            var dataItem = grid.dataItem(selectedRow);
            if (dataItem != undefined) {
                ImpostaObjP_Agenda(1, dataItem.chiave, false, function () {
                    Dati_Relativi_Percorso_Selezione2(1);
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

function nascondiBottoniAzienda() {
    var grid = $("#divKendoAzienda").data('kendoGrid');
    grid.tbody.find("tr[role='row']").each(function () {

        var model = grid.dataItem(this);

        let visibilita = riportaParametroVisibilita();
        visibilita = visibilita.split('=')[1];

        if (visibilita == "2") {
            permesso_impresa_write = false;
        }

        if (!permesso_impresa_write) {
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
        }

    });
}

function kReadValorizzazioneAzienda_rows(options) {

    var data = $('#hdKendoAzienda_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneAzienda_col() {

    var data = $('#hdKendoAzienda_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_columns;
}

function kReadValorizzazioneAzienda_mod() {

    var data = $('#hdKendoAzienda_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaAzienda() {

    var parametri = {}

    //$.ajax({
    //    type: 'POST',
    //    url: 'MenuBs_Anagrafica.aspx/CaricaAzienda',
    //    data: parametri,
    //    contentType: 'application/json; charset=utf-8',
    //    cache: false,
    //    dataType: 'json', async: true,
    //    success: function (r) {
    //        $('#hdKendoAzienda_Valorizzazione').val(r.d);
    //        kendoAzienda_inizializza("divKendoAzienda");
    //    }
    //});

    ajaxAgronica(indirizzohttp+"/CaricaAzienda", JSON.stringify(parametri), function (risposta) {
        $('#hdKendoAzienda_Valorizzazione').val(risposta.RispostaStringa);
        kendoAzienda_inizializza("divKendoAzienda");
    }, null)

}

function infoAzienda(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/InfoImpresa',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Impresa_edit.aspx" + riportaParametroVisibilita();
        }
    });

}

function modificaAzienda(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditImpresa',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Impresa_edit.aspx" + riportaParametroVisibilita();
        }
    });
}

function eliminaAzienda(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var xTipoNodo = 1;
    var chiave = datiRiga.chiave;

    var streelemento = Traduzione(menuBSAnagraficaResx, "Azienda", "Azienda") +
        ": <b>" + datiRiga.rag_soc + "</b><br><br>" +
        Traduzione(menuBSAnagraficaResx,
            "MenuBS_Anagrafica_eliminaAzienda",
            "<b>ATTENZIONE:</b> Verranno cancellate a cascata tutte le informazioni ad essa collegate.");

    Popup_delete(streelemento, xTipoNodo, chiave);
}
