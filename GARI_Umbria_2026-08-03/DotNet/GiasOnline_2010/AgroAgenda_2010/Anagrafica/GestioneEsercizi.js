function SelezionaEsercizi(options) {
    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = $("#tab_esercizi").data("kendoGrid");
    var dataItem = grid.dataItem(row);

    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
}

function eseguiAzioneEsercizi(IDControllo) {
    var azione = Get_KendoDDLValue("ddlAzioneEsercizio");
    var dataChiusura = $('input[name$="txtDataChiusura"]').val();
    if (dataChiusura == "" && (azione == "0" || azione == "-1")) {
        kendo.alert(TraduzioneMultiResx(gestioneEserciziResx, "ImpostareDataChiusuraEsercizio", "Impostare la data chiusura dell'esercizio"));
    } else {
        var errore = "";
        var grid = $(IDControllo).data("kendoGrid");
        var esercizi = [];
        var items = grid.dataSource.data().filter(function (dataitem) { return dataitem.Selected == true });
        items.forEach(function(item) {
            var chiave = item.chiave.split("_");
            var data_chiusura = $("#txtDataChiusura").data("kendoDatePicker").value();
            if (azione == "0" && item.Validita_Inizio > data_chiusura) {
                errore = TraduzioneMultiResx(gestioneEserciziResx, "DataChiusuraPrecedenteDataInizioValiditàEsercizio",
                    "La data chiusura non può precedere la data inizio validità dell'esercizio");
            } else if (azione == "0" && item.Validita_Fine_Impianto < data_chiusura) {
                errore = TraduzioneMultiResx(gestioneEserciziResx, "DataChiusuraSuccessivaDataFineValiditàImpianto",
                    "La data chiusura esercizio non può essere successiva alla data fine validità dell'impianto");
            } else if (data_chiusura == null && item.Validita_Fine >= new Date(2100, 11, 31)) {
                errore = TraduzioneMultiResx(gestioneEserciziResx, "DataChiusuraEsercizioNonImpostata",
                    "La data chiusura esercizio deve essere impostata");
            }
            if (errore == "") {
                var data_inizio = kendo.toString(item.Validita_Inizio, 'd');
                var data_fine = kendo.toString(item.Validita_Fine, 'd');
                esercizi.push({ piva: chiave[0], sa_cod: chiave[1], appezza: chiave[2], id_reg: chiave[3], progetto_cod: chiave[4], validita_inizio: data_inizio, validita_fine: data_fine });
            }
        });
        if (errore == "" && esercizi.length == 0) errore = TraduzioneMultiResx(gestioneEserciziResx, "SelezionareAlmenoUnEsercizio", "Selezionare almeno un esercizio");
        if (errore != "") kendo.alert(errore); else EseguiAzioneEsercizi(esercizi);
    }    
}

function aggiornaGrigliaEsercizi(IDControllo) {
    var grid = $("#" + IDControllo).getKendoGrid();
    grid.setDataSource(grid.dataSource);
    grid.dataSource.read();
}

function popolaGrigliaEsercizi(IDControllo) {

    $.logThis("GestioneEsercizi.popolaGrigliaEsercizi");

    var funzioniCRUD = {
        funzioneRead: LeggiEsercizi,
        checkBoxFunction: SelezionaEsercizi
    };

    var idModel = "chiave";
    var campiKendoModel = {
        chiave: { type: "string" },
        rag_soc: { type: "string" },
        Sa_Nome: { type: "string" },
        Campo_Des: { type: "string" },
        app_nome: { type: "string" },
        Codice_Impianto: { type: "string" },
        utilizzo: { type: "string" },
        gru_des: { type: "string" },
        grfi_des: { type: "string" },
        sup_imp: { type: "number" },
        Progetto_Cod: { type: "string" },
        Progetto_Nome: { type: "string" },
        Progetto_Des: { type: "string" },
        Distinta_Chiusa: { type: "string" },
        Validita_Inizio: { type: "date" },
        Validita_Fine: { type: "date" },
        Validita_Inizio_Impianto: { type: "date" },
        Validita_Fine_Impianto: { type: "date" },
        Validita_Inizio_Appezzamento: { type: "date" },
        Validita_Fine_Appezzamento: { type: "date" },
        Durata: { type: "number" }
    };
    var colonneKendoGrid = [
        { field: "rag_soc", title: TraduzioneMultiResx(gestioneEserciziResx, "Azienda", "Azienda"), filterable: { multi: true, search: true }, hidden: true },
        { field: "Sa_Nome", title: TraduzioneMultiResx(gestioneEserciziResx, "Centro", "Centro"), filterable: { multi: true, search: true } },
        { field: "Campo_Des", title: TraduzioneMultiResx(gestioneEserciziResx, "Campo", "Campo"), filterable: { multi: true, search: true } },
        { field: "app_nome", title: TraduzioneMultiResx(gestioneEserciziResx, "NomeAppezzamento", "Nome Appezzamento"), filterable: { multi: true, search: true } },
        { field: "Codice_Impianto", title: TraduzioneMultiResx(gestioneEserciziResx, "CodiceImpianto", "Codice Impianto"), filterable: { multi: true, search: true }, hidden: true },
        { field: "utilizzo", title: TraduzioneMultiResx(gestioneEserciziResx, "Utilizzo", "Utilizzo"), filterable: { multi: true, search: true } },
        { field: "gru_des", title: TraduzioneMultiResx(gestioneEserciziResx, "GruppoVegetale", "Gruppo Vegetale"), filterable: { multi: true, search: true } },
        { field: "grfi_des", title: TraduzioneMultiResx(gestioneEserciziResx, "Finalità", "Finalità"), filterable: { multi: true, search: true }, hidden: true },
        { field: "sup_imp", title: TraduzioneMultiResx(gestioneEserciziResx, "SuperficieImpiantoAbbr", "Sup. Impianto") + " [Ha]", format: "{0:n4}" },
        { field: "Progetto_Nome", title: TraduzioneMultiResx(gestioneEserciziResx, "Lotto", "Lotto"), filterable: { multi: true, search: true } },
        { field: "Progetto_Des", title: TraduzioneMultiResx(gestioneEserciziResx, "Descrizione", "Descrizione"), filterable: { multi: true, search: true } },
        { field: "Distinta_Chiusa", title: TraduzioneMultiResx(gestioneEserciziResx, "Chiuso", "Chiuso"), filterable: { multi: true, search: true }, hidden: true },
        { field: "Validita_Inizio", title: TraduzioneMultiResx(gestioneEserciziResx, "ValiditàEsercizioDal", "Validità Esercizio dal"), format: "{0:dd/MM/yyyy}" },
        { field: "Validita_Fine", title: TraduzioneMultiResx(gestioneEserciziResx, "ValiditàEsercizioAl", "Validità Esercizio al"), format: "{0:dd/MM/yyyy}" },
        { field: "Validita_Inizio_Impianto", title: TraduzioneMultiResx(gestioneEserciziResx, "ValiditàImpiantoDal", "Validità Impianto dal"), format: "{0:dd/MM/yyyy}", hidden: true },
        { field: "Validita_Fine_Impianto", title: TraduzioneMultiResx(gestioneEserciziResx, "ValiditàImpiantoAl", "Validità Impianto al"), format: "{0:dd/MM/yyyy}", hidden: true },
        { field: "Validita_Inizio_Appezzamento", title: TraduzioneMultiResx(gestioneEserciziResx, "ValiditàAppezzamentoDal", "Validità Appezzamento dal"), format: "{0:dd/MM/yyyy}", hidden: true },
        { field: "Validita_Fine_Appezzamento", title: TraduzioneMultiResx(gestioneEserciziResx, "ValiditàAppezzamentoAl", "Validità Appezzamento al"), format: "{0:dd/MM/yyyy}", hidden: true }
        // { field: "Durata", title: "Durata (giorni)", format: "{0:n0}", hidden: true }
    ];
    var parametriPerLettura = null;
    var parametriDataSource = {};
    /* parametriDataSource.filter = {
            logic: "and", filters: [
                { field: "gru_des", operator: "eq", value: "Arboree" },
                { field: "Durata", operator: "gt", value: 365 }
            ]
        };
    */
        
    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        editable: false,
        groupable: true,
        reorderable: true,
        columnMenu: true,
        selectable: false,
        pdf: false,
        scrollable: false,
        search: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3, refresh: true },
        checkSelezioneRiga: { filterable: false, field: null, width: "30px" }
    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: function (e) {
            // var grid = $("#" + IDControllo).data('kendoGrid');
            var gridId = e.sender.element[0].id;
            var grid = $("#" + gridId).data("kendoGrid");
            for (var i = 0; i < grid.columns.length; i++) {
                if (grid.columns[i].width === undefined) {
                    grid.autoFitColumn(i);
                }
            }
            grid.tbody.find("tr[role='row']").each(function () {
                var model = grid.dataItem(this);
                var oggi = new Date().setHours(0, 0, 0, 0);
                if (model.Validita_Inizio > oggi || model.Validita_Fine < oggi) {
                    $(this).addClass("DimGray");
                }
            });
        }
    };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

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

function popolaAzioneEsercizio(options) {
    
    var azioni = [
        { "Azione_Cod": -1, "Azione_Des": TraduzioneMultiResx(gestioneEserciziResx, "ChiusuraImpianto", "Chiusura Impianto") },
        { "Azione_Cod": 0, "Azione_Des": TraduzioneMultiResx(gestioneEserciziResx, "ChiusuraEsercizio", "Chiusura Esercizio") }];
    for (i = 1; i <= 20; i++) {
        azioni.push({ "Azione_Cod": i, "Azione_Des": kendo.format(TraduzioneMultiResx(gestioneEserciziResx, "ChiusuraEsercizioEdAperturaEnnesimaAnnualità", "Chiusura Esercizio ed Apertura {0} annualità"), i) });
    }
    options.success(azioni);
}

function Aggiorna_Griglia_Esercizi() {
    var grid = $("#tab_esercizi").data("kendoGrid");
    grid.dataSource.read();
    grid.refresh();
}