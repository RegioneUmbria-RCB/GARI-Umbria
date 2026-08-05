
function Carica_ParametriQualitativiXReferenza() {
    var funzioniCRUD = {
        funzioneRead: Leggi_ParametriQualitativiXReferenza,
        funzioneSubmit: { funzione: Salva_ParametriQualitativiXReferenza },
            UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
            UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"
        };

    var idModel = "IdParam";
    var campiKendoModel = {
        IdParam: { editable: true, type: "string" },
        Piva: { editable: true, type: "string", defaultValue: piva },
        Id_Testata: { editable: true, type: "number", defaultValue: 0 }, 
        Id_Testata_Des: { editable: true, type: "string", defaultValue: "" },
        Tipo: { editable: false, type: "number", defaultValue: 1 },
        Tabella_ID: { editable: true, type: "string", defaultValue: "" }, 
        Tabella_Key: { editable: true, type: "string", defaultValue: "" },
        Configurazione_Des: { editable: false, type: "string", defaultValue: "" },
        Ordine: { editable: true, type: "number", defaultValue: 0, validation: { min: 1, max: 100 } },
        Tabella_Key_Rif: { editable: true, type: "string", defaultValue: "" },
        Tabella_Key_Rif_Des: { editable: true, type: "string", defaultValue: "" },
        ChkReferenza: { editable: true, type: "boolean", defaultValue: false },
        ChkReferenzaDes: { editable: true, type: "string", defaultValue: "No" },
        ChkOmni_Invisibili: { editable: false, type: "boolean", defaultValue: false },
        ChkEtichetta: { editable: false, type: "boolean", defaultValue: false },
        ChkEdit: { editable: false, type: "boolean", defaultValue: true },
        ChkObbligatorio: { editable: false, type: "boolean", defaultValue: false },    
        ChkObbligatorioDes: { editable: true, type: "string", defaultValue: "No" },
        Validita_Inizio: { editable: true, type: "date", defaultValue: new Date("1900/01/01") },
        Validita_Fine: { editable: true, type: "date", defaultValue: new Date("2100/12/31") },
        Data_Creazione: { editable: false, type: "date" },
        Inviato: { editable: false, type: "number" },
        Username_Creazione: { editable: false, type: "string" }
    };

        var colonneKendoGrid = [
            {
                field: "Id_Testata_Des", title: TraduzioneMultiResx(resxObj, "GruppiReferenze", "GruppiReferenze"), editor: Testate_DropDownEditor, filterable: { multi: true, search: true }
            },
            {
                field: "Tabella_Key", title: TraduzioneMultiResx(resxObj, "Parametro", "Parametro"), editor: ParametriQualitativi_DropDownEditor, filterable: { multi: true, search: true }
            },
            {
                field: "Ordine", title: TraduzioneMultiResx(resxObj, "Ordine", "Ordine"), filterable: { multi: true, search: true }
            },
            {
                field: "Tabella_Key_Rif_Des", title: TraduzioneMultiResx(resxObj, "RiferimentoA", "Riferimento a..."), editor: RiferimentoConfigurazione_DropDownEditor, filterable: { multi: true, search: true }
            },
            {
                field: "ChkReferenzaDes", title: TraduzioneMultiResx(resxObj, "MostraValoriDaAssociareInAnagraficaProdotti", "Mostra valori da associare in anagrafica prodotti"),
                editor: ChkReferenza_CheckBoxEditor, filterable: { multi: true, search: true }
            },
            {
                field: "ChkObbligatorioDes", title: TraduzioneMultiResx(resxObj, "Obbligatorio", "Obbligatorio"),
                editor: ChkObbligatorio_CheckBoxEditor, filterable: { multi: true, search: true }
            }
    ];

        var parametriPerLettura = null;
        var parametriDataSource = { pagesize: 10 };
        var parametriKendoGrid = {
            pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 4 },
        };
        //var funzioniPrimaDopoEventi = { funzioneDaChiamarePrimaDelDataBinding: onDataBindingVFattVarParamQual };
        var funzioniPrimaDopoEventi = {};
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

function ImpostaYesNo(yesNo) {
    var item = chkYesNo.find((e) => e.YesNo_Cod == yesNo.Cod);
    yesNo.Cod = item.YesNo_Cod;
    yesNo.Des = item.YesNo_Des;
}

function Testate_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Id_Testata_Des", "Id_Testata", elencoTestate, changeTestate);
}

function changeTestate(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Id_Testata = dataItem.Id_Testata;
    model.Id_Testata_Des = dataItem.Id_Testata_Des;
}

function ParametriQualitativi_DropDownEditor(container, options) {
    var elencoParametriQualitativiValidi = [];
    var elem = options.model.Id_Testata;
    var moduloTestata = elencoTestate.find((x) => x.Id_Testata == elem);
    if (moduloTestata != undefined && moduloTestata != null)
        elencoParametriQualitativiValidi = elencoParametriQualitativi.filter((x => x.Modulo_Generazione == moduloTestata.Modulo_Generazione));
    creaDropDownEditor(container, "Tabella_Key", "Tabella_ID", elencoParametriQualitativiValidi, changeParametriQualitativi);
}

function changeParametriQualitativi(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Tabella_ID = dataItem.Tabella_ID;
    model.Tabella_Key = dataItem.Tabella_Key;

    if (model.isNew()) {
        var referenza = (chkReferenzaAttiva.findIndex((e) => e.TabellaId == dataItem.Tabella_ID) > -1);
        var yesNo = chkYesNo.find((e) => e.YesNo_Cod == referenza);
        model.ChkReferenza = yesNo.YesNo_Cod;
        model.ChkReferenzaDes = yesNo.YesNo_Des;
    }
    grid.refresh();
}

function RiferimentoConfigurazione_DropDownEditor(container, options) {
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(container.closest("tr"));
    var dataSource = grid.dataSource.data();

    elencoRiferimenti = [];
    elencoRiferimenti.push({ "Tabella_Key_Rif": "", "Tabella_Key_Rif_Des": "" });
    dataSource.filter((x) => model.Id_Testata == x.Id_Testata &&
        model.Tabella_ID != x.Tabella_ID)
        .map((e) => {
            var elem = { "Tabella_Key_Rif": "" + e.Tabella_Key, "Tabella_Key_Rif_Des": "" + e.Tabella_Key };
            var presente = elencoRiferimenti.find((x) => x.Tabella_Key_Rif == elem.Tabella_Key_Rif);
            if (presente == undefined)
                elencoRiferimenti.push(elem);
        });

    creaDropDownEditor(container, "Tabella_Key_Rif_Des", "Tabella_Key_Rif", elencoRiferimenti, changeRiferimentoConfigurazione, false);
}

function changeRiferimentoConfigurazione(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Tabella_Key_Rif = dataItem.Tabella_Key_Rif;
    model.Tabella_Key_Rif_Des = dataItem.Tabella_Key_Rif_Des;
}

function ChkReferenza_CheckBoxEditor(container, options) {
    creaDropDownEditorId(container, "YesNo_Des", "YesNo_Cod", chkYesNo, changeChkReferenza, "ChkReferenza");
}

function changeChkReferenza(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    var yesNo = chkYesNo.find((e) => e.YesNo_Cod == dataItem.YesNo_Cod);
    model.ChkReferenza = yesNo.YesNo_Cod;
    model.ChkReferenzaDes = yesNo.YesNo_Des;
}

function ChkObbligatorio_CheckBoxEditor(container, options) {
    creaDropDownEditorId(container, "YesNo_Des", "YesNo_Cod", chkYesNo, changeChkObbligatorio, "ChkObbligatorio");
}

function changeChkObbligatorio(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#" + IDControllo).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");

    var yesNo = chkYesNo.find((e) => e.YesNo_Cod == dataItem.YesNo_Cod);
    model.ChkObbligatorio = yesNo.YesNo_Cod;
    model.ChkObbligatorioDes = yesNo.YesNo_Des;
}