// ************************************************* Crea il Kendo Grid *************************************************
function popolaGrigliaSchemaDocumenti(IDControllo) {
    var uteAbilitatoScrit = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    let UteAbilitatoCanc = uteAbilitatoScrit;

    var funzioniCRUD = {
        funzioneRead: SchemaDocumenti_CaricaConfigurazioniDaDB,
        funzioneSubmit: { funzione: SubmitGrid_SchemaDocumenti, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: uteAbilitatoScrit,
        UtenteAbilitatoCancellazione: uteAbilitatoScrit,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false,
    };

    var colonna_editabile = false;

    if (uteAbilitatoScrit === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    var idModel = "Id_Schema_Template";
    var campiKendoModel = CaricaCampiKendoModel(colonna_editabile)
    var colonneKendoGrid = CaricaColonneKendoGrid()

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var parametriKendoGrid = {
        pdf: false,
        excel: false,
        //editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = null;

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditSchemaDocumentiConfig, funzioneDaChiamareDopoDataBound: autoFitAllColumns, funzioneDaChiamareDopoDelete: null };

    creaKendoGrid(IDControllo,            // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,                     //funzioni js da chiamare per read, insert, update, delete
        idModel,                          // chiave riga 
        campiKendoModel,                  // campi modello
        colonneKendoGrid,                 // colonne da mostrare
        parametriPerLettura,              // parametri da passare alla lettura
        parametriDataSource,              // parametri data source { chiave - valore}
        parametriKendoGrid,               // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi,          // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate,            // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}



function autoFitAllColumns(e) {
    var grid = $("#grdSchemaDocumenti").data("kendoGrid");
    for (i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].width === undefined) {
            grid.autoFitColumn(i);
        }
    }
}

//ritorna 1 se sono impostati correttamente
function CampiObbligatoriSonoImpostati(rigaDati) {
    return rigaDati.Id_Schema_Template > 0 && (rigaDati.Ambito > -15 || rigaDati.Ambito < -14) && (rigaDati.Fase > -21 || rigaDati.Fase < -16) && rigaDati.Tipologia !== null && Tipologia != undefined && rigaDati.Nr_Documenti !== null;
}

function RigaCaricataDalServer(riga) {
    return !riga.Modificabile;
}

function CampoNonModificabile(elem) {
    return elem.hasClass("edit_onInsert");
}




function ControlloCampiObbligatoriImpostati(data, index, tipoElem) {
    let campiInvalidi = []

    if (data.Ambito == null || data.Ambito > -14 || data.Ambito < -15)
        campiInvalidi.push("Ambito");

    if (data.Fase == null || data.Fase > -16 || data.Fase < -21)
        campiInvalidi.push("Fase");

    if (data.Tipologia == null || data.Tipologia == undefined || data.Tipologia == 0)
        campiInvalidi.push("Tipologia");

    if (data.Nr_Documenti == null)
        campiInvalidi.push("Nr_Documenti");

    if (campiInvalidi.length === 0)
        return "";
    else if (campiInvalidi.length === 1)
        return tipoElem + "Index " + (index + 1) + ". Campo: " + campiInvalidi[0] + " non contiene un valore valido.";
    else
        return tipoElem + "Index " + (index + 1) + ". Campi: " + campiInvalidi.join(", ") + " non contengono un valore valido.";
}

function ImpostaCampiDefault(data) {
    if (data.Servizio_Cod == null)
        data.Servizio_Cod = 0;

    if (data.Stato_Da == null)
        data.Stato_Da = 0;

    if (data.Stato_A == null)
        data.Stato_A = 0;

    if (data.Flag_Firmato_Digit == null || data.Flag_Firmato_Digit == -1)
        data.Flag_Firmato_Digit = 0;

    if (data.Flag_Obbligatorio == null || data.Flag_Obbligatorio == -1)
        data.Flag_Obbligatorio = 0;

    if (data.Nr_Documenti == null)
        data.Nr_Documenti = 0;

    if (data.inviato == null)
        data.inviato = 0;

    if (data.datainvio == null)
        data.datainvio = new Date("1900/1/1");

    if (data.DataCreazione == null)
        data.DataCreazione = new Date("1900/1/1")

    if (data.DataModifica == null)
        data.DataModifica = new Date("1900/1/1");

    if (data.UsernameCreazione == null)
        data.UsernameCreazione = "";

    if (data.UsernameModifica == null)
        data.UsernameModifica = "";

    if (data.ValiditaInizio == null)
        data.ValiditaInizio = new Date("1900/1/1");

    if (data.ValiditaFine == null)
        data.ValiditaFine = new Date("2100/12/31");
}

function Servizi_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    //if (CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
    //    return;

    PopolaElenco_Servizi().then(
        elenco => {
            let ddl = creaDropDownEditor(container, "Servizio_Des", "Servizio_Cod", elenco, ChangeServizi);
            ddl.value(row.Servizio_Cod);
        }
    )
}

function ChangeServizi(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Servizio_Des = dataItem.Servizio_Des;
        model.Servizio_Cod = dataItem.Servizio_Cod;
        model.Stato_A_Des = "nessuno";
        model.Stato_Da_Des = "nessuno";
        model.Stato_A = 0;
        model.Stato_Da = 0;
        model.dirty = true;
    }

    grid.refresh();
}

function Stato_Da_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    //if (CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
    //    return;

    PopolaElenco_Stato_Da(row.Servizio_Cod).then(
        elenco => {
            let ddl = creaDropDownEditor(container, "Stato_Da_Des", "Stato_Da", elenco, ChangeStato_Da);
            ddl.value(row.Stato_Da);
        }
    )
}

function ChangeStato_Da(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Stato_Da_Des = dataItem.Stato_Da_Des;
        model.Stato_Da = dataItem.Stato_Da;
        model.dirty = true;
    }

    grid.refresh();
}

function Stato_A_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    //if (CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
    //    return;

    PopolaElenco_Stato_A(row.Servizio_Cod).then(
        elenco => {
            let ddl = creaDropDownEditor(container, "Stato_A_Des", "Stato_A", elenco, ChangeStato_A);
            ddl.value(row.Stato_A);
        }
    )
}

function ChangeStato_A(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Stato_A_Des = dataItem.Stato_A_Des;
        model.Stato_A = dataItem.Stato_A;
        model.dirty = true;
    }

    grid.refresh();
}

function Tipologia_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    //if (CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
    //    return;

    PopolaElenco_Tipologia().then(
        elenco => {
            let ddl = creaDropDownEditor(container, "Tipologia_Des", "Tipologia", elenco, ChangeTipologia);
            ddl.value(row.Tipologia);
        }
    )
}

function ChangeTipologia(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Tipologia_Des = dataItem.Tipologia_Des;
        model.Tipologia = dataItem.Tipologia;
        model.dirty = true;
    }

    grid.refresh();
}

function Ambito_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    //if (CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
    //    return;

    PopolaElenco_Ambito().then(
        elenco => {
            let ddl = creaDropDownEditor(container, "Ambito_Des", "Ambito", elenco, ChangeAmbito);
            ddl.value(row.Ambito);
        }
    )
}

function ChangeAmbito(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Ambito_Des = dataItem.Ambito_Des;
        model.Ambito = dataItem.Ambito;
        model.dirty = true;
    }

    grid.refresh();
}

function Fase_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    //if (CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
    //    return;

    PopolaElenco_Fase().then(
        elenco => {
            let ddl = creaDropDownEditor(container, "Fase_Des", "Fase", elenco, ChangeFase);
            ddl.value(row.Fase);
        }
    )
}

function ChangeFase(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Fase_Des = dataItem.Fase_Des;
        model.Fase = dataItem.Fase;
        model.dirty = true;
    }

    grid.refresh();
}

function Firmato_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    //if (CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
    //    return;

    PopolaElenco_Firmato().then(
        elenco => {
            let ddl = creaDropDownEditor(container, "Firmato", "Flag_Firmato_Digit", elenco, ChangeFirmato);
            ddl.value(row.Flag_Firmato_Digit);
        }
    )
}

function ChangeFirmato(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Firmato = dataItem.Firmato;
        model.Flag_Firmato_Digit = dataItem.Flag_Firmato_Digit;
        model.dirty = true;
        model.dirtyFields.Firmato = true;
        model.dirtyFields.Flag_Firmato_Digit = true;
    }

    grid.refresh();
}

function Obbligatorio_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    PopolaElenco_Obbligatorio().then(
        elenco => {
            let ddl = creaDropDownEditor(container, "Obbligatorio", "Flag_Obbligatorio", elenco, ChangeObbligatorio);
            ddl.value(row.Flag_Obbligatorio);
        }
    )
}

function ChangeObbligatorio(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#" + grdSchemaDocumenti).data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Obbligatorio = dataItem.Obbligatorio;
        model.Flag_Obbligatorio = dataItem.Flag_Obbligatorio;
        model.dirty = true;
        model.dirtyFields.Obbligatorio = true;
        model.dirtyFields.Flag_Obbligatorio = true;
    }

    grid.refresh();
}

async function SubmitGrid_SchemaDocumenti(options) {
    var grid = $("#grdSchemaDocumenti").data("kendoGrid");


    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            var errMess = ControlloCampiObbligatoriImpostati(currentData[i], i, "Nuovi elementi. ");

            if (errMess === "") {
                ImpostaCampiDefault(currentData[i]);
                newRecords.push(currentData[i].toJSON());
            }
            else {
                MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
                return;
            }
        }
        else if (currentData[i].dirty) {
            var errMess = ControlloCampiObbligatoriImpostati(currentData[i], i, "Elementi modificati. ");

            if (errMess === "") {
                ImpostaCampiDefault(currentData[i]);
                updatedRecords.push(currentData[i].toJSON());
            }
            else {
                MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
                return;
            }
        }
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        ImpostaCampiDefault(grid.dataSource._destroyed[i]);
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    // Salvataggio righe //
    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        await InviaRigheModificate(newRecords, updatedRecords, deletedRecords);

        let grid = $("#grdSchemaDocumenti").data("kendoGrid");
        grid.dataSource.read();
        grid.refresh();
    }
}


function SchemaDocumenti_MessaggioCampiUguali(elem) {
    let elemUguali = TraduciLavorazioni("elementiUguali", "Sono stati trovati due elementi uguali:");
    let id_schema = TraduciLavorazioni("Id_Schema_Template", "Template Schema");


    return elemUguali + id_schema + "=" + elem.Id_Schema_Template;
}

function SchemaDocumenti_sonoUguali(p1, p2) {
    return p1.Id_Schema_Template === p2.Id_Schema_Template;
}


function CaricaCampiKendoModel(colonna_editabile) {
    return {
        Id_Schema_Template: { editable: colonna_editabile, type: "number", defaultValue: 0 },

        Servizio_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Stato_Da_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Stato_A_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Tipologia_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Ambito_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Fase_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Firmato: { editable: colonna_editabile, type: "string", defaultValue: "No"}, 
        Obbligatorio: { editable: colonna_editabile, type: "string", defaultValue: "No"},


        Ambito: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        Servizio_Cod: { editable: false, type: "number", defaultValue: 0 },
        Stato_Da: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        Stato_A: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        Fase: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        Ordine: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        Tipologia: { editable: colonna_editabile, type: "number", defaultValue: undefined },
        Suffisso_File: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Descrizione: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Flag_Obbligatorio: { editable: colonna_editabile, type: "number", defaultValue: 0},
        Flag_Firmato_Digit: { editable: colonna_editabile, type: "number", defaultValue: 0},
        Nome_Modello: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Nr_Documenti: { editable: colonna_editabile, type: "number", defaultValue: 0 },


        inviato: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        datainvio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1") },
        Data_Creazione: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1") },
        Data_Modifica: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1") },
        Username_Creazone: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Username_Modifica: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Validita_Inizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1") },
        Validita_Fine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") }
    };
}

function CaricaColonneKendoGrid() {
    return [
        {
            field: "Ambito_Des",
            title: "Ambito",
            filterable: { multi: true, search: true },
            editor: Ambito_DropDownEditor,
            attributes: { class: "edit_onInsert" }
        },
        {
            field: "Servizio_Des",
            title: "Servizio",
            filterable: { multi: true, search: true },
            editor: Servizi_DropDownEditor,
            attributes: { class: "edit_onInsert" }

        },
        {
            field: "Stato_Da_Des",
            title: "Stato Da",
            filterable: { multi: true, search: true },
            editor: Stato_Da_DropDownEditor,
            attributes: { class: "edit_onInsert" }

        },
        {
            field: "Stato_A_Des",
            title: "Stato A",
            filterable: { multi: true, search: true },
            editor: Stato_A_DropDownEditor,
            attributes: { class: "edit_onInsert" }

        },
        {
            field: "Tipologia_Des",
            title: "Tipologia",
            filterable: { multi: true, search: true },
            editor: Tipologia_DropDownEditor,
            attributes: { class: "edit_onInsert" }

        },
        {
            field: "Fase_Des",
            title: "Fase",
            filterable: { multi: true, search: true },
            editor: Fase_DropDownEditor,
            attributes: { class: "edit_onInsert" }

        },

        { field: "Ordine", title: "Ordine" },
        { field: "Suffisso_File", title: "Suffisso File" },
        { field: "Descrizione", title: "Descrizione" },
        {
            field: "Obbligatorio",
            title: "Obbligatorio",
            filterable: { multi: true, search: true },
            editor: Obbligatorio_DropDownEditor,
            attributes: { class: "edit_onInsert" }

        },
        {
            field: "Firmato",
            title: "Firmato",
            filterable: { multi: true, search: true },
            editor: Firmato_DropDownEditor,
            attributes: { class: "edit_onInsert" }

        },
       // { field: "Nome_Modello", title: "Nome Modello" },
        { field: "Nr_Documenti", title: "Numero Documenti" },

        {
            field: "Validita_Inizio",
            title: "Validità Inizio",
            format: "{0:dd/MM/yyyy}",
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #'
        },
        {
            field: "Validita_Fine",
            title: "Validità Fine",
            format: "{0:dd/MM/yyyy}",
            template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Validita_Fine, "dd/MM/yyyy" ) #'
        }
    ];
}

function TraduciCampi(chiave, testoAlternativo) {
    return TraduzioneMultiResx(confSchemaDocumentiResx, chiave, testoAlternativo);
}

function onEditSchemaDocumentiConfig(e) {
    //if (CampiObbligatoriSonoImpostati(e.model) && RigaCaricataDalServer(e.model)) {
    //    if (CampoNonModificabile($(e.container[0]))) {
    //        e.sender.closeCell();
    //    }
    //}
}