var promises;

var ddlContatto1Nome = "inCedenteCessionario1";
var ddlContatto2Nome = "inCedenteCessionario2";
var ddlContattoCoop1Nome = "inCedenteCessionarioAcc3";
var ddlContattoCoop2Nome = "inCedenteCessionarioAcc4";

var btnFiltraContatti1XRaccolte = "btn_cedente_cessionario_1_raccolte";
var btnFiltraContatti2XRaccolte = "btn_cedente_cessionario_2_raccolte";

var btnLockContatti1XCompliantISCC = "btn_cedente_cessionario_1_compliant_iscc";
var btnLockContatti2XCompliantISCC = "btn_cedente_cessionario_2_compliant_iscc";

var txtPivaNome = "inPivaCf";
var txtProgressivoNome = "inProgressivo";

var txtAttivitaNome = "inAttivita";

var pivaPadreGerarchiaConf = "";
var pivaPadreGerarchiaCoop1 = "-1";     //imposto così, perché se "", quando non ho scelto la CONFERENTE verrebbe fuori tutto l'elenco, invece dovrebbe essere vuoto
var pivaPadreGerarchiaCoop2 = "-1";     //imposto così, perché se "", quando non ho scelto la COOP1 verrebbe fuori tutto l'elenco, invece dovrebbe essere vuoto
var pivaPadreGerarchiaProd = "-1";

//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            resxObj.push(readResxFile(resxSinglePath, "DocContabile_jQueryDocReady.js"));
        });
    }

    // leggiArrayCostanti.js -> Inizializzo le costanti in lingua
    Inizializza_enum_LavCod(resxObj);
    Inizializza_enum_udm(resxObj);
    Inizializza_elencoTempoCarenza(resxObj);
    Inizializza_elenchiPrezzoLivello(resxObj);
    Inizializza_elencoScontoMaggiorazione(resxObj);
    Inizializza_elencoScontoModalita(resxObj);

    // Converto i moduli attivi in integer
    for (let i = 0; i < modulo_anagrafe_log.length; i++) {
        modulo_anagrafe_log[i] = parseInt(modulo_anagrafe_log[i]);
    }

    // Su nuovo se non è presente il centro aziendale viene aperta la finestra di selezione (se sono più di 1)
    selezionaCentro = selezionaCentro && cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value && Qs_SaCod == "0";

    $("#selectCentro").kendoWindow({
        title: TraduzioneMultiResx(resxObj, "SelezionaCentroAziendale", "Seleziona Centro Aziendale"),
        // actions: ["Maximize", "Close"],
        close: function (e) {
            if (Qs_SaCod != "0") {
                CaricaInterfacciaDocumento();
            } else {
                Azione_Indietro_DocContabile();
            }
        },
        modal: true,
        visible: false,
        width: "80%",
        maxWidth: 640,
        //height: 180
    });

    $("#btn_selectCentro").kendoButton({
        enable: false
    });

    $("#btn_selectCentro").click(function () {
        $("#selectCentro").data("kendoWindow").close();
    });

    $("#btn_annullaCentro").click(function () {
        //document.location = "../Menu/MenuBS_2017.aspx";
        //$("#selectCentro").data("kendoWindow").close();
        Azione_Indietro_DocContabile();
    });

    if (selezionaCentro) {
        SelezionaCentro();
    }

    $("#btnSalvaDistanzaDoc").kendoButton({
        enable: true
    });
    

    //WaitFrame.show();

    if (isContrattoAffitto()) {
        idControlloCausaleTrasporto = "inCausaleTrasportoAffitto";
    }

    if (isLavCodForzaGestioneSemplificata() === true) {
        gestioneContabilita = enum_Livello_GestContabilita_NonGestita;
    } else {
        gestioneContabilita = parseInt(GetPropertyFromJson($(cIdOpzioniContab).val(), "SUPERUSER_LIVELLO_GESTIONE_CONTABILITA"));
    }
    
    contattiAcc4ConGerarchia = GetPropertyFromJson($(cIdOpzioniContab).val(), "SUPERUSER_ACCETTAZIONE_CON_GERARCHIA");
    FF_gest_materiale_vivaistico = GetPropertyFromJson($(cIdOpzioniContab).val(), "SUPERUSER_FF_GEST_MATERIALE_VIVAISTICO");
    flagPesiRiscontrati = GetPropertyFromJson($(cIdOpzioniContab).val(), "SuperUser_PesiColli_Riscontrati");
    gestionePesiRiscontrati = flagPesiRiscontrati && (cIdLavCod === enum_LavCod.DDT_Emesso.value || cIdLavCod === enum_LavCod.DDT_Contabilizzato_Emesso.value);
    flagSceltaImputazione = GetPropertyFromJson($(cIdOpzioniContab).val(), "SUPERUSER_DocContabili_SceltaImputazione") && (cIdLavCod === enum_LavCod.DDT_Emesso.value || cIdLavCod === enum_LavCod.DDT_Contabilizzato_Emesso.value || cIdLavCod === enum_LavCod.Ordine_Acquisto.value);

    tabStrip_Dettagli = $("#tabstrip_dettagli").kendoTabStrip({
        animation: false,
        select: onSelectTabStripDettagli
        //show: onShow,
    }).data("kendoTabStrip");

    $("#panelbarTestata").kendoPanelBar({
        expandMode: "multiple",
        select: onSelectPanelBar
    }).data("kendoPanelBar");

    $("#panelbarFormProdottoUC").kendoPanelBar({
        expandMode: "multiple",
        select: onSelectPanelBar
    }).data("kendoPanelBar");

    $("#panelbarRiepilogoPesi").kendoPanelBar({
        expandMode: "multiple"
    }).data("kendoPanelBar");

    $("#tabs a").on("click", function (e) {
        var tabName = e.target.innerText.toUpperCase();
    });

    $('#tabs a[data-toggle="tab"]').on("shown.bs.tab", onTabPrincipaliShown);

    //if (parseInt($(cIdAgenda).val()) === 0) {
    //    //if (gestioneContabilita === enum_Livello_GestContabilita_NonGestita) {
    //    //    $("#a_tabDettagliDoc").tab("show");
    //    //    $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_Riepilogo]).attr("style", "display:none");
    //    //    $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_Dettaglio]).attr("style", "display:inline-block");
    //    //    $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ScaricoGiacenza]).attr("style", "display:none");
    //    //    tabStrip_Dettagli.select(index_tabStrip_Dettagli_Dettaglio);
    //    //}
    //    //else
    //    $("#a_tabTestataDoc").tab("show");
    //} else {
    //    $("#a_tabDettagliDoc").tab("show");
    //}

    nascondiControlliVisualizzazioneSemplificata();

    lavCodAccettazione = isLavCodAccettazione(cIdLavCod);
    lavCodAccettazionePomodoro = lavCodAccettazione && $(cTipoAccettazione).val() == "-1";
    lavCodOrdine = isLavCodOrdine(cIdLavCod);
    lavCodFattura = isLavCodFattura(cIdLavCod);
    lavCodDocEmesso = isNumeroDocumentoEmesso(cIdLavCod);
    lavCodVendita = isDocumentoVendita(cIdLavCod);
    lavCodMovMagazzino = isMovimentoMagazzino(cIdLavCod);

    // In base all'impostazione SU definisce se applicare il Server Filteringo su Contatt1 e Contatto2
    applicaServerFilteringSuCedenteCessionario = GetPropertyFromJson($(cIdOpzioniContab).val(), "RICERCA_CON_FILTRO_CESSIONARIO_DOC_CONT");
    if (applicaServerFilteringSuCedenteCessionario) {
        // Gestita solo in inserimento e modifica
        if (cIdTipoOp !== enum_TipoOperazioneDB.Scrittura.value && cIdTipoOp !== enum_TipoOperazioneDB.Modifica.value)
            applicaServerFilteringSuCedenteCessionario = false;
        // Non gestita per conferimenti a 4 livelli
        if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true)
            applicaServerFilteringSuCedenteCessionario = false;
    }

    // Questa impostazione è valida solo in caso di s/carichi
    impedisciCreazioneCarichiMultiriga = lavCodMovMagazzino === true && GetPropertyFromJson($(cIdOpzioniContab).val(), "ImpedisciCreazioneCarichiMultiriga") == true;

    if (lavCodMovMagazzino === true || isContrattoAffitto()) {
        $("#panelBarIntestatario").hide();
    }

    if (lavCodAccettazione === false || contattiAcc4ConGerarchia === false) {
        document.getElementById("panelBarCooperativa").remove();
        document.getElementById("panelBarCooperativa2").remove();
    }

    if (lavCodAccettazione === true && contattiAcc4ConGerarchia === true) {
        pivaPadreGerarchiaConf = $(cIdPiva).val();
    }

    if (lavCodAccettazione) {
        $("#lbl_data_spedizione").html(TraduzioneMultiResx(resxObj, "OraAccettazione", "Ora Accettazione"));
    }

    // Evita l'utilizzo dell'invio
    // TODO Stefano
    $(window).keydown(function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            return false;
        }
    });

    //imposto sola lettura oppure leggo le opzioni (non ho necessità di leggerle se sono in sola lettura)
    if (cIdTipoOp === enum_TipoOperazioneDB.Lettura.value)
        ImpostaReadOnly(true, true, true, true);

    //KENDO DatePicker***************************************

    //$(".kendoCalendar").kendoDatePicker({ footer: false });//Non mostra il footer
    $(".kendoCalendar").kendoDatePicker({ footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31),  
        format: "dd/MM/yyyy"
    });

    validitaSportello = RecuperaValiditaSportello($('input[name$="hf_Qs_ServizioCod"]').val());

    if (parseInt($(cOp).val()) == 0) {
        $(".kendoCalendarMM").kendoDatePicker({
            footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
            format: "dd/MM/yyyy"
        });
    } else {
        $(".kendoCalendarMM").kendoDatePicker({
            footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
            max: new Date(validitaSportello.Second),
            min: new Date(validitaSportello.First),
            format: "dd/MM/yyyy"
        });
    }

    KendoDate("inDataEmissione").bind("change", inDataEmissione_change);

    //Contratti di Affitto
    KendoDate("inDataInizVal").bind("change", inDataInizVal_change);
    KendoDate("inDataFineVal").bind("change", inDataFineVal_change);

    //kendoDateTimePicker
    $(".kendoCalendarTime").kendoDateTimePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31, 23, 59),
        format: "dd/MM/yyyy HH:mm"
    });

    //*******************************************************

    $("input[name='inTrasportoCura']").bind("change", inTrasportoCura_change);

    creaKendoDropDownListWithData("inTipologiaDocumento",
        CreaTipoDocumento(cIdLavCod, lavCodAccettazione)
    ).bind("change", inTipologiaDocumento_change);

    if (KendoDDL("inTipologiaDocumento").dataItems().filter(function (x) { return parseInt(x.value) === cIdLavCod; }).length === 0) {
        MessaggioErrore_Bootstrap("Il Lav Cod " + cIdLavCod + " non è consentito", "DIV_Messaggi");
    }

    Set_KendoDDLValue("inTipologiaDocumento", cIdLavCod);
    $("#inTipologiaDocumento").change();    //forzo il change

    //KENDO NumericTextBox***********************************
    $("#inNumDoc").kendoNumericTextBox({ format: "0", min: 0, decimals: 0, change: inNumDoc_change });
    $("#inNumDoc2").kendoNumericTextBox({ format: "0", min: 0, decimals: 0, change: inNumDoc2_change });
    $("#inNumRegistrazione").kendoNumericTextBox({ format: "0", min: 1, decimals: 0 });
    $("#inNumProtocollo").kendoNumericTextBox({ format: "0", min: 0, decimals: 0 });
    $("#inNumeroColli").kendoNumericTextBox({ format: "0", min: 0, decimals: 0 });
    $("#inPesoTotale").kendoNumericTextBox({ format: "###,##0.##", min: 0, decimals: 2 });
    $("#inPesoTaraTrasporto").kendoNumericTextBox({ format: "###,##0.##", min: 0, decimals: 2 });
    $("#ntbDistanzaTrasporto").kendoNumericTextBox({ format: "###,##0.###", min: 0, decimals: 3 });
    $("#inAgenteProvvigione").kendoNumericTextBox({ format: "0.##", min: 0, decimals: 2 });
    $("#inAgenteProvvigionePerc").kendoNumericTextBox({ format: "0.##\\%", min: 0, max: 100, decimals: 2 });
    $("#inCapoAreaProvvigione").kendoNumericTextBox({ format: "0.##", min: 0, decimals: 2 });
    $("#inCapoAreaProvvigionePerc").kendoNumericTextBox({ format: "0.##\\%", min: 0, max: 100, decimals: 2 });
    //*******************************************************

    $("#panelbarTestata").data("kendoPanelBar").bind("collapse", onCollapsePanelBar);
    $("#panelbarFormProdottoUC").data("kendoPanelBar").bind("collapse", onCollapsePanelBar);
    $("#panelbarRiepilogoPesi").data("kendoPanelBar").bind("collapse", onCollapsePanelBar);
    $("#panelbarTestata").data("kendoPanelBar").bind("expand", onExpandPanelBar);
    let pbFormProdottoUC = $("#panelbarFormProdottoUC").data("kendoPanelBar");
    pbFormProdottoUC.bind("expand", onExpandPanelBar);
    pbFormProdottoUC.bind("activate", onActivatePanelBar);
    $("#panelbarRiepilogoPesi").data("kendoPanelBar").bind("expand", onExpandPanelBar);
    $("#tabstrip_dettagli").data("kendoTabStrip").bind("show", onShowTabStrip);

    if (!selezionaCentro) CaricaInterfacciaDocumento();    

    $('body').kendoTooltip({
        filter: '.btn[title]'
    })
});

async function CaricaInterfacciaDocumento() {

    console.log("Sto per mostrare WaitFrame generale");

    WaitFrame.show();

    docReady().then(
        async function (result) {
            console.log("Ho terminato letture asincrone: " + result);

            KendoDDL(ddlContatto1Nome).select(-1);
            KendoDDL(ddlContatto2Nome).select(-1);
            // ----- INIZIO Caricamento impostazioni generali  ------------

            // INIZIO Impostazioni giacenze e lotti
            impostazioni_Blocca_SottoGiacenza = GetPropertyFromJson($(cIdOpzioniContab).val(), "UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE");
            impostazioni_Categorie_Giacenza = GetPropertyFromJson($(cIdOpzioniContab).val(), "UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE");
            impostazioni_Categorie_Lotti = GetPropertyFromJson($(cIdOpzioniContab).val(), "UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI");
            impostazioni_Edit_Lotto_Accettazione = GetPropertyFromJson($(cIdOpzioniContab).val(), "SUPERUSER_EDIT_LOTTO");
            impostazioni_DataScadenza_Lotto = GetPropertyFromJson($(cIdOpzioniContab).val(), "FiltroDataScadenzaFarmaco");
            // FINE Impostazioni giacenze e lotti

            // INIZIO Impostazione flag imballi FF
            // Vengono impostati purché siano gestiti in almeno una specie/varietà, perché servono per la ricerca
            if (paramQualGestiti_FF !== null && paramQualGestiti_FF.length !== 0) {
                for (let iPar = 0; iPar < paramQualGestiti_FF.length; iPar++) {
                    if (paramQualGestiti_FF[iPar].Tabella_ID !== 0) {
                        if (paramQualGestiti_FF[iPar].Tabella_ID == "4")
                            gestitoImballaggio_FF = true;
                        if (paramQualGestiti_FF[iPar].Tabella_ID == "8")
                            gestitoContenitore_FF = true;
                        if (paramQualGestiti_FF[iPar].Tabella_ID == "5")
                            gestitoConfezione_FF = true;

                        if (paramQualISCC.some(function (elem) { return elem === paramQualGestiti_FF[iPar].Tabella_Cod_Des; }))
                            gestitiParamQualISCC = true;
                    }
                }
            }
            // FINE Impostazione flag imballi FF
            // ----- FINE Caricamento impostazioni generali  ------------

            ImpostaOpzioniVisibilitaGenerale($(cIdOpzioniContab).val());

            if (isGestitoTabConfezionamento()) {
                if (parseInt($(cIdAgenda).val()) !== 0) {
                    $("#a_tabBeniConfezionamento").show();
                } else {
                    $("#a_tabBeniConfezionamento").hide();
                }
            } else {
                $("#a_tabBeniConfezionamento").hide();
            }

            if (lavCodMovMagazzino === true) {
                RendiObbligatorio(ddlContatto1Nome, false);
            }

            if (lavCodMovMagazzino === true || isContrattoAffitto()) {
                RendiObbligatorio("inTipoIndirizzoCC1", false);
            }

            if (lavCodAccettazionePomodoro) {
                //RendiObbligatorio(ddlContatto2Nome, true);
                if (contattiAcc4ConGerarchia) {
                    // RendiObbligatorio(ddlContattoCoop1Nome, true);
                }

                RendiObbligatorio("inDataSpedizione", true);
                $("#inDataSpedizione").addClass("TimePickerNoZero");
                KendoDateTime("inDataSpedizione").bind("change", VerificaDatiMinimiTestata);

                RendiObbligatorio("inMezzoTrasporto", true);
                $("#inMezzoTrasporto").bind("change", VerificaDatiMinimiTestata);

                RendiObbligatorio("inNImmatricolazioneRimorchio", true);
                $("#inNImmatricolazioneRimorchio").bind("change", VerificaDatiMinimiTestata);
            }

            if (result === true) {

                let idAgenda = parseInt($(cIdAgenda).val());

                if (idAgenda !== undefined && idAgenda !== null && !isNaN(idAgenda) && idAgenda !== 0) {

                    $.logThis("Modifica di documento: [" + idAgenda + "]");
                    ImpostaOpzioniVisibilitaOperazione($(cIdOpzioniContab).val(), cIdLavCod);
                    WaitFrame.show();
                    await caricaDocumento(parseInt(idAgenda));

                    ImpostaDefaultNumeratore(cIdLavCod, cIdTipoOp, "inNumDocDDL", "inNumDocSin", "inNumDocDes");

                    let mesAccessoNonConsentito = $(cIdMesAccessoNonConsentito).val();
                    if (mesAccessoNonConsentito !== undefined && mesAccessoNonConsentito !== null && mesAccessoNonConsentito !== "") {
                        $("<div></div>").kendoAlert({
                            title: TraduzioneMultiResx(resxObj, "OperazioneBloccata", "Operazione Bloccata"),
                            content: mesAccessoNonConsentito,
                            width: "400px",
                            actions: [
                                {
                                    text: TraduzioneMultiResx(resxObj, "TornaIndietro", "Torna Indietro"),
                                    primary: true,
                                    action: function (e) {
                                        Azione_Indietro_DocContabile();
                                        return true;
                                    }
                                }
                            ]
                        }).data("kendoAlert").open();

                        // Interrompo l'esecuzione del doc.ready
                        return false;
                    }

                } else {

                    $.logThis("Nuovo Documento");
                     
                    ImpostaOpzioniVisibilitaOperazione($(cIdOpzioniContab).val(), cIdLavCod);
                    ImpostaDefault($(cIdOpzioniContab).val(), cIdLavCod);

                    ImpostaDefaultNumeratore(cIdLavCod, cIdTipoOp, "inNumDocDDL", "inNumDocSin", "inNumDocDes");

                    //TODO: nei casi di accettazione il numero accettazione deve essere non modificabile e scritto su sequenza progressivi

                    //RefreshNumDoc();

                    if (isContrattoAffitto()) {
                        //RefreshNumDoc();
                        //RefreshNumDoc2();
                        Set_KendoNumTBValue("inNumDoc2", 0);
                    }

                    GetNewProgressivo();

                }

                //TODO: lettura di dati che servono nel dettaglio, spostata qui perché è necessario avere l'anno del documento
                RicercaAnniApertiConti(true, $(cIdPiva).val(), 0, false);

                let dataDoc = kendo.parseDate($("#inDataEmissione").val());
                let cauMov = $('input[name$="hf_Qs_CaricoScarico"]').val();
                let tipoDareAvereEco = getTipoDareAvere("ECO", cIdLavCod, cauMov);
                let tipoDareAverePat = getTipoDareAvere("PAT", cIdLavCod, cauMov);

                //TODO: in realtà andrebbe prima impostata la dropdown dell'anno e queste sono collegate a quella dell'anno
                RicercaContoEconomico(true, $(cIdPiva).val(), dataDoc.getFullYear(), tipoDareAvereEco, false, "", 0);
                RicercaContoPatrimoniale(true, $(cIdPiva).val(), dataDoc.getFullYear(), tipoDareAverePat, false, "", 0);

                creaControlli_FormProdottoUC();
                //if (parseInt($(cIdAgenda).val()) === 0)
                //    Imposta_Visibilita_FormProdottoUC(enum_TipoOperazioneDB.Scrittura.value);
                //else
                //    Imposta_Visibilita_FormProdottoUC(enum_TipoOperazioneDB.Lettura.value);

                //imposto l'asterisco sulla label dei campi required
                $("*[required]").each(function () {
                    $('label[for="' + this.name + '"]').addClass("campiObbligatori");
                });

                //elimina la validazione tramite plugin jQuery Validator impostata nella Master
                var validator = $("#aspnetForm").validate();
                validator.destroy();

                //visto che ho i dati di testata sparsi su più div, sono costretta ad avere dei validatori separati per ogni blocco 
                // per evitare sovrapposizioni con i dati di dettaglio
                validatorIntestazione = inizializzaKendoValidator("intestazione", true, true, false, false, true, true);
                validatorTabTestata = inizializzaKendoValidator("tabTestataDoc", true, false, false, true, false, true);

                $("#dialogSessioneScaduta").on("show.bs.modal",
                    function (event) {
                        impostaRedirectStart();
                    });

                //KENDOWINDOW
                $("#nuovoContattoWindow").kendoWindow({
                    actions: ["Close"],
                    //al momento il refresh non funzia ==> lo disabilito
                    //actions: [ "Close", "Refresh" ],
                    visible: false,
                    draggable: false,
                    height: "85%",
                    width: "90%",
                    modal: true,
                    resizable: false,
                    title: "Nuovo Contatto",
                    iframe: true,
                    open:
                        function (e) { //evita lo scrolling della pagina principale quando lo scrolling della modale raggiunge la fine
                            $("body").addClass("ob-no-scroll");
                        },
                    close: function (e) {
                        $("body").removeClass("ob-no-scroll");
                    }
                    //activate: function (e) {
                    //    var h = $("#nuovoContattoWindow").height();
                    //    var footH = $("#nuovoContattoWindow .window-footer").outerHeight(true);
                    //    var contH = h - footH;
                    //    $("#nuovoContattoWindow .container").height(contH).css("overflow", "auto");
                    //}
                });

            } else {

                alert("Errori nella lettura dei dati");

            }

            //--------------------------------------------------
            // INIZIO Controlli Caricamento righe già presenti
            //--------------------------------------------------

            popolaDocContabileRighe("tab_elenco_movimenti");
            if (KendoGrid("tab_elenco_movimenti") !== undefined) {
                maxOrdineDet = KendoGrid("tab_elenco_movimenti").dataSource.aggregates().Ordine_Det.max;
            } else {
                maxOrdineDet = 0;
            }
            impostaRiepilogoPesiUC("tab_elenco_movimenti");

            impostaCastellettoUC("tag_griglia_castelletto")
            
            //--------------------------------------------------
            // FINE Controlli Caricamento righe già presenti
            //--------------------------------------------------

            //----------------------------------
            // INIZIO Controlli Form Prodotto
            //----------------------------------

            // INIZIO Cambio label per pesi riscontrati
            if (Qs_CaricoScarico === CAU_CARICO) {
                $("#lblQuantitaRiscontrata").text("Quantità dichiarata");
                $("#lblTaraRiscontrata").text("Tara totale dichiarata");
                $("#lblKgLordiRiscontrati").text("Kg lordi dichiarati");
                $("#lblKgNettiRiscontrati").text("Kg netti dichiarati");
            }
            // FINE Cambio label per pesi riscontrati

            // INIZIO Sistemazione Date Riga
            $("#idDataDenuncia").kendoDatePicker({
                footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
                max: new Date(2100, 11, 31)
            });
            set_data("idDataDenuncia", formattedDate(new Date(), "/"), null);
            // FINE Sistemazione Date Riga

            //--------------------------------
            // FINE Controlli Form Prodotto
            //--------------------------------

            //--------------------------------
            // INIZIO DocContabileDettagliUC
            //--------------------------------

            impostaDocContabileDettagliUC();

            //--------------------------------
            // FINE DocContabileDettagliUC
            //--------------------------------

            //--------------------------------
            // INIZIO BeniConfezionamentoUC
            //--------------------------------

            if (lavCodAccettazione || is_FF_FormProdottoUC()) {
                impostaBeniConfezionamentoUC(true, lavCodAccettazione, lavCodAccettazione);
            }

            //--------------------------------
            // FINE BeniConfezionamentoUC
            //--------------------------------

            //--------------------------------
            // INIZIO ImputazioneImpiantiUC
            //--------------------------------

            if (imputazioneImpianti_AbilitazioneGenerale()) {

                impostaImputazioneImpiantiUC();

                if (lavCodAccettazionePomodoro) {
                    creaKendoDropDownList("ddlCodVarietaPomodoro", { read: RiempiCodiceVarietaPomodoro }, "Desc_Varieta", "Cod_Varieta");
                    $("#lblCodVarietaPomodoro").addClass("campiObbligatori");
                    $("#ddlCodVarietaPomodoro").prop("required", true);
                    $("#lblDescAppezzamenti").addClass("campiObbligatori");
                    $("#txtDescAppezzamenti").prop("required", true);
                    $("#row_impianti_pomodoro").show();
                }
            }

            //--------------------------------
            // FINE ImputazioneImpiantiUC
            //--------------------------------

            //--------------------------------
            // INIZIO RifCatastaliUC
            //--------------------------------

            if (isContrattoAffitto()) {
                impostaRifCatastaliUC();
            }

            //--------------------------------
            // FINE RifCatastaliUC
            //--------------------------------


            switch (true) {

                case (parseInt($(cIdAgenda).val()) === 0 && lavCodMovMagazzino):
                    $("#a_tabDettagliDoc").tab("show");
                    break;

                case (parseInt($(cIdAgenda).val()) === 0 || lavCodAccettazionePomodoro):
                    //if (gestioneContabilita === enum_Livello_GestContabilita_NonGestita) {
                    //    $("#a_tabDettagliDoc").tab("show");
                    //    $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_Riepilogo]).attr("style", "display:none");
                    //    $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_Dettaglio]).attr("style", "display:inline-block");
                    //    $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ScaricoGiacenza]).attr("style", "display:none");
                    //    tabStrip_Dettagli.select(index_tabStrip_Dettagli_Dettaglio);
                    //}
                    //else
                    $("#a_tabTestataDoc").tab("show");
                    break;

                default:
                    $("#a_tabDettagliDoc").tab("show");

            }

            //IMPORTANT: lasciare per ultimo, perché sennò potrebbe non avere ancora tutti gli elementi da nascondere
            documentoLoaded = true;   //mi serve per verificare i dati testata minimi con validatori
            VerificaDatiMinimiTestata();

            $.logThis("DocReady: FINE");

            WaitFrame.hide();

            console.log("Ho nascosto WaitFrame generale");

        });

    console.log("Thread Principale -> Terminate letture asincrone");

}

async function docReady() {
    try {
        promises = new Array();
        promises.push(get_Cmb_Sezionale("inSezionale"));
        promises.push(get_Cmb_ModPagamento("inModPagamento"));
        promises.push(get_Cmb_AspettoBeni("inAspettoBeni"));
        promises.push(get_Cmb_CausaleTrasporto(idControlloCausaleTrasporto));

        // Modalità ServerFiltering: creo la DDL Contatto1 in Async solo se sono in modifica o lettura, perché poi forzerò la lettura con il CodRisum
        // e il Contatto1 in questi casi non é modificabile
        if (!applicaServerFilteringSuCedenteCessionario || cIdTipoOp === enum_TipoOperazioneDB.Lettura.value || cIdTipoOp === enum_TipoOperazioneDB.Modifica.value) {
            promises.push(get_Cmb_Contatto1(ddlContatto1Nome));
        }
        // Modalità ServerFiltering: creo la DDL Contatto2 in Async solo se sono in lettura, perché poi forzerò la lettura con il CodDestinazione.
        // In modifica la creo Sync perché é modificabile 
        if (!applicaServerFilteringSuCedenteCessionario || cIdTipoOp === enum_TipoOperazioneDB.Lettura.value) {
           promises.push(get_Cmb_Contatto2(ddlContatto2Nome));
        }

        promises.push(get_Cmb_Agente("inAgente"));
        promises.push(get_Cmb_CapoArea("inCapoArea"));
        promises.push(get_Cmb_TipoDocumento("inTipoDocumento"));
        promises.push(get_Cmb_TrasportoUdm("ddlUnitaMisuraTrasporto"));

        if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true) {
            promises.push(get_Cmb_ContattoCoop1(ddlContattoCoop1Nome));
            promises.push(get_Cmb_ContattoCoop2(ddlContattoCoop2Nome));
        }

        let resp = await Promise.all(promises).then(console.log("Creazione CMB OK, Procedi!!!")).catch(new Error("Creazioni CMB FALLITE!"));
        console.log(resp);

        if (raccolteXConferimenti_AbilitazioneGenerale(false) && cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value && !applicaServerFilteringSuCedenteCessionario) {
            creaBtnFiltraContattiXRaccolte(btnFiltraContatti1XRaccolte, ddlContatto1Nome);
            creaBtnFiltraContattiXRaccolte(btnFiltraContatti2XRaccolte, ddlContatto2Nome);
        }
        else {
            $("#" + btnFiltraContatti1XRaccolte).hide();
            $("#" + btnFiltraContatti2XRaccolte).hide();
        }

        let indice = 0;
        Cmb_Sezionali = resp[indice++];
        Cmb_ModPagamento = resp[indice++];
        Cmb_AspettoBeni = resp[indice++];
        Cmb_CausaleTrasporto = resp[indice++];
        if (!applicaServerFilteringSuCedenteCessionario || cIdTipoOp === enum_TipoOperazioneDB.Lettura.value || cIdTipoOp === enum_TipoOperazioneDB.Modifica.value) {
            Cmb_Contatto1 = resp[indice++];
        }
        if (!applicaServerFilteringSuCedenteCessionario || cIdTipoOp === enum_TipoOperazioneDB.Lettura.value) {
            Cmb_Contatto2 = resp[indice++];
        }
        Cmb_Agente = resp[indice++];
        Cmb_CapoArea = resp[indice++];
        Cmb_TipoDocumento = resp[indice++];
        Cmb_TrasportoUdm = resp[indice++];

        if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true) {
            Cmb_Coop1 = resp[indice++];
            Cmb_Coop2 = resp[indice++];
        }
        //Check Fattura Accompagnatoria
        creaKendoSwitch("chkAccompagnatoria", "Si", "No", true, chkAccompagnatoria_change);
        creaKendoSwitch("chkProvvisorio", "Si", "No", false);

        switch (lavCodFattura) {
            case true:

                //Lettura Pendenze Valide per il Documento
                LeggiElencoPendenze_change();

                if (lavCodDocEmesso == false) {

                    //Nascondo i check dedicati alle fatture emesse                                 
                    $("#groupinTipoDocumento").hide();
                }

                ImpostaVisibilitaNotaFattura(true, false);
                $("#inNotaFattura").on("change", VerificaDatiMinimiTestata);
                $("#inDataNotaFattura").bind("change", VerificaDatiMinimiTestata);

                break;
            default:

                //Nascondo i check dedicati alle fatture               
                $("#checkboxacc").hide();
                $("#boxChkProvvisorio").hide();
                $("#groupinTipoDocumento").hide();
                ImpostaVisibilitaNotaFattura(false, false);
        }

        let idTipoInd1 = "inTipoIndirizzoCC1";
        creaKendoDropDownList(idTipoInd1,
            { read: LeggiTipoIndirizzoCC, data: { idControllo: idTipoInd1, idCedCes: ddlContatto1Nome } },
            "Tipo_Indirizzo_Des", "Cod_Indirizzo", null, null, null, false
        ).bind("change", { idTipoIndirizzo: idTipoInd1, cc: "CC1" }, inTipoIndirizzoCC_change);

        let idTipoInd2 = "inTipoIndirizzoCC2";
        creaKendoDropDownList(idTipoInd2,
            { read: LeggiTipoIndirizzoCC, data: { idControllo: idTipoInd2, idCedCes: ddlContatto2Nome } },
            "Tipo_Indirizzo_Des", "Cod_Indirizzo", null, null, null, false
        ).bind("change", { idTipoIndirizzo: idTipoInd2, cc: "CC2" }, inTipoIndirizzoCC_change);

        creaKendoDropDownList("inCausaleContabilita", { read: LeggiCausaliContabilita }, "Cau_Contab_Descrizione", "Cau_Contab_Codice", null, null, null, false);
        Cmb_CausaleContabilita = KendoDDL("inCausaleContabilita");

        creaKendoDropDownList("inGestioneVettore", { read: LeggiGestioneVettore }, "GestioneVettore_Des", "GestioneVettore_Cod", null, null, null, false);
        Cmb_GestioneVettore = KendoDDL("inGestioneVettore");

        creaKendoDropDownList("inDipendenti", { read: LeggiAgentiCapoAreaTerzisti, data: { idControllo: "inDipendenti", tipoRapporto: enum_TipoRapporto.Dipendenti } }, "Rag_Soc_Completa", "Cod_RisUm", null, null, null, false);
        Cmb_Dipendente = KendoDDL("inDipendenti");

        creaKendoDropDownList("inVettore", { read: LeggiAgentiCapoAreaTerzisti, data: { idControllo: "inVettore", tipoRapporto: enum_TipoRapporto.Vettori } }, "Rag_Soc_Completa", "Cod_RisUm", null, null, null, false).bind("change", inVettore_change);
        Cmb_Vettore = KendoDDL("inVettore");

        creaKendoDropDownList("inTipoIndirizzoVettore", { read: LeggiTipoIndirizzoVettore }, "Tipo_Indirizzo_Des", "Cod_Indirizzo", null, null, null, false
        ).bind("change", inTipoIndirizzoVettore_change);
        Cmb_TipoIndVettore = KendoDDL("inTipoIndirizzoVettore");

        creaKendoDropDownList("inAccModalitaTrasporto", { read: LeggiAccModalitaTrasporto }, "ModalitaTrasporto_Des", "ModalitaTrasporto_Cod", null, null, null, false);
        Cmb_AccModalitaTrasporto = KendoDDL("inAccModalitaTrasporto");

        creaKendoDropDownList("inAccUnitaTrasporto", { read: LeggiAccUnitaTrasporto }, "UnitaTrasporto_Des", "UnitaTrasporto_Cod", null, null, null, false);
        Cmb_AccUnitaTrasporto = KendoDDL("inAccUnitaTrasporto");

        creaKendoDropDownList("inMezzoTrasporto", { read: LeggiMezzoTrasporto }, "Targa", "Mac_Cod", "contains", null, $("#noDataTemplateMezzoTrasporto").html(), false).bind("change", inMezzoTrasporto_change);
        Cmb_MezzoTrasporto = KendoDDL("inMezzoTrasporto");

        creaKendoDropDownList("inOperatore", { read: LeggiOperatore }, "Utente", "Codice_Fiscale", null, null, null, false);
        Cmb_Operatore = KendoDDL("inOperatore");
        Cmb_Operatore.enable(false);

        promises = null;

        //Prima di leggere devo impostare le date del documento, perché mi servono per alcune letture

        //Se sono in modifica imposto direttamente la data del documento, per avere già le letture conseguenti giuste, senza necessità di rifarle
        if (parseInt($(cIdAgenda).val()) !== 0 && $(cIdDataDocumento).val() !== "") {
            //KendoDate("inDataEmissione").value(JSON.parse($(cIdDataDocumento).val()));
            set_data("inDataEmissione", formattedDate(JSON.parse($(cIdDataDocumento).val()), "/"), null);
        } else {
            set_data("inDataEmissione", formattedDate(new Date(), "/"), null);
        }
        set_data("inDataRegistrazione", formattedDate(new Date(), "/"), null);
        set_data("inDataEmissione2", formattedDate(new Date(), "/"), null);

        if (lavCodAccettazionePomodoro) {
            let oggi = new Date();
            oggi.setHours(0, 0, 0, 0);
            set_dataTime("inDataSpedizione", oggi, null);
        } else {
            set_dataTime("inDataSpedizione", new Date(), null);
        }

        promises = new Array();

        promises.push(Cmb_Sezionali.dataSource.read());
        promises.push(Cmb_AspettoBeni.dataSource.read());
        promises.push(Cmb_CausaleContabilita.dataSource.read());
        promises.push(Cmb_CausaleTrasporto.dataSource.read());

        if (!applicaServerFilteringSuCedenteCessionario || cIdTipoOp === enum_TipoOperazioneDB.Lettura.value || cIdTipoOp === enum_TipoOperazioneDB.Modifica.value) {
            promises.push(Cmb_Contatto1.dataSource.read());
        }
        promises.push(Cmb_Agente.dataSource.read());
        promises.push(Cmb_CapoArea.dataSource.read());
        promises.push(Cmb_GestioneVettore.dataSource.read());
        promises.push(Cmb_Dipendente.dataSource.read());
        promises.push(Cmb_Vettore.dataSource.read());
        promises.push(Cmb_AccModalitaTrasporto.dataSource.read());
        promises.push(Cmb_AccUnitaTrasporto.dataSource.read());
        promises.push(Cmb_Operatore.dataSource.read());
        promises.push(Cmb_TrasportoUdm.dataSource.read());

        let resp1 = await Promise.all(promises).then(console.log("Letture OK, Procedi!!!")).catch(new Error("LETTURE FALLITE!"));

        if (!applicaServerFilteringSuCedenteCessionario || cIdTipoOp === enum_TipoOperazioneDB.Lettura.value) {
            //se uso gerarchia devo leggere solo Contatto1, perché gli altri dipendono da quello che scelgo in Contatto1
            if (lavCodAccettazione === false || contattiAcc4ConGerarchia === false) {
                //lo posso fare qui, perché utilizza lo stesso elenco del Contatto1, che ora è locale, non deve essere letto da db, quindi dovrebbe ritornare immediatamente
                await Cmb_Contatto2.dataSource.read();
            }
        }

        //se nuovo documento su db sono già stati letti perché c'è il popup di pre-scelta
        creaKendoDropDownList("inCentroAziendale", { read: LeggiCentri }, "sa_nome", "sa_cod", null, null, null, true);
        //posso direttamente impostare la ddl, perché che sia modifica oo nuovo, cmq il sa_cod so già qual è
        Set_KendoDDLValue("inCentroAziendale", Qs_SaCod);

        //TODO: fare unica combo vettore che viene valorizzata con vettore se trasporto a cure del vettore, dipendenti se a cura del cedente e vuota (magari pure nascosta) se a cura del cessionario
        //TODO: da combo dipendenti/vettori devo filtrare il proprietario dell'operazione (mRsContatti.Filter = "Cod_Contatto <> '" & mPiva & "' And Terzista = 1")

        creaKendoDropDownListWithData("inTipoPeso",
            [
                { text: TraduzioneMultiResx(resxObj, "PesoLordo", "Peso Lordo:"), value: "0" },
                { text: TraduzioneMultiResx(resxObj, "PesoNetto", "Peso Netto:"), value: "1" }
            ]
        );

        Set_KendoNumTBValue("inNumProtocollo", 0);
        Set_KendoNumTBValue("inNumeroColli", 0);
        Set_KendoNumTBValue("inPesoTotale", 0);
        Set_KendoNumTBValue("inAgenteProvvigione", 0);
        Set_KendoNumTBValue("inAgenteProvvigionePerc", 0);
        Set_KendoNumTBValue("inCapoAreaProvvigione", 0);
        Set_KendoNumTBValue("inCapoAreaProvvigionePerc", 0);

        Set_KendoDDLValue("inOperatore", operatoreCodFisc);

        // --------------------------------- 
        //  Lettura Tabelle di base di riga
        // --------------------------------- 

        promises = null;
        promises = new Array();

        promises.push(RicercaCausali_Riga_Async(true));

        let forzaTipoDestinazioneMagazzino = true;

        // TODO: Spostare o comunque duplicare all'onchange della categoria prodotto
        if (lavCodAccettazione)
            promises.push(RicercaCelleEMagazzini_Async(true, CELLA, $(cIdPiva).val(), parseInt(Qs_SaCod), "C", forzaTipoDestinazioneMagazzino));
        else
            if (cIdLavCod === enum_LavCod.DDT_Ricevuto.value)
                promises.push(RicercaCelleEMagazzini_Async(true, FABBRICATI_NO_STALLE, $(cIdPiva).val(), parseInt(Qs_SaCod), "M", forzaTipoDestinazioneMagazzino));
            else
                promises.push(RicercaCelleEMagazzini_Async(true, FABBRICATI_NO_STALLE, $(cIdPiva).val(), parseInt(Qs_SaCod), "", forzaTipoDestinazioneMagazzino));
        promises.push(RicercaIVA_Aliquote_Async($(cIdPiva).val()));
        promises.push(RicercaImballaggio_Async(true, $(cIdPiva).val(), 4));
        promises.push(RicercaImballaggio_Async(true, $(cIdPiva).val(), 8));
        promises.push(RicercaImballaggio_Async(true, $(cIdPiva).val(), 5));
        promises.push(RicercaPUA_Regolamenti_Async(false, Qs_PuaRegolamento, SENZA_CATEGORIA));
        promises.push(RicercaParametriQualitativi_Async(false, $(cIdPiva).val()));
        promises.push(RicercaUdm_Optimize_Async("", false, false, 0, 0, CAU_CARICO, -999, 0, false, false, false));
        promises.push(RicercaConfezionamentoLotto_Async(true,$(cIdPiva).val()));

        let respRiga = await Promise.all(promises).then(console.log("Letture Riga OK, Procedi!!!")).catch(new Error("LETTURE Riga FALLITE!"));
         
        elencoCausali_Riga = respRiga[0];
        elencoCelleMagazzini = respRiga[1];

        // Le divido addirittura qui senza fare due letture separate
        // Userò le sole celle solo in caso di F&F + Trasformati vegetali; i soli magazzini in caso contrario
        elencoCelle = [];
        elencoMagazzini = [];

        if (Array.isArray(elencoCelleMagazzini)) {
            elencoCelle = elencoCelleMagazzini.filter(function (x) {
                return (x.Tipo_Destinazione !== MAGAZZINO);
            });
            elencoMagazzini = elencoCelleMagazzini.filter(function (x) {
                return (x.Tipo_Destinazione !== CELLA);
            });
        }
        
        elencoIVA_Aliquote = respRiga[2];
        //Sotto insieme ristretto per le aliquote sceglibili in caso di sconto merce
        elencoIVA_Aliquote_ScontoMerce = [];

        if (Array.isArray(elencoIVA_Aliquote)) {
            elencoIVA_Aliquote_ScontoMerce = elencoIVA_Aliquote.filter(function (x) {
                return ([61, 67, 78].includes(x.Cod_IVA));
            });
        }

        elencoImballaggi = respRiga[3];
        elencoContenitori = respRiga[4];
        elencoConfezioni = respRiga[5];
        elencoPUA_Regolamenti = respRiga[6];
        elencoPUA_Regolamenti_ElemCod = SENZA_CATEGORIA;
        paramQualGestiti_FF = respRiga[7];
        elencoUdmOptimized = respRiga[8];
        elencoConfezionamentoLotto = respRiga[9];

        //TODO: è da fare solo per alcuni lav_cod (se doc ricevuti non deve comparire DDL)
        creaKendoDropDownList("inNumDocDDL", { read: GetNumeratoriPiuDefaults },
            "NumeratoreTipo_Des", "Numeratore_Tipo", null, null, null, true,
            kendo.template($("#templateNumDocDDL").html())).bind("change", inNumDocDDL_change);

        creaKendoSwitch("inNumDocLock",
            "<i class='fa fa-lg fa-lock'></i>",
            "<i class='fa fa-lg fa-unlock'></i>",
            false, inNumDocLock_change, "65", "12px");
        //mi tocca fare così per fare in modo che in hoover compaia il tooltip esplicativo
        if ($("#inNumDocLock").parent(".kendoSwitch").length === 1) {
            $("#inNumDocLock").parent(".kendoSwitch")[0].title = TraduzioneMultiResx(resxObj, "PermettiModificaNumeroDoc", "Permetti modifica numero documento");
        }

       


        //TODO: in teoria non serve più qui, perché lo faccio già quando imposto la visibilità per lav_cod
        //RiBloccoNumeroDoc(true);

        //TODO: DataLock
        creaKendoSwitch("inDataLock",
            "<i class='fa fa-lg fa-lock'></i>",
            "<i class='fa fa-lg fa-unlock'></i>",
            false, inDataLock_change, "65", "12px");
        //mi tocca fare così per fare in modo che in hoover compaia il tooltip esplicativo
        if ($("#inDataLock").parent(".kendoSwitch").length === 1) {
            $("#inDataLock").parent(".kendoSwitch")[0].title = TraduzioneMultiResx(resxObj, "PermettiModificaDataDoc", "Permetti modifica data documento");
        }

        //Distanza Lock
        creaKendoSwitch("inDistanzaLock",
            "<i class='fa fa-lg fa-lock'></i>",
            "<i class='fa fa-lg fa-unlock'></i>",
            true, inDistanzaLock_change, "65", "12px");
        //mi tocca fare così per fare in modo che in hoover compaia il tooltip esplicativo
        if ($("#inDistanzaLock").parent(".kendoSwitch").length === 1) {
            $("#inDistanzaLock").parent(".kendoSwitch")[0].title = TraduzioneMultiResx(resxObj, "PermettiModificaDistanza", "Permetti modifica dati relativi la distanza");
        }

        RiBloccoDistanzaDoc(false);
        GestioneVisibilitaDistanzaLock();

        creaKendoSwitch("inScadenzaUnica", "", "", false, inScadenzaUnica_change);
        creaKendoSwitch("inEvasioneTassativa", "", "", false);

        $("#inAnnoOrdineConsorzio").kendoDatePicker({
            start: "decade",
            depth: "decade",
            format: "yyyy"
        });

        if (applicaServerFilteringSuCedenteCessionario) {
            if (cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value)
                create_Cmb_Contatto1Sync(ddlContatto1Nome);
            if (cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value || cIdTipoOp === enum_TipoOperazioneDB.Modifica.value)
                create_Cmb_Contatto2Sync(ddlContatto2Nome);
        }

        return true;

    } catch (err) {
        console.error(err);
        let msgErr = "Errore in letture asincrone: " + err;
        alert(msgErr);
        return false;
    }
}

async function caricaDocumento(idAgenda) {

    if (typeof (RicercaTestataDocumentoContab) !== "function")
        return;

    //Lettura spostata lato Server
    var docData = JSON.parse($('input[name$="hdKendo_TestataDoc"]').val());

    //Impostazione Fattura Accompagnatoria
    switch (lavCodFattura) {

        case true:

            if (docData.Accompagnatoria == 0) {
                setKendoSwitch("chkAccompagnatoria", false);
            }
            else {
                setKendoSwitch("chkAccompagnatoria", true);
            }

             //Disabilitazione ChkAccompagnatoria
            $("#chkAccompagnatoria").data("kendoSwitch").enable(false);

            //LeggiElencoPendenze_change();

            if (docData.BloccoFlag == 1000) {
                setKendoSwitch("chkProvvisorio", true);
            }
            else {
                setKendoSwitch("chkProvvisorio", false);
            }

            break;
        
       case false:
            break;
    }
   
    Set_KendoDDLValue("inTipologiaDocumento", docData.LavCod);
    $("#inTipologiaDocumento").change();

    Set_KendoDDLValue("inOperatore", docData.UsernameModifica); // corrisponde a operatoreModificaCodFisc

    //TODO: DEBUG: per il momento lo commento!
    //KendoDDL("inTipologiaDocumento").enable(false);

    $("#inNumDocSin").val(docData.DocNumeroSin);
    Set_KendoNumTBValue("inNumDoc", docData.DocNumero);
    $("#inNumDocDes").val(docData.DocNumeroDes);

    //Per evitare casini ora in modifica viene settata direttamente la data giusta del documento, quindi non c'è bisogno di ri-settarla e fare il change
    //set_data("inDataEmissione", formattedDate(docData.DataMovimento, "/"), null);
    ////la data è può essere diversa dalla data odierna che è stata impostata sul docReady (è cmq necessaria una data per fare alcune seguenti letture)
    ////devo quindi rileggere i default, numeratori ==> per il momento richiamo direttamente il change perché non c'è nulla che mi dà fastidio
    //KendoDate("inDataEmissione").trigger("change");

    $("#inNumDocShow").val(docData.DocNumeroVisualizzato);
    set_data("inDataRegistrazione", formattedDate(docData.DataRegistrazione, "/"), null);

    Set_KendoNumTBValue("inNumRegistrazione", docData.ProgrRegistrazione);
    Set_KendoNumTBValue("inNumProtocollo", docData.ProgrProtocollo);

    Set_KendoDDLValue("inSezionale", docData.SezionaleCod, "");
    KendoDDL("inAspettoBeni").text(docData.Aspetto);

    if (docData.CausaleTrasportoCod === 0 && docData.CausaleTrasporto !== "") {
        //Aggiungo la voce inserita dall'utente
        KendoDDL(idControlloCausaleTrasporto).dataSource.add({
            Causale_Trasporto_Des: docData.CausaleTrasporto,
            Causale_Trasporto_Cod: 0
        });
    }
    Set_KendoDDLValue(idControlloCausaleTrasporto, docData.CausaleTrasportoCod, docData.CausaleTrasporto);

    Set_KendoNumTBValue("inNumeroColli", docData.Colli);
    Set_KendoDDLValue("inTipoPeso", docData.TipoPeso);
    Set_KendoNumTBValue("inPesoTotale", docData.Peso);

    $("#inNaturaBeni").val(docData.NaturaBeni);

    console.log("IMPOSTO " + ddlContatto1Nome);
    await Set_KendoDDLValueVirtual(ddlContatto1Nome, docData.CodRisUm, "");
    $("#" + ddlContatto1Nome).change();
    //KendoDDL("inTipoIndirizzoCC1").dataSource.read();
    KendoDDL("inTipoIndirizzoCC1").select(-1);
    Set_KendoDDLValue("inTipoIndirizzoCC1", docData.CodIndirizzoRisUm, "");
    $("#inTipoIndirizzoCC1").change();

    if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true) {
        console.log("IMPOSTO " + ddlContattoCoop1Nome);
        Set_KendoDDLValue(ddlContattoCoop1Nome, docData.CodRisUmAltro, "");
        $("#" + ddlContattoCoop1Nome).change();
        console.log("IMPOSTO " + ddlContattoCoop2Nome);
        Set_KendoDDLValue(ddlContattoCoop2Nome, docData.SecondaCooperativa, "");
        $("#" + ddlContattoCoop2Nome).change();
    }

    console.log("IMPOSTO " + ddlContatto2Nome);
    await Set_KendoDDLValueVirtual(ddlContatto2Nome, docData.CodDestinazione, "", 20000, 50);
    $("#" + ddlContatto2Nome).change();
    console.log("IMPOSTO il resto!");
    //KendoDDL("inTipoIndirizzoCC2").dataSource.read();
    Set_KendoDDLValue("inTipoIndirizzoCC2", docData.CodIndirizzoDestinazione, "");
    $("#inTipoIndirizzoCC2").change();

    switch (docData.LavCod) {

        case enum_LavCod.Accettazione_da_diversi.value:
        case enum_LavCod.Distinta_Carico_Accettazione.value:
        case enum_LavCod.Auto_Ddt_Emesso_Accettazione.value:

            if (docData.DocumentoAccettazione !== undefined && docData.DocumentoAccettazione !== null) {
                $("#inNumDocSin2").val(docData.DocumentoAccettazione.DocNumeroSin);
                Set_KendoNumTBValue("inNumDoc2", docData.DocumentoAccettazione.DocNumero);
                $("#inNumDocDes2").val(docData.DocumentoAccettazione.DocNumeroDes);

                //TODO: imposta anche la data del documento!!!!
                set_data("inDataEmissione2", formattedDate(docData.DocumentoAccettazione.DataMovimento, "/"), null);
            }

            break;

        case enum_LavCod.Contratto_Affitto.value:

            if (docData.DocumentoContrattoAffitto !== undefined && docData.DocumentoContrattoAffitto !== null) {
                $("#inNumDocSin2").val(docData.DocumentoContrattoAffitto.DocNumeroSin);
                Set_KendoNumTBValue("inNumDoc2", docData.DocumentoContrattoAffitto.DocNumero);
                $("#inNumDocDes2").val(docData.DocumentoContrattoAffitto.DocNumeroDes);
                set_data("inDataInizVal", formattedDate(docData.DocumentoContrattoAffitto.DataInizioValidita, "/"), null);
                set_data("inDataFineVal", formattedDate(docData.DocumentoContrattoAffitto.DataFineValidita, "/"), null);
                $("#inAltriLocatori").val(docData.DocumentoContrattoAffitto.AltriLocatori);
                $("#inRifOrdini").val(docData.DocumentoContrattoAffitto.RiferimentoOrdini);
            }

            break;

    }

    if (lavCodOrdine === true) {
        setKendoSwitch("inScadenzaUnica", docData.ScadenzaUnica);
        KendoSwitch("inScadenzaUnica").trigger("change");

        if (kendo.parseDate(docData.DataEvasionePrevista).getTime() !== AGRODATAFINE.getTime()) {
            set_data("inDataEvasionePrevista", formattedDate(docData.DataEvasionePrevista, "/"), null);
            KendoDate("inDataEvasionePrevista").trigger("change");
        }
        setKendoSwitch("inEvasioneTassativa", docData.EvasioneTassativa);
        if (kendo.parseDate(docData.DataSpedizionePrevista).getTime() !== AGRODATAFINE.getTime()) {
            set_dataTime("inDataSpedizionePrevista", docData.DataSpedizionePrevista, null);
        }

        $("#inNOrdineCliente").val(docData.NumDocOrdineCliente);
        if (kendo.parseDate(docData.DataDocOrdineCliente).getTime() !== AGRODATAINIZIO.getTime()) {
            set_data("inDataOrdineCliente", formattedDate(docData.DataDocOrdineCliente, "/"), null);
        }
        $("#inNOrdineConsorzio").val(docData.NumDocOrdineEnte);
        if (docData.AnnoDocOrdineEnte !== null && docData.AnnoDocOrdineEnte !== "" &&
            parseInt(docData.AnnoDocOrdineEnte) !== 0 &&
            parseInt(docData.AnnoDocOrdineEnte) !== AGRODATAFINE.getFullYear()) {
            //$("#inAnnoOrdineConsorzio").val(docData.AnnoDocOrdineEnte);
            set_data("inAnnoOrdineConsorzio", new Date(docData.AnnoDocOrdineEnte, 0, 1), null);
        }

    } else {
        set_dataTime("inDataSpedizione", docData.OraSpedizione, null);
    }

    $("#inNoteTestata").val(docData.NoteIntestazione);

    $("#inDocAllegati").val(docData.Documenti_Allegati);
    $("#inLavAssociate").val(docData.Lavorazioni_Associate);

    $("#inNotaFattura").val(docData.N_Nota_Fattura);
    if (kendo.parseDate(docData.Data_Nota_Fattura).getTime() !== AGRODATAINIZIO.getTime()) {
        set_data("inDataNotaFattura", formattedDate(docData.Data_Nota_Fattura, "/"), null);
    }

    $("#inDDTResoSDI").val(docData.N_Nota_DDT_Reso_SDI);
    if (kendo.parseDate(docData.Data_Nota_DDT_Reso_SDI).getTime() !== AGRODATAINIZIO.getTime()) {
        set_data("inDataDDTResoSDI", formattedDate(docData.Data_Nota_DDT_Reso_SDI, "/"), null);
    }
    $("#inRigaDDTResoSDI").val(docData.N_Nota_Riga_DDT_Reso_SDI);

    Set_RadioGroupValue("inTrasportoCura", docData.Mezzo);
    Set_KendoDDLValue("inGestioneVettore", docData.GestioneVettore, "");

    Set_KendoDDLValue("inVettore", docData.CodVettore, "");
    KendoDDL("inTipoIndirizzoVettore").dataSource.read();
    Set_KendoDDLValue("inTipoIndirizzoVettore", docData.CodIndirizzoVettore, "");
    $("#inTipoIndirizzoVettore").change();

    Set_KendoDDLValue("inAccModalitaTrasporto", docData.ModalitaTrasporto, "");
    Set_KendoDDLValue("inAccUnitaTrasporto", docData.UnitaTrasporto, "");

    //TODO: gestire l'aggiunta come per causale trasporto!
    KendoDDL("inMezzoTrasporto").dataSource.read();
    if (docData.MacCodTrasporto === 0 && docData.Targa !== "") {
        //Aggiungo la voce inserita dall'utente
        KendoDDL("inMezzoTrasporto").dataSource.add({
            Targa: docData.Targa,
            Mac_Cod: 0
        });
    }
    Set_KendoDDLValue("inMezzoTrasporto", docData.MacCodTrasporto, docData.Targa);

    Set_KendoDDLValue("inModPagamento", docData.ModalitaPagamento, 0);

    $("#inDescrizioneMezzo").val(docData.DescrizioneMezzo);
    $("#inNImmatricolazioneRimorchio").val(docData.NumImmatricolazioneRimorchio);
    $("#inNAutorizzazioneTrasporto").val(docData.NumAutorizzazioneTrasporto);
    set_data("inDataAutorizzazioneTrasporto", formattedDate(docData.DataRilascioAutorizzazione, "/"), null);
    Set_KendoNumTBValue("inPesoTaraTrasporto", docData.PesoTaraTrasporto);
    Set_KendoDDLValue("ddlUnitaMisuraTrasporto", docData.DistanzaTrasportoUdm, enum_Udm.chilometri.value);
    Set_KendoNumTBValue("ntbDistanzaTrasporto", docData.DistanzaTrasporto);

    Set_KendoDDLValue("inAgente", docData.AgenteCod, "");
    Set_KendoNumTBValue("inAgenteProvvigione", docData.AgenteProvvigione);
    Set_KendoDDLValue("inCapoArea", docData.CapoAreaCod, "");
    Set_KendoNumTBValue("inCapoAreaProvvigione", docData.CapoAreaProvvigione);

    Set_RadioGroupValue("inFormatiStampa", docData.Layout_FormatiStampa);

    if (docData.BloccoFlag === 1) {
        let dataBlocco = kendo.toString(kendo.parseDate(docData.BloccoData), "dd/MM/yyyy");
        let utenteBlocco = docData.BloccoUtente;
        let msgBlocco = "Documento BLOCCATO in data " + dataBlocco;
        if (utenteBlocco !== "")
            msgBlocco = msgBlocco + " da " + utenteBlocco;
        $("#msgBlocco").text(msgBlocco);
    }

    Set_KendoDDLValue("inTipoDocumento", docData.TipoDocumento, "");

    MostraStatoEvasioneOrdine();

}

function impostaDocContabileDettagliUC() {
    $.logThis("EMBEDDED docContabileDettagliUC_jQueryDocReady: INIZIO");

    //$("#idOra").kendoTimePicker({
    //    dateInput: true,
    //    value: new Date(1900, 0, 1, 12, 0)
    //    //,
    //    //culture: kendo.culture().name,
    //});

    // In inserimento entro direttamente sulla nuova riga
    if (parseInt($(cIdAgenda).val()) === 0
        //&& gestioneContabilita === enum_Livello_GestContabilita_NonGestita
    ) {
        ImpostaVisibilitaElencoOSingolaRiga(true);
    }
    else {
        // Rendo visibile il TAB dell'elenco righe
        ImpostaVisibilitaElencoOSingolaRiga(false); 
    }


    // TODO Inizio - condizionare anche al tipo di documento e non mettere l'id del div dello userControl ma del Tab
    if (Qs_CaricoScarico !== CAU_CARICO) {
        
        ////KENDO Tabstrip***********************************************
        ////////var tabstrip_GiacenzeMagazzino =  $(".kendoTabStrip_GiacenzeMagazzino").kendoTabStrip({
        ////////    animation: {
        ////////        open: {
        ////////            effects: "fadeIn"
        ////////        }
        ////////    }
        ////////});
        ////*************************************************************
        ////////tabstrip_GiacenzeMagazzino.select(0);


        //Giulia - 03/12/2021: commentati perché non dovrebbero usare la parte di giacenze_magazzino_ws_client.js perché incompatibili
        //con le modifiche per le lavorazioni, in teoria questa parte non dovrebbe più essere usata, se poi dovesse servire si dovrà verificare bene


    //    $(".areaGiacenzeMagazzino").show();

    //    // Vengono mostrati purché siano gestiti in almeno una specie/varietà, perché servono per la ricerca
    //    if (paramQualGestiti_FF !== null && paramQualGestiti_FF.length !== 0) {
    //        for (let ipar = 0; ipar < paramQualGestiti_FF.length; ipar++) {
    //            if (paramQualGestiti_FF[ipar].Tabella_Des === "Calibro") {
    //                creaKendoDDL_ParametriQualitativi("idCalibro", $(cIdPiva).val(), 1, "ocalibro");
    //            }
    //            if (paramQualGestiti_FF[ipar].Tabella_Des === "Qualità") {
    //                creaKendoDDL_ParametriQualitativi("idQualita", $(cIdPiva).val(), 3, "oqualità");
    //            }
    //            if (paramQualGestiti_FF[ipar].Tabella_Des === "Certificazioni") {
    //                creaKendoDDL_ParametriQualitativi("idCertificazione", $(cIdPiva).val(), 12, "ocertificazioni");
    //            }
    //        }
             
    //        if (gestitoImballaggio_FF)
    //            creaKendoDDL_ParametriQualitativi("idImballaggio", $(cIdPiva).val(), 4, "oimballaggio");
    //        if (gestitoContenitore_FF)
    //            creaKendoDDL_ParametriQualitativi("idContenitore", $(cIdPiva).val(), 8, "ocontenitore");
    //        if (gestitoConfezione_FF)
    //            creaKendoDDL_ParametriQualitativi("idConfezione", $(cIdPiva).val(), 5, "oconfezione");
    //    }

    //    var ds = new kendo.data.DataSource({ transport: { read: RiempiSpecie } });
    //    $('input[name$="ddlSpecie"]').kendoDropDownList({
    //        filter: "contains",
    //        dataSource: ds,
    //        dataTextField: "Veg_Des",
    //        dataValueField: "Veg_Cod"
    //    }).bind("change", ddlSpecie_change);

    //    creaKendoMultiselect("multiselVarieta", { read: RiempiVarieta, data: { Veg_Cod: -1 } }, "Cul_Des", "Cul_Cod");

    //    //eventi di click pulsanti
    //    $("#btn_ricerca_GiacenzeMagazzino").click(function () {

    //        dataDaControllare = get_data("inDataEmissione");
    //        dataValida = true;
    //        if (dataDaControllare !== "")
    //            dataValida = isValidDate(dataDaControllare);
    //        if (!dataValida)
    //            MessaggioErrore_Bootstrap("Data movimento non valida", "DIV_Messaggi");
    //        else {
    //            RicercaGiacenzeMagazzino(indirizzoHttp_DocContabile_WS, $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True");
    //        }
    //    });

    //    // FINE controlli utilizzati per ricerca su giacenze

    //    //TODO $('#a_tabScaricoDaGiacenza').click(function (e) { return tabClick(e, this); });

    }

    if (lavCodFattura == true && parseInt($(cIdAgenda).val()) !== 0) {
        LeggiElencoPendenze_change();
    }

    chkAccompagnatoria_change();

    document.getElementById("panelAreaDocContabileDettagli").style.opacity = "1";

    //// Se sono in inserimento imposto i controlli della nuova riga
    //if (gestioneContabilita === enum_Livello_GestContabilita_NonGestita &&
    //    parseInt($(cIdAgenda).val()) === 0) {
    //    DocContabileNuovaRiga();
    //}

    $.logThis("EMBEDDED docContabileDettagliUC_jQueryDocReady: FINE");


    MostraBtnDocumenti();
}

function onSelectTabStripDettagli(e) {
    let selectedIndex = $(e.item).index();
    if (selectedIndex === index_tabStrip_Dettagli_ImputazioneImpianti) {

        // Nothing to do?

    }
}

function chkAccompagnatoria_change(e) {

    if (getKendoSwitch("chkAccompagnatoria") && isLavCodSenzaDatiSpedizione() === false) {

        $("#groupAspettoBeni").show();
        $("#groupCausaleTrasporto").show();
        $("#groupNumeroColli").show();
        $("#groupPesoTotale").show();
        $("#groupNaturaBeni").show();

        //Impostazione Default Tipo Documento in caso di nuovo documento
        if (lavCodFattura && parseInt($(cIdAgenda).val()) == 0) {
            if (cIdLavCod !== enum_LavCod.Nota_Accredito_Ricevuta.value && cIdLavCod !== enum_LavCod.Nota_Accredito_Emessa.value) {
                Set_KendoDDLValue("inTipoDocumento", 1, "");
                KendoDDL("inTipoDocumento").trigger("change");
            }
            else {
                Set_KendoDDLValue("inTipoDocumento", 4, "");
            }
        }

    } else {

        $("#groupAspettoBeni").hide();
        $("#groupCausaleTrasporto").hide();
        $("#groupNumeroColli").hide();
        $("#groupPesoTotale").hide();
        $("#groupNaturaBeni").hide();

        //Impostazione Default Tipo Documento in caso di nuovo documento
        if (lavCodFattura && parseInt($(cIdAgenda).val()) == 0) {
            if (cIdLavCod !== enum_LavCod.Nota_Accredito_Ricevuta.value && cIdLavCod !== enum_LavCod.Nota_Accredito_Emessa.value) {
                Set_KendoDDLValue("inTipoDocumento", 24, "");
                KendoDDL("inTipoDocumento").trigger("change");
            }
            else {
                Set_KendoDDLValue("inTipoDocumento", 4, "");
            }
        }

    }

    //Rilettura Pendenze Valide
    LeggiElencoPendenze_change();

}

function LeggiElencoPendenze_change() {

    RicercaCausali_Riga_Sync(true);

}