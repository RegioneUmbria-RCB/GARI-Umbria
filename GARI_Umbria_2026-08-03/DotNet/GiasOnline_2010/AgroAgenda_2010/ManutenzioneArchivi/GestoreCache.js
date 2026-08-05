var UtenteAbilitatoScrittura = true;

function popolaGrigliaCache() {
    
    var funzioniCRUD = {
        funzioneRead: RicercaElementiCache
        // checkBoxFunction: SelezionaMVV
    };

    var idModel = "";
    var campiKendoModel = {
        Chiave: {type: "string" },
        NomeGruppoCache: { type: "string" },
        NomeServer: { type: "string" },
        NomeClasse: { type: "string" },
        NomeMetodo: { type: "string" },
        StringaSql: { type: "string" },
        Valore: { type: "string" },
        NumeroRisultati: { type: "number" },
        ValoreJson: { type: "string" }
    };
    var colonneKendoGrid = [
        { field: "Chiave", title: "Chiave", hidden: true },
        { field: "NomeGruppoCache", title: "Tipo Cache", filterable: { multi: true, search: true }, width: 200 },
        { field: "NomeServer", title: "Database", filterable: { multi: true, search: true }, width: 150 },
        { field: "NomeClasse", title: "Classe", filterable: { multi: true, search: true }, width: 150 },
        { field: "NomeMetodo", title: "Metodo", filterable: { multi: true, search: true }, width: 150 },
        // { field: "Destinatario_Diverso_Rag_Soc", title: "Intestatario Diverso Rag.Soc.", filterable: { multi: true, search: true }, hidden: true, width:150 },
        { field: "StringaSql", title: "Stringa SQL", widht: 200 },
        { field: "Valore", title: "Valore", editor: textAreaEditor, width: 2000 },
        { field: "NumeroRisultati", title: "Righe", width: 150 },
        { field: "ValoreJson", title: "Valore JSON", editor: textAreaEditor, width: 2000}
    ];

    var parametriPerLettura = null;
    var parametriDataSource = {
        //sort: [{ field: "Documento_DataDocumento", dir: "desc" }]
    };

    var colCustKendoGrid = [{
        command: [
            {
                text: "&nbsp;",
                name: "Pulisci",
                iconClass: "fa fa-eraser",
                click: EliminaElemento
            },
            {
                iconClass: "fa fa-external-link-square",
                name: "Visualizza",
                text: "&nbsp;",
                click: Visualizza
                //visible: function (dataItem) { return dataItem.Gias_Status != null; }
            }
        ],
        title: "Azioni", width: "120px"
    }];

    var parametriKendoGrid = {
        colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        // toolbarCommands: ["template_consultaMVV"],
        editable: false,
        reorderable: true,
        columnMenu: true,
        selectable: false,
        pdf: false,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 }
        // checkSelezioneRiga: { filterable: false, field: null, width: "30px" }
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: DataBoundCache };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(
        "gridCache", // rappresenta l'ID del div a cui si associa la griglia
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

    // tooltip comandi
    $("#gridCache").kendoTooltip({ filter: ".k-grid-Pulisci", content: "Rimuovi elemento dalla Cache" });
    $("#gridCache").kendoTooltip({ filter: ".k-grid-Visualizza", content: "Visualizza contenuto" });
} 

function textAreaEditor(container, options) {
    $('<textarea class="k-textbox" name="' + options.field + '" style="width:2000px;height:100px;" />').appendTo(container);
}


function aggiornaGrigliaMVVE() {
    var grid = $("#gridMVVE").getKendoGrid();
    grid.dataSource.read();
    grid.refresh();
}

function DataBoundCache(e) {
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    for (var i = 0; i < grid.columns.length; i++) {
        grid.autoFitColumn(i);
    }

    //var rows = e.sender.tbody.children();
    //for (var j = 0; j < rows.length; j++) {
    //    var row = $(rows[j]);
    //    var dataItem = e.sender.dataItem(row);
    //    if (dataItem.get("MVV_Status") == "3") {
    //        row.addClass("rowKendoOk");
    //    } else if (dataItem.get("MVV_Status") == "4" || dataItem.get("MVV_Status") == "5") {
    //        row.addClass("rowKendoWarning");
    //    } else if (dataItem.get("Gias_Status") == "403") {
    //        row.addClass("rowKendoCritical");
    //    }
    //}
}

function Conferma(messaggio, azione) {
    var kendoConfirm = $("<div></div>").kendoConfirm({
        title: "MVV Elettronico",
        messages: { okText: "Sì", cancel: "No" },
        content: messaggio
    }).data("kendoConfirm");
    kendoConfirm.result.done(azione);
    kendoConfirm.open();
}

function EliminaElemento(e)
{
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    pulisciElemento(dataItem.Chiave, dataItem.NomeGruppoCache);
}

function Visualizza(e) {

}

/*
function Consulta(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    windowMVVE.content(templateMVVE(dataItem));
    windowMVVE.open();
    LogMVV(dataItem.PIVA,dataItem.Id_Agenda);
}

function Stampa(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    StampaMVV(dataItem);
}

function Invia(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    Conferma("Vuoi trasmettere i dati MVV in formato elettronico?", function () { InviaMVV(dataItem); });
}

function Annulla(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    Conferma("Vuoi annullare l'MVV-E?", function () { AnnullaMVV(dataItem); });
}

function Scarica(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    ScaricaMVV(dataItem);
}

function SelezionaMVV(e) {

    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = $("#gridMVVE").data("kendoGrid");
    var dataItem = grid.dataItem(row);
    dataItem.Selected = checked;

    rowKendoGridSelected(row, checked)
}
*/

function dialogErrore(msgErrore) {
    var alert = $("<div style='display:inline-block; text-align:center;'></div>").kendoAlert({
        width: 400,
        title: "MVV Elettronico",
        content: msgErrore
    }).data("kendoAlert");
    alert.open();
}