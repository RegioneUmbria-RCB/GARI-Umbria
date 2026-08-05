var Cmb_Parametri;
var Cmb_Mod_Finalita;
var Cmb_Mod_Disciplinare;
var Cmb_Mod_IAF;
var Cmb_Mod_Tipologia;
var Cmb_Mod_StatoImpianto;
var txt_Mod_N;
var txt_Mod_P;
var txt_Mod_K;
var Cmb_Mod_Varieta;
var Cmb_Mod_Grva;
var Cmb_Metodo_Produzione;
var Txt_Data_Fine_Appezzamento;
var Txt_Data_Fine_Impianto;
var Cmb_Copertura;
var Cmb_FormaAllevamento;
var Cmb_Portinnesto;
var Txt_Data_Inizio_Portinnesto;
var Txt_Resa;
var Txt_SuFila;
var Txt_TraFila;
var Txt_Data_Semina;
var Txt_Data_Raccolta;
var Txt_Data_Fioritura;
var Cmb_Mod_Esercizi;
var Txt_Data_Esercizi;
var Cmb_CapitolatoPrivato;
var Txt_Certificazione;
var Cmb_OrganismoReferente;
var Cmb_MagazzinoConferimento;
var Cmb_ImpIrrigazione;
var Cmb_Regolamento;
var Cmb_Disciplinare;
var FlagSecondoRaccolto;

var selected_add;
var obj_ModificaMultipla;
var win_ModificaMultipla;

var parametri_multipli = false;
var capitolato_required = true;

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
        }
    }).data("kendoWindow");

    win_ModificaMultipla.title(title);
}

function chiudiModificaMultipla() {
    pulisciControlli();
    $("#winModificaMultipla").data("kendoWindow").close();
}

function apriModificaMultipla() {

    let data = new Array();
    parametri_multipli = true; // ereditatore == "2";

    $("#avvertimentoModificaMultipla").text("");

    if (obj_ModificaMultipla.anagrafica == "1") {

        data.push({ des: Traduzione(menuBSAnagraficaResx, "MetodoProduzione", "Metodo Produzione"), value: "5" });
        data.push({ des: Traduzione(menuBSAnagraficaResx, "ChiusuraAppezzamento", "Chiusura Appezzamento"), value: "27" });

    } else {

        let stessa_specie = obj_ModificaMultipla.veg_cod !== undefined && obj_ModificaMultipla.veg_cod != null && obj_ModificaMultipla.veg_cod != 0;
        let stessa_finalita = stessa_specie && obj_ModificaMultipla.grfi_cod !== undefined;
        let avvertimento = false;

        if (obj_ModificaMultipla.anagrafica == "2") {

            if (stessa_specie) {

                data.push({ des: Traduzione(menuBSAnagraficaResx, "Finalità", "Finalità"), value: "1" });
                data.push({ des: Traduzione(menuBSAnagraficaResx, "Varietà", "Varietà"), value: "3" });
                data.push({ des: Traduzione(menuBSAnagraficaResx, "GruppoVarietale", "Gruppo Varietale"), value: "4" });
                data.push({ des: Traduzione(menuBSAnagraficaResx, "Copertura", "Copertura"), value: "6" });
                data.push({ des: Traduzione(menuBSAnagraficaResx, "FormaAllevamento", "Forma Allevamento"), value: "24" });
                data.push({ des: Traduzione(menuBSAnagraficaResx, "Portinnesto", "Portinnesto"), value: "25" });
                data.push({ des: Traduzione(menuBSAnagraficaResx, "DataInizioPortinnesto", "Messa a dimora Portinnesto"), value: "26" });

            } else if (obj_ModificaMultipla.veg_cod === undefined) {
                avvertimento = true;
                $("#avvertimentoModificaMultipla").text(Traduzione(menuBSAnagraficaResx,
                    "MenuBS_Anagrafica_parametriNonSelezionabiliImpiantiDiversoUtilizzo",
                    "ATTENZIONE: Alcuni parametri non sono modificabili in quanto sono stati selezionati impianti con utilizzi (specie vegetali) diversi."
                ));
            }

        }

        if (obj_ModificaMultipla.anagrafica == "2") { //IMPIANTO
            data.push({ des: Traduzione(menuBSAnagraficaResx, "ChiusuraImpianto", "Chiusura Impianto"), value: "28" });
            data.push({ des: Traduzione(menuBSAnagraficaResx, "ImpiantoIrrigazione", "Impianto Irrigazione"), value: "19" });
            data.push({ des: Traduzione(menuBSAnagraficaResx, "SuFila", "Distanza Su Fila [m]"), value: "22" });
            data.push({ des: Traduzione(menuBSAnagraficaResx, "TraFila", "Distanza Tra Fila [m]"), value: "23" });
        }

        if (obj_ModificaMultipla.anagrafica == "3") { //ESERCIZIO
            data.push({ des: Traduzione(menuBSAnagraficaResx, "ResaPrevista", "Resa Prevista"), value: "7" });
            data.push({ des: Traduzione(menuBSAnagraficaResx, "DataSeminaPrevista", "Data Semina Prevista"), value: "8" });
            data.push({ des: Traduzione(menuBSAnagraficaResx, "DataRaccoltaPrevista", "Data Raccolta Prevista"), value: "9" });
            data.push({ des: Traduzione(menuBSAnagraficaResx, "DataFiorituraPrevista", "Data Fioritura Prevista"), value: "10" });
            data.push({ des: Traduzione(menuBSAnagraficaResx, "CapitolatoPrivato", "Capitolato Privato"), value: "12" });
            data.push({ des: Traduzione(menuBSAnagraficaResx, "Certificazione", "Certificazione"), value: "15" });
            data.push({ des: Traduzione(menuBSAnagraficaResx, "OrganismoReferente", "Organismo Referente"), value: "13" });
            data.push({ des: Traduzione(menuBSAnagraficaResx, "MagazzinoConferimento", "Magazzino Conferimento"), value: "14" });

            if (stessa_finalita) {
                data.push({ des: Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_disciplinareMassimaliNPK", "Disciplinare - Massimali NPK"), value: "2" });
            } else if (!avvertimento && obj_ModificaMultipla.grfi_cod === undefined) {
                $("#avvertimentoModificaMultipla").text(Traduzione(menuBSAnagraficaResx,
                    "MenuBS_Anagrafica_parametriNonSelezionabiliImpiantiDiversaFinalità",
                    "ATTENZIONE: Alcuni parametri non sono modificabili in quanto sono stati selezionati impianti con finalità diverse."
                ));
            }

            // Modifica puntuale Regolamento, Disciplinare e Massimali NPK
            // if (ereditatore == "2") {
            data.push({ des: Traduzione(menuBSAnagraficaResx, "Regolamento", "Regolamento"), value: "20" });
            data.push({ des: Traduzione(menuBSAnagraficaResx, "Disciplinare", "Disciplinare"), value: "21" });
            data.push({ des: "N (kg/ha)", value: "16" });
            data.push({ des: "P2O5 (kg/ha)", value: "17" });
            data.push({ des: "K2O (kg/ha)", value: "18" });

            data.push({ des: "Secondo Raccolto", value: "30" });

        }
    }

    // combo selezione parametri
    if (parametri_multipli && Cmb_Parametri != undefined) {

        Cmb_Parametri.setDataSource(data);
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
                    dataSource: data,
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
                    placeholder: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
                }).data("kendoMultiSelect");

            });

        } else {

            return new Promise((resolve, reject) => {
                Cmb_Parametri = $("#Cmb_Parametri").kendoDropDownList({
                    filter: "contains",
                    autoBind: true,
                    dataTextField: "des",
                    dataValueField: "value",
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

    let parametriEsercizio = ["2", "7", "8", "9", "10", "12", "13", "14", "15", "16", "17", "18", "20", "21", "30"];
    let parametroEsercizio = false;
    for (i = 0; i < parametri.length; i++) {
        if (parametriEsercizio.includes(parametri[i])) {
            parametroEsercizio = true;
            break;
        }
    }

    // Applica a esercizi
    if (obj_ModificaMultipla.anagrafica == "2" && parametroEsercizio) {
        $("#modifica_esercizi").show();
        mantieniDato(Cmb_Mod_Esercizi, get_Cmb_Mod_Esercizi);
        mantieniDato(Txt_Data_Esercizi, get_Txt_Data_Esercizi);
        //Cmb_Mod_Esercizi = await get_Cmb_Mod_Esercizi();
        //Txt_Data_Esercizi = await get_Txt_Data_Esercizi();
    } else {
        $("#modifica_esercizi").hide();
        $("#modifica_esercizi_data").hide();
    }


    //Finalità
    if (parametri.includes("1")) {
        $("#modifica_finalita").show();
        mantieniDato(Cmb_Mod_Finalita, get_Cmb_Mod_Finalita);
        //Cmb_Mod_Finalita = await get_Cmb_Mod_Finalita();        
    } else {
        $("#modifica_finalita").hide();
    }

    //Disciplinare
    if (parametri.includes("2")) {
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
    if (parametri.includes("3")) {
        $("#modifica_varieta").show();
        mantieniDato(Cmb_Mod_Varieta, get_Cmb_Mod_Varieta);
        //Cmb_Mod_Varieta = await get_Cmb_Mod_Varieta();   
    } else {
        $("#modifica_varieta").hide();
    }

    //GruppoVarietale
    if (parametri.includes("4")) {
        $("#modifica_Grva").show();
        mantieniDato(Cmb_Mod_Grva, get_Cmb_Mod_Grva);
        //Cmb_Mod_Grva = await get_Cmb_Mod_Grva();
    } else {
        $("#modifica_Grva").hide();
    }

    //MetodoProduzione
    if (parametri.includes("5")) {
        $("#modifica_metodo_produzione").show();
        mantieniDato(Cmb_Metodo_Produzione, get_Cmb_Metodo_Produzione);
        //Cmb_Metodo_Produzione = await get_Cmb_Metodo_Produzione();
    } else {
        $("#modifica_metodo_produzione").hide();
    }

    //Copertura
    if (parametri.includes("6")) {
        $("#modifica_Copertura").show();
        mantieniDato(Cmb_Copertura, get_Cmb_Copertura);
        //Cmb_Copertura = await get_Cmb_Copertura();
    } else {
        $("#modifica_Copertura").hide();
    }

    //Resa
    if (parametri.includes("7")) {
        $("#modifica_Resa").show();
        mantieniDato(Txt_Resa, get_Txt_Resa);
        //Txt_Resa = await get_Txt_Resa();
    } else {
        $("#modifica_Resa").hide();
    }

    //Data Semina
    if (parametri.includes("8")) {
        $("#modifica_Data_Semina").show();
        mantieniDato(Txt_Data_Semina, get_Txt_Data_Semina);
        //Txt_Data_Semina = await get_Txt_Data_Semina();
    } else {
        $("#modifica_Data_Semina").hide();
    }

    //Data Raccolta
    if (parametri.includes("9")) {
        $("#modifica_Data_Raccolta").show();
        mantieniDato(Txt_Data_Raccolta, get_Txt_Data_Raccolta);
        //Txt_DataRaccolta = await get_Txt_Data_Raccolta();
    } else {
        $("#modifica_Data_Raccolta").hide();
    }

    //Data Fioritura
    if (parametri.includes("10")) {
        $("#modifica_Data_Fioritura").show();
        mantieniDato(Txt_Data_Fioritura, get_Txt_Data_Fioritura);
        //Txt_Data_Fioritura = await get_Txt_Data_Fioritura();
    } else {
        $("#modifica_Data_Fioritura").hide();
    }

    //Capitolato Privato
    if (parametri.includes("12")) {
        $("#modifica_CapitolatoPrivato").show();
        mantieniDato(Cmb_CapitolatoPrivato, get_Cmb_CapitolatoPrivato);
        //Cmb_CapitolatoPrivato = await get_Cmb_CapitolatoPrivato();
    } else {
        $("#modifica_CapitolatoPrivato").hide();
    }

    //Organismo Referente
    if (parametri.includes("13")) {
        $("#modifica_OrganismoReferente").show();
        mantieniDato(Cmb_OrganismoReferente, get_Cmb_OrganismoReferente);
        //Cmb_OrganismoReferente = await get_Cmb_OrganismoReferente();
    } else {
        $("#modifica_OrganismoReferente").hide();
    }

    //Magazzino Conferimento
    if (parametri.includes("14")) {
        $("#modifica_MagazzinoConferimento").show();
        mantieniDato(Cmb_MagazzinoConferimento, get_Cmb_MagazzinoConferimento);
        //Cmb_MagazzinoConferimento = await get_Cmb_MagazzinoConferimento();
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
        //Cmb_ImpIrrigazione = await get_Cmb_ImpIrrigazione();
    } else {
        $("#modifica_ImpIrrigazione").hide();
    }

    //Regolamento
    if (parametri.includes("20")) {
        $("#modifica_Regolamento").show();
        mantieniDato(Cmb_Regolamento, get_Cmb_Regolamento);
        //Cmb_Regolamento = await get_Cmb_Regolamento();
    } else {
        $("#modifica_Regolamento").hide();
    }

    //Disciplinare
    if (parametri.includes("21")) {
        $("#modifica_DPI").show();
        mantieniDato(Cmb_Disciplinare, get_Cmb_Disciplinare);
        //Cmb_Disciplinare = await get_Cmb_Disciplinare();
    } else {
        $("#modifica_DPI").hide();
    }

    //Su Fila
    if (parametri.includes("22")) {
        $("#modifica_SuFila").show();
        mantieniDato(Txt_SuFila, get_Txt_SuFila);
        //Txt_SuFila = await get_Txt_SuFila();
    } else {
        $("#modifica_SuFila").hide();
    }

    //Tra Fila
    if (parametri.includes("23")) {
        $("#modifica_TraFila").show();
        mantieniDato(Txt_TraFila, get_Txt_TraFila);
        //Txt_TraFila = await get_Txt_TraFila();
    } else {
        $("#modifica_TraFila").hide();
    }

    //Forma Allevamento
    if (parametri.includes("24")) {
        $("#modifica_FormaAllevamento").show();
        mantieniDato(Cmb_FormaAllevamento, get_Cmb_FormaAllevamento);
        //Cmb_FormaAllevamento = await get_Cmb_FormaAllevamento();
    } else {
        $("#modifica_FormaAllevamento").hide();
    }

    //Portinnesto
    if (parametri.includes("25")) {
        $("#modifica_Portinnesto").show();
        mantieniDato(Cmb_Portinnesto, get_Cmb_Portinnesto);
        //Cmb_Portinnesto = await get_Cmb_Portinnesto();
    } else {
        $("#modifica_Portinnesto").hide();
    }

    //Data Portinnesto
    if (parametri.includes("26")) {
        $("#modifica_Data_Inizio_Portinnesto").show();
        mantieniDato(Txt_Data_Inizio_Portinnesto, get_Txt_Data_Inizio_Portinnesto);
        //Txt_Data_Inizio_Portinnesto = await get_Txt_Data_Inizio_Portinnesto();
    } else {
        $("#modifica_Data_Inizio_Portinnesto").hide();
    }

    //Fine Validità Appezzamento
    if (parametri.includes("27")) {
        $("#modifica_Data_Fine_Appezzamento").show();
        mantieniDato(Txt_Data_Fine_Appezzamento, get_Txt_Data_Fine_Appezzamento);
        //Txt_Data_Fine_Appezzamento = await get_Txt_Data_Fine_Appezzamento();
    } else {
        $("#modifica_Data_Fine_Appezzamento").hide();
    }

    //Fine Validità Impianto
    if (parametri.includes("28")) {
        $("#modifica_Data_Fine_Impianto").show();
        mantieniDato(Txt_Data_Fine_Impianto, get_Txt_Data_Fine_Impianto);
        //Txt_Data_Fine_Impianto = await get_Txt_Data_Fine_Impianto();
    } else {
        $("#modifica_Data_Fine_Impianto").hide();
    }

    //Flag Secondo Raccolto
    if (parametri.includes("30")) {
        $("#modifica_FlagSecondoRaccolto").show();
        mantieniDato(FlagSecondoRaccolto, get_FlagSecondoRaccolto);
        //Txt_Data_Fine_Impianto = await get_Txt_Data_Fine_Impianto();
    } else {
        $("#modifica_FlagSecondoRaccolto").hide();
    }
}

// 22/12/31: Bug FIx - 
async function mantieniDato(dato, f) {
    if (dato == undefined) {
        dato = await f();
    }

    if (dato = "") {
        dato = await f();
    }
}

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
    //Cmb_Mod_Disciplinare = await get_Cmb_Mod_Disciplinare();

    if (Cmb_Mod_IAF != undefined) {
        mantieniDato(Cmb_Mod_IAF, get_Cmb_Mod_IAF);
        //Cmb_Mod_IAF.value([]);
        //Cmb_Mod_IAF.trigger("change");
    } else {
        mantieniDato(Cmb_Mod_IAF, get_Cmb_Mod_IAF);
        mantieniDato(Cmb_Mod_Tipologia, get_Cmb_Mod_Tipologia);
        mantieniDato(Cmb_Mod_StatoImpianto, get_Cmb_Mod_StatoImpianto);
        //Cmb_Mod_IAF = await get_Cmb_Mod_IAF();
        //Cmb_Mod_Tipologia = await get_Cmb_Mod_Tipologia();
        //Cmb_Mod_StatoImpianto = await get_Cmb_Mod_StatoImpianto();
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
                    obj_ModificaMultipla.Regolamento_Concimazione_Cod = 0;
                    obj_ModificaMultipla.Flag_PubblicoPrivato = 0;
                    obj_ModificaMultipla.id_tr = 0;

                } else if (dataItem.value == "0") {
                    obj_ModificaMultipla.Reg_Cod = 1;
                    obj_ModificaMultipla.Dpi_Cod = 0;
                    obj_ModificaMultipla.Regolamento_Concimazione_Cod = 0;
                    obj_ModificaMultipla.Flag_PubblicoPrivato = 0;
                    obj_ModificaMultipla.id_tr = 0;
                } else {
                    var arr = dataItem.value.split("/");
                    obj_ModificaMultipla.Reg_Cod = 1;
                    obj_ModificaMultipla.Dpi_Cod = arr[0];
                    obj_ModificaMultipla.Flag_PubblicoPrivato = arr[1];
                    obj_ModificaMultipla.Regolamento_Concimazione_Cod = arr[2];
                    obj_ModificaMultipla.id_tr = arr[3];
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
            optionLabel: Traduzione(menuBSAnagraficaResx, "Seleziona", "Seleziona").toUpperCase()
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
                impostaNPK_MM(obj_ModificaMultipla.Regolamento_Concimazione_Cod, obj_ModificaMultipla.veg_cod, obj_ModificaMultipla.Tipologia, obj_ModificaMultipla.StatoImpianto);
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
                impostaNPK_MM(obj_ModificaMultipla.Regolamento_Concimazione_Cod, obj_ModificaMultipla.veg_cod, obj_ModificaMultipla.Tipologia, obj_ModificaMultipla.StatoImpianto);
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
        if (Txt_Data_Semina == undefined) {
            Txt_Data_Semina = $("#Txt_Data_Semina").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#" }).data("kendoDatePicker");
        }
        resolve(Txt_Data_Semina);
    });
}

function get_Txt_Data_Raccolta() {
    return new Promise((resolve, reject) => {
        if (Txt_Data_Raccolta == undefined) {
            Txt_Data_Raccolta = $("#Txt_Data_Raccolta").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#" }).data("kendoDatePicker");
        }
        resolve(Txt_Data_Raccolta);
    });
}

function get_Txt_Data_Fioritura() {
    return new Promise((resolve, reject) => {
        if (Txt_Data_Fioritura == undefined) {
            Txt_Data_Fioritura = $("#Txt_Data_Fioritura").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#" }).data("kendoDatePicker");
        }
        resolve(Txt_Data_Fioritura);
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
            Txt_Data_Fine_Appezzamento = $("#Txt_Data_Fine_Appezzamento").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#", max: new Date(2100, 11, 31) }).data("kendoDatePicker");
        }
        resolve(Txt_Data_Fine_Appezzamento);
    });
}

function get_Txt_Data_Fine_Impianto() {
    return new Promise((resolve, reject) => {
        if (Txt_Data_Fine_Impianto == undefined) {
            Txt_Data_Fine_Impianto = $("#Txt_Data_Fine_Impianto").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#", max: new Date(2100, 11, 31) }).data("kendoDatePicker");
        }
        resolve(Txt_Data_Fine_Impianto);
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

function get_Txt_Data_Esercizi() {
    return new Promise((resolve, reject) => {
        if (Txt_Data_Esercizi == undefined) {
            Txt_Data_Esercizi = $("#Txt_Data_Esercizi").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#" }).data("kendoDatePicker");
        }
        resolve(Txt_Data_Esercizi);
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
            { Metodo_Produzione_Des: Traduzione(menuBSAnagraficaResx, "Integrato", "Integrato"), Metodo_Produzione_Cod: 1 },
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

function applicaModifiche() {

    if (Cmb_Parametri.value() == "" || Cmb_Parametri.value().length == 0) {
        kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareParametrodaModificare", "Selezionare un parametro su cui eseguire delle modifiche."));
        return false;
    }

    let parametri = parametri_multipli ? Cmb_Parametri.value() : [Cmb_Parametri.value()];

    for (i = 0; i < parametri.length; i++) {

        switch (parametri[i]) {
            case "1":
                var grfi_cod = Cmb_Mod_Finalita.value();
                var grfi_des = Cmb_Mod_Finalita.text();
                if (grfi_cod == "") {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareFinalitàDaApplicare", "Selezionare la finalità da applicare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].grfi_cod = grfi_cod;
                        selected_add[i].grfi_des = grfi_des;
                    }

                    //$("#Cmb_Mod_Finalita").data("kendoDropDownList").value(null);
                }
                break;
            case "2":
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
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Disciplinare = disciplinare_selezionato_cod;
                        if (disciplinare_selezionato_cod == "-2") {
                            selected_add[i].Reg_Cod = 4;
                            selected_add[i].Dpi_Cod = 0;
                            selected_add[i].Regolamento_Concimazione_Cod = 0;
                            selected_add[i].Flag_PubblicoPrivato = 0;
                            selected_add[i].id_tr = 0;

                            if (selected_add[i].MetodoProduzione_Cod != "3" && selected_add[i].MetodoProduzione_Cod != "2") {
                                selected_add[i].MetodoProduzione_Cod = "3";
                            }

                        } else if (disciplinare_selezionato_cod == "0") {
                            selected_add[i].Reg_Cod = 1;
                            selected_add[i].Dpi_Cod = 0;
                            selected_add[i].Regolamento_Concimazione_Cod = 0;
                            selected_add[i].Flag_PubblicoPrivato = 0;
                            selected_add[i].id_tr = 0;
                        } else {
                            var arr = disciplinare_selezionato_cod.split("/");
                            selected_add[i].Reg_Cod = 1;
                            selected_add[i].Dpi_Cod = arr[0];
                            selected_add[i].Flag_PubblicoPrivato = arr[1];
                            selected_add[i].Regolamento_Concimazione_Cod = arr[2];
                            selected_add[i].id_tr = arr[3];
                        }

                        if (IAFVal != "") {
                            selected_add[i].IAF = IAFVal;
                        } else {
                            selected_add[i].IAF = "";
                        }

                        if (stato_impianto_selezionato_cod != 0) {
                            selected_add[i].StatoImpianto_Cod = stato_impianto_selezionato_cod.toString();
                        } else {
                            selected_add[i].StatoImpianto_Cod = "0";
                        }

                        if (tipologia_selezionata != undefined && tipologia_selezionata != null && tipologia_selezionata != 0) {
                            selected_add[i].Finalita_Concimazione_Impianto = tipologia_selezionata;
                        } else {
                            selected_add[i].Finalita_Concimazione_Impianto = 0;
                        }

                        if (n_selezionato != null) {
                            selected_add[i].N = n_selezionato;
                        } else {
                            selected_add[i].N = "";
                        }

                        if (p_selezionato != null) {
                            selected_add[i].P = p_selezionato;
                        } else {
                            selected_add[i].P = "";
                        }

                        if (k_selezionato != null) {
                            selected_add[i].K = k_selezionato;
                        } else {
                            selected_add[i].K = "";
                        }

                    }

                    //$("#Cmb_Mod_IAF").data("kendoMultiSelect").value(null);
                    //$("#Cmb_Mod_Disciplinare").data("kendoDropDownList").value(null);
                    //$("#Cmb_Mod_StatoImpianto").data("kendoDropDownList").value(null);
                    //$("#Cmb_Mod_Tipologia").data("kendoDropDownList").value(null);

                    //$("#Txt_Mod_N").data("kendoNumericTextBox").value(null);
                    //$("#Txt_Mod_P").data("kendoNumericTextBox").value(null);
                    //$("#Txt_Mod_K").data("kendoNumericTextBox").value(null);
                }
                break;
            case "3":
                var cul_cod = Cmb_Mod_Varieta.value();
                var cul_des = Cmb_Mod_Varieta.text();
                if (cul_cod == "") {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareVarietàDaApplicare", "Selezionare la varietà da applicare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].cul_cod = cul_cod;
                        selected_add[i].cul_des = cul_des;
                    }
                    //$("#Cmb_Mod_Varieta").data("kendoDropDownList").value(null);
                }
                break;
            case "4":
                var grva_cod = Cmb_Mod_Grva.value();
                var grva_des = Cmb_Mod_Grva.text();
                if (grva_cod == "") {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareTipologiaVarietaleDaApplicare", "Selezionare la tipologia varietale da applicare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].grva_cod = grva_cod;
                        selected_add[i].grva_des = grva_des;
                    }
                    //$("#Cmb_Mod_Grva").data("kendoDropDownList").value(null);
                }
                break;
            case "5":
                var metodo_produzione_cod = Cmb_Metodo_Produzione.value();
                var metodo_produzione_des = Cmb_Metodo_Produzione.text();
                if (metodo_produzione_cod == "") {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareMetodoDiProduzioneDaApplicare", "Selezionare un metodo di produzione da applicare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].MetodoProduzione_Cod = metodo_produzione_cod;
                        selected_add[i].MetodoProduzione_Des = metodo_produzione_des;
                    }
                    //$("#Cmb_Metodo_Produzione").data("kendoDropDownList").value(null);
                }
                break;
            case "6":
                var Cop_Cod = Cmb_Copertura.value();
                var Cop_Des = Cmb_Copertura.text();
                if (Cop_Cod == "") {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareCoperturaDaApplicare", "Selezionare una copertura da applicare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Cop_Cod = Cop_Cod;
                        selected_add[i].Cop_Des = Cop_Des;
                    }
                    //$("#Cmb_Copertura").data("kendoDropDownList").value(null);
                }
                break;
            case "7":
                var resa = Txt_Resa.value();
                if (resa === "" || resa == null) {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_impostareResaDaApplicare", "Impostare una resa da applicare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Resa = resa;
                    }
                    //$("#Txt_Resa").data("kendoNumericTextBox").value(null);
                }
                break;
            case "8":
                var Data_Semina = Txt_Data_Semina.value();
                //Anna 21/04/21: aggiunta possibilità di inserire data vuota
                //if (Data_Semina == "" || Data_Semina == null) {
                //    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_impostareDataSeminaDaApplicare", "Impostare una data di semina da applicare."));
                //    return false;
                //} else {
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Data_Semina = kendo.toString(Data_Semina, 'd');
                }
                //$("#Txt_Data_Semina").data("kendoDatePicker").value(null);
                //}
                break;
            case "9":
                var Data_Raccolta = Txt_Data_Raccolta.value();
                //Anna 21/04/21: aggiunta possibilità di inserire data vuota
                //if (Data_Raccolta == "" || Data_Raccolta == null) {
                //    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_impostareDataRaccoltaDaApplicare", "Impostare una data di raccolta prevista da applicare."));
                //    return false;
                //} else {
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Data_Raccolta = kendo.toString(Data_Raccolta, 'd');
                }
                //$("#Txt_Data_Raccolta").data("kendoDatePicker").value(null);
                //}
                break;
            case "10":
                var Data_Fioritura = Txt_Data_Fioritura.value();
                //Anna 21/04/21: aggiunta possibilità di inserire data vuota
                //if (Data_Fioritura == "" || Data_Fioritura == null) {
                //    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_impostareDataFiorituraDaApplicare", "Impostare una data di fioritura prevista da applicare."));
                //    return false;
                /*} else {*/
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].Data_Fioritura = kendo.toString(Data_Fioritura, 'd');
                }
                //$("#Txt_Data_Fioritura").data("kendoDatePicker").value(null);
                //}
                break;
            case "12":
                var Capitolato_Cod = Cmb_CapitolatoPrivato.value();
                var Capitolato_Des = Cmb_CapitolatoPrivato.text();
                if (capitolato_required && Capitolato_Cod == "") {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareValoreDiCapitolatoPrivato", "Selezionare un valore di Capitolato Privato."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].CapitolatoPrivato = Capitolato_Cod;
                        selected_add[i].CapitolatoPrivato_Des = Capitolato_Des;
                    }
                    //$("#Cmb_CapitolatoPrivato").data("kendoDropDownList").value(null);
                }
                break;
            case "13":
                var Organismo_Cod = Cmb_OrganismoReferente.value();
                var Organismo_Des = Cmb_OrganismoReferente.text();
                if (Organismo_Cod == "") {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareValoreDiOrganismoReferente", "Selezionare un valore di Organismo Referente."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].OrganismoReferente = Organismo_Cod;
                        selected_add[i].OrganismoReferente_Des = Organismo_Des;
                    }
                    //$("#Cmb_OrganismoReferente").data("kendoDropDownList").value(null);
                }
                break;
            case "14":
                var Magazzino_Cod = Cmb_MagazzinoConferimento.value();
                var Magazzino_Des = Cmb_MagazzinoConferimento.text();
                if (Magazzino_Cod == "") {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareValoreDiMagazzinoConferimento", "Selezionare un valore di Magazzino Conferimento."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].MagazzinoConferimento = Magazzino_Cod;
                        selected_add[i].MagazzinoConferimento_Des = Magazzino_Des;
                    }
                    //$("#Cmb_MagazzinoConferimento").data("kendoDropDownList").value(null);
                }
                break;
            case "15":
                var Certificazione = Txt_Certificazione.val();
                if (Certificazione == "" || Certificazione == null) {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_impostareCertificazione", "Impostare la certificazione."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Certificazione = Certificazione;
                    }

                    //$("#Txt_Certificazione").val(null);
                }
                break;
            case "16":
                for (let i = 0; i < selected_add.length; i++) {
                    if (txt_Mod_N.value() != null) {
                        selected_add[i].N = txt_Mod_N.value();
                    } else {
                        selected_add[i].N = "";
                    }
                }
                //$("#Txt_Mod_N").data("kendoNumericTextBox").value(null);
                break;
            case "17":
                for (let i = 0; i < selected_add.length; i++) {
                    if (txt_Mod_P.value() != null) {
                        selected_add[i].P = txt_Mod_P.value();
                    } else {
                        selected_add[i].P = "";
                    }
                }
                //$("#Txt_Mod_P").data("kendoNumericTextBox").value(null);
                break;
            case "18":
                for (let i = 0; i < selected_add.length; i++) {
                    if (txt_Mod_K.value() != null) {
                        selected_add[i].K = txt_Mod_K.value();
                    } else {
                        selected_add[i].K = "";
                    }
                }
                //$("#Txt_Mod_K").data("kendoNumericTextBox").value(null);
                break;
            case "19":
                var ImpIrrigazione_Cod = Cmb_ImpIrrigazione.value();
                var ImpIrrigazione_Des = Cmb_ImpIrrigazione.text();
                if (ImpIrrigazione_Cod == "") {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareValoreDiImpiantoIrrigazione", "Selezionare un valore di Impianto Irrigazione."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].ImpIrrigazione = ImpIrrigazione_Cod;
                        selected_add[i].ImpIrrigazione_Des = ImpIrrigazione_Des;
                    }
                    //$("#Cmb_ImpIrrigazione").data("kendoDropDownList").value(null);
                }
                break;
            case "20":
                var Regolamento_Cod = Cmb_Regolamento.value();
                var Regolamento_Des = Cmb_Regolamento.text();
                if (Regolamento_Cod == "") {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareRegolamento", "Selezionare un Regolamento."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Reg_Cod = Regolamento_Cod;
                        selected_add[i].Reg_Des = Regolamento_Des;
                    }
                    //$("#Cmb_Regolamento").data("kendoDropDownList").value(null);
                }
                break;
            case "21":
                var Disciplinare_Cod = Cmb_Disciplinare.value();
                var Disciplinare_Des = Cmb_Disciplinare.text();
                if (Disciplinare_Cod == "") {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_selezionareDisciplinare", "Selezionare un Disciplinare."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Disciplinare = Disciplinare_Cod;
                        selected_add[i].Disciplinare_Des = Disciplinare_Des;
                        let keyArr = Disciplinare_Cod.split("/");
                        if (keyArr.length == 1) {
                            selected_add[i].Dpi_Cod = keyArr[0];
                            selected_add[i].Flag_PubblicoPrivato = 0;
                        } else {
                            selected_add[i].Dpi_Cod = keyArr[0];
                            selected_add[i].Flag_PubblicoPrivato = keyArr[1];
                            selected_add[i].id_tr = keyArr[3];
                        }
                    }
                    //$("#Cmb_Disciplinare").data("kendoDropDownList").value(null);
                }
                break;
            case "22":
                var Su_Fila = Txt_SuFila.value();
                if (Su_Fila == "" || Su_Fila == null) {
                    kendo.alert("Impostare un valore da applicare per Su Fila");
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Su_Fila = Su_Fila;
                    }
                    //$("#Txt_SuFila").data("kendoNumericTextBox").value(null);
                }
                break;
            case "23":
                var Tra_Fila = Txt_TraFila.value();
                if (Tra_Fila == "" || Tra_Fila == null) {
                    kendo.alert("Impostare un valore da applicare per Tra Fila");
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Tra_Fila = Tra_Fila;
                    }
                    //$("#Txt_TraFila").data("kendoNumericTextBox").value(null);
                }
                break;
            case "24":
                var Foral_Cod = Cmb_FormaAllevamento.value();
                var Foral_Des = Cmb_FormaAllevamento.text();
                if (Foral_Cod == "") {
                    kendo.alert("Selezionare una forma allevamento da applicare.");
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Foral_Cod = Foral_Cod;
                        selected_add[i].Foral_Des = Foral_Des;
                    }
                    //$("#Cmb_FormaAllevamento").data("kendoDropDownList").value(null);
                }
                break;
            case "25":
                var Port_Cod = Cmb_Portinnesto.value();
                var Port_Des = Cmb_Portinnesto.text();
                if (Port_Cod == "") {
                    kendo.alert("Selezionare una portinnesto da applicare.");
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Port_Cod = Port_Cod;
                        selected_add[i].Port_Des = Port_Des;
                    }
                    //$("#Cmb_Portinnesto").data("kendoDropDownList").value(null);
                }
                break;
            case "26":
                var Data_Inizio_Portinnesto = Txt_Data_Inizio_Portinnesto.value();
                if (Data_Inizio_Portinnesto == "" || Data_Inizio_Portinnesto == null) {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_impostareDataInizioPortinnesto", "Impostare una data per la messa a dimora portinnesto."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Data_Inizio_Portinnesto = kendo.toString(Data_Inizio_Portinnesto, 'd');
                    }
                    //$("#Txt_Data_Inizio_Portinnesto").data("kendoDatePicker").value(null);
                }
                break;
            case "27":
                var Data_Fine_Appezzamento = Txt_Data_Fine_Appezzamento.value();
                if (Data_Fine_Appezzamento == "" || Data_Fine_Appezzamento == null) {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_impostareDataFineAppezzamento", "Impostare una data per la chiusura appezzamento."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Data_Fine_Appezzamento = kendo.toString(Data_Fine_Appezzamento, 'd');
                    }
                    //$("#Txt_Data_Fine_Appezzamento").data("kendoDatePicker").value(null);
                }
                break;
            case "28":
                var Data_Fine_Impianto = Txt_Data_Fine_Impianto.value();
                if (Data_Fine_Impianto == "" || Data_Fine_Impianto == null) {
                    kendo.alert(Traduzione(menuBSAnagraficaResx, "MenuBS_Anagrafica_impostareDataFineImpianto", "Impostare una data per la chiusura impianto."));
                    return false;
                } else {
                    for (let i = 0; i < selected_add.length; i++) {
                        selected_add[i].Data_Fine_Impianto = kendo.toString(Data_Fine_Impianto, 'd');
                    }
                    //$("#Txt_Data_Fine_Impianto").data("kendoDatePicker").value(null);
                }
                break;
            case "30":
                var flagSecondoRaccolto = FlagSecondoRaccolto.value();
                for (let i = 0; i < selected_add.length; i++) {
                    selected_add[i].FlagSecondoRaccolto = flagSecondoRaccolto
                }
                break;
        }

    }

    var chiudi = ModificaMultipla();
    if (chiudi == true) {
        chiudiModificaMultipla();
    }

    return true;
}

function pulisciControlli() {
    if ($("#Cmb_Mod_Finalita").data("kendoDropDownList") !== undefined)
        $("#Cmb_Mod_Finalita").data("kendoDropDownList").value(null);

    if ($("#Cmb_Mod_IAF").data("kendoMultiSelect") !== undefined)
        $("#Cmb_Mod_IAF").data("kendoMultiSelect").value(null);

    if ($("#Cmb_Mod_Disciplinare").data("kendoDropDownList") !== undefined)
        $("#Cmb_Mod_Disciplinare").data("kendoDropDownList").value(null);

    if ($("#Cmb_Mod_StatoImpianto").data("kendoDropDownList") !== undefined)
        $("#Cmb_Mod_StatoImpianto").data("kendoDropDownList").value(null);

    if ($("#Cmb_Mod_Tipologia").data("kendoDropDownList") !== undefined)
        $("#Cmb_Mod_Tipologia").data("kendoDropDownList").value(null);

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

    if ($("#Cmb_Copertura").data("kendoDropDownList") !== undefined)
        $("#Cmb_Copertura").data("kendoDropDownList").value(null);

    if ($("#Txt_Resa").data("kendoNumericTextBox") !== undefined)
        $("#Txt_Resa").data("kendoNumericTextBox").value(null);

    if ($("#Txt_Data_Semina").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Semina").data("kendoDatePicker").value(null);

    if ($("#Txt_Data_Raccolta").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Raccolta").data("kendoDatePicker").value(null);

    if ($("#Txt_Data_Fioritura").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Fioritura").data("kendoDatePicker").value(null);

    if ($("#Cmb_CapitolatoPrivato").data("kendoDropDownList") !== undefined)
        $("#Cmb_CapitolatoPrivato").data("kendoDropDownList").value(null);

    if ($("#Cmb_OrganismoReferente").data("kendoDropDownList") !== undefined)
        $("#Cmb_OrganismoReferente").data("kendoDropDownList").value(null);

    if ($("#Cmb_MagazzinoConferimento").data("kendoDropDownList") !== undefined)
        $("#Cmb_MagazzinoConferimento").data("kendoDropDownList").value(null);

    if ($("#Txt_Certificazione") !== undefined)
        $("#Txt_Certificazione").val(null);

    if ($("#Cmb_ImpIrrigazione").data("kendoDropDownList") !== undefined)
        $("#Cmb_ImpIrrigazione").data("kendoDropDownList").value(null);

    if ($("#Cmb_Regolamento").data("kendoDropDownList") !== undefined)
        $("#Cmb_Regolamento").data("kendoDropDownList").value(null);

    if ($("#Cmb_Disciplinare").data("kendoDropDownList") !== undefined)
        $("#Cmb_Disciplinare").data("kendoDropDownList").value(null);

    if ($("#Txt_SuFila").data("kendoNumericTextBox") !== undefined)
        $("#Txt_SuFila").data("kendoNumericTextBox").value(null);

    if ($("#Txt_TraFila").data("kendoNumericTextBox") !== undefined)
        $("#Txt_TraFila").data("kendoNumericTextBox").value(null);

    if ($("#Cmb_FormaAllevamento").data("kendoDropDownList") !== undefined)
        $("#Cmb_FormaAllevamento").data("kendoDropDownList").value(null);

    if ($("#Cmb_Portinnesto").data("kendoDropDownList") !== undefined)
        $("#Cmb_Portinnesto").data("kendoDropDownList").value(null);

    if ($("#Txt_Data_Inizio_Portinnesto").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Inizio_Portinnesto").data("kendoDatePicker").value(null);

    if ($("#Txt_Data_Fine_Appezzamento").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Fine_Appezzamento").data("kendoDatePicker").value(null);

    if ($("#Txt_Data_Fine_Impianto").data("kendoDatePicker") !== undefined)
        $("#Txt_Data_Fine_Impianto").data("kendoDatePicker").value(null);

    if ($("#FlagSecondoRaccolto").data("kendoSwitch") !== undefined)
        $("#FlagSecondoRaccolto").data("kendoSwitch").value(null);
}


function ModificaMultipla() {
    var chiudi = false

    WaitFrame.show()

    obj_ModificaMultipla.parametri = parametri_multipli ? Cmb_Parametri.value() : [Cmb_Parametri.value()];
    if (Cmb_Mod_Esercizi != undefined) obj_ModificaMultipla.mod_esercizi = Cmb_Mod_Esercizi.value();
    if (Txt_Data_Esercizi != undefined) obj_ModificaMultipla.data_esercizi = kendo.toString(Txt_Data_Esercizi.value(), 'd');
    var param = kendo.stringify({ parametri: kendoEscapeOggetto(obj_ModificaMultipla), dati: kendoEscapeOggetto(selected_add) });

    ajaxAgronica(indirizzohttp + "/ModificaMultipla", param,
        function (risposta) {
            if (risposta.RispostaOK) {
                chiudi = true
                WaitFrame.hide()
                kendo.alert(risposta.RispostaStringa);
                AggiornaDati();
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
    //model.Regolamento_Concimazione_Cod, veg_cod, model.grfi_cod
    let vegCod = obj_ModificaMultipla.veg_cod;
    let regCod = obj_ModificaMultipla.Regolamento_Concimazione_Cod;
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
    let regCod = obj_ModificaMultipla.Regolamento_Concimazione_Cod;
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
            options.success(resp);
        }, null, null, false);

}


function getOrganismoReferente_MM(options) {

    var parametri = kendo.stringify({ "objP_server": objP_server, "piva": JSON.parse(objP_agenda).Piva });

    ajaxAgronica(pathCoreWS + "Anagrafica/Contatti.asmx/CaricaComboCmb_OrganismoReferente",
        parametri,
        function (risposta) {
            let resp = JSON.parse(risposta.RispostaStringa);
            //objVuoto = { "text": "", "value": "" };
            //resp.unshift(objVuoto);
            options.success(resp);
        }, null, null, false);

}

function getMagazzinoConferimento_MM(options) {

    var parametri = kendo.stringify({ "objP_server": objP_server, "piva": "" });

    ajaxAgronica(pathCoreWS + "Anagrafica/Contatti.asmx/CaricaComboCmb_MagazzinoConferimento",
        parametri,
        function (risposta) {
            let resp = JSON.parse(risposta.RispostaStringa);
            //objVuoto = { "text": "", "value": "" };
            //resp.unshift(objVuoto);
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
            // objVuoto = { "Imp_Cod": "", "Imp_Des": "" };
            // resp.unshift(objVuoto);
            options.success(resp);
        }, null, null, false);
}


function getRegolamento_MM(options) {
    var parametri = kendo.stringify({ "xFiltroAggiuntivo": "", "xOrderBy": "", "objP_server": objP_server });
    ajaxAgronica(pathCoreWS + "Metaschema/Regolamenti.asmx/CaricaComboRegolamenti",
        parametri,
        function (risposta) {
            let resp = JSON.parse(risposta.RispostaStringa);
            // objVuoto = { "Reg_Cod": "", "Reg_Des": "" };
            // resp.unshift(objVuoto);
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