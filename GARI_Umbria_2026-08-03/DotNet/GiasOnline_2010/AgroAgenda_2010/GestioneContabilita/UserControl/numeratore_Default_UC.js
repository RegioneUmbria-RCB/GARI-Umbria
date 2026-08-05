
function Popola_Numeratori_Default(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: CaricaDocumentiDefault,
        funzioneSubmit: { funzione: SubmitDocumentiDefault, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };
    var idModel = "Key_Default";
    var campiKendoModel = {
        Key_Default: { editable: false, type: "string" },
        Piva: { editable: true, type: "string" },
        Sa_Cod: { editable: true, type: "number", defaultValue: -1 },
        Sa_Descr: { editable: true, type: "string" },
        Sezionale_Cod: { editable: true, type: "number", validation: { required: false }, defaultValue: -99 },
        Sezionale_Descr: { editable: true, type: "string", validation: { required: false } },
        CausaleDoc_Cod: { editable: true, type: "number", validation: { required: false }, defaultValue: -99 },
        CausaleDoc_Descr: { editable: true, type: "string", validation: { required: false } },
        NumTipo_Cod: { editable: true, type: "number", validation: { required: false }, defaultValue: -99 },
        NumTipo_Descr: { editable: true, type: "string", validation: { required: false } },
        Lav_Cod: { editable: true, type: "number", defaultValue: -0 },
        Lav_Descr: { editable: true, type: "string" },
        TipoFattura_Cod: { editable: true, type: "string", defaultValue: " " },
        TipoFattura_Descr: { editable: true, type: "string" },
        Vincolante: { editable: true, type: "boolean" },

    };
    var colonneKendoGrid = [
        { field: "Sa_Descr", title: "Centro Aziendale", daDuplicare: true, editor: centroAziendale_DropDownEditor, filterable: { multi: true, search: true } },
        { field: "Lav_Descr", title: "Tipo Documento", daDuplicare: true, editor: lav_DropDownEditor, filterable: { multi: true, search: true } },
        { field: "TipoFattura_Descr", title: "Tipo Fattura", daDuplicare: true, editor: tipoFattura_DropDownEditor, filterable: { multi: true, search: true } },
        {
            field: "Vincolante", title: "Vincolante", template: "#=(Vincolante ? 'Si' : 'No')#", editor: booleanEditor, width: 90, attributes: {
                style: "text-align: center;", daDuplicare: true
            }, filterable: { multi: true, search: true }
        },
        { field: "NumTipo_Descr", title: "Default Tipo Numeratore", daDuplicare: true, editor: numTipo_DropDownEditor, filterable: { multi: true, search: true } },
        { field: "Sezionale_Descr", title: "Default Sezionale", daDuplicare: true, editor: sezionale_DropDownEditor, filterable: { multi: true, search: true } },
        { field: "CausaleDoc_Descr", title: "Default Causale", daDuplicare: true, editor: casualeDoc_DropDownEditor, filterable: { multi: true, search: true } }
        
    ];
    var parametriPerLettura = null;
    var parametriDataSource = {};

    var colCustKendoGrid = [
        {
            command: [
                {
                    iconClass: "fa fa-pencil fa-xs", className: "blockModifica", name: "edit", text: { edit: "", update: "Conf.", cancel: "Ann.",  }
                },
                {
                    iconClass: "fa fa-trash fa-xs", className: "blockCancella", name: "destroy", text: ""
                }
            ],
            title: "Operazioni", width: "220px"
        }
    ];

    // Se l'utente non è abilitato in modifica non mostro il pulsante di duplicazione
    // Le colonne modifica / cancellazione e inserimento nuova riga sono già gestite nel GiasBase
    if (UteAbilitatoInsMod) {
        colCustKendoGrid[0].command.push(
            {
                iconClass: "fa fa-files-o fa-xs", className: "blockDuplica", name: "duplica", text: "", click: duplicaRigaKendoGridDocumentoDefault
            });
    }

    var parametriKendoGrid = {
        editable: {
            mode: "inline"
        },
        colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"] },
        columnMenu: true,
        reorderable: true
        //,
        //filterable: {
        //    mode: "row"
        //},
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditDocumentoDefault, funzioneDaChiamareDopoDelete: onDeleteNumeratoreTipo };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = ["Sa_Descr", "Lav_Descr"];

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

function booleanEditor(container, options) {
    var guid = kendo.guid();
    $('<input class="k-checkbox" id="' + guid + '" type="checkbox" name="' + options.field + '" data-type="boolean" data-bind="checked:' + options.field + '">').appendTo(container);
    $('<label class="k-checkbox-label" for="' + guid + '">&#8203;</label>').appendTo(container);
}

function centroAziendale_DropDownEditor(container, options) {

    var dde = creaDropDownEditor(container, "Sa_Descr", "Sa_Cod", elencoCentriAziendali, changeCentroAziendale);
    if (!options.model.isNew()) {
        dde.enable(false);
    }
}
function changeCentroAziendale(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_numeratore_defaults").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Sa_Cod = dataItem.Sa_Cod;
    model.Sa_Descr = dataItem.Sa_Descr;
}


function sezionale_DropDownEditor(container, options) {

    var dde = creaDropDownEditor(container, "Sezionale_Descr", "Sezionale_Cod", elencoSezionali, changeSezionale);
}
function changeSezionale(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_numeratore_defaults").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Sezionale_Cod = dataItem.Sezionale_Cod;
    model.Sezionale_Descr = dataItem.Sezionale_Descr;
    if (!model.isNew()) {
        model.dirty = true;
    }
}

function casualeDoc_DropDownEditor(container, options) {
    var elencoCausaliValide = ElencoCausaliValide(options.model.Lav_Cod);
    var dde = creaDropDownEditor(container, "CausaleDoc_Descr", "CausaleDoc_Cod", elencoCausaliValide, changeCausaleDoc);
}

function ElencoCausaliValide(lavCod) {
    var elencoCausaliValide = [];
    console.log(lavCod);
    if (elencoCausaliTrasporto != undefined && elencoCausaliTrasporto != null && elencoCausaliTrasporto.length > 0) {
        elencoCausaliValide = elencoCausaliTrasporto.filter(e => e.DocumentiValidi != null &&
            e.DocumentiValidi.indexOf(lavCod) > -1);
    }
    return elencoCausaliValide;
}

function changeCausaleDoc(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_numeratore_defaults").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.CausaleDoc_Cod = dataItem.CausaleDoc_Cod;
    model.CausaleDoc_Descr = dataItem.CausaleDoc_Descr;
    if (!model.isNew()) {
        model.dirty = true;
    }
}

function numTipo_DropDownEditor(container, options)
{
    var dde = creaDropDownEditor(container, "NumTipo_Descr", "NumTipo_Cod", elencoNumeratoriTipo, changeNumTipo);
}
function changeNumTipo(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_numeratore_defaults").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.NumTipo_Cod = dataItem.NumTipo_Cod;
    model.NumTipo_Descr = dataItem.NumTipo_Descr;
    if (!model.isNew()) {
        model.dirty = true;
    }
}

function lav_DropDownEditor(container, options) {

    var dde = creaDropDownEditor(container, "Lav_Descr", "Lav_Cod", elencoTipiDocumento, changeLav);
    if (!options.model.isNew()) {
        dde.enable(false);
    }
}
function changeLav(e) {

    var lav_Cod = undefined;
    var dataItem = e.sender.dataItem();
    var grid = $("#tab_numeratore_defaults").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    var tipoFatturaDde = $('input[name$="TipoFattura_Cod"]').data("kendoDropDownList")
    var causaleDocDde = $('input[name$="CausaleDoc_Cod"]').data("kendoDropDownList")

    lav_Cod = dataItem.Lav_Cod;
    model.Lav_Cod = lav_Cod;
    model.Lav_Descr = dataItem.Lav_Descr;
    model.TipoFattura_Cod = " "
    model.TipoFattura_Descr = "";

    if (lav_Cod === 1000 || lav_Cod === 1001) {
        
        var tipiFattura = elencoTipiFattura.filter(function (tf) {
            return tf.Lav_Cod == lav_Cod;
        });
        tipoFatturaDde.setDataSource(tipiFattura);
    }
    else
    {
        var tipiFattura = [{ "Lav_Cod": "1000", "TipoFattura_Cod": " ", "TipoFattura_Descr": "" }]
        tipoFatturaDde.setDataSource(tipiFattura);
    }

    if (lav_Cod > 0) {
        var elenco = ElencoCausaliValide(lav_Cod);
        causaleDocDde.setDataSource(elenco);
    }
    
}

function tipoFattura_DropDownEditor(container, options) {

    var elenco = [
        { "Lav_Cod": "-99", "TipoFattura_Cod": " ", "TipoFattura_Descr": "" },
    ];

    var dde = creaDropDownEditor(container, "TipoFattura_Descr", "TipoFattura_Cod", elenco, changeTipoFattura);
    if (!options.model.isNew()) {
        dde.enable(false);
    }
}
function changeTipoFattura(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_numeratore_defaults").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.TipoFattura_Cod = dataItem.TipoFattura_Cod;
    model.TipoFattura_Descr = dataItem.TipoFattura_Descr;
}


function onEditDocumentoDefault(e)
{
    //e.container.find("input[name='Sigla']").attr('maxlength', '10');
    //e.container.find("input[name='Descrizione']").attr('maxlength', '255');

    duplica_Effettivo_Righe_KendoGrid(e, rigaDuplicataGridDocumentoDefault, rigaDaCopiareGridDocumentoDefault);

}

function grid_cellClose(e)
{
    input = e.container.find("input[name='Validita_Fine']").data("kendoDatePicker");
    if (input != undefined) {
        if (input.value() == "" || input.value() == undefined || input.value() == null)
            e.model.Validita_Fine = new Date("2100/12/31")
    }

    input = e.container.find("input[name='Validita_Inizio']").data("kendoDatePicker");
    if (input != undefined) {
        if (input.value() == "" || input.value() == undefined || input.value() == null)
            e.model.Validita_Inizio = new Date("1900/01/01")
    }
}

function textAreaEditor(container, options) {
    $('<textarea class="k-textbox" name="' + options.field + '" style="width:100%;height:100px;" />').appendTo(container);
}

function duplicaRigaKendoGridDocumentoDefault(e) {

    var grid = $("#tab_numeratore_defaults").data("kendoGrid");
    var row = $(e.target).closest("tr");

    var hasChanges = grid.dataSource.hasChanges();

    if (!hasChanges) {

        e.preventDefault();
        rigaDaCopiareGridDocumentoDefault = grid.dataItem(row);
        rigaDuplicataGridDocumentoDefault = true;
        grid.addRow();
    }
    else {
        alert("Sono presenti righe non salvate: procedere prima con il salvataggio");
    }
}


function onDeleteNumeratoreTipo(e) {
    //var grid = $("#tab_numeratore_tipo").data("kendoGrid");
    //var row = $(e.target).closest("tr");
    //var dataItem = grid.dataItem(row);
    //var Tipo = dataItem.Tipo;

    
}






