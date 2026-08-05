
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////     GRIGLIE   //////// ///////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function ConfiguraGrigliaImpianti(IDControllo, ChkSmart) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === StatoNonLettura();
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === StatoNonLettura();

    var funzioniCRUD = {
        funzioneRead: CaricaGrigliaImpianti,
        //funzioneSubmit: { funzione: SubmitGrid_Impianti, flagInsert: true, flagUpdate: false, flagDelete: false },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc 
    };
    
    var idModel = "Key";
    
    var campiKendoModel = {
        Key: { editable: false, type: "number" },
        Id_Mov_Det: { editable: false, type: "number" },
        Piva: { editable: false, type: "string", validation: { required: true } },
        SA_COD: { editable: false, type: "number" },
        Sa_Nome: { editable: false, type: "string" },
        Campo_Cod: { editable: false, type: "number", validation: { required: true } },
        Campo_Des: { editable: false, type: "string", validation: { required: true } },
        Appezza: { editable: false, type: "number", validation: { required: true } },
        App_Nome: { editable: false, type: "string", validation: { required: true } },
        ID_REG: { editable: false, type: "number", validation: { required: true } },     
        Progetto_Cod: { editable: false, type: "number", validation: { required: true } },     
        Progetto_Nome: { editable: false, type: "string", validation: { required: true } },        
        Cul_Des: { editable: false, type: "string" },
        Catasto: { editable: false, type: "string", validation: { required: true } },
        Numero_Raccolte: { editable: false, type: "number", validation: { required: true } },
        Data_Ultima_Raccolta: { editable: false, type: "string", validation: { required: true } },  
        Data_Trapianto: { editable: false, type: "string", validation: { required: true } },  
        KPIN: { editable: false, type: "string", validation: { required: false } },  
        BLOCK: { editable: false, type: "string", validation: { required: false } },  
        Sup_Imp: { editable: false, type: "number", validation: { required: true } },
        Piante: { editable: false, type: "number", validation: { required: true } },
        Qta: { editable: StatoNonLetturaBoolean(), type: "number", validation: { required: true } }
    };

    let titoloColonnaQta = TraduzioneMultiResx(resxSceltaImpiantiUC, "RisorsaQuantità", "Qta") + " Kg";
    if (FF_gest_materiale_vivaistico) {
        titoloColonnaQta = TraduzioneMultiResx(resxSceltaImpiantiUC, "Numero", "Numero");
    }
    var colonneKendoGrid = [        
        { field: "Sa_Nome", title: TraduzioneMultiResx(resxSceltaImpiantiUC, "CentroAziendale", "Centro Aziendale"), filterable: { multi: true, search: true } },
        { field: "Campo_Des", title: TraduzioneMultiResx(resxSceltaImpiantiUC, "Campo", "Campo"), filterable: { multi: true, search: true } },
        { field: "App_Nome", title: TraduzioneMultiResx(resxSceltaImpiantiUC, "Appezzamento", "Appezzamento"), filterable: { multi: true, search: true } },
        { field: "Progetto_Nome", title: TraduzioneMultiResx(resxSceltaImpiantiUC, "LottoEsercizio", "Lotto Esercizio"), filterable: { multi: true, search: true } },
        { field: "Cul_Des", title: TraduzioneMultiResx(resxSceltaImpiantiUC, "Varietà", "Varietà"), filterable: { multi: true, search: true }, hidden: FiltroSpecieVegetale() },
        { field: "Catasto", title: TraduzioneMultiResx(resxSceltaImpiantiUC, "ParticelleCatastali", "Particelle Catastali") },
        { field: "Numero_Raccolte", title: TraduzioneMultiResx(resxSceltaImpiantiUC, "NumeroRaccolte", "Numero Raccolte") },
        { field: "Data_Ultima_Raccolta", title: TraduzioneMultiResx(resxSceltaImpiantiUC, "DataUltimaRaccolta", "Data Ultima Raccolta") },
        { field: "Data_Trapianto", title: TraduzioneMultiResx(resxSceltaImpiantiUC, "DataTrapianto", "Data Trapianto") },
        { field: "KPIN", title: "KPIN" }, 
        { field: "BLOCK", title: "BLOCK" }, 
        { field: "Sup_Imp", title: TraduzioneMultiResx(resxSceltaImpiantiUC, "SuperficieAbbr", "Sup.") + " Ha", template: "#= kendo.toString(Sup_Imp, 'n2') #" },
        { field: "Piante", title: TraduzioneMultiResx(resxSceltaImpiantiUC, "NumeroPiante", "Num. Piante") },
        { field: "Qta", title: titoloColonnaQta, template: "#= kendo.toString(Qta, 'n2') #", aggregates: ["sum"], footerTemplate: "<div id='QtaImpiantiTotale'>#= kendo.toString(sum, 'n2') #</div>" } // in caso di aggiunta di colonne modificare il col number 11!!
    ];

    colonneKendoGrid.push(
        {
            attributes: { class: "OpImputazioneImpianti"},
            command: [{
                text: " ",
                name: "CalcolaSingolaRiga",
                className: "impostaSingolaRigaImpianti",
                iconClass: "fa fa-calculator fa-lg",
                click: function (e) { CalcolaSingolaRigaImpianti(e, IDControllo) }
            }],
            title: TraduzioneMultiResx(resxSceltaImpiantiUC, "Operazioni", "Operazioni"), width: "82px"
        }
    );
    
    var parametriPerLettura = [ChkSmart];    
    var parametriDataSource = {
        aggregate: [{ field: "Qta", aggregate: "sum"}]
    };

    var parametriKendoGrid = {
        groupable: false,
        editable: { mode: "incell" },
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
        //columnMenu: false,
        // colonneCustomKendoGrid: colCustKendoGrid
    };

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: onDataBoundGrigliaImpianti,
        funzioneDaChiamareDopoEdit: EditGrigliaImpianti,
        funzioneDaChiamareDopoSave: SaveGrigliaImpianti
    };
    
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

    // tooltip comandi
    $("#" + IDControllo).kendoTooltip({ filter: ".k-grid-CalcolaSingolaRiga", content: TraduzioneMultiResx(resxSceltaImpiantiUC, "ScegliSoloQuestaRiga", "Scegli solo questa riga") });


    //var grid = $("#" + IDControllo).data("kendoGrid");
    //grid.bind("cellClose", CellCloseGrigliaImpianti);

}

function CalcolaSingolaRigaImpianti(e, id) {

    if (KendoDDL("cmbRipartizione").dataItem() !== undefined &&
        KendoDDL("cmbRipartizione").dataItem() !== null) {
        let tipoRip = KendoDDL("cmbRipartizione").dataItem().Tipo_Ripartizione;

        if (parseInt(tipoRip) === 0) {
            if (StatoNonLetturaBoolean()) {

                var grid = $("#" + id).data("kendoGrid");
                let pesoNetto = kendo.parseFloat(Get_KendoNumTBValue("idKgNetti", true));
                let udm = parseInt(Get_KendoDDLValue("ddlUM"));
                switch (udm) {
                    case enum_Udm.quintali.value:
                        pesoNetto = pesoNetto * 100;
                        break;
                    case enum_Udm.tonnellate.value:
                        pesoNetto = pesoNetto * 1000;
                }
                let qtaDaDistribuire = kendo.parseFloat(pesoNetto);

                //Ricavo la riga
                var model = grid.dataItem($(e.currentTarget).closest("tr"));
                let uidRigaMod = model.uid;

                let data = grid.dataSource.data();

                for (let nrRighe = 0; nrRighe < data.length; nrRighe++) {

                    let currentDataItem = data[nrRighe];

                    if (currentDataItem.uid === uidRigaMod) {
                        //è la riga dove ho deciso di scaricare tutto il prodotto
                        currentDataItem.set("Qta", qtaDaDistribuire);
                    } else {
                        //è una delle altre righe ==> azzero il valore
                        currentDataItem.set("Qta", 0);
                    }

                    data[nrRighe].dirty = true;

                }
            }
        } else {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxSceltaImpiantiUC, "UtilizzareRipartizioneManuale", "Utilizzare modalità ripartizione manuale."), "DIV_Messaggi");
        }

    }
}


function SaveGrigliaImpianti(e) {
    //Necessario perché venga aggiornato correttamente il totale
    var grid = e.sender;
    setTimeout(function () {
        grid.refresh();
    });
}

function CellCloseGrigliaImpianti(e) {
    //console.log(e);
    if (e.type === "save") {

        let grid = $("#tab_griglia_impianti").data("kendoGrid");
        let data = grid.dataSource.data();
        let totalQta = 0;

        //Questa soluzione di fare tutto a mano fa cagarissimo, ma sembra che non si possa fare altrimenti
        $(data).each(function (index, item) {
            totalQta += item.Qta;
        });
        grid.dataSource.aggregates().Qta.sum = totalQta;
        $("#QtaImpiantiTotale").text(kendo.toString(totalQta, "n2"));
    }
}

function EditGrigliaImpianti(e) {

    var grid = $("#tab_griglia_impianti").data("kendoGrid");
    var ddl = KendoDDL("cmbRipartizione");

    // Blocco il campo lotto se non è gestito 
    if (e.container.contex !== undefined) {
   
        if (e.container.context.cellIndex === 11 && ddl.dataItem().Tipo_Ripartizione !== 0) {
            grid.closeCell();
            }
    }


}

function StatoNonLettura() {

    if (cIdTipoOp === enum_TipoOperazioneDB.Lettura.value) { 
        return "False";
    }
    else {
        return "True";
    }

}

function StatoNonLetturaBoolean() {

    if (cIdTipoOp === enum_TipoOperazioneDB.Lettura.value) {
        return false;
    }
    else {
        return true;
    }

}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////// SUBMIT SINGOLE GRIGLIE ///////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


function SubmitGrid_Impianti(options) {

    //var grid = $("#tab_griglia_impianti").data("kendoGrid");
    //var currentData = grid.dataSource.data();

    //// controllo che tutte le righe CREATE e MODIFICATE siano complete
    //var nrErr = controllaRigheValidePerSubmitGrid(options.data.created) +
    //    controllaRigheValidePerSubmitGrid(options.data.updated);


    ////Controllo Obbligatorietà Imputazione
    //var Qta = Get_KendoNumTBValue("idKgNetti");

    //if (Qta == 0 && getKendoSwitch("ChkImpiantiIndefiniti") == false && parseInt(Obbligo_Ripartizione) == 1) {

    //    MessaggioErrore_Bootstrap("Imputazione impianti non impostata correttamente.", "DIV_Messaggi");
    //    return
    //}

    ////for (var nrRighe = 0; nrRighe < currentData.length; nrRighe++) {
    ////    if (currentData[nrRighe].deleted !== true) {

    ////        qta += currentData[nrRighe].Qta;
    ////        piante += currentData[nrRighe].Piante;

    ////    }
    ////}


    //if (nrErr > 0) {
    //    if (nrErr === 1)
    //        MessaggioErrore_Bootstrap("Esiste una riga con dati non completi nella griglia imputazione impianti.", "DIV_Messaggi");
    //    else
    //        MessaggioErrore_Bootstrap("Esistono " + nrErr + " righe con dati non completi nella griglia imputazione impianti.", "DIV_Messaggi");

    //    return;
    //}

    //// Non ci sono errori, procedo con aggiornamenti
    //var updatedRecords = [];
    //var newRecords = [];
    //var deletedRecords = [];

    //for (let i = 0; i < currentData.length; i++) {

    //    if (currentData[i].isNew()) {
    //        newRecords.push(currentData[i].toJSON());
    //    } else if (currentData[i].dirty) {
    //        updatedRecords.push(currentData[i].toJSON());
    //    }

    //}

    //for (let i = 0; i < grid.dataSource._destroyed.length; i++) {

    //    deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    //}

    //if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {
        
    //    righeInseriteGrid_Impianti = kendoEscapeOggetto(newRecords);
    //    righeModificateGrid_Impianti = kendoEscapeOggetto(updatedRecords);
    //    righeCancellateGrid_Impianti = kendoEscapeOggetto(deletedRecords);

    //    AggiornaEffettivo(false);

    //}
}


function RiempicmbFiltroImpianti(options) {
    options.success(Elenco_FiltroImpianti);
}

function cmbFiltroImpianti_change(e) {
    ConfiguraGrigliaImpianti("tab_griglia_impianti", 0);
}


function RiempiCmbRipartizione(options) {
    options.success(Elenco_Ripartizione);
}

function cmbRipartizione_change(e) {

    var grid = $("#tab_griglia_impianti").data("kendoGrid");

    if (grid !== undefined) {
        var data = grid.dataSource.data();

        var currentData = grid.dataSource.data();

        var superficie = 0;
        var piante = 0;
        var Qta = Get_KendoNumTBValue("idKgNetti");
        var udm = parseInt(Get_KendoDDLValue("ddlUM"));
        switch (udm) {
            case enum_Udm.quintali.value:
                Qta = Qta * 100;
                break;
            case enum_Udm.tonnellate.value:
                Qta = Qta * 1000;
        }
        var residuo = Qta;

        //Calcolo Totale
        for (var nrRighe = 0; nrRighe < currentData.length; nrRighe++) {
            if (currentData[nrRighe].deleted !== true) {

                superficie += currentData[nrRighe].Sup_Imp;
                piante += currentData[nrRighe].Piante;

            }
        }

        var ddl = KendoDDL("cmbRipartizione");
        switch (ddl.dataItem().Tipo_Ripartizione) {

            case 0: //Manuale

                break;

            case 1: //Automatica Superficie
                
                for (let nrRighe1 = 0; nrRighe1 < currentData.length; nrRighe1++) {

                    let currentDataItem = data[nrRighe1];

                    if (currentData[nrRighe1].deleted !== true && superficie !== 0) {

                        if ((nrRighe1 + 1) < currentData.length) {

                            currentDataItem.set("Qta", Math.round(currentDataItem.Sup_Imp * Qta * 100 / superficie) / 100);
                            residuo = residuo - currentDataItem.Qta;

                        } else {
                            currentDataItem.set("Qta", Math.round(residuo * 100) / 100);
                        }

                    } else {
                        currentDataItem.set("Qta", 0);
                    }

                    data[nrRighe1].dirty = true;

                }

                break;


            case 2: //Automatica Piante

                for (let nrRighe1 = 0; nrRighe1 < currentData.length; nrRighe1++) {

                    let currentDataItem = data[nrRighe1];

                    if (currentData[nrRighe1].deleted !== true && piante !== 0) {

                        if ((nrRighe1 + 1) < currentData.length) {

                            currentDataItem.set("Qta", Math.round(currentDataItem.Piante * Qta * 100 / piante) / 100);
                            residuo = residuo - currentDataItem.Qta;

                        } else {
                            currentDataItem.set("Qta", Math.round(residuo * 100) / 100);
                        }
                    } else {
                        currentDataItem.set("Qta", 0);
                    }

                    data[nrRighe1].dirty = true;
                }

                break;

        }
    }
}


function ImpostaLottiImpianti() {

    var max = 0; 
    var lotti = "";
    var grid = $("#tab_griglia_impianti").data("kendoGrid");

    if (grid !== undefined && getKendoSwitch("ChkImpiantiIndefiniti") === false) {

        var currentData = grid.dataSource.data();

        for (var nrRighe = 0; nrRighe < currentData.length; nrRighe++) {
            if (currentData[nrRighe].deleted !== true) {

                if (currentData[nrRighe].Qta !== undefined && currentData[nrRighe].Qta !== 0) {

                    if (currentData[nrRighe].Qta > max) {
                        max = currentData[nrRighe].Qta;
                        lotti = currentData[nrRighe].Progetto_Nome;
                    }

                }
            }
        }
    }
   
    return lotti;
    
}




function ImpostaAppezamenti() {

    var max = 0;
    var appezzamenti = "";
    var grid = $("#tab_griglia_impianti").data("kendoGrid");

    if (grid !== undefined && getKendoSwitch("ChkImpiantiIndefiniti") === false) {

        var currentData = grid.dataSource.data();

        for (var nrRighe = 0; nrRighe < currentData.length; nrRighe++) {
            if (currentData[nrRighe].deleted !== true) {

                if (currentData[nrRighe].Qta !== undefined && currentData[nrRighe].Qta !== 0) {

                    if (currentData[nrRighe].Qta > max) {
                        max = currentData[nrRighe].Qta;
                        appezzamenti = currentData[nrRighe].PIVA + "|" + currentData[nrRighe].SA_COD + "|" + currentData[nrRighe].Appezza;
                    }

                }
            }
        }
    }

    return appezzamenti;

}


function onDataBoundGrigliaImpianti(e) {

    //if (parseInt(KendoDDL("cmbFiltroImpianti").dataItem().Tipo_Filtro) > 1) {

    //    var griglia = $("#tab_griglia_impianti").data("kendoGrid");

    //    var currentData = griglia.dataSource.data();

    //    for (var nrRighe = 0; nrRighe < currentData.length; nrRighe++) {
    //        currentData[nrRighe].Cul_Des = "";
          
    //    }

       
    //}
}

function FiltroSpecieVegetale() {

    if (parseInt(KendoDDL("cmbFiltroImpianti").dataItem().Tipo_Filtro)  > 1) {       
    
        return true;
    }
     else {
        return false;
    }
    
}

