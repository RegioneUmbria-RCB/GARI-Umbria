
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////     GRIGLIE    ///////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function ConfiguraGrigliaDettagli(IDControllo, bRead) {

    funzioneSubmitDaUsare = { funzione: SubmitIndice, flagInsert: true, flagUpdate: false, flagDelete: true };

    var Abilitazione = "True";
    var AbilitazioneBoolean = true;
    if (bBloccaControlli) {
        Abilitazione = "False";
        AbilitazioneBoolean = false;
    }
    var funzioniCRUD = {
        funzioneRead: Ricerca_Dettagli,   
        funzioneSubmit: funzioneSubmitDaUsare,
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == Abilitazione,
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == Abilitazione,
        omettiPulsantiSalva: true
    };
    var idModel = "ID_Indice_Det";
    var campiKendoModel = {
        ID_Indice_Det: { editable: false, type: "number" },
        Valore: { editable: AbilitazioneBoolean, type: "string" },  
        Validita_Inizio: { editable: AbilitazioneBoolean, type: "date", defaultValue: new Date("1900/01/01"), validation: { required: false }},
        Validita_Fine: { editable: AbilitazioneBoolean, type: "date", defaultValue: new Date("2100/12/31"), validation: { required: false }}
        
    };
    var colonneKendoGrid = [
        { field: "Valore", title: TraduzioneMultiResx(resxObj , "Valore", "Valore") },
        { field: "Validita_Inizio", title: TraduzioneMultiResx(resxObj , "ValiditàInizio", "Validità Inizio"), format: "{0:dd/MM/yyyy}" },
        { field: "Validita_Fine", title: TraduzioneMultiResx(resxObj , "ValiditàFine", "Validità Fine"), format: "{0:dd/MM/yyyy}" } 
    ];

    var colCustKendoGrid = [];
    var parametriPerLettura = [bRead];
    var parametriDataSource = {};
    var colonneDisabilitateSoloInModifica = [];
   
    var parametriKendoGrid = {
        //columnMenu: false,
      // editable: { mode: "inline" },
       colonneCustomKendoGrid: colCustKendoGrid
    }; // { salvaRipristinaPersonalizzazioni: { url: pathCoreWS }};
    
    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: kendo_Operazioni_onDataBoundedRighe_Dettagli };
    
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



function RiempicmbArea(options) {
    options.success(Elenco_Aree);
}


function RiempicmbTipologia(options) {
    options.success(Elenco_Tipologie);
}


function RiempicmbTipoCampo(options) {
    options.success(Elenco_TipoCampo);
}

function RiempicmbElenco(options) {
    options.success(Elenco_Tipo);
}


function RiempicmbElenco_Valore(options) {
    options.success(Elenco_Valore);
}


function RiempicmbLibero(options) {
    options.success(Elenco_Libero);
}


function CmbArea_change(options) {

    var id_area = KendoDDL("cmbArea").dataItem().ID_Area;

    Elenco_Tipologie = Elenco_Tipologie_Riempi(id_area);

    $("#cmbTipologia").data("kendoMultiSelect").dataSource.read();

}



function CmbTipo_change(options) {
    
    switch (KendoDDL("cmbTipo").dataItem().TipoCampo) {
        case 0: //Libera Imputazione
            $("#tab_griglia_dettagli").attr("style", "display:none");     
            $("#id_elenco").hide();
            $("#id_elenco_valore").hide();
            $("#id_libero").show();
            break;
        case 1: //Valori
            $("#tab_griglia_dettagli").attr("style", "display:inline-block");
            $("#id_elenco").hide();
            $("#id_elenco_valore").hide();
            $("#id_libero").hide();
            break;
        case 2: //Elenco
            $("#tab_griglia_dettagli").attr("style", "display:none");
            $("#id_elenco").show();
            $("#id_elenco_valore").hide(); //.show();
            $("#id_libero").hide();
            break;
    }


}


function CmbElenco_change(options) {


}


function CmbElenco_Valore_change(options) {

  
}


function CmbLibero_change(options) {
    
}

function kendo_Operazioni_onDataBoundedRighe_Dettagli(e) {

    var griglia_dettagli = $("#tab_griglia_dettagli").data("kendoGrid");
    griglia_dettagli.autoFitColumn(0);
    
}






function SubmitIndice(options) {

    let errMess = "";
    let found = false;
    bDatiNecessariInseriti = true;
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var grid = $("#tab_griglia_dettagli").data("kendoGrid");

    var currentData = grid.dataSource.data();

    var titoloindice = $('input[name$="txt_titolo_indice"]').val();

    if (titoloindice === "") {
        MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj , "TitoloNonImpostatoCorrettamente", "Titolo non impostato correttamente."), "DIV_Messaggi");
        bDatiNecessariInseriti = false;        
        return 0;
    }

    
    //var id_area = KendoDDL("cmbArea").dataItem().ID_Area;

    //if (id_area == 0 || id_area == undefined) {
    //    MessaggioErrore_Bootstrap("Categoria non impostata correttamente.", "DIV_Messaggi");
    //    bDatiNecessariInseriti = false;
    //    return 0;
    //}

    //Controllo Impostazione Elenco
    if (parseInt(KendoDDL("cmbTipo").dataItem().TipoCampo) == 2) {        

        //var elenco_cod = 0;
        //if (KendoDDL("cmbElenco") !== undefined) {
        //    if (KendoDDL("cmbElenco").dataItem().Elenco_Cod !== undefined) {
        //        elenco_cod = KendoDDL("cmbElenco").dataItem().Elenco_Cod;
        //    }
        //}

        //var elenco_val = 0;
        //if (KendoDDL("cmbElenco_Valore") !== undefined) {
        //    if (KendoDDL("cmbElenco_Valore").dataItem() !== undefined) {
        //        elenco_val = KendoDDL("cmbElenco_Valore").dataItem().Elenco_Val;
        //    }
            
        //}

        //if (parseInt(elenco_cod) == 0)  || parseInt(elenco_val) == 0) {
        //    bDatiNecessariInseriti = false;
        //    MessaggioErrore_Bootstrap("Impostare un valore elenco valido.", "DIV_Messaggi");

        //}

    }


    if (parseInt(KendoDDL("cmbTipo").dataItem().TipoCampo) == 1) {

        bDatiNecessariInseriti = false;

        for (var i = 0; i < currentData.length; i++) {

            bDatiNecessariInseriti = true;

            errMess = controllaRigheValidePerSubmitGrid_Dettagli(options.data.created, "");
            errMess = controllaRigheValidePerSubmitGrid_Dettagli(options.data.updated, errMess);

        }


        if (!bDatiNecessariInseriti) {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj , "ImpostareAlmenoUnValoreValidoInGriglia", "Impostare almeno un valore valido in griglia."), "DIV_Messaggi");
        } else {

            if (errMess !== "") bDatiNecessariInseriti = false;

            else {

               
                currentData = grid.dataSource.data();

                for (let i = 0; i < currentData.length; i++) {

                    //Formattazione data inizio in stringa
                    var originalDateInizio = currentData[i].Validita_Inizio;
                    var stringDateInizio = formattedDate(originalDateInizio,"/");
                    currentData[i].Validita_Inizio = stringDateInizio;

                    //Formattazione data fine in stringa
                    var originalDateFine = currentData[i].Validita_Fine;
                    var stringDateFine = formattedDate(originalDateFine, "/");
                    currentData[i].Validita_Fine = stringDateFine;

                    if (currentData[i].isNew()) {
                        newRecords.push(currentData[i].toJSON());
                    } else if (currentData[i].dirty) {
                        updatedRecords.push(currentData[i].toJSON());
                    }

                    currentData[i].Validita_Inizio = originalDateInizio;
                    currentData[i].Validita_Fine = originalDateFine;

                }


                for (var i = 0; i < grid.dataSource._destroyed.length; i++) {
                    deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
                }

                if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

                    // Variabili globali
                    righeInseriteGrid_Dettagli = kendoEscapeOggetto(newRecords);
                    righeModificateGrid_Dettagli = kendoEscapeOggetto(updatedRecords);
                    righeCancellateGrid_Dettagli = kendoEscapeOggetto(deletedRecords);



                }
            }


           }

        }

       
}

function SalvaIndice(FlagEsci) {

    bDatiNecessariInseriti = true;

    var griglia_dettagli = $("#tab_griglia_dettagli").data("kendoGrid");
    griglia_dettagli.saveChanges();

    if (bDatiNecessariInseriti == true) {

        // Aggiornamento effettivo
        AggiornaEffettivo(FlagEsci);

    }
}

