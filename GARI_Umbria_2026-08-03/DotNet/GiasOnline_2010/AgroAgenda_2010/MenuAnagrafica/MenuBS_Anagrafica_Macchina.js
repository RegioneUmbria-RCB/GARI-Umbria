function kendoMacchina_onDataBoundedRighe(e) {

    //Se in mobile mostro solo la colonna unica
    //mostraColonnaUnicaSeInMobile(e, 1);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe("#divKendoMacchina", 1);

}

function kendoMacchina_inizializza(divKendo, keys) {

    var funzioniCRUD = { funzioneRead: kReadValorizzazioneMacchina_rows };

    var idModel = "chiave";

    var campiKendoModel = kReadValorizzazioneMacchina_mod();
    var colonneKendoGrid = kReadValorizzazioneMacchina_col();

    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };

    var templateCommands = "<div class='btn-group-vertical'>" +
        "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoMacchina(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaMacchina(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + "</div>" +
        "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaMacchina(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + "</div>" +
        "</div>";
    var widthAzioni = "97px";

    if (GiasVersioneMaster === "2022") {
        templateCommands = '<button type="button" class="btn-Info k-grid-Info k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=infoMacchina(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" class="btn-Modifica k-grid-Modifica k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=modificaMacchina(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" class="btn-Cancella k-grid-Cancella k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=eliminaMacchina(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>';
        widthAzioni = "140px";
    }

    var parametriKendoGrid = {
        columnMenu: true,
        impostaColonneKendoGridDaCookie: false,
        excel: true,
        pdf: false,
        search: true,
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
            autoFitSeMobile(e);

            //Accorcio l'altezza delle righe
            riduciAltezzaRighe(e, 1);

            if (keys != undefined) {
                var arrKeys = keys.split(",");
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
            }
            nascondiBottoniMacchine();
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


function nascondiBottoniMacchine() {
    var grid = $("#divKendoMacchina").data('kendoGrid');
    grid.tbody.find("tr[role='row']").each(function () {

        var model = grid.dataItem(this);

        if (!permesso_macchine_write) {
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
        }

        if (model.sa_cod == "-1" && !permesso_gestione_macchine_pubbliche_write) {
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
        }

    });
}

function kReadValorizzazioneMacchina_rows(options) {

    var data = $('#hdKendoMacchina_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneMacchina_col() {

    //var data = $('#hdKendoMacchina_Valorizzazione').val();
    //jSonParsed_Kendo = JSON.parse(data);
    return [
        {
            "field": "Contatto_Des",
            "title": Traduzione(menuBSAnagraficaResx, "Contatto", "Contatto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "300px"
        },
        {
            "field": "Tipologia",
            "title": Traduzione(menuBSAnagraficaResx, "Tipologia", "Tipologia"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        //{
        //    "field": "sa_cod",
        //    "title": "sa_cod",
        //    "filterable": {
        //        "multi": true,
        //        "search": true
        //    }
        //},
        {
            "field": "Ditta_Des",
            "title": Traduzione(menuBSAnagraficaResx, "Marca", "Marca"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Modello",
            "title": Traduzione(menuBSAnagraficaResx, "Modello", "Modello"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Macchina",
            "title": Traduzione(menuBSAnagraficaResx, "MacchinaAttrezzatura", "Macchina / Attrezzatura"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Telaio",
            "title": Traduzione(menuBSAnagraficaResx, "Telaio", "Telaio"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Targa",
            "title": Traduzione(menuBSAnagraficaResx, "Targa", "Targa"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Codice",
            "title": Traduzione(menuBSAnagraficaResx, "Codice", "Codice"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": true
        },
        {
            "field": "Data_Modifica",
            "title": Traduzione(menuBSAnagraficaResx, "DataModifica", "Data Modifica"),
            filterable: {
                ui: "datepicker"
            },
            "format": "{0:dd/MM/yyyy}"
        },
        {
            "field": "Utente_Modifica",
            "title": Traduzione(menuBSAnagraficaResx, "UtenteModifica", "Utente Modifica"),
            "filterable": {
                "multi": true,
                "search": true
            },
        },
        {
            "field": "Data_Creazione",
            "title": Traduzione(menuBSAnagraficaResx, "DataCreazione", "Data Creazione"),
            filterable: {
                ui: "datepicker"
            },
            "format": "{0:dd/MM/yyyy}"
        },
        {
            "field": "Utente_Creazione",
            "title": Traduzione(menuBSAnagraficaResx, "UtenteCreazione", "Utente Creazione"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
    ];
}

function kReadValorizzazioneMacchina_mod() {

    var data = $('#hdKendoMacchina_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaMacchina(keys) {

    var parametri = {};

    //$.ajax({
    //    type: 'POST',
    //    url: 'MenuBs_Anagrafica.aspx/CaricaMacchine',
    //    data: parametri,
    //    contentType: 'application/json; charset=utf-8',
    //    cache: false,
    //    dataType: 'json', async: true,
    //    success: function (r) {
    //        $('#hdKendoMacchina_Valorizzazione').val(r.d);
    //        kendoMacchina_inizializza("divKendoMacchina");
    //    }
    //});

    ajaxAgronica(indirizzohttp + "/CaricaMacchine", JSON.stringify(parametri), function (risposta) {
        $('#hdKendoMacchina_Valorizzazione').val(risposta.RispostaStringa);
        kendoMacchina_inizializza("divKendoMacchina", keys);
    }, null)

}


function infoMacchina(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/InfoMacchina',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Macchina_Edit.aspx" + riportaParametroVisibilita();
        }
    });

}

function modificaMacchina(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditMacchina',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Macchina_edit.aspx" + riportaParametroVisibilita();
        }
    });
}

function eliminaMacchina(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var xTipoNodo = 12;
    var chiave = datiRiga.chiave;

    var streelemento = Traduzione(menuBSAnagraficaResx, "MacchinaAziendale", "Macchina Aziendale") + ": <b>" + datiRiga.Macchina + "</b>";

    Popup_delete(streelemento, xTipoNodo, chiave);
}
