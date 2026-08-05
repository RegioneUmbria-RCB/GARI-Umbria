/**
  
DOC READY

**/

jQuery(document).ready(function () {

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            contattoEditResxArray.push(readResxFile(resxSinglePath, "New_Contatto_Edit_jQueryDocReady.js"));
        });
    }

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });
    // -----------------------------

    elencoTipiRapporto[0].TipoRapporto_Des = TraduzioneMultiResx(contattoEditResxArray, "Continuativo");
    elencoTipiRapporto[1].TipoRapporto_Des = TraduzioneMultiResx(contattoEditResxArray, "Occasionale");

    elencoTipiRubrica[0].TipoRubrica_Des = TraduzioneMultiResx(contattoEditResxArray, "Telefono");
    elencoTipiRubrica[1].TipoRubrica_Des = TraduzioneMultiResx(contattoEditResxArray, "Fax");
    elencoTipiRubrica[2].TipoRubrica_Des = TraduzioneMultiResx(contattoEditResxArray, "Cellulare");
    elencoTipiRubrica[3].TipoRubrica_Des = TraduzioneMultiResx(contattoEditResxArray, "Email");
    elencoTipiRubrica[4].TipoRubrica_Des = TraduzioneMultiResx(contattoEditResxArray, "SitoWeb");
    elencoTipiRubrica[5].TipoRubrica_Des = TraduzioneMultiResx(contattoEditResxArray, "NonAssegnato");

    elencoTipiCosto[1].Udm_Des = TraduzioneMultiResx(contattoEditResxArray, "Ora");

    if ($(Controls.UtenteAbiliato_Modifica_Associazione_Listini).val() === "False") {
        $("#btn_associa_listini_acq").hide();
        $("#btn_associa_listini_ven").hide();
    }

    elencoOperazioniXNote = [
        { "LAV_COD": "1031", "LAV_DES": TraduzioneMultiResx(contattoEditResxArray, "EmissioneDDT", "Emissione DDT") },
        { "LAV_COD": "1001", "LAV_DES": TraduzioneMultiResx(contattoEditResxArray, "EmissioneFatture", "Emissione Fatture") },
        { "LAV_COD": "1053", "LAV_DES": TraduzioneMultiResx(contattoEditResxArray, "EmissioneRicevuteFiscali", "Emissione Ricevute Fiscali") },
        { "LAV_COD": "1052", "LAV_DES": TraduzioneMultiResx(contattoEditResxArray, "EmissioneBolleConferimentoADiversi", "Emissione Bolle di Conferimento a Diversi") },

        { "LAV_COD": "1069", "LAV_DES": TraduzioneMultiResx(contattoEditResxArray, "EmissioneDDTCorrispettivi", "Emissione DDT Corrispettivi") },
        { "LAV_COD": "2002", "LAV_DES": TraduzioneMultiResx(contattoEditResxArray, "EmissioneOrdiniDiVendita", "Emissione Ordini di Vendita") },
        { "LAV_COD": "1064", "LAV_DES": TraduzioneMultiResx(contattoEditResxArray, "EmissioneFattureProforma", "Emissione Fatture Proforma") },
        { "LAV_COD": "1061", "LAV_DES": TraduzioneMultiResx(contattoEditResxArray, "EmissioneDoco", "Emissione Doco") }
    ];

    elencoDocumentiFatturazione = [
        { "LAV_COD": "0", "LAV_DES": TraduzioneMultiResx(contattoEditResxArray, "Fattura", "Fattura") },
        { "LAV_COD": "1", "LAV_DES": TraduzioneMultiResx(contattoEditResxArray, "Autofattura", "Autofattura") }
    ];

    elencoModalitaFatturazione = [
        { "Codice": "0", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "NessunRaggruppamentoInFattura", "Nessun Raggruppamento in Fattura") },
        { "Codice": "1", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "RaggruppamentoDDTInFattura", "Raggruppamento DDT in Fattura") }
    ];

    elencoTipiAbilitazioneLiquidita = [
        { "ChkAbilitazione": "1", "Abilitazione_Des": TraduzioneMultiResx(contattoEditResxArray, "PartitaDoppiaPagamentiIncassi", "Partita Doppia/Pagamenti/Incassi") },
        { "ChkAbilitazione": "2", "Abilitazione_Des": TraduzioneMultiResx(contattoEditResxArray, "PartitaDoppia", "Partita Doppia") },
        { "ChkAbilitazione": "0", "Abilitazione_Des": TraduzioneMultiResx(contattoEditResxArray, "NessunaSoloConsultazione", "Nessuna (Solo Consultazione)") }
    ];

    elencoTipiIndirizzoStandard = [
        { "IndirizzoTipo_Cod": "1", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "SedeOperativa", "Sede Operativa") },
        { "IndirizzoTipo_Cod": "101", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "SedeLegale", "Sede Legale") },
        { "IndirizzoTipo_Cod": "102", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "SedeAziendale", "Sede Aziendale") },
        { "IndirizzoTipo_Cod": "103", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "Stabilimento", "Stabilimento") },

        { "IndirizzoTipo_Cod": "201", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "StabileOrganizzazione", "Stabile Organizzazione") },
        { "IndirizzoTipo_Cod": "2", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "Domicilio", "Domicilio") },
        { "IndirizzoTipo_Cod": "3", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "Residenza", "Residenza") },
        { "IndirizzoTipo_Cod": "4", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "ResidenzaEstiva", "Residenza Estiva") },
        { "IndirizzoTipo_Cod": "5", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "LuogoDiNascita", "Luogo di Nascita") }
    ];

    elencoItaEste = [
        { "Codice": "0", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "ContattoItaliano", "Contatto Italiano") },
        { "Codice": "2", "Descrizione": TraduzioneMultiResx(contattoEditResxArray, "ContattoEstero", "Contatto Estero") },
    ];

    elencoApplicabilitaTipiIndirizzo = [
        { "ApplicabilitaCod": 0, "ApplicabilitaDes": TraduzioneMultiResx(contattoEditResxArray, "PersoneFisiche", "Persone Fisiche") },
        { "ApplicabilitaCod": 1, "ApplicabilitaDes": TraduzioneMultiResx(contattoEditResxArray, "PersoneGiuridiche", "Persone Giuridiche") }
    ];

    // -----------------------------

    var tipoCarica = $(Controls.TipoOperazioneContatto).val();
    var gestisciContabilita = GestisciDatiContabili();
    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);

    //Lettura impostazione utente per mostare/nascondere controllo Calo Peso Default
    let respCaloPeso = LeggiImpostazioniSuperuser(IMPOSTAZIONE_COD_CALO_PESO_DEFAULT)
    if (respCaloPeso == "1") {
        permessoCaloPesoDefault = true;
        $("#divCaloPeso").show();
        $('input[name$="Txt_Calo_Peso"]').kendoNumericTextBox({
            value: 0,
            min: 0,
            decimals: 2,
            spinners: false
        });     
        $('input[name$="Txt_Coeff_Calo_Peso"]').kendoNumericTextBox({
            value: 0,
            min: 0,
            decimals: 2,
            spinners: false
        });  
    } else {
        permessoCaloPesoDefault = false;
        $("#divCaloPeso").hide();
    }

    PopolaGrigliaIndirizziTipo("griglia_indirizzi_tipo");

    // KENDOWINDOW
    $("#nuovoTipoIndirizzoWindow").kendoWindow({
        actions: ["Close"],
        //al momento il refresh non funzia ==> lo disabilito
        //actions: [ "Close", "Refresh" ],
        visible: false,
        draggable: false,
        height: "80%",
        width: "60%",
        modal: true,
        resizable: false,
        title: TraduzioneMultiResx(contattoEditResxArray, "GestioneTipiIndirizzo", "Gestione Tipi Indirizzo"),
        open:
            function (e) {
                $("body").addClass("ob-no-scroll");
            },
        close: function (e) {
            PopolaGrigliaIndirizziTipo("griglia_indirizzi_tipo");
            $("body").removeClass("ob-no-scroll");
        }
    });

    //onchange = "centroAziendaleChange(this)"
    $(Controls.Visibilita).on("change", function (e) {

        if (parseInt(e.target.value) === 0) {
            // Contetto che diventa privato
            // Se no è un inserimento controllo che non sia già utilizzato da altre pive
            var tipoCarica = $(Controls.TipoOperazioneContatto).val();
            if (tipoCarica != 1) {
                var retVal = ControllaContattoRiferimenti();
                if (!retVal) {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    e.target.value = ddlCentriAziendaliPrevValue;
                    return false;
                }
            }
        }
    });

    $(Controls.Visibilita).on("click", function (e) {
        ddlCentriAziendaliPrevValue = e.target.value;
    });

    Inizializza_Combo_IVA_Default();
    Inizializza_Combo_ContoEconomico_Default();
    Inizializza_Combo_ContoPatrimoniale_Default();

    PopolaElencoRapportiContabili();
    PopolaElencoCodiciLingue();
    PopolaElencoQualifiche(true);
    PopolaElencoMansioni(true);
    PopolaElencoClassRisUm(true);
    Popola_Istituti_Credito(false);
    Popola_Piano_Conti(false);
    DropDownRapportiContabiliXCosti();
    // DropDownTipologiaIndirizzo();
    PopolaGrigliaRapportiContabili("griglia_rapporti_contabili", tipoCarica);
    PopolaGrigliaRubrica("griglia_rubrica");
    PopolaGrigliaCosti("griglia_costi");
    PopolaGrigliaIndirizzi("grdIndirizzi");

    if ($(Controls.ImpresaGias).val() === "True") {
        // Nascondo le tab indirizzi e contatti
        $(".tab_rubrica").find("*").css("display", "none");
        $(".tab_indirizzi").find("*").css("display", "none");
    }

    if (gestisciContabilita == true) {
        $(".tab_dettagli_contabili").find("*").css("display", "");
        PopolaGrigliaLiquidita("griglia_liquidita");
        PopolaGrigliaConti("griglia_conti");

        var grigliaLiquidita = $("#griglia_liquidita").data("kendoGrid");
        grigliaLiquidita.tbody.on("click", ".k-checkbox", onClickGrigliaLiquidita);

        grigliaLiquidita.tbody.on("change", "input.chkbx", function (e) {
            var grid = $("#griglia_liquidita").data("kendoGrid");
            var dataItem = grid.dataItem($(e.target).closest("tr"));
            dataItem.set("ChkDefault", this.checked);
            dataItem.dirty = true;
        });
        grigliaLiquidita.bind("cellClose", grid_cellClose);

        var grigliaConti = $("#griglia_conti").data("kendoGrid");
        grigliaConti.bind("cellClose", grid_cellClose);

    }


    Inizializza_Combo_Rapp_Contabile_Principale();
    Inizializza_Combo_Stato();
    Inizializza_Combo_Lingua();
    Inizializza_Combo_ItaEste();
    Inizializza_Combo_ModalitaPagamento();
    Inizializza_Combo_IBAN();


    Inizializza_Combo_Agente();
    Inizializza_Combo_referenteConferimento();
    Inizializza_Combo_CapoArea();
    Inizializza_Combo_Vettore();
    Inizializza_Combo_IndirizzoFatturazione();
    Inizializza_Combo_FatturazioneAutomatica();
    Inizializza_Combo_DocumentoFatturazione();
    Inizializza_Combo_ListinoPrezziAcq();
    Inizializza_Combo_ListinoPrezziVen();
    Inizializza_Combo_GestioneVettore();
    creaKendoDropDownListServerFiltering("ddl_destinazione_diversa", "Rag_Soc_Completa", "Cod_RisUm", Leggi_Clienti_Fornitori_Filtered, destinazioneDiversa_change, 3, -1, "--- " + TraduzioneMultiResx(contattoEditResxArray, "Seleziona", "Seleziona") + " ---", null);
    creaKendoMultiselect("multiselNoteOperazioni", { read: RiempiNoteOperazioni }, "LAV_DES", "LAV_COD");
    creaKendoMultiselect("multiselNoteOperazioni2", { read: RiempiNoteOperazioni }, "LAV_DES", "LAV_COD");
    $('input[name$="idScontoCliente"]').kendoNumericTextBox();
    $('input[name$="idScontoAdd1"]').kendoNumericTextBox();
    $('input[name$="idScontoAdd2"]').kendoNumericTextBox();
    $('input[name$="idScontoAdd3"]').kendoNumericTextBox();
    $('input[name$="idScontoAddTot"]').kendoNumericTextBox();
    $("#idScontoAddTot").data("kendoNumericTextBox").enable(false);
    $('input[name$="idProvvigioneAgente"]').kendoNumericTextBox();
    $('input[name$="idProvvigioneACapoArea"]').kendoNumericTextBox();
    creaKendoSwitch("cb_fittizio", undefined, undefined, false);
    creaKendoSwitch("cb_eudr", undefined, undefined, false);

    if (tipoCarica != 1) {
        var codRisUmDestDiversa = $(Controls.DettCont_Destinazione_Diversa).val();
        if (codRisUmDestDiversa != 0) {
            Leggi_Cliente_Fornitore(codRisUmDestDiversa);
        }
    }

    var ultimoTipoSelezionato = "";
    var apertoDaPopup = $(Controls.AperturaDaPopup).val();
    var btnAnnulla = $(Controls.ImgBtn_AnnullaTutto);
    btnAnnulla.hide();

    var grigliaRappCont = $("#griglia_rapporti_contabili").data("kendoGrid");
    grigliaRappCont.bind("cellClose", grid_cellClose);

    var grigliaCosti = $("#griglia_costi").data("kendoGrid");
    grigliaCosti.bind("cellClose", grid_cellClose);

    $("#ddl_ItaEste").data("kendoDropDownList").bind("select", itaEste_select);
    $("#ddl_ItaEste").data("kendoDropDownList").bind("change", itaEste_change);
    $("#idScontoAdd1").data("kendoNumericTextBox").bind("change", RicalcolaScontoAddizionaleTotale);
    $("#idScontoAdd2").data("kendoNumericTextBox").bind("change", RicalcolaScontoAddizionaleTotale);
    $("#idScontoAdd3").data("kendoNumericTextBox").bind("change", RicalcolaScontoAddizionaleTotale);

    //eventi di click pulsanti
    $("#btn_apri_gestione_documenti").click(function () {
        apri_gestione_documenti();
    });

    $("#btn_associa_listini_acq").click(function () {
        apri_associazione_listini(enum_tipoListino.Acquisto);
    });

    $("#btn_associa_listini_ven").click(function () {
        apri_associazione_listini(enum_tipoListino.Vendita);
    });

    // #region ANNA 03 / 24 -- WIP ALLINEAMENTO PATENTINO  PASSANDO DA DOCUMENTALE.INTERROTTO, RIPRENDERE IN FUTURO!
    if (1 === 2) {
        if (tipoCarica == 1) { //scrittura
            $("#btn_apri_ricerca_documenti").hide()
        } else {
            $("#btn_apri_ricerca_documenti").show()
            //eventi di click pulsanti
            $("#btn_apri_ricerca_documenti").click(function () {
                apri_ricerca_documenti();
            });
        }

        //eventi di click pulsanti
        $("#btn_apri_edit_documenti").click(function () {
            apri_edit_documenti();
        });

        window.addEventListener('message', event => {

            var kWin = $('#tab_documentale').data("kendoWindow");
            var urlKendoWin = kWin.options.content.url;

            if (verificaOriginSecondaria(window, urlKendoWin, event) && event.data.includes("FrameDocumentale")) {
                kWin.close();
            }
        });
    }
    // #endregion


    $(Controls.TipoUtente).on("click", function (e) {
        if (!$(e.target).is("input"))
            return;

        var tipoCarica = $(Controls.TipoOperazioneContatto).val();

        // Se sono in un contatto nuovo e già presenti indirizzi avviso
        if (tipoCarica == 1) {
            var indirizziKendoCount = $("#grdIndirizzi").data("kendoGrid").dataSource._data.length;

            if (indirizziKendoCount > 0) {

                var confirmation = confirm(TraduzioneMultiResx(contattoEditResxArray, "ConfermaCambiamentoTipoContatto", "Cambiando il tipo di contatto verranno eliminati gli indirizzi inseriti. Proseguire?"));
                if (!confirmation) {
                    e.preventDefault();
                    return false;
                }
            }
            else {
                e.stopImmediatePropagation();
                return true;
            }
        }
        else {
            e.stopImmediatePropagation();
            return true;
        }

    });

    $(Controls.TipoUtente).on("change", function (e) {

        var tipoCarica = $(Controls.TipoOperazioneContatto).val();

        // Se sono in un contatto nuovo e già presenti indirizzi avviso
        if (tipoCarica == 1) {
            var indirizziKendoCount = $("#grdIndirizzi").data("kendoGrid").dataSource._data.length;
            if (indirizziKendoCount > 0) {
                SvuotaIndirizziKendo();
            }
        }

        onChangeTipoContatto(tipoCarica);

    });

    $(Controls.TipoIndirizzo).change(function (e) {
        onChangeTipoIndirizzo();
    });


    $(Controls.Provincia).change(function (e) {
        onChangeProvincia();
    });

    $(Controls.Comune).change(function (e) {
        onChangeComune();
    });

    $(Controls.Visibilita).change(function (e) {
        onChangeVisibilita();
    });

    //carico eventualmente la tabella di Rubrica
    var initInidirzzi;
    var initCosti;
    var initRapporti;

    if (jsIndirizzi != "") {
        initInidirzzi = jsIndirizzi;
    } else {
        initInidirzzi = "";
    }

    if (jsRapporti != "") {
        initRapporti = jsRapporti;
    } else {
        initRapporti = "";
    }


    if (tipoCarica == 1) {
        $(Controls.TipoUtente).find('input').prop('disabled', false);
    } else {
        $(Controls.TipoUtente).find('input').prop('disabled', true);
    }

    Inizializza_Combo_Convenevoli();
    Inizializza_Combo_RappFiscale();
    Inizializza_Combo_OriginiSpedizione();
    Inizializza_Combo_TipologieDestinazione();
    Inizializza_Combo_UfficiDogane();

    var fl1 = false;
    var fl2 = false;
    var fl3 = false;
    var fl4 = false;
    var fl5 = false;

    nascondi_riepilogo_error();

    //#region Gestione Riepilogo Errori (Validazione)
    $(Controls.CF).keyup(function () {
        if ($(Controls.CF).val() != "") {
            $('.voce_1').hide();
            fl1 = true;
        }
        else {
            $('.voce_1').show();
            fl1 = false;
        }

        nascondi_riepilogo_error();
    });

    $(Controls.ImgBtn_CF).click(function () {
        $('.voce_1').hide();
        fl1 = true;

        nascondi_riepilogo_error();
    });

    $(Controls.Cognome).keyup(function () {
        if ($(Controls.Cognome).val() != "") {
            $('.voce_2').hide();
            fl2 = true;
        }
        else {
            $('.voce_2').show();
            fl2 = false;
        }

        nascondi_riepilogo_error();
    });

    $(Controls.Nome).keyup(function () {
        if ($(Controls.Nome).val() != "") {
            $('.voce_3').hide();
            fl3 = true;
        }
        else {
            $('.voce_3').show();
            fl3 = false;
        }

        nascondi_riepilogo_error();
    });

    $(Controls.PIVA).keyup(function () {
        if ($(Controls.PIVA).val() != "") {
            $('.voce_4').hide();
            fl4 = true;
        }
        else {
            $('.voce_4').show();
            fl4 = false;
        }
        $(Controls.CF_Estero).val($(Controls.PIVA).val());
        nascondi_riepilogo_error();
    });

    $(Controls.RagioneSociale).keyup(function () {
        if ($(Controls.RagioneSociale).val() != "") {
            $('.voce_5').hide();
            fl5 = true;
        }
        else {
            $('.voce_5').show();
            fl5 = false;
        }

        nascondi_riepilogo_error();
    });

    function nascondi_riepilogo_error() {
        if (fl1)
            $('.voce_1').hide();

        if (fl2)
            $('.voce_2').hide();

        if (fl3)
            $('.voce_3').hide();

        if (fl4)
            $('.voce_4').hide();

        if (fl5)
            $('.voce_5').hide();

        if ((fl1) && (fl2) && (fl3) || (fl4) && (fl5))
            $('#div_riepilogo_error').hide();
        else
            $('#div_riepilogo_error').show();
    }

    //#endregion

    function onActivate(e) {
        var selectedIndex = $(e.item).index();
    }

    //onChangeTipoContatto(tipoCarica);
    $("#ddl_ItaEste").data("kendoDropDownList").trigger("change");

    // Disabilito controlli client
    if (tipoCarica == 0) {
        KendoDDL("ddlConvenevoli").enable(false);
        KendoDDL("Ddl_Rappresentante_Fiscale").enable(false);
        $("#id_multiselNoteOperazioni").data("kendoMultiSelect").enable(false);
        $("#id_multiselNoteOperazioni2").data("kendoMultiSelect").enable(false);
        KendoDDL("ddl_Origine_Spedizione").enable(false);
        KendoDDL("ddl_Cod_Tipo_Destinazione").enable(false);
        KendoDDL("ddl_Cod_Uff_Dogan").enable(false);
        $("#Txt_Altri_Dati_Note").attr("readonly", true);
        $("#Txt_Altri_Dati_Note2").attr("readonly", true);
        if (permessoCaloPesoDefault) {
            $("#Txt_Calo_Peso").data("kendoNumericTextBox").enable(false);
            $("#Txt_Coeff_Calo_Peso").data("kendoNumericTextBox").enable(false);
        }
        $("#cb_fittizio").data("kendoSwitch").enable(false);
        $("#cb_eudr").data("kendoSwitch").enable(false);
        $("#Txt_Memo").attr("readonly", true);
        $("#txt_dich_intenti_data").data("kendoDatePicker").enable(false);

        if (gestisciContabilita == true) {
            $("#idScontoCliente").data("kendoNumericTextBox").enable(false);
            $("#idScontoAdd1").data("kendoNumericTextBox").enable(false);
            $("#idScontoAdd2").data("kendoNumericTextBox").enable(false);
            $("#idScontoAdd3").data("kendoNumericTextBox").enable(false);
            KendoDDL("ddl_iva_default").enable(false);
            KendoDDL("ddl_contoEco_default").enable(false);
            KendoDDL("ddl_contoPat_default").enable(false);
            KendoDDL("ddl_agente").enable(false);
            KendoDDL("ddl_capo_area").enable(false);
            KendoDDL("ddl_vettore").enable(false);
            KendoDDL("ddl_indirizzo_fatturazione").enable(false);
            KendoDDL("ddl_fatturazione_automatica").enable(false);
            KendoDDL("ddl_documento_fatturazione").enable(false);
            KendoDDL("ddl_modalita_pagamento").enable(false);
            KendoDDL("ddl_iban_default").enable(false);
            $("#idProvvigioneAgente").data("kendoNumericTextBox").enable(false);
            $("#idProvvigioneACapoArea").data("kendoNumericTextBox").enable(false);
            KendoDDL("ddl_listino_prezzi_acq").enable(false);
            KendoDDL("ddl_listino_prezzi_ven").enable(false);
            KendoDDL("ddl_gestione_vettore").enable(false);
            KendoDDL("ddl_destinazione_diversa").enable(false);
            KendoDDL("ddl_referenteConferimento").enable(false);

            var kendoIndDestDiv = $("#ddl_ind_destinazione_diversa").data("kendoDropDownList");
            if (kendoIndDestDiv != null && kendoIndDestDiv != undefined)
                kendoIndDestDiv.enable(false);
            else
                $("#ddl_ind_destinazione_diversa").attr("disabled", "disabled");
        }
    }

    if (tipoCarica != 1) {
        $("#txt_dich_intenti_data").val($(Controls.DichiarazioneIntentoDataProtocollo).val());
        $("#Txt_Altri_Dati_Note").val($(Controls.Note).val());
        $("#Txt_Altri_Dati_Note2").val($(Controls.Note2).val());
        if (permessoCaloPesoDefault) {
            var val = $(Controls.CaloPeso).val();
            if (val !== null && val !== undefined && val !== '')
            {
                Set_KendoNumTBValue("Txt_Calo_Peso", JSON.parse($(Controls.CaloPeso).val()));
            }
            val = $(Controls.CoeffCaloPeso).val();
            if (val !== null && val !== undefined && val !== '') {
                Set_KendoNumTBValue("Txt_Coeff_Calo_Peso", JSON.parse($(Controls.CoeffCaloPeso).val()));
            }
            
        }
        KendoMultisel("multiselNoteOperazioni").value($(Controls.NoteOperazioni).val().split("|"));
        KendoMultisel("multiselNoteOperazioni2").value($(Controls.NoteOperazioni2).val().split("|"));

        Set_KendoNumTBValue("idScontoCliente", $(Controls.DettCont_Sconto_Cliente).val());
        Set_KendoNumTBValue("idScontoAdd1", $(Controls.DettCont_Sconto_Add1).val());
        Set_KendoNumTBValue("idScontoAdd2", $(Controls.DettCont_Sconto_Add2).val());
        Set_KendoNumTBValue("idScontoAdd3", $(Controls.DettCont_Sconto_Add3).val());
        $("#idScontoAdd1").data("kendoNumericTextBox").trigger("change");
        Set_KendoNumTBValue("idProvvigioneAgente", $(Controls.DettCont_Provvigione_Agente).val());
        Set_KendoNumTBValue("idProvvigioneACapoArea", $(Controls.DettCont_Provvigione_Capo_area).val());

        $(ImgBtn_CF_ClientId).css("display", "none");
        $(ImgBtn_PIVA_ClientId).css("display", "none");

        $("#Txt_Memo").val($(Controls.Memo).val());
        KendoDDL("ddl_ItaEste").enable(false);
        setKendoSwitch("cb_fittizio", JSON.parse($(Controls.Fittizio).val().toLowerCase()));
        setKendoSwitch("cb_eudr", JSON.parse($(Controls.EUDR).val().toLowerCase()));
    }

    // Controlla Impostazioni Super User - Utente
    var gestioneDaa = GetPropertyFromJson($(Controls.OpzioniContatti).val(), "UTENTE_DAA");
    if (gestioneDaa != 1) {
        $("#ddl_Origine_Spedizione").data("kendoDropDownList").enable(false);
        $("#ddl_Cod_Tipo_Destinazione").data("kendoDropDownList").enable(false);
        $("#ddl_Cod_Uff_Dogan").data("kendoDropDownList").enable(false);
        $(Controls.CodiceAccisa).attr("readonly", true);
        $(Controls.CodiceUA).attr("readonly", true);
        $(Controls.CodContoGaranzia).attr("readonly", true);
        $(Controls.RifDepositoFiscale).attr("readonly", true);
    }

    var apriDatiPatentino = $(Controls.AperturaDatiPatentino).val();
    if (apriDatiPatentino === "True") {
        apri_gestione_documenti();

        //ANNA 03 / 24 -- WIP ALLINEAMENTO PATENTINO  PASSANDO DA DOCUMENTALE.INTERROTTO, RIPRENDERE IN FUTURO!
        //apri_edit_documenti(); SCOMMENTARE QUESTO, RIMUOVERE apri_gestione_documenti()
    }

    var apertoDaPopup = $(Controls.AperturaDaPopup).val();
    if (apertoDaPopup == "True" && tipoCarica == 1) {
        var cod_Rapporto = $(Controls.PopupCodRapporto).val();
        SelezionaKendoDropDownItem("ddl_Rapporto_Principale", cod_Rapporto, "Cod_Rapporto");
        AggiungiRapportoContabilePrincilapele();
    }


    $('body').kendoTooltip({
        filter: '.btn[title]'
    })
});

$('.nav-tabs a').on('shown.bs.tab', function (event) {
    var currentTabName = $(event.target).text();         // active tab
    var previousTabName = $(event.relatedTarget).text();  // previous tab
    var tabId = event.target.id;

    if (currentTabName === "Dettagli Contabili") {      //quando cambio tab e vado in "Dettagli Contabili" controllo se ci sono dei conti economici mancanti
        if (!ContoEconMancante)                         //nel caso non ci fossero, imposto la ddl sul valore di default
            SelezionaKendoDropDownItem("ddl_contoEco_default", CodContoEcon, "Cod_Conto");
        else {                                          //altrimenti notifico con un alert e aggiorno la variabile (così non lo notifica ogni volta che si riapre il tab)
            myalert(contiAlertString);
            ContoEconMancante = false;
        }

        if (!ContoPatMancante)                          //stessa cosa con i conti patrimoniali
            SelezionaKendoDropDownItem("ddl_contoPat_default", CodContoPat, "Cod_Conto_Pat");
        else {
            myalert(contiAlertString);
            ContoPatMancante = false;
        }

        ddlSize();                                      //questa funzione -_-dovrebbe-_- aggiustare le dimensioni delle ddl in modo tale da non occupare spazio inappropriato
    }

    if (currentTabName.includes("Rapporti Contabili")) {//nel tab "Rapporti Contabili" controllo se ci sono dei conti mancanti, stessa idea dei precedenti senza mantenere il valore attuale
        if (ContoContattoMancante) {
            myalert(contiAlertString);
            ContoContattoMancante = false;
        }
    }

});

function myalert(content) {                         //kendo alert persoonalizzati
    $("<div></div>").kendoAlert({
        title: "Conto mancante",
        content: content
    }).data("kendoAlert").open();
}

function ddlSize() {
    $.each($(".calc-width"), function () {
        $(this).children(".input-group-addon").css("width", $(this).children(".input-group-addon").outerWidth());
        $(this).css({ "table-layout": "fixed", "width": "100%" });
    });
}
