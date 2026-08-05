
var sezioniVuote = new Array();
var leftSwitchesControls = new Array();
var centralSwitchesControls = new Array();
var reportAbilitati = new Array();

//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    //INIZIALIZZA GLI SWITCH
    // Helper function to initialize switches from config
    function initializeSwitches(configArray, targetArray) {
        configArray.forEach(function(config) {
            targetArray.push(creaKendoSwitch(config.id, ...config.params));
        });
    }

    //sezioni vuote
    initializeSwitches(sezioniVuoteSwitchesConfig, sezioniVuote);

    //sinistra
    initializeSwitches(leftSwitchesConfig, leftSwitchesControls);
    
    //centro
    initializeSwitches(centralSwitchesConfig, centralSwitchesControls);

    //destra
    
    creaKendoSwitch("chkPrioritaColturePrecedenti", undefined, undefined, false);
    creaKendoSwitch("chkVisualizzaTipologieVarietali", undefined, undefined, false);
    creaKendoSwitch("chkVisualizzaCapitolatoPrivato", undefined, undefined, false);
    creaKendoSwitch("chkVisualizzaFinalita", undefined, undefined, false);
    creaKendoSwitch("chkRaggruppaXCampo", undefined, undefined, false);
    creaKendoSwitch("chkVisualizzaAcquaHa", undefined, undefined, false);
    creaKendoSwitch("chkStampaAnnoImpiantoPluriennali", undefined, undefined, true);
    creaKendoSwitch("chkMostraValoriSignificativiNeiRilievi", undefined, undefined, true);
    creaKendoSwitch("checkSuperfici", "Impianti", "Appezzamenti", true, undefined, 125, 12);
    creaKendoSwitch("checkData", "Intervallo date", "Data specifica", true, undefined, 135, 12);
    creaKendoSwitch("checkStampeMagazzino", "Scheda Fertilizzanti", "Scheda Fitosanitari", false, undefined, 180, 12);
    creaKendoSwitch("checkLogoRegione", "SI", "NO", true);

    creaKendoSwitch("check_RegCondizionalita", "SI", "NO", false);
    creaKendoSwitch("check_RegMisura10", "SI", "NO", false);
    creaKendoSwitch("check_RegPSR", "SI", "NO", false);


    creaKendoSwitch("checkMostraDataStampa", "SI", "NO", c_mostraDataOdierna);
    creaKendoSwitch("checkStampaODC", "SI", "NO", c_mostaFirmaODC);
    creaKendoSwitch("checkImpostaOrganismoReferente", undefined, undefined, c_impostaOrganismoReferente);

    if ($(cId_RegolamentiDiv).val() == "visible") {
        mostraRegolamenti();
    }
    if ($(cId_OrdinamentoDiv).val() == "visible") {
        $("#DivOrdinamento").show();
    }  
    if($(cId_TempoDiRientroDiv).val() == "visible" || $(cId_GlobalGapDiv).val() == "visible") {
        $('#divGlobalTempo').css('display', 'flex');
    }
    
    if ($(cId_TempoDiRientroDiv).val() == "visible") {
        $("#DivTempoRientro").show();
    }
    
    if (!c_mostaFirmaODC) {
        $("#divCheckStampaODC").hide();
    }
    
    if ($(cId_GlobalGapDiv).val() == "visible") {
        $('#DivGlobalGap').css('display', '');
        creaKendoSwitch("check_globalgap", undefined, undefined, true);
    }

    //if ($(cIdReportSelezionato).val() == enum_CodificaStampe.Eurep_Gap_Multicentro ||
    //    $(cIdReportSelezionato).val() == enum_CodificaStampe.Eurep_Gap) {

    //    $("#divcheckVisualizzaLotto").show();

    //    creaKendoSwitch("checkVisualizzaLotto", undefined, undefined, true);
    //} else {
    //    $("#divcheckVisualizzaLotto").hide();
    //}



    //1) disabilito gli options da tenere disabilitati by design 
    for (let i = 0; i < leftSwitchesControls.length; i++) {
        if (leftSwitchesControls[i].element[0].value == ElencoReportChiave.MANUTEN_MACCHINARI ||
            leftSwitchesControls[i].element[0].value == ElencoReportChiave.MACCH_SOLO_DIFESA ||
            leftSwitchesControls[i].element[0].value == ElencoReportChiave.VISITE_ISPETTIVE)
            leftSwitchesControls[i].enable(false);
    }

    setKendoSwitchVisible(centralSwitchesControls[0].element[0].name, false);
    //Prima c'era solo la linea sopra, visto che nessuno di quei switch e' visibile per la stampa Scheda Colturale Bio, li disattivo tutti fino a quando 
    //Questa pagina non le gestira tutte le stampe.
    // for (let i = 0; i < 4; i++) {
    //     setKendoSwitchVisible(centralSwitchesControls[i].element[0].name, false);
    //     //document.getElementById(`lblAvversitaQta${i}`).style.display = 'none';
    // }
    document.getElementById("lblAvversitaQta").style.display = 'none';

    // Modifiche ad hoc per colture bio, finche la pagina non gestira tutto
    // document.getElementById("lblVisualizzaCapitolatoPrivato").style.display = 'none';
    // setKendoSwitchVisible("chkVisualizzaCapitolatoPrivato", false);
    // setKendoSwitch("chkRaggruppaXCampo", true)


    function applyPermissions(sezioniConPermessi) {
        debugger;
        if (sezioniConPermessi) {
            const arrayElencoSezioniConPermessi = sezioniConPermessi.split(",");
            arrayElencoSezioniConPermessi.forEach(function(sezione) {
                const divId = "#div_" + sezione;
                const div = $(divId);
                if (div.length) { 
                    div.show();
                }
            });
        }
    }

    function enableControlsByReport(reportAbilitatiValue) {
        const arrayReportAbilitati = reportAbilitatiValue ? reportAbilitatiValue.split(",") : [];
        
        leftSwitchesControls.forEach(function(control) {
            const isEnabled = arrayReportAbilitati.includes(control.element[0].value);
            control.enable(isEnabled);
            if (!isEnabled) {
                control.check(false);
            }
        });

        // If reportAbilitatiValue is null or empty, all leftSwitchesControls should be disabled and unchecked.
        // The loop above already handles this: if arrayReportAbilitati is empty, isEnabled will always be false.
        if (!reportAbilitatiValue) {
             leftSwitchesControls.forEach(function(control) {
                control.enable(false);
                control.check(false);
            });
        }
    }

    function configureUIForReport() {
        const reportSelezionato = $(cIdReportSelezionato).val();
        const visualizzaImpiantiFiltrati = $(cId_VisualizzaImpiantiFiltrati).val();
        const sezione = $(cId_Sezione).val();
        const vegCod = $(cId_VegCod).val();
        const gruCod = $(cId_gruCod).val();
        const flagErbOrt = $(cId_flagErbOrt).val();
        const sezioniConPermessiValue = $(cId_ElencoSezioneSuPermessi).val();
        const reportAbilitatiValue = $(cId_ElencoReportAbilitati).val();


        // 2) Verifico sezioni da mostrare in base ai permessi  
        applyPermissions(sezioniConPermessiValue);

        // 3) abilito tutti gli option in base alla lista dei report abilitati 
        enableControlsByReport(reportAbilitatiValue);
        
        // 4) PARTE CHE SI OCCUPA DELLA SELEZIONE DEGLI SWITCHES E CONTROLLI
        if (sezione == "") {
            switch (reportSelezionato) {
                case enum_CodificaStampe.Registro_Trattamenti_Massivo.toString():
                case enum_CodificaStampe.Registro_Fertilizzazioni_Massivo.toString():
                case enum_CodificaStampe.Registro_Fertilizzazioni.toString():
                case enum_CodificaStampe.RegistroTrattamenti_Semplificata.toString():
                case enum_CodificaStampe.RegistroTrattamenti_Veneto.toString():
                case enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita.toString():
                case enum_CodificaStampe.SchedaCampagna_Multi_Lombardia.toString():
                case enum_CodificaStampe.RegistroAziendaleUnico.toString():
                    $("#divCentroAziendale").hide();
                    $("divSpecieVegetale").hide();
                    break;
                default:
                    if (visualizzaImpiantiFiltrati == "1") {
                        $("#divCentroAziendale").hide();
                        $("#divSpecieVegetale").hide();
                    } else {
                        $("#divCentroAziendale").show();
                        $("#divSpecieVegetale").show();
                    }
                    // Initialize DDLs only if they are visible or needed
                    //if ($("#divCentroAziendale").is(":visible") || $("#divSpecieVegetale").is(":visible") || visualizzaImpiantiFiltrati == "1") {
                     //   Inizializza_Ddl_Specie_Vegetali();
                    //}
                    break;
            }

            if($(cId_Lista_Sa_Cod_Da_Variabili_Stampe).val() != ""){
                Inizializza_Ddl_Centri_Aziendali();
            }     
            if($(cId_Lista_Veg_Cod_Da_Variabili_Stampe).val() != ""){
                Inizializza_Ddl_Specie_Vegetali();
            }
            
            if (reportSelezionato == enum_CodificaStampe.SchedaInterventiAgronomici.toString()) {
                setKendoSwitchVisible("checkImpostaOrganismoReferente", true);
                $("#divImpostaOrganismoReferente").show();
            }

            if (reportSelezionato != enum_CodificaStampe.SchedaCampagna_Biologico.toString()) {
                if (document.getElementById("lblchkRaggruppaXCampo")) {
                    document.getElementById("lblchkRaggruppaXCampo").style.display = 'none';
                }
                setKendoSwitchVisible("chkRaggruppaXCampo", false);
            }
            if (reportSelezionato == enum_CodificaStampe.SchedaCampagna_Multi_Lombardia.toString()) {
                setKendoSwitch("checkLogoRegione", false);
            }

            if (reportSelezionato == enum_CodificaStampe.RegistroTrattamenti_Veneto.toString() ||
                reportSelezionato == enum_CodificaStampe.SchedaCampagna_Biologico.toString()) {
                setKendoSwitchVisible("chkTutteRaccolte", false);
                setKendoSwitchVisible("chkQtaQtaRaccolte", false);
                setKendoSwitchVisible("chkDataUltimaRaccolta", false);
                if (document.getElementById("lblTutteRaccolte")) document.getElementById("lblTutteRaccolte").style.display = 'none';
                if (document.getElementById("lblQtaQtaRaccolte")) document.getElementById("lblQtaQtaRaccolte").style.display = 'none';
                if (document.getElementById("lblDataUltimaRaccolta")) document.getElementById("lblDataUltimaRaccolta").style.display = 'none';
            }

            if (reportSelezionato == enum_CodificaStampe.SchedaCampagna_Multicentro_ACA.toString() ||
                reportSelezionato == enum_CodificaStampe.SchedaCampagna_Biologico.toString()) {
                $("#divMostraDataStampa").show();
            }

            const enumTarget = [
                enum_CodificaStampe.Eurep_Gap_Semplificata,
                enum_CodificaStampe.SchedaCampagna_Pizzoli,
                enum_CodificaStampe.Eurep_Gap_Multicentro,
                enum_CodificaStampe.SchedaCampagna_ProvAut_Trento,
                enum_CodificaStampe.SchedaCampagna_Multicentro,
                enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                enum_CodificaStampe.SchedaRegistrazione_Semplificata,
                enum_CodificaStampe.SchedaCampagna_ConserveItalia
            ];

            if (enumTarget.map(String).includes(reportSelezionato)) {
                $("#divChkVisualizzaCapitolatoPrivato").show();
            }

            if (reportSelezionato == enum_CodificaStampe.RegistroTrattamenti_Veneto.toString()) {
                creaKendoSwitch("check_manutenzione_veneto", undefined, undefined, false);
                creaKendoDropDownListWithData("ddlSchede", schedaVeneto);
                if (document.getElementById("btn_daStampareAncheSeVuote")) {
                    document.getElementById("btn_daStampareAncheSeVuote").style.display = 'none';
                }
                VisualizzaSezionixRegTrattVeneto();
            } else {
                SelezionaTutte(reportSelezionato);
                // Chiamata aggiunta per impostare le sezioni vuote predefinite
                impostaSezioniVuotePredefinite(reportSelezionato);
            }
        } else {
            //abilita switches in base ai report abilitati (this part seems redundant if enableControlsByReport is called earlier)
            // reportAbilitati = $(cId_ElencoReportAbilitati).val().split(",");
            // for (let i = 0; i < reportAbilitati.length; i++) { // This loop might be incorrect as reportAbilitati is an array of strings, not indices
            //     if (reportAbilitati[i] == leftSwitchesControls[i].element[0].value) // Comparing string with control value
            //         leftSwitchesControls[i].check(true);
            //     else
            //         leftSwitchesControls[i].check(false);
            // }
             if (reportAbilitatiValue) {
                const arrayReportAbilitati = reportAbilitatiValue.split(",");
                leftSwitchesControls.forEach(function(control) {
                    if (arrayReportAbilitati.includes(control.element[0].value)) {
                        control.check(true);
                    } else {
                        control.check(false);
                    }
                });
            }
        }

        if (sezione == "m") {
            leftSwitchesControls.forEach(function(control) {
                if (control.element[0].value == 'm') {
                    control.check(true);
                } else {
                    control.check(false);
                }
            });
        }

        if (vegCod == "-1") {
            leftSwitchesControls.forEach(function(control) {
                control.enable(false);
            });
        }

        if (gruCod == 1 && flagErbOrt == "False") {
            $("#divRotazione").hide();
            setKendoSwitch("chkPrioritaColturePrecedenti", false);
        }
    }
 
    configureUIForReport(); // Call the main configuration function
    // Assicurarsi che impostaSezioniVuotePredefinite sia chiamata anche qui
    // se configureUIForReport non la chiama in tutti i casi necessari al primo caricamento.
    // Tuttavia, la logica attuale di configureUIForReport dovrebbe coprire il caricamento iniziale.
    // Se necessario, si può aggiungere una chiamata esplicita qui:
    // impostaSezioniVuotePredefinite($(cIdReportSelezionato).val());
 
    nascondiVecchiControlli(true);
    mostraData(false);

    var switchData = $("#checkData").data("kendoSwitch");
    switchData.bind("change", function () {
        mostraData(!this.check()); // Simplified
    });

    var switchTratta = $("#check_trattamenti").data("kendoSwitch");
    switchTratta.bind("change", SelezionaDeselezionaFito);

    
    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31) //Larghezza calendario come il campo di input...
        //        open: function () {
        //            var calendar = this.dateView.calendar;
        //            calendar.wrapper.width(this.wrapper.width() - 6);
        //        }
    });


    var inizio = new Date(new Date().getFullYear(), 0, 1);
    var fine = new Date(new Date().getFullYear(), 11, 31);
    var data_singola = kendo.date.today();
    set_data("txtValiditaInizio", inizio, null);
    set_data("txtValiditaFine", fine, null);
    set_data("txtStampaGiorno", data_singola, null);

    creaKendoDropDownListWithData("ddlRegioni", regioni);
    Leggi_Regione_Appartenenza();
    creaKendoDropDownListWithData("ddlArrotondamento", arrotondamenti);
    creaKendoDropDownListWithData("ddlOrdinamento", ordinamenti);
    caricaMagazzini();
    //creaKendoDropDownListWithData("ddlMagazzino", magazzini);
    Set_KendoDDLValue("ddlArrotondamento", "3", null);
    Set_KendoDDLValue("ddlOrdinamento", "0", null);

    const gruCod = $(cId_gruCod).val();
    const flagErbOrt = $(cId_flagErbOrt).val(); 

    const gruCodNumerico = parseInt(gruCod, 10);
    const flagErbOrtBooleano = (flagErbOrt === "True"); 

    $("#toolTip").kendoTooltip({
        showOn: "click mouseenter",
        content:"La stampa definitiva salverà una copia del file nella cartella " + $(cId_pathAllegati).val()
    });
        
    $("#btn_stampaProva").click(stampaNewTrue);
    $("#btn_stampaDefinitiva").click(stampaNewFalse);

    var windowOptions = {
        actions: ["Minimize", "Maximize", "Close"],
        draggable: true,
        resizable: true,
        width: "500px",
        title: "Inserisci manualmente le colture precedenti",
        visible: false,
        scrollable: true
    };

    var windowOptionsElenRota = {
        actions: ["Custom", "Minimize", "Maximize", "Close"],
        draggable: true,
        resizable: true,
        width: "500px",
        title: "Elenco Report",
        visible: false
    };

    var windowOptionsGlobalGap = {
        actions: ["Minimize", "Maximize", "Close"],
        draggable: true,
        resizable: true,
        width: "1000px",
        height: "250px",
        title: "Inserisci dati GLOBAL GAP",
        visible: false
    };

    $("#globalGapWindow").kendoWindow(windowOptionsGlobalGap);

    $("#tabellaRotazione").kendoWindow(windowOptions);
    $("#elencoReportWindow").kendoWindow(windowOptionsElenRota);

    var windowOptions = {
        actions: ["Minimize", "Maximize", "Close"],
        draggable: true,
        resizable: true,
        width: "550px",
        height: "450px",
        title: "Sezioni Da stampare anche se vuote",
        visible: false
    };
    $("#SezioniVuoteWindow").kendoWindow(windowOptions);
    $.logThis("DocReady: FINE");
});
