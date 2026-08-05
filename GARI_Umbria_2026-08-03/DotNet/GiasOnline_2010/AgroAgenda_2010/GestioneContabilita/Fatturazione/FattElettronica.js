
function popolaGrigliaFattElettr(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: RicercaFatture,
        checkBoxFunction: SelezionaFatture,
        funzioneInsert: null,
        funzioneUpdate: null,
        funzioneDelete: null,
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };

    var idModel = "IdAgenda";
    var campiKendoModel = {
        IdSDINumeroDocumento: { type: "string" },
        NumeroDocumento: { type: "string" },
        DataDocumento: { type: "date" },
        TipoDocumento: { type: "string" },
        Cliente: { type: "string" },
        Piva: { type: "string" },
        IdAgenda: { type: "number" },
        BloccoFlag: { type: "number" },
        BloccoFlagDes: { type: "string" },
        BloccoData: { type: "date" },
        BloccoUsername: { type: "string" },
        Descrizione: { type: "string" },
        NomeFileXML: { type: "string" },
        NomeFileZIP: { type: "string" },
        DataGenXML: { type: "date" },
        IdLog: { type: "number" },
        IdSDI: { type: "number" },
        DataLogSDI: { type: "date" },
        DataOraInvio: { type: "date" },
        TipoFattura: { type: "string" },
        NrTentativi: { type: "number" },
        StatoInvio: { type: "string" },
        StatoEsito: { type: "string" },
        StatoInvioDes: { type: "string" },
        StatoEsitoDes: { type: "string" },
        InvioXML: { type: "boolean" },
        Note: { type: "string" }
    };
    var colonneKendoGrid = [
        { field: "NumeroDocumento", title: "Nr Doc", width: 100 },
        { field: "DataDocumento", title: "Data Doc", format: "{0:dd/MM/yyyy}", width: 100 },
        { field: "TipoDocumento", title: "Tipo Doc", width: 200, filterable: { multi: true, search: true } },
        { field: "Cliente", title: "Cliente", width: 300, filterable: { multi: true, search: true } },
        { field: "Piva", title: "Partita IVA", hidden: true },
        { field: "Descrizione", title: "Descrizione", hidden: true },
        { field: "NomeFileXML", title: "Nome File xml" },
        { field: "NomeFileZIP", title: "Nome File zip", hidden: true },
        { field: "DataGenXML", title: "Data Creazione xml", template: '#=templateData(data.DataGenXML)#' },
        { field: "IdSDI", title: "Id SDI", hidden: true },
        { field: "DataLogSDI", title: "Data Log SDI", template: '#=templateData(data.DataLogSDI)#', hidden: true },
        { field: "DataOraInvio", title: "Data e Ora Invio", template: '#=templateData(data.DataOraInvio)#', hidden: true },
        { field: "TipoFattura", title: "Tipo Fattura", hidden: true },
        { field: "NrTentativi", title: "Nr Tentativi", hidden: true },
        { field: "BloccoFlagDes", title: "Blocco", filterable: { multi: true, search: true } },
        { field: "BloccoData", title: "Blocco Data", template: '#=templateData(data.BloccoData)#', hidden: true },
        { field: "BloccoUsername", title: "Blocco Username", hidden: true },
        { field: "StatoInvioDes", title: "Stato Invio", filterable: { multi: true, search: true } },
        { field: "StatoEsitoDes", title: "Esito", filterable: { multi: true, search: true }  }
    ];

    var parametriPerLettura = null;
    var parametriDataSource = {};

    var colCustKendoGrid = [{
        command: [
            {
                iconClass: "fa fa-external-link fa-lg",
                className: "e_link",
                name: "e_link",
                text: "&nbsp;",
                click: visualizzaElemento
            }
        ],
        title: "Dettagli", width: "84px"
    }];

    var parametriKendoGrid = {
        // filterable: { mode: "row" },scarica
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        editable: false,
        reorderable: true,
        columnMenu: true,
        //selectable: false,
        colonneCustomKendoGrid: colCustKendoGrid,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        checkSelezioneRiga: { filterable: false, field: null, width: "32px" }
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: kendo_Operazioni_onDataBoundedRighe };

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

function SelezionaFatture(e) {

    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = $("#tab_fatt_elettronica").data("kendoGrid");

    var dataItem = grid.dataItem(row);
    dataItem.Selected = checked;
    
    rowKendoGridSelected(row, checked)
}

function templateBlocco(blocco) {
    if (blocco === 0) {
        return "NO";
    } else if (blocco === 1) {
        return "SI";
    } else if (blocco === 1000) {
        return "Standby";
    }
    return "";
}

function templateStatoInvio(stato) {
    if (stato === "1") {
        return "In elaborazione";
    } else if (stato === "300") {
        return "XML non generato";
    } else if (stato === "301") {
        return "XML non generabile";
    } else if (stato === "400") {
        return "XML generato";
    } else if (stato === "401") {
        return "XML esportato";
    } else if (stato === "402") {
        return "XML da rigenerare";
    } else if (stato === "403") {
        return "Non Inviato";
    } else if (stato === "500") {
        return "Inviato";
    }
    return "";
}

function templateStatoEsito(stato) {
    if (stato === "100") {
        return "Non ancora disponibile";
    } else if (stato === "200") {
        return "Esiti multipli";
    } else if (stato === "300") {
        return "Negativo";
    } else if (stato === "400") {
        return "Positivo";
    } else if (stato === "500") {
        return "Mancata consegna";
    }
    return "";
}

function templateData(data) {
    if (kendo.toString(data, "dd/MM/yyyy") === "01/01/1900") return "";
    return kendo.toString(data, "dd/MM/yyyy HH:mm:ss");
}

function controllaDate() {
    var dataDal = $('input[name$="txt_DataDal"]').val();
    if (dataDal === "" || !isValidDate(dataDal)) {
        MessaggioErrore_Bootstrap("Data Dal mancante o non valida", "DIV_Messaggi");
        return false;
    }
    var dataAl = $('input[name$="txt_DataAl"]').val();
    if (dataAl === "" || !isValidDate(dataAl)) {
        MessaggioErrore_Bootstrap("Data Al mancante o non valida", "DIV_Messaggi");
        return false;
    }

    var oggi = new Date();
    oggi = oggi.setHours(0, 0, 0, 0);
    var inizio = $("#txt_DataDal").data("kendoDatePicker").value();
    var fine = $("#txt_DataAl").data("kendoDatePicker").value();

    if (inizio > fine) {
        MessaggioErrore_Bootstrap("L'intervallo di date non è valido", "DIV_Messaggi");
        return false;
    } else if (inizio > oggi || fine > oggi) {
        // kendo.alert("Attenzione: la procedura di fatturazione elettronica considera solo le fatture fino alla data corrente");
        MessaggioAttenzione_Bootstrap("Attenzione: la procedura di fatturazione elettronica considera solo le fatture fino alla data corrente", "DIV_Messaggi");
    }
    
    return true;
}

function popolaComboImprese() {
    $("#ddlAziende").kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: ElencoImprese } },
        dataTextField: "Rag_Soc",
        dataValueField: "Piva",
        change: function (e) {
            var dataItem = e.sender.dataItem();
            popolaGrigliaFattElettronica();
            LeggiConfigurazione();
        },
        value: $(cIdPiva).val()
    });
}

function popolaGrigliaFattElettronica() {
    if (controllaDate()) {
        popolaGrigliaFattElettr("tab_fatt_elettronica");
        $(".elencoFattElettronica").show();
    }
}

function visualizzaElemento(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    wnd.content(detailsTemplate(dataItem));
    wnd.center().open();
    LogFattura(dataItem.IdAgenda);
}

function kendo_Operazioni_onDataBoundedRighe(e) {
    /* var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    for (var i = 0; i < grid.columns.length; i++) {
        grid.autoFitColumn(i);
    } */
    coloraRigheOperazioni("#tab_fatt_elettronica", e);
}

function coloraRigheOperazioni(grid_elem, eventArgs) {

    var grid = $(grid_elem).data('kendoGrid');
    var items = eventArgs.sender.items();

    items.each(function (index) {

        var dataItem = grid.dataItem(this);

        if (dataItem.BloccoFlag !== 0) {
            this.className += " kendoRiga_Arancione";
        }

        switch (dataItem.StatoEsito) {
            case "300":
                this.className += " kendoRiga_Rossa";
                break;
            default:
        }
        
    });

}


