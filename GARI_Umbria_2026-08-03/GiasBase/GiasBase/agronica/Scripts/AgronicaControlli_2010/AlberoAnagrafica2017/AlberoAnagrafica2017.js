

/* AlberoAnagrafica2017.js */



function creaAlberoAnagrafica2017(
    // PARAMETRI OBBLIGATORI
    cfg,
    IDControllo,
    funzioniCRUD,
    idModel,
    campiKendoModel,
    // PARAMETRI FACOLTATIVO
    parametriPerLettura, // parametri da passare alla lettura
    parametriDataSource, // parametri data source { chiave - valore}
    parametriKendoTreeView,   // parametri kendo TreeView [{ chiave - valore}]
    funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi { chiave - valore}:
    //{   
    //    funzioneDaChiamarePrimaDelDataBinding: yyyyy, // funzione da chiamare all'inizio del databinding
    //    funzioneDaChiamareDopoDataBinding: yyyyy, // funzione da chiamare alla fine del databinding
    //    funzioneDaChiamarePrimaDelDataBound: yyyyy, // funzione da chiamare all'inizio del databound
    //    funzioneDaChiamareDopoDataBound: yyyyy, // funzione da chiamare alla fine del databound
    //    funzioneDaChiamarePrimaDelSave: yyyyy, // funzione da chiamare all'inizio del save
    //    funzioneDaChiamareDopoSave: yyyyy // funzione da chiamare alla fine del save
    //    funzioneDaChiamareDopoDelete: yyyyy // funzione da chiamare dopo la cancellazione di una riga 
    //    funzioneDaChiamareCheck: yyyyy // funzione da chiamare su evento check
    //}        
) {

    var AlberoAnagraficaSearchTreeView = creaKendoDialogTreeViewFilter(IDControllo, funzioniCRUD, idModel, campiKendoModel, "Seleziona..", parametriPerLettura, parametriDataSource, parametriKendoTreeView, funzioniPrimaDopoEventi);
    return AlberoAnagraficaSearchTreeView;
}

