function Carica_ElencoValori_ParametriQualitativi() {
    var funzioniCRUD = {
        funzioneRead: Leggi_ElencoValoriParametriQualitativi,
        funzioneSubmit: { funzione: Salva_ElencoValoriParametriQualitativi },
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"
    };

    var idModel = "IdParam";
    var campiKendoModel = {
        IdParam: { editable: true, type: "string" },
        Piva: { editable: false, type: "string", defaultValue: piva },
        Tabella_Cod: { editable: false, type: "number", defaultValue: 0 },
        Tabella_Cod_Des: { editable: true, type: "string", defaultValue: "" },
        Tabella_Par_Cod: { editable: false, type: "number", defaultValue: 0 },
        Modulo_Cod: { editable: true, type: "number" },
        Modulo_Des: { editable: true, type: "string" },
        Descrizione: { editable: true, type: "string", defaultValue: "" },
        Sigla: { editable: true, type: "string", defaultValue: "" },
        Codice_Origine: { editable: true, type: "string", defaultValue: "" },
        ChkInvisibile: { editable: false, type: "boolean", defaultValue: false },
        Valore_Min: { editable: true, type: "number", defaultValue: "" },
        Valore_Max: { editable: true, type: "number", defaultValue: "" },
        OFiltro_Veg_Cod: { editable: false, defaultValue: [] },
        OFiltro_Veg_Cod_Orig: { editable: false, defaultValue: [] },
        Specie_Des_String: { editable: true, type: "string", defaultValue: "" },
        OFiltro_Cul_Cod: { editable: false, defaultValue: [] },
        OFiltro_Cul_Cod_Orig: { editable: false, defaultValue: [] },
        Varieta_Des_String: { editable: true, type: "string", defaultValue: "" },
        OFiltro_Colore: { editable: false, defaultValue: "" },
        OFiltro_Categoria: { editable: false, defaultValue: "" },
        OFiltro_Classificazione: { editable: false, defaultValue: "" },
        OFiltro_Dicitura: { editable: false, defaultValue: "" },
        OFiltro_Caratteristica: { editable: false, defaultValue: "" },
        OFiltro_Deno: { editable: false, defaultValue: "" },
        OFiltro_Finalita: { editable: false, defaultValue: "" },
        OFiltro_Regolamento: { editable: false, defaultValue: "" },
        Codice_Generazione_Link: { editable: false, type: "number", defaultValue: 0 },
        Mat_Cod_Generazione_Link: { editable: false, type: "number", defaultValue: 0 },
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
            field: "Tabella_Cod_Des", title: TraduzioneMultiResx(resxObj, "DescrizioneSintetica", "Descrizione sintetica"), editor: ElencoValoriParametriQualitativi_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Descrizione", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), filterable: { multi: true, search: true }
        },
        {
            field: "Sigla", title: TraduzioneMultiResx(resxObj, "Sigla", "Sigla"), filterable: { multi: true, search: true }
        },
        {
            field: "Codice_Origine", title: TraduzioneMultiResx(resxObj, "CodiceOrigine", "Codice Origine"), filterable: { multi: true, search: true }
        },
        {
            field: "Valore_Min", title: TraduzioneMultiResx(resxObj, "ValoreMinimo", "Valore Minimo"), filterable: { multi: true, search: true }
        },
        {
            field: "Valore_Max", title: TraduzioneMultiResx(resxObj, "ValoreMassimo", "Valore Massimo"), filterable: { multi: true, search: true }
        },
        {
            field: "Specie_Des_String", title: TraduzioneMultiResx(resxObj, "FiltroSpecie", "Filtro Specie"), editor: FiltroSpecie_MultiSelectEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Varieta_Des_String", title: TraduzioneMultiResx(resxObj, "FiltroVarieta", "Filtro Varietà"), editor: FiltroVarieta_MultiSelectEditor, filterable: { multi: true, search: true }
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
        funzioneDaChiamareDopoDataBound: function (e) {
            var grid = e.sender;
            var rows = grid.tbody.children();

            rows.each(function (e) {
                var dataItem = grid.dataItem(this);
                if (elencoTipoParametriDisabilitati.findIndex(i => i == dataItem.Tabella_Cod) > -1) {
                    var row = $("[data-uid=" + dataItem.uid + "]");
                    var rowH = row.height();
                    var deleteButton = row.find(".btn-Cancella");
                    deleteButton.hide();
                    row.height(rowH);
                }
            })
        },
        funzioneDaChiamarePrimaDiEdit: function (e) {
            var grid = e.sender;
            var cell = grid.select();
            if (cell.index() > -1) {
                if (elencoTipoParametriDisabilitati.findIndex(i => i == e.model.Tabella_Cod) > -1) {
                    e.sender.closeCell();
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
function ElencoValoriParametriQualitativi_DropDownEditor(container, options) {
    var elencoValori = elencoElencoValoriParametriQualitativi.filter(x => x.Modulo_Generazione == options.model.Modulo_Cod);
    if (elencoValori != undefined && elencoValori != null && elencoValori.length > 0)
        creaDropDownEditor(container, "Tabella_Cod_Des", "Tabella_Cod", elencoValori, changeElencoValoriParametriQualitativi);
}
function changeElencoValoriParametriQualitativi(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Tabella_Cod = dataItem.Tabella_Cod;
    model.Tabella_Cod_Des = dataItem.Tabella_Cod_Des;
}
function FiltroSpecie_MultiSelectEditor(container) {
    $('<input name="OFiltro_Veg_Cod"/>')
        .appendTo(container)
        .kendoMultiSelect({
            autoBind: true,
            dataTextField: "Veg_Des",
            dataValueField: "Veg_Cod",
            dataSource: elencoSpecie,
            filter: "contains",
            open: function (e) {
                var listContainer = e.sender.list.closest(".k-list-container");
                listContainer.width(listContainer.width() + kendo.support.scrollbar());
            },
            change: change_FitroSpecie
        }).data("kendoMultiSelect");

    var ddl = $('input[name$="OFiltro_Veg_Cod"]').data("kendoMultiSelect");
    ddl.list.width("auto");
}
function change_FitroSpecie(e) {
    var scelte = $('input[name$="OFiltro_Veg_Cod"]').data("kendoMultiSelect").dataItems();

    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    var newValue = {
        OFiltro_Veg_Cod: scelte.map(function (e) { return e.Veg_Cod; }).toString(),
        OFiltro_Cul_Cod: model.OFiltro_Cul_Cod.toString()
    };

    var originValue = {
        OFiltro_Veg_Cod: model.OFiltro_Veg_Cod_Orig.map(function (e) { return e; }).toString(),
        OFiltro_Cul_Cod: model.OFiltro_Cul_Cod.toString()
    };

    switch (scelte.length) {
        case 0: // nessuna selezione
            model.OFiltro_Veg_Cod = [];
            model.Specie_Des_String = "";
            model.OFiltro_Cul_Cod = [];
            model.Varieta_Des_String = "";
            break;

        case 1: // selezionata una sola specie
            model.OFiltro_Veg_Cod = scelte.map(function (e) { return e.Veg_Cod; });
            model.Specie_Des_String = scelte.map(function (e) { return e.Veg_Des; }).join(",");
            break;

        default: // selezionata più di una specie
            model.OFiltro_Veg_Cod = scelte.map(function (e) { return e.Veg_Cod; });
            model.Specie_Des_String = scelte.map(function (e) { return e.Veg_Des; }).join(",");
            model.OFiltro_Cul_Cod = [];
            model.Varieta_Des_String = "";
            break;
    }
    grid.refresh();

    var dirty = (originValue.OFiltro_Veg_Cod != newValue.OFiltro_Veg_Cod);
    dirty = dirty || (originValue.OFiltro_Cul_Cod != newValue.OFiltro_Cul_Cod);
    model.dirty = dirty;
}
function FiltroVarieta_MultiSelectEditor(container) {
    $('<input name="OFiltro_Cul_Cod"/>')
        .appendTo(container)
        .kendoMultiSelect({
            autoBind: true,
            dataTextField: "Cul_Des",
            dataValueField: "Cul_Cod",
            dataSource: [],
            filter: "contains",
            open: function (e) {
                var listContainer = e.sender.list.closest(".k-list-container");
                listContainer.width(listContainer.width() + kendo.support.scrollbar());

                var varieta = [];
                var grid = $("#" + IDControllo).data("kendoGrid");
                var model = grid.dataItem(e.sender.element.closest("tr"));

                if (model.OFiltro_Veg_Cod.length == 1) {
                    varieta = RicercaVarieta(piva, model.OFiltro_Veg_Cod[0]);
                }

                var varietaDDL = $('input[name$="OFiltro_Cul_Cod"]').data("kendoMultiSelect");
                varietaDDL.setDataSource(varieta);
                varietaDDL.value(model.OFiltro_Cul_Cod);
            },
            change: change_FiltroVarieta
        }).data("kendoMultiSelect");

    var ddl = $('input[name$="OFiltro_Cul_Cod"]').data("kendoMultiSelect");
    ddl.list.width("auto");
}
function change_FiltroVarieta(e) {
    var scelte = $('input[name$="OFiltro_Cul_Cod"]').data("kendoMultiSelect").dataItems();

    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    var newValue = {
        OFiltro_Veg_Cod: model.OFiltro_Veg_Cod.toString(),
        OFiltro_Cul_Cod: scelte.map(function (e) { return e.Veg_Cod; }).toString()
    };

    var originValue = {
        OFiltro_Veg_Cod: model.OFiltro_Cul_Cod_Orig.map(function (e) { return e; }).toString(),
        OFiltro_Cul_Cod: model.OFiltro_Veg_Cod_Orig.map(function (e) { return e; }).toString()
    };

    switch (scelte.length) {
        case 0: // nessuna selezione
            model.OFiltro_Cul_Cod = [];
            model.Varieta_Des_String = "";
            break;

        case 1: // selezionata una sola specie
        default: // selezionata più di una specie
            model.OFiltro_Cul_Cod = scelte.map(function (e) { return e.Cul_Cod; });
            model.Varieta_Des_String = scelte.map(function (e) { return e.Cul_Des; }).join(",");
            break;
    }
    grid.refresh();

    var dirty = (originValue.OFiltro_Veg_Cod != newValue.OFiltro_Veg_Cod);
    dirty = dirty || (originValue.OFiltro_Cul_Cod != newValue.OFiltro_Cul_Cod);
    model.dirty = dirty;
}