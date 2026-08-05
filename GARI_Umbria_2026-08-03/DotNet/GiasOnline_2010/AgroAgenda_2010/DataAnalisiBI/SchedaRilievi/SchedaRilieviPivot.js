

function kendoPivot_DataBound_Rilievi() {

}


var kendoRilievi_Inizializzato = false;

function kendoRilieviPivot_Inizializza() {

    funzioneReadPivotGrid = letturaDatiRilievi();
    modelKendoPivot = {
        fields: {
            Data: { type: "string" },
            Anno: { type: "number" },
            mm_pioggia: { type: "number" },
            note: { type: "string" },
            Temp_min: { type: "number" },
            Temp_max: { type: "number" },
            Temp_media: { type: "number" },
            Bagnatura: { type: "number" },
            umidita: { type: "number" },
            unita_calore_gg: { type: "number" },
            somma_unita_calore_gg: { type: "number" }
        }
    };

    cubeDimensionsKendoPivot = {
        Data: { caption: "Data" },
        Anno: { caption: "Anno" }
    };
    cubeMeasuresKendoPivot = {
        "Media Umidita Relativa": { field: "umidita", format: "{0:n2}", aggregate: "average" },
        "Media Temp": { field: "Temp_media", format: "{0:n2}", aggregate: "average" },
        "Temp Min": { field: "Temp_min", format: "{0:n2}", aggregate: "average" },
        "Temp Max": { field: "Temp_max", format: "{0:n2}", aggregate: "average" },
        "Media Bagnatura": { field: "Bagnatura", format: "{0:n2}", aggregate: "average" },
        "Media mm Pioggia": { field: "mm_pioggia", format: "{0:n2}", aggregate: "average" },
        "Unità Calore": { field: "unita_calore_gg", format: "{0:n2}", aggregate: "sum" },
        "Somma Unità Calore": { field: "somma_unita_calore_gg", format: "{0:n2}", aggregate: "sum" },
    };
    colonneDefaultKendoPivotGrid = [{ name: "Anno", expand: true }];
    righeDefaultKendoPivotGrid = [{ name: "Data", expand: true }];
    misureDefaultKendoPivotGrid = ["Temp Min", "Temp Max", "Unità Calore", "Somma Unità Calore"];

    parametriPerLettura = null;
    parametriKendoPivotConfigurator = {};
    parametriDataSourcePivotGrid = {};
    parametriKendoPivotGrid = { chartCfg: { divchart: "#divKendoChartRilievi", group: "column", category: "row", format: "{0}", type: "line", sort: true } }; // rowHeaderTemplate: $("#rowTemplate").html()
    funzioniPrimaDopoEventiPivotGrid = { funzioneDaChiamareDopoDataBound: kendoPivot_DataBound_Rilievi, collapseMember: collapseMember, expandMember: expandMember };

    kendoRilievi_Inizializzato = true;


}


function kendoRilieviPivot(IDPivotConfigurator, IDPivotGrid) {

    kendoRilieviPivot_Inizializza();

    KendoPivotRilievi = creaKendoPivotGrid(IDPivotConfigurator, // rappresenta l'ID del div a cui si associa la griglia
        parametriKendoPivotConfigurator,
        IDPivotGrid,  // ID Pivot grid
        funzioneReadPivotGrid,  //funzioni js da chiamare per read, insert, update, delete
        modelKendoPivot,
        cubeDimensionsKendoPivot,
        cubeMeasuresKendoPivot,
        colonneDefaultKendoPivotGrid,
        righeDefaultKendoPivotGrid,
        misureDefaultKendoPivotGrid,
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSourcePivotGrid, // parametri data source { chiave - valore}
        parametriKendoPivotGrid,   // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventiPivotGrid
    );
}
