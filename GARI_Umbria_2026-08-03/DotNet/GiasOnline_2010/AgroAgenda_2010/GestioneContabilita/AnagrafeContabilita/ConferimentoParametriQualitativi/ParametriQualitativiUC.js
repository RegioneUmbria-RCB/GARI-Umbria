function Carica_ParametriQualitativi() {
    var funzioniCRUD = {
        funzioneRead: Leggi_ParametriQualitativi,
        funzioneSubmit: { funzione: Salva_ParametriQualitativi },
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"
    };

    var idModel = "IdParam";
    var campiKendoModel = {
        IdParam: { editable: true, type: "string" },
        Piva: { editable: false, type: "string", defaultValue: "AAAAAAAAAAA" },
        Tabella_Cod: { editable: false, type: "number", defaultValue: 0 },
        Modulo_Cod: { editable: true, type: "number" },
        Modulo_Des: { editable: true, type: "string" },
        Tipo_Cod: { editable: true, type: "number", defaultValue: 0 },
        Tipo_Des: { editable: true, type: "string", defaultValue: "" },        
        Tabella_Des: { editable: true, type: "string", defaultValue: "" },
        Tabella_Cod_Des: { editable: true, type: "string", defaultValue: "" },
        ChkEsclusione: { editable: true, type: "number", defaultValue: 0 },
        Tipo_Generazione_Link: { editable: true, type: "number", defaultValue: 0 },
        SottoTipo_Generazione_Link: { editable: true, type: "number", defaultValue: 0 },
        Valore_Minimo: { editable: true, type: "number", defaultValue: "" },
        Valore_Maximo: { editable: true, type: "number", defaultValue: "" },
        NumDecimali_Maximo: { editable: true, type: "number", defaultValue: 0 },
        Validita_Inizio: { editable: false, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: false, type: "date", defaultValue: new Date("2100/12/31") },
        Data_Creazione: { editable: false, type: "date" },
        inviato: { editable: false, type: "number" },
        Username_Creazione: { editable: false, type: "string" }
    };

    var colonneKendoGrid = [
        {
            field: "Modulo_Des", title: TraduzioneMultiResx(resxObj, "ModuloGenerazione", "Modulo Generazione"), editor: ModuloGenerazione_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Tipo_Des", title: TraduzioneMultiResx(resxObj, "Tipo", "Tipo"), editor: TipoParametro_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Tabella_Des", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), filterable: { multi: true, search: true }
        },
        {
            field: "Tabella_Cod_Des", title: TraduzioneMultiResx(resxObj, "DescrizioneSintetica", "Descrizione sintetica"), filterable: { multi: true, search: true }
        },
        {
            field: "Valore_Minimo", title: TraduzioneMultiResx(resxObj, "ValoreMinimo", "Valore Minimo"), filterable: { multi: true, search: true }
        },
        {
            field: "Valore_Maximo", title: TraduzioneMultiResx(resxObj, "ValoreMassimo", "Valore Massimo"), filterable: { multi: true, search: true }
        },
        {
            field: "NumDecimali_Maximo", title: TraduzioneMultiResx(resxObj, "NumeroMassimoDecimali", "Numero Massimo Decimali"), filterable: { multi: true, search: true }
        }
    ];

    var parametriPerLettura = null;
    var parametriDataSource = { pagesize: 10 };
    var parametriKendoGrid = {
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 4 },
        selectable: {
            mode: "cell"
        }
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDiEdit: function (e) {
            var grid = e.sender;
            var cell = grid.select();
            if (cell.index() > -1) {
                var columnName = grid.columns[cell.index()].field;
                switch (columnName) {
                    case "Valore_Minimo":
                    case "Valore_Maximo":
                        if (isAttivaCampiMinMax.findIndex(i => i == e.model.Tipo_Cod) < 0)
                            e.sender.closeCell();
                        break;

                    case "NumDecimali_Maximo":
                        if (isAttivaCampoDecimali.findIndex(i => i == e.model.Tipo_Cod) < 0)
                            e.sender.closeCell();
                        break;

                    default:
                        break;
                }
            }
        }
    };
    var mostraRigheCancellate = true;
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
}

function ModuloGenerazione_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Modulo_Des", "Modulo_Cod", elencoModuloGenerazione, changeModuloGenerazione);
}

function changeModuloGenerazione(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Modulo_Cod = dataItem.Modulo_Cod;
    model.Modulo_Des = dataItem.Modulo_Des;
}

function TipoParametro_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Tipo_Des", "Tipo_Cod", elencoTipoParametro, changeTipoParametro);
}

function changeTipoParametro(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    
    model.Tipo_Cod = dataItem.Tipo_Cod;
    model.Tipo_Des = dataItem.Tipo_Des;

    if (isAttivaCampiMinMax.findIndex(i => i == model.Tipo_Cod) < 0) {
        model.Valore_Minimo = "";
        model.Valore_Massimo = "";
        model.NumDecimali_Massimo = "";
    }

    grid.refresh();
}