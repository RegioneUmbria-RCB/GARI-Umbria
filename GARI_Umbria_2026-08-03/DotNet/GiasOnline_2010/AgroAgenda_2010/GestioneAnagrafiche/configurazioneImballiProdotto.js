
function popolaTestateGrigliaImballiProdotto(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: RicercaImballiProdotto,
        funzioneSubmit: { funzione: SubmitImballiProdotto },
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"
    };
    var idModel = "Id_Config";
    var campiKendoModel = {
        Id_Config: { editable: false, type: "number" },
        Tabella_Cod: { editable: true, type: "number", validation: { required: true} },
        Tipo_Imballo_Des: { editable: true, type: "string", validation: { required: true} },
        Mat_Cod: { editable: true, type: "number", validation: { required: true} },
        Imballo_Des: { editable: true, type: "string", validation: { required: true} },
        Veg_Cod: { editable: true, type: "number", validation: { required: true} },
        Veg_Des: { editable: true, type: "string", validation: { required: true} },
        Cul_Cod: { editable: true, type: "number", validation: { required: false} },
        Cul_Des: { editable: true, type: "string", validation: { required: false} },
        Valore: { editable: true, type: "number", validation: { required: true } }
        
    };

    let titleQty = "Peso Kg";
    let formatQty = "{0:n3}";
    if (FF_gest_materiale_vivaistico) {
        titleQty = "Numero";
        formatQty = "{0:n0}";
    }
    var colonneKendoGrid = [
                            
        { field: "Tipo_Imballo_Des", title: "Tipo Bene di Confezionamento", editor: Tipo_Imballo_DropDownEditor},
        { field: "Imballo_Des", title: "Bene di Confezionamento", editor: Imballo_DropDownEditor},
        { field: "Veg_Des", title: "Specie Vegetale", editor: Specie_DropDownEditor},
        { field: "Cul_Des", title: "Varietà Colturale", editor: Varieta_DropDownEditor},
        { field: "Valore", title: titleQty, format: formatQty, editor: numberEditor3decimals }

    ];


    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = { salvaRipristinaPersonalizzazioni: { url: pathCoreWS }};
    var funzioniPrimaDopoEventi = {};
    var mostraRigheCancellate = true;
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
}


function Tipo_Imballo_DropDownEditor(container, options) {

    creaDropDownEditor(container, "Tipo_Imballo_Des", "Tabella_Cod", Tipo_Imballaggio, changeTipoImballaggio);
}

function changeTipoImballaggio(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_griglia_configurazioneimballiprodotto").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Tabella_Cod = dataItem.Tabella_Cod;
    model.Tipo_Imballo_Des = dataItem.Tipo_Imballo_Des;

    model.mat_cod = 0;
    model.imballo_des = "";

    kendoFastRedrawRow(grid, row);

}

function Imballo_DropDownEditor(container, options) {

    var Tabella_Cod = options.model.Tabella_Cod;

    switch (Tabella_Cod) {

        case 4: //Imballaggio

            creaDropDownEditor(container, "imballo_des", "mat_cod", Elenco_Imballaggi, changeImballaggio);
            break;

        case 8: //Contenitore
            creaDropDownEditor(container, "imballo_des", "mat_cod", Elenco_Contenitori, changeImballaggio);
            break;

        case 5: //Confezione
            creaDropDownEditor(container, "imballo_des", "mat_cod", Elenco_Confezioni, changeImballaggio);
            break;
    }
}

function changeImballaggio(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_griglia_configurazioneimballiprodotto").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    
    model.Mat_Cod = dataItem.mat_cod;
    model.Imballo_Des = dataItem.imballo_des;

}

function Specie_DropDownEditor(container, options) {

    creaDropDownEditor(container, "Veg_Des", "Veg_Cod", Elenco_Specie, changeSpecieVegetale);

    // Imposto come default la riga vuota
    options.model.Veg_Cod = -1;
}

function changeSpecieVegetale(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_griglia_configurazioneimballiprodotto").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Veg_Cod = dataItem.Veg_Cod;
    model.Veg_Des = dataItem.Veg_Des;

    kendoFastRedrawRow(grid, row);
}


function Varieta_DropDownEditor(container, options) {
    
    var Veg_Cod = options.model.Veg_Cod;
    
    if (Veg_Cod != 0) {
        Elenco_Varieta = RicercaVarieta($(cIdPiva).val(), Veg_Cod);
        creaDropDownEditor(container, "Cul_Des", "Cul_Cod", Elenco_Varieta, changeVarieta);
    }

    options.model.Cul_Cod = 0;

}

function changeVarieta(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_griglia_configurazioneimballiprodotto").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Cul_Cod = dataItem.Cul_Cod;
    model.Cul_Des = dataItem.Cul_Des;

}

