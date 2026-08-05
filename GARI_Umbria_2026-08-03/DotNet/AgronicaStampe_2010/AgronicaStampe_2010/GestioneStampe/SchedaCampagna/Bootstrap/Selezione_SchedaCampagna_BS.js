function changeChk(e) {
    let nameServerElem = e.sender.element.data("name_server_elem");
    let serverElem = $("#" + nameServerElem);
    serverElem.attr("checked", e.checked);
}
function mostraRegolamenti(){
    $("#divcheck_RegPSR").show();
    $("#divCheck_RegCondizionalita").show();
    $("#divcheck_RegMisura10").show();
    $("#check_RegCondizionalita").data("kendoSwitch").toggle();
    $("#check_RegPSR").data("kendoSwitch").toggle();
    $("#check_RegMisura10").data("kendoSwitch").toggle();
    
}
function caricaMagazzini() {
    //var options = Leggi_Magazzini();
    //var opt;
    //let str = "["
    //for (var i = 0, iLen = options.length; i < iLen; i++) {
    //    opt = options[i];
    //    var temp = '{"text": "' + opt.text + '", ' + '"value": "' + opt.value + '"},'
    //    str = str + temp
    //}
    //str = str.slice(0, -1) + "]"
    //console.log(str)
    //magazzini = JSON.parse(str);
    $("#ddlMagazzino").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Sa_Nome",
        dataValueField: "Sa_Cod",
        dataSource: { transport: { read: Leggi_Magazzini } },
        open: kendoDropDownAdjustWidth
    }).data("kendoDropDownList");
}
 // TODO UHALID, da confermare con la funzione SEZIONI_SelezionaTutte
function attivaDisattivaSezioniDaStampare(status) {

    for (var i = 0; i < leftSwitchesControls.length; i++) {
        if (leftSwitchesControls[i].options.enabled)
            leftSwitchesControls[i].check(status);
    }
}
function mostraData(status) {
    if (status == true) {
        $("#Data").show();
        $("#Data_Inizio").hide();
        $("#Data_Fine").hide();
    }
    if (status == false) {
        $("#Data").hide();
        $("#Data_Inizio").show();
        $("#Data_Fine").show();
    }
}
function stampaMagazzinoFalse() {
    stampaMagazzinoNew(false);
}
function stampaMagazzinoTrue() {
    stampaMagazzinoNew(true);
}
function stampaMagazzinoNew(SchedaFertilizzanti) {
    //CONTROLLA SELEZIONE
    isDdlSelected = Get_KendoDDLValue('ddlMagazzino');
    if (isDdlSelected == '-1') {
        kendo.alert("Selezionare un magazzino");
        return;
    }
    //RACCOGLIE I SETTAGGI UTENTE 
    var isStampaVeneto = false;
    if ($(cIdReportSelezionato).val() == enum_CodificaStampe.RegistroTrattamenti_Veneto)
        isStampaVeneto = true;

    var parametriServer = Prepara_Oggetto_Parametri_Stampa(false, SchedaFertilizzanti);
    if (parametriServer === null)
        return;

    //MANDA AL CONTROLLO PRE STAMPA
    Controlli_PreStampa_New(parametriServer, isStampaVeneto, true);

}
function stampaNewFalse() {
    stampaNew(false);
}
function stampaNewTrue() {
    stampaNew(true);
}
function stampaNew(StampaProva) {

    //RACCOGLIE I SETTAGGI UTENTE 
    var isStampaVeneto = false;
    if ($(cIdReportSelezionato).val() == enum_CodificaStampe.RegistroTrattamenti_Veneto)
        isStampaVeneto = true;

    var parametriServer = Prepara_Oggetto_Parametri_Stampa(StampaProva, false);
    if (parametriServer === null)
        return;
    //MANDA AL CONTROLLO PRE STAMPA
    Controlli_PreStampa_New(parametriServer, isStampaVeneto, false);
}

function _getKendoSwitchStateAndJQueryValue(switchId) {
    return {
        state: getKendoSwitch(switchId),
        valueAttribute: $("#" + switchId).attr("Value")
    };
}

function Prepara_Oggetto_Parametri_Stampa(StampaProva, SchedaFertilizzanti) {
    var parametri = new Object();
    var switchData;

    // Helper function to add switch parameters that require both state and .attr("Value")
    function addSwitchParamWithAttrValue(paramBaseName, switchId) {
        switchData = _getKendoSwitchStateAndJQueryValue(switchId);
        parametri[paramBaseName] = switchData.state;
        parametri[paramBaseName + "_value"] = switchData.valueAttribute;
    }

    //SWITCHES SELEZIONE STAMPA (sezioni):
    addSwitchParamWithAttrValue("check_frontespizio", "check_frontespizio");
    addSwitchParamWithAttrValue("check_personale", "check_personale");
    addSwitchParamWithAttrValue("check_dati_catastali", "check_dati_catastali");
    addSwitchParamWithAttrValue("check_semine", "check_semine");
    addSwitchParamWithAttrValue("check_Fertilizzazioni", "check_Fertilizzazioni");
    addSwitchParamWithAttrValue("check_trattamenti", "check_trattamenti");
    addSwitchParamWithAttrValue("check_fitoregolatori", "check_fitoregolatori");
    addSwitchParamWithAttrValue("check_fasi_fenologiche", "check_fasi_fenologiche");
    addSwitchParamWithAttrValue("check_trappole", "check_trappole");
    addSwitchParamWithAttrValue("check_ril_avver_trappole", "check_ril_avver_trappole");
    addSwitchParamWithAttrValue("check_ril_avver_campo", "check_ril_avver_campo");
    addSwitchParamWithAttrValue("check_irrigazione", "check_irrigazione");
    addSwitchParamWithAttrValue("check_operazioni_colturali", "check_operazioni_colturali");
    addSwitchParamWithAttrValue("check_ind_maturita", "check_ind_maturita");
    addSwitchParamWithAttrValue("check_rilievo_prod", "check_rilievo_prod");
    addSwitchParamWithAttrValue("check_piogge", "check_piogge");
    addSwitchParamWithAttrValue("check_informazioni", "check_informazioni");
    addSwitchParamWithAttrValue("check_manutenzione", "check_manutenzione");
    addSwitchParamWithAttrValue("check_visite_ispettive", "check_visite_ispettive");
    addSwitchParamWithAttrValue("check_trattamenti_post_raccolta", "check_trattamenti_post_raccolta");
addSwitchParamWithAttrValue("check_formazione", "check_formazione");
    addSwitchParamWithAttrValue("check_gestione_rifiuti", "check_gestione_rifiuti");
addSwitchParamWithAttrValue("check_pratiche_ecologiche", "check_pratiche_ecologiche");

    // Switches that only need getKendoSwitch()
    parametri.check_verifiche_conf = getKendoSwitch("check_verifiche_conf");
    parametri.chkAvversitaQta = getKendoSwitch("chkAvversitaQta");
    parametri.chkTutteRaccolte = getKendoSwitch("chkTutteRaccolte");
    parametri.chkQtaQtaRaccolte = getKendoSwitch("chkQtaQtaRaccolte");
    parametri.chkDataUltimaRaccolta = getKendoSwitch("chkDataUltimaRaccolta"); // Original had this ID twice, keeping one instance
    parametri.checkLogoRegione = getKendoSwitch("checkLogoRegione");


    //SWITCHES OPZIONI STAMPA:
    if ($(cId_GlobalGapDiv).val() == "visible") {
        parametri.checkGlobalGap = getKendoSwitch("check_globalgap");
        if (!kendo.parseInt($("#txt_tempo_rientro").val())) {
            kendo.alert("Inerire valore corretto in Tempo Rientro");
            return null;
        }
        parametri.tempoRientro = $("#txt_tempo_rientro").val();
    }
    parametri.chkPrioritaColturePrecedenti = getKendoSwitch("chkPrioritaColturePrecedenti");
    parametri.chkVisualizzaTipologieVarietali = getKendoSwitch("chkVisualizzaTipologieVarietali");
    parametri.chkVisualizzaCapitolatoPrivato = getKendoSwitch("chkVisualizzaCapitolatoPrivato");
    parametri.chkVisualizzaFinalita = getKendoSwitch("chkVisualizzaFinalita");
    parametri.chkRaggruppaXCampo = getKendoSwitch("chkRaggruppaXCampo");
    parametri.chkVisualizzaAcquaHa = getKendoSwitch("chkVisualizzaAcquaHa");
    parametri.chkStampaAnnoImpiantoPluriennali = getKendoSwitch("chkStampaAnnoImpiantoPluriennali");
    parametri.chkMostraValoriSignificativiNeiRilievi = getKendoSwitch("chkMostraValoriSignificativiNeiRilievi");
    parametri.checkSuperfici = getKendoSwitch("checkSuperfici");
    parametri.checkData = getKendoSwitch("checkData");
    parametri.checkStampaProva = StampaProva;
    //parametri.checkLogoRegione = getKendoSwitch("checkLogoRegione");
    parametri.checkStampeMagazzino = SchedaFertilizzanti;
    parametri.reportSelezionato = $(cIdReportSelezionato).val();

    parametri.checkMostraFirmaODC = getKendoSwitch("checkStampaODC");
    parametri.checkMostraDataDiStampa = getKendoSwitch("checkMostraDataStampa");
    parametri.checkImpostaOrganismoReferente = getKendoSwitch("checkImpostaOrganismoReferente");
    
    parametri.checkRegCondizionalita = getKendoSwitch("check_RegCondizionalita");
    parametri.checkRegPSR = getKendoSwitch("check_RegPSR");
    parametri.checkRegMisura10 = getKendoSwitch("check_RegMisura10");
    //parametri.checkVisualizzaLotto = getKendoSwitch("checkVisualizzaLotto") == null ? false : getKendoSwitch("checkVisualizzaLotto");

    //ALTRI CONTROLLI:
    parametri.ddlRegioni = Get_KendoDDLValue("ddlRegioni");
    parametri.ddlArrotondamento = Get_KendoDDLValue("ddlArrotondamento");
    parametri.ddlMagazzino = Get_KendoDDLValue("ddlMagazzino");
    parametri.ddlOrdinamento = Get_KendoDDLValue("ddlOrdinamento");

    //MAGAZZINI
    parametri.piva = $(cIds_piva).val();


    //IMPIANTI FILTRATI
    parametri.ddlCentroAziendale = Get_KendoDDLValue("ddlCentroAziendale");
    parametri.ddlSpecieVegetale = Get_KendoDDLValue("ddlSpecieVegetale");
    parametri.UtilizzaImpiantiFiltrati = $(cId_VisualizzaImpiantiFiltrati).val();

    // SEZIONI VUOTE
    parametri.ElencoSezioniVuote = elencoSezioniVuote;

    // Parametri per stampa Veneto
    parametri.schedaVeneto = "";
    if ($(cIdReportSelezionato).val() == enum_CodificaStampe.RegistroTrattamenti_Veneto) {
        parametri.schedaVeneto = Get_KendoDDLValue("ddlSchede");
        if (parametri.schedaVeneto == "") {
            kendo.alert("Selezionare una scheda");
            return null;
        }
    }
    if (parametri.checkData == true) {
        parametri.txtValiditaInizio = KendoDate("txtValiditaInizio").value();
        parametri.txtValiditaFine = KendoDate("txtValiditaFine").value();
        //controllo date
        var diff = Math.round(parametri.txtValiditaInizio - parametri.txtValiditaFine);
        if (diff == 0) {
            kendo.alert('date incongruenti'); //stessa data
            return null;
        }
        if (diff > 0) {
            kendo.alert('date incongruenti'); //la data iniziale è temporalmente maggiore della data finale
            return null;
        }
    }
    else
        parametri.txtStampaGiorno = KendoDate("txtStampaGiorno").value();

    var parametriEscaped = kendoEscapeOggetto(parametri);
    var parametriServer = " { objParams: '" + parametriEscaped + "' }";

    return parametriServer;
}
function SelezionaDeselezionaFito() {

    var switchTrattamenti = $("#check_trattamenti").data("kendoSwitch");
    var switchFitoregolatori = $("#check_fitoregolatori").data("kendoSwitch");
    var checked = switchTrattamenti.check();
    if (!checked) {
        switchFitoregolatori.check(false);
        switchFitoregolatori.enable(false);
    }
    else {
        switchFitoregolatori.check(true);
        switchFitoregolatori.enable(true);
    }

}
function SelezionaTutte(report) {
    var currentReport = parseInt(report);
    var config = reportSelezionaTutteConfig[currentReport] || reportSelezionaTutteConfig.defaultCase;

    for (let i = 0; i < leftSwitchesControls.length; i++) { // Iterate all controls
        const control = leftSwitchesControls[i];
        const controlValue = control.element[0].value;

        if (config.checkAllEnabled) {
            if (control.options.enabled) {
                control.check(true); // Check if enabled by default
            }
        } else {
            control.check(false); // Uncheck all by default if checkAllEnabled is false
        }

        // Apply specific settings from sectionsToSet
        if (config.sectionsToSet.hasOwnProperty(controlValue)) {
            if (control.options.enabled) { // Only change if enabled
                 control.check(config.sectionsToSet[controlValue]);
            }
        }
    }

    // Common logic for VERIFICHE_CONFORMITA, always set to false if it exists
    for (let i = 0; i < leftSwitchesControls.length; i++) {
        if (leftSwitchesControls[i].element[0].value == ElencoReportChiave.VERIFICHE_CONFORMITA) {
            leftSwitchesControls[i].check(false);
            break; 
        }
    }

    // Special handling for FITOFARMACI based on TRATTAMENTI, if not RegistroTrattamenti_Veneto
    if (currentReport != enum_CodificaStampe.RegistroTrattamenti_Veneto) {
        let trattamentiChecked = false;
        // First pass to find if TRATTAMENTI is checked
        for (let i = 0; i < leftSwitchesControls.length; i++) {
            if (leftSwitchesControls[i].element[0].value == ElencoReportChiave.TRATTAMENTI) {
                trattamentiChecked = leftSwitchesControls[i].check();
                break;
            }
        }
        // Second pass to set FITOFARMACI if TRATTAMENTI was checked
        if (trattamentiChecked) {
            for (let i = 0; i < leftSwitchesControls.length; i++) {
                if (leftSwitchesControls[i].element[0].value == ElencoReportChiave.FITOFARMACI) {
                    if (leftSwitchesControls[i].options.enabled) {
                        leftSwitchesControls[i].check(true);
                    }
                    break;
                }
            }
        }
    }

    // Special handling for chkAvversitaQta visibility
    if (currentReport == enum_CodificaStampe.SchedaCampagna_Multicentro ||
        currentReport == enum_CodificaStampe.RegistroAziendaleUnico ||
        currentReport == enum_CodificaStampe.Eurep_Gap_Multicentro) {
        for (let i = 0; i < centralSwitchesControls.length; i++) { 
            if (centralSwitchesControls[i].element[0].name == "chkAvversitaQta") {
                setKendoSwitchVisible(centralSwitchesControls[i].element[0].name, true);
                
                if (document.getElementById("lblAvversitaQta")) {
                    document.getElementById("lblAvversitaQta").style.display = 'inline';
                }
                if($(cId_StampaCampagna_Default_Vedi_AvversitaQta).val() == "1"){
                    setKendoSwitch("chkAvversitaQta", true);
                }
                
                break; 
            }
        }
    }
}
function VisualizzaSezionixRegTrattVeneto() {
    document.getElementById("sezioniNonVeneto-left").style.display = "none";
    document.getElementById("sezioniNonVeneto-right").style.display = "none";
    document.getElementById("sezioniVeneto-left").style.display = "inline";
    document.getElementById("sezioniVeneto-right").style.display = "inline";
}
function impostaSezioniVuotePredefinite(codiceReport) {
    var sezioniDaPreselezionare = [];
    var reportSelezionato = parseInt(codiceReport);

    // Determina le sezioni da preselezionare in base al report
    // Questa logica ora utilizza reportSelezionaTutteConfig da Selezione_SchedaCampagna_BS_globali.js
    // e in particolare la configurazione per SchedaInterventiAgronomici
    if (reportSelezionato === enum_CodificaStampe.SchedaInterventiAgronomici) {
        // Le sezioni predefinite per SchedaInterventiAgronomici sono definite in reportSelezionaTutteConfig
        // e sono 'a', 'b', 'c', 'f', 'g', 'h', 'i', 'm', 'n', 'r'
        // Queste corrispondono ai 'value' dei checkbox nella modale SezioniVuoteWindow
        const configReport = reportSelezionaTutteConfig[enum_CodificaStampe.SchedaInterventiAgronomici];
        if (configReport && configReport.sezioniVuotePredefinite) { // Aggiungo un controllo per la proprietà
            sezioniDaPreselezionare = configReport.sezioniVuotePredefinite;
        } else {
            // Fallback o logica alternativa se sezioniVuotePredefinite non è definito per questo report
            // Per ora, usiamo l'elenco fornito dall'utente come fallback per SchedaInterventiAgronomici
            // se non trovato specificamente in reportSelezionaTutteConfig
             sezioniDaPreselezionare = ['a', 'b', 'c', 'f', 'g', 'h', 'i', 'm', 'n', 'r'];
        }
    }
    // Altrimenti, per altri report, potremmo voler deselezionare tutto o mantenere lo stato corrente.
    // Per ora, se non è SchedaInterventiAgronomici, non facciamo nulla o deselezioniamo tutto.
    // Qui scegliamo di non fare nulla, lasciando che l'utente gestisca manualmente per altri report.

    // Aggiorna i checkbox nella modale SezioniVuoteWindow e la variabile globale/hidden field
    // La variabile 'sezioniVuote' (array di Kendo Switch) è definita in Selezione_SchedaCampagna_BS_jQueryDocReady.js
    // e inizializzata con gli switch della modale.
    var sezioniVuoteAttualmenteSelezionate = [];
    if (typeof sezioniVuote !== 'undefined' && Array.isArray(sezioniVuote)) {
        sezioniVuote.forEach(function(switchControl) {
            var switchValue = switchControl.element.attr("value");
            var deveEssereSelezionato = sezioniDaPreselezionare.includes(switchValue);
            switchControl.check(deveEssereSelezionato);
            if (deveEssereSelezionato) {
                sezioniVuoteAttualmenteSelezionate.push(switchValue);
            }
        });
    }

    // Aggiorna la variabile globale e il campo nascosto
    elencoSezioniVuote = sezioniVuoteAttualmenteSelezionate.join('|');
    
    // Tentativo di aggiornare il campo nascosto.
    // L'ID del campo hidden in ASPX è "checkedSezioneVuote".
    // ASP.NET potrebbe aggiungere prefissi (es. ctl00_MainContent_checkedSezioneVuote).
    // Usiamo un selettore "ends with" per maggiore robustezza.
    var hiddenField = $('input[type="hidden"][id$="checkedSezioneVuote"]');
    if (hiddenField.length) {
        hiddenField.val(elencoSezioniVuote);
    } else {
        // Fallback se il selettore "ends with" non funziona (improbabile per controlli server-side)
        // console.warn("Campo hidden 'checkedSezioneVuote' non trovato con il selettore 'ends with'.");
    }
    //console.log("Sezioni vuote predefinite impostate per report " + codiceReport + ": " + elencoSezioniVuote);
}
function Inizializza_Ddl_Centri_Aziendali() {
    $("#ddlCentroAziendale").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Sa_Nome",
        dataValueField: "Sa_Cod",
        dataSource: { transport: { read: Leggi_Centri_Aziendali } },
        open: kendoDropDownAdjustWidth
    }).data("kendoDropDownList");

}
function Inizializza_Ddl_Specie_Vegetali() {
    $("#ddlSpecieVegetale").kendoDropDownList({
        filter: "contains",
        autoBind: true,
        dataTextField: "Sv_Nome",
        dataValueField: "Sv_Cod",
        dataSource: { transport: { read: Leggi_Specie_Vegetali } },
        open: kendoDropDownAdjustWidth
    }).data("kendoDropDownList");
}
function aggiungiSezioneVuote() {
    let str = ""
    for (let i = 0; i < sezioniVuote.length; i++) {
        var checked = sezioniVuote[i].check();
        if (checked) {
            str += sezioniVuote[i].element[0].value + '|';
        }
    }
    elencoSezioniVuote = str.slice(0, -1);
    $("#SezioniVuoteWindow").data("kendoWindow").close();

}
function annullaSezioniVuote() {
    for (let i = 0; i < sezioniVuote.length; i++) {
        var checked = sezioniVuote[i].check();
        if (checked)
            sezioniVuote[i].check(false);
    }
    if (elencoSezioniVuote != "")
        elencoSezioniVuote = "";
    $("#SezioniVuoteWindow").data("kendoWindow").close();
}
function DatiGlobalGap() {
    var dati = document.getElementById('txt_revisione').value;
    if (dati != "")
        SalvaDatiGlobalGap(dati);
}
function ChiudiGlobalGap() {
    $("#globalGapWindow").data("kendoWindow").close();
}
function ChiudiColture() {
    $("#tabellaRotazione").data("kendoWindow").close();
}
function DatiColture() {
    //inserire il salvataggio

    $("#tabellaRotazione").data("kendoWindow").close();
}
//utility
function nascondiVecchiControlli(status) {

    //controlli    
    var elencoReportWindow = document.getElementById("hide");

    if (status == true) {

        elencoReportWindow.style.display = 'none';

    }
    if (status == false) {

        elencoReportWindow.style.display = 'inline';
    }
}

function kReadValorizzazioneGrid_rows(options) {

    let data = $('#hdKendoTabellaRotazioneValore').val();
    let jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}
function kReadValorizzazioneGridElRe_rows(options) {

    let data = $('#hdKendoTabellaElencoReport').val();
    let jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}
function onEditNumeratoriTipi(e) {

    e.container.find("input[name='Coltura1']").attr('maxlength', '10');
    e.container.find("input[name='Coltura2']").attr('maxlength', '10');
    e.container.find("input[name='Coltura3']").attr('maxlength', '10');
    e.container.find("input[name='Coltura4']").attr('maxlength', '10');



}
function SubmitCulturePrecedenti(options) {
    var grid = $("#divKendoRotazione").data("kendoGrid");
    var currentData = grid.dataSource.data();

   
    

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];

    // modificate / inserite
    for (let i = 0; i < currentData.length; i++) {

        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
    }

    // cancellate
    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {

        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        var righeInserite = kendoEscapeOggetto(newRecords);
        var righeModificate = kendoEscapeOggetto(updatedRecords);
        var righeCancellate = kendoEscapeOggetto(deletedRecords);

        var allOk = false;

        var objParametri = new Object();
        objParametri.RigheInserite = righeInserite;
        objParametri.RigheModificate = righeModificate;
        objParametri.RigheCancellate = righeCancellate;

        var paramEscaped = kendoEscapeOggetto(objParametri);
        var param = "{paramString: '" + paramEscaped + "'}";

      
        ajaxAgronicaSync(indirizzohttp + "/SalvaColturePrecedenti",
            param, false,
            function (risposta) {
                allOk = true;
                //grid.dataSource._destroyed = [];
                //grid.dataSource.read();
                //grid.refresh();
                Leggi_ColturePrecedenti();
                MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggipopup");
            }, function (risposta) {
                var errori = risposta.RispostaStringa + ' ' + risposta.Errore;
                MessaggioErrore_Bootstrap(errori, "DIV_Messaggi");
            });

        if (!allOk) {
            erroreSubmitGriglia(grid);
        }
        
        
    }
}
function Cultura_DropDownEditor(container) {
    if (ElencoCulture == null || ElencoCulture == undefined)
        ElencoCulture = CaricaComboSpecieVegetaliDestinazioneUso();

    creaDropDownEditor(container, "Coltura1", "CodColtura1", ElencoCulture, changeCulture, false);

}
function Cultura_DropDownEditor2(container) {
    if (ElencoCulture == null || ElencoCulture == undefined)
        ElencoCulture = CaricaComboSpecieVegetaliDestinazioneUso();

    let ElendoCodici = [];
    for (var x = 0; x < ElencoCulture.length; x++) {
        ElendoCodici.push({ "CodColtura2": ElencoCulture[x]["CodColtura1"], "Coltura2": ElencoCulture[x]["Coltura1"] });
    }

    creaDropDownEditor(container, "Coltura2", "CodColtura2", ElendoCodici, changeCulture2, false);

}
function Cultura_DropDownEditor3(container) {
    if (ElencoCulture == null || ElencoCulture == undefined)
        ElencoCulture = CaricaComboSpecieVegetaliDestinazioneUso();

    let ElendoCodici = [];
    for (var x = 0; x < ElencoCulture.length; x++) {
        ElendoCodici.push({ "CodColtura3": ElencoCulture[x]["CodColtura1"], "Coltura3": ElencoCulture[x]["Coltura1"] });
    }

    creaDropDownEditor(container, "Coltura3", "CodColtura3", ElendoCodici, changeCulture3, false);

}
function Cultura_DropDownEditor4(container) {
    if (ElencoCulture == null || ElencoCulture == undefined)
        ElencoCulture = CaricaComboSpecieVegetaliDestinazioneUso();

    let ElendoCodici = [];
    for (var x = 0; x < ElencoCulture.length; x++) {
        ElendoCodici.push({ "CodColtura4": ElencoCulture[x]["CodColtura1"], "Coltura4": ElencoCulture[x]["Coltura1"] });
    }

    creaDropDownEditor(container, "Coltura4", "CodColtura4", ElendoCodici, changeCulture4, false);

}


function changeCulture(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#divKendoRotazione").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.CodColtura1 = dataItem.CodColtura1;
    model.Coltura1 = dataItem.Coltura1;
    model.dirty = true;
}
function changeCulture2(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#divKendoRotazione").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.CodColtura2 = dataItem.CodColtura2;
    model.Coltura2 = dataItem.Coltura2;
    model.dirty = true;
}
function changeCulture3(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#divKendoRotazione").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.CodColtura3 = dataItem.CodColtura3;
    model.Coltura3 = dataItem.Coltura3;
    model.dirty = true;
}
function changeCulture4(e) {

    var dataItem = e.sender.dataItem();
    var grid = $("#divKendoRotazione").data("kendoGrid");
    var model = grid.dataItem(this.element.closest("tr"));
    model.CodColtura4 = dataItem.CodColtura4;
    model.Coltura4 = dataItem.Coltura4;
    model.dirty = true;
}




function kendoCulture_inizializza(divKendo, keys) {

    let jSonParsed_Kendo = JSON.parse(keys);
    var campiKendoModel = jSonParsed_Kendo.kendo_model;


    //let columns = jSonParsed_Kendo.kendo_columns;
    var funzioniCRUD = { funzioneRead: kReadValorizzazioneGrid_rows };

    if (ElencoCulture == null || ElencoCulture == undefined) {
        ElencoCulture = CaricaComboSpecieVegetaliDestinazioneUso();
        ElencoCulture.push({ "CodColtura1": "", "Coltura1": ""});
        for (var RO = 0; RO < jSonParsed_Kendo.kendo_rows.length; RO++) {
            var TrovatoElemento = false;
            for (var x = 0; x < ElencoCulture.length; x++) {
                if (ElencoCulture[x]["Coltura1"] === jSonParsed_Kendo.kendo_rows[RO]["Coltura1"]) {
                    TrovatoElemento = true;
                }
            }
            if (!TrovatoElemento) {
                ElencoCulture.push({ "CodColtura1": jSonParsed_Kendo.kendo_rows[RO]["Coltura1"], "Coltura1": jSonParsed_Kendo.kendo_rows[RO]["Coltura1"] });
            }
            TrovatoElemento = false;
            for (var x = 0; x < ElencoCulture.length; x++) {
                if (ElencoCulture[x]["Coltura1"] === jSonParsed_Kendo.kendo_rows[RO]["Coltura2"]) {
                    TrovatoElemento = true;
                }
            }
            if (!TrovatoElemento) {
                ElencoCulture.push({ "CodColtura1": jSonParsed_Kendo.kendo_rows[RO]["Coltura2"], "Coltura1": jSonParsed_Kendo.kendo_rows[RO]["Coltura2"] });
            }
            TrovatoElemento = false;

            for (var x = 0; x < ElencoCulture.length; x++) {
                if (ElencoCulture[x]["Coltura1"] === jSonParsed_Kendo.kendo_rows[RO]["Coltura3"]) {
                    TrovatoElemento = true;
                }
            }
            if (!TrovatoElemento) {
                ElencoCulture.push({ "CodColtura1": jSonParsed_Kendo.kendo_rows[RO]["Coltura3"], "Coltura1": jSonParsed_Kendo.kendo_rows[RO]["Coltura3"] });
            }
            TrovatoElemento = false;
            for (var x = 0; x < ElencoCulture.length; x++) {
                if (ElencoCulture[x]["Coltura1"] === jSonParsed_Kendo.kendo_rows[RO]["Coltura4"]) {
                    TrovatoElemento = true;
                }
            }
            if (!TrovatoElemento) {
                ElencoCulture.push({ "CodColtura1": jSonParsed_Kendo.kendo_rows[RO]["Coltura4"], "Coltura1": jSonParsed_Kendo.kendo_rows[RO]["Coltura4"] });
            }
        }
        ElencoCulture.sort(function (a, b) {
            if (a.Coltura1 > b.Coltura1) return 1;
            else if (a.Coltura1 < b.Coltura1) return -1;
            return 0;
        }); //contollare il sort kiwano
    }

    var colonneKendoGrid = [
        { field: 'Sa_Nome', title: 'Centro Aziendale', width: 150 },
        { field: 'Campo_Des', title: 'Campo', width: 150 },
        { field: 'App_Nome', title: 'App.', width: 150 },
        { field: 'Veg_Des', title: 'Specie', width: 150 },
        { field: 'Cul_Des', title: 'Varietà', width: 150 },
        { field: 'Dest_Uso', title: 'Dest. Uso', width: 150 },
        { field: 'Sup_Imp', title: 'Sup.Imp. [ha]', width: 150 },
        { field: 'Validita', title: 'Validità Impianto', width: 150 },
        { field: "Coltura1", title: "Coltura Precedente", editor: Cultura_DropDownEditor, width: 150 },
        { field: "Coltura2", title: "Coltura Precedente 2", editor: Cultura_DropDownEditor2, width: 150 },
        { field: "Coltura3", title: "Coltura Precedente 3", editor: Cultura_DropDownEditor3, width: 150 },
        { field: "Coltura4", title: "Coltura Precedente 4", editor: Cultura_DropDownEditor4, width: 150 }
    ];

    var colCustKendoGrid = [
        {
            command: [
                {
                    iconClass: "fa fa-pencil fa-xs", className: "blockModifica", name: "edit", text: { edit: "", update: "Conf.", cancel: "Ann." }
                }
            ],
            title: "Operazioni", width: "150px"
        }
    ];



    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };
    var parametriKendoGrid = {
        editable: {
            mode: "inline"
        },
        columnMenu: false,
        impostaColonneKendoGridDaCookie: false,
        excel: false,
        pdf: false,
        sortable: true,
        groupable: false,
        reorderable: false,
        salvaRipristinaPersonalizzazioni: false,
        scrollable: true,
        selectable: "row",
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: false,
        btnEliminaTuttiFiltri: false,
        colonneCustomKendoGrid: colCustKendoGrid,
        height: "100%"

    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: function (e) {
            //Se in mobile mostro solo la colonna unica
            //mostraColonnaUnicaSeInMobile(e, 1);
            //autoFitSeMobile(e);

            //Accorcio l'altezza delle righe
            //riduciAltezzaRighe(e, 1);

            //    if (keys != undefined) {
            //        var arrKeys = keys.split(",");
            //        var grid = $("#" + divKendo).data("kendoGrid");
            //        var data = grid.dataSource.data();
            //        for (var i = 0; i < arrKeys.length; i++) {
            //            for (var j = 0; j < data.length; j++) {
            //                if (data[j].chiave == arrKeys[i]) {
            //                    var rowUid = data[j].uid;
            //                    var row = grid.table.find("[data-uid=" + rowUid + "]");
            //                    grid.select(row);
            //                }
            //            }
            //        }
            //    }
            //    nascondiBottoniProdotto();
        },
        funzioneDaChiamareDopoEdit: null //onEditNumeratoriTipi
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];
    var funzioniCRUD = {
        funzioneRead: kReadValorizzazioneGrid_rows,
        funzioneSubmit: { funzione: SubmitCulturePrecedenti, flagInsert: false, flagUpdate: true, flagDelete: false },
        UtenteAbilitatoInserimentoModifica: true

    };

    KendoOperazioni = creaKendoGrid(divKendo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        "KeyReg", // chiave riga
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


function kendoElencoReport_inizializza(divKendo, keys) {
    let jSonParsed_Kendo = JSON.parse(keys);
    var campiKendoModel = jSonParsed_Kendo.kendo_model;
    var colonneKendoGrid = [
        { field: 'Piva', title: 'Piva Aziendale', width: 150 },
        { field: 'Documento_Cod', title: 'Documento', width: 150 },
        { field: 'SottoCartella', title: 'SottoCartella.', width: 150 },
        { field: 'NomeFile', title: 'NomeFile', width: 150 },
        { field: 'Inizio', title: 'Inizio', width: 150 },
        { field: 'Fine', title: 'Fine', width: 150 }
    ];

    var templateVisualizzaProdotto = "<span class='fa fa-info fa-2x info_elem' title='Scarica file pdf' onclick=ScaricaFile(this.closest('tr'),this.closest('.k-grid'),0)></span>";
    var colCustKendoGrid = [
        {
            command: [
                {
                    template: templateVisualizzaProdotto
                }
            ],
            title: "Dowload", width: "150px"
        }
    ];



    var parametriPerLettura = [];
    var parametriDataSource = { pagesize: 50 };
    var parametriKendoGrid = {
        editable: {
            mode: "inline"
        },
        columnMenu: false,
        impostaColonneKendoGridDaCookie: false,
        excel: false,
        pdf: false,
        sortable: true,
        groupable: false,
        reorderable: false,
        salvaRipristinaPersonalizzazioni: false,
        scrollable: true,
        selectable: "row",
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 },
        filterable: false,
        btnEliminaTuttiFiltri: false,
        colonneCustomKendoGrid: colCustKendoGrid,

    };
    var funzioniPrimaDopoEventi = {
        funzioneDaChiamareDopoDataBound: function (e) {
        },
        funzioneDaChiamareDopoEdit: null //onEditNumeratoriTipi
    };
    var mostraRigheCancellate = true;
    var colonneDisabilitateSoloInModifica = [];
    var funzioniCRUD = {
        funzioneRead: kReadValorizzazioneGridElRe_rows,
        UtenteAbilitatoInserimentoModifica: true

    };

    KendoOperazioni = creaKendoGrid(divKendo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        "PIva", // chiave riga
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
