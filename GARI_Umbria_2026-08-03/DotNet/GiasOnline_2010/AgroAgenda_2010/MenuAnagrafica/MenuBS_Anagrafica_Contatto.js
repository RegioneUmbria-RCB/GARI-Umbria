function kendoContatto_onDataBoundedRighe(e) {

    //Se in mobile mostro solo la colonna unica
    //mostraColonnaUnicaSeInMobile(e, 1);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe("#divKendoContatto", 1);

}

function kendoContatto_inizializza(divKendo, keys) {

    var funzioniCRUD = { funzioneRead: kReadValorizzazioneContatto_rows };

    var idModel = "chiave";

    var campiKendoModel = kReadValorizzazioneContatto_mod();
    var colonneKendoGrid = kReadValorizzazioneContatto_col();

    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };

    var ImpedisciCancellazioneContatto = ($(impedisci_eliminazione_contatti).val() == "1");

    var templateCommands = "<div class='btn-group-vertical'>" +
        "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoContatto(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaContatto(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + "</div>";

    if (!ImpedisciCancellazioneContatto)
        templateCommands = templateCommands + "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaContatto(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + "</div>";
    templateCommands = templateCommands + "</div>";
    
    var widthAzioni = "97px";

    if (GiasVersioneMaster === "2022") {
        templateCommands = '<button type="button" class="btn-Info k-grid-Info k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=infoContatto(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" class="btn-Modifica k-grid-Modifica k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=modificaContatto(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>';

        if (ImpedisciCancellazioneContatto)
            templateCommands = templateCommands + '<button type="button" class="btn-Cancella k-grid-Cancella k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=eliminaContatto(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>';
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
            //mostraColonnaUnicaSeInMobile(e, 1);
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
            nascondiBottoniContatti();
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

function nascondiBottoniContatti() {
    var grid = $("#divKendoContatto").data('kendoGrid');
    grid.tbody.find("tr[role='row']").each(function () {

        var model = grid.dataItem(this);

        if (!permesso_contatti_write) {
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
        }

        if (model.sa_cod == "-1" && !permesso_modifica_contatti_pubblici_write) {
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
        }

    });
}

function kReadValorizzazioneContatto_rows(options) {

    var data = $('#hdKendoContatto_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneContatto_col() {

    return [{
        "field": "CF",
        "title": "CF", //i18n
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
        "field": "Impresa",
        "title": Traduzione(menuBSAnagraficaResx, "ImpresaReferente", "Impresa Referente"),
        "filterable": {
            "multi": true,
            "search": true
        }
    },
    {
        "field": "Contatto_Des",
        "title": Traduzione(menuBSAnagraficaResx, "NomeDelContatto", "Nome del Contatto"),
        "filterable": {
            "multi": true,
            "search": true
        }
    },
    {
        "field": "Settore_Des",
        "title": Traduzione(menuBSAnagraficaResx, "CodiceContatto", "Codice Contatto"),
        "filterable": {
            "multi": true,
            "search": true
        },
        "hidden": true
    },
    {
        "field": "Rapporto_Des",
        "title": Traduzione(menuBSAnagraficaResx, "Rapporto", "Rapporto"),
        "filterable": {
            "multi": true,
            "search": true
        }
    },
    {
        "field": "Tipo_Contatto",
        "title": Traduzione(menuBSAnagraficaResx, "Tipo", "Tipo"),
        "filterable": {
            "multi": true,
            "search": true
        }
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

function kReadValorizzazioneContatto_mod() {

    var data = $('#hdKendoContatto_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaContatto(keys) {

    var parametri = {};

    //$.ajax({
    //    type: 'POST',
    //    url: 'MenuBs_Anagrafica.aspx/CaricaContatti',
    //    data: parametri,
    //    contentType: 'application/json; charset=utf-8',
    //    cache: false,
    //    dataType: 'json', async: true,
    //    success: function (r) {
    //        $('#hdKendoContatto_Valorizzazione').val(r.d);
    //        kendoContatto_inizializza("divKendoContatto");
    //    }
    //});

    ajaxAgronica(indirizzohttp + "/CaricaContatti", JSON.stringify(parametri), function (risposta) {
        $('#hdKendoContatto_Valorizzazione').val(risposta.RispostaStringa);
        kendoContatto_inizializza("divKendoContatto", keys);
    }, null)

}


function infoContatto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/InfoContatto',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            let parametroVisibilita = riportaParametroVisibilita();
            if (r.d.indexOf("?") >= 0) {
                parametroVisibilita = parametroVisibilita.replace("?", "&")
            }
            window.location = r.d + parametroVisibilita;
        }
    });

}

async function modificaContatto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var chiave = datiRiga.chiave;

    let permessoModifica = await PermessoModificaContatto(chiave);

    if (permessoModifica) {
        WaitFrame.show();
        $.ajax({
            type: 'POST',
            url: './MenuBs_Anagrafica.aspx/EditContatto',
            data: "{chiave: '" + chiave + "' }",
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'json', async: true,
            success: function (r) {
                let parametroVisibilita = riportaParametroVisibilita();
                if (r.d.indexOf("?") >= 0) {
                    parametroVisibilita = parametroVisibilita.replace("?", "&")
                }
                window.location = r.d + parametroVisibilita;
            }
        });
    }


}

async function eliminaContatto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var xTipoNodo = 36;
    var chiave = datiRiga.chiave;

    let permessoModifica = await PermessoModificaContatto(chiave);

    if (permessoModifica) {
        var streelemento = Traduzione(menuBSAnagraficaResx, "SeiSicuroDiEliminare", "Sei sicuro di voler eliminare") + " <b>" + datiRiga.Contatto_Des + "</b>";

        Popup_delete(streelemento, xTipoNodo, chiave);
    }


}

function PermessoModificaContatto(xChiave) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            xChiave: xChiave
        });

        ajaxAgronica("MenuBS_Anagrafica.aspx/PermessoModificaContatto",
            parametri,
            function (risposta) {
                if (risposta.RispostaConferma == false) {
                    kendo.alert(risposta.RispostaStringa);
                }
                resolve(risposta.RispostaConferma);
            }, null, null, false);
    });
}