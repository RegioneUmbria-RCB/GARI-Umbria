async function PopolaGrigliaMacchine(piva, richiesta_cod) {
    var IDControllo = "tab_griglia_macchine"
    var omettiAnnulla = true;

    var funzioneSubmitDaUsare = { /*funzione: SubmitGrid_Dettagli_Impianti, */flagInsert: true, flagUpdate: true /*flagDelete: true*/ };

    var dt_macchine = await RicercaMacchine(piva, richiesta_cod)

    var funzioniCRUD = {
        funzioneRead: (options) => { options.success(dt_macchine); },
        //funzioneSubmit: funzioneSubmitDaUsare,
        //UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        //UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        //omettiPulsantiSalva: false,
        //omettiPulsantiAnnulla: omettiAnnulla,
        checkBoxFunction: KendoCheck_Macchine
    };

    var idModel = "chiave";
    var campiKendoModel = null;

    campiKendoModel = {
        Selected: { editable: false, type: "Boolean" },
        chiave: { editable: false, type: "String" },
        Cod_Contatto: { editable: false, type: "String" },
        Contatto_Des: { editable: false, type: "String" },
        tipologia: { editable: false, type: "String" },
        sa_cod: { editable: false, type: "String" },
        Modello: { editable: false, type: "String" },
        macchina: { editable: false, type: "String" },
        Telaio: { editable: false, type: "String" },
        Targa: { editable: false, type: "String" },
        Codice: { editable: false, type: "String" },
        CUAA_Proprietario: { editable: false, type: "String" },
        Denominazione_Proprietario: { editable: false, type: "String" },
        TitoloPossesso: { editable: false, type: "String" },
        TitoloPossesso_Des: { editable: false, type: "String" },
        Data_Creazione: { editable: false, type: "date" },
        Utente_Creazione: { editable: false, type: "String" },
        Data_Modifica: { editable: false, type: "date" },
        Utente_Modifica: { editable: false, type: "String" },
        Validita_Inizio: { editable: false, type: "date" },
        Validita_Fine: { editable: false, type: "date" },
        Possesso: { editable: false, type: "String" },
        isTarga_Obbligatoria: { editable: false, type: "number" },
        Targa_Obbligatoria: { editable: false, type: "String" }
    };

    var styleOut = "vertical-align: middle; text-align: center;";
    var colonneKendoGrid = [
        { field: "Contatto_Des", title: TraduzioneMultiResx(gestioneCarbResx, "Contatto_Des", "Contatto"), headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, hidden: true },
        { field: "tipologia", title: TraduzioneMultiResx(gestioneCarbResx, "Tipologia", "Tipologia"), headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Modello", title: TraduzioneMultiResx(gestioneCarbResx, "Modello", "Modello"), headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "macchina", title: TraduzioneMultiResx(gestioneCarbResx, "Macchina", "Macchina"), headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Targa", title: TraduzioneMultiResx(gestioneCarbResx, "Targa", "Targa"), headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "CUAA_Proprietario", title: "CF Proprietario", headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Denominazione_Proprietario", title: "Proprietario", headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "TitoloPossesso_Des", title: "Titolo Possesso", headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Data_Creazione", title: TraduzioneMultiResx(gestioneCarbResx, "Data_Creazione", "Data Creazione"), format: "{0:dd/MM/yyyy}", headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, hidden: true },
        { field: "Utente_Creazione", title: TraduzioneMultiResx(gestioneCarbResx, "Utente_Creazione", "Utente Creazione"), headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, hidden: true },
        { field: "Data_Modifica", title: TraduzioneMultiResx(gestioneCarbResx, "Data_Modifica", "Data Modifica"), format: "{0:dd/MM/yyyy}", headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, hidden: true },
        { field: "Utente_Modifica", title: TraduzioneMultiResx(gestioneCarbResx, "Utente_Modifica", "Utente Modifica"), headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, hidden: true },
        { field: "Validita_Inizio", title: TraduzioneMultiResx(gestioneCarbResx, "Validita_Inizio", "Validità Inizio"), format: "{0:dd/MM/yyyy}", headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Validita_Fine", title: TraduzioneMultiResx(gestioneCarbResx, "Validita_Fine", "Validità Fine"), format: "{0:dd/MM/yyyy}", headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
        { field: "Targa_Obbligatoria", title: TraduzioneMultiResx(gestioneCarbResx, "Targa_Obbligatoria", "Targa Obbligatoria"), headerAttributes: { style: styleOut }, filterable: { multi: true, search: true } },
    ]

    if ($("#stato_pratica_cod").val() == Verifica_In_Corso.toString() || $("#stato_pratica_cod").val() == Verifica_Completata.toString()) {
        colonneKendoGrid.push(
            { field: "Possesso", title: TraduzioneMultiResx(gestioneCarbResx, "Aziende_Registrata_Stessa_Targa", "Aziende su cui è registrata la stessa targa"), headerAttributes: { style: styleOut }, filterable: { multi: true, search: true }, width: "300px", default: "" }
        )
    }

    var parametriPerLettura = [];
    var parametriDataSource = {};

    var parametriKendoGrid = {
        columnMenu: true,
        pageable: { pageSizes: [50] },
        //columnMenu: false
        // colonneCustomKendoGrid: colCustKendoGrid
    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: Macchine_onDataBound
        /*funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoEdit: onEditGrigliaDettagliImpianti, funzioneDaChiamareDopoDataBound: onDataBoundGrigliaDettagliImpianti, funzioneDaChiamareDopoDelete: HideTabDettagli*/
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


    if ($(cAbilitato_Macchine).val().toLowerCase() == "true")
        //Quando viene caricata effettivamente la griglia mostro il pulsante
        $("#btn_aggiungi_macchina").show()
}


function KendoCheck_Macchine(e) {
    var row = $(this).parents("tr");
    var grid = $('#tab_griglia_macchine').data("kendoGrid");
    var dataItem = grid.dataItem(row);

    if (modifica_richiesto) {
        var checked = this.checked;
        dataItem.Selected = checked;
        dataItem.dirty = true;
        rowKendoGridSelected(row, checked)
    } else {
        this.checked = !this.checked;
    }

    controllaTargaObbligatoria(row, dataItem)
}

function Macchine_onDataBound(e) {
    var rows = e.sender.tbody.children();
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        var dataItem = e.sender.dataItem(row);

        controllaTargaObbligatoria(row, dataItem)
    }
}

function controllaTargaObbligatoria(row, dataItem) {
    var grid = $('#tab_griglia_macchine').data("kendoGrid");
    var indexColumnTarga = grid.wrapper.find(".k-grid-header [data-field=" + "Targa" + "]").index();

    if (dataItem.isTarga_Obbligatoria == 1 && dataItem.Targa == "" && dataItem.Selected) {
        AddErrorClass(row, indexColumnTarga, errorCell, "Targa obbligatoria per questa tipologia di macchina");
    } else {
        RemoveErrorClass(row, indexColumnTarga, errorCell);
    }

    if ($("#stato_pratica_cod").val() == Verifica_In_Corso.toString() || $("#stato_pratica_cod").val() == Verifica_Completata.toString()) {
        //Targa obbligatoria, presente sull'anagrafica di altre imprese
        if (dataItem.isTarga_Obbligatoria == 1 && dataItem.Possesso !== "" && dataItem.Possesso != null) {
            row[0].style.backgroundColor = "#e3c668"
            row[0].style.color = '#333'
        }
    }
}
