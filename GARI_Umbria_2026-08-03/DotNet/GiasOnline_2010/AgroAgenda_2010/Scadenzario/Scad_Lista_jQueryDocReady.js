
//DOCUMENT READY
$(document).ready(async function () {


    $.logThis("DocReady: INIZIO");
    WaitFrame.show();

    currentPiva = JSON.parse(objP_agenda).Piva;

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            scadListaResx.unshift(readResxFile(resxSinglePath, "Scad_Lista_jQueryDocReady.js"));
        });
    }

    if ($("input[name$='hf_UploadMultiploAllegatiAbilitato']").val() == "True") {
        UploadMultiploAllegatiAbilitato = true;
    }


    // Anna 29/04/22: aggiunti campi al filtro di ricerca
    Elenco_Validazioni = [
        { "Validazione_Flag": 0, "Validazione_Des": TraduzioneMultiResx(scadListaResx, "DocumentoDaValidare", "Documento da validare") },
        { "Validazione_Flag": -1, "Validazione_Des": TraduzioneMultiResx(scadListaResx, "DocumentoNonValidoUfficio", "Documento non valido ufficio") },
        { "Validazione_Flag": -2, "Validazione_Des": TraduzioneMultiResx(scadListaResx, "DocumentoNonValidoAutocontrollo", "Documento non valido autocontrollo") },
        { "Validazione_Flag": 1, "Validazione_Des": TraduzioneMultiResx(scadListaResx, "DocumentoValidoUfficio", "Documento valido ufficio") },
        { "Validazione_Flag": 2, "Validazione_Des": TraduzioneMultiResx(scadListaResx, "DocumentoValidoAutocontrollo", "Documento valido autocontrollo") }
    ];
    Validazioni_Filtro = [
        { "Validazione_Cod": -10, "Validazione_Des": "" }, //obj vuoto per il filtro
        { "Validazione_Cod": 0, "Validazione_Des": TraduzioneMultiResx(scadListaResx, "DocumentoDaValidare", "Documento da validare") },
        { "Validazione_Cod": -1, "Validazione_Des": TraduzioneMultiResx(scadListaResx, "DocumentoNonValidoUfficio", "Documento non valido ufficio") },
        { "Validazione_Cod": -2, "Validazione_Des": TraduzioneMultiResx(scadListaResx, "DocumentoNonValidoAutocontrollo", "Documento non valido autocontrollo") },
        { "Validazione_Cod": 1, "Validazione_Des": TraduzioneMultiResx(scadListaResx, "DocumentoValidoUfficio", "Documento valido ufficio") },
        { "Validazione_Cod": 2, "Validazione_Des": TraduzioneMultiResx(scadListaResx, "DocumentoValidoAutocontrollo", "Documento valido autocontrollo") }
    ];
    Elenco_Storico = [
        { "Storico_Cod": -1, "Storico_Des": TraduzioneMultiResx(scadListaResx, "Tutti", "Tutti") }, //obj vuoto per il filtro
        { "Storico_Cod": 1, "Storico_Des": TraduzioneMultiResx(scadListaResx, "SoloStoricizzati", "Solo storicizzati") },
        { "Storico_Cod": 0, "Storico_Des": TraduzioneMultiResx(scadListaResx, "SoloNonStoricizzati", "Solo non storicizzati") }
    ];


    //Se l'utente è abilitato aggiungo il pulsante per l'inserimento di nuove Scadenze
    UtenteAbilitatoScrittura = $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True";
    if (UtenteAbilitatoScrittura) {
        $("#BtnScad_ImportaScad").show();
        if ($("input[name$='hf_AbilitazioneDocumentiAPP']").val() == "True") {
            $("#BtnImportaApp").show();
        }
    }

    creaKendoSwitch(
        "ChkSoloAttive",
        TraduzioneMultiResx(scadListaResx, "Si", "Sì"),
        TraduzioneMultiResx(scadListaResx, "No", "No"),
        false,
        async function (e) {
            await ddlAzienda_Load($(cPiva).val());
        });

    creaKendoSwitch(
        "chkAziendaCorrente",
        TraduzioneMultiResx(scadListaResx, "Si", "Sì"),
        TraduzioneMultiResx(scadListaResx, "No", "No"),
        false,
        async function (e) {
            if (e.checked) {
                let currentPiva = JSON.parse(objP_agenda).Piva
                if (currentPiva != '') {
                    let ds = KendoDDL("ddlAzienda").dataSource._data
                    if (ds.filter((ds) => ds.piva == currentPiva))
                        await Set_KendoDDLValueVirtual("ddlAzienda", currentPiva)
                }
            } else {
                await Set_KendoDDLValueVirtual("ddlAzienda", '')
            }
        });

    //Carico la combo delle aziende
    await ddlAzienda_Load($(cPiva).val());

    //Carico le combo di cateogrie e tipologie
    Elenco_Aree = Elenco_Aree_Riempi(true);
    creaKendoDropDownList("cmbArea", { read: RiempicmbArea }, "Nome", "ID_Area").bind("change", CmbArea_change);
    creaKendoMultiselect("cmbTipologia", { read: RiempicmbTipologia }, "Nome", "ID_Tipologia");


    // Se ci sono solo due valori (valore effettivo + objVuoto) lo imposto in automatico
    if (KendoDDL("cmbArea").dataSource._data.length === 2) {
        Set_KendoDDLValue("cmbArea", KendoDDL("cmbArea").dataSource._data[1].ID_Area);
        CmbArea_change();
    }

    GestionePannelli();

    //Applico il calendario            
    $("#txt_Validita_Inizio").kendoDatePicker();
    $("#txt_Validita_Fine").kendoDatePicker();

    creaKendoSwitch("ChkValidazione", TraduzioneMultiResx(scadListaResx, "Si", "Sì"), TraduzioneMultiResx(scadListaResx, "No", "No"), false, function (e) { });

    //Anna 29/04/22: aggiunti campi al filtro di ricerca
    creaKendoDropDownList("cmbValidazione", { read: RiempicmbValidazione }, "Validazione_Des", "Validazione_Cod");

    $("#txt_Inizio_upload").kendoDateTimePicker();
    $("#txt_Fine_upload").kendoDateTimePicker();

    var eseguiRicercaxDashboard = false
    if ($(cDataUpload).val() !== "") {
        $("#txt_Inizio_upload").val($(cDataUpload).val())
        eseguiRicercaxDashboard = true
    }

    creaKendoSwitch("chkUtenteUpload", TraduzioneMultiResx(scadListaResx, "Si", "Sì"), TraduzioneMultiResx(scadListaResx, "No", "No"), false, function (e) {
        //if (e.checked) { } else { }
    });

    creaKendoSwitch("ChkGestioneStorico", TraduzioneMultiResx(scadListaResx, "Si", "Sì"), TraduzioneMultiResx(scadListaResx, "No", "No"), false, function (e) {
        if (e.checked) {
            bcheckstoricoabilitato = true;
            popolaGrigliaScadenze("tabella_scadenze", true);
        } else {
            bcheckstoricoabilitato = true;
            popolaGrigliaScadenze("tabella_scadenze", false);
        }
    });
    $("#GestioneStorico").hide();


    if ($(cAllegato_Validazione_Visibilita).val() === "False") {
        $("#lblValidazione").hide();
        setKendoSwitchVisible("ChkValidazione", false);

    }

    if ($(cAllegato_Validazione).val() === "False") {
        $("#ChkValidazione").data("kendoSwitch").enable(false);

    }


    //Anna 02/05/22: modificato campo Storico in DDL
    creaKendoDropDownList("ddlStorico", { read: RiempiDDLStorico }, "Storico_Des", "Storico_Cod");
    $("#ddlStorico").data("kendoDropDownList").value(0);

    // nella nuova versione il panel è sostituito dalla sidebar destra
    if (GiasVersioneMaster !== "2022") {
        $("#panelFiltriRicerca").kendoPanelBar({
            expandMode: "multiple",
            select: function (e) {
                $("#panelFiltriRicerca > li > span").addClass(GIAS_K_STATE_SELECTED);
            }
        });
        $("#panelFiltriRicerca > li > span").addClass(GIAS_K_STATE_SELECTED);
    }
    // Introdotto per mostrare il pannello solo quando tutti i controlli sono caricati
    document.getElementById("pnlFiltriRicerca").style.opacity = "1";


    //Controllo Provenienza (se da Ricerca Documenti, Analisi, Documenti Contabili o UMA...)


    if (!isNaN($(cArea_Provenienza).val()) && $(cArea_Provenienza).val() != 0) {
        //Preselezione Area
        KendoDDL("cmbArea").value(parseInt($(cArea_Provenienza).val()));
        CmbArea_change();

        //Blocco Controllo
        KendoDDL("cmbArea").enable(false);
        KendoDDL("ddlAzienda").enable(false);

        $("#lblSoloAttive").hide();
        setKendoSwitchVisible("ChkSoloAttive", false);
        setKendoSwitchVisible("chkAziendaCorrente", false);

        if ($(cArea_Provenienza).val() == 3) {

            //Analisi del Terreno

            //Preselezione Tipologia
            if ($(cTipologia_Provenienza).val() == -4) {

                KendoMultisel("cmbTipologia").value("-4");

                KendoMultisel("cmbTipologia").enable(false);
            }
        }


        if ($(cArea_Provenienza).val() == 7) {

            // UMA - Report Controllo

            //Preselezione Tipologia
            if ($(cTipologia_Provenienza).val() !== undefined && $(cTipologia_Provenienza).val() !== 0 && $(cTipologia_Provenienza).val() !== '0') {

                KendoMultisel("cmbTipologia").value($(cTipologia_Provenienza).val());

                KendoMultisel("cmbTipologia").enable(false);
            }
        }

        //ANNA 03 / 24 -- WIP ALLINEAMENTO PATENTINO  PASSANDO DA DOCUMENTALE.INTERROTTO, RIPRENDERE IN FUTURO!
        if ($(cCod_Contatto).val() !== "" && $(cArea_Provenienza).val() !== 0) {
            KendoMultisel("cmbTipologia").value("-1"); //PATENTINI
            KendoMultisel("cmbTipologia").enable(false);
        }

        //Avvio Ricerca
        eseguiRicercaScadenze();
    }


    //eventi di click pulsanti
    $("#btn_Ricerca").click(
        function () {
            eseguiRicercaScadenze();
        }
    );

    if (eseguiRicercaxDashboard) {
        eseguiRicercaScadenze();
    } else {
        if ($(cArea_Provenienza).val() == 0) {
            //SetParametri();
            //Anna Lettura filtri/personalizzazioni griglia
            await Leggi_Filtri();
        }
    }

    //$(".btn_scarica_file").click(
    //    function () {
    //        scaricaZip(false);
    //        //$("btn_scarica_file").enable(false);
    //        $(this).prop("disabled", true);
    //    }
    //);

    //$(".btn_scarica_ZIP").click(
    //    function () {
    //        scaricaZip(true);         
    //    }
    //);


    //VENGO DALL'AUDIT -- NASCONOD PANNELLO FILTRI
    if ($(cSito_Provenienza).val() == '7') {
        $("#pnlFiltriRicerca").css('display', 'none')
        $("#xoRicercaDocToggleFiltri").css('display', 'none')
    }

    window.addEventListener('message', event => {

        let kWin = $('#GestionePassaggioDiStatoWindowDocumentale').data("kendoWindow");
        let urlKWin = kWin.options.content.url;

        if (verificaOriginSecondaria(window, urlKWin, event) && (typeof event.data == "string") && event.data.includes("RispostaStringa")) {
            let respMsg = JSON.parse(event.data);
            switch (respMsg.Tipo) {
                case 'Profilazione_PassaggioStato':
                    $(document.body).append('<div id="alert"></div>');
                    $("#alert").kendoAlert({
                        content: respMsg.RispostaStringa,
                        actions: [{
                            text: "Ok",
                            action: function (e) {
                                chiudiWindowPassaggioDiStato();
                            }
                        }]
                    }).data("kendoAlert").open();
                    break;
                default:
                    console.log("Evento non gestito")
            }
        }
        //alert("message")
    });

    WaitFrame.hide();
    $.logThis("DocReady: FINE");

});

