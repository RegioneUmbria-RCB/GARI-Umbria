function PopolaModalitaPagamento(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: Leggi_ModalitaPagamento,
        funzioneSubmit: { funzione: SubmitModalitaPagamento },
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"
    };

    var idModel = "IdParam";
    var campiKendoModel = {
        IdParam: { editable: false, type: "string" },
        Piva: { editable: false, type: "string", defaultValue: piva },
        Cau_Pagamento_Cod: { editable: false, type: "number", defaultValue: 0 },
        Cau_Pagamento_Des: { editable: true, type: "string", defaultValue: "" },
        Cau_Pagamento_Sigla: { editable: true, type: "string", defaultValue: "" },
        Giorni_Scadenza: { editable: true, type: "number", defaultValue: 0 },
        Opzione_Cod: { editable: false, type: "number", defaultValue: 0 },
        Opzione_Des: { editable: true, type: "string" },
        Risorsa_Cod: { editable: false, type: "number", defaultValue: 0 },
        Risorsa_Des: { editable: true, type: "string" },
        Tipologia_Causale_Cod: { editable: false, type: "number", defaultValue: 0 },
        Tipologia_Causale_Des: { editable: true, type: "string" },
        Riferimento: { editable: false, type: "number", defaultValue: -1 },
        Validita_Inizio: { editable: false, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: false, type: "date", defaultValue: new Date("2100/12/31") },
        Data_Creazione: { editable: false, type: "date" },
        inviato: { editable: false, type: "number" },
        Username_Creazione: { editable: false, type: "string" }
    };
    var colonneKendoGrid = [
        {
            field: "Cau_Pagamento_Des", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), filterable: { multi: true, search: true }
        },
        {
            field: "Cau_Pagamento_Sigla", title: TraduzioneMultiResx(resxObj, "Sigla", "Sigla"), filterable: { multi: true, search: true }
        },
        {
            field: "Giorni_Scadenza", title: TraduzioneMultiResx(resxObj, "GiorniScadenza", "Giorni Scadenza"), filterable: { multi: true, search: true }
        },
        {
            field: "Opzione_Des", title: TraduzioneMultiResx(resxObj, "OpzioneCausale", "Opzione Causale"), editor: OpzioneCausale_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Risorsa_Des", title: TraduzioneMultiResx(resxObj, "RisorsaCausale", "Risorsa Causale"), editor: RisorsaCausale_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Tipologia_Causale_Des", title: TraduzioneMultiResx(resxObj, "TipologiaCausale", "Tipologia Causale"), editor: TipologiaCausale_DropDownEditor, filterable: { multi: true, search: true }
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

function OpzioneCausale_DropDownEditor(container, options) {
    creaDropDownEditor(container, "opzione_des", "opzione_cod", elencoOpzioni, changeOpzioneCausale);
}

function changeOpzioneCausale(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Opzione_Cod = dataItem.opzione_cod;
    model.Opzione_Des = dataItem.opzione_des;
}

function RisorsaCausale_DropDownEditor(container, options) {
    creaDropDownEditor(container, "risorsa_des", "risorsa_cod", elencoRisorseCausale, changeRisorsaCausale);
}

function changeRisorsaCausale(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Risorsa_Cod = dataItem.risorsa_cod;
    model.Risorsa_Des = dataItem.risorsa_des;
}

function TipologiaCausale_DropDownEditor(container, options) {
    var elencoValori = [];
    for (var idx = 0; idx < elencoTipologiaCausale.length; idx++) {
        var item = elencoTipologiaCausale[idx];
        var presente = item.risorsa_cod.map((r) => r)
            .find(x => (x == 0) || (x == options.model.Risorsa_Cod));
        if (presente != undefined && presente != null)
            elencoValori.push(item);
    }
    if (elencoValori != undefined && elencoValori != null && elencoValori.length > 0)
        creaDropDownEditor(container, "tipologia_causale_des", "tipologia_causale_cod", elencoValori, changeTipologiaCausale);
}

function changeTipologiaCausale(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Tipologia_Causale_Cod = dataItem.tipologia_causale_cod;
    model.Tipologia_Causale_Des = dataItem.tipologia_causale_des;
}

function ImpostaOpzioneCausale(value) {
    var result = elencoOpzioni.find(e => e.opzione_cod == value);
    if (result == undefined || result == null) {
        result = elencoOpzioni.find(e => e.opzione_cod == 0);
    }
    return result;
}

function ImpostaRisorsaCausale(value) {
    var result = elencoRisorseCausale.find(e => e.risorsa_cod == value);
    if (result == undefined || result == null) {
        result = elencoRisorseCausale.find(e => e.risorsa_cod == 0);
    }
    return result;
}

function ImpostaTipologiaCausale(value) {
    var result = elencoTipologiaCausale.find(e => e.tipologia_causale_cod == value);
    if (result == undefined || result == null) {
        result = elencoTipologiaCausale.find(e => e.tipologia_causale_cod == 0);
    }
    return result;
}