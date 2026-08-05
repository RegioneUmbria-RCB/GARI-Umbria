
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////     GRIGLIE    ///////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function ConfiguraGrigliaIndici(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: Ricerca_Indici,    
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True"
    };


    var idModel = "ID_Indice";
    var campiKendoModel = {
        ID_Indice: { editable: false, type: "string" },
        TitoloIndice: { editable: false, type: "string", validation: { required: false } },
        TipoCampo_Des: { editable: false, type: "string", validation: { required: false } },
        Obbligatorio: { editable: false, type: "string", validation: { required: false } },
        Validita_Inizio_Short: { editable: false, type: "string", validation: { required: false } },
        Validita_Fine_Short: { editable: false, type: "string", validation: { required: false } },
    };
    var colonneKendoGrid = [
        { field: "TitoloIndice", title: TraduzioneMultiResx(resxRicercaIndiciDocumentaliUC, "Titolo", "Titolo"), filterable: { multi: true, search: true } },        
        { field: "TipoCampo_Des", title: TraduzioneMultiResx(resxRicercaIndiciDocumentaliUC, "Tipo", "Tipo"), filterable: { multi: true, search: true } },      
        { field: "Obbligatorio", title: TraduzioneMultiResx(resxRicercaIndiciDocumentaliUC, "Obbligatorio", "Obbligatorio"), filterable: { multi: true, search: true } },
        { field: "Validita_Inizio_Short", title: TraduzioneMultiResx(resxRicercaIndiciDocumentaliUC, "ValiditàInizio", "Validita Inizio"), filterable: { multi: false, search: false } },
        { field: "Validita_Fine_Short", title: TraduzioneMultiResx(resxRicercaIndiciDocumentaliUC, "ValiditàFine", "Validita Fine"), filterable: { multi: false, search: false } },
        
    ];


    var colCustKendoGrid = [{
        command: [
            {
                iconClass: "fa fa-external-link fa-lg",//fa-pencil-square-o fa-external-link
                className: "e_link",
                name: "e_link",
                text: "&nbsp",
                click: Modifica
            }
            ,
            {
                iconClass: "fa fa-trash fa-lg",
                className: "destroy",
                name: "destroy_fake",
                text: "&nbsp",
                click: Cancella
            }
        ],
        title: TraduzioneMultiResx(resxRicercaIndiciDocumentaliUC, "Operazioni", "Operazioni")
    }];

    //Anna 31/05/22 Se l'utente non è abilitato alla modifica rimuovo solo la possibilità di cancellazione
    //rimane l'altro pulsante per visualizzare l'elemento in lettura (ci sono controlli successivi per disattivare la modfifca effetiva)
    if ($("input[name$='hf_UtenteAbilitatoScrittura']").val() === "False") {
        colCustKendoGrid[0].command.splice(1, 1)
    }

    var parametriPerLettura = [];
    var parametriDataSource = {};
    var colonneDisabilitateSoloInModifica = [];
   
    var parametriKendoGrid = {
        //columnMenu: false,
        colonneCustomKendoGrid: colCustKendoGrid,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 }
    }; // { salvaRipristinaPersonalizzazioni: { url: pathCoreWS }};


    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: kendo_Operazioni_onDataBoundedRighe_Alert };
    
    var funzioniPrimaDopoDatabound = {};
    var mostraRigheCancellate = false;
    

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






function Nuovo(e) {

    var id_indice = 0;

    window.location = "./Indici_Documentale.aspx?p=" + $(cPiva_Codificata).val() + "&id_indice=" + id_indice + "&origine=./Scad_Anagrafiche.aspx";

}




function Modifica(e) {

    var di = this.dataItem($(e.currentTarget).closest("tr"));
    var id_indice = di.ID_Indice;
   
    window.location = "./Indici_Documentale.aspx?p=" + $(cPiva_Codificata).val() + "&id_indice=" + id_indice + "&origine=./Scad_Anagrafiche.aspx";

   
}



// Click elimina
function Cancella(e) {

    var di = this.dataItem($(e.currentTarget).closest("tr"));
    var id_indice = di.ID_Indice;

    $('#confermaEliminazioneDialog').kendoDialog({
        width: "400px",
        title: TraduzioneMultiResx(resxRicercaIndiciDocumentaliUC, "GestioneIndiciDocumentali", "Gestione Indici Documentali"),
        closable: false,
        modal: true,
        visible: false,
        content: "<p>" + TraduzioneMultiResx(resxRicercaIndiciDocumentaliUC, "ConfermaEliminazioneIndiceDocumentale", "Eliminare definitivamente questo indice documentale?") + "<p>",
        actions: [
            {
                text: TraduzioneMultiResx(resxRicercaIndiciDocumentaliUC, "Conferma", "Conferma"), action: function (e) {

                    if (Elimina_Indice(id_indice) === "") {

                        ConfiguraGrigliaIndici("tab_griglia_Indici");

                    }

                    else {

                        MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxRicercaIndiciDocumentaliUC, "IndiceDocumentaleAssociatoScadenzarioNonEliminabile", "L'indice documentale non può essere eliminato poiché associato allo scadenzario."), "DIV_Messaggi");

                    }
                }
            },
            { text: TraduzioneMultiResx(resxRicercaIndiciDocumentaliUC, "Annulla", "Annulla"), primary: true }
        ]
    });
    $("#confermaEliminazioneDialog").data("kendoDialog").open();


}





function kendo_Operazioni_onDataBoundedRighe_Alert(e) {

    coloraRigheAlert("#tab_griglia_Indici", e);

    var griglia_Indici = $("#tab_griglia_Indici").data("kendoGrid");
    //griglia_Indici.autoFitColumn(0);
    //griglia_Indici.autoFitColumn(1);

}

function coloraRigheAlert(grid_elem, eventArgs) {

    var grid = $(grid_elem).data('kendoGrid');
    var items = eventArgs.sender.items();

    items.each(function (index) {

        var dataItem = grid.dataItem(this);

        if (dataItem.ID_Indice < 0) {
           //Coloro gli indici riservati
            this.className += " kendoRiga_Arancione";    

            //Se è una delle nostre categorie, nascondo il pulsante di cancellazione
            if (parseInt($(cRiservato).val()) == 0) {
                $(this.querySelector(".destroy")).hide();
            }

        }

    });

}