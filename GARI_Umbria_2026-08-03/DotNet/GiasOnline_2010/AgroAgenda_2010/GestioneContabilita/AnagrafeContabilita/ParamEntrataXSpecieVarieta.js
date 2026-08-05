// INIZIO Funzioni Griglia ParamEntrata
function popolaParamEntrata(IDControllo) {

    var funzioniCRUD = {
        funzioneRead: Leggi_ParamEntrataXSpecieVarieta,
        funzioneSubmit: { funzione: SubmitParamEntrataXSpecieVarieta },
        UtenteAbilitatoInserimentoModifica: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True",
        UtenteAbilitatoCancellazione: $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"
    };

    var idModel = "Id_Param";
    var campiKendoModel = {
        Id_Param: { editable: true, type: "number" },
        Veg_Cod: { editable: true, type: "number" },
        Cul_Cod: { editable: true, type: "number"},
        Percentuale_Degrado: { editable: true, type: "number", defaultValue: 0, validation: {  min: 0, max:100 }},
        Riferimento_Prezzi_Des: { editable: true, type: "string", defaultValue:""},
        Riferimento_Prezzi_Cod: { editable: true, type: "string", defaultValue: " "},
        Cul_Des: { editable: true, type: "string" },
        Veg_Des: { editable: true, type: "string" },
        FormulaFissaLiquidazione_Cod: { editable: true, type: "number", defaultValue: enum_ResiduoSeccoBorlotto_Nessuna },
        FormulaFissaLiquidazione_Des: { editable: true, type: "string", defaultValue: enum_ResiduoSeccoBorlotto_Nessuna_Des},
        Validita_Inizio: { editable: true, type: "date", defaultValue: new Date("1900/01/01")},
        Validita_Fine: { editable: true, type: "date", defaultValue: new Date("2100/12/31") },
        Regolamento_Cod: { editable: true, type: "number", defaultValue: enum_Regolamento_Nessuno },
        Regolamento_Des: { editable: true, type: "string", defaultValue: enum_Regolamento_Nessuno_Des },
        Data_Creazione: { editable: false, type: "date" },
        inviato: { editable: false, type: "number" },
        Username_Creazione: { editable: false, type: "string" }
    };
    var colonneKendoGrid = [
        {
            field: "Veg_Des", title: "Specie", editor: Specie_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Cul_Des", title: "Varietà", editor: val_codDropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Regolamento_Des", title: "Regolamento", editor: Regolamento_CodDropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "FormulaFissaLiquidazione_Des", title: "Formula da applicare in liquidazione", editor: FormulaLiq_DropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Riferimento_Prezzi_Des", title: "Riferimento Prezzo", editor: RifPrezzo_codDropDownEditor, filterable: { multi: true, search: true }
        },
        {
            field: "Percentuale_Degrado", title: "% Degrado", format: "{0:n}", attributes: {
                style: "text-align: center;"
            }, headerAttributes: {
                style: "text-align: center;"
            } 
        },
        {
            field: "Validita_Inizio", title: "Validità inizio", format: "{0:dd/MM/yyyy}", attributes: {
                style: "text-align: center;"
            }, headerAttributes: {
                style: "text-align: center;"
            } 
        },
        {
            field: "Validita_Fine", title: "Validità fine", format: "{0:dd/MM/yyyy}", attributes: {
                style: "text-align: center;"
            }, headerAttributes: {
                style: "text-align: center;"
            }
        }
    ];
    var parametriPerLettura = null;
    var parametriDataSource = { pagesize: 10 };
    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 4 },
    };
    //var funzioniPrimaDopoEventi = { funzioneDaChiamarePrimaDelDataBinding: onDataBindingVFattVarParamQual };
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
function Specie_DropDownEditor(container, options) {

    creaDropDownEditor(container, "Veg_Des", "Veg_Cod", elencoSpecie, changeSpecie);
}

function changeSpecie(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#tab_parametri_specie_varieta").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Veg_Cod = dataItem.Veg_Cod;
    model.Veg_Des = dataItem.Veg_Des;

    model.Cul_Cod = 0;
    model.Cul_Des = "";
  //   kendoFastRedrawRow(grid, row);
}

function val_codDropDownEditor(container, options) {
    var elencoVarieta = RicercaVarieta($(cIdPiva).val(), options.model.Veg_Cod);
    creaDropDownEditor(container, "Cul_Des", "Cul_Cod", elencoVarieta, changeVarieta);
}

function RifPrezzo_codDropDownEditor(container, options) {

    let ElencoRiferimentoPrezzi = RicercaTipoRiferimentoPrezzi();

    ElencoRiferimentoPrezzi.unshift({ Riferimento_Prezzi_Cod: " ", Riferimento_Prezzi_Des: "" });

    creaDropDownEditor(container, "Riferimento_Prezzi_Des", "Riferimento_Prezzi_Cod", ElencoRiferimentoPrezzi, changeRifPrezzo);  
}

function changeVarieta(e)
{
    var dataItem = e.sender.dataItem();
    var grid = $("#tab_parametri_specie_varieta").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.Cul_Cod = dataItem.Cul_Cod;
    model.Cul_Des = dataItem.Cul_Des;
}

function changeRifPrezzo(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#tab_parametri_specie_varieta").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.Riferimento_Prezzi_Cod = dataItem.Riferimento_Prezzi_Cod;
    model.Riferimento_Prezzi_Des = dataItem.Riferimento_Prezzi_Des;
}

function FormulaLiq_DropDownEditor(container, options) {
    let formulaLiq = [
        { "FormulaFissaLiquidazione_Cod": enum_ResiduoSeccoBorlotto_Nessuna, "FormulaFissaLiquidazione_Des": "" + enum_ResiduoSeccoBorlotto_Nessuna_Des + "" },
        { "FormulaFissaLiquidazione_Cod": enum_ResiduoSeccoBorlotto_45_50, "FormulaFissaLiquidazione_Des": "" + enum_ResiduoSeccoBorlotto_45_50_Des + "" }
    ];

    creaDropDownEditor(container, "FormulaFissaLiquidazione_Des", "FormulaFissaLiquidazione_Cod", formulaLiq, changeFormulaLiq); 
}

function changeFormulaLiq(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#tab_parametri_specie_varieta").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.FormulaFissaLiquidazione_Cod = dataItem.FormulaFissaLiquidazione_Cod;
    model.FormulaFissaLiquidazione_Des = dataItem.FormulaFissaLiquidazione_Des;
}

function Regolamento_CodDropDownEditor(container, options) {
    let ElencoRegolamento = [
        { "Regolamento_Cod": enum_Regolamento_Nessuno, "Regolamento_Des": "" + enum_Regolamento_Nessuno_Des + "" },
        { "Regolamento_Cod": enum_Regolamento_Convenzionale_no_Bio, "Regolamento_Des": "" + enum_Regolamento_Convenzionale_no_Bio_Des + "" },
        { "Regolamento_Cod": enum_Regolamento_Biologico, "Regolamento_Des": "" + enum_Regolamento_Biologico_Des + "" }
    ];

    creaDropDownEditor(container, "Regolamento_Des", "Regolamento_Cod", ElencoRegolamento, changeRegolamento); 
}

function changeRegolamento(e) {
    var dataItem = e.sender.dataItem();
    var grid = $("#tab_parametri_specie_varieta").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.Regolamento_Cod = dataItem.Regolamento_Cod;
    model.Regolamento_Des = dataItem.Regolamento_Des;
}