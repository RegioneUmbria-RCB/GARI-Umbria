function PopolaCausaleTrasporto(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: Leggi_CausaleTrasporto,
        funzioneSubmit: { funzione: SubmitCausaleTrasporto },
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"
    };

    var idModel = "IdParam";
    var campiKendoModel = {
        IdParam: { editable: false, type: "string" },
        Piva: { editable: false, type: "string", defaultValue: piva },
        Causale_Trasporto_Cod: { editable: false, type: "number", defaultValue: 0 },
        Causale_Trasporto_Des: { editable: true, type: "string", defaultValue: "" },
        Causale_Trasporto_Sigla: { editable: true, type: "string", defaultValue: "" },
        Tipo_Causale_Trasporto_Cod: { editable: false, type: "number", defaultValue: 0 },
        Tipo_Causale_Trasporto_Des: { editable: true, type: "string", defaultValue: ImpostaDescrizioneTipoCausaleTrasporto(0) },
        Validita_Inizio: { editable: false, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: false, type: "date", defaultValue: new Date("2100/12/31") },
        Data_Creazione: { editable: false, type: "date" },
        inviato: { editable: false, type: "number" },
        Username_Creazione: { editable: false, type: "string" }
    };
    var colonneKendoGrid = [
        {
            field: "Causale_Trasporto_Des", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), filterable: { multi: true, search: true }
        },
        {
            field: "Causale_Trasporto_Sigla", title: TraduzioneMultiResx(resxObj, "Sigla", "Sigla"), filterable: { multi: true, search: true }
        },
        {
            field: "Tipo_Causale_Trasporto_Des", title: TraduzioneMultiResx(resxObj, "TipologiaCausale", "Tipologia Causale"), editor: TipoCausaleTrasporto_DropDownEditor, filterable: { multi: true, search: true }
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
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: function (e) {
            var grid = e.sender;
            var rows = grid.tbody.children();

            rows.each(function (e) {
                var dataItem = grid.dataItem(this);
                if (dataItem.Piva == 'AAAAAAAAAAA') {
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
                if (e.model.Piva == 'AAAAAAAAAAA') {
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

function TipoCausaleTrasporto_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Tipo_Des", "Tipo_Cod", elencoTipoCausale, changeTipoCausaleTrasporto);
}

function changeTipoCausaleTrasporto(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Tipo_Causale_Trasporto_Cod = dataItem.Tipo_Cod;
    model.Tipo_Causale_Trasporto_Des = dataItem.Tipo_Des;
}

function ImpostaTipoCausaleTrasporto(value) {
    var result = elencoTipoCausale.find(e => e.Tipo_Cod == value);
    if (result == undefined || result == null) {
        result = elencoTipoCausale.find(e => e.Tipo_Cod == 0);
    }
    return result;
}

function ImpostaDescrizioneTipoCausaleTrasporto(value) {
    var item = ImpostaTipoCausaleTrasporto(value);
    return item.Tipo_Des;
}
