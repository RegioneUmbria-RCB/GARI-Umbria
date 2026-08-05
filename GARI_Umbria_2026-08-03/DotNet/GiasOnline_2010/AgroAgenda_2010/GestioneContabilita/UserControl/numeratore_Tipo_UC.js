// ************************************************************************************
// ************************************************************************************

function Popola_Numeratori_Tipo(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: CaricaNumeratoriTipo,
        funzioneSubmit: { funzione: SubmitNumeratoreTipo, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };
    var idModel = "Key_Numeratore_Tipo";
    var campiKendoModel = {
        key_numeratore_tipo: { editable: false, type: "string" },
        Piva: { editable: false, type: "string" },
        PivaSuperUser: { editable: false, type: "string" },
        Tipo: { editable: false, type: "number" },
        Sigla: {
            editable: true,
            type: "string",
            validation: { required: true }
        },
        Descrizione: { editable: true, type: "string", validation: { required: true } },
        Validita_Inizio: { editable: true, type: "date", defaultValue: AGRODATAINIZIO, validation: { required: false } },
        Validita_Fine: { editable: true, type: "date", defaultValue: AGRODATAFINE, validation: { required: false } }
    };
    var colonneKendoGrid = [
        {
            field: "Sigla", title: "Sigla", daDuplicare: true, width: "150px",
            attributes: { style: "text-align: center;" },
            headerAttributes: { style: "text-align: left;" },
            filterable: { multi: true, search: true }
        },
        {
            field: "Descrizione", title: "Descrizione", daDuplicare: true, width: "800px", editor: textAreaEditor, filterable: { multi: true, search: true }
        }
        //{ field: "Validita_Inizio", title: "Data inizio", format: "{0:dd/MM/yyyy}", daDuplicare: true },
        //{ field: "Validita_Fine", title: "Data fine", format: "{0:dd/MM/yyyy}", daDuplicare: true }

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
            title: "Operazioni", width: "220px"
        }
    ];

    // Se l'utente non è abilitato in modifica non mostro il pulsante di duplicazione
    // Le colonne modifica / cancellazione e inserimento nuova riga sono già gestite nel GiasBase
    if (UteAbilitatoInsMod) {
        colCustKendoGrid[0].command.push(
            {
                iconClass: "fa fa-files-o fa-xs", className: "blockDuplica", name: "duplica", text: "", click: duplicaRigaKendoGridNumeratori
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

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: onEditNumeratoriTipi, funzioneDaChiamareDopoDelete: onDeleteNumeratoreTipo };
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

function onEditNumeratoriTipi(e)
{
    e.container.find("input[name='Sigla']").attr('maxlength', '10');
    e.container.find("input[name='Descrizione']").attr('maxlength', '255');

    duplica_Effettivo_Righe_KendoGrid(e, rigaDuplicataGridNumeratoriTipo, rigaDaCopiareGridNumeratoriTipo);

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

function duplicaRigaKendoGridNumeratori(e) {

    var grid = $("#tab_numeratore_tipo").data("kendoGrid");
    var row = $(e.target).closest("tr");

    var hasChanges = grid.dataSource.hasChanges();

    if (!hasChanges) {

        e.preventDefault();
        rigaDaCopiareGridNumeratoriTipo = grid.dataItem(row);
        rigaDuplicataGridNumeratoriTipo = true;
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






