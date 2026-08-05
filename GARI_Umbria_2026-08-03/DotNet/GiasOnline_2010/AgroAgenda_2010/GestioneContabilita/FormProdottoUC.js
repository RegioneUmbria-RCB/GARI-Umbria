
// ----------------------------------
// --- Inizio Nuova riga         ----
// ----------------------------------
function DocContabileNuovaRiga() {

    EntrataInRigaDoc();

    //impedisco la modifica di alcuni elementi di testata, se sto facendo una riga
    AbilitaModificaDatiMinimiTestata(false);

    // Rendo visibile il TAB singola riga
    ImpostaVisibilitaElencoOSingolaRiga(true);

    // Resetto i campi che innescano la ricerca dei parametri qualitativi F&F
    current_Elem_Cod = -1;
    current_Veg_Cod = -1;
    current_Cul_Cod = -1;
    current_Reg_Cod = 0;
    current_Mat_Cod_OMNI = -1;
    current_Prodotto_Cod = "";

    if (propostaDatiRiga !== undefined && propostaDatiRiga.categoriaMagazzino !== undefined) {
        Set_KendoDDLValue("ddlCategorieMagazzino", propostaDatiRiga.categoriaMagazzino);
    }

    ImpostaCampiFormProdotto(null, enum_TipoOperazioneDB.Scrittura.value, false);

    Imposta_Visibilita_FormProdottoUC(enum_TipoOperazioneDB.Scrittura.value);

    validatorTabDettaglio = inizializzaKendoValidator("tabDettagliDoc", false, false, true, true);

    MostraBtnEsciRiga(false);

    if (lavCodAccettazionePomodoro) {
        MostraBtnAnnullaRiga(false);
    }
}
// ----------------------------------
// --- Fine Nuova riga           ----
// ----------------------------------

// ----------------------------------------
// --- Inizio Modifica / Duplica Riga  ----
// ----------------------------------------
function Modifica_Riga_Doc(dataItem, operazione) {

    EntrataInRigaDoc();

    // Rendo visibile il TAB singola riga
    ImpostaVisibilitaElencoOSingolaRiga(true);

    ImpostaCampiFormProdotto(dataItem, operazione, false);

    Imposta_Visibilita_FormProdottoUC(operazione);

    Carica_Stati_PanelBar();

    validatorTabDettaglio = inizializzaKendoValidator("tabDettagliDoc", false, false, true, true);
    if (operazione === enum_TipoOperazioneDB.Lettura.value) {
        MostraBtnEsciRiga(true);
    } else {
        MostraBtnEsciRiga(false);
    }

    if (lavCodAccettazionePomodoro) {
        MostraBtnAnnullaRiga(false);
        // AggiornaDettagliEconomici();
    }
}

function Duplica_Riga_Doc(dataItem, operazione) {

    EntrataInRigaDoc();

    // Rendo visibile il TAB singola riga
    ImpostaVisibilitaElencoOSingolaRiga(true);

    ImpostaCampiFormProdotto(dataItem, operazione, false);

    // Resetto i due campi che innescano la ricerca dei parametri qualitativi F&F
    //current_Elem_Cod = -1;
    //current_Veg_Cod = -1;
    //current_Cul_Cod = -1;
    //current_Reg_Cod = 0;
    Imposta_Visibilita_FormProdottoUC(enum_TipoOperazioneDB.Copia.value);  

    validatorTabDettaglio = inizializzaKendoValidator("tabDettagliDoc", false, false, true, true);

    MostraBtnEsciRiga(false);
}

function MostraBtnEsciRiga(visualizza) {
    if (visualizza === true) {
        $("#btn_EsciRigaDoc").show();
        $("#btn_EsciRigaDoc2").show();
        $("#btn_EsciRigaDoc3").show();
        $("#btn_EsciRigaDoc4").show();
    } else {
        $("#btn_EsciRigaDoc").hide();
        $("#btn_EsciRigaDoc2").hide();
        $("#btn_EsciRigaDoc3").hide();
        $("#btn_EsciRigaDoc4").hide();
    }
}

function MostraBtnAnnullaRiga(visualizza) {
    if (visualizza === true) {
        $("#btn_AnnullaModifiche").show();
        $("#btn_AnnullaModifiche2").show();
        $("#btn_AnnullaModifiche3").show();
        $("#btn_AnnullaModifiche4").show();
    } else {
        $("#btn_AnnullaModifiche").hide();
        $("#btn_AnnullaModifiche2").hide();
        $("#btn_AnnullaModifiche3").hide();
        $("#btn_AnnullaModifiche4").hide();
    }
}

function UscitaDaRigaDoc() {
    sonoInDettaglioRigaDoc = false;

    ResetCampiChiaveFormProdotto();
    if (isLavCodSenzaTabRiepilogo() === true) {
        $("#a_tabRiepilogoPesi").hide();
    } else {
        $("#a_tabRiepilogoPesi").show();
    }
    ImpostaVisibilitaTabCastelletto();
    if (isGestitoTabConfezionamento()) {
        if (parseInt($(cIdAgenda).val()) !== 0) {
            $("#a_tabBeniConfezionamento").show();
        } else {
            $("#a_tabBeniConfezionamento").hide();
        }
    } else {
        $("#a_tabBeniConfezionamento").hide();
    }
    if (lavCodAccettazionePomodoro) {
        $("#a_tabTestataDoc").tab("show");
    }

    //Gestione Parametri Distanza
    RiBloccoDistanzaDoc(false, false);
    GestioneVisibilitaDistanzaLock();
}

function EntrataInRigaDoc() {
    sonoInDettaglioRigaDoc = true;
    $("#a_tabBeniConfezionamento").hide();
    $("#a_tabCastelletto").hide();
    $("#a_tabRiepilogoPesi").hide();    //per evitare che si modifichi il peso totale e che si disallineai con il peso netto
}

function ImpostaCampiFormProdotto(dataItem, operazione, impostaDaOrdine) {

    if (isContrattoAffitto()) {
        $("#lblProdottoDes").html(TraduzioneMultiResx(resxFormProdottoUC, "Descrizione", "Descrizione") + ":");
        $("#lblExtra_Str").html(TraduzioneMultiResx(resxFormProdottoUC, "Riferimento", "Riferimento") + ":");
    }

    inizializzaFormDettaglioRiga = true; // Distinguo nelle funzioni di change interne il primo caricamento

    // SALVO LA RIGA ORIGINALE IN MODO DA POTER CONFRONTARE AL MOMENTO 
    // DEL SAVE I CAMPI CHIAVE E I CAMPI QUANTITA'
    if (operazione !== enum_TipoOperazioneDB.Copia.value)
        riga_originale_entrata_FormProdottoUC = dataItem;

    if ((dataItem !== null && operazione === enum_TipoOperazioneDB.Modifica.value) || isContrattoAffitto()) {
        // Non rendo modificabile la categoria di magazzino:
        // - in modifica (ALMENO PER ORA!)
        // - se contratto di affitto
        KendoDDL("ddlCategorieMagazzino").enable(false);
    } else {
        KendoDDL("ddlCategorieMagazzino").enable(true);
    }

    // INIZIO CAMPO PRODOTTO
    CreaDdlProdottoDes();

    /*Creazione Sezione Parametri Indici GHG  */
    creaParametriQualitativi_Indici_GHG()


    // FINE CAMPO PRODOTTO

    if (operazione === enum_TipoOperazioneDB.Scrittura.value) {
        let kddlProdAlias = KendoDDL("ddlProdAlias");
        if (kddlProdAlias.dataSource.data().length > 0) {
            // Sono in questa casistica se l'utente è andato precedentemente in modifica/copia poi in scrittura
            kddlProdAlias.dataSource.data([]);
        }
    }

    if (operazione !== enum_TipoOperazioneDB.Scrittura.value &&
        operazione !== enum_TipoOperazioneDB.Copia.value) {
        //$(hdKendo_RigaDoc).val(kendo.stringify(dataItem));
        $('input[name$="hdKendo_RigaDoc"]').val(kendo.stringify(dataItem));
        $('input[name$="hf_key_mov_dett"]').val(dataItem.key_mov_dett);
        $('input[name$="hf_Cal_Cod"]').val(dataItem.Cal_Cod);
        $('input[name$="hf_Cod_Progetto"]').val(dataItem.Cod_Progetto);
        $('input[name$="hf_riga_DataOraUltimaLettura"]').val(dataItem.DataOraUltimaLettura);
        $('input[name$="hf_Qta_Extra"]').val(dataItem.Qta_Extra);
        $('input[name$="hf_Udm_Cod_Extra"]').val(dataItem.Udm_Cod_Extra);

        if (dataItem.ListRifMovDettaglio !== undefined && dataItem.ListRifMovDettaglio !== null) {
            //rifMovDettaglio = dataItem.RifMovDettaglio;
            listRifMovDettaglio = dataItem.ListRifMovDettaglio;
        } else if (dataItem.List_Riferimenti !== undefined && dataItem.List_Riferimenti !== null &&
            dataItem.List_Riferimenti !== "") {

            let parsedRif = JSON.parse(dataItem.List_Riferimenti);

            if (parsedRif.length > 0) {
                listRifMovDettaglio = parsedRif;
            } else {
                listRifMovDettaglio = [];
            }

            //rifMovDettaglio = {
            //    Piva_Rif: dataItem.Piva_Rif,
            //    Sa_Cod_Rif: dataItem.Sa_Cod_Rif,
            //    Id_Agenda_Rif: dataItem.Id_Agenda_Rif,
            //    Id_Mov_Rif: dataItem.Id_Mov_Rif,
            //    Id_Mov_Det_Rif: dataItem.Id_Mov_Det_Rif,
            //    Lav_Cod_Rif: dataItem.Lav_Cod_Rif,
            //    Cau_Mov_Rif: dataItem.Cau_Mov_Rif,
            //    Qta: dataItem.Qta_Rif
            //};
        } else {
            //rifMovDettaglio = null;
            listRifMovDettaglio = [];
        }

        //if (dataItem.RifMovDettaglio !== undefined && dataItem.RifMovDettaglio !== null) {
        //    rifMovDettaglio = dataItem.RifMovDettaglio;
        //} else if (dataItem.Id_Agenda_Rif !== 0 && dataItem.Id_Mod_Det_Rif !== 0) {
        //    rifMovDettaglio = {
        //        Piva_Rif: dataItem.Piva_Rif,
        //        Sa_Cod_Rif: dataItem.Sa_Cod_Rif,
        //        Id_Agenda_Rif: dataItem.Id_Agenda_Rif,
        //        Id_Mov_Rif: dataItem.Id_Mov_Rif,
        //        Id_Mov_Det_Rif: dataItem.Id_Mov_Det_Rif,
        //        Lav_Cod_Rif: dataItem.Lav_Cod_Rif,
        //        Cau_Mov_Rif: dataItem.Cau_Mov_Rif,
        //        Qta: dataItem.Qta_Rif
        //    };
        //} else {
        //    //rifMovDettaglio = null;
        //    listRifMovDettaglio = [];
        //}

    } else {
        $('input[name$="hdKendo_RigaDoc"]').val("");
        $('input[name$="hf_key_mov_dett"]').val("");
        // Questi valori che fungono da chiave per la giacenza in fase di copia li riporto se sto collegando un ordine/ddt a questa riga, 
        // oppure anche in caso di copia semplice, se sono in scarico, di modo che aggancio la stessa giacenza (se poi era finita, viene gestita di conseguenza
        // in base alle impostazioni di sotto-giacenza)
        if (operazione === enum_TipoOperazioneDB.Copia.value && (impostaDaOrdine || lavCodVendita)) {
            $('input[name$="hf_Cal_Cod"]').val(dataItem.Cal_Cod);
            $('input[name$="hf_Cod_Progetto"]').val(dataItem.Cod_Progetto);
        } else {
            $('input[name$="hf_Cal_Cod"]').val(0);
            $('input[name$="hf_Cod_Progetto"]').val(0);
        }
        $('input[name$="hf_riga_DataOraUltimaLettura"]').val(null);
        $('input[name$="hf_Qta_Extra"]').val(0);
        $('input[name$="hf_Udm_Cod_Extra"]').val(0);
        //rifMovDettaglio = null;
        listRifMovDettaglio = [];
    }

    let ddlCategorieMagazzinoValue = 0;
    if (dataItem !== null && dataItem.Cat_Cod !== 0)
        ddlCategorieMagazzinoValue = dataItem.Cat_Cod;
    else
        ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
    if (ddlCategorieMagazzinoValue !== 0 && ddlCategorieMagazzinoValue !== RIGA_DESCRIZIONE_LIBERA && ddlCategorieMagazzinoValue !== ALTRI_BENI) {  
        CostruisciLinkInfoProdotto(ddlCategorieMagazzinoValue);
    }

    if ((Qs_CaricoScarico === CAU_CARICO && Get_KendoDDLValue("ddlUbicDestinazione") === "") ||
        (Qs_CaricoScarico === CAU_SCARICO && Get_KendoDDLValue("ddlUbicProvenienza") === "")) {
        KendoDDL("ddlProdottoDes").enable(false);
        KendoDDL("ddlProdAlias").enable(false);
    } else {
        KendoDDL("ddlProdottoDes").enable(true);
        KendoDDL("ddlProdAlias").enable(true);
    }
    //TODO txtGiacenzaProvenienza
     
    //TODO txtGiacenzaDestinazione

    if (dataItem !== null) {
        Set_KendoDDLValue("ddlCausale_Riga", dataItem.Pendente);
    } else {
        
        if (elencoCausali_Riga !== null && elencoCausali_Riga.length === 1)
            Set_KendoDDLValue("ddlCausale_Riga", elencoCausali_Riga[0].KeyCausale);
    }
        
    if (dataItem !== null && dataItem.Pendente === enum_Pendenza.Furto) {
        $("#panelBar_Denuncia").show();
        $("#txtNrDenuncia").val(dataItem.Extra_Str);
        set_data("idDataDenuncia", formattedDate(dataItem.Extra_Date, "/"), null);
    } else {
        $("#panelBar_Denuncia").hide();
        $("#txtNrDenuncia").val("");
        set_data("idDataDenuncia", formattedDate(AGRODATAINIZIO, "/"), null);
    }

    if (dataItem !== null) {
        Set_KendoDDLValue("ddlCategorieMagazzino", dataItem.Cat_Cod);
        $("#ddlCategorieMagazzino").trigger("change");
    } else {
        //if ($('input[name$="hf_Categoria_Magazzino_Dft"]').val() !== undefined) {
        //    Set_KendoDDLValueNoDef("ddlCategorieMagazzino", $('input[name$="hf_Categoria_Magazzino_Dft"]').val());
        //    $("#ddlCategorieMagazzino").trigger("change");
        //} else {
            // Lascio impostato quello presente
            $("#ddlCategorieMagazzino").trigger("change");
        //}
    }

    // TODO per trasferimenti e altri dove ci sia sia carico che scarico
    // Aggiorna dataSource magazzini
    if (dataItem !== null && ddlCategorieMagazzinoValue !== RIGA_DESCRIZIONE_LIBERA && ddlCategorieMagazzinoValue !== ALTRI_BENI) {
        if (Qs_CaricoScarico === CAU_CARICO) {
            ImpostaDataSourceDdlUbic("ddlUbicDestinazione", null);
            Set_KendoDDLValue("ddlUbicDestinazione", dataItem.key_Dest);
        } else if (Qs_CaricoScarico === CAU_SCARICO) {
            ImpostaDataSourceDdlUbic("ddlUbicProvenienza", null);
            Set_KendoDDLValue("ddlUbicProvenienza", dataItem.key_Dest);
        } else if (Qs_CaricoScarico === CAU_TRASFERIMENTO) {
            // TODO
            ImpostaDataSourceDdlUbic("ddlUbicProvenienza", null);
            ImpostaDataSourceDdlUbic("ddlUbicDestinazione", null);
        }
    } else {
        // Lascio impostato quello presente
    }

    if (dataItem !== null && is_Trasf_Veg_Anim_FormProdottoUC()) {
        // ----> Lo fa nel trigger("change")
        //if (Get_KendoDDLValue("ddlProdottoDes") !== "" &&
        //    (current_Elem_Cod !== dataItem.Cat_Cod ||
        //    current_Veg_Cod !== dataItem.Veg_Cod ||
        //    current_Cul_Cod !== dataItem.Cul_Cod)) {
        //    current_Elem_Cod = dataItem.Cat_Cod;
        //    current_Veg_Cod = dataItem.Veg_Cod;
        //    current_Cul_Cod = dataItem.Cul_Cod;
        //    creaParametriQualitativi_FF_Zoo(dataItem.Cat_Cod, current_Veg_Cod, current_Cul_Cod);  //TODO Solo se F&F o calibro
        //}
    } else {
        $("#id_parametri_qualitativi_list div").html(""); 
    }

    //TODO txtInfoSulProdotto
    //TODO ddlLottoImpianto
    //TODO chkAggregaLottoImpianto

    if (ddlCategorieMagazzinoValue === SEMILAVORATI_VEGETALI) {
        //let Cod_Progetto = dataItem.Cod_Progetto;
        // TODO ddlCalibro
        // TODO chkParametroQualitativo
        // Vedi Ripristina_DATI_nei_Controlli_2 riga 13917
    }
    
    if (dataItem !== null) {
        $("#txtExtra_Str").val(dataItem.Extra_Str);
    } else {
        $("#txtExtra_Str").val("");
    }

    // Imposta Prodotto: viene volutamente fatto qui
    let w_Prod_Cod = "";

    if (dataItem !== null && ddlCategorieMagazzinoValue !== RIGA_DESCRIZIONE_LIBERA && ddlCategorieMagazzinoValue !== ALTRI_BENI) {

        w_Prod_Cod = Carica_ProdCod_PositivoNegativo(dataItem);

        if (!lavCodAccettazionePomodoro) {

            let w_Dati_Prodotto_Modifica = Carica_Dati_Prodotto_Modifica(w_Prod_Cod, dataItem);

            KendoDDL("ddlProdottoDes").dataSource.data([w_Dati_Prodotto_Modifica]);

        }

    } else {

        switch (true) {

            // Se esiste un solo prodotto utilizzabile per i contratti di affitto lo seleziono

            case (isContrattoAffitto() && operazione === enum_TipoOperazioneDB.Scrittura.value):

                KendoDDL("ddlProdottoDes").dataSource.read();

                let elencoProdotti = KendoDDL("ddlProdottoDes").dataSource.data();

                if (elencoProdotti.length === 1) {
                    w_Prod_Cod = elencoProdotti[0].Prodotto_Cod;
                }

                break;

            // Se non è accettazione pomodoro e proposta innesco da trappola, carico elemento vuoto

            case (!lavCodAccettazionePomodoro && !PropostaInnescoDaTrappola()):

                //let w_Dati_Prodotto_Inserimento = Carica_Dati_Prodotto_Inserimento();

                //KendoDDL("ddlProdottoDes").dataSource.data([w_Dati_Prodotto_Inserimento]);

                break;

        }

    }

    if (w_Prod_Cod !== "") {
        //NB: il settaggio del valore riscatena la read() se è stato impostato un filtro!
        Set_KendoDDLValue("ddlProdottoDes", w_Prod_Cod);
    }

    // per il pomodoro forzo la chiamata perchè non scatta il trigger
    if (lavCodAccettazionePomodoro) {
        KendoDDL("ddlCategorieMagazzino").enable(false);
        ddlProdottoDes_change();
    } else KendoDDL("ddlProdottoDes").trigger("change");
    ////KendoDDL("ddlProdottoDes").bind("filtering", ddlProdottoDes_filtering);

    //Questi vanno per forza settati dopo il prodotto, perché dalla ddl prodotto potrebbe arrivare un default, 
    //ma questo è il dato salvato sul documento, che potrebbe essere diverso
    if (dataItem !== null && ddlCategorieMagazzinoValue !== RIGA_DESCRIZIONE_LIBERA && ddlCategorieMagazzinoValue !== ALTRI_BENI) {

        Set_KendoNumTBValue("idTxt_N", dataItem.N);
        Set_KendoNumTBValue("idTxt_P2O5", dataItem.P205);
        Set_KendoNumTBValue("idTxt_K2O", dataItem.K20);
        Set_KendoNumTBValue("idTxt_Cu", dataItem.Cu);

        let puaRegolamento = dataItem.Cod_Regolamento;
        if (puaRegolamento !== undefined && puaRegolamento !== null && puaRegolamento !== "" && parseInt(puaRegolamento) !== 0) {
            Set_KendoDDLValue("ddlPUARegolamento", puaRegolamento);
            RicercaUdm_Optimize_Regolamento(false, xSa_Cod, xFabbricato_Cod, Qs_CaricoScarico, ddlCategorieMagazzinoValue, parseInt(Get_KendoDDLValue("ddlProdottoDes")), puaRegolamento, false, false, false);
            KendoDDL("ddlUM").dataSource.data(elencoUdmOptimizedRegolamento);
        }
        else {
            Set_KendoDDLValue("ddlPUARegolamento", "0");
        }

        TrattaPuaRegolamento();

    } else {

        Set_KendoNumTBValue("idTxt_N", null);
        Set_KendoNumTBValue("idTxt_P2O5", null);
        Set_KendoNumTBValue("idTxt_K2O", null);
        Set_KendoNumTBValue("idTxt_Cu", null);

    }

    //TODO Verificare se ci sono altri casi da gestire oltre a quello sotto
    // In più va introdotta la nuova gestione in scarico
    if (dataItem !== null && ddlCategorieMagazzinoValue !== RIGA_DESCRIZIONE_LIBERA) {
        if (Qs_CaricoScarico === CAU_CARICO) {
            $("#txtLottoAccettazione").val(dataItem.Lotto);

            let kDataScadenza = KendoDate("dpDataScadenza");

            if (kDataScadenza !== undefined && kDataScadenza !== null) {
                const regexDataScadLotto = new RegExp(/^\d{4}(-\d{2}){2}/);

                let matchesDataScadenza = regexDataScadLotto.exec(dataItem.Lotto);

                if (Array.isArray(matchesDataScadenza)) {
                    kDataScadenza.value(new Date(matchesDataScadenza[0]));
                }
                else {
                    kDataScadenza.value(null);
                }
            }

        } else if (Qs_CaricoScarico === CAU_SCARICO || Qs_CaricoScarico === CAU_TRASFERIMENTO) {
            //elencoLottiAccettazione_FormProdottoUC = [{
            //    Lotto: dataItem.Lotto,
            //    Lotto_Cod: dataItem.Lotto
            //}];
            //ImpostaDdlLottoAccettazione(elencoLottiAccettazione_FormProdottoUC, dataItem.Lotto);
            Set_KendoDDLValue("ddlLottoAccettazione", dataItem.Lotto);
        }  
    } else {
        $("#txtLottoAccettazione").val("");
        //elencoLottiAccettazione_FormProdottoUC = [];
        //ImpostaDdlLottoAccettazione(elencoLottiAccettazione_FormProdottoUC, ""); 
    }
    Set_KendoDDLValue("ddlConfezionamentoLotto", "");

    if ((dataItem !== null && is_Trasf_Veg_Anim_FormProdottoUC()) ||
        (dataItem === null && is_FF_FormProdottoUC())) {

        if (dataItem !== null) {
            $("#txtBoxNote").val(dataItem.FF_ONote_Descrizione);
        }
        else {
            $("#txtBoxNote").val("");
        }

        ImpostaParametriQualitativi(dataItem);
        ImpostaParametriIndiciGHG(LeggiParametriIndiciSalvati());
    }

    // TODO txtDoseEtichetta
    if (dataItem !== null && ddlCategorieMagazzinoValue !== RIGA_DESCRIZIONE_LIBERA) {

        if (dataItem.Udm_Cod === String(enum_Udm.numero.value) && is_Trasf_Veg_Anim_FormProdottoUC() &&
            dataItem.FF_confezione_Tipo_Cod != undefined && dataItem.FF_confezione_Tipo_Cod != null && dataItem.FF_confezione_Tipo_Cod != 0) {

            // prodotto in confezioni
            // in questo caso l'unità di misura è numero, ma devo impostare i kg perché così è espressa la giacenza
            dataItem.Udm_Cod = String(enum_Udm.chilogrammi.value);

            //Nel casio siamo in vendita e non è presente l'udm kg nella ddl la devo anche aggiungere

            // Tramite questa "impostazione" la funzione di change della um, richiamata da impostaDftValueDdlUM, non azzera il lotto e non ricarica la giacenza,
            // da fare perché in questo caso il lotto è coerente con la giacenza anche se l'unità di misura principale è diversa
            $("#ddlUM").data("conservalottoconfezioniff", true);

            let kddlUm = KendoDDL("ddlUM");

            let hasUmKg = kddlUm.dataSource.data().some(function (dataItem) { return parseInt(dataItem.Udm_Cod) === enum_Udm.chilogrammi.value });
            if (!hasUmKg) {
                kddlUm.dataSource.add({
                    Udm_Cod: enum_Udm.chilogrammi.value,
                    Udm_Des: enum_Udm.chilogrammi.name
                });
            }

        }

        impostaDftValueDdlUM(dataItem.Udm_Cod);

        //Sono nella seguente casistica in caso di collegamento di una riga di ddt ad una riga di fattura
        if (operazione === enum_TipoOperazioneDB.Copia.value && gestionePesiRiscontrati === true && (
            dataItem.Peso_Lordo_Riscontrato !== 0 ||
            dataItem.Peso_Netto_Riscontrato !== 0 ||
            dataItem.Num_Imballi_Riscontrati !== -1 ||
            dataItem.Tara_Unit_Imballo_Riscontrata !== -1 ||
            dataItem.Num_Colli_Riscontrati !== -1 ||
            dataItem.Tara_Unit_Collo_Riscontrata !== -1 ||
            dataItem.Num_Conf_Riscontrate !== -1 ||
            dataItem.Tara_Unit_Conf_Riscontrata !== -1)) {

            // I valori riscontrati del ddt devono corrispondere ai valori reali per la fattura
            dataItem.Tara = dataItem.Tara_Totale_Riscontrata;
            dataItem.Qta = dataItem.Peso_Netto_Riscontrato;
            dataItem.KgLordi = dataItem.Peso_Lordo_Riscontrato;
            dataItem.KgNetti = dataItem.Peso_Netto_Riscontrato;

            dataItem.Tara_Totale_Riscontrata = 0;
            dataItem.Peso_Netto_Riscontrato = 0;
            dataItem.Peso_Lordo_Riscontrato = 0;
        }

        let udm_multipli_g = [enum_Udm.chilogrammi.value, enum_Udm.quintali.value, enum_Udm.tonnellate.value];

        if (udm_multipli_g.includes(parseInt(dataItem.Udm_Cod)) && is_Trasf_Veg_Anim_FormProdottoUC()) {
            // TODO U.M. TEST
            let wKgNetti = 0;
            if (dataItem.KgNetti !== 0) {
                Set_KendoNumTBValue("idKgLordi", dataItem.KgLordi);
                Set_KendoNumTBValue("idKgNetti", dataItem.KgNetti);
                wKgNetti = dataItem.KgNetti;

                if (dataItem.Peso_Lordo_Riscontrato !== undefined && dataItem.Peso_Lordo_Riscontrato !== 0)
                    Set_KendoNumTBValue("idKgLordiRiscontrati", dataItem.Peso_Lordo_Riscontrato);

                if (dataItem.Peso_Netto_Riscontrato !== undefined && dataItem.Peso_Netto_Riscontrato !== 0)
                    Set_KendoNumTBValue("idKgNettiRiscontrati", dataItem.Peso_Netto_Riscontrato);

            } else if (dataItem.Qta !== 0) {
                // Questo dovrebbe capitare quando passiamo sotto F&F clienti che non lo erano 
                Set_KendoNumTBValue("idKgLordi", dataItem.Qta + dataItem.Tara);

                if (dataItem.Peso_Netto_Riscontrato !== undefined && dataItem.Peso_Netto_Riscontrato !== 0 &&
                    dataItem.Tara_Totale_Riscontrata !== undefined && dataItem.Tara_Totale_Riscontrata !== 0)
                    Set_KendoNumTBValue("idKgLordiRiscontrati", dataItem.Peso_Netto_Riscontrato + dataItem.Tara_Totale_Riscontrata);

                Set_KendoNumTBValue("idKgNetti", dataItem.Qta);

                wKgNetti = dataItem.Qta;

                if (dataItem.Peso_Netto_Riscontrato !== undefined && dataItem.Peso_Netto_Riscontrato !== 0)
                    Set_KendoNumTBValue("idKgNettiRiscontrati", dataItem.Peso_Netto_Riscontrato);  
            }

            Set_KendoNumTBValue("idDegradoPerc", dataItem.Degrado);   
            calcolaDegrado_FormProdottoUC(wKgNetti, dataItem.Degrado);

            KendoDDL("ddlPrezzoRiferitoA").setDataSource(elencoPrezzoLivelloFF);

        } else {
            Set_KendoNumTBValue("idQuantita", dataItem.Qta);
            if (dataItem.Peso_Netto_Riscontrato !== undefined && dataItem.Peso_Netto_Riscontrato !== 0)
                Set_KendoNumTBValue("idQuantitaRiscontrata", dataItem.Peso_Netto_Riscontrato);

            // TODO seconda UM gestita
            // if (!secondaUMGestita)
            KendoDDL("ddlPrezzoRiferitoA").setDataSource(elencoPrezzoLivelloSoloQta);
            // else
            //      KendoDDL("ddlPrezzoRiferitoA").setDataSource(elencoPrezzoLivelloNoFF);

        }

        Set_KendoNumTBValue("idTara", dataItem.Tara);
        if (dataItem.Tara_Totale_Riscontrata !== undefined && dataItem.Tara_Totale_Riscontrata !== 0)
            Set_KendoNumTBValue("idTaraRiscontrata", dataItem.Tara_Totale_Riscontrata);

        Set_KendoDDLValue("ddlPrezzoRiferitoA", dataItem.Prezzo_Livello);

        //if (VerificaValorizzazionePesiRiscontrati() === false) {
        //    // Se i valori riscontrati corrispondono ai rispettivi default, li disabilito
        //    let ntbKgLordiRisc = KendoNumTB("idKgLordiRiscontrati");
        //    ntbKgLordiRisc.enable(false);

        //    let ntbKgNettiRisc = KendoNumTB("idKgNettiRiscontrati");
        //    ntbKgNettiRisc.enable(false);

        //    let ntbQuantitaRisc = KendoNumTB("idQuantitaRiscontrata");
        //    ntbQuantitaRisc.enable(false);
        //}

    } else {
        if (Get_KendoDDLValue("ddlUM") === "0")
            impostaDftValueDdlUM(0); 
        AzzeraQuantita();
        AzzeraGiacenza();
        if (is_FF_FormProdottoUC()) {
            KendoDDL("ddlPrezzoRiferitoA").setDataSource(elencoPrezzoLivelloFF);
            Set_KendoDDLValueNoDef("ddlPrezzoRiferitoA", -1);
        } 
        else {
            KendoDDL("ddlPrezzoRiferitoA").setDataSource(elencoPrezzoLivelloSoloQta);
            Set_KendoDDLValueNoDef("ddlPrezzoRiferitoA", 0);
        }

        // In scrittura, Disabilito i campi riscontrati, vengono riabilitati dal pulsante "ImpostaValoriRiscontrati"
        //let ntbKgLordiRisc = KendoNumTB("idKgLordiRiscontrati");
        //ntbKgLordiRisc.enable(false);

        //let ntbKgNettiRisc = KendoNumTB("idKgNettiRiscontrati");
        //ntbKgNettiRisc.enable(false);

        //let ntbQuantitaRisc = KendoNumTB("idQuantitaRiscontrata");
        //ntbQuantitaRisc.enable(false);

    }
     
    if (dataItem !== null && ddlCategorieMagazzinoValue !== RIGA_DESCRIZIONE_LIBERA) {
        Set_KendoNumTBValue("idPrezzo", dataItem.Prezzo_Unitario);
        Set_KendoNumTBValue("idScontoBase", Math.abs(dataItem.Sconto)); //lo sconto su db è salvato come numero negativo, ma qui lo devo mostrare positivo
        Set_KendoNumTBValue("idScontoAddiz1", dataItem.Sconto1);
        Set_KendoNumTBValue("idScontoAddiz2", dataItem.Sconto2);
        Set_KendoNumTBValue("idScontoAddiz3", dataItem.Sconto3);
        Set_KendoNumTBValue("idScontoCalcolato", dataItem.Sconto_Calcolato);
        Set_KendoNumTBValue("idPrezzoNetto", dataItem.Prezzo_Unitario_Netto);
        Set_KendoDDLValue("ddlScontoModalita", dataItem.Sconto_Modalita);
        Set_KendoDDLValue("ddlScontoMagg", dataItem.ScontoMaggiorazione);
        if (lavCodAccettazionePomodoro === true) {
            Set_KendoNumTBValue("idImponibileTotale", dataItem.Prezzo_Unitario_Netto * Math.round(dataItem.KgNetti * (1 - dataItem.Degrado / 100)));
            Set_KendoNumTBValue("idImponibileTotaleNetto", dataItem.Prezzo_Unitario_Netto * Math.round(dataItem.KgNetti * (1 - dataItem.Degrado / 100)));
        } else {
            Set_KendoNumTBValue("idImponibileTotale", dataItem.Imponibile);
            Set_KendoNumTBValue("idImponibileTotaleNetto", dataItem.Imponibile_Netto);
        }
        Set_KendoDDLValue("ddlCodIva", dataItem.Cod_Iva);

        setKendoSwitch("chkForzaIva", dataItem.ChkIva_Manuale);
        Set_KendoNumTBValue("idIva", dataItem.Iva);
        if (getKendoSwitch("chkForzaIva")) {
            KendoNumTB("idIva").enable(true); //campo idIva modificabile
        } else {
            KendoNumTB("idIva").enable(false);
        }
        Set_KendoNumTBValue("idImportoUnitario", dataItem.Importo_Unitario);
        Set_KendoNumTBValue("idImportoTotale", dataItem.Importo_Totale);
        Set_KendoDDLValue("ddlValoreRiferimento", dataItem.TempoCarenza);
        Set_KendoDDLValue("ddlAnno", dataItem.Anno);
        Set_KendoDDLValue("ddlContoEconomico", dataItem.Cod_Conto_Economico);
        Set_KendoDDLValue("ddlContoPatrimoniale", dataItem.Cod_Conto_Patrimoniale);
        Set_KendoNumTBValue("idProvvigioni", dataItem.Provvigione);

        //rende visibili/abilitati i giusti campi in base ai valori dei campi
        CampiEconomiciAbilitati(false);

        //Devo forzare l'aggiornamento economico perché ci sono campi che non sono calcolati sulla griglia (ad es: ScontoCalcolatoEuro)
        AggiornaDettagliEconomici();
    } else {
        AzzeraPrezziScontiImporti();
        Set_KendoDDLValue("ddlAnno", kendo.parseDate($("#inDataEmissione").val()).getFullYear());
        Set_KendoDDLValue("ddlContoEconomico", 0); //TODO
        Set_KendoDDLValue("ddlContoPatrimoniale", 0); //TODO
        Set_KendoNumTBValue("idProvvigioni", 0);
    }

    if (dataItem !== null && (ddlCategorieMagazzinoValue === RIGA_DESCRIZIONE_LIBERA || ddlCategorieMagazzinoValue === ALTRI_BENI)) {
        $("#txtBeniStrumentali").val(dataItem.Mov_Det_Des);
    } else {
        $("#txtBeniStrumentali").val("");
    }

    //TODO: da fare solo se Conferimento/ConferimentoPomodoro!!!
    //TODO: casomai fare due if una dentro l'altra (i campi in questione esistono sempre)
    if (lavCodAccettazionePomodoro === true) {
        if (dataItem !== null && ddlCategorieMagazzinoValue !== RIGA_DESCRIZIONE_LIBERA) {
            $("#txtTagliandoPesa").val(dataItem.TagliandoPesa);
            Set_KendoDDLValue("ddlCodVarietaPomodoro", dataItem.CodVarietaConferimento);
            $("#txtDescAppezzamenti").val(dataItem.DescAppezzamenti);
            //dataItem.PremioComplessivo
        } else {
            $("#txtTagliandoPesa").val("");
            Set_KendoDDLValue("ddlCodVarietaPomodoro", "");
            $("#txtDescAppezzamenti").val("");
        }
    }

    if (dataItem !== null
        ////&& Qs_CaricoScarico === CAU_SCARICO
    ) {
        // Per scarichi F&F viene fatto a comando con un pulsante  
        ////if (!is_Trasf_Veg_Anim_FormProdottoUC() ||
        ////    (cIdLavCod !== enum_LavCod.Ordine_Vendita_Emesso.value &&
        ////        cIdLavCod !== enum_LavCod.DDT_Emesso.value &&
        ////        cIdLavCod !== enum_LavCod.Fattura_Emessa.value)) {
            // E comunque quando si entra in modifica scatta la change del prodotto e quindi la fa da lì 
           CaricaGiacenze(false);
        ////}
    } 


    if (dataItem !== null && ddlCategorieMagazzinoValue !== RIGA_DESCRIZIONE_LIBERA) {
        $("#rigaRifNOrdine").val(dataItem.N_Doc_Cliente);
        set_data("rigaRifDataOrdine", dataItem.Data_Doc_Cliente, null);
    }
    else {
        $("#rigaRifNOrdine").val("");
        set_data("rigaRifDataOrdine", null, null);
    }

    if (dataItem !== null) {
        // Questi campi vengono mostrati secondo i criteri dati da questa funzione: isVisibile_NotaDDTEsterna
        $("#notaDDTEsterna").val(dataItem.N_Nota_DDT);
        $("#notaRigaDDTEsterna").val(dataItem.N_Nota_Riga_DDT);
        set_data("notaDataDDTEsterna", dataItem.Data_Nota_DDT, null);
    }

    Visibilita_PanelRaccolte(false);
    if ((isContattoImpresaGias || isProduttoreImpresaGias) && is_210_FormProdottoUC()) {

        let dsRaccolteSelPreEsistenti = [];

        if (raccolteXConferimenti_AbilitazioneGenerale()) {

            Titolo_PanelRaccolte();
            Visibilita_PanelRaccolte(true);

            // Reimposto le raccolte selezionate in caso avessi precedentemente aperto una riga in modifica
            raccolteXConferimenti_dsSelezionate = [];

            let idMovDetConf = 0;
            if (operazione === enum_TipoOperazioneDB.Modifica.value) {
                idMovDetConf = dataItem.Id_Mov_Det;
            }

            let numRaccolte = 0;

            let pivaContattoRaccolte = GetPivaConferente();
            if (pivaContattoRaccolte !== "") {
                numRaccolte = raccolteConfUC_set(pivaContattoRaccolte, KendoDate("inDataEmissione").value(), idMovDetConf);
            }

            if (numRaccolte > 0) {

                // Se sono in modifica, verifico il tipo di associazione delle raccolte collegate alla riga del conferimento:
                // perché se la colonna è zero, significa che l'associazione era stata fatta con una raccolta creata automaticamente,
                // di conseguenza devo nascondere la mia nuova funzionalità
                if (operazione === enum_TipoOperazioneDB.Modifica.value) {
                    raccolteXConferimenti_dsSelezionate = raccolteConfUC_ottieniRigheSelezionate();

                    if (raccolteXConferimenti_dsSelezionate.length > 0) {
                        dsRaccolteSelPreEsistenti = raccolteXConferimenti_dsSelezionate.filter(function (dr) {
                            return parseInt(dr.Tipo_Associazione) === 1;
                        });

                        if (dsRaccolteSelPreEsistenti.length === 0) {
                            Visibilita_PanelRaccolte(false);
                        }
                    }
                }

            }
            else {
                Visibilita_PanelRaccolte(false);
            }

        }

        // Mostro la modalità di scelta impianti se è abilitata dalle relative impostazioni e se non ci sono raccolte pre-esistenti selezionate
        if (imputazioneImpianti_AbilitazioneGenerale() && dsRaccolteSelPreEsistenti.length === 0) {
            $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ImputazioneImpianti]).toggle(true);

            // Inizializzo la griglia degli impianti solo in modifica, perché in scrittura viene inizializzata al change del prodotto (TODO Verificare il comportamento in copia ed in lettura)
            if (operazione === enum_TipoOperazioneDB.Modifica.value) {
                ConfiguraGrigliaImpianti("tab_griglia_impianti", 1);
            }
        }
        else {
            $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ImputazioneImpianti]).toggle(false);
        }

    }

    inizializzaFormDettaglioRiga = false;
}

function ImpostaDdlLottoAccettazione(elencoLottiAccettazione_FormProdottoUC, dftValue) {
    let ddlLottoAccettazione = KendoDDL("ddlLottoAccettazione");

    // Se la gestione giacenza ammette di andare in negativo dò possibilità di aggiungere un lotto
    let w_gest_giacenza = getGestioneGiacenza();
    if (w_gest_giacenza !== enum_Gestione_Giacenze_TuttiProdotti) {
        ddlLottoAccettazione.setOptions({ noDataTemplate: "Lotto non trovato" });

    } else {
        ddlLottoAccettazione.setOptions({ noDataTemplate: $("#aggiuntaLottoAccettazioneTemplate").html() });

    }
    ddlLottoAccettazione.dataSource.data(elencoLottiAccettazione_FormProdottoUC);
    ddlLottoAccettazione.value(dftValue);
}

function ImpostaDataSourceDdlUbic(IdControllo, dftValue) {
    if (KendoDDL("ddlCategorieMagazzino").dataSource._data.length !== 0) {
        let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));     

        if (ddlCategorieMagazzinoValue !== 0) {
            let x = 0;
            let cambiaDS = true;
            let DScorrente = KendoDDL(IdControllo).dataSource.data();

            if (lavCodAccettazione)
                // Se è accettazione non cambio nulla perchè sta già lavorando con le sole celle
                cambiaDS = false;
            else {
                // Non cambio nulla se il dataSource da applicare è lo stesso
                let foundCella = false;
                let foundMag = false;

                for (x = 0; x < DScorrente.length; x++) {
                    if (DScorrente[x].key_Dest !== "") {
                        if (DScorrente[x].Tipo_Destinazione === CELLA)
                            foundCella = true;
                        if (DScorrente[x].Tipo_Destinazione !== CELLA)
                            foundMag = true;
                    }
                }

                //se non ha elementi, oppure l'unico elemento è quello vuoto, non entro qua dentro e cambio sempre il DS
                if (DScorrente.length > 0 && !(DScorrente.length === 1 && DScorrente[0].key_Dest === "") ) {

                        if (is_Trasf_Veg_Anim_FormProdottoUC()) {
                            if (!foundMag)
                                cambiaDS = false;
                        } else {
                            if (!foundCella)
                                cambiaDS = false;
                        }

                }

            }

            if (cambiaDS) {
                if (is_Trasf_Veg_Anim_FormProdottoUC())
                    KendoDDL(IdControllo).dataSource.data(elencoCelle);
                else
                    KendoDDL(IdControllo).dataSource.data(elencoMagazzini);

                if (dftValue !== null) {
                    Set_KendoDDLValue(IdControllo, dftValue);
                    $("#" + IdControllo).trigger('change');
                } else {
                    DScorrente = KendoDDL(IdControllo).dataSource.data();
                    if (DScorrente.length === 2) {
                        let keyMag = "";
                        for (x = 0; x < DScorrente.length; x++) {
                            if (DScorrente[x].key_Dest !== "") {
                                keyMag = DScorrente[x].key_Dest;
                                break;
                            }
                        }
                        Set_KendoDDLValue(IdControllo, keyMag);
                        AzzeraGiacenza();
                        // N.B.  non eseguo volutamente il change del magazzino sennò va in loop, dato che arrivo qui dal change categoria magazzino
                        ///////$("#" + IdControllo).trigger('change');
                    }
                }
            }
        } else {
            KendoDDL(IdControllo).dataSource.data(elencoCelleMagazzini);
        }
    }
}

// ----------------------------------------
// --- Fine Modifica / Duplica Riga    ----
// ----------------------------------------


// ---------------------------------------------------------------------------------------------------------------------------
// --- Inizio Funzione che forza il ricalcolo del peso lordo a partire dal totale camion - Tara Modifica / Duplica Riga   ----
// ---------------------------------------------------------------------------------------------------------------------------

function Forza_Modifica_Riga_Doc(KgLordiDaForzare, msg) {

    let aggNonPossibile = false;
    let dataItem = null;
    let righeImpiantiValorizzate = null;
    let udm_multipli_g = [enum_Udm.chilogrammi.value, enum_Udm.quintali.value, enum_Udm.tonnellate.value];

    let nrRigheConferimento = KendoGrid("tab_elenco_movimenti").dataSource.data().length;

    if (nrRigheConferimento !== 1) {

        aggNonPossibile = true;

        msg = TraduzioneMultiResx(resxFormProdottoUC, "AttenzionePesiDelDocumentoNonCoerenti", // La variabile di traduzione contiene anche il resx di pagina DocContabile
            'ATTENZIONE - Su scheda "Riepilogo Pesi" il peso totale del documento è diverso dalla somma dei pesi lordi dei prodotti + imballi vuoti + tara del veicolo.');

        msg += " " + kendo.format(TraduzioneMultiResx(resxFormProdottoUC, "PresentiNRigheDiEntrataImpossibileAggiornarePesiAutomatico",
            'Sono presenti {0} righe di entrata, non è possibile fare aggiornamenti in automatico.'), nrRigheConferimento);

        msg += " " + TraduzioneMultiResx(resxFormProdottoUC, "ModificheSalvateControllareDati", 'Le modifiche sono state comunque salvate, controllare i dati');

    }

    if (!aggNonPossibile) {
       
        dataItem = KendoGrid("tab_elenco_movimenti").dataSource.data()[0];
        let w_um = 0;
        if (dataItem.Udm_Cod !== null && dataItem.Udm_Cod !== undefined)
            w_um = parseInt(dataItem.Udm_Cod);
       
        if (w_um === 0 || !udm_multipli_g.includes(w_um)) {
            msg = TraduzioneMultiResx(resxRiepilogoPesiUC, "AttenzionePesiDelDocumentoNonCoerenti",
                'ATTENZIONE - Su scheda "Riepilogo Pesi" il peso totale del documento è diverso dalla somma dei pesi lordi dei prodotti + imballi vuoti + tara del veicolo.');

            aggNonPossibile = true;
        } else {

            EntrataInRigaDoc();

            ImpostaCampiFormProdotto(dataItem, enum_TipoOperazioneDB.Modifica.value, false);

            if (KgLordiDaForzare <= kendo.parseFloat(Get_KendoNumTBValue("idTara"))) {
                msg = TraduzioneMultiResx(resxFormProdottoUC, "AttenzionePesiDelDocumentoNonCoerenti",
                    'ATTENZIONE - Su scheda "Riepilogo Pesi" il peso totale del documento è diverso dalla somma dei pesi lordi dei prodotti + imballi vuoti + tara del veicolo.');

                msg += TraduzioneMultiResx(resxFormProdottoUC, "PesiRigaNonAggiornatiPesoLordoMinoreTaraImballi",
                    'Non è stato possibile aggiornare la riga in automatico perchè il peso lordo di riga sarebbe minore della Tara degli imballi inseriti sulla riga.');

                msg += " " + TraduzioneMultiResx(resxFormProdottoUC, "ModificheSalvateControllareDati", 'Le modifiche sono state comunque salvate, controllare i dati');

                aggNonPossibile = true;
            }
        }
    }

    if (!aggNonPossibile) {

        if (isImputazioneImpiantiAttiva()) {

            ConfiguraGrigliaImpianti("tab_griglia_impianti", 1);

            if (KendoGrid("tab_griglia_impianti") !== undefined) {
                let righeImpiantiAll = KendoGrid("tab_griglia_impianti").dataSource.data();

                righeImpiantiValorizzate = righeImpiantiAll.filter(function (dataItem) { return dataItem.Qta !== 0; });

                if (righeImpiantiValorizzate !== undefined && righeImpiantiValorizzate !== null && righeImpiantiValorizzate.length > 1) {
                    aggNonPossibile = true;
                    msg = TraduzioneMultiResx(resxFormProdottoUC, "AttenzionePesiDelDocumentoNonCoerenti",
                        'ATTENZIONE - Su scheda "Riepilogo Pesi" il peso totale del documento è diverso dalla somma dei pesi lordi dei prodotti + imballi vuoti + tara del veicolo.');

                    msg += " " + TraduzioneMultiResx(resxFormProdottoUC, "PesiRigaNonAggiornatiPiuImpiantiPerRaccolta",
                        'Non è stato possibile aggiornare la riga in automatico perchè sono stati scelti più impianti a cui attribuire il raccolto.');

                    msg += " " + TraduzioneMultiResx(resxFormProdottoUC, "ModificheSalvateControllareDati", 'Le modifiche sono state comunque salvate, controllare i dati');
                }


            }
        }
    }
                    
    if (!aggNonPossibile) {
        // TODO U.M. TEST
        let w_um = parseInt(Get_KendoDDLValue("ddlUM"));
        let kgLordiTB = KgLordiDaForzare;

        switch (w_um) {
            case enum_Udm.quintali.value:
                kgLordiTB = kgLordiTB / 100;
                break;
            case enum_Udm.tonnellate.value:
                kgLordiTB = kgLordiTB / 1000;
        }

        Set_KendoNumTBValue("idKgLordi", kgLordiTB);
        KendoNumTB("idKgLordi").trigger("change");

        if (righeImpiantiValorizzate !== undefined && righeImpiantiValorizzate !== null)
            SalvaRigaDoc(Get_KendoNumTBValue("idKgNetti", true));
        else
            SalvaRigaDoc();

        msg = 'ATTENZIONE - Su scheda "Riepilogo Pesi" il peso totale del documento era diverso dalla somma dei pesi lordi dei prodotti + imballi vuoti + tara del veicolo.  ';
        msg += 'La riga di conferimento è stata modificata in automatico portando il peso lordo a ' + String(KgLordiDaForzare) + ' kg';

        msg = kendo.format(TraduzioneMultiResx(resxFormProdottoUC, "PesiDocumentoAggiornatiImpostandoPesoLordoRiga",
            'ATTENZIONE - Su scheda "Riepilogo Pesi" il peso totale del documento era diverso dalla somma dei pesi lordi dei prodotti + imballi vuoti + tara del veicolo.' +
            ' La riga di conferimento è stata modificata in automatico portando il peso lordo a {0} kg'), KgLordiDaForzare);

    }
     
    return msg;
}


// -------------------------------------------------------------------------------------------------------------------------
// --- Fine Funzione che forza il ricalcolo del peso lordo a partire dal totale camion - Tara Modifica / Duplica Riga   ----
// -------------------------------------------------------------------------------------------------------------------------


// -----------------------------------------
// --- Inizio evento salvataggio riga   ----
// -----------------------------------------

function EstraiCampiInput() {

    let elemCod = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino", 0));

    // Scarico il magazzino se non sono negli ordini/contratti oppure se sto movimentando Macchine, Altri Beni, Servizi
    let jolly_int = enum_JollyInt.MagazzinoMovimentato;
    if (CategorieMagazzinoNonMovimentato.includes(elemCod) || lavCodOrdine === true || isContrattoAffitto()) {
        jolly_int = enum_JollyInt.MagazzinoNonMovimentato;
    }

    //Marco: Controllo Fattura non Accompagnatoria e dettaglio collegato.
    //Al momento funziona così_ se è agganciato ad un dettaglio è sempre non movimentato a meno che si tratti di un ordine.
    //Da completare man mano con le casistiche mancanti e non ancora implementate
    if (jolly_int == enum_JollyInt.MagazzinoMovimentato && lavCodFattura === true && getKendoSwitch("chkAccompagnatoria") == false &&
        listRifMovDettaglio !== null && listRifMovDettaglio.length > 0) {

        if (listRifMovDettaglio.some(function(x) { return isLavCodOrdine(x.Lav_Cod_Rif) === false; })) {
            jolly_int = enum_JollyInt.MagazzinoNonMovimentato; // Fattura non accompagnatoria collegata e dettaglio non ordine
        }

        //if (isLavCodOrdine(rifMovDettaglio.Lav_Cod_Rif) === false) {        
        //    jolly_int = enum_JollyInt.MagazzinoNonMovimentato; // Fattura non accompagnatoria collegata e dettaglio non ordine
        //}
    }

    if (jolly_int == enum_JollyInt.MagazzinoMovimentato && isLavCodNotaAccredito(cIdLavCod) === true && getKendoSwitch("chkAccompagnatoria") == false) {
        jolly_int = enum_JollyInt.MagazzinoNonMovimentato;
    }

    let piva = $(cIdPiva).val();
    let saCod = 0;
    let idAgenda = 0;
    let idMov = 0;
    let idMovDet = 0;
    let appezzaDestOld = 0;
    let tipoDestinazioneOld = 0;
    let saCodDestOld = 0;
    let idDestinazioneOld = 0;

    let oldKeyDet = $('input[name$="hf_key_mov_dett"]').val();
    if (oldKeyDet !== "") {
        piva = oldKeyDet.split("_")[0];
        saCod = parseInt(oldKeyDet.split("_")[1]);
        idAgenda = parseInt(oldKeyDet.split("_")[2]);
        idMov = parseInt(oldKeyDet.split("_")[3]);
        idMovDet = parseInt(oldKeyDet.split("_")[4]);

        appezzaDestOld = parseInt(oldKeyDet.split("_")[5]);
        tipoDestinazioneOld = parseInt(oldKeyDet.split("_")[6]);
        saCodDestOld = parseInt(oldKeyDet.split("_")[7]);
        idDestinazioneOld = parseInt(oldKeyDet.split("_")[8]);
    } else {
        //TODO: DEBUG GIULIA --> SISTEMA
        //recupero quello che posso dalla testata!!!
    }

    let tipoDestinazioneScarico = 0;
    let saCodDestScarico = 0;
    let idDestinazioneScarico = 0;
    let ubicazioneScarico = Get_KendoDDLValue("ddlUbicProvenienza");
    if (ubicazioneScarico !== "") {
        tipoDestinazioneScarico = parseInt(ubicazioneScarico.split("_")[0]);
        saCodDestScarico = parseInt(ubicazioneScarico.split("_")[1]);
        idDestinazioneScarico = parseInt(ubicazioneScarico.split("_")[2]);
    }

    let tipoDestinazioneCarico = 0;
    let saCodDestCarico = 0;
    let idDestinazioneCarico = 0;
    let ubicazioneCarico = Get_KendoDDLValue("ddlUbicDestinazione");
    if (ubicazioneCarico !== "") {
        tipoDestinazioneCarico = parseInt(ubicazioneCarico.split("_")[0]);
        saCodDestCarico = parseInt(ubicazioneCarico.split("_")[1]);
        idDestinazioneCarico = parseInt(ubicazioneCarico.split("_")[2]);
    }

    if (isContrattoAffitto()) {
        saCodDestCarico = Qs_SaCod;
    }

    let proCod = 0;
    let matCod = 0;
    let codProdotto = parseInt(Get_KendoDDLValue("ddlProdottoDes"));
    if (codProdotto < 0) {
        matCod = Math.abs(codProdotto);
    } else {
        proCod = codProdotto;
    }
     
    // Cal_Cod
    let w_cal_cod = parseInt($('input[name$="hf_Cal_Cod"]').val());
    let w_cod_progetto = parseInt($('input[name$="hf_Cod_Progetto"]').val());
    //TODO Da completare quando si fanno gli scarichi
    //if (elemCod === TRASFORMATI_VEGETALI &&
    //    w_cal_cod !== 0) {
    //    let altriMovimentiStessoCalCod = Verifica_Utilizzo_CalCod();
    //    if (altriMovimentiStessoCalCod !== "")
    //        w_cal_cod = 0;
    //}
  

    // Se gestiti ricerca nr totale imballaggi, contenitori e confezioni dalla griglia
    // TODO Questo sotto è da migliorare: occorrerebbe caricarla solo all'onchange del prodotto
    let wNrImb_FF = 0;
    let wNrCont_FF = 0;
    let wNrConfez_FF = 0;
    if ((elemCod === TRASFORMATI_VEGETALI || elemCod === TRASFORMATI_ANIMALI ) &&
        KendoGrid("tab_imballaggi_formProdottoUC") !== undefined) {
        if (gestitoImballaggio_FF) {
            wNrImb_FF = kendo.parseFloat(KendoGrid("tab_imballaggi_formProdottoUC").dataSource.aggregates().NrImballaggi.sum);
        }
        if (gestitoContenitore_FF) {
            wNrCont_FF = kendo.parseFloat(KendoGrid("tab_imballaggi_formProdottoUC").dataSource.aggregates().NrContenitori.sum);
        }
        if (gestitoConfezione_FF) {
            wNrConfez_FF = kendo.parseFloat(KendoGrid("tab_imballaggi_formProdottoUC").dataSource.aggregates().NrConfezioni.sum);
        }
    }
    
    let pendente = getPendente(elemCod);
    let w_extra_str = $('input[name$="txtExtra_Str"]').val();
    let w_extra_date = "";
    if (pendente !== null && pendente === enum_Pendenza.Furto) {
        w_extra_str = $('input[name$="txtNrDenuncia"]').val();
        w_extra_date = get_data("idDataDenuncia");
    }
     
    let w_LottoAccettazione = "";
    Set_KendoDDLValue("ddlConfezionamentoLotto", "");
    if (Qs_CaricoScarico === CAU_CARICO) {
        let w_gest_lotti = getGestioneLotti();
        if (w_gest_lotti === enum_Gestione_Lotti_Nessuna) {
            $("#txtLottoAccettazione").val("");
            w_LottoAccettazione = "";
        } else {

            // TODO Questa assegnazione è da passare lato server
            if ($('input[name$="txtLottoAccettazione"]').val() === "") {

                //TODO: ATTENZIONE - va sistemato per il verso e capire come fare e se serve quando non si è in accettazione
                // se non faccio così va in errore, per esempio con ddt ricevuto e bene con vegetale
                if (lavCodAccettazione === true) {
                    let lotto = Impostazione_LottoProdotto(w_gest_lotti);
                    if (lotto !== null)
                        $("#txtLottoAccettazione").val(lotto);
                    else {
                        if (w_gest_lotti === enum_Gestione_Lotti_Obbligatoria)
                            return null;
                        else
                           {
                            $("#txtLottoAccettazione").val("");
                        }
                    }

                }
            }
            w_LottoAccettazione = $('input[name$="txtLottoAccettazione"]').val();

            // Verifico la gestione della data di scadenza, perché in alcuni casi la imposto prima del salvataggio nel caso sia valorizzata
            let gestDataScad = ValImpDataScadLotto();

            let dataScad = null;

            let kDataScadenza = KendoDate("dpDataScadenza");

            if (kDataScadenza !== undefined && kDataScadenza !== null) {
                dataScad = kDataScadenza.value();
            }

            if (gestDataScad !== enum_Gestione_Lotti_Nessuna && dataScad !== null) {
                w_LottoAccettazione = concatenaDataScadenzaLotto(dataScad, w_LottoAccettazione);
            }

        }
    } else {
            w_LottoAccettazione = Get_KendoDDLValue("ddlLottoAccettazione");
    }  
        
    let w_PesoLordo = 0;
    let w_Tara = 0;
    let w_PesoNetto = 0;
    let w_qta = 0;
    let w_qta_extra = 0;
    let w_udm_cod_extra = 0;
    let w_um = 0;
    let pesoLordoRiscontrato = 0;
    let pesoNettoRiscontrato = 0;
    var w_extra_int = 0;

    if (elemCod !== RIGA_DESCRIZIONE_LIBERA) {

        w_um = parseInt(Get_KendoDDLValue("ddlUM"));

        let udmMultipliKg = [enum_Udm.quintali.value, enum_Udm.tonnellate.value];

        //Il controllo > 1 è come fatto nella funzione di lettura DocContabileDettagliUC.Udm_Optimize_Regolamento → CaricaListControl.Udm_Optimize_Regolamento
        if ((elemCod === FERTILIZZANTI && Get_KendoDDLValue("ddlPUARegolamento") !== "" && parseInt(Get_KendoDDLValue("ddlPUARegolamento")) > 1) ||
            (elemCod !== FERTILIZZANTI && udmMultipliKg.includes(w_um))) {
            w_extra_int = w_um;
        }

        if ((w_um === enum_Udm.chilogrammi.value || udmMultipliKg.includes(w_um)) && is_Trasf_Veg_Anim_FormProdottoUC()) {
            w_PesoLordo = Get_KendoNumTBValue("idKgLordi", true);
            w_PesoNetto = Get_KendoNumTBValue("idKgNetti", true); // Viene salvato in Qta_Extra_Totale E anche in Qta. In quest'ultima no se sono presenti imballi di tipo confezione
            pesoLordoRiscontrato = Get_KendoNumTBValue("idKgLordiRiscontrati", true);
            pesoNettoRiscontrato = Get_KendoNumTBValue("idKgNettiRiscontrati", true);
        } else {
            if (KendoDDL("ddlProdottoDes").dataItem() !== undefined &&
                KendoDDL("ddlProdottoDes").dataItem().Qta_Extra !== undefined) {
                w_qta_extra = parseFloat(KendoDDL("ddlProdottoDes").dataItem().Qta_Extra);
            } else {
                w_qta_extra = parseFloat($('input[name$="hf_Qta_Extra"]').val());
            }

            if (KendoDDL("ddlProdottoDes").dataItem() !== undefined &&
                KendoDDL("ddlProdottoDes").dataItem().Udm_Cod_Extra !== undefined) {
                w_udm_cod_extra = parseInt(KendoDDL("ddlProdottoDes").dataItem().Udm_Cod_Extra);
            } else {
                w_udm_cod_extra = parseInt($('input[name$="hf_Udm_Cod_Extra"]').val());
            }
            w_qta = Get_KendoNumTBValue("idQuantita", true);
            w_PesoNetto = w_qta * w_qta_extra; // Viene salvato in Qta_Extra_Totale, TODO verificare se è necessario ed utile per udm diverse da numero
            pesoNettoRiscontrato = Get_KendoNumTBValue("idQuantitaRiscontrata", true);
        }

        w_Tara = Get_KendoNumTBValue("idTara", true);
    }

    let w_modulo_anagrafe_log = moduloFromElemCod(elemCod);

    // Collegamento raccolte conferimenti attraverso scelta impianti con raccolta creata automaticamente
    let _pivaConferente = "";
    let _chkImpiantiIndefiniti = false;
    let _noteRaccolta = "";
    let _righeImpianti = [];

    if (isImputazioneImpiantiAttiva()) {
        _pivaConferente = GetPivaConferente();
        _chkImpiantiIndefiniti = getKendoSwitch("ChkImpiantiIndefiniti");
        _noteRaccolta = $('input[name$="txt_note_raccolta"]').val();

        let kgridImpianti = KendoGrid("tab_griglia_impianti");
        if (kgridImpianti !== undefined) {
            let righeImpiantiAll = kgridImpianti.dataSource.data();

            //devo passare solo le righe che hanno una qta <> 0
            let righeImpiantiValorizzate = righeImpiantiAll.filter(function (dataItem) { return dataItem.Qta !== 0; });

            _righeImpianti = righeImpiantiValorizzate;
        }
    }

    // Collegamento raccolte conferimenti attraverso scelta di raccolte pre-esistenti
    let arrRaccolteXConferimenti = [];
    let tipoAssociazione = 0;

    if (isRaccolteXConferimentiAttive()) {
        _pivaConferente = GetPivaConferente();

        if (raccolteXConferimenti_dsSelezionate.length > 0) {
            tipoAssociazione = 1;
        }

        for (let i = 0; i < raccolteXConferimenti_dsSelezionate.length; i++) {

            let detRaccolta = raccolteXConferimenti_dsSelezionate[i];

            if (detRaccolta.Cau_Mov_Carico !== "") {
                arrRaccolteXConferimenti.push({
                    Piva: detRaccolta.Piva,
                    Sa_Cod: detRaccolta.Sa_Cod_Carico,
                    Id_Agenda: detRaccolta.Id_Agenda,
                    Cau_Mov: detRaccolta.Cau_Mov_Carico,
                    Id_Mov: detRaccolta.Id_Mov_Carico,
                    Id_Mov_Det: detRaccolta.Id_Mov_Det_Carico
                });
            }
            else {
                arrRaccolteXConferimenti.push({
                    Piva: detRaccolta.Piva,
                    Sa_Cod: detRaccolta.Sa_Cod_Campagna,
                    Id_Agenda: detRaccolta.Id_Agenda,
                    Cau_Mov: detRaccolta.Cau_Mov_Campagna,
                    Id_Mov: detRaccolta.Id_Mov_Campagna,
                    Id_Mov_Det: detRaccolta.Id_Mov_Det_Campagna
                });
            }
        }
    }

    var objCampi = {
        ChiaveRiga: oldKeyDet,
        Piva: piva,
        SaCod: saCod,
        IdAgenda: idAgenda,
        IdMov: idMov,
        IdMovDet: idMovDet,
        //RifMovDettaglio: rifMovDettaglio,
        ListRifMovDettaglio: listRifMovDettaglio,

        DestinazioneScarico: { TipoDestinazione: tipoDestinazioneScarico, SaCod: saCodDestScarico, IdDestinazione: idDestinazioneScarico },
        DestinazioneCarico: { TipoDestinazione: tipoDestinazioneCarico, SaCod: saCodDestCarico, IdDestinazione: idDestinazioneCarico },

        idDataMovimento: get_data("inDataEmissione"),
        idOra: $('input[name$="idOra"]').val(),

        Note: $('input[name$="txtBoxNote"]').val(),
        txtGiacenzaProvenienza: $('input[name$="txtGiacenzaProvenienza"]').val(),
        txtGiacenzaDestinazione: $('input[name$="txtGiacenzaDestinazione"]').val(),
        //txtNrDenuncia: "'" + $('input[name$="txtNrDenuncia"]').val() + "'",
        //idDataDenuncia: get_data("idDataDenuncia"),
        
        ElemCod: elemCod,
        ProCod: proCod,
        MatCod: matCod,
        MatDes: KendoDDL("ddlProdottoDes").text(),

        N: kendo.parseFloat(Get_KendoNumTBValue("idTxt_N", true)),
        P2O5: kendo.parseFloat(Get_KendoNumTBValue("idTxt_P2O5", true)),
        K2O: kendo.parseFloat(Get_KendoNumTBValue("idTxt_K2O", true)),
        Cu: kendo.parseFloat(Get_KendoNumTBValue("idTxt_Cu", true)),
        PuaRegolamento: Get_KendoDDLValue("ddlPUARegolamento"),

        BeniStrumentali: $('input[name$="txtBeniStrumentali"]').val(),
        ddlLotto: Get_KendoDDLValue("ddlLottoImpianto"),
        chkAggregaLottoImpianto: getKendoSwitch("chkAggregaLottoImpianto"),
        Lotto: w_LottoAccettazione,  
        ddlCalibro: Get_KendoDDLValue("ddlCalibro"),
        chkParametroQualitativo: getKendoSwitch("chkParametroQualitativo"),  
        Extra_Str: w_extra_str,
        Extra_Date: w_extra_date,
        UdM: w_um,
        ExtraInt: w_extra_int,
        jolly_int: jolly_int,
        //TODO: NON c'è?
        //txtDoseEtichetta: Get_KendoNumTBValue("txtDoseEtichetta", true),

        //TODO: NON c'è?
        //ddlImballaggio: Get_KendoDDLValue("ddlImballaggio"),
        //idNrImballaggio: Get_KendoNumTBValue("idNrImballaggio"),
        //idTaraImballaggio: Get_KendoNumTBValue("idTaraImballaggio"),
        //Riscontrati_NumImballi: Get_KendoNumTBValue("idNrRiscontratiImballaggio"),

        //TODO: NON c'è?
        //ddlContenitore: Get_KendoDDLValue("ddlContenitore"),
        //idNrContenitore: Get_KendoNumTBValue("idNrContenitore"),
        //idContPerImb: Get_KendoNumTBValue("idContPerImb"),
        //idTaraContenitore: Get_KendoNumTBValue("idTaraContenitore"),
        //Riscontrati_NumContenitori: Get_KendoNumTBValue("idNrRiscontratiContenitore"),

        //TODO: NON c'è?
        //ddlConfezione: Get_KendoDDLValue("ddlConfezione"),
        //idNrConfezione: Get_KendoNumTBValue("idNrConfezione"),
        //idConfPerCont: Get_KendoNumTBValue("idConfPerCont"),
        //idTaraConfezione: Get_KendoNumTBValue("idTaraConfezione"),
        //Riscontrati_NumConfezioni: Get_KendoNumTBValue("idNrRiscontratiConfezione"),

        //Quantita: Get_KendoNumTBValue("idQuantita"),
        Prezzo: kendo.parseFloat(Get_KendoNumTBValue("idPrezzo", true)),

        //TODO: per il momento così, ma devo capire se lo devo rendere prendere sempre così, o solo quando è sensato (ho la seconda udm)
        PrezzoEffettivoKgL: kendo.parseFloat(Get_KendoNumTBValue("idPrezzo", true)),

        PrezzoNetto: kendo.parseFloat(Get_KendoNumTBValue("idPrezzoNetto", true)),
        ScontoModalita: parseInt(Get_KendoDDLValue("ddlScontoModalita")),
        ScontoBase: Get_KendoNumTBValue("idScontoBase", true),
        ScontoAddiz1: Get_KendoNumTBValue("idScontoAddiz1", true),
        ScontoAddiz2: Get_KendoNumTBValue("idScontoAddiz2", true),
        ScontoAddiz3: Get_KendoNumTBValue("idScontoAddiz3", true),

        //TODO: controlli mancanti:
        // - ddlScontoMagg (è usato? mi pare di no sul LAN, si era cominciato a farlo, ma non ha mai funzionato?!?)
        //ddlScontoMagg: Get_KendoDDLValue("ddlScontoMagg"), //implicherebbe il segno degli sconti?

        //idQuantitaRiscontrata: Get_KendoNumTBValue("idQuantitaRiscontrata", true),
        CalCod: w_cal_cod,
        Cod_Progetto: w_cod_progetto,
        NumConfezioni: wNrConfez_FF,
        NumContenitori: wNrCont_FF,
        NumImballi: wNrImb_FF,

        KgLordi: w_PesoLordo,
        Tara: w_Tara,
        KgNetti: w_PesoNetto,
        Degrado: Get_KendoNumTBValue("idDegradoPerc", true),
        Quantita: w_qta,
        Qta_Extra: w_qta_extra,
        Udm_Cod_Extra: w_udm_cod_extra,

        Riscontrati_PesoLordo: pesoLordoRiscontrato,
        //idTaraRiscontrata: Get_KendoNumTBValue("idTaraRiscontrata", true),
        Riscontrati_PesoNetto: pesoNettoRiscontrato,

        //TODO: NON c'è?
        PrezzoRiferitoA: parseInt(Get_KendoDDLValue("ddlPrezzoRiferitoA")),

        //TODO: NON c'è?
        //ddlListino: Get_KendoDDLValue("ddlListino"),

        // - cosa ci va a finire? se l'importo in euro dello sconto non serve, 
        //se ci va la percentuale risultante dai soli sconti addizionali a cascata sì, perché va in sconto_listino
        ScontoCalcolato: Get_KendoNumTBValue("idScontoCalcolato", true),

        ForzaIva: getKendoSwitch("chkForzaIva"),   
        CodIva: parseInt(Get_KendoDDLValue("ddlCodIva")),
        AliquotaIva: getAliquotaIva(true),
        Iva: Get_KendoNumTBValue("idIva", true),

        ImponibileTotale: Get_KendoNumTBValue("idImponibileTotale", true),
        ImponibileTotaleNetto: Get_KendoNumTBValue("idImponibileTotaleNetto", true),
        ImportoUnitario: Get_KendoNumTBValue("idImportoUnitario", true),
        ImportoTotale: Get_KendoNumTBValue("idImportoTotale", true),

        ValoreRiferimentoPrezzo: parseInt(Get_KendoDDLValue("ddlValoreRiferimento")),

        Anno: parseInt(Get_KendoDDLValue("ddlAnno")),
        ContoEconomico: parseInt(Get_KendoDDLValue("ddlContoEconomico")),
        ContoPatrimoniale: parseInt(Get_KendoDDLValue("ddlContoPatrimoniale")),

        ProvvigioneAgente: Get_KendoNumTBValue("idProvvigioni", true),
        //ProvvigioneCapoArea: Get_KendoNumTBValue("idProvvigioni", true),

        ModuloGias: w_modulo_anagrafe_log,

        Pendente: pendente,

        PivaConferente: _pivaConferente,
        ChkImpiantiIndefiniti: _chkImpiantiIndefiniti,
        NoteRaccolta: _noteRaccolta,
        RigheImpianti: _righeImpianti,

        Raccolte: arrRaccolteXConferimenti,
        Tipo_Associazione: tipoAssociazione,

        N_Nota_DDT: $("#notaDDTEsterna").val(),
        N_Nota_Riga_DDT: $("#notaRigaDDTEsterna").val(),
        Data_Nota_DDT: kendo.parseDate($("#notaDataDDTEsterna").val())

    };

    // In caso di ordini, prendo il documento di riferimento dalla testata, mentre in caso di lavCod specifici,
    // il documento di riferimento è specificabile a livello di riga con controlli appositi
    if (lavCodOrdine === true) {
        //queste info vanno ripetute anche su tutte le righe!
        objCampi.N_Doc_Cliente = $("#inNOrdineCliente").val();
        objCampi.Data_Doc_Cliente = kendo.parseDate($("#inDataOrdineCliente").val());
    }
    else {
        if (isVisibile_RifOrdine(cIdLavCod) === true) {
            objCampi.N_Doc_Cliente = $("#rigaRifNOrdine").val();
            objCampi.Data_Doc_Cliente = kendo.parseDate($("#rigaRifDataOrdine").val());
        }
    }

    if (riga_originale_entrata_FormProdottoUC !== null) {
        objCampi.OrdineDet = riga_originale_entrata_FormProdottoUC.Ordine_Det;
    } else {
        //è nuova riga
        objCampi.OrdineDet = maxOrdineDet + 1;
    }

    if (KendoDDL("ddlProdottoDes").dataItem() !== undefined) {
        objCampi.VegCod = KendoDDL("ddlProdottoDes").dataItem().Veg_Cod;
        objCampi.CulCod = KendoDDL("ddlProdottoDes").dataItem().Cul_Cod;
    } else {
        objCampi.VegCod = 0;
        objCampi.CulCod = 0;
    }

    let matCodAlias = parseInt(KendoDDL("ddlProdAlias").value());
    objCampi.MatCodAlias = isNaN(matCodAlias) ? 0 : matCodAlias;

    if ((elemCod === TRASFORMATI_VEGETALI || elemCod === TRASFORMATI_ANIMALI) &&
        KendoGrid("tab_imballaggi_formProdottoUC") !== undefined &&
        KendoGrid("tab_imballaggi_formProdottoUC").dataSource !== undefined) {

        objCampi.Confezionamenti = KendoGrid("tab_imballaggi_formProdottoUC").dataSource.data();
    }

    if (lavCodAccettazione === true) {
        //TODO: questo è per il pomodoro
        if (lavCodAccettazionePomodoro === true) {
            let objConfSpeciale = {
                FaseCodContratto: contrattoPomodoro.Fase_Cod,
                TagliandoPesa: $("#txtTagliandoPesa").val(),
                PremioComplessivo: riepilogoPomodoro!= null ? riepilogoPomodoro.Premio_Complessivo : 0,
                CodVarieta: Get_KendoDDLValue("ddlCodVarietaPomodoro"),
                DescAppezzamenti: $("#txtDescAppezzamenti").val()
            }
            objCampi.Conferimento_Speciale = objConfSpeciale;
        }
    }

    // Forza campi non gestiti dai movimenti di magazzino
    if (lavCodMovMagazzino === true) {

        //Iva
        objCampi.ForzaIva = 0;
        objCampi.CodIva = 0;
        objCampi.AliquotaIva = 0;
        objCampi.Iva = 0;

        //Conti
        objCampi.Anno = 0;
        objCampi.ContoEconomico = 0;
        objCampi.ContoPatrimoniale = 0;

        //Altro
        objCampi.OrdineDet = 0;

    }

    return objCampi;
}

function AnnullaModificheRigaDoc_click() {
    
    $('#confermaAnnullamentoFormProdottoUCDialog').kendoDialog({
        width: "400px",
        title: TraduzioneMultiResx(resxFormProdottoUC, "GestioneDocumenti", "Gestione documenti"),  //TODO
        closable: false,
        modal: true,
        visible: false,
        content: "<p>" + TraduzioneMultiResx(resxObj, "ConfermaAnnullamentoModifiche", "Confermi l'annullamento delle modifiche?") + "<p>",
        actions: [
            {
                text: TraduzioneMultiResx(resxFormProdottoUC, "Si", "Si"),
                action: function (e) {

                    SvuotaSegnalazioniErrori(false, false, true);
                    // Rendo visibile il TAB dell'elenco righe
                    ImpostaVisibilitaElencoOSingolaRiga(false);

                    raccolteXConferimenti_dsSelezionate = [];

                    //ri-abilito la modifica di alcuni elementi di testata, solo se non ho già altre righe
                    if ($("#tab_elenco_movimenti").data("kendoGrid").dataSource.data().length === 0) {
                        AbilitaModificaDatiMinimiTestata(true);

                        //se salvassi qui, verrebbe salvato un documento monco con solo la testata
                        VisualizzaPulsantiSalvataggio(false);

                    }

                    UscitaDaRigaDoc();
                }
            },
            {
                text: TraduzioneMultiResx(resxFormProdottoUC, "No", "No"),
                primary: true
            }
        ]
    }); 

    $('#confermaAnnullamentoFormProdottoUCDialog').data("kendoDialog").open();

}

function SalvaENuovaRigaDoc_click() {
    btnSalvaDoc_click(false, false, false);
    DocContabileNuovaRiga();
}

function ResetCampiChiaveFormProdotto() {
    $('input[name$="hdKendo_RigaDoc"]').val("");
    $('input[name$="hf_key_mov_dett"]').val("");
    $('input[name$="hf_Cal_Cod"]').val(0);
    $('input[name$="hf_Cod_Progetto"]').val(0);
    $('input[name$="hf_riga_DataOraUltimaLettura"]').val(null);
    $('input[name$="hf_Qta_Extra"]').val(0);
    $('input[name$="hf_Udm_Cod_Extra"]').val(0);
    //rifMovDettaglio = null;
    listRifMovDettaglio = [];
    riga_originale_entrata_FormProdottoUC = null;

    //visto che nella riga potrei avere reso bloccata o filtrata la ddlIva, devo resettare anche queste cose
    KendoDDL("ddlCodIva").enable(true);
    ResetFiltroIvaScontoMerce();
    Last_Iva_Default_Prodotto = null;

}

function EsciRigaDoc_click() {
    SvuotaSegnalazioniErrori(false, false, true);
    // Rendo visibile il TAB dell'elenco righe
    ImpostaVisibilitaElencoOSingolaRiga(false);

    UscitaDaRigaDoc();
}

function SalvaRigaDoc_click() {

    //TODO: devo sempre verificare anche la testata o solo all'inizio, quando ancora la devo scrivere?!?
    SvuotaSegnalazioniErrori(true, true, true);

    let isValidIntestazione = validatorIntestazione.validate();
    let isValidTabTestata = validatorTabTestata.validate();
    let isValidDettaglio = validatorTabDettaglio.validate();

    let msgErrore = "";

    //TODO: DEBUG GIULIA!
    //if (false === true) {
    if (isValidIntestazione === false || isValidTabTestata === false || isValidDettaglio === false) {
        $.logThis("Validator Dettaglio NO");
        EvidenziaErroriIntestazione();  //TODO: lo devo sempre fare?
        EvidenziaErroriTabTestata();    //TODO: lo devo sempre fare?
        EvidenziaErroriTabDettaglio();

        //Se ho usato il pulsante di salva in fondo, non capisco che ci sono errori ==> salto al primo riepilogo errori che trovo
        if (isValidIntestazione === false) {
            document.getElementById("erroriMsgIntestazione").scrollIntoView();
        } else {
            document.getElementById("tabs").scrollIntoView();   //mi sposto sulla tab dove si vedono i badge
        }
    }
    else {

        msgErrore = ControlliFormDettaglio();
        if (msgErrore === "") {
            msgErrore = ControlliProduttore();
        }

        if (msgErrore !== "") {
            $.logThis("Validator Dettaglio NO - Controlli Aggiuntivi ");
            MessaggioErrore_Bootstrap(msgErrore, "DIV_Messaggi");
            //DisabilitaSalvataggio();
        } else {
            $.logThis("Validator Dettaglio OK");
            let salvataggioOK = SalvaRigaDoc();
            if (salvataggioOK)
                UscitaDaRigaDoc();
        }
    }
}


// -----------------------------------------
// ---   Fine evento salvataggio riga   ----
// -----------------------------------------


// --------------------------
// --- Inizio Change DDL ----
// --------------------------


function idDegradoPerc_change(e) {
    calcolaDegrado_FormProdottoUC(Get_KendoNumTBValue("idKgNetti"), Get_KendoNumTBValue("idDegradoPerc"));
    AggiornaDettagliEconomici();
}

function idPrezzo_change() {
    AggiornaDettagliEconomici();
}

function idScontoBase_change() {
    AggiornaDettagliEconomici();
}

function idScontoAddiz1_change() {
    AggiornaDettagliEconomici();
}

function idScontoAddiz2_change() {
    AggiornaDettagliEconomici();
}

function idScontoAddiz3_change() {
    AggiornaDettagliEconomici();
}

function idIva_change() {
    AggiornaDettagliEconomici();
}

function idImportoUnitario_change() {
    AggiornaDettagliEconomici();
}

function idImportoTotale_change() {
    AggiornaDettagliEconomici();
}

function idImponibileTotale_change() {
    AggiornaDettagliEconomici();
}

function idImponibileTotaleNetto_change() {
    AggiornaDettagliEconomici();
}

function ddlValoreRiferimento_change(e) {
    ddlValoreRiferimento_CambioValoreVisibilita(true);
    AggiornaDettagliEconomici();
}

function ddlValoreRiferimento_CambioValoreVisibilita(flagSetDefault) {

    //Bloccare campi che vanno ricalcolati in automatico
    var valoreRiferimento = Get_KendoDDLValue("ddlValoreRiferimento", "0");

    switch (valoreRiferimento) {
        case "0": // "Prezzo unitario"
            KendoNumTB("idPrezzo").enable(true);
            KendoNumTB("idImportoUnitario").enable(false);
            KendoNumTB("idImportoTotale").enable(false);
            KendoNumTB("idImponibileTotale").enable(false);
            break;

        case "1": // "Imponibile totale"
            KendoNumTB("idPrezzo").enable(false);
            KendoNumTB("idImportoUnitario").enable(false);
            KendoNumTB("idImportoTotale").enable(false);
            KendoNumTB("idImponibileTotale").enable(true);
            break;

        case "2": // "Importo totale"
            KendoNumTB("idPrezzo").enable(false);
            KendoNumTB("idImportoUnitario").enable(false);
            KendoNumTB("idImportoTotale").enable(true);
            KendoNumTB("idImponibileTotale").enable(false);
            break;

        case "3": // "Importo unitario"
            KendoNumTB("idPrezzo").enable(false);
            KendoNumTB("idImportoUnitario").enable(true);
            KendoNumTB("idImportoTotale").enable(false);
            KendoNumTB("idImponibileTotale").enable(false);
            break;
    }
}

function ddlPrezzoRiferitoA_change(e) {
    AggiornaDettagliEconomici();
}

function ddlScontoModalita_change(e) {

    //Marco: modifica per reimpostare il prezzo riferito <> prezzo unitario
    let valModSconto = parseInt(Get_KendoDDLValue("ddlScontoModalita", 0));

    if (valModSconto > 0) {
        Set_KendoDDLValueNoDef("ddlValoreRiferimento", 0);
        ddlValoreRiferimento_change();
        KendoDDL("ddlValoreRiferimento").enable(false);
    }
    else {
        KendoDDL("ddlValoreRiferimento").enable(true);
    }


    ddlScontoModalita_CambioValoreVisibilita(true);
    AggiornaDettagliEconomici();
}

function ddlScontoModalita_CambioValoreVisibilita(flagSetDefault) {

    let valModSconto = parseInt(Get_KendoDDLValue("ddlScontoModalita", 0));
    let aliquotaIva = getAliquotaIva(true);

    switch (valModSconto) {

        case 0: // "Sconto % su prezzo unitario"

            if (flagSetDefault) {
                Set_KendoNumTBValue("idScontoBase", 0);
            }

            KendoNumTB("idScontoBase").enable(true);
            KendoNumTB("idScontoAddiz1").enable(true);
            KendoNumTB("idScontoAddiz2").enable(true);
            KendoNumTB("idScontoAddiz3").enable(true);

            //Se prima avevo scelto qualcosa che mi restringeva l'elenco ive, ora devo rimetterle tutte
            KendoDDL("ddlCodIva").enable(true);
            ResetFiltroIvaScontoMerce();

            if (flagSetDefault) {
                //TODO: devo risettare l'iva di default del prodotto
                if (Last_Iva_Default_Prodotto !== null && Last_Iva_Default_Prodotto !== undefined && aliquotaIva === 0) {
                    Set_KendoDDLValue("ddlCodIva", Last_Iva_Default_Prodotto);
                    ddlCodIva_change();
                }
            }

            break;

        case 1: // "Sconto Merce"
            
            if (flagSetDefault) {
                Set_KendoNumTBValue("idScontoBase", 100);
                Set_KendoNumTBValue("idScontoAddiz1", 0);
                Set_KendoNumTBValue("idScontoAddiz2", 0);
                Set_KendoNumTBValue("idScontoAddiz3", 0);
            }

            KendoNumTB("idScontoBase").enable(false);
            KendoNumTB("idScontoAddiz1").enable(false);
            KendoNumTB("idScontoAddiz2").enable(false);
            KendoNumTB("idScontoAddiz3").enable(false);

            KendoDDL("ddlCodIva").enable(true);
            //solo 3 aliquote iva possibili
            SetFiltroIvaScontoMerce();

            if (flagSetDefault) {
                Set_KendoDDLValue("ddlCodIva", 61);
            }

            break;

        case 3: // "Campioni gratuiti"

            if (flagSetDefault) {
                Set_KendoNumTBValue("idScontoBase", 100);
                Set_KendoNumTBValue("idScontoAddiz1", 0);
                Set_KendoNumTBValue("idScontoAddiz2", 0);
                Set_KendoNumTBValue("idScontoAddiz3", 0);
            }

            KendoNumTB("idScontoBase").enable(false);
            KendoNumTB("idScontoAddiz1").enable(false);
            KendoNumTB("idScontoAddiz2").enable(false);
            KendoNumTB("idScontoAddiz3").enable(false);

            ResetFiltroIvaScontoMerce();

            //c'è un'unica aliquota utilizzabile, quindi la setto e disabilito il menù a tendina, così non devo gestire un filtro apposito
            if (flagSetDefault) {
                Set_KendoDDLValue("ddlCodIva", 79);
            }
            KendoDDL("ddlCodIva").enable(false);
            
            break;

        case 2: // "Campioni omaggio senza rivalsa Iva"
        case 4: // "Campioni omaggio con rivalsa Iva"

            if (flagSetDefault) {
                Set_KendoNumTBValue("idScontoBase", 100);
                Set_KendoNumTBValue("idScontoAddiz1", 0);
                Set_KendoNumTBValue("idScontoAddiz2", 0);
                Set_KendoNumTBValue("idScontoAddiz3", 0);
            }

            KendoNumTB("idScontoBase").enable(false);
            KendoNumTB("idScontoAddiz1").enable(false);
            KendoNumTB("idScontoAddiz2").enable(false);
            KendoNumTB("idScontoAddiz3").enable(false);

            KendoDDL("ddlCodIva").enable(true);
            ResetFiltroIvaScontoMerce();

            if (flagSetDefault) {
                //TODO: devo risettare l'iva di default
                if (Last_Iva_Default_Prodotto !== null && Last_Iva_Default_Prodotto !== undefined && aliquotaIva === 0) {
                    Set_KendoDDLValue("ddlCodIva", Last_Iva_Default_Prodotto);
                    ddlCodIva_change();
                }
            }

            break;

    }
}

function SetFiltroIvaScontoMerce() {
    let ddlIva = KendoDDL("ddlCodIva");

    ddlIva.setDataSource(elencoIVA_Aliquote_ScontoMerce);

    //TODO: devo anche rileggere???
    //ddlIva.dataSource.read();
    //ddlIva.refresh();

    //let filterIva = {
    //    agroIdentity: "FILTRO_SCONTO_MERCE",
    //    logic: "or",
    //    filters: [
    //        { field: "Cod_IVA", operator: "eq", value: 61 },
    //        { field: "Cod_IVA", operator: "eq", value: 67 },
    //        { field: "Cod_IVA", operator: "eq", value: 78 }
    //    ]
    //};

    ////Filter the source manually
    //let oldFilter = ddlIva.dataSource.filter();
    //let newFilter = undefined;
    //if (oldFilter === undefined) {
    //    newFilter = filterIva;
    //}

    //ddlIva.dataSource.filter(newFilter);

    ////!--IMPORTANT: Update filter state of the widget-- >
    //ddlIva.listView.setDSFilter(ddlIva.dataSource.filter());

}

function ResetFiltroIvaScontoMerce() {
    //TODO: il filtro iva va resettato anche all'uscita dalla singola riga!!!
    let ddlIva = KendoDDL("ddlCodIva");

    //ri-setto la funzione iniziale, solo se il dataSource non è = a RiempiElencoIVA_Aliquote
    //se avevo switchato sull'elenco ristretto per lo sconto merce, allora sarà un array
    let dsRead = ddlIva.dataSource.transport.read;
    if (dsRead !== RiempiElencoIVA_Aliquote) {
        let newDs = { transport: { read: RiempiElencoIVA_Aliquote } };
        ddlIva.setDataSource(newDs);

        //TODO: devo anche rileggere???
        //ddlIva.dataSource.read();
        //ddlIva.refresh();
    }



//    if (ddlIva !== null && ddlIva !== undefined &&
//        ddlIva.dataSource !== null && ddlIva.dataSource !== undefined) {
//        var filtro = ddlIva.dataSource.filter();

//        if (filtro !== null && filtro !== undefined) {

//            //resetto anche l'input?!?
//            ddlIva.text("");

//            if (filtro.agroIdentity === "FILTRO_SCONTO_MERCE") {
//                //è l'unico filtro che ho, quindi devo direttamente azzerare l'intero filtro
//                ddlIva.dataSource.filter({});
//                ddlIva.listView.setDSFilter(ddlIva.dataSource.filter());

//            } else if (filtro.filters !== null && filtro.filters !== undefined &&
//                filtro.filters.length > 0) {
                
//                //Avevo un filtro, quindi devo capire se è quello dello sconto merce da eliminare
//                filtro.filters = filtro.filters.filter(function(x) { return x.agroIdentity !== "FILTRO_SCONTO_MERCE"; });

//                ddlIva.dataSource.filter(filtro);
//                ddlIva.listView.setDSFilter(ddlIva.dataSource.filter());
//            }
//        }
//    }
}

function CampiEconomiciAbilitati(flagSetDefault) {
    // Quando riapro una riga e/o cambio alcuni valori devo abilitare/disabilitare o gestire la visibilità di altri campi
    ddlValoreRiferimento_CambioValoreVisibilita(flagSetDefault);
    ddlScontoModalita_CambioValoreVisibilita(flagSetDefault);
}

function ddlScontoMaggiorazione_change(e) {
    // TODO Ricalcolo importo
    var ddlScontoMaggiorazione = KendoDDL("ddlScontoMagg");
    if (ddlScontoMaggiorazione.value() === "Sconto") {

    } else if (ddlScontoMaggiorazione.value() === "Maggiorazione") {

    }
    AggiornaDettagliEconomici();
}

function ddlCodIva_change(e) {

    let aliquota = getAliquotaIva(true);

    //TODO: per il calcolo dell'IVA si deve usare l'imponibile netto che ancora non esiste come campo!!!
    let imponibile = kendo.parseFloat(Get_KendoNumTBValue("idImponibileTotaleNetto", true));

    Set_KendoNumTBValue("idIva", kendo.parseFloat(imponibile * aliquota / 100));
    
    AggiornaDettagliEconomici();
}

function chkForzaIva_change(e) {
    if (getKendoSwitch("chkForzaIva")) {
        KendoNumTB("idIva").enable(true);   //campo idIva modificabile
    } else {
        KendoNumTB("idIva").enable(false);  //campo idIva non modificabile e ricalcola Iva
        AggiornaDettagliEconomici();
    }
}

function ddlCategorieMagazzino_change(e) {

    // Aggiorna dataSource magazzini
    ImpostaDataSourceDdlUbic("ddlUbicProvenienza", null);
    ImpostaDataSourceDdlUbic("ddlUbicDestinazione", null);

    let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

    if (KendoDDL("ddlCategorieMagazzino").dataSource._data.length === 0) {

        // TODO ??????

    } else {

        AzzeraAlCambioCategoriaOProdotto("C");

        impostaVisibilitaBtnNuovoProdotto(ddlCategorieMagazzinoValue);

        if (isNaN(ddlCategorieMagazzinoValue) === false && ddlCategorieMagazzinoValue !== 0) {

            $("#lblParametroQualitativo").hide();
            setKendoSwitchVisible("chkParametroQualitativo", false);

            ImpostaVisibilita_PerCategoria(Qs_CaricoScarico, ddlCategorieMagazzinoValue);

            Visibilita_PUARegolamento(false, false);

            //La imposto qui e poi la nascondo dove non si deve vedere
            if (FF_gest_materiale_vivaistico || lavCodAccettazionePomodoro)
                Visibilita_UnitaMisura(false, false);
            else
                Visibilita_UnitaMisura(true, true);

            //visibilità sezione Parametri GHG
            Visibilita_ParametriIndici(ddlCategorieMagazzinoValue)

            //Tutte le varie impostazioni di visibilità dei campi spostate dentro a ImpostaVisibilita_PerCategoria, 
            //visto che viene già richiamata qui sopra e molto codice era duplicato (viene lasciato qui il reset dei
            //vari campi)

            switch (ddlCategorieMagazzinoValue) {

                case RIGA_DESCRIZIONE_LIBERA:

                    ClearDDL("ddlUM");
                    Visibilita_UnitaMisura(false, false);

                    Set_KendoDDLValueNoDef("ddlUbicProvenienza", "");
                    Set_KendoDDLValueNoDef("ddlUbicDestinazione", "");
                    KendoDDL("ddlProdottoDes").enable(false);
                    KendoDDL("ddlProdAlias").enable(false);

                    break;

                case ALTRI_BENI:

                    Set_KendoDDLValueNoDef("ddlUbicProvenienza", "");
                    Set_KendoDDLValueNoDef("ddlUbicDestinazione", "");
                    KendoDDL("ddlProdottoDes").enable(false);
                    KendoDDL("ddlProdAlias").enable(false);

                    RicercaUdm_Optimize("", false, false, 0, 0, CAU_CARICO, ddlCategorieMagazzinoValue, 0, false, false, false);
                    KendoDDL("ddlUM").dataSource.data(elencoUdmOptimized);
                    assegnaDefaultDdlUMFormProdotto();

                    break;

                case SERVIZI:

                    Set_KendoDDLValueNoDef("ddlUbicProvenienza", "");
                    Set_KendoDDLValueNoDef("ddlUbicDestinazione", "");
                    KendoDDL("ddlProdottoDes").enable(true);
                    KendoDDL("ddlProdAlias").enable(false);

                    RicercaUdm_Optimize("", false, false, 0, 0, CAU_CARICO, ddlCategorieMagazzinoValue, 0, false, false, false);
                    KendoDDL("ddlUM").dataSource.data(elencoUdmOptimized);
                    assegnaDefaultDdlUMFormProdotto();

                    break;

                // TUTTO IL RESTO
                default:

                    // Imposto le unità di misura possibili per la categoria corrente; poi all'interno del singolo prodotto si va a ridefinirle
                    RicercaUdm_Optimize("", false, false, 0, 0, CAU_CARICO, ddlCategorieMagazzinoValue, 0, false, false, false);
                    KendoDDL("ddlUM").dataSource.data(elencoUdmOptimized);
                    assegnaDefaultDdlUMFormProdotto();

                    KendoDDL("ddlProdottoDes").enable(true);
                    KendoDDL("ddlProdAlias").enable(true);

                    // TODO
                    //ATTENZIONE!!!!!!!
                    //Il prodotto è materiale --> verifico se il dettaglio sia stato già movimentato da bolla precedente

                    switch (ddlCategorieMagazzinoValue) {

                        case SEMENTI: case ALTRE_MATERIE:
                        case SEMILAVORATI_VEGETALI: case MATERIE_VEGETALI: case BENI_CONFEZ_VEGETALE: case TRASFORMATI_VEGETALI:
                        case SEMILAVORATI_ANIMALI: case MATERIE_ANIMALI: case BENI_CONFEZ_ANIMALE: case TRASFORMATI_ANIMALI:

                            if (ddlCategorieMagazzinoValue === SEMILAVORATI_VEGETALI) {
                                // TODO
                                //SEMILAVORATI_VEGETALI
                                //Me.Txt_CercaLotto.Visible = True
                                //Me.lbl_CercaLotto.Visible = True
                                $(".lblParametroQualitativo").show();
                                setKendoSwitchVisible("chkParametroQualitativo", true);
                            }
                            if (ddlCategorieMagazzinoValue === SEMILAVORATI_ANIMALI) {
                                $(".lblParametroQualitativo").show();
                                setKendoSwitchVisible("chkParametroQualitativo", true);
                            }
                            if (is_Trasf_Veg_Anim_FormProdottoUC()) {
                                var gridId = "tab_imballaggi_formProdottoUC";
                                var grid = $("#" + gridId).data("kendoGrid");
                                if (grid !== undefined) {
                                    kendo_AggiustaDimensioneColonne("#" + gridId);
                                }
                            }
                            break;

                        case FORMULATI:

                            $("#BtnInfo_Fito").show();

                            if (gestioneRegolamentoFormulati === true) {
                                Se_Abilita_PUARegolamento(ddlCategorieMagazzinoValue);
                            }

                            break;

                        case FERTILIZZANTI:

                            $("#BtnInfo_Concime").show();

                            Se_Abilita_PUARegolamento(ddlCategorieMagazzinoValue);

                            break;

                    }

            }

            CostruisciLinkInfoProdotto(ddlCategorieMagazzinoValue);

        } else {

            // Metto Hide tutto ciò che non si deve vedere quando non ho ancora scelto una categoria
            Visibilita_Ferti_Dettagli(false, false);

        }

        if (isNaN(ddlCategorieMagazzinoValue) === false && ddlCategorieMagazzinoValue !== 0) {

            let switchParametroQualitativo = $("#chkParametroQualitativo").kendoSwitch().data("kendoSwitch");
            setKendoSwitch("chkParametroQualitativo", false);
            
            if (ddlCategorieMagazzinoValue === SEMILAVORATI_VEGETALI && Qs_CaricoScarico === CAU_SCARICO) {
            
                // TODO dopo aver capito a cosa servono questi campi
                //Chk_LottoImpianto.Checked = True
                //Chk_LottoImpianto.Enabled = True
                setKendoSwitch("chkParametroQualitativo", true);
                switchParametroQualitativo.enable(true);

                $("#groupLottoImpianto").hide();
                $("#groupAggregaLottoImpianto").hide();
                ClearDDL("ddlLottoImpianto");
                Set_KendoDDLValueNoDef("ddlLottoImpianto", "");  //TODO Valore dft corretto?
                //KendoDDL("ddlLottoImpianto").wrapper.hide(); 

                $("#lblAggregaLottoImpianto").hide();
                setKendoSwitchVisible("chkAggregaLottoImpianto", false);

                $("#idParametroQualitativoCalibro").hide();
                $("#lblCalibro").hide();
                Set_KendoDDLValueNoDef("ddlCalibro", 0);  //TODO Valore dft corretto?
                KendoDDL("ddlCalibro").wrapper.hide(); 

            } else if ((ddlCategorieMagazzinoValue === RIGA_DESCRIZIONE_LIBERA ||  
                        ddlCategorieMagazzinoValue === TRASFORMATI_VEGETALI || 
                        ddlCategorieMagazzinoValue === ALTRI_BENI || 
                        ddlCategorieMagazzinoValue === BENI_CONFEZ_VEGETALE || 
                        ddlCategorieMagazzinoValue === FORMULATI || 
                        ddlCategorieMagazzinoValue === FERTILIZZANTI) 
                //&& Qs_CaricoScarico === CAU_SCARICO
            ) {

                switchParametroQualitativo.enable(false);

                // TODO dopo aver capito a cosa servono questi campi
                //Chk_LottoImpianto.Checked = False
                //Chk_LottoImpianto.Enabled = False
                
                $("#groupLottoImpianto").hide();
                $("#groupAggregaLottoImpianto").hide();
                ClearDDL("ddlLottoImpianto");
                Set_KendoDDLValueNoDef("ddlLottoImpianto", "");  //TODO Valore dft corretto?
                //KendoDDL("ddlLottoImpianto").wrapper.hide(); 

                $("#lblAggregaLottoImpianto").hide();
                setKendoSwitchVisible("chkAggregaLottoImpianto", false);

                $("#idParametroQualitativoCalibro").hide();
                $("#lblCalibro").hide();
                Set_KendoDDLValueNoDef("ddlCalibro", 0);  //TODO Valore dft corretto?
                //KendoDDL("ddlCalibro").wrapper.hide(); 
                
            } else {

                //TODO: invertire questi due blocchi una volta che si è capito su quali categorie vanno mostrati

                // TODO dopo aver capito a cosa servono questi campi
                //Chk_LottoImpianto.Checked = False
                //Chk_LottoImpianto.Enabled = False
                setKendoSwitch("chkParametroQualitativo", false);
                //getKendoSwitch("chkParametroQualitativo").enable(false);
                let switchParametroQualitativo = $("#chkParametroQualitativo").kendoSwitch().data("kendoSwitch");
                switchParametroQualitativo.enable(false);

                //$("#idLottoImpianto").show();
                //$("#lblLottoImpianto").show();
                //ClearDDL("ddlLottoImpianto");
                //Set_KendoDDLValueNoDef("ddlLottoImpianto", "");  //TODO Valore dft corretto?
                //KendoDDL("ddlLottoImpianto").wrapper.show();

                //$("#lblAggregaLottoImpianto").show();
                //setKendoSwitchVisible("chkAggregaLottoImpianto", false);   //TODO

                //$("#idParametroQualitativoCalibro").show();
                //$("#lblCalibro").show();
                //Set_KendoDDLValueNoDef("ddlCalibro", 0);  //TODO Valore dft corretto?
                //KendoDDL("ddlCalibro").wrapper.show();
                
            }

        }

    }

    impostaPanelBarContabilita();

}

function Se_RicaricaPUA_Regolamenti(ddlCategorieMagazzinoValue) {

    //Ricarico elenco solo se è cambiata la categoria con cui l'ho caricato in precedenza
    if (ddlCategorieMagazzinoValue !== elencoPUA_Regolamenti_ElemCod) {

        RicercaPUA_Regolamenti(false, Qs_PuaRegolamento, ddlCategorieMagazzinoValue);

        //Scateno la lettura del dataSource
        KendoDDL("ddlPUARegolamento").dataSource.read();

    }

}

function ddlLottoImpianto_change(e) {
    
    // TODO Vedi FormProdotto OLD
}

function ddlPUARegolamento_change(e) {

    // In entrata se cambia il regolamento azzero il prodotto in modo che venga ricaricato
    if (Qs_CaricoScarico === CAU_CARICO)
        CreaDdlProdottoDes();

    TrattaPuaRegolamento();
}

function TrattaPuaRegolamento() {

    let puaRegolamentoCod = 0;
    let tipoPuaRegolamento = 0;

    Set_KendoNumTBValue("idTxt_N", 0);
    Set_KendoNumTBValue("idTxt_P2O5", 0);
    Set_KendoNumTBValue("idTxt_K2O", 0);
    Set_KendoNumTBValue("idTxt_Cu", 0);

    if (Qs_CaricoScarico === CAU_CARICO && Get_KendoDDLValue("ddlPUARegolamento") !== "") {
        Visibilita_Ferti_Dettagli(false, false);

        let ddlPuaReg = KendoDDL("ddlPUARegolamento");

        if (ddlPuaReg.value() !== "" && parseInt(ddlPuaReg.value()) !== 0) {
            puaRegolamentoCod = parseInt(ddlPuaReg.value());
            tipoPuaRegolamento = parseInt(ddlPuaReg.dataItem().Regolamento_Tipo);
        }

        if (puaRegolamentoCod !== 0) {
            if (tipoPuaRegolamento === enum_PUARegolamenti_Tipo_PUA) {
                KendoNumTB("idTxt_N").enable(true);
                KendoNumTB("idTxt_P2O5").enable(true);
                KendoNumTB("idTxt_K2O").enable(true);
                KendoNumTB("idTxt_Cu").enable(true);
                if (Get_KendoDDLValue("ddlProdottoDes") !== "" &&
                    KendoDDL("ddlProdottoDes").dataItem() !== undefined) {
                    Set_KendoNumTBValue("idTxt_N", KendoDDL("ddlProdottoDes").dataItem().N);
                    Set_KendoNumTBValue("idTxt_P2O5", KendoDDL("ddlProdottoDes").dataItem().P2O5);
                    Set_KendoNumTBValue("idTxt_K2O", KendoDDL("ddlProdottoDes").dataItem().K2O);
                    Set_KendoNumTBValue("idTxt_Cu", KendoDDL("ddlProdottoDes").dataItem().Cu);
                }

                Visibilita_Ferti_Dettagli(true, false);

                if (Get_KendoDDLValue("ddlProdottoDes") !== "") {
                    let w_udm = Recupera_UdmFertilizzante(parseInt(Get_KendoDDLValue("ddlProdottoDes")), puaRegolamentoCod);
                    impostaDftValueDdlUM(w_udm);

                    let puaRegolamentiUdmObbligata = [78, 125]; // Regolamenti regione Umbria

                    if (w_udm !== null && puaRegolamentiUdmObbligata.includes(puaRegolamentoCod)) {
                        KendoDDL("ddlUM").enable(false);
                    }
                    else {
                        KendoDDL("ddlUM").enable(true);
                    }
                }
            }
            else {
                KendoDDL("ddlUM").enable(true);
            }
        }
        else {
            KendoDDL("ddlUM").enable(true);
        }
    } 

}

function ddlLottoAccettazione_change(e) {
    var ddlCategorieMagazzinoValue = parseInt(KendoDDL("ddlCategorieMagazzino").value());
    var ddlLottoAccettazione = KendoDDL("ddlLottoAccettazione");
    var w_old_udm = Get_KendoDDLValue("ddlUM");

    let Cau_Mov = "";
    if (Qs_CaricoScarico === CAU_SCARICO || Qs_CaricoScarico === CAU_TRASFERIMENTO)
        Cau_Mov = CAU_SCARICO;
    else
        Cau_Mov = CAU_CARICO;

    if (ddlLottoAccettazione.value() !== undefined && ddlLottoAccettazione.value() !== "") {
        $("#idParametroQualitativoCalibro").hide();
        $("#lblCalibro").hide();
        ClearDDL("ddlCalibro");
        KendoDDL("ddlCalibro").wrapper.hide();

        AzzeraGiacenza();
        //AzzeraQuantita();
        //AzzeraPrezziScontiImporti();

        if (ddlCategorieMagazzinoValue === SEMILAVORATI_VEGETALI ||
            ddlCategorieMagazzinoValue === TRASFORMATI_VEGETALI) {
            KendoDDL("ddlCalibro").enable(true);
        }

        switch (ddlCategorieMagazzinoValue) {

            case SEMILAVORATI_VEGETALI:
            case TRASFORMATI_VEGETALI:

                if (getKendoSwitch("chkParametroQualitativo")) {
                    ClearDDL("ddlCalibro");
                    let dataSourceCalibro = new kendo.data.DataSource({
                        data: [{
                            Cal_Des: "",
                            Cal_Cod: 0
                        }]
                    });
                    $("#ddlCalibro").data("kendoDropDownList").setDataSource(dataSourceCalibro);
                    Set_KendoDDLValueNoDef("ddlCalibro", 0);
                    KendoDDL("ddlCalibro").enable(true);
                    KendoDDL("ddlCalibro").wrapper.show();
                    $("#ddlCalibro").trigger('change');

                } else {
                    // TODO Caricare calibro   VEDO Cambio_LottoAccettazione 
                    if (ddlCategorieMagazzinoValue === SEMILAVORATI_VEGETALI) {
                        //KendoDDL("ddlCalibro").enable(true);
                    }

                    if (ddlCategorieMagazzinoValue === TRASFORMATI_VEGETALI) {
                        //KendoDDL("ddlCalibro").enable(true);
                    }
                }

                // TODO , Flag_QtaNoZero, Flag_QtaMaggioreZero
                RicercaUdm_Optimize_Lotto($(cIdPiva).val(), false, true, xSa_Cod, xFabbricato_Cod, Cau_Mov, ddlCategorieMagazzinoValue, Get_KendoDDLValue("ddlProdottoDes"), Get_KendoDDLValue("ddlLottoAccettazione"), is_Trasf_Veg_Anim_FormProdottoUC(), false, false);
                KendoDDL("ddlUM").dataSource.data(elencoUdmOptimized);
                // Qui non serve perchè poi riassegno il default subito dopo per non fare perdere la scelta dell'utente   
                // assegnaDefaultDdlUMFormProdotto();

                // setto con il valore selezionato in precedenza
                impostaDftValueDdlUM(w_old_udm);  

                break;


            default:

                // TODO , Flag_QtaNoZero, Flag_QtaMaggioreZero
                RicercaUdm_Optimize_Lotto($(cIdPiva).val(), false, true, xSa_Cod, xFabbricato_Cod, Cau_Mov, ddlCategorieMagazzinoValue, Get_KendoDDLValue("ddlProdottoDes"), Get_KendoDDLValue("ddlLottoAccettazione"), is_Trasf_Veg_Anim_FormProdottoUC(), false, false);
                KendoDDL("ddlUM").dataSource.data(elencoUdmOptimized);
                // Qui non serve perchè poi riassegno il default subito dopo per non fare perdere la scelta dell'utente   
                // assegnaDefaultDdlUMFormProdotto();

                // setto con il valore selezionato in precedenza
                impostaDftValueDdlUM(w_old_udm);  

                break;

        }
    }

    // TODO  Togliere parte commentata se va bene il resto del giro
    ////// Per scarichi F&F viene fatto a comando con un pulsante  
    //////if (!is_Trasf_Veg_Anim_FormProdottoUC() ||
    //////   (cIdLavCod !== enum_LavCod.Ordine_Vendita_Emesso.value &&
    //////        cIdLavCod !== enum_LavCod.DDT_Emesso.value &&
    //////        cIdLavCod !== enum_LavCod.Fattura_Emessa.value)) {
        CaricaGiacenze(null);
    //////}
         
}

function txtLottoAccettazione_focus(e) {
    old_Lotto_FormProdottoUC = $("#txtLottoAccettazione").val();
}

function txtLottoAccettazione_change(e) {
    if (old_Lotto_FormProdottoUC !== "" &&
        $("#txtLottoAccettazione").val() === ""
        // se si vuole fare solo in modifica && $('input[name$="hf_key_mov_dett"]').val() === ""
    ) {

        $('#confermaCambioLottoAccettazioneUCDialog').kendoDialog({
            width: "400px",
            title: TraduzioneMultiResx(resxFormProdottoUC, "GestioneDocumenti", "Gestione documenti"),  //TODO
            closable: false,
            modal: true,
            visible: false,
            content: "<p>" + TraduzioneMultiResx(resxFormProdottoUC, "ConfermaRicalcoloLottoDaConfigurazione",
                "Il lotto sarà ricalcolato al momento del salvataggio della riga in base ai criteri di configurazione; confermi?") + "<p>",
            actions: [
                {
                    text: TraduzioneMultiResx(resxFormProdottoUC, "Si", "Si"),
                    action: function (e) {

                        // Nulla da fare
                    }
                },
                {
                    text: TraduzioneMultiResx(resxFormProdottoUC, "No", "No"),
                    primary: true,
                    action: function (e) {

                        $("#txtLottoAccettazione").val(old_Lotto_FormProdottoUC);
                    }
                }
            ]
        });

        $('#confermaCambioLottoAccettazioneUCDialog').data("kendoDialog").open();

    }
}

function ddlCalibro_change(e) {
    // TODO Vedi FormProdotto OLD
}

function ddlUbicProvenienza_change(e) {

    if (Qs_CaricoScarico !== CAU_CARICO) {

        AzzeraGiacenza();
        //AzzeraQuantita();
        //AzzeraPrezziScontiImporti();

        // non pulisco la chiave nel caso di ddt e fattura emessa e nota accredito ricevuta
        if (cIdLavCod !== enum_LavCod.DDT_Emesso.value && cIdLavCod !== enum_LavCod.Fattura_Emessa.value && cIdLavCod !== enum_LavCod.Nota_Accredito_Ricevuta.value) {
            $("#ddlCategorieMagazzino").trigger("change");
        }

        if (KendoDDL("ddlLottoAccettazione") !== undefined) {
            ImpostaDdlLottoAccettazione([], null);
        }

        if (KendoDDL("ddlUbicProvenienza").value() !== "") {

            KendoDDL("ddlProdottoDes").enable(true);
            KendoDDL("ddlProdAlias").enable(true);

            CaricaGiacenze(null);

            SeImpostaFiltroTuttiProdotti(Evento_MagazzinoScarico_Change, null);

        } else {

            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxFormProdottoUC, "SelezionareIlMagazzinoDiProvenienza", "Selezionare il magazzino di provenienza!"), "DIV_Messaggi");
            // Nascondo la griglia delle giacenze e pulisco i dati perché andranno comunque ricaricati
            $("#tab_grid_scelta_da_giacenza_formProdottoUC").hide();
            $("#titolo_grid_scelta_da_giacenza").hide();
            $("#tab_grid_scelta_da_giacenza_formProdottoUC").html("");
            //DisabilitaSalvataggioErrore();
        }
       
    } else {

        if (KendoDDL("ddlUbicProvenienza").value() === "") {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxFormProdottoUC, "SelezionareIlMagazzinoDiProvenienza", "Selezionare il magazzino di provenienza!"), "DIV_Messaggi");
            //DisabilitaSalvataggioErrore();
        }

    }

}
 
function ddlUbicDestinazione_change(e) {

    if (Qs_CaricoScarico !== CAU_SCARICO) {
   
        AzzeraGiacenza();
        //AzzeraQuantita();
        //AzzeraPrezziScontiImporti();

        if (Get_KendoDDLValue("ddlUbicDestinazione") !== "") {
            KendoDDL("ddlProdottoDes").enable(true);
            KendoDDL("ddlProdAlias").enable(true);

            //CaricaGiacenze(null); 
 
        }
        else {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxFormProdottoUC, "SelezionareIlMagazzinoDiDestinazione", "Selezionare il magazzino di destinazione!"), "DIV_Messaggi");
            //DisabilitaSalvataggioErrore();
             }
    } else {
        if (Get_KendoDDLValue("ddlUbicDestinazione") === "") {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxFormProdottoUC, "SelezionareIlMagazzinoDiDestinazione", "Selezionare il magazzino di destinazione!"), "DIV_Messaggi");
            //DisabilitaSalvataggioErrore();
        }
    }

}

function ddlProdottoDes_change(e) {
    var ddlProdottoDes = KendoDDL("ddlProdottoDes");

    if (ddlProdottoDes.value() !== undefined && ddlProdottoDes.value() !== 0 &&
        ddlProdottoDes.value() !== "" && ddlProdottoDes.value() !== current_Prodotto_Cod) {

        //console.log("Prodotto cambiato" + ddlProdottoDes.value());

        // Imposta la categoria prendendola dal prodotto
        if (parseInt(KendoDDL("ddlCategorieMagazzino").value()) !== ddlProdottoDes.dataItem().Elem_Cod) {
            Set_KendoDDLValueNoDef("ddlCategorieMagazzino", ddlProdottoDes.dataItem().Elem_Cod);
            AzioniCambioCategoriaDaSceltaProdotto();
        }
        
        AzioniCambioProdotto();

        // Lasciare qui alla fine del change
        current_Elem_Cod = KendoDDL("ddlProdottoDes").dataItem().Elem_Cod;
        current_Veg_Cod = KendoDDL("ddlProdottoDes").dataItem().Veg_Cod;
        current_Cul_Cod = KendoDDL("ddlProdottoDes").dataItem().Cul_Cod;
        current_Reg_Cod = KendoDDL("ddlProdottoDes").dataItem().Reg_Cod;
        current_Mat_Cod_OMNI = KendoDDL("ddlProdottoDes").dataItem().Mat_Cod_OMNI;
        current_Prodotto_Cod = KendoDDL("ddlProdottoDes").dataItem().Prodotto_Cod.toString();

        // INIZIO Questo per quando si passa da un prodotto legato a specie ad uno non legato
        if (current_Veg_Cod === 0)
            current_Veg_Cod = -1;
        if (current_Cul_Cod === 0)
            current_Cul_Cod = -1;
        // FINE Questo per quando si passa da un prodotto legato a specie ad uno non legato

    }
}

function ddlProdottoDes_filtering(e) {

    var filter = e.filter;

    if (filter !== undefined && (!filter.value || filter.value.length < LunghezzaMinimaFiltroProdotto)) {

        //prevent filtering if the filter does not value
        e.preventDefault();

        SeImpostaFiltroTuttiProdotti(Evento_Prodotto_Filtering, filter);

    }

}

function ddlProdottoDes_open(e) {

    SeImpostaFiltroTuttiProdotti(Evento_Prodotto_Open, null);

}

function ddlProdAlias_read(options) {
    let arrAliasLegatiProd = [];

    let kddlProdottoDes = KendoDDL("ddlProdottoDes");
    let prodottoCod = parseInt(kddlProdottoDes.value());
    let codRisUm = parseInt(Get_KendoDDLValue(ddlContatto1Nome));
    // La gestione degli alias è valida solo per i prodotti "aziendali" e non quelli standard. In altre parole per i prodotti identificati tramite mat_cod e non pro_cod
    if (prodottoCod < 0) {
        arrAliasLegatiProd = RicercaAliasDaMatCod($(cIdPiva).val(), prodottoCod * -1, codRisUm, true);
    }

    options.success(arrAliasLegatiProd);
}

function ddlProdAlias_databound(ev) {
    // Seleziono l'alias di default.
    // Caso 1) In fase di inizializzazione del form in modifica/duplicazione, verifico se ho un mat_cod_alias da selezionare
    // Caso 2) Negli altri casi, verifico se il cod_risum della testata corrisponde con uno fra quelli presenti
    //         negli alias che ho caricato
    if (inizializzaFormDettaglioRiga === true && riga_originale_entrata_FormProdottoUC !== null && riga_originale_entrata_FormProdottoUC.Mat_Cod_Alias !== undefined) {
        ev.sender.value(riga_originale_entrata_FormProdottoUC.Mat_Cod_Alias);
        //ev.sender.trigger("change");
    }
    else {
        // L'evento databound viene eseguito anche se l'utente filtra sulla ddl stessa, in questo caso non ricalcolo il default
        if (KendoDDL("ddlProdottoDes").value() !== current_Prodotto_Cod) {
            // L'utente ha selezionato un prodotto diverso, verifico se il contatto selezionato ha registrato un alias di default fra quelli caricati del prodotto
            let aliasPerSogg = ev.sender.dataSource.data().filter(function (elem) {
                let arrCodRisumAlias = elem.Filtro_Contatti_String.split("|");
                return arrCodRisumAlias.includes(Get_KendoDDLValue(ddlContatto1Nome));
            });
            if (aliasPerSogg.length === 1) {
                ev.sender.value(aliasPerSogg[0].Mat_Cod_Alias);
            }
        }
    }
}

function gestioneAlias() {
    let dataItemProd = KendoDDL("ddlProdottoDes").dataItem();
    return lavCodVendita === true && dataItemProd !== undefined && dataItemProd.Prodotto_Cod < 0;
}

function AzioniCambioCategoriaDaSceltaProdotto() {

    let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

    // Aggiorna dataSource magazzini
    ImpostaDataSourceDdlUbic("ddlUbicProvenienza", null);
    ImpostaDataSourceDdlUbic("ddlUbicDestinazione", null);

    impostaVisibilitaBtnNuovoProdotto(ddlCategorieMagazzinoValue);

    $("#lblParametroQualitativo").hide();
    setKendoSwitchVisible("chkParametroQualitativo", false);

    ImpostaVisibilita_PerCategoria(Qs_CaricoScarico, ddlCategorieMagazzinoValue);

    Visibilita_PUARegolamento(false, false);

    // La imposto qui e poi la nascondo dove non si deve vedere
    if (FF_gest_materiale_vivaistico || lavCodAccettazionePomodoro)
        Visibilita_UnitaMisura(false, false);
    else
        Visibilita_UnitaMisura(true, true);

    switch (ddlCategorieMagazzinoValue) {

        case ALTRI_BENI:

            Set_KendoDDLValueNoDef("ddlUbicProvenienza", "");
            Set_KendoDDLValueNoDef("ddlUbicDestinazione", "");
                    
            RicercaUdm_Optimize("", false, false, 0, 0, CAU_CARICO, ddlCategorieMagazzinoValue, 0, false, false, false);
            KendoDDL("ddlUM").dataSource.data(elencoUdmOptimized);
            assegnaDefaultDdlUMFormProdotto();

            break;

        case SERVIZI:

            Set_KendoDDLValueNoDef("ddlUbicProvenienza", "");
            Set_KendoDDLValueNoDef("ddlUbicDestinazione", "");
                   
            // TODO PRIMA FAI IL TEST SULL'ATTUALE
            // AgronicaCoreUtility.CaricaListControl.CaricaServizi(Me.cmb_Prodotti, False, "", "", "", "", "", objParametri_Server)

            RicercaUdm_Optimize("", false, false, 0, 0, CAU_CARICO, ddlCategorieMagazzinoValue, 0, false, false, false);
            KendoDDL("ddlUM").dataSource.data(elencoUdmOptimized);
            assegnaDefaultDdlUMFormProdotto();

            break;

        // TUTTO IL RESTO
        default:

            // Imposto le unità di misura possibili per la categoria corrente; poi all'interno del singolo prodotto si va a ridefinirle
            RicercaUdm_Optimize("", false, false, 0, 0, CAU_CARICO, ddlCategorieMagazzinoValue, 0, false, false, false);
            KendoDDL("ddlUM").dataSource.data(elencoUdmOptimized);
            assegnaDefaultDdlUMFormProdotto();

            switch (ddlCategorieMagazzinoValue) {

                case SEMILAVORATI_VEGETALI: case SEMILAVORATI_ANIMALI: 

                    $(".lblParametroQualitativo").show();
                    setKendoSwitchVisible("chkParametroQualitativo", true);

                    break;

                case TRASFORMATI_VEGETALI: case TRASFORMATI_ANIMALI:
                    if (is_Trasf_Veg_Anim_FormProdottoUC()) {
                        var gridId = "tab_imballaggi_formProdottoUC";
                        var grid = $("#" + gridId).data("kendoGrid");
                        if (grid !== undefined) {
                            kendo_AggiustaDimensioneColonne("#" + gridId);
                        }
                    }
                    break;

                case FORMULATI:

                    $("#BtnInfo_Fito").show();

                    if (gestioneRegolamentoFormulati === true) {
                        Se_Abilita_PUARegolamento(ddlCategorieMagazzinoValue);
                    }

                    break;

                case FERTILIZZANTI:

                    $("#BtnInfo_Concime").show();

                    Se_Abilita_PUARegolamento(ddlCategorieMagazzinoValue);

                    break;

            }

    }

    CostruisciLinkInfoProdotto(ddlCategorieMagazzinoValue);
     
    /*  TODO - Tutto da approfondire
            if (ddlCategorieMagazzinoValue === SEMILAVORATI_VEGETALI && Qs_CaricoScarico === CAU_SCARICO) {

                // TODO dopo aver capito a cosa servono questi campi
                //Chk_LottoImpianto.Checked = True
                //Chk_LottoImpianto.Enabled = True
                setKendoSwitch("chkParametroQualitativo", true);
                switchParametroQualitativo.enable(true);

                $("#idLottoImpianto").hide();
                $("#lblLottoImpianto").hide();
                ClearDDL("ddlLottoImpianto");
                Set_KendoDDLValueNoDef("ddlLottoImpianto", "");  //TODO Valore dft corretto?
                KendoDDL("ddlLottoImpianto").wrapper.hide();

                $("#lblAggregaLottoImpianto").hide();
                setKendoSwitchVisible("chkAggregaLottoImpianto", false);

                $("#idParametroQualitativoCalibro").hide();
                $("#lblCalibro").hide();
                Set_KendoDDLValueNoDef("ddlCalibro", 0);  //TODO Valore dft corretto?
                KendoDDL("ddlCalibro").wrapper.hide();

            } else if ((ddlCategorieMagazzinoValue === RIGA_DESCRIZIONE_LIBERA ||
                ddlCategorieMagazzinoValue === TRASFORMATI_VEGETALI ||
                ddlCategorieMagazzinoValue === ALTRI_BENI ||
                ddlCategorieMagazzinoValue === BENI_CONFEZ_VEGETALE ||
                ddlCategorieMagazzinoValue === FORMULATI ||
                ddlCategorieMagazzinoValue === FERTILIZZANTI)
                //&& Qs_CaricoScarico === CAU_SCARICO
            ) {

                switchParametroQualitativo.enable(false);

                // TODO dopo aver capito a cosa servono questi campi
                //Chk_LottoImpianto.Checked = False
                //Chk_LottoImpianto.Enabled = False

                $("#idLottoImpianto").hide();
                $("#lblLottoImpianto").hide();
                ClearDDL("ddlLottoImpianto");
                Set_KendoDDLValueNoDef("ddlLottoImpianto", "");  //TODO Valore dft corretto?
                KendoDDL("ddlLottoImpianto").wrapper.hide();

                $("#lblAggregaLottoImpianto").hide();
                setKendoSwitchVisible("chkAggregaLottoImpianto", false);

                $("#idParametroQualitativoCalibro").hide();
                $("#lblCalibro").hide();
                Set_KendoDDLValueNoDef("ddlCalibro", 0);  //TODO Valore dft corretto?
                KendoDDL("ddlCalibro").wrapper.hide();

            } else {

                //TODO: invertire questi due blocchi una volta che si è capito su quali categorie vanno mostrati

                // TODO dopo aver capito a cosa servono questi campi
                //Chk_LottoImpianto.Checked = False
                //Chk_LottoImpianto.Enabled = False
                setKendoSwitch("chkParametroQualitativo", false);
                //getKendoSwitch("chkParametroQualitativo").enable(false);
                let switchParametroQualitativo = $("#chkParametroQualitativo").kendoSwitch().data("kendoSwitch");
                switchParametroQualitativo.enable(false);

                $("#idLottoImpianto").show();
                $("#lblLottoImpianto").show();
                ClearDDL("ddlLottoImpianto");
                Set_KendoDDLValueNoDef("ddlLottoImpianto", "");  //TODO Valore dft corretto?
                KendoDDL("ddlLottoImpianto").wrapper.show();

                $("#lblAggregaLottoImpianto").show();
                setKendoSwitchVisible("chkAggregaLottoImpianto", false);   //TODO

                $("#idParametroQualitativoCalibro").show();
                $("#lblCalibro").show();
                Set_KendoDDLValueNoDef("ddlCalibro", 0);  //TODO Valore dft corretto?
                KendoDDL("ddlCalibro").wrapper.show();

            }
    */

    impostaPanelBarContabilita();
}


function AzioniCambioProdotto() {

    var ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

    AzzeraAlCambioCategoriaOProdotto("P");

    let messaggioErrore = "";

    // In caso di Carico F&F Trasf Vegetali e prodotto legato a linea, se il magazzino è vuoto, vado ad aggiornare il magazzino di default in base alla linea
    if (modulo_anagrafe_log.includes(Modulo_FreshFood) &&
        Get_KendoDDLValue("ddlProdottoDes") !== "" &&
        KendoDDL("ddlProdottoDes").dataItem().LegatoALinea === 1 &&
        KendoDDL("ddlProdottoDes").dataItem().Elem_Cod === TRASFORMATI_VEGETALI) {
        let codProdotto = Math.abs(parseInt(Get_KendoDDLValue("ddlProdottoDes"))); 
        if (Get_KendoDDLValue("ddlUbicDestinazione") === "" && is_210_FormProdottoUC() && Qs_CaricoScarico === CAU_CARICO) {
            let keyUbic = RicercaMagazzinoCellaDaLineaProdotto($(cIdPiva).val(), xSa_Cod, Modulo_FreshFood, codProdotto);
            if (keyUbic !== undefined && keyUbic !== null)
                Set_KendoDDLValue("ddlUbicDestinazione", keyUbic);
            else
                messaggioErrore += TraduzioneMultiResx(resxFormProdottoUC, "SelezionareIlMagazzinoDiDestinazione", "Selezionare il magazzino di destinazione");
        }  
    }

    if (ddlCategorieMagazzinoValue !== ALTRI_BENI &&
        ddlCategorieMagazzinoValue !== SERVIZI) {
        if (Qs_CaricoScarico === CAU_TRASFERIMENTO) {
            if (Get_KendoDDLValue("ddlUbicProvenienza") === "")
                messaggioErrore += TraduzioneMultiResx(resxFormProdottoUC, "SelezionareIlMagazzinoDiProvenienza", "Selezionare il magazzino di provenienza");
            if (Get_KendoDDLValue("ddlUbicDestinazione") === "")
                messaggioErrore += TraduzioneMultiResx(resxFormProdottoUC, "SelezionareIlMagazzinoDiDestinazione", "Selezionare il magazzino di destinazione");
        }
        else if (Qs_CaricoScarico === CAU_SCARICO) {
            if (Get_KendoDDLValue("ddlUbicProvenienza") === "")
                messaggioErrore += TraduzioneMultiResx(resxFormProdottoUC, "SelezionareIlMagazzinoDiProvenienza", "Selezionare il magazzino di provenienza");
        }
        else if (Qs_CaricoScarico === CAU_CARICO) {
            //if (Get_KendoDDLValue("ddlUbicDestinazione") === "")
            //    messaggioErrore += TraduzioneMultiResx(resxFormProdottoUC, "SelezionareIlMagazzinoDiDestinazione", "Selezionare il magazzino di destinazione");
        }
    }

    if (messaggioErrore === "") {

        Recupera_ChiaveMagazzino();

        if (ddlCategorieMagazzinoValue !== 0 && Get_KendoDDLValue("ddlProdottoDes") !== "") {

            let dataItemProd = KendoDDL("ddlProdottoDes").dataItem();

            if (dataItemProd.Extra_Str !== null && dataItemProd.Extra_Str !== undefined) {
                // Valorizzo la txt "descrizione addizionale" con l'omonimo valore da anagrafica (è solo per le materie_prime)
                $('input[name$="txtExtra_Str"]').val(dataItemProd.Extra_Str);
            }
            else {
                // Per gli altri prodotti che non la gestiscono riazzero il valore eventualmente inserito
                $('input[name$="txtExtra_Str"]').val("");
            }

            let Cau_Mov = "";
            let RegolamentoCod = 0;

            if (Qs_CaricoScarico === CAU_SCARICO || Qs_CaricoScarico === CAU_TRASFERIMENTO)
                Cau_Mov = CAU_SCARICO;
            else
                Cau_Mov = CAU_CARICO;

            switch (ddlCategorieMagazzinoValue) {

                case RIGA_DESCRIZIONE_LIBERA:
                case ALTRI_BENI:
                case SERVIZI:
                    break;

                case SEMILAVORATI_VEGETALI:
                    // TODO , Flag_QtaNoZero, Flag_QtaMaggioreZero
                    RicercaUdm_Optimize_Regolamento(false, xSa_Cod, xFabbricato_Cod, Cau_Mov, ddlCategorieMagazzinoValue, parseInt(Get_KendoDDLValue("ddlProdottoDes")), 0, false, false, false);
                    KendoDDL("ddlUM").dataSource.data(elencoUdmOptimizedRegolamento);
                    assegnaDefaultDdlUMFormProdotto();

                    KendoDDL("ddlCalibro").enable(true);

                    //TODO Aggrega lotto impianto
                    //$("#groupLottoImpianto").show();
                    //$("#groupAggregaLottoImpianto").show();
                    //ClearDDL("ddlLottoImpianto");
                    //KendoDDL("ddlLottoImpianto").wrapper.show(); 
                    //$("#lblAggregaLottoImpianto").show();
                    //setKendoSwitchVisible("chkAggregaLottoImpianto", false);

                    if (Cau_Mov !== CAU_CARICO) {

                        if (getKendoSwitch("chkAggregaLottoImpianto")) {

                            KendoDDL("ddlCalibro").enable(false);
                            // TODO  Cambio_LottoInterno();

                        } else {

                            // TODO INIZIO
                            //caso vecchio
                            //AgronicaCoreUtility.CaricaListControl.Semilavorati_LottoInterno(Me.cmb_Lotto,
                            //    Num_Totale,
                            //    xPiva,
                            //    xSa_Cod,
                            //    xFabbricato_Cod,
                            //    -Me.cmb_Prodotti.SelectedItem.Value,
                            //    True, "", "",
                            //    "",
                            //    CDate(Me.Txt_DataMovimento.Text),
                            //    CStr(Cau_Mov),
                            //    "", "", objParametri_Server)
                            // TODO FINE
                        }

                    } else {

                        // CARICO

                        if (cIdTipoOp === enum_TipoOperazioneDB.Lettura.value) {

                            //'CASO DI LETTURA

                            //'CASO A
                            //'STO CONSULTANDO IN INFO UN CASO DI CARICO RIFERITO DI SEMILAVORATI
                            //'( i semilavorati sono stati caricati con un'operazione di raccolta
                            //'e hanno il cod_progetto valorizzato, ovvero l'impianto)
                            //'AgronicaCoreUtility.CaricaListControl.Semilavorati_LottoInterno(Me.cmb_Lotto, _
                            //'                    Num_Totale, _
                            //'                    xPiva, _
                            //'                    xSa_Cod, _
                            //'                    xFabbricato_Cod, _
                            //'                    -Me.cmb_Prodotti.SelectedItem.Value, _
                            //'                   True, "", "", _
                            //'                    Me.Txt_CercaLotto.Text, CDate(Me.Txt_DataMovimento.Text), "", "", objParametri_Server)
                            //'CASO B
                            //'i semilavorati provengono da terzi
                            //'hanno il cod_progetto =0, ovvero nessun impianto

                            // TODO Con i nomi dei campi giusti
                            let ddlLottoImpianto = KendoDDL("ddlLottoImpianto");
                            ddlLottoImpianto.dataSource.add({
                                LottoID: 0,
                                LottoName: "Da Terzi"
                            });

                        } else {

                            //CASO DI SCRITTURA O MODIFICA

                            //'i semilavorati provengono da terzi
                            //'hanno il cod_progetto =0, ovvero nessun impianto
                            let ddlLottoImpianto = KendoDDL("ddlLottoImpianto");

                            // TODO Quale delle due è corretta?
                            ddlLottoImpianto.dataSource.add({
                                LottoID: "",
                                LottoName: ""
                            });
                            ddlLottoImpianto.dataSource.add({
                                LottoID: 0,
                                LottoName: "Da Terzi"
                            });

                        }

                    }
                    break;

                case SEMENTI: case TRASFORMATI_VEGETALI: case ALTRE_MATERIE: case TRASFORMATI_ANIMALI: 
                    if (!FF_gest_materiale_vivaistico) {
                        // TODO , Flag_QtaNoZero, Flag_QtaMaggioreZero
                        RicercaUdm_Optimize_Regolamento(false, xSa_Cod, xFabbricato_Cod, Cau_Mov, ddlCategorieMagazzinoValue, parseInt(Get_KendoDDLValue("ddlProdottoDes")), 0, is_Trasf_Veg_Anim_FormProdottoUC(), false, false);
                        KendoDDL("ddlUM").dataSource.data(elencoUdmOptimizedRegolamento);
                        assegnaDefaultDdlUMFormProdotto();
                    }
                    break;

                case FERTILIZZANTI:

                    RegolamentoCod = Get_KendoDDLValue("ddlPUARegolamento", 0);
                    if (RegolamentoCod === "") {
                        RegolamentoCod = 0;
                    }
                    // TODO , Flag_QtaNoZero, Flag_QtaMaggioreZero

                    RicercaUdm_Optimize_Regolamento(false, xSa_Cod, xFabbricato_Cod, Cau_Mov, ddlCategorieMagazzinoValue, parseInt(Get_KendoDDLValue("ddlProdottoDes")), RegolamentoCod, false, false, false);
                    KendoDDL("ddlUM").dataSource.data(elencoUdmOptimizedRegolamento);
                    assegnaDefaultDdlUMFormProdotto();

                    TrattaPuaRegolamento();

                    $("#BtnInfo_Concime").show();

                    break;

                case FORMULATI:

                    RegolamentoCod = Get_KendoDDLValue("ddlPUARegolamento", 0);
                    if (RegolamentoCod === "") {
                        RegolamentoCod = 0;
                    }
                    // TODO , Flag_QtaNoZero, Flag_QtaMaggioreZero
                    RicercaUdm_Optimize_Regolamento(false, xSa_Cod, xFabbricato_Cod, Cau_Mov, ddlCategorieMagazzinoValue, parseInt(Get_KendoDDLValue("ddlProdottoDes")), RegolamentoCod, false, false, false);
                    KendoDDL("ddlUM").dataSource.data(elencoUdmOptimizedRegolamento);
                    assegnaDefaultDdlUMFormProdotto();

                    var messaggiVerificaPermessoEUdm = VerificaClasseToxPatentinoERicercaUdm(parseInt(Get_KendoDDLValue("ddlProdottoDes")), KendoDDL("ddlProdottoDes").dataItem().Udm_Cod);

                    if (messaggiVerificaPermessoEUdm != undefined && messaggiVerificaPermessoEUdm != null) {

                        if (messaggiVerificaPermessoEUdm.messStop !== "") {
                            messaggioErrore = messaggiVerificaPermessoEUdm.messStop;
                            CreaDdlProdottoDes();
                        } else if (messaggiVerificaPermessoEUdm.messWarning !== "") {
                            alert(messaggiVerificaPermessoEUdm.messWarning);
                        }

                        if (messaggiVerificaPermessoEUdm.newUdm_Cod) {
                            impostaDftValueDdlUM(messaggiVerificaPermessoEUdm.newUdm_Cod);
                        }
                    }

                    $("#BtnInfo_Fito").show();

                    break;

                case FARMACI:
                    let farmCod = parseInt(Get_KendoDDLValue("ddlProdottoDes"));
                    Recupera_UdmsFarmaco(farmCod);

                    if (elencoUdmOptimized.length === 0) {                        
                        RegolamentoCod = Get_KendoDDLValue("ddlPUARegolamento", 0);
                        if (RegolamentoCod == "")
                            RegolamentoCod = 0;

                        RicercaUdm_Optimize_Regolamento(false, xSa_Cod, xFabbricato_Cod, Cau_Mov, ddlCategorieMagazzinoValue, farmCod, RegolamentoCod, false, false, false);
                        KendoDDL("ddlUM").dataSource.data(elencoUdmOptimizedRegolamento);

                        let UDM_ML = 101;
                        if (elencoUdmOptimizedRegolamento.some(udm => udm.Udm_Cod = UDM_ML)) 
                            impostaDftValueDdlUM(UDM_ML);
                        else 
                            assegnaDefaultDdlUMFormProdotto();
                    } else {
                        // Ottiene l'UdM di default del farmaco
                        let defaultUdm;
                        if (elencoUdmOptimized.some(udm => udm.isDefault == 1))
                            defaultUdm = elencoUdmOptimized.find(udm => udm.isDefault == 1).Udm_Cod;
                        else
                            defaultUdm = elencoUdmOptimized[0].Udm_Cod;

                        KendoDDL("ddlUM").dataSource.data(elencoUdmOptimized);
                        impostaDftValueDdlUM(defaultUdm);
                    }

                    break;

                default:

                    RegolamentoCod = Get_KendoDDLValue("ddlPUARegolamento", 0);
                    if (RegolamentoCod === "") {
                        RegolamentoCod = 0;
                    }
                    // TODO , Flag_QtaNoZero, Flag_QtaMaggioreZero
                    RicercaUdm_Optimize_Regolamento(false, xSa_Cod, xFabbricato_Cod, Cau_Mov, ddlCategorieMagazzinoValue, parseInt(Get_KendoDDLValue("ddlProdottoDes")), RegolamentoCod, false, false, false);
                    KendoDDL("ddlUM").dataSource.data(elencoUdmOptimizedRegolamento);
                    assegnaDefaultDdlUMFormProdotto();

            }
        } 

        if (ddlCategorieMagazzinoValue !== 0 &&
            ddlCategorieMagazzinoValue === FORMULATI) {
                //per i formulati visualizzo cmq l'icona
                //anziché andare nella scheda del profitosan
                //andrà nella homepage

                $("#BtnInfo_Fito").show();
        }

        if (messaggioErrore === "") {

            // TODO TOGLIERE RIGHE COMMENTATE SE VA BENE CERCARLA SEMPRE
            ////////Per scarichi F & F viene fatto a comando con un pulsante  
            //////if (!is_Trasf_Veg_Anim_FormProdottoUC() ||
            //////    (cIdLavCod !== enum_LavCod.Ordine_Vendita_Emesso.value &&
            //////        cIdLavCod !== enum_LavCod.DDT_Emesso.value &&
            //////        cIdLavCod !== enum_LavCod.Fattura_Emessa.value)) {
                CaricaGiacenze(null);
            //////}

            if (KendoDDL("ddlCategorieMagazzino").dataSource.data().length === 2) {
                // TODO  Impostare dft al primo valore
                //Set_KendoDDLValueNoDef("ddlCategorieMagazzino", KendoDDL("ddlCategorieMagazzino").dataSource.data()[0].Udm_Cod);
                //$("#ddlCategorieMagazzino").trigger("change");
                // TODO  Cambio_LottoInterno();    
            }
        }

    }
    
    if (cIdTipoOp !== enum_TipoOperazioneDB.Lettura.value && !isContrattoAffitto()) {
        CaricaIvaContiDefault();
    }

    // se accettazione pomodoro verifica se c'è contratto attivo
    if (messaggioErrore === "") {
        if (lavCodAccettazionePomodoro) {
            messaggioErrore = Verifica_Contratto_Pomodoro(true);
        } else {
            Riepilogo_Prezzi_Listini(true);
        }
    }

    if (messaggioErrore !== "") {
        MessaggioErrore_Bootstrap(messaggioErrore, "DIV_Messaggi");
        //DisabilitaSalvataggioErrore();
    }

    let w_KeyDet = $('input[name$="hf_key_mov_dett"]').val();

    if (isImputazioneImpiantiAttiva() && (w_KeyDet === "" || current_Veg_Cod !== -1)) {

        // Se nuovo prodotto è legato a diverso prodotto OMNI ricarico griglia impianti
        if (KendoDDL("ddlProdottoDes").dataItem().Prodotto_Cod !== undefined &&
            KendoDDL("ddlProdottoDes").dataItem().Prodotto_Cod !== 0 &&
            (KendoDDL("ddlProdottoDes").dataItem().Veg_Cod !== current_Veg_Cod ||
             KendoDDL("ddlProdottoDes").dataItem().Cul_Cod !== current_Cul_Cod ||
             KendoDDL("ddlProdottoDes").dataItem().Reg_Cod !== current_Reg_Cod)) {
                ConfiguraGrigliaImpianti("tab_griglia_impianti", 1);
        }
    }

    const ghg = existsGHGParameter()
    if (ghg && lavCodAccettazione === true) {
        AggiornaGHG();
    }

    return messaggioErrore;
}

function existsGHGParameter() {
    if (paramQual_FF_filtrospevar !== undefined && paramQual_FF_filtrospevar !== null && paramQual_FF_filtrospevar.length !== 0) {
        for (let i = 0, e = paramQual_FF_filtrospevar.length; i < e; i++) {
            if (paramQual_FF_filtrospevar[i].Tabella_Cod_Des == "ghgforec") {
                return true;
            }
        }
    }
    return false
}

function ddlUM_change(e) {

    let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
    let w_um = 0;

    if (Get_KendoDDLValue("ddlUM") !== undefined && Get_KendoDDLValue("ddlUM") !== null && Get_KendoDDLValue("ddlUM") !== "")
        w_um = parseInt(Get_KendoDDLValue("ddlUM"));

    let udm_multipli_g = [enum_Udm.chilogrammi.value, enum_Udm.quintali.value, enum_Udm.tonnellate.value];

    if (w_um !== 0) {

        // Normalmente, al cambio unità di misura occorre ri-calcolare la giacenza, ma se l'u.m.
        // precedentemente selezionata era kg od i suoi multipli e quella attualmente selezionata lo è ancora,
        // la giacenza è la stessa e viene mostrata in kg
        let um_prec = $("#ddlUM").data("umselprec");
        $("#ddlUM").data("umselprec", w_um);
        let nonAzzerareQtaGiac = udm_multipli_g.includes(w_um) && udm_multipli_g.includes(um_prec);

        // Se questa configurazione è impostata a true la utilizzo poi la disattivo.
        // È utilizzata in caso di cambio della u.m. quando si sceglie una giacenza 
        // il cui prodotto è movimentato in confezioni come parametri qualitativi,
        // perché l'u.m. impostata in automatico risulta essere numero, ma occorre cambiarla in kg
        let conservaLottoConfFF = $("#ddlUM").data("conservalottoconfezioniff");
        if (conservaLottoConfFF === true) {
            $("#ddlUM").data("conservalottoconfezioniff", false);
        }
        else {
            conservaLottoConfFF = false;
        }

        //dataSource prezzo livello
        if (udm_multipli_g.includes(w_um) && is_Trasf_Veg_Anim_FormProdottoUC()) {
            KendoDDL("ddlPrezzoRiferitoA").setDataSource(elencoPrezzoLivelloFF);
            Set_KendoDDLValueNoDef("ddlPrezzoRiferitoA", -1);
        } else {
            // TODO seconda UM gestita
            // if (!secondaUMGestita)
            KendoDDL("ddlPrezzoRiferitoA").setDataSource(elencoPrezzoLivelloSoloQta);
            // else
            //      KendoDDL("ddlPrezzoRiferitoA").setDataSource(elencoPrezzoLivelloNoFF);

            Set_KendoDDLValueNoDef("ddlPrezzoRiferitoA", 0);
        }

        // F&F o Zoo attivo con U.M. KG
        if (udm_multipli_g.includes(w_um) && is_Trasf_Veg_Anim_FormProdottoUC()) {

            Visibilita_KgNetti(true, true);
            Visibilita_Quantita(false, false);
            Visibilita_QuantitaRiscontrata(false, false);

            //gestisce i diversi casi per la visibilità/obbligatorietà del campo
            Gestione_Visibilita_Degrado(GetPropertyFromJson($(cIdOpzioniContab).val(), "Degrado_Visibilita_Obbligatorieta"))

            if (FF_gest_materiale_vivaistico) {
                Visibilita_Tara(false, false);
                Visibilita_TaraRiscontrata(false, false);
                Visibilita_KgLordi(false, false);
                Visibilita_KgNettiRiscontrati(false, false);
                Visibilita_KgLordiRiscontrati(false, false);
                Visibilita_BtnRiscontrati(false);
                Visibilita_Degrado(false, false);
            }
            else {
                Visibilita_KgLordi(true, true);

                if (lavCodAccettazionePomodoro) {
                    Visibilita_Tara(false, false);
                    Visibilita_TaraRiscontrata(false, false);
                    Visibilita_BtnRiscontrati(false);
                    Visibilita_KgLordiRiscontrati(false, false);

                    Visibilita_KgNettiRiscontrati(gestionePesiRiscontrati, false);
                    KendoNumTB("idKgNettiRiscontrati").enable(true);
                }
                else {
                    Visibilita_KgNettiRiscontrati(gestionePesiRiscontrati, false);
                    Visibilita_KgLordiRiscontrati(gestionePesiRiscontrati, false);
                    Visibilita_BtnRiscontrati(gestionePesiRiscontrati);

                    if (VerificaValorizzazionePesiRiscontrati() === false) {
                        KendoNumTB("idKgNettiRiscontrati").enable(false);
                        KendoNumTB("idKgLordiRiscontrati").enable(false);
                    }
                    else {
                        KendoNumTB("idKgNettiRiscontrati").enable(true);
                        KendoNumTB("idKgLordiRiscontrati").enable(true);
                    }


                    Visibilita_Tara(true, false);
                    Visibilita_TaraRiscontrata(gestionePesiRiscontrati, false);

                    if (gestitoImballaggio_FF || gestitoContenitore_FF || gestitoConfezione_FF) {
                        KendoNumTB("idTara").enable(false);
                        KendoNumTB("idTaraRiscontrata").enable(false);
                    }
                }
            }

        } else {
            Visibilita_Degrado(false, false);
            Visibilita_KgNetti(false, false);
            Visibilita_KgLordi(false, false);
            Visibilita_KgNettiRiscontrati(false, false);
            Visibilita_KgLordiRiscontrati(false, false);
            Visibilita_Quantita(true, true);

            // Setto il nr decimali e formato dinamicamente
            let format = "n0";
            let decimals = 0;
            for (let key in enum_Udm) {
                let um = enum_Udm[key];
                if (w_um === um.value) {
                    format = um.format;
                    decimals = um.decimals;
                    break;
                }
            }
            KendoNumTB("idQuantita").setOptions({ format: format, decimals: decimals });
            //KendoNumTB("idQuantita").focus(); //Necessario sennò il campo appare vuoto

            //TODO: verificare se per tutte le altre categorie deve essere visibile o meno la tara (intanto tolgo l'obbligatorietà)
            if (ddlCategorieMagazzinoValue !== undefined &&
                ddlCategorieMagazzinoValue !== BENI_CONFEZ_VEGETALE &&
                ddlCategorieMagazzinoValue !== CARBURANTI &&
                ddlCategorieMagazzinoValue !== FERTILIZZANTI &&
                ddlCategorieMagazzinoValue !== FORMULATI &&
                ddlCategorieMagazzinoValue !== FARMACI &&
                ddlCategorieMagazzinoValue !== INNESCHI &&
                ddlCategorieMagazzinoValue !== TRAPPOLE &&
                ddlCategorieMagazzinoValue !== RIGA_DESCRIZIONE_LIBERA &&
                ddlCategorieMagazzinoValue !== ALTRE_MATERIE &&
                ddlCategorieMagazzinoValue !== SERVIZI &&
                ddlCategorieMagazzinoValue !== ALTRI_BENI) {

                if (!udm_multipli_g.includes(w_um) && is_Trasf_Veg_Anim_FormProdottoUC()) {
                    KendoNumTB("idTara").enable(false);
                }
                else {
                    KendoNumTB("idTara").enable(true);
                }

                Visibilita_Tara(true, false);

                if (gestionePesiRiscontrati) {
                    Visibilita_QuantitaRiscontrata(true, false);
                    KendoNumTB("idQuantitaRiscontrata").setOptions({ format: format, decimals: decimals });
                    //KendoNumTB("idQuantitaRiscontrata").focus(); //Necessario sennò il campo appare vuoto

                    if (!udm_multipli_g.includes(w_um) && is_Trasf_Veg_Anim_FormProdottoUC()) {
                        Visibilita_BtnRiscontrati(true);
                        if (VerificaValorizzazionePesiRiscontrati() === false) {
                            KendoNumTB("idQuantitaRiscontrata").enable(false);
                        }
                        else {
                            KendoNumTB("idQuantitaRiscontrata").enable(true);
                        }

                        KendoNumTB("idTaraRiscontrata").enable(false);
                        Visibilita_TaraRiscontrata(true, false);
                    }
                    else {
                        Visibilita_BtnRiscontrati(false);
                        KendoNumTB("idQuantitaRiscontrata").enable(true);

                        // La tara riscontrata sarebbe da mostrare per mantenere lo stesso comportamento del campo tara, 
                        // ma non salvandola a database e non avendo in questo caso gli imballi ed il peso lordo
                        // per poterla ricalcolare la nascondo
                        //KendoNumTB("idTaraRiscontrata").enable(true);
                        Visibilita_TaraRiscontrata(false, false);
                    }
                }
                else {
                    Visibilita_QuantitaRiscontrata(false, false);
                    Visibilita_TaraRiscontrata(false, false);
                    Visibilita_BtnRiscontrati(false);
                }

            } else {
                Visibilita_QuantitaRiscontrata(gestionePesiRiscontrati, false);
                KendoNumTB("idQuantitaRiscontrata").enable(true);

                Visibilita_Tara(false, false);
                Visibilita_TaraRiscontrata(false, false);
                Visibilita_BtnRiscontrati(false);
            }

            //KendoDDL("ddlUM").focus(); //Rimetto il focus sul campo , per evitare che rimanga selezionata la quantità
        }

        // solo se non si tratta di altri beni o servizi
        if ($('input[name$="txtBeniStrumentali"]').attr("type") === "hidden" ||
            $('input[name$="txtBeniStrumentali"]').is(":hidden")) {

            if (Qs_CaricoScarico === CAU_CARICO) {
                if (Get_KendoDDLValue("ddlUbicDestinazione") !== "") {
                    //TODO: al momento non c'è
                    //Carica_Giacenze_Prezzo();
                }
            }
            else if (Qs_CaricoScarico === CAU_SCARICO) {
                if (Get_KendoDDLValue("ddlUbicProvenienza") !== "") {
                    //TODO: al momento non c'è
                    //Carica_Giacenze_Prezzo();
                    if (KendoDDL("ddlLottoAccettazione") !== undefined && nonAzzerareQtaGiac === false && conservaLottoConfFF === false) {
                        ImpostaDdlLottoAccettazione([], null);
                    }
                }
            } else if (Qs_CaricoScarico === CAU_TRASFERIMENTO) {

                if (Get_KendoDDLValue("ddlUbicDestinazione") !== "" && Get_KendoDDLValue("ddlUbicProvenienza") !== "" &&
                    Get_KendoDDLValue("ddlUbicProvenienza") === Get_KendoDDLValue("ddlUbicDestinazione")) {
                    MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxFormProdottoUC, "MagazziniDiProvenienzaEDestinazioneCoincidono",
                        "Il magazzino di provenienza e quello di destinazione coincidono! Modificare uno dei due magazzini."), "DIV_Messaggi");
                    //DisabilitaSalvataggioErrore();
                }
                else {
                    if (KendoDDL("ddlLottoAccettazione") !== undefined && nonAzzerareQtaGiac === false && conservaLottoConfFF === false) {
                        ImpostaDdlLottoAccettazione([], null);
                    }
                }

                //TODO: al momento non c'è
                //Carica_Giacenze_Prezzo();
            }

            if (nonAzzerareQtaGiac === false && conservaLottoConfFF === false) {
                if (Qs_CaricoScarico === CAU_CARICO) {
                    // TODO ?
                    // TODO ? If CDbl(ViewState("Giacenza_Magazzino")) >= 0 Then
                    // TODO ?Me.Txt_Quantita.Text = CStr(ViewState("Giacenza_Magazzino"))
                    // TODO ?Else
                    AzzeraGiacenza();
                    AzzeraQuantita();
                    // TODO ?End If
                }

                let kddlProdotto = KendoDDL("ddlProdottoDes");

                if (kddlProdotto.dataSource.data() !== undefined && kddlProdotto.dataItem() !== undefined && kddlProdotto.value() !== 0 && kddlProdotto.value() !== "") {
                    CaricaGiacenze(null);
                }
            }
            
        }

        // Aggiorno il prezzo
        if (!lavCodAccettazionePomodoro)
            Riepilogo_Prezzi_Listini(true);

    } else {

        //dataSource prezzo livello
        if (is_Trasf_Veg_Anim_FormProdottoUC())
            KendoDDL("ddlPrezzoRiferitoA").setDataSource(elencoPrezzoLivelloFF);
        else {
            // TODO seconda UM gestita
            // if (!secondaUMGestita)
            KendoDDL("ddlPrezzoRiferitoA").setDataSource(elencoPrezzoLivelloSoloQta);
            // else
            //      KendoDDL("ddlPrezzoRiferitoA").setDataSource(elencoPrezzoLivelloNoFF);
        }

        Visibilita_KgNetti(false, false);
        Visibilita_KgLordi(false, false);
        Visibilita_KgNettiRiscontrati(false, false);
        Visibilita_KgLordiRiscontrati(false, false);
        Visibilita_BtnRiscontrati(false);
        Visibilita_Quantita(false, false);
        Visibilita_QuantitaRiscontrata(false, false);
        Visibilita_Tara(false, false);
        Visibilita_TaraRiscontrata(false, false);

        Visibilita_Degrado(false, false);
    }

}

function impostaDftValueDdlUM(value) {

    let w_um_precedente = "";
    if (KendoDdlConDataSourceDefinito("ddlUM")) {
        w_um_precedente = Get_KendoDDLValue("ddlUM");
    }

    // Al momento le varie letture dell'U.M. possibili non mettono la riga vuota
    // Al primo giro qui si entra con 0 e quindi non viene trovata l'UM; per ora la scelta è di mettere la prima

    let w_um = String(value);
    let w_um_settata = false;
    let w_um_trovata = false;

    if (KendoDdlConDataSourceDefinito("ddlUM")) {

        for (let i = 0; i < KendoDDL("ddlUM").dataSource.data().length; i++) {

            if (String(KendoDDL("ddlUM").dataSource.data()[i].Udm_Cod) === w_um) {

                if (w_um === "0" || w_um !== w_um_precedente || Get_KendoDDLValue("ddlProdottoDes") === "") {
                    Set_KendoDDLValueNoDef("ddlUM", w_um);
                    w_um_settata = true;
                }

                w_um_trovata = true;
                break;

            }

        }

    }

    if (w_um_settata) {
        ddlUM_change();
    }

    return w_um_trovata;

}

function KendoDdlConDataSourceDefinito(nomeDdl) {

    if (KendoDDL(nomeDdl) !== undefined && KendoDDL(nomeDdl).dataSource !== undefined && KendoDDL(nomeDdl).dataSource.data() !== undefined) {

        return true;

    } else {

        return false;

    }

}

function assegnaDefaultDdlUMFormProdotto() {

    let found = false;

    // 0 - Accettazione pomodoro --> Sempre Kg
    if (lavCodAccettazionePomodoro)
        found = impostaDftValueDdlUM(enum_Udm.chilogrammi.value);

    // 1 - Solo una UM --> setto quella
    if (!found) {
        if (KendoDDL("ddlUM").dataSource !== undefined && KendoDDL("ddlUM").dataSource.data().length === 1) {
            found = impostaDftValueDdlUM(KendoDDL("ddlUM").dataSource.data()[0].Udm_Cod);
        } 
    }

    // 2 - Utilizzo il default dal prodotto --> setto quella
    if (!found) {
        if (KendoDDL("ddlProdottoDes") !== undefined && KendoDDL("ddlProdottoDes").dataItem() !== undefined &&
            KendoDDL("ddlProdottoDes").dataItem() !== null && KendoDDL("ddlProdottoDes").dataItem().Udm_Cod !== 0) {
            found = impostaDftValueDdlUM(KendoDDL("ddlProdottoDes").dataItem().Udm_Cod);
        }
    }
        
    // 3 - Utilizzo UM di default dell'utente
    if (!found) {
        if (hf_ArrayElemCodUdmCod !== undefined && hf_ArrayElemCodUdmCod !== "" && KendoDDL("ddlCategorieMagazzino") !== undefined) {
            let hf_ArrayElemCodUdmCodArray = JSON.parse(hf_ArrayElemCodUdmCod);
            let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
            for (var i = 0; i < hf_ArrayElemCodUdmCodArray.length; i++) {
                var obj = hf_ArrayElemCodUdmCodArray[i];
                if (obj.Elem_Cod === ddlCategorieMagazzinoValue) {
                    found = impostaDftValueDdlUM(obj.Udm_Cod);
                    break;
                }
            }
        }
    }

    // 4 - Imposto la prima dell'elenco
    if (!found) {
        if (KendoDDL("ddlUM").dataSource !== undefined && KendoDDL("ddlUM").dataSource.data().length !== 0) {
            found = impostaDftValueDdlUM(KendoDDL("ddlUM").dataSource.data()[0].Udm_Cod);
        }
    }

}

function btnImpostaPesiRiscontratiClick(ev) {

    if (sonoInModificaImballi) {
        kendo.alert(TraduzioneMultiResx(resxFormProdottoUC, "OccorreConfermareAnnullareModificheImballi", "Occorre prima confermare od annullare le modifiche agli imballi!"));
        return true;
    }

    if (VerificaValorizzazionePesiRiscontrati() === true) {

        let content = "<p>" + TraduzioneMultiResx(resxFormProdottoUC, "ConfermaSostituzionePesiRiscontratiConReali",
            "Continuando, i valori attualmente inseriti nei campi 'riscontrati' verranno sostituiti con quelli di riferimento, si desidera proseguire?") + "</p>";

        $('#formProdottoUC_dialogConfermaGenerica').kendoDialog({
            width: "450px",
            title: TraduzioneMultiResx(resxFormProdottoUC, "Conferma", "Conferma"),
            closable: true,
            modal: true,
            visible: false,
            content: content,
            actions: [
                { text: TraduzioneMultiResx(resxFormProdottoUC, "Si", 'Sì'), action: ImpostaPesiRiscontrati },
                { text: TraduzioneMultiResx(resxFormProdottoUC, "No", 'No'), primary: true, action: function () { return true; } }
            ]
        });

        $('#formProdottoUC_dialogConfermaGenerica').data("kendoDialog").open();
    }
    else {
        // Proseguo senza necessità di chiedere conferma
        ImpostaPesiRiscontrati();
    }

}

/** La funzione prende i valori dei pesi attesi e li copia sui campi dei pesi riscontrati */
function ImpostaPesiRiscontrati() {

    let elemCod = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino", 0));

    if (elemCod !== RIGA_DESCRIZIONE_LIBERA) {

        w_um = parseInt(Get_KendoDDLValue("ddlUM"));

        if (is_Trasf_Veg_Anim_FormProdottoUC()) {

            Set_KendoNumTBValue("idTaraRiscontrata", Get_KendoNumTBValue("idTara"));

            GrigliaImballiImpostaRiscontrati();

            let udm_multipli_g = [enum_Udm.chilogrammi.value, enum_Udm.quintali.value, enum_Udm.tonnellate.value];

            if (udm_multipli_g.includes(w_um)) {

                let ntbKgLordiRisc = KendoNumTB("idKgLordiRiscontrati");
                let kgLordi = Get_KendoNumTBValue("idKgLordi", true);
                if (kgLordi !== 0) {
                    ntbKgLordiRisc.value(kgLordi);
                    ntbKgLordiRisc.enable(true);
                }

                let ntbKgNettiRisc = KendoNumTB("idKgNettiRiscontrati");
                let kgNetti = Get_KendoNumTBValue("idKgNetti", true);
                if (kgNetti !== 0) {
                    ntbKgNettiRisc.value(kgNetti);
                    ntbKgNettiRisc.enable(true);
                }

            }
            else {
                let ntbQuantitaRisc = KendoNumTB("idQuantitaRiscontrata");
                let qta = Get_KendoNumTBValue("idQuantita", true);
                if (qta !== 0) {
                    ntbQuantitaRisc.value(qta);
                    ntbQuantitaRisc.enable(true);
                }
            }

        }
        else {
            let ntbQuantitaRisc = KendoNumTB("idQuantitaRiscontrata");
            let qta = Get_KendoNumTBValue("idQuantita", true);
            if (qta !== 0) {
                ntbQuantitaRisc.value(qta);
                ntbQuantitaRisc.enable(true);
            }
        }
    }
}

function btnResettaPesiRiscontratiClick(ev) {

    if (sonoInModificaImballi) {
        kendo.alert(TraduzioneMultiResx(resxFormProdottoUC, "OccorreConfermareAnnullareModificheImballi", "Occorre prima confermare od annullare le modifiche agli imballi!"));
        return true;
    }

    if (VerificaValorizzazionePesiRiscontrati() === true) {

        let content = "<p>" + TraduzioneMultiResx(resxFormProdottoUC, "ConfermaCancellazionePesiRiscontrati",
            "Continuando, i valori attualmente inseriti nei campi 'riscontrati' verranno cancellati, si desidera proseguire? ") + "</p>";

        $('#formProdottoUC_dialogConfermaGenerica').kendoDialog({
            width: "450px",
            title: TraduzioneMultiResx(resxFormProdottoUC, "Conferma", "Conferma"),
            closable: true,
            modal: true,
            visible: false,
            content: content,
            actions: [
                { text: TraduzioneMultiResx(resxFormProdottoUC, "Si", 'Sì'), action: ResettaPesiRiscontrati },
                { text: TraduzioneMultiResx(resxFormProdottoUC, "No", 'No'), primary: true, action: function () { return true; } }
            ]
        });

        $('#formProdottoUC_dialogConfermaGenerica').data("kendoDialog").open();
    }
    else {
        // Proseguo senza necessità di chiedere conferma
        ResettaPesiRiscontrati();
    }

}


function ResettaPesiRiscontrati() {
    let elemCod = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino", 0));

    if (elemCod !== RIGA_DESCRIZIONE_LIBERA) {

        w_um = parseInt(Get_KendoDDLValue("ddlUM"));

        // Se sono abilitati i moduli F&F e la categoria prodotto è trasformati animali/vegetali allora ho la gestione degli imballaggi e di conseguenza delle tare,
        // altrimenti ho solo l'input di quantità generico
        if (is_Trasf_Veg_Anim_FormProdottoUC()) {

            Set_KendoNumTBValue("idTaraRiscontrata", null);

            GrigliaImballiResettaRiscontrati();

            let udm_multipli_g = [enum_Udm.chilogrammi.value, enum_Udm.quintali.value, enum_Udm.tonnellate.value];

            if (udm_multipli_g.includes(w_um)) {

                let ntbKgLordiRisc = KendoNumTB("idKgLordiRiscontrati");
                ntbKgLordiRisc.value(0);
                ntbKgLordiRisc.enable(false);
                //Set_KendoNumTBValue("idKgLordiRiscontrati", 0);

                let ntbKgNettiRisc = KendoNumTB("idKgNettiRiscontrati");
                ntbKgNettiRisc.value(0);
                ntbKgNettiRisc.enable(false);
                //Set_KendoNumTBValue("idKgNettiRiscontrati", 0);
            }
            else {
                let ntbQuantitaRisc = KendoNumTB("idQuantitaRiscontrata");
                ntbQuantitaRisc.value(0);
                ntbQuantitaRisc.enable(false);
                //Set_KendoNumTBValue("idQuantitaRiscontrata", 0);
            }

        }
        else {
            let ntbQuantitaRisc = KendoNumTB("idQuantitaRiscontrata");
            ntbQuantitaRisc.value(0);
            ntbQuantitaRisc.enable(false);
            //Set_KendoNumTBValue("idQuantitaRiscontrata", 0);
        }
    }
}

function VerificaValorizzazionePesiRiscontrati() {
    let flagModifiche = false;

    let elemCod = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino", 0));

    if (elemCod !== RIGA_DESCRIZIONE_LIBERA) {

        let w_um = parseInt(Get_KendoDDLValue("ddlUM"));

        // Se sono abilitati i moduli F&F e la categoria prodotto è trasformati animali/vegetali allora ho la gestione degli imballaggi e di conseguenza delle tare,
        // altrimenti ho solo l'input di quantità generico
        if (is_Trasf_Veg_Anim_FormProdottoUC()) {

            //Set_KendoNumTBValue("idTaraRiscontrata", null);
            if (Get_KendoNumTBValue("idTaraRiscontrata") !== null) {
                flagModifiche = true;
            }

            let udm_multipli_g = [enum_Udm.chilogrammi.value, enum_Udm.quintali.value, enum_Udm.tonnellate.value];

            if (udm_multipli_g.includes(w_um)) {
                if (Get_KendoNumTBValue("idKgLordiRiscontrati", true) !== 0) {
                    flagModifiche = true;
                }
                if (Get_KendoNumTBValue("idKgNettiRiscontrati", true) !== 0) {
                    flagModifiche = true;
                }
            }
            else {
                if (Get_KendoNumTBValue("idQuantitaRiscontrata", true) !== 0) {
                    flagModifiche = true;
                }
            }

        }
        else {
            //Set_KendoNumTBValue("idQuantitaRiscontrata", Get_KendoNumTBValue("idQuantita", true));
            if (Get_KendoNumTBValue("idQuantitaRiscontrata", true) !== 0) {
                flagModifiche = true;
            }
        }
    }

    return flagModifiche;
}

function ddlCausale_Riga_change() {
    var ddlCausale_Riga = KendoDDL("ddlCausale_Riga");

    if (ddlCausale_Riga.value() === enum_Pendenza.Furto) {
        // abilito il campo per scrivere la denuncia
        $("#panelBar_Denuncia").show();
        ////set_data("idDataDenuncia", formattedDate(new Date(), "/"), null);
    } else {
        $("#panelBar_Denuncia").hide();
        $("#txtNrDenuncia").val("");
        set_data("idDataDenuncia", formattedDate(new Date(1900, 1, 1), "/"), null);
    }
}

function chkParametroQualitativo_change() {
    if (getKendoSwitch("chkParametroQualitativo")) {
        ClearDDL("ddlCalibro");
        let dataSourceCalibro = new kendo.data.DataSource({
            data: [{
                Cal_Des: "",
                Cal_Cod: 0
            }]
        });
        Set_KendoDDLValueNoDef("ddlCalibro", 0);
        ddlCalibro_change();
        KendoDDL("ddlCalibro").enable(false);
    } else {      
        KendoDDL("ddlCalibro").enable(true);
        ddlLottoAccettazione_change();
    }
}

// ------------------------
// --- Fine Change DDL ----
// ------------------------


// -------------------------------
// --- INIZIO Pulizia campi   ----
// -------------------------------

function AzzeraAlCambioCategoriaOProdotto(CorP) {
    $("#BtnInfo_Fito").hide();
    $("#BtnInfo_Concime").hide();

    Set_KendoNumTBValue("idTxt_N", null);
    Set_KendoNumTBValue("idTxt_P2O5", null);
    Set_KendoNumTBValue("idTxt_K2O", null);
    Set_KendoNumTBValue("idTxt_Cu", null);
    Visibilita_Ferti_Dettagli(false, false);

    $("#txtDoseEtichetta").val("");

    // TODO Quando va mostrata dose etichetta?
    Visibilita_DoseEtichetta(false, false);

    Visibilita_TxtBeniStrumentali(false, false);

    Visibilita_DdlProdotto(true, true);

    // Se è cambiata la categoria azzero il prodotto
    if (CorP === "C") {

        if (!is_Trasf_Veg_Anim_FormProdottoUC()) {
            $("#id_parametri_qualitativi_list div").html("");
        }

        if (isImputazioneImpiantiAttiva()) {
            $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ImputazioneImpianti]).attr("style", "display:inline-block");
        }
        else {
            $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ImputazioneImpianti]).attr("style", "display:none");
        }

        if (isRaccolteXConferimentiAttive()) {
            Visibilita_PanelRaccolte(true);
        }
        else {
            if (inizializzaFormDettaglioRiga === false && raccolteXConferimenti_AbilitazioneGenerale() && is_210_FormProdottoUC() && raccolteXConferimenti_dsSelezionate.length === 0) {
                // Nel caso in cui le raccolte non siano visibili, ma siano abilitate e la categoria appena selezionata è compatibile con la gestione,
                // allora la modalità delle raccolte potrebbe non essere ancora stata inizializzata, quindi ne richiamo il costruttore
                let numRaccolte = 0;

                let pivaContattoRaccolte = GetPivaConferente();
                if (pivaContattoRaccolte !== "") {

                    let idMovDetConf = 0;
                    let isOperazioneModifica = riga_originale_entrata_FormProdottoUC !== undefined && riga_originale_entrata_FormProdottoUC !== null ? true : false;
                    if (isOperazioneModifica) {
                        idMovDetConf = riga_originale_entrata_FormProdottoUC.Id_Mov_Det;
                    }

                    numRaccolte = raccolteConfUC_set(pivaContattoRaccolte, KendoDate("inDataEmissione").value(), idMovDetConf);
                }

                if (numRaccolte > 0) {
                    Visibilita_PanelRaccolte(true);
                }
            }
            else {
                raccolteXConferimenti_dsSelezionate = [];
                Visibilita_PanelRaccolte(false);
            }
        }

        CreaDdlProdottoDes();

        // In caso di cambio categoria azzero gli alias e sostanzialmente nascondo la ddl perché non ho un prodotto selezionato
        let kddlProdAlias = KendoDDL("ddlProdAlias");
        if (kddlProdAlias.dataSource.data().length > 0) {
            kddlProdAlias.dataSource.data([]);
        }
        Visibilita_DdlProdAlias(gestioneAlias(), false);

        // Carico griglia con imballaggi se accettazione oppure se uscita prodotto e F&F / Zoo + Trasformati Vegetali / Animali
        // lo faccio qui perchè ci passa sempre sia in modifica che in inserimento(in questo caso la carica vuota)
        if (lavCodAccettazione ||
            is_Trasf_Veg_Anim_FormProdottoUC()) {

            let w_Id_Agenda = 0;
            let w_Id_Mov_Det = 0;
            let w_KeyDet = $('input[name$="hf_key_mov_dett"]').val();
            if (w_KeyDet !== "") {
                w_Id_Agenda = parseInt(w_KeyDet.split("_")[2]);
                w_Id_Mov_Det = parseInt(w_KeyDet.split("_")[4]);
            }

            if (gestitoImballaggio_FF || gestitoContenitore_FF || gestitoConfezione_FF) {
                Ricerca_ImballiFormProdottoUC($(cIdPiva).val(), w_Id_Agenda, w_Id_Mov_Det);
                popola_ImballiFormProdottoUC("tab_imballaggi_formProdottoUC");
            }
        }

        // Resetto i campi che innescano la ricerca dei parametri qualitativi F&F
        current_Elem_Cod = -1;
        current_Veg_Cod = -1;
        current_Cul_Cod = -1;
    }

    if (CorP === "P") {

        if (!inizializzaFormDettaglioRiga) {
            seAzzeraCalCod();
        }

        // Gestisco la ddl degli alias del prodotto
        if (gestioneAlias()) {
            // Gli alias sono contemplati solo in caso di documenti di vendita e di prodotti identificati da mat_cod
            KendoDDL("ddlProdAlias").dataSource.read();
            Visibilita_DdlProdAlias(true, false);
        }
        else {
            // Imposto un valore vuoto manualmente per non far eseguire l'evento dataBound ma avere comunque un valore di default per la funzione di scrittura
            KendoDDL("ddlProdAlias").dataSource.data([{
                    Mat_Cod_Alias: 0,
                    Mat_Des_Alias: "",
                    Filtro_Contatti_String: ""
                }]);

            Visibilita_DdlProdAlias(false, false);
        }

        if (is_Trasf_Veg_Anim_FormProdottoUC()) {
            if (KendoDDL("ddlProdottoDes").dataItem().Prodotto_Cod !== undefined &&
                    KendoDDL("ddlProdottoDes").dataItem().Prodotto_Cod !== 0) {

                //Trasformati vegetali
                if (is_210_FormProdottoUC()) {

                    if (lavCodAccettazionePomodoro)
                        creaParametriQualitativiPomodoro_FF(KendoDDL("ddlProdottoDes").dataItem().Veg_Cod, KendoDDL("ddlProdottoDes").dataItem().Cul_Cod);
                    else {
                        // Cambiata specie o varietà --> Aggiorno i parametri qualitativi
                        if (current_Elem_Cod !== KendoDDL("ddlProdottoDes").dataItem().Elem_Cod ||
                            current_Veg_Cod !== KendoDDL("ddlProdottoDes").dataItem().Veg_Cod ||
                            current_Cul_Cod !== KendoDDL("ddlProdottoDes").dataItem().Cul_Cod) {

                            creaParametriQualitativi_FF_Zoo(KendoDDL("ddlProdottoDes").dataItem().Elem_Cod, KendoDDL("ddlProdottoDes").dataItem().Veg_Cod, KendoDDL("ddlProdottoDes").dataItem().Cul_Cod);  //TODO Solo se F&F o calibro
                        }

                        // Cambiata specie o varietà o regolamento --> Aggiorno il degrado
                        // N.B. al momento questo default è gestito solo per Trasformati Vegetali
                        if (current_Elem_Cod !== KendoDDL("ddlProdottoDes").dataItem().Elem_Cod ||
                            current_Veg_Cod !== KendoDDL("ddlProdottoDes").dataItem().Veg_Cod ||
                            current_Cul_Cod !== KendoDDL("ddlProdottoDes").dataItem().Cul_Cod ||
                            current_Reg_Cod !== KendoDDL("ddlProdottoDes").dataItem().Reg_Cod) {

                            let w_um = 0;
                            if (Get_KendoDDLValue("ddlUM") !== undefined && Get_KendoDDLValue("ddlUM") !== null && Get_KendoDDLValue("ddlUM") !== "")
                                w_um = parseInt(Get_KendoDDLValue("ddlUM"));

                            if (w_um === enum_Udm.chilogrammi.value &&
                                lavCodAccettazione && !lavCodAccettazionePomodoro &&
                                KendoDDL("ddlProdottoDes").dataItem() !== undefined && KendoDDL("ddlProdottoDes").dataItem().Veg_Cod !== undefined) {
                                // Aggiorno il campo degrado prendendolo da tabella
                                let degrado = RicercaDegrado($(cIdPiva).val(), KendoDDL("ddlProdottoDes").dataItem().Veg_Cod, KendoDDL("ddlProdottoDes").dataItem().Cul_Cod, KendoDDL("ddlProdottoDes").dataItem().Reg_Cod, get_data("inDataEmissione"));
                                if (degrado !== 0)
                                    Set_KendoNumTBValue("idDegradoPerc", degrado);
                                else
                                    Set_KendoNumTBValue("idDegradoPerc", null);

                                idDegradoPerc_change();
                            }
                        }
                    }
                }

                //Trasformati animali
                if (is_310_FormProdottoUC()) {
                    //TODO Cerca param qual solo se cambiata specie animale ... da fare quando sarà gestito il filtro sui param qual per specie animale
                    // Cambiata specie animale o razza --> Aggiorno i parametri qualitativi
                    if (current_Elem_Cod !== KendoDDL("ddlProdottoDes").dataItem().Elem_Cod 
                        //|| current_Veg_Cod !== KendoDDL("ddlProdottoDes").dataItem().Veg_Cod ||
                        //current_Cul_Cod !== KendoDDL("ddlProdottoDes").dataItem().Cul_Cod
                    ) {
                        creaParametriQualitativi_FF_Zoo(KendoDDL("ddlProdottoDes").dataItem().Elem_Cod, KendoDDL("ddlProdottoDes").dataItem().Veg_Cod, KendoDDL("ddlProdottoDes").dataItem().Cul_Cod);
                    }
                }

                impostaDefaultParametriQualitativi_FF(); // Non viene eseguita al primo caricamento del form in modifica, per non sovrascrivere i dati inseriti dall'utente
            }
        }
    }

    //impostaVisibilitaFreshAndFood_FormProdottoUC(is_Trasf_Veg_Anim_FormProdottoUC());

    $("#groupLottoImpianto").hide();
    $("#groupAggregaLottoImpianto").hide();
    ClearDDL("ddlLottoImpianto");
    Set_KendoDDLValueNoDef("ddlLottoImpianto", "");  //TODO Valore dft corretto?
    //KendoDDL("ddlLottoImpianto").wrapper.hide();   

    $("#lblAggregaLottoImpianto").hide();
    setKendoSwitchVisible("chkAggregaLottoImpianto", false);

    ImpostaVisibilitaLottoAccettazione();
    $("#txtLottoAccettazione").val("");
    Set_KendoDDLValue("ddlConfezionamentoLotto", "");

    // Nascondo la griglia delle giacenze perchè andrà comunque ricaricata
    $("#btn_scelta_da_giacenza_formProdottoUC").hide();
    $("#tab_grid_scelta_da_giacenza_formProdottoUC").hide();
    $("#titolo_grid_scelta_da_giacenza").hide();
    if (KendoDDL("ddlProdottoDes").dataItem() !== undefined &&
        KendoDDL("ddlProdottoDes").dataItem().Prodotto_Cod !== undefined &&
        KendoDDL("ddlProdottoDes").dataItem().Prodotto_Cod !== 0) {
        // Mostro pulsante per ricerca giacenze
        //if (is_Trasf_Veg_Anim_FormProdottoUC()) {
        switch (cIdLavCod) {
            case enum_LavCod.Ordine_Vendita_Emesso.value:
            case enum_LavCod.DDT_Emesso.value:
            case enum_LavCod.Fattura_Emessa.value:
            case enum_LavCod.Nota_Accredito_Ricevuta.value:
                //$("#id_row_button_scelta_da_giacenza").show();
                break;
            default:
                $("#tab_grid_scelta_da_giacenza_formProdottoUC").html('');
                //$("#id_row_button_scelta_da_giacenza").hide();
                break;
        }
    //} else {
    //    $("#tab_grid_scelta_da_giacenza_formProdottoUC").html('');
    //    $("#id_row_button_scelta_da_giacenza").hide();
    //    $("#id_row_grid_scelta_da_giacenza").hide();
    //    $("#tab_imballaggi_formProdottoUC").html('');
    //}

    }
        
    //TODO
    $("#idParametroQualitativoCalibro").hide();
    $("#lblCalibro").hide();
    ClearDDL("ddlCalibro");
    KendoDDL("ddlCalibro").wrapper.hide();  

    //recupero le info su finescorta
    $("#idInfoSulProdotto").hide();
    $("#lblInfoSulProdotto").hide();
    if (parseInt(Get_KendoDDLValue("ddlCategorieMagazzino")) === FORMULATI && Get_KendoDDLValue("ddlProdottoDes") !== "") {
        var infotxt = FineScorta_e_altreInfo(parseInt(Get_KendoDDLValue("ddlProdottoDes")));
        if (infotxt !== "") {
            $("#lblInfoSulProdotto").val(infotxt);
            $("#idInfoSulProdotto").show();
            $("#lblInfoSulProdotto").show();
        }
    }

    if (CorP === "C") {
        AzzeraQuantita();    
    }
    AzzeraGiacenza();
    AzzeraPrezziScontiImporti();

    if (Qs_CaricoScarico !== CAU_CARICO) {
        if (KendoDDL("ddlLottoAccettazione") !== undefined) {
            ImpostaDdlLottoAccettazione([], null);
        }
    }

}

function AzzeraGiacenza() {
    $('input[name$="txtGiacenzaProvenienza"]').val("");
    $('input[name$="txtGiacenzaDestinazione"]').val("");
}

function AzzeraQuantita() {

    let quantitaDefault = null;

    if (isContrattoAffitto()) {
        quantitaDefault = 1;
    }

    Set_KendoNumTBValue("idTara", null);
    Set_KendoNumTBValue("idTaraRiscontrata", null);
    if (lavCodAccettazionePomodoro || 
        Get_KendoDDLValue("ddlProdottoDes") === undefined ||
        Get_KendoDDLValue("ddlProdottoDes") === null ||
        Get_KendoDDLValue("ddlProdottoDes") === "")
            Set_KendoNumTBValue("idDegradoPerc", null);
    $("#lblDegradoRisultatoCalc").text("0");
    $("#lblKgEffettiviRisultatoCalc").text("0");

    // TODO U.M. e 'TODO U.M.

    if (is_FF_FormProdottoUC()) {

        //Set_KendoNumTBValue("idNrImballaggio", 0);
        //Set_KendoNumTBValue("idTaraImballaggio", 0);
        //Set_KendoNumTBValue("idNrRiscontratiImballaggio", 0);
        //Set_KendoNumTBValue("idNrContenitore", 0);
        //Set_KendoNumTBValue("idTaraContenitore", 0);
        //Set_KendoNumTBValue("idContPerImb", 0);
        //Set_KendoNumTBValue("idNrRiscontratiContenitore", 0);
        //Set_KendoNumTBValue("idNrConfezione", 0);
        //Set_KendoNumTBValue("idTaraConfezione", 0);
        //Set_KendoNumTBValue("idConfPerCont", 0);
        //Set_KendoNumTBValue("idNrRiscontratiConfezione", 0);

        let pesoImballiEntratiVuoti = kendo.parseFloat(Get_KendoNumTBValue("inImballiVuotiRiepilogo"));
        let taraVeicolo = kendo.parseFloat(Get_KendoNumTBValue("inTaraVeicoloRiepilogo"));
        let pesoLordoVeicolo = kendo.parseInt(Get_KendoNumTBValue("inPesoTotaleRiepilogo"));
        let pesoLordoProdotti = kendo.parseFloat(Get_KendoNumTBValue("inPesoLordoRiepilogo"));
        let w_diff_pesi = pesoLordoVeicolo - taraVeicolo - pesoLordoProdotti - pesoImballiEntratiVuoti;

        if (is_Trasf_Veg_Anim_FormProdottoUC() && w_diff_pesi > 0) {

            Set_KendoNumTBValue("idKgLordi", w_diff_pesi);
            Set_KendoNumTBValue("idKgNetti", w_diff_pesi);

            // TODO - sottrarre anche tara totale imballi vuoti

        } else {

            // Cambiato per non proporre a zero il nr
            Set_KendoNumTBValue("idKgLordi", null);
            Set_KendoNumTBValue("idKgNetti", null);
            
        }
           
        Set_KendoNumTBValue("idKgLordiRiscontrati", null);
        Set_KendoNumTBValue("idKgNettiRiscontrati", null);

        //let gridImb = $("#tab_imballaggi_formProdottoUC").data("kendoGrid");
        //if (gridImb !== undefined) {
        //    let dataSourceImb = gridImb.dataSource;
        //    dataSourceImb.data([]);
        //    dataSourceImb.sync();
        //    kendo_AggiustaDimensioneColonne("#tab_imballaggi_formProdottoUC");
        //} 

        let tare_da_griglia = CalcolaTaraTotaleImballi();
        if (tare_da_griglia.taraTotale > 0) {
            Set_KendoNumTBValue("idTara", tare_da_griglia.taraTotale);
            Set_KendoNumTBValue("idTaraRiscontrata", tare_da_griglia.taraTotaleRiscontrata);
        } else {
            Set_KendoNumTBValue("idTara", null);
            Set_KendoNumTBValue("idTaraRiscontrata", null);
        }

        Set_KendoNumTBValue("idQuantita", quantitaDefault);
        Set_KendoNumTBValue("idQuantitaRiscontrata", null);

    } else {

        Set_KendoNumTBValue("idQuantita", quantitaDefault);
        Set_KendoNumTBValue("idQuantitaRiscontrata", null);        

    }
    
}

function AzzeraPrezziScontiImporti() {

    let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
    
    // TODO Controllare che ci siano tutti i campi da gestire
    Set_KendoNumTBValue("idPrezzo", null);
    
    if (ddlCategorieMagazzinoValue !== 0 && ddlCategorieMagazzinoValue !== RIGA_DESCRIZIONE_LIBERA && is_Trasf_Veg_Anim_FormProdottoUC())
        Set_KendoDDLValueNoDef("ddlPrezzoRiferitoA", -1);
    else
        Set_KendoDDLValueNoDef("ddlPrezzoRiferitoA", 0);
    Set_KendoNumTBValue("idPrezzoNetto", 0);
    Set_KendoNumTBValue("idScontoBase", null);
    Set_KendoNumTBValue("idScontoAddiz1", null);
    Set_KendoNumTBValue("idScontoAddiz2", null);
    Set_KendoNumTBValue("idScontoAddiz3", null);
    Set_KendoNumTBValue("idScontoCalcolato", 0); 
    Set_KendoNumTBValue("idScontoCalcolatoEuro", 0); 
    Set_KendoNumTBValue("idImponibileTotale", null);
    Set_KendoNumTBValue("idImponibileTotaleNetto", null);
    Set_KendoDDLValueNoDef("ddlCodIva", -1);
    Set_KendoNumTBValue("idIva", null);

    setKendoSwitch("chkForzaIva", false);
    KendoNumTB("idIva").enable(false);
    
    Set_KendoNumTBValue("idImportoUnitario", null);
    Set_KendoNumTBValue("idImportoTotale", null);
    Set_KendoDDLValueNoDef("ddlValoreRiferimento", 0);
    Set_KendoDDLValueNoDef("ddlScontoModalita", 0);
    Set_KendoDDLValueNoDef("ddlScontoMagg", 0);
    document.getElementById("idPrezzo").disabled = false;
    document.getElementById("idImportoUnitario").disabled = true;
    document.getElementById("idImportoTotale").disabled = true;
    document.getElementById("idImponibileTotale").disabled = true;
    document.getElementById("idImponibileTotaleNetto").disabled = true;
}

// -------------------------------
// --- FINE Pulizia campi   ----
// -------------------------------


function Recupera_ChiaveMagazzino() {
    xTipoDestinazione = 0;
    xSa_Cod = 0;
    xFabbricato_Cod = 0;
    if (Get_KendoDDLValue("ddlUbicProvenienza") !== "" && (Qs_CaricoScarico === CAU_SCARICO || Qs_CaricoScarico === CAU_TRASFERIMENTO)) {
        xTipoDestinazione = parseInt(Get_KendoDDLValue("ddlUbicProvenienza").split("_")[0]);
        xSa_Cod = parseInt(Get_KendoDDLValue("ddlUbicProvenienza").split("_")[1]);
        xFabbricato_Cod = parseInt(Get_KendoDDLValue("ddlUbicProvenienza").split("_")[2]);
    }
    else if (Get_KendoDDLValue("ddlUbicDestinazione") !== "" && Qs_CaricoScarico === CAU_CARICO) {
        xTipoDestinazione = parseInt(Get_KendoDDLValue("ddlUbicDestinazione").split("_")[0]);
        xSa_Cod = parseInt(Get_KendoDDLValue("ddlUbicDestinazione").split("_")[1]);
        xFabbricato_Cod = parseInt(Get_KendoDDLValue("ddlUbicDestinazione").split("_")[2]);
    }
}


//Aggiunta lotto
function aggiungiNuovoLotto(widgetId, value) {
    var widget = $("#" + widgetId).getKendoDropDownList();
    var dataSource = widget.dataSource;

    dataSource.add({
        Lotto_Cod: value,
        Lotto: value
    });

    dataSource.one("sync", function () {
        widget.select(dataSource.view().length - 1);
    });

    dataSource.sync();
    dataSource.sort({ field: "Lotto", dir: "asc" });
     
}

function ApriEditProdotto() {
    CostruisciLinkNuovoProdotto();
    ApriWindowNuovoProdotto();
}

function ApriWindowNuovoProdotto() {
    let url = $('input[name$="hf_LinkEditProdotto"]').val() + "&win=1";
    $(document.body).append('<div id="windowNuovoProdotto" style="padding: 10"></div>');
    $('#windowNuovoProdotto').kendoWindow({
        title: TraduzioneMultiResx(resxFormProdottoUC, "CreazioneNuovoProdotto", "Creazione Nuovo Prodotto"),
        modal: true,
        resizable: true,
        visible: false,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url, // "../Anagrafica/Prodotto_Edit.aspx?p=" + getParameterByName('p'),
        actions: ["Maximize", "Close"],
        close: function () {
            setTimeout(function () { $('#windowNuovoProdotto').kendoWindow('destroy'); }, 200);
        }
    }).data('kendoWindow').center().open();
}

function InfoProfitosan() {
 
    let stringa = Get_KendoDDLValue("ddlProdottoDes");
    if (stringa === undefined || stringa === null) {
        return false;
    }
    if (stringa.toString().length > 0) {
        var splitted = stringa.split("£");
        let pro_cod = splitted[0];
        let targetUrl = $("#" + hf_indirizzoProfitosan).val();
        
        window.open(targetUrl + pro_cod, "profitosan", "");
    }
}


function InfoFertilizzante() {

    var w_InfoFertilizzante = "";
    let stringa = Get_KendoDDLValue("ddlProdottoDes");
    if (stringa === undefined || stringa === null) {
        return false;
    }
    if (stringa.toString().length > 0) {
        var splitted = stringa.split("£");
        let pro_cod = splitted[0];

        w_InfoFertilizzante = CostruisciInfoFertilizzante(pro_cod);

        $("#infoFertilizzanteDialog").kendoDialog({
            width: "600px",
            height: "400px",
            title: TraduzioneMultiResx(resxFormProdottoUC, "InformazioniSulFertilizzante", "Informazioni sul fertilizzante"),  //TODO
            closable: true,
            modal: true,
            visible: false,
            content: w_InfoFertilizzante,
            messages: {
                close: TraduzioneMultiResx(resxFormProdottoUC, "Chiudi", "Chiudi")
            },
            actions: [{
                text: TraduzioneMultiResx(resxFormProdottoUC, "Chiudi", "Chiudi")
            }]
        });

        $("#infoFertilizzanteDialog").data("kendoDialog").open();

    }
}

function CaricaIvaContiDefault() {
    
    let elemCod = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino", 0));

    if (elemCod !== RIGA_DESCRIZIONE_LIBERA) {

        let piva = $(cIdPiva).val();
        let data_movimento = kendo.parseDate($("#inDataEmissione").val());
        let cod_risum = parseInt(Get_KendoDDLValue(ddlContatto1Nome, 0));

        let proCod = 0;
        let matCod = 0;
        let codProdotto = parseInt(Get_KendoDDLValue("ddlProdottoDes"));
        if (codProdotto < 0) {
            matCod = Math.abs(codProdotto);
        } else {
            proCod = codProdotto;
        }

        IvaConti(piva, data_movimento, cod_risum, elemCod, proCod, matCod, Qs_CaricoScarico);
    }
}

function DisabilitaSalvataggio() {
    //$("#btnSalvaTestataPiuRiga").hide();
    //$("#btnSalvaTestataPiuRiga2").hide();
    MostraBtnAnnullaRiga(false);
}

function DisabilitaSalvataggioErrore() {
    $("#btnSalvaTestataPiuRiga").hide();
    $("#btnSalvaTestataPiuRiga2").hide();

    $("#btnSalvaEsciDoc").hide();
    $("#btnSalvaEsciDoc2").hide();

    $("#btnSalvaENuovoDoc").hide();
    $("#btnSalvaENuovoDoc2").hide();
}

function AbilitaSalvataggio() {
    VisualizzaPulsantiSalvataggio(true);
    MostraBtnAnnullaRiga(true);
}

function getPendente(elem_cod) {
    let Pendente = 0;
     
    switch (elem_cod) {
        case SERVIZI:
        case ALTRI_BENI:
            if (hf_PendenzaIniziale === enum_Pendenza.DocBolla || hf_PendenzaIniziale === enum_Pendenza.DocFattura)
                Pendente = hf_PendenzaIniziale;
            else
                Pendente = enum_Pendenza.MovESENTE;
            break;

        default:

            switch (lavCodFattura) {
                case true:

                    Pendente = elencoCausali_Riga[0].KeyCausale;
                    break;

                default:

                    if (Get_KendoDDLValue("ddlCausale_Riga") === "")
                        Pendente = hf_PendenzaIniziale;
                    else
                        Pendente = parseInt(Get_KendoDDLValue("ddlCausale_Riga"));
                    break;
            }

    }

    return Pendente;
}

function getDatiContabRiga() {
    let data = $('input[name$="hdKendo_RigaDoc"]').val();
    let jSonParsed = null;
    if (data !== null && data !== undefined && data !== "") {
        jSonParsed = JSON.parse(data);
    }
    return jSonParsed;
}

function btnCalcolaLotto_click() {
    let w_gest_lotti = getGestioneLotti();
    let lotto = Impostazione_LottoProdotto(w_gest_lotti);
    $("#txtLottoAccettazione").val(lotto);
}

function AggiornaGHG() {
    const piva = $(cIdPiva).val();
    const matCod = Math.abs(parseInt(Get_KendoDDLValue("ddlProdottoDes")));
    const elemCod = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino", 0));
    const data = KendoDate("inDataEmissione").value();

    let arrRaccolte = raccolteConfUC_ottieniRigheSelezionate().map((racc) => ({ partitaIva: racc.Piva, saCod: racc.Sa_Cod, agenda: racc.Id_Agenda }));
    let ris = OttieniParamsQualGHGAjax(arrRaccolte)
    if (ris == undefined) {
        console.error("L'ottenimento dei parametri qualitativi del GHG è fallito.");
        return;
    }
    let risEEC = OttieniEECAjax(piva, elemCod, matCod, data);
    if (risEEC == undefined) {
        console.error("L'ottenimento dell'EEC per il GHG è fallito.");
        return;
    }
    ris["FF_ghgforec_Val_Cod"] = risEEC
    ImpostaParametriQualitativi(ris);
    AggiornaGHGTotal();
}

function btnAggiornaGHG_click() {
    AggiornaGHG();
}

function btnCaricaGiacenzeFF_click() {
    // Per ora le ricarico sempre così se nel frattempo sono state
    // fatte altre operazioni in parallelo c'è la situazione aggiornata
    //$("#id_row_grid_scelta_da_giacenza").show();
    CaricaGiacenze(null);
}

function AggiungiValoriParametriQualitativi(objCampi) {
    if (is_Trasf_Veg_Anim_FormProdottoUC()) {
        
        if (paramQual_FF_filtrospevar !== undefined && paramQual_FF_filtrospevar !== null && paramQual_FF_filtrospevar.length !== 0) {

            let arrParams = [];

            for (let iPar = 0; iPar < paramQual_FF_filtrospevar.length; iPar++) {
                if (paramQual_FF_filtrospevar[iPar].Tabella_ID !== 0) {
                    if (paramQual_FF_filtrospevar[iPar].Tabella_ID !== "4" &&
                        paramQual_FF_filtrospevar[iPar].Tabella_ID !== "5" &&
                        paramQual_FF_filtrospevar[iPar].Tabella_ID !== "8") {

                        let oTipo = "o" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des;

                        let tipoCod = 0;
                        let valCod = 0;

                        // Tipo 3 sono i parametri a libera imputazione numerici
                        // Tipo 4 sono i parametri a libera imputazione stringa
                        // Tipo 5 sono i parametri a libera imputazione data
                        switch (paramQual_FF_filtrospevar[iPar].Tipo) {
                            case 3:
                                valCod = Get_KendoNumTBValue("txt" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des);
                                break;

                            case 4:
                                var nomeCampoStr = "txtStr" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des;
                                valCod = $('input[name$=' + nomeCampoStr + ']').val();
                                break;

                            case 5:
                                valCod = get_data("date" + paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des);
                                break;

                            default:
                                let TipoParametroQualitativo = paramQual_FF_filtrospevar[iPar].Tabella_Cod_Des;
                                let idControlloKendo = "ddl" + TipoParametroQualitativo;
                                if (KendoDDL(idControlloKendo) !== undefined) {
                                    tipoCod = Get_KendoDDLValue(idControlloKendo);
                                    if (!isNumeric(tipoCod)) {
                                        let messaggio = TraduzioneMultiResx(resxFormProdottoUC, "ParametroQualitativoNonTrovato", "Parametro qualitativo non trovato") + " (" + TipoParametroQualitativo + ")";
                                        MessaggioErrore_Bootstrap(messaggio, "DIV_Messaggi");
                                        throw new Error(messaggio);
                                    }
                                }
                                break;
                        }

                        //verifico se è già presente nell'array, sennò creo con oTipo + tipo_cod
                        let elems = arrayLookup(arrParams, "Tabella_Nome", oTipo);

                        //l'oggetto è di fatto ritornato per riferimento, quindi le modifiche fatte qui non sono su una copia, ma sull'elemento stesso
                        let wValCod = null;
                        switch (paramQual_FF_filtrospevar[iPar].Tipo) {
                            case 1:
                                wValCod = kendo.parseFloat(valCod);
                                break;
                            case 3:
                                wValCod = kendo.parseFloat(valCod);
                                break;
                            case 4:
                                wValCod = valCod;
                                break;
                            case 5:
                                wValCod = valCod;
                                break;
                        }
                        if (elems !== undefined && elems !== null) {
                            elems.Tipo_Cod = parseInt(tipoCod);
                            elems.Val_Cod = wValCod;
                        } else {
                            let res = {
                                Progressivo: parseInt($('input[name$="hf_Cal_Cod"]').val()),
                                Tipo: oTipo,
                                Tipo_Cod: parseInt(tipoCod),
                                Val_Cod: wValCod,
                                ChkTara_Campionatura: 0,
                                Tara_Campionatura: 0
                            };
                            arrParams.push(res);
                        }


                        //dataItem.forEach(function (valore, campo) {

                        //    if (campo === "FF_" + paramQualGestiti[ipar].Tabella_Cod_Des + "_Tipo_Cod") {
                        //        Get_KendoDDLValue("ddl" + paramQualGestiti[ipar].Tabella_Cod_Des);
                        //    }
                        //});
                    }
                }
            }

            if (arrParams.length > 0) {

                //devo verificare che non mi sia venuto qualche elemento sgaffo, con tara ma non tipo_cod
                //(è un caso che non dovrebbe mai verificarsi), se è presente lo rimuovo
                //for (var i = 0, len = arrParams.length; i < len; i++)
                //    if (arrParams[i].Tipo_Cod === undefined) arrParams.splice(i, 1);

                objCampi.MateriePrimeCampionature = arrParams; //kendoEscapeOggetto(arrParams);
            }

        }
    }
}


function AggiungiValoriGHG_Registrazioni(objCampi) {

    if (parametri_indici_creati == true) {

        let arrParams = [];

        if (paramQual_FF_indici_GHG !== undefined && paramQual_FF_indici_GHG !== null && paramQual_FF_indici_GHG.length !== 0) {


            for (var iindice = 0; iindice < paramQual_FF_indici_GHG.length; iindice++) {
                if (paramQual_FF_indici_GHG[iindice].TipoIndice == "GHG" && paramQual_FF_indici_GHG[iindice].ID_Indice !== 0) {

                    if (paramQual_FF_indici_GHG[iindice].TipoCampo == 0 ||
                        paramQual_FF_indici_GHG[iindice].TipoCampo == 1 ||
                        paramQual_FF_indici_GHG[iindice].TipoCampo == 2) {

                        var chiave3 = "GHG" + "_" + paramQual_FF_indici_GHG[iindice].TipoCampo + "_" + paramQual_FF_indici_GHG[iindice].ID_Indice;

                        let vID_Indice = parseInt(paramQual_FF_indici_GHG[iindice].ID_Indice);
                        let vTipoDato = paramQual_FF_indici_GHG[iindice].TipoDato;
                        let vNome_Campo = paramQual_FF_indici_GHG[iindice].Nome_Campo;
                        let vID_Indice_Det = 0;
                        let vElenco_Val = 0;
                        let vValore_Des = "";
                        let vTipoCampo = paramQual_FF_indici_GHG[iindice].TipoCampo;


                        switch (parseInt(paramQual_FF_indici_GHG[iindice].TipoCampo)) {

                            case 0: //Valore Libero

                                switch (paramQual_FF_indici_GHG[iindice].TipoDato) {

                                    case "boolean":

                                        vValore_Des = getKendoSwitch("chk" + chiave3)
                                        break;

                                    default:

                                        vValore_Des = $('input[name$="txt' + chiave3 + '"]').val();
                                        break;
                                }

                                break;

                            case 1: //Dettaglio

                                vID_Indice_Det = Get_KendoDDLValue("ddl" + chiave3);
                                break;

                            case 2: //Elenco

                                vElenco_Val = Get_KendoDDLValue("ddl" + chiave3);
                                break;
                        }


                        //verifico se è già presente nell'array
                        let elems = arrayLookup(arrParams, "ID_Indice", chiave3);

                        //l'oggetto è di fatto ritornato per riferimento, quindi le modifiche fatte qui non sono su una copia, ma sull'elemento stesso
                        if (elems !== undefined && elems !== null) {

                            elems.ID_Indice = vID_Indice;
                            elems.ID_Indice_Det = vID_Indice_Det;
                            elems.Elenco_Val = vElenco_Val;
                            elems.Valore_Des = vValore_Des;
                            elems.TipoCampo = vTipoCampo;
                            elems.TipoDato = vTipoDato;
                            elems.Nome_Campo = vNome_Campo;

                        } else {
                            let res = {
                                ID_Indice: vID_Indice,
                                ID_Indice_Det: vID_Indice_Det,
                                Elenco_Val: vElenco_Val,
                                Valore_Des: vValore_Des,
                                TipoCampo: vTipoCampo,
                                TipoDato: vTipoDato,
                                Nome_Campo: vNome_Campo

                            };
                            arrParams.push(res);
                        }
                    }
                }
            }


            if (arrParams.length > 0) {
           
                objCampi.GHG_Registrazioni = arrParams; //kendoEscapeOggetto(arrParams);
            }
        }
    }

}








function getGestioneLotti() {

    if (isContrattoAffitto()) {
        return enum_Gestione_Lotti_Nessuna;
    }

    let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
    let w_gest_lotti = enum_Gestione_Lotti_Nessuna;
     
    if (ddlCategorieMagazzinoValue !== 0 && impostazioni_Categorie_Lotti !== undefined && impostazioni_Categorie_Lotti !== null && impostazioni_Categorie_Lotti.length > 0) {

        let itemCat = arrayLookup(impostazioni_Categorie_Lotti, "Elem_Cod", ddlCategorieMagazzinoValue);
        if (itemCat !== undefined && itemCat !== null) {
            w_gest_lotti = itemCat.Impostazione_Valore;
        }

        //avevo impostato il lotto come obbligatorio in questa categorie, ma visto che mi trovo in un ordine, sovrascrivo l'impostazione per dire che è facoltativo
        if (lavCodOrdine === true && w_gest_lotti === enum_Gestione_Lotti_Obbligatoria) {
            w_gest_lotti = enum_Gestione_Lotti_Facoltativa;
        }
    }

    return w_gest_lotti;
}

function getGestioneGiacenza() {

    let w_gest_giacenza = enum_Gestione_Giacenze_TuttiProdotti;
    if (impostazioni_Blocca_SottoGiacenza === true) {
        w_gest_giacenza = enum_Gestione_Giacenze_SoloPresenti;
    } else {
        let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

        if (ddlCategorieMagazzinoValue !== 0 && impostazioni_Categorie_Giacenza !== undefined && impostazioni_Categorie_Giacenza !== null && impostazioni_Categorie_Giacenza.length > 0) {

            let itemCat = arrayLookup(impostazioni_Categorie_Giacenza, "Elem_Cod", ddlCategorieMagazzinoValue);
            if (itemCat !== undefined && itemCat !== null) {
                w_gest_giacenza = itemCat.Impostazione_Valore;
            }
        }
    }

    //trattandosi di ordine, a prescindere dall'impostazione salvata, considero che si possa andare sotto giacenza
    if (lavCodOrdine === true) {
        w_gest_giacenza = enum_Gestione_Giacenze_TuttiProdotti;
    }
    
    return w_gest_giacenza;
}

function getAliquotaIva(defaultZero) {
    let aliquota = undefined;
    if (defaultZero) {
        aliquota = 0;
    } else {
        aliquota = null;
    }

    let ddlCodIva = KendoDDL("ddlCodIva");
    if (ddlCodIva !== undefined && ddlCodIva !== null &&
        ddlCodIva.dataItem() !== null && ddlCodIva.dataItem() !== undefined) {
        aliquota = kendo.parseFloat(ddlCodIva.dataItem().Aliquota_IVA);
    }

    return aliquota;
}
 
function popolaOrdiniFormProdottoUC(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: kRead_OrdiniFormProdottoUC_rows,
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };
    var idModel = "key_mov_dett";
    var campiKendoModel = kRead_OrdiniFormProdottoUC_mod();
    var colonneKendoGrid = kRead_OrdiniFormProdottoUC_col();
    var parametriPerLettura = null;
    var parametriDataSource = {
        pagesize: 50,
        aggregate: [
            { field: "Ordine_Det", aggregate: "max" },
            { field: "NrImballaggi", aggregate: "sum" },  // TODO Questo e altri solo se gestiti
            { field: "NrContenitori", aggregate: "sum" },
            { field: "NrConfezioni", aggregate: "sum" },
            { field: "KgLordi", aggregate: "sum" },
            { field: "KgNetti", aggregate: "sum" },
            { field: "Tara", aggregate: "sum" },
            { field: "Qta", aggregate: "sum" },
            { field: "Quantita_Escluso_Degrado", aggregate: "sum" },
            { field: "Num_Imballi_Riscontrati", aggregate: "sum" },
            { field: "Num_Colli_Riscontrati", aggregate: "sum" },
            { field: "Num_Conf_Riscontrate", aggregate: "sum" },
            { field: "Peso_Lordo_Riscontrato", aggregate: "sum" },
            { field: "Peso_Netto_Riscontrato", aggregate: "sum" },
            { field: "Tara_Totale_Riscontrata", aggregate: "sum" },
            { field: "Importo_Totale", aggregate: "sum" },
            { field: "Importo_Unitario", aggregate: "sum" },
            { field: "Iva", aggregate: "sum" },
            { field: "Imponibile", aggregate: "sum" },
            { field: "Imponibile_Netto", aggregate: "sum" }
        ]
    };

    // Se l'utente non è abilitato in modifica non mostro il pulsante per aggiungere la riga
    if (UteAbilitatoInsMod) {
        colonneKendoGrid.unshift(
            {
                command: [{ iconClass: "fa fa-arrow-down fa-lg", className: "blockInsert", name: "aggiungi", text: "", click: aggiungiRiga_OrdiniFormProdottoUCKendoGrid }],
                title: TraduzioneMultiResx(resxFormProdottoUC, "Operazioni", "Operazioni"),
                width: "85px"
            }
        );
    }

    var parametriKendoGrid = {
        excel: false, pdf: false, groupable: false, pageable: true,
        //pageable: { pageSizes: [5, 10, 20, 50, 100] },
        editable: false,
        // colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true,
        scrollable: true
    };

    var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundRighe_OrdiniFormProdottoUC };
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
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

function onDataBoundRighe_OrdiniFormProdottoUC(e) {
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    //kendo_AggiustaDimensioneColonne("#" + gridId);
}

function kRead_OrdiniFormProdottoUC_rows(options) {
    var data = $('input[name$="hdKendo_Ordini_formProdottoUC"]').val();
    var jSonParsed_Kendo = JSON.parse(data);
    options.success(jSonParsed_Kendo.kendo_rows);
}

function kRead_OrdiniFormProdottoUC_mod() {
    var data = $('input[name$="hdKendo_Ordini_formProdottoUC"]').val();
    var jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_model;
}

function kRead_OrdiniFormProdottoUC_col() {
    var data = $('input[name$="hdKendo_Ordini_formProdottoUC"]').val();
    var jSonParsed_Kendo = JSON.parse(data);
    return jSonParsed_Kendo.kendo_columns;
}

function aggiungiRiga_OrdiniFormProdottoUCKendoGrid(e) {

    var grid = $("#tab_ordini_cliente_formProdottoUC").data("kendoGrid");
    var row = $(e.target).closest("tr");
    var dataItem = grid.dataItem(row);

    let collegaOrdiniInviati = parseInt(GetPropertyFromJson($(cIdOpzioniContab).val(), "Collega_Solo_Ordini_Inviati"));

    if (collegaOrdiniInviati === 1 && dataItem.Pratica_Stato_Cod !== 160) {
        $("<div></div>").kendoAlert({
            content: "Non è possibile collegare questa riga d'ordine in quanto non è in stato 'inviato con successo'. Il suo stato attuale è '" + dataItem.Pratica_Stato_Des + "'",
            title: "Attenzione!"
        }).data("kendoAlert").open();
        return;
    }

    // forzo la quantita con quella residua
    if (dataItem.Qta_Residua != null) {
        dataItem.Qta = dataItem.Qta_Residua;

        let udm_multipli_g = [enum_Udm.chilogrammi.value, enum_Udm.quintali.value, enum_Udm.tonnellate.value];
        let udm = parseInt(dataItem.Udm_Cod);

        if (udm_multipli_g.includes(udm) && is_Trasf_Veg_Anim_FormProdottoUC()) {
            dataItem.KgNetti = dataItem.Qta_Residua;

            let tara = dataItem.Tara;

            switch (udm) {
                case enum_Udm.quintali.value:
                    tara = tara / 100;
                    break;
                case enum_Udm.tonnellate.value:
                    tara = tara / 1000;
            }

            dataItem.KgLordi = dataItem.KgNetti + tara;
        }
    }

    // ImpostaVisibilitaElencoOSingolaRiga(true);
    ImpostaCampiFormProdotto(dataItem, enum_TipoOperazioneDB.Copia.value, true);
    if (gestitoImballaggio_FF || gestitoContenitore_FF || gestitoConfezione_FF) {
        Ricerca_ImballiFormProdottoUC($(cIdPiva).val(), dataItem.Id_Agenda, dataItem.Id_Mov_Det);
        popola_ImballiFormProdottoUC("tab_imballaggi_formProdottoUC");
    }

    //rifMovDettaglio = {
    let locRifMovDettaglio = {
        Piva_Rif: dataItem.Piva,
        Sa_Cod_Rif: dataItem.Sa_Cod,
        Id_Agenda_Rif: dataItem.Id_Agenda,
        Id_Mov_Rif: dataItem.Id_Mov,
        Id_Mov_Det_Rif: dataItem.Id_Mov_Det,
        Lav_Cod_Rif: dataItem.Lav_Cod,
        Cau_Mov_Rif: dataItem.Cau_Mov,
        Qta: dataItem.Qta,
        Preserva_Legame: 0,
        Tipo_Associazione: 0
    };

    listRifMovDettaglio.push(locRifMovDettaglio);

    Imposta_Visibilita_FormProdottoUC(enum_TipoOperazioneDB.Copia.value);
    validatorTabDettaglio = inizializzaKendoValidator("tabDettagliDoc", false, false, true, true);
    //$("#panelbarFormProdottoUC").data("kendoPanelBar").collapse($("#panelBar_OrdiniCliente"));
    //$("#panelBar_OrdiniCliente").hide();
    // grid.removeRow(row);
    Visibilita_PanelRaccolte(false);
}

function popolaDDTFormProdottoUC(IDControllo) {

    var UteAbilitatoInsMod = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";
    var UteAbilitatoCanc = $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True";

    var funzioniCRUD = {
        funzioneRead: function (options) {

            var data = $('input[name$="hdKendo_DDT_formProdottoUC"]').val();
            var jSonParsed_Kendo = JSON.parse(data);
            options.success(jSonParsed_Kendo.kendo_rows);

        },
        UtenteAbilitatoInserimentoModifica: UteAbilitatoInsMod,
        UtenteAbilitatoCancellazione: UteAbilitatoCanc
    };
    var idModel = "key_mov_dett";

    // La lettura è già stata effettuata ed il modello delle colonne è impostato nel campo nascosto
    var data = $('input[name$="hdKendo_DDT_formProdottoUC"]').val();
    var jSonParsed_Kendo = JSON.parse(data);

    var campiKendoModel = jSonParsed_Kendo.kendo_model;
    var colonneKendoGrid = jSonParsed_Kendo.kendo_columns;

    var parametriPerLettura = null;
    var parametriDataSource = {
        pagesize: 50,
        aggregate: [
            { field: "Ordine_Det", aggregate: "max" },
            { field: "NrImballaggi", aggregate: "sum" },  // TODO Questo e altri solo se gestiti
            { field: "NrContenitori", aggregate: "sum" },
            { field: "NrConfezioni", aggregate: "sum" },
            { field: "KgLordi", aggregate: "sum" },
            { field: "KgNetti", aggregate: "sum" },
            { field: "Tara", aggregate: "sum" },
            { field: "Qta", aggregate: "sum" },
            { field: "Quantita_Escluso_Degrado", aggregate: "sum" },
            { field: "Num_Imballi_Riscontrati", aggregate: "sum" },
            { field: "Num_Colli_Riscontrati", aggregate: "sum" },
            { field: "Num_Conf_Riscontrate", aggregate: "sum" },
            { field: "Peso_Lordo_Riscontrato", aggregate: "sum" },
            { field: "Peso_Netto_Riscontrato", aggregate: "sum" },
            { field: "Tara_Totale_Riscontrata", aggregate: "sum" },
            { field: "Importo_Totale", aggregate: "sum" },
            { field: "Importo_Unitario", aggregate: "sum" },
            { field: "Iva", aggregate: "sum" },
            { field: "Imponibile", aggregate: "sum" },
            { field: "Imponibile_Netto", aggregate: "sum" }
        ]
    };

    // Se l'utente non è abilitato in modifica non mostro il pulsante per aggiungere la riga
    if (UteAbilitatoInsMod) {
        colonneKendoGrid.unshift(
            {
                command: [{ iconClass: "fa fa-arrow-down fa-lg", className: "blockInsert", name: "aggiungi", text: "", click: aggiungiRiga_DDTFormProdottoUCKendoGrid }],
                title: TraduzioneMultiResx(resxFormProdottoUC, "Operazioni", "Operazioni"),
                width: "85px"
            }
        );
    }

    var parametriKendoGrid = {
        excel: false, pdf: false, groupable: false, pageable: true,
        //pageable: { pageSizes: [5, 10, 20, 50, 100] },
        editable: false,
        // colonneCustomKendoGrid: colCustKendoGrid,
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        columnMenu: true,
        reorderable: true,
        scrollable: true
    };

    //var funzioniPrimaDopoEventi = { funzioneDaChiamareDopoDataBound: onDataBoundRighe_DDTFormProdottoUC };
    var funzioniPrimaDopoEventi = {};
    var mostraRigheCancellate = false;
    var colonneDisabilitateSoloInModifica = null;

    creaKendoGrid(IDControllo, // rappresenta l'ID del div a cui si associa la griglia
        funzioniCRUD,  //funzioni js da chiamare per read, insert, update, delete
        idModel, // chiave riga 
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

function aggiungiRiga_DDTFormProdottoUCKendoGrid(e) {

    var grid = $("#tab_ddt_cliente_formProdottoUC").data("kendoGrid");
    var row = $(e.target).closest("tr");
    var dataItem = grid.dataItem(row);

    // forzo la quantita con quella residua
    if (dataItem.Qta_Residua != null) {
        dataItem.Qta = dataItem.Qta_Residua;

        let udm_multipli_g = [enum_Udm.chilogrammi.value, enum_Udm.quintali.value, enum_Udm.tonnellate.value];
        let udm = parseInt(dataItem.Udm_Cod);

        if (udm_multipli_g.includes(udm) && is_Trasf_Veg_Anim_FormProdottoUC()) {
            dataItem.KgNetti = dataItem.Qta_Residua;

            let tara = dataItem.Tara;

            switch (udm) {
                case enum_Udm.quintali.value:
                    tara = tara / 100;
                    break;
                case enum_Udm.tonnellate.value:
                    tara = tara / 1000;
            }

            dataItem.KgLordi = dataItem.KgNetti + tara;
        }
    }

    // ImpostaVisibilitaElencoOSingolaRiga(true);
    ImpostaCampiFormProdotto(dataItem, enum_TipoOperazioneDB.Copia.value, true);
    if (gestitoImballaggio_FF || gestitoContenitore_FF || gestitoConfezione_FF) {
        Ricerca_ImballiFormProdottoUC($(cIdPiva).val(), dataItem.Id_Agenda, dataItem.Id_Mov_Det);
        popola_ImballiFormProdottoUC("tab_imballaggi_formProdottoUC");
    }

    //rifMovDettaglio = {
    let locRifMovDettaglio = {
        Piva_Rif: dataItem.Piva,
        Sa_Cod_Rif: dataItem.Sa_Cod,
        Id_Agenda_Rif: dataItem.Id_Agenda,
        Id_Mov_Rif: dataItem.Id_Mov,
        Id_Mov_Det_Rif: dataItem.Id_Mov_Det,
        Lav_Cod_Rif: dataItem.Lav_Cod,
        Cau_Mov_Rif: dataItem.Cau_Mov,
        Qta: dataItem.Qta,
        Preserva_Legame: 0,
        Tipo_Associazione: 0
    };
    listRifMovDettaglio.push(locRifMovDettaglio);

    Imposta_Visibilita_FormProdottoUC(enum_TipoOperazioneDB.Copia.value);
    validatorTabDettaglio = inizializzaKendoValidator("tabDettagliDoc", false, false, true, true);
    //$("#panelbarFormProdottoUC").data("kendoPanelBar").collapse($("#panelBar_OrdiniCliente"));
    //$("#panelBar_OrdiniCliente").hide();
    // grid.removeRow(row);

}

function raccolteXConferimenti_btnAssociaClick(ev) {
    let arrRaccolte = raccolteConfUC_ottieniRigheSelezionate();

    if (!Array.isArray(arrRaccolte)) {
        return;
    }
    else {
        if (arrRaccolte.length === 0) {
            // Gli azzeramenti sono svolti dagli eventi specifici
            return;
        }

        let msgErr = "";
        let kgridImballi = KendoGrid("tab_imballaggi_formProdottoUC");
        if (kgridImballi !== undefined && kgridImballi.dataSource.data().length > 1) {
            msgErr = TraduzioneMultiResx(resxFormProdottoUC, "SoloUnImballoPerAssociareRaccolte",
                "Non è possibile associare le raccolte a causa della presenza di multiple righe di imballi");
        }

        if (raccolteConfUC_verificaCoerenzaRigheSelezionate(arrRaccolte) === false) {
            msgErr = TraduzioneMultiResx(resxFormProdottoUC, "RaccolteSelezionateNonCoerenti", // La variabile di traduzione contiene anche il resx di pagina DocContabile
                "Le righe di raccolta selezionate si riferiscono a impianti appartenenti a centri aziendali diversi oppure con specie o unità di misura diverse");
        }

        if (msgErr !== "") {
            $("<div></div>").kendoAlert({
                title: TraduzioneMultiResx(resxFormProdottoUC, "OperazioneNonConsentita", "Operazione non consentita"),
                content: msgErr,
            }).data("kendoAlert").open();

            return;
        }
    }

    ajaxAgronicaWaitFrame(true);

    raccolteXConferimenti_dsSelezionate = arrRaccolte;

    // Rieseguo, perché la funzione verifica il numero di raccolte selezionate
    AbilitaModificaDatiMinimiTestata(false);

    if (parseInt(KendoDDL("ddlCategorieMagazzino").value()) !== TRASFORMATI_VEGETALI) {
        KendoDDL("ddlCategorieMagazzino").value(TRASFORMATI_VEGETALI);
        $("#ddlCategorieMagazzino").trigger("change");
    }

    let impProponiPesoRaccolta = raccolteConfUC_LeggiImpostazioneProponiPeso(arrRaccolte[0].Sa_Cod_Campagna, arrRaccolte[0].Impianto_Veg_Cod, false);

    // Richiamo la funzione di lettura dei prodotti con filtri su specie/varietà, andando poi a selezionare il prodotto giusto col mat_cod se presente.
    // Fare riferimento alla funzione RicercaProdottiCompleto_DocContGenerico

    Recupera_ChiaveMagazzino();

    let ddlPuaReg = KendoDDL("ddlPUARegolamento");
    let xPuaRegolamento = 0;
    let xTipoPuaRegolamento = 0;

    if (ddlPuaReg !== undefined && ddlPuaReg.value() !== "" && parseInt(ddlPuaReg.value()) !== 0) {
        xPuaRegolamento = parseInt(ddlPuaReg.value());
        xTipoPuaRegolamento = parseInt(ddlPuaReg.dataItem().Regolamento_Tipo);
    }

    // TODO Verificare con Stefano/Giulia se va bene prendere il lotto accettazione in questo modo, oppure no
    let w_LottoAccettazione = "";
    //if (Qs_CaricoScarico === CAU_CARICO) {
        w_LottoAccettazione = $('input[name$="txtLottoAccettazione"]').val();
    //} else {
        //w_LottoAccettazione = Get_KendoDDLValue("ddlLottoAccettazione");
    //}

    let bloccaPerSottoGiacenza = false;
    let Flag_QtaNoZero = false;
    let filtroSpecie = [arrRaccolte[0].Impianto_Veg_Cod];
    let filtroVarieta = [];
    // Questo setup globale indica se la ricerca deve essere effettuata anche per la varietà oppure no, questo perché in campagna le aziende potrebbero gestire varietà generiche
    if (raccolteConfUC_filtraPerVarieta === true) {
        for (let i = 0; i < arrRaccolte.length; i++) {

            let varietaXImpianti = arrRaccolte[i].Impianto_Cul_Cod;
            let arrVarImpianti = varietaXImpianti.split("|");

            for (let x = 0; x < arrVarImpianti.length; x++) {
                filtroVarieta.push(arrVarImpianti[i]);
            }
        }
    }

    // Se è entrata da conferimento (ovvero da novembre 2020 tutti i prodotti Core Business)
    // filtro per chiave conferimento
    let xFiltroAggiuntivoMateriePrime = "";
    //if (lavCodAccettazione || lavCodAccettazionePomodoro) {
        xFiltroAggiuntivoMateriePrime = $('input[name$="hf_filtroMateriePrimeConferimento"]').val();
    //} else {
    //    let tipoRapporto = GetTipoRapporto(cIdLavCod);
    //    if (tipoRapporto === enum_TipoRapporto.Fornitori && is_FF_FormProdottoUC()) {
    //        xFiltroAggiuntivoMateriePrime = " (Materie_Prime.ELEM_COD NOT IN (" + TRASFORMATI_VEGETALI + ", " + TRASFORMATI_ANIMALI + ")) "
    //    }
    //}

    //let elemCod = arrRaccolte[0].Elem_Cod;
    let elencoProdottiCompleto = RicercaElencoCompletoProdotti(objP_super_server, objP_server, objP_utenti, $(cIdPiva).val(),
        xSa_Cod, xFabbricato_Cod, xTipoDestinazione,
        TRASFORMATI_VEGETALI, bloccaPerSottoGiacenza, null,
        Qs_Mode, Qs_CaricoScarico, get_data("inDataEmissione"), xPuaRegolamento, w_LottoAccettazione, false, Flag_QtaNoZero,
        xTipoPuaRegolamento, filtroSpecie, filtroVarieta, xFiltroAggiuntivoMateriePrime, false, -1, true);
    
    let dataSource = new kendo.data.DataSource({
        data: JSON.parse(elencoProdottiCompleto)
    });
    let kddlProd = KendoDDL("ddlProdottoDes");
    kddlProd.unbind("filtering");
    kddlProd.setDataSource(dataSource);
    kddlProd.dataSource.read(); // "Leggo" dal datasource impostato, necessario per il controllo kendo

    let arrRaccolteNonFast = arrRaccolte.filter(raccolta => raccolta.Mat_Cod_Campagna != 0);
    const existsRaccolte = arrRaccolteNonFast.length > 0;

    if (existsRaccolte) {
        kddlProd.value(arrRaccolteNonFast[0].Mat_Cod_Campagna * -1);
    }

    ddlProdottoDes_change();

    if (existsRaccolte) {

        if (impProponiPesoRaccolta == 1) {

            impostaDftValueDdlUM(arrRaccolteNonFast[0].Udm_Cod_Campagna);

            let qtaTotRaccolte = 0;

            for (let i = 0; i < arrRaccolte.length; i++) {
                qtaTotRaccolte += arrRaccolte[i].Qta_Dettaglio;
            }

            if (arrRaccolteNonFast[0].Udm_Cod_Campagna === enum_Udm.chilogrammi.value) {
                let kntbKgNetti = KendoNumTB("idKgNetti");
                kntbKgNetti.value(qtaTotRaccolte);
                kntbKgNetti.trigger("change");
            }
            else {
                let kntbQta = KendoNumTB("idQuantita");
                kntbQta.value(qtaTotRaccolte);
                kntbQta.trigger("change");
            }
        }
    }

    ajaxAgronicaWaitFrame(false);
}

function raccolteXConferimenti_btnAssociaNoRaccolteSel(ev) {
    // Reimposto la ddl
    CreaDdlProdottoDes();

    let impProponiPesoRaccolta = raccolteConfUC_LeggiImpostazioneProponiPeso(0, 0, false);
    if (impProponiPesoRaccolta == 1) {

        if (parseInt(Get_KendoDDLValue("ddlUM")) === enum_Udm.chilogrammi.value) {
            let kntbKgNetti = KendoNumTB("idKgNetti");
            kntbKgNetti.value(0);
            kntbKgNetti.trigger("change");
        }
        else {
            let kntbQta = KendoNumTB("idQuantita");
            kntbQta.value(0);
            kntbQta.trigger("change");
        }
    }

    // Se non ho più raccolte selezionate, posso ri-mostrare la modalità di scelta degli impianti
    if (imputazioneImpianti_AbilitazioneGenerale()) {
        $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ImputazioneImpianti]).attr("style", "display:inline-block");
    }

    $("#panelBar_OrdiniCliente").show();
    $("#panelbarFormProdottoUC").data("kendoPanelBar").collapse($("#panelBar_OrdiniCliente"));

    // Rieseguo, perché la funzione verifica il numero di raccolte selezionate
    AbilitaModificaDatiMinimiTestata(false);
}

function raccolteXConferimenti_btnAssociaPrimaRaccoltaSel(ev) {
    // Alla prima raccolta selezionata, nascondo la modalità di scelta degli impianti anche se di fatto non ho ancora confermato l'associazione tramite apposito pulsante
    if (imputazioneImpianti_AbilitazioneGenerale()) {
        $(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ImputazioneImpianti]).attr("style", "display:none");
    }

    $("#panelBar_OrdiniCliente").hide();
    $("#panelbarFormProdottoUC").data("kendoPanelBar").collapse($("#panelBar_OrdiniCliente"));

}

function raccolteXConferimenti_btnAssociaRaccoltaRimossa(ev) {
    if (raccolteXConferimenti_dsSelezionate.length > 0) {
        raccolteXConferimenti_dsSelezionate = raccolteConfUC_ottieniRigheSelezionate();
    }
}

function seAzzeraCalCod() {

    if (cIdLavCod === enum_LavCod.Ordine_Vendita_Emesso.value ||
        cIdLavCod === enum_LavCod.DDT_Emesso.value ||
        cIdLavCod === enum_LavCod.Fattura_Emessa.value ||
        cIdLavCod === enum_LavCod.Scarico_Magazzino.value ||
        cIdLavCod === enum_LavCod.Nota_Accredito_Ricevuta.value) {

        $('input[name$="hf_Cal_Cod"]').val(0);
        $('input[name$="hf_Cod_Progetto"]').val(0);

    }

}

function is_LavCod_MostraGiacenze() {

    let lavCod_MostraGiacenze = false;

    if (cIdLavCod === enum_LavCod.Ordine_Vendita_Emesso.value ||
        cIdLavCod === enum_LavCod.DDT_Emesso.value ||
        cIdLavCod === enum_LavCod.Fattura_Emessa.value ||
        cIdLavCod === enum_LavCod.Nota_Accredito_Ricevuta.value ||
        cIdLavCod === enum_LavCod.Scarico_Magazzino.value) {
        lavCod_MostraGiacenze = true;
    }

    return lavCod_MostraGiacenze;

}

function PropostaInnescoDaTrappola() {

    let w_propostaInnescoDaTrappola = false;

    if (propostaDatiRiga !== undefined) {

        let categoriaMagazzinoSelezionata = 0;
        if (KendoDDL("ddlCategorieMagazzino") !== undefined) {
            categoriaMagazzinoSelezionata = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
        }

        if (propostaDatiRiga.categoriaMagazzino !== undefined &&
            propostaDatiRiga.categoriaMagazzino === INNESCHI &&
            categoriaMagazzinoSelezionata === INNESCHI &&
            propostaDatiRiga.codiceTrappola !== undefined) {
            w_propostaInnescoDaTrappola = true;
        }

    }

    return w_propostaInnescoDaTrappola;

}

function Se_Abilita_PUARegolamento(ddlCategorieMagazzinoValue) {

    if (Qs_CaricoScarico === CAU_CARICO) {

        if (gestioneRegolamentoFormulati === true) {
            Se_RicaricaPUA_Regolamenti(ddlCategorieMagazzinoValue);
        }

        Visibilita_PUARegolamento(true, true);

        Set_KendoDDLValue("ddlPUARegolamento", "0");

        TrattaPuaRegolamento();

    }

}

function Carica_ProdCod_PositivoNegativo(dataItem) {

    let w_Prod_Cod = "";

    if (parseInt(dataItem.Pro_Cod) !== 0) {

        w_Prod_Cod = dataItem.Pro_Cod;

    } else if (parseInt(dataItem.Mat_Cod) !== 0) {

        w_Prod_Cod = dataItem.Mat_Cod * -1;

    }

    return w_Prod_Cod;

}

function Carica_Dati_Prodotto_Modifica(w_Prod_Cod, dataItem) {

    let w_Prod_Des = dataItem.Mat_Des;

    if (dataItem.Mov_Det_Des !== undefined && dataItem.Mov_Det_Des !== null && dataItem.Mov_Det_Des !== "") {
        w_Prod_Des = dataItem.Mov_Det_Des;
    }

    let w_Dati_Prodotto_Modifica = {
        Prodotto_Des: w_Prod_Des,
        Prodotto_Cod: w_Prod_Cod,
        Elem_Cod: dataItem.Cat_Cod,
        Veg_Cod: dataItem.Veg_Cod,
        Reg_Cod: dataItem.Reg_Cod,
        Cul_Cod: dataItem.Cul_Cod,
        Mat_Cod_OMNI: dataItem.Mat_Cod_OMNI,
        N: dataItem.N,
        P2O5: dataItem.P205,
        K2O: dataItem.K20,
        Cu: dataItem.Cu,
        Udm_Cod: dataItem.Udm_Cod,
        Extra_Str: dataItem.Extra_Str
    };

    return w_Dati_Prodotto_Modifica;

}

function Carica_Dati_Prodotto_Inserimento() {

    let w_Dati_Prodotto_Inserimento = {
        Prodotto_Des: "",
        Prodotto_Cod: "",
        Elem_Cod: 0,
        Veg_Cod: 0,
        Cul_Cod: 0,
        Reg_Cod: 0,
        Mat_Cod_OMNI: 0,
        N: 0,
        P2O5: 0,
        K2O: 0,
        Cu: 0
    };

    return w_Dati_Prodotto_Inserimento;

}

function SeImpostaFiltroTuttiProdotti(evento, objFiltering) {

    SeVisualizzaInserireCaratteri();

    if (PresentaTuttiProdottiCategoriaMagazzino()) {

        let filtroTuttiProdotti = false;

        let objFiltroInput = new Object();

        switch (evento) {

            case Evento_Prodotto_Filtering:

                // Se evento prodotto filtering e il nuovo filtro impostato con lunghezza = 0, 
                // seleziono tutti i prodotti
                if (objFiltering !== undefined && objFiltering.value !== undefined && objFiltering.value.length === 0) {

                    filtroTuttiProdotti = true;

                }

                break;

            case Evento_Prodotto_Open:

                // Se evento prodotto open e:
                // - il valore del primo filtro è diverso dal valore qualsiasi prodotto
                // - il valore del filtro input ha lunghezza = 0
                // seleziono tutti i prodotti

                objFiltroInput = LeggiFiltroInput(KendoDDL("ddlProdottoDes"));

                let valorePrimoFiltro = LeggiValorePrimoFiltroDDL(KendoDDL("ddlProdottoDes"));

                if (valorePrimoFiltro !== FiltroQualsiasiProdotto && objFiltroInput.value.length === 0) {

                    filtroTuttiProdotti = true;

                }

                break;

            case Evento_MagazzinoScarico_Change:
            case Evento_DataEmissione_Change:

                // Se evento magazzino scarico o data emissione change, 
                // seleziono tutti i prodotti e inizializzo il filtro input

                objFiltroInput = LeggiFiltroInput(KendoDDL("ddlProdottoDes"));

                filtroTuttiProdotti = true;

                if (objFiltroInput.value.length > 0) {
                    KendoDDL("ddlProdottoDes").filterInput[0].value = "";
                }

                break;

        }

        if (filtroTuttiProdotti) {

            ImpostaFiltroTuttiProdotti();

        }

    }

}

function PresentaTuttiProdottiCategoriaMagazzino() {

    let leggiTuttiProdotti = false;

    //----------------------------------------------------------------------
    // Contratti di affitto
    //----------------------------------------------------------------------

    if (isContrattoAffitto()) {

        leggiTuttiProdotti = true;

    }

    //----------------------------------------------------------------------
    // Categoria SERVIZI
    //----------------------------------------------------------------------

    if (leggiTuttiProdotti === false &&
        KendoDDL("ddlCategorieMagazzino") !== undefined &&
        Get_KendoDDLValue("ddlCategorieMagazzino") !== "" &&
        parseInt(Get_KendoDDLValue("ddlCategorieMagazzino")) === SERVIZI) {

        leggiTuttiProdotti = true;

    }

    //----------------------------------------------------------------------
    // Scarico di magazzino e...
    //----------------------------------------------------------------------
    //    - categoria impostata
    //    - magazzino impostato
    //    - categoria che movimenta il magazzino
    //    - categoria che prevede un controllo di giacenza
    //----------------------------------------------------------------------

    if (leggiTuttiProdotti === false &&
        Qs_CaricoScarico === CAU_SCARICO &&
        KendoDDL("ddlCategorieMagazzino") !== undefined && Get_KendoDDLValue("ddlCategorieMagazzino") !== "" &&
        KendoDDL("ddlUbicProvenienza") !== undefined && Get_KendoDDLValue("ddlUbicProvenienza") !== "") {

        let w_Categ_Mag = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

        //NB: In caso di ordini w_gest_giacenza è uguale a enum_Gestione_Giacenze_TuttiProdotti
        let w_gest_giacenza = getGestioneGiacenza();

        if (!CategorieMagazzinoNonMovimentato.includes(w_Categ_Mag) && w_gest_giacenza !== enum_Gestione_Giacenze_TuttiProdotti) {

            leggiTuttiProdotti = true;

        }

    }

    return leggiTuttiProdotti;

}

function ImpostaFiltroTuttiProdotti() {

    // Salvo valore prodotto selezionato

    let w_Prod_Cod = parseInt(Get_KendoDDLValue("ddlProdottoDes"));

    // Imposto nuovo filtro

    let objFilter = {
        value: FiltroQualsiasiProdotto,
        field: "Prodotto_Des",
        operator: "contains",
        ignoreCase: true
    };

    //NB: il settaggio del filter riscatena la read()!

    WaitFrame.show();

    setTimeout(function () {

        KendoDDL("ddlProdottoDes").dataSource.filter({
            logic: "and",
            filters: [objFilter]
        });

        // Reset prodotto selezionato

        KendoDDL("ddlProdottoDes").select(-1);

        // Imposto nuovamente il prodotto selezionato se presente in elenco

        let elencoProdotti = KendoDDL("ddlProdottoDes").dataSource.data();

        let trovatoProdotto = elencoProdotti.find(prodotto => prodotto.Prodotto_Cod === w_Prod_Cod);

        if (trovatoProdotto !== undefined) {

            var DDL = KendoDDL("ddlProdottoDes");

            DDL.select(function (dataItem) {
                return dataItem.Prodotto_Cod === w_Prod_Cod;
            });

        }

        WaitFrame.hide();

    }, 150);

}

function LeggiValorePrimoFiltroDDL(controlloDDL) {

    let valorePrimoFiltro = ""

    let controlloDS = controlloDDL.dataSource;

    if (controlloDS !== undefined && controlloDS.filter() !== undefined) {

        let elencoFiltri = controlloDS.filter();

        if (elencoFiltri.filters !== undefined && elencoFiltri.filters.length > 0) {

            valorePrimoFiltro = elencoFiltri.filters[0].value;
            
        }

    }

    return valorePrimoFiltro;

}

function SeVisualizzaInserireCaratteri() {

    if (PresentaTuttiProdottiCategoriaMagazzino()) {

        $("#lblInserireCaratteri").hide();
        KendoDDL("ddlProdottoDes").noDataTemplate = function () { return "Nessun prodotto trovato" };

    } else {

        $("#lblInserireCaratteri").show();
        KendoDDL("ddlProdottoDes").noDataTemplate = NoDataTemplateDefaultProdotto;

    }

}

function LeggiFiltroInput(controlloDDL) {

    let objFiltroInput = {
        value: ""
    };

    if (controlloDDL.filterInput !== undefined && controlloDDL.filterInput.length > 0) {
        objFiltroInput.value = controlloDDL.filterInput[0].value
    }

    return objFiltroInput;

}

function ddlConfezionamentoLotto_open(e) {
    old_ConfezionamentoLotto_FormProdottoUC = KendoDDL("ddlConfezionamentoLotto").text();
}

function ddlConfezionamentoLotto_change(e) {

    let ddlConfezionamento = KendoDDL("ddlConfezionamentoLotto");

    var objLottoConfez = {
        lotto: "",
        confezionamentoOld: "",
        separatoConfezionamento: false,
        posizioneConfezionamento: enum_posizioneConfezionamentoLotto.Nessuna
    };

    objLottoConfez.lotto = $("#txtLottoAccettazione").val();

    if (objLottoConfez.lotto !== "") {

        if (old_ConfezionamentoLotto_FormProdottoUC !== "") {

            objLottoConfez.confezionamentoOld = old_ConfezionamentoLotto_FormProdottoUC;

            separaConfezionamentoDaLotto(objLottoConfez)

        } else {

            elencoConfezionamentoLotto.every(function (element) {

                if (element.mat_des !== "") {

                    objLottoConfez.confezionamentoOld = element.mat_des;

                    separaConfezionamentoDaLotto(objLottoConfez)

                    if (objLottoConfez.separatoConfezionamento === true) {

                        return false;

                    }

                }

                return true;

            });

        }

    }

    let primaParteLotto = "";
    let secondaParteLotto = "";

    if (objLottoConfez.posizioneConfezionamento === enum_posizioneConfezionamentoLotto.Inizio) {

        primaParteLotto = ddlConfezionamento.text();
        secondaParteLotto = objLottoConfez.lotto;

    } else {

        primaParteLotto = objLottoConfez.lotto;
        secondaParteLotto = ddlConfezionamento.text();

    }

    let lottoConfezionato = primaParteLotto.concat(secondaParteLotto);
        
    $("#txtLottoAccettazione").val(lottoConfezionato);

    $("#txtLottoAccettazione").trigger("change");

}

function separaConfezionamentoDaLotto(objLottoConfez) {

    if (objLottoConfez.lotto.endsWith(objLottoConfez.confezionamentoOld)) {

        let posizioneConfezOld = objLottoConfez.lotto.lastIndexOf(objLottoConfez.confezionamentoOld);

        objLottoConfez.lotto = objLottoConfez.lotto.substr(0, posizioneConfezOld);

        objLottoConfez.separatoConfezionamento = true;

        objLottoConfez.posizioneConfezionamento = enum_posizioneConfezionamentoLotto.Fine;

        return objLottoConfez.posizioneConfezionamento;

    }

    if (objLottoConfez.lotto.startsWith(objLottoConfez.confezionamentoOld)) {

        let lunghezzaConfezOld = objLottoConfez.confezionamentoOld.length;

        let lunghezzaLotto = objLottoConfez.lotto.length;

        objLottoConfez.lotto = objLottoConfez.lotto.substr(lunghezzaConfezOld, lunghezzaLotto - 1);

        objLottoConfez.separatoConfezionamento = true;

        objLottoConfez.posizioneConfezionamento = enum_posizioneConfezionamentoLotto.Inizio;

        return objLottoConfez.posizioneConfezionamento;

    }

    return objLottoConfez.posizioneConfezionamento;

}

function dpDataScadenza_change(e) {

    let lottoAttuale = $("#txtLottoAccettazione").val();
    let nuovaData = e.sender.value();

    lottoAttuale = concatenaDataScadenzaLotto(nuovaData, lottoAttuale);

    $("#txtLottoAccettazione").val(lottoAttuale);
}


function concatenaDataScadenzaLotto(nuovaData, lottoAttuale) {

    const regexDataScadLotto = new RegExp(/^\d{4}(-\d{2}){2}/);

    let dataInLotto = nuovaData === null ? "" : kendo.toString(nuovaData, "yyyy-MM-dd");

    if (regexDataScadLotto.test(lottoAttuale)) {
        lottoAttuale = lottoAttuale.replace(regexDataScadLotto, dataInLotto).trimStart();
    }
    else if (dataInLotto !== "") {
        lottoAttuale = dataInLotto + " " + lottoAttuale;
    }

    return lottoAttuale;
}