function kendoCentro_onDataBoundedRighe(e) {

    //Se in mobile mostro solo la colonna unica
    //mostraColonnaUnicaSeInMobile(e, 1);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe("#divKendoCentro", 1);

}

function kendoCentro_inizializza(divKendo, keys) {

    var funzioniCRUD = { funzioneRead: kReadValorizzazioneCentro_rows };

    var idModel = "chiave";

    var campiKendoModel = kReadValorizzazioneCentro_mod();
    var colonneKendoGrid = kReadValorizzazioneCentro_col();

    var qsVisibilita = Request_QueryString("visibilita");
    var colonneCustomKendoGrid = [];

    if (qsVisibilita != "2") {

        var templateCommands = "<div class='btn-group-vertical'>" +
            "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoCentro(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
            "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaCentro(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + "</div>" +
            "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaCentro(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + "</div>" +
            "</div>";
        var widthAzioni = "97px";

        if (GiasVersioneMaster === "2022") {
            templateCommands = '<button type="button" class="btn-Info k-grid-Info k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=infoCentro(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
                '<button type="button" class="btn-Modifica k-grid-Modifica k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=modificaCentro(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
                '<button type="button" class="btn-Cancella k-grid-Cancella k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=eliminaCentro(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>';
            widthAzioni = "130px";
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
        search: true,
        sortable: true,
        groupable: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        scrollable: true,
        selectable: "row",
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: true,
        btnEliminaTuttiFiltri: false,
        //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        toolbarCommands: ["templateKendoCentri"],
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
            arrKeys.push(objParametri_Agenda.Piva + '_' + objParametri_Agenda.Sa_Cod)
            var grid = $("#" + divKendo).data("kendoGrid");
            var data = grid.dataSource.data();
            for (var i = 0; i < arrKeys.length; i++) {
                for (var j = 0; j < data.length; j++) {
                    if (data[j].chiave == arrKeys[i]) {
                        var rowUid = data[j].uid;
                        var row = grid.table.find("[data-uid=" + rowUid + "]");
                        grid.select(row);
                        grid.trigger("change");
                    }
                }
            }
            Dati_Relativi_Percorso_Selezione2(2);
            nascondiBottoniCentro();
        },
        funzioneDaChiamareDopoChange: function (e) {
            var grid = $("#" + divKendo).data("kendoGrid");
            var selectedRow = grid.select();
            var dataItem = grid.dataItem(selectedRow);
            ImpostaObjP_Agenda(2, dataItem.chiave, false, function () {
                Dati_Relativi_Percorso_Selezione2(2);
            });
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

function nascondiBottoniCentro() {
    var grid = $("#divKendoCentro").data('kendoGrid');
    grid.tbody.find("tr[role='row']").each(function () {

        var model = grid.dataItem(this);

        let visibilita = riportaParametroVisibilita();
        visibilita = visibilita.split('=')[1];

        if (visibilita == "2"){
            permesso_centro_write = false;
        }

        if (!permesso_centro_write) {
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
        }

    });
}

function kReadValorizzazioneCentro_rows(options) {

    var data = $('#hdKendoCentro_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneCentro_col() {

    //var data = $('#hdKendoCentro_Valorizzazione').val();
    //jSonParsed_Kendo = JSON.parse(data);
    return [
        {
            "field": "piva",
            "title": Traduzione(menuBSAnagraficaResx, "PartitaIVA", "Partita IVA"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": true
        },
        {
            "field": "rag_soc",
            "title": Traduzione(menuBSAnagraficaResx, "RagioneSociale", "Ragione Sociale"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": true
        },
        {
            "field": "sa_nome",
            "title": Traduzione(menuBSAnagraficaResx, "Nome", "Nome"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "ind_des",
            "title": Traduzione(menuBSAnagraficaResx, "Indirizzo", "Indirizzo"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "com_des",
            "title": Traduzione(menuBSAnagraficaResx, "Comune", "Comune"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "pro_cod",
            "title": Traduzione(menuBSAnagraficaResx, "Provincia", "Provincia"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "CAP",
            "title": Traduzione(menuBSAnagraficaResx, "CAP", "CAP"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Stato",
            "title": Traduzione(menuBSAnagraficaResx, "Stato", "Stato"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Superficie_Totale",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieTotale", "Superficie Totale") + " [Ha]",
            "filterable": {
                operators: {
                    number: {
                        eq: Traduzione(menuBSAnagraficaResx, "UgualeA", "Uguale a"),
                        gte: Traduzione(menuBSAnagraficaResx, "MaggioreDi", "Maggiore di"),
                        lte: Traduzione(menuBSAnagraficaResx, "MinoreDi", "Minore di")
                    }
                }
            }
        },
        //{
        //    "field": "Superficie_Totale",
        //    "title": "Superficie Totale [Ha]",
        //    "filterable": {
        //        "cell": {
        //            "operator": "contains",
        //            "suggestionOperator": "contains"
        //        }
        //    }
        //},
        {
            "field": "Superficie_Tare",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieTare", "Superficie Tare") + " [Ha]",
            "filterable": {
                operators: {
                    number: {
                        eq: Traduzione(menuBSAnagraficaResx, "UgualeA", "Uguale a"),
                        gte: Traduzione(menuBSAnagraficaResx, "MaggioreDi", "Maggiore di"),
                        lte: Traduzione(menuBSAnagraficaResx, "MinoreDi", "Minore di")
                    }
                }
            }
        },
        {
            "field": "SAU_Totale",
            "title": Traduzione(menuBSAnagraficaResx, "SAUTotale", "SAU Totale") + " [Ha]",
            "filterable": {
                operators: {
                    number: {
                        eq: Traduzione(menuBSAnagraficaResx, "UgualeA", "Uguale a"),
                        gte: Traduzione(menuBSAnagraficaResx, "MaggioreDi", "Maggiore di"),
                        lte: Traduzione(menuBSAnagraficaResx, "MinoreDi", "Minore di")
                    }
                }
            }
        },
        {
            "field": "Validita_Inizio",
            "title": Traduzione(menuBSAnagraficaResx, "InizioValidità", "Inizio Validità"),
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #'
        },
        {
            "field": "Validita_Fine",
            "title": Traduzione(menuBSAnagraficaResx, "FineValidità", "Fine Validità"),
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Validita_Fine, "dd/MM/yyyy" ) #'
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
            }
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

function kReadValorizzazioneCentro_mod() {

    var data = $('#hdKendoCentro_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaCentro(keys) {

    var parametri = {};

    //$.ajax({
    //    type: 'POST',
    //    url: 'MenuBs_Anagrafica.aspx/CaricaCentri',
    //    data: parametri,
    //    contentType: 'application/json; charset=utf-8',
    //    cache: false,
    //    dataType: 'json', async: true,
    //    success: function (r) {
    //        $('#hdKendoCentro_Valorizzazione').val(r.d);
    //        kendoCentro_inizializza("divKendoCentro");
    //    }
    //});

    ajaxAgronica(indirizzohttp + "/CaricaCentri", JSON.stringify(parametri), function (risposta) {
        $('#hdKendoCentro_Valorizzazione').val(risposta.RispostaStringa);
        kendoCentro_inizializza("divKendoCentro", keys);
    }, null)

}


function infoCentro(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/InfoCentro',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Centro_Edit.aspx" + riportaParametroVisibilita();
        }
    });

}

function modificaCentro(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditCentro',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Centro_edit.aspx" + riportaParametroVisibilita();
        }
    });
}

function eliminaCentro(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var xTipoNodo = 2;
    var chiave = datiRiga.chiave;
    var streelemento = Traduzione(menuBSAnagraficaResx, "CentroAziendale", "Centro Aziendale") +
        ": <b>" + datiRiga.sa_nome + "</b><br/> " +
        Traduzione(menuBSAnagraficaResx,
            "MenuBS_Anagrafica_eliminaCentro",
            "Verrà eliminato il centro e tutti gli elementi ad esso correlati (appezzamenti, impianti, etc.)");

    Popup_delete(streelemento, xTipoNodo, chiave);
}
