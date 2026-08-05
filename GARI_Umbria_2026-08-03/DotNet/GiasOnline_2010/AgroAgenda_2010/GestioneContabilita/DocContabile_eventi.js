
function btnModificaContatto_click(tipologia_Contatto) {

    var cod_Contatto = "0";
    var piva = $(cIdPiva).val();
    var pivaProprietaria = "";
    var sa_cod = 0;
    var dataItem = null;

    switch (tipologia_Contatto) {
        case enum_tipologia_Contatto.cedente:
            dataItem = KendoDDL(ddlContatto1Nome).dataItem();
            //cod_Contatto = KendoDDL(ddlContatto1Nome).dataItem().Cod_Contatto;
            break;
        case enum_tipologia_Contatto.agente:
            dataItem = KendoDDL("inAgente").dataItem();
            //cod_Contatto = KendoDDL("inAgente").dataItem().Cod_Contatto;
            break;
        case enum_tipologia_Contatto.vettore:
            dataItem = KendoDDL("inVettore").dataItem();
            //cod_Contatto = KendoDDL("inVettore").dataItem().Cod_Contatto;
            break;
    }

    cod_Contatto = dataItem.Cod_Contatto;
    pivaProprietaria = dataItem.Piva_Proprietaria;
    sa_cod = dataItem.Sa_Cod;

    if (cod_Contatto == undefined || cod_Contatto === "")
        return;

    var modificaAmmessa = false;
    if (sa_cod != -1)
        modificaAmmessa = true;
    else {
        if (piva === pivaProprietaria)
            modificaAmmessa = true;
    }

    if (!modificaAmmessa) {
        var kendoConfirm = $("<div></div>").kendoConfirm({
            title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
            messages: { okText: TraduzioneMultiResx(resxObj, "Si", "Sì"), cancel: TraduzioneMultiResx(resxObj, "No", "No") },
            content: TraduzioneMultiResx(resxObj, "ConfermaContattoNonModificabileAprireVisualizzazione",
                "Il contatto è stato creato da un'altra impresa pertanto non è possibile modificarlo. Aprirlo comunque in sola visualizzazione?")
        }).data("kendoConfirm");
        kendoConfirm.result.done(function () {
            crea_Modifica_Contatto(cod_Contatto, tipologia_Contatto, true, pivaProprietaria);
        });

        kendoConfirm.result.fail(function () {
        });

        kendoConfirm.open();

    }
    else
        crea_Modifica_Contatto(cod_Contatto, tipologia_Contatto, false, piva);

}

function btnNuovoContatto_click(tipologia_Contatto) {
    var cod_Contatto = "0";
    var piva = $(cIdPiva).val();
    crea_Modifica_Contatto(cod_Contatto, tipologia_Contatto, false, piva);
}

function inTipologiaDocumento_change(e) {
    let valore = parseInt(this.value);

    let labelCentroAziendale = $("#lbl_centro_aziendale");
    let labelCedCes1 = $("#lbl_cedente_cessionario_1");
    let labelCedCes2 = $("#lbl_cedente_cessionario_2");
    let labelIntestatario = $("#panelBarTitleIntestatario");
    let labelDestinatario = $("#panelBarTitleDestinatario");
    let labelRifOrdine = $("#lbl_n_ordine_cliente");
    let labelDataSpedPrevistaOrdine = $("#lbl_data_spedizione_prevista");
    let labelRifOrdineFormProdotto = $("#lbl_rif_n_ordine"); // Definita in FormProdottoUC.ascx
    let labelDataOrdineFormProdotto = $("#lbl_rif_data_ordine"); // Definita in FormProdottoUC.ascx
    //let labelIndirizzoDestinazione = $("#lbl_indirizzo_destinazione");

    lavCodAccettazione = isLavCodAccettazione(valore);
    lavCodDocEmesso = isNumeroDocumentoEmesso(valore);
    lavCodVendita = isDocumentoVendita(valore);

    switch (valore) {

        case enum_LavCod.Contratto_Affitto.value:
            labelCedCes1.html(TraduzioneMultiResx(resxObj, "Locatore", "Locatore") + ":");
            $("#cardContrattiAffitto").show();
            RendiObbligatorio("inDataInizVal", true);
            RendiObbligatorio("inDataFineVal", true);
            $("#groupAltriLocatori").show();
            $("#groupRifOrdini").show();
            break;

        case enum_LavCod.Fattura_Emessa.value:
        case enum_LavCod.DDT_Emesso.value:
        case enum_LavCod.DDT_Contabilizzato_Emesso.value:
        case enum_LavCod.Ordine_Vendita_Emesso.value:
        case enum_LavCod.Scarico_Magazzino.value:
        case enum_LavCod.Nota_Accredito_Emessa.value:

            if (valore === enum_LavCod.Scarico_Magazzino.value) {
                labelCentroAziendale.html(TraduzioneMultiResx(resxObj, "CentroAziendale", "Centro aziendale") + " :");
            } else {
                labelCentroAziendale.html(TraduzioneMultiResx(resxObj, "Partenza", "Partenza") + ":");
            }

            if (valore === enum_LavCod.Scarico_Magazzino.value) {

                labelCedCes1.html(TraduzioneMultiResx(resxObj, "IntestatarioScarico", "Intestatario Scarico:"));
                labelIntestatario.contents().filter(function () {
                    return this.nodeType === 3;
                }).each(function () { this.textContent = TraduzioneMultiResx(resxObj, "IndirizzoIntestatario", "Indirizzo Intestatario").toUpperCase(); });

            } else {

                labelCedCes1.html(TraduzioneMultiResx(resxObj, "Cessionario", "Cessionario") + ":");
                labelIntestatario.contents().filter(function () {
                    return this.nodeType === 3;
                }).each(function () { this.textContent = TraduzioneMultiResx(resxObj, "IndirizzoCessionario", "Indirizzo Cessionario").toUpperCase(); });


            }
            labelCedCes2.html(TraduzioneMultiResx(resxObj, "Destinatario", "Destinatario") + ":");
            labelDestinatario.contents().filter(function () {
                return this.nodeType === 3;
            }).each(function () { this.textContent = TraduzioneMultiResx(resxObj, "DestinazioneDiversa", "Destinazione Diversa").toUpperCase(); });
            //labelIndirizzoDestinazione.html("Indirizzo Destinazione merce");

            labelRifOrdine.html(TraduzioneMultiResx(resxObj, "RifOrdineCliente", "Rif. Ordine Cliente N") + ":");
            labelDataSpedPrevistaOrdine.html(TraduzioneMultiResx(resxObj, "DataSpedizionePrevista", "Data Spedizione Prevista") + ":");

            $("#panelBar_OrdiniCliente > .k-link").contents().first().replaceWith(TraduzioneMultiResx(resxObj, "OrdiniCliente", "Ordini Cliente").toUpperCase());
            $("#panelBar_DDTCliente > .k-link").contents().first().replaceWith(TraduzioneMultiResx(resxObj, "DdtCliente", "DDT Cliente").toUpperCase());

            labelRifOrdineFormProdotto.html(TraduzioneMultiResx(resxObj, "OrdineClienteRiferimento", "Riferimento Ordine") + ":");
            labelDataOrdineFormProdotto.html(TraduzioneMultiResx(resxObj, "DataOrdine", "Data Ordine") + ":");

            if (valore === enum_LavCod.Nota_Accredito_Emessa.value) {
                $("#lblAccompagnatoria").html(TraduzioneMultiResx(resxObj, "NotaCreditoAccompagnatoria", "Nota di Credito Accompagnatoria"));
            }

            if (valore === enum_LavCod.Fattura_Emessa.value) {
                $("#lbl_n_nota_fattura").html(TraduzioneMultiResx(resxObj, "NumeroFatturaSDI", "Numero Fattura SDI"));
                $("#lbl_data_nota_fattura").html(TraduzioneMultiResx(resxObj, "DataFatturaSDI", "Data Fattura SDI"));
            }

            if (valore === enum_LavCod.Fattura_Emessa.value ||
                valore === enum_LavCod.DDT_Emesso.value ||
                valore === enum_LavCod.DDT_Contabilizzato_Emesso.value ||
                valore === enum_LavCod.Nota_Accredito_Emessa.value) {
                ImpostaVisibilitaModPagamento(true);
            }
            else {
                ImpostaVisibilitaModPagamento(false);
            }

            break;

        case enum_LavCod.Fattura_Ricevuta.value:
        case enum_LavCod.DDT_Ricevuto.value:
        case enum_LavCod.Distinta_Carico.value:
        case enum_LavCod.Auto_Ddt_Emesso.value:
        case enum_LavCod.Accettazione_da_diversi.value:
        case enum_LavCod.Distinta_Carico_Accettazione.value:
        case enum_LavCod.Auto_Ddt_Emesso_Accettazione.value:
        case enum_LavCod.Ordine_Acquisto.value:
        case enum_LavCod.Carico_Magazzino.value:
        case enum_LavCod.Nota_Accredito_Ricevuta.value:

            if (valore === enum_LavCod.Carico_Magazzino.value) {
                labelCentroAziendale.html(TraduzioneMultiResx(resxObj, "CentroAziendale", "Centro aziendale") + ":");
            } else {
                labelCentroAziendale.html(TraduzioneMultiResx(resxObj, "Destinazione", "Destinazione") + ":");
            }

            if (lavCodAccettazione === true && contattiAcc4ConGerarchia === true) {

                labelCedCes1.html(TraduzioneMultiResx(resxObj, "Conferente", "Conferente") + ":");
                labelCedCes2.html(TraduzioneMultiResx(resxObj, "Produttore", "Produttore") + ":");

            } else {

                if (valore === enum_LavCod.Carico_Magazzino.value) {

                    labelCedCes1.html(TraduzioneMultiResx(resxObj, "IntestatarioCarico", "Intestatario Carico") + ":");

                } else {

                    labelCedCes1.html(TraduzioneMultiResx(resxObj, "Fornitore", "Fornitore") + ":");

                }

                labelCedCes2.html(TraduzioneMultiResx(resxObj, "Provenienza", "Provenienza") + ":");

            }

            if (valore === enum_LavCod.Carico_Magazzino.value) {

                labelIntestatario.contents().filter(function () {
                    return this.nodeType === 3;
                }).each(function () { this.textContent = TraduzioneMultiResx(resxObj, "IndirizzoIntestatario", "Indirizzo Intestatario").toUpperCase(); });

            } else {

                labelIntestatario.contents().filter(function () {
                    return this.nodeType === 3;
                }).each(function () { this.textContent = TraduzioneMultiResx(resxObj, "IntestatarioDocumento", "Intestatario Documento").toUpperCase(); });

            }

            labelDestinatario.contents().filter(function () {
                return this.nodeType === 3;
            }).each(function () { this.textContent = TraduzioneMultiResx(resxObj, "OrigineMerce", "Origine Merce").toUpperCase(); });
            //labelIndirizzoDestinazione.html("Indirizzo Origine merce");

            labelRifOrdine.html(TraduzioneMultiResx(resxObj, "RifOrdineFornitore", "Rif. Ordine Fornitore N") + ":");
            labelDataSpedPrevistaOrdine.html(TraduzioneMultiResx(resxObj, "DataConsegnaPrevista", "Data Consegna Prevista") + ":");

            if (lavCodAccettazione === true) {
                $("#panelBar_OrdiniCliente > .k-link").contents().first().replaceWith(TraduzioneMultiResx(resxObj, "OrdiniConferente", "Ordini Conferente").toUpperCase());
            }
            else {
                $("#panelBar_OrdiniCliente > .k-link").contents().first().replaceWith(TraduzioneMultiResx(resxObj, "OrdiniFornitore", "Ordini Fornitore").toUpperCase());
                $("#panelBar_DDTCliente > .k-link").contents().first().replaceWith(TraduzioneMultiResx(resxObj, "DdtFornitore", "DDT FORNITORE").toUpperCase());
            }

            if (valore === enum_LavCod.DDT_Ricevuto.value) {
                labelRifOrdineFormProdotto.html(TraduzioneMultiResx(resxObj, "RifNumeroFattura", "Rif Numero Fattura") + ":");
                labelDataOrdineFormProdotto.html(TraduzioneMultiResx(resxObj, "RifDataFattura", "Rif Data Fattura") + ":");
            }

            if (valore === enum_LavCod.Nota_Accredito_Ricevuta.value) {
                $("#lblAccompagnatoria").html(TraduzioneMultiResx(resxObj, "NotaCreditoAccompagnatoria", "Nota di Credito Accompagnatoria"));
            }
            
            if (valore === enum_LavCod.Fattura_Ricevuta.value) {
                $("#lbl_n_nota_fattura").html(TraduzioneMultiResx(resxObj, "NumeroFatturaSDI", "Numero Fattura SDI"));
                $("#lbl_data_nota_fattura").html(TraduzioneMultiResx(resxObj, "DataFatturaSDI", "Data Fattura SDI"));
            }

            if (valore === enum_LavCod.Fattura_Ricevuta.value ||
                valore === enum_LavCod.DDT_Ricevuto.value ||
                valore === enum_LavCod.Nota_Accredito_Ricevuta.value) {
                ImpostaVisibilitaModPagamento(true);
            }
            else {
                ImpostaVisibilitaModPagamento(false);
            }


            break;
    }

    if (cIdLavCod !== valore) {
        cIdLavCod = valore;
        $('input[name$="hdLavCod"]').val(valore);

        ImpostaOpzioniVisibilitaOperazione($(cIdOpzioniContab).val(), cIdLavCod);

        // rileggo le dropdown che dipendono dal lav_cod
        elencoCausaliTrasporto = null;
        //KendoDDL(idCausaliTrasporto).dataSource.read();
        //KendoDDL(ddlContatto1Nome).dataSource.read();
        //KendoDDL(ddlContatto2Nome).dataSource.read();

        //TODO: resetta tutti i campi
        //if (cedCes1.length > 0) {
        //  KendoDDL(ddlContatto1Nome).value(-1);
        //}
    }

}

function inCessionarioCedente_change(e) {
    let idTipoIndirizzo = e.data.idTipoIndirizzo;
    let tipoCc = e.data.cc;
    let idCedCes2 = e.data.idCedCesDestDiv;
    let idTipoInd2 = e.data.idTipoIndDestDiv;
    let idVettore = e.data.idVettore;
    let idAgente = e.data.idAgente;
    let idCapoArea = e.data.idCapoArea;

    let idCedCes = this.id;
    let thisDdl = KendoDDL(idCedCes);
    let tipoIndDestDefault = 0;

    let ruoloAccettazione = e.data.ruoloAccettazione;

    if (tipoCc === "CC1") {
        VerificaDatiMinimiTestata();
    }

    if (thisDdl !== undefined &&
        thisDdl !== null &&
        thisDdl.dataItem() !== undefined &&
        thisDdl.dataItem() !== null) {

        let partitaIvaCf = "";
        if (thisDdl.dataItem().Partita_Iva !== "") {
            partitaIvaCf = thisDdl.dataItem().Partita_Iva;
        }
        $("#" + txtPivaNome + tipoCc).val(partitaIvaCf);

        let progressivoCf = "";
        if (thisDdl.dataItem().Progressivo !== "") {
            progressivoCf = thisDdl.dataItem().Progressivo;
        }
        $("#" + txtProgressivoNome + tipoCc).val(progressivoCf);

        if (tipoCc === "CC1" || tipoCc === "CC2") {
            //in accettazione (FRUTTAGEL?!?) non mostro l'attività
            //TODO: gestire meglio come capire se va fatta o no (se il campo è visibile,...)
            let attivitaCf = "";
            if (thisDdl.dataItem().Attivita_Des !== "") {
                attivitaCf = thisDdl.dataItem().Attivita_Des;
            }
            $("#" + txtAttivitaNome + tipoCc).val(attivitaCf);
        }

        if (tipoCc === "CC1") {

            if (ruoloAccettazione === "") {
                let riUmDestDefault = thisDdl.dataItem().Cod_Risum_Destinazione_Diversa;
                if (riUmDestDefault !== 0) {

                    let ddlCesCed2 = KendoDDL(idCedCes2);
                    ddlCesCed2.select(function (dataItem) {
                        return dataItem.Cod_RisUm === riUmDestDefault;
                    });

                    //forzo il change
                    $("#" + idCedCes2).change();

                    let ddlTipoInd2 = KendoDDL(idTipoInd2);
                    //ddlTipoInd2.dataSource.read();

                    tipoIndDestDefault = thisDdl.dataItem().Tipo_Indirizzo_Default_Destinazione_Diversa;
                    if (tipoIndDestDefault !== 0) {
                        ddlTipoInd2.select(function (dataItem) {
                            return dataItem.Tipo_Indirizzo === tipoIndDestDefault;
                        });
                    }

                    $("#" + idTipoInd2).change();
                }
            }

            let riUmVettDefault = thisDdl.dataItem().Vettore_Cod;
            if (riUmVettDefault !== 0) {

                let ddlVettore = KendoDDL(idVettore);
                ddlVettore.select(function (dataItem) {
                    return dataItem.Cod_RisUm === riUmVettDefault;
                });

                //forzo il change
                $("#" + idVettore).change();
            }

            let riUmAgenteDefault = thisDdl.dataItem().Agente_Cod;
            if (riUmAgenteDefault !== 0) {
                let ddlAgente = KendoDDL(idAgente);

                ddlAgente.select(function (dataItem) {
                    return dataItem.Cod_RisUm === riUmAgenteDefault;
                });

                //forzo il change
                $("#" + idAgente).change();

                let provvigione = thisDdl.dataItem().Provvigione;
                Set_KendoNumTBValue(idAgente + "ProvvigionePerc", provvigione);
            }

            let riUmCapoAreaDefault = thisDdl.dataItem().CapoArea_Cod;
            if (riUmCapoAreaDefault !== 0) {

                let ddlCapoArea = KendoDDL(idCapoArea);
                ddlCapoArea.select(function (dataItem) {
                    return dataItem.Cod_RisUm === riUmCapoAreaDefault;
                });

                //forzo il change
                $("#" + idCapoArea).change();

                let provvigioneArea = thisDdl.dataItem().Provvigione_CapoArea;
                Set_KendoNumTBValue(idCapoArea + "ProvvigionePerc", provvigioneArea);
            }

            isContattoImpresaGias = thisDdl.dataItem().IsImpresaGias;

            Set_KendoDDLValue("inModPagamento", thisDdl.dataItem().Modalita_Pagamento, 0);
        }

        if (tipoCc === "CC2") {
            isProduttoreImpresaGias = thisDdl.dataItem().IsImpresaGias;
        }

        /* se
         1) sono in conferimento
         2) gestitiParamQualISCC è true
         3) tipoOperazione = scrittura
         4) isProduttoreImpresaGias è true o isContattoImpresaGias è true
         5) contatto.Compliance_ISCC <> 1
         allora blocco l'inserimento
        */
        if (lavCodAccettazione === true &&
            gestitiParamQualISCC === true &&
            cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value &&
            (isProduttoreImpresaGias === true || isContattoImpresaGias === true) &&
            (tipoCc === "CC1" || tipoCc === "CC2") &&
            thisDdl.dataItem().Compliance_ISCC !== 1) {

            let inputISCC = "";
            let boxISCC = "";

            if (tipoCc === "CC1") {
                inputISCC = "reqCompliantISCC_Contatto1";
                boxISCC = "boxCompliantISCC_Contatto1";
            }
            else if (tipoCc === "CC2") {
                inputISCC = "reqCompliantISCC_Contatto2";
                boxISCC = "boxCompliantISCC_Contatto2";
            }

            $("#" + inputISCC).val(thisDdl.dataItem().Compliance_ISCC);

            //$("#" + inputISCC).attr("required", true);
            $("#" + inputISCC).addClass("ISCCNonConforme");
            $("#" + boxISCC).addClass("k-invalid");
            $("#" + inputISCC).addClass("k-invalid");
            VerificaDatiMinimiTestata();

            if ($("input[name$='hf_utenteAbilitatoISCCNonConforme']").val() == "True") {

                if (tipoCc === "CC1") {
                    creaBtnLockContattiXCompliantISCC(btnLockContatti1XCompliantISCC, ddlContatto1Nome);
                }
                else if (tipoCc === "CC2") {
                    creaBtnLockContattiXCompliantISCC(btnLockContatti2XCompliantISCC, ddlContatto2Nome);
                }
            }

        }
        else if ($("#reqCompliantISCC_Contatto1").hasClass("ISCCNonConforme") || $("#reqCompliantISCC_Contatto2").hasClass("ISCCNonConforme")) {
            let inputISCC = "";
            let boxISCC = "";

            if (tipoCc === "CC1") {
                inputISCC = "reqCompliantISCC_Contatto1";
                boxISCC = "boxCompliantISCC_Contatto1";
            }
            else if (tipoCc === "CC2") {
                inputISCC = "reqCompliantISCC_Contatto2";
                boxISCC = "boxCompliantISCC_Contatto2";
            }

            $("#" + inputISCC).val(thisDdl.dataItem().Compliance_ISCC);

            //$("#" + inputISCC).attr("required", false);
            $("#" + inputISCC).removeClass("ISCCNonConforme");
            $("#" + boxISCC).removeClass("k-invalid");
            $("#" + inputISCC).removeClass("k-invalid");

            if (tipoCc === "CC1") {
                $("#" + btnLockContatti1XCompliantISCC).hide();
            }
            else if (tipoCc === "CC2") {
                $("#" + btnLockContatti2XCompliantISCC).hide();
            }

            VerificaDatiMinimiTestata();
        }

    } else {
        if (tipoCc === "CC1") {
            isContattoImpresaGias = false;
            Set_KendoDDLValue("inModPagamento", 0);
        }
        if (tipoCc === "CC2") {
            isProduttoreImpresaGias = false;
        }
    }

    if (ruoloAccettazione !== "") {
        let partitaIvaSelezionata = "";
        if (thisDdl !== undefined &&
            thisDdl !== null &&
            thisDdl.dataItem() !== undefined &&
            thisDdl.dataItem() !== null) {
            partitaIvaSelezionata = thisDdl.dataItem().Partita_Iva;

            // verifica se esiste contratto per conferente
            if (lavCodAccettazionePomodoro && ruoloAccettazione == "Conferente") {
                let messaggioErrore = Verifica_Contratto_Pomodoro(false);
                if (messaggioErrore !== "") {
                    MessaggioErrore_Bootstrap(messaggioErrore, "DIV_Messaggi");
                }
            }
        }

        impostaDdlCascadeAccettazione(ruoloAccettazione, partitaIvaSelezionata);

    }

    if (tipoCc === "CC1" || tipoCc === "CC2") {
        let inTipoIndirizzoCC = KendoDDL(idTipoIndirizzo);
        if (inTipoIndirizzoCC !== undefined && inTipoIndirizzoCC !== null) {
            inTipoIndirizzoCC.dataSource.read();
        }
    }
}

function impostaDdlCascadeAccettazione(ruoloAccettazione, partitaIvaSelezionata) {
    console.log("impostaDdlCascadeAccettazione: R=[" + ruoloAccettazione + "], P=[" + partitaIvaSelezionata + "]");
    switch (ruoloAccettazione) {

        case "Conferente":
            if (partitaIvaSelezionata !== "") {
                pivaPadreGerarchiaCoop1 = partitaIvaSelezionata;
                pivaPadreGerarchiaProd = partitaIvaSelezionata;
            } else {
                pivaPadreGerarchiaCoop1 = "-1";
                pivaPadreGerarchiaCoop2 = "-1";
                pivaPadreGerarchiaProd = "-1";
            }

            console.log("pivaPadreGerarchiaCoop1 = " + pivaPadreGerarchiaCoop1);
            console.log("pivaPadreGerarchiaProd = " + pivaPadreGerarchiaProd);

            Cmb_Coop1.dataSource.read();
            $("#" + txtPivaNome + "CC3").val("");
            $("#" + txtProgressivoNome + "CC3").val("");

            Cmb_Coop2.dataSource.read();
            $("#" + txtPivaNome + "CC4").val("");
            $("#" + txtProgressivoNome + "CC4").val("");

            Cmb_Contatto2.dataSource.read();
            $("#" + txtPivaNome + "CC2").val("");
            $("#" + txtProgressivoNome + "CC2").val("");

            break;

        case "Coop1":

            if (partitaIvaSelezionata !== "") {
                pivaPadreGerarchiaCoop2 = partitaIvaSelezionata;
                pivaPadreGerarchiaProd = partitaIvaSelezionata;
            } else {
                //ho tolto la selezione oppure non l'ho mai fatta
                pivaPadreGerarchiaCoop2 = "-1"; // così mi assicuro che la query non restituisca risultati

                //il PRODUTTORE lo devo caricare in base al conferente selezionato
                let conferenteSelezionato = "";
                if (KendoDDL(ddlContatto1Nome).dataItem() !== undefined &&
                    KendoDDL(ddlContatto1Nome).dataItem() !== null) {
                    conferenteSelezionato = KendoDDL(ddlContatto1Nome).dataItem().Partita_Iva;
                }

                if (conferenteSelezionato !== "") {
                    pivaPadreGerarchiaProd = conferenteSelezionato;
                } else {
                    pivaPadreGerarchiaProd = "-1";
                }
            }

            console.log("pivaPadreGerarchiaCoop2 = " + pivaPadreGerarchiaCoop2);
            console.log("pivaPadreGerarchiaProd = " + pivaPadreGerarchiaProd);

            Cmb_Coop2.dataSource.read();
            $("#" + txtPivaNome + "CC4").val("");
            $("#" + txtProgressivoNome + "CC4").val("");

            Cmb_Contatto2.dataSource.read();
            $("#" + txtPivaNome + "CC2").val("");
            $("#" + txtProgressivoNome + "CC2").val("");

            break;

        case "Coop2":

            if (partitaIvaSelezionata !== "") {
                pivaPadreGerarchiaProd = partitaIvaSelezionata;
            } else {

                //il PRODUTTORE lo devo caricare in base a: se avevo indicato COOP1, uso quello, altrimenti uso CONFERENTE

                let coop1Selezionato = "";
                if (KendoDDL(ddlContattoCoop1Nome).dataItem() !== undefined &&
                    KendoDDL(ddlContattoCoop1Nome).dataItem() !== null) {
                    coop1Selezionato = KendoDDL(ddlContattoCoop1Nome).dataItem().Partita_Iva;
                }

                let conferenteSelezionato = "";
                if (coop1Selezionato === "" &&
                    KendoDDL(ddlContatto1Nome).dataItem() !== undefined &&
                    KendoDDL(ddlContatto1Nome).dataItem() !== null) {
                    conferenteSelezionato = KendoDDL(ddlContatto1Nome).dataItem().Partita_Iva;
                }

                if (coop1Selezionato !== "") {
                    pivaPadreGerarchiaProd = coop1Selezionato;
                } else if (conferenteSelezionato !== "") {
                    pivaPadreGerarchiaProd = conferenteSelezionato;
                } else {
                    pivaPadreGerarchiaProd = "-1";
                }

            }

            console.log("pivaPadreGerarchiaProd = " + pivaPadreGerarchiaProd);

            Cmb_Contatto2.dataSource.read();
            $("#" + txtPivaNome + "CC2").val("");
            $("#" + txtProgressivoNome + "CC2").val("");

            break;
    }
}

function inTipoIndirizzoCC_change(e) {
    let idTipoIndirizzo = e.data.idTipoIndirizzo;
    let cc = e.data.cc;

    let ddlTipoIndirizzoCC = KendoDDL(idTipoIndirizzo);

    let indirizzo = "";
    let localita = "";
    let comune = "";
    let provincia = "";
    let cap = "";
    let stato = "";

    let ds = ddlTipoIndirizzoCC.dataSource.get();

    if (ddlTipoIndirizzoCC.dataItem() !== undefined && ddlTipoIndirizzoCC.dataItem() !== null) {
        indirizzo = ddlTipoIndirizzoCC.dataItem().Indirizzo;
        localita = ddlTipoIndirizzoCC.dataItem().Localita;
        comune = ddlTipoIndirizzoCC.dataItem().Comune;
        if (ddlTipoIndirizzoCC.dataItem().Stato.toUpperCase() !== "IT") {
            provincia = ddlTipoIndirizzoCC.dataItem().Provincia;
        }
        else {
            provincia = ddlTipoIndirizzoCC.dataItem().Provincia_Sigla + " - " + ddlTipoIndirizzoCC.dataItem().Provincia;
        }

        cap = ddlTipoIndirizzoCC.dataItem().CAP;
        stato = ddlTipoIndirizzoCC.dataItem().Stato;
        //} else if (ds !== undefined && ds !== null) {
        //    indirizzo = "<p>" + ds.Indirizzo + "</p>";
    }

    $("#inIndirizzo" + cc).val(indirizzo);
    $("#inLocalita" + cc).val(localita);
    $("#inComune" + cc).val(comune);
    $("#inProvincia" + cc).val(provincia);
    $("#inCap" + cc).val(cap);
    $("#inStato" + cc).val(stato);

    if ($("#" + idTipoIndirizzo).prop("required") === true) {
        VerificaDatiMinimiTestata();
    }
}

function inVettore_change(e) {
    Cmb_TipoIndVettore.dataSource.read();
    Cmb_MezzoTrasporto.dataSource.read();

    VerificaDatiMinimiTestata();    //per il momento lo faccio sempre?!?
}

function inTipoIndirizzoVettore_change(e) {
    let indirizzo = "";
    let ds = Cmb_TipoIndVettore.dataSource.get();

    if (Cmb_TipoIndVettore.dataItem() !== undefined) {
        indirizzo = "<p>" + Cmb_TipoIndVettore.dataItem().IndirizzoCompleto + "</p>";
    } else if (ds !== undefined && ds !== null) {
        indirizzo = "<p>" + ds.Indirizzo + "</p>";
    }

    $("#inDettaglioIndirizzoVettore").html(indirizzo);

    VerificaDatiMinimiTestata();    //per il momento lo faccio sempre?!?
}

function inMezzoTrasporto_change(e) {
    let descrizioneMezzo = "";
    let pesoMezzo = 0;
    let numImmatricolazioneRimorchio = "";
    let numAutorizzazione = "";
    let dataAutorizzazione = "";

    if (Cmb_MezzoTrasporto.dataItem() !== undefined) {
        descrizioneMezzo = Cmb_MezzoTrasporto.dataItem().Mac_Des;
        pesoMezzo = Cmb_MezzoTrasporto.dataItem().Peso;
        numImmatricolazioneRimorchio = Cmb_MezzoTrasporto.dataItem().N_Immatricolazione_Rimorchio;
        numAutorizzazione = Cmb_MezzoTrasporto.dataItem().N_Autorizzazione_Trasporto;
        dataAutorizzazione = Cmb_MezzoTrasporto.dataItem().Data_Rilascio_Autorizzazione;

        set_data("inDataAutorizzazioneTrasporto", formattedDate(dataAutorizzazione, "/"), null);
    } else {
        $("#inDataAutorizzazioneTrasporto").val(dataAutorizzazione);
    }

    $("#inDescrizioneMezzo").val(descrizioneMezzo);
    Set_KendoNumTBValue("inPesoTaraTrasporto", pesoMezzo);
    $("#inNImmatricolazioneRimorchio").val(numImmatricolazioneRimorchio);
    $("#inNAutorizzazioneTrasporto").val(numAutorizzazione);
}

function btnRefreshNumDoc_click(e) {
    RefreshNumDoc();
    MostraNumeroDocCompleto("inNumDocDDL", "inNumDoc", "inNumDocSin", "inNumDocDes", "inNumDocShow");
    GetNewProgressivo();
}

function inNumDoc_change(e) {

    if (cIdTipoOp === enum_TipoOperazioneDB.Modifica.value && e.sender.value() === 0) {
        //se sono in modifica il numero 0 non è consentito!
        alert("Il numero documento deve essere maggiore di 0");
        let data = JSON.parse($('input[name$="hdKendo_TestataDoc"]').val());
        Set_KendoNumTBValue("inNumDoc", data.DocNumero);
    }

    MostraNumeroDocCompleto("inNumDocDDL", "inNumDoc", "inNumDocSin", "inNumDocDes", "inNumDocShow");

    VerificaDatiMinimiTestata();
}

function inDataInizVal_change(e) {
    ControllaDataConRiferimentiCatastali("inDataInizVal", enum_tipoDataValidita.Inizio, enum_ContestoControlloDate.OnChange);
    VerificaDatiMinimiTestata();
}

function inDataFineVal_change(e) {
    ControllaDataConRiferimentiCatastali("inDataFineVal", enum_tipoDataValidita.Fine, enum_ContestoControlloDate.OnChange);
    VerificaDatiMinimiTestata();
}

function ControllaDataConRiferimentiCatastali(idControllo, tipoDataValidita, contesto) {

    let messaggioErrore = "";

    if ($(tabGrigliaRifCatastali) !== undefined && $(tabGrigliaRifCatastali).data("kendoGrid") !== undefined) {

        let nuovaDataStringa = $("#" + idControllo).val();

        let nuovaDataStringaPerConversione = sistemaDataInBaseAllaCulture(nuovaDataStringa);

        let nuovaData = new Date(nuovaDataStringaPerConversione);

        let grid = $(tabGrigliaRifCatastali).data("kendoGrid");

        let currentData = grid.dataSource.data();

        if (currentData.length > 0) {

            for (let i = 0; i < currentData.length; i++) {

                var item = currentData[i];

                //Controllo data inizio validità

                if (tipoDataValidita === enum_tipoDataValidita.Inizio) {

                    if (nuovaData > item.Validita_Inizio) {

                        let dataValiditaInizio = formattedDate(item.Validita_Inizio, "/");

                        if (messaggioErrore === "") {
                            //messaggioErrore = `Validità dal <b>${nuovaDataStringa}</b> successiva ad inizio affitto particelle catastali:`
                            messaggioErrore = kendo.format(TraduzioneMultiResx(resxObj, "InizioValiditaSuccessivaInizioAffitto",
                                "Validità dal <b>{0}</b> successiva ad inizio affitto particelle catastali"), nuovaDataStringa);
                        }
                        messaggioErrore += `</br>- ${TraduzioneMultiResx(resxObj, "Codice", "Codice")}: <b>${item.Cod_Particella}</b> ` &
                            `- ${TraduzioneMultiResx(resxObj, "InizioAffitto", "Inizio affitto")}: <b>${dataValiditaInizio}</b>`

                    }

                }

                //Controllo data fine validità

                if (tipoDataValidita === enum_tipoDataValidita.Fine) {

                    if (nuovaData < item.Validita_Fine) {

                        let dataValiditaFine = formattedDate(item.Validita_Fine, "/");

                        if (messaggioErrore === "") {
                            //messaggioErrore = `Validità al <b>${nuovaDataStringa}</b> precedente a fine affitto particelle catastali:`
                            messaggioErrore = kendo.format(TraduzioneMultiResx(resxObj, "FineValiditaPrecedenteFineAffitto",
                                "Validità al <b>{0}</b> precedente a fine affitto particelle catastali"), nuovaDataStringa);
                        }
                        messaggioErrore += `</br>- ${TraduzioneMultiResx(resxObj, "Codice", "Codice")}: <b>${item.Cod_Particella}</b> ` &
                            `- ${TraduzioneMultiResx(resxObj, "FineAffitto", "Fine affitto")}: <b>${dataValiditaFine}</b>`

                    }

                }

            }

        }

    }

    if (messaggioErrore !== "" && contesto === enum_ContestoControlloDate.OnChange) {

        MessaggioErrore_Bootstrap(messaggioErrore, "DIV_Messaggi");

    }

    return messaggioErrore;

}

function inNumDoc2_change(e) {
    //if ($("#" + idTipoIndirizzo).prop("required") === true) {
    VerificaDatiMinimiTestata();
    //}
}

function inNumDocSin_change(e) {
    MostraNumeroDocCompleto("inNumDocDDL", "inNumDoc", "inNumDocSin", "inNumDocDes", "inNumDocShow");
}

function inNumDocDes_change(e) {
    MostraNumeroDocCompleto("inNumDocDDL", "inNumDoc", "inNumDocSin", "inNumDocDes", "inNumDocShow");
}

function inNumDocDDL_change(e) {

    let dataItem = KendoDDL("inNumDocDDL").dataItem();
    if (dataItem !== undefined && dataItem !== null) {

        //metto prefisso e suffisso in sin e des
        $("#inNumDocSin").val(dataItem.Doc_Numero_Sin);
        $("#inNumDocDes").val(dataItem.Doc_Numero_Des);

        //ricostruisco il numero completo
        MostraNumeroDocCompleto("inNumDocDDL", "inNumDoc", "inNumDocSin", "inNumDocDes", "inNumDocShow");

    }

}

function inNumDocLock_change(e) {
    let state = e.checked;

    if (state === true) {

        //Ho appena bloccato ==> rendo non editabili i numeri
        RiBloccoNumeroDoc(false, "inNumDocDDL", "inNumDoc", "inNumDocSin", "inNumDocDes", "inNumDocLock");

    } else {
        //Ho appena sbloccato ==> rendo editabili i numeri

        let kendoConfirm = $("<div></div>").kendoConfirm({
            title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
            messages: { okText: TraduzioneMultiResx(resxObj, "Si", "Sì"), cancel: TraduzioneMultiResx(resxObj, "No", "No") },
            content: TraduzioneMultiResx(resxObj, "ConfermaModificaNumeroDoc", "Permettere modifica manuale del numero di documento. Proseguire?")
        }).data("kendoConfirm");

        kendoConfirm.result.done(function () {
            DecidoDiSbloccareNumero("inNumDocDDL", "inNumDoc", "inNumDocSin", "inNumDocDes", "inNumDocShow");
            //Disabilito il lock, in modo che non si possa ri-bloccare finché non si salva
            $("#inNumDocLock").data("kendoSwitch").enable(false);
        });

        kendoConfirm.result.fail(function () {
            setKendoSwitch("inNumDocLock", true);
        });

        kendoConfirm.open();

    }
}

function DecidoDiSbloccareNumero(idNumDocDDL, idNumDoc, idNumDocSin, idNumDocDes, idNumDocShow) {
    //lock è stato spento

    //rendo editabile il num doc (se era nuovo doc = 0)
    KendoNumTB(idNumDoc).enable(true);

    let ddlNumDoc = KendoDDL(idNumDocDDL);
    //Ho configurazione?
    if (ddlNumDoc !== undefined && ddlNumDoc !== null &&
        ddlNumDoc.dataItems() !== undefined &&
        ddlNumDoc.dataItems() !== null &&
        ddlNumDoc.dataItems().length > 0) {

        //YES, ho configurazione

        //Se esiste un numeratore default e vincolante, lo imposto
        if (ddlNumDoc.dataItems().some(function (dataItem) { return dataItem.IsDefault === true && dataItem.Vincolante === true; })) {
            ddlNumDoc.select(function (dataItem) {
                return dataItem.IsDefault === true && dataItem.Vincolante === true;
            });
        }

        //se ho trovato questo elemento, devo disabilitare la DDL
        if (ddlNumDoc.dataItem() !== undefined &&
            ddlNumDoc.dataItem().Vincolante === true) {
            ddlNumDoc.enable(false);
        } else {
            //non avevo un elemento default vincolante, la DDL è modificabile ed anche il numero
            ddlNumDoc.enable(true);
        }

    } else {

        //NO, non ho configurazione ==> dipende da gestione contabilità

        if (gestioneContabilita === enum_Livello_GestContabilita_NonGestita) {
            //sparisce proprio la ddl
            ddlNumDoc.wrapper.hide();

            //prefisso e suffisso = ""
            $("#" + idNumDocSin).val("");
            $("#" + idNumDocDes).val("");

            //prefisso e suffisso sparisce (di fatto è modificabile solo il numero)
            $("#" + idNumDocSin).hide();
            $("#" + idNumDocDes).hide();

            //nascondo anche il numero completo
            $("#" + idNumDocShow).hide();

        } else {

            //sparisce ddl
            ddlNumDoc.wrapper.hide();

            //prefisso e suffisso modificabili?!?
            $("#" + idNumDocSin).attr("disabled", false);
            $("#" + idNumDocDes).attr("disabled", false);

        }
    }

    //abilito ddl numero (se ho configurazione e non è vincolante)
    //se ho configurazione, prefisso e suffisso rimangono disabilitati (devo passare sempre dalla ddl)
    //se non ho configurazione, dipende da gestione contabilità



    //quando salvo passo il valore di lock ed il numero (+prefisso/suffisso)
    //se mi è arrivato cmq zero, lo assegno lato server (anche se il lock è OFF)
    //se mi è arrivato un numero, lo prendo per buono (sempre che superi il controllo di già presenza e di coerenza num/date)

    //dopo aver salvata almeno una volta il lock deve tornare su ON, per evitare che tutte le righe successive faccio scattare 1000 controlli
    //RiBloccoNumeroDoc(true);
}

function RiBloccoNumeroDoc(impostaAncheLock, idNumDocDDL, idNumDoc, idNumDocSin, idNumDocDes, idNumDocLock) {
    //Ho appena bloccato ==> rendo non editabili i numeri

    if (impostaAncheLock === true) {
        setKendoSwitch(idNumDocLock, true);
    }

    $("#" + idNumDocSin).attr("disabled", true);
    KendoNumTB(idNumDoc).enable(false);
    $("#" + idNumDocDes).attr("disabled", true);
    KendoDDL(idNumDocDDL).enable(false);
}

function inDataLock_change(e) {
    let state = e.checked;

    if (state === true) {
        console.log("Ho bloccato la data");
        //Ho appena bloccato ==> rendo non editabile la data
        RiBloccoDataDoc(false, "inDataEmissione", "inDataLock");

        //per evitare problemi, nel caso l'utente l'abbia modificata, riporto la data a quella salvata
        set_data("inDataEmissione", formattedDate(JSON.parse($(cIdDataDocumento).val()), "/"), null);

    } else {
        //Ho appena sbloccato ==> rendo editabili i numeri
        DecidoDiSbloccareData("inDataEmissione");

        //let kendoConfirm = $("<div></div>").kendoConfirm({
        //    title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
        //    messages: { okText: TraduzioneMultiResx(resxObj, "Si", "Sì"), cancel: TraduzioneMultiResx(resxObj, "No", "No") },
        //    content: "Permettere modifica manuale delle data del documento. Proseguire?"
        //}).data("kendoConfirm");

        //kendoConfirm.result.done(function () {
        //    console.log("Ho sbloccato la data");
        //    DecidoDiSbloccareData("inDataEmissione");
        //    $("#btnSalvaDataDoc").show();
        //    //Disabilito il lock, in modo che non si possa ri-bloccare finché non si salva
        //    //$("#inDataLock").data("kendoSwitch").enable(false);
        //});

        //kendoConfirm.result.fail(function () {
        //    setKendoSwitch("inDataLock", true);
        //    $("#btnSalvaDataDoc").hide();
        //});

        //kendoConfirm.open();

    }
}

function DecidoDiSbloccareData(idDataDoc) {

    //lock è stato spento

    //rendo editabile la data doc (se era nuovo doc = Now)
    KendoDate(idDataDoc).enable(true);

    //Impedisco cmq di cambiare l'anno
    let dataAttuale = KendoDate(idDataDoc).value();
    let anno = dataAttuale.getFullYear();
    KendoDate(idDataDoc).min(validitaSportello.First);
    KendoDate(idDataDoc).max(validitaSportello.Second);

    //abilito ddl numero (se ho configurazione e non è vincolante)
    //se ho configurazione, prefisso e suffisso rimangono disabilitati (devo passare sempre dalla ddl)
    //se non ho configurazione, dipende da gestione contabilità

    //quando salvo passo il valore di lock ed il numero (+prefisso/suffisso)
    //se mi è arrivato cmq zero, lo assegno lato server (anche se il lock è OFF)
    //se mi è arrivato un numero, lo prendo per buono (sempre che superi il controllo di già presenza e di coerenza num/date)

    //dopo aver salvata almeno una volta il lock deve tornare su ON, per evitare che tutte le righe successive faccio scattare 1000 controlli
    //RiBloccoNumeroDoc(true);

    $("#btnSalvaDataDoc").show();

    //Nascondo i pulsanti di salvataggio
    VisualizzaPulsantiSalvataggio(false);

}

function RiBloccoDataDoc(impostaAncheLock, idDataDoc, idDataLock) {

    //Ho appena bloccato ==> rendo non editabile la data

    if (impostaAncheLock === true) {
        setKendoSwitch(idDataLock, true);
    }

    KendoDate(idDataDoc).enable(false);

    //$("#" + idNumDocSin).attr("disabled", true);
    //KendoNumTB(idNumDoc).enable(false);
    //$("#" + idNumDocDes).attr("disabled", true);

    //KendoDDL(idNumDocDDL).enable(false);

    $("#btnSalvaDataDoc").hide();

    //Visualizzo i pulsanti di salvataggio
    VisualizzaPulsantiSalvataggio(true);

}

function inDataEmissione_change(e) {
    //sono costretta ad usare un wrapper perché per usare await la funzione deve essere async
    inDataEmissione_change_Async(e);

    if (sonoInDettaglioRigaDoc === true) {

        SeImpostaFiltroTuttiProdotti(Evento_DataEmissione_Change, null);

    }

}

async function inDataEmissione_change_Async(e) {

    // Impedisco che l'utente possa scrivere un valore più piccolo del minimo o più grande del massimo
    var dt = e.sender;
    var value = dt.value();

    if (value === null) {
        value = kendo.parseDate(dt.element.val(), dt.options.parseFormats);
    }

    if (value < dt.min()) {
        dt.value(dt.min());
        //kendo.alert("Non è possibile inserire un'operazione prima del " + dt.min().toLocaleDateString() + ". Sportello chiuso");
        kendo.alert(kendo.format(TraduzioneMultiResx(resxObj, "DataAntecedenteAperturaSportello",
            "Non è possibile inserire un'operazione prima del {0}. Sportello chiuso"), dt.min().toLocaleDateString()));
    } else if (value > dt.max()) {
        dt.value(dt.max());
        //kendo.alert("Non è possibile inserire un'operazione dopo il " + dt.max().toLocaleDateString() + ".  Sportello chiuso");
        kendo.alert(kendo.format(TraduzioneMultiResx(resxObj, "DataSuccessivaChiusuraSportello",
            "Non è possibile inserire un'operazione dopo il {0}. Sportello chiuso"), dt.max().toLocaleDateString()))
    }

    VerificaDatiMinimiTestata();

    //rileggo i defaults
    elencoNumeratoriDefaults = null;
    elencoNumeratori = null;
    KendoDDL("inNumDocDDL").dataSource.read();

    //imposto il default, se esiste
    ImpostaDefaultNumeratore(cIdLavCod, cIdTipoOp, "inNumDocDDL", "inNumDocSin", "inNumDocDes");

    //RefreshNumDoc();

    //Rileggo i regolamenti a fronte di un cambio data
    RicercaPUA_Regolamenti(false, Qs_PuaRegolamento, SENZA_CATEGORIA);
    //Se sono in dettaglio riga, scateno la lettura del dataSource
    if (sonoInDettaglioRigaDoc === true) {
        KendoDDL("ddlPUARegolamento").dataSource.read();
    }

    //se la data di registrazione è antecedente alla nuova data di emissione, sincronizzo le due date
    let newDataEm = KendoDate("inDataEmissione").value();
    let oldDataReg = KendoDate("inDataRegistrazione").value();
    if (oldDataReg < newDataEm) {
        KendoDate("inDataRegistrazione").value(newDataEm);
    }

    //rileggo i contatti

    //visto che ci sono dei change che risettano anche i default di altri contatti (cliente-destinatario)
    //devo prima leggere tutti i valori precedenti
    let valPrec = LeggiValoriAttuali();

    if (raccolteXConferimenti_AbilitazioneGenerale() && cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value) {
        // Nel caso della modalità di conferimenti da raccolte, devo rileggere i contatti al cambio della data
        // per la possibilità di filtrare quelli che hanno delle raccolte collegabili in un determinato periodo
        elencoContatti[enum_TipoRapporto.Conferenti] = null;
    }

    if (lavCodMovMagazzino === false) {
        WaitFrame.show();
        await RileggoContattiPerValidita(true, ddlContatto1Nome, valPrec.cc1Prec, "lbl_cedente_cessionario_1", "inTipoIndirizzoCC1", valPrec.cc1IndPrec);

        if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true) {
            //ho la gerarchia, quindi devo reimpostare le ddl una alla volta (su ogni change vengono rilette le seguenti ddl)
            await RileggoContattiPerValidita(false, ddlContattoCoop1Nome, valPrec.cc3Prec, "lbl_cedente_cessionario_acc_3", "", 0);
            await RileggoContattiPerValidita(false, ddlContattoCoop2Nome, valPrec.cc4Prec, "lbl_cedente_cessionario_acc_4", "", 0);
            await RileggoContattiPerValidita(false, ddlContatto2Nome, valPrec.cc2Prec, "lbl_cedente_cessionario_2", "inTipoIndirizzoCC2", valPrec.cc2IndPrec);
        } else {
            await RileggoContattiPerValidita(true, ddlContatto2Nome, valPrec.cc2Prec, "lbl_cedente_cessionario_2", "inTipoIndirizzoCC2", valPrec.cc2IndPrec);
        }

        await RileggoContattiPerValidita(true, "inVettore", valPrec.vettorePrec, "lbl_vettore", "inTipoIndirizzoVettore", valPrec.vettoreIndPrec);
        await RileggoContattiPerValidita(true, "inDipendenti", valPrec.dipendentePrec, "lbl_dipendente", "", 0);

        if (GetPropertyFromJson($(cIdOpzioniContab).val(), "SUPERUSER_COD_GESTIONE_AGENTI") === true) {
            await RileggoContattiPerValidita(true, "inAgente", valPrec.agentePrec, "lbl_agente", "", 0);
        }

        if (GetPropertyFromJson($(cIdOpzioniContab).val(), "SUPERUSER_COD_GESTIONE_CAPOAREA") === true) {
            await RileggoContattiPerValidita(true, "inCapoArea", valPrec.capoAreaPrec, "lbl_capoarea", "", 0);
        }
        WaitFrame.hide();
    }
}

function LeggiValoriAttuali() {
    let valoriPrecedenti = {};
    valoriPrecedenti.cc1Prec = parseInt(Get_KendoDDLValue(ddlContatto1Nome, 0));
    valoriPrecedenti.cc1IndPrec = parseInt(Get_KendoDDLValue("inTipoIndirizzoCC1", 0));
    valoriPrecedenti.cc2Prec = parseInt(Get_KendoDDLValue(ddlContatto2Nome, 0));
    valoriPrecedenti.cc2IndPrec = parseInt(Get_KendoDDLValue("inTipoIndirizzoCC2", 0));

    if (contattiAcc4ConGerarchia === true && lavCodAccettazione === true) {
        valoriPrecedenti.cc3Prec = parseInt(Get_KendoDDLValue(ddlContattoCoop1Nome, 0));
        valoriPrecedenti.cc4Prec = parseInt(Get_KendoDDLValue(ddlContattoCoop2Nome, 0));
    }

    valoriPrecedenti.vettorePrec = parseInt(Get_KendoDDLValue("inVettore", 0));
    valoriPrecedenti.vettoreIndPrec = parseInt(Get_KendoDDLValue("inTipoIndirizzoVettore", 0));
    valoriPrecedenti.dipendentePrec = parseInt(Get_KendoDDLValue("inDipendenti", 0));
    valoriPrecedenti.agentePrec = parseInt(Get_KendoDDLValue("inAgente", 0));
    valoriPrecedenti.capoAreaPrec = parseInt(Get_KendoDDLValue("inCapoArea", 0));

    return valoriPrecedenti;
}

async function RileggoContattiPerValidita(eseguiLettura, nomeDdl, valorePrecedente, nomeLabel, ddlIndirizzoCollegato, indirizzoPrec) {

    if (eseguiLettura === true) {
        console.log("BEFORE Ri-lettura [" + nomeDdl + "] numero elementi: " + KendoDDL(nomeDdl).dataSource.data().length);
        await KendoDDL(nomeDdl).dataSource.read();
        console.log("POST Ri-lettura [" + nomeDdl + "] numero elementi: " + KendoDDL(nomeDdl).dataSource.data().length);
    }

    //se c'era un valore già selezionato, provo a re-impostarlo
    if (valorePrecedente !== 0) {
        if (KendoDDL(nomeDdl).dataSource.data().filter(function (x) { return parseInt(x.Cod_RisUm) === valorePrecedente; }).length === 1) {
            //il contatto esiste ancora, lo re-imposto
            if (nomeDdl == ddlContatto1Nome || nomeDdl == ddlContatto2Nome) { // se e' una kendo dropdown virtuale devo impostare il valore con il corrispetivo metodo
                await Set_KendoDDLValueVirtual(nomeDdl, valorePrecedente);
            } else {
                Set_KendoDDLValue(nomeDdl, valorePrecedente);
            }
            $("#" + nomeDdl).change();

            if (indirizzoPrec !== 0) {
                Set_KendoDDLValue(ddlIndirizzoCollegato, indirizzoPrec);
                $("#" + ddlIndirizzoCollegato).change();
            }
        } else {
            //non mostro il messaggio in caso ho la modalità di conferimenti da raccolte perché il contatto potrebbe non essere più disponibile anche nel caso in cui non
            //abbia raccolte collegabili nel nuovo periodo se l'utente ha usato il pulsante di filtro apposito
            if (raccolteXConferimenti_AbilitazioneGenerale() === 0) {
                //era stato impostato un contatto che ora non è più presente nell'elenco a causa della validità
                let label = $("#" + nomeLabel).text();
                kendo.alert(kendo.format(TraduzioneMultiResx(resxObj, "ContattoIndisponibilePeriodoValidita",
                    "Il contatto {0} precedentemente selezionato non è più disponibile a causa del suo periodo di validità."), label));
            }
        }
    }
}

function inDataEmissione2_change(e) {
    //if ($("#" + idTipoIndirizzo).prop("required") === true) {
    VerificaDatiMinimiTestata();
    //}
}

//var necessarioRicalcoloNumDoc = false;
//function btnNecessarioRicalcoloNumDoc_click() {
//    //TODO: crea variabile globale per segnare quando è da ricalcolare (mette il numero di documento a zero, dovrà segnare il vecchio documento tra i buchi di numerazione)

//    //TODO: dare messaggio che verrà resettato il numero doc e chiedere se proseguire
//    alert("TODO: dare messaggio che verrà resettato il numero doc e chiedere se proseguire");

//    necessarioRicalcoloNumDoc = true; 

//    if (necessarioRicalcoloNumDoc === true) {
//        Set_KendoNumTBValue("inNumDoc", 0);
//        $("#iconNumDoc").removeClass("fa-chain");
//        $("#iconNumDoc").addClass("fa-chain-broken");

//        $("#btnNecessarioRicalcoloNumDoc").removeClass("btn-success");
//        $("#btnNecessarioRicalcoloNumDoc").addClass("btn-danger");
//    //} else {
//    //    //TODO: 
//    //    Set_KendoNumTBValue("inNumDoc", 999);
//    //    $("#iconNumDoc").removeClass("fa-chain-broken");
//    //    $("#iconNumDoc").addClass("fa-chain");

//    //    $("#btnNecessarioRicalcoloNumDoc").removeClass("btn-danger");
//    //    $("#btnNecessarioRicalcoloNumDoc").addClass("btn-success");
//    }
//}

function inScadenzaUnica_change(e) {
    let state = $(e.sender.element).data("kendoSwitch").check();

    if (state === true) {
        $("#groupDataEvasionePrevista").show();
        //$("#groupEvasioneTassativa").show();
        $("#groupDataSpedizionePrevista").show();
    } else {

        KendoDate("inDataEvasionePrevista").value(null);
        setKendoSwitch("inEvasioneTassativa", false);
        KendoDateTime("inDataSpedizionePrevista").value(null);

        $("#groupDataEvasionePrevista").hide();
        $("#groupEvasioneTassativa").hide();
        $("#groupDataSpedizionePrevista").hide();

    }
}

function inDataEvasionePrevista_change(e) {
    //se è data valida
    if (e.sender.value() !== null) {
        $("#groupEvasioneTassativa").show();
    } else {
        setKendoSwitch("inEvasioneTassativa", false);
        $("#groupEvasioneTassativa").hide();
    }
}

function inTrasportoCura_change(e) {

    //TODO: si potrebbe ridurre solo a vettore + default (negli altri casi c'è altro da impostare?!?)

    switch ($(this).val()) {
        case "0":   // Cedente / Fornitore
            RendiObbligatorio("inVettore", false);
            RendiObbligatorio("inTipoIndirizzoVettore", false);
            $("#inVettore").removeClass("DdlRequiredNoZero");
            break;
        case "1":   // Cessionario / Cliente
            RendiObbligatorio("inVettore", false);
            RendiObbligatorio("inTipoIndirizzoVettore", false);
            $("#inVettore").removeClass("DdlRequiredNoZero");
            break;
        case "2":   // Vettore
            RendiObbligatorio("inVettore", true);
            RendiObbligatorio("inTipoIndirizzoVettore", true);

            //mi tocca forzarlo aggiungendo questa classe + custom validation rule perché c'è l'elemento "0" che è il vuoto, 
            // ma per kendo è equiparato a tutte le altre voci, perciò secondo lui è valorizzato
            $("#inVettore").addClass("DdlRequiredNoZero");

            break;
        default:
            RendiObbligatorio("inVettore", false);
            RendiObbligatorio("inTipoIndirizzoVettore", false);
            $("#inVettore").removeClass("DdlRequiredNoZero");
            break;
    }

    VerificaDatiMinimiTestata();    //per il momento lo faccio sempre?!?

}

function btnSalvaTestataDoc_click() {

    SvuotaSegnalazioniErrori(true, true, false);

    let isValidIntestazione = validatorIntestazione.validate();
    let isValidTabTestata = validatorTabTestata.validate();

    if (isValidIntestazione === false || isValidTabTestata === false) {

        $.logThis("Validator Testata NO");
        EvidenziaErroriIntestazione();
        EvidenziaErroriTabTestata();

        //Se ho usato il pulsante di salva in fondo, non capisco che ci sono errori ==> salto al primo riepilogo errori che trovo
        if (isValidIntestazione === false) {
            document.getElementById("erroriMsgIntestazione").scrollIntoView();
        } else {
            document.getElementById("tabs").scrollIntoView();   //mi sposto sulla tab dove si vedono i badge
        }

    } else {

        $.logThis("Validator Testata OK");
        SalvaTestataDoc();

    }

}

function ControlliProduttore() {
    let msgErrore = "";

    if (lavCodAccettazione === true && contattiAcc4ConGerarchia === true &&
        parseInt($(cIdAgenda).val()) !== 0) {

        let oldData = JSON.parse($('input[name$="hdKendo_TestataDoc"]').val());
        let codRisumProduttore = Get_KendoDDLValue(ddlContatto2Nome, 0);
        codRisumProduttore = !isNumeric(codRisumProduttore) ? 0 : codRisumProduttore;
        if (parseInt(codRisumProduttore) !== oldData.CodDestinazione) {

            if (KendoGrid("tab_elenco_movimenti").dataSource.data().length > 1 ||
                (KendoGrid("tab_elenco_movimenti").dataSource.data().length === 1 &&
                    sonoInDettaglioRigaDoc === true &&
                    $('input[name$="hf_key_mov_dett"]').val() === "")) {

                //Ho più di una riga, oppure in griglia c'è una sola riga, ma sto aggiungendo una seconda riga
                // (visto che in questo caso sto salvando la testata e la seconda riga, ma non la prima riga, 
                // devo cmq dare errore perché sennò la prima riga potrebbe essere disallineata)

                msgErrore = TraduzioneMultiResx(resxObj, "CambioProduttoreMultipleRighe", "Il produttore del documento è stato cambiato. " +
                    "<br>Nel documento sono presenti molteplici righe, pertanto è necessario eliminare tutte le righe, tranne la prima," +
                    " quindi entrare in modifica della sola riga rimasta per aggiornare l'assegnazione degli impianti e salvare." +
                    "<br>A questo punto re-inserire gli altri dettagli del documento.");
            } else if (sonoInDettaglioRigaDoc === false) {
                msgErrore = TraduzioneMultiResx(resxObj, "CambioProduttoreUnaRiga", "Il produttore del documento è stato cambiato. " +
                    "<br>è necessario entrare dentro alla riga del documento per verificare l'assegnazione degli impianti e ri-salvare.");
            }

        }
    }

    return msgErrore;
}

function btnSalvaDoc_click(esciAlTermine, stampa, nuovoDoc) {

    // se accettazione pomodoro verifica se c'è contratto attivo
    if (lavCodAccettazionePomodoro) {
        let messaggioErrore = Verifica_Contratto_Pomodoro(false);
        if (messaggioErrore !== "") {
            MessaggioErrore_Bootstrap(messaggioErrore, "DIV_Messaggi");
            return;
        }
        //if ($("#ddlCodVarietaPomodoro").val() == "") {
        //    MessaggioErrore_Bootstrap("Codice varietà pomodoro non impostata.", "DIV_Messaggi");
        //    return;
        //}
        //if ($("#txtDescAppezzamenti").val() == "") {
        //    MessaggioErrore_Bootstrap("Descrizione appezzamenti non impostata.", "DIV_Messaggi");
        //    return;
        //}
    }

    // Controllo che non ci siano righe imballaggi in sospeso
    // Non posso utilizzare grid.dataSource.hasChanges() perché non utilizziamo il salvataggio standard
    if (sonoInModificaImballi) {
        MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "EsistonoRigheImballoNonSalvate", "SALVATAGGIO NON POSSIBILE: Esistono righe imballo non salvate"), "DIV_Messaggi");
    } else {

        SvuotaSegnalazioniErrori(true, true, true);

        let isValidIntestazione = validatorIntestazione.validate();
        let isValidTabTestata = validatorTabTestata.validate();

        let isValidDettaglio = true;
        if (sonoInDettaglioRigaDoc === true) {
            isValidDettaglio = validatorTabDettaglio.validate();
        }

        if (isValidIntestazione === false || isValidTabTestata === false || isValidDettaglio === false) {
            $.logThis("Validator Salva Tutto NO");
            EvidenziaErroriIntestazione();
            EvidenziaErroriTabTestata();

            if (sonoInDettaglioRigaDoc === true) {
                EvidenziaErroriTabDettaglio();
            }

            //Se ho usato il pulsante di salva in fondo, non capisco che ci sono errori ==> salto al primo riepilogo errori che trovo
            if (isValidIntestazione === false) {
                document.getElementById("erroriMsgIntestazione").scrollIntoView();
            } else {
                document.getElementById("tabs").scrollIntoView();   //mi sposto sulla tab dove si vedono i badge
            }

        } else {

            let msgErrore = "";

            if (sonoInDettaglioRigaDoc === true) {
                msgErrore = ControlliFormDettaglio();
            }

            if (msgErrore === "") {
                msgErrore = ControlliProduttore();
            }

            if (msgErrore === "" && isContrattoAffitto()) {
                msgErrore = ControllaDataConRiferimentiCatastali("inDataInizVal", enum_tipoDataValidita.Inizio, enum_ContestoControlloDate.Salvataggio);
                if (msgErrore !== "") {
                    msgErrore += "</br>"
                }
                msgErrore += ControllaDataConRiferimentiCatastali("inDataFineVal", enum_tipoDataValidita.Fine, enum_ContestoControlloDate.Salvataggio);
            }

            if (msgErrore !== "") {
                $.logThis("Validator Salva Tutto NO - Controlli Aggiuntivi Dettaglio");
                MessaggioErrore_Bootstrap(msgErrore, "DIV_Messaggi");
                //DisabilitaSalvataggio();
            } else {

                $.logThis("Validator Salva Tutto OK");

                //L'uscita dalla riga documento è spezzata:
                //1. In SalvaTestataPiuRigaDocumento viene già visualizzata la griglia
                //2. In UscitaDaRigaDoc vengono sistemati le variabili

                let risultatoSalva = false;
                risultatoSalva = SalvaTestataPiuRigaDocumento();

                //======================================================================
                //Gestione controllo trappole con innesco
                //======================================================================

                let richiestaCaricoInnesco = false;

                //Se tutte le seguenti condizioni sono rispettate:
                //salvataggio andato a buon fine, sono in dettaglio riga e tratta di un carico
                if (risultatoSalva === true && sonoInDettaglioRigaDoc === true && Qs_CaricoScarico === CAU_CARICO) {

                    // Verifico se si tratta di una trappola che richiede un innesco
                    if (SeTrappolaConInnesco()) {

                        //Se non è presente un innesco fra le righe, chiedo se si vuole fare il carico
                        if (!EsisteInnescoPerTrappola()) {

                            richiestaCaricoInnesco = true;
                            ChiediConfermaCaricoInnesco(risultatoSalva, stampa, esciAlTermine, nuovoDoc);

                        }

                    }

                }

                //======================================================================

                //Questo lo faccio sempre, anche se devo eventualmente inserire una nuova riga
                //di innesco a fronte di una trappola
                if (risultatoSalva) {
                    UscitaDaRigaDoc();
                }

                let risultatoControlliWorkflow = true;
                let mesControlliWorkflow = [];

                if (esciAlTermine || stampa) {
                    // Effettuo dei controlli che possono bloccare l'uscita dalla pagina per mostrare messaggi all'utente

                    let data = $('input[name$="hdKendo_RigheDoc"]').val();
                    let jsonParsed_Kendo = JSON.parse(data);

                    if (GetPropertyFromJson($(cIdOpzioniContab).val(), "GruppoMerce_Controllo") == true) {
                        let gruppiMerceAssociati = jsonParsed_Kendo.kendo_rows.every(function (x) { return x.Id_Gruppo_Merce !== 0; });

                        risultatoControlliWorkflow = risultatoControlliWorkflow && gruppiMerceAssociati;

                        if (!gruppiMerceAssociati) {
                            mesControlliWorkflow.push("Inserire il Gruppo Merce per tutti i prodotti usati nel documento per permetterne l'invio");
                        }
                    }

                    if (flagSceltaImputazione && GetPropertyFromJson($(cIdOpzioniContab).val(), "CdC_Wbs_Controllo") == true) {

                        let cdcWbsAssociati = jsonParsed_Kendo.kendo_rows.every(function (x) { return x.CdC_Des_Imputazione !== ""; });

                        risultatoControlliWorkflow = risultatoControlliWorkflow && cdcWbsAssociati;

                        if (!cdcWbsAssociati) {
                            mesControlliWorkflow.push("Inserire il CdC/Wbs in tutti i dettagli del documento per permetterne l'invio");
                        }
                    }

                    if (GetPropertyFromJson($(cIdOpzioniContab).val(), "IB_RisorseUmane_SettoreDes_Controllo") == true) {
                        let progressivoCorretto = false;
                        let progressivoPrimoContatto = $("#" + txtProgressivoNome + "CC1").val();

                        if (progressivoPrimoContatto !== "") {

                            let causaleDocDataItem = KendoDDL(idControlloCausaleTrasporto).dataItem();

                            let regexProgressivo = /[FC]\d+/; // Questa sintassi crea un oggetto di classe Regex

                            if (causaleDocDataItem.Causale_Trasporto_Cod == 15 || causaleDocDataItem.Causale_Trasporto_Des.toUpperCase() == "MERCE RESA") {
                                regexProgressivo = /F\d+/;
                            }
                            else {
                                if (lavCodVendita) {
                                    regexProgressivo = /C\d+/;
                                }
                                else {
                                    regexProgressivo = /F\d+/;
                                }
                            }

                            progressivoCorretto = regexProgressivo.test(progressivoPrimoContatto);
                        }

                        risultatoControlliWorkflow = risultatoControlliWorkflow && progressivoCorretto;

                        if (!progressivoCorretto) {
                            mesControlliWorkflow.push("Il codice del contatto non è corretto, controllare di aver scelto il contatto con il rapporto contabile giusto " +
                                "e nel caso aggiornare il codice dall'anagrafica del contatto nella sezione 'rapporti contabili' ");
                        }
                    }

                    if (mesControlliWorkflow.length > 0) {
                        $("<div></div>").kendoAlert({
                            content: "- " + mesControlliWorkflow.join("<br/>- "),
                            title: "Attenzione!"
                        }).data("kendoAlert").open();
                    }
                }

                if (richiestaCaricoInnesco === false && risultatoControlliWorkflow) {
                    SeStampaEsciAlTermine(risultatoSalva, stampa, esciAlTermine, nuovoDoc);
                }
            }

        }

    }

}

function btnSalvaDataDoc_click() {
    //devo recuperare la vecchia data documento
    let oldDoc = JSON.parse($('input[name$="hdKendo_TestataDoc"]').val());
    let attualeDataDoc = kendo.parseDate(oldDoc.DataMovimento);

    let newDataDoc = kendo.parseDate(KendoDate("inDataEmissione").value());

    console.log("Qui passo lato server per salvare: Vecchia data [" + attualeDataDoc + "] - Nuova data [" + newDataDoc + "]");
    let risModifica = ModificaDataDocumento($(cIdPiva).val(), parseInt($(cIdAgenda).val()), cIdLavCod, Qs_CaricoScarico, attualeDataDoc, newDataDoc);

    //TODO: se salvataggio ok, allora devo ribloccare la data e rileggere le righe...

    console.debug(risModifica);
    //TODO: devo ricaricare il documento...
}

function btnDocmmenti_click() {

    let areaProvenienza = "10";

    if (cIdLavCod === enum_LavCod.Contratto_Affitto.value) {
        areaProvenienza = "12";
    }

    var url = "../Scadenzario/Scad_lista.aspx?type=doc" + "&area_provenienza=" + areaProvenienza + "&p=" + $(cIdPiva).val() + "&id_agenda=" + $(cIdAgenda).val();

    $(document.body).append('<div id="ricerca_documentale"></div>');
    $('#ricerca_documentale').kendoWindow({
        title: TraduzioneMultiResx(resxObj, "RicercaDocumenti", "Ricerca Documenti"),
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#ricerca_documentale').kendoWindow('destroy');
        }
    }).data('kendoWindow').center().maximize();
}




function ApriKendoWindowAggiungiNuovoAllegato() {

    var ID_Alert_Entita = -1;
    var ID_Elenco = -1;
    var Modalita = "doc";
    var Tipologia = "";
    var Piva = $(cIdPiva).val();
    var Id_Agenda = parseInt($(cIdAgenda).val());
    var IdArea = 10;

    if (Tipologia == undefined || Tipologia == null || Tipologia == "") {
        switch (cIdLavCod) {
            case 2004: //Ordine Acquisto           
                Tipologia = -18;
                break;
            case 1025: //DDT Ricevuto              
                Tipologia = -19;
                break;
            case 1054:
            case 1076:
            case 1078: //Conferimento    
                Tipologia = -20;
                break;
            case 1031: //DDT Emesso                
                Tipologia = -21;
                break;
            case 2002: //Ordine Vendita            
                Tipologia = -22;
                break;
            case 1000: //Ordine Vendita
                Tipologia = -23;
                break;
            case 1001: //Fattura emessa            
                Tipologia = -24;
                break;
            case 2006: //Contratto d'affitto
                Tipologia = -26;
                IdArea = 12;
                break;
        }
    }


    var param = kendo.stringify({ 'Piva': Piva, 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita, 'Id_Area': IdArea, 'Tipologia': Tipologia, 'area_provenienza': IdArea, 'Id_Agenda': Id_Agenda });
    //var param = kendo.stringify({ 'ID_Elenco': ID_Elenco, 'TipoOperazione': 1, 'ID_Alert_Entita': ID_Alert_Entita });
    var url = "../Scadenzario/Scad_CreaModificaItem.aspx?scadstr=" + param + "&type=" + Modalita + "&p=" + Piva;



    //var url = "../Scadenzario/Scad_CreaModificaItem.aspx?type=doc" + "&area_provenienza=" + "10" + "&p=" + dataItem.PIVA + "&id_agenda=" + dataItem.Id_Agenda + "&Tipologia=" + dataItem.Tipo;


    $(document.body).append('<div id="ricerca_documentale"></div>');
    $('#ricerca_documentale').kendoWindow({
        title: TraduzioneMultiResx(resxObj, "NuovoAllegato", "Nuovo Allegato"),
        modal: true,
        resizable: true,
        iframe: true,
        width: "80%",
        height: "80%",
        content: url,
        actions: ["Maximize", "Close"],
        close: function () {
            $('#ricerca_documentale').kendoWindow('destroy');
            MostraBtnDocumenti();
        }
    }).data('kendoWindow').center().maximize();
}



function Azione_Indietro_DocContabile() {
    //Se l'hd è valorizzato allora vuol dire che arrivo dalle liquidazioni e quindi chiudi
    //l'iframe invece che fare il redirect.
    switch (cPagRitorno) {
        case "aperturadaIframe":
            window.parent.chiudiKendoWindowDocContabileRisultatoLiquidazioneUC();
            break;

        case "aperturadaFinestra":
            window.close();
            break;

        case "apertodaGiasNG":
            window.parent.postMessage("chiudiWindowGiasNG", ottieniTargetOrigin(window));
            break;

        default:
            //Azione_Indietro();
            var urlRedirect = Url_Indietro_DocContabile();
            window.location.href = urlRedirect;
            break;
    }
    //if (cPagRitorno === "aperturadaIframe") {
    //    window.parent.chiudiKendoWindowDocContabileRisultatoLiquidazioneUC();
    //} else {
    //    if (cPagRitorno === "aperturadaFinestra") {
    //        window.close();
    //    }
    //    else {
    //        Azione_Indietro();
    //    }
    //}
}

function btnForzaEvasioneDocumento_click() {

    let kendoConfirm = $("<div></div>").kendoConfirm({
        title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
        messages: { okText: TraduzioneMultiResx(resxObj, "Si", "Sì"), cancel: TraduzioneMultiResx(resxObj, "No", "No") },
        content: TraduzioneMultiResx(resxObj, "ConfermaEvasioneForzataRigheDoc", "Confermare l'evasione forzata di tutte le righe ancora inevase del documento?")
    }).data("kendoConfirm");

    kendoConfirm.result.done(function () {

        var grid = KendoGrid("tab_elenco_movimenti");
        if (grid !== undefined && grid !== null) {
            let ds = grid.dataSource.data();

            let listDet = [];
            for (let i = 0; i < ds.length; i++) {
                if (ds[i].Cat_Cod !== RIGA_DESCRIZIONE_LIBERA && ![2, 5].includes(ds[i].StatoEvasione_Cod)) {
                    listDet.push(ds[i].Id_Mov_Det);
                }
            }

            if (listDet.length > 0) {
                ForzaEvasioneRigheOrdine(listDet, true);
            }
        }

    });

    kendoConfirm.open();
}

function onTabPrincipaliShown(e) {
    Carica_Stati_PanelBar();

    let target = $(e.target).attr("id"); // activated tab

    switch (target) {
        case "a_tabRiepilogoPesi":
            if (FF_gest_materiale_vivaistico) {
                $("#lblPesoNettoRiepilogo").text("Numero:");
                $("#lblPesoLordoRiepilogo").hide();
                KendoNumTB("inPesoLordoRiepilogo").wrapper.hide();
                $("#idValoreDegradoRiepilogo").hide();
                $("#idPesoPagamentoRiepilogo").hide();
            } else if (!lavCodAccettazione) {
                $("#idValoreDegradoRiepilogo").hide();
                $("#idPesoPagamentoRiepilogo").hide();
            }
            break;

        case "a_tabDettagliDoc":
            if (parseInt($(cIdAgenda).val()) === 0) {
                if (Get_KendoDDLValue("ddlProdottoDes") === "") {
                    //alert("non ci sono righe di dettaglio inserite, vai direttamente al dettaglio");
                    DocContabileNuovaRiga();
                }
            } else {
                var gridId = "tab_elenco_movimenti";
                var grid = KendoGrid(gridId);
                if (grid !== undefined && grid.dataSource.data().length > 0) {
                    if (lavCodAccettazionePomodoro) {
                        ApriModificaDettaglioRiga(grid.dataSource.data()[0], cIdTipoOp);
                        // AggiornaDettagliEconomici();
                    } else {
                        kendo_AggiustaDimensioneColonne("#" + gridId);
                        grid.resize(); //sono costretta a rifarlo, perché sennò si vede dello spazio bianco in fondo alla griglia
                    }
                }
            }
            break;
    }

}

function onSelectPanelBar(e) { }

function onCollapsePanelBar(e) {

    var item = $(e.item);
    var panelBarID = item[0].id;
    var panelBarParentId = item.parent().prop("id");
    var nomeCompleto = panelBarParentId + "." + panelBarID;
    if (!panelBarToccatiDopoLoad.includes(nomeCompleto)) {
        panelBarToccatiDopoLoad.push(nomeCompleto);
    }

    Salva_Stato_PanelBar(panelBarParentId, panelBarID, 0);
}

function onExpandPanelBar(e) {
    var item = $(e.item);
    var panelBarID = item[0].id;
    var panelBarParentId = item.parent().prop("id");

    var nomeCompleto = panelBarParentId + "." + panelBarID;
    if (!panelBarToccatiDopoLoad.includes(nomeCompleto)) {
        panelBarToccatiDopoLoad.push(nomeCompleto);
    }

    Salva_Stato_PanelBar(panelBarParentId, panelBarID, 1);
}

function onActivatePanelBar(e) {
    if (e.item.id === "panelBar_RaccolteXConferimenti") {
        raccolteConfUC_set();
    }
    // carico la griglia quando viene espanso il pannello ordini cliente
    if (e.item.id === "panelBar_OrdiniCliente" && KendoDDL(ddlContatto1Nome).dataItem() != null) {
        // Passo il lav_cod del documento perché lato server lo utilizzo per distinguere quali ordini devo ricercare e quali elem_cod devo escludere
        Ricerca_OrdiniFormProdottoUC($(cIdPiva).val(), KendoDDL(ddlContatto1Nome).dataItem().Cod_RisUm, cIdLavCod);
    }
    if (e.item.id === "panelBar_DDTCliente" && KendoDDL(ddlContatto1Nome).dataItem() != null) {
        Ricerca_DDTFormProdottoUC($(cIdPiva).val(), KendoDDL(ddlContatto1Nome).dataItem().Cod_RisUm, cIdLavCod);
    }
}

function onShowTabStrip(e) {
    Carica_Stati_PanelBar();
}

function SeTrappolaConInnesco() {

    let w_usoTrappola = 0;

    let k_ddlProDes = KendoDDL("ddlProdottoDes");

    if (k_ddlProDes.dataItem() !== undefined &&
        k_ddlProDes.dataItem().Elem_Cod !== undefined) {

        if (k_ddlProDes.dataItem().Elem_Cod === TRAPPOLE) {

            if (k_ddlProDes.dataItem().Uso === undefined || k_ddlProDes.dataItem().Uso === null) {
                RileggiProdotto(TRAPPOLE, k_ddlProDes)
            }

            w_usoTrappola = k_ddlProDes.dataItem().Uso;
        }

    }

    if (w_usoTrappola === enum_TrappoleUso.Monitor || w_usoTrappola === enum_TrappoleUso.CattureDiMassa) {
        return true
    } else {
        return false
    }

}

function RileggiProdotto(categoriaMagazzino, controlloDDL) {

    let w_Prodotto_Cod = controlloDDL.dataItem().Prodotto_Cod;

    let elencoProdottiCompleto = RicercaElencoCompletoProdotti(
        objP_super_server,
        objP_server,
        objP_utenti,
        $(cIdPiva).val(),
        xSa_Cod,            //---|
        xFabbricato_Cod,    //   |===> variabili globali
        xTipoDestinazione,  //---|
        categoriaMagazzino,
        false,              //soloInGiacenza
        null,               //FiltroDescrizioneProdotto
        Qs_Mode,
        Qs_CaricoScarico,
        get_data("inDataEmissione"),
        0,                  //xPUARegolamento
        null,               //xLottoAccettazione
        false,              //leggiUMformulati
        false,              //Flag_QtaNoZero
        0,                  //xTipoPUARegolamento
        null,               //Specie_Array
        null,               //Varieta_Array
        null,               //xFiltroAggiuntivoMateriePrime
        false,              //creaGriglia
        -1,                 //filtroProdottiValorizzati
        true,               //flagDiversificaDesFertilizzanti
        w_Prodotto_Cod);    //FiltroCodiceProdotto

    let dataSource = new kendo.data.DataSource({
        data: JSON.parse(elencoProdottiCompleto)
    });

    controlloDDL.unbind("filtering");
    controlloDDL.setDataSource(dataSource);
    controlloDDL.dataSource.read();

    Set_KendoDDLValue("ddlProdottoDes", w_Prodotto_Cod);

}

function EsisteInnescoPerTrappola() {

    let trovatoInnesco = false;

    //Ubicazione carico
    let ubicazioneCarico = Get_KendoDDLValue("ddlUbicDestinazione");

    //Recupero dati griglia
    var grid = $("#tab_elenco_movimenti").data("kendoGrid");
    var data = grid.dataSource.data();

    //Inizializzazione variabili riga griglia
    let riga = {
        catMag: 0,
        destCar: ""
    }

    //Ciclo sulle righe per verificare se è presente un innesco sullo stesso magazzino
    $.each(data, function (index, item) {
        riga.catMag = parseInt(item.Cat_Cod)
        riga.destCar = item.key_Dest
        if (riga.catMag === INNESCHI && riga.destCar === ubicazioneCarico) {
            trovatoInnesco = true;
        }
    });

    return trovatoInnesco;

}

function ChiediConfermaCaricoInnesco(risultatoSalva, stampa, esciAlTermine, nuovoDoc) {

    var kendoConfirm = $("<div></div>").kendoConfirm({
        title: TraduzioneMultiResx(resxObj, "Attenzione", "Attenzione"),
        content: TraduzioneMultiResx(resxObj, "ConfermaCaricamentoInnescoPerScaricareTrappola",
            "Per poter scaricare la trappola selezionata occorre caricare anche un relativo innesco.<br/>Si desidera caricarlo?"),
        messages: {
            okText: TraduzioneMultiResx(resxObj, "Si", "Sì"),
            cancel: TraduzioneMultiResx(resxObj, "No", "No")
        }
    }).data("kendoConfirm");

    kendoConfirm.result.done(function () {
        propostaDatiRiga = {
            categoriaMagazzino: INNESCHI,
            codiceTrappola: KendoDDL("ddlProdottoDes").dataItem().Prodotto_Cod
        }
        DocContabileNuovaRiga();
        propostaDatiRiga = undefined;
    });

    kendoConfirm.result.fail(function () {
        SeStampaEsciAlTermine(risultatoSalva, stampa, esciAlTermine, nuovoDoc);
    });

    kendoConfirm.open();

}



function SeStampaEsciAlTermine(risultatoSalva, stampa, esciAlTermine, nuovoDoc) {

    if (risultatoSalva === true && stampa === true) {
        StampaDoc();
    }

    if (risultatoSalva === true && nuovoDoc === true) {

        if ($(hf_UrlNuovoDocumento).val() !== "") {

            // Salvo la barra del titolo con la chiamata ajax e nella callback MenuBS_2017.js/gestioneRedirect ricarico la pagina
            var url_MenuBS_WS = "../Menu/MenuBS_2017.aspx";
            var idSezione = $(hf_IdSessionNuovoDoc).val();

            ajaxAgronica(url_MenuBS_WS + "/salvaTitoloSezioneConGestioneRedirect",
                "{ IDSezione: " + idSezione + "  }",
                gestioneRedirect, null);

        }

    }

    if (risultatoSalva === true && esciAlTermine === true) {
        Azione_Indietro_DocContabile();
    }

}


function inDistanzaLock_change(e) {
    let state = e.checked;

    if (state === true) {
        console.log("Ho bloccato la distanza");
        //Ho appena bloccato ==> rendo non editabile la data
        RiBloccoDistanzaDoc(false);

    } else {
        //Ho appena sbloccato ==> rendo editabili i numeri
        DecidoDiSbloccareDistanza();


    }
}

function DecidoDiSbloccareDistanza() {

    //lock è stato spento

    KendoDDL("ddlUnitaMisuraTrasporto").enable(true);
    KendoNumericBox("ntbDistanzaTrasporto").enable(true);

    $("#btnSalvaDistanzaDoc").attr("style", "display:inline-block");
    $("#btnSalvaDistanzaDoc").attr("style", "background-color:#3fdc4b");

    //Nascondo i pulsanti di salvataggio
    VisualizzaPulsantiSalvataggio(false);

}

function RiBloccoDistanzaDoc(impostaAncheLock, gestisciVisibilitaSalvataggio) {

    //Ho appena bloccato ==> rendo non editabile la data

    if (impostaAncheLock === true) {
        setKendoSwitch("inDistanzaLock", true);
    }


    KendoDDL("ddlUnitaMisuraTrasporto").enable(false);
    KendoNumericBox("ntbDistanzaTrasporto").enable(false);

    $("#btnSalvaDistanzaDoc").attr("style", "display:none");

    //Visualizzo i pulsanti di salvataggio
    if (gestisciVisibilitaSalvataggio === undefined || gestisciVisibilitaSalvataggio === true) {
        VisualizzaPulsantiSalvataggio(true);
    }
}


function btnSalvaDistanzaDoc_click() {

    let udm_cod = KendoDDL("ddlUnitaMisuraTrasporto").value();
    let distanza = $("input[name$='ntbDistanzaTrasporto']").val();
    let valore = CalcoloStandardFactor(distanza, 0, udm_cod, 1, $("#inStatoCC1").val());

    console.log("Qui passo lato server per salvare la nuova distanza");
    let risModifica = ModificaDistanzaDocumento($(cIdPiva).val(), parseInt($(cIdAgenda).val()), udm_cod, distanza, valore);

    console.debug(risModifica);

}

function GestioneVisibilitaDistanzaLock() {

    if (cIdLavCod !== enum_LavCod.DDT_Emesso.value || parseInt($(cIdAgenda).val()) == 0 || $("input[name$='hf_UtenteAbilitatoGestioneGHG']").val() === "False") {
        $("#groupDistanzaLock").hide();
        KendoDDL("ddlUnitaMisuraTrasporto").enable(true);
        KendoNumericBox("ntbDistanzaTrasporto").enable(true);

    } else {

        $("#groupDistanzaLock").show();


    }

}