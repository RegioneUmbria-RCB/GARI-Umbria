//Popola la griglia kendo
function popolaGrigliaCatastoAffitti(idControllo) {

    // Creazione Griglia
    var funzioniCRUD = {
        funzioneRead: gridCatastoAffittiRead,
        UtenteAbilitatoInserimentoModifica: false,
        UtenteAbilitatoCancellazione: true,
        omettiPulsantiSalva: true,
        omettiPulsantiAnnulla: true
    };

    var idModel = "Cod_RisUm";

    //Campi della kendoGrid
    var campiKendoModel = {
        Cod_Particella: { type: "string" },
        Appezzamento: {type: "number"},
        Zona: { type: "string" },
        Cod_Comune: { type: "string" },
        Provincia: { type: "number" },
        Comune: { type: "number" },
        Descr_Comune: { type: "string" },
        Foglio: { type: "number" },
        Num_Particella: { type: "number" },
        Concedente: { type: "string" },
        LocatarioExtra: { type: "string" },
        RifOrdini: { type: "string" },
        CentroAz: { type: "string" },
        BioVincolo: { type: "boolean" },
        Sup_Catast: { type: "number" },
        SauNoBoschi: { type: "number" },
        TaraSatSau: { type: "number" },
        sup_Arboree_Orticole_Erbacee: { type: "number" },
        SauBoschi: { type: "number" },
        Data_Reg_Contr: { type: "date" },
        numReg: { type: "number" },
        PAC: { type: "number" },
        RD: { type: "number" },
        RA: { type: "number" },
        MetodoProd: { type: "string" },
        InizioContratto: { type: "date" },
        FineContratto: { type: "date" },
        InizioConversione: { type: "date" },
        FineConversione: { type: "date" },
        latiNoConf: { type: "string" },
        profFascia: { type: "number" },
        note: { type: "string" },

        //hidden field (da modificare)
        InizioMetodo: { type: "date" },
        FineMetodo: { type: "date" },
        Sup_Cond: { type: "number" },
        Sup_Contratto: { type: "number" },
        RifContr: { type: "string" },
        Id_Agenda: { type: "number" },
    };

    var colonneKendoGrid = [
        { field: "Cod_Particella", title: "Codice Particella", filterable: { multi: true, search: true }, width: 100 },
        { field: "Appezzamento", title: "APPEZZAMENTO", filterable: { multi: true, search: true }, width: 100 },
        { field: "Zona", title: "ZONA", filterable: { multi: true, search: true }, width: 100 },
        { field: "Cod_Comune", title: "Codice Comune", filterable: { multi: true, search: true }, width: 100 },
        { field: "Provincia", title: "Provincia", filterable: { multi: true, search: true }, width: 100 },
        { field: "Comune", title: "Comune", filterable: { multi: true, search: true }, width: 100 },
        { field: "Descr_Comune", title: "Descrizione Comune", filterable: { multi: true, search: true }, width: 150 },
        { field: "Foglio", title: "Foglio", filterable: { multi: true, search: true }, width: 100 },
        { field: "Num_Particella", title: "Particella", filterable: { multi: true, search: true }, width: 100 },
        { field: "Concedente", title: "Concedente", filterable: { multi: true, search: true }, width: 150 },
        { field: "LocatarioExtra", title: "Altri locatari", filterable: { multi: true, search: true }, width: 100 },
        { field: "RifOrdini", title: "Riferimento ordini", filterable: { multi: true, search: true }, width: 100 },
        { field: "CentroAz", title: "Centro aziendale", filterable: { multi: true, search: true }, width: 100 },

        //Checkbox legata al valore di MetodoProd (BIO/CONV = true)
        {
            template: '#=dirtyField(data,"BioVincolo")#<input type="checkbox" disabled #= BioVincolo ? \'checked="checked"\' : "" # class="chkbx k-checkbox k-checkbox-md k-rounded-md" />',
            title: "BIO VINCOLO",
            width: 100, attributes: { class: "k-text-center" },
        },
        { field: "Sup_Catast", title: "SAT da Affitti", format: "{0:n4}", filterable: { multi: true, search: true }, width: 100 },
        { field: "SauNoBoschi", title: "SAU NO Boschi", format: "{0:n4}", filterable: { multi: true, search: true }, width: 100 },
        { field: "TaraSatSau", title: "Tara (SAT - SAU)", format: "{0:n4}", filterable: { multi: true, search: true }, width: 100 },
        { field: "sup_Arboree_Orticole_Erbacee", title: "SAU TOT", format: "{0:n4}", filterable: { multi: true, search: true }, width: 100 },
        { field: "SauBoschi", title: "SAU Boschi", format: "{0:n4}", filterable: { multi: true, search: true }, width: 100 },
        {
            field: "Data_Reg_Contr", title: "Data Registrazione", /*format: "{0:dd/MM/yyyy}",*/
            template: '#= kendo.toString(Data_Reg_Contr, "dd/MM/yyyy") == "01/01/1900" ? "..." : kendo.toString(Data_Reg_Contr, "dd/MM/yyyy") #',
            width: 100
        },
        { field: "NumReg", title: "N° Registrazione", filterable: { multi: true, search: true }, width: 100 },
        { field: "PAC", title: "PAC", filterable: { multi: true, search: true }, width: 100 },
        { field: "RD", title: "R.D.", filterable: { multi: true, search: true }, width: 100 },
        { field: "RA", title: "R.A.", filterable: { multi: true, search: true }, width: 100 },
        { field: "MetodoProd", title: "BIO/CONV.", filterable: { multi: true, search: true }, width: 100 },

        {
            field: "InizioContratto", title: "Inizio Contratto", /*format: "{0:dd/MM/yyyy}",*/
            template: '#= kendo.toString(InizioContratto, "dd/MM/yyyy") == "01/01/1900" ? "..." : kendo.toString(InizioContratto, "dd/MM/yyyy") #',
            width: 100
        },
        {
            field: "FineContratto", title: "Fine Contratto", /*format: "{0:dd/MM/yyyy}",*/
            template: '#= kendo.toString(FineContratto, "dd/MM/yyyy") == "01/01/1900" ? "..." : kendo.toString(FineContratto, "dd/MM/yyyy") #',
            width: 100
        },
        {
            field: "InizioConversione", title: "Inizio Conversione", /*format: "{0:dd/MM/yyyy}",*/
            template: '#= kendo.toString(InizioConversione, "dd/MM/yyyy") == "01/01/1900" ? "..." : kendo.toString(InizioConversione, "dd/MM/yyyy") #',
            width: 100
        },
        {
            field: "FineConversione", title: "Fine Conversione", /*format: "{0:dd/MM/yyyy}",*/
            template: '#= kendo.toString(FineConversione, "dd/MM/yyyy") == "01/01/1900" ? "..." : kendo.toString(FineConversione, "dd/MM/yyyy") #',
            width: 100
        },
        {   //hidden field
            field: "InizioMetodo", title: "InizioMetodo", /*format: "{0:dd/MM/yyyy}",*/
            template: '#= kendo.toString(InizioMetodo, "dd/MM/yyyy") == "01/01/1900" ? "..." : kendo.toString(InizioMetodo, "dd/MM/yyyy") #',
            width: 100,
            hidden: true
        },
        {   //hidden field
            field: "FineMetodo", title: "FineMetodo", /*format: "{0:dd/MM/yyyy}",*/
            template: '#= kendo.toString(FineMetodo, "dd/MM/yyyy") == "01/01/1900" ? "..." : kendo.toString(FineMetodo, "dd/MM/yyyy") #',
            width: 100,
            hidden: true
        },

        { field: "latiNoConf", title: "Lati non Conformi", filterable: { multi: true, search: true }, width: 100 },
        { field: "profFascia", title: "Profondità di fascia di rispetto / zona tampone", filterable: { multi: true, search: true }, width: 100 },
        { field: "note", title: "NOTE", filterable: { multi: true, search: true }, width: 200 },
        { field: "Sup_Contratto", title: "Superficie Contr", filterable: { multi: true, search: true }, hidden: true, width: 100 }, //hidden field
        { field: "Sup_Cond", title: "Superficie Cond", filterable: { multi: true, search: true }, hidden: true, width: 100 },           //hidden field
        { field: "RifContr", title: "Riferimento Contr", filterable: { multi: true, search: true }, hidden: true, width: 100 },           //hidden field
        { field: "Id_Agenda", title: "ID Agenda", filterable: { multi: true, search: true }, hidden: true, width: 100 },         //hidden field

    ];

    var parametriPerLettura = [];

    var parametriDataSource = {
        serverFiltering: false,
        pagesize: 15
    };

    //Parametri per la kendoGrid
    var parametriKendoGrid = {
        excel: true,
        pdf: false,
        editable: false,
        groupable: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 4 },
        reorderable: true,
        scrollable: true
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: gridCatastoAffittiDataBound, /*funzioneDaChiamareDopoSelectAllRows: gridAssociazioniDopoCheckAllRows*/ };

    creaKendoGrid(
        idControllo,            // Rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,           // Funzioni js da chiamare per read, insert, update, delete
        idModel,                // Chiave riga 
        campiKendoModel,        // Campi modello
        colonneKendoGrid,       // Colonne da mostrare
        parametriPerLettura,    // Parametri da passare alla lettura
        parametriDataSource,    // Parametri data source { chiave - valore}
        parametriKendoGrid,     // Parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi // Funzioni da chiamare all'inizio e alla fine dei vari eventi
    );

}

//Per campo checkbox istanziato sopra
function dirtyField(data, fieldName) {
    var hasClass = $("[data-uid=" + data.uid + "]").find(".k-dirty-cell").length < 1;
    if (data.dirty && data.dirtyFields[fieldName] && hasClass) {
        return "<span class='k-dirty'></span>"
    }
    else {
        return "";
    }
}


function gridCatastoAffittiRead(options, dataDal, dataAl) {

    let dsCatastoAffitti = [];

    //Filtri selezionati da multiSelect
    let centriAziendali = KendoMultisel("multiselCentroAziendale").value().join("|");

    //Datatable contente dati della kendoGrid
    dsCatastoAffitti = leggiDS_CatastoAffitti($(cIdPiva).val(), $("#txt_DataDal").val(), $("#txt_DataAl").val(), centriAziendali);

    options.success(dsCatastoAffitti);
}

function gridCatastoAffittiDataBound(ev) {

}