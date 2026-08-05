var pq
var copiaPq
var editDataItem

function popola_Griglia_CriteriAggregazione(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: RicercaCriteriAggregazione,
        funzioneSubmit: { funzione: checkCancella, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };
    var idModel = "key_criteri_aggregazione";

    var campiKendoModel = {
        key_criteri_aggregazione: { editable: false, type: "string" },
        Piva: { editable: false, type: "string", validation: { required: true } },
        Rag_Soc: { editable: false, type: "string", validation: { required: true } },
        Tipologia_Lavorazione_Cod: { editable: false, type: "number", validation: { required: true } },
        Tipologia_Lavorazione: { editable: false, type: "string", validation: { required: true } },
        Tipologia_Lavorazione_Linea_Cod: { editable: false, type: "number", validation: { required: true } },
        Tipologia_Lavorazione_Linea: { editable: false, type: "string", validation: { required: true } },
        Aggrega_Fornitore_Cod: { editable: false, type: "number", validation: { required: true } },
        Aggrega_Fornitore: { editable: false, type: "string", validation: { required: true } },
        Aggrega_Specie_Cod: { editable: false, type: "number", validation: { required: true } },
        Aggrega_Specie: { editable: false, type: "string", validation: { required: true } },
        Aggrega_Varieta_Cod: { editable: false, type: "number", validation: { required: true } },
        Aggrega_Varieta: { editable: false, type: "string", validation: { required: true } },
        Aggrega_Regolamento_Cod: { editable: false, type: "number", validation: { required: true } },
        Aggrega_Regolamento: { editable: false, type: "string", validation: { required: true } },
        Aggrega_Lotto_Cod: { editable: false, type: "number", validation: { required: true } },
        Aggrega_Lotto: { editable: false, type: "string", validation: { required: true } },
        Lotto_Modifica_Uscita_Cod: { editable: false, type: "number", validation: { required: true } },
        Lotto_Modifica_Uscita: { editable: false, type: "string", validation: { required: true } },
        Aggrega_Prodotto_Cod: { editable: false, type: "number", validation: { required: true } },
        Aggrega_Prodotto: { editable: false, type: "string", validation: { required: true } },
        Prodotto_Modifica_Uscita_Cod: { editable: false, type: "number", validation: { required: true } },
        Prodotto_Modifica_Uscita: { editable: false, type: "string", validation: { required: true } },
        Aggrega_Cella_Cod: { editable: false, type: "number", validation: { required: true } },
        Aggrega_Cella: { editable: false, type: "string", validation: { required: true } },
        Cella_Modifica_Uscita_Cod: { editable: false, type: "number", validation: { required: true } },
        Cella_Modifica_Uscita: { editable: false, type: "string", validation: { required: true } },
        Aggrega_Unita_Misura_Cod: { editable: false, type: "number", validation: { required: true } },
        Aggrega_Unita_Misura: { editable: false, type: "string", validation: { required: true } },
        Criteri_Aggiuntivi: { editable: false, type: "string", validation: { require: true }},
        Criteri_Aggiuntivi_JSON: { editable: false, type: "string", validation: { require: true }}
    };

    var colonneKendoGrid = [];

    colonneKendoGrid.push(

        {
            field: "Rag_Soc", title: TraduzioneMultiResx(resxObj, "Azienda", "Azienda"), width: 100,filterable: { multi: true, search: true }
        },
        {
            field: "Tipologia_Lavorazione", title: TraduzioneMultiResx(resxObj, "TipologiaLavorazione.Text", "Tipologia Lavorazione").replace(":", ""), width: 125, filterable: { multi: true, search: true }
        },
        {
            field: "Tipologia_Lavorazione_Linea", title: TraduzioneMultiResx(resxObj, "LineaLavorazione.Text", "Lavorazione Linea").replace(":", ""), width: 125, filterable: { multi: true, search: true }
        },
        {
            field: "Aggrega_Fornitore", title: TraduzioneMultiResx(resxObj, "AggregaFornitore", "Aggrega Fornitore"), width: 175, filterable: { multi: true, search: true }
        },
        {
            field: "Aggrega_Specie", title: TraduzioneMultiResx(resxObj, "AggregaSpecie", "Aggrega Specie"), width: 150, filterable: { multi: true, search: true }
        },
        {
            field: "Aggrega_Varieta", title: TraduzioneMultiResx(resxObj, "AggregaVarieta", "Aggrega Varieta"), width: 150, filterable: { multi: true, search: true }
        },
        { field: "Aggrega_Regolamento", title: TraduzioneMultiResx(resxObj, "AggregaRegolamento", "Aggrega Regolamento"), width: 175, filterable: { multi: true, search: true } },
        { field: "Aggrega_Lotto", title: TraduzioneMultiResx(resxObj, "AggregaLotto", "Aggrega Lotto"), width: 150, filterable: { multi: true, search: true } },
        { field: "Lotto_Modifica_Uscita", title: TraduzioneMultiResx(resxObj, "LottoModificaUscita", "Lotto Modifica Uscita"), width: 175, filterable: { multi: true, search: true } },
        { field: "Aggrega_Prodotto", title: TraduzioneMultiResx(resxObj, "AggregaProdotto", "Aggrega Prodotto"), width: 175, filterable: { multi: true, search: true } },
        { field: "Prodotto_Modifica_Uscita", title: TraduzioneMultiResx(resxObj, "ProdottoModificaUscita", "Prodotto Modifica Uscita"), width: 200, filterable: { multi: true, search: true } },
        { field: "Aggrega_Cella", title: TraduzioneMultiResx(resxObj, "AggregaCella", "Aggrega Cella"), width: 175, filterable: { multi: true, search: true } },
        { field: "Cella_Modifica_Uscita", title: TraduzioneMultiResx(resxObj, "CellaModificaUscita", "Cella Modifica Uscita"), width: 200, filterable: { multi: true, search: true } },
        { field: "Aggrega_Unita_Misura", title: TraduzioneMultiResx(resxObj, "AggregaUnitaMisura", "Aggrega Unita Misura"), width: 175, filterable: { multi: true, search: true } },
        { field: "Criteri_Aggiuntivi", title: TraduzioneMultiResx(resxObj, "CriteriAggiuntivi", "Criteri Aggiuntivi"), width: 175, filterable: { multi: true, search: true }, encoded: false }
    );

    //campiKendoModel.validita_inizio.defaultValue = new Date("1900/01/01");

    //campiKendoModel.validita_fine.defaultValue = new Date("2100/12/31");

    var parametriPerLettura = null;
    var parametriDataSource = {};

    var colCustKendoGrid = [
        {
            hidden: false,
            command: [
            ],
            title: TraduzioneMultiResx(resxObj, "Operazioni", "Operazioni"),
            width: 130//, locked: true
        }
    ];

    // Se l'utente non è abilitato in modifica non mostro il pulsante di duplicazione
    // Le colonne modifica / cancellazione e inserimento nuova riga sono già gestite nel GiasBase
    if (UteAbilitatoInsMod) {
        colCustKendoGrid[0].command.push(
            {
                iconClass: "fa fa-pencil fa-xs", className: "blockModifica", name: "edit", text: "", click: editForm
            },
            {
                iconClass: "fa fa-files-o fa-xs", className: "blockDuplica", name: "duplica", text: "", click: copiaCriterio
            },
            {
                iconClass: "fa fa-trash fa-xs", className: "blockCancella", name: "destroy", text: ""
            });
    }

    var parametriKendoGrid = {
        editable: {
            mode: "inline"
        },
        colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true,
        //,
        //filterable: true
    };
    //parametriKendoGrid.toolbarCommands = [];
    //parametriKendoGrid.toolbarCommands.push("templateBtnNuovaRiga");

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: newForm };

    var mostraRigheCancellate = false;
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

    var grid = $("#" + IDControllo).data("kendoGrid");
    //grid.hideColumn(0);
    //for (let count = 1; count < grid.columns.length; count++) {
    //    let col = $('#griglia_Aggregazioni th:eq('+count+')')[0]
    //    $('#griglia_Aggregazioni th:eq(' + count + ')').title($('#griglia_Aggregazioni th:eq(' + count + ')').title().replace('>' + col.dataset.title, ('>' + col.dataset.title).replaceAll(' ','<br>')))
    //}

}

function checkCancella() {
    var grid = $("#griglia_Aggregazioni").data("kendoGrid");
    if (grid.dataSource.destroyed().length > 0)
        submitCriterio(2);
}

function newForm(e) {
    //var grid = $("#griglia_Aggregazioni").data("kendoGrid");
    //var dataItem = grid.dataItem($(e.currentTarget).closest("tr"))
    if (e.model.id === "") {
        CreaForm("Nuovo")
        $("#NuovoCriterio").kendoWindow({
            title: TraduzioneMultiResx(resxObj, "NuovoCriterio", "Nuovo Criterio"),
            modal: true,
            visible: false,
            resizable: false,
            height: "100%",
            width: "100%",
            actions: ["Maximize", "Close"],
            close: function () {
                setTimeout(function () { $('#NuovoCriterio').kendoWindow('destroy'); }, 200);
                $("#griglia_Aggregazioni").data("kendoGrid").dataSource.read();
            }
        }).data('kendoWindow').center().open();
    } else {
        editDataItem = e.model
    }
}

function editForm(e) {
    var grid = $("#griglia_Aggregazioni").data("kendoGrid");
    var dataItem = grid.dataItem($(e.currentTarget).closest("tr"))
    if (dataItem === null || dataItem === undefined) {
        dataItem = editDataItem
    }
    copiaPq = JSON.parse(dataItem.Criteri_Aggiuntivi_JSON)
    CreaForm("Modifica")
    CopiaParamIniziali(dataItem)
    KendoDDL("ddlAzienda").enable(false)
    KendoDDL("Lavorazione").enable(false)
    KendoDDL("LineaLav").enable(false)
    $("#ModificaCriterio").kendoWindow({
        title: TraduzioneMultiResx(resxObj, "ModificaCriterio", "Modifica Criterio"),
        modal: true,
        visible: false,
        resizable: false,
        height: "100%",
        width: "100%",
        actions: ["Maximize", "Close"],
        close: function () {
            copiaPq = undefined
            setTimeout(function () { $('#ModificaCriterio').kendoWindow('destroy'); }, 200);
            $("#griglia_Aggregazioni").data("kendoGrid").dataSource.read();
        }
    }).data('kendoWindow').center().open();
}

function copiaCriterio(e) {
    var grid = $("#griglia_Aggregazioni").data("kendoGrid");
    var dataItem = grid.dataItem($(e.currentTarget).closest("tr"))
    copiaPq = JSON.parse(dataItem.Criteri_Aggiuntivi_JSON)
    CreaForm("Copia")
    CopiaParamIniziali(dataItem)
    $("#CopiaCriterio").kendoWindow({
        title: TraduzioneMultiResx(resxObj, "CopiaCriterio", "Copia Criterio"),
        modal: true,
        visible: false,
        resizable: false,
        height: "100%",
        width: "100%",
        actions: ["Maximize", "Close"],
        close: function () {
            copiaPq = undefined
            setTimeout(function () { $('#CopiaCriterio').kendoWindow('destroy'); }, 200);
            $("#griglia_Aggregazioni").data("kendoGrid").dataSource.read();
        }
    }).data('kendoWindow').center().open();
}

function CreaForm(text) {
    $(document.body).append('<div id="'+ text + 'Criterio"></div>');
    $("#"+ text + "Criterio").append('<div id="row1" class="row"></div>');
    $("#row1").append('<div class="col-lg-8 col-md-8 col-sm-12"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="CTRL_Azienda" for="ddlAzienda">' + TraduzioneMultiResx(resxObj, "Azienda", "Azienda") + '</label><input type="text" id="ddlAzienda" name="ddlAzienda" class="form-control" aria-describedby="CTRL_Azienda" onchange="azienda_change();"></div></div></div></div>')
    $("#row1").append('<div class="col-lg-5 col-md-5 col-sm-12"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="Lavlab" for="Lavorazione">' + TraduzioneMultiResx(resxObj, "TipologiaLavorazione.Text", "Tipologia Lavorazione").replace(":", "") + '</label><input type="text" id="Lavorazione" name="Lavorazione" class="form-control" onchange="lavorazioni_change();" disabled="disabled"></div></div></div ></div>')
    $("#row1").append('<div class="col-lg-4 col-md-4 col-sm-12 text-center"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="LineaLavLab" for="LineaLav">' + TraduzioneMultiResx(resxObj, "LineaLavorazione.Text", "Lavorazione Linea").replace(":", "") + '</label><input type="text" id="LineaLav" name="LineaLav" class="form-control" onchange="linee_change();" disabled="disabled"></div></div></div></div >')
    $("#"+ text + "Criterio").append('<div id="row2" class="row"></div>');
    $("#row2").append('<div class="col-lg-3 col-md-6 col-sm-6 text-center"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="fornitoreLab" for="fornitore">' + TraduzioneMultiResx(resxObj, "AggregaFornitore", "Aggrega Fornitore") + '</label><input type="text" id="fornitore" name="fornitore" class="form-control"></div></div></div></div >')
    $("#row2").append('<div class="col-lg-2 col-md-6 col-sm-6 text-center"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="specielab" for="specie">' + TraduzioneMultiResx(resxObj, "AggregaSpecie", "Aggrega Specie") + '</label><input type="text" id="specie" name="specie" class="form-control"></div></div></div></div >')
    $("#row2").append('<div class="col-lg-2 col-md-6 col-sm-6"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="varietalab" for="varieta">' + TraduzioneMultiResx(resxObj, "AggregaVarieta", "Aggrega Varieta") + '</label><input type="text" id="varieta" name="varieta" class="form-control"></div></div></div></div >')
    $("#row2").append('<div class="col-lg-2 col-md-6 col-sm-6"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="regolamentolab" for="regolamento">' + TraduzioneMultiResx(resxObj, "AggregaRegolamento", "Aggrega Regolamento") + '</label><input type="text" id="regolamento" name="regolamento" class="form-control"></div></div></div></div >')
    $("#"+ text + "Criterio").append('<div id="row3" class="row"></div>');
    $("#row3").append('<div class="col-lg-3 col-md-6 col-sm-6 text-center"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="lottoLab" for="lotto">' + TraduzioneMultiResx(resxObj, "AggregaLotto", "Aggrega Lotto") + '</label><input type="text" id="lotto" name="lotto" class="form-control" onchange="Checks();"></div></div></div></div >')
    $("#row3").append('<div class="col-lg-2 col-md-6 col-sm-6 text-center"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="lottoUscitalab" for="lottoUscita">' + TraduzioneMultiResx(resxObj, "LottoModificaUscita", "Lotto Modifica Uscita")  + '</label><input type="text" id="lottoUscita" name="lottoUscita" class="form-control" onchange="Checks();"></div></div></div></div >')
    $("#row3").append('<div class="col-lg-2 col-md-6 col-sm-6"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="prodottolab" for="prodotto">' + TraduzioneMultiResx(resxObj, "AggregaProdotto", "Aggrega Prodotto") + '</label><input type="text" id="prodotto" name="prodotto" class="form-control" onchange="Checks();"></div></div></div></div >')
    $("#row3").append('<div class="col-lg-2 col-md-6 col-sm-6"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="prodottoUscitalab" for="prodottoUscita">' + TraduzioneMultiResx(resxObj, "ProdottoModificaUscita", "Prodotto Modifica Uscita") + '</label><input type="text" id="prodottoUscita" name="prodottoUscita" class="form-control" onchange="Checks();"></div></div></div></div >')
    $("#row3").append('<div class="col-lg-3 col-md-6 col-sm-6"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="cellalab" for="cella">' + TraduzioneMultiResx(resxObj, "AggregaCella", "Aggrega Cella") + '</label><input type="text" id="cella" name="cella" class="form-control" onchange="Checks();"></div></div></div></div >')
    $("#row3").append('<div class="col-lg-2 col-md-6 col-sm-6"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="cellaUscitalab" for="cellaUscita">' + TraduzioneMultiResx(resxObj, "CellaModificaUscita", "Cella Modifica Uscita") + '</label><input type="text" id="cellaUscita" name="cellaUscita" class="form-control" onchange="Checks();"></div></div></div></div >')
    $("#row3").append('<div class="col-lg-2 col-md-6 col-sm-6"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="unitaMisuralab" for="unitaMisura">' + TraduzioneMultiResx(resxObj, "AggregaUnitaMisura", "Aggrega Unita Misura") + '</label><input type="text" id="unitaMisura" name="unitaMisura" class="form-control"></div></div></div></div >')
    $("#"+ text + "Criterio").append('<div id="row4" class="row"></div>');
    $("#row4").append('<div id="paramQual" class="ParametriQualitativi"></div >')
    $("#"+ text + "Criterio").append('<div id="row5" class="row"></div>');
    $("#row5").append('<div id="errors" class="errors"></div >')
    $("#" + text + "Criterio").append('<div id="row6" class="row"></div>');
    if (text === "Modifica") {
        $("#row6").append('<div class="btn btn-success" id="btn_Edit" style="margin-right: 3px; margin-left: 15px;" onclick="submitCriterio(1)" title="' + TraduzioneMultiResx(resxObj, "Modifica", "Modifica") + '"><span class= "fa fa-pencil lampeggiante" > </span ><span class="lampeggiante">' + TraduzioneMultiResx(resxObj, "Modifica", "Modifica") + '</span></div >')
    } else {
        $("#row6").append('<div class="btn btn-success" id="btn_Insert" style="margin-right: 3px; margin-left: 15px;" onclick="submitCriterio(0)" title="' + TraduzioneMultiResx(resxObj, "Aggiungi", "Aggiungi") + '"><span class= "fa fa-plus lampeggiante" > </span ><span class="lampeggiante">' + TraduzioneMultiResx(resxObj, "Aggiungi", "Aggiungi") + '</span></div >')
    }
    creaKendoDropDownList("fornitore", {
        read: function (options) {
            options.success([
                { Val: 0, Des: TraduzioneMultiResx(resxObj, "Tutti", "Tutti") },
                { Val: 1, Des: TraduzioneMultiResx(resxObj, "SoloUguali", "Solo Uguali") },
            ]);
        }
    }, "Des", "Val");

    creaKendoDropDownList("specie", {
        read: function (options) {
            options.success([
                { Val: 0, Des: TraduzioneMultiResx(resxObj, "Tutti", "Tutti") },
                { Val: 1, Des: TraduzioneMultiResx(resxObj, "SoloUguali", "Solo Uguali") },
            ]);
        }
    }, "Des", "Val");

    creaKendoDropDownList("varieta", {
        read: function (options) {
            options.success([
                { Val: 0, Des: TraduzioneMultiResx(resxObj, "Tutti", "Tutti") },
                { Val: 1, Des: TraduzioneMultiResx(resxObj, "SoloUguali", "Solo Uguali") },
            ]);
        }
    }, "Des", "Val");

    creaKendoDropDownList("regolamento", {
        read: function (options) {
            options.success([
                { Val: 0, Des: TraduzioneMultiResx(resxObj, "Tutti", "Tutti") },
                { Val: 1, Des: TraduzioneMultiResx(resxObj, "SoloUguali", "Solo Uguali") },
            ]);
        }
    }, "Des", "Val");

    creaKendoDropDownList("lotto", {
        read: function (options) {
            options.success([
                { Val: 0, Des: TraduzioneMultiResx(resxObj, "Tutti", "Tutti") },
                { Val: 1, Des: TraduzioneMultiResx(resxObj, "SoloUguali", "Solo Uguali") },
            ]);
        }
    }, "Des", "Val");

    creaKendoDropDownList("lottoUscita", {
        read: function (options) {
            options.success([
                { Val: 1, Des: TraduzioneMultiResx(resxObj, "Si", "Si") },
                { Val: 0, Des: TraduzioneMultiResx(resxObj, "No", "No") },
            ]);
        }
    }, "Des", "Val");

    creaKendoDropDownList("prodotto", {
        read: function (options) {
            options.success([
                { Val: 0, Des: TraduzioneMultiResx(resxObj, "Tutti", "Tutti") },
                { Val: 1, Des: TraduzioneMultiResx(resxObj, "SoloUguali", "Solo Uguali") },
            ]);
        }
    }, "Des", "Val");

    creaKendoDropDownList("prodottoUscita", {
        read: function (options) {
            options.success([
                { Val: 1, Des: TraduzioneMultiResx(resxObj, "Si", "Si") },
                { Val: 0, Des: TraduzioneMultiResx(resxObj, "No", "No") },
            ]);
        }
    }, "Des", "Val");

    creaKendoDropDownList("cella", {
        read: function (options) {
            options.success([
                { Val: 0, Des: TraduzioneMultiResx(resxObj, "Tutti", "Tutti") },
                { Val: 1, Des: TraduzioneMultiResx(resxObj, "SoloUguali", "Solo Uguali") },
            ]);
        }
    }, "Des", "Val");

    creaKendoDropDownList("cellaUscita", {
        read: function (options) {
            options.success([
                { Val: 1, Des: TraduzioneMultiResx(resxObj, "Si", "Si") },
                { Val: 0, Des: TraduzioneMultiResx(resxObj, "No", "No") },
            ]);
        }
    }, "Des", "Val");

    creaKendoDropDownList("unitaMisura", {
        read: function (options) {
            options.success([
                //{ Val: 0, Des: TraduzioneMultiResx(resxObj, "Tutti", "Tutti") },
                { Val: 1, Des: TraduzioneMultiResx(resxObj, "SoloUguali", "Solo Uguali") },
            ]);
        }
    }, "Des", "Val");

    if (text != "Nuovo") {
        $('#ddlAzienda').kendoDropDownList({
            filter: "contains",
            dataSource: {
                transport: {
                    read: RiempiDdlAzienda
                }
            },
            dataTextField: "rag_soc",
            dataValueField: "piva",
            mapValueTo: "dataItem",
            dataBound: ddlAzienda_OnDataBound,
        });
    } else {
        $('#ddlAzienda').kendoDropDownList({
            filter: "contains",
            dataSource: {
                transport: {
                    read: RiempiDdlAzienda
                }
            },
            dataTextField: "rag_soc",
            dataValueField: "piva",
            mapValueTo: "dataItem",
            dataBound: ddlAzienda_OnDataBound,
            virtual: {
                itemHeight: 26,
                valueMapper: function (options) {
                    var val = options.value;
                    var ind = ""
                    if (val != "" && val != "-1") {
                        var aziende = KendoDDL("ddlAzienda").dataSource.data()
                        //Se l'azienda con cui sono entrato è nella lista delle aziende selezionabili, la scelgo
                        aziende.forEach(function (item, index) {
                            //console.log(item)
                            if (item !== undefined) {
                                if (item.piva == val) {
                                    ind = index
                                }
                            }
                        })
                        options.success(ind);
                        //var a = KendoDDL("ddlAzienda").dataSource._pristineData.find((el) =>
                        //    el.piva == val
                        //);
                    } else {
                        options.success("");
                    }
                }
            },
        });
    }

    $('#Lavorazione').kendoDropDownList({
        filter: "contains",
        dataSource: {
            transport: {
                read: RicercaPreparazioniGeneriche
            }
        },
        dataTextField: "Preparazione_Des",
        dataValueField: "Preparazione_Cod",
        mapValueTo: "dataItem",
        dataBound: Lavorazioni_OnDataBound,
    });

    Checks();
}

function CopiaParamIniziali(dataItem) {
    KendoDDL("fornitore").select(dataItem.Aggrega_Fornitore_Cod)
    KendoDDL("specie").select(dataItem.Aggrega_Specie_Cod)
    KendoDDL("varieta").select(dataItem.Aggrega_Varieta_Cod)
    KendoDDL("regolamento").select(dataItem.Aggrega_Regolamento_Cod)
    KendoDDL("lotto").select(dataItem.Aggrega_Lotto_Cod)    
    KendoDDL("lottoUscita").select(dataItem.Lotto_Modifica_Uscita_Cod === 1 ? 0 : 1)
    KendoDDL("prodotto").select(dataItem.Aggrega_Prodotto_Cod)
    KendoDDL("prodottoUscita").select(dataItem.Prodotto_Modifica_Uscita_Cod === 1 ? 0 : 1)
    KendoDDL("cella").select(dataItem.Aggrega_Cella_Cod)
    KendoDDL("cellaUscita").select(dataItem.Cella_Modifica_Uscita_Cod === 1 ? 0 : 1)
    let index = KendoDDL("unitaMisura").dataSource.data().indexOf(KendoDDL("unitaMisura").dataSource.data().find(x => x.Val === dataItem.Aggrega_Unita_Misura_Cod))
    KendoDDL("unitaMisura").select(index);
    index = KendoDDL("ddlAzienda").dataSource.data().indexOf(KendoDDL("ddlAzienda").dataSource.data().find(x => x.piva === dataItem.Piva))
    KendoDDL("ddlAzienda").select(index);
    ddlAzienda.onchange();
    index = KendoDDL("Lavorazione").dataSource.data().indexOf(KendoDDL("Lavorazione").dataSource.data().find(x => x.Preparazione_Cod === dataItem.Tipologia_Lavorazione_Cod))
    KendoDDL("Lavorazione").select(index)
    Lavorazione.onchange();
    index = KendoDDL("LineaLav").dataSource.data().indexOf(KendoDDL("LineaLav").dataSource.data().find(x => x.Preparazione_Cod === dataItem.Tipologia_Lavorazione_Linea_Cod))
    KendoDDL("LineaLav").select(index)
    LineaLav.onchange();
    CopiaParamQual();
    if (KendoDDL("lotto").value() === '0') {
        KendoDDL("lottoUscita").enable(false);
    } else {
        KendoDDL("lottoUscita").enable(true);
    }
    if (KendoDDL("cella").value() === '0') {
        KendoDDL("cellaUscita").enable(false);
    } else {
        KendoDDL("cellaUscita").enable(true);
    }
    if (KendoDDL("prodotto").value() === '0') {
        KendoDDL("prodottoUscita").enable(false);
    } else {
        KendoDDL("prodottoUscita").enable(true);
    }
}

function CopiaParamQual() {
    var pqTemp
    var esistePq
    let ddlTemp
    let indexTemp
    if (copiaPq.length > 0) {
        for (i = 0; i < copiaPq.length; i++) {
            pqTemp = copiaPq[i];
            esistePq = pq.find(x => x.Tabella_Cod_Des == pqTemp.CodParam)
            if (esistePq != undefined) {
                ddlTemp = KendoDDL("aggrega" + esistePq.Tabella_Cod_Des + "")
                indexTemp = ddlTemp.dataSource.data().indexOf(ddlTemp.dataSource.data().find(x => x.Val === pqTemp.AggregaParam))
                KendoDDL("aggrega" + esistePq.Tabella_Cod_Des + "").select(indexTemp)
                ddlTemp = KendoDDL("modif" + esistePq.Tabella_Cod_Des + "")
                indexTemp = ddlTemp.dataSource.data().indexOf(ddlTemp.dataSource.data().find(x => x.Val === pqTemp.ModifUscita))
                KendoDDL("modif" + esistePq.Tabella_Cod_Des + "").select(indexTemp)
            }
        }
    }
}

function ddlAzienda_OnDataBound(e) {
    var ds = this.dataSource.data();
    var targetPiva = ""
    if (cIdPiva.localeCompare(KendoDDL("ddlImpresa").value()) !== 0 && KendoDDL("ddlImpresa").value() !== "")
        targetPiva = KendoDDL("ddlImpresa").value();
    else
        targetPiva = cIdPiva
    var index = ds.indexOf(ds.find(x => x.piva === targetPiva));
    if (ds.length == 1) {
        this.select(0); //seleziono l'elemento 
        ddlAzienda.onchange(); //forzo l'evento di onchange
    }
    if (index > 0) {
        this.select(index);
        ddlAzienda.onchange();
    }

}

function ddlImpresa_OnDataBound(e) {
    var ds = this.dataSource.data();
    var index = ds.indexOf(ds.find(x => x.piva === cIdPiva));
    if (ds.length == 1) {
        this.select(0); //seleziono l'elemento 
        ddlImpresa.onchange(); //forzo l'evento di onchange
    }
    if (index > 0) {
        this.select(index);
        ddlImpresa.onchange();
    }

}

function Lavorazioni_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(0); //seleziono l'elemento 
        Lavorazione.onchange(); //forzo l'evento di onchange
    }
    //todo riabilita il prossimo campo
}

function Linee_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(0); //seleziono l'elemento 
        LineaLav.onchange(); //forzo l'evento di onchange
    }
    //todo riabilita il prossimo campo
}

function azienda_change(e) {
    if (KendoDDL('Lavorazione') !== undefined) {
        KendoDDL('Lavorazione').enable(true);
        if (KendoDDL('Lavorazione').value() != 0) {
            if (pq.length > 0) {
                document.getElementById("paramQual").textContent = '';
            }
            pq = RicercaParametriQualitativiFiltroSpecieVarieta(false, KendoDDL("ddlAzienda").value(), KendoDDL("Lavorazione").dataItem().Modulo_Generazione, 0, 0);
            CreaParametriQualitativi(pq);
            if (copiaPq != undefined) {
                CopiaParamQual();
            }
            if (KendoDDL('LineaLav') != undefined) {
                KendoDDL('LineaLav').refresh();
            }
        }
    }
}

function impresa_change(e) {
    if ($("#griglia_Aggregazioni").data("kendoGrid") === undefined) {
        if ($("#griglia_Aggregazioni").data("kendoGrid") != undefined) {
            $("#griglia_Aggregazioni").data("kendoGrid").dataSource.read();
        }
    }
    else {
        let primaPiva = $("#griglia_Aggregazioni").data("kendoGrid")._data[0]
        let ultimaPiva = $("#griglia_Aggregazioni").data("kendoGrid")._data[$("#griglia_Aggregazioni").data("kendoGrid")._data.length - 1]
        let pivaCorrente = KendoDDL("ddlImpresa").value()
        if (pivaCorrente.localeCompare(primaPiva) !== 0 && pivaCorrente.localeCompare(ultimaPiva) !== 0)
            $("#griglia_Aggregazioni").data("kendoGrid").dataSource.read();
    }
}

function lavorazioni_change(e) {
    if (pq != undefined && pq.length > 0) {
        document.getElementById("paramQual").textContent = '';
    }
    pq = RicercaParametriQualitativiFiltroSpecieVarieta(false, KendoDDL("ddlAzienda").value(), KendoDDL("Lavorazione").dataItem().Modulo_Generazione, 0, 0);
    CreaParametriQualitativi(pq);
    if (copiaPq != undefined) {
        CopiaParamQual();
    }
    if (KendoDDL('LineaLav') == undefined) {
        $('#LineaLav').kendoDropDownList({
            filter: "contains",
            dataSource: {
                transport: {
                    read: RicercaLineePreparazione
                }
            },
            dataTextField: "descrizione",
            dataValueField: "Preparazione_Cod",
            mapValueTo: "dataItem",
            dataBound: Linee_OnDataBound,
        });
    } else {
        KendoDDL('LineaLav').dataSource.read();
    }
}

function linee_change(e) {
     
}

function CreaParametriQualitativi(params) {
    for (var i = 0; i < params.length; i++) {
        var item = params[i];
        var nomeDiv = item.Tabella_Cod_Des
        var nomePQ = item.Tabella_Des.replace("/", " ").replace(".", "").replace("(", "").replace(")", "").replace("(", "").replace(")", "").replace("-","")
        $("#paramQual").append('<p></p><div id="' + nomeDiv + '"><h4 style="padding-left: 15px;">' + nomePQ + '</h4></div>')
        $("#" + nomeDiv + "").append('<div class="col-lg-4 col-md-4 col-sm-6"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="lblaggrega' + nomeDiv + '" for="aggrega' + nomeDiv + '">Criterio Aggregazione</label><input type="text" class="critAggr ' + nomeDiv + '" id="aggrega' + nomeDiv + '" name="aggrega' + nomeDiv + '" class="form-control"></div></div></div></div >')
        $("#" + nomeDiv + "").append('<div class="col-lg-4 col-md-4 col-sm-6"><div class= "form-horizontal" ><div class="form-group"><div class="input-group"><label class="input-group-addon control-label alert-info" id="lblmodif' + nomeDiv + '" for="modif' + nomeDiv + '">Modificabile in uscita</label><input type="text" class="critMod ' + nomeDiv + '" id="modif' + nomeDiv + '" name="modif' + nomeDiv + '" class="form-control"></div></div></div></div ><br><br><br>')
        creaKendoDropDownList("aggrega" + nomeDiv + "", {
            read: function (options) {
                if (item.Tipo === 3) {
                    options.success([
                        { Val: 0, Des: "" },
                        { Val: 1, Des: TraduzioneMultiResx(resxObj, "SoloUguali", "Solo Uguali"), },
                        { Val: 10, Des: TraduzioneMultiResx(resxObj, "Media", "Media") },
                        { Val: 11, Des: TraduzioneMultiResx(resxObj, "Minimo", "Minimo") },
                        { Val: 12, Des: TraduzioneMultiResx(resxObj, "Massimo", "Massimo") },
                    ]);
                } else {
                    options.success([
                        { Val: 0, Des: "" },
                        { Val: 1, Des: TraduzioneMultiResx(resxObj, "SoloUguali", "Solo Uguali") },
                    ]);
                }
            }
        }, "Des", "Val");
        creaKendoDropDownList("modif" + nomeDiv + "", {
            read: function (options) {
                options.success([
                    { Val: 1, Des: TraduzioneMultiResx(resxObj, "No", "No") },
                    { Val: 0, Des: TraduzioneMultiResx(resxObj, "Si", "Si") },
                ]);
            }
        }, "Des", "Val");
    }
}

function Checks() {
    if (KendoDDL("lotto").value() === '0') {
        KendoDDL("lottoUscita").value(1);
        KendoDDL("lottoUscita").enable(false);
    } else if($("#lottoUscita").prop('disabled')) {
        KendoDDL("lottoUscita").value(0);
        KendoDDL("lottoUscita").enable(true);
    }
    if (KendoDDL("prodotto").value() === '0') {
        KendoDDL("prodottoUscita").value(1);
        KendoDDL("prodottoUscita").enable(false);
    } else if ($("#prodottoUscita").prop('disabled')) {
        KendoDDL("prodottoUscita").value(0);
        KendoDDL("prodottoUscita").enable(true);
    }
    if (KendoDDL("cella").value() === '0') {
        KendoDDL("cellaUscita").value(1);
        KendoDDL("cellaUscita").enable(false);
    } else if ($("#cellaUscita").prop('disabled')) {
        KendoDDL("cellaUscita").value(0);
        KendoDDL("cellaUscita").enable(true);
    }
}

function submitCriterio(azione) {
    if (azione === 2) {
        cancellaCriterio()
    } else {
        var paramQualRes = "[";
        var inputs = document.getElementsByClassName('critAggr')
        var mod = document.getElementsByClassName('critMod')
        var dato
        var found = 0
        for (var i = 1; i < inputs.length; i = i + 2) {
            dato = inputs[i]
            if (dato.value != 0) {
                if (found == 0) {
                    found++;
                } else {
                    paramQualRes = paramQualRes.concat(",")
                }
                paramQualRes = paramQualRes.concat('{"CodParam": "' + dato.classList[1] + '", "AggregaParam": ' + dato.value + ', "ModifUscita": ' + mod[i].value + '}')
            }
        }
        paramQualRes = paramQualRes.concat(']')
        nuovoModificaCriterio(paramQualRes,azione)
    }
}

function gestisciRispostaFinestra(res) {
    if (res == 'true') {
        setTimeout(function () { $('#NuovoCriterio').kendoWindow('destroy'); }, 200);
        setTimeout(function () { $('#CopiaCriterio').kendoWindow('destroy'); }, 200);
        setTimeout(function () { $('#ModificaCriterio').kendoWindow('destroy'); }, 200);
        copiaPq = undefined;
        $("#griglia_Aggregazioni").data("kendoGrid").dataSource.read()
    }
}