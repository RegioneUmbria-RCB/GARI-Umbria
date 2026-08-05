
var reportQdCResx = [];
var arrPathResx = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Statistiche/QdC/App_LocalResources/ReportQdC.aspx.resx"
];

//DOCUMENT READY
$(document).ready(async function () {

    $.logThis("DocReady: INIZIO");

    if (Array.isArray(arrPathResx) && arrPathResx.length > 0) {
        arrPathResx.forEach(function (singlePathResx) {
            reportQdCResx.push(readResxFile(singlePathResx, "ReportQdC_JQueryDocReady.js"));
        });
    }

    ElencoMisureImpianti = {
        [TraduzioneMultiResx(reportQdCResx, "SupApp", "Superficie_Appezzamento")]: { caption: TraduzioneMultiResx(reportQdCResx, "SupApp", "Superficie_Appezzamento"), field: "SupApp", format: "{0:n2}", aggregate: "sum" },
        [TraduzioneMultiResx(reportQdCResx, "NumImpianti", "Numero_Impianti")]: { caption: TraduzioneMultiResx(reportQdCResx, "NumImpianti", "Numero_Impianti"), field: "num_impianti", format: "{0:n0}", aggregate: "sum" },
        [TraduzioneMultiResx(reportQdCResx, "numPoligoni", "Poligoni")]: { caption: TraduzioneMultiResx(reportQdCResx, "numPoligoni", "Poligoni"), field: "num_poligoni", format: "{0:n0}", aggregate: "sum" },
        [TraduzioneMultiResx(reportQdCResx, "numPoligoniMancanti", "Poligoni_Mancanti")]: { caption: TraduzioneMultiResx(reportQdCResx, "numPoligoniMancanti", "Poligoni_Mancanti"), field: "num_poligoni_mancanti", format: "{0:n0}", aggregate: "sum" },
    }

    ElencoMisureOperazioni = {
        [TraduzioneMultiResx(reportQdCResx, "NumOperazioni", "Nr. Operazioni")]: {
            caption: TraduzioneMultiResx(reportQdCResx, "NumOperazioni", "Nr. Operazioni"),
            field: "Id_Agenda",
            format: "{0:n0}",
            aggregate: function (value, state, context) {
                let conta = 1;
                if (value > 0) {
                    if (state.elencoIdAgenda === undefined) {
                        state.elencoIdAgenda = [];
                        state.elencoIdAgenda.push(value);
                    } else {
                        let indice = state.elencoIdAgenda.findIndex((x) => x === value)
                        if (indice === -1) {
                            state.elencoIdAgenda.push(value);
                        } else {
                            conta = 0;
                        }
                    }
                } else {
                    conta = 0;
                }
                return (state.accumulator || 0) + conta;
            }
        },
        [TraduzioneMultiResx(reportQdCResx, "NumRigheOperazioni", "Nr. Righe Operazioni")]: { caption: TraduzioneMultiResx(reportQdCResx, "NumRigheOperazioni", "Nr. Righe Operazioni"), field: "Id_Agenda", format: "{0:n0}", aggregate: "count" },
    }

    ElencoMisureOperazioniImpianti = {
        [TraduzioneMultiResx(reportQdCResx, "SupTrattata", "Superficie Trattata")]: {
            caption: [TraduzioneMultiResx(reportQdCResx, "SupTrattata", "Superficie Trattata")],
            field: "SupTrattata",
            format: "{0:n2}",
            aggregate: function(value, state, context) {
                var dataItem = context.dataItem;
                var appezza = dataItem.Appezza;
                var SupTrattata = dataItem.SupTrattata;

                if (state.elencoAppezza === undefined) {
                    state.elencoAppezza = [];
                }
                if (state.elencoAppezza.findIndex((x) => x === appezza) === -1) {
                    state.elencoAppezza.push(appezza);
                    // manual aggregation
                    state.res = (state.res || 0) + value;
                }

                return state.res;
            }
        },
        [TraduzioneMultiResx(reportQdCResx, "SupApp", "Superficie_Appezzamento")]: {
            caption: TraduzioneMultiResx(reportQdCResx, "SupApp", "Superficie_Appezzamento"),
            field: "SupApp",
            format: "{0:n2}",
            aggregate: function (value, state, context) {
                var dataItem = context.dataItem;
                var appezza = dataItem.Appezza;
                var supApp = dataItem.supApp;

                if (state.elencoAppezza === undefined) {
                    state.elencoAppezza = [];
                }
                if (state.elencoAppezza.findIndex((x) => x === appezza)  === -1) {
                    state.elencoAppezza.push(appezza);
                    // manual aggregation
                    state.res = (state.res || 0) + value;
                }

                return state.res;
            }
        }
    }
    /*
    function(value, state, context) {
                var dataItem = context.dataItem;
                var appezza = dataItem.appezza;
                var supApp = dataItem.supApp;

                if (state.elencoAppezza === undefined) {
                    state.elencoAppezza = [];
                } else {
                    let indice = state.elencoAppezza.findIndex((x) => x === appezza)
                    if (indice === -1) {
                        state.elencoAppezza.push(appezza);
                    } 
                }

                // manual aggregation
                state.res = (state.res || 0) + value;
            },
            result: function (state) {
                if (state.res == 0) {
                    return 0;
                } else {
                    return state.res;
                }
            }
    */
    ElencoMisureOperazioniProdotti = {
        [TraduzioneMultiResx(reportQdCResx, "QtaExtraTot", "Quantità Prodotto")]: { caption: [TraduzioneMultiResx(reportQdCResx, "QtaExtraTot", "Quantità Prodotto")], field: "Qta_Extra_Totale", format: "{0:n2}", aggregate: "sum" },
        //[TraduzioneMultiResx(reportQdCResx, "QtaProd", "Quantità Prodotto")]: { caption: [TraduzioneMultiResx(reportQdCResx, "QtaProd", "Quantità Prodotto")], field: "QtaProd", format: "{0:n0}", aggregate: "sum" },
    }

    ////"Quantità Extra": { caption: "Quantità Extra", field: "QTA_EXTRA", format: "{0:n3}", aggregate: "sum" },

    ElencoMisureOperazioniProdottiImpianti = {
        [TraduzioneMultiResx(reportQdCResx, "QtaImp", "Quantità Prodotto Impianto")]: { caption: TraduzioneMultiResx(reportQdCResx, "QtaImp", "Quantità Prodotto Impianto"), field: "QtaImp", format: "{0:n2}", aggregate: "sum" },
        }

    ElencoDimensioniImpianti = {
        rag_soc: { caption: TraduzioneMultiResx(reportQdCResx, "RagioneSociale", "Ragione sociale") },
        sa_nome: { caption: TraduzioneMultiResx(reportQdCResx, "CentroAziendale", "Centro aziendale") },
        //Sa_Cod: { caption: "" },
        //Id_Agenda: { caption: "number" },
        //Id_Mov: { caption: "number" },
        //Id_Mov_Det: { caption: "number" },
        //Lav_Cod: { caption: "number" },
        //TODOFORSE Username_Creazione: { caption: TraduzioneMultiResx(reportQdCResx, "UtenteCreazione", "Utente Creazione") },
        //Cau_Mov: { caption: "number" },
        //Cul_Cod: { caption: "number" },
        //Veg_Cod: { caption: "number" },
        Veg_Des: { caption: TraduzioneMultiResx(reportQdCResx, "SpecieImp", "Specie impianto") },
        cul_des: { caption: TraduzioneMultiResx(reportQdCResx, "VarietàImp", "Varieta impianto") },
        //Raccoglitore_Cod: { caption: "number" },
        //TODOFORSE Tecnico: { caption: TraduzioneMultiResx(reportQdCResx, "TecnicoReferente", "Tecnico") },
        //Tipo_Destinazione: { caption: "number" },
        //Appezza: { caption: "number" },
        App_Nome: { caption: TraduzioneMultiResx(reportQdCResx, "Appezzamento", "Appezzamento") },
        //Elem_Cod: { caption: "number" },
        //Mat_Cod: { caption: "number" },
        //Pro_Cod: { caption: "number" }
        //Cod_Articolo: { caption: "number" },
        campo_des: { caption: TraduzioneMultiResx(reportQdCResx, "Campo", "Campo") },
        //IdImpianto: { caption: "number" },
        LottoImpianto: { caption: TraduzioneMultiResx(reportQdCResx, "Lotto", "Lotto") },
        //DestinaazioneTerreniNudi_Cod: { caption: "number" },
        Azienda_Padre: { caption: TraduzioneMultiResx(reportQdCResx, "AziendaReferente", "Azienda referente") },
        //TODO Piva_Padre: { caption: TraduzioneMultiResx(reportQdCResx, "PivaReferente", "Piva referente") },
        Stato: { caption: TraduzioneMultiResx(reportQdCResx, "Nazione", "Nazione") },
        Regione: { caption: TraduzioneMultiResx(reportQdCResx, "Regione", "Regione") },
        Provincia: { caption: TraduzioneMultiResx(reportQdCResx, "ProvinciaAbbr", "Provincia") },
        Localita: { caption: TraduzioneMultiResx(reportQdCResx, "Comune", "Comune") },
        //TODOFORSE Anno_movimento: { caption: TraduzioneMultiResx(reportQdCResx, "AnnoMovimento", "Anno movimento") },
        //TODOFORSE Mese_movimento: { caption: TraduzioneMultiResx(reportQdCResx, "MeseMovimento", "Mese movimento") },
        //TODOFORSE Data_creazione: { caption: TraduzioneMultiResx(reportQdCResx, "DataCreazione", "Data creazione") },
        Data_Creazione_Azienda: { caption: TraduzioneMultiResx(reportQdCResx, "Data_Creazione_Azienda", "Data Creazione Azienda"), format: "{0:dd/MM/yyyy}" },
        Anno_Creazione_Azienda: { caption: TraduzioneMultiResx(reportQdCResx, "Anno_Creazione_Azienda", "Anno Creazione Azienda") },
        Mese_Creazione_Azienda: { caption: TraduzioneMultiResx(reportQdCResx, "Mese_Creazione_Azienda", "Mese Creazione Azienda") },
        Utente_Creazione_Azienda: { caption: TraduzioneMultiResx(reportQdCResx, "Utente_Creazione_Azienda", "Utente Creazione Azienda") },
        Data_Creazione_Imp: { caption: TraduzioneMultiResx(reportQdCResx, "Data_Creazione_Imp", "Data Creazione Impianto"), format: "{0:dd/MM/yyyy}" },
        Anno_Creazione_Imp: { caption: TraduzioneMultiResx(reportQdCResx, "Anno_Creazione_Imp", "Anno Creazione Impianto") },
        Mese_Creazione_Imp: { caption: TraduzioneMultiResx(reportQdCResx, "Mese_Creazione_Imp", "Mese Creazione Impianto") },
        Utente_Creazione_Imp: { caption: TraduzioneMultiResx(reportQdCResx, "Utente_Creazione_Imp", "Utente Creazione Impianto") }
    } 

    ElencoDimensioniOperazioni = {
        Piva: { caption: TraduzioneMultiResx(reportQdCResx, "PartitaIvaAbbr", "P.IVA") },
        rag_soc: { caption: TraduzioneMultiResx(reportQdCResx, "RagioneSociale", "Ragione sociale") },
        sa_nome: { caption: TraduzioneMultiResx(reportQdCResx, "CentroAziendale", "Centro aziendale") },
        Des_Lib: { caption: TraduzioneMultiResx(reportQdCResx, "DesLib", "Descrizione Operazione") },
        Data_Movimento: { caption: TraduzioneMultiResx(reportQdCResx, "DataMovimento", "Data operazione") },
        Ora: { caption: TraduzioneMultiResx(reportQdCResx, "Ora", "Ora") },
        Mov_Desc: { caption: TraduzioneMultiResx(reportQdCResx, "DescMovimento", "Note operazione") },
        lav_des: { caption: TraduzioneMultiResx(reportQdCResx, "Operazione", "Operazione") },
        gru_des: { caption: TraduzioneMultiResx(reportQdCResx, "GruppoOperazioni", "Gruppo operazione") },
        tipo: { caption: TraduzioneMultiResx(reportQdCResx, "TipoOperazione", "Tipo Operazione") },
        Azienda_Padre: { caption: TraduzioneMultiResx(reportQdCResx, "AziendaReferente", "Azienda referente") },
        Stato: { caption: TraduzioneMultiResx(reportQdCResx, "Nazione", "Nazione") },
        Regione: { caption: TraduzioneMultiResx(reportQdCResx, "Regione", "Regione") },
        Provincia: { caption: TraduzioneMultiResx(reportQdCResx, "ProvinciaAbbr", "Provincia") },
        Localita: { caption: TraduzioneMultiResx(reportQdCResx, "Comune", "Comune") },
        DestinazioneTerreniNudi_Des: { caption: TraduzioneMultiResx(reportQdCResx, "DestTerreniNudi", "Destinazione terreni nudi") },
        Data_Ultima_Modifica_Intervento: { caption: TraduzioneMultiResx(reportQdCResx, "UltimaModificaIntervento", "Data ultima modifica intervento") },
        validita_inizio_destinazione: { caption: TraduzioneMultiResx(reportQdCResx, "InizioValiditaDest", "Inizio validita destinazione") },
        Data_Creazione_Azienda: { caption: TraduzioneMultiResx(reportQdCResx, "Data_Creazione_Azienda", "Data Creazione Azienda"), format: "{0:dd/MM/yyyy}" },
        Anno_Creazione_Azienda: { caption: TraduzioneMultiResx(reportQdCResx, "Anno_Creazione_Azienda", "Anno Creazione Azienda") },
        Mese_Creazione_Azienda: { caption: TraduzioneMultiResx(reportQdCResx, "Mese_Creazione_Azienda", "Mese Creazione Azienda") },
        Utente_Creazione_Azienda: { caption: TraduzioneMultiResx(reportQdCResx, "Utente_Creazione_Azienda", "Utente Creazione Azienda") },
        Anno_Creazione_Op: { caption: TraduzioneMultiResx(reportQdCResx, "Anno_Creazione_Op", "Anno Creazione Operazione") },
        Data_Creazione_Op: { caption: TraduzioneMultiResx(reportQdCResx, "Data_Creazione_Op", "Data Creazione Operazione") },
        Mese_Creazione_Op: { caption: TraduzioneMultiResx(reportQdCResx, "Mese_Creazione_Op", "Mese Creazione Operazione") },
        Utente_Creazione_Op: { caption: TraduzioneMultiResx(reportQdCResx, "Utente_Creazione_Op", "Utente Creazione Operazione") }
    };

    ElencoDimensioniOperazioniImpianti = {
        Piva: { caption: TraduzioneMultiResx(reportQdCResx, "PartitaIvaAbbr", "P.IVA") },
        rag_soc: { caption: TraduzioneMultiResx(reportQdCResx, "RagioneSociale", "Ragione sociale") },
        sa_nome: { caption: TraduzioneMultiResx(reportQdCResx, "CentroAziendale", "Centro aziendale") },
        Des_Lib: { caption: TraduzioneMultiResx(reportQdCResx, "DesLib", "Descrizione Operazione") },
        Data_Movimento: { caption: TraduzioneMultiResx(reportQdCResx, "DataMovimento", "Data operazione") },
        Ora: { caption: TraduzioneMultiResx(reportQdCResx, "Ora", "Ora") },
        Mov_Desc: { caption: TraduzioneMultiResx(reportQdCResx, "DescMovimento", "Note operazione") },
        lav_des: { caption: TraduzioneMultiResx(reportQdCResx, "Operazione", "Operazione") },
        gru_des: { caption: TraduzioneMultiResx(reportQdCResx, "GruppoOperazioni", "Gruppo operazione") },
        tipo: { caption: TraduzioneMultiResx(reportQdCResx, "TipoOperazione", "Tipo Operazione") },
        Azienda_Padre: { caption: TraduzioneMultiResx(reportQdCResx, "AziendaReferente", "Azienda referente") },
        Stato: { caption: TraduzioneMultiResx(reportQdCResx, "Nazione", "Nazione") },
        Regione: { caption: TraduzioneMultiResx(reportQdCResx, "Regione", "Regione") },
        Provincia: { caption: TraduzioneMultiResx(reportQdCResx, "ProvinciaAbbr", "Provincia") },
        Localita: { caption: TraduzioneMultiResx(reportQdCResx, "Comune", "Comune") },
        DestinazioneTerreniNudi_Des: { caption: TraduzioneMultiResx(reportQdCResx, "DestTerreniNudi", "Destinazione terreni nudi") },
        Data_Ultima_Modifica_Intervento: { caption: TraduzioneMultiResx(reportQdCResx, "UltimaModificaIntervento", "Data ultima modifica intervento") },
        validita_inizio_destinazione: { caption: TraduzioneMultiResx(reportQdCResx, "InizioValiditaDest", "Inizio validita destinazione") },
        UdmProdSim: { caption: TraduzioneMultiResx(reportQdCResx, "Unità di misura prodotto simbolo", "Unità di misura prodotto simbolo") },
        UdmExtraSim: { caption: TraduzioneMultiResx(reportQdCResx, "Unità di misura prodotto simbolo", "Unità di misura prodotto simbolo") },
        Data_Creazione_Azienda: { caption: TraduzioneMultiResx(reportQdCResx, "Data_Creazione_Azienda", "Data Creazione Azienda"), format: "{0:dd/MM/yyyy}" },
        Anno_Creazione_Azienda: { caption: TraduzioneMultiResx(reportQdCResx, "Anno_Creazione_Azienda", "Anno Creazione Azienda") },
        Mese_Creazione_Azienda: { caption: TraduzioneMultiResx(reportQdCResx, "Mese_Creazione_Azienda", "Mese Creazione Azienda") },
        Utente_Creazione_Azienda: { caption: TraduzioneMultiResx(reportQdCResx, "Utente_Creazione_Azienda", "Utente Creazione Azienda") },
        Anno_Creazione_Op: { caption: TraduzioneMultiResx(reportQdCResx, "Anno_Creazione_Op", "Anno Creazione Operazione") },
        Data_Creazione_Op: { caption: TraduzioneMultiResx(reportQdCResx, "Data_Creazione_Op", "Data Creazione Operazione") },
        Mese_Creazione_Op: { caption: TraduzioneMultiResx(reportQdCResx, "Mese_Creazione_Op", "Mese Creazione Operazione") },
        Utente_Creazione_Op: { caption: TraduzioneMultiResx(reportQdCResx, "Utente_Creazione_Op", "Utente Creazione Operazione") },
        Veg_Des: { caption: TraduzioneMultiResx(reportQdCResx, "SpecieImp", "Specie impianto") },
        cul_des: { caption: TraduzioneMultiResx(reportQdCResx, "VarietàImp", "Varieta impianto") },
        App_Nome: { caption: TraduzioneMultiResx(reportQdCResx, "Appezzamento", "Appezzamento") },
        campo_des: { caption: TraduzioneMultiResx(reportQdCResx, "Campo", "Campo") },
        LottoImpianto: { caption: TraduzioneMultiResx(reportQdCResx, "Lotto", "Lotto") },
        Data_Creazione_Imp: { caption: TraduzioneMultiResx(reportQdCResx, "Data_Creazione_Imp", "Data Creazione Impianto"), format: "{0:dd/MM/yyyy}" },
        Anno_Creazione_Imp: { caption: TraduzioneMultiResx(reportQdCResx, "Anno_Creazione_Imp", "Anno Creazione Impianto") },
        Mese_Creazione_Imp: { caption: TraduzioneMultiResx(reportQdCResx, "Mese_Creazione_Imp", "Mese Creazione Impianto") },
        Utente_Creazione_Imp: { caption: TraduzioneMultiResx(reportQdCResx, "Utente_Creazione_Imp", "Utente Creazione Impianto") }

    };

    ElencoDimensioniProdotti = {
        TipoProdotto: { caption: TraduzioneMultiResx(reportQdCResx, "TipoProdotto", "Tipo Prodotto") },
        NomeProdotto: { caption: TraduzioneMultiResx(reportQdCResx, "NomeProdotto", "Nome Prodotto") }
        //UdmProd: { caption: TraduzioneMultiResx(reportQdCResx, "UdmProd", "Unità di misura Prodotto") },
    }

    ElencoDimensioniOperazioniProdottiNoImpianti = {
        UdmExtraSim: { caption: TraduzioneMultiResx(reportQdCResx, "UdmExtra", "Unità di Misura Prodotto") }
    }

    ElencoDimensioniProdottiImpianti = {
        UdmImpSim: { caption: TraduzioneMultiResx(reportQdCResx, "UdmProdImp", "Unità di misura Prodotto") },
    }

    $("#TipoOutput").kendoDropDownList({ /*change: CampiReportPDF*/ });
    $(".reportArea").hide();
    $(".pivotGridArea").show();
    $(".gridArea").hide();
    $(".mostraGrafico").hide();

    if ($(cIdType).val() === "A") {
        // In questo caso sovrascrivo le scritte di default facenti riferimento ai clienti
        $("#lblmsClienti").text("Fornitori");
        $("#id_msClienti").data("placeholder", "Tutti i fornitori");
        $("#a_tabFiltroClienti").text("Filtro Fornitori / Agenti / Nazioni");
    }

    // nascondo option inclusione corrispettivi xchè da mostrare solo se presente un filtro per Nazione
    $("#div_opt_corrispettivi").hide();

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });

    $(".searchArea").show();

    //eventi di click pulsanti

    //$("#deselect").click(function () {
    //    required.value([]);
    //});

    $("#btn_aggiorna").click(function () {
        var kendoConfirm = $("<div></div>").kendoConfirm({
            title: "Report Vendite",
            messages: { okText: "Sì", cancel: "No" },
            content: "Vuoi aggiornare i dati del report vendite per il periodo selezionato?<br><br>L'operazione potrebbe richiedere diverso tempo"
        }).data("kendoConfirm");
        kendoConfirm.result.done(function () { AggiornaCuboReportVendite(); });
        kendoConfirm.open();        
    });

    $("#btn_esegui").click(Esegui_Report);

    Elenco_Specie = Leggi_Specie();
    ddlTipoOperazioni = await RiempiDdlTipoOperazione();
    Leggi_Operazioni([0]);
    Elenco_Nazioni = LeggiNazioni(false, false);
    Elenco_Referenti = LeggiReferenti();
    Elenco_Aziende = LeggiAziende();
    //Elenco_Estrazione = [{ "Text": TraduzioneMultiResx(reportQdCResx, "Tutte", "Tutte"), "Cod": 0 },
    //{ "Text": TraduzioneMultiResx(reportQdCResx, "AziendeNoImp", "Solo aziende senza impianti"), "Cod": 3 },
    //    { "Text": TraduzioneMultiResx(reportQdCResx, "AziendeSoloImp", "Solo aziende con impianti"), "Cod": 4 }]
    Elenco_Filtro_Date_Impianto = [{ "Text": TraduzioneMultiResx(reportQdCResx, "DateFisse", "Date Impostate"), "Cod": 0 },
        { "Text": TraduzioneMultiResx(reportQdCResx, "PeriodoOggi", "Da oggi"), "Cod": 1 },
        { "Text": TraduzioneMultiResx(reportQdCResx, "PeriodoIeri", "Da ieri"), "Cod": 2 },
        { "Text": TraduzioneMultiResx(reportQdCResx, "PeriodoSett", "Dalla settimana scorsa"), "Cod": 3 },
        { "Text": TraduzioneMultiResx(reportQdCResx, "PeriodoPers", "Periodo Impostato"), "Cod": 4 }]
    Elenco_Estrazione = [{ "Text": TraduzioneMultiResx(reportQdCResx, "Tutte", "Tutte"), "Cod": 0 },
        { "Text": TraduzioneMultiResx(reportQdCResx, "AziendeNoOp", "Solo aziende senza operazioni"), "Cod": 1 },
        { "Text": TraduzioneMultiResx(reportQdCResx, "AziendeSoloOp", "Solo aziende con operazioni"), "Cod": 2 }];

    creaKendoSwitch("SwitchEstraiImpianti", "Si", "No", false, function (e) {
        if (e.checked)
            $("#a_tabFiltroImpianti").show();
        else {
            $("#a_tabFiltroImpianti").hide();
            SvuotaParametriImpianti();
        }
        personalizzazioniPivot = null;
    });
    creaKendoSwitch("SwitchEstraiProdotti", "Si", "No", false, undefined);
    
    $("#id_selTipoAnalisi").kendoDropDownList({
        autoBind: true,
        dataTextField: "Text",
        dataValueField: "Cod",
        dataSource: [{ "Text": TraduzioneMultiResx(reportQdCResx, "AziendeOp", "Aziende / Operazioni"), "Cod": 0 }, { "Text": TraduzioneMultiResx(reportQdCResx, "AziendeImp", "Aziende / Impianti"), "Cod": 1 }],//  TODO
        open: kendoDropDownAdjustWidth,
        dataBound: function (e) {
            
        },
        change: function (e) {
            if (KendoDDL("id_selTipoAnalisi").value() == 1) {
                $("#classSwitchImpianti").hide();
                $("#classSwitchProdotti").hide();
                setKendoSwitch("SwitchEstraiImpianti", false)
                setKendoSwitch("SwitchEstraiProdotti", false)
                $("#a_tabFiltroOperazioni").hide();
                $("#a_tabFiltroImpianti").show();
                Elenco_Estrazione = [{ "Text": TraduzioneMultiResx(reportQdCResx, "Tutte", "Tutte"), "Cod": 0 },
                    { "Text": TraduzioneMultiResx(reportQdCResx, "AziendeNoImp", "Solo aziende senza impianti"), "Cod": 3 },
                    { "Text": TraduzioneMultiResx(reportQdCResx, "AziendeSoloImp", "Solo aziende con impianti"), "Cod": 4 }]
            } else {
                $("#classSwitchImpianti").show();
                $("#classSwitchProdotti").show();
                $("#a_tabFiltroOperazioni").show();
                $("#a_tabFiltroImpianti").hide();
                Elenco_Estrazione = [{ "Text": TraduzioneMultiResx(reportQdCResx, "Tutte", "Tutte"), "Cod": 0 },
                    { "Text": TraduzioneMultiResx(reportQdCResx, "AziendeNoOp", "Solo aziende senza operazioni"), "Cod": 1 },
                    { "Text": TraduzioneMultiResx(reportQdCResx, "AziendeSoloOp", "Solo aziende con operazioni"), "Cod": 2 }]
            }
            KendoDDL("id_selEstrazione").setDataSource(new kendo.data.DataSource({ data: Elenco_Estrazione }));
            KendoDDL("id_selEstrazione").value(0);
            SvuotaParametriImpianti();
            personalizzazioniPivot = null;
        }
    })

    $("#id_selEstrazione").kendoDropDownList({
        autoBind: true,
        dataTextField: "Text",
        dataValueField: "Cod",
        dataSource: new kendo.data.DataSource({ data: Elenco_Estrazione }),
        open: kendoDropDownAdjustWidth,
        dataBound: function (e) {

        },
        change: function (e) {

            //resolve();
        }
    })

    creaKendoMultiselect("multiselSpecie", { read: RiempiSpecie, data: { Veg_Cod: -1 } }, "veg_des", "veg_cod", null, null, null, SpecieChange);

    creaKendoMultiselect("selTipoOperazione", { read: RiempiTipoOperazioni, data: { gru_cod: -1 } }, "gru_des", "gru_cod", null, null, null, TipoOperazioneChange);
    creaKendoMultiselect("multiselOperazione", { read: RiempiOperazioni, data: { LAV_COD: -1 } }, "LAV_DES", "LAV_COD", null, null, null, null);
    //creaKendoSwitch("SwitchImpianti","Si", "No", false, undefined);
    //creaKendoSwitch("SwitchProdotti", "Si", "No", false, undefined);
    //KendoSwitch("SwitchProdotti").enable(false);
    //creaKendoSwitch("SwitchOperazioni", "Si", "No", false, function (e) {if (e.checked) {
    //                                                                        KendoSwitch("SwitchProdotti").enable(true);
    //                                                                     } else {
    //                                                                        KendoSwitch("SwitchProdotti").enable(false);
    //                                                                        setKendoSwitch("SwitchProdotti", false);
    //                                                                     }
    //                                                                    });

    creaKendoMultiselect("multiselReferente", { read: RiempiReferenti, data: { padre: -1 } }, "RagSoc_Padre", "padre", null, null, null, ReferenteChange);
    creaKendoMultiselect("multiselAzienda", { read: RiempiAziende, data: { PIVA: -1 } }, "rag_soc", "PIVA", null, null, null, AziendaChange);
    creaKendoMultiselect("multiselCentroAzienda", { read: RiempiCentri, data: { PivaSa: -1 } }, "sa_nome", "PivaSa", null, null, null, null);
    KendoMultisel("multiselCentroAzienda").enable(false);

    creaKendoMultiselect("multiselNazione", { read: RiempiNazioni, data: { Codice: -1 } }, "Descrizione", "Codice", null, null, null, NazioneChange);
    creaKendoMultiselect("multiselContea", { read: RiempiRegioni, data: { REG: -1 } }, "Regione_Des", "REG", null, null, null, RegioneChange);
    KendoMultisel("multiselContea").enable(false);
    creaKendoMultiselect("multiselSottocontea", { read: RiempiProvince, data: { PROV: -1 } }, "PROVINCIA", "PROV", null, null, null, ProvinciaChange);
    KendoMultisel("multiselSottocontea").enable(false);
    creaKendoMultiselect("multiselDistretto", { read: RiempiComuni, data: { COM: -1 } }, "LOCALITA", "COM", null, null, null, null);
    KendoMultisel("multiselDistretto").enable(false);

    $("#periodoGiorni")[0].defaultValue = -1;
    $("#periodoGiorniOperazioni")[0].defaultValue = -1;
    $("#ddlFiltroDateImpianto").kendoDropDownList({
        autoBind: true,
        dataTextField: "Text",
        dataValueField: "Cod",
        dataSource: new kendo.data.DataSource({ data: Elenco_Filtro_Date_Impianto }),
        open: kendoDropDownAdjustWidth,
        dataBound: function (e) {

        },
        change: function (e) {
            $("#Txt_DataImpDal").data("kendoDatePicker").value("");
            $("#Txt_DataImpAl").data("kendoDatePicker").value("");

            switch (parseInt(KendoDDL("ddlFiltroDateImpianto").value())) {
                case 0:
                    $("#DateImpostate").show();
                    $("#PeriodoImpostato").hide();
                    $("#periodoGiorni")[0].defaultValue = -1
                    $("#periodoGiorni")[0].value = -1
                    $("#periodoGiorni").attr("disabled", "disabled");
                    break;

                case 1:
                    $("#DateImpostate").hide();
                    $("#PeriodoImpostato").show();
                    $("#periodoGiorni")[0].defaultValue = 0
                    $("#periodoGiorni")[0].value = 0
                    $("#periodoGiorni").attr("disabled", "disabled");
;
                    break;

                case 2:
                    $("#DateImpostate").hide();
                    $("#PeriodoImpostato").show();
                    $("#periodoGiorni")[0].defaultValue = 1;
                    $("#periodoGiorni")[0].value = 1
                    $("#periodoGiorni").attr("disabled", "disabled");
                    break;

                case 3:
                    $("#DateImpostate").hide();
                    $("#PeriodoImpostato").show();
                    $("#periodoGiorni")[0].defaultValue = 7;
                    $("#periodoGiorni")[0].value = 7
                    $("#periodoGiorni").attr("disabled", "disabled");
                    break;

                case 4:
                    $("#DateImpostate").hide();
                    $("#PeriodoImpostato").show();
                    $("#periodoGiorni")[0].defaultValue = 0;
                    $("#periodoGiorni")[0].value = 0
                    $("#periodoGiorni").removeAttr("disabled");
                    break;

                default:

            }         
        }
    })

    $("#ddlFiltroDateOperazione").kendoDropDownList({
        autoBind: true,
        dataTextField: "Text",
        dataValueField: "Cod",
        dataSource: new kendo.data.DataSource({ data: Elenco_Filtro_Date_Impianto }),
        open: kendoDropDownAdjustWidth,
        dataBound: function (e) {

        },
        change: function (e) {
            $("#Txt_DataOpDal").data("kendoDatePicker").value("");
            $("#Txt_DataOpAl").data("kendoDatePicker").value("");

            switch (parseInt(KendoDDL("ddlFiltroDateOperazione").value())) {
                case 0:
                    $("#DateImpostateOperazioni").show();
                    $("#PeriodoImpostatoOperazioni").hide();
                    $("#periodoGiorniOperazioni")[0].defaultValue = -1
                    $("#periodoGiorniOperazioni")[0].value = -1
                    $("#periodoGiorniOperazioni").attr("disabled", "disabled");
                    break;

                case 1:
                    $("#DateImpostateOperazioni").hide();
                    $("#PeriodoImpostatoOperazioni").show();
                    $("#periodoGiorniOperazioni")[0].defaultValue = 0
                    $("#periodoGiorniOperazioni")[0].value = 0
                    $("#periodoGiorniOperazioni").attr("disabled", "disabled");
                    ;
                    break;

                case 2:
                    $("#DateImpostateOperazioni").hide();
                    $("#PeriodoImpostatoOperazioni").show();
                    $("#periodoGiorniOperazioni")[0].defaultValue = 1;
                    $("#periodoGiorniOperazioni")[0].value = 1
                    $("#periodoGiorniOperazioni").attr("disabled", "disabled");
                    break;

                case 3:
                    $("#DateImpostateOperazioni").hide();
                    $("#PeriodoImpostatoOperazioni").show();
                    $("#periodoGiorniOperazioni")[0].defaultValue = 7;
                    $("#periodoGiorniOperazioni")[0].value = 7
                    $("#periodoGiorniOperazioni").attr("disabled", "disabled");
                    break;

                case 4:
                    $("#DateImpostateOperazioni").hide();
                    $("#PeriodoImpostatoOperazioni").show();
                    $("#periodoGiorniOperazioni")[0].defaultValue = 0;
                    $("#periodoGiorniOperazioni")[0].value = 0
                    $("#periodoGiorniOperazioni").removeAttr("disabled");
                    break;

                default:

            }
        }
    })

    //creaKendoMultiselect("multiselVarieta", { read: [], data: { Cul_Cod: -1 } }, "Cul_Des", "Cul_Cod", null, null, null, null);

    ////creaKendoMultiselect("multiselSpecie", { read: [] }, "Rapporto_Des", "Cod_Rapporto");
    //creaKendoMultiselect("multiselClienti", { read: [] }, "nome", "cod_contatto");
    //creaKendoMultiselect("multiselAgenti", { read: [] }, "nome", "cod_contatto");
    //creaKendoMultiselect("multiselNazioni", { read: [] }, "Descrizione", "Codice", undefined, undefined, undefined, onChange_MsNazioni);

    ////function creaKendoMultiselect(IDControllo, t, textfield, valuefield, filterType, functionSelect, functionDeselect, functionChange) {

    //creaKendoMultiselect("multiselReferente", { read: [] }, "Elem_Des", "Elem_Cod");
    //creaKendoMultiselect("multiselCentroAzienda", { read: [] }, "Linea_Classe_Des", "Linea_Classe_Cod");
    //creaKendoMultiselectServerFiltering("multiselAzienda", { read: [] }, 3, "Prodotto_Des", "Prodotto_Cod");

    creaKendoDropDownList("id_selListaReport", { read: RiempiListaReport }, "desc", "cod").bind("change", CambiaListaReport);

    $("#btn_salva").click(function () {
        var nome_report = KendoDDL("selListaReport").value();
        kendo.prompt("Inserire il nome del report (se già presente verrà sovrascritto)", nome_report).then(function (data) {
            if (data && data !== "") Salva_Report(data);
            else kendo.alert("Nome report non inserito");
        });
    });

    $("#btn_elimina").click(function () {
        var nome_report = KendoDDL("selListaReport").value();
        if (nome_report && nome_report !== "") {
            var kendoConfirm = $("<div></div>").kendoConfirm({
                title: "Report Analisi Progetti",
                messages: { okText: "Sì", cancel: "No" },
                content: "Vuoi eliminare il report \"" + nome_report + "\""
            }).data("kendoConfirm");
            kendoConfirm.result.done(function () { Cancella_Report(nome_report); });
            kendoConfirm.open();
        } else kendo.alert("Selezionare un report da cancellare.");
    });

    //eventi di click pulsanti
    $("#mostraTutto").click(function () {
        $("#configuratore_tab_riepilogo").show();
        $("#pivot_tab_riepilogo").show();
        $("#exportExcel").show();
        $("#exportPdf").hide()
        //$("#exportPdf").show();
        $("#grafico_tab_riepilogo").hide();
        $("#grafico_tab_config").hide();

        $("#mostraConfiguratore").removeClass('btn-danger');
        $("#mostraConfiguratore").addClass('btn-success');
        $("#mostraPivot").removeClass('btn-danger');
        $("#mostraPivot").addClass('btn-success');
        //if ($("#mostraGrafico").hasClass("btn-danger")) AggiornaGraficoPivot();
        $("#mostraGrafico").removeClass('btn-danger');
        $("#mostraGrafico").addClass('btn-success');        
    });
    $("#exportPdf").hide();

    $("#mostraConfiguratore").click(function () {
        $(this).toggleClass('btn-success btn-danger');
        $("#configuratore_tab_riepilogo").toggle();
    });

    $("#mostraPivot").click(function () {
        $(this).toggleClass('btn-success btn-danger');
        $("#pivot_tab_riepilogo").toggle();
        $("#exportExcel").toggle();
        //$("#exportPdf").toggle();
    });

    $("#mostraGrafico").click(function () {
        $("#grafico_tab_config").toggle();
        $("#grafico_tab_riepilogo").toggle();
        //if ($(this).hasClass("btn-danger")) AggiornaGraficoPivot();
        $(this).toggleClass('btn-success btn-danger');
    });

    $("#tab_testata_griglia_report_vendite").on("click", ".mostraGraficoReport", function () {
        $("#boxGraficoReport").toggle();
        $("#configGraficoReport").toggle();
        $("#graficoReportVendite").toggle();
        if ($(this).hasClass("btn-danger")) AggiornaGrafico();
        $(this).toggleClass('btn-success btn-danger');
    });

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    $.logThis("DocReady: FINE");

});