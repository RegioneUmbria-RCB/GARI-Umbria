function onChange_DaAnno(e) {
    
}

function onChange_AAnno(e) {
    if ($('#txtDaAnno').data("kendoNumericTextBox").value() == null) {
        return;
    }

    let DaAnno = $('#txtDaAnno').data("kendoNumericTextBox").value();
    if ($('#txtAAnno').data("kendoNumericTextBox").value() < DaAnno) {
        kendo.alert("Attenzione, verificare i parametri di ricerca.")
    }
}

function Ricerca() {
    //if (KendoMultisel("cmbAziende").value().length <= 0) {
    //    kendo.alert("Selezionare almeno un'azienda");
    //    return;
    //}
    $(cIdElencoPiva).val(KendoMultisel("cmbAziende").value());
    $(cIdDaAnno).val($('#txtDaAnno').data("kendoNumericTextBox").value());
    $(cIdAAnno).val($('#txtAAnno').data("kendoNumericTextBox").value());

    LeggiElencoDomanderrigue();
}

function ReadDati(options) {
    var data = JSON.parse($(cIdDatiElencoDomande).val());
    options.success(data);
}

function popolaGriglia(IDControllo) {
    var funzioniCRUD = {
        funzioneRead: ReadDati
    };

    var idModel = "id";
    var campiKendoModel = {
        id: { editable: false, type: "number" },
        ragionesociale: { editable: false, type: "string" },
        piva: { editable: false, type: "string" },
        pivaReale: { editable: false, type: "string" },
        anno: { editable: false, type: "number" },
        superficietotale: { editable: false, type: "number" }
    };

    var colonneKendoGrid = [
        { field: "id", title: "Id domanda", hidden: true },
        { field: "ragionesociale", title: "Ragione Sociale", filterable: { multi: true, search: true }, width: 200 },
        { field: "pivaReale", title: "Partita Iva", filterable: { multi: true, search: true }, width: 150 },
        { field: "anno", title: "Anno competenza", filterable: { multi: true, search: true }, width: 150 },
        { field: "superficietotale", title: "Sup. Tot. (Ha)", format: '{0:0.0000}', filterable: { multi: true, search: true }, width: 150 }
    ];

    colonneKendoGrid.unshift({
        command: [
            {
                template: "<span class='fa fa-info fa-2x info_elem' title='Apri scheda' style='margin: 5px; vertical-align: middle;' onclick=apriScheda(this.closest('tr'),this.closest('.k-grid'))></span>"
            }
        ], title: "Azioni", width: "20px"
    });
    

    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        editable: true,
        groupable: false,
        reorderable: false,
        columnMenu: true,
        selectable: false,
        pdf: false,
        scrollable: true,
        resizable: true,
        pageable: false,
        btnEliminaTuttiFiltri: false
    };

    var funzioniPrimaDopoEventi = {};
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

function apriScheda(tr_elem, grid_elem) {
    var dataItem = $(grid_elem).data('kendoGrid').dataItem(tr_elem);
    ApriSchedaDomanda(dataItem.id);
}