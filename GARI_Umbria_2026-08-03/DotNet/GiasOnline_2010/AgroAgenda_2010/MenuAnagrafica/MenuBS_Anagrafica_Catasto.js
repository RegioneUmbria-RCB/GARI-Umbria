function kendoCatasto_onDataBoundedRighe(e) {

    //Se in mobile mostro solo la colonna unica
    mostraColonnaUnicaSeInMobile(e, 1);

    //Accorcio l'altezza delle righe
    riduciAltezzaRighe(e, 1);
}

function kendoCatasto_inizializza(divKendo, keys) {

    var funzioniCRUD = { funzioneRead: kReadValorizzazioneCatasto_rows };

    var idModel = "chiave";

    var campiKendoModel = kReadValorizzazioneCatasto_mod();
    var colonneKendoGrid = kReadValorizzazioneCatasto_col();

    var parametriPerLettura = [];
    var parametriDataSource = {
        pagesize: 50,
        aggregate: [
            { field: "Sup_Catastale", aggregate: "sum" },
            { field: "sup_Condotta", aggregate: "sum" }
        ]
    };

    var templateCommands = "<div class='btn-group-vertical'>" +
        "<div class='btn btn-info btnInfo' style='display:block;width:70px;border:0px;' onclick=infoCatasto(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Info", "Info") + "</div>" +
        "<div class='btn btn-success btnModifica' style='display:block;width:70px;border:0px;' onclick=modificaCatasto(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "Modifica", "Modifica") + "</div>" +
        "<div class='btn btn-danger btnCancella' style='display:block;width:70px;border:0px;' onclick=eliminaCatasto(this.closest('tr'),this.closest('.k-grid'))>" + Traduzione(menuBSAnagraficaResx, "RisorsaCancella", "Cancella") + "</div>" +
        "</div>";
    var widthAzioni = "97px";

    if (GiasVersioneMaster === "2022") {
        templateCommands = '<button type="button" class="btn-Info k-grid-Info k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=infoCatasto(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" class="btn-Modifica k-grid-Modifica k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=modificaCatasto(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>' +
            '<button type="button" class="btn-Cancella k-grid-Cancella k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-icon-button" onclick=eliminaCatasto(this.closest("tr"),this.closest(".k-grid"))><span class="k-button-icon"></span></button>';
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
            var objParametri_Agenda = JSON.parse(objP_agenda);
            var arrKeys = new Array();
            arrKeys.push(objParametri_Agenda.Piva);

            let idRigaSelezionataDellAlbero = getIdRigaSelezionataDellAlbero();
            var grid = $("#" + divKendo).data("kendoGrid");
            var data = grid.dataSource.data();
            for (var i = 0; i < arrKeys.length; i++) {
                for (var j = 0; j < data.length; j++) {
                    if (idRigaSelezionataDellAlbero != "") {
                        if (data[j].chiave === idRigaSelezionataDellAlbero) {
                            var rowUid = data[j].uid;
                            var row = grid.table.find("[data-uid=" + rowUid + "]");
                            grid.select(row);
                        }
                    }
                    else if (data[j].chiave == arrKeys[i]) {
                        var rowUid = data[j].uid;
                        var row = grid.table.find("[data-uid=" + rowUid + "]");
                        grid.select(row);
                    }
                }
            }
            Dati_Relativi_Percorso_Selezione2(6);
            nascondiBottoniCatasto();
        },
        funzioneDaChiamareDopoChange: function (e) {
            var grid = $("#" + divKendo).data("kendoGrid");
            var selectedRow = grid.select();
            var dataItem = grid.dataItem(selectedRow);
            ImpostaObjP_Agenda(6, dataItem.chiave, false, function () {
                Dati_Relativi_Percorso_Selezione2(6);
            });
        }
        // ,funzioneDaChiamarePrimaDelDetailInit: detailInitCatasto
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

async function detailInitCatasto(e) {
    WaitFrame.show();
    var chiave_selezionata = e.data.chiave;
    var id_div = "GrigliaAppezzamento" + chiave_selezionata.toString();
    $("<div id='" + id_div + "_hidden' style='display: none' />").appendTo(e.detailCell);
    $("<div id='" + id_div + "' />").appendTo(e.detailCell);
    var resp = await caricaDatiAppezzamento(chiave_selezionata);
    $("#" + id_div + "_hidden").val(resp);
    popolaGrigliaAppezzamento(chiave_selezionata, id_div, resp);
    WaitFrame.hide();
}

function caricaDatiAppezzamento(particella) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            "_particella": particella
        });
        ajaxAgronica("MenuBS_Anagrafica.aspx/Carica_Appezzamenti_Catasto",
            parametri,
            function (risposta) {
                let obj_risposta = risposta.RispostaStringa;
                resolve(obj_risposta);
            }, null, null, false);
    });
}

function popolaGrigliaAppezzamento(chiave, id_div, data) {
    jSonParsed_Kendo = JSON.parse(data);
    var funzioniCRUD = {
        funzioneRead: function (options) {
            options.success(jSonParsed_Kendo.kendo_rows);
        }
    };
    var idModel = "chiave";
    var campiKendoModel = jSonParsed_Kendo.kendo_model;
    var colonneKendoGrid = kReadAppezzamento_col(); // jSonParsed_Kendo.kendo_columns;
    var parametriPerLettura = null;
    var parametriDataSource = { };
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
        editable: false,
        colonneCustomKendoGrid: [],
        btnEliminaTuttiFiltri: false
    };

    var funzioniPrimaDopoEventi = { };
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

function kReadAppezzamento_col() {
    return [
        {
            "field": "sa_nome",
            "title": "Centro",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "Campo_Des",
            "title": "Campo",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "app_nome",
            "title": "Appezzamento",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "150px"
        },
        {
            "field": "Area",
            "title": "Sup. Intersecata [ha]",
            "filterable": {
                "cell": {
                    "operator": "contains",
                    "suggestionOperator": "contains"
                }
            },
            "width": "150px"
        },
        {
            "field": "sup_app",
            "title": "Sup. Appezzamento [ha]",
            "filterable": {
                operators: {
                    number: {
                        eq: "Uguale a",
                        gte: "Maggiore di",
                        lte: "Minore di"
                    }
                }
            },
            "width": "150px"
        },
        {
            "field": "Validita_Inizio_App",
            "title": "Validità appezzamento dal",
            filterable: {
                ui: "datepicker"
            },
            "format": "{0:dd/MM/yyyy}",
            "width": "150px"
            // template: '#= (kendo.toString(Validita_Inizio_App, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio_App, "dd/MM/yyyy" ) #'
        },
        {
            "field": "Validita_Fine_App",
            "title": "Validità appezzamento al",
            filterable: {
                ui: "datepicker"
            },
            "format": "{0:dd/MM/yyyy}",
            "width": "150px"
            // template: '#= (kendo.toString(Validita_Fine_App, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Validita_Fine_App, "dd/MM/yyyy" ) #'
        }
    ];
}

function nascondiBottoniCatasto() {
    var grid = $("#divKendoCatasto").data('kendoGrid');
    grid.tbody.find("tr[role='row']").each(function () {

        var model = grid.dataItem(this);

        if (!permesso_particelle_write) {
            $(this).find(".btnModifica").each(function (item) {
                $(this).hide();
            });
            $(this).find(".btnCancella").each(function (item) {
                $(this).hide();
            });
        }

    });
}

function kReadValorizzazioneCatasto_rows(options) {

    var data = $('#hdKendoCatasto_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazioneCatasto_col() {

    //var data = $('#hdKendoCatasto_Valorizzazione').val();
    //jSonParsed_Kendo = JSON.parse(data);
    //return jSonParsed_Kendo.kendo_columns;

    return [
        {
            "field": "Sa_Nome",
            "title": Traduzione(menuBSAnagraficaResx, "Centro", "Centro"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "COMUNE",
            "title": Traduzione(menuBSAnagraficaResx, "ComuneAbbr", "Com."),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "PROVINCIA",
            "title": Traduzione(menuBSAnagraficaResx, "ProvinciaAbbr", "Prov."),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "SEZIONE",
            "title": Traduzione(menuBSAnagraficaResx, "SezioneAbbr", "Sez."),
            "filterable": {
                "multi": true,
                "search": true
            },
            template: '#= (SEZIONE == 0) ? "" : SEZIONE #'
        },
        {
            "field": "FOGLIO",
            "title": Traduzione(menuBSAnagraficaResx, "FoglioAbbr", "Fgl."),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "NUMERO",
            "title": Traduzione(menuBSAnagraficaResx, "NumeroAbbr", "Num."),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "SUBALTERNO",
            "title": "S.", // i18n
            "filterable": {
                "multi": true,
                "search": true
            },
            template: '#= (SUBALTERNO == 0) ? "" : SUBALTERNO #'
        },
        /*{
            "field": "part_cod",
            "title": "Cod. Particella",
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": true
        },*/
        {
            "field": "cod_particella",
            "title": Traduzione(menuBSAnagraficaResx, "CodiceParticellaAbbr", "Cod. Particella"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": true
        },
        {
            "field": "Titolo_possesso",
            "title": Traduzione(menuBSAnagraficaResx, "Possesso", "Possesso"),
            "filterable": {
                "multi": true,
                "search": true
            }
        },
        {
            "field": "Sup_Catastale",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieCatastaleAbbr", "Sup. Catastale") + " [ha]",
            "filterable": {
                "multi": true,
                "search": true
            },
            "format": "{0:n4}",
            "footerTemplate": "Totale: #: kendo.toString(sum, \"n4\") # "
        },
        {
            "field": "sup_Condotta",
            "title": Traduzione(menuBSAnagraficaResx, "SuperficieCondottaAbbr", "Sup. Condotta") + " [ha]",
            "filterable": {
                "multi": true,
                "search": true
            },
            "format": "{0:n4}",
            "footerTemplate": "Totale: #: kendo.toString(sum, \"n4\") # "
        },
        //{
        //    "field": "Macrousi",
        //    "title": "Macrousi",
        //    encoded: false,
        //    "filterable": {
        //        "cell": {
        //            "operator": "contains",
        //            "suggestionOperator": "contains"
        //        }
        //    }
        //},
        //{
        //    "field": "Utilizzi",
        //    "title": "Utilizzi",
        //    encoded: false,
        //    "filterable": {
        //        "cell": {
        //            "operator": "contains",
        //            "suggestionOperator": "contains"
        //        }
        //    }
        //},
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

function kReadValorizzazioneCatasto_mod() {

    var data = $('#hdKendoCatasto_Valorizzazione').val();
    jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function caricaGrigliaCatasto(keys) {

    var parametri = {};

    //$.ajax({
    //    type: 'POST',
    //    url: 'MenuBs_Anagrafica.aspx/CaricaCatasto',
    //    data: parametri,
    //    contentType: 'application/json; charset=utf-8',
    //    cache: false,
    //    dataType: 'json', async: true,
    //    success: function (r) {
    //        $('#hdKendoCatasto_Valorizzazione').val(r.d);
    //        kendoCatasto_inizializza("divKendoCatasto");
    //    }
    //});

    ajaxAgronica(indirizzohttp + "/CaricaCatasto", JSON.stringify(parametri), function (risposta) {
        $('#hdKendoCatasto_Valorizzazione').val(risposta.RispostaStringa);
        kendoCatasto_inizializza("divKendoCatasto", keys);
    }, null)

}


async function infoCatasto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    //var chiave = datiRiga.chiave;
    //$.ajax({
    //    type: 'POST',
    //    url: './MenuBs_Anagrafica.aspx/InfoCatasto',
    //    data: "{chiave: '" + chiave + "' }",
    //    contentType: 'application/json; charset=utf-8',
    //    cache: false,
    //    dataType: 'json', async: true,
    //    success: function (r) {
    //        window.location = "../Anagrafica/Catasto_Edit.aspx";
    //    }
    //});

    resp = await caricaInfoCatasto(datiRiga.Piva, datiRiga.Prov, datiRiga.Com, datiRiga.FOGLIO, datiRiga.SEZIONE, datiRiga.NUMERO, datiRiga.SUBALTERNO);

    kendoDialogInfoCatasto.open();
    kendoDialogInfoCatasto.center();


    WaitFrame.hide();

}

function modificaCatasto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    WaitFrame.show();
    var chiave = datiRiga.chiave;
    $.ajax({
        type: 'POST',
        url: './MenuBs_Anagrafica.aspx/EditCatasto',
        data: "{chiave: '" + chiave + "' }",
        contentType: 'application/json; charset=utf-8',
        cache: false,
        dataType: 'json', async: true,
        success: function (r) {
            window.location = "../Anagrafica/Catasto_edit.aspx" + riportaParametroVisibilita();
        }
    });
}

function eliminaCatasto(tr_elem, grid_elem) {

    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);

    var xTipoNodo = 10;
    var chiave = datiRiga.chiave;

    var streelemento = Traduzione(menuBSAnagraficaResx, "ParticellaCatastale", "Particella Catastale") + ": " +
        Traduzione(menuBSAnagraficaResx, "ProvinciaAbbr", "Prov.") + " <b>" + datiRiga.PROVINCIA + "(" + chiave.split('_')[2] + ")" + "</b> " +
        Traduzione(menuBSAnagraficaResx, "ComuneAbbr", "Com.") + " <b>" + datiRiga.COMUNE + "(" + chiave.split('_')[3] + ")" + "</b> " +
        Traduzione(menuBSAnagraficaResx, "Sezione", "Sezione") + " <b>" + chiave.split('_')[4] + "</b> " +
        Traduzione(menuBSAnagraficaResx, "Foglio", "Foglio") + " <b>" + chiave.split('_')[5] + "</b> " +
        Traduzione(menuBSAnagraficaResx, "Numero", "Numero") + " <b>" + chiave.split('_')[6] + "</b> " +
        Traduzione(menuBSAnagraficaResx, "Subalterno", "Subalterno") + " <b>" + chiave.split('_')[7] + "</b>";

    WarningParticella(streelemento, xTipoNodo, chiave);

    // Popup_delete(streelemento, xTipoNodo, chiave);
}

function WarningParticella(messaggio, tipo, chiave) {

    var warning = "<b>ATTENZIONE !!!</b><br>Insieme alla particella verranno cancellate tutte le informazioni che la legano ai campi o appezzamenti che ne hanno utilizzato la superficie.";
    var parametri = kendo.stringify({ "chiave": chiave });
    
    ajaxAgronica("./MenuBs_Anagrafica.aspx/LeggiWarningParticella",
        parametri,
        function (risposta) {
            if (risposta.RispostaStringa != "") warning += "<br><br>" + risposta.RispostaStringa;
            if (messaggio != "") messaggio += "<br><br>" + warning; else messaggio = warning;
            Popup_delete(messaggio, tipo, chiave);
        }, function (risposta) {
            kendo.alert(risposta.Errore);
        });
}

async function caricaInfoCatasto(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO) {
    return new Promise((resolve, reject) => {

        let promises = new Array();

        promises.push(caricaInfoMacrousi(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO));
        promises.push(caricaInfoUtilizzi(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO));
        promises.push(caricaInfoClassamento(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO));
        promises.push(caricaInfoAppezzamenti(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO));

        Promise.all(promises).then(value => {
            resolve();
        }, reason => {
            reject();
        });

    });
}

async function caricaInfoMacrousi(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO) {
    //return new Promise(async (resolve, reject) => {
        let resp = await ws_caricaMacrousi_Catasto(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO);

        let divKendo = "kendoMacrousiCatasto"

        jSonParsed_Kendo = JSON.parse(resp);

        var funzioniCRUD = {
            funzioneRead: function (options) {
                options.success(jSonParsed_Kendo.kendo_rows);
            }
        };

        var idModel = "chiave";

        

        var campiKendoModel = jSonParsed_Kendo.kendo_model;
        var colonneKendoGrid = jSonParsed_Kendo.kendo_columns;

        var parametriPerLettura = [];
        var parametriDataSource = {
            pagesize: 50
        };
        var parametriKendoGrid = {
            columnMenu: true,
            impostaColonneKendoGridDaCookie: false,
            excel: false,
            pdf: false,
            sortable: true,
            groupable: false,
            reorderable: true,
            //salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
            scrollable: false,
            selectable: "row",
            pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
            filterable: true,
            btnEliminaTuttiFiltri: false,
            //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
            colonneCustomKendoGrid: []

        };
        var funzioniPrimaDopoEventi = {
            funzioneDaChiamareDopoDataBound: function (e) {}
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

        //resolve();
    //});
}

async function caricaInfoUtilizzi(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO) {
    //return new Promise(async (resolve, reject) => {
        let resp = await ws_caricaUtilizzi_Catasto(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO);

        let divKendo = "kendoUtilizziCatasto";

        jSonParsed_Kendo = JSON.parse(resp);

        var funzioniCRUD = {
            funzioneRead: function (options) {
                options.success(jSonParsed_Kendo.kendo_rows);
            }
        };

        var idModel = "chiave";



        var campiKendoModel = jSonParsed_Kendo.kendo_model;
        var colonneKendoGrid = jSonParsed_Kendo.kendo_columns;

        var parametriPerLettura = [];
        var parametriDataSource = {
            pagesize: 50
        };
        var parametriKendoGrid = {
            columnMenu: true,
            impostaColonneKendoGridDaCookie: false,
            excel: false,
            pdf: false,
            sortable: true,
            groupable: false,
            reorderable: true,
            //salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
            scrollable: false,
            selectable: "row",
            pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
            filterable: true,
            btnEliminaTuttiFiltri: false,
            //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
            colonneCustomKendoGrid: []

        };
        var funzioniPrimaDopoEventi = {
            funzioneDaChiamareDopoDataBound: function (e) { }
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


        //resolve();
    //});
}

async function caricaInfoClassamento(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO) {
    //return new Promise(async (resolve, reject) => {
        let resp = await ws_caricaClassamento_Catasto(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO);

        let divKendo = "kendoClassamentoCatasto";

        jSonParsed_Kendo = JSON.parse(resp);

        var funzioniCRUD = {
            funzioneRead: function (options) {
                options.success(jSonParsed_Kendo.kendo_rows);
            }
        };

        var idModel = "chiave";



        var campiKendoModel = jSonParsed_Kendo.kendo_model;
        var colonneKendoGrid = jSonParsed_Kendo.kendo_columns;

        var parametriPerLettura = [];
        var parametriDataSource = {
            pagesize: 50
        };
        var parametriKendoGrid = {
            columnMenu: true,
            impostaColonneKendoGridDaCookie: false,
            excel: false,
            pdf: false,
            sortable: true,
            groupable: false,
            reorderable: true,
            //salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
            scrollable: false,
            selectable: "row",
            pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
            filterable: true,
            btnEliminaTuttiFiltri: false,
            //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
            colonneCustomKendoGrid: []

        };
        var funzioniPrimaDopoEventi = {
            funzioneDaChiamareDopoDataBound: function (e) { }
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

        //resolve();
    //});
}

async function caricaInfoAppezzamenti(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO) {
    //return new Promise(async (resolve, reject) => {
        let resp = await ws_caricaAppezzamenti_Catasto(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO);

        let divKendo = "kendoAppezzamentiCatasto";

        jSonParsed_Kendo = JSON.parse(resp);

        //Anna 17-05-22: Aggiunto footer somma AREA e Sup_App
        jSonParsed_Kendo.kendo_columns[5].footerTemplate = "Totale: #: kendo.toString(sum, \"n4\") # ";
        jSonParsed_Kendo.kendo_columns[6].footerTemplate = "Totale: #: kendo.toString(sum, \"n4\") # ";

        var funzioniCRUD = {
            funzioneRead: function (options) {
                options.success(jSonParsed_Kendo.kendo_rows);
            }
        };

        var idModel = "chiave";

        var campiKendoModel = jSonParsed_Kendo.kendo_model;
        var colonneKendoGrid = jSonParsed_Kendo.kendo_columns;

        var parametriPerLettura = [];
        var parametriDataSource = {
            pagesize: 50,
            aggregate: [
                { field: "Sup_App", aggregate: "sum" },
                { field: "AREA", aggregate: "sum" }
            ]
        };
        //var parametriDataSource = { pagesize: 50 }
        var parametriKendoGrid = {
            columnMenu: true,
            impostaColonneKendoGridDaCookie: false,
            excel: false,
            pdf: false,
            sortable: true,
            groupable: false,
            reorderable: true,
            //salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
            scrollable: false,
            selectable: "row",
            pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
            filterable: true,
            btnEliminaTuttiFiltri: false,
            //checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
            colonneCustomKendoGrid: []

        };
        var funzioniPrimaDopoEventi = {
            funzioneDaChiamareDopoDataBound: function (e) { }
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

        //resolve();
    //});
}

function ws_caricaMacrousi_Catasto(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO){
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({ "Piva": Piva, "Prov": Prov, "Com": Com, "SEZIONE": SEZIONE, "FOGLIO": FOGLIO, "NUMERO": NUMERO, "SUBALTERNO": SUBALTERNO });

        ajaxAgronica("./MenuBs_Anagrafica.aspx/InfoMacrousiCatasto",
            parametri,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, null, false);
        
    });
}

function ws_caricaUtilizzi_Catasto(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({ "Piva": Piva, "Prov": Prov, "Com": Com, "SEZIONE": SEZIONE, "FOGLIO": FOGLIO, "NUMERO": NUMERO, "SUBALTERNO": SUBALTERNO });

        ajaxAgronica("./MenuBs_Anagrafica.aspx/InfoUtilizziCatasto",
            parametri,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, null, false);

    });
}

function ws_caricaClassamento_Catasto(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({ "Piva": Piva, "Prov": Prov, "Com": Com, "SEZIONE": SEZIONE, "FOGLIO": FOGLIO, "NUMERO": NUMERO, "SUBALTERNO": SUBALTERNO });

        ajaxAgronica("./MenuBs_Anagrafica.aspx/InfoClassamentoCatasto",
            parametri,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, null, false);

    });
}

function ws_caricaAppezzamenti_Catasto(Piva, Prov, Com, FOGLIO, SEZIONE, NUMERO, SUBALTERNO) {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({ "Piva": Piva, "Prov": Prov, "Com": Com, "SEZIONE": SEZIONE, "FOGLIO": FOGLIO, "NUMERO": NUMERO, "SUBALTERNO": SUBALTERNO });

        ajaxAgronica("./MenuBs_Anagrafica.aspx/InfoAppezzamentiCatasto",
            parametri,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, null, false);

    });
}