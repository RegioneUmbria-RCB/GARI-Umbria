function kendoAppezzamento_onDataBoundedRighe(e) {

    //Se in mobile mostro solo la colonna unica
    mostraColonnaUnicaSeInMobile(e, 1);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe("#divKendoAppezzamento", 1);

}

function kendoAppezzamento_inizializza(divKendo, keys) {

    var funzioniCRUD = {
        funzioneRead: kReadValorizzazioneAppezzamento_rows,
        checkBoxFunction: KendoOperazioni_appezzamenti
    };

    var idModel = "chiave";

    var campiKendoModel = kReadValorizzazioneAppezzamento_mod();
    var colonneKendoGrid = kReadValorizzazioneAppezzamento_col();

    var parametriPerLettura = null;
    var parametriDataSource = {
        pagesize: 50,
        aggregate: [{ field: "sup_app", aggregate: "sum" }]
    };

    var templateCommands = "<div class='btn-group-vertical'>" +
        "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoAppezzamento(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaAppezzamento(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + "</div>" +
        "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaAppezzamento(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + "</div>" +
        "</div>";
    var widthAzioni = "97px";

    if (GiasVersioneMaster === "2022") {
        templateCommands = '<button type="button" class="btn-Info k-grid-Info k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=infoAppezzamento(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" class="btn-Modifica k-grid-Modifica k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=modificaAppezzamento(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" class="btn-Cancella k-grid-Cancella k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=eliminaAppezzamento(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>';
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
        checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        toolbarCommands: ["templateKendoAppezzamenti"],
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
            riduciAltezzaRighe(e, 2);
            var objParametri_Agenda = JSON.parse(objP_agenda);
            var arrKeys = new Array();
            arrKeys.push(objParametri_Agenda.Piva + '_' + objParametri_Agenda.Sa_Cod + '_' + objParametri_Agenda.Appezza);
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
            Dati_Relativi_Percorso_Selezione2(4);
            nascondiBottoniAppezza();
        },
        funzioneDaChiamareDopoChange: function (e) {
            var grid = $("#" + divKendo).data("kendoGrid");
            var selectedRow = grid.select();
            var dataItem = grid.dataItem(selectedRow);
            var campo_cod = dataItem.Campo_Cod != null ? dataItem.Campo_Cod : 0;
            ImpostaObjP_Agenda(4, dataItem.chiave + "_" + campo_cod, false, function () {
                Dati_Relativi_Percorso_Selezione2(4);
            });
        },
        funzioneDaChiamarePrimaDelDetailInit: detailInitAppezza
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

    CreaToolBarAppezza();

}

function KendoOperazioni_appezzamenti(e) {

    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = $('#divKendoAppezzamento').data("kendoGrid");
    var dataItem = grid.dataItem(row);
    dataItem.Selected = checked;
    dataItem.dirty = true;
    rowKendoGridSelected(row, checked)
}

function nascondiBottoniAppezza() {
    var grid = $("#divKendoAppezzamento").data('kendoGrid');
    grid.tbody.find("tr[role='row']").each(function () {

        var model = grid.dataItem(this);

        if (model.blk_flag == "-1") {
            $(this).addClass("red");
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
        }

        var oggi = new Date().setHours(0, 0, 0, 0);
        if (model.Validita_Inizio > oggi || model.Validita_Fine < oggi) {
            $(this).addClass("DimGray");
        }

        if (!permesso_appezzamento_write) {
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
        }

    });
}

async function detailInitAppezza(e) {
    WaitFrame.show();
    var chiave_selezionata = e.data.chiave;
    var id_div = "GrigliaCatasto" + chiave_selezionata.toString();
    $("<div id='" + id_div + "_hidden' style='display: none' />").appendTo(e.detailCell);
    $("<div id='" + id_div + "' />").appendTo(e.detailCell);
    var resp = await caricaDatiCatasto(e.data.Piva, e.data.Sa_Cod, e.data.Appezza);
    $("#" + id_div + "_hidden").val(resp);
    popolaGrigliaCatasto(chiave_selezionata, id_div, resp);
    WaitFrame.hide();
}

function kReadValorizzazioneAppezzamento_rows(options) {

    var data = $('#hdKendoAppezzamento_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneAppezzamento_col() {

    //var data = $('#hdKendoAppezzamento_Valorizzazione').val();
    //jSonParsed_Kendo = JSON.parse(data);
    //return jSonParsed_Kendo.kendo_columns;

    return [
        {
            "field": "sa_nome",
            "title": Traduzione(menuBSAnagraficaResx, "Centro", "Centro"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "Campo_Des",
            "title": Traduzione(menuBSAnagraficaResx, "Campo", "Campo"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "app_nome",
            "title": Traduzione(menuBSAnagraficaResx, "Nome", "Nome"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "utilizzo",
            "title": Traduzione(menuBSAnagraficaResx, "Utilizzo", "Utilizzo"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "rif_alfanumerico",
            "title": Traduzione(menuBSAnagraficaResx, "RiferimentoAppezzamentoAbbr", "Rif. Appezzamento"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": true,
            "width": "150px"
        },
        {
            "field": "cod_biologico",
            "title": Traduzione(menuBSAnagraficaResx, "AppBioCod", "Cod. Biologico App."),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": true,
            "width": "150px"
        },
        {
            "field": "cod_kpin",
            "title": "KPIN", // i18n
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px",
            "hidden": true
        },
        {
            "field": "cod_block",
            "title": "BLOCK", // i18n
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px",
            "hidden": true
        },
        {
            "field": "sup_app",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieAbbr", "Sup.") + " [ha]",
            "filterable": {
                operators: {
                    number: {
                        eq: Traduzione(menuBSAnagraficaResx, "UgualeA", "Uguale a"),
                        gte: Traduzione(menuBSAnagraficaResx, "MaggioreDi", "Maggiore di"),
                        lte: Traduzione(menuBSAnagraficaResx, "MinoreDi", "Minore di")
                    }
                }
            },
            "format": "{0:n4}",
            "footerTemplate": Traduzione(menuBSAnagraficaResx, "Totale", "Totale") + ": #: kendo.toString(sum, \"n4\") # ",
            "width": "150px"
        },
        {
            "field": "Validita_Inizio",
            "title": Traduzione(menuBSAnagraficaResx, "InizioValidità", "Inizio Validità"),
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #',
            "width": "150px"
        },
        {
            "field": "Validita_Fine",
            "title": Traduzione(menuBSAnagraficaResx, "FineValidità", "Fine Validità"),
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Validita_Fine, "dd/MM/yyyy" ) #',
            "width": "150px"
        },
        {
            "field": "Data_Modifica",
            "title": Traduzione(menuBSAnagraficaResx, "DataModifica", "Data Modifica"),
            filterable: {
                ui: "datepicker"
            },
            "format": "{0:dd/MM/yyyy}",
            "width": "150px"
        },
        {
            "field": "Utente_Modifica",
            "title": Traduzione(menuBSAnagraficaResx, "UtenteModifica", "Utente Modifica"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "Data_Creazione",
            "title": Traduzione(menuBSAnagraficaResx, "DataCreazione", "Data Creazione"),
            filterable: {
                ui: "datepicker"
            },
            "format": "{0:dd/MM/yyyy}",
            "width": "150px"
        },
        {
            "field": "Utente_Creazione",
            "title": Traduzione(menuBSAnagraficaResx, "UtenteCreazione", "Utente Creazione"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "isola",
            "title": Traduzione(menuBSAnagraficaResx, "Isola", "Isola"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "MetodoProduzione_Des",
            "title": "Metodo Produzione",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "blk_flag",
            "title": Traduzione(menuBSAnagraficaResx, "Bloccato", "Bloccato"),
            "filterable": {
                "multi": true,
                "search": true
            },
            values: [
                { text: Traduzione(menuBSAnagraficaResx, "No", "NO"), value: 0 },
                { text: Traduzione(menuBSAnagraficaResx, "Si", "SI"), value: -1 }
            ],
            hidden: true,
            "width": "150px"
        }
    ];
}

function kReadValorizzazioneAppezzamento_mod() {

    var data = $('#hdKendoAppezzamento_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaAppezzamento(keys) {

    var parametri = {};

    //$.ajax({
    //    type: 'POST',
    //    url: 'MenuBs_Anagrafica.aspx/CaricaAppezzamenti',
    //    data: parametri,
    //    contentType: 'application/json; charset=utf-8',
    //    cache: false,
    //    dataType: 'json', async: true,
    //    success: function (r) {
    //        $('#hdKendoAppezzamento_Valorizzazione').val(r.d);
    //        kendoAppezzamento_inizializza("divKendoAppezzamento");
    //    }
    //});

    ajaxAgronica(indirizzohttp + "/CaricaAppezzamenti", JSON.stringify(parametri), function (risposta) {

        $('#hdKendoAppezzamento_Valorizzazione').val(risposta.RispostaStringa);
        kendoAppezzamento_inizializza("divKendoAppezzamento", keys);

        // posiziona griglia su appezzamento selezionato
        var objParametri_Agenda = JSON.parse(objP_agenda);
        var chiave = objParametri_Agenda.Piva + '_' + objParametri_Agenda.Sa_Cod + '_' + objParametri_Agenda.Appezza;
        var grid = $("#divKendoAppezzamento").data("kendoGrid");
        SelezionaRigaGriglia(chiave, grid, "chiave");

    }, null);

}


function infoAppezzamento(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/InfoAppezzamento',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Appezzamento_Edit.aspx" + riportaParametroVisibilita();
        }
    });

}

function modificaAppezzamento(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditAppezzamento',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Appezzamento_edit.aspx" + riportaParametroVisibilita();
        }
    });
}

function eliminaAppezzamento(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var xTipoNodo = 4;
    var chiave = datiRiga.chiave;

    var streelemento = "";
    if (datiRiga.Sa_Nome !== undefined) {
        streelemento += Traduzione(menuBSAnagraficaResx, "Centro", "Centro") + ": <b>" + datiRiga.Sa_Nome + "</b>   ";
    }
    if (datiRiga.Campo_Des !== undefined) {
        streelemento += Traduzione(menuBSAnagraficaResx, "Campo", "Campo") + ": <b>" + datiRiga.Campo_Des + "</b>   ";
    }
    streelemento += Traduzione(menuBSAnagraficaResx, "Appezzamento", "Appezzamento") + ": <b>" + datiRiga.app_nome + "</b>   ";

    Popup_delete(streelemento, xTipoNodo, chiave);
}

function caricaDatiCatasto(piva, sa_cod, appezza) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            "_piva": piva,
            "_sa_cod": sa_cod,
            "_appezza": appezza
        });
        ajaxAgronica("MenuBS_Anagrafica.aspx/Carica_Catasto_Appezzamento",
            parametri,
            function (risposta) {
                let obj_risposta = risposta.RispostaStringa;
                resolve(obj_risposta)
            }, null, null, false);
    });
}

function popolaGrigliaCatasto(chiave, id_div, data) {
    datiCatasto = data;
    var funzioniCRUD = {
        funzioneRead: function (options) {
            jSonParsed_Kendo = JSON.parse(data);
            options.success(jSonParsed_Kendo.kendo_rows);
        },
    };
    var idModel = "key";
    var campiKendoModel = function (options) {
        jSonParsed_Kendo = JSON.parse(data);
        return jSonParsed_Kendo.kendo_model;
    };
    var colonneKendoGrid = kReadCatasto_col();
    var parametriPerLettura = null;
    var parametriDataSource = {
        parametriInsert: [{ "id_div": id_div }, { "data": data }, { "chiave": chiave }],
        parametriUpdate: [{ "id_div": id_div }, { "data": data }, { "chiave": chiave }],
        parametriDelete: [{ "id_div": id_div }, { "data": data }, { "chiave": chiave }]
    };
    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: false,
        sortable: true,
        pdf: false,
        excel: false,
        groupable: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: false,
        checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        //toolbarCommands: ["templateBtnSalvaCatasto"],
        editable: false,
        colonneCustomKendoGrid: [],
        btnEliminaTuttiFiltri: false 
    };

    var funzioniPrimaDopoEventi = {
        //funzioneDaChiamareDopoDataBound: App_onDataBoundRigheCatasto,
        //funzioneDaChiamareDopoEdit: App_onEdit_Catasto
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(id_div, // rappresenta l'ID del div a cui si associa la griglia
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

function kReadCatasto_col() {
    return [
        {
            "field": "Prov",
            "title": Traduzione(menuBSAnagraficaResx, "IstatProvinciaAbbr", "Istat Prov."),
            "filterable": {
                "cell": {
                    "operator": "contains",
                    "suggestionOperator": "contains"
                }
            },
            "width": 80
        },
        {
            "field": "Comuni_Prov",
            "title": Traduzione(menuBSAnagraficaResx, "ProvinciaAbbr", "Prov."),
            "filterable": {
                "cell": {
                    "operator": "contains",
                    "suggestionOperator": "contains"
                }
            },
            "width": 50
        },
        {
            "field": "Com",
            "title": Traduzione(menuBSAnagraficaResx, "IstatComuneAbbr", "Istat Com."),
            "filterable": {
                "cell": {
                    "operator": "contains",
                    "suggestionOperator": "contains"
                }
            },
            "width": 80
        },
        {
            "field": "Localita",
            "title": Traduzione(menuBSAnagraficaResx, "Comune", "Comune"),
            "filterable": {
                "cell": {
                    "operator": "contains",
                    "suggestionOperator": "contains"
                }
            }
        },
        {
            "field": "Sezione",
            "title": Traduzione(menuBSAnagraficaResx, "SezioneAbbr", "Sez."),
            "filterable": {
                "cell": {
                    "operator": "contains",
                    "suggestionOperator": "contains"
                }
            },
            "width": 50
        },
        {
            "field": "Foglio",
            "title": Traduzione(menuBSAnagraficaResx, "FoglioAbbr", "Fgl."),
            "filterable": {
                "cell": {
                    "operator": "contains",
                    "suggestionOperator": "contains"
                }
            },
            "width": 50
        },
        {
            "field": "Numero",
            "title": Traduzione(menuBSAnagraficaResx, "NumeroAbbr", "Num."),
            "filterable": {
                "cell": {
                    "operator": "contains",
                    "suggestionOperator": "contains"
                }
            },
            "width": 80
        },
        {
            "field": "Subalterno",
            "title": Traduzione(menuBSAnagraficaResx, "SubalternoAbbr", "Sub."),
            "filterable": {
                "cell": {
                    "operator": "contains",
                    "suggestionOperator": "contains"
                }
            },
            "width": 50
        },
        {
            "field": "Area",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieAbbr", "Sup.") + " [ha]",
            "filterable": {
                "cell": {
                    "operator": "contains",
                    "suggestionOperator": "contains"
                }
            }
        }
    ];
}

function CreaToolBarAppezza() {

    var gridTB = $("#divKendoAppezzamento").find(".k-grid-toolbar");

    // Create the column menu items.
    //var $menu = $("<ul id='context-menu'></ul>");

    if (permesso_sblocca) {
        let titleSblocca = Traduzione(menuBSAnagraficaResx, 'MenuBS_Anagrafica_sbloccaAppezzamentiSelezionati', 'Sblocca appezzamenti selezionati');
        let titleBlocca = Traduzione(menuBSAnagraficaResx, 'MenuBS_Anagrafica_bloccaAppezzamentiSelezionati', 'Blocca appezzamenti selezionati');

        gridTB.append('<div class="xi-grid-button-group">'+
            '<div id="SbloccaApp" class="k-button k-grid--button k-grid--button__appezzamenti" data-title="' + titleSblocca + '" title = "' + titleSblocca + '">' +
            '<span class="fa fa-unlock"></span></div> <div id="BloccaApp" class="k-button k-grid--button k-grid--button__appezzamenti" data-title="' + titleBlocca + '" title="' + titleBlocca + '"><span class="fa fa-lock"></span></div></div> ');
    }

    if (permesso_appezzamento_ereditatore) {
        let titleModMultipla = Traduzione(menuBSAnagraficaResx, 'MenuBS_Anagrafica_modificaMultiplaAppezzamenti', 'Modifica Multipla Appezzamenti');

        if (GiasVersioneMaster === "2022") {
            gridTB.append(' <div id="btn_ModificaMultiplaAppezza" class="k-button k-button-icontext k-grid--button" data-title="' + titleModMultipla + '" title="' + titleModMultipla + '"><i class="k-icon k-i-list-unordered"></i></div> ');
        } else {
            gridTB.append(' <div id="btn_ModificaMultiplaAppezza" class="k-button k-button-icontext"><i class="k-icon k-i-list-unordered"></i> ' + titleModMultipla + '</div> ');
        }
    }

    //gridTB.append($menu);

    $("#SbloccaApp").click(function () {
        sblocca_appezza();
    });

    $("#BloccaApp").click(function () {
        blocca_appezza();
    });

    $("#btn_ModificaMultiplaAppezza").click(function () {
        ModificaMultiplaAppezza();
    });
}

function ModificaMultiplaAppezza() {
    
    creaModificaMultipla(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_modificaAppezzamentiSelezionati", "Modifica appezzamenti selezionati"));

    var grid = $("#divKendoAppezzamento").data("kendoGrid");
    var data = grid.dataSource.data();
    selected_add = new Array();

    // grid.select().each(function () { selected_add.push(grid.dataItem(this)); });

    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected && data[i].blk_flag!=-1) {            
            selected_add.push(data[i]);
        }
    }

    if (selected_add.length > 0) {
        obj_ModificaMultipla = {};
        obj_ModificaMultipla.anagrafica = "1";
        win_ModificaMultipla.open();
    } else {
        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_SelezionareAppezzamentiNonBloccati", "Selezionare almeno un appezzamento non bloccato."));
    }
}

async function sblocca_appezza() {
    var lista_app = appezzamenti_selezionati();
    var resp = await ws_sblocca_appezza(lista_app);
    kendo.alert(resp);
    setTimeout(function () {
        location.reload();
    }, 2000);
}

async function blocca_appezza() {
    var lista_app = appezzamenti_selezionati();
    var resp = await ws_blocca_appezza(lista_app);
    kendo.alert(resp);
    setTimeout(function () {
        location.reload();
    }, 2000);
}


function appezzamenti_selezionati() {

    var datiGriglia = $("#divKendoAppezzamento").data('kendoGrid');
    var dataAppezza = datiGriglia.dataSource.data();
    var appezzaSelezionati = new Array();
    for (i = 0; i < dataAppezza.length; i++) {
        if (dataAppezza[i].Selected == true) {
            appezzaSelezionati.push(dataAppezza[i].chiave);
        }
    }

    return appezzaSelezionati;

}

function ws_sblocca_appezza(lista_app) {
    return new Promise((resolve, reject) => {

        var parametri = {
            chiavi_appezza: lista_app
        };

        ajaxAgronica(indirizzohttp + "/sblocca_appezzamenti", JSON.stringify(parametri), function (risposta) {
            resolve(risposta.RispostaStringa);
        }, null);

    });
}

function ws_blocca_appezza(lista_app) {
    return new Promise((resolve, reject) => {

        var parametri = {
            chiavi_appezza: lista_app
        };

        ajaxAgronica(indirizzohttp + "/blocca_appezzamenti", JSON.stringify(parametri), function (risposta) {
            resolve(risposta.RispostaStringa);
        }, null);

    });
}

