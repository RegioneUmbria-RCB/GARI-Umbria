function PopolaSezionali(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: Leggi_Sezionali,
        funzioneSubmit: { funzione: SubmitSezionali },
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"
    };

    var idModel = "IdParam";
    var campiKendoModel = {
        IdParam: { editable: false, type: "string" },
        Piva: { editable: false, type: "string", defaultValue: piva },
        Sezionale_Cod: { editable: false, type: "number", defaultValue: -1 },
        Sezionale_Des: { editable: true, type: "string", defaultValue: "" },
        Regime_Fiscale_Cod: { editable: false, type: "number", defaultValue: 0 },
        Regime_Fiscale_Des: { editable: true, type: "string", defaultValue: ImpostaDescrizioneRegimeFiscale(0) },
        Esigibilita_Iva_Cod: { editable: false, type: "number", defaultValue: 0 },
        Esigibilita_Iva_Des: { editable: true, type: "string", defaultValue: ImpostaDescrizioneEsigibilitaIva(0) },
        Fatturazione_Elettronica_Cod: { editable: false, type: "number", defaultValue: 1 },
        Fatturazione_Elettronica_Des: { editable: true, type: "string", defaultValue: ImpostaDescrizioneFatturazione(1) },
        inviato: { editable: false, type: "number" },
        Data_Creazione: { editable: false, type: "date" },
        Data_Modifica: { editable: false, type: "date" },
        Username_Creazione: { editable: false, type: "string" },
        Username_Modifica: { editable: false, type: "string" },
        Validita_Inizio: { editable: true, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: true, type: "date", defaultValue: new Date("2100/12/31") },
        RegimeIva: { editable: false, type: "number", defaultValue: 0 },
        LiquidazioneIva: { editable: false, type: "number", defaultValue: 0 },
        InteresseDebitoIva_Perc: { editable: false, type: "number", defaultValue: 0 },
        ChkPrefissoSuffisso: { editable: false, type: "number", defaultValue: 0 },
        Prefisso: { editable: false, type: "string", defaultValue: "" },
        Suffisso: { editable: false, type: "string", defaultValue: "" },
    };
    var colonneKendoGrid = [
        {
            field: "Sezionale_Des", title: TraduzioneMultiResx(resxObj, "Descrizione", "Descrizione"), filterable: { multi: true, search: true }
        },
        {
            field: "Regime_Fiscale_Des", title: TraduzioneMultiResx(resxObj, "RegimeFiscale", "Regime Fiscale"), editor: RegimeFiscale_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Esigibilita_Iva_Des", title: TraduzioneMultiResx(resxObj, "EsigibilitaIva", "Esigibilità Iva"), editor: EsigibilitaIva_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Fatturazione_Elettronica_Des", title: TraduzioneMultiResx(resxObj, "FatturazioneElettronica", "Fatturazione Elettronica"), editor: FatturazioneElettronica_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Validita_Inizio", title: "Validità inizio", format: "{0:dd/MM/yyyy}", attributes: {
                style: "text-align: center;"
            }, headerAttributes: {
                style: "text-align: center;"
            }
        },
        {
            field: "Validita_Fine", title: "Validità fine", format: "{0:dd/MM/yyyy}", attributes: {
                style: "text-align: center;"
            }, headerAttributes: {
                style: "text-align: center;"
            }
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
                if (dataItem.Sezionale_Cod == 0) {
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
                if (e.model.Sezionale_Cod == 0) {
                    var columnName = grid.columns[cell.index()].field;
                    switch (columnName) {
                        case "Sezionale_Des":
                        case "Validita_Inizio":
                        case "Validita_Fine":
                            e.sender.closeCell();
                            break;

                        default:
                            break;
                    }
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

function RegimeFiscale_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Descrizione", "RegimeFiscale_Cod", elencoRegimiFiscale, changeRegimeFiscale);
}

function changeRegimeFiscale(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Regime_Fiscale_Cod = dataItem.RegimeFiscale_Cod;
    model.Regime_Fiscale_Des = dataItem.Descrizione;
}

function EsigibilitaIva_DropDownEditor(container, options) {
    creaDropDownEditor(container, "EsigibilitaIva_des", "EsigibilitaIva_cod", elencoEsigibilitaIva, changeEsigibilitaIva);
}

function changeEsigibilitaIva(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Esigibilita_Iva_Cod = dataItem.EsigibilitaIva_cod;
    model.Esigibilita_Iva_Des = dataItem.EsigibilitaIva_des;
}

function FatturazioneElettronica_DropDownEditor(container, options) {
    creaDropDownEditor(container, "FatturazioneElettronica_Des", "FatturazioneElettronica_Cod", chkYesNo, changeFatturazioneElettronica);
}

function changeFatturazioneElettronica(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Fatturazione_Elettronica_Cod = dataItem.FatturazioneElettronica_Cod;
    model.Fatturazione_Elettronica_Des = dataItem.FatturazioneElettronica_Des;
}

function ImpostaValoreDefault(codice, defaultValue) {
    if (codice != undefined && codice != null)
        return codice;
    else
        return defaultValue;
}

function ImpostaDescrizioneRegimeFiscale(codiceRegimeFiscale) {
    var result = "";
    if (codiceRegimeFiscale != undefined && codiceRegimeFiscale != null) {
        var desc = elencoRegimiFiscale.find(e => e.RegimeFiscale_Cod == codiceRegimeFiscale);
        if (desc !== undefined && desc !== null)
            result = desc.Descrizione;
    }
    return result;
}

function ImpostaDescrizioneEsigibilitaIva(codiceEsigibilitaIva) {
    var result = "";
    if (codiceEsigibilitaIva != undefined && codiceEsigibilitaIva != null) {
        var desc = elencoEsigibilitaIva.find(e => e.EsigibilitaIva_cod == codiceEsigibilitaIva);
        if (desc !== undefined && desc !== null)
            result = desc.EsigibilitaIva_des;
    }
    return result;
}

function ImpostaDescrizioneFatturazione(codiceFatturazione) {
    var result = "";
    if (codiceFatturazione != undefined && codiceFatturazione != null) {
        var desc = chkYesNo.find(e => e.FatturazioneElettronica_Cod == codiceFatturazione);
        if (desc !== undefined && desc !== null)
            result = desc.FatturazioneElettronica_Des;
    }
    return result;
}