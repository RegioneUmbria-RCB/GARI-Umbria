function Url_Indietro_DocContabile() {
    var url = "";
    let param = kendo.stringify({
        codPagRitorno: cPagRitorno,
        piva: $(cIdPiva).val(),
        ricercaType: $(cIdRicercaType).val(),
        ricercaDoc: $(cIdRicercaDoc).val()
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Url_Indietro_DocContabile",
        param,
        false,
        function (risposta) {
            url = risposta.RispostaStringa;
        }, function (risposta) {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
        });

    return url;
}

function RecuperaValiditaSportello(servizioCod) {
    let res;
    let param = kendo.stringify({
        Piva: $(cIdPiva).val(),
        ServizioCod: servizioCod,
        objP_server: objP_server,
        objP_utenti: objP_utenti
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/RecuperaValiditaSportello",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            res = risp;
        }, function (risposta) {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
        }, null, false);

    return res;
}

function LeggiTestataDocumentoContab(options) {
    let idAgenda = parseInt($(cIdAgenda).val());

    let testataDoc = RicercaTestataDocumentoContab($(cIdPiva).val(), 0, idAgenda, cIdLavCod);
    $('input[name$="hdKendo_TestataDoc"]').val(JSON.stringify(testataDoc));

    options.success(testataDoc);
}

function RicercaTestataDocumentoContab(piva, saCod, idAgenda, lavCod) {

    var testataDoc = null;
    var param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: piva,
        saCod: saCod,
        idAgenda: idAgenda,
        lavCod: lavCod
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/LeggiTestataDocumento",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            testataDoc = risp;
        }, function (risposta) {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
        });

    return testataDoc;
}

function ModificaTestataDocumentoContab(piva, testataDoc, messaggioDettagliato) {

    let risultatoModifica;

    let param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: piva,
        contabTestata: kendo.stringify(testataDoc),
        messaggioDettagliato: messaggioDettagliato
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/ModificaTestataDocumento",
        param,
        false,
        function (risposta) {
            risultatoModifica = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            if (risposta !== undefined && risposta !== null && risposta.RispostaOK === false) {
                risultatoModifica = JSON.parse(risposta.RispostaStringa);
                MessaggioErrore_Bootstrap(risultatoModifica.MsgError + "<br/>" + TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
            } else {
                MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreSalvataggio", "Errore salvataggio"), "DIV_Messaggi");
            }
        });

    return risultatoModifica;
}

function ScriviTestataDocumentoContab(piva, testataDoc, messaggioDettagliato) {

    let risultatoInserimento;

    let param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: piva,
        contabTestata: kendo.stringify(testataDoc),
        messaggioDettagliato: messaggioDettagliato
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/ScriviTestataDocumento",
        param,
        false,
        function (risposta) {
            risultatoInserimento = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            if (risposta !== undefined && risposta !== null && risposta.RispostaOK === false) {
                risultatoInserimento = JSON.parse(risposta.RispostaStringa);
                MessaggioErrore_Bootstrap(risultatoInserimento.MsgError + "<br/>" + TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
            } else {
                MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreSalvataggio", "Errore salvataggio"), "DIV_Messaggi");
            }
        });

    return risultatoInserimento;
}

function SalvaTestataPiuRigaDocumento() {

    let risultatoSalva = false;

    let contabTestata = "";

    let testataDoc = GeneraOggettoTestata();
    contabTestata = kendo.stringify(testataDoc);

    let idMovCS = 0;
    let idMovDet = 0;
    let contabDettaglio = "";
    if (sonoInDettaglioRigaDoc === true) {

        //riprendo i dati che avevo letto, così ho già molte cose valorizzate come idAgenda, id_Mov_Det e dataOraUltimaLettura
        let rigaDoc = getDatiContabRiga();

        //TODO: verificare se servono tutti i campi
        let objCampi = EstraiCampiInput();

        if (objCampi !== undefined && objCampi !== null) {

            objCampi.Fornitore = parseInt(Get_KendoDDLValue(ddlContatto1Nome));

            AggiungiValoriParametriQualitativi(objCampi);
            AggiungiValoriGHG_Registrazioni(objCampi);

            //Riempio con campi della riga originale
            if (rigaDoc !== undefined && rigaDoc !== null) {
                objCampi.DataOraUltimaLettura = rigaDoc.DataOraUltimaLettura;
            }

            objCampi.Piva = testataDoc.Piva;
            objCampi.IdAgenda = testataDoc.IdAgenda;

            if (objCampi.IdMov === 0) {
                switch (Qs_CaricoScarico) {
                case CAU_CARICO:
                    if (testataDoc.MovimentoCarico !== undefined && testataDoc.MovimentoCarico !== null) {
                        objCampi.IdMov = testataDoc.MovimentoCarico.IdMov;
                    }
                    break;

                case CAU_SCARICO:
                    if (testataDoc.MovimentoScarico !== undefined && testataDoc.MovimentoScarico !== null) {
                        objCampi.IdMov = testataDoc.MovimentoScarico.IdMov;
                    }
                    break;

                case CAU_TRASFERIMENTO:
                    if ((testataDoc.MovimentoScarico !== undefined && testataDoc.MovimentoScarico !== null) &&
                        (testataDoc.MovimentoCarico !== undefined && testataDoc.MovimentoCarico !== null)) {

                        //In questo caso dovrei avrei scritto entrambi i movimenti di carico e scarico
                        //TODO: devo restituirmeli in due oggetti diversi
                        objCampi.IdMov = testataDoc.MovimentoScarico.idMov;
                        objCampi.IdMov = testataDoc.MovimentoCarico.idMov;

                    }
                    break;
                }
            }

            idMovCS = objCampi.IdMov;
            idMovDet = objCampi.IdMovDet;

            contabDettaglio = kendo.stringify(objCampi);
        } else {
            return false;
        }


    }

    let param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        contabTestata: contabTestata,
        contabDettaglio: contabDettaglio,
        cauMov: Qs_CaricoScarico,
        idMovCS: idMovCS,
        idMovDet: idMovDet,
        messaggioDettagliato: true
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/SalvaTestataPiuRigaDocumento",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            console.debug(risp);
            console.log("riporta i nuovi valori --> non serve se poi chiudo direttamente la riga e rileggo la griglia (se è la prima riga ed ho anche salvato la testata, le chiavi di testata devo essere scritte)");

            if (risp !== undefined && risp !== null && risp.Risultato === true) {

                let msgOk = "Salvataggio effettuato correttamente";

                //la testata la salvo sempre; se non era la prima scrittura molte delle cose quin dentro non sarebbe necessario sovrascriverle, 
                //ma meglio essere lineare e fargliele sempre fare, tanto male non fanno
                console.log("Ho salvato la testata!");
                msgOk = AggiornaValoriPostSalvaAgenda(testataDoc, Qs_CaricoScarico, risp);

                //Non ho bisogno di farlo, per il momento, perché i totali vengono già impostata dal PostSalvaAgenda
                //Aggiorno conteggi riepilogo pesi e altri legati alla riga
                //AggiornaValoriPostSalvaDettaglio(testataDoc, risp);

                // Rendo visibile il TAB elenco righe
                ImpostaVisibilitaElencoOSingolaRiga(lavCodAccettazionePomodoro);
                
                //ricarico
                ResetCampiChiaveFormProdotto();

                //TODO: lo devo fare sempre o solo quando ero in edit singola riga?!?
                if (sonoInDettaglioRigaDoc === true || lavCodOrdine === true) {

                    //Ricarico Griglia dettagli movimento
                    DocContabileRicercaMovimenti(indirizzoHttp_DocContabile_WS, "tab_elenco_movimenti", $(cIdPiva).val(), $(cIdAgenda).val(), cIdLavCod);

                    //Ricarico griglia per confezionamenti collegati e mostro TAB Beni confezionamento
                    if (isGestitoTabConfezionamento()) {
                        ConfiguraGrigliaCaricoEredita("tab_griglia_carico_eredita");
                        $("#a_tabBeniConfezionamento").show();
                    } else {
                        $("#a_tabBeniConfezionamento").hide();
                    }
                }


                //Disabilitazione ChkAccompagnatoria
                if (lavCodFattura) {
                    $("#chkAccompagnatoria").data("kendoSwitch").enable(false);
                }

                let msgRiepilogoPesi = VerificaCongruenzaPesi();
                if (msgRiepilogoPesi !== "")
                    alert(msgRiepilogoPesi);

                MessaggioTuttoOK_Bootstrap(msgOk, "DIV_Messaggi");

                risultatoSalva = true;

            }

        }, function (risposta) {

            let msgError = TraduzioneMultiResx(resxObj, "ErroreSalvataggio", "Errore salvataggio");

            if (risposta !== undefined && risposta !== null && risposta.RispostaOK === false) {
                let risp = JSON.parse(risposta.RispostaStringa);

                if (risp.MsgError !== "" && risposta.Errore !== "") {
                    msgError = risp.MsgError + "<br/>" + TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore;
                } else if (risp.MsgError !== "") {
                    msgError = risp.MsgError;
                } else if (risposta.Errore !== "") {
                    msgError = risposta.Errore;
                }

            }

            MessaggioErrore_Bootstrap(msgError, "DIV_Messaggi");

            risultatoSalva = false;
        });

    return risultatoSalva;

}

function VerificaModificaDataDocumento(piva, idAgenda, lavCod, cauMov, dataDoc, newDataDoc) {
    let param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: piva,
        idAgenda: idAgenda,
        lavCod: lavCod,
        cauMov: cauMov,
        dataDoc: dataDoc,
        newDataDoc: newDataDoc
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/VerificaModificaDataDocumento",
        param,
        false,
        function(risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            console.debug(risp);
        },
        function (risposta) {
            if (risposta !== undefined && risposta !== null && risposta.RispostaOK === false) {
                let risp = JSON.parse(risposta.RispostaStringa);
                console.debug(risp);
                MessaggioErrore_Bootstrap(risp.MsgError + "<br/>" + TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
            } else {
                MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreSalvataggio", "Errore salvataggio"), "DIV_Messaggi");
            }
        });
}

function ModificaDataDocumento(piva, idAgenda, lavCod, cauMov, attualeDataDoc, newDataDoc) {

    let testataDoc = GeneraOggettoTestata();

    let risultatoModifica;

    let param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: piva,
        idAgenda: idAgenda,
        lavCod: lavCod,
        cauMov: cauMov,
        attualeDataDoc: attualeDataDoc,
        newDataDoc: newDataDoc,
        contabTestata: kendo.stringify(testataDoc)
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/ModificaDataDocumento",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            console.debug(risp);
            //TODO: devo ricaricare il documento

            if (risp !== undefined && risp !== null && risp.Risultato === true) {

                let msgOk = "Salvataggio effettuato correttamente";

                console.log("Ho modificato la data!");
                //msgOk = AggiornaValoriPostSalvaAgenda(testataDoc, Qs_CaricoScarico, risp);


                MessaggioTuttoOK_Bootstrap(msgOk, "DIV_Messaggi");

                risultatoModifica = true;

                //Refresh page!!!
                setTimeout(function () {
                    window.location.reload();
                }, 1000);
            }

        },
        function (risposta) {
            if (risposta !== undefined && risposta !== null && risposta.RispostaOK === false) {
                risultatoModifica = JSON.parse(risposta.RispostaStringa);
                let msg = risultatoModifica.MsgError;
                if (risposta.Errore !== "") {
                    msg = msg + "<br/>" + TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore;
                }
                MessaggioErrore_Bootstrap(msg, "DIV_Messaggi");
            } else {
                MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreSalvataggio", "Errore salvataggio"), "DIV_Messaggi");
            }
        });

    return risultatoModifica;
}


async function LeggiSezionali(options) {
    try {
        let tipoValue = 0; // Tipo_Value = 0 -> Sezionale_Cod

        let resp = await RicercaSezionali($(cIdPiva).val(), tipoValue, false);
        options.success(resp);

    } catch (err) {
        let msgErr = "Errore in lettura sezionali: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

async function LeggiModPagamento(options) {
    try {

        options.success(RicercaPagamenti_Causali(true, $(cIdPiva).val(), 0).sort((a, b) => (a.Cau_Pagamento_Sigla > b.Cau_Pagamento_Sigla) ? 1 : -1));        

    } catch (err) {
        let msgErr = "Errore in lettura causali di pagamento: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

async function LeggiAspettoBeni(options) {
    try {
        // TODO In futuro con le cantine passare w_modulo_anagrafe_log valorizzato
        //let w_modulo_anagrafe_log = moduloFromElemCod(wElemCod);
        let w_modulo_anagrafe_log = 0;
        let resp = await RicercaAspettoBeni(w_modulo_anagrafe_log, false).then(function (data) {
            options.success(data);
        }).catch(new Error("LeggiAspettoBeni"));
        //options.success(resp);

    } catch (err) {
        let msgErr = "Errore in lettura aspetto beni: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

async function LeggiCausaliContabilita(options) {
    try {

        let categoria = "";
        switch (cIdLavCod) {
            case enum_LavCod.Nota_Accredito_Emessa.value:
            case enum_LavCod.Nota_Accredito_Ricevuta.value:
                categoria = "NC";
                break;
            case enum_LavCod.Ordine_Vendita_Emesso.value:
            case enum_LavCod.Ordine_Acquisto.value:
            case enum_LavCod.DDT_Ricevuto.value:
            case enum_LavCod.DDT_Emesso.value:
            case enum_LavCod.DDT_Contabilizzato_Emesso.value:
            case enum_LavCod.Fattura_Emessa.value:
            case enum_LavCod.Fattura_Ricevuta.value:
                categoria = "FA";
                break;
            default:
                categoria = "NON_USATA";    //per fare in modo che non ritorni nulla
        }

        let cauContab = await RicercaCausaliContabilita(true, $(cIdPiva).val(), 0, categoria, "", "", false).then(function (data) {
            options.success(data);
        }).catch(new Error("LeggiCausaliContabilita"));
        //options.success(cauContab);

    } catch (err) {
        let msgErr = "Errore in lettura causali contabilità: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

async function LeggiCausaliTrasporto(options) {
    try {
        //in realtà solo per alcuni casi di accettazione nel lan viene valorizzata, sennò, anche in presenza di FF o cantina viene cmq passato 0
        // Al momento gli passo uno di quelli gestiti, tanto poi nella ricerca causali lo testa per = 0 o != 0
        let w_modulo_anagrafe_log = moduloFromElemCod(TRASFORMATI_VEGETALI);
        if (w_modulo_anagrafe_log === 0)
            w_modulo_anagrafe_log = moduloFromElemCod(TRASFORMATI_ANIMALI);
        let cauTrasp = await RicercaCausaliTrasporto(w_modulo_anagrafe_log, cIdLavCod, false).then(function(data) {
            options.success(data);
        }).catch(new Error("LeggiCausaliTrasporto"));
        //options.success(cauTrasp);

    } catch (err) {
        let msgErr = "Errore in lettura causali trasporto: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }

}



async function LeggiTipoDocumento(options) {
    try {
        
        let SigleAE = await RicercaSigleAE(cIdLavCod, false).then(function (data) {
            options.success(data);
        }).catch(new Error("LeggiTipoDocumento"));
        //options.success(cauTrasp);

    } catch (err) {
        let msgErr = "Errore in lettura sigle AE: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }

}


function LeggiConferentiGerarchiaSync(options) {
    let tipoRapporto = 0;
    try {
        tipoRapporto = GetTipoRapporto(cIdLavCod);

        let dataValidita = KendoDate("inDataEmissione").value();
        let accettazioneConGerarchia = 0;
        let pivaPadreGerarchia = "";

        switch (this.data.ruoloAccettazione) {
            case "Conferente":
                accettazioneConGerarchia = 1;
                pivaPadreGerarchia = pivaPadreGerarchiaConf;
                break;
            case "Coop1":
                accettazioneConGerarchia = 2;
                pivaPadreGerarchia = pivaPadreGerarchiaCoop1;
                break;
            case "Coop2":
                accettazioneConGerarchia = 3;
                pivaPadreGerarchia = pivaPadreGerarchiaCoop2;
                break;
            case "Produttore":
                accettazioneConGerarchia = 4;
                pivaPadreGerarchia = pivaPadreGerarchiaProd;
                break;
        }

        if (elencoConferentiAccettazione[accettazioneConGerarchia] !== undefined && elencoConferentiAccettazione[accettazioneConGerarchia] !== null) {
            if (elencoConferentiAccettazione[accettazioneConGerarchia].ruoloAccettazione === this.data.ruoloAccettazione &&
                elencoConferentiAccettazione[accettazioneConGerarchia].pivaPadreGerarchia !== pivaPadreGerarchia) {
                //Azzero elenco
                elencoConferentiAccettazione[accettazioneConGerarchia].elencoContatti = null;
            }
        }

        let resp = RicercaConferentiGerarchiaDocumentoSync(true, $(cIdPiva).val(), dataValidita, false, true, true, true,
            accettazioneConGerarchia, this.data.ruoloAccettazione, pivaPadreGerarchia, "", 0, "", false, this.data.checkRaccolte, dataValidita);

        options.success(resp);

        //devo forzare la de-selezione
        KendoDDL(this.data.idControllo).select(-1);
        $("#" + this.data.idControllo).change();

    } catch (err) {
        let msgErr = "Errore in lettura contatti[" + tipoRapporto + "][ruoloAccettazione=" + this.data.ruoloAccettazione + "]: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

async function LeggiCessionariCedentiAsync(options) {
    let tipoRapporto = 0;
    try {
        tipoRapporto = GetTipoRapporto(cIdLavCod);

        let dataValidita = KendoDate("inDataEmissione").value();
        let accettazioneConGerarchia = 0;
        let pivaPadreGerarchia = "";

        if (tipoRapporto === enum_TipoRapporto.Conferenti && this.data.ruoloAccettazione !== "") {
            //devo per forza ri-azzerarlo, perché vanno sempre riletti
            elencoContatti[tipoRapporto] = null;
        }

        switch (this.data.ruoloAccettazione) {
            case "Conferente":
                accettazioneConGerarchia = 1;
                pivaPadreGerarchia = pivaPadreGerarchiaConf;
                break;
            case "Coop1":
                accettazioneConGerarchia = 2;
                pivaPadreGerarchia = pivaPadreGerarchiaCoop1;
                break;
            case "Coop2":
                accettazioneConGerarchia = 3;
                pivaPadreGerarchia = pivaPadreGerarchiaCoop2;
                break;
            case "Produttore":
                accettazioneConGerarchia = 4;
                pivaPadreGerarchia = pivaPadreGerarchiaProd;
                break;
        }

        // Se lo stato dell'azienda corrente non é Italia non richiedo i dati relativi all'indirizzo in modo da velocizzare le letture
        let includiIndirizzo = true;
        if ($(cStato_Cod).val() !== "IT")
            includiIndirizzo = false;
        let filtraDataSuDb = false;

        if (raccolteXConferimenti_AbilitazioneGenerale() && cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value)
            filtraDataSuDb = true;

        // Se this.data.cod_risum_contatto é valorizzato significa che sono in lettura o in modifica. 
        // In lettura evito di rileggere tutti i contatti.
        // In modifica per quanto riguarda il contatto1 visto che non é modificabile evito sempre di rileggere; per il contatto2 invece evito solo se sono in modalità serverFilternig.
        let cod_risum_contatto = 0;
        elencoContatti[tipoRapporto] = null;

        if (this.data.cod_risum_contatto !== undefined && this.data.cod_risum_contatto !== null && this.data.cod_risum_contatto !== 0) {
            cod_risum_contatto = this.data.cod_risum_contatto;
        }

        let resp = await RicercaContattiDocumentoAsync(true, $(cIdPiva).val(), tipoRapporto, dataValidita, filtraDataSuDb, true, true,
                                                        includiIndirizzo, accettazioneConGerarchia, pivaPadreGerarchia, "", cod_risum_contatto, "", false,
                                                        this.data.checkRaccolte, dataValidita);

        options.success(resp);

        //devo forzare la de-selezione
        KendoDDL(this.data.idControllo).select(-1);
        $("#" + this.data.idControllo).change();

    } catch (err) {
        let msgErr = "Errore in lettura contatti[" + tipoRapporto + "][ruoloAccettazione=" + this.data.ruoloAccettazione + "]: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

function LeggiCessionariCedentiSync(options) {

    // Utilizzato per riapplicare i filtri al cambio data accettazione
    if (options.data.filter !== undefined && options.data.filter.filters.length === 0 ) {

        if (this.data.idControllo === "inCedenteCessionario1" && ultimoFiltroServerFilteringCessionario.inCedenteCessionario1 !== null)
            options.data.filter = ultimoFiltroServerFilteringCessionario.inCedenteCessionario1;
        if (this.data.idControllo === "inCedenteCessionario2" && ultimoFiltroServerFilteringCessionario.inCedenteCessionario2 !== null)
            options.data.filter = ultimoFiltroServerFilteringCessionario.inCedenteCessionario2;
    }

    if ((options.data.filter !== undefined &&
        (options.data.filter.filters.length !== 0 && options.data.filter.filters[0].value.length >= LunghezzaMinimaFiltroContatto)) ||
        
        (//this.data.idControllo === "inCedenteCessionario2" &&
            this.data.cod_risum_contatto !== undefined && this.data.cod_risum_contatto !== null && this.data.cod_risum_contatto !== 0)
        ) {

        if (this.data.idControllo === "inCedenteCessionario1")
            ultimoFiltroServerFilteringCessionario.inCedenteCessionario1 = options.data.filter;
        if (this.data.idControllo === "inCedenteCessionario2")
            ultimoFiltroServerFilteringCessionario.inCedenteCessionario2 = options.data.filter;

        let tipoRapporto = 0;
        try {
            tipoRapporto = GetTipoRapporto(cIdLavCod);

            let dataValidita = KendoDate("inDataEmissione").value();
            let accettazioneConGerarchia = 0;
            let pivaPadreGerarchia = "";

            if (tipoRapporto === enum_TipoRapporto.Conferenti && this.data.ruoloAccettazione !== "") {
                //devo per forza ri-azzerarlo, perché vanno sempre riletti
                elencoContatti[tipoRapporto] = null;
            }

            switch (this.data.ruoloAccettazione) {
                case "Conferente":
                    accettazioneConGerarchia = 1;
                    pivaPadreGerarchia = pivaPadreGerarchiaConf;
                    break;
                case "Coop1":
                    accettazioneConGerarchia = 2;
                    pivaPadreGerarchia = pivaPadreGerarchiaCoop1;
                    break;
                case "Coop2":
                    accettazioneConGerarchia = 3;
                    pivaPadreGerarchia = pivaPadreGerarchiaCoop2;
                    break;
                case "Produttore":
                    accettazioneConGerarchia = 4;
                    pivaPadreGerarchia = pivaPadreGerarchiaProd;
                    break;
            }

            // Se lo stato dell'azienda corrente non é Italia non richiedo i dati relativi all'indirizzo in modo da velocizzare le letture
            let includiIndirizzo = true;
            if ($(cStato_Cod).val() !== "IT")
                includiIndirizzo = false;

            if (raccolteXConferimenti_AbilitazioneGenerale() && cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value)
                filtraDataSuDb = true;

            let testoRicerca = options.data.filter === undefined ? null : JSON.stringify(options.data.filter.filters);

            // Se this.data.cod_risum_contatto é valorizzato significa che sono in modifica e sto gestendo il contatto2
            // In questo caso evito di rileggere.
            // N.B. se arrivo qui sono in modalità serverFilternig
            let cod_risum_contatto = 0;
            if (this.data.cod_risum_contatto !== undefined && this.data.cod_risum_contatto !== null && this.data.cod_risum_contatto !== 0) {
                elencoContatti[tipoRapporto] = null;
                cod_risum_contatto = this.data.cod_risum_contatto;
                this.data.cod_risum_contatto = 0;
            }

            let resp = RicercaContattiDocumentoSync(true, $(cIdPiva).val(), tipoRapporto, true, true,
                includiIndirizzo, accettazioneConGerarchia, pivaPadreGerarchia, testoRicerca, cod_risum_contatto, "", false,
                this.data.checkRaccolte, dataValidita, dataValidita);

            options.success(resp);

            elencoContatti[tipoRapporto] = null;

            //devo forzare la de-selezione
            KendoDDL(this.data.idControllo).select(-1);
            $("#" + this.data.idControllo).change();

        } catch (err) {
            let msgErr = "Errore in lettura contatti[" + tipoRapporto + "][ruoloAccettazione=" + this.data.ruoloAccettazione + "]: " + err;
            console.error(err);
            alert(msgErr);
            options.error(msgErr);
        }
    } else {
        // Rientro quando l'utente riapre la ddl dopo aver fatto un filtro ed una selezione,
        // invece di ricercare tutti i contatti mostro quelli che avevo caricato con l'ultimo filtro.
        // Perchè kendo, dopo aver fatto un filtro ed una selezione cancella il filtro e riscatena la lettura di tutti gli elementi,
        // in quanto dà per scontato che il filtro non serva più e l'utente voglia selezionare un nuovo elemento fra tutti i disponibili.
        // Nel nostro caso dato che i cobtatti possono essere moltissimi ripropongo gli ultimi caricati ed eventualmente l'utente
        // farà una nuova ricerca. Tenere presente che é stato impostato che l'utente debba inserire almeno 5 caratteri per far partire la ricerca.
        options.success(KendoDDL(this.data.idControllo).dataSource.data());
    }
}

async function LeggiAgentiCapoAreaTerzisti(options) {
    try {
        let dataValidita = KendoDate("inDataEmissione").value();
        let resp = await RicercaContattiDocumentoAsync(true, $(cIdPiva).val(), this.data.tipoRapporto, dataValidita, false, true, true, true, 0, "", "", 0, "", false);
        options.success(resp);

        //devo forzare la de-selezione
        KendoDDL(this.data.idControllo).select(-1);   
        $("#" + this.data.idControllo).change();

    } catch (err) {
        let msgErr = "Errore in lettura contatti[" + this.data.tipoRapporto + "]: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

function LeggiTipoIndirizzoCC(options) {
    let idTipoInd = "";
    try {
        let ddlCessionario = KendoDDL(this.data.idCedCes);

        idTipoInd = this.data.idControllo;
        let ddlTipoIndirizzo = KendoDDL(idTipoInd);

        let codContatto = "";
        let elencoTipiIndirizzo = Array();
        let tipoIndDefault = 0;

        if (ddlCessionario !== undefined && ddlCessionario !== null && ddlCessionario.dataItem() !== undefined) {
            codContatto = ddlCessionario.dataItem().Cod_Contatto;
        }

        //vado a leggere gli indirizzi solo se ho scelto un contatto
        if (codContatto !== "") {
            elencoTipiIndirizzo = RicercaTipoIndirizzo(false, $(cIdPiva).val(), codContatto, false);
        }
        options.success(elencoTipiIndirizzo);
        
        if (codContatto !== "") {

            if (ddlCessionario !== undefined && ddlCessionario !== null && ddlCessionario.dataItem() !== undefined) {
                tipoIndDefault = ddlCessionario.dataItem().Tipo_Indirizzo_Default;
            }

            if (tipoIndDefault !== 0) {
                ddlTipoIndirizzo.select(function(dataItem) {
                    return dataItem.Tipo_Indirizzo === tipoIndDefault;
                });

                if (ddlTipoIndirizzo.dataItem() === undefined) {
                    //era impostato un indirizzo di default che non esiste (ad esempio 101 quando sono impresa gias e la sede legale non c'è)
                    ddlTipoIndirizzo.select(0); //imposto la prima voce in elenco
                }
            } else {
                //non c'era indirizzo default, imposto cmq il primo
                ddlTipoIndirizzo.select(0); //imposto la prima voce in elenco
            }

        }
        $('#' + idTipoInd).change();
    } catch (err) {
        let msgErr = "Errore in lettura indirizzo " + idTipoInd + ": " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

async function LeggiGestioneVettore(options) {
    try {
        let gestVett = await RicercaGestioneVettore(false);
        options.success(gestVett);

    } catch (err) {
        let msgErr = "Errore in lettura gestione vettore: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

function LeggiTipoIndirizzoVettore(options) {
    try {
        let elencoTipiIndirizzo = Array();

        let codContatto = "";
        if (Cmb_Vettore.dataItem() !== undefined && Cmb_Vettore.dataItem() !== null) {
            //vado a leggere gli indirizzi solo se ho un vettore movimentato
            codContatto = Cmb_Vettore.dataItem().Cod_Contatto;
            if (codContatto !== "") {
                elencoTipiIndirizzo = RicercaTipoIndirizzo(false, $(cIdPiva).val(), codContatto, false);
            }
        }
        options.success(elencoTipiIndirizzo);

        if (codContatto !== "" && Cmb_Vettore.dataItem() !== undefined && Cmb_Vettore.dataItem() !== null) {
            let tipoIndDefault = Cmb_Vettore.dataItem().Tipo_Indirizzo_Default;
            if (tipoIndDefault !== 0) {
                Cmb_TipoIndVettore.select(function(dataItem) {
                    return dataItem.Tipo_Indirizzo === tipoIndDefault;
                });
            } else {

                //Guardo se c'è il 101 (sede legale) lo imposto
                if (Cmb_TipoIndVettore.dataItems().some(function (dataItem) { return dataItem.Tipo_Indirizzo === 101; })) {
                    Cmb_TipoIndVettore.select(function (dataItem) {
                        return dataItem.Tipo_Indirizzo === 101;
                    });
                }
            }
        }

        $("#inTipoIndirizzoVettore").change();
    } catch (err) {
        let msgErr = "Errore in lettura indirizzo vettore: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

async function LeggiAccModalitaTrasporto(options) {
    try {
        let modTrasp = await RicercaAccModalitaTrasporto(false, false);
        options.success(modTrasp);

    } catch (err) {
        let msgErr = "Errore in lettura modalita trasporto: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

async function LeggiAccUnitaTrasporto(options) {
    try {
        let unitaTrasp = await RicercaAccUnitaTrasporto(true, false);
        options.success(unitaTrasp);
    } catch (err) {
        let msgErr = "Errore in lettura unita trasporto: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

function LeggiMezzoTrasporto(options) {
    try {
        var elencoMezzoTrasporto = Array();

        let dataItemVettore = Cmb_Vettore.dataItem();
        if (dataItemVettore !== undefined && dataItemVettore !== null) {
            //vado a leggere i mezzi solo se ho un vettore movimentato
            if (dataItemVettore.Cod_Contatto !== "") {
                elencoMezzoTrasporto = RicercaMezzoTrasporto(false, dataItemVettore.Piva_Proprietaria, dataItemVettore.Sa_Cod, dataItemVettore.Cod_Contatto, " Targa ASC ", false);
            }
        }
        options.success(elencoMezzoTrasporto);

        //se ho solo una voce scelgo direttamente quella
        if (elencoMezzoTrasporto.length === 1) {
            KendoDDL("inMezzoTrasporto").select(0);
        } else if (elencoMezzoTrasporto.length > 1) {
            //forzo la deselezione, per evitare che mi imposti cmq il primo
            KendoDDL("inMezzoTrasporto").select(-1);
        }

        $("#inMezzoTrasporto").change();

    } catch (err) {
        let msgErr = "Errore in lettura mezzo trasporto: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

function AggiungiMacchinaDaTarga(piva, saCod, codContatto, targa, macDes) {

    let param = kendo.stringify({
        piva: piva,
        saCod: saCod,
        codContatto: codContatto,
        targa: targa,
        macDes: macDes
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/AggiungiMacchinaDaTarga",
        param,
        false,
        function (risposta) {

            let macCodNew = risposta.RispostaStringa;

            //TODO: ricarica ddlMezzo + setta sul nuovo valore

            console.debug(risposta);
            
            Cmb_MezzoTrasporto.dataSource.read();   //rileggo ddl

            Set_KendoDDLValue("inMezzoTrasporto", parseInt(macCodNew)); //setto il valore

            $("#inMezzoTrasporto").change();    //forzo il change

        }, function (risposta) {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
        });

}

function UrlGestioneContatto(cod_Contatto, tipologia_Contatto, solo_Lettura, piva) {

    let url = "";
    let param = kendo.stringify(
        {
            piva: piva,
            lavCod: cIdLavCod,
            cod_Contatto: cod_Contatto,
            tipologia_Contatto: tipologia_Contatto,
            solo_Lettura: solo_Lettura
        });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/UrlGestioneContatto",
        param,
        false,
        function (risposta) {
            url = risposta.RispostaStringa;
        }, function (risposta) {
            kendo.alert(risposta.Errore);
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
        });

    return url;
}

function LeggiOpzioniContab() {
    let param = kendo.stringify({ piva: $(cIdPiva).val(), objP_server: objP_server, objP_utenti: objP_utenti });

    ajaxAgronicaSync(url + pathCoreWS + "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/LeggiOpzioni_DocContabili",
        param,
        false,
        function (risposta) {
            $(cIdOpzioniContab).val(risposta.RispostaStringa);
        }, function (risposta) {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
        });

}

async function LeggiOperatore(options) {
    try {
        //TODO: al momento, come sul LAN, legge tutti gli utenti, capire se si può lasciare così o in qualche modo 
        // è necessario ristringere il campo (in ogni caso se non è attiva l'opzione di modifica dell'operatore, 
        // la dropdown funge da semplice visualizzatore dell'autore ultima modifica)
        let operatori = [];

        let modificaOperatore = GetPropertyFromJson($(cIdOpzioniContab).val(), "SUPERUSER_OPERATORE_ACCETTAZIONE");

        if (modificaOperatore === false) {
            // Se non è possibile modificare l'utente Gias che fa l'operazione, mostro l'ultimo utente di modifica o l'utente corrente
            let idAgenda = parseInt($(cIdAgenda).val());

            if (idAgenda !== undefined && idAgenda !== null && !isNaN(idAgenda) && idAgenda !== 0) {

                operatori.push({ Codice_Fiscale: operatoreModificaCodFisc, Utente: operatoreModificaNominativo });
            }
            else {
                operatori.push({ Codice_Fiscale: operatoreCodFisc, Utente: operatoreNominativo });
            }
        }
        else {
            operatori = await GetOperatore(true, "", false, false);
        }

        options.success(operatori);
    } catch (err) {
        let msgErr = "Errore in lettura Operatore: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

function Carica_Stati_PanelBar() {
    var param = kendo.stringify({
        Lav_Cod: cIdLavCod
    });

    if (statiPanelsBar === null || statiPanelsBar === undefined) {
        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/CaricaStatiPanelBar",
            param,
            false,
            function (risposta) {
                var risp = risposta.RispostaStringa;
                if(risp != "") 
                    statiPanelsBar = JSON.parse(risp);

            }, function (risposta) {
                MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
            },
            null,
            false);
    }


    if (statiPanelsBar != null && statiPanelsBar != undefined && statiPanelsBar != "") {

        for (var i = 0; i < statiPanelsBar.length; i++) {
            var obj = statiPanelsBar[i];
            var panelBar = $("#" + obj.idPannelloPadre).data("kendoPanelBar");

            var nomeCompleto = obj.idPannelloPadre + "." + obj.idPannelloFiglio;

            if (!panelBarToccatiDopoLoad.includes(nomeCompleto)) {
                if (panelBar != null && panelBar != undefined) {
                    if (obj.Stato == 0) {
                        panelBar.collapse($("#" + obj.idPannelloFiglio), false);
                    }
                    else { panelBar.expand($("#" + obj.idPannelloFiglio), false); }
                }
            }
        }
    }

    // Gli ordini nel pannello di dettaglio li mostro solo in entrata riga di vendita
    $("#panelBar_OrdiniCliente").hide();
    $("#panelBar_DDTCliente").hide();
}

function Salva_Stato_PanelBar(idPannelloPadre, idPannelloFiglio, stato) {

    var impostazioni = [];

    var impostazione = new Object();
    impostazione.Pannelli = [];
    impostazione.LavCod = cIdLavCod;
    impostazione.Pagina = location.pathname;

    var pannello = new Object();
    pannello.idPannelloPadre = idPannelloPadre;
    pannello.idPannelloFiglio = idPannelloFiglio;
    pannello.Stato = stato;
    impostazione.Pannelli.push(pannello);

    impostazioni.push(impostazione);

    var url = indirizzoHttp_DocContabile_WS + "/SalvaStatiPanelBar";
    var param = kendo.stringify(
        {
            parametri: kendoEscapeOggetto(impostazioni)
        });

    ajaxAgronicaSync(url,
        param,
        false,
        function (risposta) {
        }, function (risposta) {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
        });

}

async function LeggiUnitaMisuraTrasporto(options) {
    try {
        let resp = await RicercaUnitaMisuraTrasporto();
        options.success(resp);

    } catch (err) {
        let msgErr = "Errore in lettura unità misura per trasporto: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

function RicercaUnitaMisuraTrasporto() {
    return new Promise(function (resolve, reject) {

        ajaxAgronica(indirizzoHttp_DocContabile_WS + "/Udm_Trasporto",
            {},
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                resolve(risp);
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Udm_Trasporto");
                reject(new Error(msgError));
            },
            null,
            false);

    });
}

function LeggiCentri(options) {
    try {
        let tipoValue = 2; // Tipo_Value = 2 ---> ex CaricaCombo_CentriAziendali2

        let resp = RicercaCentriAziendali($(cIdPiva).val(), false, tipoValue, false);
        options.success(resp);

    } catch (err) {
        let msgErr = "Errore in lettura centri: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

function StampaDoc() {

    let piva = $(cIdPiva).val();
    let Id_Agenda = parseInt($(cIdAgenda).val());
    let Lav_Cod = cIdLavCod;
    let tipoAccettazione = $(cTipoAccettazione).val();
    // 23-11-2020   ora in accettazione possono entrare anche trasf. animali e potrei avere nello stesso doc sia trasf. animali
    //              che vegetali; se è accettazione per ora imposto comunque il modulo anagrafe F&F perchè la bolla di entrata
    //              andrà uniformata per entrambe le categorie.
    //              Anche in caso di modifiche future se è conferimento pomodoro dovrà rimanere impostato a F&F
    //              Se non è accettazione invece lo lascio a zero
    //              Considerare che modulo_anagrafe_log nel frattempo è diventato un array
    let w_modulo_anagrafe_log = 0;
    if (lavCodAccettazione ||
        lavCodAccettazionePomodoro)
        w_modulo_anagrafe_log = Modulo_FreshFood; 

    var param = kendo.stringify({
        piva: piva,
        Id_Agenda: Id_Agenda,
        Lav_Cod: Lav_Cod,
        Modulo: w_modulo_anagrafe_log,
        Tipo_Accettazione: tipoAccettazione
    });
    var risp = "";

    ajaxAgronica(indirizzoHttp_DocContabile_WS + "/StampaDocumento",
        param,
        function (risposta) {
            risp = risposta.RispostaStringa;
            var win = window.open(risp);
            win.focus();
        }, function (rispostaErrore) {
            MessaggioErrore_Bootstrap(rispostaErrore.Errore, "DIV_Messaggi");
        });
}

function GetNumeratoriPiuDefaults(options) {

    let piva = $(cIdPiva).val();
    let saCod = parseInt(Qs_SaCod);
    let lavCod = cIdLavCod;
    let dataDocumento = KendoDate("inDataEmissione").value();
    let fatturaAccompagnatoria = false;
    let flagVuoto = false;


    let ret = RicercaNumeratoriPiuDefaults(piva, saCod, lavCod, dataDocumento, fatturaAccompagnatoria);

    if (ret !== undefined &&
        ret !== null &&
        ret.Numeratori !== undefined &&
        ret.Numeratori !== null &&
        ret.Numeratori.length > 0) {

        elencoNumeratori = ret.Numeratori;

        //TODO: sono in modifica ==> potrebbe essere stata modificata la tabella di configurazione, quindi nell'elenco non ho più il prefisso/suffisso che era stato usato

    } else {

        elencoNumeratori = [];
        //flagVuoto = true;

        //devo aggiungere la voce vuota solo se sono in creazione e non ho ottenuto nessuna configurazione,
        // se sono in modifica e non ho configurazione, lascio la DDL vuota
        if (cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value &&
            gestioneContabilita !== enum_Livello_GestContabilita_NonGestita) {
            flagVuoto = true;
        }

        //TODO: Non ho numeratori ==> potrebbe essere che è stata modificata la tabella ed essendo in modifica del doc devo cmq mostrare quelli che c'erano salvati?!?

        //TODO: se sono in creazione ==> dovrebbe diventare prefisso e suffisso = ""
    }

    if (flagVuoto === true) {
        let objVuoto = {
            Numeratore_Tipo: 0,
            Doc_Numero_Sin: "",
            Doc_Numero_Des: "",
            PrefissoSuffisso_Des: "",
            Lunghezza_Centro: 0,
            CarattereFormattazione: "",
            Sigla: "",
            NumeratoreTipo_Des: "",
            Vincolante: false,
            IsDefault: false
        };
        elencoNumeratori.unshift(objVuoto);
    }

    options.success(elencoNumeratori);
}

function ForzaEvasioneRigheOrdine(listDettagli, forzaEvasione) {

    let param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: $(cIdPiva).val(),
        idAgenda: $(cIdAgenda).val(),
        listDettagli: listDettagli,
        forzaEvasione: forzaEvasione
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/ForzaEvasioneRigheOrdine",
        param,
        false,
        function (risposta) {

            //ricarico
            DocContabileRicercaMovimenti(indirizzoHttp_DocContabile_WS, "tab_elenco_movimenti", $(cIdPiva).val(), $(cIdAgenda).val(), cIdLavCod);

        },
        function (risposta) {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
        });

}

function imputazioneImpianti_GetImpostazione(_piva, _saCod) {
    var impostazione = 1;

    var parametri = kendo.stringify({ piva: _piva, saCod: _saCod });
    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/LeggiImpostazioneImputazioneImpianti",
        parametri,
        false,
        function (rispServer) {
            impostazione = rispServer.RispostaStringa;
        },
        null);

    return impostazione;
}

function raccolteXConferimenti_GetImpostazione(_piva, _saCod, gestioneWaitFrame) {
    var impostazione = 1;

    var parametri = kendo.stringify({ piva: _piva, saCod: _saCod });
    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/LeggiImpostazioneRaccolteConferimenti",
        parametri,
        false,
        function (rispServer) {
            impostazione = rispServer.RispostaStringa;
        },
        null, null, gestioneWaitFrame);

    return impostazione;
}



function ModificaDistanzaDocumento(piva, idAgenda, udm_cod, distanza, valore) {

    let risultatoModifica;

    let param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: piva,
        id_agenda: idAgenda,
        udm_cod: udm_cod,
        distanza: distanza,
        valore: valore
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/ModificaDistanzaDocumento",
        param,
        false,
        function (risposta) {
            let risp = risposta.RispostaStringa;
            console.debug(risp);
            //TODO: devo ricaricare il documento

            if (risp === "True") {

                let msgOk = "Salvataggio ETD effettuato correttamente";

                console.log("Ho modificato la distanza!");
                //msgOk = AggiornaValoriPostSalvaAgenda(testataDoc, Qs_CaricoScarico, risp);


                MessaggioTuttoOK_Bootstrap(msgOk, "DIV_Messaggi");

                risultatoModifica = true;
                RiBloccoDistanzaDoc(true);
                $("#btnSalvaDistanzaDoc").attr("style", "display:none");
            }

        },
        function (risposta) {
            if (risposta !== undefined && risposta !== null && risposta.RispostaOK === false) {
                risultatoModifica = JSON.parse(risposta.RispostaStringa);
                let msg = risultatoModifica.MsgError;
                if (risposta.Errore !== "") {
                    msg = msg + "<br/>" + TraduzioneMultiResx(resxObj, "ErroreDuePunti_", "Errore: ") + risposta.Errore;
                }
                MessaggioErrore_Bootstrap(msg, "DIV_Messaggi");
            } else {
                MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreSalvataggio", "Errore salvataggio"), "DIV_Messaggi");
            }
        });

    return risultatoModifica;
}
