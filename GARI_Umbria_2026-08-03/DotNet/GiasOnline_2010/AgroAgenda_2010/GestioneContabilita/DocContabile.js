function Set_RadioGroupValue(nameGroup, value) {
    let radio = $("input[type='radio'][name='" + nameGroup + "']");
    radio.removeAttr("checked");
    radio.eq(value).prop("checked", true); //.trigger('change');
    radio.eq(value).change(); //forzo il change
}

function Get_RadioGroupValue(nameGroup) {
    let selValue = $("input[type='radio'][name='" + nameGroup + "']:checked").val();

    if (selValue !== undefined && selValue !== null && selValue !== "")
        return parseInt(selValue);
    else
        return 0;
}

//IMPORTANTE: non cambiare nome perché così si chiama nella pagina dei contatti_manager
//TODO: sistemare perché in caso Fruttagel non devo rileggere il 2 e l'1 non è detto che lo possa impostare
//function Contatto_gestisciValore(codContatto) {

//    //ho aggiunto un nuovo contatto-->svuoto array e rileggo
//    let tipoRapporto = GetTipoRapporto(cIdLavCod);
//    elencoContatti[tipoRapporto] = null;

//    KendoDDL(ddlContatto1Nome).dataSource.read();
//    KendoDDL(ddlContatto2Nome).dataSource.read();

//    KendoDDL(ddlContatto1Nome).select(function (dataItem) {
//        return dataItem.Cod_Contatto === codContatto;
//    });

//    $("#" + ddlContatto1Nome).change();

//    $("#nuovoContattoWindow").data("kendoWindow").close();
//}

function GetTipoRapporto(lavCod) {
    let tipoRapporto = 0;
     
    switch (lavCod) {
        case enum_LavCod.Fattura_Emessa.value:
        case enum_LavCod.DDT_Emesso.value:
        case enum_LavCod.DDT_Contabilizzato_Emesso.value:
        case enum_LavCod.Ordine_Vendita_Emesso.value:
        case enum_LavCod.Nota_Accredito_Emessa.value:
            // 1001 = Fattura Emessa, 1031 = DDT emesso, 1069 = DDT Contabilizzato emesso
            tipoRapporto = enum_TipoRapporto.Clienti;
            break;

        case enum_LavCod.Fattura_Ricevuta.value:
        case enum_LavCod.DDT_Ricevuto.value:
        case enum_LavCod.Ordine_Acquisto.value:
        case enum_LavCod.Contratto_Affitto.value:
        case enum_LavCod.Nota_Accredito_Ricevuta.value:
            // 1000 = Fattura ricevuta, 1025 = DDT Ricevuto
            tipoRapporto = enum_TipoRapporto.Fornitori;
            break;

        case enum_LavCod.Accettazione_da_diversi.value: // 1054 = Accettazione da diversi
        case enum_LavCod.Distinta_Carico.value:
        case enum_LavCod.Distinta_Carico_Accettazione.value:
        case enum_LavCod.Auto_Ddt_Emesso.value:
        case enum_LavCod.Auto_Ddt_Emesso_Accettazione.value:
            tipoRapporto = enum_TipoRapporto.Conferenti;
            break;
    }

    return tipoRapporto;
}

function getDatiContabTestata() {
    let data = $('input[name$="hdKendo_TestataDoc"]').val();
    let jSonParsed = null;
    if (data !== null && data !== undefined && data !== "") {
        jSonParsed = JSON.parse(data);
    }
    return jSonParsed;
}

function GetNewProgressivo() {
    let data = kendo.parseDate($("#inDataRegistrazione").val());
    if (data !== undefined && data !== null) {
        let anno = data.getFullYear();
        let tipoProgressivo = 14;

        let newProgressivo = 1;
        newProgressivo = NuovoProgressivoValBase($(cIdPiva).val(), anno, tipoProgressivo, "", "", 0, 0);
        Set_KendoNumTBValue("inNumRegistrazione", newProgressivo);
    }
}

function RefreshNumDoc() {
    let data = kendo.parseDate($("#inDataEmissione").val());

    if (data !== undefined && data !== null) {
        let docNumeroSin = $("#inNumDocSin").val().toUpperCase();
        let docNumeroDes = $("#inNumDocDes").val().toUpperCase();
        let anno = data.getFullYear();

        let lastNumDoc = 0;
        lastNumDoc = LastNumDocumento($(cIdPiva).val(), cIdLavCod, anno, docNumeroSin, docNumeroDes, 0);
        let newNumDoc = lastNumDoc + 1;
        Set_KendoNumTBValue("inNumDoc", newNumDoc);
    }
}

function RefreshNumDoc2() {
    let data2 = kendo.parseDate($("#inDataEmissione").val());

    if (data2 !== undefined && data2 !== null) {
        let docNumeroSin2 = $("#inNumDocSin2").val().toUpperCase();
        let docNumeroDes2 = $("#inNumDocDes2").val().toUpperCase();
        let anno2 = data2.getFullYear();

        let lastNumDoc2 = 0;
        lastNumDoc2 = LastNumDocumento($(cIdPiva).val(), cIdLavCod, anno2, docNumeroSin2, docNumeroDes2, 0, CAU_REGISTRAZIONE_SECONDARIA);
        let newNumDoc2 = lastNumDoc2 + 1;
        Set_KendoNumTBValue("inNumDoc2", newNumDoc2);
    }
}

function PulsantiSalvataggioSolaLettura() {
    $("#btnNuovoDocmmento").hide();
    $("#btnNuovoDocmmento2").hide();

    $("#btnDocmmenti").hide();
    $("#btnDocmmenti2").hide();

    $("#btnSalvaTestataPiuRiga").hide();
    $("#btnSalvaTestataPiuRiga2").hide();

    $("#btnSalvaEsciDoc").hide();
    $("#btnSalvaEsciDoc2").hide();

    $("#btnSalvaENuovoDoc").hide();
    $("#btnSalvaENuovoDoc2").hide();

    $("#btn_SalvaENuovoRiga").hide();
    $("#btn_SalvaENuovoRiga2").hide();

    MostraBtnStampa(true);

    $("#btnEsciDocTxt").text(TraduzioneMultiResx(resxObj, "Esci", "Esci"));
    $("#btnEsciDocTxt2").text(TraduzioneMultiResx(resxObj, "Esci", "Esci"));

}

function PulsantiSalvataggioModifica() {

    MostraBtnDocumenti();

    $("#btnSalvaTestataPiuRiga").show();
    $("#btnSalvaTestataPiuRiga2").show();

    $("#btnSalvaEsciDoc").show();
    $("#btnSalvaEsciDoc2").show();

    $("#btnSalvaENuovoDoc").show();
    $("#btnSalvaENuovoDoc2").show();

    if (impedisciCreazioneCarichiMultiriga == false) {
        $("#btn_SalvaENuovoRiga").show();
        $("#btn_SalvaENuovoRiga2").show();
    }

    MostraBtnStampa(false);

    $("#btnEsciDocTxt").text(TraduzioneMultiResx(resxObj, "EsciSenzaSalvare", "Esci senza salvare"));
    $("#btnEsciDocTxt2").text(TraduzioneMultiResx(resxObj, "EsciSenzaSalvare", "Esci senza salvare"));

}

function PulsantiSalvataggioScrittura() {

    $("#btnSalvaEsciDoc").show();
    $("#btnSalvaEsciDoc2").show();

    $("#btnSalvaENuovoDoc").show();
    $("#btnSalvaENuovoDoc2").show();

    if (impedisciCreazioneCarichiMultiriga == false) {
        $("#btn_SalvaENuovoRiga").show();
        $("#btn_SalvaENuovoRiga2").show();
    }

    MostraBtnStampa(false);

    $("#btnEsciDocTxt").text(TraduzioneMultiResx(resxObj, "EsciSenzaSalvare", "Esci senza salvare"));
    $("#btnEsciDocTxt2").text(TraduzioneMultiResx(resxObj, "EsciSenzaSalvare", "Esci senza salvare"));

}

function VisualizzaPulsantiSalvataggio(visualizzaPulsanti) {

    if (visualizzaPulsanti === true) {

        $("#btnSalvaTestataPiuRiga").show();
        $("#btnSalvaTestataPiuRiga2").show();

        $("#btnSalvaEsciDoc").show();
        $("#btnSalvaEsciDoc2").show();

        $("#btnSalvaENuovoDoc").show();
        $("#btnSalvaENuovoDoc2").show();

        if (impedisciCreazioneCarichiMultiriga == false) {
            $("#btn_SalvaENuovoRiga").show();
            $("#btn_SalvaENuovoRiga2").show();
        }

        if (!isLavCodSenzaStampa()) {
            $("#btnSalvaStampa").show();
            $("#btnSalvaStampa2").show();
        }

    } else {

        $("#btnSalvaTestataPiuRiga").hide();
        $("#btnSalvaTestataPiuRiga2").hide();

        $("#btnSalvaEsciDoc").hide();
        $("#btnSalvaEsciDoc2").hide();

        $("#btnSalvaStampa").hide();
        $("#btnSalvaStampa2").hide();

        $("#btnSalvaENuovoDoc").hide();
        $("#btnSalvaENuovoDoc2").hide();

        $("#btn_SalvaENuovoRiga").hide();
        $("#btn_SalvaENuovoRiga2").hide();
    }

}

function ImpostaReadOnly(flagReadOnly, suIntestazione, suTabTestata, suTabDettagli) {
    if (suIntestazione === true) {
        $("#intestazione").find("input").attr("readonly", flagReadOnly);

        if (flagReadOnly === true) {
            PulsantiSalvataggioSolaLettura();
        } else {
            if (parseInt($(cIdAgenda).val()) !== 0 && jSonParsed_Kendo.kendo_rows.length > 0) {
                PulsantiSalvataggioScrittura();
            } else {
                PulsantiSalvataggioModifica();
            }
        }
    }

    if (suTabTestata === true) {
        $("#tabTestataDoc").find("input").attr("readonly", flagReadOnly);
        $("#tabTestataDoc").find("textarea").attr("readonly", flagReadOnly);
        //$("#tabTestataDoc").find(":radio:not(:checked)").attr("disabled", flagReadOnly);
        //$("#tabTestataDoc").find(":radio:checked").attr("disabled", flagReadOnly);
        $("#tabTestataDoc").find(":radio").attr("disabled", flagReadOnly);
    }

    if (suTabDettagli === true) {
        //$("#tabDettagli").find("input").attr("readonly", flagReadOnly);
        //$("#tabDettagli").find("textarea").attr("readonly", flagReadOnly);
        //$("#tabDettagli").find(":radio").attr("disabled", flagReadOnly);
    }
}

function GetPivaConferente() {
    let pivaConferente = "";

    //può essere il produttore oppure il conferente se il primo non è specificato
    let kddlConferente = KendoDDL(ddlContatto1Nome);
    let kddlProduttore = KendoDDL(ddlContatto2Nome);

    if (contattiAcc4ConGerarchia === true) {

        //se accettazione con gerarchia, non mi preoccupo di isContattoImpresaGias, perché tutti i contatti sono per forza di cose impreseGias

        if (kddlProduttore.dataItem() !== undefined &&
            kddlProduttore.dataItem() !== null &&
            kddlProduttore.dataItem().Cod_Contatto !== "") {

            pivaConferente = kddlProduttore.dataItem().Cod_Contatto;

        } else if (kddlConferente.dataItem() !== undefined && kddlConferente.dataItem() !== null) {
            pivaConferente = kddlConferente.dataItem().Cod_Contatto;
        }

    } else {
        if (kddlProduttore.dataItem() !== undefined &&
            kddlProduttore.dataItem() !== null &&
            kddlProduttore.dataItem().Cod_Contatto !== "" &&
            kddlProduttore.dataItem().IsImpresaGias) {

            pivaConferente = kddlProduttore.dataItem().Cod_Contatto;

        } else {
            if (kddlConferente.dataItem() !== undefined &&
                kddlConferente.dataItem() !== null &&
                kddlConferente.dataItem().IsImpresaGias) {

                pivaConferente = kddlConferente.dataItem().Cod_Contatto;
            }
        }

    }

    return pivaConferente;
}

function GeneraOggettoTestata() {
    //riprendo i dati che avevo letto, così ho già molte cose valorizzate come idAgenda e dataOraUltimaLettura
    let testataDoc = getDatiContabTestata();

    if (testataDoc === null) {
        testataDoc = new Object();
        testataDoc.Piva = $(cIdPiva).val();
        testataDoc.IdAgenda = 0;
        testataDoc.IdMov = 0;
        testataDoc.StatoExport2 = 0;
        testataDoc.TipoDocumento = 0;

        //TODO: devo andare ad aggiornare il contatore del num registrazione (seq tab), perché prima l'ho solo letto
        //se il risultato è diverso da quello che avevo lo devo anche cambiare
    }

    if (testataDoc.StatoExport2 === 1) {
        //Usato da export bolle a TerreEmerse (era già stato esportato ma ora il documento viene modificato)
        testataDoc.StatoExport2 = 2;
    }
    
    //devo sostituire gli elementi di layout rispetto a quelli che ho letto precedentemente
    //TODO: consentire il cambiamento di lav cod? forse solo in alcuni casi (ddt emesso - ddt contabilizzato emesso)
    testataDoc.LavCod = parseInt(Get_KendoDDLValue("inTipologiaDocumento"));    //ri-sovrascrivo anche il lavcod, anche se non dovrebbe cambiare
    // 23-11-2020   ora in accettazione possono entrare anche trasf. animali e potrei avere nello stesso doc sia trasf. animali
    //              che vegetali; quindi non imposto il modulo anagrafe così come viene fatto nei documenti di vendita tranne che non sia conferimento pomodoro
    //              modulo_anagrafe_log comunque nel frattempo è diventato un array
    //testataDoc.ModuloGias = modulo_anagrafe_log[0];
    if (lavCodAccettazionePomodoro)
        testataDoc.ModuloGias = Modulo_FreshFood;
    else
        testataDoc.ModuloGias = 0;

    if (Cmb_Operatore.element[0].disabled === false)
        testataDoc.UsernameModifica = Get_KendoDDLValue("inOperatore");
    else
        testataDoc.UsernameModifica = operatoreCodFisc;

    testataDoc.DocNumeroSin = $("#inNumDocSin").val().toUpperCase();
    testataDoc.DocNumero = Get_KendoNumTBValue("inNumDoc");

    if (testataDoc.DocNumero === null || testataDoc.DocNumero === undefined) {
        testataDoc.DocNumero = 0;
    }

    testataDoc.DocNumeroDes = $("#inNumDocDes").val().toUpperCase();
    testataDoc.DocNumeroVisualizzato = $("#inNumDocShow").val().toUpperCase();
    testataDoc.DocNumeroLock = getKendoSwitch("inNumDocLock");

    let docNumeroLunghezza = 0;
    let docNumeroCarattereFormattazione = "";
    if (KendoDDL("inNumDocDDL") !== undefined &&
        KendoDDL("inNumDocDDL").dataItem() !== undefined &&
        KendoDDL("inNumDocDDL").dataItem() !== null) {

        //se è lockato, non mi passo neanche le informazioni di formattazione perché non mi servirebbero (e potrebbero essere diverse da quelle usate precedentemente)
        // a meno che non debba assegnarlo io lato server
        if (testataDoc.DocNumero === 0 || (testataDoc.DocNumero > 0 && testataDoc.DocNumeroLock === false)) {
            docNumeroLunghezza = parseInt(KendoDDL("inNumDocDDL").dataItem().Lunghezza_Centro);
            docNumeroCarattereFormattazione = KendoDDL("inNumDocDDL").dataItem().CarattereFormattazione;
        }

    }
    testataDoc.DocNumeroLunghezza = docNumeroLunghezza;
    testataDoc.DocNumeroCarattereFormattazione = docNumeroCarattereFormattazione;

    testataDoc.DataMovimento = kendo.parseDate($("#inDataEmissione").val());
    testataDoc.DataRegistrazione = kendo.parseDate($("#inDataRegistrazione").val());

    testataDoc.ProgrRegistrazione = Get_KendoNumTBValue("inNumRegistrazione");
    testataDoc.ProgrProtocollo = Get_KendoNumTBValue("inNumProtocollo");

    //Impostazione Campo per Determinazione Fattura Accompagnatoria
    if (getKendoSwitch("chkAccompagnatoria")) {
        testataDoc.Extra_Int = 1;
    } else {
        testataDoc.Extra_Int = 0;
    }
    testataDoc.Accompagnatoria = testataDoc.Extra_Int;

    let chkProvvisorioVal = getKendoSwitch("chkProvvisorio");

    if (testataDoc.IdAgenda === 0) {
        // In scrittura se attiva devo impostare i 3 campi. Se non attiva posso non fare niente.
        if (chkProvvisorioVal === true) {
            testataDoc.BloccoFlag = 1000;
            testataDoc.BloccoUsername = testataDoc.UsernameModifica;
            testataDoc.BloccoData = new Date();
        }
    }
    else {
        /*
        In modifica devo prima controllare se è stata variata rispetto a ciò che era salvato.
        1. Se era salvato provvisorio ed è ancora provvisorio non devo fare niente.
        2. Se era salvato provvisorio e ora non lo è più, devo impostare i campi ai default.
        3. Se era salvato come definitivo e lo è ancora non devo fare niente.
        4. Se era salvato come definitivo e non lo è più, devo impostare i campi.
        */
        if (testataDoc.BloccoFlag === 1000 && chkProvvisorioVal === false) {
            // Caso 2
            testataDoc.BloccoFlag = 0;
            testataDoc.BloccoUsername = "";
            testataDoc.BloccoData = AGRODATAINIZIO;
        } else if (testataDoc.BloccoFlag !== 1000 && chkProvvisorioVal === true) {
            // Caso 4
            testataDoc.BloccoFlag = 1000;
            testataDoc.BloccoUsername = testataDoc.UsernameModifica;
            testataDoc.BloccoData = new Date();
        }
    }

    testataDoc.SezionaleCod = parseInt(Get_KendoDDLValue("inSezionale"));
    testataDoc.TipoDocumento = parseInt(Get_KendoDDLValue("inTipoDocumento"));

    if (lavCodMovMagazzino === true) {
        testataDoc.Aspetto = "";
        testataDoc.CausaleTrasportoCod = 0;
        testataDoc.CausaleTrasporto = "";
        testataDoc.ProgrRegistrazione = 0;
    } else {
        testataDoc.Aspetto = KendoDDL("inAspettoBeni").text();
        testataDoc.CausaleTrasportoCod = parseInt(Get_KendoDDLValue(idControlloCausaleTrasporto));
        testataDoc.CausaleTrasporto = KendoDDL(idControlloCausaleTrasporto).text();
    }

    testataDoc.Colli = Get_KendoNumTBValue("inNumeroColli");
    testataDoc.TipoPeso = parseInt(Get_KendoDDLValue("inTipoPeso"));
    testataDoc.Peso = Get_KendoNumTBValue("inPesoTotale");

    testataDoc.NaturaBeni = $("#inNaturaBeni").val();

    if (KendoDDL(ddlContatto1Nome).dataItem()) {
        testataDoc.RagSoc = KendoDDL(ddlContatto1Nome).dataItem().Rag_Soc;
    } else {
        testataDoc.RagSoc = "";
    }
    testataDoc.CodRisUm = parseInt(Get_KendoDDLValue(ddlContatto1Nome, 0));
    testataDoc.CodIndirizzoRisUm = parseInt(Get_KendoDDLValue("inTipoIndirizzoCC1", 0));

    testataDoc.CodDestinazione = parseInt(Get_KendoDDLValue(ddlContatto2Nome, 0));
    if (isNaN(testataDoc.CodDestinazione)) {
        testataDoc.CodDestinazione = 0;
    }
    testataDoc.CodIndirizzoDestinazione = parseInt(Get_KendoDDLValue("inTipoIndirizzoCC2", 0));

    testataDoc.NoteIntestazione = $("#inNoteTestata").val();

    testataDoc.Mezzo = Get_RadioGroupValue("inTrasportoCura");

    testataDoc.GestioneVettore = parseInt(Get_KendoDDLValue("inGestioneVettore"));

    testataDoc.CodVettore = parseInt(Get_KendoDDLValue("inVettore", 0));
    testataDoc.CodIndirizzoVettore = parseInt(Get_KendoDDLValue("inTipoIndirizzoVettore", 0));

    testataDoc.ModalitaTrasporto = parseInt(Get_KendoDDLValue("inAccModalitaTrasporto"));
    testataDoc.UnitaTrasporto = parseInt(Get_KendoDDLValue("inAccUnitaTrasporto"));

    //TODO: sistema gestione come per causale trasporto!
    testataDoc.MacCodTrasporto = parseInt(Get_KendoDDLValue("inMezzoTrasporto"));
    testataDoc.Targa = KendoDDL("inMezzoTrasporto").text();

    testataDoc.DescrizioneMezzo = $("#inDescrizioneMezzo").val();
    testataDoc.NumImmatricolazioneRimorchio = $("#inNImmatricolazioneRimorchio").val();
    testataDoc.NumAutorizzazioneTrasporto = $("#inNAutorizzazioneTrasporto").val();
    testataDoc.DataRilascioAutorizzazione = kendo.parseDate($("#inDataAutorizzazioneTrasporto").val());
    testataDoc.PesoTaraTrasporto = Get_KendoNumTBValue("inPesoTaraTrasporto");

    testataDoc.TaraVeicolo = Get_KendoNumTBValue("inTaraVeicoloRiepilogo");

    testataDoc.AgenteCod = parseInt(Get_KendoDDLValue("inAgente"));
    testataDoc.AgenteProvvigione = Get_KendoNumTBValue("inAgenteProvvigione");
    testataDoc.CapoAreaCod = parseInt(Get_KendoDDLValue("inCapoArea"));
    testataDoc.CapoAreaProvvigione = Get_KendoNumTBValue("inCapoAreaProvvigione");

    testataDoc.Layout_FormatiStampa = Get_RadioGroupValue("inFormatiStampa");

    testataDoc.DistanzaTrasportoUdm = Get_KendoDDLValue("ddlUnitaMisuraTrasporto", 0);
    testataDoc.DistanzaTrasporto = Get_KendoNumTBValue("ntbDistanzaTrasporto", true);

    testataDoc.N_Nota_Fattura = $("#inNotaFattura").val();
    testataDoc.Data_Nota_Fattura = kendo.parseDate($("#inDataNotaFattura").val());
    testataDoc.N_Nota_DDT_Reso_SDI = $("#inDDTResoSDI").val();
    testataDoc.Data_Nota_DDT_Reso_SDI = kendo.parseDate($("#inDataDDTResoSDI").val());
    testataDoc.N_Nota_Riga_DDT_Reso_SDI = $("#inRigaDDTResoSDI").val();

    if (lavCodOrdine === true) {
        testataDoc.ScadenzaUnica = getKendoSwitch("inScadenzaUnica");
        testataDoc.EvasioneTassativa = getKendoSwitch("inEvasioneTassativa");
        testataDoc.DataEvasionePrevista = kendo.parseDate($("#inDataEvasionePrevista").val());
        testataDoc.DataSpedizionePrevista = kendo.parseDate($("#inDataSpedizionePrevista").val());

        testataDoc.NumDocOrdineCliente = $("#inNOrdineCliente").val();
        testataDoc.DataDocOrdineCliente = kendo.parseDate($("#inDataOrdineCliente").val());
        testataDoc.NumDocOrdineEnte = $("#inNOrdineConsorzio").val();
        if ($("#inAnnoOrdineConsorzio").val() !== null && $("#inAnnoOrdineConsorzio").val() !== "") {
            testataDoc.AnnoDocOrdineEnte = parseInt($("#inAnnoOrdineConsorzio").val());
        } else {
            testataDoc.AnnoDocOrdineEnte = null;
        }
    } else {
        testataDoc.OraSpedizione = kendo.parseDate($("#inDataSpedizione").val());
    }

    switch (cIdLavCod) {
        case enum_LavCod.DDT_Ricevuto.value:
        case enum_LavCod.Fattura_Ricevuta.value:
        case enum_LavCod.Nota_Accredito_Ricevuta.value:
        case enum_LavCod.DDT_Emesso.value:
        case enum_LavCod.DDT_Contabilizzato_Emesso.value:
        case enum_LavCod.Fattura_Emessa.value:
        case enum_LavCod.Nota_Accredito_Emessa.value:
            testataDoc.ModalitaPagamento = parseInt(Get_KendoDDLValue("inModPagamento"));
            break;
    }

    switch (testataDoc.LavCod) {
        case enum_LavCod.Accettazione_da_diversi.value:
        case enum_LavCod.Distinta_Carico_Accettazione.value:
        case enum_LavCod.Auto_Ddt_Emesso_Accettazione.value:

            if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true) {
                testataDoc.CodRisUmAltro = parseInt(Get_KendoDDLValue(ddlContattoCoop1Nome));
                testataDoc.SecondaCooperativa = parseInt(Get_KendoDDLValue(ddlContattoCoop2Nome));
            } else {
                testataDoc.CodRisUmAltro = 0;
                testataDoc.SecondaCooperativa = 0;
            }

            if (testataDoc.DocumentoAccettazione === undefined || testataDoc.DocumentoAccettazione === null) {
                testataDoc.DocumentoAccettazione = new Object();
                testataDoc.DocumentoAccettazione.IdMov = 0;
            }
            testataDoc.DocumentoAccettazione.DocNumeroSin = $("#inNumDocSin2").val().toUpperCase();
            testataDoc.DocumentoAccettazione.DocNumero = Get_KendoNumTBValue("inNumDoc2");
            testataDoc.DocumentoAccettazione.DocNumeroDes = $("#inNumDocDes2").val().toUpperCase();

            testataDoc.DocumentoAccettazione.DataMovimento = kendo.parseDate($("#inDataEmissione2").val());

            // passo tipo accettazione per distinguere conferimento pomodoro
            testataDoc.TipoAccettazione = $(cTipoAccettazione).val();
            break;

        case enum_LavCod.Contratto_Affitto.value:

            if (testataDoc.DocumentoContrattoAffitto === undefined || testataDoc.DocumentoContrattoAffitto === null) {
                testataDoc.DocumentoContrattoAffitto = new Object();
                testataDoc.DocumentoContrattoAffitto.IdMov = 0;
            }
            testataDoc.DocumentoContrattoAffitto.DocNumeroSin = $("#inNumDocSin2").val().toUpperCase();
            testataDoc.DocumentoContrattoAffitto.DocNumero = Get_KendoNumTBValue("inNumDoc2");
            testataDoc.DocumentoContrattoAffitto.DocNumeroDes = $("#inNumDocDes2").val().toUpperCase();
            testataDoc.DocumentoContrattoAffitto.DataInizioValidita = kendo.parseDate($("#inDataInizVal").val());
            testataDoc.DocumentoContrattoAffitto.DataFineValidita = kendo.parseDate($("#inDataFineVal").val());
            testataDoc.DocumentoContrattoAffitto.AltriLocatori = $("#inAltriLocatori").val();
            testataDoc.DocumentoContrattoAffitto.RiferimentoOrdini = $("#inRifOrdini").val();            

        default:
            testataDoc.CodRisUmAltro = 0;
            testataDoc.SecondaCooperativa = 0;
            break;
    }

    return testataDoc;
}

function AggiornaValoriPostSalvaAgenda(testataDoc, cauMov, rispServer) {

    let msgOk = TraduzioneMultiResx(resxObj, "SalvataggioEffettuatoCorrettamente", "Salvataggio effettuato correttamente");

    //devo rimettere i valori salvati nell'oggetto e ricreare il json
    testataDoc.IdAgenda = parseInt(rispServer.Id_Agenda);
    testataDoc.IdMov = parseInt(rispServer.Id_Mov_Testata);
    testataDoc.DataOraUltimaLettura = kendo.parseDate(rispServer.Time);
    testataDoc.DescrizioneAgenda = rispServer.Des_Lib;

    //TODO: se ho generato il numero lato server, lo devo impostare in interfaccia
    if (rispServer.Doc_Numero !== undefined && rispServer.Doc_Numero !== null) {

        //se il numero era 0, quindi ho generato io un numero lo specifico nel messaggio
        if (Get_KendoNumTBValue("inNumDoc") === 0) {
            msgOk = kendo.format(TraduzioneMultiResx(resxObj, "SalvataggioDocumentoNumeroCorretto",
                "Salvataggio del nuovo documento numero {0} effettuato correttamente."), rispServer.Doc_Numero_Visualizzato);
        }

        testataDoc.DocNumero = rispServer.Doc_Numero;
        testataDoc.DocNumeroVisualizzato = rispServer.Doc_Numero_Visualizzato;
        Set_KendoNumTBValue("inNumDoc", testataDoc.DocNumero);
        $("#inNumDocShow").val(testataDoc.DocNumeroVisualizzato);
    }
    $("#inNumDocLock").data("kendoSwitch").enable(true); //Ri-abilito il lock
    RiBloccoNumeroDoc(true, "inNumDocDDL", "inNumDoc", "inNumDocSin", "inNumDocDes", "inNumDocLock");    //Avendo salvato blocco il numero doc

    if (rispServer.Progr_Protocollo !== undefined && rispServer.Progr_Protocollo !== null) {
        Set_KendoNumTBValue("inNumProtocollo", rispServer.Progr_Protocollo);
    }

    //devo impostare campi che ho contro-aggiornato con le righe
    if (cauMov !== "" && rispServer.ConteggiTotali !== undefined && rispServer.ConteggiTotali !== null) {

        testataDoc.Colli = parseInt(rispServer.ConteggiTotali.Colli);
        testataDoc.TipoPeso = 0;  // fisso a peso lordo
        testataDoc.Peso = kendo.parseFloat(rispServer.ConteggiTotali.PesoLordoDoc);
        testataDoc.PesoNettoProd = kendo.parseFloat(rispServer.ConteggiTotali.PesoNettoProd);
        testataDoc.TaraImballi = kendo.parseFloat(rispServer.ConteggiTotali.TaraImballi);

        testataDoc.ImballiVuoti = kendo.parseFloat(rispServer.ConteggiTotali.ImballiVuoti);

        //In teoria tara veicolo è quella che avevo già scritto, quindi non ci sarebbe bisogno di sovrascriverla
        testataDoc.TaraVeicolo = kendo.parseFloat(rispServer.ConteggiTotali.TaraVeicolo);
        testataDoc.PesoTaraTrasporto = kendo.parseFloat(rispServer.ConteggiTotali.TaraVeicolo);

        Set_KendoNumTBValue("inNumeroColli", testataDoc.Colli);

        //La parte di aggiornamento della UI dei riepiloghi verrà già fatta nel momento in cui ricarica la griglia dei movimenti

    }

    if (testataDoc.DocumentoAccettazione !== undefined && testataDoc.DocumentoAccettazione !== null) {
        testataDoc.DocumentoAccettazione.IdMov = parseInt(rispServer.Id_Mov_Secondario);
    }

    if (testataDoc.DocumentoContrattoAffitto !== undefined && testataDoc.DocumentoContrattoAffitto !== null) {
        testataDoc.DocumentoContrattoAffitto.IdMov = parseInt(rispServer.Id_Mov_Secondario);
    }

    //Marco: Aggiornamento Riepiloghi Castelletto
    if (rispServer.ConteggiTotali !== null) {

        let riepilogoImporti = rispServer.ConteggiTotali.RiepilogoImporti;

        Set_KendoNumTBValue("idImponibileTotaleLordo", riepilogoImporti.ImponibileLordo);
        Set_KendoNumTBValue("idVariazione", riepilogoImporti.Variazioni);
        Set_KendoNumTBValue("idImponibileNetto", riepilogoImporti.ImponibileNetto);
        Set_KendoNumTBValue("idImposta", riepilogoImporti.Imposta);
        Set_KendoNumTBValue("idTotale", riepilogoImporti.TotaleDocumento);

        testataDoc.TotaleDocumento = riepilogoImporti.TotaleDocumento;

        //Rileggo il datasource della griglia Operazioni/Attivita
        let gridCasteletto = $("#tab_griglia_castelletto").data("kendoGrid");
        if (gridCasteletto !== undefined) {
            //Utilizzo il setDatasource per aggiornare i filtri della colonna se precedentemente erano
            //stati selezionati.

            let ds = new kendo.data.DataSource({ data: JSON.parse(rispServer.ConteggiTotali.RiepilogoCastelletto) });
            gridCasteletto.setDataSource(ds);
            gridCasteletto.dataSource.read();

        }
    }
    //

    //Se ho scritto anche il movimento di magazzino, devo salvarmi la chiave generata
    switch (cauMov) {

        case CAU_CARICO:
            if (testataDoc.MovimentoCarico === undefined || testataDoc.MovimentoCarico === null) {
                testataDoc.MovimentoCarico = rispServer.MovimentoMagazzino;
                $(cId_Mov_BC_Principale).val(testataDoc.MovimentoCarico.IdMov);       //Usato in BeniConfezionamentoUC
            }
            break;

        case CAU_SCARICO:
            if (testataDoc.MovimentoScarico === undefined || testataDoc.MovimentoScarico === null) {
                testataDoc.MovimentoScarico = rispServer.MovimentoMagazzino;
                $(cId_Mov_BC_Principale).val(testataDoc.MovimentoScarico.IdMov);      //Usato in BeniConfezionamentoUC
            }
            break;

        case CAU_TRASFERIMENTO:
            if ((testataDoc.MovimentoScarico === undefined || testataDoc.MovimentoScarico === null) &&
                (testataDoc.MovimentoCarico === undefined || testataDoc.MovimentoCarico === null)) {

                //In questo caso dovrei avrei scritto entrambi i movimenti di carico e scarico
                //TODO: devo restituirmeli in due oggetti diversi
                testataDoc.MovimentoScarico = rispServer.MovimentoMagazzino;
                testataDoc.MovimentoCarico = rispServer.MovimentoMagazzino;
            }
            break;
    }

    $(cIdAgenda).val(testataDoc.IdAgenda);

    //è da cambiare anche il valore di cIdTipoOp (anche il campo nascosto), mettendo modifica
    cIdTipoOp = enum_TipoOperazioneDB.Modifica.value;
    $('input[name$="hdTipoOp"]').val(enum_TipoOperazioneDB.Modifica.value);

    $('input[name$="hdKendo_TestataDoc"]').val(kendo.stringify(testataDoc));

    let newTitle = "Modifica " + testataDoc.DescrizioneAgenda;
    if (document.title !== newTitle) {
        document.title = newTitle;
    }

    //Se contratti di affitto, dopo il salvataggio viene visualizzato il tab riferimenti catastali
    if (cIdLavCod === enum_LavCod.Contratto_Affitto.value) {
        $("#a_tabRifCatastali").show();
    }

    return msgOk;
}

function AggiornaValoriPostSalvaDettaglio(testataDoc, rispServer) {

    //devo rimettere i valori salvati nell'oggetto e ricreare il json


    //TODO: devo impostare  campi che ho contro-aggiornato con le righe 

    //TODO: meglio prima impostare in interfaccia?!?

    testataDoc.DataOraUltimaLettura = kendo.parseDate(rispServer.Time);

    if (rispServer.ConteggiTotali !== undefined && rispServer.ConteggiTotali !== null) {

        testataDoc.Colli = parseInt(rispServer.ConteggiTotali.Colli);
        testataDoc.TipoPeso = 0;  // fisso a peso lordo
        testataDoc.Peso = kendo.parseFloat(rispServer.ConteggiTotali.PesoLordoDoc);
        testataDoc.PesoNettoProd = kendo.parseFloat(rispServer.ConteggiTotali.PesoNettoProd);
        testataDoc.TaraImballi = kendo.parseFloat(rispServer.ConteggiTotali.TaraImballi);

        testataDoc.ImballiVuoti = kendo.parseFloat(rispServer.ConteggiTotali.ImballiVuoti);

        //In teoria tara veicolo è quella che avevo già scritto, quindi non ci sarebbe bisogno di sovrascriverla
        testataDoc.TaraVeicolo = kendo.parseFloat(rispServer.ConteggiTotali.TaraVeicolo);
        testataDoc.PesoTaraTrasporto = kendo.parseFloat(rispServer.ConteggiTotali.TaraVeicolo);

        Set_KendoNumTBValue("inNumeroColli", testataDoc.Colli);

        //La parte di aggiornamento della UI dei riepiloghi verrà già fatta nel momento in cui ricarica la griglia dei movimenti

        ////Aggiorno in interfaccia il riepilogo pesi ed altri campi (su db sono già stati sistemati)

        //Set_KendoNumTBValue("inPesoTotale", testataDoc.Peso);
        //Set_KendoDDLValue("inTipoPeso", testataDoc.TipoPeso);

        ////In teoria tara veicolo è quella che avevo già scritto, quindi non ci sarebbe bisogno di sovrascriverla
        //Set_KendoNumTBValue("inPesoTaraTrasporto", testataDoc.PesoTaraTrasporto);

        //let degrado = 0;
        //if ($("#tab_elenco_movimenti").data("kendoGrid").columns.some(function (column) { return column.field === "Degrado_Calcolato"; })) {
        //    degrado = kendo.parseFloat($("#tab_elenco_movimenti").data("kendoGrid").dataSource.aggregates().Degrado_Calcolato.sum);
        //}

        ////Imposto i campi di Riepilogo pesi ed aggiorno i conteggi
        //ImpostaControlliRiepilogoPesi(
        //    rispServer.ConteggiTotali.PesoLordoDoc,
        //    rispServer.ConteggiTotali.TaraVeicolo,
        //    rispServer.ConteggiTotali.PesoLordoProd,
        //    rispServer.ConteggiTotali.TaraImballi,
        //    rispServer.ConteggiTotali.PesoNettoProd,
        //    degrado,
        //    0);

        //RicalcolaRiepilogoPesi(false);

    }

    $('input[name$="hdKendo_TestataDoc"]').val(kendo.stringify(testataDoc));

}

function SalvaTestataDoc() {

    let testataDoc = GeneraOggettoTestata();

    let ris = undefined;
    if (testataDoc.IdAgenda !== 0) {
        //UPDATE
        ris = ModificaTestataDocumentoContab($(cIdPiva).val(), testataDoc, true);
    } else {
        //INSERT
        ris = ScriviTestataDocumentoContab($(cIdPiva).val(), testataDoc, true);
    }
    
    if (ris !== undefined && ris !== null && ris.Risultato === true) {

        let msgOk = TraduzioneMultiResx(resxObj, "SalvataggioTestataDocumentoCorretto", "Salvataggio testata documento effettuato correttamente");

        msgOk = AggiornaValoriPostSalvaAgenda(testataDoc, "", ris);

        MessaggioTuttoOK_Bootstrap(msgOk, "DIV_Messaggi");
    }
}

function ImpostaDefault(impJson, lavCod) {
    switch (lavCod) {
        case enum_LavCod.DDT_Emesso.value:  //1031 = Bolla emessa
            $("#inNoteTestata").val(GetPropertyFromJson(impJson, "SUPERUSER_COD_NOTE_DEFAULT_BOLLA_EMESSA"));
            Set_RadioGroupValue("inFormatiStampa", GetPropertyFromJson(impJson, "SuperUser_LayOut_FormatiStampa_DDT"));
            break;
        case enum_LavCod.Fattura_Emessa.value:  //1001 = Fattura emessa
            $("#inNoteTestata").val(GetPropertyFromJson(impJson, "SUPERUSER_COD_NOTE_DEFAULT_FATTURA_EMESSA"));
            break;
    }

    //Forzo il default della scadenza unica su true
    if (lavCodOrdine) {
        setKendoSwitch("inScadenzaUnica", true);
        KendoSwitch("inScadenzaUnica").trigger("change");
    }

    //Imposto la causale di trasporto in base al lav_cod
    //TODO: integrare questo codice impostando il default che arriva dalla tabella dei numeratori se definito

    switch (true) {

        // Causale default = "AFFITTO PARTICELLE CATASTALI"
        case (isContrattoAffitto()):
            Set_KendoDDLValue(idControlloCausaleTrasporto, 35);
            break;

         // Causale default = "CONTO CONFERIMENTO"
        case (lavCodAccettazione):
            Set_KendoDDLValue(idControlloCausaleTrasporto, 8);
            break;

        // Causale default = "ACQUISTO"
        case (isLavCodCarico(lavCod)):
            Set_KendoDDLValue(idControlloCausaleTrasporto, 2);
            break;

        // Causale default = "VENDITA"
        default:
            Set_KendoDDLValue(idControlloCausaleTrasporto, 1);
            break;
    }

    if (lavCodDocEmesso === true) {
        Set_KendoNumTBValue("inNumDoc", 0);
    }

    if (lavCodMovMagazzino === false && !isContrattoAffitto()) {
        Set_RadioGroupValue("inTrasportoCura", GetPropertyFromJson(impJson, "SUPERUSER_COD_MODALITA_TRASPORTO_DEFAULT"));
    }

}

function ImpostaOpzioniVisibilitaGenerale(impJson) {

    let agenti = GetPropertyFromJson(impJson, "SUPERUSER_COD_GESTIONE_AGENTI");
    if (agenti === false) {
        $("#groupAgente").hide();
        $("#groupAgenteProvvigione").hide();
        $("#groupAgenteProvvigionePerc").hide();
        $("#groupNuovoModificaAgente").hide();
    } else {
        $("#groupAgente").show();
        $("#groupAgenteProvvigione").show();
        $("#groupAgenteProvvigionePerc").show();
        $("#groupNuovoModificaAgente").show();
    }

    let capoArea = GetPropertyFromJson(impJson, "SUPERUSER_COD_GESTIONE_CAPOAREA");
    if (capoArea === false) {
        $("#groupCapoArea").hide();
        $("#groupCapoAreaProvvigione").hide();
        $("#groupCapoAreaProvvigionePerc").hide();
        $("#groupNuovoModificaCapoArea").hide();
    } else {
        $("#groupCapoArea").show();
        $("#groupCapoAreaProvvigione").show();
        $("#groupCapoAreaProvvigionePerc").show();
        $("#groupNuovoModificaCapoArea").show();
    }

    if (agenti === false && capoArea === false)
        $("#panelBarProvvigioni").hide();
    else
        $("#panelBarProvvigioni").show();

    let modificaOperatore = GetPropertyFromJson(impJson, "SUPERUSER_OPERATORE_ACCETTAZIONE");
    if (modificaOperatore === true)
        Cmb_Operatore.enable(true);
    else
        Cmb_Operatore.enable(false);

}

function nascondiControlliVisualizzazioneSemplificata() {
    //Visualizzazione semplificata
    if (gestioneContabilita === enum_Livello_GestContabilita_NonGestita) {
        $("#btnRefreshNumDoc").hide();
        $("#cardDocSecondario").hide();
        $("#rowNumDocSecondario").hide();
        $("#rowDataSecondaria").hide();
        $("#groupDataRegistrazione").hide();
        $("#groupNumRegistrazione").hide();
        $("#groupNumProtocollo").hide();
        $("#groupSezionale").hide();

        $("#groupAspettoBeni").hide();
        $("#groupCausaleTrasporto").hide();
        $("#groupNumeroColli").hide();
        $("#groupPesoTotale").hide();
        $("#groupNaturaBeni").hide();
    }
}

function ImpostaOpzioniVisibilitaOperazione(impJson, lavCod) {

    //Date e numeri nei casi di accettazione
    switch (lavCod) {

        case enum_LavCod.Scarico_Magazzino.value:
        case enum_LavCod.Carico_Magazzino.value:
            $("#lbl_data_emissione").html(TraduzioneMultiResx(resxObj, "Data", "Data") + ":");
            $("#lbl_num_protocollo").html(TraduzioneMultiResx(resxObj, "NumMovimento", "Num. movim.") + ":");
            break;

        case enum_LavCod.Contratto_Affitto.value:
            $("#lbl_des_num_doc").html(TraduzioneMultiResx(resxObj, "NumRegistrazione", "Num. registrazione") + ":");
            $("#lbl_data_emissione").html(TraduzioneMultiResx(resxObj, "DataRegistrazione", "Data registrazione") + ":");
            $("#cardDocSecondario").show();
            $("#rowNumDocSecondario").show();
            $("#lbl_des_num_doc2").html(TraduzioneMultiResx(resxObj, "NumInterno", "Num. interno") + ":");
            $("#inNumDoc2").attr("required", "");
            break;

        case enum_LavCod.Accettazione_da_diversi.value:
        case enum_LavCod.Auto_Ddt_Emesso_Accettazione.value:

            $("#lbl_data_emissione").html(TraduzioneMultiResx(resxObj, "DataAccettazione", "Data Accettazione") + ":");
            $("#lbl_des_num_doc").html(TraduzioneMultiResx(resxObj, "NumAccettazione", "N° Accettazione") + ":");
            $("#cardDocSecondario").show();
            $("#rowNumDocSecondario").show();
            $("#lbl_des_num_doc2").html(TraduzioneMultiResx(resxObj, "NumeroDDT", "Num. DDT") + ":");
            $("#inNumDoc2").attr("required", "");
            $("#rowDataSecondaria").show();
            $("#lbl_data_emissione2").html(TraduzioneMultiResx(resxObj, "DataEmissioneDDT", "Data Emissione DDT") + ":");
            $("#inDataEmissione2").attr("required", "");
            KendoDate("inDataEmissione2").bind("change", inDataEmissione2_change);
            break;

        //case enum_LavCod.Distinta_Carico.value:

        //    $("#lbl_des_num_doc").html("Progressivo Carico:");
        //    $("#cardDocSecondario").hide();
        //    $("#rowNumDocSecondario").hide();
        //    $("#inNumDoc2").removeAttr("required");
        //    $("#rowDataSecondaria").hide();
        //    $("#inDataEmissione2").removeAttr("required");
        //    break;

        case enum_LavCod.Distinta_Carico_Accettazione.value:

            $("#lbl_data_emissione").html(TraduzioneMultiResx(resxObj, "DataAccettazione", "Data Accettazione") + ":");
            $("#lbl_des_num_doc").html(TraduzioneMultiResx(resxObj, "NumAccettazione", "N° Accettazione") + ":");
            $("#cardDocSecondario").show();
            $("#rowNumDocSecondario").show();
            $("#lbl_des_num_doc2").html(TraduzioneMultiResx(resxObj, "ProgressivoCarico", "Progressivo Carico") + ":");
            $("#inNumDoc2").attr("required", "");
            $("#rowDataSecondaria").show();
            $("#lbl_data_emissione2").html(TraduzioneMultiResx(resxObj, "DataEmissioneDistintaCarico", "Data emissione distinta carico") + ":");
            $("#inDataEmissione2").attr("required", "");
            KendoDate("inDataEmissione2").bind("change", inDataEmissione2_change);
            break;

        default:
            $("#lbl_des_num_doc").html(TraduzioneMultiResx(resxObj, "NumeroDocumento", "Numero Documento") + ":");
            $("#cardDocSecondario").hide();
            $("#rowNumDocSecondario").hide();
            $("#inNumDoc2").removeAttr("required");
            $("#rowDataSecondaria").hide();
            $("#inDataEmissione2").removeAttr("required");
            break;
    }

    //Num protocollo
    switch (lavCod) {
        case enum_LavCod.Fattura_Ricevuta.value:
        case enum_LavCod.Fattura_Emessa.value:
        case enum_LavCod.Nota_Accredito_Ricevuta.value:
        case enum_LavCod.Nota_Accredito_Emessa.value:
        case enum_LavCod.Carico_Magazzino.value:
        case enum_LavCod.Scarico_Magazzino.value:
            $("#groupNumProtocollo").show();
            break;
        default:
            $("#groupNumProtocollo").hide();
            break;
    }

    //Scadenza e Numerazione Ordine
    switch (lavCod) {
        case enum_LavCod.Ordine_Vendita_Emesso.value:
        case enum_LavCod.Ordine_Acquisto.value:

            $("#panelBarScadenzaOrdine").show();
            $("#panelBarNumerazioneOrdine").show();

            var rifOrdineEnte = GetPropertyFromJson(impJson, "SuperUser_RifDoc_EnteConsorzio");
            if (rifOrdineEnte === true) {
                $("#groupNOrdineConsorzio").show();
                $("#groupAnnoOrdineConsorzio").show();
            } else {
                $("#groupNOrdineConsorzio").hide();
                $("#groupAnnoOrdineConsorzio").hide();
            }

            KendoDate("inDataEvasionePrevista").bind("change", inDataEvasionePrevista_change);

            $("#groupScadenzaUnica").show();
            //$("#groupDataEvasionePrevista").show();
            //$("#groupEvasioneTassativa").show();
            //$("#groupDataSpedizionePrevista").show();

            //li lascio nascosti, perché anche se ordine, devono comparire solo se scadenza unica è true
            $("#groupDataEvasionePrevista").hide();
            $("#groupEvasioneTassativa").hide();
            $("#groupDataSpedizionePrevista").hide();

            $("#groupDataSpedizione").hide();
            $("#groupNaturaBeni").hide();
            break;
            
        default:

            $("#panelBarScadenzaOrdine").hide();
            $("#panelBarNumerazioneOrdine").hide();
            //$("#groupScadenzaUnica").hide();
            //$("#groupDataEvasionePrevista").hide();
            //$("#groupEvasioneTassativa").hide();
            //$("#groupDataSpedizionePrevista").hide();
            break;
    }

    // Particolarità movimenti di magazzino
    switch (lavCod) {
        case enum_LavCod.Carico_Magazzino.value:
        case enum_LavCod.Scarico_Magazzino.value:
            $("#groupDataSpedizione").hide();
            $("#panelBarDestinatario").hide();
            $("#panelBarDatiTrasporto").hide();
            $("#panelBarAllegatiDocumento").hide();
            $("#a_tabRiepilogoPesi").hide();
            $("#a_tabBeniConfezionamento").hide(); //Tab Imballi
            $("#cardInfoTrasporto").hide();
            $("#inNumDoc").removeAttr("required");
            $("#inNumDocGroup").hide();
            $("#groupTipologiaDocumento").hide();
    }

    //Tab castelletto
    ImpostaVisibilitaTabCastelletto();

    let idAgenda = parseInt($(cIdAgenda).val());

    // Particolarità contratti affitto
    switch (lavCod) {

        case enum_LavCod.Contratto_Affitto.value:
            $("#groupDataSpedizione").hide();
            $("#panelBarDestinatario").hide();
            $("#panelBarDatiTrasporto").hide();
            $("#panelBarAllegatiDocumento").hide();
            $("#a_tabRiepilogoPesi").hide();
            $("#a_tabBeniConfezionamento").hide(); //Tab Imballi
            $("#a_tabCastelletto").hide();
            $("#cardInfoTrasporto").hide();
            if (idAgenda !== undefined && idAgenda !== null && !isNaN(idAgenda) && idAgenda !== 0) {
                $("#a_tabRifCatastali").show();
            } else {
                $("#a_tabRifCatastali").hide();
            }
            break;

        default:
            $("#a_tabRifCatastali").hide();
            break;

    }

    //Tab layout
    switch (lavCod) {

        case enum_LavCod.DDT_Emesso.value:
            $("#groupFormatiStampa").show();
            $("#a_tabLayoutDoc").show();
            break;

        case enum_LavCod.Fattura_Emessa.value:
            //TODO: va ancora gestito correttamente per la fattura, perché cambiano le opzioni
            $("#groupFormatiStampa").hide();
            $("#a_tabLayoutDoc").hide();
            break;

        default:
            $("#a_tabLayoutDoc").hide();
            break;
    }

    //Num doc principale vincolato o libero
    if (lavCodDocEmesso === true) {
        //doc emessi = numero doc vincolato

        KendoDDL("inNumDocDDL").wrapper.show();
        $("#inNumDocShow").show();
        RiBloccoNumeroDoc(true, "inNumDocDDL", "inNumDoc", "inNumDocSin", "inNumDocDes", "inNumDocLock");

    } else {

        //doc ricevuto, ho solo i tre campi per il numero, svincolati

        KendoDDL("inNumDocDDL").wrapper.hide();
        $("#inNumDocShow").hide();

        setKendoSwitch("inNumDocLock", false);
        setKendoSwitchVisible("inNumDocLock", false);

        //doc ricevuti = numero doc libero
        //$("#inNumDoc").removeAttr("disabled");
    }

    //TODO: DataLock
    if (idAgenda > 0 && (Qs_CaricoScarico === CAU_CARICO || lavCod === enum_LavCod.Scarico_Magazzino.value)) {

        //Sono in modifica di agenda con:
        //- movimento carico  : Carico magazzino, DDT, Fattura, ecc.
        //- movimento scarico : Scarico magazzino (solo specifico lavCod)
        $("#groupDataLock").show();

        //La modifica della data è sempre bloccata in modifica
        RiBloccoDataDoc(true, "inDataEmissione", "inDataLock");

    } else {

        $("#groupDataLock").hide();

    }

    /*
    
    btn_nuovo_cedente_cessionario_1
    btn_modifica_cedente_cessionario_1
    btn_nuovo_vettore
    btn_modifica_vettore

    I bottoni partono visibili:

    - li devo nascondere se l'utente non ha il permesso
    - altrimenti, se ha il permesso, li mostro/nascondo in base alle logiche del codice
    - in particolare in caso di accettazione con gerarchia, mostro la possibilità di modificare il contatto principale ma non di crearlo

    */

    if ($("input[name$='hf_utenteAbilitatoContattiScrittura']").val() == "True") {
        switch (true) {

            case isMovimentoMagazzino(lavCod):
                $("#groupContatto1").hide();
                $("#btn_nuovo_cedente_cessionario_1").hide();
                $("#btn_modifica_cedente_cessionario_1").hide();
                $("#groupPiva1").hide();
                $("#groupProgressivo1").hide();
                break;

            case (lavCodAccettazione === true && contattiAcc4ConGerarchia === true):
                //nascondo bottone Nuovo contatto se conferente con gerarchia
                $("#btn_nuovo_cedente_cessionario_1").hide();
                $("#btn_modifica_cedente_cessionario_1").show();
                $("#btn_nuovo_vettore").show();
                $("#btn_modifica_vettore").show();
                break;

            default:
                $("#btn_nuovo_cedente_cessionario_1").show();
                $("#btn_modifica_cedente_cessionario_1").show();
                $("#btn_nuovo_vettore").show();
                $("#btn_modifica_vettore").show();
                break;
        }
    }
    else {
        $("#btn_nuovo_cedente_cessionario_1").hide();
        $("#btn_modifica_cedente_cessionario_1").hide();
        $("#btn_nuovo_vettore").hide();
        $("#btn_modifica_vettore").hide();
    }

}

function ImpostaVisibilitaTabCastelletto() {

    if ($("input[name$='hf_UtenteAbilitatoGestionePrezziLettura']").val() !== "True" || isMovimentoMagazzino(cIdLavCod) === true || isContrattoAffitto()) {

        $("#a_tabCastelletto").hide();

    } else {

        $("#a_tabCastelletto").show();

    }

}

function addNewCausaleTrasporto(widgetId, value) {
    let widget = KendoDDL(widgetId);
    let dataSource = widget.dataSource;

    //TODO: mettere sempre ADD? se ne avevo già inserito 1, ho già la riga con codice 0, quindi potrei modificarla?!?
    dataSource.add({
        Causale_Trasporto_Des: value,
        Causale_Trasporto_Cod: 0
    });

    dataSource.one("sync", function () {
        widget.select(dataSource.view().length - 1);
    });

    dataSource.sync();
    widget.select(function (dataItem) {
        return dataItem.Causale_Trasporto_Des === value;
    });

    widget.trigger("change");
    widget.close();

}

function addNewMezzoTrasporto(widgetId, value) {

    let vettore = Cmb_Vettore.dataItem();

    if (vettore === undefined || vettore === null) {
        MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "SelezionaVettorePerMezzo", "Selezionare prima il vettore a cui associare il mezzo."), "DIV_Messaggi");
        return;
    }

    let targa = value.toUpperCase();

    let macDes = vettore.Rag_Soc + " " + targa;

    AggiungiMacchinaDaTarga(vettore.Piva_Proprietaria, vettore.Sa_Cod, vettore.Cod_Contatto, targa, macDes);


    //let widget = KendoDDL(widgetId);
    //let dataSource = widget.dataSource;

    ////TODO: mettere sempre ADD? se ne avevo già inserito 1, ho già la riga con codice 0, quindi potrei modificarla?!?
    //dataSource.add({
    //    Targa: value,
    //    Mac_Cod: 0
    //});

    //dataSource.one("sync", function () {
    //    widget.select(dataSource.view().length - 1);
    //});

    //dataSource.sync();
    //widget.select(function (dataItem) {
    //    return dataItem.Targa === value;
    //});

    //widget.trigger("change");
    //widget.close();

}

function CreaTipoDocumento(lavCod, accettazione) {
    let arrayLavCod = [];
    if (accettazione === true) {

        //arrayLavCod.push(addLavCodArray(enum_LavCod.DDT_Ricevuto));
        arrayLavCod.push(addLavCodArray(enum_LavCod.Accettazione_da_diversi));
        //arrayLavCod.push(addLavCodArray(enum_LavCod.Distinta_Carico));
        arrayLavCod.push(addLavCodArray(enum_LavCod.Distinta_Carico_Accettazione));
        //arrayLavCod.push(addLavCodArray(enum_LavCod.Auto_Ddt_Emesso));
        arrayLavCod.push(addLavCodArray(enum_LavCod.Auto_Ddt_Emesso_Accettazione));

    } else {

        switch (lavCod) {
            case enum_LavCod.DDT_Ricevuto.value:
                arrayLavCod.push(addLavCodArray(enum_LavCod.DDT_Ricevuto));
                break;

            case enum_LavCod.DDT_Emesso.value:
            case enum_LavCod.DDT_Contabilizzato_Emesso.value: 
                arrayLavCod.push(addLavCodArray(enum_LavCod.DDT_Emesso));
                arrayLavCod.push(addLavCodArray(enum_LavCod.DDT_Contabilizzato_Emesso));
                break;

            case enum_LavCod.Ordine_Vendita_Emesso.value:
                arrayLavCod.push(addLavCodArray(enum_LavCod.Ordine_Vendita_Emesso));
                break;
            
            case enum_LavCod.Ordine_Acquisto.value:
                arrayLavCod.push(addLavCodArray(enum_LavCod.Ordine_Acquisto));
                break;

            case enum_LavCod.Corrispettivo_Vendita.value:
                arrayLavCod.push(addLavCodArray(enum_LavCod.Corrispettivo_Vendita));
                break;

            case enum_LavCod.Fattura_Emessa.value:
                arrayLavCod.push(addLavCodArray(enum_LavCod.Fattura_Emessa));
                break;

            case enum_LavCod.Fattura_Ricevuta.value:
                arrayLavCod.push(addLavCodArray(enum_LavCod.Fattura_Ricevuta));
                break;

            case enum_LavCod.Carico_Magazzino.value:
                arrayLavCod.push(addLavCodArray(enum_LavCod.Carico_Magazzino));
                break;

            case enum_LavCod.Scarico_Magazzino.value:
                arrayLavCod.push(addLavCodArray(enum_LavCod.Scarico_Magazzino));
                break;

            case enum_LavCod.Contratto_Affitto.value:
                arrayLavCod.push(addLavCodArray(enum_LavCod.Contratto_Affitto));
                break;

            case enum_LavCod.Nota_Accredito_Ricevuta.value:
                arrayLavCod.push(addLavCodArray(enum_LavCod.Nota_Accredito_Ricevuta));
                break;

            case enum_LavCod.Nota_Accredito_Emessa.value:
                arrayLavCod.push(addLavCodArray(enum_LavCod.Nota_Accredito_Emessa));
                break;
        }

    }

    if (arrayLavCod.length > 0) {
        arrayLavCod.sort(function(a, b) {
            return a.text === b.text ? 0 : +(a.text > b.text) || -1;
        });
    } else {
        //TODO: nascondi la dropdown!!!
    }

    return arrayLavCod;
}

function addLavCodArray(enumLavCodItem) {
    return {
        text: enumLavCodItem.name + " [" + enumLavCodItem.value + "]",
        value: enumLavCodItem.value
    };
}

function isLavCodAccettazione(lavCod) {
    let isAccettazione = false;

    switch (lavCod) {
        case enum_LavCod.Accettazione_da_diversi.value:
        case enum_LavCod.Distinta_Carico_Accettazione.value:
        case enum_LavCod.Auto_Ddt_Emesso_Accettazione.value:
            isAccettazione = true;
            break;
        case enum_LavCod.Distinta_Carico.value:
        case enum_LavCod.Auto_Ddt_Emesso.value:
            isAccettazione = true;
            break;
    }

    return isAccettazione;
}

function isLavCodCarico(lavCod) {
    let isLavCodCarico = false;

    switch (lavCod) {
        case enum_LavCod.Accettazione_da_diversi.value:
        case enum_LavCod.Distinta_Carico_Accettazione.value:
        case enum_LavCod.Auto_Ddt_Emesso_Accettazione.value:
        case enum_LavCod.Ordine_Acquisto.value:
        case enum_LavCod.DDT_Ricevuto.value:
        case enum_LavCod.Fattura_Ricevuta.value:
        case enum_LavCod.Nota_Accredito_Ricevuta.value: // Gestita dal punto di vista contabile e non di magazzino
            isLavCodCarico = true;
            break;
        case enum_LavCod.Ordine_Vendita_Emesso.value:
        case enum_LavCod.DDT_Emesso.value:
        case enum_LavCod.DDT_Contabilizzato_Emesso.value:
        case enum_LavCod.Fattura_Emessa.value:
        case enum_LavCod.Nota_Accredito_Emessa.value: // Gestita dal punto di vista contabile e non di magazzino
            isLavCodCarico = false;
            break;
    }

    return isLavCodCarico;
}

function isLavCodOrdine(lavCod) {
    let isLavCodOrdine = false;

    switch (lavCod) {
        case enum_LavCod.Ordine_Vendita_Emesso.value:
        case enum_LavCod.Ordine_Acquisto.value:
            isLavCodOrdine = true;
            break;
        default:
            isLavCodOrdine = false;
            break;
    }

    return isLavCodOrdine;
}



function isLavCodFattura(lavCod) {
    let isLavCodFattura = false;

    switch (lavCod) {
        case enum_LavCod.Fattura_Emessa.value:
        case enum_LavCod.Nota_Accredito_Emessa.value:
            isLavCodFattura = true;
            break;
        case enum_LavCod.Fattura_Ricevuta.value:
        case enum_LavCod.Nota_Accredito_Ricevuta.value:
            isLavCodFattura = true;
            break;
        default:
            isLavCodFattura = false;
            break;
    }

    return isLavCodFattura;
}

function isLavCodNotaAccredito(lavCod) {
    let isLavCodNotaAccredito = false;

    switch (lavCod) {
        case enum_LavCod.Nota_Accredito_Emessa.value:
            isLavCodNotaAccredito = true;
            break;
        case enum_LavCod.Nota_Accredito_Ricevuta.value:
            isLavCodNotaAccredito = true;
            break;
        default:
            isLavCodNotaAccredito = false;
            break;
    }

    return isLavCodNotaAccredito;
}

function isNumeroDocumentoEmesso(lavCod) {
    let isNumeroDocumentoEmesso = false;

    switch (lavCod) {
        case enum_LavCod.Accettazione_da_diversi.value:
        case enum_LavCod.Distinta_Carico_Accettazione.value:
        case enum_LavCod.Auto_Ddt_Emesso_Accettazione.value:
        case enum_LavCod.Ordine_Acquisto.value:
            isNumeroDocumentoEmesso = true;
            break;
        case enum_LavCod.Ordine_Vendita_Emesso.value:
        case enum_LavCod.DDT_Emesso.value:
        case enum_LavCod.DDT_Contabilizzato_Emesso.value:
        case enum_LavCod.Fattura_Emessa.value:
        case enum_LavCod.Nota_Accredito_Emessa.value:
            isNumeroDocumentoEmesso = true;
            break;
    }

    return isNumeroDocumentoEmesso;
}

function isDocumentoVendita(lavCod) {
    let isdocvend = false;

    switch (lavCod) {
        case enum_LavCod.Ordine_Vendita_Emesso.value:
        case enum_LavCod.DDT_Emesso.value:
        case enum_LavCod.DDT_Contabilizzato_Emesso.value:
        case enum_LavCod.Fattura_Emessa.value:
            isdocvend = true;
            break;
    }

    return isdocvend;
}

function isMovimentoMagazzino(lavCod) {
    let ismovmag = false;

    switch (lavCod) {
        case enum_LavCod.Carico_Magazzino.value:
        case enum_LavCod.Scarico_Magazzino.value:
            ismovmag = true;
            break;
    }

    return ismovmag;

}

function isContrattoAffitto() {

    return cIdLavCod === enum_LavCod.Contratto_Affitto.value;

}

function getSegno(lavCod, cauMov) {

    let mStrCaption = "";

    //mSegno è il segno della parte contabile (quindi se carico di magazzino, il segno è -)
    let mSegno = "";

    //switch (lavCod) {
    //    case enum_LavCod.Fattura_Ricevuta.value:
    //    case enum_LavCod.DDT_Ricevuto.value:
    //    case enum_LavCod.Accettazione_da_diversi.value:
    //    case enum_LavCod.Distinta_Carico.value:
    //    case enum_LavCod.Distinta_Carico_Accettazione.value:
    //    case enum_LavCod.Auto_Ddt_Emesso.value:
    //    case enum_LavCod.Auto_Ddt_Emesso_Accettazione.value:
    //        mSegno = "-";
    //        break;

    //    default:
    //        mSegno = "+";
    //        break;
    //}


    switch (cauMov) {
        case CAU_CARICO:
            mStrCaption = "Distinta di Carico";
            mSegno = "-";
            break;

        case CAU_SCARICO:
            mStrCaption = "Distinta di Scarico";
            mSegno = "+";
            break;

        //case CAU_CONFERIMENTO:
        //case CAU_ACCETTAZIONE_BENI:
        //    mSegno = "-";
        //    break;

        //case CAU_CONFERIMENTO_DIVERSI:
        //    mSegno = "+";
        //    break;

        case CAU_TRASFERIMENTO:
            mStrCaption = "Distinta di Trasferimento Merci";
            mSegno = "+";   //Fittizio
            break;
    }

    return mSegno;

    //switch (cauMov) {
    //    case CAU_CARICO:
    //    case CAU_CONFERIMENTO:
    //    case CAU_CONFERIMENTO_DIVERSI:
    //    case CAU_ACCETTAZIONE_BENI:

    //        //Check Permessi sui Prodotti Aziendali
    //        //if (ObjUtente.CheckPermesso(CAU_PRODOTTI_AZIENDALI, DBLeggi) && ObjUtente.CheckPermesso(CAU_PRODOTTI_AZIENDALI, DBmodifica)) {
    //        //    TbProdotti.Tools("ID_NuovoProdotto").enabled = true;
    //        //} else {
    //        //    TbProdotti.Tools("ID_NuovoProdotto").enabled = false;
    //        //}
    //        break;

    //    case CAU_SCARICO:
    //    case CAU_TRASFERIMENTO:

    //        //Disabilito possibilità inserimento nuove risorse aziendali
    //        //TbProdotti.Tools("ID_NuovoProdotto").Visible = false;
    //        break;
    //}

}

function getTipoDareAvere(tipoEcoPat, lavCod, cauMov) {

    let bAutofatturaAcquisto = false;
    let bAutofatturaVendita = false;

    let mSegno = getSegno(lavCod, cauMov);

    let pivaContatto = "";
    if (Cmb_Contatto1 !== undefined && Cmb_Contatto1 !== null &&
        Cmb_Contatto1.dataItem() !== undefined && Cmb_Contatto1.dataItem() !== null) {
        pivaContatto = Cmb_Contatto1.dataItem().Cod_Contatto;
    }

    //Controllo Autofattura
    if ($(cIdPiva).val() === pivaContatto && mSegno === "-") {
        //Autofattura Acquisto
        bAutofatturaAcquisto = true;
    }
    if ($(cIdPiva).val() === pivaContatto && mSegno === "+") {
        //Autofattura Vendita
        bAutofatturaVendita = true;
    }


    //con mSegno = "+" sto di fatto indicando gli scarichi di magazzino
    //if (((mPiva(PROVENIENZA) === $(cIdPiva).val() && cIdLavCod !== 1002 && cIdLavCod !== 1056 && cIdLavCod !== 1057) ||
    if (((mSegno === "+" && lavCod !== 1002 && lavCod !== 1056 && lavCod !== 1057) ||
            lavCod === 1003) &&
        !bAutofatturaAcquisto) {

        if (tipoEcoPat === "ECO")
            return "A";
        else
            return "D";
    } else {
        if (tipoEcoPat === "ECO")
            return "D";
        else
            return "A";
    }
}

function SelezionaCentro() {

    var centriAziendali = RicercaCentriAziendali($(cIdPiva).val(), false, 2, true);

    if (centriAziendali.length === 1) {
        
        Qs_SaCod = centriAziendali[0].sa_cod;
        $('input[name$="hf_Qs_SaCod"]').val(Qs_SaCod);
        selezionaCentro = false;

    } else if (centriAziendali.length > 1) {

        $("#ddlCentro").kendoDropDownList({
            filter: "contains",
            dataTextField: "sa_nome",
            dataValueField: "sa_cod",
            dataSource: { transport: { read: function (options) { options.success(centriAziendali); } } },
            open: kendoDropDownAdjustWidth,
            // dataBound: function (e) { kendoDropDownAdjustWidth(e); },
            optionLabel: TraduzioneMultiResx(resxObj, "Seleziona", "Seleziona") + "...",
            change: function (e) {
                Qs_SaCod = this.value() !== "" ? this.value() : "0";
                $('input[name$="hf_Qs_SaCod"]').val(Qs_SaCod);
                $("#btn_selectCentro").data("kendoButton").enable(Qs_SaCod!="0");
            }
        }).data("kendoDropDownList");

        $("#selectCentro").data("kendoWindow").center().open();

    } else {

        kendo.alert(TraduzioneMultiResx(resxObj, "NessunCentroAziendale", "Non sono presenti centri aziendali"));

    }
    
}

function VerificaDatiMinimiTestata() {

    //se non ho già l'interfaccia, con tutti i controlli caricati, mi scatterebbero degli eventi change che non piacciono al validatore
    if (documentoLoaded === true) {

        SvuotaSegnalazioniErrori(true, true, false);

        let isValidIntestazione = validatorIntestazione.validate();
        let isValidTabTestata = validatorTabTestata.validate();

        let datiInseriti = true;
        if (isValidIntestazione === false || isValidTabTestata === false) {
            datiInseriti = false;
            EvidenziaErroriIntestazione();
            EvidenziaErroriTabTestata();
            $("#a_tabTestataDoc").tab("show");
        }
        
        ////Contatto
        //if (parseInt(Get_KendoDDLValue(ddlContatto1Nome, 0)) === 0) {
        //    datiInseriti = false;
        //}

        ////Data emissione
        //if ($("#inDataEmissione").val() === "" || $("#inDataEmissione").data("kendoDatePicker").value() === null) {
        //    datiInseriti = false;
        //}


        if (datiInseriti === false) {

            DisabilitaAltreTab();

        } else {

            //ho inserito tutto, ora riabilito tutte le tab
            RiabilitaAltreTab();

            // Se sono in inserimento imposto i controlli della nuova riga
            if (gestioneContabilita === enum_Livello_GestContabilita_NonGestita &&
                parseInt($(cIdAgenda).val()) === 0 &&
                contattiAcc4ConGerarchia === false) {
                //DocContabileNuovaRiga();

                //imposto direttamente la tab dei dettagli (di conseguenza, non essendoci ancora righe, entrerà direttamente in edit nuova riga)
                //lo faccio solo la prima volta, per evitare che salti nella tab, anche quando l'utente ha cliccato da altre parti.
                if (documentoValidatoPrimaVolta === false &&
                    isLavCodCarico(cIdLavCod) === true &&
                    lavCodAccettazione === false) {

                    $("#a_tabDettagliDoc").tab("show");

                    documentoValidatoPrimaVolta = true;
                }

                //$(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_Riepilogo]).attr("style", "display:none");
                //$(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_Dettaglio]).attr("style", "display:inline-block");
                //$(tabStrip_Dettagli.items()[index_tabStrip_Dettagli_ScaricoGiacenza]).attr("style", "display:none");

                //tabStrip_Dettagli.select(index_tabStrip_Dettagli_Dettaglio);
            }

        }
    }

}

function DisabilitaAltreTab() {
    /*disable non active tabs*/
    $(".nav li").not(".active").addClass("disabled");
    /*to actually disable clicking the bootstrap tab, as noticed in comments by user3067524*/
    $(".nav li").not(".active").find("a").removeAttr("data-toggle");
}

function RiabilitaAltreTab() {
    $(".nav li").not(".active").removeClass("disabled");
    $(".nav li").not(".active").find("a").attr("data-toggle", "tab");
}

function AbilitaModificaDatiMinimiTestata(flagAbilita) {

    KendoDate("inDataEmissione").enable(flagAbilita);
    if (flagAbilita === false && parseInt($(cIdAgenda).val()) === 0) {
        //se ancora non ho salvato nulla, lascio sbloccata la data
        //TODO: DA GESTIRE SULL'ONCHANGE DELLA DATA, IL FATTO CHE POTREI AVERE LA RIGA APERTA E SE CAMBIO LA DATA CI SONO DELLE RIPERCUSSIONI
        KendoDate("inDataEmissione").enable(true);
    }
    KendoDDL(ddlContatto1Nome).enable(flagAbilita);
    $("#" + btnFiltraContatti1XRaccolte).attr("disabled", !flagAbilita);

    if (isRaccolteXConferimentiAttive() && raccolteXConferimenti_dsSelezionate.length > 0) {
        //in questo caso devo bloccare anche il produttore (perché suoi potrebbero essere gli impianti) 
        KendoDDL(ddlContatto2Nome).enable(flagAbilita);
        $("#" + btnFiltraContatti2XRaccolte).attr("disabled", !flagAbilita);

        if (contattiAcc4ConGerarchia === true) {
            //e di conseguenza anche tutti gli altri contatti (altrimenti se si cambia Coop1, viene ricaricato anche il produttore)
            let kddlContattoCoop1Nome = KendoDDL(ddlContattoCoop1Nome);
            if (kddlContattoCoop1Nome !== undefined) {
                kddlContattoCoop1Nome.enable(flagAbilita);
            }

            let kddlContattoCoop2Nome = KendoDDL(ddlContattoCoop2Nome);
            if (kddlContattoCoop2Nome !== undefined) {
                kddlContattoCoop2Nome.enable(flagAbilita);
            }
        }

        //Ora li lasciamo sempre sbloccati → Li blocchiamo per la nuova gestione delle raccolte
    }
    else {
        // Tengo sbloccati gli altri contatti:
        // questo ha conseguenze sulle righe soltanto per i conferimenti a cui viene associata una raccolta automatica.
        // Al salvataggio, la funzione in DocContabile_Eventi.js, ControlliProduttore() blocca se il produttore è stato cambiato senza aggiornare i dati di dettaglio
        KendoDDL(ddlContatto2Nome).enable(true);
        $("#" + btnFiltraContatti2XRaccolte).attr("disabled", false);

        if (contattiAcc4ConGerarchia === true) {
            
            let kddlContattoCoop1Nome = KendoDDL(ddlContattoCoop1Nome);
            if (kddlContattoCoop1Nome !== undefined) {
                kddlContattoCoop1Nome.enable(true);
            }

            let kddlContattoCoop2Nome = KendoDDL(ddlContattoCoop2Nome);
            if (kddlContattoCoop2Nome !== undefined) {
                kddlContattoCoop2Nome.enable(true);
            }
        }
    }

    if (lavCodFattura === true && parseInt($(cIdAgenda).val()) === 0) {
        KendoSwitch("chkAccompagnatoria").enable(flagAbilita);
    }
}

function MostraNumeroDocCompleto(idNumDocDDL, idNumDoc, idNumDocSin, idNumDocDes, idNumDocShow) {
    //ricostruisco il numero completo

    let lunghezzaCentro = 0;
    let carFormat = "";

    let ddlNumDoc = KendoDDL(idNumDocDDL);
    if (ddlNumDoc !== undefined && ddlNumDoc !== null &&
        ddlNumDoc.dataItem() !== undefined &&
        ddlNumDoc.dataItem() !== null) {
        lunghezzaCentro = parseInt(ddlNumDoc.dataItem().Lunghezza_Centro);
        carFormat = ddlNumDoc.dataItem().CarattereFormattazione;
    }

    let numero = Get_KendoNumTBValue(idNumDoc);
    let numPadded = "";
    if (lunghezzaCentro > 0) {
        numPadded = String(numero).padStart(lunghezzaCentro, carFormat);
    } else {
        numPadded = String(numero);
    }

    let prefisso = $("#" + idNumDocSin).val();
    let suffisso = $("#" + idNumDocDes).val();
    let numeroCompleto = prefisso + numPadded + suffisso;

    $("#" + idNumDocShow).val(numeroCompleto);
}

function ImpostaDefaultNumeratore(lavCod, tipoOp, idNumDocDDL, idNumDocSin, idNumDocDes) {

    if (isNumeroDocumentoEmesso(lavCod) === true &&
        KendoDDL(idNumDocDDL) !== undefined &&
        KendoDDL(idNumDocDDL).dataSource.data() !== null &&
        KendoDDL(idNumDocDDL).dataSource.data().length > 0) {

        if (tipoOp === enum_TipoOperazioneDB.Scrittura.value) {

            let dsNumeratori = KendoDDL(idNumDocDDL).dataItems();

            //Se esiste un numeratore default, lo imposto
            let esisteItemCompatibile = dsNumeratori.some(function (dataItem) {
                return dataItem.IsDefault === true;
            });

            if (esisteItemCompatibile === true) {
                KendoDDL(idNumDocDDL).select(function (dataItem) { return dataItem.IsDefault === true; });
                $("#" + idNumDocDDL).trigger("change"); //forzo il change
            } else if (dsNumeratori.length >= 1) {
                //L'elemento è già selezionato, forzo il change
                $("#" + idNumDocDDL).trigger("change");
            }

            //se è vincolante devo impedirne la modifica
            if (KendoDDL(idNumDocDDL).dataItem() !== undefined && KendoDDL(idNumDocDDL).dataItem().Vincolante === true) {
                KendoDDL(idNumDocDDL).enable(false);
            }

        } else {

            //verifico se esiste un elemento che ha stessi prefissi/suffissi del documento che ho aperto
            //così se sblocco il numero, almeno non rischio che mi cambi il prefisso e suffisso da sotto il culo

            let prefisso = $("#" + idNumDocSin).val();
            let suffisso = $("#" + idNumDocDes).val();
            let esisteItemCompatibile = KendoDDL(idNumDocDDL).dataItems().some(function(dataItem) {
                return dataItem.Doc_Numero_Sin === prefisso && dataItem.Doc_Numero_Des === suffisso;
            });
            if (esisteItemCompatibile === true) {
                KendoDDL(idNumDocDDL).select(function(dataItem) {
                    return dataItem.Doc_Numero_Sin === prefisso && dataItem.Doc_Numero_Des === suffisso;
                });

                $("#" + idNumDocDDL).change(); //forzo il change
            }

        }

    }
}

function crea_Modifica_Contatto(cod_Contatto, tipologia_Contatto, solo_Lettura, piva) {

    let url = UrlGestioneContatto(cod_Contatto, tipologia_Contatto, solo_Lettura, piva);
    if (url === "")
        return;

    let id = "target_iframe";
    let dialog = $("#nuovoContattoWindow").data("kendoWindow");
    $("#inTipoGestioneContatto").val(tipologia_Contatto);
    if (cod_Contatto != "0")
        dialog.title("Modifica Contatto");

    dialog.center().open();

    $("<form />",
        {
            action: url,
            method: "post",
            target: id
        })
        .hide().appendTo("body")
        // add any data
        .append("<input name='foo' />").find("[name=foo]").val("bar").end()
        .submit().remove();
}

async function nuovoCedenteCessionario_Salvato(codContatto) {

    let dialog = $("#nuovoContattoWindow").data("kendoWindow");
    dialog.close();

    // se la form è in modalità solo lettura non tocco i ddl
    if (cIdTipoOp !== enum_TipoOperazioneDB.Lettura.value) {

        var tipoGestioneContatto = kendo.parseInt($("#inTipoGestioneContatto").val());
        let tipoRapporto = 0;
        let accettazioneConGerarchia = 0;
        let pivaPadreGerarchia = "";

        // TODO
        //if (tipoRapporto === enum_TipoRapporto.Conferenti && this.data.ruoloAccettazione !== "") {
        //    //devo per forza ri-azzerarlo, perché vanno sempre riletti
        //    elencoContatti[tipoRapporto] = null;
        //}

        let promises = new Array();
        let idControlloContatto = "";
        switch (tipoGestioneContatto) {
            case enum_tipologia_Contatto.cedente:
                tipoRapporto = GetTipoRapporto(cIdLavCod);
                idControlloContatto = ddlContatto1Nome;
                break;
            case enum_tipologia_Contatto.agente:
                tipoRapporto = enum_TipoRapporto.Agenti;
                idControlloContatto = "inAgente";
                break;
            case enum_tipologia_Contatto.vettore:
                tipoRapporto = enum_TipoRapporto.Vettori;
                idControlloContatto = "inVettore";
                break;
            case enum_tipologia_Contatto.capoArea:
                tipoRapporto = enum_TipoRapporto.CapoArea;
                idControlloContatto = "inCapoArea";
                break;
        }
        elencoContatti[tipoRapporto] = null;

        if (idControlloContatto !== "") {

            // Se il controllo di destinazione è disabilitato non lo aggiorno

            if ($("#" + idControlloContatto).prop("disabled") === false) {

                promises.push(KendoDDL(idControlloContatto).dataSource.read());
                let resp1 = await Promise.all(promises).then(console.log("Rilettura Ok, Procedi!!!")).catch(new Error("RILETTURA FALLITA!"));
                KendoDDL(idControlloContatto).select(function (dataItem) {
                    return dataItem.Cod_Contatto === codContatto;
                });

                $("#" + idControlloContatto).change();
            }
        }
    }
}

function MostraStatoEvasioneOrdine() {
    if (statoEvasioneDoc.Stato_Cod !== 0 && statoEvasioneDoc.Stato_Des !== "") {
        $("#spanStatoEvasioneOrdine").html(statoEvasioneDoc.Stato_Des);

        $("#statoEvasioneOrdine").removeClass();
        $("#statoEvasioneOrdine").addClass("alert");
        switch (statoEvasioneDoc.Stato_Cod) {
            case 1: //inevaso
                $("#statoEvasioneOrdine").addClass("alert-warning");
                break;
            case 2: //evaso
            case 5: //evaso forzatamente
                $("#statoEvasioneOrdine").addClass("alert-success");
                break;
            case 3: //parzialmente evaso
                $("#statoEvasioneOrdine").addClass("alert-info");
                break;
            case 4: //non pronto
                $("#statoEvasioneOrdine").addClass("alert-danger");
                break;
            default:
                $("#statoEvasioneOrdine").addClass("alert-info");
                break;
        }
        $("#rowStatoOrdine").show();

        if ([1, 3, 4].includes(statoEvasioneDoc.Stato_Cod)) {
            $("#btn_forzaEvasioneDocumento").show();
        } else {
            $("#btn_forzaEvasioneDocumento").hide();
        }


    } else {
        $("#rowStatoOrdine").hide();
    }
}

function MostraBtnDocumenti() {

    let piva = $(cIdPiva).val();
    let idAgenda = parseInt($(cIdAgenda).val());

    let countRigheAllegati = 0;

    if (idAgenda != 0) {
        if ($("input[name$='hf_UtenteAbilitatoGestioneVisualizaAllegato']").val() == "True") {
            var result = RicercaDocumentiConAgenda(piva, idAgenda);
            if (result !== undefined && result !== null && result.length > 0) {
                allegati = result.filter(x => x.Allegati_Documenti_Cod != 0);
                countRigheAllegati = allegati.length;
            }
        }

        if ($("input[name$='hf_UtenteAbilitatoGestioneNuovoAllegato']").val() == "True") {
            $("#btnNuovoDocmmento").show();
            $("#btnNuovoDocmmento2").show();
        }
    }

    if (countRigheAllegati > 0) {
        $("#btnDocmmenti").show();
        $("#btnDocmmenti2").show();
    } else {
        $("#btnDocmmenti").hide();
        $("#btnDocmmenti2").hide();
    }
}

function MostraBtnStampa(solaLettura) {

    let visualizzaSalvaStampa = false;
    let visualizzaStampaDocum = false;

    switch (true) {

        case isLavCodSenzaStampa():
            break;

        case solaLettura === true:
            visualizzaSalvaStampa = false;
            visualizzaStampaDocum = true;
            break;

        case solaLettura === false:
            visualizzaSalvaStampa = true;
            visualizzaStampaDocum = false;
            break;

    }

    if (visualizzaSalvaStampa) {
        $("#btnSalvaStampa").show();
        $("#btnSalvaStampa2").show();
    } else {
        $("#btnSalvaStampa").hide();
        $("#btnSalvaStampa2").hide();
    }

    if (visualizzaStampaDocum) {
        $("#btnStampaDoc").show();
        $("#btnStampaDoc2").show();
    } else {
        $("#btnStampaDoc").hide();
        $("#btnStampaDoc2").hide();
    }

}

function isLavCodSenzaStampa() {

    return (isMovimentoMagazzino(cIdLavCod) || isContrattoAffitto());

}

function isLavCodForzaGestioneSemplificata() {

    return (isMovimentoMagazzino(cIdLavCod) || isContrattoAffitto());

}

function isLavCodSenzaDatiSpedizione() {

    return (isMovimentoMagazzino(cIdLavCod) || isContrattoAffitto());

}

function isLavCodSenzaTabRiepilogo() {

    return (isMovimentoMagazzino(cIdLavCod) || isContrattoAffitto());

}

function isLavCodSenzaTabConfezionamento() {

    return (isMovimentoMagazzino(cIdLavCod) || isContrattoAffitto());

}

function isGestitoTabConfezionamento() {

    return (lavCodAccettazione || is_FF_FormProdottoUC()) && !isLavCodSenzaTabConfezionamento();

}


function ImpostaVisibilitaNotaFattura(flagVisibile, flagObbligatorio) {
    if (flagVisibile === true) {
        $("#boxRifNotaCredito").show();

        RendiObbligatorio("inNotaFattura", flagObbligatorio);
        RendiObbligatorio("inDataNotaFattura", flagObbligatorio);
    } else {
        $("#boxRifNotaCredito").hide();

        RendiObbligatorio("inNotaFattura", false);
        RendiObbligatorio("inDataNotaFattura", false);
    }
}

function ImpostaVisibilitaModPagamento(flagVisibile) {
    if (flagVisibile === true) {
        $("#groupModPagamento").show();

    } else {
        $("#groupModPagamento").hide();

        // La sezione padre contiene solo questo campo e il campo Sezionale, se è nascosto anche quest'ultimo nascondo l'intera sezione (accordion della tab Testata)
        if (gestioneContabilita === enum_Livello_GestContabilita_NonGestita) {
            $("#panelBarInfoEconomiche").hide();
        }
    }
}