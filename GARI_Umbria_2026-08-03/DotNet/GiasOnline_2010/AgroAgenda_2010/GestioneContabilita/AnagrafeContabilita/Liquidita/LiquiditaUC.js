function PopolaLiquidita(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: Leggi_Liquidita,
        funzioneSubmit: { funzione: SubmitLiquidita },
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"
    };

    var idModel = "IdParam";
    var campiKendoModel = {
        IdParam: { editable: false, type: "string" },
        Piva: { editable: false, type: "string", defaultValue: piva },
        Cod_Liquidita: { editable: false, type: "number", defaultValue: -1 },
        Riferimento: { editable: true, type: "string", defaultValue: "" },
        Risorsa_Cod: { editable: false, type: "number", defaultValue: 1 },
        Risorsa_Des: { editable: false, type: "string" },
        Istituto_Cod: { editable: false, type: "number", defaultValue: 0 },
        Istituto_Des: { editable: true, type: "string" },
        Nazione: { editable: true, type: "string", defaultValue: "" },
        Cifre_Controllo: { editable: true, type: "string", defaultValue: "" },
        Cin: { editable: true, type: "string", defaultValue: "" },
        Abi: { editable: true, type: "string", defaultValue: "" },
        Cab: { editable: true, type: "string", defaultValue: "" },
        Numero: { editable: true, type: "string", defaultValue: "" },
        Bic: { editable: true, type: "string", defaultValue: "" },
        Contatto_Cod: { editable: false, type: "string", defaultValue: piva },
        Default_Cod: { editable: false, type: "string", defaultValue: 0 },
        Default_Des: { editable: true, type: "string", defaultValue: ImpostaDescrizioneDefault(0) },
        Validita_Inizio: { editable: false, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: false, type: "date", defaultValue: new Date("2100/12/31") },
        inviato: { editable: false, type: "number" },
        Username_Creazione: { editable: false, type: "string" }
    };
    var colonneKendoGrid = [
        {
            field: "Istituto_Des", title: TraduzioneMultiResx(resxObj, "IstitutiCredito", "Istituti Credito"), editor: IstitutiCredito_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Nazione", title: TraduzioneMultiResx(resxObj, "Nazione", "Nazione"), filterable: { multi: true, search: true }
        },
        {
            field: "Cifre_Controllo", title: TraduzioneMultiResx(resxObj, "CifreControllo", "Cifre Controllo"), filterable: { multi: true, search: true }
        },
        {
            field: "Cin", title: TraduzioneMultiResx(resxObj, "Cin", "Cin"), filterable: { multi: true, search: true }
        },
        {
            field: "Abi", title: TraduzioneMultiResx(resxObj, "Abi", "Abi"), filterable: { multi: true, search: true }
        },
        {
            field: "Cab", title: TraduzioneMultiResx(resxObj, "Cab", "Cab"), filterable: { multi: true, search: true }
        },
        {
            field: "Numero", title: TraduzioneMultiResx(resxObj, "ContoCorrente", "Conto Corrente"), filterable: { multi: true, search: true }
        },
        {
            field: "Bic", title: TraduzioneMultiResx(resxObj, "Bic", "Bic"), filterable: { multi: true, search: true }
        },
        {
            field: "Default_Des", title: TraduzioneMultiResx(resxObj, "Default", "Default"), editor: Default_DropDownEditor, filterable: { multi: true, search: true }
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
        },
        funzioneDaChiamareDopoEdit: function (e) {
            var grid = e.sender;
            var cell = grid.select();
            if (cell.index() > -1) {
                if (e.model.isNew() && e.model.Risorsa_Cod != 1) {
                    var risorsa = ImpostaRisorsaCausale(1);
                    e.model.Risorsa_Cod = risorsa.risorsa_cod;
                    e.model.Risorsa_Des = risorsa.risorsa_des;
                    grid.refresh();
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

function IstitutiCredito_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Istituto_Des", "Istituto_Cod", elencoIstitutiCredito, changeIstitutiCredito);
}

function changeIstitutiCredito(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    model.Istituto_Cod = dataItem.Istituto_Cod;
    model.Istituto_Des = dataItem.Istituto_Des;
}

function Default_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Default_Des", "Default_Cod", chkYesNo, changeDefault);
}

function changeDefault(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    if (dataItem.YesNo_Cod == 1) {
        var currentData = grid.dataSource.data();
        var ccDefault = currentData.find(e => e.Default_Cod == 1);
        if (ccDefault != undefined || ccDefault != null) {
            var togliDefault = ImpostaDefault(0);
            ccDefault.Default_Cod = togliDefault.Default_Cod;
            ccDefault.Default_Des = togliDefault.Default_Des;
            ccDefault.dirty = true;
        }
    }
    model.Default_Cod = dataItem.Default_Cod;
    model.Default_Des = dataItem.Default_Des;
    grid.refresh();
}

function ImpostaRisorsaCausale(value) {
    var result = elencoRisorseCausale.find(e => e.risorsa_cod == value);
    if (result == undefined || result == null) {
        result = elencoRisorseCausale.find(e => e.risorsa_cod == 0);
    }
    return result;
}

function ImpostaIstitutiCredito(value) {
    var result = elencoIstitutiCredito.find(e => e.Istituto_Cod == value);
    if (result == undefined || result == null) {
        result = elencoIstitutiCredito.find(e => e.Istituto_Cod == 0);
    }
    return result;
}

function ImpostaDefault(value) {
    var result = chkYesNo.find(e => e.Default_Cod == value);
    if (result == undefined || result == null) {
        result = chkYesNo.find(e => e.Default_Cod == 0);
    }
    return result;
}

function ImpostaDescrizioneDefault(codiceDefault) {
    var item = ImpostaDefault(codiceDefault);
    return item.Default_Des;
}