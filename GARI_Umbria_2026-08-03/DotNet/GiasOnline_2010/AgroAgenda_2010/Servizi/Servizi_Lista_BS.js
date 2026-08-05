async function EsportazioneRegione() {
    let resp = await ws_EsportazioneRegione();
    window.location = resp;
}

async function EsportazioneZespri() {
    let resp = await ws_EsportazioneZespri();
    window.location = resp;
}

async function cercaPratiche() {
    WaitFrame.show();

    objLista_Pratiche = await ws_carica_pratiche();
    CreaKendoPratiche();

    WaitFrame.hide();
}

async function Carica_Cmb_Imprese() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        $("#Cmb_Imprese").kendoDropDownList({
            filter: "contains",
            //autoBind: true,
            dataTextField: "Rag_Soc",
            dataValueField: "Piva",
            mapValueTo: "dataItem",
            dataSource: { transport: { read: ws_Carica_Cmb_Imprese } },
            //open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                var ds = this.dataSource.data();
                if (ds.length == 1) {
                    this.select(1); //seleziono l'elemento 
                    this.onchange(); //forzo l'evento di onchange
                } else if (piva != "" && primo_ddlAzienda_OnDataBound === 0) {
                    this.value(piva);
                    if (this.selectedIndex === -1) {
                        this.select(0);
                    }
                    primo_ddlAzienda_OnDataBound = 1;
                }
                resolve(this);
            },
            virtual: {
                itemHeight: 26,
                valueMapper: function (options) {

                    var val = options.value;
                    if (val != "" && val != "-1") {
                        var a = this.dataSource._pristineData.find(el => el.Piva == val);
                        options.success(this.dataSource._pristineData.indexOf(a));
                    } else {
                        options.success("");
                    }
                }
            },
        }).data("kendoDropDownList");
    });
}

function CreaKendoFiltroPratiche(idDiv) {
    $("#" + idDiv).html("");

    funzioniCRUD = {
        funzioneRead: FiltroPratiche_kReadValorizzazione_rows,
        funzioneInsert: FiltroPratiche_kWriteValorizzazione_rows_insert,
        funzioneUpdate: FiltroPratiche_kWriteValorizzazione_rows_insert,
        funzioneDelete: FiltroPratiche_kWriteValorizzazione_rows_insert,
        //funzioneSubmit: { funzione: FiltroPratiche_kWriteValorizzazione_rows_insert, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: true,
        UtenteAbilitatoCancellazione: true
    };



    //funzioniCRUD.funzioneSubmit = { funzione: FiltroPratiche_kWriteValorizzazione_rows_insert, flagInsert: true, flagUpdate: true, flagDelete: false };

    //colonneCustomKendoGrid = [
    //    {
    //        command: [
    //            {
    //                iconClass: "fa fa-pencil fa-lg", className: "block-modifica", name: "edit",
    //                text: { edit: "&nbsp", update: "&nbsp", cancel: "&nbsp" }
    //            },
    //            {
    //                iconClass: "fa fa-trash fa-lg", className: "block-cancella", name: "destroy",
    //                text: "&nbsp"
    //            }
    //        ], title: "Operazioni"
    //    }
    //];

    colonneCustomKendoGrid = [];

    var idModel = "Servizio_Cod";
    var campiKendoModel = FiltriPratiche_kReadValorizzazione_mod();
    var colonneKendoGrid = FiltriPratiche_kReadValorizzazione_col();
    var parametriPerLettura = null;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: false,
        sortable: false,
        pdf: false,
        excel: false,
        groupable: false,
        //pageable:
        //{
        //    pageSize: 50,
        //    pageSizes: [5, 10, 20, 50, 100, "all"],
        //    buttonCount: 3
        //},
        //editable: { mode: "incell" },
        pageable: false,
        filterable: false,
        btnEliminaTuttiFiltri: false,
        selectable: false,
        colonneCustomKendoGrid: colonneCustomKendoGrid
    };


    var funzioniPrimaDopoEventi = {
        //funzioneDaChiamareDopoEdit: FiltroPratiche_kWriteValorizzazione_rows_insert,
        //funzioneDaChiamareDopoSave: FiltroPratiche_kWriteValorizzazione_rows_insert
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];

    //creaKendoGrid(idDiv, // rappresenta l'ID del div a cui si associa la griglia
    //    funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
    //    idModel, // chiave riga 
    //    campiKendoModel, // campi modello
    //    colonneKendoGrid, // colonne da mostrare
    //    parametriPerLettura, // parametri da passare alla lettura
    //    parametriDataSource, // parametri data source { chiave - valore}
    //    parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
    //    funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
    //    mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
    //    colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    //);


    var model = {
        id: "Servizio_Cod",
        fields: campiKendoModel
    };

    dataSource = new kendo.data.DataSource({
        transport: {
            read: FiltroPratiche_kReadValorizzazione_rows,
            update: FiltroPratiche_kWriteValorizzazione_rows_insert,
            destroy: FiltroPratiche_kWriteValorizzazione_rows_insert,
            create: FiltroPratiche_kWriteValorizzazione_rows_insert
            //    //parameterMap: function (options, operation) {
            //    //    if (operation !== "read" && options.models) {
            //    //        return { models: kendo.stringify(options.models) };
            //    //    }
            //    //}
        },
        batch: true,
        pageSize: 20,
        schema: {
            model: model
        }
        //schema: {
        //    model: {
        //        id: "Servizio_Cod",
        //        fields: {
        //            "Operatore": {
        //                editable: true,
        //                type: "string"
        //            },
        //            Servizio_Cod: {
        //                editable: true,
        //                type: "string"
        //            },
        //            "Servizio_Des": {
        //                editable: true,
        //                type: "string"
        //            },
        //            UnitPrice: { type: "number", validation: { required: true, min: 1 } },
        //            UnitsInStock: { type: "number", validation: { min: 0, required: true } }
        //        }
        //    }
        //}
    });

    col = FiltriPratiche_kReadValorizzazione_col();
    col.push({
        command: [
            {
                name: "edit",
                text: { edit: "&nbsp;", update: "&nbsp;", cancel: "&nbsp;" },
                iconClass: { edit: "fa fa-pencil-square-o fa-2x edit_elem", update: "fa fa-check fa-lg custom-update-icon", cancel: "fa fa-times fa-lg custom-cancel-icon" }
            },
            {
                name: "destroy",
                text: "&nbsp;",
                iconClass: "fa fa-trash fa-lg custom-delete-icon"
            }
        ],
        title: "&nbsp;",
        width: "250px"
    });


    //col = [
    //    {
    //        field: "Operatore",
    //        title: "Operatore",
    //        editor: Operatore_Editor
    //    },
    //    {
    //        field: "Servizio_Des",
    //        title: "Servizio_Des",
    //        editor: Servizio_Editor1
    //    },
    //    { field: "UnitPrice", title: "Unit Price", format: "{0:c}", width: "120px" },
    //    { field: "UnitsInStock", title: "Units In Stock", width: "120px" },
    //    { command: ["edit", "destroy"], title: "&nbsp;", width: "250px" }];

    $("#" + idDiv).kendoGrid({
        dataSource: dataSource,
        pageable: true,
        //height: 550,
        toolbar: ["create"],
        columns: col,
        editable: "inline"
    });

    return $("#" + idDiv).data("kendoGrid");

}

function FiltroPratiche_kReadValorizzazione_rows(options) {
    options.success(JSON.parse("[]"));
}

function FiltroPratiche_kWriteValorizzazione_rows_insert(options) {
    //kendo.alert("ok!");
    //kendo.alert(JSON.stringify(e));
    options.success(true);
}

function FiltriPratiche_kReadValorizzazione_mod() {
    return {
        //"Operatore": {
        //    editable: true,
        //    type: "string"
        //},
        "Servizio_Cod": {
            editable: true,
            type: "string"
        },
        "Servizio_Des": {
            editable: true,
            type: "string"
        },
        "Stati_Des": {
            editable: true,
            type: "string",
            //validation: { required: true }
        },
        "Stati_Cod": {
            editable: true,
            type: "string",
            //validation: { required: true }
        }
    };
}

function FiltriPratiche_kReadValorizzazione_col() {
    return [
        //{
        //    field: "Operatore",
        //    title: "Operatore",
        //    editor: Operatore_Editor
        //},
        {
            field: "Servizio_Des",
            title: "Servizio",
            editor: Servizio_Editor1
        },
        {
            field: "Stati_Des",
            title: "Stato",
            editor: Stato_Editor,
            //template: "#= Stati_Des.join(', ') #",
        }
    ];
}

function Servizio_Editor1(container, options) {
    var ddl = creaDropDownEditor(container, "Servizio_Des", "Servizio_Cod", objServizi, changeServizio);
}

function Operatore_Editor(container, options) {
    var objOperatore = [
        {
            "Op_Cod": "e",
            "Op_Des": "e"
        },
        {
            "Op_Cod": "o",
            "Op_Des": "o"
        }
    ];
    var ddl = creaDropDownEditor(container, "Op_Des", "Op_Cod", objOperatore, changeOperatore);
}

var ddlMultiSelect = undefined;

async function Stato_Editor(container, options) {
    var grid = $("#kendoFiltroPratiche").data("kendoGrid");
    var model = options.model;

    var objStati;
    if (model.Servizio_Cod == 0 || model.Servizio_Cod == "" || model.Servizio_Cod == undefined) {
        objStati = [{
            "Stato_Des": "",
            "Stato_Cod": "0"
        }];
    } else {
        let resp = await trovaStatiServizio(model.Servizio_Cod);

        objStati = JSON.parse(resp);
        //var dataSource = new kendo.data.DataSource({
        //    data: JSON.parse(resp)
        //});

        //ddlMultiSelect.setDataSource(dataSource);
    }

    //ddlMultiSelect = creaKendoMultiselectEditor(container, "Stato_Des", "Stato_Cod", objStati, changeStato);
    ddlMultiSelect = creaDropDownEditor(container, "Stato_Des", "Stato_Cod", objStati, changeStato);
}

async function changeOperatore(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#kendoFiltroPratiche").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Operatore = dataItem.Op_Cod;
}

async function changeServizio(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#kendoFiltroPratiche").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Servizio_Cod = dataItem.Servizio_Cod;
    model.Servizio_Des = dataItem.Servizio_Des;

    let resp = await trovaStatiServizio(model.Servizio_Cod);

    objStati = JSON.parse(resp);

    objStati.unshift({ Stato_Cod: 0, Stato_Des: "TUTTI GLI STATI" })

    var dataSource = new kendo.data.DataSource({
        data: objStati
    });

    ddlMultiSelect.setDataSource(dataSource);
    //kendoFastRedrawRow(grid, row);

}

function changeStato(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#kendoFiltroPratiche").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    //var selectedData = [];
    //var items = e.sender.value();
    //for (var i = 0; i < items.length; i++) {
    //    selectedData.push(items[i]);
    //}

    model.Stati_Cod = dataItem.Stato_Cod;
    model.Stati_Des = dataItem.Stato_Des;

}

function CreaKendoPratiche() {
    $("#" + divKendo_Pratiche).html("");

    var funzioniCRUD;
    var colonneCustomKendoGrid;

    if (UtenteAbilitato_W) {
        funzioniCRUD = {
            funzioneRead: Pratiche_kReadValorizzazione_rows,
            //funzioneInsert: Pratiche_kWriteValorizzazione_rows_insert,
            funzioneUpdate: Pratiche_kWriteValorizzazione_rows_update,
            funzioneDelete: Pratiche_kWriteValorizzazione_rows_delete,
            checkBoxFunction: KendoOperazioni_checkedPratica
        };

        colonneCustomKendoGrid = [
            {
                command: [
                    {
                        name: "edit",
                        text: "&nbsp;",
                        className: "edit_elem",
                        iconClass: "fa fa-pencil-square-o fa-2x"
                        
                    },
                    {
                        name: "destroy",
                        text: "&nbsp;",
                        className: "del_elem",
                        iconClass: "fa fa-trash-o fa-2x"
                    }
                ],
                title: "Operazioni",
                width: "80px"
            }
        ];

    } else {
        funzioniCRUD = {
            funzioneRead: Pratiche_kReadValorizzazione_rows,
            checkBoxFunction: KendoOperazioni_checkedPratica
        };

        colonneCustomKendoGrid = [];

    }

    //var funzioniCRUD = {
    //    funzioneRead: Pratiche_kReadValorizzazione_rows,
    //    //funzioneInsert: Pratiche_kWriteValorizzazione_rows_insert,
    //    funzioneUpdate: Pratiche_kWriteValorizzazione_rows_update,
    //    funzioneDelete: Pratiche_kWriteValorizzazione_rows_delete,
    //    checkBoxFunction: KendoOperazioni_checkedPratica
    //}

    //funzioniCRUD.funzioneSubmit = { funzione: Pratiche_kWriteValorizzazione_rows, flagInsert: true, flagUpdate: true, flagDelete: false };

    var idModel = "Pratica_Cod";

    var campiKendoModel = Pratiche_kReadValorizzazione_mod();
    var colonneKendoGrid = Pratiche_kReadValorizzazione_col();

    if (visualizzaKPIN_Block_Name == true) {
        campiKendoModel.Descrizione_Progetto = {
            editable: false,
            type: "string"
            //validation: { required: true }
        };
        campiKendoModel.KPIN = {
            editable: false,
            type: "string"
            //validation: { required: true }
        };
        campiKendoModel.Block_Name = {
            editable: false,
            type: "string"
            //validation: { required: true }
        };

        colonneKendoGrid.push({
            "field": "Descrizione_Progetto",
            "title": "Descrizione Progetto",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "250px"
        });

        colonneKendoGrid.push({
            "field": "KPIN",
            "title": "KPIN",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "80px"
        });

        colonneKendoGrid.push({
            "field": "Block_Name",
            "title": "Block Name",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "110px"
        });

    }


    var parametriPerLettura = null;
    var parametriDataSource = {};

    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: true,
        sortable: true,
        pdf: false,
        excel: true,
        groupable: false,
        reorderable: true,
        pageable:
        {
            pageSize: 50,
            pageSizes: [5, 10, 20, 50, 100, "all"],
            buttonCount: 3
        },
        filterable: true,
        btnEliminaTuttiFiltri: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        selectable: false,
        toolbarCommands: ["templatekendoPratiche"],
        checkSelezioneRiga: { filterable: false, field: null, width: "30px" },
        //salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        editable: {
            mode: "popup",
            //template: template,
            window: {
                title: "Modifica Dati Pratica"
            }
        },
        colonneCustomKendoGrid: colonneCustomKendoGrid
    };


    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: PraticheGridEdit,
        funzioneDaChiamareDopoSave: PraticheSaveGriglia,
        funzioneDaChiamarePrimaDelDetailInit: detailInit,
        funzioneDaChiamareDopoAnnulla: onAnnulla,
        funzioneDaChiamareDopoDataBound: DataBound_Pratiche
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];

    creaKendoGrid(divKendo_Pratiche, // rappresenta l'ID del div a cui si associa la griglia
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

    $(document).on('click', '.k-grid-edit-command', function () {
        $("body > div.k-window > div.k-popup-edit-form.k-window-content > div.k-edit-buttons.k-actions-start > button.k-grid-cancel-command.k-button.k-button-md.k-rounded-md.k-button-solid.k-button-solid-base > span.k-button-text").hide()
        $("body > div.k-window > div.k-popup-edit-form.k-window-content > div.k-edit-buttons.k-actions-start > button.k-grid-save-command.k-button.k-button-md.k-rounded-md.k-button-solid.k-button-solid-primary > span.k-button-text").hide()
    });
    if (UtenteAbilitato_W) {
        $("#AttivaServizi").show();
        $("#PassaggioStato").show();
    }

    if (UtenteAbilitatoZespri) {
        $("#EsportazioneZespri").show();
    }

    if (UtenteAbilitatoBloccaSblocca) {
        $("#BloccaP").show();
        $("#SbloccaP").show();
    }

    if (UtenteAbilitatoRegione) {
        $("#EsportazioneRegione").show();
    }

    if (isSuperuser) {
        $("#CambiaServizio").show();
    }

    if (UtenteAbilitatoUndoPratiche) {
        $("#UndoP").show();
    }

}

function DataBound_Pratiche(e) {
    var grid = $(e.sender.element[0]).data('kendoGrid');
    var items = e.sender.items();
    items.each(function (index) {
        var dataItem = grid.dataItem(this);

        //k-button
        var hide = false;
        if (!dataItem.Blocco_Flag) {
            $(this).find(".k-button").show();
            hide = false;
        } else {
            $(this).find(".k-button").hide();
            hide = true;
        }

        switch (dataItem.Blocco_Flag) {
            case 0:
                if (!hide) {
                    $(this).find(".k-button").show();
                }
                break;
            case -1:
                $(this).find(".k-button").hide();
                break;
            case -2:
                $(this).find(".k-button").hide();
                break;
        }


    });
}

function onAnnulla() {
    CreaKendoPratiche();
}

function ModificaStatoC(e) {
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    let Pratica_Cod = dataItem.Pratica_Cod;
    let dt_Pratiche = $("#" + divKendo_Pratiche).data("kendoGrid").dataSource.data();
    for (var i = 0; i < dt_Pratiche.length; i++) {
        if (dt_Pratiche[i].Pratica_Cod === Pratica_Cod) {
            dt_Pratiche[i].Selected = true;
        } else {
            dt_Pratiche[i].Selected = false;
        }
    }
    let PassaggioDiStato_Cod = dataItem.PassaggioDiStato_Cod;
    PassaggioDiStato(e, this, dataItem);
}

function showDetails(e) {
    PassaggioDiStato(e, this, undefined);
}

async function Passaggio_di_Stato() {
    //PassaggioDiStato();
    gestionePassaggioDiStato();
}

async function PassaggioDiStato(e, kg, PassaggioDiStato) {
    WaitFrame.show();
    initPassaggioStato();
    $("#divDSS").hide();
    var praticheSelezionate = controlloPraticheSelezionate();
    if (praticheSelezionate === false) {
        WaitFrame.hide();
        return false;
    }
    $("#HDPassaggioStato_Pratica_Cod").val(JSON.stringify(praticheSelezionate));
    if (PassaggioDiStato !== undefined) {
        $("#HDPassaggioStato_PassaggioDiStato").val(JSON.stringify(PassaggioDiStato));
    } else {
        $("#HDPassaggioStato_PassaggioDiStato").val("");
    }

    var dataItem = primoDataItemSelezionato();

    if (dataItem === null) {
        kendo.alert("Selezionare almeno un elemento.");
        return false;
    }

    let Servizio_Des;
    let Servizio_Cod;
    let Stato;
    let Pratica_Cod;
    let StatoAttuale_Cod;
    let note = "";
    let Data_Riferimento = new Date();
    var PassaggioDiStato_Cod = 0;
    if (PassaggioDiStato !== undefined) {
        PassaggioDiStato_Cod = PassaggioDiStato.PassaggioDiStato_Cod;
        Servizio_Des = dataItem.Servizio_Des;
        Servizio_Cod = dataItem.Servizio_Cod;
        Pratica_Cod = dataItem.Pratica_Cod;
        StatoAttuale_Cod = PassaggioDiStato.Stato_Cod;
        note = PassaggioDiStato.note;
        Data_Riferimento = PassaggioDiStato.Validita_Inizio_Stato;
    } else {
        Servizio_Des = dataItem.Servizio_Des;
        Servizio_Cod = dataItem.Servizio_Cod;
        Pratica_Cod = dataItem.Pratica_Cod;
        StatoAttuale_Cod = dataItem.StatoAttuale_Cod;
        note = "";
        Data_Riferimento = new Date();
    }




    $("#lbl_Servizio").text("Servizio:" + Servizio_Des);
    $("#lbl_StatoAttuale").text("Stato Attuale:" + Stato);

    ddlProcedure = await ws_caricaProcedura(Pratica_Cod);
    CmbProcedura = await Carica_cmb_Procedura(ddlProcedure);
    CmbProcedura.refresh();

    if (PassaggioDiStato_Cod === 0) {
        ddlStatiDestinazione = await ws_caricaStatiDestinazione(StatoAttuale_Cod, Servizio_Cod);
    } else {
        ddlStatiDestinazione = [{ Stato_Des: PassaggioDiStato.Stato_Des, Stato_Cod: StatoAttuale_Cod }];
    }

    Cmb_Stato_Destinazione = await Carica_cmb_StatiDestinazione(ddlStatiDestinazione);
    Cmb_Stato_Destinazione.refresh();

    if (Servizio_Cod === 1009 && (StatoAttuale_Cod === 1001 && PassaggioDiStato_Cod === 0 || StatoAttuale_Cod === 1002 && PassaggioDiStato_Cod !== 0)) {
        if (UtenteAbilitato_Provisioning_R === true) {

            ddlPacchettiDSS = await ws_caricaPacchettiDSS();
            DSS_cmb_pacchettiAcquistati = await Carica_DSS_cmb_pacchettiAcquistati(ddlPacchettiDSS);
            DSS_cmb_pacchettiAcquistati.refresh();

            Txt_Data_Scadenza = get_Data_Scadenza();
            Txt_Data_Scadenza.value(new Date());

            if (DSS_cmb_pacchettiAcquistati.dataSource.data().length === 1) {
                DSS_cmb_pacchettiAcquistati.select(1);
            }

            objLista_DSS_Selezionati = await ws_CaricaDSS_PassaggioStato(PassaggioDiStato_Cod);
            creaKendoDSS(divKendoDSS);

            $("#divDSS").show();
        } else {
            kendo.alert("Non si hanno i permessi per questo tipo di servizio.");
            return false;
        }
    }

    if (CmbProcedura.dataSource.data().length === 1) {
        CmbProcedura.select(1);
    }

    if (Cmb_Stato_Destinazione.dataSource.data().length === 1) {
        Cmb_Stato_Destinazione.select(1);
    }

    Txt_Data_Riferimento = get_Data_Riferimento();

    Txt_Data_Riferimento.value(Data_Riferimento);

    $("#Txt_Note").val(note);


    if (PassaggioDiStato_Cod !== 0) {
        Cmb_Stato_Destinazione.enable(false);
        CmbProcedura.enable(false);
    } else {
        Cmb_Stato_Destinazione.enable(true);
        CmbProcedura.enable(true);
    }

    WaitFrame.hide();
    kendoDialogPassaggioStato.open();
    kendoDialogPassaggioStato.center();
}

function contieneStato(strStati, Stato_Cod) {
    var stati = JSON.parse(strStati);
    for (var i = 0; i < stati.length; i++) {
        if (stati[i].Stato_Cod === Stato_Cod) {
            return true;
        }
    }
    return false;
}

function controlloPraticheSelezionate() {
    var grid = $("#" + divKendo_Pratiche).data("kendoGrid");
    var data = grid.dataSource.data();
    var Servizio_Cod = 0;
    var Stato_Cod = 0;
    selected_add = new Array();
    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected === true) {
            if (Servizio_Cod === 0) {
                Servizio_Cod = data[i].Servizio_Cod;
            }
            if (Stato_Cod === 0) {
                Stato_Cod = data[i].Stato;
            }
            if (Servizio_Cod !== data[i].Servizio_Cod || Stato_Cod !== data[i].Stato) {
                kendo.alert("Si può modificare lo stato di pratiche con lo stesso servizio e lo stesso stato.");
                return false;
            } else {
                selected_add.push(data[i]);
            }
            if (data[i].Blocco_Flag === -2) {
                kendo.alert("Hai selezionato una pratica bloccata, non è possibile procedere con l'avanzamento di stato.");
                return false;
            }
        }
    }
    if (selected_add.length === 0) {
        kendo.alert("Selezionare almeno una pratica!");
        WaitFrame.hide();
        return false;
    }
    return selected_add;
}

function primoDataItemSelezionato() {
    var grid = $("#" + divKendo_Pratiche).data("kendoGrid");
    var data = grid.dataSource.data();
    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected === true) {
            return data[i];
        }
    }
    return null;
}

function elencoPraticheSelezionate() {

}

function initPassaggioStato() {

    if (kendoDialogPassaggioStato !== undefined) {
        return;
    }

    kendoDialogPassaggioStato = $("#kendoDialogPassaggioStato").kendoDialog({
        width: "670px",
        heigth: "467px",
        maxHeight: "467px",
        modal: false,
        title: 'Passaggio di Stato',
        closable: true,
        visible: false,
        actions: [
            { text: 'Annulla' },
            { text: 'Conferma', action: CambiaStatoOK1, primary: true }
        ]
    }).data("kendoDialog");

}

function CambiaStatoOK1() {
    CambiaStatoOK();
}

async function CambiaStatoOK() {

    var grid = $("#" + divKendo_Pratiche).data("kendoGrid");
    var data = grid.dataSource.data();
    selected_add = new Array();
    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected === true) {
            selected_add.push(data[i]);
        }
    }

    if (Cmb_Stato_Destinazione.value() === "") {
        kendo.alert("Selezionare uno stato di destinazione valido.");
        return false;
    }

    if (CmbProcedura.value() === "") {
        kendo.alert("Selezionare una procedura valida.");
        return false;
    }

    if (Txt_Data_Riferimento.value() === "") {
        kendo.alert("Selezionare una data di riferimento valida.");
        return false;
    }


    let PassaggioDiStato_cod = 0;
    if ($("#HDPassaggioStato_PassaggioDiStato").val() !== "" && $("#HDPassaggioStato_PassaggioDiStato").val() !== null) {
        let PassaggioDiStato = JSON.parse($("#HDPassaggioStato_PassaggioDiStato").val());
        PassaggioDiStato_cod = PassaggioDiStato.PassaggioDiStato_Cod;
    }

    if (objLista_DSS_Selezionati === undefined) {
        objLista_DSS_Selezionati = new Array();
    }

    let resp = await ws_PassaggioStato(selected_add, Cmb_Stato_Destinazione.value(), Txt_Data_Riferimento.value(), $("#Txt_Note").val(), CmbProcedura.value(), PassaggioDiStato_cod, objLista_DSS_Selezionati, false);

    if (resp.RispostaConferma === true) {
        kendo.alert(resp.RispostaStringa);
        cercaPratiche();
    } else {
        kendo.alert(resp.RispostaStringa);
    }

}

async function Carica_cmb_Procedura(ddlProcedure) {
    return new Promise((resolve, reject) => {
        if (CmbProcedura !== undefined) {
            CmbProcedura.destroy();
            $("#CmbProcedura").html("");
        }
        let onLoad = true;
        $("#CmbProcedura").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "WorkFlow_des",
            dataValueField: "WorkFlow_Cod",
            dataSource: ddlProcedure,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                resolve(this);
            },
            optionLabel: 'SELEZIONA'
        }).data("kendoDropDownList");

    });
}

async function Carica_cmb_StatiDestinazione(ddlStatiDestinazione) {
    return new Promise((resolve, reject) => {
        if (Cmb_Stato_Destinazione !== undefined) {
            Cmb_Stato_Destinazione.destroy();
            $("#Cmb_Stato_Destinazione").html("");
        }
        let onLoad = true;
        $("#Cmb_Stato_Destinazione").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Stato_Des",
            dataValueField: "Stato_Cod",
            dataSource: ddlStatiDestinazione,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                resolve(this);
            },
            optionLabel: 'SELEZIONA'
        }).data("kendoDropDownList");
    });
}

async function Carica_DSS_cmb_pacchettiAcquistati(ddlPacchettiDSS) {
    return new Promise((resolve, reject) => {
        if (DSS_cmb_pacchettiAcquistati !== undefined) {
            DSS_cmb_pacchettiAcquistati.destroy();
            $("#DSS_cmb_pacchettiAcquistati").html("");
        }
        let onLoad = true;
        $("#DSS_cmb_pacchettiAcquistati").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "ModelliPrevisionaliRaggruppamenti_Des",
            dataValueField: "ModelliPrevisionaliRaggruppamenti_Cod",
            dataSource: ddlPacchettiDSS,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                resolve(this);
            },
            optionLabel: 'SELEZIONA'
        }).data("kendoDropDownList");
    });
}

function get_Data_Riferimento() {
    if (Txt_Data_Riferimento === undefined) {
        return $("#Txt_Data_Riferimento").kendoDatePicker({
            dateInput: true
        }).data("kendoDatePicker");
    } else {
        return Txt_Data_Riferimento;
    }
}

function get_Data_Scadenza() {
    if (Txt_Data_Scadenza === undefined) {
        return $("#Txt_Data_Scadenza").kendoDatePicker({
            dateInput: true
        }).data("kendoDatePicker");
    } else {
        return Txt_Data_Scadenza;
    }
}

function PraticheGridEdit(e) {
    $($(e.container).find(".k-edit-label")[0]).hide();
    $($(e.container).find(".k-edit-field")[0]).hide();
}

function PraticheSaveGriglia(e) {
    //alert("PraticheSaveGriglia")
}

function Pratiche_onDataBoundRighe(e) {

}

async function Pratiche_kWriteValorizzazione_rows(e) {
    kendo.alert("Pratiche_kWriteValorizzazione_rows");
}

async function Pratiche_kWriteValorizzazione_rows_insert(e) {
    kendo.alert("Pratiche_kWriteValorizzazione_rows_insert");
}

async function Pratiche_kWriteValorizzazione_rows_update(e) {
    WaitFrame.show();

    await ws_aggiornaPratica(e.data.models);
    objLista_Pratiche = await ws_carica_pratiche();
    CreaKendoPratiche();

    WaitFrame.hide();
}

async function Pratiche_kWriteValorizzazione_rows_delete(e) {
    let resp = await ws_eliminaPratica(e.data.models);
    kendo.alert(resp.RispostaStringa);
    cercaPratiche();
}

function Pratiche_kReadValorizzazione_rows(options) {
    options.success(objLista_Pratiche);
}

function Pratiche_kReadValorizzazione_mod() {
    return {
        "Pratica_Cod": {
            "editable": false,
            "type": "number",
            validation: { required: true }
        },
        "Piva": {
            "editable": false,
            "type": "string",
            validation: { required: true }
        },
        "Cuaa": {
            "editable": false,
            "type": "string",
            validation: { required: true }
        },
        "Rag_Soc": {
            "editable": false,
            "type": "string"
        },
        "Servizio_Des": {
            "editable": true,
            "type": "string",
            validation: { required: true }
        },
        "Servizio_Cod": {
            "editable": true,
            "type": "number",
            validation: { required: true }
        },
        "Data_Inizio": {
            "editable": true,
            "type": "date"
        },
        "Data_Fine": {
            "editable": true,
            "type": "date"
        },
        "Stato_Cod": {
            "editable": false,
            "type": "number",
            validation: { required: true }
        },
        "Stato": {
            "editable": false,
            "type": "string",
            validation: { required: true }
        },
        "Colore": {
            "editable": false,
            "type": "string"
        },
        "stato_Origine_cod": {
            "editable": false,
            "type": "string"
        },
        "PassaggioDiStato_cod": {
            "editable": false,
            "type": "string"
        },
        "StatoAttuale_Des": {
            "editable": false,
            "type": "string"
        },
        "Numero": {
            "editable": true,
            "type": "string",
            validation: { required: false }
        },
        "WWorkflow_Cod": {
            "editable": false,
            "type": "string",
            validation: { required: true }
        },
        "dtStati": {
            "editable": false,
            "type": "string"
        },
        "Utente": {
            "editable": false,
            "type": "string"
        },
        "Blocco_Flag": {
            "editable": false,
            "type": "number"
        },
        "Programmazione_Des_Long": {
            editable: false,
            type: "string",
            //validation: { required: true }
        },
        "Entita_Des": {
            editable: false,
            type: "string",
            //validation: { required: true }
        },
        "Data_Modifica": {
            editable: false,
            type: "date",
            //validation: { required: true }
        },
        "Data_Creazione": {
            editable: false,
            type: "date",
            //validation: { required: true }
        }
    };
}

function Pratiche_kReadValorizzazione_col() {

    return [
        {
            "field": "Servizio_Des",
            "title": "Servizio",
            "filterable": {
                "multi": true,
                "search": true
            },
            editor: Servizio_Editor,
            "width": "200px"
        },
        {
            "field": "Rag_Soc",
            "title": "Impresa",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "200px"
        },
        {
            "field": "PivaReale",
            "title": "P.IVA",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "110px"
        },
        {
            "field": "Cuaa",
            "title": "CUAA",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "180px"
        },
        {
            "field": "Data_Inizio",
            "title": "Data Inizio",
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Data_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Data_Inizio, "dd/MM/yyyy" ) #',
            "width": "110px"
        },
        {
            "field": "Data_Fine",
            "title": "Data Fine",
            filterable: {
                ui: "datepicker"
            },
            template: '#= (kendo.toString(Data_Fine, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Data_Fine, "dd/MM/yyyy" ) #',
            "width": "110px"
        },
        {
            "field": "Stato",
            "title": "Stato",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "200px"
        },
        {
            "field": "Numero",
            "title": "Numero",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "200px"
        },
        {
            "field": "Utente",
            "title": "Utente",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "200px"
        },
        {
            "field": "Programmazione_Des_Long",
            "title": "Piano",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "110px"
        },
        {
            "field": "Entita_Des",
            "title": "Entità",
            "filterable": {
                "multi": true,
                "search": true
            },
            "width": "110px"
        },
        {
            "field": "Data_Modifica",
            "title": "Data Modifica",
            filterable: {
                ui: "datepicker"
            },
            "format": "{0:dd/MM/yyyy}",
            "width": "110px"
        },
        {
            "field": "Data_Creazione",
            "title": "Data Creazione",
            filterable: {
                ui: "datepicker"
            },
            "format": "{0:dd/MM/yyyy}",
            "width": "110px"
        }
    ];
}

function KendoOperazioni_checkedPratica(e) {
    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = $('#kendoPratiche').data("kendoGrid");
    var dataItem = grid.dataItem(row);
    dataItem.Selected = checked;
    dataItem.dirty = true;
    rowKendoGridSelected(row, checked)
}

function detailInit(e) {
    detaliInitA(e);
}

async function detaliInitA(e) {
    var pratica_selezionata = e.data.Pratica_Cod;
    var id_div = "GrigliaCatasto" + pratica_selezionata.toString();
    $("<div id='" + id_div + "' />").appendTo(e.detailCell);
    var dataStr = await Carica_PassaggiDiStato(pratica_selezionata);
    var data = JSON.parse(dataStr);
    popolaGrigliaCronologiaStati(pratica_selezionata, id_div, data);
}

var stati_pratica;

function popolaGrigliaCronologiaStati(chiave, id_div, data) {
    stati_pratica = data;
    var funzioniCRUD = {
        funzioneRead: Read_Stati,
        funzioneUpdate: updateStati
    };
    var idModel = "PassaggioDiStato_Cod";
    var campiKendoModel = App_kReadValorizzazione_mod_Stati();
    var colonneKendoGrid = App_kReadValorizzazione_col_Stati();
    var parametriPerLettura = null;
    var parametriDataSource = {};
    //var parametriDataSource = {
    //    parametriInsert: [{ "id_div": id_div }, { "data": data }, { "chiave": chiave }],
    //    parametriUpdate: [{ "id_div": id_div }, { "data": data }, { "chiave": chiave }],
    //    parametriDelete: [{ "id_div": id_div }, { "data": data }, { "chiave": chiave }]
    //};

    var colonneCustomKendoGrid;

    if (UtenteAbilitato_W) {
        colonneCustomKendoGrid = [
            {
                command: [
                    {
                        name: " ", click: ModificaStatoC, iconClass: "fa fa-pencil fa-2"
                    }
                ],
                title: "Operazioni",
                width: "105px"
            }
        ];
    } else {
        colonneCustomKendoGrid = [];
    }

    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: false,
        sortable: false,
        pdf: false,
        excel: false,
        groupable: false,
        pageable: false,
        filterable: true,
        colonneCustomKendoGrid: colonneCustomKendoGrid,
        editable: { mode: "inline" }
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: DataBound_CronologiaStati
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

function DataBound_CronologiaStati(e) {
    var grid = $(e.sender.element[0]).data('kendoGrid');
    var items = e.sender.items();
    items.each(function (index) {
        var dataItem = grid.dataItem(this);

        //k-button
        var hide = false;
        if (dataItem.modificastato) {
            $(this).find(".k-button").show();
            hide = false;
        } else {
            $(this).find(".k-button").hide();
            hide = true;
        }


        switch (dataItem.Blocco_Flag) {
            case 0:
                if (!hide) {
                    $(this).find(".k-button").show();
                }
                break;
            case -1:
                $(this).find(".k-button").hide();
                break;
            case -2:
                $(this).find(".k-button").hide();
                break;
        }

    });
}

function Read_Stati(options, e) {
    //jSonParsed_Kendo = JSON.parse(stati_pratica);
    options.success(stati_pratica);
}

async function updateStati(e) {

    let data_riferimeno = e.data.models[0].Validita_Inizio_Stato;
    let note = e.data.models[0].note;
    let PassaggioDiStato_Cod = e.data.models[0].PassaggioDiStato_Cod;
    let Stato = e.data.models[0].Stato_Cod;
    let stato_Origine = e.data.models[0].stato_Origine_cod;

    var obj_pratica;
    for (var i = 0; i < objLista_Pratiche.length; i++) {
        if (objLista_Pratiche[i].Pratica_Cod === e.data.models[0].Pratica_Cod) {
            obj_pratica = objLista_Pratiche[i];
            break;
        }
    }

    let Servizio_Cod = obj_pratica.Servizio_Cod;

    let resp = await ws_aggiornaStato(obj_pratica.Pratica_Cod, Stato, data_riferimeno, note, 0, PassaggioDiStato_Cod, Servizio_Cod, stato_Origine);

    if (resp.RispostaConferma) {
        kendo.alert(resp.RispostaStringa);
        cercaPratiche();
    } else {
        kendo.alert(resp.RispostaStringa);
    }

}

function App_kReadValorizzazione_mod_Stati() {
    return {
        "PassaggioDiStato_Cod": {
            "editable": false,
            "type": "number"
        },
        "Stato_Cod": {
            "editable": false,
            "type": "number"
        },
        "Stato_Des": {
            "editable": false,
            "type": "string"
        },
        "Validita_Inizio_Stato": {
            "editable": true,
            "type": "date",
            validation: { required: true }
        },
        "note": {
            "editable": true,
            "type": "string"
        },
        "colore": {
            "editable": false,
            "type": "string"
        },
        "Pratica_Cod": {
            "editable": false,
            "type": "number"
        },
        "stato_Origine_cod": {
            "editable": false,
            "type": "number"
        },
        "Utente": {
            "editable": false,
            "type": "string"
        },
        "Blocco_Flag": {
            "editable": false,
            "type": "number"
        }
    };
}

function App_kReadValorizzazione_col_Stati() {
    return [
        //{
        //    "field": "PassaggioDiStato_Cod",
        //    "title": "PassaggioDiStato_Cod",
        //    "filterable": false
        //},
        //{
        //    "field": "Stato_Cod",
        //    "title": "Stato_Cod",
        //    "filterable": false
        //},
        {
            "field": "Stato_Des",
            "title": "Stato",
            "filterable": false
        },
        {
            "field": "Validita_Inizio_Stato",
            "title": "Data",
            "filterable": false,
            template: '#= (kendo.toString(Validita_Inizio_Stato, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio_Stato, "dd/MM/yyyy" ) #'
        },
        {
            "field": "note",
            "title": "Note",
            "filterable": false
        },
        {
            "field": "Utente",
            "title": "Utente",
            "filterable": false
        }
        //{
        //    "field": "colore",
        //    "title": "colore",
        //    "filterable": false
        //},
        //{
        //    "field": "Pratica_Cod",
        //    "title": "Pratica_Cod",
        //    "filterable": false
        //}
    ];
}

async function AttivaServizi() {
    WaitFrame.show();
    try {
        initAttivaServizi();
        CmbImprese_New = await get_CmbImprese_New();
        CmbServizi_New = await get_CmbServizi_New();
        Data_Inizio_New = get_Data_Inizio_New();
        Data_Fine_New = get_Data_Fine_New();
        Cmb_Data_Apertura = get_Data_Apertura();
        Data_Inizio_New.value("");
        Data_Fine_New.value("");
        Cmb_Data_Apertura.value("");

        if (piva !== "") {
            CmbImprese_New.value(piva);
        }

        kendoDialogNuoviServizi.open();
        kendoDialogNuoviServizi.center();
        WaitFrame.hide();
    } catch (e) {
        WaitFrame.hide();
        kendo.alert(e.message);
    }
}

function Servizio_Editor(container, options) {
    var ddl = creaDropDownEditor(container, "Servizio_Des", "Servizio_Cod", objServizi, changeServizio);
    if (options.model.Pratica_Cod !== 0) {
        ddl.enable(false);
    }
}

function initAttivaServizi() {

    if (kendoDialogNuoviServizi !== undefined) {
        return;
    }

    kendoDialogNuoviServizi = $("#kendoDialogNuovo").kendoDialog({
        width: "670px",
        heigth: "467px",
        modal: true,
        title: 'Attivazione nuovi servizi',
        closable: true,
        visible: false,
        actions: [
            { text: 'Annulla' },
            { text: 'Conferma', primary: true, action: AttivaServizioOK1 }
        ]
    }).data("kendoDialog");

}

function AttivaServizioOK1() {
    AttivaServizioOK();
}

async function AttivaServizioOK() {
    if (ControlloDatiNuovoServizio()) {
        let resp = await ws_attivaServizio();
        if (resp.RispostaConferma === true) {
            cercaPratiche();
        }
        kendo.alert(resp.RispostaStringa);
    }
}

function ControlloDatiNuovoServizio() {
    if (CmbImprese_New.value() === "") {
        kendo.alert("Selezionare un'impresa.");
        return false;
    }
    if (CmbServizi_New.value() === "") {
        kendo.alert("Selezionare un servizio.");
        return false;
    }
    if (Cmb_Data_Apertura.value() === "" || Cmb_Data_Apertura.value() === null || Cmb_Data_Apertura.value() === undefined) {
        kendo.alert("Selezionare una data di apertura della pratica.");
        return false;
    }
    return true;
}

async function get_CmbImprese_New() {
    return new Promise((resolve, reject) => {
        if (CmbImprese_New === undefined) {
            let onLoad = true;
            $("#Cmb_Imprese_New").kendoDropDownList({
                filter: "contains",
                //autoBind: true,
                dataTextField: "Rag_Soc",
                dataValueField: "Piva",
                mapValueTo: "dataItem",
                dataSource: { transport: { read: ws_Carica_Cmb_Imprese_New } },
                //open: kendoDropDownAdjustWidth,
                dataBound: function (e) {
                    var ds = this.dataSource.data();
                    if (ds.length == 1) {
                        this.select(1); //seleziono l'elemento 
                        //ddlAzienda.onchange(); //forzo l'evento di onchange
                    } else if (piva != "" && primo_ddlAzienda_OnDataBound === 0) {
                        this.value(piva);
                        if (this.selectedIndex === -1) {
                            this.select(0);
                        }
                        primo_ddlAzienda_OnDataBound = 1;
                    }
                    resolve(this);
                },
                virtual: {
                    itemHeight: 26,
                    valueMapper: function (options) {

                        var val = options.value;
                        if (val != "" && val != "-1") {
                            var a = this.dataSource._pristineData.find(el => el.Piva == val);
                            options.success(this.dataSource._pristineData.indexOf(a));
                        } else {
                            options.success("");
                        }
                    }
                },
                optionLabel: 'SELEZIONA'
            }).data("kendoDropDownList");
        } else {
            resolve(CmbImprese_New);
        }
    });
}

async function get_CmbServizi_New() {
    //CmbServizi_New = creaDropDownEditor("Cmb_Servizio", "Servizio_Des", "Servizio_Cod", objServizi, changeServizio);
    return new Promise((resolve, reject) => {
        if (CmbServizi_New === undefined) {
            let onLoad = true;
            $("#Cmb_Servizio").kendoDropDownList({
                filter: "contains",
                autoBind: true,
                dataTextField: "Servizio_Des",
                dataValueField: "Servizio_Cod",
                dataSource: objServizi,
                open: kendoDropDownAdjustWidth,
                dataBound: function (e) {
                    kendoDropDownAdjustWidth(e);
                    resolve(this);
                },
                optionLabel: 'SELEZIONA'
            }).data("kendoDropDownList");
        } else {
            resolve(CmbServizi_New);
        }
    });
}

function get_Data_Inizio_New() {
    if (Data_Inizio_New === undefined) {
        return $("#Cmb_Data_Inizio").kendoDatePicker({
            dateInput: true
        }).data("kendoDatePicker");
    } else {
        return Data_Inizio_New;
    }
}

function get_Data_Fine_New() {
    if (Data_Fine_New === undefined) {
        return $("#Cmb_Data_Fine").kendoDatePicker({
            dateInput: true
        }).data("kendoDatePicker");
    } else {
        return Data_Fine_New;
    }
}

function get_Data_Apertura() {
    if (Cmb_Data_Apertura === undefined) {
        return $("#Cmb_Data_Apertura").kendoDatePicker({
            dateInput: true
        }).data("kendoDatePicker");
    } else {
        return Cmb_Data_Apertura;
    }
}

function AggiungiDSS() {
    if (DSS_cmb_pacchettiAcquistati.value() === "") {
        kendo.alert("Selezionare un pacchetto DSS");
        return false;
    }

    if (Txt_Data_Scadenza.value() === "" || Txt_Data_Scadenza.value() === null) {
        kendo.alert("Selezionare una data di scadenza valida");
        return false;
    }

    let ModelliPrevisionaliRaggruppamenti_Cod = DSS_cmb_pacchettiAcquistati.value();
    let ModelliPrevisionaliRaggruppamenti_Des = DSS_cmb_pacchettiAcquistati.text();
    let Data_Scadenza = Txt_Data_Scadenza.value();

    objInit = {
        ModelliPrevisionaliRaggruppamenti_Cod: ModelliPrevisionaliRaggruppamenti_Cod,
        ModelliPrevisionaliRaggruppamenti_Des: ModelliPrevisionaliRaggruppamenti_Des,
        Data_Scadenza: Data_Scadenza
    };

    for (var i = 0; i < objLista_DSS_Selezionati.length; i++) {
        if (objLista_DSS_Selezionati[i].ModelliPrevisionaliRaggruppamenti_Cod === objInit.ModelliPrevisionaliRaggruppamenti_Cod) {
            kendo.alert("Pacchetto DSS già inserito.");
            return false;
        }
    }

    objLista_DSS_Selezionati.push(objInit);
    creaKendoDSS(divKendoDSS);

}

function creaKendoDSS(id_div) {

    $("#" + id_div).html("");

    var funzioniCRUD = {
        funzioneRead: ReadDSS,
        //funzioneDelete: deleteDSS,
        //funzioneInsert: deleteDSS,
        funzioneUpdate: deleteDSS
    };
    var idModel = "ModelliPrevisionaliRaggruppamenti_Cod";
    var campiKendoModel = {
        "ModelliPrevisionaliRaggruppamenti_Cod": { "editable": true, "type": "number" },
        "ModelliPrevisionaliRaggruppamenti_Des": { "editable": true, "type": "string" },
        "Data_Scadenza": { "editable": true, "type": "date" }
    };
    var colonneKendoGrid = [
        { "field": "ModelliPrevisionaliRaggruppamenti_Des", "title": "Modello", "filterable": false },
        { "field": "Data_Scadenza", "title": "Data Scadenza", "filterable": false, template: '#= (kendo.toString(Data_Scadenza, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Data_Scadenza, "dd/MM/yyyy" ) #' }
    ];
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        impostaColonneKendoGridDaCookie: false,
        columnMenu: false,
        sortable: false,
        pdf: false,
        excel: false,
        groupable: false,
        pageable: false,
        filterable: true,
        btnEliminaTuttiFiltri: false,
        colonneCustomKendoGrid: [
            {
                command: [
                    {
                        name: " ", click: deleteDSS, iconClass: "fa fa-close fa-2"
                    }
                ],
                title: "Operazioni",
                width: "105px"
            }
        ],
        editable: { mode: "inline" }
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: DSSGridEdit,
        funzioneDaChiamareDopoSave: DSSSaveGriglia
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

function DSSGridEdit(e) {

}

function DSSSaveGriglia(e) {

}

function deleteDSS(e) {
    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    for (var i = 0; i < objLista_DSS_Selezionati.length; i++) {
        if (dataItem.ModelliPrevisionaliRaggruppamenti_Cod === objLista_DSS_Selezionati[i].ModelliPrevisionaliRaggruppamenti_Cod) {
            objLista_DSS_Selezionati.splice(i, 1);
            break;
        }
    }
    creaKendoDSS(divKendoDSS);
}

function ReadDSS(options) {
    //jSonParsed_Kendo = JSON.parse(objLista_DSS_Selezionati);
    options.success(objLista_DSS_Selezionati);
}


function BloccaPratiche() {
    BloccaSbloccaPratiche(1);
}

function SbloccaPratiche() {
    BloccaSbloccaPratiche(0);
}

async function BloccaSbloccaPratiche(blocca) {
    WaitFrame.show();
    var grid = $("#" + divKendo_Pratiche).data("kendoGrid");
    var data = grid.dataSource.data();
    selected_add = new Array();
    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected === true) {
            selected_add.push(data[i].Pratica_Cod);
        }
    }
    if (selected_add.length === 0) {
        WaitFrame.hide();
        kendo.alert("Selezionare almeno una pratica");
        return false;
    }
    let resp = await ws_BloccaPratiche(blocca, selected_add);
    WaitFrame.hide();
    cercaPratiche();
    kendo.alert(resp);
}


async function CambiaServizio() {
    WaitFrame.show();
    var grid = $("#" + divKendo_Pratiche).data("kendoGrid");
    var data = grid.dataSource.data();
    selected_add = new Array();
    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected === true) {
            selected_add.push(data[i].Pratica_Cod);
        }
    }
    if (selected_add.length == 0 || selected_add.length > 1) {
        WaitFrame.hide();
        kendo.alert("Selezionare una pratica");
        return false;
    }

    CmbCambiaServizio = await get_CmbServizi_Cambia();
    initCambiaServizio();
    kendoDialogCambiaServizio.open();
    kendoDialogCambiaServizio.center();
    WaitFrame.hide();
}

async function get_CmbServizi_Cambia() {
    //CmbServizi_New = creaDropDownEditor("Cmb_Servizio", "Servizio_Des", "Servizio_Cod", objServizi, changeServizio);
    return new Promise((resolve, reject) => {
        if (CmbCambiaServizio === undefined) {
            let onLoad = true;
            $("#CmbCambiaServizio").kendoDropDownList({
                filter: "contains",
                autoBind: true,
                dataTextField: "Servizio_Des",
                dataValueField: "Servizio_Cod",
                dataSource: objServizi,
                open: kendoDropDownAdjustWidth,
                dataBound: function (e) {
                    kendoDropDownAdjustWidth(e);
                    resolve(this);
                },
                optionLabel: 'SELEZIONA'
            }).data("kendoDropDownList");
        } else {
            resolve(CmbCambiaServizio);
        }
    });
}

async function initCambiaServizio() {

    if (kendoDialogCambiaServizio !== undefined) {
        return;
    }

    kendoDialogCambiaServizio = $("#kendoDialogCambiaServizio").kendoDialog({
        width: "670px",
        heigth: "467px",
        maxHeight: "467px",
        modal: false,
        title: 'Cambia Servizio',
        closable: true,
        visible: false,
        actions: [
            { text: 'Annulla' },
            { text: 'Conferma', action: CambiaServizio1, primary: true }
        ]
    }).data("kendoDialog");

}

function CambiaServizio1() {
    CambiaServizio1A();
}

async function CambiaServizio1A() {


    let resp = await ws_CambiaServizio(selected_add[0], CmbCambiaServizio.value());
    WaitFrame.hide();
    cercaPratiche();
    kendo.alert(resp);
}

async function UndoPratiche() {
    WaitFrame.show();
    var grid = $("#" + divKendo_Pratiche).data("kendoGrid");
    var data = grid.dataSource.data();
    selected_add = new Array();
    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected === true) {
            selected_add.push(data[i].Pratica_Cod);
        }
    }
    if (selected_add.length === 0) {
        WaitFrame.hide();
        kendo.alert("Selezionare almeno una pratica");
        return false;
    }
    let resp = await ws_UndoPratiche(selected_add);
    WaitFrame.hide();
    cercaPratiche();
    kendo.alert(resp);
}


function gestionePassaggioDiStato(tr_elem, grid_elem) {
    var grid = $("#kendoPratiche").data("kendoGrid");
    var datiRiga = grid.dataItem(tr_elem);
    var praticheSelezionate = controlloPraticheSelezionate();
    if (praticheSelezionate.length == 0 || praticheSelezionate.length > 1) {
        kendo.alert("Selezionare una pratica.")
        return;
    }
    var parametri = {
        Pratica_Cod: praticheSelezionate[0].Pratica_Cod,
        Passaggio_Di_Stato_Cod: 0
    }
    $.ajax({
        type: "POST",
        url: "Servizi_Lista_BS.aspx/gestionePassaggioDiStato",
        data: JSON.stringify(parametri),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            apriGestionePassaggioDiStato(msg.d.RispostaStringa);
        }
    });
}

function apriGestionePassaggioDiStato(url) {
    $(document.body).append('<div id="GestionePassaggioDiStatoWindow"></div>');
    $('#GestionePassaggioDiStatoWindow').kendoWindow({
        title: "Gestione Passaggio Di Stato",
        modal: true,
        resizable: false,
        iframe: true,
        width: "80%",
        height: "60%",
        content: url,
        close: function () {
            setTimeout(function () {
                $('#GestionePassaggioDiStatoWindow').kendoWindow('destroy');
                //$("#btn_CercaPratiche").trigger("click");
            }, 200);
        }
    }).data('kendoWindow').center();
}

function chiudiWindowPassaggioDiStato(msg) {

    setTimeout(function () {
        kendo.alert(msg);
        $('#GestionePassaggioDiStatoWindow').kendoWindow('destroy');
        $("#btn_CercaPratiche").trigger("click");
    }, 200);

}