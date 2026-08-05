var SupTot = 0.0;

function CaricaDatiVideata() {
    let data = JSON.parse($(cIdDatiDomanda).val());

    $("#txtRagSoc").val(data.datiAzienda.RagioneSociale);
    $("#txtPIVA").val(data.piva);
    $("#txtCFCUAA").val(data.datiAzienda.CUAA);
    $("#txtVia").val(data.datiAzienda.indirizzo.Via);
    $("#txtFrazione").val(data.datiAzienda.indirizzo.Frazione);
    $("#txtProvincia").val(data.datiAzienda.indirizzo.Provincia);
    $("#txtComune").val(data.datiAzienda.indirizzo.Comune);
    $("#txtCAP").val(data.datiAzienda.indirizzo.CAP);
    $("#txtStato").val(data.datiAzienda.indirizzo.Stato);
    $("#txtAnno").val(kendo.parseDate(data.ValiditaInizio, 'yyyy-MM-ddTHH:mm').getFullYear());
    $("#txtID").val(data.id);
    $("#groupID").hide();
    //$("#txtTelefono").val(data.datiAzienda.RagioneSociale);

    popolaGrigliaDettaglio("divKendoOut");
    if (data.dettaglio.length <= 0) {
        kendo.alert("Attenzione nessun appezzamento valido nell'anno " + $("#txtAnno").val());
    }
}

function PopolaTestata(datiTestata) {

}

function ReadDettaglioDomanda(options) {
    var data = JSON.parse($(cIdDatiDomanda).val());
    options.success(data.dettaglio);
}

function kEventoSelezionaRiga(e) {
    let checked = this.checked,
        row = $(this).parents("tr").eq(0),
        kGrid = $("#divKendoOut").data("kendoGrid"),
        dataItem = kGrid.dataItem(row);

    if ($(cIdEnableMod).val() === "True") {

        dataItem.Selected = checked; // dataItem è per riferimento, di conseguenza viene aggiornato il dataSource della griglia
        if (checked) {
            row.addClass(GIAS_K_STATE_SELECTED);
            dataItem.Selezionato = true;
            RicalcolaSuperficie("+", dataItem.Superficie);
        } else {
            row.remove(GIAS_K_STATE_SELECTED);
            dataItem.Selezionato = false;
            RicalcolaSuperficie("-", dataItem.Superficie);
        }
        dataItem.dirty = true;
        kGrid.refresh();
        CurrentMod = true;
    } else {
        this.checked = !checked;
        kendo.alert("Modifica non permessa. Utente non abilitato o periodo di inserimento domanda irrigua chiuso. ");
    }
}

function RicalcolaSuperficie(sign, value) {
    if (sign === "+") {
        SupTot = SupTot + value;
    } else {
        SupTot = SupTot - value;
    }
    $("#txtSupTotale").val(SupTot);
}

function RicalcolaSuperficieTotale() {
    var datiGriglia = $("#divKendoOut").data('kendoGrid');
    var righe = datiGriglia.dataSource.data();

    let SupTotCurr = 0.0;

    for (var i = 0; i < righe.length; i++) {
        if (righe[i].Selezionato === true) {
            SupTotCurr = SupTotCurr + righe[i].Superficie;
        }
    }

    $("#txtSupTotale").val(parseFloat(SupTotCurr).toFixed(4));
    SupTot = SupTotCurr;
}

function onDataBoundRighe(e) {
    selezionaParticelleImpiegate();
}

function popolaGrigliaDettaglio(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: ReadDettaglioDomanda,
        checkBoxFunction: kEventoSelezionaRiga
    };

    var idModel = "riga";
    var campiKendoModel = {
        Selezionato: { editable: true , type: "boolean"},
        app_nome: { editable: false, type: "string" },
        PROV: { editable: false, type: "string" },
        PROV_Des: { editable: false, type: "string" },
        COM: { editable: false, type: "string" },
        COM_Des: { editable: false, type: "string" },
        Sezione: { editable: false, type: "string" },
        Foglio: { editable: false, type: "string" },
        Numero: { editable: false, type: "string" },
        Superficie: { editable: false, type: "number" },
        veg_des: { editable: false, type: "string" }
    };
    var colonneKendoGrid = [
        //{ field: "Selezionato", title: "", template: '<input type="checkbox" #= Selezionato ? \'checked="checked"\' : "" # class="k-checkbox"/>',filterable: { multi: true, search: true }, width: 30 },
        { field: "app_nome", title: "Nome App.", hidden: false, width: 30, filterable: { multi: true, search: true } },
        { field: "PROV", title: "PROV", hidden: false, width: 30, filterable: { multi: true, search: true } },
        { field: "PROV_Des", title: "Provincia", hidden: false, width: 30, filterable: { multi: true, search: true } },
        { field: "COM", title: "COM", hidden: false, width: 30, filterable: { multi: true, search: true } },
        { field: "COM_Des", title: "Comune", hidden: false, width: 30, filterable: { multi: true, search: true } },
        { field: "Sezione", title: "SEZ", hidden: false, width: 30, filterable: { multi: true, search: true } },
        { field: "Foglio", title: "FOGLIO", hidden: false, width: 30, filterable: { multi: true, search: true } },
        { field: "Numero", title: "NUM", hidden: false, width: 30, filterable: { multi: true, search: true } },
        { field: "Superficie", title: "Superficie (Ha)", format: '{0:0.0000}', filterable: { multi: true, search: true }, width: 30 },
        { field: "veg_des", title: "Coltura", hidden: false, width: 30, filterable: { multi: true, search: true } }
    ];

    var toolbars = [];

    var parametriPerLettura = null;
    var parametriDataSource = { };
    var parametriKendoGrid = {
        editable: false,
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
    parametriKendoGrid.checkSelezioneRiga = { filterable: false, field: null, width: "10px" }

    var funzioniPrimaDopoEventi = { funzioneDaChiamarePrimaDelDataBound: onDataBoundRighe };
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


function selezionaParticelleImpiegate() {

    var datiGriglia = $("#divKendoOut").data('kendoGrid');
    var elemVisibiliHtml = datiGriglia.tbody.find("tr");
    var elemVisibiliDati = datiGriglia.dataSource.view();

    elemVisibiliDati.forEach(function (element, ind) {
        if (element.Selezionato === true) {
            element.Selected = true;
            var row = $(elemVisibiliHtml[ind]);
            row.addClass(GIAS_K_STATE_SELECTED)
                .find(".checkbox")
                .attr("checked", "checked");
            //$('#LblSuperficie_Con_Catasto').text(parseFloat($('#LblSuperficie_Con_Catasto').text()) + element.SuperficieImpiegata);
            //checkedIds[dataItem.id] = true;
            //console.log(checkedIds[dataItem.id]);
            //row = $(element).parents("tr");
            //row
            //datiGriglia.select(row);
        }
    }, this);
    RicalcolaSuperficieTotale();
}

function AbilitaModifica(value) {
    if (value === "True") {
        $("#btnSaveData").show();
        $("#btnSaveDataExit").show();
    } else {
        $("#btnSaveData").hide();
        $("#btnSaveDataExit").hide();
    }
}