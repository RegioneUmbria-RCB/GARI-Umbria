const larghezzaStdCampoNumerico = 100;
const larghezzaStdCampoData = 140;
const larghezzaStdCampoGestione = 110;

$("#btn_carica_elenco").click(
    async function () {
        init_tabLavUMA = false;
        init_tabLavAlt = false;
        init_tabUMAConfigurazioneAllevamenti = false;
        var elem = $("#tabstrip_elenco").data("kendoTabStrip").select().index();
        SeleTab(elem);
        UF_popolaGriglia("grdConfigurazioneUF");
    });

// ************************************************* Crea il Kendo Grid *************************************************
function popolaGrigliaConfigurazioniUMA(IDControllo) {
    var uteAbilitatoScrit = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = uteAbilitatoScrit;

    var funzioniCRUD = {
        funzioneRead: CaricaConfigurazioniDaDB,
        funzioneSubmit: { funzione: SubmitGrid_Lavorazioni, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: uteAbilitatoScrit,
        UtenteAbilitatoCancellazione: uteAbilitatoScrit,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false,
    };

    var colonna_editabile = false;

    if (uteAbilitatoScrit === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    var idModel = "RegioneCod";
    var campiKendoModel =CaricaCampiKendoModel(colonna_editabile)
    var colonneKendoGrid = CaricaColonneKendoGrid()

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var parametriKendoGrid = {
        pdf: false,
        excel: true,
        editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = null;

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: onEditLavorazioneUMAConfig,
        funzioneDaChiamareDopoDataBound: autoFitAllColumns,
        funzioneDaChiamareDopoDelete: null
    };

    creaKendoGrid(IDControllo,            // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,                     //funzioni js da chiamare per read, insert, update, delete
        idModel,                          // chiave riga 
        campiKendoModel,                  // campi modello
        colonneKendoGrid,                 // colonne da mostrare
        parametriPerLettura,              // parametri da passare alla lettura
        parametriDataSource,              // parametri data source { chiave - valore}
        parametriKendoGrid,               // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi,          // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate,            // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

function LavAlt_popolaGriglia(IDControllo) {
    var uteAbilitatoScrit = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = uteAbilitatoScrit;

    var funzioniCRUD = {
        funzioneRead: LavAlt_Caricale,
        funzioneSubmit: { funzione: LavorazioniAlternative_SubmitGrid, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: uteAbilitatoScrit,
        UtenteAbilitatoCancellazione: uteAbilitatoScrit,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false,
    };

    var colonna_editabile = false;

    if (uteAbilitatoScrit === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    var idModel = "Chiave";
    var campiKendoModel = LavAlt_CaricaCampiKendoModel(colonna_editabile)
    var colonneKendoGrid = LavAlt_CaricaColonneKendoGrid()

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var parametriKendoGrid = {
        pdf: false,
        excel: true,
        editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = null;

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: LavAlt_disabilitaCell,
        funzioneDaChiamareDopoDataBound: autoFitAllColumns_LavAlt,
        funzioneDaChiamareDopoDelete: null
    };

    creaKendoGrid(IDControllo,            // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,                     //funzioni js da chiamare per read, insert, update, delete
        idModel,                          // chiave riga 
        campiKendoModel,                  // campi modello
        colonneKendoGrid,                 // colonne da mostrare
        parametriPerLettura,              // parametri da passare alla lettura
        parametriDataSource,              // parametri data source { chiave - valore}
        parametriKendoGrid,               // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi,          // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate,            // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

function Setup_popolaGriglia(ID_controllo) {
    var uteAbilitatoScrit = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = uteAbilitatoScrit;

    var funzioniCRUD = {
        funzioneRead: Setup_Caricale,
        funzioneSubmit: { funzione: Setup_SubmitGrid, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: uteAbilitatoScrit,
        UtenteAbilitatoCancellazione: uteAbilitatoScrit,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false,
    };

    var colonna_editabile = false;

    if (uteAbilitatoScrit === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    var idModel = "Chiave";
    var campiKendoModel = Setup_CaricaCampiKendoModel(colonna_editabile)
    var colonneKendoGrid = Setup_CaricaColonneKendoGrid()

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var parametriKendoGrid = {
        pdf: false,
        excel: true,
        editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        colonneCustomKendoGrid: [
            {
                command: {
                    template: "<div class='btn btn-info btnDuplica fa fa-copy' style='display:block;width:20px;border:0px;' onclick=duplicaAnno(this.closest('tr'),this.closest('.k-grid'))></div>"
                }, title: "Duplica", width: "97px"
            }
        ]
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = null;

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: Setup_disabilitaCell,
        funzioneDaChiamareDopoDataBound: autoFitAllColumns_Setup,
        funzioneDaChiamareDopoDelete: null
    };

    creaKendoGrid(ID_controllo,            // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,                     //funzioni js da chiamare per read, insert, update, delete
        idModel,                          // chiave riga 
        campiKendoModel,                  // campi modello
        colonneKendoGrid,                 // colonne da mostrare
        parametriPerLettura,              // parametri da passare alla lettura
        parametriDataSource,              // parametri data source { chiave - valore}
        parametriKendoGrid,               // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi,          // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate,            // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

function DateRendicontazioni_popolaGriglia(ID_controllo) {
    var uteAbilitatoScrit = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = uteAbilitatoScrit;

    var funzioniCRUD = {
        funzioneRead: DateRendicontazioni_Caricale,
        funzioneSubmit: { funzione: DateRendicontazioni_SubmitGrid, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: uteAbilitatoScrit,
        UtenteAbilitatoCancellazione: uteAbilitatoScrit,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false,
    };

    var colonna_editabile = false;

    if (uteAbilitatoScrit === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    var idModel = "chiave";
    var campiKendoModel = DateRendicontazione_CaricaCampiKendoModel(colonna_editabile)
    var colonneKendoGrid = DateRendicontazione_CaricaColonneKendoGrid()

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var parametriKendoGrid = {
        pdf: false,
        excel: true,
        editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = null;

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: SetupDateRendicontazioni_onEdit,
        funzioneDaChiamareDopoDelete: null
    };

    creaKendoGrid(ID_controllo,            // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,                     //funzioni js da chiamare per read, insert, update, delete
        idModel,                          // chiave riga 
        campiKendoModel,                  // campi modello
        colonneKendoGrid,                 // colonne da mostrare
        parametriPerLettura,              // parametri da passare alla lettura
        parametriDataSource,              // parametri data source { chiave - valore}
        parametriKendoGrid,               // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi,          // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate,            // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

function UMAConfigurazioneAllevamenti_popolaGriglia(IDControllo) {
    var uteAbilitatoScrit = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = uteAbilitatoScrit;

    var funzioniCRUD = {
        funzioneRead: UMAConfigurazioneAllevamenti_Caricale,
        funzioneSubmit: { funzione: UMAConfigurazioneAllevamenti_SubmitGrid, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: uteAbilitatoScrit,
        UtenteAbilitatoCancellazione: uteAbilitatoScrit,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false,
    };

    var colonna_editabile = false;

    if (uteAbilitatoScrit === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    var idModel = "Chiave";
    var campiKendoModel = UMAConfigurazioneAllevamenti_CaricaCampiKendoModel(colonna_editabile)
    var colonneKendoGrid = UMAConfigurazioneAllevamenti_CaricaColonneKendoGrid()

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var parametriKendoGrid = {
        pdf: false,
        excel: true,
        editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = null;

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoEdit: UMAConfigurazioneAllevamenti_disabilitaCell, funzioneDaChiamareDopoDataBound: null, funzioneDaChiamareDopoDelete: null };

    creaKendoGrid(IDControllo,            // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,                     //funzioni js da chiamare per read, insert, update, delete
        idModel,                          // chiave riga 
        campiKendoModel,                  // campi modello
        colonneKendoGrid,                 // colonne da mostrare
        parametriPerLettura,              // parametri da passare alla lettura
        parametriDataSource,              // parametri data source { chiave - valore}
        parametriKendoGrid,               // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi,          // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate,            // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

function UF_popolaGriglia(ID_controllo) {
    var uteAbilitatoScrit = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = uteAbilitatoScrit;

    var funzioniCRUD = {
        funzioneRead: UF_Caricale,
        funzioneSubmit: { funzione: UF_SubmitGrid, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: uteAbilitatoScrit,
        UtenteAbilitatoCancellazione: uteAbilitatoScrit,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false,
    };

    var colonna_editabile = false;

    if (uteAbilitatoScrit === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    var idModel = "Occupazione_Cod";
    var campiKendoModel = UF_CaricaCampiKendoModel(colonna_editabile)
    var colonneKendoGrid = UF_CaricaColonneKendoGrid()

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var parametriKendoGrid = {
        pdf: false,
        excel: true,
        editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        colonneCustomKendoGrid: [],
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["Occupazione_Des", "Destinazione_Des", "Uso_Des", "Qualita_Des", "Occupazione_Cod", "Destinazione_Cod", "Uso_Cod", "Qualita_Cod", "Validita_Inizio"];

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: Setup_disabilitaCell,
        funzioneDaChiamareDopoDataBound: autoFitAllColumns_Setup,
        funzioneDaChiamareDopoDelete: null
    };

    creaKendoGrid(ID_controllo,            // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,                     //funzioni js da chiamare per read, insert, update, delete
        idModel,                          // chiave riga 
        campiKendoModel,                  // campi modello
        colonneKendoGrid,                 // colonne da mostrare
        parametriPerLettura,              // parametri da passare alla lettura
        parametriDataSource,              // parametri data source { chiave - valore}
        parametriKendoGrid,               // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi,          // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate,            // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

function ElencoMacrousi_popolaGriglia(ID_controllo) {
    var uteAbilitatoScrit = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = uteAbilitatoScrit;

    var funzioniCRUD = {
        funzioneRead: ElencoMacrousi_Caricale,
        funzioneSubmit: { funzione: ElencoMacrousi_SubmitGrid, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: uteAbilitatoScrit,
        UtenteAbilitatoCancellazione: uteAbilitatoScrit,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false,
    };

    var colonna_editabile = false;

    if (uteAbilitatoScrit === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    var idModel = "Macrouso_UMA_Cod";
    var campiKendoModel = ElencoMacrousi_CaricaCampiKendoModel(colonna_editabile)
    var colonneKendoGrid = ElencoMacrousi_CaricaColonneKendoGrid()

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var parametriKendoGrid = {
        pdf: false,
        excel: true,
        editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        colonneCustomKendoGrid: [],
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["Macrouso_UMA_Cod", "Validita_Inizio", "Validita_Fine"];

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: Setup_disabilitaCell,
        funzioneDaChiamareDopoDataBound: autoFitAllColumns_Setup,
        funzioneDaChiamareDopoDelete: null
    };

    creaKendoGrid(ID_controllo,            // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,                     //funzioni js da chiamare per read, insert, update, delete
        idModel,                          // chiave riga 
        campiKendoModel,                  // campi modello
        colonneKendoGrid,                 // colonne da mostrare
        parametriPerLettura,              // parametri da passare alla lettura
        parametriDataSource,              // parametri data source { chiave - valore}
        parametriKendoGrid,               // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi,          // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate,            // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

function ElencoLavorazioni_popolaGriglia(ID_controllo) {
    var uteAbilitatoScrit = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = uteAbilitatoScrit;

    var funzioniCRUD = {
        funzioneRead: ElencoLavorazioni_Caricale,
        funzioneSubmit: { funzione: ElencoLavorazioni_SubmitGrid, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: uteAbilitatoScrit,
        UtenteAbilitatoCancellazione: uteAbilitatoScrit,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false,
    };

    var colonna_editabile = false;

    if (uteAbilitatoScrit === true && UteAbilitatoCanc === true)
        colonna_editabile = true;

    var idModel = "Lav_UMA_Cod";
    var campiKendoModel = ElencoLavorazioni_CaricaCampiKendoModel(colonna_editabile)
    var colonneKendoGrid = ElencoLavorazioni_CaricaColonneKendoGrid()

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var parametriKendoGrid = {
        pdf: false,
        excel: true,
        editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        colonneCustomKendoGrid: [],
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["Lav_UMA_Cod", "Validita_Inizio", "Validita_Fine"];

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamarePrimaDiEdit: PrevieniModificaCheckBox,
        funzioneDaChiamareDopoEdit: Setup_disabilitaCell,
        funzioneDaChiamareDopoDataBound: autoFitAllColumns_Setup,
        funzioneDaChiamareDopoDelete: null
    };

    creaKendoGrid(ID_controllo,            // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,                     //funzioni js da chiamare per read, insert, update, delete
        idModel,                          // chiave riga 
        campiKendoModel,                  // campi modello
        colonneKendoGrid,                 // colonne da mostrare
        parametriPerLettura,              // parametri da passare alla lettura
        parametriDataSource,              // parametri data source { chiave - valore}
        parametriKendoGrid,               // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi,          // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate,            // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

function PrevieniModificaCheckBox(e) {
    let grid = $("#grdElencoLavorazioniUMA").data("kendoGrid");
    let row = e.container.closest("tr");
    let model = grid.dataItem(row);
    let field = grid.columns[e.container.index()].field;
    if (field === "Maggiorazione_Terreno_MedioTenace" || field === "GestioneTerzista" || field === "Utilizzata_Da_Consorzio_Bonifica") {
        model[field] = !model[field];
        model.dirty = true;
        grid.refresh();
    }
}

function ElencoAllevamenti_popolaGriglia(ID_controllo) {
    var uteAbilitatoScrit = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = uteAbilitatoScrit;

    var funzioniCRUD = {
        funzioneRead: ElencoAllevamenti_Caricale,
        funzioneSubmit: { funzione: ElencoAllevamenti_SubmitGrid, flagInsert: true, flagUpdate: true, flagDelete: true },
        UtenteAbilitatoInserimentoModifica: uteAbilitatoScrit,
        UtenteAbilitatoCancellazione: uteAbilitatoScrit,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false,
    };

    var colonna_editabile = false;

    if (uteAbilitatoScrit === true && UteAbilitatoCanc === true)
        colonna_editabile = true;


    var idModel = "UMA_All_Cod";
    var campiKendoModel = ElencoAllevamenti_CaricaCampiKendoModel(colonna_editabile)
    var colonneKendoGrid = ElencoAllevamenti_CaricaColonneKendoGrid()

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var parametriKendoGrid = {
        pdf: false,
        excel: false,
        editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        colonneCustomKendoGrid: [],
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["UMA_All_Cod", "Validita_Inizio", "Validita_Fine"];

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: Generic_disabilitaCell,
        funzioneDaChiamareDopoDataBound: autoFitAllColumns_Setup,
        funzioneDaChiamareDopoDelete: null
    };

    creaKendoGrid(ID_controllo,            // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,                     //funzioni js da chiamare per read, insert, update, delete
        idModel,                          // chiave riga 
        campiKendoModel,                  // campi modello
        colonneKendoGrid,                 // colonne da mostrare
        parametriPerLettura,              // parametri da passare alla lettura
        parametriDataSource,              // parametri data source { chiave - valore}
        parametriKendoGrid,               // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi,          // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate,            // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

function ElencoAssociazioniMacrousi_popolaGriglia(ID_controllo) {
    $("#btnModificaMassiva")[0].setAttribute("disabled", "");

    var uteAbilitatoScrit = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    let UteAbilitatoCanc = uteAbilitatoScrit;

    var funzioniCRUD = {
        funzioneRead: ElencoAssociazioniMacrousi_Caricale,
        funzioneSubmit: { funzione: ElencoAssociazioniMacrousi_SubmitGrid, flagInsert: false, flagUpdate: true, flagDelete: false },
        UtenteAbilitatoInserimentoModifica: uteAbilitatoScrit,
        UtenteAbilitatoCancellazione: uteAbilitatoScrit,
        omettiPulsantiSalva: false,
        omettiPulsantiAnnulla: false,
        checkBoxFunction: kEventoSelezionaRiga_AssociazioniMacrousiUMA
    };

    var colonna_editabile = false;

    if (uteAbilitatoScrit === true && UteAbilitatoCanc === true)
        colonna_editabile = true;


    var idModel = "Chiave";
    var campiKendoModel = ElencoAssociazioniMacrousi_CaricaCampiKendoModel(colonna_editabile)
    var colonneKendoGrid = ElencoAssociazioniMacrousi_CaricaColonneKendoGrid()

    var parametriPerLettura = null;
    var parametriDataSource = { batch: true };
    var parametriKendoGrid = {
        pdf: false,
        excel: false,
        editable: { mode: "incell" },
        columnMenu: true,
        reorderable: true,
        editable: true,
        groupable: true,
        pageable: { pageSizes: [5, 10, 20, 50, 100] },
        colonneCustomKendoGrid: [],
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS }
    };

    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = ["Validita_Inizio", "Validita_Fine"];

    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoEdit: Setup_disabilitaCell,
        funzioneDaChiamareDopoDataBound: autoFitAllColumns_Setup,
        funzioneDaChiamareDopoDelete: null
    };

    creaKendoGrid(ID_controllo,            // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,                     //funzioni js da chiamare per read, insert, update, delete
        idModel,                          // chiave riga 
        campiKendoModel,                  // campi modello
        colonneKendoGrid,                 // colonne da mostrare
        parametriPerLettura,              // parametri da passare alla lettura
        parametriDataSource,              // parametri data source { chiave - valore}
        parametriKendoGrid,               // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi,          // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate,            // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );
}

function autoFitAllColumns(e) {
    var grid = $("#grdConfigurazioniUMA").data("kendoGrid");
    for (i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].width === undefined) {
            grid.autoFitColumn(i);
        }
    }
}

function autoFitAllColumns_Setup(e) {
    //var grid = $("#grdSetup").data("kendoGrid");
    //if (grid != null && grid != undefined) {
    //    for (i = 0; i < grid.columns.length; i++) {
    //        if (grid.columns[i].width === undefined) {
    //            grid.autoFitColumn(i);
    //        }
    //    }
    //}
}

function autoFitAllColumns_LavAlt(e) {
    //var grid = $("#grdLavorazioniAlternative").data("kendoGrid");
    //if (grid != null && grid != undefined) {
    //    for (i = 0; i < grid.columns.length; i++) {
    //        if (grid.columns[i].width === undefined) {
    //            grid.autoFitColumn(i);
    //        }
    //    }
    //}
}

function LavAlt_CampiObbligatoriSonoImpostati(rigaDati) {
    return rigaDati.Gruppo_Colturale_UMA !== "" && rigaDati.Lavorazione_UMA !== "" && rigaDati.Lavorazione_UMA_Alt !== "";
}

function Setup_CampiObbligatoriSonoImpostati(rigaDati) {
    return rigaDati.Anno !== 0;
}

function SetupDateRendicontazioni_CampiObbligatoriSonoImpostati(rigaDati) {
    return rigaDati.Anno_Richiesta !== 0 && rigaDati.Anno_Richiesta !== "" && rigaDati.Tipo_Azienda !== 0 && rigaDati.Tipo_Azienda !== "";
}

function UMAConfigurazioneAllevamenti_CampiObbligatoriSonoImpostati(rigaDati) {
    return rigaDati.UMA_All_Cod !== 0;
}

function UMAConfigurazioneGruppiAllevamento_CampiObbligatoriSonoImpostati(rigaDati) {
    return rigaDati.UMA_All_Cod !== null;
}

function CampiObbligatoriSonoImpostati(rigaDati) {
    return rigaDati.MacrousoUMACod !== "" && rigaDati.LavUMACod !== "" && rigaDati.LavCod !== 0;
}

function RigaCaricataDalServer(riga) {
    return !riga.Modificabile;
}

function CampoNonModificabile(elem) {
    return elem.hasClass("edit_onInsert");
}

// *********************************** Faccio alcuni campi non editabili sulla condizione seguente ***********************************
function onEditLavorazioneUMAConfig(e) {
    if (CampiObbligatoriSonoImpostati(e.model) && RigaCaricataDalServer(e.model)) {
        if (CampoNonModificabile($(e.container[0]))) {
            e.sender.closeCell();
        }
    }
}

function LavAlt_disabilitaCell(e) {
    if (LavAlt_CampiObbligatoriSonoImpostati(e.model) && RigaCaricataDalServer(e.model)) {
        if (CampoNonModificabile($(e.container[0]))) {
            e.sender.closeCell();
        }
    }
}

function Setup_disabilitaCell(e) {
    if (Setup_CampiObbligatoriSonoImpostati(e.model) && RigaCaricataDalServer(e.model)) {
        if (CampoNonModificabile($(e.container[0]))) {
            e.sender.closeCell();
        }
    }
}

function Generic_disabilitaCell(e) {
    //if (Setup_CampiObbligatoriSonoImpostati(e.model) && RigaCaricataDalServer(e.model)) {
    //    if (CampoNonModificabile($(e.container[0]))) {
    //        e.sender.closeCell();
    //    }
    //}
}

function SetupDateRendicontazioni_onEdit(e) {
    //if (SetupDateRendicontazioni_CampiObbligatoriSonoImpostati(e.model) && RigaCaricataDalServer(e.model)) {
    //    if (CampoNonModificabile($(e.container[0]))) {
    //        e.sender.closeCell();
    //    }
    //}

    DateRendicontazione_controllaDati(e)
}

function DateRendicontazione_controllaDati(e) {
    var grid = $("#grdConfigurazioneDateRendicontazioni").data('kendoGrid');

    var indexColumnDataInizio = grid.wrapper.find(".k-grid-header [data-field=" + "Data_Inizio_Rendicontazione" + "]").index();
    var indexColumnDataFine = grid.wrapper.find(".k-grid-header [data-field=" + "Data_Fine_Rendicontazione" + "]").index();
    var indexColumnTermineUltimo = grid.wrapper.find(".k-grid-header [data-field=" + "Termine_Ultimo_Rendicontazione" + "]").index();
    var indexColumnDataFineBlocco = grid.wrapper.find(".k-grid-header [data-field=" + "Data_Fine_Blocco_Rendic_Conto_Proprio" + "]").index();

    var indexColumnAnnoRichiesta = grid.wrapper.find(".k-grid-header [data-field=" + "Anno_Richiesta" + "]").index();

    var rows = e.sender.tbody.children();

    //controlla la correttezza di ogni riga presente
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        var dataItem = e.sender.dataItem(row);

        if (dataItem.Data_Inizio_Rendicontazione != "" && dataItem.Data_Fine_Rendicontazione != "") {
            if (dataItem.Data_Inizio_Rendicontazione > dataItem.Data_Fine_Rendicontazione) {
                AddErrorClass(row, indexColumnDataInizio, "errorCell", "La Data Inizio Rendicontazione è superiore alla Data Fine");
                AddErrorClass(row, indexColumnDataFine, "errorCell", "La Data Fine Rendicontazione è minore della Data Inizio");
            } else {
                RemoveErrorClass(row, indexColumnDataInizio, "errorCell");
                RemoveErrorClass(row, indexColumnDataFine, "errorCell");
            }
        }

        if (dataItem.Data_Fine_Rendicontazione != "" && dataItem.Termine_Ultimo_Rendicontazione != "") {
            if (dataItem.Data_Fine_Rendicontazione > dataItem.Termine_Ultimo_Rendicontazione) {
                AddErrorClass(row, indexColumnTermineUltimo, "errorCell", "Il Termine Ultimo Rendicontazione è minore della Data Fine");
            } else {
                RemoveErrorClass(row, indexColumnTermineUltimo, "errorCell");
            }
        }

        if (dataItem.Anno_Richiesta != "") {
            let data = grid.dataSource.data();
            let datiFiltrati = data.filter(el => {
                return el.Tipo_Azienda == dataItem.Tipo_Azienda &&
                    el.Anno_Richiesta == dataItem.Anno_Richiesta
            });
            if (datiFiltrati.length > 1) {
                AddErrorClass(row, indexColumnAnnoRichiesta, "errorCell", "Esistono già valori per il tipo " + dataItem.Tipo_AziendaDes + " nell'anno " + dataItem.Anno_Richiesta.toString());
                return
            } else {
                RemoveErrorClass(row, indexColumnAnnoRichiesta, "errorCell");
            }
        }

        if ((dataItem.Data_Fine_Blocco_Rendic_Conto_Proprio != null &&
            dataItem.Data_Fine_Blocco_Rendic_Conto_Proprio != undefined &&
            dataItem.Data_Fine_Blocco_Rendic_Conto_Proprio != '') &&
            dataItem.Data_Fine_Blocco_Rendic_Conto_Proprio.getFullYear() < 2100) {
            if (dataItem.Tipo_Azienda != 1) {
                AddErrorClass(row, indexColumnDataFineBlocco, "errorCell", "La compilazione di questo campo è consentita solo se associato ad una configurazione di tipo Azienda Agricola Privata");
            } else if (dataItem.Data_Fine_Blocco_Rendic_Conto_Proprio > dataItem.Termine_Ultimo_Rendicontazione) {
                AddErrorClass(row, indexColumnDataFineBlocco, "errorCell", "Questa data non può essere superiore al Termine Ultimo Rendicontazione");
            } else if (dataItem.Data_Fine_Blocco_Rendic_Conto_Proprio < dataItem.Data_Inizio_Rendicontazione) {
                AddErrorClass(row, indexColumnDataFineBlocco, "errorCell", "Questa data non può essere inferiore alla Data Inizio Rendicontazione");
            } else {
                RemoveErrorClass(row, indexColumnDataFineBlocco, "errorCell");
            }
        } else {
            RemoveErrorClass(row, indexColumnDataFineBlocco, "errorCell");
        }

    }
}

function AddErrorClass(row, index, errClass, content) {
    row.children().eq(index).addClass(errClass);
    $(row.children().eq(index)).kendoTooltip({
        content: content,
        position: "top"
    });
}

function RemoveErrorClass(row, index, errClass) {
    if (row.children().eq(index).hasClass(errClass)) {
        row.children().eq(index).removeClass(errClass);
    }
}

function UMAConfigurazioneAllevamenti_disabilitaCell(e) {
    if (UMAConfigurazioneAllevamenti_CampiObbligatoriSonoImpostati(e.model) && RigaCaricataDalServer(e.model)) {
        if (CampoNonModificabile($(e.container[0]))) {
            e.sender.closeCell();
        }
    }
}

//function onTabPrincipaliShown(e) {
//   // Carica_Stati_PanelBar();

//    let target = $(e.target).attr("id"); // activated tab

//    switch (target) {
//        case "a_tabRiepilogoPesi":
//            break;

//        case "a_tabDettagliDoc":
//            break;
//    }
//}

function LavAlt_ControlloCampiObbligatoriImpostati(data, index, tipoElem) {
    let campiInvalidi = [];

    if (data.Gruppo_Colturale_UMA == null || data.Gruppo_Colturale_UMA === "" || data.Gruppo_Colturale_UMA === "Non Selezionato")
        campiInvalidi.push(TraduciLavorazioni("Gruppo_Colturale_UMA", "Macrouso UMA Descrizione"));

    if (data.Lavorazione_UMA == null || data.Lavorazione_UMA === "")
        campiInvalidi.push(TraduciLavorazioni("Lavorazione_UMA", "Lavorazione UMA Descrizione"));

    if (data.Lavorazione_UMA_Alt == null || data.Lavorazione_UMA_Alt === "")
        campiInvalidi.push(TraduciLavorazioni("Lavorazione_UMA_Alt", "Lavorazione Alternativa UMA Descrizione"));

    if (campiInvalidi.length === 0)
        return "";
    else
        return LavAlt_ValoriNonValidi(tipoElem, index);
}

function Setup_ControlloCampiObbligatoriImpostati(data, index, tipoElem) {
    let campiInvalidi = [];

    if (data.Anno == null || data.Anno === 0)
        campiInvalidi.push(TraduciLavorazioni("Anno", "Anno"));

    if (data.Anno < 1900 || data.anno > 2100)
        campiInvalidi.push(TraduciLavorazioni("Anno", "Anno"));

    if (campiInvalidi.length > 0)
        return Setup_ValoriNonValidi(tipoElem, index);

    campiInvalidi = [];

    if (data.Tipologia_Report_Elas == null)
        campiInvalidi.push(TraduciLavorazioni("TipologiaReportELAS", "Tipologia Report ELAS"));

    if (campiInvalidi.length > 0)
        return Setup_TipologieReportNonValide(tipoElem, index, campiInvalidi);

    return "";
}

function DateRendicontazioni_ControlloCampiObbligatoriImpostati(data, index, tipoElem) {
    let campiInvalidi = [];

    if (data.Anno_Richiesta == null || data.Anno_Richiesta === 0 || data.Anno_Richiesta === "")
        campiInvalidi.push(TraduciLavorazioni("Anno", "Anno"));

    if (data.Tipo_Azienda == null || data.Tipo_Azienda === 0 || data.Tipo_Azienda === "")
        campiInvalidi.push(TraduciLavorazioni("TipoAzienda", "Tipo Azienda"));

    if (data.Data_Inizio_Rendicontazione == null || data.Data_Inizio_Rendicontazione === "")
        campiInvalidi.push(TraduciLavorazioni("DataInizioRendicontazione", "Data Inizio Rendicontazione"));

    if (data.Data_Fine_Rendicontazione == null || data.Data_Fine_Rendicontazione === "")
        campiInvalidi.push(TraduciLavorazioni("DataFineRendicontazione", "Data Fine Rendicontazione"));

    if (data.Termine_Ultimo_Rendicontazione == null || data.Termine_Ultimo_Rendicontazione === "")
        campiInvalidi.push(TraduciLavorazioni("TermineUltimoRendicontazione", "Termine Ultimo Rendicontazione"));

    if (campiInvalidi.length === 0)
        return "";
    else
        return SetupDateRendicontazioni_ValoriNonValidi(tipoElem, campiInvalidi, index);
}

function ControlloCampiObbligatoriImpostati(data, index, tipoElem) {
    let campiInvalidi = []

    if (data.MacrousoUMACod == null || data.MacrousoUMACod === "")
        campiInvalidi.push("Macrouso UMA Desrizione");

    if (data.LavUMACod == null || data.LavUMACod === "")
        campiInvalidi.push("Lavorazione UMA Descrizione");

    if (data.LavCod == null || data.LavCod === 0)
        campiInvalidi.push("Operazioni Lav. Descrizione");

    if (data.IdAttivita == null)
        campiInvalidi.push("Attività Descrizione");

    if (!Number.isInteger(data.NMaxOperazioni))
        campiInvalidi.push("Max. Num. Operazioni");

    if (!Number.isInteger(data.Ordinamento))
        campiInvalidi.push("Ordinamento");

    if (campiInvalidi.length === 0)
        return "";
    else if (campiInvalidi.length === 1)
        return tipoElem + "Index " + (index + 1) + ". Campo: " + campiInvalidi[0] + " non contiene un valore valido.";
    else
        return tipoElem + "Index " + (index + 1) + ". Campi: " + campiInvalidi.join(", ") + " non contengono un valore valido.";
}

function UMAConfigurazioneAllevamenti_ControlloCampiObbligatoriImpostati(data, index, tipoElem) {
    let campiInvalidi = [];

    if (data.UMA_All_Cod == null || data.UMA_All_Cod === "")
        campiInvalidi.push(TraduciLavorazioni("UMA_All_Cod", "UMA_All_Cod"));

    if (data.Regione_Cod == null || data.Regione_Cod === "")
        campiInvalidi.push(TraduciLavorazioni("Regione_Cod", "Regione_Cod"));

    if (campiInvalidi.length === 0)
        return "";
    else
        return UMAConfigurazioneAllevamenti_ValoriNonValidi(tipoElem, index);
}

function LavAlt_ValoriNonValidi(tipoElem, index) {
    return tipoElem + TraduciLavorazioni("ErroreElem", "Errore all'indice") + " " + (index + 1) + ". I campi: Macrouso UMA Descrizione, Lavorazione UMA Descrizione e Lavorazione Alternativa UMA Descrizione sono obbligatori.";
}

function Setup_ValoriNonValidi(tipoElem, index) {
    return tipoElem + TraduciLavorazioni("ErroreElem", "Errore all'indice") + " " + (index + 1) + ". Il campo: Anno deve essere compreso fra 1900 e 2100";
}

function Setup_TipologieReportNonValide(tipoElem, index, campiInvalidi) {

    if (campiInvalidi.length === 1)
        return TraduciLavorazioni("ErroreElem", "Errore all'indice") + " " + (index + 1) + ". Il campo " + campiInvalidi[0] + " non contiene un valore valido.";
    else
        return TraduciLavorazioni("ErroreElem", "Errore all'indice") + " " + (index + 1) + ". I campi " + campiInvalidi.join(", ") + " non contengono un valore valido.";
}

function SetupDateRendicontazioni_ValoriNonValidi(tipoElem, campiInvalidi, index) {
    return tipoElem + TraduciLavorazioni("ErroreElem", "Errore alla riga ") + (index + 1) + ". <br>" +
        " I campi: " + campiInvalidi.join(", ") + " sono obbligatori.";
}

function UMAConfigurazioneAllevamenti_ValoriNonValidi(tipoElem, index) {
    return tipoElem + TraduciLavorazioni("ErroreElem", "Errore all'indice") + " " + (index + 1) + ".";
}

function LavAlt_ImpostaCampiDefault(data) {
    // Impostare un valore di default ai valori non nullable. Gli altri valori vengono gestiti server-side.
    if (data.Inviato == null)
        data.Inviato = 0;
}

function Setup_ImpostaCampiDefault(data) {
    // Impostare un valore di default ai valori non nullable. Gli altri valori vengono gestiti server-side.

    if (data.Inviato == null)
        data.Inviato = 0;

    if (data.Invio == null)
        data.Invio = new Date("1900/1/1");

    if (data.DataCreazione == null)
        data.DataCreazione = new Date("1900/1/1")

    if (data.DataModifica == null)
        data.DataModifica = new Date("1900/1/1");

    if (data.UsernameCreazione == null)
        data.UsernameCreazione = "";

    if (data.UsernameModifica == null)
        data.UsernameModifica = "";

    if (data.ValiditaInizio == null)
        data.ValiditaInizio = new Date("1900/1/1");

    if (data.ValiditaFine == null)
        data.ValiditaFine = new Date("2100/12/31");
}

function UF_CheckCampiDefault(data) {
    // Impostare un valore di default ai valori non nullable. Gli altri valori vengono gestiti server-side.

    if (data.Occupazione_Cod == null || data.Occupazione_Cod == 0)
        data.Occupazione_Cod = '000';

    if (data.Destinazione_Cod == null || data.Destinazione_Cod == 0)
        data.Destinazione_Cod = '000';

    if (data.Uso_Cod == null || data.Uso_Cod == 0)
        data.Uso_Cod = '000'

    if (data.Qualita_Cod == null || data.Qualita_Cod == 0)
        data.Qualita_Cod = '000';

    if (data.ValiditaInizio == null)
        data.ValiditaInizio = new Date("1900/1/1");

    if (data.ValiditaFine == null)
        data.ValiditaFine = new Date("2100/12/31");

    if (data.Inviato == null)
        data.Inviato = 0;

    if (data.DataCreazione == null)
        data.DataCreazione = new Date("1900/1/1")

    if (data.DataModifica == null)
        data.DataModifica = new Date("1900/1/1");

    if (data.UsernameCreazione == null)
        data.UsernameCreazione = "";

    if (data.UsernameModifica == null)
        data.UsernameModifica = "";

    data.datainvio = null;
}

function ElencoMacrousi_CheckCampiDefault(data) {
    // Impostare un valore di default ai valori non nullable. Gli altri valori vengono gestiti server-side.

    if (data.Macrouso_UMA_Cod == null)
        data.Macrouso_UMA_Cod = '000';

    if (data.ValiditaInizio == null)
        data.ValiditaInizio = new Date("1900/1/1");

    if (data.ValiditaFine == null)
        data.ValiditaFine = new Date("2100/12/31");

    if (data.Inviato == null)
        data.Inviato = 0;

    if (data.DataCreazione == null)
        data.DataCreazione = new Date("1900/1/1")

    if (data.DataModifica == null)
        data.DataModifica = new Date("1900/1/1");

    if (data.UsernameCreazione == null)
        data.UsernameCreazione = "";

    if (data.UsernameModifica == null)
        data.UsernameModifica = "";

    data.datainvio = null;
}

function ElencoLavorazioni_CheckCampiDefault(data) {
    // Impostare un valore di default ai valori non nullable. Gli altri valori vengono gestiti server-side.

    if (data.Lav_UMA_Cod == null)
        data.Lav_UMA_Cod = '000';

    if (data.ValiditaInizio == null)
        data.ValiditaInizio = new Date("1900/1/1");

    if (data.ValiditaFine == null)
        data.ValiditaFine = new Date("2100/12/31");

    if (data.Inviato == null)
        data.Inviato = 0;

    if (data.DataCreazione == null)
        data.DataCreazione = new Date("1900/1/1")

    if (data.DataModifica == null)
        data.DataModifica = new Date("1900/1/1");

    if (data.UsernameCreazione == null)
        data.UsernameCreazione = "";

    if (data.UsernameModifica == null)
        data.UsernameModifica = "";

    data.datainvio = null;
}

function ElencoAllevamenti_CheckCampiDefault(data) {
    // Impostare un valore di default ai valori non nullable. Gli altri valori vengono gestiti server-side.

    if (data.UMA_All_Cod == null)
        data.UMA_All_Cod = '000';

    if (data.UMA_AllGru_Cod == null)
        data.UMA_AllGru_Cod = '000';

    if (data.ValiditaInizio == null)
        data.ValiditaInizio = new Date("1900/1/1");

    if (data.ValiditaFine == null)
        data.ValiditaFine = new Date("2100/12/31");

    if (data.Inviato == null)
        data.Inviato = 0;

    if (data.DataCreazione == null)
        data.DataCreazione = new Date("1900/1/1")

    if (data.DataModifica == null)
        data.DataModifica = new Date("1900/1/1");

    if (data.UsernameCreazione == null)
        data.UsernameCreazione = "";

    if (data.UsernameModifica == null)
        data.UsernameModifica = "";

    data.datainvio = null;
}

function DateRendicontazione_ImpostaCampiDefault(data) {
    // Impostare un valore di default ai valori non nullable. Gli altri valori vengono gestiti server-side.

    if (data.Inviato == null)
        data.Inviato = 0;

    if (data.DataInvio == null)
        data.DataInvio = new Date("1900/1/1");

    if (data.DataCreazione == null)
        data.DataCreazione = new Date("1900/1/1")

    if (data.DataModifica == null)
        data.DataModifica = new Date("1900/1/1");

    if (data.UsernameCreazione == null)
        data.UsernameCreazione = "";

    if (data.UsernameModifica == null)
        data.UsernameModifica = "";

    if (data.ValiditaInizio == null)
        data.ValiditaInizio = new Date("1900/1/1");

    if (data.ValiditaFine == null)
        data.ValiditaFine = new Date("2100/12/31");
}

function UMAConfigurazioneAllevamenti_ImpostaCampiDefault(data) {
    // Impostare un valore di default ai valori non nullable. Gli altri valori vengono gestiti server-side.

    if (data.Regione_Cod == null)
        data.Regione_Cod = "010";

    if (data.TipoOperazioneCod == null)
        data.TipoOperazioneCod = 1;

    if (data.GasolioLt == null)
        data.GasolioLt = 0;

    if (data.BenzinaLt == null)
        data.BenzinaLt = 0;

    if (data.Qta_Aggiuntiva_Carro == null)
        data.Qta_Aggiuntiva_Carro = 0;

    if (data.N_Max_Allevamenti == null)
        data.N_Max_Allevamenti = 1;

    if (data.inviato == null)
        data.inviato = 0;

    if (data.datainvio == null)
        data.datainvio = new Date("1900/1/1");

    if (data.DataCreazione == null)
        data.DataCreazione = new Date("1900/1/1")

    if (data.DataModifica == null)
        data.DataModifica = new Date("1900/1/1");

    if (data.UsernameCreazione == null)
        data.UsernameCreazione = "";

    if (data.UsernameModifica == null)
        data.UsernameModifica = "";

    if (data.ValiditaInizio == null)
        data.ValiditaInizio = new Date("1900/1/1");

    if (data.ValiditaFine == null)
        data.ValiditaFine = new Date("2100/12/31");

    if (data.ufl_min == null)
        data.ufl_min = 0;

    if (data.ufl_max == null)
        data.ufl_max = 0;

    if (data.ufc_min == null)
        data.ufc_min = 0;

    if (data.ufc_max == null)
        data.ufc_max = 0;
}

function ImpostaCampiDefault(data) {
    if (data.RegioneCod == null)
        data.RegioneCod = "010";

    if (data.TipoOperazioneCod == null)
        data.TipoOperazioneCod = 1;

    if (data.GasolioLt == null)
        data.GasolioLt = 0;

    if (data.BenzinaLt == null)
        data.BenzinaLt = 0;

    if (data.Ordinamento == null)
        data.Ordinamento = 0;

    if (data.NMaxOperazioni == null)
        data.NMaxOperazioni = 0;

    if (data.Default == null)
        data.Default = 0;

    if (data.Inviato == null)
        data.Inviato = 0;

    if (data.Invio == null)
        data.Invio = new Date("1900/1/1");

    if (data.DataCreazione == null)
        data.DataCreazione = new Date()

    if (data.DataModifica == null)
        data.DataModifica = new Date();

    if (data.UsernameCreazione == null)
        data.UsernameCreazione = "";

    if (data.UsernameModifica == null)
        data.UsernameModifica = "";

    if (data.ValiditaInizio == null)
        data.ValiditaInizio = new Date();

    if (data.ValiditaFine == null)
        data.ValiditaFine = new Date("2100/12/31");

    if (data.UDMAlternativa == null)
        data.UDMAlternativa = "";

    if (data.GasolioLTxBiologico == null)
        data.GasolioLTxBiologico = 0;

    if (data.BenzinaLTxBiologico == null)
        data.BenzinaLTxBiologico = 0;

    if (data.LimiteMax == null)
        data.LimiteMax = 0;

    if (data.MaxxHa == null)
        data.MaxxHa = 0;
}

function LavAlt_Lavorazione_UMA_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#" + grdLavorazioniAlternative).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (LavAlt_CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
        return;

    LavAlt_PopolaElenco_Lavorazione_UMA().then(
        elenco => {
            creaDropDownEditor(container, "LavUMA_Lav_UMA_Des", "Lavorazione_UMA", elenco, LavAlt_ChangeLavorazioneUMA);
        }
    )
}

function LavAlt_Lavorazione_UMA_Alt_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#" + grdLavorazioniAlternative).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (LavAlt_CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
        return;


    LavAlt_PopolaElenco_Lavorazione_UMA_Alt().then(
        elenco => {
            creaDropDownEditor(container, "LavUMAAlt_Lav_UMA_Des", "Lavorazione_UMA_Alt", elenco, LavAlt_ChangeAltLavorazioneUMA);
        }
    )
}

function LavAlt_Gruppo_Colturale_UMA_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#" + grdLavorazioniAlternative).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (LavAlt_CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
        return;


    LavAlt_PopolaElenco_Gruppo_Colturale_UMA().then(
        elenco => {
            creaDropDownEditor(container, "Macrouso_UMA_Des", "Gruppo_Colturale_UMA", elenco, LavAlt_ChangeGruppoColturale);
        }
    )
}

// *************************************** Fill dropdown list for MacrousoUMADes ***************************************
function Programmazione_MacrousoUMADes_DropDownEditor(container, options) {
    WaitFrame.show();

    let rowHtml = $(container).parents("tr")[0];
    
    let grid = $("#grdConfigurazioniUMA").data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
        return;

    PopolaElenco_MacrousoUMACod().then(
        elenco_MacrousoUMACod => {
            WaitFrame.hide();
            creaDropDownEditor(container, "Macrouso_UMA_Des", "Macrouso_UMA_Cod", elenco_MacrousoUMACod, change_macrousoUMACod);
        }
    )
}

// *************************************** Fill dropdown list for LavorazioniUMA ***************************************
function Programmazione_LavorazioniUMA_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#grdConfigurazioniUMA").data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
        return;

    PopolaElenco_UMALavorazioni().then(
        elenco_lavorazioniUMA => {
            creaDropDownEditor(container, "UMA_Lavorazioni_Des", "UMA_Lavorazioni_Cod", elenco_lavorazioniUMA, change_lavorazioniUMA);
        }
    )
}

// *************************************** Fill dropdown list for Operazioni ***************************************
function Programmazione_Operazioni_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#grdConfigurazioniUMA").data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
        return;

    PopolaElenco_Operazioni().then(
        elenco_operazioni => {
            creaDropDownEditor(container, "Operazioni_Des", "Operazioni_Cod", elenco_operazioni, change_operazioni);
        }
    )

}

// *************************************** Fill dropdown list for Attivita ***************************************
function Programmazione_Attivita_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#grdConfigurazioniUMA").data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    if (CampiObbligatoriSonoImpostati(row) && RigaCaricataDalServer(row))
        return;

    PopolaElenco_Attivita().then(
        elenco_attivita => {
            creaDropDownEditor(container, "Attivita_Des", "Attivita_Cod", elenco_attivita, change_attivita);
        }
    )
}
// *************************************** Fill dropdown list for Tipo Operazione ***************************************
function Programmazione_TipoOperazione_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#grdConfigurazioniUMA").data("kendoGrid");

    let elencoOrd = new Array();
    elencoOrd.push(
        {
            TipoOperazioneDes: "Ordinaria",
            TipoOperazioneCod: 1,
        }
    );
    elencoOrd.push(
        {
            TipoOperazioneDes: "Straordinaria",
            TipoOperazioneCod: 2,
        }
    );

    creaDropDownEditor(container, "TipoOperazioneDes", "TipoOperazioneCod", elencoOrd, change_tipoOperazione);
    
}

// *************************************** Fill dropdown list for Gruppi Allevamento ***************************************
function Programmazione_GruppiAllevamentoUMA_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#grdElencoAllevamentiUMA").data("kendoGrid");
    //let row = grid.dataItem(rowHtml);

    PopolaElenco_GruppiAllevamentoUMA().then(
        elenco_GruppiAllevamentoUMA => {
            creaDropDownEditor(container, "UMA_AllGru_Des", "UMA_AllGru_Cod", elenco_GruppiAllevamentoUMA, change_gruppiAllevamentoUMA);
        }
    )
}

function Programmazione_MacrousiUMA_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#grdAssociazioniMacrousiUMA").data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    PopolaElenco_MacrousiUMA().then(
        elenco_MacrousiUMA => {
            creaDropDownEditor(container, "Macrouso_UMA_Des", "Macrouso_UMA_Cod", elenco_MacrousiUMA, change_associazioniMacrousoUMA);
        }
    )
}

function MacrousiUMA_Load() {

    $('#dllMacrousoUMA').kendoDropDownList({
        filter: "contains",
        dataSource: { transport: { read: RiempiElencoMacrousiUMA } },
        dataTextField: "Macrouso_UMA_Des",
        dataValueField: "Macrouso_UMA_Cod",
        //optionLabel: { "stato": TraduzioneMultiResx(gestioneCarbResx, "Seleziona", "Seleziona").toUpperCase() + " TUTTI", "cod": "-1" },
        autoWidth: true,
        dataBound: ddlMacrousiUMA_OnDataBound
    });

}

function ddlMacrousiUMA_OnDataBound(e) {
    var ds = this.dataSource.data();
    if (ds.length == 1) {
        this.select(1); //seleziono l'elemento 
        //ddlStatiPratiche.onchange(); //forzo l'evento di onchange
    }
}

function FlagNoteCompObbl_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#grdConfigurazioniUMA").data("kendoGrid");

    let elencoNote = new Array();
    elencoNote.push(
        {
            FlagNoteCompObblDes: "NO",
            FlagNoteCompObbl: 0,
        }
    );
    elencoNote.push(
        {
            FlagNoteCompObblDes: "SI",
            FlagNoteCompObbl: 1,
        }
    );

    creaDropDownEditor(container, "FlagNoteCompObblDes", "FlagNoteCompObbl", elencoNote, change_noteobbligatorie);
}

function FlagTrueFalse_Maggiorazione_Terreno_MedioTenace_DropDownEditor(container, options) {
    let elencoNote = new Array();
    elencoNote.push(
        {
            Text: "False",
            Value: false,
        }
    );
    elencoNote.push(
        {
            Text: "True",
            Value: true,
        }
    );

    creaDropDownEditor(container, "Text", "Value", elencoNote, change_Maggiorazione_Terreno_MedioTenace);
}

function FlagTrueFalse_GestioneTerzista_DropDownEditor(container, options) {
    let elencoNote = new Array();
    elencoNote.push(
        {
            Text: "False",
            Value: false,
        }
    );
    elencoNote.push(
        {
            Text: "True",
            Value: true,
        }
    );

    creaDropDownEditor(container, "Text", "Value", elencoNote, change_GestioneTerzista);
}

function FlagTrueFalse_Utilizzata_Da_Consorzio_Bonifica_DropDownEditor(container, options) {
    let elencoNote = new Array();
    elencoNote.push(
        {
            Text: "False",
            Value: false,
        }
    );
    elencoNote.push(
        {
            Text: "True",
            Value: true,
        }
    );

    creaDropDownEditor(container, "Text", "Value", elencoNote, change_Utilizzata_Da_Consorzio_Bonifica);
}

//caricamento della dropdown Tipo Aziende nella sezione delle date di rendicontazioni
function Programmazione_TipoAzienda_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#grdConfigurazioneDateRendicontazioni").data("kendoGrid");

    let elencoTipiAzienda = new Array();
    elencoTipiAzienda.push(
        {
            TipoAziendaDes: "Azienda Agricola Privata",
            TipoAzienda: 1,
        }
    );
    elencoTipiAzienda.push(
        {
            TipoAziendaDes: "Azienda Terzista",
            TipoAzienda: 2,
        }
    );
    elencoTipiAzienda.push(
        {
            TipoAziendaDes: "Cooperativa Agricola",
            TipoAzienda: 3,
        }
    );
    elencoTipiAzienda.push(
        {
            TipoAziendaDes: "Azienda Agricola Pubblica",
            TipoAzienda: 4,
        }
    );
    elencoTipiAzienda.push(
        {
            TipoAziendaDes: "Consorzio di Bonifica e Irrigazione",
            TipoAzienda: 5,
        }
    );

    creaDropDownEditor(container, "TipoAziendaDes", "TipoAzienda", elencoTipiAzienda, change_tipoAzienda);


}

// *************************************** Fill dropdown list for UMA_All_Des ***************************************

function UMAConfigurazioneAllevamenti_Allevamenti_DropDownEditor(container, options) {
    var arrTipi = [];
    arrTipi = UMAConfigurazioneAllevamenti_elencoAllevamenti();
    creaDropDownEditor(container, "UMA_All_Des", "UMA_All_Cod", arrTipi, UMAConfigurazioneAllevamenti_Allevamenti_DropDownChange);
}

// *************************************** Fill dropdown list for Tipo_Operazione ***************************************

function Programmazione_Tipo_Operazione_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    let grid = $("#grdUMAConfigurazioneAllevamenti").data("kendoGrid");

    let elencoOrd = new Array();
    elencoOrd.push(
        {
            Tipo_Operazione_Des: "Ordinaria",
            Tipo_Operazione: 1,
        }
    );
    elencoOrd.push(
        {
            Tipo_Operazione_Des: "Straordinaria",
            Tipo_Operazione: 2,
        }
    );

    creaDropDownEditor(container, "Tipo_Operazione_Des", "Tipo_Operazione", elencoOrd, UMAConfigurazioneAllevamenti_ChangeTipoOperazione);

}

function UMAConfigurazioneAllevamenti_Allevamenti_DropDownChange(ev) {
    var dataItem = ev.sender.dataItem();
    var gridID = "grdUMAConfigurazioneAllevamenti";
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    var indexColUmaAllDes = grid.thead.find("th[data-field='UMA_All_Des']").index();

    model.dirty = true;

    model.UMA_All_Cod = dataItem.UMA_All_Cod;
    model.dirtyFields.UMA_All_Cod = true;

    model.UMA_All_Des = dataItem.UMA_All_Des;
    model.dirtyFields.UMA_All_Des = true;

    //  ucUmaAllevamenti_calcolaFabbisogno(model);

    //kendoFastRedrawRow(grid, row);
    grid.refresh(); // In questo modo ri-effettuo anche l'evento dataBound per i controlli di validità
}

function change_tipoAzienda(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#grdConfigurazioneDateRendicontazioni").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Tipo_Azienda = dataItem.TipoAzienda;
        model.Tipo_AziendaDes = dataItem.TipoAziendaDes;
        model.dirty = true;
    }
    grid.refresh();
}

function change_tipoOperazione(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#grdConfigurazioniUMA").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.TipoOperazioneCod = dataItem.TipoOperazioneCod;
        model.TipoOperazioneDes = dataItem.TipoOperazioneDes;
        model.dirty = true;
    }
    grid.refresh();
}

function change_noteobbligatorie(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#grdConfigurazioniUMA").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.FlagNoteCompObbl = dataItem.FlagNoteCompObbl;
        model.FlagNoteCompObblDes = dataItem.FlagNoteCompObblDes;
        model.dirty = true;
    }
    grid.refresh();
}

function change_Maggiorazione_Terreno_MedioTenace(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#grdElencoLavorazioniUMA").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    console.log(dataItem.Value);

    if (model != null) {
        model.Maggiorazione_Terreno_MedioTenace = dataItem.Value;
        model.dirty = true;
    }
    grid.refresh();
}

function change_GestioneTerzista(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#grdElencoLavorazioniUMA").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.GestioneTerzista = dataItem.Value;
        model.dirty = true;
    }
    grid.refresh();
}

function change_Utilizzata_Da_Consorzio_Bonifica(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#grdElencoLavorazioniUMA").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Utilizzata_Da_Consorzio_Bonifica = dataItem.Value;
        model.dirty = true;
    }
    grid.refresh();
}

function LavAlt_ChangeAltLavorazioneUMA(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#" + grdLavorazioniAlternative).data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.LavUMAAlt_Lav_UMA_Des = dataItem.LavUMAAlt_Lav_UMA_Des;
        model.Lavorazione_UMA_Alt = dataItem.Lavorazione_UMA_Alt;
        model.dirty = true;
    }

    grid.refresh();
}

function LavAlt_ChangeLavorazioneUMA(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#" + grdLavorazioniAlternative).data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.LavUMA_Lav_UMA_Des = dataItem.LavUMA_Lav_UMA_Des;
        model.Lavorazione_UMA = dataItem.Lavorazione_UMA;
        model.dirty = true;
    }

    grid.refresh();
}

function LavAlt_ChangeGruppoColturale(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#" + grdLavorazioniAlternative).data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Macrouso_UMA_Des = dataItem.Macrouso_UMA_Des;
        model.Gruppo_Colturale_UMA = dataItem.Gruppo_Colturale_UMA;
        model.dirty = true;
    }

    grid.refresh();
}

function change_macrousoUMACod(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#grdConfigurazioniUMA").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.MacrousoUMACod = dataItem.Macrouso_UMA_Cod;
        model.UMAMacrousi_MacrousoUMADes = dataItem.Macrouso_UMA_Des;
        model.dirty = true;
    }

    grid.refresh();
}

function change_associazioniMacrousoUMA(e) {

    let dataItem = e.sender.dataItem();

    let grid = $("#grdAssociazioniMacrousiUMA").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Macrouso_UMA_Cod = dataItem.Macrouso_UMA_Cod;
        model.Macrouso_UMA_Des = dataItem.Macrouso_UMA_Des;
        model.dirty = true;
    }

    grid.refresh();
}

function change_lavorazioniUMA(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#grdConfigurazioniUMA").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.LavUMACod = dataItem.UMA_Lavorazioni_Cod;
        model.UMALavorazioni_LavUmaDes = dataItem.UMA_Lavorazioni_Des;
        model.dirty = true;
    }

    grid.refresh();
}

function change_operazioni(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#grdConfigurazioniUMA").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.LavCod = dataItem.Operazioni_Cod;
        model.Operazioni_LavDeS = dataItem.Operazioni_Des;
        model.dirty = true;
    }

    grid.refresh();
}

function change_attivita(e) {
    let dataItem = e.sender.dataItem();
    let grid = $("#grdConfigurazioniUMA").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.IdAttivita = dataItem.Attivita_Cod;
        model.Attivita_Desc = dataItem.Attivita_Des;
        model.dirty = true;
    }

    grid.refresh();
}

function change_gruppiAllevamentoUMA(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    model.UMA_AllGru_Cod = dataItem.UMA_AllGru_Cod;
    model.UMA_AllGru_Des = dataItem.UMA_AllGru_Des;
    model.dirty = true;

    grid.refresh();
    
}

function UMAConfigurazioneAllevamenti_ChangeTipoOperazione(e) {

    let dataItem = e.sender.dataItem();
    let grid = $("#" + grdUMAConfigurazioneAllevamenti).data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Tipo_Operazione = dataItem.Tipo_Operazione;
        model.Tipo_Operazione_Des = dataItem.Tipo_Operazione_Des;
        model.dirty = true;
    }

    grid.refresh();
}

async function LavorazioniAlternative_SubmitGrid(options) {
    var grid = $("#" + grdLavorazioniAlternative).data("kendoGrid");


    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            var errMess = LavAlt_ControlloCampiObbligatoriImpostati(currentData[i], i, TraduciLavorazioni("NuoviElementi", "Nuovi elementi. "));

            if (errMess === "") {
                LavAlt_ImpostaCampiDefault(currentData[i]);
                newRecords.push(currentData[i].toJSON());
            }
            else {
                MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
                return;
            }
        }
        else if (currentData[i].dirty) {
            var errMess = LavAlt_ControlloCampiObbligatoriImpostati(currentData[i], i, TraduciLavorazioni("ElementiModificati. ", "Elementi modificati. "));

            if (errMess === "") {
                LavAlt_ImpostaCampiDefault(currentData[i]);
                updatedRecords.push(currentData[i].toJSON());
            }
            else {
                MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
                return;
            }
        }
    }

    let elementiUgualiErrore = arrayContieneElementiUguali(newRecords, "lav_alt");
    if (elementiUgualiErrore !== null) {
        MessaggioErrore_Bootstrap(elementiUgualiErrore, "DIV_Messaggi");
        return;
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        ImpostaCampiDefault(grid.dataSource._destroyed[i]);
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    // Salvataggio righe //
    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        await LavAlt_InviaRigheModificate(newRecords, updatedRecords, deletedRecords);

        let grid = $("#" + grdLavorazioniAlternative).data("kendoGrid");
        grid.dataSource.read();
        grid.refresh();
    }
}

async function SubmitGrid_Lavorazioni(options) {
    var grid = $("#grdConfigurazioniUMA").data("kendoGrid");


    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            var errMess = ControlloCampiObbligatoriImpostati(currentData[i], i, "Nuovi elementi. ");

            if (errMess === "") {
                ImpostaCampiDefault(currentData[i]);
                newRecords.push(currentData[i].toJSON());
            }
            else {
                MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
                return;
            }
        }
        else if (currentData[i].dirty) {
            var errMess = ControlloCampiObbligatoriImpostati(currentData[i], i, "Elementi modificati. ");

            if (errMess === "") {
                ImpostaCampiDefault(currentData[i]);
                updatedRecords.push(currentData[i].toJSON());
            }
            else {
                MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
                return;
            }
        }
    }

    let elementiUgualiErrore = arrayContieneElementiUguali(newRecords, "uma_lav");
    if (elementiUgualiErrore !== null) {
        MessaggioErrore_Bootstrap(elementiUgualiErrore, "DIV_Messaggi");
        return;
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        ImpostaCampiDefault(grid.dataSource._destroyed[i]);
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    // Salvataggio righe //
    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        await InviaRigheModificate(newRecords, updatedRecords, deletedRecords);

        let grid = $("#grdConfigurazioniUMA").data("kendoGrid");
        grid.dataSource.read();
        grid.refresh();
    }
}

async function Setup_SubmitGrid(options) {
    var grid = $("#grdSetup").data("kendoGrid");


    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            var errMess = Setup_ControlloCampiObbligatoriImpostati(currentData[i], i, "Nuovi elementi. ");

            if (errMess === "") {
                Setup_ImpostaCampiDefault(currentData[i]);
                newRecords.push(currentData[i].toJSON());
            }
            else {
                MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
                return;
            }
        }
        else if (currentData[i].dirty) {
            var errMess = Setup_ControlloCampiObbligatoriImpostati(currentData[i], i, "Elementi modificati. ");

            if (errMess === "") {
                Setup_ImpostaCampiDefault(currentData[i]);
                updatedRecords.push(currentData[i].toJSON());
            }
            else {
                MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
                return;
            }
        }
    }

    let elementiUgualiErrore = arrayContieneElementiUguali(newRecords, "uma_setup");
    if (elementiUgualiErrore !== null) {
        MessaggioErrore_Bootstrap(elementiUgualiErrore, "DIV_Messaggi");
        return;
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        Setup_ImpostaCampiDefault(grid.dataSource._destroyed[i]);
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    // Salvataggio righe //
    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        await Setup_InviaRigheModificate(newRecords, updatedRecords, deletedRecords);

        let grid = $("#grdSetup").data("kendoGrid");
        grid.dataSource.read();
        grid.refresh();
    }
}

async function UF_SubmitGrid(options) {
    var grid = $("#grdConfigurazioneUF").data("kendoGrid");

    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();
    var ok = true;
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            if (currentData[i].Occupazione_Cod == "000") {
                kendo.alert("E' necessario specificare un codice di occupazione diverso da '000'");
                ok = false;
            } else {
                UF_CheckCampiDefault(currentData[i]);
                newRecords.push(currentData[i].toJSON());
            }
        }
        else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    // Salvataggio righe //
    if (ok && (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0)) {

        await UF_InviaRigheModificate(newRecords, updatedRecords, deletedRecords);

        let grid = $("#grdConfigurazioneUF").data("kendoGrid");
        grid.dataSource.read();
        grid.refresh();
    }
}

async function ElencoMacrousi_SubmitGrid(options) {
    var grid = $("#grdElencoMacrousiUMA").data("kendoGrid");

    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            if (/^\d+$/.test(currentData[i].Macrouso_UMA_Cod) && currentData[i].Macrouso_UMA_Cod[0] !== '0') {
                ElencoMacrousi_CheckCampiDefault(currentData[i]);
                newRecords.push(currentData[i].toJSON());
            } else {
                kendo.alert("E' necessario inserire un codice di Macrouso composto da sole cifre numeriche di cui la prima diversa da zero");
            }
        }
        else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    // Salvataggio righe //
    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        await ElencoMacrousi_InviaRigheModificate(newRecords, updatedRecords, deletedRecords);

        let grid = $("#grdElencoMacrousiUMA").data("kendoGrid");
        grid.dataSource.read();
        grid.refresh();
    }
}

async function ElencoLavorazioni_SubmitGrid(options) {
    var grid = $("#grdElencoLavorazioniUMA").data("kendoGrid");

    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            if (/^\d+$/.test(currentData[i].Lav_UMA_Cod) && currentData[i].Lav_UMA_Cod[0] !== '0') {
                ElencoLavorazioni_CheckCampiDefault(currentData[i]);
                newRecords.push(currentData[i].toJSON());
            } else {
                kendo.alert("E' necessario inserire un codice di Lavorazione composto da sole cifre numeriche di cui la prima diversa da zero");
            }
        }
        else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    // Salvataggio righe //
    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        await ElencoLavorazioni_InviaRigheModificate(newRecords, updatedRecords, deletedRecords);

        let grid = $("#grdElencoLavorazioniUMA").data("kendoGrid");
        grid.dataSource.read();
        grid.refresh();
    }
}

async function ElencoAllevamenti_SubmitGrid(options) {
    var grid = $("#grdElencoAllevamentiUMA").data("kendoGrid");

    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            ElencoAllevamenti_CheckCampiDefault(currentData[i]);
            newRecords.push(currentData[i].toJSON());
        }
        else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    // Salvataggio righe //
    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        await ElencoAllevamenti_InviaRigheModificate(newRecords, updatedRecords, deletedRecords);

        let grid = $("#grdElencoAllevamentiUMA").data("kendoGrid");
        grid.dataSource.read();
        grid.refresh();
    }
}

async function ElencoAssociazioniMacrousi_SubmitGrid(options) {
    var grid = $("#grdAssociazioniMacrousiUMA").data("kendoGrid");

    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();

    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    // Salvataggio righe //
    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        await ElencoAssociazioniMacrousi_InviaRigheModificate(newRecords, updatedRecords, deletedRecords);

        let grid = $("#grdAssociazioniMacrousiUMA").data("kendoGrid");
        grid.dataSource.read();
        grid.refresh();
    }
}

async function DateRendicontazioni_SubmitGrid(options) {
    var grid = $("#grdConfigurazioneDateRendicontazioni").data("kendoGrid");


    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var errMess = ""

    var currentData = grid.dataSource.data();

    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            errMess = DateRendicontazioni_ControlloCampiObbligatoriImpostati(currentData[i], i, "Nuovi elementi. ");

            if (errMess === "") {
                newRecords.push(currentData[i].toJSON());
            }

        } else if (currentData[i].dirty) {
            errMess = DateRendicontazioni_ControlloCampiObbligatoriImpostati(currentData[i], i, "Elementi modificati. ");

            if (errMess === "") {
                updatedRecords.push(currentData[i].toJSON());
            }
        }

        if (errMess !== "") {
            MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
            return;
        } else {
            //DateRendicontazione_ImpostaCampiDefault(currentData[i]);
        }
    }

    let elementiUgualiErrore = arrayContieneElementiUguali(currentData, "date_rendicontazioni");
    if (elementiUgualiErrore !== null) {
        MessaggioErrore_Bootstrap(elementiUgualiErrore, "DIV_Messaggi");
        return;
    }

    let datiIncorretti = DateRendicontazione_datiIncorretti(currentData);
    if (datiIncorretti.length > 0) {
        MessaggioErrore_Bootstrap(datiIncorretti.join("<br>"), "DIV_Messaggi");
        return;
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        //DateRendicontazione_ImpostaCampiDefault(grid.dataSource._destroyed[i]);
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    // Salvataggio righe //
    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        await DateRendicontazioni_InviaRigheModificate(newRecords, updatedRecords, deletedRecords);

        let grid = $("#grdConfigurazioneDateRendicontazioni").data("kendoGrid");
        grid.dataSource.read();
        grid.refresh();
    }
}


function DateRendicontazione_datiIncorretti(arr) {
    var datiIncorretti = []
    for (let i = 0; i < arr.length - 1; ++i) {
        if (arr[i].Data_Inizio_Rendicontazione > arr[i].Data_Fine_Rendicontazione) {
            datiIncorretti.push("Riga " + (i + 1).toString() + ". La Data Inizio Rendicontazione è superiore alla Data Fine")
        }

        if (arr[i].Data_Fine_Rendicontazione > arr[i].Termine_Ultimo_Rendicontazione) {
            datiIncorretti.push("Riga " + (i + 1).toString() + ". Il Termine Ultimo Rendicontazione è minore della Data Fine")
        }
        if ((arr[i].Data_Fine_Blocco_Rendic_Conto_Proprio != null || arr[i].Data_Fine_Blocco_Rendic_Conto_Proprio != undefined) && arr[i].Data_Fine_Blocco_Rendic_Conto_Proprio.getFullYear() < 2100) {
            if (arr[i].Tipo_Azienda != 1) {
                datiIncorretti.push("Riga " + (i + 1).toString() + ". La compilazione del campo 'Data da cui permettere la chiusura rendicontazione conto proprio anche in mancanza di chiusura del terzista' è consentita solo se associato ad una configurazione di tipo Azienda Agricola Privata")
            } else if (arr[i].Data_Fine_Blocco_Rendic_Conto_Proprio > arr[i].Termine_Ultimo_Rendicontazione) {
                datiIncorretti.push("Riga " + (i + 1).toString() + ". Questa data non può essere superiore al Termine Ultimo Rendicontazione");
            } else if (arr[i].Data_Fine_Blocco_Rendic_Conto_Proprio < arr[i].Data_Inizio_Rendicontazione) {
                datiIncorretti.push("Riga " + (i + 1).toString() + ". Questa data non può essere inferiore alla Data Inizio Rendicontazione");
            }
        }
    }
    return datiIncorretti;
}

async function UMAConfigurazioneAllevamenti_SubmitGrid(options) {
    var grid = $("#grdUMAConfigurazioneAllevamenti").data("kendoGrid");


    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    var currentData = grid.dataSource.data();
    for (let i = 0; i < currentData.length; i++) {
        if (currentData[i].isNew()) {
            var errMess = UMAConfigurazioneAllevamenti_ControlloCampiObbligatoriImpostati(currentData[i], i, "Nuovi elementi. ");

            if (errMess === "") {
                UMAConfigurazioneAllevamenti_ImpostaCampiDefault(currentData[i]);
                newRecords.push(currentData[i].toJSON());
            }
            else {
                MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
                return;
            }
        }
        else if (currentData[i].dirty) {
            var errMess = UMAConfigurazioneAllevamenti_ControlloCampiObbligatoriImpostati(currentData[i], i, "Elementi modificati. ");

            if (errMess === "") {
                UMAConfigurazioneAllevamenti_ImpostaCampiDefault(currentData[i]);
                updatedRecords.push(currentData[i].toJSON());
            }
            else {
                MessaggioErrore_Bootstrap(errMess, "DIV_Messaggi");
                return;
            }
        }
    }

    let elementiUgualiErrore = arrayContieneElementiUguali(newRecords, "uma_configurazioneAllevamenti");
    if (elementiUgualiErrore !== null) {
        MessaggioErrore_Bootstrap(elementiUgualiErrore, "DIV_Messaggi");
        return;
    }

    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {
        UMAConfigurazioneAllevamenti_ImpostaCampiDefault(grid.dataSource._destroyed[i]);
        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    // Salvataggio righe //
    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        await UMAConfigurazioneAllevamenti_InviaRigheModificate(newRecords, updatedRecords, deletedRecords);

        let grid = $("#grdUMAConfigurazioneAllevamenti").data("kendoGrid");
        grid.dataSource.read();
        grid.refresh();
    }
}

function arrayContieneElementiUguali(arr, etype) {
    for (let i = 0; i < arr.length - 1; ++i) {
        for (let j = i + 1; j < arr.length; ++j) {
            switch (etype) {
                case "lav_alt":
                    if (LavAlt_sonoUguali(arr[i], arr[j]))
                        return LavAlt_MessaggioCampiUguali(arr[i]);
                    break;
                //case "uma_lav":
                //    if (UMALav_sonoUguali(arr[i], arr[j]))
                //        return MessaggioCampiUguali(arr[i]);
                //    break;
                case "uma_setup":
                    if (UMASetup_sonoUguali(arr[i], arr[j]))
                        return Setup_MessaggioCampiUguali(arr[i]);
                    break;
                case "uma_configurazioneAllevamenti":
                    if (UMAConfigurazioneAllevamenti_sonoUguali(arr[i], arr[j]))
                        return UMAConfigurazioneAllevamenti_MessaggioCampiUguali(arr[i]);
                    break;
                case "date_rendicontazioni":
                    if (DateRendicontazione_sonoUguali(arr[i], arr[j]))
                        return DateRendicontazione_MessaggioCampiUguali(arr[i]);
                    break;
            }
        }
    }
    return null;
}

function LavAlt_MessaggioCampiUguali(elem) {
    let elemUguali = TraduciLavorazioni("elementiUguali", "Ci sono stati trovati due elementi uguali:");
    let grpColUMA = TraduciLavorazioni("Gruppo_Colturale_UMA", "Macrouso UMA Descrizione");
    let lavUMA = TraduciLavorazioni("Lavorazione_UMA", "Lavorazione UMA Descrizione");
    let lavUMAAlt = TraduciLavorazioni("Lavorazione_UMA_Alt", "Lavorazione Alternativa UMA Descrizione");

    return elemUguali + grpColUMA + "=" + elem.Macrouso_UMA_Des + " " + lavUMA + "=" + elem.LavUMA_Lav_UMA_Des + " " + lavUMAAlt + "=" + elem.LavUMAAlt_Lav_UMA_Des;
}

function Setup_MessaggioCampiUguali(elem) {
    let elemUguali = TraduciLavorazioni("elementiUguali", "Sono sono stati trovati due elementi uguali:");
    return elemUguali + " Anno='" + elem.Anno;
}

function UMAConfigurazioneAllevamenti_MessaggioCampiUguali(elem) {
    let elemUguali = TraduciLavorazioni("elementiUguali", "Sono sono stati trovati due elementi uguali:");
    return elemUguali + " UMA_All_Cod='" + elem.UMA_All_Cod;
}

function DateRendicontazione_MessaggioCampiUguali(elem) {
    return "Esistono già valori per il tipo " + elem.Tipo_AziendaDes + " nell'anno " + elem.Anno_Richiesta.toString();
}

function MessaggioCampiUguali(elem) {
    let elemUguali = TraduciLavorazioni("elementiUguali", "Ci sono stati trovati due elementi uguali:");
    return elemUguali + " Operazioni Lav. Descrizione='" + elem.Operazioni_LavDeS + "', Macrousi UMA Descrizione='" + elem.UMAMacrousi_MacrousoUMADes + "', Lavorazioni UMA Descrizione='" + elem.UMALavorazioni_LavUmaDes + "', Attivita Descrizione='" + elem.Attivita_Desc + "'";
}

function LavAlt_sonoUguali(p1, p2) {
    return p1.Gruppo_Colturale_UMA === p2.Gruppo_Colturale_UMA && p1.Lavorazione_UMA === p2.Lavorazione_UMA && p1.Lavorazione_UMA_Alt === p2.Lavorazione_UMA_Alt && p1.ValiditaInizio && p2.ValiditaInizio;
}

function UMALav_sonoUguali(p1, p2) {
    return p1.RegioneCod === p2.RegioneCod && p1.LavCod === p2.LavCod && p1.MacrousoUMACod === p2.MacrousoUMACod && p1.LavUMACod === p2.LavUMACod && p1.IdAttivita === p2.IdAttivita;
}

function UMASetup_sonoUguali(p1, p2) {
    return p1.Anno === p2.Anno;
}

function UMAConfigurazioneAllevamenti_sonoUguali(p1, p2) {
    //  return p1.UMA_All_Cod === p2.UMA_All_Cod;
}

function DateRendicontazione_sonoUguali(p1, p2) {
    return p1.Tipo_Azienda === p2.Tipo_Azienda
        && p1.Anno_Richiesta === p2.Anno_Richiesta;
}

function kReadConfigurazione_mod() {

    return {
        "Chiave": { "editable": false, "type": "string", validation: { required: true } },
        "Regione_Cod": { "editable": false, "type": "string", validation: { required: true } },
        "Macrouso_UMA_Cod": { "editable": true, "type": "string", validation: { required: true } },
        "Macrouso_UMA_Des": { "editable": true, "type": "string", validation: { required: true } },
        "Lav_UMA_Cod": { "editable": true, "type": "string", validation: { required: true } },
        "Lav_UMA_Des": { "editable": true, "type": "string", validation: { required: true } },
        "Lav_Cod": { "editable": true, "type": "string", validation: { required: true } },
        "Lav_Des": { "editable": true, "type": "string", validation: { required: true } },
        "Id_Attivita": { "editable": true, "type": "string", validation: { required: true } },
        "Attivita": { "editable": true, "type": "string", validation: { required: true } },
        "Tipo_Operazione": { "editable": true, "type": "number", validation: { required: true } },
        "Gasolio_Lt": { "editable": true, "type": "number", validation: { required: true } },
        "Benzina_Lt": { "editable": true, "type": "number", validation: { required: true } },
        "Ordinamento": { "editable": true, "type": "number", validation: { required: true } },
        "N_Max_Operazioni": { "editable": true, "type": "number", validation: { required: true } }
    };

}

function CaricaCampiKendoModel(colonna_editabile) {
    return {
        Operazioni_LavDeS: { editable: colonna_editabile, type: "string", defaultValue: "" },
        UMAMacrousi_MacrousoUMADes: { editable: colonna_editabile, type: "string", defaultValue: "" },
        UMALavorazioni_LavUmaDes: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Attivita_Desc: { editable: colonna_editabile, type: "string", defaultValue: "" },

        RegioneCod: { editable: colonna_editabile, type: "string", defaultValue: "" },
        LavCod: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        MacrousoUMACod: { editable: colonna_editabile, type: "string", defaultValue: "" },
        LavUMACod: { editable: colonna_editabile, type: "string", defaultValue: "" },
        IdAttivita: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        TipoOperazioneCod: { editable: colonna_editabile, type: "number", defaultValue: 1 },
        TipoOperazioneDes: { editable: colonna_editabile, type: "string", defaultValue: "Ordinaria" },
        GasolioLt: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        BenzinaLt: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        Ordinamento: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        NMaxOperazioni: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        Default: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        DataCreazione: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1") },
        DataModifica: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1") },
        UsernameCreazione: { editable: colonna_editabile, type: "string", defaultValue: "" },
        UsernameModifica: { editable: colonna_editabile, type: "string", defaultValue: "" },
        ValiditaInizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1") },
        ValiditaFine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") },
        UDMAlternativa: { editable: colonna_editabile, type: "string", defaultValue: "" },
        GasolinoLTxBiologico: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        BenzinaLTxBiologico: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        LimiteMax: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        MaxxHa: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        ID: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        Modificabile: { editable: false, type: "boolean", defaultValue: true },
        Regolamento_Cod: { editable: colonna_editabile, type: "number", defaultValue: 1 },
        Regolamento_CodDes: { editable: colonna_editabile, type: "string", defaultValue: "Convenzionale" },
        Coefficiente_Distribuzione_Acqua: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        FlagNoteCompObbl: { editable: false, type: "number", defaultValue: 0 },
        FlagNoteCompObblDes: { editable: colonna_editabile, type: "string" },
    };
}

function LavAlt_CaricaCampiKendoModel(colonna_editabile) {
    return {
        Chiave: { editable: true, type: "number" },
        Macrouso_UMA_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },

        LavUMA_Lav_UMA_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },
        LavUMAAlt_Lav_UMA_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },

        Gruppo_Colturale_UMA: { type: "string", defaultValue: "" },
        Lavorazione_UMA: { type: "string", defaultValue: "" },
        Lavorazione_UMA_Alt: { type: "string", defaultValue: "" },

        Validita_Inizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1") },
        Validita_Fine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") },

        Inviato: { type: "number", defaultValue: 0 },
        DataInvio: { type: "date", defaultValue: new Date("1900/1/1") },
        Data_Creazione: { type: "date", defaultValue: new Date("1900/1/1") },
        Data_Modifica: { type: "date", defaultValue: new Date("1900/1/1") },
        Username_Creazione: { type: "date", defaultValue: "" },
        Username_Modifica: { type: "date", defaultValue: "" },
        Modificabile: { editable: false, type: "boolean", defaultValue: true },
        Regolamento_Cod: { editable: colonna_editabile, type: "number", defaultValue: 1 },
        Regolamento_CodDes: { editable: colonna_editabile, type: "string", defaultValue: "Convenzionale" },
    };
}

function LavAlt_CaricaColonneKendoGrid() {
    return [
        {
            field: "Macrouso_UMA_Des",
            title: TraduciLavorazioni("MacrousoUMADescrizione", "Macrouso UMA Descrizione"),
            filterable: { multi: true, search: true },
            editor: LavAlt_Gruppo_Colturale_UMA_DropDownEditor,
            attributes: { class: "edit_onInsert" },
            width: 200
        },
        {
            field: "Regolamento_CodDes",
            title: TraduciLavorazioni("Regolamento_Cod ", "Regolamento"),
            filterable: { multi: true, search: true },
            editor: Regolamento_Cod_DropDownEditor,
            attributes: { class: "edit_onInsert" },
            width: 150
        },
        {
            field: "LavUMA_Lav_UMA_Des",
            title: TraduciLavorazioni("LavorazioneUMA_Desc", "Lavorazione UMA Descrizione"),
            filterable: { multi: true, search: true },
            editor: LavAlt_Lavorazione_UMA_DropDownEditor,
            attributes: { class: "edit_onInsert" }
        },
        {
            field: "LavUMAAlt_Lav_UMA_Des",
            title: TraduciLavorazioni("LavorazioneUMAAlt_Desc", "Lavorazione Alternativa UMA Descrizione"),
            filterable: { multi: true, search: true },
            editor: LavAlt_Lavorazione_UMA_Alt_DropDownEditor,
            attributes: { class: "edit_onInsert" }
        },
        {
            field: "Validita_Inizio",
            title: TraduciLavorazioni("ValiditaInizio", "Validità Inizio"),
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #',
            width: larghezzaStdCampoData
        },
        {
            field: "Validita_Fine",
            title: TraduciLavorazioni("ValiditaFine", "Validità Fine"),
            template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Validita_Fine, "dd/MM/yyyy" ) #',
            width: larghezzaStdCampoData
        },
    ];
}

function CaricaColonneKendoGrid() {
    return [
        {
            field: "UMAMacrousi_MacrousoUMADes",
            title: TraduciLavorazioni("Macrouso_UMA_Des", "Macrousi UMA Descrizione"),
            filterable: { multi: true, search: true },
            editor: Programmazione_MacrousoUMADes_DropDownEditor,
            attributes: { class: "edit_onInsert" }
        },
        {
            field: "Regolamento_CodDes",
            title: TraduciLavorazioni("Regolamento_Cod ", "Regolamento"),
            filterable: { multi: true, search: true },
            editor: Regolamento_Cod_DropDownEditor,
            attributes: { class: "edit_onInsert" }
        },
        {
            field: "UMALavorazioni_LavUmaDes",
            title: TraduciLavorazioni("lavorazioniUMADes", "Lavorazioni UMA Descrizione"),
            filterable: { multi: true, search: true },
            editor: Programmazione_LavorazioniUMA_DropDownEditor,
            attributes: { class: "edit_onInsert" }
        },
        {
            field: "Operazioni_LavDeS",
            title: TraduciLavorazioni("operazioniLavDes", "Operazioni Lav. Descrizione"),
            filterable: { multi: true, search: true },
            editor: Programmazione_Operazioni_DropDownEditor,
            attributes: { class: "edit_onInsert" }
        },
        {
            field: "Attivita_Desc",
            title: TraduciLavorazioni("attivitaDescrizione", "Attivita Descrizione"),
            filterable: { multi: true, search: true },
            editor: Programmazione_Attivita_DropDownEditor,
            attributes: { class: "edit_onInsert" }
        },
        {
            field: "TipoOperazioneDes",
            title: TraduciLavorazioni("tipoOp", "Tipo Operazione"),
            filterable: { multi: true, search: true },
            editor: Programmazione_TipoOperazione_DropDownEditor,
        },
        { field: "GasolioLt", title: TraduciLavorazioni("GasolioLT", "Gasolio Lt.") },
        { field: "BenzinaLt", title: TraduciLavorazioni("BenzinaLT", "Benzina Lt.") },
        { field: "Ordinamento", title: TraduciLavorazioni("Ordinamento", "Ordinamento"), hidden: true },
        { field: "NMaxOperazioni", title: TraduciLavorazioni("MaxNumOp", "Max Num. Operazioni") },
        { field: "Default", title: TraduciLavorazioni("Default", "Default"), hidden: true },
        {
            field: "ValiditaInizio",
            title: TraduciLavorazioni("ValiditaInizio", "Validità Inizio"),
            format: "{0:dd/MM/yyyy}",
            template: '#= (kendo.toString(ValiditaInizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(ValiditaInizio, "dd/MM/yyyy" ) #'
        },
        {
            field: "ValiditaFine",
            title: TraduciLavorazioni("ValiditaFine", "Validità Fine"),
            format: "{0:dd/MM/yyyy}",
            template: '#= (kendo.toString(ValiditaFine, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(ValiditaFine, "dd/MM/yyyy" ) #',
            editor: function (container, options) { //impostare la data minima selezionabile a oggi oppure alla data inserita
                let rowHtml = $(container).parents("tr")[0];
                let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;
                let grid = $("#" + ID_Grid).data("kendoGrid");
                let row = grid.dataItem(rowHtml);
                var minDate = row.ValiditaInizio;

                $('<input data-bind="value:' + options.field + '" />')
                    .appendTo(container)
                    .kendoDatePicker({
                        min: minDate,
                        format: "dd/MM/yyyy",
                        parseFormats: ["dd/MM/yyyy"]
                    });
            },
        },
        {
            field: "UDMAlternativa",
            title: TraduciLavorazioni("UDMAlternativa", "UDM Alternativa"),
            filterable: { multi: true, search: true }
        },
        {
            field: "GasolinoLTxBiologico",
            title: TraduciLavorazioni("GasolioLTBio", "Gasolio LT Biologico"),
            hidden: true
        },
        {
            field: "BenzinaLTxBiologico",
            title: TraduciLavorazioni("BensinaLTBio", "Bensina LT Biologico"),
            hidden: true
        },
        { field: "LimiteMax", title: TraduciLavorazioni("LimiteMax", "Limite Max"), editor: limiteMaxEditor },
        { field: "MaxxHa", title: TraduciLavorazioni("maxxHa", "Max xHa") },
        { field: "Coefficiente_Distribuzione_Acqua", title: TraduciLavorazioni("Coefficiente_Distribuzione_Acqua", "Coefficiente Distribuzione Acqua") },
        {
            field: "FlagNoteCompObblDes",
            title: TraduciLavorazioni("FlagNoteCompObbl", "Note Compilatore Obbligatorie"),
            filterable: { multi: true, search: true },
            editor: FlagNoteCompObbl_DropDownEditor,
        },
    ];
}

function limiteMaxEditor(container, options) {
    $('<input data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            min: 0,
            max: 1
        });
}

function Setup_CaricaCampiKendoModel(colonna_editabile) {
    return {
        Chiave: { editable: true, type: "number" },
        Anno: { editable: colonna_editabile, type: "number", defaultValue: "", validation: { required: true } },

        Per_riduzione: { editable: colonna_editabile, type: "number", defaultValue: "" },
        Per_Mag_Terreno_B: { editable: colonna_editabile, type: "number", defaultValue: "" },
        Per_Mag_Terreno_Medio: { editable: colonna_editabile, type: "number", defaultValue: "" },
        Per_Mag_Terreno_Tenace: { editable: colonna_editabile, type: "number", defaultValue: "" },
        Altre_Cfg: { editable: colonna_editabile, type: "string", defaultValue: "1" },
        Nr_Litri_Maggiorazione: { editable: colonna_editabile, type: "number", defaultValue: "" },
        Percentuale_Integrazione_Terzista: { editable: colonna_editabile, type: "number", defaultValue: "" },

        //Validita_Inizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1") },
        //Validita_Fine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") },

        Inviato: { type: "number", defaultValue: 0 },
        DataInvio: { type: "date", defaultValue: new Date("1900/1/1") },
        Data_Creazione: { type: "date", defaultValue: new Date("1900/1/1") },
        Data_Modifica: { type: "date", defaultValue: new Date("1900/1/1") },
        Username_Creazione: { type: "date", defaultValue: "" },
        Username_Modifica: { type: "date", defaultValue: "" },
        Modificabile: { editable: false, type: "boolean", defaultValue: true },
        Vincola_Rendicontazione_e_Richiesta: { editable: true, type: "string", defaultValue: "Abilitato" },
        Gestione_Biologico: { editable: true, type: "number", defaultValue: 0, validation: { required: true } },
        Gestione_BiologicoDes: { editable: colonna_editabile, type: "string", defaultValue: "Disabilitato " },
        Gestione_Rimanenze: { editable: true, type: "number", defaultValue: 0, validation: { required: true } },
        Gestione_RimanenzeDes: { editable: colonna_editabile, type: "string", defaultValue: "Disabilitato" },
        Gestione_Anticipazioni_Colturali: { editable: true, type: "number", defaultValue: 0, validation: { required: true } },
        Gestione_Anticipazioni_ColturaliDes: { editable: colonna_editabile, type: "string", defaultValue: "Tutte" },
        Percentuale_Richieste_Anticipo: { editable: true, type: "number", defaultValue: 0, validation: { required: true } },
        Macchine_Targa_Obbligatoria: { editable: true, defaultValue: "0", validation: { required: false } },
        Macchine_Targa_ObbligatoriaDes: { editable: true, defaultValue: "Tutto", validation: { required: false } },
        Macchine_Targa_ObbligatoriaString: { editable: true, type: "string", defaultValue: "0", validation: { required: false } },
        Macchine_Targa_ObbligatoriaDesString: { editable: true, type: "string", defaultValue: "", validation: { required: false } },
        Tipologia_Report_Elas: { editable: true, type: "number", defaultValue: 0, validation: { required: true } },
        Tipologia_Report_ElasDes: { editable: colonna_editabile, type: "string", defaultValue: "Nessuna" },
        Tipologia_Elenco_Inadempienti: { editable: false, type: "number", defaultValue: 0, validation: { required: true } },
        Tipologia_Elenco_InadempientiDes: { editable: colonna_editabile, type: "String", defaultValue: "Nessuna", validation: { required: true } },
        Tipologia_Report_SegnalazioneAccise: { editable: colonna_editabile, type: "number", defaultValue: 0, validation: { required: true } },
        Tipologia_Report_SegnalazioneAcciseDes: { editable: colonna_editabile, type: "String", defaultValue: "Nessuna", validation: { required: true } },

        Stati_Invio_Mail: { editable: true, defaultValue: "0", validation: { required: false } },
        Stati_Invio_MailDes: { editable: true, defaultValue: "Tutti", validation: { required: false } },
        Stati_Invio_MailString: { editable: true, type: "string", defaultValue: "0", validation: { required: false } },
        Stati_Invio_MailDesString: { editable: true, type: "string", defaultValue: "", validation: { required: false } },

        Gruppi_Utenti_Invio_Mail: { editable: true, defaultValue: "0", validation: { required: false } },
        Gruppi_Utenti_Invio_MailDes: { editable: true, defaultValue: "Tutti", validation: { required: false } },
        Gruppi_Utenti_Invio_MailString: { editable: true, type: "string", defaultValue: "0", validation: { required: false } },
        Gruppi_Utenti_Invio_MailDesString: { editable: true, type: "string", defaultValue: "", validation: { required: false } },
    };
}

function Setup_CaricaColonneKendoGrid() {
    return [
        {
            field: "Anno",
            title: TraduciLavorazioni("Anno", "Anno"),
            filterable: { multi: true, search: true },
            attributes: { class: "edit_onInsert" },
            width: larghezzaStdCampoNumerico
        },
        {
            field: "Per_riduzione",
            title: TraduciLavorazioni("PerRiduzione", "% Riduzione"),
            filterable: { multi: true, search: true },
            width: larghezzaStdCampoNumerico
        },
        {
            field: "Per_Mag_Terreno_B",
            title: TraduciLavorazioni("PerMagTerrenoB", "% Magg. Terreno B"),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "Per_Mag_Terreno_Medio",
            title: TraduciLavorazioni("PerMagTerrenoMedio", "% Magg. Terreno Medio"),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "Per_Mag_Terreno_Tenace",
            title: TraduciLavorazioni("PerMagTerrenoTenace", "% Magg. Terreno Tenace"),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "Nr_Litri_Maggiorazione",
            title: TraduciLavorazioni("NrLitriMaggiorazione", "Nr Litri Magg. Trasferim."),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "Percentuale_Integrazione_Terzista",
            title: TraduciLavorazioni("PercentualeIntegrazioneTerzista", "% Acq. Min. Integrazione Terzista"),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "Percentuale_Richieste_Anticipo",
            title: TraduciLavorazioni("Percentuale_Richieste_Anticipo", "Percentuale Richieste Anticipo"),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "Vincola_Rendicontazione_e_Richiesta",
            title: TraduciLavorazioni("Vincola_Rendicontazione_e_Richiesta", "Vincola Rendicontazione e Richiesta"),
            filterable: { multi: true, search: true },
            editor: VincolaRendRich_DropDownEditor,
            width: larghezzaStdCampoGestione
        },
        {
            field: "Gestione_BiologicoDes",
            title: TraduciLavorazioni("Gestione_Biologico", "Gestione Biologico"),
            filterable: { multi: true, search: true },
            editor: SiNo_Gestione_Biologico_DropDownEditor,
            width: larghezzaStdCampoGestione
        },
        {
            field: "Gestione_RimanenzeDes",
            title: TraduciLavorazioni("Gestione_Rimanenze", "Gestione Rimanenze"),
            filterable: { multi: true, search: true },
            editor: SiNo_Gestione_Rimanenze_DropDownEditor,
            width: larghezzaStdCampoGestione
        },
        {
            field: "Gestione_Anticipazioni_ColturaliDes",
            title: TraduciLavorazioni("Gestione_Anticipazioni_Colturali", "Gestione Anticipazioni Colturali"),
            filterable: { multi: true, search: true },
            editor: SiNo_Gestione_Anticipazioni_Colturali_DropDownEditor,
            width: larghezzaStdCampoGestione
        },
        {
            field: "Macchine_Targa_ObbligatoriaDesString",
            title: TraduciLavorazioni("Macchine_Targa_Obbligatoria", "Macchine Soggette a Controllo Targa Obbligatoria"),
            filterable: { multi: true, search: true },
            editor: Macchine_Targa_Obbligatoria_MultiselectEditor,
            width: 200
        },
        {
            field: "Tipologia_Report_ElasDes",
            title: TraduciLavorazioni("Tipologia_Report_Elas", "Tipologia Report Elas"),
            filterable: { multi: true, search: true },
            editor: Setup_TipologiaReportELAS_DropDownEditor,
            width: 200
        },
        {
            field: "Tipologia_Elenco_InadempientiDes",
            title: TraduciLavorazioni("Tipologia_Elenco_Inadempienti", "Tipologia Elenco Inadempienti"),
            filterable: { multi: true, search: true },
            editor: Setup_TipologiaElenco_Inadempienti_DropDownEditor,
            width: 200
        },
        {
            field: "Tipologia_Report_SegnalazioneAcciseDes",
            title: TraduciLavorazioni("Tipologia_Report_SegnalazioneAccise", "Tipologia Report Segnalazione Recupero Accise"),
            filterable: { multi: true, search: true },
            editor: Setup_TipologiaReport_SegnalazioneAccise_DropDownEditor,
            width: 200
        },
        {
            field: "Stati_Invio_MailDesString",
            title: TraduciLavorazioni("Stati_Invio_Mail", "Stati Invio Mail"),
            filterable: { multi: true, search: true },
            editor: Stati_Invio_Mail_MultiselectEditor,
            width: 200
        },
        {
            field: "Gruppi_Utenti_Invio_MailDesString",
            title: TraduciLavorazioni("Gruppi_Utenti_Invio_Mail", "Gruppi utenti invio mail"),
            filterable: { multi: true, search: true },
            editor: Gruppi_Utenti_Invio_Mail_MultiselectEditor,
            width: 200
        },
        ];
}

function DateRendicontazione_CaricaCampiKendoModel(colonna_editabile) {
    return {
        chiave: { editable: false, type: "string" },
        Tipo_Azienda: { editable: true, type: "number", defaultValue: "", validation: { required: true } },
        Tipo_AziendaDes: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Anno_Richiesta: { editable: true, type: "number", defaultValue: "", validation: { min: 2010, max: 2100, required: true } },

        Data_Inizio_Rendicontazione: { editable: true, type: "date", defaultValue: "", validation: { required: true } },
        Data_Limite_INS_Azienda_Terzista: { editable: true, type: "date", defaultValue: "" },
        Data_Fine_Rendicontazione: { editable: true, type: "date", defaultValue: "", validation: { required: true } },
        Termine_Ultimo_Rendicontazione: { editable: true, type: "date", defaultValue: "", validation: { required: true } },
        //Inviato: { type: "number", defaultValue: 0 },
        //DataInvio: { type: "date", defaultValue: new Date("1900/1/1") },
        //Data_Creazione: { type: "date", defaultValue: new Date("1900/1/1") },
        //Data_Modifica: { type: "date", defaultValue: new Date("1900/1/1") },
        //Username_Creazione: { type: "date", defaultValue: "" },
        //Username_Modifica: { type: "date", defaultValue: "" },

        //Validita_Inizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1") },
        //Validita_Fine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") },
        Data_Limite_INS_Richiesta_Anticipo: { editable: colonna_editabile, type: "date", defaultValue: "" },
        Data_Fine_Blocco_Rendic_Conto_Proprio: { editable: colonna_editabile, type: "date", defaultValue: "" },
    };
}

function DateRendicontazione_CaricaColonneKendoGrid() {
    return [
        {
            field: "Tipo_AziendaDes",
            title: TraduciLavorazioni("TipoAzienda", "Tipo Azienda"),
            filterable: { multi: true, search: true },
            editor: Programmazione_TipoAzienda_DropDownEditor,
            attributes: { class: "edit_onInsert" }
        },
        {
            field: "Anno_Richiesta",
            title: TraduciLavorazioni("AnnoRichiesta", "Anno Richiesta"),
            filterable: { multi: true, search: true },
            attributes: { class: "edit_onInsert" },
            width: larghezzaStdCampoNumerico
        },
        {
            field: "Data_Inizio_Rendicontazione",
            title: TraduciLavorazioni("DataInizioRendicontazione", "Data Inizio Rendicontazione"),
            template: '#= (kendo.toString(Data_Inizio_Rendicontazione, "dd/MM/yyyy" ))  #',
            width: larghezzaStdCampoData
        },
        {
            field: "Data_Fine_Rendicontazione",
            title: TraduciLavorazioni("DataFineRendicontazione", "Data Fine Rendicontazione"),
            template: '#= (kendo.toString(Data_Fine_Rendicontazione, "dd/MM/yyyy" ))  #',
            width: larghezzaStdCampoData
        },
        {
            field: "Data_Limite_INS_Azienda_Terzista",
            title: TraduciLavorazioni("DataLimiteINSAziendaTerzista", "Data Limite Inserimento Nuovi CUAA per Azienda Terzista"),
            template: '#= ((kendo.toString(Data_Limite_INS_Azienda_Terzista, "dd/MM/yyyy") === null) ||' +
                '(kendo.toString(Data_Limite_INS_Azienda_Terzista, "dd/MM/yyyy" ) === "31/12/2100") ||' +
                '(kendo.toString(Data_Limite_INS_Azienda_Terzista, "dd/MM/yyyy") === "01/01/1900")) ? "" : kendo.toString(Data_Limite_INS_Azienda_Terzista, "dd/MM/yyyy" ) #',
            width: larghezzaStdCampoData
        },
        {
            field: "Termine_Ultimo_Rendicontazione",
            title: TraduciLavorazioni("TermineUltimoRendicontazione", "Termine Ultimo Rendicontazione"),
            template: '#= (kendo.toString(Termine_Ultimo_Rendicontazione, "dd/MM/yyyy" ))  #',
            width: larghezzaStdCampoData
        },
        //{
        //    field: "Validita_Inizio",
        //    title: TraduciLavorazioni("ValiditaInizio", "Validità Inizio"),
        //    template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #'
        //},
        //{
        //    field: "Validita_Fine",
        //    title: TraduciLavorazioni("ValiditaFine", "Validità Fine"),
        //    template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Validita_Fine, "dd/MM/yyyy" ) #'
        //},
        {
            field: "Data_Limite_INS_Richiesta_Anticipo",
            title: TraduciLavorazioni("DataLimiteINSRichiestaAnticipo", "Data Limite Inserimento Richiesta Anticipo"),
            template: '#= ((kendo.toString(Data_Limite_INS_Richiesta_Anticipo, "dd/MM/yyyy") === null) ||' +
                '(kendo.toString(Data_Limite_INS_Richiesta_Anticipo, "dd/MM/yyyy" ) === "31/12/2100") ||' +
                '(kendo.toString(Data_Limite_INS_Richiesta_Anticipo, "dd/MM/yyyy") === "01/01/1900")) ? "" : kendo.toString(Data_Limite_INS_Richiesta_Anticipo, "dd/MM/yyyy" ) #',
            width: larghezzaStdCampoData
        },
        {
            field: "Data_Fine_Blocco_Rendic_Conto_Proprio",
            title: TraduciLavorazioni("DataFineBloccoRendicContoProprio", "Data da cui permettere la chiusura rendicontazione conto proprio anche in mancanza di chiusura del terzista"),
            template: '#= ((kendo.toString(Data_Fine_Blocco_Rendic_Conto_Proprio, "dd/MM/yyyy") === null) ||' +
                '(kendo.toString(Data_Fine_Blocco_Rendic_Conto_Proprio, "dd/MM/yyyy" ) === "31/12/2100") ||' +
                '(kendo.toString(Data_Fine_Blocco_Rendic_Conto_Proprio, "dd/MM/yyyy") === "01/01/1900")) ? "" : kendo.toString(Data_Fine_Blocco_Rendic_Conto_Proprio, "dd/MM/yyyy" ) #',
            width: larghezzaStdCampoData
        }

    ];
}

function UMAConfigurazioneAllevamenti_CaricaCampiKendoModel(colonna_editabile) {
    return {
        Chiave: { editable: false, type: "string" },
        Regione_Cod: { editable: false, type: "string", defaultValue: "010" },
        UMA_All_Cod: { editable: false, type: "string", validation: { required: true } },

        UMA_All_Des: { editable: colonna_editabile, type: "string", defaultValue: "" },
        Tipo_Operazione: { editable: colonna_editabile, type: "number", defaultValue: 1 },
        Tipo_Operazione_Des: { editable: colonna_editabile, type: "string", defaultValue: "Ordinaria" },
        Gasolio_Lt: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        Benzina_Lt: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        Qta_Aggiuntiva_Carro: { editable: colonna_editabile, type: "number", defaultValue: 0 },
        N_Max_Allevamenti: { editable: colonna_editabile, type: "number", defaultValue: 1 },

        Validita_Inizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1") },
        Validita_Fine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") },

        inviato: { type: "number", defaultValue: 0 },
        datainvio: { type: "date", defaultValue: new Date("1900/1/1") },
        Data_Creazione: { type: "date", defaultValue: new Date("1900/1/1") },
        Data_Modifica: { type: "date", defaultValue: new Date("1900/1/1") },
        Username_Creazione: { type: "date", defaultValue: "" },
        Username_Modifica: { type: "date", defaultValue: "" },
        ID: { type: "number", defaultValue: "0" },
        ufl_min: { type: "number", defaultValue: "0" },
        ufl_max: { type: "number", defaultValue: "0" },
        ufc_min: { type: "number", defaultValue: "0" },
        ufc_max: { type: "number", defaultValue: "0" },
    };
}

function UMAConfigurazioneAllevamenti_CaricaColonneKendoGrid() {
    return [
        {
            field: "UMA_All_Des",
            title: TraduciLavorazioni("UMA_All_Des", "Descrizione"),
            filterable: { multi: true, search: true },
            //attributes: { class: "edit_onInsert" },
            editor: UMAConfigurazioneAllevamenti_Allevamenti_DropDownEditor,
            width: 300
        },
        {
            field: "Tipo_Operazione_Des",
            title: TraduciLavorazioni("Tipo_Operazione_Des", "Tipo Operazione"),
            filterable: { multi: true, search: true },
            editor: Programmazione_Tipo_Operazione_DropDownEditor,
            width: 120
        },
        {
            field: "Gasolio_Lt",
            title: TraduciLavorazioni("Gasolio_Lt", "Gasolio Lt"),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "Benzina_Lt",
            title: TraduciLavorazioni("Benzina_Lt", "Benzina Lt"),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "Qta_Aggiuntiva_Carro",
            title: TraduciLavorazioni("Qta_Aggiuntiva_Carro", "Q.ta Aggiuntiva Carro"),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "N_Max_Allevamenti",
            title: TraduciLavorazioni("N_Max_Allevamenti", "Nr Max Allevamenti"),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "Validita_Inizio",
            title: TraduciLavorazioni("ValiditaInizio", "Validità Inizio"),
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #',
            width: larghezzaStdCampoData
        },
        {
            field: "Validita_Fine",
            title: TraduciLavorazioni("ValiditaFine", "Validità Fine"),
            template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Validita_Fine, "dd/MM/yyyy" ) #',
            width: larghezzaStdCampoData
        },
        {
            field: "ufl_min",
            title: TraduciLavorazioni("ufl_min", "Unità Foraggere Latte (Min)"),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "ufl_max",
            title: TraduciLavorazioni("ufl_max", "Unità Foraggere Latte (Max)"),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "ufc_min",
            title: TraduciLavorazioni("ufc_min", "Unità Foraggere Carne (Min)"),
            width: larghezzaStdCampoNumerico
        },
        {
            field: "ufc_max",
            title: TraduciLavorazioni("ufc_max", "Unità Foraggere Carne (Max)"),
            width: larghezzaStdCampoNumerico
        },

    ];
}

function UF_CaricaCampiKendoModel(colonna_editabile) {
    return {
        //Chiave: { editable: true, type: "number" },
        Occupazione_Cod: { editable: colonna_editabile, type: "string", defaultValue: "000", validation: { required: true } },
        Occupazione_Des: { editable: colonna_editabile, type: "string", defaultValue: "", validation: { required: true } },
        Destinazione_Cod: { editable: colonna_editabile, type: "string", defaultValue: "000", validation: { required: true } },
        Destinazione_Des: { editable: colonna_editabile, type: "string", defaultValue: "", validation: { required: true } },
        Uso_Cod: { editable: colonna_editabile, type: "string", defaultValue: "000", validation: { required: true } },
        Uso_Des: { editable: colonna_editabile, type: "string", defaultValue: "", validation: { required: true } },
        Qualita_Cod: { editable: colonna_editabile, type: "string", defaultValue: "000", validation: { required: true } },
        Qualita_Des: { editable: colonna_editabile, type: "string", defaultValue: "", validation: { required: true } },
        UF_Ha: { editable: colonna_editabile, type: "number", defaultValue: 0, validation: { required: true } },
        UFL_Ha: { editable: colonna_editabile, type: "number", defaultValue: 0, validation: { required: true } },
        UFC_Ha: { editable: colonna_editabile, type: "number", defaultValue: 0, validation: { required: true } },
        UF_Ha_Irrigua: { editable: colonna_editabile, type: "number", defaultValue: 0, validation: { required: true } },
        UFL_Ha_Irrigua: { editable: colonna_editabile, type: "number", defaultValue: 0, validation: { required: true } },
        UFC_Ha_Irrigua: { editable: colonna_editabile, type: "number", defaultValue: 0, validation: { required: true } },
        Validita_Inizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1"), validation: { required: true } },
        Validita_Fine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") },
        Inviato: { type: "number", defaultValue: 0 },
        DataInvio: { type: "date" },
        Data_Creazione: { type: "date", defaultValue: new Date("1900/1/1") },
        Data_Modifica: { type: "date", defaultValue: new Date("1900/1/1") },
        Username_Creazione: { type: "date", defaultValue: "" },
        Username_Modifica: { type: "date", defaultValue: "" },
    };
}

function UF_CaricaColonneKendoGrid() {
    return [
        {
            field: "Occupazione_Cod",
            title: TraduciLavorazioni("Occupazione_Cod", "Codice occupazione del suolo"),
            filterable: { multi: true, search: true },
            editor: OccupazioneCOD_DropDownEditor,
            width: 300
        },
        {
            field: "Occupazione_Des",
            title: TraduciLavorazioni("Occupazione_Des", "Occupazione del suolo"),
            filterable: { multi: true, search: true },
            editor: Occupazione_DropDownEditor,
            width: 300
        },        
        {
            field: "Destinazione_Cod",
            title: TraduciLavorazioni("Destinazione_Cod", "Codice Destinazione"),
            filterable: { multi: true, search: true },
            editor: DestinazioneCOD_DropDownEditor,
            width: 225
        },
        {
            field: "Destinazione_Des",
            title: TraduciLavorazioni("Destinazione_Des", "Destinazione"),
            filterable: { multi: true, search: true },
            editor: Destinazione_DropDownEditor,
            width: 225
        },        
        {
            field: "Uso_Cod",
            title: TraduciLavorazioni("Uso_Cod", "Codice Uso"),
            filterable: { multi: true, search: true },
            editor: UsoCOD_DropDownEditor,
            width: 225
        },
        {
            field: "Uso_Des",
            title: TraduciLavorazioni("Uso_Des", "Uso"),
            filterable: { multi: true, search: true },
            editor: Uso_DropDownEditor,
            width: 225
        },
        {
            field: "Qualita_Cod",
            title: TraduciLavorazioni("Qualita_Cod", "Codice Qualita"),
            filterable: { multi: true, search: true },
            editor: QualitaCOD_DropDownEditor,
            width: 225
        },
        {
            field: "Qualita_Des",
            title: TraduciLavorazioni("Qualita_Des", "Qualita"),
            filterable: { multi: true, search: true },
            editor: Qualita_DropDownEditor,
            width: 225
        },
        {
            field: "Validita_Inizio",
            title: TraduciLavorazioni("Validita_Inizio", "Validita Inizio"),
            filterable: { multi: true, search: true },
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ))  #',
            width: 225,
            hidden: true
        },
        {
            field: "Validita_Fine",
            title: TraduciLavorazioni("Validita_Fine", "Validita Fine"),
            filterable: { multi: true, search: true },
            template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ))  #',
            width: 200,
            hidden: true
        },
        {
            field: "UF_Ha",
            title: TraduciLavorazioni("U.F.", "Unita Foraggere"),
            filterable: { multi: true, search: true },
            width: 200
        },
        {
            field: "UFL_Ha",
            title: TraduciLavorazioni("U.F.L.", "Unita Foraggere Latte"),
            filterable: { multi: true, search: true },
            width: 200
        },
        {
            field: "UFC_Ha",
            title: TraduciLavorazioni("U.F.C.", "Unita Foraggere Carne"),
            filterable: { multi: true, search: true },
            width: 200
        },
        {
            field: "UF_Ha_Irrigua",
            title: TraduciLavorazioni("U.F.I.", "Unita Foraggere Irrigua"),
            filterable: { multi: true, search: true },
            width: 200
        },
        {
            field: "UFL_Ha_Irrigua",
            title: TraduciLavorazioni("U.F.L.I.", "Unita Foraggere Latte Irrigua"),
            filterable: { multi: true, search: true },
            width: 200
        },
        {
            field: "UFC_Ha_Irrigua",
            title: TraduciLavorazioni("U.F.C.I.", "Unita Foraggere Carne Irrigua"),
            filterable: { multi: true, search: true },
            width: 200
        },        
    ];
}

function ElencoMacrousi_CaricaCampiKendoModel(colonna_editabile) {
    return {
        //Chiave: { editable: true, type: "number" },
        Regione_Cod: { editable: false, type: "string", defaultValue: codiceRegione, validation: { required: true } },
        Macrouso_UMA_Cod: { editable: colonna_editabile, type: "string", defaultValue: 0, validation: { required: true } },
        Macrouso_UMA_Des: { editable: colonna_editabile, type: "string", defaultValue: "", validation: { required: true } },
        Inviato: { type: "number", defaultValue: 0 },
        DataInvio: { type: "date" },
        Data_Creazione: { type: "date", defaultValue: new Date("1900/1/1") },
        Data_Modifica: { type: "date", defaultValue: new Date("1900/1/1") },
        Username_Creazione: { type: "date", defaultValue: "" },
        Username_Modifica: { type: "date", defaultValue: "" },
        Validita_Inizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1"), validation: { required: true } },
        Validita_Fine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") },
    };
}

function ElencoMacrousi_CaricaColonneKendoGrid() {
    return [
        {
            field: "Macrouso_UMA_Cod",
            title: TraduciLavorazioni("Macrouso_UMA_Cod", "Codice macrouso UMA"),
            filterable: { multi: true, search: true },
            width: 175
        },
        {
            field: "Macrouso_UMA_Des",
            title: TraduciLavorazioni("Macrouso_UMA_Des", "Macrouso UMA descrizione"),
            filterable: { multi: true, search: true },
            width: 625
        },
        {
            field: "Validita_Inizio",
            title: TraduciLavorazioni("Validita_Inizio", "Validita Inizio"),
            filterable: { multi: true, search: true },
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ))  #',
            width: 225,
            hidden: true
        },
        {
            field: "Validita_Fine",
            title: TraduciLavorazioni("Validita_Fine", "Validita Fine"),
            filterable: { multi: true, search: true },
            template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ))  #',
            editor: function (container, options) { //impostare la data minima selezionabile a oggi oppure alla data inserita
                let rowHtml = $(container).parents("tr")[0];
                let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;
                let grid = $("#" + ID_Grid).data("kendoGrid");
                let row = grid.dataItem(rowHtml);
                var minDate = row.ValiditaInizio;

                $('<input data-bind="value:' + options.field + '" />')
                    .appendTo(container)
                    .kendoDatePicker({
                        min: minDate,
                        format: "dd/MM/yyyy",
                        parseFormats: ["dd/MM/yyyy"]
                    });
            },
            width: 225,
            hidden: true
        },
    ];
}

function ElencoLavorazioni_CaricaCampiKendoModel(colonna_editabile) {
    return {
        //Chiave: { editable: true, type: "number" },
        Regione_Cod: { editable: false, type: "string", defaultValue: codiceRegione, validation: { required: true } },
        Lav_UMA_Cod: { editable: colonna_editabile, type: "string", defaultValue: 0, validation: { required: true } },
        Lav_UMA_Des: { editable: colonna_editabile, type: "string", defaultValue: "", validation: { required: true } },
        Inviato: { type: "number", defaultValue: 0 },
        DataInvio: { type: "date" },
        Data_Creazione: { type: "date", defaultValue: new Date("1900/1/1") },
        Data_Modifica: { type: "date", defaultValue: new Date("1900/1/1") },
        Username_Creazione: { type: "date", defaultValue: "" },
        Username_Modifica: { type: "date", defaultValue: "" },
        Validita_Inizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1"), validation: { required: true } },
        Validita_Fine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") },
        Maggiorazione_Terreno_MedioTenace: { editable: colonna_editabile, type: "boolean", defaultValue: false },
        GestioneTerzista: { editable: colonna_editabile, type: "boolean", defaultValue: false },
        Utilizzata_Da_Consorzio_Bonifica: { editable: colonna_editabile, type: "boolean", defaultValue: false },
    };
}

function ElencoLavorazioni_CaricaColonneKendoGrid() {
    return [
        {
            field: "Lav_UMA_Cod",
            title: TraduciLavorazioni("Lav_UMA_Cod", "Codice lavorazione UMA"),
            filterable: { multi: true, search: true },
            width: 175
        },
        {
            field: "Lav_UMA_Des",
            title: TraduciLavorazioni("Lav_UMA_Des", "Lavorazione UMA descrizione"),
            filterable: { multi: true, search: true },
            width: 400
        },
        {
            field: "Maggiorazione_Terreno_MedioTenace",
            title: TraduciLavorazioni("Maggiorazione_Terreno_MedioTenace", "Maggiorazione terreno mediotenace"),
            filterable: { multi: true, search: true },
            template: creaKendoGridCheckColumn("Maggiorazione_Terreno_MedioTenace"),
            width: 150
        },
        {
            field: "GestioneTerzista",
            title: TraduciLavorazioni("GestioneTerzista", "Gestione terzista"),
            filterable: { multi: true, search: true },
            template: creaKendoGridCheckColumn("GestioneTerzista"),
            width: 150
        },
        {
            field: "Validita_Inizio",
            title: TraduciLavorazioni("Validita_Inizio", "Validita Inizio"),
            filterable: { multi: true, search: true },
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ))  #',
            width: 225,
            hidden: true
        },
        {
            field: "Validita_Fine",
            title: TraduciLavorazioni("Validita_Fine", "Validita Fine"),
            filterable: { multi: true, search: true },
            template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ))  #',
            editor: function (container, options) { //impostare la data minima selezionabile a oggi oppure alla data inserita
                let rowHtml = $(container).parents("tr")[0];
                let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;
                let grid = $("#" + ID_Grid).data("kendoGrid");
                let row = grid.dataItem(rowHtml);
                var minDate = row.ValiditaInizio;

                $('<input data-bind="value:' + options.field + '" />')
                    .appendTo(container)
                    .kendoDatePicker({
                        min: minDate, 
                        format: "dd/MM/yyyy", 
                        parseFormats: ["dd/MM/yyyy"] 
                    });
            },
            width: 225,
            hidden: true
        },
    ];
}

function ElencoAllevamenti_CaricaCampiKendoModel(colonna_editabile) {
    return {
        //Chiave: { editable: true, type: "number" },
        Regione_Cod: { editable: false, type: "string", defaultValue: codiceRegione, validation: { required: true } },
        UMA_All_Cod: { editable: colonna_editabile, type: "string", defaultValue: "000", validation: { required: true } },
        UMA_All_Des: { editable: colonna_editabile, type: "string", defaultValue: "", validation: { required: true } },
        Inviato: { type: "number", defaultValue: 0 },
        DataInvio: { type: "date" },
        Data_Creazione: { type: "date", defaultValue: new Date("1900/1/1") },
        Data_Modifica: { type: "date", defaultValue: new Date("1900/1/1") },
        Username_Creazione: { type: "date", defaultValue: "" },
        Username_Modifica: { type: "date", defaultValue: "" },
        Validita_Inizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1"), validation: { required: true } },
        Validita_Fine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") },
        UMA_AllGru_Cod: { editable: colonna_editabile, type: "string", defaultValue: 0 },
        UMA_AllGru_Des: { editable: colonna_editabile, type: "string", defaultValue: "Non Selezionato" },
    };
}

function ElencoAllevamenti_CaricaColonneKendoGrid() {
    return [
        {
            field: "UMA_All_Cod",
            title: TraduciLavorazioni("UMA_All_Cod", "Codice allevamento UMA"),
            filterable: { multi: true, search: true },
            width: 175
        },
        {
            field: "UMA_All_Des",
            title: TraduciLavorazioni("UMA_All_Des", "Allevamento UMA descrizione"),
            filterable: { multi: true, search: true },
            width: 400
        },
        {
            field: "UMA_AllGru_Des",
            title: TraduciLavorazioni("UMA_AllGru_Des", "Gruppo allevamento UMA descrizione"),
            filterable: { multi: true, search: true },
            editor: Programmazione_GruppiAllevamentoUMA_DropDownEditor,
            width: 250
        },
        {
            field: "Validita_Inizio",
            title: TraduciLavorazioni("Validita_Inizio", "Validita Inizio"),
            filterable: { multi: true, search: true },
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ))  #',
            width: 225,
            hidden: true
        },
        {
            field: "Validita_Fine",
            title: TraduciLavorazioni("Validita_Fine", "Validita Fine"),
            filterable: { multi: true, search: true },
            template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ))  #',
            editor: function (container, options) { //impostare la data minima selezionabile a oggi oppure alla data inserita
                let rowHtml = $(container).parents("tr")[0];
                let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;
                let grid = $("#" + ID_Grid).data("kendoGrid");
                let row = grid.dataItem(rowHtml);
                var minDate = row.ValiditaInizio;

                $('<input data-bind="value:' + options.field + '" />')
                    .appendTo(container)
                    .kendoDatePicker({
                        min: minDate,
                        format: "dd/MM/yyyy",
                        parseFormats: ["dd/MM/yyyy"]
                    });
            },
            width: 225,
            hidden: true
        },
    ];
}

function ElencoAssociazioniMacrousi_CaricaCampiKendoModel(colonna_editabile) {
    return {
        Chiave: { editable: true, type: "string" },
        Cul_Cod_Agea: { editable: false, type: "string", validation: { required: true } },
        Cul_Des_Agea: { editable: false, type: "string", validation: { required: true } },
        Uso_Cod: { editable: false, type: "string", validation: { required: true } },
        Uso_Des: { editable: false, type: "string", validation: { required: true } },
        Macrouso_Cod: { editable: false, type: "string", validation: { required: true } },
        Macrouso_Des: { editable: false, type: "string", validation: { required: true } },
        Occupazione_Cod: { editable: false, type: "string", validation: { required: true } },
        Occupazione_Des: { editable: false, type: "string", validation: { required: true } },
        Destinazione_Cod: { editable: false, type: "string", validation: { required: true } },
        Destinazione_Des: { editable: false, type: "string", validation: { required: true } },
        Qualita_Cod: { editable: false, type: "string", validation: { required: true } },
        Qualita_Des: { editable: false, type: "string", validation: { required: true } },
        Inviato: { type: "number", defaultValue: 0 },
        DataInvio: { type: "date" },
        Data_Creazione: { type: "date", defaultValue: new Date("1900/1/1") },
        Data_Modifica: { type: "date", defaultValue: new Date("1900/1/1") },
        Username_Creazione: { type: "date", defaultValue: "" },
        Username_Modifica: { type: "date", defaultValue: "" },
        Validita_Inizio: { editable: colonna_editabile, type: "date", defaultValue: new Date("1900/1/1"), validation: { required: true } },
        Validita_Fine: { editable: colonna_editabile, type: "date", defaultValue: new Date("2100/12/31") },
        Macrouso_UMA_Cod: { editable: true, type: "string", defaultValue: 0 },
        Macrouso_UMA_Des: { editable: colonna_editabile, type: "string", defaultValue: "Non Selezionato", validation: { required: true } },
    };
}

function ElencoAssociazioniMacrousi_CaricaColonneKendoGrid() {
    return [
        {
            field: "Cul_Cod_Agea",
            title: TraduciLavorazioni("Cul_Cod_Agea", "Codice Varietà"),
            filterable: { multi: true, search: true },
            width: 100
        },
        {
            field: "Cul_Des_Agea",
            title: TraduciLavorazioni("Cul_Des_Agea", "Varietà"),
            filterable: { multi: true, search: true },
            width: 250
        },
        {
            field: "Uso_Cod",
            title: TraduciLavorazioni("Uso_Cod", "Codice Uso"),
            filterable: { multi: true, search: true },
            width: 100
        },
        {
            field: "Uso_Des",
            title: TraduciLavorazioni("Uso_Des", "Uso"),
            filterable: { multi: true, search: true },
            width: 250
        },
        {
            field: "Macrouso_Cod",
            title: TraduciLavorazioni("Macrouso_Cod", "Codice Macrouso"),
            filterable: { multi: true, search: true },
            width: 100,
            hidden: true
        },
        {
            field: "Macrouso_Des",
            title: TraduciLavorazioni("Macrouso_Des", "Macrouso"),
            filterable: { multi: true, search: true },
            width: 250,
            hidden: true
        },
        {
            field: "Occupazione_Cod",
            title: TraduciLavorazioni("Occupazione_Cod", "Codice Occupazione del suolo"),
            filterable: { multi: true, search: true },
            width: 100
        },
        {
            field: "Occupazione_Des",
            title: TraduciLavorazioni("Occupazione_Des", "Occupazione del suolo"),
            filterable: { multi: true, search: true },
            width: 250
        },
        {
            field: "Destinazione_Cod",
            title: TraduciLavorazioni("Destinazione_Cod", "Codice Destinazione"),
            filterable: { multi: true, search: true },
            width: 100
        },
        {
            field: "Destinazione_Des",
            title: TraduciLavorazioni("Destinazione_Des", "Destinazione"),
            filterable: { multi: true, search: true },
            width: 250
        },
        {
            field: "Qualita_Cod",
            title: TraduciLavorazioni("Qualita_Cod", "Codice Qualità"),
            filterable: { multi: true, search: true },
            width: 100
        },
        {
            field: "Qualita_Des",
            title: TraduciLavorazioni("Qualita_Des", "Qualità"),
            filterable: { multi: true, search: true },
            width: 250
        },
        {
            field: "Macrouso_UMA_Des",
            title: TraduciLavorazioni("Macrouso_UMA_Des", "Macrouso UMA"),
            filterable: { multi: true, search: true },
            editor: Programmazione_MacrousiUMA_DropDownEditor,
            width: 250
        },
        {
            field: "Validita_Inizio",
            title: TraduciLavorazioni("Validita_Inizio", "Validita Inizio"),
            filterable: { multi: true, search: true },
            template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ))  #',
            width: 225,
            hidden: true
        },
        {
            field: "Validita_Fine",
            title: TraduciLavorazioni("Validita_Fine", "Validita Fine"),
            filterable: { multi: true, search: true },
            template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ))  #',
            width: 225,
            hidden: true
        },
    ];
}

function VincolaRendRich_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Vincola_Rendicontazione_e_Richiesta", "Altre_Cfg", [
        { Altre_Cfg: 0, Vincola_Rendicontazione_e_Richiesta: "Disabilitato" },
        { Altre_Cfg: 1, Vincola_Rendicontazione_e_Richiesta: "Abilitato" },
    ], changeVincolaRendRich);
}

function changeVincolaRendRich(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Vincola_Rendicontazione_e_Richiesta = dataItem.Vincola_Rendicontazione_e_Richiesta;
    model.Altre_Cfg = dataItem.Altre_Cfg.toString();

}

function SiNo_Gestione_Biologico_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Gestione_BiologicoDes", "Gestione_Biologico", [
        { Gestione_Biologico: 0, Gestione_BiologicoDes: "Disabilitato" },
        { Gestione_Biologico: 1, Gestione_BiologicoDes: "Abilitato" },
    ], changeGestione_Biologico);
}

function changeGestione_Biologico(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    model.Gestione_BiologicoDes = dataItem.Gestione_BiologicoDes;
    model.Gestione_Biologico = dataItem.Gestione_Biologico;

}

function SiNo_Gestione_Rimanenze_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Gestione_RimanenzeDes", "Gestione_Rimanenze", [
        { Gestione_Rimanenze: 0, Gestione_RimanenzeDes: "Disabilitato" },
        { Gestione_Rimanenze: 1, Gestione_RimanenzeDes: "Abilitato" },
        { Gestione_Rimanenze: 2, Gestione_RimanenzeDes: "Abilitato Solo Rendicontazioni" },
    ], changeGestione_Rimanenze);
}

function changeGestione_Rimanenze(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.Gestione_RimanenzeDes = dataItem.Gestione_RimanenzeDes;
    model.Gestione_Rimanenze = dataItem.Gestione_Rimanenze;

}

function SiNo_Gestione_Anticipazioni_Colturali_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Gestione_Anticipazioni_ColturaliDes", "Gestione_Anticipazioni_Colturali", [
        { Gestione_Anticipazioni_Colturali: 0, Gestione_Anticipazioni_ColturaliDes: "Tutte" },
        { Gestione_Anticipazioni_Colturali: 1, Gestione_Anticipazioni_ColturaliDes: "Azienda Agricola Privata" },
        { Gestione_Anticipazioni_Colturali: 2, Gestione_Anticipazioni_ColturaliDes: "Azienda Terzista" },
        { Gestione_Anticipazioni_Colturali: 3, Gestione_Anticipazioni_ColturaliDes: "Cooperativa Agricola" },
        { Gestione_Anticipazioni_Colturali: 4, Gestione_Anticipazioni_ColturaliDes: "Azienda Agricola Pubblica" },
        { Gestione_Anticipazioni_Colturali: 5, Gestione_Anticipazioni_ColturaliDes: "Consorzio di Bonifica e Irrigazione" },
        //{ Gestione_Anticipazioni_Colturali: 23, Gestione_Anticipazioni_ColturaliDes: "Azienda Terzista / Cooperativa Agricola",          }

    ], changeGestione_Anticipazioni_Colturali);
}

function changeGestione_Anticipazioni_Colturali(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    console.log(dataItem);
    console.log(model);

    model.Gestione_Anticipazioni_ColturaliDes = dataItem.Gestione_Anticipazioni_ColturaliDes;
    model.Gestione_Anticipazioni_Colturali = dataItem.Gestione_Anticipazioni_Colturali;

}

function Regolamento_Cod_DropDownEditor(container, options) {
    creaDropDownEditor(container, "Regolamento_CodDes", "Regolamento_Cod", [
        { Regolamento_Cod: 0, Regolamento_CodDes: "Entrambi" },
        { Regolamento_Cod: 1, Regolamento_CodDes: "Convenzionale" },
        { Regolamento_Cod: 4, Regolamento_CodDes: "Biologico" },
    ], changeGestione_Regolamento_Cod);
}

function changeGestione_Regolamento_Cod(e) {

    var dataItem = e.sender.dataItem();
    var gridID = e.sender.element.parents("div[data-role='grid']")[0].id;
    var grid = $("#" + gridID).data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    model.Regolamento_CodDes = dataItem.Regolamento_CodDes;
    model.Regolamento_Cod = dataItem.Regolamento_Cod;

}

function Setup_TipologiaReportELAS_DropDownEditor(container, options) {
    let elenco_tipologie = PopolaElenco_TipologiaReport(true, false);
    creaDropDownEditor(container, "Tipologia_Report_ElasDes", "Tipologia_Report_Elas", elenco_tipologie, change_tipologia);
}

function change_tipologia(e) {
    let dataItem = e.sender.dataItem();
    let grid = $("#grdSetup").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Tipologia_Report_Elas = dataItem.Tipologia_Report_Elas;
        model.Tipologia_Report_ElasDes = dataItem.Tipologia_Report_ElasDes;
        model.dirty = true;
    }

    grid.refresh();
}

function Setup_TipologiaElenco_Inadempienti_DropDownEditor(container, options) {
    let elenco_tipologie = PopolaElenco_TipologiaReport(false, false);
    creaDropDownEditor(container, "Tipologia_Elenco_InadempientiDes", "Tipologia_Elenco_Inadempienti", elenco_tipologie, change_tipologia_inadempienti);
}

function change_tipologia_inadempienti(e) {
    let dataItem = e.sender.dataItem();
    let grid = $("#grdSetup").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Tipologia_Elenco_Inadempienti = dataItem.Tipologia_Elenco_Inadempienti;
        model.Tipologia_Elenco_InadempientiDes = dataItem.Tipologia_Elenco_InadempientiDes;
        model.dirty = true;
    }

    grid.refresh();
}

function Setup_TipologiaReport_SegnalazioneAccise_DropDownEditor(container, options) {
    let elenco_tipologie = PopolaElenco_TipologiaReport(false, true);
    creaDropDownEditor(container, "Tipologia_Report_SegnalazioneAcciseDes", "Tipologia_Report_SegnalazioneAccise", elenco_tipologie, change_segnalazione_accise);
}

function change_segnalazione_accise(e) {
    let dataItem = e.sender.dataItem();
    let grid = $("#grdSetup").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Tipologia_Report_SegnalazioneAccise = dataItem.Tipologia_Report_SegnalazioneAccise;
        model.Tipologia_Report_SegnalazioneAcciseDes = dataItem.Tipologia_Report_SegnalazioneAcciseDes;
        model.dirty = true;
    }

    grid.refresh();
}

async function Macchine_Targa_Obbligatoria_MultiselectEditor(container, options) {
    //getElencoTipoMacchine().then( elencoMacchine =>
    //    creaKendoMultiselectEditor(container, "CLASS_DESC", "CLASS_CODE", elencoMacchine, Macchine_Targa_Obbligatoria_Change)
    //)

    let rowHtml = $(container).parents("tr")[0];
    let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

    let grid = $("#" + ID_Grid).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    $('<input name="Macchine_Targa_Obbligatoria"/>')
        .appendTo(container)
        .kendoMultiSelect({
            autoBind: true,
            dataTextField: "Macchine_Targa_ObbligatoriaDes",
            dataValueField: "Macchine_Targa_Obbligatoria",
            dataSource: elencoMacchine,
            filter: "contains",
            open: function (e) {
                var listContainer = e.sender.list.closest(".k-list-container");
                listContainer.width(listContainer.width() + kendo.support.scrollbar());
            },
            change: Macchine_Targa_Obbligatoria_Change
        }).data("kendoMultiSelect");

    var ddl = $('input[name$="Macchine_Targa_Obbligatoria"]').data("kendoMultiSelect");
    ddl.list.width("auto");
}

function Macchine_Targa_Obbligatoria_Change(e) {
    var grid = $("#grdSetup").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    var Tipi_Scelti = $('input[name$="Macchine_Targa_Obbligatoria"]').data("kendoMultiSelect").dataItems();
    model.Macchine_Targa_Obbligatoria = [];
    model.Macchine_Targa_ObbligatoriaDes = [];
    model.Macchine_Targa_ObbligatoriaString = "";
    model.Macchine_Targa_ObbligatoriaDesString = "";

    if (Tipi_Scelti.length > 0) {

        for (var x = 0; x < Tipi_Scelti.length; x++) {
            model.Macchine_Targa_Obbligatoria.push(Tipi_Scelti[x].Macchine_Targa_Obbligatoria);
            model.Macchine_Targa_ObbligatoriaDes.push(Tipi_Scelti[x].Macchine_Targa_ObbligatoriaDes);
            model.Macchine_Targa_ObbligatoriaString += Tipi_Scelti[x].Macchine_Targa_Obbligatoria;
            //--------------------------------------------------------------------------------
            // Se seleziono una classe di livello 1 o 2, includo anche tutti i rispettivi 
            // sottolivelli
            //--------------------------------------------------------------------------------
            //if (Tipi_Scelti[x].Macchine_Targa_Obbligatoria.length < 8) {
            //    model.Macchine_Targa_ObbligatoriaString += "%";
            //}
            //--------------------------------------------------------------------------------
            model.Macchine_Targa_ObbligatoriaString += "|";
            model.Macchine_Targa_ObbligatoriaDesString += Tipi_Scelti[x].Macchine_Targa_ObbligatoriaDes + ",";
        }

        //Rimuovo l'ultima virgola
        if (model.Macchine_Targa_ObbligatoriaDesString !== "") {
            model.Macchine_Targa_ObbligatoriaDesString = model.Macchine_Targa_ObbligatoriaDesString.slice("0", model.Macchine_Targa_ObbligatoriaDesString.length - 1);
            model.Macchine_Targa_ObbligatoriaString = model.Macchine_Targa_ObbligatoriaString.slice("0", model.Macchine_Targa_ObbligatoriaString.length - 1);
        }
    }
}

async function Stati_Invio_Mail_MultiselectEditor(container, options) {
    //getElencoTipoMacchine().then( elencoMacchine =>
    //    creaKendoMultiselectEditor(container, "CLASS_DESC", "CLASS_CODE", elencoMacchine, Stati_Invio_Mail_Change)
    //)

    let rowHtml = $(container).parents("tr")[0];
    let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

    let grid = $("#" + ID_Grid).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    $('<input name="Stati_Invio_Mail"/>')
        .appendTo(container)
        .kendoMultiSelect({
            autoBind: true,
            dataTextField: "Stati_Invio_MailDes",
            dataValueField: "Stati_Invio_Mail",
            dataSource: elencoStati,
            filter: "contains",
            open: function (e) {
                var listContainer = e.sender.list.closest(".k-list-container");
                listContainer.width(listContainer.width() + kendo.support.scrollbar());
            },
            change: Stati_Invio_Mail_Change
        }).data("kendoMultiSelect");

    var ddl = $('input[name$="Stati_Invio_Mail"]').data("kendoMultiSelect");
    ddl.list.width("auto");
}

function Stati_Invio_Mail_Change(e) {
    var grid = $("#grdSetup").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    var Tipi_Scelti = $('input[name$="Stati_Invio_Mail"]').data("kendoMultiSelect").dataItems();
    model.Stati_Invio_Mail = [];
    model.Stati_Invio_MailDes = [];
    model.Stati_Invio_MailString = "";
    model.Stati_Invio_MailDesString = "";

    if (Tipi_Scelti.length > 0) {

        for (var x = 0; x < Tipi_Scelti.length; x++) {
            model.Stati_Invio_Mail.push(Tipi_Scelti[x].Stati_Invio_Mail);
            model.Stati_Invio_MailDes.push(Tipi_Scelti[x].Stati_Invio_MailDes);
            model.Stati_Invio_MailString += Tipi_Scelti[x].Stati_Invio_Mail;
            //--------------------------------------------------------------------------------
            // Se seleziono una classe di livello 1 o 2, includo anche tutti i rispettivi 
            // sottolivelli
            //--------------------------------------------------------------------------------
            //if (Tipi_Scelti[x].Stati_Invio_Mail.length < 8) {
            //    model.Stati_Invio_MailString += "%";
            //}
            //--------------------------------------------------------------------------------
            model.Stati_Invio_MailString += "|";
            model.Stati_Invio_MailDesString += Tipi_Scelti[x].Stati_Invio_MailDes + ",";
        }

        //Rimuovo l'ultima virgola
        if (model.Stati_Invio_MailDesString !== "") {
            model.Stati_Invio_MailDesString = model.Stati_Invio_MailDesString.slice("0", model.Stati_Invio_MailDesString.length - 1);
            model.Stati_Invio_MailString = model.Stati_Invio_MailString.slice("0", model.Stati_Invio_MailString.length - 1);
        }
    }
}

async function Gruppi_Utenti_Invio_Mail_MultiselectEditor(container, options) {
    //getElencoTipoMacchine().then( elencoMacchine =>
    //    creaKendoMultiselectEditor(container, "CLASS_DESC", "CLASS_CODE", elencoMacchine, Gruppi_Utenti_Invio_Mail_Change)
    //)

    let rowHtml = $(container).parents("tr")[0];
    let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

    let grid = $("#" + ID_Grid).data("kendoGrid");
    let row = grid.dataItem(rowHtml);

    $('<input name="Gruppi_Utenti_Invio_Mail"/>')
        .appendTo(container)
        .kendoMultiSelect({
            autoBind: true,
            dataTextField: "Gruppi_Utenti_Invio_MailDes",
            dataValueField: "Gruppi_Utenti_Invio_Mail",
            dataSource: elencoGruppi,
            filter: "contains",
            open: function (e) {
                var listContainer = e.sender.list.closest(".k-list-container");
                listContainer.width(listContainer.width() + kendo.support.scrollbar());
            },
            change: Gruppi_Utenti_Invio_Mail_Change
        }).data("kendoMultiSelect");

    var ddl = $('input[name$="Gruppi_Utenti_Invio_Mail"]').data("kendoMultiSelect");
    ddl.list.width("auto");
}

function Gruppi_Utenti_Invio_Mail_Change(e) {
    var grid = $("#grdSetup").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    var Tipi_Scelti = $('input[name$="Gruppi_Utenti_Invio_Mail"]').data("kendoMultiSelect").dataItems();
    model.Gruppi_Utenti_Invio_Mail = [];
    model.Gruppi_Utenti_Invio_MailDes = [];
    model.Gruppi_Utenti_Invio_MailString = "";
    model.Gruppi_Utenti_Invio_MailDesString = "";

    if (Tipi_Scelti.length > 0) {

        for (var x = 0; x < Tipi_Scelti.length; x++) {
            model.Gruppi_Utenti_Invio_Mail.push(Tipi_Scelti[x].Gruppi_Utenti_Invio_Mail);
            model.Gruppi_Utenti_Invio_MailDes.push(Tipi_Scelti[x].Gruppi_Utenti_Invio_MailDes);
            model.Gruppi_Utenti_Invio_MailString += Tipi_Scelti[x].Gruppi_Utenti_Invio_Mail;
            //--------------------------------------------------------------------------------
            // Se seleziono una classe di livello 1 o 2, includo anche tutti i rispettivi 
            // sottolivelli
            //--------------------------------------------------------------------------------
            //if (Tipi_Scelti[x].Gruppi_Utenti_Invio_Mail.length < 8) {
            //    model.Gruppi_Utenti_Invio_MailString += "%";
            //}
            //--------------------------------------------------------------------------------
            model.Gruppi_Utenti_Invio_MailString += "|";
            model.Gruppi_Utenti_Invio_MailDesString += Tipi_Scelti[x].Gruppi_Utenti_Invio_MailDes + ",";
        }

        //Rimuovo l'ultima virgola
        if (model.Gruppi_Utenti_Invio_MailDesString !== "") {
            model.Gruppi_Utenti_Invio_MailDesString = model.Gruppi_Utenti_Invio_MailDesString.slice("0", model.Gruppi_Utenti_Invio_MailDesString.length - 1);
            model.Gruppi_Utenti_Invio_MailString = model.Gruppi_Utenti_Invio_MailString.slice("0", model.Gruppi_Utenti_Invio_MailString.length - 1);
        }
    }
}

async function Occupazione_DropDownEditor(container, options) {

    let rowHtml = $(container).parents("tr")[0];

    if ($(container).parents("div[data-role='grid']")[0] === undefined) {
        return;
    } else {
        let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

        let grid = $("#" + ID_Grid).data("kendoGrid");
        let row = grid.dataItem(rowHtml);

        if (!row.isNew()) {
            return;
        }
    }

    getElencoAgea(1, false).then( elencoOccupazione => 
        creaDropDownEditor(container, "occupazione_Des", "occupazione_Cod", elencoOccupazione, change_occupazione));
}

async function OccupazioneCOD_DropDownEditor(container, options) {

    let rowHtml = $(container).parents("tr")[0];

    if ($(container).parents("div[data-role='grid']")[0] === undefined) {
        return;
    } else {
        let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

        let grid = $("#" + ID_Grid).data("kendoGrid");
        let row = grid.dataItem(rowHtml);

        if (!row.isNew()) {
            return;
        }
    }

    getElencoAgea(1, true).then(elencoOccupazione =>
        creaDropDownEditor(container, "occupazione_Cod", "occupazione_Cod", elencoOccupazione, change_occupazione));
}

function change_occupazione(e) {
    let dataItem = e.sender.dataItem();
    let grid = $("#grdConfigurazioneUF").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Occupazione_Cod = dataItem.occupazione_Cod;
        model.Occupazione_Des = dataItem.occupazione_Des;
        model.dirty = true;
    }

    grid.refresh();
}

async function Destinazione_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    if ($(container).parents("div[data-role='grid']")[0] === undefined) {
        return;
    } else {
        let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

        let grid = $("#" + ID_Grid).data("kendoGrid");
        let row = grid.dataItem(rowHtml);

        if (!row.isNew()) {
            return;
        }
    }

    getElencoAgea(2, false).then(elencoDestinazione =>
        creaDropDownEditor(container, "destinazione_Des", "destinazione_Cod", elencoDestinazione, change_Destinazione));
}

async function DestinazioneCOD_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    if ($(container).parents("div[data-role='grid']")[0] === undefined) {
        return;
    } else {
        let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

        let grid = $("#" + ID_Grid).data("kendoGrid");
        let row = grid.dataItem(rowHtml);

        if (!row.isNew()) {
            return;
        }
    }

    getElencoAgea(2, true).then(elencoDestinazione =>
        creaDropDownEditor(container, "destinazione_Cod", "destinazione_Cod", elencoDestinazione, change_Destinazione));
}

function change_Destinazione(e) {
    let dataItem = e.sender.dataItem();
    let grid = $("#grdConfigurazioneUF").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Destinazione_Cod = dataItem.destinazione_Cod;
        model.Destinazione_Des = dataItem.destinazione_Des;
        model.dirty = true;
    }

    grid.refresh();
}

async function Uso_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    if ($(container).parents("div[data-role='grid']")[0] === undefined) {
        return;
    } else {
        let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

        let grid = $("#" + ID_Grid).data("kendoGrid");
        let row = grid.dataItem(rowHtml);

        if (!row.isNew()) {
            return;
        }
    }

    getElencoAgea(3, false).then(elencoUso =>
        creaDropDownEditor(container, "uso_Des", "uso_Cod", elencoUso, change_Uso));
}

async function UsoCOD_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    if ($(container).parents("div[data-role='grid']")[0] === undefined) {
        return;
    } else {
        let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

        let grid = $("#" + ID_Grid).data("kendoGrid");
        let row = grid.dataItem(rowHtml);

        if (!row.isNew()) {
            return;
        }
    }

    getElencoAgea(3, true).then(elencoUso =>
        creaDropDownEditor(container, "uso_Cod", "uso_Cod", elencoUso, change_Uso));
}

function change_Uso(e) {
    let dataItem = e.sender.dataItem();
    let grid = $("#grdConfigurazioneUF").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Uso_Cod = dataItem.uso_Cod;
        model.Uso_Des = dataItem.uso_Des;
        model.dirty = true;
    }

    grid.refresh();
}

async function Qualita_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    if ($(container).parents("div[data-role='grid']")[0] === undefined) {
        return;
    } else {
        let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

        let grid = $("#" + ID_Grid).data("kendoGrid");
        let row = grid.dataItem(rowHtml);

        if (!row.isNew()) {
            return;
        }
    }

    getElencoAgea(4, false).then(elencoQualita =>
        creaDropDownEditor(container, "qualita_Des", "qualita_Cod", elencoQualita, change_Qualita));
}

async function QualitaCOD_DropDownEditor(container, options) {
    let rowHtml = $(container).parents("tr")[0];

    if ($(container).parents("div[data-role='grid']")[0] === undefined) {
        return;
    } else {
        let ID_Grid = $(container).parents("div[data-role='grid']")[0].id;

        let grid = $("#" + ID_Grid).data("kendoGrid");
        let row = grid.dataItem(rowHtml);

        if (!row.isNew()) {
            return;
        }
    }

    getElencoAgea(4, true).then(elencoQualita =>
        creaDropDownEditor(container, "qualita_Cod", "qualita_Cod", elencoQualita, change_Qualita));
}

function change_Qualita(e) {
    let dataItem = e.sender.dataItem();
    let grid = $("#grdConfigurazioneUF").data("kendoGrid");
    let elem = this.element.closest("tr");
    let model = grid.dataItem(elem);

    if (model != null) {
        model.Qualita_Cod = dataItem.qualita_Cod;
        model.Qualita_Des = dataItem.qualita_Des;
        model.dirty = true;
    }

    grid.refresh();
}

//function TipologieDocumenti_UMA_DropDownEditor(container, options) {
//    let rowHtml = $(container).parents("tr")[0];

//    var grid = $("#grdSetup").data("kendoGrid");
//    let row = grid.dataItem(rowHtml);

//    getTipologieDocumenti().then(
//        elenco => {
//            creaDropDownEditor(container, "Nome", "ID_Tipologia", elenco, TipologieDocumenti_UMA_Change);
//        }
//    )
//}

//function TipologieDocumenti_UMA_Change(e) {

//    let dataItem = e.sender.dataItem();
//    let grid = $("#grdSetup").data("kendoGrid");
//    let elem = this.element.closest("tr");
//    let model = grid.dataItem(elem);

//    if (model != null) {
//        model.LavUMA_Lav_UMA_Des = dataItem.LavUMA_Lav_UMA_Des;
//        model.Lavorazione_UMA = dataItem.Lavorazione_UMA;
//        model.dirty = true;
//    }

//    grid.refresh();
//}

function duplicaAnno(tr_elem, grid_elem) {
    var datiGriglia = $(grid_elem).data('kendoGrid');
    var datiRiga = datiGriglia.dataItem(tr_elem);
    var riga;

    datiGriglia.addRow();

    riga = datiGriglia._data[0];

    riga.Per_riduzione = datiRiga.Per_riduzione;
    riga.Per_Mag_Terreno_B = datiRiga.Per_Mag_Terreno_B;
    riga.Per_Mag_Terreno_Medio = datiRiga.Per_Mag_Terreno_Medio;
    riga.Per_Mag_Terreno_Tenace = datiRiga.Per_Mag_Terreno_Tenace;
    riga.Altre_Cfg = datiRiga.Altre_Cfg;
    riga.Nr_Litri_Maggiorazione = datiRiga.Nr_Litri_Maggiorazione;
    riga.Percentuale_Integrazione_Terzista = datiRiga.Percentuale_Integrazione_Terzista;
    riga.Vincola_Rendicontazione_e_Richiesta = datiRiga.Vincola_Rendicontazione_e_Richiesta;
    riga.Gestione_Biologico = datiRiga.Gestione_Biologico;
    riga.Gestione_BiologicoDes = datiRiga.Gestione_BiologicoDes;
    riga.Gestione_Rimanenze = datiRiga.Gestione_Rimanenze;
    riga.Gestione_RimanenzeDes = datiRiga.Gestione_RimanenzeDes;
    riga.Gestione_Anticipazioni_Colturali = datiRiga.Gestione_Anticipazioni_Colturali;
    riga.Gestione_Anticipazioni_ColturaliDes = datiRiga.Gestione_Anticipazioni_ColturaliDes;
    riga.Percentuale_Richieste_Anticipo = datiRiga.Percentuale_Richieste_Anticipo;
    riga.Macchine_Targa_Obbligatoria = datiRiga.Macchine_Targa_Obbligatoria;
    riga.Macchine_Targa_ObbligatoriaDes = datiRiga.Macchine_Targa_ObbligatoriaDes;
    riga.Macchine_Targa_ObbligatoriaString = datiRiga.Macchine_Targa_ObbligatoriaString;
    riga.Macchine_Targa_ObbligatoriaDesString = datiRiga.Macchine_Targa_ObbligatoriaDesString;

    riga.Tipologia_Report_Elas = datiRiga.Tipologia_Report_Elas
    riga.Tipologia_Report_ElasDes = datiRiga.Tipologia_Report_ElasDes

    riga.Tipologia_Elenco_Inadempienti = datiRiga.Tipologia_Elenco_Inadempienti
    riga.Tipologia_Elenco_InadempientiDes = datiRiga.Tipologia_Elenco_InadempientiDes

    riga.Tipologia_Report_SegnalazioneAccise = datiRiga.Tipologia_Report_SegnalazioneAccise
    riga.Tipologia_Report_SegnalazioneAcciseDes = datiRiga.Tipologia_Report_SegnalazioneAcciseDes

    riga.Stati_Invio_Mail = datiRiga.Stati_Invio_Mail
    riga.Stati_Invio_MailDes = datiRiga.Stati_Invio_MailDes
    riga.Stati_Invio_MailString = datiRiga.Stati_Invio_MailString
    riga.Stati_Invio_MailDesString = datiRiga.Stati_Invio_MailDesString

    riga.Gruppi_Utenti_Invio_Mail = datiRiga.Gruppi_Utenti_Invio_Mail
    riga.Gruppi_Utenti_Invio_MailDes = datiRiga.Gruppi_Utenti_Invio_MailDes
    riga.Gruppi_Utenti_Invio_MailString = datiRiga.Gruppi_Utenti_Invio_MailString
    riga.Gruppi_Utenti_Invio_MailDesString = datiRiga.Gruppi_Utenti_Invio_MailDesString

    datiGriglia.refresh();

}

function creaKendoGridCheckColumn(fieldName) {
    var chk = '#=dirtyField(data,"' + fieldName + '")';
    chk += '#<input type="checkbox" #= ' + fieldName + ' ? \'checked="checked"\' : "" # class="chkbx k-checkbox k-checkbox-md k-rounded-md" />';
    return chk;
}

function dirtyField(data, fieldName) {
    var hasClass = $("[data-uid=" + data.uid + "]").find(".k-dirty-cell").length < 1;
    if (data.dirty && data.dirtyFields[fieldName] && hasClass) {
        return "<span class='k-dirty'></span>"
    }
    else {
        return "";
    }
}

function kEventoSelezionaRiga_AssociazioniMacrousiUMA(e) {
    var checked = this.checked,
        row = $(this).parents("tr"),
        grid = $("#grdAssociazioniMacrousiUMA").data("kendoGrid"),
        dataItem = grid.dataItem(row);

    dataItem.Selected = checked;
    let selected = grid.dataSource.data().filter((el) => { return el.Selected == true });

    if (selected.length <= 0) {
        $("#btnModificaMassiva")[0].setAttribute("disabled", "");
    }
    else {
        $("#btnModificaMassiva")[0].removeAttribute("disabled", "");
    }

    rowKendoGridSelected(row, checked);   
}