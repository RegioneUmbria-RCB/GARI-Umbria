//-------------------------------------------------------------
//--------------------------VARIABILI--------------------------
//-------------------------------------------------------------

var Cmb_Parametri;
var Cmb_modifica_Razza_Madre;
var obj_ModificaMultipla;
var selected_add;
var selected_rows;
var response;
var parametri_multipli = false;
var editableFields = ["RAZ_DES", "MAT_MADRE", "RazDes_Madre", "MAT_PADRE", "RazDes_Padre", "Sesso", "Validato", "Metodo_Produzione", "CF_DETENTORE", "CF_PROPRIETARIO", "Certificato", "Modello4_Ingresso", "Modello4_Ingresso_Numero", "Modello4_Uscita", "Modello4_Uscita_Numero", "Modello4_Ingresso_Prenotazione", "Modello4_Uscita_Prenotazione", "Data_Documento_Ingresso", "Data_Documento_Uscita", "Lotto_Fornitore", "Progetto_Des", "CF_FornFatt", "CF_FornProv", "RagSoc_FornFatt", "RagSoc_FornProv", "Data_DDT_Ingresso", "Data_DDT_Uscita", "N_Bolla_Fornitore", "N_Bolla_Uscita", "Codice_Azienda_Fornitore", "AUSL_AZI_NASCITA", "CF_StallaSvezz", "RagSoc_StallaSvezz", "Incremento_Teorico"];
var campiTextboxEditabili = ["MAT_MADRE", "MAT_PADRE", "CF_DETENTORE", "CF_PROPRIETARIO", "Certificato", "Modello4_Ingresso", "Modello4_Uscita", "Modello4_Ingresso_Numero", "Modello4_Uscita_Numero", "N_Bolla_Fornitore", "N_Bolla_Uscita", "Codice_Azienda_Fornitore", "AUSL_AZI_NASCITA", "Incremento_Teorico"];
var updatedRecords;
var obj_ModificaMultipla = {};
var arrayTipoAnagraficaModificati = [];
var stessa_specie_cod = [];
var warningCounter = false;
var hiddenCols = [];

//-------------------------------------------------------------
//------------------------GRIGLIA-KENDO------------------------
//-------------------------------------------------------------

function popolaGriglia_ModificaMultipla(IDControllo) {

    var idModel = "chiave";
    var funzioniCRUD = {};
    var funzioniPrimaDopoEventi = {};

    var colonneKendoGrid = kReadValorizzazione_col_ModificaMultipla();
    var campiKendoModel = kReadValorizzazione_mod_ModificaMultipla();


    ischeckable = campiKendoModel.hasOwnProperty(idModel);

    var parametriDataSource = {};
    var parametriPerLettura = null;

    var parametriKendoGrid = {
        columnMenu: true,
        pdf: false,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        groupable: false,
        reorderable: true,
        scrollable: true,
        filterable: { mode: "menu" },
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 }
    };

    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;
    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    if (ischeckable === true) {
        funzioniCRUD = {
            funzioneRead: kReadValorizzazione_rows_ModificaMultipla,
            checkBoxFunction: KendoModificaMultipla_checked,
            funzioneSubmit: { funzione: SubmitGrid_ModificaMultiplaZoo, flagInsert: true, flagUpdate: true, flagDelete: true },
            UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
            omettiPulsantiSalva: false, omettiPulsantiAnnulla: false
        };
        funzioniPrimaDopoEventi = { funzioneDaChiamareDopoSelectAllRows: function () { }, funzioneDaChiamareDopoDataBound: function () { }, funzioneDaChiamareDopoSave: ValorizzaArrayModificati };
    } else {
        funzioniCRUD = {
            funzioneRead: kReadValorizzazione_rows_ModificaMultipla,
            checkBoxFunction: KendoModificaMultipla_checked,
            funzioneSubmit: { funzione: SubmitGrid_ModificaMultiplaZoo, flagInsert: true, flagUpdate: true, flagDelete: true },
            UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
            omettiPulsantiSalva: false, omettiPulsantiAnnulla: false
        };
        funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBounding_ModificaMultipla };
    }

    creaKendoGrid(IDControllo, // Rappresenta l'ID del div a cui si associa la griglia.
        funzioniCRUD, // Funzioni js da chiamare per read, insert, update, delete.
        idModel, // Chiave riga.
        campiKendoModel, // Campi modello.
        colonneKendoGrid, // Colonne da mostrare.
        parametriPerLettura, // Parametri da passare alla lettura.
        parametriDataSource, // Parametri data source { chiave - valore}.
        parametriKendoGrid, // Parametri griglia [{ chiave - valore}].
        funzioniPrimaDopoEventi, // Funzioni da chiamare all'inizio e alla fine dei vari eventi.
        mostraRigheCancellate, // Se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione.
        colonneDisabilitateSoloInModifica // Colonne non modificabili in modifica ["colA", "colB", ...].
    );
    KendoModifica_Multipla_Zoo = $("#divKendoModifica_Multipla_Zoo").data("kendoGrid");    
    //KendoModifica_Multipla_Zoo.bind("columnHide", gestioneHiddenCols);
    //KendoModifica_Multipla_Zoo.bind("columnShow", gestioneShowedCols);
    KendoModifica_Multipla_Zoo.content.scrollLeft(0);
    CreaToolBarZoo();
}

function ValorizzaArrayModificati(e)
{
    var campoModificato = Object.keys(e.values)[0];
    if (!arrayTipoAnagraficaModificati.includes(campoModificato))
    {
        arrayTipoAnagraficaModificati.push(campoModificato);
    }
}

function kReadValorizzazione_rows_ModificaMultipla(options) {
    WaitFrame.show();
    ws_leggiGiacenze().then((jSonParsed_Kendo) => {
        if (ischeckable) {
            for (let i = 0; i < jSonParsed_Kendo.kendo_rows.length; i++) {
                jSonParsed_Kendo.kendo_rows[i].Selected = false;
            }
        }
        WaitFrame.hide();
        options.success(jSonParsed_Kendo.kendo_rows);
    })
}

function kReadValorizzazione_col_ModificaMultipla() {
    var kendo_columns = [
        {
            "field": "partitaIvaReale",
            "title": TraduzioneMultiResx(resxObj, "PartitaIVA", "Partita IVA"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Matricola",
            "title": TraduzioneMultiResx(resxObj, "Matricola", "Matricola"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Sesso",
            "title": TraduzioneMultiResx(resxObj, "Sesso", "Sesso"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Validato",
            "title": TraduzioneMultiResx(resxObj, "Validato", "Validato"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "dat_nascita",
            "title": TraduzioneMultiResx(resxObj, "DataNascita", "Data Nascita"),
            "filterable": {},
            "hidden": false,
            "widthfisso": true,
            "width": "145px",
            "format": "{0:dd/MM/yyyy}",
            template: "#= (kendo.toString(dat_nascita, 'dd/MM/yyyy' ) == '01/01/1900') ? '' : kendo.toString(dat_nascita, 'dd/MM/yyyy' ) #"
        },
        {
            "field": "SPE_DES",
            "title": TraduzioneMultiResx(resxObj, "Specie", "Specie"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "RAZ_DES",
            "title": TraduzioneMultiResx(resxObj, "Razza", "Razza"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "sa_nome",
            "title": TraduzioneMultiResx(resxObj, "Centro", "Centro"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "FlagBDN",
            "title": TraduzioneMultiResx(resxObj, "PresenteBDN", "Presente in BDN"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "bdn_codice_azienda",
            "title": TraduzioneMultiResx(resxObj, "CodiceASL", "Codice ASL"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Metodo_Produzione",
            "title": TraduzioneMultiResx(resxObj, "MetodoDiProduzione", "Metodo Di Produzione"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Modello4_Ingresso_Numero",
            "title": TraduzioneMultiResx(resxObj, "NumModello4Ingresso", "N. Modello 4 Ingresso"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        //{
        //    "field": "Modello4_Ingresso",
        //    "title": TraduzioneMultiResx(resxObj, "Modello 4 Entrata", "Modello 4 Entrata"),
        //    "filterable": {
        //        "multi": true,
        //        "search": true
        //    },
        //    "hidden": false,
        //    "widthfisso": true,
        //    "width": "145px"
        //},
        {
            "field": "Modello4_Uscita_Numero",
            "title": TraduzioneMultiResx(resxObj, "NumModello4Uscita", "N. Modello 4 Uscita"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        //{
        //    "field": "Modello4_Uscita",
        //    "title": TraduzioneMultiResx(resxObj, "Modello 4 Uscita", "Modello 4 Uscita"),
        //    "filterable": {
        //        "multi": true,
        //        "search": true
        //    },
        //    "hidden": false,
        //    "widthfisso": true,
        //    "width": "145px"
        //},
        {
            "field": "MAT_PADRE",
            "title": TraduzioneMultiResx(resxObj, "MatricolaPadre", "Matricola Padre"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "RazDes_Padre",
            "title": TraduzioneMultiResx(resxObj, "RazzaPadre", "Razza Padre"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "MAT_MADRE",
            "title": TraduzioneMultiResx(resxObj, "MatricolaMadre", "Matricola Madre"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "RazDes_Madre",
            "title": TraduzioneMultiResx(resxObj, "RazzaMadre", "Razza Madre"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Id_Capo_BDN",
            "title": TraduzioneMultiResx(resxObj, "IdentifCapoAllevBDN", "ID Capo BDN"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "CF_PROPRIETARIO",
            "title": TraduzioneMultiResx(resxObj, "CodFiscProprietario", "CF Proprietario"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "CF_DETENTORE",
            "title": TraduzioneMultiResx(resxObj, "CodFiscDetentore", "CF Detentore"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "RagSoc_FornFatt",
            "title": TraduzioneMultiResx(resxObj, "FornitoreFatturazione", "Fornitore Fatturazione"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "CF_FornFatt",
            "title": TraduzioneMultiResx(resxObj, "CodFiscFornitoreFatturazione", "C.F. Fornitore Fatturazione"),
            "hidden": true,
            "menu": false,
        },
        {
            "field": "RagSoc_FornProv",
            "title": TraduzioneMultiResx(resxObj, "FornitoreProvenienza", "Fornitore Provenienza"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "CF_FornProv",
            "title": TraduzioneMultiResx(resxObj, "CodFiscFornitoreProvenienza", "C.F. Fornitore Provenienza"),
            "hidden": true,
            "menu": false,
        },
        {
            "field": "AUSL_AZI_NASCITA",
            "title": TraduzioneMultiResx(resxObj, "CodAUSLAziendaNascita", "Codice Azienda Nascita"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "RagSoc_StallaSvezz",
            "title": TraduzioneMultiResx(resxObj, "StallaSvezzamento", "Stalla Svezzamento"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "CF_StallaSvezz",
            "title": TraduzioneMultiResx(resxObj, "CodFiscStallaSvezzamento", "C.F. Stalla Svezzamento"),
            "hidden": true,
            "menu": false,
        },
        {
            "field": "Certificato",
            "title": TraduzioneMultiResx(resxObj, "CertificatoIntra", "Certificato INTRA"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Validita_Inizio",
            "title": TraduzioneMultiResx(resxObj, "DataInizio", "Data Inizio"),
            "filterable": {},
            "hidden": false,
            "widthfisso": true,
            "width": "145px",
            "format": "{0:dd/MM/yyyy}",
            template: "#= (kendo.toString(Validita_Inizio, 'dd/MM/yyyy' ) == '01/01/1900') ? '' : kendo.toString(Validita_Inizio, 'dd/MM/yyyy' ) #"
        },
        {
            "field": "Validita_Fine",
            "title": TraduzioneMultiResx(resxObj, "DataFine", "Data Fine"),
            "filterable": {},
            "hidden": false,
            "widthfisso": true,
            "width": "145px",
            "format": "{0:dd/MM/yyyy}",
            template: "#= (kendo.toString(Validita_Fine, 'dd/MM/yyyy' ) == '31/12/2100') ? '' : kendo.toString(Validita_Fine, 'dd/MM/yyyy' ) #"
        },
        {
            "field": "Data_Modifica",
            "title": TraduzioneMultiResx(resxObj, "DataModifica", "Data Modifica"),
            "filterable": {},
            "hidden": false,
            "widthfisso": true,
            "width": "145px",
            "format": "{0:dd/MM/yyyy}"
        },
        {
            "field": "Utente_Modifica",
            "title": TraduzioneMultiResx(resxObj, "UtenteModifica", "Utente Modifica"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Data_Creazione",
            "title": TraduzioneMultiResx(resxObj, "DataCreazione", "Data Creazione"),
            "filterable": {},
            "hidden": false,
            "widthfisso": true,
            "width": "145px",
            "format": "{0:dd/MM/yyyy}"
        },
        {
            "field": "Utente_Creazione",
            "title": TraduzioneMultiResx(resxObj, "UtenteCreazione", "Utente Creazione"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Lotto_Fornitore",
            "title": TraduzioneMultiResx(resxObj, "LottoFornitore", "Lotto Fornitore"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Codice_Distinta",
            "title": TraduzioneMultiResx(resxObj, "Lotto", "Lotto"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Modello4_Ingresso_Prenotazione",
            "title": TraduzioneMultiResx(resxObj, "CodiceModello4Ingresso", "Codice Modello 4 Ingresso"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Data_Documento_Ingresso",
            "title": TraduzioneMultiResx(resxObj, "DataDocumentoIngresso", "Data Documento Ingresso"),
            "filterable": {},
            "hidden": false,
            "widthfisso": true,
            "width": "145px",
            "format": "{0:dd/MM/yyyy}",
            template: "#= ((kendo.toString(Data_Documento_Ingresso, 'dd/MM/yyyy' ) == null) || Data_Documento_Ingresso <= AGRODATAINIZIO) ? '' : kendo.toString(Data_Documento_Ingresso, 'dd/MM/yyyy' ) #"
        },
        {
            "field": "Codice_Azienda_Fornitore",
            "title": TraduzioneMultiResx(resxObj, "CodiceAziendaFornitore", "Codice Azienda Fornitore"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "N_Bolla_Fornitore",
            "title": TraduzioneMultiResx(resxObj, "NumeroDDTIngresso", "Numero DDT Ingresso"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Data_DDT_Ingresso",
            "title": TraduzioneMultiResx(resxObj, "DataDDTIngresso", "Data DDT Ingresso"),
            "filterable": {},
            "hidden": false,
            "widthfisso": true,
            "width": "145px",
            "format": "{0:dd/MM/yyyy}",
            template: "#= ((kendo.toString(Data_DDT_Ingresso, 'dd/MM/yyyy' ) == null) || Data_DDT_Ingresso <= AGRODATAINIZIO) ? '' : kendo.toString(Data_DDT_Ingresso, 'dd/MM/yyyy' ) #"
        },
        {
            "field": "Modello4_Uscita_Prenotazione",
            "title": TraduzioneMultiResx(resxObj, "CodiceModello4Uscita", "Codice Modello 4 Uscita"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Data_Documento_Uscita",
            "title": TraduzioneMultiResx(resxObj, "DataDocumentoUscita", "Data Documento Uscita"),
            "filterable": {},
            "hidden": false,
            "widthfisso": true,
            "width": "145px",
            "format": "{0:dd/MM/yyyy}",
            template: "#= ((kendo.toString(Data_Documento_Uscita, 'dd/MM/yyyy' ) == null) || Data_Documento_Uscita >= AGRODATAFINE ) ? '' : kendo.toString(Data_Documento_Uscita, 'dd/MM/yyyy' ) #"
        },
        //{
        //    "field": "Imrpesa",
        //    "title": TraduzioneMultiResx(resxObj, "Impresa", "Impresa"),
        //    "filterable": {
        //        "multi": true,
        //        "search": true
        //    },
        //    "hidden": false,
        //    "widthfisso": true,
        //    "width": "145px"
        //},
        {
            "field": "N_Bolla_Uscita",
            "title": TraduzioneMultiResx(resxObj, "NumeroDDTUscita", "Numero DDT Uscita"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
        {
            "field": "Data_DDT_Uscita",
            "title": TraduzioneMultiResx(resxObj, "DataDDTUscita", "Data DDT Uscita"),
            "filterable": {},
            "hidden": false,
            "widthfisso": true,
            "width": "145px",
            "format": "{0:dd/MM/yyyy}",
            "template": "#= ((kendo.toString(Data_DDT_Uscita, 'dd/MM/yyyy' ) == null) || Data_DDT_Uscita >= AGRODATAFINE ) ? '' : kendo.toString(Data_DDT_Uscita, 'dd/MM/yyyy' ) #"
        },
        {
            "field": "Incremento_Teorico",
            "title": TraduzioneMultiResx(resxObj, "IncrementoTeoricoKg", "Incremento Teorico Kg"),
            "filterable": {
                "multi": true,
                "search": true
            },
            "hidden": false,
            "widthfisso": true,
            "width": "145px"
        },
    ];

    // Aggiunta dell'editor per poter creare le dropdown nella griglia.
    kendo_columns.filter(element => element.field == "Sesso").forEach(function (element) {
        element["editor"] = CreaDropDownEditorTabSesso;
    });

    kendo_columns.filter(element => element.field == "Validato").forEach(function (element) {
        element["editor"] = CreaDropDownEditorTabValidato;
    });

    kendo_columns.filter(element => element.field == "RAZ_DES").forEach(function (element) {
        element["editor"] = CreaDropDownEditorTabRaz_des;
    });

    kendo_columns.filter(element => element.field == "RazDes_Madre").forEach(function (element) {
        element["editor"] = CreaDropDownEditorTabRazDes_Madre;
    });

    kendo_columns.filter(element => element.field == "RazDes_Padre").forEach(function (element) {
        element["editor"] = CreaDropDownEditorTabRazDes_Padre;
    });

    kendo_columns.filter(element => element.field == "Metodo_Produzione").forEach(function (element) {
        element["editor"] = CreaDropDownEditorTabMetodo_Produzione;
    });

    kendo_columns.filter(element => element.field == "RagSoc_FornFatt").forEach(function (element) {
        element["editor"] = CreaDropDownEditorTabFornitoreFatt;
    });

    kendo_columns.filter(element => element.field == "RagSoc_FornProv").forEach(function (element) {
        element["editor"] = CreaDropDownEditorTabFornitoreProv;
    });
    
    kendo_columns.filter(element => element.field == "RagSoc_StallaSvezz").forEach(function (element) {
        element["editor"] = CreaDropDownEditorTabStallaSvezz;
    });

    for (let i = 0; i < kendo_columns.length; i++) {
        if (hiddenCols.includes(kendo_columns[i].field)) {
            kendo_columns[i].hidden = true;
        }
    }

    return kendo_columns;
}

function kReadValorizzazione_mod_ModificaMultipla() {
    //var data = $('#' + idKendoModifica_Multipla_Zoo).val();
    //if (data != '') {

    //    jSonParsed_Kendo = JSON.parse(data);

    //    for (let key in jSonParsed_Kendo.kendo_model) {
    //        if (editableFields.includes(key)) {
    //            jSonParsed_Kendo.kendo_model[key].editable = true;
    //        }
    //    }

    //    return jSonParsed_Kendo.kendo_model;

    //}

    var kendo_model = {
        "chiave": {
            "editable": false,
            "type": "string"
        },
        "PIVA": {
            "editable": false,
            "type": "string"
        },
        "partitaIvaReale": {
            "editable": false,
            "type": "string"
        },
        "Matricola": {
            "editable": false,
            "type": "string"
        },
        "Sesso": {
            "editable": false,
            "type": "string"
        },
        "Validato": {
            "editable": false,
            "type": "string"
        },
        "dat_nascita": {
            "editable": false,
            "type": "date"
        },
        "SPE_DES": {
            "editable": false,
            "type": "string"
        },
        "RAZ_DES": {
            "editable": false,
            "type": "string"
        },
        "sa_nome": {
            "editable": false,
            "type": "string"
        },
        "FlagBDN": {
            "editable": false,
            "type": "string"
        },
        "bdn_codice_azienda": {
            "editable": false,
            "type": "string"
        },
        "Metodo_Produzione": {
            "editable": false,
            "type": "string"
        },
        //"Modello4_Ingresso": {
        //    "editable": false,
        //    "type": "string"
        //},
        //"Modello4_Uscita": {
        //    "editable": false,
        //    "type": "string"
        //},
        "MAT_PADRE": {
            "editable": false,
            "type": "string"
        },
        "RazDes_Padre": {
            "editable": false,
            "type": "string"
        },
        "MAT_MADRE": {
            "editable": false,
            "type": "string"
        },
        "RazDes_Madre": {
            "editable": false,
            "type": "string"
        },
        "Id_Capo_BDN": {
            "editable": false,
            "type": "string"
        },
        "CF_PROPRIETARIO": {
            "editable": false,
            "type": "string"
        },
        "CF_DETENTORE": {
            "editable": false,
            "type": "string"
        },
        "AUSL_AZI_NASCITA": {
            "editable": false,
            "type": "string"
        },
        "Certificato": {
            "editable": false,
            "type": "string"
        },
        "Validita_Inizio": {
            "editable": false,
            "type": "date"
        },
        "Validita_Fine": {
            "editable": false,
            "type": "date"
        },
        "Data_Modifica": {
            "editable": false,
            "type": "date"
        },
        "Utente_Modifica": {
            "editable": false,
            "type": "string"
        },
        "Data_Creazione": {
            "editable": false,
            "type": "date"
        },
        "Utente_Creazione": {
            "editable": false,
            "type": "string"
        },
        "Lotto_Fornitore": {
            "editable": false,
            "type": "string"
        },
        "Progetto_Des": {
            "editable": false,
            "type": "string"
        },
        "Modello4_Ingresso_Prenotazione": {
            "editable": false,
            "type": "string"
        },
        "Modello4_Uscita_Prenotazione": {
            "editable": false,
            "type": "string"
        },
        "Data_Documento_Ingresso": {
            "editable": false,
            "type": "date"
        },
        "Data_Documento_Uscita": {
            "editable": false,
            "type": "date"
        },
        "Rag_Soc": {
            "editable": false,
            "type": "string"
        },
        "Cod_Contatto": {
            "editable": false,
            "type": "string"
        },
        "IPRO_COD": {
            "editable": false,
            "type": "number"
        },
        "RAZ_COD": {
            "editable": false,
            "type": "number"
        },
        "SPE_COD": {
            "editable": false,
            "type": "number"
        },
        "GEN_COD": {
            "editable": false,
            "type": "number"
        },
        "Sa_Cod": {
            "editable": false,
            "type": "number"
        },
        "Cod_Progetto": {
            "editable": false,
            "type": "number"
        },
        "Cod_Animale": {
            "editable": false,
            "type": "number"
        },
        "RazCod_Padre": {
            "editable": false,
            "type": "number"
        },
        "RazCod_Madre": {
            "editable": false,
            "type": "number"
        },
        "RagSoc_FornFatt": {
            "editable": false,
            "type": "string"
        },
        "CF_FornFatt": {
            "editable": false,
            "type": "string"
        },
        "RagSoc_FornProv": {
            "editable": false,
            "type": "string"
        },
        "CF_FornProv": {
            "editable": false,
            "type": "string"
        },
        "N_Bolla_Fornitore": {
            "editable": false,
            "type": "string"
        },
        "Data_DDT_Ingresso": {
            "editable": false,
            "type": "date"
        },
        "N_Bolla_Uscita": {
            "editable": false,
            "type": "string"
        },
        "Data_DDT_Uscita": {
            "editable": false,
            "type": "date"
        },
        "Codice_Azienda_Fornitore": {
            "editable": false,
            "type": "string"
        },
        "RagSoc_StallaSvezz": {
            "editable": false,
            "type": "string"
        },
        "CF_StallaSvezz": {
            "editable": false,
            "type": "string"
        },
        "Incremento_Teorico": {
            "editable": false,
            "type": "number"
        }
    }

    for (let key in kendo_model) {
        if (editableFields.includes(key)) {
            kendo_model[key].editable = true;
        }
    }

    return kendo_model;
}

function onDataBounding_ModificaMultipla(e) {
    var gridId = e.sender.element[0].id;
    kendo_AggiustaDimensioneColonne("#" + gridId);
}

function KendoModificaMultipla_checked(e) {

    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = $('#divKendoModifica_Multipla_Zoo').data("kendoGrid");
    var dataItem = grid.dataItem(row);
    dataItem.Selected = true;
    dataItem.Selected = checked;
    dataItem.dirty = true;
    rowKendoGridSelected(row, checked)
}

function gestioneHiddenCols(e) {
    if (!hiddenCols.includes(e.column.field)) {
        hiddenCols.push(e.column.field);
    }
}

function gestioneShowedCols(e) {
    for (let i = 0; i < hiddenCols.length; i++) {
        if (hiddenCols[i] == e.column.field) {
            hiddenCols.splice(i, 1);
        }
    }
}

function CreaDropDownEditorTabSesso(container) {
    // Sesso è presente due volte, altrimenti non veniva visualizzato correttamente nella kendo dropdown list
    let ds = [{ Sesso: "M", Sesso_Des: TraduzioneMultiResx(resxObj, "MaschioSigla", "M") }, { Sesso: "F", Sesso_Des: TraduzioneMultiResx(resxObj, "FemminaSigla", "F") }];
    creaDropDownEditor(container, "Sesso_Des", "Sesso", ds, changeTabSesso);
}

function CreaDropDownEditorTabValidato(container) {
    let ds = [{ Validato: "Si", Validato_Des: TraduzioneMultiResx(resxObj, "Si", "Si") }, { Validato: "No", Validato_Des: TraduzioneMultiResx(resxObj, "No", "No") }];
    creaDropDownEditor(container, "Validato_Des", "Validato", ds, changeTabValidato);
}

//async function CreaDropDownEditorTabRaz_des(container, e) {
//    let ds = await ws_getRazzeGriglia(e.model.SPE_COD, e.model.GEN_COD)
//    creaDropDownEditor(container, "RAZ_DES", "RAZ_COD", ds, changeTabRaz_des);
//    KendoDDL("RAZ_COD").value(e.model.RAZ_COD);
//}

function CreaDropDownEditorTabRaz_des(container, e) {
    let ds = ws_getRazzeGrigliaSync(e.model.SPE_COD, e.model.GEN_COD);
    creaDropDownEditorId(container, "RAZ_DES", "RAZ_COD", ds, changeTabRaz_des, "RAZ_COD", "");
    KendoDDL("RAZ_COD").element.removeAttr("required");
}

//async function CreaDropDownEditorTabRazDes_Madre(container, e) {
//    let ds = await ws_getRazzeGrigliaMadre(e.model.SPE_COD, e.model.GEN_COD);

//    creaDropDownEditor(container, "RazDes_Madre", "RazCod_Madre", ds, changeTabRazDes_Madre);
//    KendoDDL("RazCod_Madre").value(e.model.RazCod_Madre);
//}

function CreaDropDownEditorTabRazDes_Madre(container, e) {
    let ds = ws_getRazzeGrigliaMadreSync(e.model.SPE_COD, e.model.GEN_COD);
    creaDropDownEditorId(container, "RazDes_Madre", "RazCod_Madre", ds, changeTabRazDes_Madre, "RazCod_Madre", "");
    KendoDDL("RazCod_Madre").element.removeAttr("required");
}

//async function CreaDropDownEditorTabRazDes_Padre(container, e) {
//    let ds = await ws_getRazzeGrigliaPadre(e.model.SPE_COD, e.model.GEN_COD);

//    creaDropDownEditor(container, "RazDes_Padre", "RazCod_Padre", ds, changeTabRazDes_Padre);
//    KendoDDL("RazCod_Padre").value(e.model.RazCod_Padre);
//}

function CreaDropDownEditorTabRazDes_Padre(container, e) {
    let ds = ws_getRazzeGrigliaPadreSync(e.model.SPE_COD, e.model.GEN_COD);
    creaDropDownEditorId(container, "RazDes_Padre", "RazCod_Padre", ds, changeTabRazDes_Padre, "RazCod_Padre", "");
    KendoDDL("RazCod_Padre").element.removeAttr("required");
}

//async function CreaDropDownEditorTabFornitoreFatt(container, e) {
//    //if (Elenco_FornitoriFatt === undefined || Elenco_FornitoriFatt === null) {
//        let ds = await CaricaComboCmb_FornitoreGriglia(e.model.PIVA);
//        ds.unshift({ Rag_Soc_Completa: TraduzioneMultiResx(resxObj, "Seleziona", "Seleziona"), Cod_Contatto: "0" });
//        let dsFatt = [...ds];
//        dsFatt = dsFatt.map((x) => { return { CF_FornFatt: x.Cod_Contatto, RagSoc_FornFatt: x.Rag_Soc_Completa } });
//        //Elenco_FornitoriFatt = [...ds];
//        //Elenco_FornitoriFatt = Elenco_FornitoriFatt.map((x) => { return { CF_FornFatt: x.Cod_Contatto, RagSoc_FornFatt: x.Rag_Soc_Completa } });
//    //}

//    creaDropDownEditor(container, "RagSoc_FornFatt", "CF_FornFatt", dsFatt, changeTabFornitoreFatt);
//    KendoDDL("CF_FornFatt").bind("close", ddlFornitore_onClose);

//    if (e.model.CF_FornFatt === "") {
//        KendoDDL("CF_FornFatt").value("0");
//    } else {
//        KendoDDL("CF_FornFatt").value(e.model.CF_FornFatt);
//    }
//}

function CreaDropDownEditorTabFornitoreFatt(container, e) {
    let ds = CaricaComboCmb_FornitoreGrigliaSync(e.model.PIVA);
    if (ds !== undefined && ds !== null) {
        ds = ds.map((x) => { return { CF_FornFatt: x.Cod_Contatto, RagSocCompl_FornFatt: x.Rag_Soc_Completa, RagSoc_FornFatt: x.Rag_Soc } });
    } else {
        ds = [];
    }    
    ds.unshift({ RagSocCompl_FornFatt: TraduzioneMultiResx(resxObj, "Seleziona", "Seleziona"), CF_FornFatt: "", RagSoc_FornFatt: "" });

    creaDropDownEditorId(container, "RagSocCompl_FornFatt", "CF_FornFatt", ds, changeTabFornitoreFatt, "CF_FornFatt", "");
    KendoDDL("CF_FornFatt").element.removeAttr("required");
    KendoDDL("CF_FornFatt").bind("close", ddlFornitore_onClose);
}

//async function CreaDropDownEditorTabFornitoreProv(container, e) {
//    //if (Elenco_FornitoriProv === undefined || Elenco_FornitoriProv === null) {
//    let ds = await CaricaComboCmb_FornitoreGriglia(e.model.PIVA);
//    ds.unshift({ Rag_Soc_Completa: TraduzioneMultiResx(resxObj, "Seleziona", "Seleziona"), Cod_Contatto: "0" });
//    let dsProv = [...ds];
//    dsProv = dsProv.map((x) => { return { CF_FornProv: x.Cod_Contatto, RagSoc_FornProv: x.Rag_Soc_Completa } });
//    //Elenco_FornitoriProv = [...ds];
//    //Elenco_FornitoriProv = Elenco_FornitoriProv.map((x) => { return { CF_FornProv: x.Cod_Contatto, RagSoc_FornProv: x.Rag_Soc_Completa } });
//    //}

//    creaDropDownEditor(container, "RagSoc_FornProv", "CF_FornProv", dsProv, changeTabFornitoreProv);
//    KendoDDL("CF_FornProv").bind("close", ddlFornitore_onClose);

//    if (e.model.CF_FornProv === "") {
//        KendoDDL("CF_FornProv").value("0");
//    } else {
//        KendoDDL("CF_FornProv").value(e.model.CF_FornProv);
//    }
//}

function CreaDropDownEditorTabFornitoreProv(container, e) {
    let ds = CaricaComboCmb_FornitoreGrigliaSync(e.model.PIVA);
    if (ds !== undefined && ds !== null) {
        ds = ds.map((x) => { return { CF_FornProv: x.Cod_Contatto, RagSocCompl_FornProv: x.Rag_Soc_Completa, RagSoc_FornProv: x.Rag_Soc } });
    } else {
        ds = [];
    }        
    ds.unshift({ RagSocCompl_FornProv: TraduzioneMultiResx(resxObj, "Seleziona", "Seleziona"), CF_FornProv: "", RagSoc_FornProv: "" });

    creaDropDownEditorId(container, "RagSocCompl_FornProv", "CF_FornProv", ds, changeTabFornitoreProv, "CF_FornProv", "");
    KendoDDL("CF_FornProv").element.removeAttr("required");
    KendoDDL("CF_FornProv").bind("close", ddlFornitore_onClose);
}

//async function CreaDropDownEditorTabStallaSvezz(container, e) {
//    /*if (Elenco_StallaSvezz === undefined || Elenco_StallaSvezz === null) {*/
//    let ds = await CaricaComboCmb_FornitoreGriglia(e.model.PIVA);
//    ds.unshift({ Rag_Soc_Completa: TraduzioneMultiResx(resxObj, "Seleziona", "Seleziona"), Cod_Contatto: "0" });
//    let dsSvezz = [...ds];
//    dsSvezz = dsSvezz.map((x) => { return { CF_StallaSvezz: x.Cod_Contatto, RagSoc_StallaSvezz: x.Rag_Soc_Completa } });
//    //Elenco_StallaSvezz = [...ds];
//    //Elenco_StallaSvezz = Elenco_StallaSvezz.map((x) => { return { CF_StallaSvezz: x.Cod_Contatto, RagSoc_StallaSvezz: x.Rag_Soc_Completa } });
//    //}

//    creaDropDownEditor(container, "RagSoc_StallaSvezz", "CF_StallaSvezz", dsSvezz, changeTabStallaSvezz);
//    KendoDDL("CF_StallaSvezz").required = false;
//    KendoDDL("CF_StallaSvezz").bind("close", ddlFornitore_onClose);

//    if (e.model.CF_StallaSvezz === "") {
//        KendoDDL("CF_StallaSvezz").value("0");
//    } else {
//        KendoDDL("CF_StallaSvezz").value(e.model.CF_StallaSvezz);
//    }
//}

function CreaDropDownEditorTabStallaSvezz(container, e) {
    let ds = CaricaComboCmb_FornitoreGrigliaSync(e.model.PIVA);
    if (ds !== undefined && ds !== null) {
        ds = ds.map((x) => { return { CF_StallaSvezz: x.Cod_Contatto, RagSocCompl_StallaSvezz: x.Rag_Soc_Completa, RagSoc_StallaSvezz: x.Rag_Soc } });
    } else {
        ds = [];
    }            
    ds.unshift({ RagSocCompl_StallaSvezz: TraduzioneMultiResx(resxObj, "Seleziona", "Seleziona"), CF_StallaSvezz: "", RagSoc_StallaSvezz: "" });

    creaDropDownEditorId(container, "RagSocCompl_StallaSvezz", "CF_StallaSvezz", ds, changeTabStallaSvezz, "CF_StallaSvezz", "");
    KendoDDL("CF_StallaSvezz").element.removeAttr("required");
    KendoDDL("CF_StallaSvezz").bind("close", ddlFornitore_onClose);
}

function CreaDropDownEditorTabMetodo_Produzione(container) {
    let ds = [
        { Metodo_Produzione: "Integrato", Metodo_Produzione_Des: TraduzioneMultiResx(resxObj, "Integrato", "Integrato") },
        { Metodo_Produzione: "In Conversione", Metodo_Produzione_Des: TraduzioneMultiResx(resxObj, "InConversione", "In Conversione") },
        { Metodo_Produzione: "Biologico", Metodo_Produzione_Des: TraduzioneMultiResx(resxObj, "Biologico", "Biologico") }
    ];

    creaDropDownEditor(container, "Metodo_Produzione_Des", "Metodo_Produzione", ds, changeTabMetodo_Produzione);
}

function changeTabSesso(e) {
    var dataItem = e.sender.dataItem();
    var grid = $('#divKendoModifica_Multipla_Zoo').data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.Sesso = dataItem.Sesso;
    model.dirty = true
    if (!arrayTipoAnagraficaModificati.includes("Sesso")) {
        arrayTipoAnagraficaModificati.push("Sesso")
    }
}

function changeTabValidato(e) {
    var dataItem = e.sender.dataItem();
    var grid = $('#divKendoModifica_Multipla_Zoo').data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.Validato = dataItem.Validato;
    model.dirty = true
    if (!arrayTipoAnagraficaModificati.includes("Validato")) {
        arrayTipoAnagraficaModificati.push("Validato")
    }
}

function changeTabRaz_des(e) {
    var dataItem = e.sender.dataItem();
    var grid = $('#divKendoModifica_Multipla_Zoo').data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
    model.RAZ_DES = dataItem.RAZ_DES;
    model.RAZ_COD = dataItem.RAZ_COD;
    model.dirty = true
    if (!arrayTipoAnagraficaModificati.includes("RAZ_COD")) {
        arrayTipoAnagraficaModificati.push("RAZ_COD")
    }
    kendoFastRedrawRow(grid, row);
}

function changeTabRazDes_Madre(e) {
    var dataItem = e.sender.dataItem();
    var grid = $('#divKendoModifica_Multipla_Zoo').data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.RazDes_Madre = dataItem.RazDes_Madre;
    model.RazCod_Madre = dataItem.RazCod_Madre
    model.Razza_Madre = dataItem.RazCod_Madre
    model.dirty = true
    if (!arrayTipoAnagraficaModificati.includes("Razza_Madre")) {
        arrayTipoAnagraficaModificati.push("Razza_Madre")
    }
}

function changeTabRazDes_Padre(e) {
    var dataItem = e.sender.dataItem();
    var grid = $('#divKendoModifica_Multipla_Zoo').data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.RazDes_Padre = dataItem.RazDes_Padre;
    model.RazCod_Padre = dataItem.RazCod_Padre
    model.Razza_Padre = dataItem.RazCod_Padre
    model.dirty = true
    arrayTipoAnagraficaModificati.push("Razza_Padre")
}

function changeTabMetodo_Produzione(e) {
    var dataItem = e.sender.dataItem();
    var grid = $('#divKendoModifica_Multipla_Zoo').data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.Metodo_Produzione = dataItem.Metodo_Produzione;
    model.dirty = true
    if (!arrayTipoAnagraficaModificati.includes("Metodo_Produzione")) {
        arrayTipoAnagraficaModificati.push("Metodo_Produzione")
    }
}


//function changeFornitoreFatt(e) {
//    var dataItem = e.sender.dataItem();
//    var grid = $("#divKendoModifica_Multipla_Zoo").data("kendoGrid");
//    var model = grid.dataItem(this.element.closest("tr"));
//    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
//    if (dataItem == undefined || dataItem.CF_FornFatt == "0" || dataItem.CF_FornFatt == "" ||
//        dataItem.CF_FornFatt == null || dataItem.CF_FornFatt == undefined) {
//        model.CF_FornFatt = "";
//        model.RagSoc_FornFatt = "";
//    } else {
//        model.CF_FornFatt = dataItem.CF_FornFatt;
//        model.RagSoc_FornFatt = dataItem.RagSoc_FornFatt;
//    }

//    model.dirty = true
//    arrayTipoAnagraficaModificati.push("CF_Fornitore")
//    kendoFastRedrawRow(grid, row);
//}

function changeTabFornitoreFatt(e) {
    var dataItem = e.sender.dataItem();
    var grid = $('#divKendoModifica_Multipla_Zoo').data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    if (dataItem == undefined || dataItem.CF_FornFatt == "0" || dataItem.CF_FornFatt == "" ||
        dataItem.CF_FornFatt == null || dataItem.CF_FornFatt == undefined) {
        model.RagSoc_FornFatt = "";
        model.CF_FornFatt = "";
    } else {
        model.RagSoc_FornFatt = dataItem.RagSoc_FornFatt;
        model.CF_FornFatt = dataItem.CF_FornFatt;
    }

    model.dirty = true
    if (!arrayTipoAnagraficaModificati.includes("CF_FornFatt")) {
        arrayTipoAnagraficaModificati.push("CF_FornFatt")
    }
}

//function changeFornitoreProv(e) {
//    var dataItem = e.sender.dataItem();
//    var grid = $("#divKendoModifica_Multipla_Zoo").data("kendoGrid");
//    var model = grid.dataItem(this.element.closest("tr"));
//    var row = grid.tbody.find("tr[data-uid='" + model.uid + "']");
//    if (dataItem == undefined || dataItem.CF_FornProv == "0" || dataItem.CF_FornProv == "" ||
//        dataItem.CF_FornProv == null || dataItem.CF_FornProv == undefined) {
//        model.CF_FornProv = "";
//        model.RagSoc_FornProv = "";
//    } else {
//        model.CF_FornProv = dataItem.CF_FornProv;
//        model.RagSoc_FornProv = dataItem.RagSoc_FornProv;
//    }

//    model.dirty = true
//    arrayTipoAnagraficaModificati.push("Fornitore_Provenienza")
//    kendoFastRedrawRow(grid, row);
//}

function changeTabFornitoreProv(e) {
    var dataItem = e.sender.dataItem();
    var grid = $('#divKendoModifica_Multipla_Zoo').data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    if (dataItem == undefined || dataItem.CF_FornProv == "0" || dataItem.CF_FornProv == "" ||
        dataItem.CF_FornProv == null || dataItem.CF_FornProv == undefined) {
        model.RagSoc_FornProv = "";
        model.CF_FornProv = "";
    } else {
        model.RagSoc_FornProv = dataItem.RagSoc_FornProv;
        model.CF_FornProv = dataItem.CF_FornProv;
    }

    model.dirty = true
    if (!arrayTipoAnagraficaModificati.includes("CF_FornProv")) {
        arrayTipoAnagraficaModificati.push("CF_FornProv")
    }
}

function changeTabStallaSvezz(e) {
    var dataItem = e.sender.dataItem();
    var grid = $('#divKendoModifica_Multipla_Zoo').data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));

    if (dataItem == undefined || dataItem.CF_StallaSvezz == "0" || dataItem.CF_StallaSvezz == "" ||
        dataItem.CF_StallaSvezz == null || dataItem.CF_StallaSvezz == undefined) {
        model.RagSoc_StallaSvezz = "";
        model.CF_StallaSvezz = "";
    } else {
        model.RagSoc_StallaSvezz = dataItem.RagSoc_StallaSvezz;
        model.CF_StallaSvezz = dataItem.CF_StallaSvezz;
    }

    model.dirty = true
    if (!arrayTipoAnagraficaModificati.includes("CF_StallaSvezz")) {
        arrayTipoAnagraficaModificati.push("CF_StallaSvezz")
    }
}

function ddlFornitore_onClose(e) {
    if (e.sender.selectedIndex == -1) {
        e.sender.value("0");
        e.sender.trigger("change");
    }
}

//-------------------------------------------------------------
//------------------------BOTTONI-KENDO------------------------
//-------------------------------------------------------------

function CreaToolBarZoo() {

    var gridTB = $("#divKendoModifica_Multipla_Zoo").find(".k-grid-toolbar");

    if (permesso_modificaMultipla) {
        if ($("#btn_ModificaMultipla_Zoo").length > 0) {
            return;
        }
        if (GiasVersioneMaster === "2022") {
            gridTB.append('<div id="btn_ModificaMultipla_Zoo" class="k-button k-button-icontext k-grid--button" data-title="' + TraduzioneMultiResx(resxObj, "FinestraModificaMultipla", "Finestra Modifica Multipla") + '"><i class="k-icon k-i-list-unordered"></i></div>');
        } else {
            gridTB.append('<div id="btn_ModificaMultipla_Zoo" class="k-button k-button-icontext"><i class="k-icon k-i-list-unordered"></i>' + TraduzioneMultiResx(resxObj, "FinestraModificaMultipla", "Finestra Modifica Multipla") + '</div>');
        }
    }

    $("#btn_ModificaMultipla_Zoo").click(function (e) {
        ModificaMultiplaZoo();
    });
}


function ModificaMultiplaZoo() {

    creaModificaMultipla();

    var grid = $("#divKendoModifica_Multipla_Zoo").data("kendoGrid");
    var data = grid.dataSource.data();
    selected_add = new Array();
    selected_rows = new Array();

    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected == true)
            selected_rows.push(data[i]);
    }

    if (selected_rows.length != null && selected_rows.length > 0) {
        var SPE_COD = "";
        var SPE_COD_uguale = true;

        for (let i = 0; i < selected_rows.length; i++) {
            if (SPE_COD == "") {
                SPE_COD = selected_rows[i].SPE_COD;
            }
            if (SPE_COD != selected_rows[i].SPE_COD) {
                SPE_COD_uguale = false;
            }
            selected_add.push(selected_rows[i]);
        }

        obj_ModificaMultipla = {};

        obj_ModificaMultipla.SPE_COD_uguale = SPE_COD_uguale;
        obj_ModificaMultipla.SPE_COD = selected_add[0].SPE_COD;
        obj_ModificaMultipla.GEN_COD = selected_add[0].GEN_COD;
        obj_ModificaMultipla.Sa_Cod = selected_add[0].Sa_Cod;
        obj_ModificaMultipla.PIVA = selected_add[0].PIVA;

        win_ModificaMultipla.open();
    }
    else {
        kendo.alert(TraduzioneMultiResx(resxObj, "SelezionareAlmenoUnAnimale", "Selezionare almeno un capo animale."));
    }
}

async function SubmitGrid_ModificaMultiplaZoo(options) {

    //arrayTipoAnagraficaModificati.push("MAT_MADRE", "MAT_PADRE", "CF_DETENTORE", "CF_PROPRIETARIO", "Certificato", "Modello4_Ingresso", "Modello4_Uscita", "Modello4_Ingresso_Prenotazione", "Modello4_Uscita_Prenotazione", "Data_Documento_Ingresso", "Data_Documento_Uscita", "Lotto_Fornitore", "Lotto", "Fornitore_Fatturazione", "Fornitore_Provenienza", "Data_DDT_Ingresso", "Data_DDT_Uscita", "N_Bolla_Fornitore", "N_Bolla_Uscita", "Codice_Azienda_Fornitore");
    // TO DO verificare date 
    //arrayTipoAnagraficaModificati.filter(onlyUnique);
    //arrayTipoAnagraficaModificati = ["RAZ_COD", "Razza_Madre", "MAT_MADRE", "Razza_Padre", "MAT_PADRE", "Sesso", "Validato", "CF_DETENTORE", "CF_PROPRIETARIO", "Metodo_Produzione", "Certificato", "Modello4_Ingresso_Numero", "Modello4_Uscita_Numero", "Modello4_Ingresso_Prenotazione", "Modello4_Uscita_Prenotazione", "Data_Documento_Ingresso", "Data_Documento_Uscita", "Lotto_Fornitore", "Lotto", "CF_FornFatt", "CF_FornProv", "Data_DDT_Ingresso", "Data_DDT_Uscita", "N_Bolla_Fornitore", "N_Bolla_Uscita", "Codice_Azienda_Fornitore", "AUSL_AZI_NASCITA", "CF_StallaSvezz", "Incremento_Teorico"];
    obj_ModificaMultipla.anagrafica = arrayTipoAnagraficaModificati.filter(onlyUnique);

    if (options.data.updated == 0) {
        kendo.alert(TraduzioneMultiResx(resxObj, "ModificareAlmenoUnDato", "Non sono stati forniti dei dati da modificare."));
        return;
    }

    let procediComunque = true;

    for (var i = 0; i < options.data.updated.length; i++) {
        for (var j = 0; j < campiTextboxEditabili.length; j++) {
            let condition = options.data.updated[i][campiTextboxEditabili[j]]

            // Non necessario in quanto l'utente deve poter lasciare vuoti i campi.
            //switch (condition) {
            //    case "":
            //    //Fall-through
            //    case " ":
            //    //Fall-through
            //    case undefined:
            //        kendo.alert("Non è stato specificato un valore in uno o più campi da modificare.");
            //        return;
            //}

            if (campiTextboxEditabili[j] == "CF_PROPRIETARIO" || campiTextboxEditabili[j] == "CF_DETENTORE")
                if (!(condition.length >= 11 && condition.length <= 16) && (warningCounter == false)) {
                    if (!(condition.length == 0)) {
                        procediComunque = await confirm(TraduzioneMultiResx(resxObj, "ConfermaSalvataggioCodFiscErrati", "Uno o più CF non sono corretti.\nVuoi procedere comunque con l'inserimento?"));
                        warningCounter = true; // Così l'utente viene avvisato una volta sola.
                    }
                }
            if (!procediComunque) {
                return
            }
        }
    }

    updatedRecords = options.data.updated;
    updatedRecords.forEach(x => {
        if (x.Data_Documento_Ingresso instanceof Date && !isNaN(x.Data_Documento_Ingresso.valueOf())) {
            x.Data_Documento_Ingresso = (x.Data_Documento_Ingresso.getDate() + '-' + (x.Data_Documento_Ingresso.getMonth() + 1) + '-' + x.Data_Documento_Ingresso.getFullYear());
        }
        if (x.Data_Documento_Uscita instanceof Date && !isNaN(x.Data_Documento_Uscita.valueOf())) {
            x.Data_Documento_Uscita = (x.Data_Documento_Uscita.getDate() + '-' + (x.Data_Documento_Uscita.getMonth() + 1) + '-' + x.Data_Documento_Uscita.getFullYear());
        }
        if (x.Data_DDT_Ingresso instanceof Date && !isNaN(x.Data_DDT_Ingresso.valueOf())) {
            x.Data_DDT_Ingresso = (x.Data_DDT_Ingresso.getDate() + '-' + (x.Data_DDT_Ingresso.getMonth() + 1) + '-' + x.Data_DDT_Ingresso.getFullYear());
        }
        if (x.Data_DDT_Uscita instanceof Date && !isNaN(x.Data_DDT_Uscita.valueOf())) {
            x.Data_DDT_Uscita = (x.Data_DDT_Uscita.getDate() + '-' + (x.Data_DDT_Uscita.getMonth() + 1) + '-' + x.Data_DDT_Uscita.getFullYear());
        }
        if (x.CF_FornFatt == undefined || x.CF_FornFatt == null) {
            x.CF_FornFatt = "0";
        }
        if (x.CF_FornProv == undefined || x.CF_FornProv == null) {
            x.CF_FornProv = "0";
        }
        if (x.CF_StallaSvezz == undefined || x.CF_StallaSvezz == null) {
            x.CF_StallaSvezz = "0";
        }
        //(x.CF_FornFatt == undefined || x.CF_FornFatt == null) ? x.CF_FornFatt = "0" : null;
        //(x.CF_FornProv == undefined || x.CF_FornProv == null) ? x.CF_FornProv = "0" : null;
    });

    WaitFrame.show();
    let success = await ws_salvataggioModificaMultiplaGriglia();
    if (success) {
        //window.location.reload(); // Aggiorna la pagina.
        KendoModifica_Multipla_Zoo.destroy();
        popolaGriglia_ModificaMultipla("divKendoModifica_Multipla_Zoo");
        kendo.alert(TraduzioneMultiResx(resxObj, "ModificaEffettuataConSuccesso", "Modifica effettuata con successo."));
        arrayTipoAnagraficaModificati.length = 0;
    } else {
        WaitFrame.hide();
        kendo.alert(TraduzioneMultiResx(resxObj, "ErroreDuranteModifica", "Errore durante la modifica."));
    }
}


//-------------------------------------------------------------
//----------------------MODIFICA-MULTIPLA----------------------
//-------------------------------------------------------------

function creaModificaMultipla() {

    win_ModificaMultipla = $("#winModificaMultipla").kendoWindow({
        width: "640px",
        height: "80%",
        modal: true,
        title: TraduzioneMultiResx(resxObj, "MenuBS_Anagrafica_modificaMultipla", "Modifica Multipla"),
        closable: true,
        visible: false,
        resizable: true,
        open: async function (e) {
            this.center();
            await apriModificaMultipla();
        }
    }).data("kendoWindow");

}


async function apriModificaMultipla() {
    await pulisciControlli();
    let data = new Array();
    parametri_multipli = true;

    $("#avvertimentoModificaMultipla").text("");

    var stessa_specie = obj_ModificaMultipla.SPE_COD_uguale;
    stessa_specie_cod = await [obj_ModificaMultipla.SPE_COD, obj_ModificaMultipla.GEN_COD]

    var presetColumns = (presetColumnsToUpdate !== "") ? JSON.parse(presetColumnsToUpdate) : [];

    // Se i capi selezionati non sono della stessa specie nella dropdown non appaiono le seguenti voci.
    if (stessa_specie) {
        data.push({ des: TraduzioneMultiResx(resxObj, "RazzaCapo", "Razza Capo"), value: "1", raggruppamento: " STESSA SPECIE" });

        // Categoria da non implementare al momento in quanto non viene presa quando viene letta la giacenza via DAL
        //data.push({ des: "Categoria Capo", value: "2", raggruppamento: " STESSA SPECIE" });

        data.push({ des: TraduzioneMultiResx(resxObj, "MatricolaMadre", "Matricola Madre"), value: "3", raggruppamento: " STESSA SPECIE" });
        data.push({ des: TraduzioneMultiResx(resxObj, "RazzaMadre", "Razza Madre"), value: "4", raggruppamento: " STESSA SPECIE" });
        data.push({ des: TraduzioneMultiResx(resxObj, "MatricolaPadre", "Matricola Padre"), value: "5", raggruppamento: " STESSA SPECIE" });
        data.push({ des: TraduzioneMultiResx(resxObj, "RazzaPadre", "Razza Padre"), value: "6", raggruppamento: " STESSA SPECIE" });
    } else {
        $("#avvertimentoModificaMultipla").text(
            TraduzioneMultiResx(resxObj, "AvvertimentoModificaMultipla", "ATTENZIONE: Alcuni parametri non sono modificabili in quanto sono state selezionate specie diverse.")
        );
    }

    data.push({ des: TraduzioneMultiResx(resxObj, "Sesso", "Sesso"), value: "7", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "MetodoProduzione", "Metodo Produzione"), value: "8", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "CodFiscDetentore", "CF Detentore"), value: "9", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "CodFiscProprietario", "CF Proprietario"), value: "10", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "CertificatoINTRA", "Certificato INTRA"), value: "11", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "NumModello4Ingresso", "N. Modello 4 Ingresso"), value: "12", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "NumModello4Uscita", "N. Modello 4 Uscita"), value: "13", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "CodiceModello4Ingresso", "Codice Modello 4 Ingresso"), value: "14", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "CodiceModello4Uscita", "Codice Modello 4 Uscita"), value: "15", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "LottoFornitore", "Lotto Fornitore"), value: "16", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "Lotto", "Lotto"), value: "17", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "DataDocumentoIngresso", "Data Documento Ingresso"), value: "18", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "DataDocumentoUscita", "Data Documento Uscita"), value: "19", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "FornitoreFatturazione", "Fornitore Fatturazione"), value: "20", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "FornitoreProvenienza", "Fornitore Provenienza"), value: "21", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "NumeroDDTIngresso", "Numero DDT Ingresso"), value: "22", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "DataDDTIngresso", "Data DDT Ingresso"), value: "23", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "NumeroDDTUscita", "Numero DDT Uscita"), value: "24", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "DataDDTUscita", "Data DDT Uscita"), value: "25", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "CodiceAziendaProvenienza", "Codice Azienda Provenienza"), value: "26", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "Validato", "Validato"), value: "27", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "CodAUSLAziendaNascita", "Codice Azienda Nascita"), value: "28", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "StallaSvezzamento", "Stalla Svezzamento"), value: "29", raggruppamento: "  CAMPI MODIFICABILI" });
    data.push({ des: TraduzioneMultiResx(resxObj, "IncrementoTeoricoKg", "Incremento Teorico Kg"), value: "30", raggruppamento: "  CAMPI MODIFICABILI" });

    if (parametri_multipli && Cmb_Parametri != undefined) {

        Cmb_Parametri.setDataSource({ data: data, group: "raggruppamento" });

        Cmb_Parametri.value(parametri_multipli ? presetColumns : "");
        Cmb_Parametri.trigger("change");

    } else {
        let onLoad = true;

        if (parametri_multipli) {
            return new Promise((resolve, reject) => {
                Cmb_Parametri = $("#Cmb_Parametri").kendoMultiSelect({
                    filter: "contains",
                    autoBind: true,
                    autoClose: false,
                    dataTextField: "des",
                    dataValueField: "value",
                    dataSource: { data: data, group: "raggruppamento" },
                    open: kendoDropDownAdjustWidth,
                    dataBound: function (e) {
                        kendoDropDownAdjustWidth(e);
                        if (onLoad) {
                            this.value(presetColumns);
                            this.trigger("change");
                            onLoad = false;
                        }
                    },
                    change: async function (e) {
                        WaitFrame.show();
                        await CambiaParametri(this.value());
                        WaitFrame.hide();
                        resolve(this);
                    },
                    placeholder: TraduzioneMultiResx(resxObj, "Seleziona", "Seleziona").toUpperCase()
                }).data("kendoMultiSelect");

            });

        } else {

            return new Promise((resolve, reject) => {
                Cmb_Parametri = $("#Cmb_Parametri").kendoDropDownList({
                    filter: "contains",
                    autoBind: true,
                    dataTextField: "des",
                    dataValueField: "value",
                    dataSource: { data: data, group: "raggruppamento" },
                    open: kendoDropDownAdjustWidth,
                    dataBound: function (e) {
                        kendoDropDownAdjustWidth(e);
                        if (onLoad) {
                            this.value("");
                            this.trigger("change");
                            onLoad = false;
                        }
                    },
                    change: async function (e) {
                        WaitFrame.show();
                        await CambiaParametri([this.value()]);
                        WaitFrame.hide();
                        resolve(this);
                    },
                    optionLabel: TraduzioneMultiResx(resxObj, "Seleziona", "Seleziona").toUpperCase()
                }).data("kendoDropDownList");
            });
        }
    }
}

async function applicaModifiche() {

    if (Cmb_Parametri.value() == "" || Cmb_Parametri.value().length == 0) {
        kendo.alert(TraduzioneMultiResx(resxObj, "SelezionareParametroDaModificare", "Selezionare parametro da modificare."));
        return false;
    }

    let parametri = parametri_multipli ? Cmb_Parametri.value() : [Cmb_Parametri.value()];

    for (i = 0; i < parametri.length; i++) {
        switch (parametri[i]) {
            case "1":
                var RAZ_COD = Cmb_Modifica_Razza_Capo.value();
                var RAZ_DES = Cmb_Modifica_Razza_Capo.text();
                if (RAZ_COD == "") {
                    kendo.alert(TraduzioneMultiResx(resxObj, "SelezionareRazzaCapoDaModificare", "Selezionare la razza del capo da modificare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].RAZ_COD = RAZ_COD;
                        selected_add[i].RAZ_DES = RAZ_DES;
                    }
                    arrayTipoAnagraficaModificati.push("RAZ_COD")
                }
                break;
            //case "2":
            //    var CAT_COD = Cmb_modifica_Categoria_Capo.val();
            //    if (CAT_COD == "" || CAT_COD == null) {
            //        kendo.alert("Inserire la categoria del capo.");
            //        return false;
            //    } else {
            //        for (let i = 0; i < selected_add.length; i++) {
            //            selected_add[i].CAT_COD = CAT_COD;
            //        }
            //        arrayTipoAnagraficaModificati.push("2")
            //    }
            //    break;
            case "3":
                var MAT_MADRE = Txt_Matricola_Madre.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].MAT_MADRE = MAT_MADRE;
                }
                arrayTipoAnagraficaModificati.push("MAT_MADRE")
                break;
            case "4":
                var RAZ_DES_MADRE = Cmb_modifica_Razza_Madre.text();
                var RAZ_COD_MADRE = Cmb_modifica_Razza_Madre.value();
                if (RAZ_COD_MADRE == "") {
                    kendo.alert(TraduzioneMultiResx(resxObj, "SelezionareRazzaMadreCapoDaModificare", "Selezionare la razza madre del capo da modificare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Razza_Madre = RAZ_COD_MADRE;
                    }
                    arrayTipoAnagraficaModificati.push("Razza_Madre")
                }
                break;
            case "5":
                var MAT_PADRE = Txt_Matricola_Padre.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].MAT_PADRE = MAT_PADRE;
                }
                arrayTipoAnagraficaModificati.push("MAT_PADRE")
                break;
            case "6":
                var RAZ_DES_PADRE = Cmb_modifica_Razza_Padre.text();
                var RAZ_COD_PADRE = Cmb_modifica_Razza_Padre.value();
                if (RAZ_COD_PADRE == "") {
                    kendo.alert(TraduzioneMultiResx(resxObj, "SelezionareRazzaPadreCapoDaModificare", "Selezionare la razza padre del capo da modificare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Razza_Padre = RAZ_COD_PADRE;
                    }
                    arrayTipoAnagraficaModificati.push("Razza_Padre")
                }
                break;
            case "7":
                var cod_Sesso = Cmb_modifica_Sesso.value();
                var Sesso = Cmb_modifica_Sesso.text();
                if (Sesso == "") {
                    kendo.alert(TraduzioneMultiResx(resxObj, "SelezionareSessoCapoDaModificare", "Selezionare il sesso del capo da modificare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Sesso = Sesso;
                    }
                    arrayTipoAnagraficaModificati.push("Sesso")
                }
                break;
            case "8":
                var IPRO_COD = Cmb_modifica_Metodo_Produzione.value();
                var IPRO_DES = Cmb_modifica_Metodo_Produzione.text();
                if (IPRO_COD == "") {
                    kendo.alert(TraduzioneMultiResx(resxObj, "SelezionareMetodoProdCapoDaModificare", "Selezionare il metodo di produzione del capo da modificare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Metodo_Produzione = IPRO_COD;
                    }
                    arrayTipoAnagraficaModificati.push("Metodo_Produzione")
                }
                break;
            case "9":
                var CF_DETENTORE = Txt_modifica_CF_Detentore.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].CF_DETENTORE = CF_DETENTORE;
                }
                arrayTipoAnagraficaModificati.push("CF_DETENTORE")
                break;
            case "10":
                var CF_PROPRIETARIO = Txt_modifica_CF_Proprietario.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].CF_PROPRIETARIO = CF_PROPRIETARIO;
                }
                arrayTipoAnagraficaModificati.push("CF_PROPRIETARIO")
                break;
            case "11":
                var Certificato = Txt_modifica_Certificato.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Certificato = Certificato;
                }
                arrayTipoAnagraficaModificati.push("Certificato")
                break;
            case "12":
                var Modello4_Ingresso = Txt_Modello4_Ingresso.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Modello4_Ingresso_Numero = Modello4_Ingresso;
                }
                arrayTipoAnagraficaModificati.push("Modello4_Ingresso_Numero")
                break;
            case "13":
                var Modello4_Uscita = Txt_Modello4_Uscita.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Modello4_Uscita_Numero = Modello4_Uscita;
                }
                arrayTipoAnagraficaModificati.push("Modello4_Uscita_Numero")
                break;
            case "14":
                var Modello4_Ingresso_Prenotazione = Txt_Modello4_Ingresso_Prenotazione.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Modello4_Ingresso_Prenotazione = Modello4_Ingresso_Prenotazione;
                }
                arrayTipoAnagraficaModificati.push("Modello4_Ingresso_Prenotazione")
                break;
            case "15":
                var Modello4_Uscita_Prenotazione = Txt_Modello4_Uscita_Prenotazione.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Modello4_Uscita_Prenotazione = Modello4_Uscita_Prenotazione;
                }
                arrayTipoAnagraficaModificati.push("Modello4_Uscita_Prenotazione")
                break;
            case "16":
                var Lotto_Fornitore = Txt_Lotto_Fornitore.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Lotto_Fornitore = Lotto_Fornitore;
                }
                arrayTipoAnagraficaModificati.push("Lotto_Fornitore")
                break;
            case "17":
                var Lotto = Txt_Lotto.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Lotto = Lotto;
                }
                arrayTipoAnagraficaModificati.push("Lotto")
                break;
            case "18":
                var Data_Documento_Ingresso = Txt_Data_Documento_Ingresso.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Data_Documento_Ingresso = new Date(Data_Documento_Ingresso);
                }
                arrayTipoAnagraficaModificati.push("Data_Documento_Ingresso")
                break;
            case "19":
                var Data_Documento_Uscita = Txt_Data_Documento_Uscita.val();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Data_Documento_Uscita = new Date(Data_Documento_Uscita);
                }
                arrayTipoAnagraficaModificati.push("Data_Documento_Uscita")
                break;
            case "20":
                var Cod_Contatto = Cmb_Fornitore_Fatt.value();
                var Rag_Soc_Completa = Cmb_Fornitore_Fatt.text();

                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Cod_Contatto = Cod_Contatto;
                }
                arrayTipoAnagraficaModificati.push("Cod_Contatto")
                break;
            case "21":
                var cod_FornitoreProv = Cmb_Fornitore_Prov.value();

                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Fornitore_Provenienza = cod_FornitoreProv;
                }
                arrayTipoAnagraficaModificati.push("Fornitore_Provenienza")
                break;
            case "22":
                var ddtIngresso = Txt_N_Bolla_Fornitore.val();

                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].N_Bolla_Fornitore = ddtIngresso;
                }
                arrayTipoAnagraficaModificati.push("N_Bolla_Fornitore")
                break;
            case "23":
                var data_ddtIngresso = Txt_Data_DDT_Ingresso.val();

                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Data_DDT_Ingresso = new Date(data_ddtIngresso);
                }
                arrayTipoAnagraficaModificati.push("Data_DDT_Ingresso")
                break;
            case "24":
                var ddtUscita = Txt_N_Bolla_Uscita.val();

                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].N_Bolla_Uscita = ddtUscita;
                }
                arrayTipoAnagraficaModificati.push("N_Bolla_Uscita")
                break;
            case "25":
                var data_ddtUscita = Txt_Data_DDT_Uscita.val();

                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Data_DDT_Uscita = new Date(data_ddtUscita);
                }
                arrayTipoAnagraficaModificati.push("Data_DDT_Uscita")
                break;
            case "26":
                var codiceAzForn = Txt_Codice_Azienda_Fornitore.val();

                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Codice_Azienda_Fornitore = codiceAzForn;
                }
                arrayTipoAnagraficaModificati.push("Codice_Azienda_Fornitore")
                break;
            case "27":
                var cod_Validato = Cmb_modifica_Validato.value();
                var Validato = Cmb_modifica_Validato.text();
                if (Validato == "") {
                    kendo.alert(TraduzioneMultiResx(resxObj, "SelezionareValiditaCapoDaModificare", "Selezionare valore per la validità del capo da modificare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Validato = Validato;
                    }
                    arrayTipoAnagraficaModificati.push("Validato")
                }
                break;
            case "28":
                var codiceAzNasc = Txt_Codice_Azienda_Nascita.val();

                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].AUSL_AZI_NASCITA = codiceAzNasc;
                }
                arrayTipoAnagraficaModificati.push("AUSL_AZI_NASCITA")
                break;
            case "29":
                var cod_StallaSvezz = Cmb_Stalla_Svezz.value();

                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Stalla_Svezzamento = cod_StallaSvezz;
                }
                arrayTipoAnagraficaModificati.push("Stalla_Svezzamento")
                break;
            case "30":
                var incrementoTeorico = Txt_Incremento_Teorico.value();

                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Incremento_Teorico = incrementoTeorico;
                }
                arrayTipoAnagraficaModificati.push("Incremento_Teorico")
                break;
        }
    }

    // Vengono aggiunti i vari campi che sono stati modificati in questo oggetto, verrà utilizzato poi per la modifica in DB.
    obj_ModificaMultipla.anagrafica = arrayTipoAnagraficaModificati.filter(onlyUnique);
    var chiudi = await ModificaMultipla();
    if (chiudi == true) {
        await chiudiModificaMultipla();
    }
    return true;
}

function onlyUnique(value, index, self) {
    return self.indexOf(value) === index;
}

async function ModificaMultipla() {
    WaitFrame.show();
    result = await ws_salvataggioModificaMultipla();
    WaitFrame.hide();
    var chiudi = false;

    if (!result) {
        kendo.alert(TraduzioneMultiResx(resxObj, "ErroreDuranteModifica", "Errore durante la modifica."));
        chiudi = false;
    } else {
        chiudi = true
        //window.location.reload(); // Aggiorna la pagina.
        KendoModifica_Multipla_Zoo.destroy();
        popolaGriglia_ModificaMultipla("divKendoModifica_Multipla_Zoo");
        kendo.alert(TraduzioneMultiResx(resxObj, "ModificaEffettuataConSuccesso", "Modifica effettuata con successo."));
    }
    return chiudi
}

async function savePresetColumns() {

    let parametri = parametri_multipli ? Cmb_Parametri.value() : [Cmb_Parametri.value()];

    WaitFrame.show();
    result = await ws_savePresetColumns(parametri);
    WaitFrame.hide();

    if (!result) {
        kendo.alert(TraduzioneMultiResx(resxObj, "ErroreSalvataggioPresetColonneSuccesso", "Errore durante il salvataggio delle colonne da modificare di default."));
    } else {
        presetColumnsToUpdate = kendo.stringify(parametri);
        kendo.alert(TraduzioneMultiResx(resxObj, "SalvataggioPresetColonneSuccesso", "Salvataggio delle colonne da modificare di default avvenuto con successo."));
    }
}

async function chiudiModificaMultipla() {
    await pulisciControlli()
    $("#winModificaMultipla").data("kendoWindow").close();
    //window.location.reload(); // Aggiorna la pagina.
    KendoModifica_Multipla_Zoo.destroy();
    popolaGriglia_ModificaMultipla("divKendoModifica_Multipla_Zoo");
}

function pulisciControlli() {
    warningCounter = false;
    obj_ModificaMultipla.anagrafica = [];
    arrayTipoAnagraficaModificati = [];
    if ($("#Txt_Matricola_Madre") !== undefined)
        $("#Txt_Matricola_Madre").val(null);

    if ($("#Txt_Matricola_Padre") !== undefined)
        $("#Txt_Matricola_Padre").val(null);

    if ($("#Txt_modifica_CF_Detentore") !== undefined)
        $("#Txt_modifica_CF_Detentore").val(null);

    if ($("#Txt_modifica_CF_Proprietario") !== undefined)
        $("#Txt_modifica_CF_Proprietario").val(null);

    if ($("#Txt_modifica_Certificato") !== undefined)
        $("#Txt_modifica_Certificato").val(null);

    if ($("#Txt_modifica_Modello4_Ingresso") !== undefined)
        $("#Txt_modifica_Modello4_Ingresso").val(null);

    if ($("#Txt_modifica_Modello4_Uscita") !== undefined)
        $("#Txt_modifica_Modello4_Uscita").val(null);

    if ($("#Txt_Modello4_Ingresso_Prenotazione") !== undefined)
        $("#Txt_Modello4_Ingresso_Prenotazione").val(null);

    if ($("#Txt_Modello4_Uscita_Prenotazione") !== undefined)
        $("#Txt_Modello4_Uscita_Prenotazione").val(null);

    if ($("#Txt_Lotto_Fornitore") !== undefined)
        $("#Txt_Lotto_Fornitore").val(null);

    if ($("#Txt_Lotto") !== undefined)
        $("#Txt_Lotto").val(null);

    if ($("#Txt_Data_Documento_Ingresso") !== undefined)
        $("#Txt_Data_Documento_Ingresso").val(null);

    if ($("#Txt_Data_Documento_Uscita") !== undefined)
        $("#Txt_Data_Documento_Uscita").val(null);

    if ($("#Txt_Codice_Azienda_Fornitore") !== undefined)
        $("#Txt_Codice_Azienda_Fornitore").val(null);

    if ($("#Txt_N_Bolla_Fornitore") !== undefined)
        $("#Txt_N_Bolla_Fornitore").val(null);

    if ($("#Txt_Data_DDT_Ingresso") !== undefined)
        $("#Txt_Data_DDT_Ingresso").val(null);

    if ($("#Txt_N_Bolla_Uscita") !== undefined)
        $("#Txt_N_Bolla_Uscita").val(null);

    if ($("#Txt_Data_DDT_Uscita") !== undefined)
        $("#Txt_Data_DDT_Uscita").val(null);

    if ($("#Txt_Codice_Azienda_Nascita") !== undefined)
        $("#Txt_Codice_Azienda_Nascita").val(null);

    if ($("#Txt_Incremento_Teorico").data("kendoNumericTextBox") !== undefined)
        $("#Txt_Incremento_Teorico").data("kendoNumericTextBox").value(null);

    if ($("#Cmb_Modifica_Razza_Capo").data("kendoDropDownList") !== undefined)
        $("#Cmb_Modifica_Razza_Capo").data("kendoDropDownList").value(null);

    if ($("#Cmb_modifica_Razza_Madre").data("kendoDropDownList") !== undefined)
        $("#Cmb_modifica_Razza_Madre").data("kendoDropDownList").value(null);

    if ($("#Cmb_modifica_Razza_Padre").data("kendoDropDownList") !== undefined)
        $("#Cmb_modifica_Razza_Padre").data("kendoDropDownList").value(null);

    if ($("#Cmb_modifica_Sesso").data("kendoDropDownList") !== undefined)
        $("#Cmb_modifica_Sesso").data("kendoDropDownList").value(null);

    if ($("#Cmb_modifica_Metodo_Produzione").data("kendoDropDownList") !== undefined)
        $("#Cmb_modifica_Metodo_Produzione").data("kendoDropDownList").value(null);

    if ($("#Cmb_Mod_Disciplinare").data("kendoDropDownList") !== undefined)
        $("#Cmb_Mod_Disciplinare").data("kendoDropDownList").value(null);

    if ($("#Cmb_Fornitore_Fatt").data("kendoDropDownList") !== undefined)
        $("#Cmb_Fornitore_Fatt").data("kendoDropDownList").value(null);

    if ($("#Cmb_Fornitore_Prov").data("kendoDropDownList") !== undefined)
        $("#Cmb_Fornitore_Prov").data("kendoDropDownList").value(null);

    if ($("#Cmb_modifica_Validato").data("kendoDropDownList") !== undefined)
        $("#Cmb_modifica_Validato").data("kendoDropDownList").value(null);

    if ($("#Cmb_Stalla_Svezz").data("kendoDropDownList") !== undefined)
        $("#Cmb_Stalla_Svezz").data("kendoDropDownList").value(null);
}

//-------------------------------------------------------------
//---------------------------COMBOS----------------------------
//-------------------------------------------------------------

function CambiaParametri(parametri) {

    // Razza Capo
    if (parametri.includes("1")) {
        $("#modifica_Razza_Capo").show();
        modifica_Razza_Capo();
    } else {
        $("#modifica_Razza_Capo").hide();
    }

    // Categoria Capo
    //if (parametri.includes("2")) {
    //    $("#modifica_Categoria_Capo").show();
    //    modifica_Categoria_Capo();
    //} else {
    //    $("#modifica_Categoria_Capo").hide();
    //}

    // Matricola Madre
    if (parametri.includes("3")) {
        $("#modifica_Matricola_Madre").show();
        Txt_Matricola_Madre = $("#Txt_Matricola_Madre");
    } else {
        $("#modifica_Matricola_Madre").hide();
    }

    // Razza Madre
    if (parametri.includes("4")) {
        $("#modifica_Razza_Madre").show();
        modifica_Razza_Madre();
    } else {
        $("#modifica_Razza_Madre").hide();
    }

    // Matricola Padre
    if (parametri.includes("5")) {
        $("#modifica_Matricola_Padre").show();
        Txt_Matricola_Padre = $("#Txt_Matricola_Padre");
    } else {
        $("#modifica_Matricola_Padre").hide();
    }

    // Razza Padre
    if (parametri.includes("6")) {
        $("#modifica_Razza_Padre").show();
        modifica_Razza_Padre();
    } else {
        $("#modifica_Razza_Padre").hide();
    }

    // Sesso
    if (parametri.includes("7")) {
        $("#modifica_Sesso").show();
        modifica_Sesso();
    } else {
        $("#modifica_Sesso").hide();
    }

    // Metodo Produzione
    if (parametri.includes("8")) {
        $("#modifica_Metodo_Produzione").show();
        modifica_Metodo_Produzione();
    } else {
        $("#modifica_Metodo_Produzione").hide();
    }

    // CF Detentore
    if (parametri.includes("9")) {
        $("#modifica_CF_Detentore").show();
        Txt_modifica_CF_Detentore = $("#Txt_modifica_CF_Detentore");
    } else {
        $("#modifica_CF_Detentore").hide();
    }

    // CF Proprietario
    if (parametri.includes("10")) {
        $("#modifica_CF_Proprietario").show();
        Txt_modifica_CF_Proprietario = $("#Txt_modifica_CF_Proprietario");
    } else {
        $("#modifica_CF_Proprietario").hide();
    }

    // Certificato
    if (parametri.includes("11")) {
        $("#modifica_Certificato").show();
        Txt_modifica_Certificato = $("#Txt_modifica_Certificato");
    } else {
        $("#modifica_Certificato").hide();
    }

    // Modello4_Ingresso
    if (parametri.includes("12")) {
        $("#modifica_Modello4_Ingresso").show();
        Txt_Modello4_Ingresso = $("#Txt_modifica_Modello4_Ingresso");
    } else {
        $("#modifica_Modello4_Ingresso").hide();
    }

    // Modello4_Uscita
    if (parametri.includes("13")) {
        $("#modifica_Modello4_Uscita").show();
        Txt_Modello4_Uscita = $("#Txt_modifica_Modello4_Uscita");
    } else {
        $("#modifica_Modello4_Uscita").hide();
    }

    // Modello4_Ingresso_Prenotazione
    if (parametri.includes("14")) {
        $("#Modello4_Ingresso_Prenotazione").show();
        Txt_Modello4_Ingresso_Prenotazione = $("#Txt_Modello4_Ingresso_Prenotazione");
    } else {
        $("#Modello4_Ingresso_Prenotazione").hide();
    }

    // Modello4_Uscita_Prenotazione
    if (parametri.includes("15")) {
        $("#Modello4_Uscita_Prenotazione").show();
        Txt_Modello4_Uscita_Prenotazione = $("#Txt_Modello4_Uscita_Prenotazione");
    } else {
        $("#Modello4_Uscita_Prenotazione").hide();
    }

    // Lotto_Fornitore
    if (parametri.includes("16")) {
        $("#Lotto_Fornitore").show();
        Txt_Lotto_Fornitore = $("#Txt_Lotto_Fornitore");
    } else {
        $("#Lotto_Fornitore").hide();
    }

    // Lotto
    if (parametri.includes("17")) {
        $("#Lotto").show();
        Txt_Lotto = $("#Txt_Lotto");
    } else {
        $("#Lotto").hide();
    }

    // Lotto
    if (parametri.includes("18")) {
        $("#Data_Documento_Ingresso").show();
        Txt_Data_Documento_Ingresso = $("#Txt_Data_Documento_Ingresso");
    } else {
        $("#Data_Documento_Ingresso").hide();
    }

    // Lotto
    if (parametri.includes("19")) {
        $("#Data_Documento_Uscita").show();
        Txt_Data_Documento_Uscita = $("#Txt_Data_Documento_Uscita");
    } else {
        $("#Data_Documento_Uscita").hide();
    }

    // Fornitore Fatturazione
    if (parametri.includes("20")) {
        $("#modifica_FornitoreFatt").show();
        modifica_FornitoreFatt();
    } else {
        $("#modifica_FornitoreFatt").hide();
    }

    // Fornitore Provenienza
    if (parametri.includes("21")) {
        $("#modifica_FornitoreProv").show();
        modifica_FornitoreProv();
    } else {
        $("#modifica_FornitoreProv").hide();
    }

    // Numero DDT Ingresso
    if (parametri.includes("22")) {
        $("#modifica_ddt_ingresso").show();
        Txt_N_Bolla_Fornitore = $("#Txt_N_Bolla_Fornitore");
    } else {
        $("#modifica_ddt_ingresso").hide();
    }

    // Data DDT Ingresso
    if (parametri.includes("23")) {
        $("#modifica_data_ddt_ingresso").show();
        Txt_Data_DDT_Ingresso = $("#Txt_Data_DDT_Ingresso");
    } else {
        $("#modifica_data_ddt_ingresso").hide();
    }

    // Numero DDT Uscita
    if (parametri.includes("24")) {
        $("#modifica_ddt_uscita").show();
        Txt_N_Bolla_Uscita = $("#Txt_N_Bolla_Uscita");
    } else {
        $("#modifica_ddt_uscita").hide();
    }

    // Data DDT Uscita
    if (parametri.includes("25")) {
        $("#modifica_data_ddt_uscita").show();
        Txt_Data_DDT_Uscita = $("#Txt_Data_DDT_Uscita");
    } else {
        $("#modifica_data_ddt_uscita").hide();
    }

    // Codice Azienda Fornitore
    if (parametri.includes("26")) {
        $("#modifica_codice_azienda_prov").show();
        Txt_Codice_Azienda_Fornitore = $("#Txt_Codice_Azienda_Fornitore");
    } else {
        $("#modifica_codice_azienda_prov").hide();
    }

    // Validato
    if (parametri.includes("27")) {
        $("#modifica_Validato").show();
        modifica_Validato();
    } else {
        $("#modifica_Validato").hide();
    }

    // Codice Azienda Nascita
    if (parametri.includes("28")) {
        $("#modifica_codice_azienda_nasc").show();
        Txt_Codice_Azienda_Nascita = $("#Txt_Codice_Azienda_Nascita");
    } else {
        $("#modifica_codice_azienda_nasc").hide();
    }

    // Stalla Svezzamento
    if (parametri.includes("29")) {
        $("#modifica_StallaSvezz").show();
        modifica_StallaSvezz();
    } else {
        $("#modifica_StallaSvezz").hide();
    }

    // Incremento Teorico
    if (parametri.includes("30")) {
        $("#modifica_Incremento_Teorico").show();
        if ($("#Txt_Incremento_Teorico").data("kendoNumericTextBox") == undefined) {
            Txt_Incremento_Teorico = $("#Txt_Incremento_Teorico").kendoNumericTextBox({
                format: "#.## \\kg",
                decimals: 2,
                min: 0,
                spinners: false
            }).data("kendoNumericTextBox");
        }
    } else {
        $("#modifica_Incremento_Teorico").hide();
    }

  
}

//-------------------------------------------------------------
//----------------------FUNZIONI-MODIFICA----------------------
//-------------------------------------------------------------

function modifica_Razza_Capo() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Modifica_Razza_Capo = $("#Cmb_Modifica_Razza_Capo").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataSource: { transport: { read: getRazze } },
            dataTextField: "RAZ_DES",
            dataValueField: "RAZ_COD",
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                resolve(this);
            },
            index: 0
        }).data("kendoDropDownList");
    });
}

// Per eventuale implementazione categoria capo
//function modifica_Categoria_Capo() {
//    return new Promise((resolve, reject) => {
//
//        let data = [
//            { Raz_Des: TraduzioneMultiResx(resxObj, "test1", "test1"), Raz_Cod: 1 },
//            { Raz_Des: TraduzioneMultiResx(resxObj, "test2", "test2"), Raz_Cod: 2 },
//            { Raz_Des: TraduzioneMultiResx(resxObj, "test3", "test3"), Raz_Cod: 3 }
//        ];
//
//        let onLoad = true;
//        Cmb_modifica_Categoria_Capo = $("#Cmb_modifica_Categoria_Capo").kendoDropDownList({
//            filter: "contains",
//            autoBind: true,
//            dataTextField: "Raz_Des",
//            dataValueField: "Raz_Cod",
//            dataSource: data,
//            open: kendoDropDownAdjustWidth,
//            dataBound: function (e) {
//                kendoDropDownAdjustWidth(e);
//                if (onLoad) {
//                    this.value("");
//                    this.trigger("change");
//                    onLoad = false;
//                }
//            },
//            change: function (e) {
//                resolve(this);
//            },
//            optionLabel: TraduzioneMultiResx(resxObj, "Seleziona", "Seleziona").toUpperCase()
//        }).data("kendoDropDownList");
//    });
//}

function modifica_Razza_Madre() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_modifica_Razza_Madre = $("#Cmb_modifica_Razza_Madre").kendoDropDownList({
            filter: "contains",
            autoBind: false,
            dataSource: { transport: { read: getRazze } },
            dataTextField: "RAZ_DES",
            dataValueField: "RAZ_COD",
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                resolve(this);
            },
            index: 0
        }).data("kendoDropDownList");
    });
}

function modifica_Razza_Padre() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_modifica_Razza_Padre = $("#Cmb_modifica_Razza_Padre").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataSource: { transport: { read: getRazze } },
            dataTextField: "RAZ_DES",
            dataValueField: "RAZ_COD",
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                resolve(this);
            },
            index: 0
        }).data("kendoDropDownList");
    });
}

function modifica_Sesso() {
    return new Promise((resolve, reject) => {

        let data = [
            { Sesso: TraduzioneMultiResx(resxObj, "MaschioSigla", "M"), cod_Sesso: 0 },
            { Sesso: TraduzioneMultiResx(resxObj, "FemminaSigla", "F"), cod_Sesso: 1 }
        ];

        let onLoad = true;
        Cmb_modifica_Sesso = $("#Cmb_modifica_Sesso").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Sesso",
            dataValueField: "cod_Sesso",
            dataSource: data,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                resolve(this);
            },
            index: 0
        }).data("kendoDropDownList");
    });
}

function modifica_Metodo_Produzione() {
    return new Promise((resolve, reject) => {

        let data = [
            { IPRO_DES: TraduzioneMultiResx(resxObj, "Convenzionale", "Convenzionale"), IPRO_COD: 1 },
            { IPRO_DES: TraduzioneMultiResx(resxObj, "InConversione", "In Conversione"), IPRO_COD: 2 },
            { IPRO_DES: TraduzioneMultiResx(resxObj, "Biologico", "Biologico"), IPRO_COD: 3 }
        ];

        let onLoad = true;
        Cmb_modifica_Metodo_Produzione = $("#Cmb_modifica_Metodo_Produzione").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "IPRO_DES",
            dataValueField: "IPRO_COD",
            dataSource: data,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                resolve(this);
            },
            index: 0
        }).data("kendoDropDownList");
    });
}

function modifica_FornitoreFatt() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Fornitore_Fatt = $("#Cmb_Fornitore_Fatt").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataSource: { transport: { read: CaricaComboCmb_Fornitore } },
            dataTextField: "Rag_Soc_Completa",
            dataValueField: "Cod_Contatto",
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                resolve(this);
            },
            index: 0
        }).data("kendoDropDownList");
    });
}

function modifica_FornitoreProv() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Fornitore_Prov = $("#Cmb_Fornitore_Prov").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataSource: { transport: { read: CaricaComboCmb_Fornitore } },
            dataTextField: "Rag_Soc_Completa",
            dataValueField: "Cod_Contatto",
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                resolve(this);
            },
            index: 0
        }).data("kendoDropDownList");
    });
}

function modifica_StallaSvezz() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Stalla_Svezz = $("#Cmb_Stalla_Svezz").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataSource: { transport: { read: CaricaComboCmb_Fornitore } },
            dataTextField: "Rag_Soc_Completa",
            dataValueField: "Cod_Contatto",
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                resolve(this);
            },
            index: 0
        }).data("kendoDropDownList");
    });
}

function modifica_Validato() {
    return new Promise((resolve, reject) => {

        let data = [
            { Validato: TraduzioneMultiResx(resxObj, "Si", "Si"), cod_Validato: 1 },
            { Validato: TraduzioneMultiResx(resxObj, "No", "No"), cod_Validato: 0 }
        ];

        let onLoad = true;
        Cmb_modifica_Validato = $("#Cmb_modifica_Validato").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Validato",
            dataValueField: "cod_Validato",
            dataSource: data,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                resolve(this);
            },
            index: 0
        }).data("kendoDropDownList");
    });
}


//-------------------------------------------------------------
//-----------------------------WS------------------------------
//-------------------------------------------------------------

function getRazze(options) {
    ws_getRazze(function (r) {
        let p = JSON.parse(r.RispostaStringa);
        options.success(p);
    }, stessa_specie_cod[0], stessa_specie_cod[1]);
}

//function CaricaComboCmbFornitore(options) {
//    CaricaComboCmb_Fornitore(function (r) {
//        let p = JSON.parse(r.RispostaStringa);
//        options.success(p);
//    });
//}