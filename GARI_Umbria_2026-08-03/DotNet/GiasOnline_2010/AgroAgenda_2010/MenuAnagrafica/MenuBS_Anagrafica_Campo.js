function kendoCampo_onDataBoundedRighe(e) {

    //Se in mobile mostro solo la colonna unica
    //mostraColonnaUnicaSeInMobile(e, 1);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe("#divKendoCampo", 1);

}

function kendoCampo_inizializza(divKendo, keys) {

    var funzioniCRUD = { funzioneRead: kReadValorizzazioneCampo_rows };

    var idModel = "chiave";

    var campiKendoModel = kReadValorizzazioneCampo_mod();
    var colonneKendoGrid = kReadValorizzazioneCampo_col();

    var parametriPerLettura = [];
    var parametriDataSource = {
        pagesize: 50,
        aggregate: [
            { field: "Superficie_Biologico", aggregate: "sum" },
            { field: "Superficie_Conversione", aggregate: "sum" },
            { field: "Superficie_Convenzionale", aggregate: "sum" },
            { field: "Superficie_Totale", aggregate: "sum" },
            { field: "Superficie_Catastale", aggregate: "sum" }
        ]
    };

    var templateCommands = "<div class='btn-group-vertical'>" +
        "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoCampo(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaCampo(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + "</div>" +
        "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaCampo(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + "</div>" +
        "</div>";
    var widthAzioni = "97px";

    if (GiasVersioneMaster === "2022") {
        templateCommands = '<button type="button" class="btn-Info k-grid-Info k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=infoCampo(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" class="btn-Modifica k-grid-Modifica k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=modificaCampo(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" class="btn-Cancella k-grid-Cancella k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=eliminaCampo(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>';
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
        toolbarCommands: ["templateKendoCampi"],
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
            arrKeys.push(objParametri_Agenda.Piva + '_' + objParametri_Agenda.Sa_Cod + '_' + objParametri_Agenda.Campo_Cod)
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
            Dati_Relativi_Percorso_Selezione2(3);
            nascondiBottoniCampo()
        },
        funzioneDaChiamareDopoChange: function (e) {
            var grid = $("#" + divKendo).data("kendoGrid");
            var selectedRow = grid.select();
            var dataItem = grid.dataItem(selectedRow);
            ImpostaObjP_Agenda(3, dataItem.chiave, false, function () {
                Dati_Relativi_Percorso_Selezione2(3);
            });
        },
        funzioneDaChiamarePrimaDelDetailInit: detailInitCampi
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

async function detailInitCampi(e) {
    WaitFrame.show();
    var chiave_selezionata = e.data.chiave;
    var id_div = "GrigliaCatastoCampi" + chiave_selezionata.toString();
    $("<div id='" + id_div + "_hidden' style='display: none' />").appendTo(e.detailCell);
    $("<div id='" + id_div + "' />").appendTo(e.detailCell);
    let chiaveArr = e.data.chiave.split("_");
    var resp = await caricaDatiCatastoCampi(chiaveArr[0], chiaveArr[1], chiaveArr[2]);
    $("#" + id_div + "_hidden").val(resp);
    popolaGrigliaCatastoCampo(chiave_selezionata, id_div, resp);
    WaitFrame.hide();
}

function nascondiBottoniCampo() {
    var grid = $("#divKendoCampo").data('kendoGrid');
    grid.tbody.find("tr[role='row']").each(function () {

        var model = grid.dataItem(this);

        if (!permesso_campo_write) {
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
        }

    });
}

function kReadValorizzazioneCampo_rows(options) {

    var data = $('#hdKendoCampo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneCampo_col() {

    //var data = $('#hdKendoCampo_Valorizzazione').val();
    //jSonParsed_Kendo = JSON.parse(data);
    //return jSonParsed_Kendo.kendo_columns;
    var strTotale = Traduzione(menuBSAnagraficaResx, "Totale", "Totale");

    return [
        {
            "field": "sa_nome",
            "title": Traduzione(menuBSAnagraficaResx, "Centro", "Centro"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
        {
            "field": "Campo",
            "title": Traduzione(menuBSAnagraficaResx, "Nome", "Nome"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "200px"
        },
        {
            "field": "Validita_Inizio",
            "title": Traduzione(menuBSAnagraficaResx, "InizioValidità", "Inizio Validità"),
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #',
            "width": "100px"
        },
        {
            "field": "Validita_Fine",
            "title": Traduzione(menuBSAnagraficaResx, "FineValidità", "Fine Validità"),
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Validita_Fine, "dd/MM/yyyy" ) #',
            "width": "100px"
        },
        {
            "field": "Gru_Des",
            "title": Traduzione(menuBSAnagraficaResx, "GruppoVegetale", "Gruppo Vegetale"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
        {
            "field": "Veg_Des",
            "title": Traduzione(menuBSAnagraficaResx, "Specie", "Specie"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "100px"
        },
        {
            "field": "Superficie_Biologico",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieBiologicoAbbr", "Sup. BIO") + " [Ha]",
            "filterable": {
                "multi": true,
                "search": true
            },
            "format": "{0:n4}",
            "footerTemplate": strTotale + ": #: kendo.toString(sum, \"n4\") # ",
            "width": "100px"
        },
        {
            "field": "Superficie_Conversione",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieConversioneAbbr", "Sup. Conversione") + " [Ha]",
            "filterable": {
                "multi": true,
                "search": true
            },
            "format": "{0:n4}",
            "footerTemplate": strTotale + ": #: kendo.toString(sum, \"n4\") # ",
            "width": "100px"
        },
        {
            "field": "Superficie_Convenzionale",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieConvenzionaleAbbr", "Sup. Convenzionale") + " [Ha]",
            "filterable": {
                "multi": true,
                "search": true
            },
            "format": "{0:n4}",
            "footerTemplate": strTotale + ": #: kendo.toString(sum, \"n4\") # ",
            "width": "100px"
        },
        {
            "field": "Superficie_Totale",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieTotaleAbbr", "Sup. Totale") + " [Ha]",
            "filterable": {
                "multi": true,
                "search": true
            },
            "format": "{0:n4}",
            "footerTemplate": strTotale + ": #: kendo.toString(sum, \"n4\") # ",
            "width": "100px"
        },
        {
            "field": "Superficie_Catastale",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieCatastaleAbbr", "Sup. Catastale") + " [Ha]",
            "filterable": {
                "multi": true,
                "search": true
            },
            "format": "{0:n4}",
            "footerTemplate": strTotale + ": #: kendo.toString(sum, \"n4\") # ",
            "width": "100px"
        },
        {
            "field": "rif_alfanumerico",
            "title": Traduzione(menuBSAnagraficaResx, "CodiceCampo", "Codice Campo"),
            "filterable": {
                "multi": true,
                "search": true
            },
            hidden: true,
            "width": "150px"
        },
        {
            "field": "sup_contratto",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieContrattoAbbr", "Sup. Contratto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            hidden: true,
            "width": "150px"
        },
        {
            "field": "filiera",
            "title": Traduzione(menuBSAnagraficaResx, "Filiera", "Filiera"),
            "filterable": {
                "multi": true,
                "search": true
            },
            hidden: true,
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
        }
    ];
}

function kReadValorizzazioneCampo_mod() {

    var data = $('#hdKendoCampo_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaCampo(keys) {

    var parametri = {};

    //$.ajax({
    //    type: 'POST',
    //    url: 'MenuBs_Anagrafica.aspx/CaricaCampi',
    //    data: parametri,
    //    contentType: 'application/json; charset=utf-8',
    //    cache: false,
    //    dataType: 'json', async: true,
    //    success: function (r) {
    //        $('#hdKendoCampo_Valorizzazione').val(r.d);
    //        kendoCampo_inizializza("divKendoCampo");
    //    }
    //});

    ajaxAgronica(indirizzohttp + "/CaricaCampi", JSON.stringify(parametri), function (risposta) {
        $('#hdKendoCampo_Valorizzazione').val(risposta.RispostaStringa);
        kendoCampo_inizializza("divKendoCampo", keys);
    }, null);

}


function infoCampo(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/InfoCampo',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Campo_Edit.aspx" + riportaParametroVisibilita();
        }
    });

}

function modificaCampo(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditCampo',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Campo_edit.aspx" + riportaParametroVisibilita();
        }
    });
}

function eliminaCampo(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var xTipoNodo = 3;
    var chiave = datiRiga.chiave;
    var streelemento = "";
    var strCampoAz = Traduzione(menuBSAnagraficaResx, "CampoAziendale", "Campo Aziendale");
    if (datiRiga.sa_nome !== undefined) {
        streelemento = Traduzione(menuBSAnagraficaResx, "Centro", "Centro") + ": <b>" + datiRiga.sa_nome + "</b> - " +
            strCampoAz + ": <b>" + datiRiga.Campo + "</b>";
    } else {
        streelemento = strCampoAz + ": <b>" + datiRiga.Campo + "</b>";
    }

    Popup_delete(streelemento, xTipoNodo, chiave);
}

function caricaDatiCatastoCampi(piva, sa_cod, campo_cod) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            "_piva": piva,
            "_sa_cod": sa_cod,
            "_campo_cod": campo_cod
        });
        ajaxAgronica("MenuBS_Anagrafica.aspx/Carica_Catasto_Campo",
            parametri,
            function (risposta) {
                let obj_risposta = risposta.RispostaStringa;
                resolve(obj_risposta);
            }, null, null, false);
    });
}

function popolaGrigliaCatastoCampo(chiave, id_div, data) {
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