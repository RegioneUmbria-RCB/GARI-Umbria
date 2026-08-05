function creaKendoTreeView(
    // PARAMETRI OBBLIGATORI
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
    //    funzioneDaChiamareCheck: yyyyy // funzione da chiamare su evento expand
    //}        
) {

    if (IDControllo == null) {
        alert("Non mi hai passato l'ID del DIV che contiene il treeview");
        return;
    }


    var dataSourceTreeView = new kendo.data.HierarchicalDataSource({
        transport: {
            read: funzioniCRUD.funzioneRead            
        },
        schema: {
            model: campiKendoModel
        }
    });

    var tW = $("#" + IDControllo).kendoTreeView({
        loadOnDemand: false,
        checkboxes: {
            checkChildren: true
        },
        dataSource: dataSourceTreeView,
        select: funzioniPrimaDopoEventi.funzioneDaChiamareSelect,
        check: funzioniPrimaDopoEventi.funzioneDaChiamareCheck,
        expand: funzioniPrimaDopoEventi.funzioneDaChiamareExpand
    });

    return tW;
}