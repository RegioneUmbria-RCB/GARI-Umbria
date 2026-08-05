
function CreaTreeView_ParametriQualitativi(ID_Controllo) {

    var dataSourceTreeView = new kendo.data.HierarchicalDataSource({
        transport: {
            read: Leggi_ModuliAnagrafeAttivi
        }
    });

    $("#" + ID_Controllo).kendoTreeView({
        loadOnDemand: false,
        dataSource: dataSourceTreeView,
        dataTextField: "Modulo_Descrizione",
        select: OnSelectTreeView_ParametriQualitativi
    });
}

function OnSelectTreeView_ParametriQualitativi(e) {

    let nodo_selezionato = $("#treeview_ParametriQualitativi").data("kendoTreeView").dataItem(e.node);

    if (nodo_selezionato !== undefined && nodo_selezionato !== null && nodo_selezionato !== "" && nodo_selezionato.Modulo_Generazione !==0) {

        $("#grid_Area_ParametriQualitativi").show();

        PopolaGrigliaTestata_ParametriQualitativi("grid_Testata_ParametriQualitativi", nodo_selezionato);

    }
}

// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA Testata Parametri Qualitativi
// --------------------------------------------------------------------------------------------------------------------------

function PopolaGrigliaTestata_ParametriQualitativi(IDControllo, nodo_selezionato) {

    var Tipo_Produzione = parseInt(nodo_selezionato.Tipo_Produzione);

    var Modulo_Generazione = parseInt(nodo_selezionato.Modulo_Generazione);

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";


    var funzioniCRUD = {
        funzioneRead: CaricaGrigliaTestata_ParametriQualitativi,
        funzioneSubmit: { funzione: SubmitGrid_Testata_E_DettagliParametriQualitativi, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };
    var idModel = "Id_Testata";
    var campiKendoModel = {
        Id_Testata: { editable: false, type: "number" },
        Descrizione: { editable: true, type: "string" }
    };

    var colonneKendoGrid = [
        {
            field: "Descrizione", title: "Descrizione", filterable: { multi: true, search: true }
        }
    ];


    switch (Tipo_Produzione) {

        case enum_TipoRisorsa.PRODUZIONE_VEGETALE:
            //Abilitazione Colonne Specie Vegetale, Varieta
            colonneKendoGrid.push({ field: "Veg_Des", title: "Specie", filterable: { multi: true, search: true } });
            colonneKendoGrid.push({ field: "Cul_Des", title: "Varietà", filterable: { multi: true, search: true } });
            break;


        case enum_TipoRisorsa.PRODUZIONE_ANIMALE:
            //Abilitazione Colonne Specie Animale, Razza
            colonneKendoGrid.push({ field: "Spe_Des", title: "Specie Animale", filterable: { multi: true, search: true } });
            colonneKendoGrid.push({ field: "Raz_Des", title: "Razza", filterable: { multi: true, search: true } });
            break;
    }

    var parametriPerLettura = [Modulo_Generazione];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        editable: "inline",
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        colonneCustomKendoGrid: [
            {
                command: [
                    {
                        iconClass: "fa fa-pencil fa-lg", className: "block-modifica", name: "edit",
                        text: { edit: "&nbsp", update: "&nbsp", cancel: "&nbsp" }
                    },
                    {
                        iconClass: "fa fa-trash fa-lg", className: "block-cancella", name: "destroy", text: "&nbsp"
                    }
                ],
                title: "Operazioni"
            }
        ],
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDelDetailInit: Testata_ParametriQualitativi_detailInit,
        funzioneDaChiamareDopoDataBound: onDataBoundRigheParametriQualitativi
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];


    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

    let gridTestata_ParametriQualitativi = $("#grid_Testata_ParametriQualitativi").data("kendoGrid");

    //Imposto la colonna "Operazioni" come prima colonna della grid,
    //se non era già impostata
    if (gridTestata_ParametriQualitativi.columns[0].title !== "Operazioni") {
        let colonne_gridTestata_ParametriQualitativi = gridTestata_ParametriQualitativi.columns;

        let PosizioneColonnaOperazioni = -1;

        for (var x = 0; x < colonne_gridTestata_ParametriQualitativi.length; x++) {
            if (colonne_gridTestata_ParametriQualitativi[x].title === "Operazioni") {
                PosizioneColonnaOperazioni = x;
                break;
            }
        }

        if (PosizioneColonnaOperazioni !== -1)
            gridTestata_ParametriQualitativi.reorderColumn(0, gridTestata_ParametriQualitativi.columns[PosizioneColonnaOperazioni]);
    }

}

function onDataBoundRigheParametriQualitativi(e) {
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    grid.autoFitColumn(0);
}

function Testata_ParametriQualitativi_detailInit(e) {

    var riga_testata = e.data;

    var id_div = "grid_Dettagli_ParametriQualitativi";
    $("<div id='" + id_div + "' />").appendTo(e.detailCell);

    PopolaGrigliaDettagli_ParametriQualitativi(id_div, riga_testata);

}

// --------------------------------------------------------------------------------------------------------------------------
// GRIGLIA Dettagli Parametri Qualitativi
// --------------------------------------------------------------------------------------------------------------------------
function PopolaGrigliaDettagli_ParametriQualitativi(IDControllo, riga_testata) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: CaricaGrigliaDettagli_ParametriQualitativi,
        funzioneSubmit: { funzione: SubmitGrid_Testata_E_DettagliParametriQualitativi, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };
    var idModel = "key";
    var campiKendoModel = {
        key: { editable: false, type: "string" },
        Modulo_Generazione: { editable: false, type: "number" },
        Tipo: { editable: true, type: "number" },
        Tipo_Des: { editable: true, type: "string" },
        ChkOmni_Invisibili: { editable: true, type: "number" },
        ChkOmni_Invisibili_Des: { editable: true, type: "string" },
        Tabella_ID: { editable: true, type: "number" },
        Tabella_Des: { editable: true, type: "string" }
    };
    var colonneKendoGrid = [
        {
            field: "Tipo_Des", title: "Tipo", filterable: { multi: true, search: true }, editor: Tipo_DropDownEditor
        },
        {
            field: "ChkOmni_Invisibili_Des", title: "Omni", filterable: { multi: true, search: true }, editor: ChkOmni_Invisibili_DropDownEditor
        },
        {
            field: "Tabella_Des", title: "Tabella", filterable: { multi: true, search: true }, editor: Tabella_DropDownEditor
        }
    ];
    var parametriPerLettura = [parseInt(riga_testata.Modulo_Generazione), parseInt(riga_testata.Id_Testata)];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true,
        pdf: false,
        excel: false,
        editable: "inline",
        colonneCustomKendoGrid: [
            {
                command: [
                    {
                        iconClass: "fa fa-pencil fa-lg", className: "block-modifica", name: "edit",
                        text: { edit: "&nbsp", update: "&nbsp", cancel: "&nbsp" }
                    },
                    {
                        iconClass: "fa fa-trash fa-lg", className: "block-cancella", name: "destroy", text: "&nbsp"
                    }
                ],
                title: "Operazioni"
            }
        ]
    };
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundRigheParametriQualitativi };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
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

    //var grid_dettaglitariffe = $("#" + IDControllo + "").data("kendoGrid");
    //grid_dettaglitariffe.bind("cellClose", grid_DettagliTariffe_cellClose);
}


//function grid_DettagliTariffe_cellClose(e) {
//    if (e.type == "save") {
//        input = e.container.find("input[name='Paga_Base']").val();
//        if (input != undefined) {
//            if (input == "" || input == undefined || input == null)
//                e.model.Paga_Base = 0;

//        }

//        input = e.container.find("input[name='Contributi']").val();
//        if (input != undefined) {
//            if (input == "" || input == undefined || input == null)
//                e.model.Contributi = 0;
//        }

//        input = e.container.find("input[name='Aumento_CCNL']").val();
//        if (input != undefined) {
//            if (input == "" || input == undefined || input == null)
//                e.model.Aumento_CCNL = 0;
//        }

//        input = e.container.find("input[name='Aumento_CIPL']").val();
//        if (input != undefined) {
//            if (input == "" || input == undefined || input == null)
//                e.model.Aumento_CIPL = 0;
//        }


//        input = e.container.find("input[name='Terzo_Elemento']").val();
//        if (input != undefined) {
//            if (input == "" || input == undefined || input == null)
//                e.model.Terzo_Elemento = 0;
//        }


//        input = e.container.find("input[name='TFR']").val();
//        if (input != undefined) {
//            if (input == "" || input == undefined || input == null)
//                e.model.TFR = 0;
//        }


//        input = e.container.find("input[name='Validita_Fine']").data("kendoDatePicker");
//        if (input != undefined) {
//            if (input.value() == "" || input.value() == undefined || input.value() == null)
//                e.model.Validita_Fine = new Date("2100/12/31");
//        }

//        input = e.container.find("input[name='Validita_Inizio']").data("kendoDatePicker");
//        if (input != undefined) {
//            if (input.value() == "" || input.value() == undefined || input.value() == null)
//                e.model.Validita_Inizio = new Date("1900/01/01");
//        }
//    }
//}


function Tipo_DropDownEditor(container) {

    creaDropDownEditor(container, "Tipo_Des", "Tipo", ElencoTipoTabella, changeTipo);

}

function changeTipo(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#grid_Dettagli_ParametriQualitativi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Tipo = dataItem.Tipo;
    model.Tipo_Des = dataItem.Tipo_Des;
}


function ChkOmni_Invisibili_DropDownEditor(container) {

    creaDropDownEditor(container, "changeChkOmni_Invisibili_Des", "ChkOmni_Invisibili", ElencoOmni, changeChkOmni_Invisibili);

}

function changeChkOmni_Invisibili(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#grid_Dettagli_ParametriQualitativi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.ChkOmni_Invisibili = dataItem.ChkOmni_Invisibili;
    model.ChkOmni_Invisibili_Des = dataItem.ChkOmni_Invisibili_Des;
}


function Tabella_DropDownEditor(container,options) {

    var ElencoTabella = CaricaddlTabellaGrigliaDettagli_ParametriQualitativi(options.model.Modulo_Generazione);

    creaDropDownEditor(container, "changeChkOmni_Invisibili_Des", "ChkOmni_Invisibili", ElencoTabella, changeTabella);

}

function changeTabella(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#grid_Dettagli_ParametriQualitativi").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.ChkOmni_Invisibili = dataItem.ChkOmni_Invisibili;
    model.ChkOmni_Invisibili_Des = dataItem.ChkOmni_Invisibili_Des;
}


function SubmitGrid_Testata_E_DettagliParametriQualitativi(options) {

    var gridTariffe = $("#tariffe_UC_griglia_tariffe").data("kendoGrid");

    //Controllo che non ci siano due codici uguali
    var errMess = "";
    let ds_grid = gridTariffe.dataSource.data();
    let Sigle = ds_grid.map(function (item) { return item.Sigla });
    let SigleDuplicate = Sigle.some(function (item, idx) {
        return Sigle.indexOf(item) != idx
    });

    if (SigleDuplicate === true) {
        errMess = "Ci sono delle Tariffe che hanno lo stesso Codice. <br>";
    }

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecordsTariffe = [];
    var newRecordsTariffe = [];
    var deletedRecordsTariffe = [];
    var righeNonCancellateTariffe = [];

    var currentDataTariffe = gridTariffe.dataSource.data();

    for (let i = 0; i < currentDataTariffe.length; i++) {

        righeNonCancellateTariffe.push(currentDataTariffe[i].toJSON());
        if (currentDataTariffe[i].isNew()) {
            newRecordsTariffe.push(currentDataTariffe[i].toJSON());
        }
        else if (currentDataTariffe[i].dirty) {
            updatedRecordsTariffe.push(currentDataTariffe[i].toJSON());
        }

    }

    for (let i = 0; i < gridTariffe.dataSource._destroyed.length; i++) {
        deletedRecordsTariffe.push(gridTariffe.dataSource._destroyed[i].toJSON());
    }


    //Parte di Salvataggio Griglia dettagli Tariffe

    var gridDettagli = $("#DettagliGridTariffe").data("kendoGrid");


    var updatedRecordsDettagli = [];
    var newRecordsDettagli = [];
    var deletedRecordsDettagli = [];
    var righeNonCancellateDettagli = [];

    if (gridDettagli !== undefined && gridDettagli !== null) {
        var currentDataDettagli = gridDettagli.dataSource.data();
        for (let i = 0; i < currentDataDettagli.length; i++) {

            //Formattazione data inizio in stringa
            var originalDateInizioDettagli = currentDataDettagli[i].Validita_Inizio;
            var FormattedDataInizioDettagli = formattedReverseDate(sistemaDataInBaseAllaCulture(currentDataDettagli[i].Validita_Inizio));
            currentDataDettagli[i].Validita_Inizio = FormattedDataInizioDettagli;

            //Formattazione data fine in stringa
            var originalDateFineDettagli = currentDataDettagli[i].Validita_Fine;
            var FormattedDataFineDettagli = formattedReverseDate(sistemaDataInBaseAllaCulture(currentDataDettagli[i].Validita_Fine));
            currentDataDettagli[i].Validita_Fine = FormattedDataFineDettagli;

            righeNonCancellateDettagli.push(currentDataDettagli[i].toJSON());
            if (currentDataDettagli[i].isNew()) {
                newRecordsDettagli.push(currentDataDettagli[i].toJSON());
            }
            else if (currentDataDettagli[i].dirty) {
                updatedRecordsDettagli.push(currentDataDettagli[i].toJSON());
            }

            currentDataDettagli[i].Validita_Inizio = originalDateInizioDettagli;
            currentDataDettagli[i].Validita_Fine = originalDateFineDettagli;
        }

        if (righeNonCancellateDettagli.length === 0 && gridDettagli.dataSource._destroyed.length !== 0) {
            MessaggioErrore_Bootstrap("Ci deve essere almeno una riga di dettaglio per ogni Tariffa", "DIV_Messaggi");
            let gridDettagli_Tariffe = $("#DettagliGridTariffe").data("kendoGrid");
            gridDettagli_Tariffe.dataSource.read();
            gridDettagli_Tariffe.refresh();
            return;
        }
        else {
            for (let i = 0; i < gridDettagli.dataSource._destroyed.length; i++) {
                deletedRecordsDettagli.push(gridDettagli.dataSource._destroyed[i].toJSON());
            }
        }
    }


    if ((newRecordsTariffe.length > 0 || updatedRecordsTariffe.length > 0 || deletedRecordsTariffe.length > 0) ||
        (newRecordsDettagli.length > 0 || updatedRecordsDettagli.length > 0 || deletedRecordsDettagli.length > 0)) {

        //Salvataggio Dettagli Griglia Tariffe
        var param = {
            piva: $(cIdPiva).val(),
            tuttelerighetariffe: kendoEscapeOggetto(righeNonCancellateTariffe),
            righeinseritetariffe: kendoEscapeOggetto(newRecordsTariffe),
            righemodificatetariffe: kendoEscapeOggetto(updatedRecordsTariffe),
            righecancellatetariffe: kendoEscapeOggetto(deletedRecordsTariffe),
            tuttelerighedettagli: kendoEscapeOggetto(righeNonCancellateDettagli),
            righeinseritedettagli: kendoEscapeOggetto(newRecordsDettagli),
            righemodificatedettagli: kendoEscapeOggetto(updatedRecordsDettagli),
            righecancellatedettagli: kendoEscapeOggetto(deletedRecordsDettagli)
        };

        let risp = "";

        ajaxAgronicaSync(indirizzohttpPaginaAnagraficheCdG + "/Salva_GridTariffeEDettagli",
            kendo.stringify(param),
            false,
            function (risposta) {
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                risp = risposta;
            }, function (risposta) {
                MessaggioErrore_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
                risp = risposta;
            });


        //Ricarico le griglie solo se il prametro due è true
        if (risp !== "" && (risp.ParametroDue === "True" || risp.ParametroDue === true)) {
            let gridTariffe = $("#tariffe_UC_griglia_tariffe").data("kendoGrid");
            gridTariffe.dataSource.read();
            gridTariffe.refresh();

            let gridDettagli = $("#DettagliGridTariffe").data("kendoGrid");
            if (gridDettagli !== undefined && gridDettagli !== null) {
                gridDettagli.dataSource.read();
                gridDettagli.refresh();
            }
        }

    }
}