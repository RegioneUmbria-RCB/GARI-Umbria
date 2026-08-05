
function Popola_Numeratori_Prefisso_Suffisso(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: CaricaNumeratoriPF,
        funzioneSubmit: { funzione: SubmitNumeratorePF, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };
    var idModel = "Id";
    var campiKendoModel = {
        Id: { editable: false, type: "number" },
        Piva: { editable: true, type: "string" },
        PivaSuperUser: { editable: true, type: "string" },
        NumTipo_Cod: { editable: true, type: "number", validation: { required: true } },
        NumTipo_Descr: { editable: true, type: "string", validation: { required: true } },
        Descrizione: { editable: true, type: "string", validation: { required: true } },
        Doc_Numero_Sin: { editable: true, type: "string", validation: { required: false } },
        Doc_Numero_Des: { editable: true, type: "string", validation: { required: false } },
        Lunghezza_Centro: { editable: true, type: "number" }, validation: { min: 0 },
        CarattereFormattazione: { editable: true, type: "string", validation: { required: false } },
        Validita_Inizio: { editable: true, type: "date", defaultValue: new Date("1900/01/01"), validation: { required: false } },
        Validita_Fine: { editable: true, type: "date", defaultValue: new Date("2100/12/31"), validation: { required: false } }
    };
    var colonneKendoGrid = [
        { field: "NumTipo_Descr", title: "Numeratore", daDuplicare: true, width: 150, editor: numeratoreTipo_DropDownEditor, filterable: { multi: true, search: true } },
        { field: "Descrizione", title: "Descrizione", daDuplicare: true, width: 250, editor: textAreaEditor, filterable: { multi: true, search: true } },
        { field: "Doc_Numero_Sin", title: "Prefisso", daDuplicare: true, width: 100, filterable: { multi: true, search: true } },
        { field: "Doc_Numero_Des", title: "Suffisso", daDuplicare: true, width: 100, filterable: { multi: true, search: true } },
        {
            field: "Lunghezza_Centro", title: "Lunghezza Centro", daDuplicare: true, attributes: {
                style: "text-align: center;"
            }, width: 105, editor: numericEditor, filterable: { multi: true, search: true }
        },
        {
            field: "CarattereFormattazione", title: "Carattere Format.", daDuplicare: true, attributes: {
                style: "text-align: center;"
            }, width: 112, filterable: { multi: true, search: true }
        },
        { field: "Validita_Inizio", title: "Data inizio", format: "{0:dd/MM/yyyy}", daDuplicare: true, width: 120 },
        { field: "Validita_Fine", title: "Data fine", format: "{0:dd/MM/yyyy}", daDuplicare: true, width: 120 }

    ];
    var parametriPerLettura = null;
    var parametriDataSource = {};

    var colCustKendoGrid = [
        {
            command: [
                {
                    iconClass: "fa fa-pencil fa-xs", className: "blockModifica", name: "edit", text: { edit: "", update: "Conf.", cancel: "Ann." }
                },
                {
                    iconClass: "fa fa-trash fa-xs", className: "blockCancella", name: "destroy", text: ""
                }
            ],
            title: "Operazioni", width: "226px"
        }
    ];

    // Se l'utente non è abilitato in modifica non mostro il pulsante di duplicazione
    // Le colonne modifica / cancellazione e inserimento nuova riga sono già gestite nel GiasBase
    if (UteAbilitatoInsMod) {
        colCustKendoGrid[0].command.push(
            {
                iconClass: "fa fa-files-o fa-xs", className: "blockDuplica", name: "duplica", text: "", click: duplicaRigaKendoGridNumPs
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

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditNumeratoriPF, funzioneDaChiamareDopoDelete: onDeleteNumeratoriPF };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = [];
    //var colonneDisabilitateSoloInModifica = ["Cod_RisUm", "Rag_Soc", "Veg_Cod", "Veg_Des", "Cul_Cod", "Cul_Des", "qualita_cod", "qualita_des", "certif_cod", "certif_des", "validita_inizio", "validita_fine"];
    //var colonneDisabilitateSoloInModifica = ["Cod_RisUm", "Rag_Soc", "Veg_Cod", "Veg_Des", "Cul_Cod", "Cul_Des", "qualita_cod", "qualita_des", "certif_cod", "certif_des"];

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

function numericEditor(container, options)
{
    $('<input name="' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            format: "{0:n0}",
            decimals: 0
        });
    
}

function numeratoreTipo_DropDownEditor(container, options) {

    creaDropDownEditor(container, "NumTipo_Descr", "NumTipo_Cod", elencoNumeratoriTipo, changeNumeratoreTipo);
}

function changeNumeratoreTipo(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_numeratore_prefisso_suffisso").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.NumTipo_Cod = dataItem.NumTipo_Cod;
    model.NumTipo_Descr = dataItem.NumTipo_Descr;
}


function onEditNumeratoriPF(e)
{
    e.container.find("input[name='Descrizione']").attr("maxlength", "250");
    e.container.find("input[name='Doc_Numero_Sin']").attr("maxlength", "50");
    e.container.find("input[name='Doc_Numero_Des']").attr("maxlength", "50");
    e.container.find("input[name='CarattereFormattazione']").attr("maxlength", "50");

    duplica_Effettivo_Righe_KendoGrid(e, rigaDuplicataGridNumPS, rigaDaCopiareGridNumPS);
}

function onDeleteNumeratoriPF(e) {

}



function grid_cellClose(e)
{
    let input = e.container.find("input[name='Validita_Fine']").data("kendoDatePicker");
    if (input != undefined) {
        if (input.value() == "" || input.value() == undefined || input.value() == null)
            e.model.Validita_Fine = AGRODATAFINE;
    }

    input = e.container.find("input[name='Validita_Inizio']").data("kendoDatePicker");
    if (input != undefined) {
        if (input.value() == "" || input.value() == undefined || input.value() == null)
            e.model.Validita_Inizio = AGRODATAINIZIO;
    }
}

function textAreaEditor(container, options) {
    $('<textarea class="k-textbox" name="' + options.field + '" style="width:100%;height:100px;" />').appendTo(container);
}

function duplicaRigaKendoGridNumPs(e) {

    var grid = $("#tab_numeratore_prefisso_suffisso").data("kendoGrid");
    var row = $(e.target).closest("tr");

    var hasChanges = grid.dataSource.hasChanges();

    if (!hasChanges) {

        e.preventDefault();
        rigaDaCopiareGridNumPS = grid.dataItem(row);
        rigaDuplicataGridNumPS = true;
        grid.addRow();

    }
    else {
        alert("Sono presenti righe non salvate: procedere prima con il salvataggio");
    }
}








