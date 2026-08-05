function PopolaCelle(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: Leggi_Celle,
        funzioneSubmit: { funzione: SubmitCelle },
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"
    };

    var idModel = "IdParam";
    var campiKendoModel = {
        IdParam: { editable: true, type: "string" },
        Piva: { editable: false, type: "string", defaultValue: piva },
        Sa_Cod: { editable: true, type: "number", defaultValue: 0 },
        Sa_Des: { editable: true, type: "string" },
        Vas_Cod: { editable: false, type: "number", defaultValue: 0 },
        Piano_Cod: { editable: true, type: "number", defaultValue: 0 },
        Piano_Des: { editable: true, type: "string", defaultValue: "" },
        Insieme_Cod: { editable: false, type: "number", defaultValue: 0 },
        Insieme_Des: { editable: true, type: "string", defaultValue: "" },
        Identificativo: { editable: true, type: "string", defaultValue: "" },
        Validita_Inizio: { editable: false, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: false, type: "date", defaultValue: new Date("2100/12/31") },
        Data_Creazione: { editable: false, type: "date" },
        inviato: { editable: false, type: "number" },
        Username_Creazione: { editable: false, type: "string" },
        deleteAll: { editable: false, type: "boolean", defaultValue: false },
    };
    var colonneKendoGrid = [
        {
            field: "Sa_Des", title: TraduzioneMultiResx(resxObj, "CentroAziendale", "Centro Aziendale"), editor: CentroDiCosto_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Piano_Des", title: TraduzioneMultiResx(resxObj, "Reparto", "Reparto"), editor: Reparto_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Insieme_Des", title: TraduzioneMultiResx(resxObj, "Cella", "Cella"), filterable: { multi: true, search: true }
        },
        {
            field: "Identificativo", title: TraduzioneMultiResx(resxObj, "Stiva", "Stiva"), filterable: { multi: true, search: true }
        }
    ];
    var parametriPerLettura = null;
    var parametriDataSource = { pagesize: 10 };
    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 4 },
        selectable: {
            mode: "cell"
        }
    };

    //var funzioniPrimaDopoEventi = { funzioneDaChiamarePrimaDelDataBinding: onDataBindingVFattVarParamQual };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDiEdit: function (e) {
            var grid = e.sender;
            var cell = grid.select();
            if (cell.index() > -1) {
                var columnName = grid.columns[cell.index()].field;
                switch (columnName) {
                    case "Sa_Des":
                        if (!e.model.isNew())
                            e.sender.closeCell();
                        break;

                    default:
                        break;
                }
            }
        },
        funzioneDaChiamareDopoDelete: function (e) {
            var grid = $("#" + IDControllo).data("kendoGrid");
            let container = document.getElementById("tab_celle");
            let id_dialog = creaNewRowDiv("id_dialog_cancella_celle");
            container.appendChild(id_dialog);
            var row = $(e.target).closest("tr");
            var dataItem = grid.dataItem(row);
            var currentData = grid.dataSource.data();
            var toDelete = currentData.filter((elem) =>
                elem.Piva == dataItem.Piva &&
                elem.Sa_Cod == dataItem.Sa_Cod &&
                elem.Piano_Cod == dataItem.Piano_Cod &&
                elem.Insieme_Cod == dataItem.Insieme_Cod);

            $("#id_dialog_cancella_celle").kendoDialog({
                title: "Conferma Cancellazione", //i18n
                closable: true,
                modal: {
                    preventScroll: true
                },
                content: ""
                    + "<ul>"
                    + "<li>Scegliendo<b>\"Solo la stiva\"</b>:<br />"
                    + "verrà cancellata la specifica stiva dalla cella mentre saranno conservate le altre eventualmente esistenti</li> "
                    + "<li>Scegliendo<b>\"Tutta la cella\"</b>:<br />"
                    + "verrà eliminata la cella, tutte le stive collegate.<br  /><br  />"
                    + "Queste cancellazioni verranno effettuate solo qualora la cella e le stive non sono mai state utilizzate", //i18n
                actions: [{
                    text: 'Solo la stiva', //i18n
                    primary: true,
                    action: function (e) {
                        if (toDelete != undefined && toDelete != null) {
                            switch (toDelete.length) {
                                case 0:
                                case 1:
                                    var messageErr = 'Non è possibile cancellare la stiva, cancellare l\'intera cella';
                                    if (messageErr != null && messageErr !== "") {
                                        MessaggioErrore_Bootstrap(messageErr, "DIV_Messaggi");
                                        erroreSubmitCelle(grid);
                                        return;
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }, {
                    text: 'Tutta la cella', //i18n
                    action: function (e) {
                        if (toDelete != undefined && toDelete != null && toDelete.length > 0) {
                            for (var i = 0; i < toDelete.length; i++) {
                                var item = toDelete[i];
                                item.deleted = true;
                                item.deleteAll = true;
                                var linkedRow = grid.tbody.find("tr[data-uid='" + item.uid + "']");
                                linkedRow.addClass("deletedKendoRow");
                                grid.dataSource._destroyed.push(item);
                            }
                        }
                    }
                }]
            });

            grid.refresh();
        }
    };



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

    var grid = $("#" + IDControllo).data("kendoGrid");
    grid.bind("cellClose", OnEditCella);
}

function OnEditCella(e) {
    var grid = $("#" + IDControllo).data("kendoGrid");
    var currentData = grid.dataSource.data();

    var toUpdate = currentData.find((elem) =>
        elem.Piva == e.model.Piva &&
        elem.Sa_Cod == e.model.Sa_Cod &&
        elem.Piano_Cod == e.model.Piano_Cod &&
        elem.Insieme_Des == e.model.Insieme_Des &&
        elem.Insieme_Cod > 0
    );

    if (toUpdate != undefined && toUpdate != null) {
        e.model.Insieme_Cod = toUpdate.Insieme_Cod;
        grid.refresh();
    } 
}

function CentroDiCosto_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Sa_Des", "Sa_Cod", elencoCentroDiCosto, changeCentroDiCosto);
}

function changeCentroDiCosto(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_celle").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Sa_Cod = dataItem.Sa_Cod;
    model.Sa_Des = dataItem.Sa_Des;
}


function Reparto_DropDownEditor(container, options) {
    var elencoRepartiValidi = [];
    var elem = options.model.Sa_Cod;
    if (elencoReparti != undefined && elencoReparti != null && elencoReparti.length > 0) {
        elencoRepartiValidi = elencoReparti.filter((x => x.Sa_Cod == elem));
    }
    creaDropDownEditor(container, "Piano_Des", "Piano_Cod", elencoRepartiValidi, changeReparto);
}

function changeReparto(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_celle").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Piano_Cod = dataItem.Piano_Cod;
    model.Piano_Des = dataItem.Piano_Des;
}