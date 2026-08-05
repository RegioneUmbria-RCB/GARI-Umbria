// ############################################################################
// #####################    G R I G L I A    K E N D O     ####################
// ############################################################################

function chiama_popola_griglia(esitoRicerca) {

    esitoRicerca = esitoRicerca == "" || esitoRicerca == null ? $('#' + idKendoModifica_Multipla_PianoColturale).val() : esitoRicerca;

    $('#' + idKendoModifica_Multipla_PianoColturale).val(esitoRicerca);

    if (esitoRicerca != "") popolaGriglia_ModificaMultipla("divKendoModifica_Multipla_PianoColturale");
}

function popolaGriglia_ModificaMultipla(IDControllo) {

    var idModel = "chiave";
    var funzioniCRUD = {};
    var funzioniPrimaDopoEventi = {};

    var campiKendoModel = kReadValorizzazione_mod_ModificaMultipla();
    var colonneKendoGrid = kReadValorizzazione_col_ModificaMultipla();

    //Controllo se c'è la colonna di selezione
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

    if (ischeckable === true) {
        funzioniCRUD = { funzioneRead: kReadValorizzazione_rows_ModificaMultipla, checkBoxFunction: KendoModificaMultipla_checked };

        //le devo mettere per forza, altrimenti non riesco ad usare i check
        funzioniPrimaDopoEventi = { funzioneDaChiamareDopoSelectAllRows: function () { }, funzioneDaChiamareDopoDataBound: function () { } };
    } else {
        funzioniCRUD = { funzioneRead: kReadValorizzazione_rows_ModificaMultipla };
        funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBounding_ModificaMultipla };
    }

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD, //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga
        campiKendoModel, // campi modello
        colonneKendoGrid, // colonne da mostrare
        parametriPerLettura, // parametri da passare alla lettura
        parametriDataSource, // parametri data source { chiave - valore}
        parametriKendoGrid, // parametri griglia [{ chiave - valore}]
        funzioniPrimaDopoEventi, // funzioni da chiamare all'inizio e alla fine dei vari eventi
        mostraRigheCancellate, // se true le righe cancellate vengono mostrate barrate e viene gestita funzione custom cancellazione
        colonneDisabilitateSoloInModifica // colonne non modificabili in modifica["colA", "colB", ...]
    );

    CreaToolBarImpianti();
}

function onDataBounding_ModificaMultipla(e) {
    var gridId = e.sender.element[0].id;
    kendo_AggiustaDimensioneColonne("#" + gridId);
}

function kReadValorizzazione_rows_ModificaMultipla(options) {
    var data = $('#' + idKendoModifica_Multipla_PianoColturale).val();
    jSonParsed_Kendo = JSON.parse(data);

    if (ischeckable) {
        for (let i = 0; i < jSonParsed_Kendo.kendo_rows.length; i++) {
            jSonParsed_Kendo.kendo_rows[i].Selected = true;
        }
    }

    options.success(jSonParsed_Kendo.kendo_rows);
}

function kReadValorizzazione_col_ModificaMultipla() {
    var data = $('#' + idKendoModifica_Multipla_PianoColturale).val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_columns;
}

function kReadValorizzazione_mod_ModificaMultipla() {
    var data = $('#' + idKendoModifica_Multipla_PianoColturale).val();
    jSonParsed_Kendo = JSON.parse(data);

    return jSonParsed_Kendo.kendo_model;
}

function KendoModificaMultipla_checked(e) {
    var checked = this.checked;
    var row = $(this).parents("tr");
    var grid = $('#divKendoModifica_Multipla_PianoColturale').data("kendoGrid");
    var dataItem = grid.dataItem(row);

    dataItem.Selected = checked;
    dataItem.dirty = true;

    rowKendoGridSelected(row, checked)

    //le righe selezionate possono avere pive diverse, devo ricontrollare
    findPiva = true
}



// ############################################################################
// #####################     B U T T O N     K E N D O     ####################
// ############################################################################

function CreaToolBarImpianti() {

    var gridTB = $("#divKendoModifica_Multipla_PianoColturale").find(".k-grid-toolbar");

    if (permesso_modificaMultipla) {
        let titleModMultipla = Traduzione(menuBSAnagraficaResx, 'MenuBS_Anagrafica_modificaMultiplaPiano', 'Modifica Multipla Piano');
        let titleGestioneEsercizi = Traduzione(menuBSAnagraficaResx, 'ChiusuraAperturaEsercizi', 'Chiusura/Apertura Esercizi');
        let titleModificaResa = Traduzione(menuBSAnagraficaResx, 'ModificaResaPrevista', 'Modifica resa prevista');

        if (GiasVersioneMaster === "2022") {
            gridTB.append('<div id="btn_ModificaMultipla_PianoColturale" class="k-button k-button-icontext k-grid--button" data-title="' + titleModMultipla + '" title="' + titleModMultipla + '"><i class="k-icon fa fa-pencil"></i></div>');
            gridTB.append('<div id="btn_GestioneEsercizi" class="k-button k-button-icontext k-grid--button" data-title="' + titleGestioneEsercizi + '" title="' + titleGestioneEsercizi + '"><i class="k-icon k-i-list-unordered"></i></div>');
            gridTB.append('<div id="btn_ModificaResa" class="k-button k-button-icontext k-grid--button" data-title="' + titleModificaResa + '" title="' + titleModificaResa + '"><i class="k-icon fa fa-pagelines"></i></div>');
        } else {
            gridTB.append('<div id="btn_ModificaMultipla_PianoColturale" class="k-button k-button-icontext"><i class="k-icon fa fa-pencil"></i>' + titleModMultipla + '</div>');
            gridTB.append('<div id="btn_GestioneEsercizi" class="k-button k-button-icontext" ><i class="k-icon k-i-list-unordered"></i>' + titleGestioneEsercizi + '</div>');
            gridTB.append('<div id="btn_ModificaResa" class="k-button k-button-icontext" ><i class="k-icon fa fa-pagelines"></i>' + titleModificaResa + '</div>');
        }
    }

    $("#btn_ModificaMultipla_PianoColturale").click(function () {
        ModificaMultiplaImpianti(1);
    });
    $("#btn_GestioneEsercizi").click(function () {
        ModificaMultiplaImpianti(2);
    });
    $("#btn_ModificaResa").click(function () {
        editResaPrevista();
    });
}

function getSelectedAdd() {
    var grid = $("#divKendoModifica_Multipla_PianoColturale").data("kendoGrid");
    var data = grid.dataSource.data();
    var selectedAdd = new Array();

    var veg_cod = "";
    var grfi_cod = "";
    var cul_cod = "";
    var veg_cod_uguale = true;
    var grfi_cod_uguale = true;
    var cul_cod_uguale = true;

    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected && data[i].Blk_Flag != -1) {
            if (veg_cod == "") {
                veg_cod = data[i].veg_cod;
            }
            if (grfi_cod == "") {
                grfi_cod = data[i].grfi_cod;
            }
            if (veg_cod != data[i].veg_cod) {
                veg_cod_uguale = false;
            }
            if (grfi_cod != data[i].grfi_cod) {
                grfi_cod_uguale = false;
            }
            selectedAdd.push(data[i]);
        }
    }

    return selectedAdd;
}

function ModificaMultiplaImpianti(chiamante) { //chiamante: 1 = modifica multipla; 2 = gestione esercizi

    if (chiamante == 1) {
        creaModificaMultipla(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_modificaImpiantiSelezionati", "Modifica impianti selezionati"));
    } else if (chiamante == 2) {
        creaGestioneEsercizi(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_modificaImpiantiSelezionati", "Modifica impianti selezionati"));
    }

    var grid = $("#divKendoModifica_Multipla_PianoColturale").data("kendoGrid");
    var data = grid.dataSource.data();

    selected_add = new Array();

    var veg_cod = "";
    var grfi_cod = "";
    var cul_cod = "";
    var veg_cod_uguale = true;
    var grfi_cod_uguale = true;
    var cul_cod_uguale = true;

    for (let i = 0; i < data.length; i++) {
        if (data[i].Selected && data[i].Blk_Flag != -1) {
            if (veg_cod == "") {
                veg_cod = data[i].veg_cod;
            }
            if (grfi_cod == "") {
                grfi_cod = data[i].grfi_cod;
            }
            if (cul_cod == "") {
                cul_cod = data[i].cul_cod;
            }
            if (veg_cod != data[i].veg_cod) {
                veg_cod_uguale = false;
            }
            if (grfi_cod != data[i].grfi_cod) {
                grfi_cod_uguale = false;
            }
            if (cul_cod != data[i].cul_cod) {
                cul_cod_uguale = false;
            }
            selected_add.push(data[i]);
        }
    }

    if (selected_add.length > 0) {
        obj_ModificaMultipla = {};

        if (veg_cod_uguale) {
            obj_ModificaMultipla.veg_cod = selected_add[0].veg_cod;
            obj_ModificaMultipla.validita_inizio = selected_add[0].validita_inizio;
        }
        if (veg_cod_uguale && grfi_cod_uguale) {
            obj_ModificaMultipla.grfi_cod = selected_add[0].grfi_cod;
        }
        if (veg_cod_uguale && cul_cod_uguale) {
            obj_ModificaMultipla.cul_cod = selected_add[0].cul_cod;
        }
        if (chiamante == 1) {
            win_ModificaMultipla.open();
        } else if (chiamante == 2) {
            win_GestioneEsercizi.open();
        }

    } else {
        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_SelezionareImpiantiNonBloccati", "Selezionare almeno un impianto non bloccato."));
    }
}


// ###################################################################################
// #####################     M O D I F I C A   M U L T I P L A    ####################
// ###################################################################################

function creaModificaMultipla(title) {

    win_ModificaMultipla = $("#winModificaMultipla").kendoWindow({
        width: "640px",
        height: "80%",
        modal: true,
        title: Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_modificaMultipla", "Modifica Multipla"),
        closable: true,
        visible: false,
        resizable: true,
        open: async function (e) {
            this.center();
            await apriModificaMultipla();
        },
        close: function (e) {
            pulisciControlli()
        }
    }).data("kendoWindow");

    win_ModificaMultipla.title(title);
}

function apriModificaMultipla() {

    let data = new Array();
    parametri_multipli = true; // ereditatore == "2";

    $("#avvertimentoModificaMultipla").text("");
    $("#avvertimentoAppBloccatiModificaMultipla").text("");
    $("#avvertimentoEserciziChiusiModificaMultipla").text("");

    data.push({ des: Traduzione(menuBSAnagraficaResx, "MetodoProduzione", "Metodo Produzione"), value: "5", raggruppamento: "  APPEZZAMENTI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "ChiusuraAppezzamento", "Chiusura Appezzamento"), value: "27", raggruppamento: "  APPEZZAMENTI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "AppBioCod", "Cod. Biologico App"), value: "29", raggruppamento: "  APPEZZAMENTI" });

    let stessa_specie = obj_ModificaMultipla.veg_cod !== undefined && obj_ModificaMultipla.veg_cod != null && obj_ModificaMultipla.veg_cod != 0;
    let stessa_finalita = stessa_specie && obj_ModificaMultipla.grfi_cod !== undefined;
    let stessa_varieta = stessa_specie && obj_ModificaMultipla.cul_cod !== undefined;
    let AppBloccati = $('#divKendoModifica_Multipla_PianoColturale').data("kendoGrid").dataSource.data().filter(function (i) { return i.Blk_Flag == -1 && i.Selected }).length > 0
    let EsercizioChiuso = $('#divKendoModifica_Multipla_PianoColturale').data("kendoGrid").dataSource.data().filter(function (i) { return i.EsercizioChiusoCod == 1 && i.Selected }).length > 0
    let avvertimento = false;

    if (AppBloccati) {
        $("#avvertimentoAppBloccatiModificaMultipla").text(Traduzione(menuBSAnagraficaResx,
            "MenuBS_Anagrafica_ImpiantiBloccati",
            "ATTENZIONE: Le modifiche non verranno applicate agli impianti selezionati che risultano bloccati"
        ));
    }

    if (EsercizioChiuso) {
        $("#avvertimentoEserciziChiusiModificaMultipla").text(Traduzione(menuBSAnagraficaResx,
            "MenuBS_Anagrafica_EserciziChiusi",
            "ATTENZIONE: Le modifiche non verranno applicate agli esercizi selezionati che risultano chiusi"
        ));
    }

    if (stessa_specie) {
        data.push({ des: Traduzione(menuBSAnagraficaResx, "Finalità", "Finalità"), value: "1", raggruppamento: " IMPIANTI" });
        data.push({ des: Traduzione(menuBSAnagraficaResx, "Varietà", "Varietà"), value: "3", raggruppamento: " IMPIANTI" });
        data.push({ des: Traduzione(menuBSAnagraficaResx, "GruppoVarietale", "Gruppo Varietale"), value: "4", raggruppamento: " IMPIANTI" });
        data.push({ des: Traduzione(menuBSAnagraficaResx, "Copertura", "Copertura"), value: "6", raggruppamento: " IMPIANTI" });
        data.push({ des: Traduzione(menuBSAnagraficaResx, "FormaAllevamento", "Forma Allevamento"), value: "24", raggruppamento: " IMPIANTI" });
        data.push({ des: Traduzione(menuBSAnagraficaResx, "Portinnesto", "Portinnesto"), value: "25", raggruppamento: " IMPIANTI" });
        data.push({ des: Traduzione(menuBSAnagraficaResx, "DataInizioPortinnesto", "Messa a dimora Portinnesto"), value: "26", raggruppamento: " IMPIANTI" });
    } else if (obj_ModificaMultipla.veg_cod === undefined) {
        avvertimento = true;

        $("#avvertimentoModificaMultipla").text(Traduzione(menuBSAnagraficaResx,
            "MenuBS_Anagrafica_parametriNonSelezionabiliImpiantiDiversoUtilizzo",
            "ATTENZIONE: Alcuni parametri non sono modificabili in quanto sono stati selezionati impianti con utilizzi (specie vegetali) diversi."
        ));
    }

    data.push({ des: Traduzione(menuBSAnagraficaResx, "ChiusuraImpianto", "Chiusura Impianto"), value: "28", raggruppamento: " IMPIANTI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "ImpiantoIrrigazione", "Impianto Irrigazione"), value: "19", raggruppamento: " IMPIANTI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "SuFila", "Distanza Su Fila [m]"), value: "22", raggruppamento: " IMPIANTI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "TraFila", "Distanza Tra Fila [m]"), value: "23", raggruppamento: " IMPIANTI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "DataInizioImpianto", "Data Inizio Impianto"), value: "40", raggruppamento: " IMPIANTI" });


    data.push({ des: Traduzione(menuBSAnagraficaResx, "ResaPrevista", "Resa Prevista"), value: "7", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "DataSeminaPrevista", "Data Semina Prevista"), value: "8", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "DataRaccoltaPrevista", "Data Raccolta Prevista"), value: "9", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "DataFioritura", "Data Fioritura Prevista"), value: "10", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "CapitolatoPrivato", "Capitolato Privato"), value: "12", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "Certificazione", "Certificazione"), value: "15", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "OrganismoReferente", "Organismo Referente"), value: "13", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "MagazzinoConferimento", "Magazzino Conferimento"), value: "14", raggruppamento: "ESERCIZI" });

    if (stessa_finalita) {
        data.push({ des: Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_disciplinareMassimaliNPK", "Disciplinare - Massimali NPK"), value: "2", raggruppamento: "ESERCIZI" });
    } else if (!avvertimento && obj_ModificaMultipla.grfi_cod === undefined) {
        $("#avvertimentoModificaMultipla").text(Traduzione(menuBSAnagraficaResx,
            "MenuBS_Anagrafica_parametriNonSelezionabiliImpiantiDiversaFinalità",
            "ATTENZIONE: Alcuni parametri non sono modificabili in quanto sono stati selezionati impianti con finalità diverse."
        ));
    }

    data.push({ des: Traduzione(menuBSAnagraficaResx, "Regolamento", "Regolamento"), value: "20", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "Disciplinare", "Disciplinare"), value: "21", raggruppamento: "ESERCIZI" });

    data.push({ des: "N (kg/ha)", value: "16", raggruppamento: "ESERCIZI" });
    data.push({ des: "P2O5 (kg/ha)", value: "17", raggruppamento: "ESERCIZI" });
    data.push({ des: "K2O (kg/ha)", value: "18", raggruppamento: "ESERCIZI" });

    data.push({ des: Traduzione(menuBSAnagraficaResx, "SecondoRaccolto", "Secondo Raccolto"), value: "30", raggruppamento: "ESERCIZI" });

    data.push({ des: Traduzione(menuBSAnagraficaResx, "CertificazioneAziendale", "Certificazione Aziendale"), value: "31", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "Contributi", "Contributi"), value: "32", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "CertificazioneProdotto", "Certificazione Prodotto"), value: "33", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "Residuo", "Residuo"), value: "34", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "LicenzaColtivazione", "Licenza Coltivazione"), value: "35", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "RiferimentoTrasferimentoDati", "Riferimento Trasferimento Dati"), value: "36", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "Tecnico", "Tecnico"), value: "37", raggruppamento: "ESERCIZI" });
    data.push({ des: Traduzione(menuBSAnagraficaResx, "PianoSemina", "Piano Semina"), value: "38", raggruppamento: "ESERCIZI" });

    if (stessa_varieta) {
        if (findPiva == true) getPiva()
        if (piva == "")
            $("#avvertimentoModificaMultipla").text(Traduzione(menuBSAnagraficaResx,
                "MenuBS_Anagrafica_parametriNonSelezionabiliImpiantiDiversaImpresa",
                "ATTENZIONE: Alcuni parametri non sono modificabili in quanto sono stati selezionati impianti di imprese diverse."
            ));
        else
            data.push({ des: Traduzione(menuBSAnagraficaResx, "Prodotto", "Prodotto"), value: "39", raggruppamento: "ESERCIZI" });

    } else if (!avvertimento && obj_ModificaMultipla.cul_cod === undefined) {
        $("#avvertimentoModificaMultipla").text(Traduzione(menuBSAnagraficaResx,
            "MenuBS_Anagrafica_parametriNonSelezionabiliImpiantiDiversaVarieta",
            "ATTENZIONE: Alcuni parametri non sono modificabili in quanto sono stati selezionati impianti con varietà diverse."
        ));
    }

    // combo selezione parametri
    if (parametri_multipli && Cmb_Parametri != undefined) {

        Cmb_Parametri.setDataSource({ data: data, group: "raggruppamento" });

        Cmb_Parametri.value(parametri_multipli ? [] : "");
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
                            this.value([]);
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
                    placeholder: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase(),
                    height: 500
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
                    optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
                }).data("kendoDropDownList");
            });
        }
    }
}

function chiudiModificaMultipla() {
    $("#winModificaMultipla").data("kendoWindow").close();
    pulisciControlli()
}

function applicaModifiche(chiamante) { //chiamante: 1 = modifica multipla; 2 = gestione esercizi -
    if (chiamante == 1) {

        if (Cmb_Parametri.value() == "" || Cmb_Parametri.value().length == 0) {
            kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareParametrodaModificare", "Selezionare un parametro su cui eseguire delle modifiche."));
            return false;
        }

        let parametri = parametri_multipli ? Cmb_Parametri.value() : [Cmb_Parametri.value()];

        //Controllo se sono stati modificati parametri di esercizio e se esiste almeno un esercizio non chiuso 
        let almenoUnParametroEsercizio = false;
        for (i = 0; i < parametri.length; i++) {
            if (ParametriEsercizio.includes(parametri[i])) {
                almenoUnParametroEsercizio = true;
                break;
            }
        }
        if (selected_add.filter(x => x.EsercizioChiusoCod == 0).length == 0 && almenoUnParametroEsercizio) {
            kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_SelezionareEserciziNonChiusi", "Selezionare almeno un esercizio non chiuso."));
            return false
        }

        let arrayTipoAnagraficaModificati = [];

        for (i = 0; i < parametri.length; i++) {

            switch (parametri[i]) {
                case Enum_ParametriModificaMultiplaPianoColturale.IMP_Finalita:
                    var grfi_cod = Cmb_Mod_Finalita.value();
                    var grfi_des = Cmb_Mod_Finalita.text();
                    if (grfi_cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareFinalitàDaApplicare", "Selezionare la finalità da applicare."));
                        return false;
                    } else {
                        valore_parametri_modificati.grfi_cod = grfi_cod
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.ImpiantiEsercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_DisciplinareMassimaleNPK:
                    var disciplinare_selezionato_cod = Cmb_Mod_Disciplinare.value();
                    var disciplinare_selezionato_text = Cmb_Mod_Disciplinare.text();

                    var IAF_selezionati = Cmb_Mod_IAF.value();
                    var tipologia_selezionata = Cmb_Mod_Tipologia.value();

                    var stato_impianto_selezionato_cod = Cmb_Mod_StatoImpianto.value();
                    var stato_impianto_selezionato_des = Cmb_Mod_StatoImpianto.text();

                    var n_selezionato = txt_Mod_N.value();
                    var p_selezionato = txt_Mod_P.value();
                    var k_selezionato = txt_Mod_K.value();

                    var IAFVal = "";
                    if (IAF_selezionati.length > 0) {
                        for (let j = 0; j < selected_add.length; j++) {
                            if (j != 0) {
                                IAFVal = IAFVal + "|";
                            }
                            IAFVal = IAFVal + IAF_selezionati[j];
                        }
                    }

                    if (disciplinare_selezionato_cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareDisciplinareDaApplicare", "Selezionare il disciplinare da applicare."));
                        return false;
                    } else {

                        valore_parametri_modificati.Disciplinare = disciplinare_selezionato_cod;
                        if (disciplinare_selezionato_cod == "-2") {
                            valore_parametri_modificati.Reg_Cod = 4;
                            valore_parametri_modificati.Dpi_Cod = 0;
                            valore_parametri_modificati.Regolamento_Concimazioni_Cod = 0;
                            valore_parametri_modificati.Flag_PubblicoPrivato = 0;
                            valore_parametri_modificati.id_tr = 0;

                        } else if (disciplinare_selezionato_cod == "0") {
                            valore_parametri_modificati.Reg_Cod = 1;
                            valore_parametri_modificati.Dpi_Cod = 0;
                            valore_parametri_modificati.Regolamento_Concimazioni_Cod = 0;
                            valore_parametri_modificati.Flag_PubblicoPrivato = 0;
                            valore_parametri_modificati.id_tr = 0;
                        } else {
                            var arr = disciplinare_selezionato_cod.split("/");
                            valore_parametri_modificati.Reg_Cod = 1;
                            valore_parametri_modificati.Dpi_Cod = arr[0];
                            valore_parametri_modificati.Flag_PubblicoPrivato = arr[1];
                            valore_parametri_modificati.Regolamento_Concimazioni_Cod = arr[2];
                            valore_parametri_modificati.id_tr = arr[3];
                        }

                        if (IAFVal != "") {
                            valore_parametri_modificati.IAF = IAFVal;
                        } else {
                            valore_parametri_modificati.IAF = "";
                        }

                        if (stato_impianto_selezionato_cod != 0) {
                            valore_parametri_modificati.StatoImpianto_Cod = stato_impianto_selezionato_cod.toString();
                        } else {
                            valore_parametri_modificati.StatoImpianto_Cod = "0";
                        }

                        if (tipologia_selezionata != undefined && tipologia_selezionata != null && tipologia_selezionata != 0) {
                            valore_parametri_modificati.Finalita_Concimazione_Impianto = tipologia_selezionata;
                        } else {
                            valore_parametri_modificati.Finalita_Concimazione_Impianto = 0;
                        }

                        if (n_selezionato != null) {
                            valore_parametri_modificati.N = n_selezionato;
                        } else {
                            valore_parametri_modificati.N = "";
                        }

                        if (p_selezionato != null) {
                            valore_parametri_modificati.P = p_selezionato;
                        } else {
                            valore_parametri_modificati.P = "";
                        }

                        if (k_selezionato != null) {
                            valore_parametri_modificati.K = k_selezionato;
                        } else {
                            valore_parametri_modificati.K = "";
                        }
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.IMP_Varieta:
                    var cul_cod = Cmb_Mod_Varieta.value();
                    var cul_des = Cmb_Mod_Varieta.text();
                    if (cul_cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareVarietàDaApplicare", "Selezionare la varietà da applicare."));
                        return false;
                    } else {
                        valore_parametri_modificati.cul_cod = cul_cod;
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.ImpiantiEsercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.IMP_GruppoVarietale:
                    var grva_cod = Cmb_Mod_Grva.value();
                    var grva_des = Cmb_Mod_Grva.text();
                    if (grva_cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareTipologiaVarietaleDaApplicare", "Selezionare la tipologia varietale da applicare."));
                        return false;
                    } else {
                        valore_parametri_modificati.grva_cod = grva_cod;
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.ImpiantiEsercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.APP_MetodoProduzione:
                    var metodo_produzione_cod = Cmb_Metodo_Produzione.value();
                    var metodo_produzione_des = Cmb_Metodo_Produzione.text();
                    if (metodo_produzione_cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareMetodoDiProduzioneDaApplicare", "Selezionare un metodo di produzione da applicare."));
                        return false;
                    } else {
                        valore_parametri_modificati.MetodoProduzione_Cod = metodo_produzione_cod;
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.IMP_Copertura:
                    var Cop_Cod = Cmb_Copertura.value();
                    var Cop_Des = Cmb_Copertura.text();
                    if (Cop_Cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareCoperturaDaApplicare", "Selezionare una copertura da applicare."));
                        return false;
                    } else {
                        valore_parametri_modificati.Cop_Cod = Cop_Cod;
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.ImpiantiEsercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_Resa:
                    var resa = Txt_Resa.value();
                    if (resa === "" || resa == null)
                        resa = 0
                    valore_parametri_modificati.Resa = resa;
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_DataSemina:
                    var Data_Semina = Txt_DataSemina.value();
                    valore_parametri_modificati.Data_Semina = kendo.toString(Data_Semina, 'd');
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_DataRaccolta:
                    var Data_Raccolta = Txt_DataRaccolta.value();
                    valore_parametri_modificati.Data_Raccolta = kendo.toString(Data_Raccolta, 'd');
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_DataFioritura:
                    var Data_Fioritura = Txt_DataFioritura.value();
                    valore_parametri_modificati.Data_Fioritura = kendo.toString(Data_Fioritura, 'd');
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_CapitolatoPrivato:
                    var Capitolato_Cod = Cmb_CapitolatoPrivato.value();
                    if (capitolato_required && Capitolato_Cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareValoreDiCapitolatoPrivato", "Selezionare un valore di Capitolato Privato."));
                        return false;
                    } else {
                        if (Capitolato_Cod == "-999")
                            Capitolato_Cod = ""
                        valore_parametri_modificati.CapitolatoPrivato = Capitolato_Cod;
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_OrganismoReferente:
                    var Organismo_Cod = Cmb_OrganismoReferente.value();
                    if (Organismo_Cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareValoreDiOrganismoReferente", "Selezionare un valore di Organismo Referente."));
                        return false;
                    } else {
                        if (Organismo_Cod == "-999")
                            Organismo_Cod = ""
                        valore_parametri_modificati.OrganismoReferente = Organismo_Cod;
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_MagazzinoConferimento:
                    var Magazzino_Cod = Cmb_MagazzinoConferimento.value(); if (Magazzino_Cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareValoreDiMagazzinoConferimento", "Selezionare un valore di Magazzino Conferimento."));
                        return false;
                    } else {
                        if (Magazzino_Cod == "-999")
                            Magazzino_Cod = ""
                        valore_parametri_modificati.MagazzinoConferimento = Magazzino_Cod;
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_Certificazione:
                    var Certificazione = Txt_Certificazione.val();
                    valore_parametri_modificati.Certificazione = Certificazione;
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_N:
                    if (txt_Mod_N.value() != null) {
                        valore_parametri_modificati.N = txt_Mod_N.value();
                    } else {
                        valore_parametri_modificati.N = "";
                    }
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_P:
                    if (txt_Mod_P.value() != null) {
                        valore_parametri_modificati.P = txt_Mod_P.value();
                    } else {
                        valore_parametri_modificati.P = "";
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_K:
                    if (txt_Mod_K.value() != null) {
                        valore_parametri_modificati.K = txt_Mod_K.value();
                    } else {
                        valore_parametri_modificati.K = "";
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.IMP_ImpIrrigazione:
                    var ImpIrrigazione_Cod = Cmb_ImpIrrigazione.value();
                    var ImpIrrigazione_Des = Cmb_ImpIrrigazione.text();
                    if (ImpIrrigazione_Cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareValoreDiImpiantoIrrigazione", "Selezionare un valore di Impianto Irrigazione."));
                        return false;
                    } else {
                        valore_parametri_modificati.ImpIrrigazione = ImpIrrigazione_Cod;
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.ImpiantiEsercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_Regolamento:
                    var Regolamento_Cod = Cmb_Regolamento.value();
                    var Regolamento_Des = Cmb_Regolamento.text();
                    if (Regolamento_Cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareRegolamento", "Selezionare un Regolamento."));
                        return false;
                    } else {
                        valore_parametri_modificati.Reg_Cod = Regolamento_Cod;
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_DPI:
                    var Disciplinare_Cod = Cmb_Disciplinare.value();
                    var Disciplinare_Des = Cmb_Disciplinare.text();
                    if (Disciplinare_Cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareDisciplinare", "Selezionare un Disciplinare."));
                        return false;
                    } else {
                        valore_parametri_modificati.Disciplinare = Disciplinare_Cod;
                        valore_parametri_modificati.Disciplinare_Des = Disciplinare_Des;
                        let keyArr = Disciplinare_Cod.split("/");
                        if (keyArr.length == 1) {
                            valore_parametri_modificati.Dpi_Cod = keyArr[0];
                            valore_parametri_modificati.Flag_PubblicoPrivato = 0;
                        } else {
                            valore_parametri_modificati.Dpi_Cod = keyArr[0];
                            valore_parametri_modificati.Flag_PubblicoPrivato = keyArr[1];
                            valore_parametri_modificati.id_tr = keyArr[3];
                        }
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.IMP_SuFila:
                    var Su_Fila = Txt_SuFila.value();
                    if (Su_Fila == "" || Su_Fila == null)
                        Su_Fila = 0;
                    valore_parametri_modificati.Su_Fila = Su_Fila;
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.ImpiantiEsercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.IMP_TraFila:
                    var Tra_Fila = Txt_TraFila.value();
                    if (Tra_Fila == "" || Tra_Fila == null)
                        Tra_Fila = 0
                    valore_parametri_modificati.Tra_Fila = Tra_Fila;
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.ImpiantiEsercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.IMP_FormaAllevamento:
                    var Foral_Cod = Cmb_FormaAllevamento.value();
                    if (Foral_Cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareFormaAllevamento", "Selezionare una forma allevamento da applicare."));
                        return false;
                    } else {
                        valore_parametri_modificati.Foral_Cod = Foral_Cod;
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.ImpiantiEsercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.IMP_Portinnesto:
                    var Port_Cod = Cmb_Portinnesto.value();
                    var Port_Des = Cmb_Portinnesto.text();
                    if (Port_Cod == "") {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionarePortinnesto", "Selezionare una portinnesto da applicare"));
                        return false;
                    } else {
                        valore_parametri_modificati.Port_Cod = Port_Cod;
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.ImpiantiEsercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.IMP_DataInizioPortinnesto:
                    var Data_Inizio_Portinnesto = Txt_Data_Inizio_Portinnesto.value();
                    if (Data_Inizio_Portinnesto == "" || Data_Inizio_Portinnesto == null)
                        Data_Inizio_Portinnesto = '01/01/1900'
                    valore_parametri_modificati.Data_Inizio_Portinnesto = kendo.toString(Data_Inizio_Portinnesto, 'd');
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.ImpiantiEsercizi)

                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.APP_DataFineAppezzamento:
                    var Data_Fine_Appezzamento = Txt_Data_Fine_Appezzamento.value();
                    if (Data_Fine_Appezzamento == "" || Data_Fine_Appezzamento == null) {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_impostareDataFineAppezzamento", "Impostare una data per la chiusura appezzamento."));
                        return false;
                    } else {
                        valore_parametri_modificati.Data_Fine_Appezzamento = kendo.toString(Data_Fine_Appezzamento, 'd');
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.IMP_DataFineImpianto:
                    var Data_Fine_Impianto = Txt_Data_Fine_Impianto.value();
                    if (Data_Fine_Impianto == "" || Data_Fine_Impianto == null) {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_impostareDataFineImpianto", "Impostare una data per la chiusura impianto."));
                        return false;
                    } else {
                        valore_parametri_modificati.Data_Fine_Impianto = kendo.toString(Data_Fine_Impianto, 'd');
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.ImpiantiEsercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.APP_NrAppBio:
                    var nrAppBio = Txt_nrAppBio.value();
                    if (nrAppBio == null) nrAppBio = ""
                    valore_parametri_modificati.nrAppBio = nrAppBio;
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_FlagSecondoRaccolto:
                    var flagSecondoRaccolto = FlagSecondoRaccolto.value();
                    valore_parametri_modificati.FlagSecondoRaccolto = flagSecondoRaccolto
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_CertificazioneAziendale:
                    var CertificazioneAziendale_selezionati = CmbMulti_CertificazioneAziendale.value();
                    var CertificazioneAziendale_Val = CertificazioneAziendale_selezionati.join("|");

                    if (CertificazioneAziendale_Val != "") {
                        valore_parametri_modificati.CertificazioneAziendale = CertificazioneAziendale_Val;
                    } else {
                        valore_parametri_modificati.CertificazioneAziendale = "";
                    }
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_Contributi:
                    var Contributi_selezionati = CmbMulti_Contributi.value();
                    var Contributi_Val = Contributi_selezionati.join("|");

                    if (Contributi_Val != "") {
                        valore_parametri_modificati.Contributi = Contributi_Val;
                    } else {
                        valore_parametri_modificati.Contributi = "";
                    }
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_CertificazioneProdotto:
                    var CertificazioneProdotto = Cmb_CertificazioneProdotto.value();
                    valore_parametri_modificati.CertificazioneProdotto = CertificazioneProdotto;
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_Residuo:
                    var Residuo = Cmb_Residuo.value();
                    valore_parametri_modificati.Residuo = Residuo;
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_LicenzaColtivazione:
                    var LicenzaColtivazione = Cmb_LicenzaColtivazione.value();
                    valore_parametri_modificati.LicenzaColtivazione = LicenzaColtivazione;
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_RiferimentoTrasferimentoDati:
                    var RiferimentoTrasferimentoDati = Cmb_RiferimentoTrasferimentoDati.value();
                    valore_parametri_modificati.RiferimentoTrasferimentoDati = RiferimentoTrasferimentoDati;
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_Tecnico:
                    var Tecnico_selezionati = CmbMulti_Tecnico.value();
                    var Tecnico_Val = Tecnico_selezionati.join("|");

                    if (Contributi_Val != "") {
                        valore_parametri_modificati.Tecnico = Tecnico_Val;
                    } else {
                        valore_parametri_modificati.Tecnico = "";
                    }
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_PianoSemina:
                    var PianoSemina = Cmb_PianoSemina.value();
                    valore_parametri_modificati.PianoSemina = PianoSemina;
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.ESE_Prodotto:
                    var Prodotto = Cmb_Prodotto.value();
                    if (Prodotto == "" || Prodotto == null) {
                        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareProdotto", "Selezionare un prodotto da applicare"));
                        return false;
                    } else {
                        valore_parametri_modificati.Prodotto = Prodotto;
                        arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.Esercizi)
                    }
                    break;
                case Enum_ParametriModificaMultiplaPianoColturale.IMP_DataInizioImpianto:
                    var Data_Inizio_Impianto = Txt_Data_Inizio_Impianto.value();
                    if (Data_Inizio_Impianto == "" || Data_Inizio_Impianto == null)
                        Data_Inizio_Impianto = '01/01/1900'
                    valore_parametri_modificati.Data_Inizio_Impianto = kendo.toString(Data_Inizio_Impianto, 'd');
                    arrayTipoAnagraficaModificati.push(Enum_EntitaModificaMultiplaPianoColturale.ImpiantiEsercizi)
                    break;
            }
        }

        obj_ModificaMultipla.anagrafica = arrayTipoAnagraficaModificati.filter(onlyUnique);
        obj_ModificaMultipla.valore_parametri_modificati = valore_parametri_modificati

        var chiudi = ModificaMultipla();
        if (chiudi == true) {
            chiudiModificaMultipla();
        }

        return true

    } else if (chiamante == 2) {

        var azione = Get_KendoDDLValue("Cmb_Azioni");
        if (azione == "") {
            kendo.alert(Traduzione(menuBSAnagraficaResx, "SelezionareUnAzione", "Selezionare il tipo di azione."));
            return false;
        }

        var dataChiusura = $('input[name$="txtDataChiusura"]').val();
        if (dataChiusura == "" || dataChiusura == null) {
            kendo.alert(Traduzione(menuBSAnagraficaResx, "ImpostareDataChiusuraEsercizio", "Impostare la data chiusura dell'esercizio."));
            return false;
        }

        var errore = "";
        var grid = $('#divKendoModifica_Multipla_PianoColturale').data("kendoGrid");
        var items = grid.dataSource.data().filter(function (dataitem) { return dataitem.Selected == true });
        items.forEach(function (item) {
            var data_chiusura = $("#txtDataChiusura").data("kendoDatePicker").value();
            if (azione == "0" && item.validita_inizio > data_chiusura) {
                errore = Traduzione(menuBSAnagraficaResx, "DataChiusuraPrecedenteDataInizioValiditàEsercizio",
                    "La data chiusura non può precedere la data inizio validità dell'esercizio");
            } else if (azione == "0" && item.Data_Fine_Impianto < data_chiusura) {
                errore = Traduzione(menuBSAnagraficaResx, "DataChiusuraSuccessivaDataFineValiditàImpianto",
                    "La data chiusura esercizio non può essere successiva alla data fine validità dell'impianto");
            } else if (data_chiusura == null && item.validita_fine >= new Date(2100, 11, 31)) {
                errore = Traduzione(menuBSAnagraficaResx, "DataChiusuraEsercizioNonImpostata",
                    "La data chiusura esercizio deve essere impostata");
            }
        });

        if (errore == "") {
            for (let i = 0; i < selected_add.length; i++) {
                valore_parametri_modificati.dataChiusura = kendo.toString(dataChiusura, 'd');
            }
        }

        var chiudi = GestioneEsercizi();
        if (chiudi == true) {
            chiudiGestioneEsercizi();
        }

        return true;
    }
}

function pulisciControlli() {
    if ($("#Cmb_Mod_Finalita").data("kendoDropDownList") !== undefined)
        $("#Cmb_Mod_Finalita").data("kendoDropDownList").value(null);

    if ($("#Cmb_Mod_Disciplinare").data("kendoDropDownList") !== undefined)
        $("#Cmb_Mod_Disciplinare").data("kendoDropDownList").value(null);

    if ($("#Cmb_Mod_IAF").data("kendoMultiSelect") !== undefined)
        $("#Cmb_Mod_IAF").data("kendoMultiSelect").value(null);

    if ($("#Cmb_Mod_Tipologia").data("kendoDropDownList") !== undefined)
        $("#Cmb_Mod_Tipologia").data("kendoDropDownList").value(null);

    if ($("#Cmb_Mod_StatoImpianto").data("kendoDropDownList") !== undefined)
        $("#Cmb_Mod_StatoImpianto").data("kendoDropDownList").value(null);

    if ($("#Txt_Mod_N").data("kendoNumericTextBox") !== undefined)
        $("#Txt_Mod_N").data("kendoNumericTextBox").value(null);

    if ($("#Txt_Mod_P").data("kendoNumericTextBox") !== undefined)
        $("#Txt_Mod_P").data("kendoNumericTextBox").value(null);

    if ($("#Txt_Mod_K").data("kendoNumericTextBox") !== undefined)
        $("#Txt_Mod_K").data("kendoNumericTextBox").value(null);

    if ($("#Cmb_Mod_Varieta").data("kendoDropDownList") !== undefined)
        $("#Cmb_Mod_Varieta").data("kendoDropDownList").value(null);

    if ($("#Cmb_Mod_Grva").data("kendoDropDownList") !== undefined)
        $("#Cmb_Mod_Grva").data("kendoDropDownList").value(null);

    if ($("#Cmb_Metodo_Produzione").data("kendoDropDownList") !== undefined)
        $("#Cmb_Metodo_Produzione").data("kendoDropDownList").value(null);

    if ($("#Txt_Data_Fine_Appezzamento").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Fine_Appezzamento").data("kendoDatePicker").value(null);

    if ($("#Txt_Data_Fine_Impianto").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Fine_Impianto").data("kendoDatePicker").value(null);

    if ($("#Txt_Data_Inizio_Impianto").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Inizio_Impianto").data("kendoDatePicker").value(null);

    if ($("#Cmb_Copertura").data("kendoDropDownList") !== undefined)
        $("#Cmb_Copertura").data("kendoDropDownList").value(null);

    if ($("#Cmb_FormaAllevamento").data("kendoDropDownList") !== undefined)
        $("#Cmb_FormaAllevamento").data("kendoDropDownList").value(null);

    if ($("#Cmb_Portinnesto").data("kendoDropDownList") !== undefined)
        $("#Cmb_Portinnesto").data("kendoDropDownList").value(null);

    if ($("#Txt_Data_Inizio_Portinnesto").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Inizio_Portinnesto").data("kendoDatePicker").value(null);

    if ($("#Txt_Resa").data("kendoNumericTextBox") !== undefined)
        $("#Txt_Resa").data("kendoNumericTextBox").value(null);

    if ($("#Txt_SuFila").data("kendoNumericTextBox") !== undefined)
        $("#Txt_SuFila").data("kendoNumericTextBox").value(null);

    if ($("#Txt_TraFila").data("kendoNumericTextBox") !== undefined)
        $("#Txt_TraFila").data("kendoNumericTextBox").value(null);

    if ($("#Txt_Data_Semina").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Semina").data("kendoDatePicker").value(null);

    if ($("#Txt_Data_Raccolta").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Raccolta").data("kendoDatePicker").value(null);

    if ($("#Txt_Data_Esercizi").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Esercizi").data("kendoDatePicker").value(null);

    if ($("#Cmb_CapitolatoPrivato").data("kendoDropDownList") !== undefined)
        $("#Cmb_CapitolatoPrivato").data("kendoDropDownList").value(null);

    if ($("#Txt_Certificazione") !== undefined)
        $("#Txt_Certificazione").val(null);

    if ($("#Cmb_OrganismoReferente").data("kendoDropDownList") !== undefined)
        $("#Cmb_OrganismoReferente").data("kendoDropDownList").value(null);

    if ($("#Cmb_MagazzinoConferimento").data("kendoDropDownList") !== undefined)
        $("#Cmb_MagazzinoConferimento").data("kendoDropDownList").value(null);

    if ($("#Cmb_ImpIrrigazione").data("kendoDropDownList") !== undefined)
        $("#Cmb_ImpIrrigazione").data("kendoDropDownList").value(null);

    if ($("#Cmb_Regolamento").data("kendoDropDownList") !== undefined)
        $("#Cmb_Regolamento").data("kendoDropDownList").value(null);

    if ($("#Cmb_Disciplinare").data("kendoDropDownList") !== undefined)
        $("#Cmb_Disciplinare").data("kendoDropDownList").value(null);

    if ($("#Txt_nrAppBio") !== undefined)
        $("#Txt_nrAppBio").val(null);

    if ($("#CmbMulti_CertificazioneAziendale").data("kendoMultiSelect") !== undefined)
        $("#CmbMulti_CertificazioneAziendale").data("kendoMultiSelect").value(null);

    if ($("#CmbMulti_Contributi").data("kendoMultiSelect") !== undefined)
        $("#CmbMulti_Contributi").data("kendoMultiSelect").value(null);

    if ($("#Cmb_CertificazioneProdotto").data("kendoDropDownList") !== undefined)
        $("#Cmb_CertificazioneProdotto").data("kendoDropDownList").value(null);

    if ($("#Cmb_Residuo").data("kendoDropDownList") !== undefined)
        $("#Cmb_Residuo").data("kendoDropDownList").value(null);

    if ($("#Cmb_LicenzaColtivazione").data("kendoDropDownList") !== undefined)
        $("#Cmb_LicenzaColtivazione").data("kendoDropDownList").value(null);

    if ($("#Cmb_RiferimentoTrasferimentoDati").data("kendoDropDownList") !== undefined)
        $("#Cmb_RiferimentoTrasferimentoDati").data("kendoDropDownList").value(null);

    if ($("#CmbMulti_Tecnico").data("kendoMultiSelect") !== undefined)
        $("#CmbMulti_Tecnico").data("kendoMultiSelect").value(null);

    if ($("#Cmb_PianoSemina").data("kendoDropDownList") !== undefined)
        $("#Cmb_PianoSemina").data("kendoDropDownList").value(null);

    if ($("#Cmb_Prodotto").data("kendoDropDownList") !== undefined)
        $("#Cmb_Prodotto").data("kendoDropDownList").value(null);

    //riprisitno Cmb_Mod_Esercizi con il valore 'Esercizi validi alla data'
    if (Cmb_Mod_Esercizi)
        Cmb_Mod_Esercizi.value(1)
}

function AggiornaDati() {

    window.location.reload();  //refresha la pagina
}

function AggiornaDati_NoRefresh(chiavi) {
    WS_Popola_Griglia_da_Chiavi(chiavi)
}

// #########################################################################################
// ###############     A P E R T U R A / C H I U S U R A   E S E R C I Z I    ##############
// #########################################################################################

function creaGestioneEsercizi(title) {

    win_GestioneEsercizi = $("#winGestioneEsercizi").kendoWindow({
        width: "1100px",
        height: "50%",
        modal: true,
        title: Traduzione(menuBSAnagraficaResx, "ChiusuraAperturaEsercizi", "Chiusura/Apertura Esercizi"),
        closable: true,
        visible: false,
        resizable: true,
        open: async function (e) {
            this.center();
            await apriGestioneEsercizi();
        }
    }).data("kendoWindow");

    win_GestioneEsercizi.title(title);
}

function apriGestioneEsercizi() {

    let data = new Array();

    $("#avvertimentoAppBloccatiGestioneEsercizi").text("");
    let AppBloccati = $('#divKendoModifica_Multipla_PianoColturale').data("kendoGrid").dataSource.data().filter(function (i) { return i.Blk_Flag == -1 && i.Selected }).length > 0
    if (AppBloccati) {
        $("#avvertimentoAppBloccatiGestioneEsercizi").text(Traduzione(menuBSAnagraficaResx,
            "MenuBS_Anagrafica_ImpiantiBloccati",
            "ATTENZIONE: Le modifiche non verranno applicate agli impianti selezionati che risultano bloccati"
        ));
    }

    data.push({ value: 0, des: Traduzione(menuBSAnagraficaResx, "ChiusuraEsercizio", "Chiusura Esercizio") });
    for (i = 1; i <= 20; i++) {
        data.push({ value: i, des: kendo.format(Traduzione(menuBSAnagraficaResx, "ChiusuraEsercizioEdAperturaEnnesimaAnnualità", "Chiusura Esercizio ed Apertura {0} annualità"), i) });
    }

    $("#txtDataChiusura").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#", max: new Date(2100, 11, 31) }).data("kendoDatePicker");

    if (Cmb_Azioni != undefined) {

        //Cmb_Azioni.setDataSource({ data: azioni });
        Cmb_Azioni.setDataSource({ data: data });

        Cmb_Azioni.trigger("change");
    } else {
        return new Promise((resolve, reject) => {
            Cmb_Azioni = $("#Cmb_Azioni").kendoDropDownList({
                filter: "contains",
                autoBind: true,
                dataTextField: "des",
                dataValueField: "value",
                dataSource: { data: data },
                open: kendoDropDownAdjustWidth,
                dataBound: function (e) {
                    kendoDropDownAdjustWidth(e);
                },
                optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
            }).data("kendoDropDownList");
        });
    }
}

function chiudiGestioneEsercizi() {
    $('#winGestioneEsercizi').data('kendoWindow').close();
}

function GestioneEsercizi() {
    var chiudi = false
    WaitFrame.show()

    var azione = Get_KendoDDLValue("Cmb_Azioni");
    var dataChiusura = $('input[name$="txtDataChiusura"]').val();

    obj_ModificaMultipla.parametri = parametri_multipli ? Cmb_Azioni.value() : [Cmb_Azioni.value()];
    if (Cmb_Mod_Esercizi != undefined) obj_ModificaMultipla.mod_esercizi = Cmb_Mod_Esercizi.value();
    if (Txt_Data_Esercizi != undefined) obj_ModificaMultipla.data_esercizi = kendo.toString(Txt_Data_Esercizi.value(), 'd');
    var param = kendo.stringify({ azione: azione, dataChiusura: dataChiusura, esercizi: kendoEscapeOggetto(selected_add) });

    ajaxAgronicaSync(indirizzohttp + "/GestioneEsercizi", param, false,
        function (risposta) {
            if (risposta.RispostaOK) {
                chiudi = true
                kendo.alert(risposta.RispostaStringa);
                WaitFrame.hide()
            }
        }, function (risposta) {
            if (risposta.RispostaStringa !== "") {
                kendo.alert(risposta.RispostaStringa);
            } else {
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            }

            WaitFrame.hide()

        }, null, false);

    return chiudi
}

function popolaAzioneEsercizio(options) {
    var azioni = [
        { "Azione_Cod": 0, "Azione_Des": TraduzioneMultiResx(gestioneEserciziResx, "ChiusuraEsercizio", "Chiusura Esercizio") }];
    for (i = 1; i <= 20; i++) {
        azioni.push({ "Azione_Cod": i, "Azione_Des": kendo.format(TraduzioneMultiResx(gestioneEserciziResx, "ChiusuraEsercizioEdAperturaEnnesimaAnnualità", "Chiusura Esercizio ed Apertura {0} annualità"), i) });
    }
    options.success(azioni);
}



//###################################################################################################################
function onlyUnique(value, index, self) {
    return self.indexOf(value) === index;
}



// ###############################################################################
// ###############     G E T   M O D I F I C A   M U L T I P L A    ##############
// ###############################################################################

function getFinalita_MM(options) {
    let veg_cod = obj_ModificaMultipla.veg_cod;
    if (veg_cod != undefined && veg_cod != 0) {
        var parametri = kendo.stringify({ "objP_server": objP_server, "Veg_Cod": veg_cod, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });
        ajaxAgronica(pathCoreWS + "Metaschema/GruppoFinalita.asmx/CaricaComboFinalita",
            parametri,
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa);
                options.success(resp);
            }, null, null, false);
    } else {
        options.success([]);
    }
}

function impostaNPK_MM(Regolamento, Veg_Cod, Grfi_Cod, Stato_Cod) {
    if (Regolamento > 0 && Veg_Cod > 0 && Grfi_Cod > 0 && Stato_Cod > 0) {
        var parametri = kendo.stringify({
            "objP_super_server": objP_super_server,
            "objP_server": objP_server,
            "Regolamento": Regolamento,
            "Veg_Cod": Veg_Cod,
            "Grfi_Cod": Grfi_Cod,
            "Stato_Cod": Stato_Cod
        });
        ajaxAgronicaSync(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/CalcoloNPK_GrfiCod_StatoCod",
            parametri, false,
            function (risposta) {
                var npkObject = JSON.parse(risposta.RispostaStringa);
                txt_Mod_N.value(npkObject.N);
                txt_Mod_P.value(npkObject.P);
                txt_Mod_K.value(npkObject.K);
            }, null);
    } else {
        txt_Mod_N.value(null);
        txt_Mod_P.value(null);
        txt_Mod_K.value(null);
    }
}

function getDisciplinari_MM(options) {
    let veg_cod = obj_ModificaMultipla.veg_cod;
    let date = obj_ModificaMultipla.validita_inizio;
    if (veg_cod !== 0 && veg_cod !== "" && veg_cod !== undefined && date !== "" && date !== undefined) {
        var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "veg_cod": veg_cod, "data": "", "flag_disciplinareprivato": flag_disciplinareprivato });
        ajaxAgronica(pathCoreWS + "AgronicaCoreDPI/DPI.asmx/CaricaComboDPI_ConTipoRegolamento",
            parametri,
            function (risposta) {
                let ddlDisciplinare = JSON.parse(risposta.RispostaStringa);
                options.success(ddlDisciplinare);
            }, null, null, false);
    } else {
        options.success([]);
    }
}

function getIAF_MM(options) {
    let veg_cod = obj_ModificaMultipla.veg_cod;
    let disciplinare_cod = obj_ModificaMultipla.Dpi_Cod;
    if (veg_cod != 0 && disciplinare_cod != "0" && disciplinare_cod != "" && veg_cod != undefined && disciplinare_cod != undefined) {
        var ActualDate = new Date();
        var stringData = ActualDate.toLocaleDateString();
        var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "data": stringData, "flag_disciplinareprivato": flag_disciplinareprivato, "veg_cod": veg_cod, "disciplinare_cod": disciplinare_cod });

        ajaxAgronicaSync(pathCoreWS + "AgronicaCoreDPI/DPI.asmx/CaricaComboIAF",
            parametri, false,
            function (risposta) {
                options.success(JSON.parse(risposta.RispostaStringa));
            }, null);
    } else {
        options.success([]);
    }
}

function getTipologia_MM(options) {
    let vegCod = obj_ModificaMultipla.veg_cod;
    let regCod = obj_ModificaMultipla.Regolamento_Concimazioni_Cod;
    let grfiCod = obj_ModificaMultipla.grfi_cod;
    if (regCod == undefined || regCod == 0 || regCod == "0" || vegCod == undefined || vegCod == 0 || vegCod == "0" || grfiCod == undefined || grfiCod == 0 || grfiCod == "0") {
        options.success([]);
    } else {
        var parametri = kendo.stringify({
            objP_super_server: objP_super_server,
            objP_server: objP_server,
            PrimaRiga_Flag: false,
            PrimaRiga_Text: "",
            PrimaRiga_Value: "",
            Regolamento_Cod: regCod,
            Veg_Cod: vegCod,
            Grfi_Cod: grfiCod,
            xFiltroAggiuntivo: "",
            xOrderBy: ""
        });

        ajaxAgronicaSync(pathCoreWS + "AgronicaCoreDPI/PianoConcimazione.asmx/PC_Finalita_Rer_WS",
            parametri, false,
            function (risposta) {
                options.success(JSON.parse(risposta.RispostaStringa));
            }, null);
    }
}

function getStatoImpianto_MM(options) {
    let vegCod = obj_ModificaMultipla.veg_cod;
    let regCod = obj_ModificaMultipla.Regolamento_Concimazioni_Cod;
    let grfiCod = obj_ModificaMultipla.grfi_cod;
    if (regCod == undefined || regCod == 0 || regCod == "0" || vegCod == undefined || vegCod == 0 || vegCod == "0" || grfiCod == undefined || grfiCod == 0 || grfiCod == "0") {
        options.success([]);
    } else {
        let parametri;
        switch (regCod) {
            case 1: case 2: case -1: case -2:

                parametri = kendo.stringify({ "objP_super_server": objP_super_server, "objP_server": objP_server, "Veg_Cod": vegCod, "Grfi_Cod": grfiCod, "Regolamento_Cod": Math.abs(regCod), "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

                ajaxAgronicaSync(pathCoreWS + "Metaschema/GruppoFinalita.asmx/CaricaComboFinalita2",
                    parametri, false,
                    function (risposta) {
                        let specievegetali = JSON.parse(risposta.RispostaStringa);
                        options.success(specievegetali);
                    }, null);

                break;
            default:
                parametri = kendo.stringify({ "objP_super_server": objP_super_server, "objP_server": objP_server, "Veg_Cod": vegCod, "Grfi_Cod": grfiCod, "Regolamento_Cod": Math.abs(regCod), "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

                ajaxAgronicaSync(pathCoreWS + "Metaschema/GruppoFinalita.asmx/CaricaComboFinalita2",
                    parametri, false,
                    function (risposta) {
                        let specievegetali = JSON.parse(risposta.RispostaStringa);
                        var cont102 = false;
                        for (let i = 0; i < specievegetali.length; i++) {
                            if (specievegetali[i].grfi_cod == 102) {
                                cont102 = true;
                            }
                        }
                        if (!cont102) {
                            obj102 = { "grfi_cod": "102", "grfi_des": Traduzione(menuBSAnagraficaResx, "ImpiantoInProduzione", "Impianto in Produzione") };
                            specievegetali.unshift(obj102);
                        }
                        options.success(specievegetali);
                    }, null);
                break;
        }
    }
}

function getVarieta_MM(options) {
    let veg_cod = obj_ModificaMultipla.veg_cod;
    if (veg_cod != undefined && veg_cod != 0) {
        var parametri = kendo.stringify({ "objP_utenti": objP_utenti, "Veg_Cod": veg_cod, "LetteraIniziale": "", "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });
        ajaxAgronica(pathCoreWS + "Metaschema/Cultivar.asmx/CaricaComboCultivar_conFiltroUtente",
            parametri,
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa);
                options.success(resp);
            }, null, null, false);
    } else {
        options.success([]);
    }
}

function getGrva_MM(options) {
    let veg_cod = obj_ModificaMultipla.veg_cod;
    if (veg_cod != undefined && veg_cod != 0) {
        var parametri = kendo.stringify({ "objP_server": objP_server, "Veg_Cod": veg_cod, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });
        ajaxAgronica(pathCoreWS + "Metaschema/GruppoVarietale.asmx/CaricaComboGruppoVarietale",
            parametri,
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa);
                options.success(resp);
            }, null, null, false);
    } else {
        options.success([]);
    }
}

function getCopertura_MM(options) {
    let veg_cod = obj_ModificaMultipla.veg_cod;
    if (veg_cod != undefined && veg_cod != 0) {
        var parametri = kendo.stringify({ "objP_server": objP_server, "Veg_Cod": veg_cod, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });
        ajaxAgronica(pathCoreWS + "Metaschema/Copertura.asmx/CaricaComboCopertura",
            parametri,
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa);

                if (resp.filter(x => x.Cop_Cod == 0).length == 0)
                    resp.unshift(
                        {
                            Cop_Cod: 0,
                            Cop_Des: Traduzione(menuBSAnagraficaResx, "Nessuno", "Nessuno")
                        }
                    )

                options.success(resp);
            }, null, null, false);
    } else {
        options.success([]);
    }
}

function getFormaAllevamento_MM(options) {
    let veg_cod = obj_ModificaMultipla.veg_cod;
    if (veg_cod != undefined && veg_cod != 0) {
        var parametri = kendo.stringify({ "objP_server": objP_server, "Veg_Cod": veg_cod, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });
        ajaxAgronica(pathCoreWS + "Metaschema/FormeAllevamento.asmx/CaricaFormeAllevamento",
            parametri,
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa);

                if (resp.filter(x => x.Foral_Cod == 0).length == 0)
                    resp.unshift(
                        {
                            Foral_Cod: 0,
                            Foral_Des: Traduzione(menuBSAnagraficaResx, "Nessuno", "Nessuno")
                        }
                    )

                options.success(resp);
            }, null, null, false);
    } else {
        options.success([]);
    }
}

function getPortinnesto_MM(options) {
    let veg_cod = obj_ModificaMultipla.veg_cod;
    if (veg_cod != undefined && veg_cod != 0) {
        var parametri = kendo.stringify({ "objP_server": objP_server, "Veg_Cod": veg_cod, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });
        ajaxAgronica(pathCoreWS + "Metaschema/Portinnesti.asmx/CaricaComboPortinnesti",
            parametri,
            function (risposta) {
                let resp = JSON.parse(risposta.RispostaStringa);

                if (resp.filter(x => x.Port_Cod == 0).length == 0)
                    resp.unshift(
                        {
                            Port_Cod: 0,
                            Port_Des: Traduzione(menuBSAnagraficaResx, "Nessuno", "Nessuno")
                        }
                    )

                options.success(resp);
            }, null, null, false);
    } else {
        options.success([]);
    }
}

function getCapitolatoPrivato_MM(options) {
    var parametri = kendo.stringify({
        "objP_server": objP_server,
        "Argomento_Cod": 1,
        "InfoAgg_Cod": "",
        "Tipo_Codifica": 0,
        "StringaCerca": "",
        "FiltroAggiuntivo": "",
        "Ordinamento": ""
    });

    ajaxAgronica(pathCoreWS + "Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiCAC_Codifica_InfoAggiuntive",
        parametri,
        function (risposta) {
            let resp = JSON.parse(risposta.RispostaStringa);

            if (resp.filter(x => x.InfoAgg_Cod == '-999').length == 0)
                resp.unshift(
                    {
                        InfoAgg_Cod: '-999',
                        InfoAgg_Des: Traduzione(menuBSAnagraficaResx, "Nessuno", "Nessuno"),
                        CodiceAux_1: 0,
                        CodiceAux_2: 0,
                        CodiceAux_3: 0
                    }
                )

            options.success(resp);
        }, null, null, false);
}

function getOrganismoReferente_MM(options) {

    var parametri = kendo.stringify({ "objP_server": objP_server, "piva": JSON.parse(objP_agenda).Piva });

    ajaxAgronica(pathCoreWS + "Anagrafica/Contatti.asmx/CaricaComboCmb_OrganismoReferente",
        parametri,
        function (risposta) {
            let resp = JSON.parse(risposta.RispostaStringa);

            if (resp.filter(x => x.value == '-999').length == 0)
                resp.unshift(
                    {
                        text: Traduzione(menuBSAnagraficaResx, "Nessuno", "Nessuno"),
                        value: '-999'
                    }
                )

            options.success(resp);
        }, null, null, false);
}

function getMagazzinoConferimento_MM(options) {

    var parametri = kendo.stringify({ "objP_server": objP_server, "piva": "" });

    ajaxAgronica(pathCoreWS + "Anagrafica/Contatti.asmx/CaricaComboCmb_MagazzinoConferimento",
        parametri,
        function (risposta) {
            let resp = JSON.parse(risposta.RispostaStringa);

            if (resp.filter(x => x.value == '-999').length == 0)
                resp.unshift(
                    {
                        text: Traduzione(menuBSAnagraficaResx, "Nessuno", "Nessuno"),
                        value: '-999'
                    }
                )

            options.success(resp);
        }, null, null, false);
}

function getImpIrrigazione_MM(options) {

    let veg_cod = obj_ModificaMultipla.veg_cod != undefined ? obj_ModificaMultipla.veg_cod : 0;

    var parametri = kendo.stringify({ "objP_server": objP_server, "Veg_Cod": veg_cod, "StringaCerca": "", "FiltroAggiuntivo": "", "Ordinamento": "" });

    ajaxAgronica(pathCoreWS + "Metaschema/ImpiantiIrrigazioni.asmx/CaricaImpiantiIrrigazioni",
        parametri,
        function (risposta) {
            let resp = JSON.parse(risposta.RispostaStringa);
            options.success(resp);
        }, null, null, false);
}

function getRegolamento_MM(options) {
    var parametri = kendo.stringify({ "xFiltroAggiuntivo": "", "xOrderBy": "", "objP_server": objP_server });
    ajaxAgronica(pathCoreWS + "Metaschema/Regolamenti.asmx/CaricaComboRegolamenti",
        parametri,
        function (risposta) {
            let resp = JSON.parse(risposta.RispostaStringa);
            options.success(resp);
        }, null, null, false);
}

function getDisciplinare_MM(options) {
    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "veg_cod": 0, "data": "", "flag_disciplinareprivato": flag_disciplinareprivato, "reg_cod": 0 });
    ajaxAgronica(pathCoreWS + "AgronicaCoreDPI/DPI.asmx/CaricaComboDisciplinare",
        parametri,
        function (risposta) {
            let resp = JSON.parse(risposta.RispostaStringa);
            options.success(resp);
        }, null, null, false);
}

function getCertificazioneAziendale_MM(options) {
    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });

    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Imprese.asmx/CaricaComboCmb_CertificazioniAziendali",
        parametri, false,
        function (risposta) {
            options.success(JSON.parse(risposta.RispostaStringa));
        }, null);
}

function getContributi_MM(options) {
    var parametri = kendo.stringify({ "objP_server": objP_server });

    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Appezzamento.asmx/CaricaComboCmb_Contributi",
        parametri, false,
        function (risposta) {
            options.success(JSON.parse(risposta.RispostaStringa));
        }, null);
}

function getCertificazioneProdotto_MM(options) {
    var parametri = kendo.stringify({ "objP_server": objP_server });

    ajaxAgronicaSync(pathCoreWS + "Codifiche/CAC_Codifica_InfoAggiuntive.asmx/CaricaComboCmb_CertificazioniProdotto",
        parametri, false,
        function (risposta) {
            var res = JSON.parse(risposta.RispostaStringa)
            if (res.length > 0) res.unshift({ text: '', value: '' })
            options.success(res);
        }, null);
}

function getResiduo_MM(options) {
    var parametri = kendo.stringify({ "objP_server": objP_server });

    ajaxAgronicaSync(pathCoreWS + "Codifiche/CAC_Codifica_InfoAggiuntive.asmx/CaricaComboCmb_Residui",
        parametri, false,
        function (risposta) {
            var res = JSON.parse(risposta.RispostaStringa)
            if (res.length > 0) res.unshift({ text: '', value: '' })
            options.success(res);
        }, null);
}

function getLicenzaColtivazione_MM(options) {
    if (findPiva == true) getPiva()

    var parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti, "piva": piva, "Tabella_ID": 1330 });

    ajaxAgronicaSync(pathCoreWS + "FreshAndFood/FreshAndFood.asmx/LeggiValoriParametriQualitativi",
        parametri, false,
        function (risposta) {
            var res = JSON.parse(risposta.RispostaStringa)
            if (res.length > 0) res.unshift({ val_des: '', val_cod: '' })
            options.success(res);
        }, null);
}

function getRiferimentoTrasferimentoDati_MM(options) {
    if (findPiva == true) getPiva()

    var parametri = kendo.stringify({ "objP_server": objP_server, "piva": piva });

    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Contatti.asmx/CaricaComboCmb_RiferimentoTrasferimentoDati",
        parametri, false,
        function (risposta) {
            var res = JSON.parse(risposta.RispostaStringa)
            if (res.length > 0) res.unshift({ text: '', value: '' })
            options.success(res);
        }, null);
}

function getTecnico_MM(options) {
    if (findPiva == true) getPiva()

    var parametri = kendo.stringify({ "piva": piva, "objP_server": objP_server });

    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Contatti.asmx/CaricaComboCmb_Tecnici",
        parametri, false,
        function (risposta) {
            options.success(JSON.parse(risposta.RispostaStringa));
        }, null);
}

function getPianoSemina_MM(options) {
    var parametri = kendo.stringify({ "objP_server": objP_server });

    ajaxAgronicaSync(pathCoreWS + "Codifiche/CAC_Codifica_InfoAggiuntive.asmx/CaricaComboCmb_PianiSemina",
        parametri, false,
        function (risposta) {
            var res = JSON.parse(risposta.RispostaStringa)
            if (res.length > 0) res.unshift({ text: '', value: '' })
            options.success(res);
        }, null);
}

function getProdotto_MM(options) {

    if (findPiva == true) getPiva()
    let veg_cod = obj_ModificaMultipla.veg_cod;
    let cul_cod = obj_ModificaMultipla.cul_cod;

    var parametri = kendo.stringify({ "objP_server": objP_server, "piva": piva, "veg_cod": veg_cod, "cul_cod": cul_cod });

    ajaxAgronicaSync(pathCoreWS + "Anagrafica/Prodotti.asmx/CaricaComboCmb_Prodotti",
        parametri, false,
        function (risposta) {
            var res = JSON.parse(risposta.RispostaStringa)
            if (res.length > 0) res.unshift({ text: 'Nessuno', value: '0' })
            options.success(res);
        }, null);
}

function getPiva() {
    findPiva = false

    var arrayPiva = []
    var items = KendoGrid("divKendoModifica_Multipla_PianoColturale").dataSource.data()
    items.forEach(function (i) {
        if (i.Selected && jQuery.inArray(i.PIVA, arrayPiva) == -1) {
            arrayPiva.push(i.PIVA)
        }
    })

    if (arrayPiva.length == 1) piva = arrayPiva[0]

    return piva
}

// #########################################################
// ###############     G E T   C O M B O S    ##############
// #########################################################
async function CambiaParametri(parametri) {

    // forzatura per incompatibilita parametri
    if (parametri_multipli && parametri.includes("2")) {
        var subtract = ["16", "17", "18", "20", "21"];
        var values = Cmb_Parametri.value().slice();
        values = $.grep(values, function (a) {
            return $.inArray(a, subtract) == -1;
        });
        Cmb_Parametri.dataSource.filter({});
        Cmb_Parametri.value(values);
        parametri = values;
    }

    let parametroEsercizio = false;
    for (i = 0; i < parametri.length; i++) {
        if (ParametriEsercizio.includes(parametri[i])) {
            parametroEsercizio = true;
            break;
        }
    }

    // Applica a esercizi
    if (parametroEsercizio) {
        $("#modifica_esercizi").show();
        mantieniDato(Cmb_Mod_Esercizi, get_Cmb_Mod_Esercizi);
        mantieniDato(Txt_Data_Esercizi, get_Txt_Data_Esercizi);
        Cmb_Mod_Esercizi.value(1)
    } else {
        $("#modifica_esercizi").hide();
        $("#modifica_esercizi_data").hide();
    }


    //Finalità
    if (parametri.includes(Enum_ParametriModificaMultiplaPianoColturale.IMP_Finalita)) {
        $("#modifica_finalita").show();
        mantieniDato(Cmb_Mod_Finalita, get_Cmb_Mod_Finalita);
    } else {
        $("#modifica_finalita").hide();
    }

    //Disciplinare
    if (parametri.includes(Enum_ParametriModificaMultiplaPianoColturale.ESE_DisciplinareMassimaleNPK)) {
        $("#modifica_disciplinare").show();
        $("#modifica_n").show();
        $("#modifica_p").show();
        $("#modifica_k").show();
        await CaricaPannelloDisciplinare();
    } else {
        $("#modifica_disciplinare").hide();
        $("#modifica_n").hide();
        $("#modifica_p").hide();
        $("#modifica_k").hide();
    }

    //Varietà
    if (parametri.includes(Enum_ParametriModificaMultiplaPianoColturale.IMP_Varieta)) {
        $("#modifica_varieta").show();
        mantieniDato(Cmb_Mod_Varieta, get_Cmb_Mod_Varieta);
    } else {
        $("#modifica_varieta").hide();
    }

    //GruppoVarietale
    if (parametri.includes(Enum_ParametriModificaMultiplaPianoColturale.IMP_GruppoVarietale)) {
        $("#modifica_Grva").show();
        mantieniDato(Cmb_Mod_Grva, get_Cmb_Mod_Grva);
    } else {
        $("#modifica_Grva").hide();
    }

    //MetodoProduzione
    if (parametri.includes("5")) {
        $("#modifica_metodo_produzione").show();
        mantieniDato(Cmb_Metodo_Produzione, get_Cmb_Metodo_Produzione);
    } else {
        $("#modifica_metodo_produzione").hide();
    }

    //Copertura
    if (parametri.includes("6")) {
        $("#modifica_Copertura").show();
        mantieniDato(Cmb_Copertura, get_Cmb_Copertura);
    } else {
        $("#modifica_Copertura").hide();
    }

    //Resa
    if (parametri.includes("7")) {
        $("#modifica_Resa").show();
        mantieniDato(Txt_Resa, get_Txt_Resa);
    } else {
        $("#modifica_Resa").hide();
    }

    //Data Semina
    if (parametri.includes("8")) {
        $("#modifica_DataSemina").show();
        mantieniDato(Txt_DataSemina, get_Txt_Data_Semina);
    } else {
        $("#modifica_DataSemina").hide();
    }

    //Data Raccolta
    if (parametri.includes("9")) {
        $("#modifica_DataRaccolta").show();
        mantieniDato(Txt_DataRaccolta, get_Txt_DataRaccolta);
    } else {
        $("#modifica_DataRaccolta").hide();
    }

    //Data Fioritura
    if (parametri.includes("10")) {
        $("#modifica_DataFioritura").show();
        mantieniDato(Txt_DataFioritura, get_Txt_Data_Fioritura);
    } else {
        $("#modifica_DataFioritura").hide();
    }

    //Capitolato Privato
    if (parametri.includes("12")) {
        $("#modifica_CapitolatoPrivato").show();
        mantieniDato(Cmb_CapitolatoPrivato, get_Cmb_CapitolatoPrivato);
    } else {
        $("#modifica_CapitolatoPrivato").hide();
    }

    //Organismo Referente
    if (parametri.includes("13")) {
        $("#modifica_OrganismoReferente").show();
        mantieniDato(Cmb_OrganismoReferente, get_Cmb_OrganismoReferente);
    } else {
        $("#modifica_OrganismoReferente").hide();
    }

    //Magazzino Conferimento
    if (parametri.includes("14")) {
        $("#modifica_MagazzinoConferimento").show();
        mantieniDato(Cmb_MagazzinoConferimento, get_Cmb_MagazzinoConferimento);
    } else {
        $("#modifica_MagazzinoConferimento").hide();
    }

    //Certificazione
    if (parametri.includes("15")) {
        $("#modifica_Certificazione").show();
        Txt_Certificazione = $("#Txt_Certificazione");
    } else {
        $("#modifica_Certificazione").hide();
    }

    //Massimale N
    if (parametri.includes("16")) {
        $("#modifica_n").show();
        if (txt_Mod_N == undefined) txt_Mod_N = $("#Txt_Mod_N").kendoNumericTextBox().data("kendoNumericTextBox");
    } else if (!parametri.includes("2")) {
        $("#modifica_n").hide();
    }

    //Massimale P
    if (parametri.includes("17")) {
        $("#modifica_p").show();
        if (txt_Mod_P == undefined) txt_Mod_P = $("#Txt_Mod_P").kendoNumericTextBox().data("kendoNumericTextBox");
    } else if (!parametri.includes("2")) {
        $("#modifica_p").hide();
    }

    //Massimale K
    if (parametri.includes("18")) {
        $("#modifica_k").show();
        if (txt_Mod_K == undefined) txt_Mod_K = $("#Txt_Mod_K").kendoNumericTextBox().data("kendoNumericTextBox");
    } else if (!parametri.includes("2")) {
        $("#modifica_k").hide();
    }

    //Impianto irrigazione
    if (parametri.includes("19")) {
        $("#modifica_ImpIrrigazione").show();
        mantieniDato(Cmb_ImpIrrigazione, get_Cmb_ImpIrrigazione);
    } else {
        $("#modifica_ImpIrrigazione").hide();
    }

    //Regolamento
    if (parametri.includes("20")) {
        $("#modifica_Regolamento").show();
        mantieniDato(Cmb_Regolamento, get_Cmb_Regolamento);
    } else {
        $("#modifica_Regolamento").hide();
    }

    //Disciplinare
    if (parametri.includes("21")) {
        $("#modifica_DPI").show();
        mantieniDato(Cmb_Disciplinare, get_Cmb_Disciplinare);
    } else {
        $("#modifica_DPI").hide();
    }

    //Su Fila
    if (parametri.includes("22")) {
        $("#modifica_SuFila").show();
        mantieniDato(Txt_SuFila, get_Txt_SuFila);
    } else {
        $("#modifica_SuFila").hide();
    }

    //Tra Fila
    if (parametri.includes("23")) {
        $("#modifica_TraFila").show();
        mantieniDato(Txt_TraFila, get_Txt_TraFila);
    } else {
        $("#modifica_TraFila").hide();
    }

    //Forma Allevamento
    if (parametri.includes("24")) {
        $("#modifica_FormaAllevamento").show();
        mantieniDato(Cmb_FormaAllevamento, get_Cmb_FormaAllevamento);
    } else {
        $("#modifica_FormaAllevamento").hide();
    }

    //Portinnesto
    if (parametri.includes("25")) {
        $("#modifica_Portinnesto").show();
        mantieniDato(Cmb_Portinnesto, get_Cmb_Portinnesto);
    } else {
        $("#modifica_Portinnesto").hide();
    }

    //Data Portinnesto
    if (parametri.includes("26")) {
        $("#modifica_Data_Inizio_Portinnesto").show();
        mantieniDato(Txt_Data_Inizio_Portinnesto, get_Txt_Data_Inizio_Portinnesto);
    } else {
        $("#modifica_Data_Inizio_Portinnesto").hide();
    }

    //Fine Validità Appezzamento
    if (parametri.includes("27")) {
        $("#modifica_Data_Fine_Appezzamento").show();
        mantieniDato(Txt_Data_Fine_Appezzamento, get_Txt_Data_Fine_Appezzamento);
    } else {
        $("#modifica_Data_Fine_Appezzamento").hide();
    }

    //Fine Validità Impianto
    if (parametri.includes("28")) {
        $("#modifica_Data_Fine_Impianto").show();
        mantieniDato(Txt_Data_Fine_Impianto, get_Txt_Data_Fine_Impianto);
    } else {
        $("#modifica_Data_Fine_Impianto").hide();
    }

    //nrAppBio
    if (parametri.includes("29")) {
        $("#modifica_nrAppBio").show();
        mantieniDato(Txt_nrAppBio, get_nrAppBio);
    } else {
        $("#modifica_nrAppBio").hide();
    }

    //Flag Secondo Raccolto
    if (parametri.includes("30")) {
        $("#modifica_FlagSecondoRaccolto").show();
        mantieniDato(FlagSecondoRaccolto, get_FlagSecondoRaccolto);
    } else {
        $("#modifica_FlagSecondoRaccolto").hide();
    }

    if (parametri.includes("31")) {
        $("#modifica_CertificazioneAziendale").show();
        mantieniDato(CmbMulti_CertificazioneAziendale, get_CmbMulti_CertificazioneAziendale);
    } else {
        $("#modifica_CertificazioneAziendale").hide();
    }

    if (parametri.includes("32")) {
        $("#modifica_Contributi").show();
        mantieniDato(CmbMulti_Contributi, get_CmbMulti_Contributi);
    } else {
        $("#modifica_Contributi").hide();
    }

    if (parametri.includes("33")) {
        $("#modifica_CertificazioneProdotto").show();
        mantieniDato(Cmb_CertificazioneProdotto, get_Cmb_CertificazioneProdotto);
    } else {
        $("#modifica_CertificazioneProdotto").hide();
    }

    if (parametri.includes("34")) {
        $("#modifica_Residuo").show();
        mantieniDato(Cmb_Residuo, get_Cmb_Residuo);
    } else {
        $("#modifica_Residuo").hide();
    }

    if (parametri.includes("35")) {
        $("#modifica_LicenzaColtivazione").show();
        mantieniDato(Cmb_LicenzaColtivazione, get_Cmb_LicenzaColtivazione);
    } else {
        $("#modifica_LicenzaColtivazione").hide();
    }

    if (parametri.includes("36")) {
        $("#modifica_RiferimentoTrasferimentoDati").show();
        mantieniDato(Cmb_RiferimentoTrasferimentoDati, get_Cmb_RiferimentoTrasferimentoDati);
    } else {
        $("#modifica_RiferimentoTrasferimentoDati").hide();
    }

    if (parametri.includes("37")) {
        $("#modifica_Tecnico").show();
        mantieniDato(CmbMulti_Tecnico, get_CmbMulti_Tecnico);
    } else {
        $("#modifica_Tecnico").hide();
    }

    if (parametri.includes("38")) {
        $("#modifica_PianoSemina").show();
        mantieniDato(Cmb_PianoSemina, get_Cmb_PianoSemina);
    } else {
        $("#modifica_PianoSemina").hide();
    }

    if (parametri.includes("39")) {
        $("#modifica_Prodotto").show();
        mantieniDato(Cmb_Prodotto, get_Cmb_Prodotto);
    } else {
        $("#modifica_Prodotto").hide();
    }

    if (parametri.includes("40")) {
        $("#modifica_Data_Inizio_Impianto").show();
        mantieniDato(Txt_Data_Inizio_Impianto, get_Txt_Data_Inizio_Impianto);
    } else {
        $("#modifica_Data_Inizio_Impianto").hide();
    }
}

async function mantieniDato(dato, f) {
    if (dato == undefined) {
        dato = await f();
    }

    if (dato = "") {
        dato = await f();
    }
}

function get_Cmb_Mod_Finalita() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Mod_Finalita = $("#Cmb_Mod_Finalita").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "grfi_des",
            dataValueField: "grfi_cod",
            dataSource: { transport: { read: getFinalita_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Mod_Disciplinare() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Mod_Disciplinare = $("#Cmb_Mod_Disciplinare").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getDisciplinari_MM } },
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
                var dataItem = e.sender.dataItem();

                if (dataItem.value == "-2") {
                    obj_ModificaMultipla.Reg_Cod = 4;
                    obj_ModificaMultipla.Dpi_Cod = 0;
                    obj_ModificaMultipla.Regolamento_Concimazioni_Cod = 0;
                    obj_ModificaMultipla.Flag_PubblicoPrivato = 0;
                    obj_ModificaMultipla.id_tr = 0;

                } else if (dataItem.value == "0") {
                    obj_ModificaMultipla.Reg_Cod = 1;
                    obj_ModificaMultipla.Dpi_Cod = 0;
                    obj_ModificaMultipla.Regolamento_Concimazioni_Cod = 0;
                    obj_ModificaMultipla.Flag_PubblicoPrivato = 0;
                    obj_ModificaMultipla.id_tr = 0;
                } else {
                    var arr = dataItem.value.split("/");
                    if (arr != "") {
                        obj_ModificaMultipla.Reg_Cod = 1;
                        obj_ModificaMultipla.Dpi_Cod = arr[0];
                        obj_ModificaMultipla.Flag_PubblicoPrivato = arr[1];
                        obj_ModificaMultipla.Regolamento_Concimazioni_Cod = arr[2];
                        obj_ModificaMultipla.id_tr = arr[3];
                    } else {
                        obj_ModificaMultipla.Reg_Cod = 1;
                        obj_ModificaMultipla.Dpi_Cod = 0;
                        obj_ModificaMultipla.Regolamento_Concimazioni_Cod = 0;
                        obj_ModificaMultipla.Flag_PubblicoPrivato = 0;
                        obj_ModificaMultipla.id_tr = 0;
                    }
                }

                if (Cmb_Mod_IAF != undefined) {
                    Cmb_Mod_IAF.dataSource.read();
                }
                if (Cmb_Mod_Tipologia != undefined) {
                    Cmb_Mod_Tipologia.dataSource.read();
                }
                if (Cmb_Mod_StatoImpianto != undefined) {
                    Cmb_Mod_StatoImpianto.dataSource.read();
                }

                impostaNPK_MM(0, 0, 0, 0);

                win_ModificaMultipla.center();

                resolve(this);
            },
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Mod_IAF() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Mod_IAF = $("#Cmb_Mod_IAF").kendoMultiSelect({
            filter: "contains",
            autoBind: true,
            autoClose: false,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getIAF_MM } },
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
                win_ModificaMultipla.center();
                resolve(this);
            },
            placeholder: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoMultiSelect");
    });
}

function get_Cmb_Mod_Tipologia() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Mod_Tipologia = $("#Cmb_Mod_Tipologia").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Descrizione",
            dataValueField: "Codice",
            dataSource: { transport: { read: getTipologia_MM } },
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
                var dataItem = e.sender.dataItem();
                obj_ModificaMultipla.Tipologia = this.value();
                impostaNPK_MM(obj_ModificaMultipla.Regolamento_Concimazioni_Cod, obj_ModificaMultipla.veg_cod, obj_ModificaMultipla.Tipologia, obj_ModificaMultipla.StatoImpianto);
                win_ModificaMultipla.center();
                resolve(this);
            },
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Mod_StatoImpianto() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Mod_StatoImpianto = $("#Cmb_Mod_StatoImpianto").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "grfi_des",
            dataValueField: "grfi_cod",
            dataSource: { transport: { read: getStatoImpianto_MM } },
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
                var dataItem = e.sender.dataItem();
                obj_ModificaMultipla.StatoImpianto = this.value();
                impostaNPK_MM(obj_ModificaMultipla.Regolamento_Concimazioni_Cod, obj_ModificaMultipla.veg_cod, obj_ModificaMultipla.Tipologia, obj_ModificaMultipla.StatoImpianto);
                win_ModificaMultipla.center();
                resolve(this);
            },
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Mod_Varieta() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Mod_Varieta = $("#Cmb_Mod_Varieta").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "cul_des",
            dataValueField: "cul_cod",
            dataSource: { transport: { read: getVarieta_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Copertura() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Copertura = $("#Cmb_Copertura").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Cop_Des",
            dataValueField: "Cop_Cod",
            dataSource: { transport: { read: getCopertura_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_FormaAllevamento() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_FormaAllevamento = $("#Cmb_FormaAllevamento").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Foral_Des",
            dataValueField: "Foral_Cod",
            dataSource: { transport: { read: getFormaAllevamento_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Portinnesto() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Portinnesto = $("#Cmb_Portinnesto").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Port_Des",
            dataValueField: "Port_Cod",
            dataSource: { transport: { read: getPortinnesto_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Mod_Esercizi() {
    return new Promise((resolve, reject) => {
        let onLoad = true;

        let dt_mod_esercizi = [
            { Des: Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_eserciziAttiviDataOdierna", "Esercizi attivi alla data odierna"), Cod: "1" },
            { Des: Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_tuttiGliEsercizi", "Tutti gli esercizi"), Cod: "2" },
            { Des: Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_eserciziValidiAllaData", "Esercizi validi alla data"), Cod: "3" }
        ];

        Cmb_Mod_Esercizi = $("#Cmb_Mod_Esercizi").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Des",
            dataValueField: "Cod",
            dataSource: dt_mod_esercizi,
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value("1");
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                if (this.value() == "3") {
                    $("#modifica_esercizi_data").show();
                } else {
                    $("#modifica_esercizi_data").hide();
                }
                resolve(this);
            }
        }).data("kendoDropDownList");
    });
}

function get_Cmb_CapitolatoPrivato() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_CapitolatoPrivato = $("#Cmb_CapitolatoPrivato").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "InfoAgg_Des",
            dataValueField: "InfoAgg_Cod",
            dataSource: { transport: { read: getCapitolatoPrivato_MM } },
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
            optionLabel: capitolato_required ? Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase() : ""
        }).data("kendoDropDownList");
    });
}

function get_Cmb_OrganismoReferente() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_OrganismoReferente = $("#Cmb_OrganismoReferente").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getOrganismoReferente_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_MagazzinoConferimento() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_MagazzinoConferimento = $("#Cmb_MagazzinoConferimento").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getMagazzinoConferimento_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Mod_Grva() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Mod_Grva = $("#Cmb_Mod_Grva").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Grva_Des",
            dataValueField: "Grva_Cod",
            dataSource: { transport: { read: getGrva_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Metodo_Produzione() {
    return new Promise((resolve, reject) => {

        let data = [
            { Metodo_Produzione_Des: Traduzione(menuBSAnagraficaResx, "Convenzionale", "Convenzionale"), Metodo_Produzione_Cod: 1 },
            { Metodo_Produzione_Des: Traduzione(menuBSAnagraficaResx, "InConversione", "In Conversione"), Metodo_Produzione_Cod: 2 },
            { Metodo_Produzione_Des: Traduzione(menuBSAnagraficaResx, "Biologico", "Biologico"), Metodo_Produzione_Cod: 3 }
        ];

        let onLoad = true;
        Cmb_Metodo_Produzione = $("#Cmb_Metodo_Produzione").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Metodo_Produzione_Des",
            dataValueField: "Metodo_Produzione_Cod",
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_ImpIrrigazione() {
    let onLoad = true;
    return new Promise((resolve, reject) => {
        Cmb_ImpIrrigazione = $("#Cmb_ImpIrrigazione").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Imp_Des",
            dataValueField: "Imp_Cod",
            dataSource: { transport: { read: getImpIrrigazione_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Regolamento() {
    let onLoad = true;
    return new Promise((resolve, reject) => {
        Cmb_Regolamento = $("#Cmb_Regolamento").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "Reg_Des",
            dataValueField: "Reg_Cod",
            dataSource: { transport: { read: getRegolamento_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Disciplinare() {
    let onLoad = true;
    return new Promise((resolve, reject) => {
        Cmb_Disciplinare = $("#Cmb_Disciplinare").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getDisciplinare_MM } },
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
                let keyArr = this.value().split("/");
                if (keyArr.length == 1) {
                    obj_ModificaMultipla.Dpi_Cod = keyArr[0];
                    obj_ModificaMultipla.Flag_PubblicoPrivato = 0;
                } else {
                    obj_ModificaMultipla.Dpi_Cod = keyArr[0];
                    obj_ModificaMultipla.Flag_PubblicoPrivato = keyArr[1];
                    obj_ModificaMultipla.id_tr = keyArr[3];
                }
                resolve(this);
            },
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Txt_Resa() {
    return new Promise((resolve, reject) => {
        if (Txt_Resa == undefined) {
            Txt_Resa = $("#Txt_Resa").kendoNumericTextBox({
                decimals: 4
            }).data("kendoNumericTextBox");
        }
        resolve(Txt_Resa);
    });
}

function get_Txt_SuFila() {
    return new Promise((resolve, reject) => {
        if (Txt_SuFila == undefined) {
            Txt_SuFila = $("#Txt_SuFila").kendoNumericTextBox({
                decimals: 4
            }).data("kendoNumericTextBox");
        }
        resolve(Txt_SuFila);
    });
}

function get_Txt_TraFila() {
    return new Promise((resolve, reject) => {
        if (Txt_TraFila == undefined) {
            Txt_TraFila = $("#Txt_TraFila").kendoNumericTextBox({
                decimals: 4
            }).data("kendoNumericTextBox");
        }
        resolve(Txt_TraFila);
    });
}

function get_Txt_Data_Semina() {
    return new Promise((resolve, reject) => {
        if (Txt_DataSemina == undefined) {
            Txt_DataSemina = $("#Txt_DataSemina").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#" }).data("kendoDatePicker");
        }
        resolve(Txt_DataSemina);
    });
}

function get_Txt_DataRaccolta() {
    return new Promise((resolve, reject) => {
        if (Txt_DataRaccolta == undefined) {
            Txt_DataRaccolta = $("#Txt_DataRaccolta").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#" }).data("kendoDatePicker");
        }
        resolve(Txt_DataRaccolta);
    });
}

function get_Txt_Data_Fioritura() {
    return new Promise((resolve, reject) => {
        if (Txt_DataFioritura == undefined) {
            Txt_DataFioritura = $("#Txt_DataFioritura").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#" }).data("kendoDatePicker");
        }
        resolve(Txt_DataFioritura);
    });
}

function get_Txt_Data_Inizio_Portinnesto() {
    return new Promise((resolve, reject) => {
        if (Txt_Data_Inizio_Portinnesto == undefined) {
            Txt_Data_Inizio_Portinnesto = $("#Txt_Data_Inizio_Portinnesto").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#" }).data("kendoDatePicker");
        }
        resolve(Txt_Data_Inizio_Portinnesto);
    });
}

function get_Txt_Data_Fine_Appezzamento() {
    return new Promise((resolve, reject) => {
        if (Txt_Data_Fine_Appezzamento == undefined) {
            Txt_Data_Fine_Appezzamento = $("#Txt_Data_Fine_Appezzamento").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#", max: new Date(2100, 12, 31) }).data("kendoDatePicker");
        }
        resolve(Txt_Data_Fine_Appezzamento);
    });
}

function get_Txt_Data_Fine_Impianto() {
    return new Promise((resolve, reject) => {
        if (Txt_Data_Fine_Impianto == undefined) {
            Txt_Data_Fine_Impianto = $("#Txt_Data_Fine_Impianto").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#", max: new Date(2100, 12, 31) }).data("kendoDatePicker");
        }
        resolve(Txt_Data_Fine_Impianto);
    });
}

function get_Txt_Data_Inizio_Impianto() {
    return new Promise((resolve, reject) => {
        if (Txt_Data_Inizio_Impianto == undefined) {
            Txt_Data_Inizio_Impianto = $("#Txt_Data_Inizio_Impianto").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#", max: new Date(2100, 12, 31) }).data("kendoDatePicker");
        }
        resolve(Txt_Data_Inizio_Impianto);
    });
}

function get_Txt_Data_Esercizi() {
    return new Promise((resolve, reject) => {
        if (Txt_Data_Esercizi == undefined) {
            Txt_Data_Esercizi = $("#Txt_Data_Esercizi").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#" }).data("kendoDatePicker");
        }
        resolve(Txt_Data_Esercizi);
    });
}

function get_nrAppBio() {
    return new Promise((resolve, reject) => {
        if (Txt_nrAppBio == undefined) {
            Txt_nrAppBio = $("#Txt_nrAppBio").kendoTextBox({
                decimals: 4
            }).data("kendoTextBox");
        }
        resolve(Txt_nrAppBio);
    });
}

function get_FlagSecondoRaccolto() {
    return new Promise((resolve, reject) => {
        if (FlagSecondoRaccolto == undefined) {
            FlagSecondoRaccolto = $("#FlagSecondoRaccolto").kendoSwitch(
                {
                    messages: {
                        checked: Traduzione(menuBSAnagraficaResx, "Si", "Sì"),
                        unchecked: Traduzione(menuBSAnagraficaResx, "No", "No")
                    }
                }
            ).data("kendoSwitch");
        }
        resolve(FlagSecondoRaccolto);
    });
}

function get_CmbMulti_CertificazioneAziendale() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        CmbMulti_CertificazioneAziendale = $("#CmbMulti_CertificazioneAziendale").kendoMultiSelect({
            filter: "contains",
            autoBind: true,
            autoClose: false,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getCertificazioneAziendale_MM } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value([]);
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                win_ModificaMultipla.center();
                resolve(this);
            },
            placeholder: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoMultiSelect");
    });
}

function get_CmbMulti_Contributi() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        CmbMulti_Contributi = $("#CmbMulti_Contributi").kendoMultiSelect({
            filter: "contains",
            autoBind: true,
            autoClose: false,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getContributi_MM } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value([]);
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                win_ModificaMultipla.center();
                resolve(this);
            },
            placeholder: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoMultiSelect");
    });
}

function get_Cmb_CertificazioneProdotto() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_CertificazioneProdotto = $("#Cmb_CertificazioneProdotto").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getCertificazioneProdotto_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Residuo() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Residuo = $("#Cmb_Residuo").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getResiduo_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_LicenzaColtivazione() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_LicenzaColtivazione = $("#Cmb_LicenzaColtivazione").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "val_des",
            dataValueField: "val_cod",
            dataSource: { transport: { read: getLicenzaColtivazione_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_RiferimentoTrasferimentoDati() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_RiferimentoTrasferimentoDati = $("#Cmb_RiferimentoTrasferimentoDati").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getRiferimentoTrasferimentoDati_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_CmbMulti_Tecnico() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        CmbMulti_Tecnico = $("#CmbMulti_Tecnico").kendoMultiSelect({
            filter: "contains",
            autoBind: true,
            autoClose: false,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getTecnico_MM } },
            open: kendoDropDownAdjustWidth,
            dataBound: function (e) {
                kendoDropDownAdjustWidth(e);
                if (onLoad) {
                    this.value([]);
                    this.trigger("change");
                    onLoad = false;
                }
            },
            change: function (e) {
                win_ModificaMultipla.center();
                resolve(this);
            },
            placeholder: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoMultiSelect");
    });
}

function get_Cmb_PianoSemina() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_PianoSemina = $("#Cmb_PianoSemina").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getPianoSemina_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

function get_Cmb_Prodotto() {
    return new Promise((resolve, reject) => {
        let onLoad = true;
        Cmb_Prodotto = $("#Cmb_Prodotto").kendoDropDownList({
            filter: "contains",
            autoBind: true,
            dataTextField: "text",
            dataValueField: "value",
            dataSource: { transport: { read: getProdotto_MM } },
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
        }).data("kendoDropDownList");
    });
}

//###################################################################################################################

function CaricaPannelloDisciplinare() {
    return new Promise((resolve, reject) => {
        ACaricaPannelloDisciplinare();
        resolve();
    });
}

async function ACaricaPannelloDisciplinare() {
    if (txt_Mod_N == undefined) txt_Mod_N = $("#Txt_Mod_N").kendoNumericTextBox().data("kendoNumericTextBox");
    if (txt_Mod_P == undefined) txt_Mod_P = $("#Txt_Mod_P").kendoNumericTextBox().data("kendoNumericTextBox");
    if (txt_Mod_K == undefined) txt_Mod_K = $("#Txt_Mod_K").kendoNumericTextBox().data("kendoNumericTextBox");

    mantieniDato(Cmb_Mod_Disciplinare, get_Cmb_Mod_Disciplinare);

    if (Cmb_Mod_IAF != undefined) {
        mantieniDato(Cmb_Mod_IAF, get_Cmb_Mod_IAF);
    } else {
        mantieniDato(Cmb_Mod_IAF, get_Cmb_Mod_IAF);
        mantieniDato(Cmb_Mod_Tipologia, get_Cmb_Mod_Tipologia);
        mantieniDato(Cmb_Mod_StatoImpianto, get_Cmb_Mod_StatoImpianto);
    }
}
