
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////     GRIGLIE    ///////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////



function ConfiguraGrigliaContrattiPomodoro(IDControllo, Key_Padre, AggiornaImpianti) {

    var omettiAnnulla = true;
    
    var funzioneSubmitDaUsare = null;
    //if (getKendoSwitch("cbDettDistinta")) {
    //    funzioneSubmitDaUsare = { funzione: null, flagInsert: false, flagUpdate: false, flagDelete: false };
    //}
    //else {
    funzioneSubmitDaUsare = { flagInsert: false, flagUpdate: true, flagDelete: true };
    //}
    var funzioniCRUD = {
        funzioneRead: RicercaContratti,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false
    }; 

    var idModel = "Contratto_Cod";
    var campiKendoModel = null;
    
    campiKendoModel = {
        Contratto_Cod: { editable: false, type: "number" },
        Contratto_Numero: { editable: false, type: "string", validation: { required: true } },
        Anno: { editable: false, type: "number", validation: { required: true } },
        Contratto_Nome: { editable: false, type: "string" },
        Rag_Soc: { editable: false, type: "string" },
        Superficie_Prevista: { editable: false, type: "number" },
        Resa_Prevista: { editable: false, type: "number" }
        
    };
    
    colonneKendoGrid = [
        { field: "Contratto_Numero", title: "Numero", filterable: { multi: true, search: true } },
        { field: "Anno", title: "Anno", filterable: { multi: true, search: true } },
        { field: "Contratto_Nome", title: "Titolo", filterable: { multi: true, search: true } },
        { field: "Rag_Soc", title: "Ragione Sociale", filterable: { multi: true, search: true } },
        { field: "Superficie_Prevista", title: "Superficie Prevista", filterable: { multi: true, search: true } },
        { field: "Resa_Prevista", title: "Resa Prevista", filterable: { multi: true, search: true } }
    ]
   
    
    var parametriPerLettura = [];
    var parametriDataSource = {};

   

    //var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoSave: HideTabDettagli, funzioneDaChiamareDopoDelete: HideTabDettagli };
    //if (getKendoSwitch("cbDettDistinta")) {
    //funzioniPrimaDopoEventi = { funzioneDaChiamareDopoChange: rigaSplitSelezionata, funzioneDaChiamareDopoDataBound: CheckDistinteChiuse};
    //}




    var colCustKendoGrid = [{
        command: [
            {
                iconClass: "fa fa-external-link fa-lg",//fa-pencil-square-o fa-external-link
                className: "e_link_contab",
                name: "e_link_contab",
                text: "&nbsp",
                click: Modifica_Contratto
            }
            ,
            {
                iconClass: "fa fa-trash fa-lg",
                className: "destroy",
                name: "destroy_fake",
                text: "&nbsp",
                click: Cancella_Contratto
            }
        ],
        title: "Operazioni"
    }];


    var parametriKendoGrid = {
        columnMenu: true,
        pdf: false,
        groupable: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        editable: { mode: "inline" },
        colonneCustomKendoGrid: colCustKendoGrid};


   
    funzioniPrimaDopoEventi = {};// funzioneDaChiamareDopoChange: rigaSplitSelezionata };

    //}


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





function ConfiguraGrigliaClausole(IDControllo, Key_Padre) {

    var omettiAnnulla = true;

    var funzioneSubmitDaUsare = null;
    
    funzioneSubmitDaUsare = { funzione: SubmitClausole, flagInsert: true, flagUpdate: true, flagDelete: true };
    //}
    var funzioniCRUD = {
        funzioneRead: RicercaClausole,
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: false
    };

    var idModel = "Clausola_Cod";
    var campiKendoModel = null;

    campiKendoModel = {
        Piva: { editable: false, type: "string" },
        Clausola_Cod: { editable: false, type: "number" },
        Clausola_Nome: { editable: true, type: "string", validation: { required: true } },
        Clausola_Numero: { editable: true, type: "string", validation: { required: true } },
        Clausola_Des: { editable: true, type: "string", validation: { required: true } },
        Cau_Contratto: { editable: true, type: "string" },
        Cau_Contratto_Des: { editable: true, type: "string", validation: { required: true } },
        Clausola_Cod_Alternativo: { editable: true, type: "string", validation: { required: true } }
        
    };

    colonneKendoGrid = [
        { field: "Clausola_Numero", title: "Numero" },
        { field: "Clausola_Cod_Alternativo", title: "Codice" },     
        { field: "Clausola_Nome", title: "Clasuola Contrattuale" },           
        { field: "Clausola_Des", title: "Dettaglio"},     
        { field: "Cau_Contratto_Des", title: "Causale", editor: Causale_Contratto_DropDownEditor }
    ]



    //var colCustKendoGrid = [{
    //    command: [
    //        {
    //            iconClass: "fa fa-external-link fa-lg",//fa-pencil-square-o fa-external-link
    //            className: "e_link",
    //            name: "e_link",
    //            text: "&nbsp",
    //            click: SubmitGridClausole
    //        }
    //        ,
    //        {
    //            iconClass: "fa fa-trash fa-lg",
    //            className: "destroy",
    //            name: "destroy",
    //            text: "&nbsp"
    //        }
    //    ],
    //    title: "Operazioni"
    //}];

    var parametriPerLettura = [];
    var parametriDataSource = {};


    

    var parametriKendoGrid = {
        //columnMenu: false,
        //editable: { mode: "inline" },
        //colonneCustomKendoGrid: colCustKendoGrid
    }; // { salvaRipristinaPersonalizzazioni: { url: pathCoreWS }};

    var funzioniPrimaDopoEventi = {}; // funzioneDaChiamareDopoDataBound: kendo_Operazioni_onDataBoundedRighe_CDG };

    var funzioniPrimaDopoDatabound = {};
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





function Causale_Contratto_DropDownEditor(container, options) {
    
    creaDropDownEditor(container, "Cau_Contratto_Des", "Cau_Contratto", Elenco_Causale, changeCausale_Contratto);
  
}

function changeCausale_Contratto(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#tab_griglia_clausole").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Cau_Contratto = dataItem.Cau_Contratto;
    model.Cau_Contratto_Des = dataItem.Cau_Contratto_Des;
    

    kendoFastRedrawRow(grid, row);
}







function RiempiContrattoAnno(options) {
    options.success(Elenco_Anno);
}

function RiempiContrattoConferente(options) {
    options.success(Elenco_Conferente);
}


function RiempiContrattoProdotto(options) {
    options.success(Elenco_Prodotto);
}




function Nuovo_Contratto_Pomodoro() {

    window.location = "./Contratti_Pomodoro.aspx?p=" + $(cPiva_Codificata).val() + "&origine=./Contratti_Menu.aspx" + "&contratto_cod=0" + "&veg_cod=52";
}


function Nuovo_Contratto_Generico() {

    window.location = "./Contratti_Pomodoro.aspx?p=" + $(cPiva_Codificata).val() + "&origine=./Contratti_Menu.aspx" + "&contratto_cod=0";
}


function Ricerca_Contratti_Pomodoro() {

    ConfiguraGrigliaContrattiPomodoro("tab_griglia_contratti_pomodoro", false);
}


function Modifica_Contratto(e) {

    var di = this.dataItem($(e.currentTarget).closest("tr"));
    var Contratto_Cod = di.Contratto_Cod;
  
    window.location = "./Contratti_Pomodoro.aspx?p=" + $(cPiva_Codificata).val() + "&origine=./Contratti_Menu.aspx" + "&contratto_cod=" + Contratto_Cod;

}





// Click elimina Contratto
function Cancella_Contratto(e) {

    var di = this.dataItem($(e.currentTarget).closest("tr"));
    var contratto_cod = di.Contratto_Cod;

    $('#confermaEliminazioneDialog').kendoDialog({
        width: "400px",
        title: "Gestione Contratti Conferimento",
        closable: false,
        modal: true,
        visible: false,
        content: "<p>Eliminare definitivamente questo contratto di conferimento?<p>",
       
        actions: [
            {
                text: "Conferma", action: function (e) {

                    if (Elimina_Contratto(contratto_cod) === "") {

                        ConfiguraGrigliaContrattiPomodoro("tab_griglia_contratti_pomodoro", false);

                    }

                    else {

                        MessaggioErrore_Bootstrap("Il contratto non può essere cancellato.", "DIV_Messaggi");

                    }
                }
            },
            { text: "Annulla", primary: true }
        ]
    });
    $("#confermaEliminazioneDialog").data("kendoDialog").open();


}



function SalvaClausole(FlagEsci) {

    bDatiNecessariInseriti = true;

    var griglia_clausole = $("#tab_griglia_clausole").data("kendoGrid");
    griglia_clausole.saveChanges();

    if (bDatiNecessariInseriti == true) {

        // Aggiornamento effettivo
        AggiornaEffettivoClausole(false);

    }
}



function SubmitClausole(options) {

    let errMess = "";
    let found = false;
    bDatiNecessariInseriti = true;
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var grid = $("#tab_griglia_clausole").data("kendoGrid");

    var currentData = grid.dataSource.data();

    for (var i = 0; i < currentData.length; i++) {

        errMess = controllaRigheValidePerSubmitGrid_Clausole(options.data.created, "");
        errMess = controllaRigheValidePerSubmitGrid_Clausole(options.data.updated, errMess);

    }

    if (errMess != "") {
        MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");

    }
    else {

        var currentData = grid.dataSource.data();
    

        for (let i = 0; i < currentData.length; i++) {
            if (currentData[i].isNew()) {
                newRecords.push(currentData[i].toJSON());
            } else if (currentData[i].dirty) {
                updatedRecords.push(currentData[i].toJSON());
            }

        }


        for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
            deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
        }

        if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

            // Variabili globali
            righeInseriteGrid_Clausole = kendoEscapeOggetto(newRecords);
            righeModificateGrid_Clausole = kendoEscapeOggetto(updatedRecords);
            righeCancellateGrid_Clausole = kendoEscapeOggetto(deletedRecords);


        }
    }

}
