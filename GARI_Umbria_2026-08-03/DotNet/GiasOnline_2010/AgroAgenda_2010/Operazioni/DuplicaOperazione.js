var KendoOperazioni_campiKendoModel;
var KendoOperazioni_colonneKendoGrid = [];


function kendo_Operazioni_onDataBoundedRighe(e) {

    kendo_AggiustaDimensioneColonne("#divKendoOperazioni");
}

function KendoOperazioni_GestioneRigheSelezionate() {
    var mostraDate = true
    var sl = kGetElementiSelezionati("#divKendoOperazioni");

    //'Se seleziono un'operazione singola oppure un multi è possibile scegliere la data intervento.
    // controllo il numero di raccoglitori presenti:
    //'se il distinct risponde con una sola riga, allora stiamo selezionando un multi e l'utente può selezionare una data
    //'se ci sono più righe significa che l'utente ha selezionato N operazioni diverse
    var raccoglitori_list = []
    sl.forEach(function (ind) {
        if (!raccoglitori_list.includes(ind.Raccoglitore_Cod))
            raccoglitori_list.push(ind.Raccoglitore_Cod)
    }, this);

    if (raccoglitori_list.length > 1 || (sl.length > 1 && raccoglitori_list.length == 1 && raccoglitori_list[0] == "0"))
        mostraDate = false


    if (sl.length == 1 || (mostraDate && sl.length > 0)) {

        $("#panelBarDatePropagate").show();
        $("#panelBarInterventi").show();

    } else {

        $("#panelBarDatePropagate").hide();
        $("#panelBarInterventi").hide();
    }
}

function kSelectAllRows_Post() {

}

function KendoOperazioni_checked(e) {

    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#divKendoOperazioni").data("kendoGrid"),
        dataItem = grid.dataItem(row);

    dataItem.Selected = checked;
    dataItem.dirty = true;

    rowKendoGridSelected(row, checked)

    KendoOperazioni_GestioneRaccoglitore_Cod(dataItem, checked)

    KendoOperazioni_GestioneRigheSelezionate();

    if ($("#divKendoImpianti").data("kendoGrid") !== undefined) {
        //Controllo le aziende solo se ho selezionato degli impianti tramite il filtro
        controlloSelezioneAziendeDiverse()
    }
}

function controlloSelezioneAziendeDiverse() {
    var operazioni = $("#divKendoOperazioni").data("kendoGrid"),
        impianti = $("#divKendoImpianti").data("kendoGrid"),
        itemsOperazioni = operazioni.dataSource.view(),
        itemsImpianti = impianti.dataSource.view(),
        countOperazioni = itemsOperazioni.length,
        countImpianti = itemsImpianti.length;

    var hide = true
    //Se vengono selezionati impianti di aziende diverse dalle operazioni, mostro un messaggio --> Non verranno copiati gli scarichi di magazzino

    itemsOperazioni.forEach(function (operazione, ind) {
        if (operazione.Selected == true) {
            itemsImpianti.forEach(function (impianto, ind) {
                if (impianto.Selected == true) {
                    if (impianto.id.split("-")[0] !== operazione.Piva) {
                        $("#MsgNoCopiaMagazzino").show()
                        hide = false
                        return true
                    }
                }
            }, this);
        }
    }, this);
    //Se arrivo qui significa che non sono stati seleziuonati impianti di aziende diverse da quella dell'operazione, nascondo l'alert
    if (hide == true) $("#MsgNoCopiaMagazzino").hide()
}


function KendoOperazioni_GestioneRaccoglitore_Cod(dataItem, checked) {
    //Se viene selezionata un'operazione che ha Raccoglitore_Cod <>0, allora seleziono/deseleziono automaticamente tutte le altre righe che hanno lo stesso raccoglitore_cod 
    if (dataItem.Raccoglitore_Cod !== 0 && dataItem.Raccoglitore_Cod !== "0") {
        SelezionaDeseleziona_conStesso_RaccoglitoreCod(checked, dataItem.Raccoglitore_Cod, dataItem.id_agenda)
    }
}

function SelezionaDeseleziona_conStesso_RaccoglitoreCod(checked, Raccoglitore_Cod, id_agenda) {
    var grid = $("#divKendoOperazioni").data("kendoGrid")
    var rows = grid.tbody.find("tr");
    var items = grid.dataSource.view();

    items.forEach(function (dataItem, ind) {
        if (dataItem.Raccoglitore_Cod === Raccoglitore_Cod && id_agenda !== dataItem.id_agenda) {

            dataItem.Selected = checked;
            dataItem.dirty = true;

            var row = $(rows[ind]);
            row.find("input[type=checkbox]").eq(0).prop("checked", checked)

            rowKendoGridSelected(row, checked)
        }
    }, this);
}

function KendoOperazioni_inizializza(divKendoOperazioni) {

    var funzioniCRUD = {
        funzioneRead: KendoOperazioni_leggi,
        checkBoxFunction: KendoOperazioni_checked
    };

    var idModel = "id_agenda";


    if (KendoOperazioni_campiKendoModel === undefined) {
        KendoOperazioni_leggi();
    }

    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        columnMenu: false,
        impostaColonneKendoGridDaCookie: false,
        toolbarCommands: [],
        excel: false,
        pdf: false,
        sortable: true,
        groupable: false,
        pageable: false,
        filterable: false
    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: kendo_Operazioni_onDataBoundedRighe,
        funzioneDaChiamareDopoSelectAllRows: kSelectAllRows_Post
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    KendoOperazioni = creaKendoGrid(divKendoOperazioni, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        KendoOperazioni_campiKendoModel, // campi modello
        KendoOperazioni_colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

//-----------------------------------------


var KendoImpianti_campiKendoModel;
var KendoImpianti_colonneKendoGrid = [];

function kendo_Impianti_onDataBoundedRighe() {
    kendo_AggiustaDimensioneColonne("#divKendoImpianti");
}

function kSelectAllRows_Impianti_Post() {
}

function KendoImpianti_GestioneRigheSelezionate() {
}

function KendoImpianti_checked(e) {
    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#divKendoImpianti").data("kendoGrid"),
        dataItem = grid.dataItem(row);

    dataItem.Selected = checked;
    dataItem.dirty = true;

    rowKendoGridSelected(row, checked);

    KendoImpianti_GestioneRigheSelezionate();

    controlloSelezioneAziendeDiverse()
}

function KendoImpianti_inizializza(divKendoImpianti) {

    var funzioniCRUD = {
        funzioneRead: KendoImpianti_leggi,
        checkBoxFunction: KendoImpianti_checked
    };

    var idModel = "kendoKey";


    if (KendoImpianti_campiKendoModel === undefined) {
        KendoImpianti_leggi();
    }

    var parametriPerLettura = [];
    var parametriDataSource = {};
    var parametriKendoGrid = {
        columnMenu: false,
        impostaColonneKendoGridDaCookie: false,
        toolbarCommands: [],
        excel: false,
        pdf: false,
        sortable: true,
        groupable: false,
        pageable: false,
        scrollable: false,
        filterable: { mode: "menu" }
    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: kendo_Impianti_onDataBoundedRighe,
        funzioneDaChiamareDopoSelectAllRows: kSelectAllRows_Impianti_Post
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];

    KendoImpianti = creaKendoGrid(divKendoImpianti, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
        KendoImpianti_campiKendoModel, // campi modello
        KendoImpianti_colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

function ApriFiltroRicercaNG() {
    var param = kendo.stringify({
        "piva": currentPiva
    });
    ajaxAgronica(indirizzohttp + "/Link_Pagina_FiltroRicercaNG",
        param,
        function (risposta) {
            apriFinestraFiltroRicercaNG(risposta.RispostaStringa)
        }, function (risposta) {
            kendo.alert(risposta.Errore)
        });
}

function apriFinestraFiltroRicercaNG(url) {
    window.addEventListener('message', chiudiFinestraFiltroRicercaNG);

    $(document.body).append('<div id="filtro_ricerca_ng"></div>');

    $('#filtro_ricerca_ng').kendoWindow({
        title: "Filtra Impianti",
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#filtro_ricerca_ng').kendoWindow('destroy');
        }
    }).data('kendoWindow').center().maximize();
}

function chiudiFinestraFiltroRicercaNG(event) {
    let kWin = $('#filtro_ricerca_ng').data("kendoWindow");
    let urlKWin = kWin.options.content.url;

    if (verificaOriginSecondaria(window, urlKWin, event) &&
        (event != null && event.data != null) && (event.data.messaggio != null) &&
        event.data.messaggio.includes("chiudiWindowGiasNG")) {

        CreaImpiantiDaChiavi(event.data.inData.chiavi)

        kWin.close();
    }
}

function CreaImpiantiDaChiavi(chiavi) {

    var param = kendo.stringify({
        "chiavi": chiavi,
        "strDtInterventi": dtInterventi
    });

    ajaxAgronica(indirizzohttp + "/CreaImpiantiDaChiavi",
        param,
        function (risposta) {
            $(id_hdKendo_Impianti).val(risposta.RispostaStringa)

            KendoImpianti_inizializza("divKendoImpianti")
            Aggiorna_hdKendo_ImpiantiSelezionaDaHidden()
        }, function (risposta) {
            kendo.alert(risposta.Errore)
        }, null, true);
}
