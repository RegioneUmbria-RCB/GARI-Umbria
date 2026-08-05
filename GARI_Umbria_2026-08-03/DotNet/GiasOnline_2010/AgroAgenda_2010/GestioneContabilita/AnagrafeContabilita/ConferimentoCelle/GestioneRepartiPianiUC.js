function PopolaRepartiPiani(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: Leggi_RepartiPiani,
        funzioneSubmit: { funzione: SubmitRepartiPiani },
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"
    };

    var idModel = "IdParam";
    var campiKendoModel = {
        IdParam: { editable: true, type: "string" },
        Piva: { editable: false, type: "string", defaultValue: piva },
        Sa_Cod: { editable: true, type: "number", defaultValue: 0 },
        Sa_Des: { editable: true, type: "string" },
        Piano_Cod: { editable: true, type: "number", defaultValue: 0 },
        Piano_Des: { editable: true, type: "string", defaultValue: "" },
        DimX: { editable: true, type: "number", defaultValue: 0 },
        DimY: { editable: true, type: "number", defaultValue: 0 },
        Colore_Interno: { editable: true, type: "number", defaultValue: 0 },
        Colore_Interno_Des: { editable: true, type: "string", defaultValue: "" },
        Colore_Esterno: { editable: true, type: "number", defaultValue: 0 },
        Colore_Esterno_Des: { editable: true, type: "string", defaultValue: "" },
        Spessore: { editable: true, type: "number", defaultValue: 0 },
        Riempimento: { editable: true, type: "number", defaultValue: 0 },
        Zoom: { editable: true, type: "number", defaultValue: 0 },
        Validita_Inizio: { editable: false, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: false, type: "date", defaultValue: new Date("2100/12/31") },
        Data_Creazione: { editable: false, type: "date" },
        inviato: { editable: false, type: "number" },
        Username_Creazione: { editable: false, type: "string" }
    };
    var colonneKendoGrid = [
        {
            field: "Sa_Des", title: TraduzioneMultiResx(resxObj, "CentroAziendale", "Centro Aziendale"), editor: CentroAziendale_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Piano_Des", title: TraduzioneMultiResx(resxObj, "Reparto", "Reparto"), filterable: { multi: true, search: true }
        }/*,
        {
            field: "DimX", title: TraduzioneMultiResx(resxObj, "DimX", "DimX"), filterable: { multi: true, search: true }
        },
        {
            field: "DimY", title: TraduzioneMultiResx(resxObj, "DimY", "DimY"), filterable: { multi: true, search: true }
        },
        {
            field: "Colore_Interno_Des", title: TraduzioneMultiResx(resxObj, "ColoreInterno", "ColoreInterno"), editor: CentroAziendale_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Colore_Esterno_Des", title: TraduzioneMultiResx(resxObj, "ColoreEsterno", "ColoreEsterno"), editor: CentroAziendale_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Spessore", title: TraduzioneMultiResx(resxObj, "Spessore", "Spessore"), filterable: { multi: true, search: true }
        },
        {
            field: "Riempimento", title: TraduzioneMultiResx(resxObj, "Riempimento", "Riempimento"), filterable: { multi: true, search: true }
        },
        {
            field: "Zoom", title: TraduzioneMultiResx(resxObj, "Zoom", "Zoom"), filterable: { multi: true, search: true }
        }*/
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
                        e.sender.closeCell();
                        break;

                    default:
                        break;
                }
            }
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

}

function CentroAziendale_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Sa_Des", "Sa_Cod", elencoCentriAziendali, changeCentroAziendale);
}

function changeCentroAziendale(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_celle_reparti_piani").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Sa_Cod = dataItem.Sa_Cod;
    model.Sa_Des = dataItem.Sa_Des;
}