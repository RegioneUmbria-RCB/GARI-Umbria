
function CaricaGiacenze(mostraGrigliaGiacenze) {

    // N.B. La ricerca delle giacenze viene fatta sempre scartando la quantità di prodotto della riga che si sta
    //      modificando tramite non lettura di questa

    // Nascondo la griglia delle giacenze e pulisco i dati perché andranno comunque ricaricati
    $("#tab_grid_scelta_da_giacenza_formProdottoUC").hide();
    $("#titolo_grid_scelta_da_giacenza").hide();
    $("#tab_grid_scelta_da_giacenza_formProdottoUC").html("");

    // mostraGrigliaGiacenze viene passato a false solo quando non si vuole comunque mostrare la griglia giacenze
    //   all'1/4/2020 arriva sempre null
    if (mostraGrigliaGiacenze === null) {
        mostraGrigliaGiacenze = false;

        if ((is_Trasf_Veg_Anim_FormProdottoUC() || is_ElemCod_CalCod()) && is_LavCod_MostraGiacenze()) {
            mostraGrigliaGiacenze = true;
        }
    }

    // Potrei avere il prodotto caricato in più unità di misura quindi se devo mostrare le giacenze le cerco senza passare U.M.
    let w_udm_cod = 0;
    let w_udm_desc = "";
    if (!mostraGrigliaGiacenze) {
        if (Get_KendoDDLValue("ddlUM") !== undefined && Get_KendoDDLValue("ddlUM") !== null && Get_KendoDDLValue("ddlUM") !== "") {
            w_udm_cod = parseInt(Get_KendoDDLValue("ddlUM"));
            w_udm_desc = KendoDDL("ddlUM").text();

            let udmMultipliKg = [enum_Udm.quintali.value, enum_Udm.tonnellate.value];

            if (udmMultipliKg.includes(w_udm_cod)) {
                // Ai fini della ricerca giacenza, tratto i multipli dei kg come questi ultimi
                w_udm_cod = enum_Udm.chilogrammi.value;
            }
        }
    }

    if (Get_KendoDDLValue("ddlProdottoDes") !== "" &&
        (w_udm_cod !== 0 || mostraGrigliaGiacenze)) {

        //   TODO quando a regime togliere i campi passati da sommare / sottrarre

        let wKeyDet = $('input[name$="hf_key_mov_dett"]').val();
        if (wKeyDet === undefined || wKeyDet === null)
            wKeyDet = "";

        let w_operazione = enum_TipoOperazioneDB.Scrittura.value;
        let w_key_idMovDet = 0;
        if (wKeyDet !== "") {
            w_key_idMovDet = parseInt(wKeyDet.split("_")[4]);
            w_operazione = enum_TipoOperazioneDB.Modifica.value;
        }

        let w_cal_cod = 0;
        // Passo il cal_cod solo in entrata prodotto per vedere la giacenza specifica del prodotto che si sta caricando
        // compresi i parametri qualitativi
        // In vendita, anche se sono in modifica, considerato che passo dalla griglia delle giacenze, non considero la 
        // giacenza per parametro qualitativo
        if (w_operazione !== enum_TipoOperazioneDB.Scrittura.value) {
            if (is_Trasf_Veg_Anim_FormProdottoUC()) {
                if (!is_LavCod_MostraGiacenze()) {
                    w_cal_cod = parseInt($('input[name$="hf_Cal_Cod"]').val());
                }
            } else {
                if (Get_KendoDDLValue("ddlCalibro") !== "") {
                    w_cal_cod = Get_KendoDDLValue("ddlCalibro");
                }
            }
        }

        let w_qta = 0;
        let w_kg_lordi = 0;
        let w_kg_netti = 0;
        if (Get_KendoNumTBValue("idQuantita") !== null)
            w_qta = kendo.parseFloat(Get_KendoNumTBValue("idQuantita"));
        if (Get_KendoNumTBValue("idKgLordi") !== null)
            w_kg_lordi = kendo.parseFloat(Get_KendoNumTBValue("idKgLordi"));
        if (Get_KendoNumTBValue("idKgNetti") !== null)
            w_kg_netti = kendo.parseFloat(Get_KendoNumTBValue("idKgNetti"));

        // Se gestiti ricerca nr totale imballaggi, contenitori e confezioni dalla griglia
        let wNrImb_FF = 0;
        let wNrCont_FF = 0;
        let wNrConfez_FF = 0;
        //if (is_Trasf_Veg_Anim_FormProdottoUC()) {
        //    if (gestitoImballaggio_FF) {
        //        wNrImb_FF = kendo.parseFloat($("#tab_imballaggi_formProdottoUC").data().kendoGrid.dataSource.aggregates().NrImballaggi.sum);
        //    }
        //    if (gestitoContenitore_FF) {
        //        wNrCont_FF = kendo.parseFloat($("#tab_imballaggi_formProdottoUC").data().kendoGrid.dataSource.aggregates().NrContenitori.sum);
        //    }
        //    if (gestitoConfezione_FF) {
        //        wNrConfez_FF = kendo.parseFloat($("#tab_imballaggi_formProdottoUC").data().kendoGrid.dataSource.aggregates().NrConfezioni.sum);
        //    } 
        //}

        var Flag_QtaMaggioreZero = false;
        var Flag_QtaNoZero = false;
        if (Qs_CaricoScarico === CAU_SCARICO) {
            let w_gest_giacenza = getGestioneGiacenza();
            if (w_gest_giacenza !== enum_Gestione_Giacenze_TuttiProdotti) {
                if (w_gest_giacenza === enum_Gestione_Giacenze_SoloPresenti) {
                    Flag_QtaMaggioreZero = true;
                    Flag_QtaNoZero = true;
                }
            }
        }

        // TODO
        let w_lotto = "";

        let w_lotto_accettazione = null;

        let ddlLottoAccettazione = KendoDDL("ddlLottoAccettazione");

        if (Qs_CaricoScarico === CAU_SCARICO &&
            ddlLottoAccettazione.dataSource.data().length > 0 &&
            ddlLottoAccettazione.dataItem() !== undefined) {
            w_lotto_accettazione = ddlLottoAccettazione.value();
        }

        let w_Elem_Cod = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
        let w_modulo_anagrafe_log = moduloFromElemCod(w_Elem_Cod);

        var param = kendo.stringify({
            w_piva: $(cIdPiva).val(),
            w_Elem_Cod: w_Elem_Cod,
            w_Udm: w_udm_cod,
            w_Udm_Desc: w_udm_desc,
            w_ChkLottoImpianto: getKendoSwitch("chkAggregaLottoImpianto"),
            w_Lotto: w_lotto,
            w_Prodotto_Cod: parseInt(Get_KendoDDLValue("ddlProdottoDes")),
            w_CauMov: Qs_CaricoScarico,
            w_Lotto_Accettazione: w_lotto_accettazione,
            w_Cal_Cod: w_cal_cod,
            w_Ubic_Provenienza: Get_KendoDDLValue("ddlUbicProvenienza"),
            w_Ubic_Destinazione: Get_KendoDDLValue("ddlUbicDestinazione"),
            w_Chk_ParametroQualitativo: getKendoSwitch("chkParametroQualitativo"),
            Operazione: w_operazione,
            DataMovimento: get_data("inDataEmissione"),
            Qta_Mask: w_qta,
            KgLordi_Mask: w_kg_lordi,
            KgNetti_Mask: w_kg_netti,
            Imballaggi_Mask: wNrImb_FF,
            Contenitori_Mask: wNrCont_FF,
            Confezioni_Mask: wNrConfez_FF,
            modulo_anagrafe_log: w_modulo_anagrafe_log,
            Flag_QtaNoZero: Flag_QtaNoZero,
            Flag_QtaMaggioreZero: Flag_QtaMaggioreZero,
            w_key_idMovDet: w_key_idMovDet,
            FF_gest_materiale_vivaistico: FF_gest_materiale_vivaistico
        });

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Carica_Giacenze",
            param, false,
            function (risposta) {

                // Risposta OK
                let risp = JSON.parse(risposta.RispostaStringa);

                $('input[name$="txtGiacenzaProvenienza"]').val(risp.DatiTotali.StrGiacenza_Provenienza);
                $('input[name$="txtGiacenzaDestinazione"]').val(risp.DatiTotali.StrGiacenza_Destinazione);

                // Se sono in scarico prodotto valorizzo la DDL del lotto oppure se questa è già valorizzata
                // carico la griglia di scelta da giacenze
                if (Qs_CaricoScarico === CAU_SCARICO) {

                    if (w_lotto_accettazione === null) {

                        elencoLottiAccettazione_FormProdottoUC = [];

                        if (risp.righe_dettaglio !== undefined && risp.righe_dettaglio.length > 0) {
                            for (let y = 0; y < risp.righe_dettaglio.length; y++) {
                                let w_lotto = risp.righe_dettaglio[y].Lotto;
                                let elencoInseriti = elencoLottiAccettazione_FormProdottoUC.filter(function (x) {
                                        return (w_lotto === x.Lotto_Cod);
                                    });

                                if (elencoInseriti.length === 0)
                                    elencoLottiAccettazione_FormProdottoUC.push({ Lotto_Cod: w_lotto, Lotto: w_lotto });
                            }
                        }
                        
                        if (elencoLottiAccettazione_FormProdottoUC.length === 1 && !lavCodOrdine) {
                            ImpostaDdlLottoAccettazione(elencoLottiAccettazione_FormProdottoUC, elencoLottiAccettazione_FormProdottoUC[0].Lotto_Cod);
                        }
                        else {
                            ImpostaDdlLottoAccettazione(elencoLottiAccettazione_FormProdottoUC);
                        }
                    }

                    // Mi riprendo il lotto di accettazione
                    //if (Get_KendoDDLValue("ddlLottoAccettazione") !== undefined &&
                    //    Get_KendoDDLValue("ddlLottoAccettazione") !== "") {
                    //    w_lotto_accettazione = Get_KendoDDLValue("ddlLottoAccettazione");
                    //}

                    // Se è stato trovato un solo lotto propongo già la griglia giacenze
                    // 1/4/2020 la faccio vedere sempre anche se non è ancora stato scelto il lotto
                    //if (w_lotto_accettazione !== "") {

                        if ((is_Trasf_Veg_Anim_FormProdottoUC() || is_ElemCod_CalCod()) && is_LavCod_MostraGiacenze()) {

                            if (mostraGrigliaGiacenze &&
                                risp.righe_dettaglio !== undefined &&
                                risp.righe_dettaglio.length > 0) {
                                $('input[name$="hdKendo_SceltaDaGiacenza_FormProdottoUC"]').val(risposta.RispostaStringa);
                                $("#tab_grid_scelta_da_giacenza_formProdottoUC").show();
                                $("#titolo_grid_scelta_da_giacenza").show();
                                popola_SceltaDaGiacenza_FormProdottoUC("tab_grid_scelta_da_giacenza_formProdottoUC");
                                $("#btn_scelta_da_giacenza_formProdottoUC").hide();
                            }

                        }

                    //}

                }

  
                if (risp.DatiTotali.messaggioErrore !== "")
                    MessaggioErrore_Bootstrap(risp.DatiTotali.messaggioErrore, "DIV_Messaggi");

                if (risp.DatiTotali.permettiSalvataggio.toLowerCase() !== "true") {
                    DisabilitaSalvataggio();
                }

            }, null);
    }

}

function FineScorta_e_altreInfo(Prodotto_Cod) {

    var infoTxt = "";
    var param = kendo.stringify({ Prodotto_Cod: Prodotto_Cod });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/FineScorta_e_altreInfo",
        param, false,
        function (risposta) {
            infoTxt = risposta.RispostaStringa;
        }, null);

    return infoTxt;
}

function VerificaClasseToxPatentinoERicercaUdm(Prodotto_Cod, Udm_Dose) {

    let risp;
    let param = kendo.stringify({
        piva: $(cIdPiva).val(),
        Data_Validita: get_data("inDataEmissione"),
        Prodotto_Cod: Prodotto_Cod,
        Udm_Dose: Udm_Dose,
        linkWsFitofarmaci: hf_LinkWsFitofarmaci
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/VerificaClasseToxPatentinoERicercaUdm",
        param, false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
        }, null);

    return risp;
}
 
function CostruisciLinkInfoProdotto(elem_cod) {
    var w_link_prodotto = "";

    if (elem_cod !== 0) {
         
        // TODO COL CHIAMANTE in orig
        let orig = "DocContabile.aspx";

        let param = kendo.stringify({
            p: $(cIdPiva).val(),
            e: elem_cod,
            l: cIdLavCod,
            c: kendoEscapeOggetto(Qs_Key),
            d: Qs_DataSelezionata,
            a: parseInt($(cIdAgenda).val()),
            orig: orig,
            mode: Qs_Mode
        });
        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/CostruisciLinkInfoProdotto",
            param, false,
            function (risposta) {
                w_link_prodotto += risposta.RispostaStringa;

            }, null);

        $("#lblLinkProdotto").val(w_link_prodotto);
    }
}

function CostruisciLinkNuovoProdotto() {
    WaitFrame.show();
    var w_edit_prodotto = "";
    let elem_cod = Get_KendoDDLValue("ddlCategorieMagazzino");

    ajaxAgronicaSync(
        indirizzoHttp_DocContabile_WS + "/CostruisciLinkNuovoProdotto",
        "{elem_cod: " + elem_cod + " }",
        true,
        function (risposta) {
            if (risposta.RispostaOK) {
                w_edit_prodotto = risposta.RispostaStringa;
            } else {
                WaitFrame.hide();
                alert(risposta.Errore);
            }
        }
    );

    $('input[name$="hf_LinkEditProdotto"]').val(w_edit_prodotto);
}

function CostruisciInfoFertilizzante(fer_cod) {
    var w_InfoFertilizzante = "";

    if (fer_cod !== 0) {
        let param = kendo.stringify({
            fer_cod: fer_cod
        });
        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/CostruisciInfoFertilizzante",
            param, false,
            function (risposta) {
                w_InfoFertilizzante = risposta.RispostaStringa;

            }, null);

    }

    return w_InfoFertilizzante;

}


function SalvaRigaDoc(KgNettiDaForzareSuImpianti) {

    let salvataggioOK = false;

    //riprendo i dati che avevo letto, così ho già molte cose valorizzate come idAgenda, id_Mov_Det e dataOraUltimaLettura
    let rigaDoc = getDatiContabRiga();

    //TODO: verificare se servono tutti i campi
    let objCampi = EstraiCampiInput();

    if (KgNettiDaForzareSuImpianti !== undefined && KgNettiDaForzareSuImpianti !== null && objCampi.RigheImpianti !== undefined && objCampi.RigheImpianti !== null && objCampi.RigheImpianti.length > 0) {
        // Significa che è stato ricalcolato il peso lordo e di conseguenza il netto e che devo aggiornare l'unica riga impianti esistente
        objCampi.RigheImpianti[0].Qta = KgNettiDaForzareSuImpianti;
    }

    if (objCampi !== null) {

        objCampi.Fornitore = parseInt(Get_KendoDDLValue(ddlContatto1Nome));

        AggiungiValoriParametriQualitativi(objCampi);
        AggiungiValoriGHG_Registrazioni(objCampi);


        //Riempio con campi della riga originale
        if (rigaDoc !== undefined && rigaDoc !== null) {
            objCampi.DataOraUltimaLettura = rigaDoc.DataOraUltimaLettura;
        }


        let usernameOperatore = operatoreCodFisc;
        if (Cmb_Operatore.element[0].disabled === false)
            usernameOperatore = Get_KendoDDLValue("inOperatore");

        let salvataggioNecessarioTestata = false;
        let testataDoc = getDatiContabTestata();
        let idMovT = 0;
        let idMovCS = 0;
        let contabTestata = "";

        if (testataDoc === undefined || testataDoc === null || testataDoc.IdMov === 0) {

            //Devo salvare anche la testata
            salvataggioNecessarioTestata = true;

            testataDoc = GeneraOggettoTestata();
            contabTestata = kendo.stringify(testataDoc);
            //throw new Error("Manca l\'Id_Mov di Testata");
        } else {
            objCampi.Piva = testataDoc.Piva;
            objCampi.IdAgenda = testataDoc.IdAgenda;
            idMovT = testataDoc.IdMov;

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

        //TODO: DEBUG GIULIA --> SISTEMA!
        if (objCampi.IdMov > 0)
            idMovCS = objCampi.IdMov;

        let param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            contabDettaglio: kendo.stringify(objCampi),
            lavCod: cIdLavCod,
            cauMov: Qs_CaricoScarico,
            idMovT: idMovT,
            idMovCS: idMovCS,
            dataOp: kendo.parseDate($("#inDataEmissione").val()),
            usernameOperatore: usernameOperatore,
            messaggioDettagliato: true,
            contabTestata: contabTestata
        });

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/SalvaSingolaRigaDocumento",
            param,
            false,
            function (risposta) {
                let risp = JSON.parse(risposta.RispostaStringa);
                console.debug(risp);
                console.log("riporta i nuovi valori --> non serve se poi chiudo direttamente la riga e rileggo la griglia (se è la prima riga ed ho anche salvato la testata, le chiavi di testata devo essere scritte)");

                if (risp !== undefined && risp !== null && risp.Risultato === true) {

                    //let msgOk = "Salvataggio riga effettuato correttamente";
                    let msgOk = TraduzioneMultiResx(resxFormProdottoUC, "SalvataggioEffettuatoCorrettamente", "Salvataggio effettuato correttamente");

                    if (salvataggioNecessarioTestata === true) {

                        console.log("Ho salvato anche la testata!");
                        msgOk = AggiornaValoriPostSalvaAgenda(testataDoc, Qs_CaricoScarico, risp);
                        //msgOk = "Salvataggio documento + riga effettuato correttamente";

                    } else {
                        console.log("Ho salvato solo riga");
                    }

                    //Aggiorno conteggi riepilogo pesi e altri legati alla riga
                    AggiornaValoriPostSalvaDettaglio(testataDoc, risp);

                    // Rendo visibile il TAB elenco righe
                    ImpostaVisibilitaElencoOSingolaRiga(false);

                    //ricarico
                    ResetCampiChiaveFormProdotto();

                    //Ricarico Griglia dettagli movimento
                    DocContabileRicercaMovimenti(indirizzoHttp_DocContabile_WS, "tab_elenco_movimenti", $(cIdPiva).val(), $(cIdAgenda).val(), cIdLavCod);

                    //Ricarico griglia per confezionamenti collegati e mostro TAB Beni confezionamento
                    if (isGestitoTabConfezionamento()) {
                        ConfiguraGrigliaCaricoEredita("tab_griglia_carico_eredita");
                        $("#a_tabBeniConfezionamento").show();

                        if (lavCodAccettazione) {
                            let msgRiepilogoPesi = VerificaCongruenzaPesi();

                            if (msgRiepilogoPesi !== "")
                                alert(msgRiepilogoPesi);
                        }
                    } else {
                        $("#a_tabBeniConfezionamento").hide();
                    }

                    
                    //Disabilitazione ChkAccompagnatoria
                    if (lavCodFattura) {
                        $("#chkAccompagnatoria").data("kendoSwitch").enable(false);
                    }


                    MessaggioTuttoOK_Bootstrap(msgOk, "DIV_Messaggi");
                }

                salvataggioOK = true;

            }, function (risposta) {

                if (risposta !== undefined && risposta !== null && risposta.RispostaOK === false) {
                    let risp = JSON.parse(risposta.RispostaStringa);
                    MessaggioErrore_Bootstrap(risp.MsgError + "<br/>" + TraduzioneMultiResx(resxFormProdottoUC, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
                } else {
                    MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxFormProdottoUC, "ErroreSalvataggio", "Errore salvataggio"), "DIV_Messaggi");
                }

                salvataggioOK = false;
            });

    } else {
        salvataggioOK = false;

    }

    return salvataggioOK;
}
 
// -----------------------------------------------------------------
// ------ Ricalcola Dettagli Economici
// -----------------------------------------------------------------
function AggiornaDettagliEconomici() {

    // Se gestiti ricerca nr totale imballaggi, contenitori e confezioni dalla griglia
    let wNrImb_FF = 0;
    let wNrCont_FF = 0;
    let wNrConfez_FF = 0;
    if (KendoGrid("tab_imballaggi_formProdottoUC") !== undefined) {
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

    let w_kg_netti = 0;
    if (Get_KendoNumTBValue("idQuantita") !== null)
        w_kg_netti = kendo.parseFloat(Get_KendoNumTBValue("idQuantita"));
    else {
        if (Get_KendoNumTBValue("idKgNetti") !== null)
            w_kg_netti = kendo.parseFloat(Get_KendoNumTBValue("idKgNetti"));
    }
    //ora il degrado è gestito direttamente dentro alla funzione server
    //let degradoPerc = Get_KendoNumTBValue("idDegradoPerc", true);
    //if (degradoPerc !== 0) {
    //    w_kg_netti = Math.round((w_kg_netti / 100 * (100 - kendo.parseFloat(degradoPerc))) * 100) / 100;
    //}

    let w_Elem_Cod = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
    let w_modulo_anagrafe_log = moduloFromElemCod(w_Elem_Cod);

    // Quando si entra per la prima volta non è ancora scattata la change di UdM, di conseguenza PrezzoRiferitoA non è impostato
    // Lo vado quindi a impostare secondo l'U.M. corrente, subito dopo scatterà comunque la change di UdM che se questa è cambiata
    // farà sì che si entri qui.
    if (Get_KendoDDLValue("ddlPrezzoRiferitoA") === "" &&
        Get_KendoDDLValue("ddlUM") !== undefined && Get_KendoDDLValue("ddlUM") !== null && Get_KendoDDLValue("ddlUM") !== "") {
        w_um = parseInt(Get_KendoDDLValue("ddlUM"));

        if (w_um !== 0) {

            //dataSource prezzo livello
            if (w_um === enum_Udm.chilogrammi.value && is_Trasf_Veg_Anim_FormProdottoUC()) {
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
        }
    }

    //Creo un oggetto estrapolando i dati da interfaccia
    var objCampi = {
        Quantita: Get_KendoNumTBValue("idQuantita"),
        Udm: parseInt(Get_KendoDDLValue("ddlUM")),
        NumConfezioni: wNrConfez_FF,
        NumContenitori: wNrCont_FF,
        NumImballi: wNrImb_FF,
        KgNetti: w_kg_netti,
        Degrado: kendo.parseFloat(Get_KendoNumTBValue("idDegradoPerc", true)),

        Prezzo: kendo.parseFloat(Get_KendoNumTBValue("idPrezzo")),
        PrezzoNetto: kendo.parseFloat(Get_KendoNumTBValue("idPrezzoNetto")),     //TODO: mettere il change?!?
        ScontoModalita: Get_KendoDDLValue("ddlScontoModalita"),
        ScontoBase: Get_KendoNumTBValue("idScontoBase"),
        ScontoAddiz1: Get_KendoNumTBValue("idScontoAddiz1"),
        ScontoAddiz2: Get_KendoNumTBValue("idScontoAddiz2"),
        ScontoAddiz3: Get_KendoNumTBValue("idScontoAddiz3"),
        PrezzoRiferitoA: Get_KendoDDLValue("ddlPrezzoRiferitoA"),
        ScontoCalcolato: Get_KendoNumTBValue("idScontoCalcolato"),

        ForzaIva: getKendoSwitch("chkForzaIva"), 
        CodIva: Get_KendoDDLValue("ddlCodIva"),
        AliquotaIva: getAliquotaIva(true),
        Iva: Get_KendoNumTBValue("idIva"),
        ImponibileTotale: Get_KendoNumTBValue("idImponibileTotale"),
        ImponibileTotaleNetto: Get_KendoNumTBValue("idImponibileTotaleNetto"),
        ImportoUnitario: Get_KendoNumTBValue("idImportoUnitario"),
        ImportoTotale: Get_KendoNumTBValue("idImportoTotale"),

        ValoreRiferimentoPrezzo: Get_KendoDDLValue("ddlValoreRiferimento"),
        ProvvigioneAgente: Get_KendoNumTBValue("idProvvigioni"),    //TODO: mettere il change?!?

        ModuloGias: w_modulo_anagrafe_log
    };

    // parametri per conferimento pomodoro
    if (lavCodAccettazionePomodoro) {
        // objCampi.PrezzoEffettivoKgL = kendo.parseFloat(Get_KendoNumTBValue("idPrezzoNetto"))
        //objCampi.Degrado = Get_KendoNumTBValue("idDegradoPerc", true);
        //objCampi.KgNetti = Math.round((w_kg_netti * (1 - kendo.parseFloat(objCampi.Degrado) / 100)));
        objCampi.Conferimento_Speciale = {};
        //if (riepilogoPomodoro != null) objCampi.PrezzoNetto = riepilogoPomodoro.Prezzo_Finale;
    }

    var param = kendo.stringify({ contabDettaglio: kendo.stringify(objCampi), lavCod: cIdLavCod });

    var risp = null;
    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/AggiornaDettagliEconomici",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            var msgError = mostraErrore(risposta, "Errore WS Aggiorna dettagli economici");
        });

    //Ri-aggiorno i dati da interfaccia
    if (risp !== undefined && risp !== null) {
        console.debug(risp);
        Set_KendoNumTBValue("idPrezzo", risp.Prezzo);
        Set_KendoNumTBValue("idPrezzoNetto", risp.PrezzoNetto);
        Set_KendoNumTBValue("idScontoCalcolato", risp.ScontoCalcolato);
        Set_KendoNumTBValue("idScontoCalcolatoEuro", risp.ScontoCalcolatoEuro);

        Set_KendoNumTBValue("idIva", risp.Iva);

        Set_KendoNumTBValue("idImponibileTotale", risp.ImponibileTotale);
        Set_KendoNumTBValue("idImponibileTotaleNetto", risp.ImponibileTotaleNetto);
        Set_KendoNumTBValue("idImportoUnitario", risp.ImportoUnitario);
        Set_KendoNumTBValue("idImportoTotale", risp.ImportoTotale);

        Set_KendoNumTBValue("idProvvigioni", risp.ProvvigioneAgente);
        //Set_KendoNumTBValue("idProvvigioni", risp.ProvvigioneCapoArea);
    }

} 

function RicercaProdottiCompleto_DocContGenerico(options) {

    let messaggioErrore = "";
    let w_cau_mov = Qs_CaricoScarico;

    if (KendoDDL("ddlCategorieMagazzino") === undefined) {
        options.success(JSON.parse("[]"));
        return;
    }
    let ddlCategorieMagazzinoValue = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));

    //----------------------------------------------------------------------------------------------------
    // Filtro categorie magazzino
    //----------------------------------------------------------------------------------------------------
    
    let elemCodArray = [];

    if (ddlCategorieMagazzinoValue === 0) {

        if (lavCodAccettazione) {

            // In accettazione se arrivo qui e non è stata scelta categoria di magazzino, filtro trasformati vegetali e animali

            elemCodArray = [TRASFORMATI_VEGETALI, TRASFORMATI_ANIMALI];

        } else {

            // Filtro le categorie presenti in ddlCategorieMagazzino

            $(KendoDDL("ddlCategorieMagazzino").dataItems()).each(function () {
                if (this.Elem_Cod !== '0') {
                    elemCodArray.push(parseInt(this.Elem_Cod));
                }
            })

        }

    }

    //----------------------------------------------------------------------------------------------------
    
    if (options.data.filter == undefined || options.data.filter.filters.length !== 0) {
        let w_LottoAccettazione = "";
        if (w_cau_mov === CAU_CARICO) {
            w_LottoAccettazione = $('input[name$="txtLottoAccettazione"]').val();
        } else {
            w_LottoAccettazione = Get_KendoDDLValue("ddlLottoAccettazione");
        }

        if (messaggioErrore === "") {

            Recupera_ChiaveMagazzino();

            let bloccaPerSottoGiacenza = false;
            let Flag_QtaNoZero = false;
            if (w_cau_mov === CAU_SCARICO) {
                let w_gest_giacenza = getGestioneGiacenza();
                if (w_gest_giacenza !== enum_Gestione_Giacenze_TuttiProdotti) {
                    if (w_gest_giacenza === enum_Gestione_Giacenze_SoloPresenti) {
                        bloccaPerSottoGiacenza = true;
                        Flag_QtaNoZero = true;
                    }
                }
                else {
                    w_cau_mov = CAU_CARICO;
                }
            }


            let ddlPuaReg = KendoDDL("ddlPUARegolamento");
            let xPuaRegolamento = 0;
            let xTipoPuaRegolamento = 0;

            if (ddlPuaReg !== undefined && ddlPuaReg.value() !== "" && parseInt(ddlPuaReg.value()) !== 0) {
                xPuaRegolamento = parseInt(ddlPuaReg.value());
                xTipoPuaRegolamento = parseInt(ddlPuaReg.dataItem().Regolamento_Tipo);
            }
            let filtroSpecie = lavCodAccettazionePomodoro ? [veg_cod_pomodoro] : null; //{ value: "pomodoro" }

            //----------------------------------------------------------------------------------------------------
            // Filtro aggiuntivo materie prime
            //----------------------------------------------------------------------------------------------------

            let xFiltroAggiuntivoMateriePrime = "";

            // Se è entrata da conferimento (ovvero da novembre 2020 tutti i prodotti Core Business)
            // filtro per chiave conferimento

            if (lavCodAccettazione || lavCodAccettazionePomodoro) {
                xFiltroAggiuntivoMateriePrime = $('input[name$="hf_filtroMateriePrimeConferimento"]').val();
            } else {
                let tipoRapporto = GetTipoRapporto(cIdLavCod);
                if (tipoRapporto === enum_TipoRapporto.Fornitori && is_FF_FormProdottoUC()) {
                    if (cIdLavCod !== enum_LavCod.Ordine_Acquisto.value && cIdLavCod !== enum_LavCod.Nota_Accredito_Ricevuta.value)
                        xFiltroAggiuntivoMateriePrime = " (Materie_Prime.ELEM_COD NOT IN (" + TRASFORMATI_VEGETALI + ", " + TRASFORMATI_ANIMALI + ")) "
                }
            }

            // Se contratti di affitto, presento solo le altre risorse utilizzabili in tale contesto

            if (isContrattoAffitto()) {
                if (xFiltroAggiuntivoMateriePrime !== "") {
                    xFiltroAggiuntivoMateriePrime += " AND ";
                }
                xFiltroAggiuntivoMateriePrime += " Materie_Prime.Extra_Int = 1 ";
            }
            //----------------------------------------------------------------------------------------------------

            let elencoProdottiCompleto = null;

            //----------------------------------------------------------------------------------------------------
            // RICHIAMO LETTURA PRODOTTI
            //----------------------------------------------------------------------------------------------------

            // Faccio la chiamata diversa perchè se nel caso di multicategoria la ricerca è meno veloce

            if (elemCodArray.length === 0) {

                let FiltroCodiceProdotto = null;

                let FiltroDescrizioneProdotto = options.data.filter == undefined ? null : JSON.stringify(options.data.filter.filters);

                //Per i Formulati posso cercare anche per numero di registrazione oltre che per nome formulato

                if (ddlCategorieMagazzinoValue === FORMULATI && FiltroDescrizioneProdotto !== null && FiltroDescrizioneProdotto !== "") {
                    let value_descrizione = JSON.parse(FiltroDescrizioneProdotto);

                    if (value_descrizione !== undefined && value_descrizione !== null && value_descrizione.length > 0 && isNumeric(value_descrizione[0].value)) {
                        FiltroDescrizioneProdotto = null;

                        FiltroCodiceProdotto = value_descrizione[0].value;
                    }
                }

                elencoProdottiCompleto = RicercaElencoCompletoProdotti(objP_super_server, objP_server, objP_utenti, $(cIdPiva).val(),
                    xSa_Cod, xFabbricato_Cod, xTipoDestinazione,
                    ddlCategorieMagazzinoValue, bloccaPerSottoGiacenza, FiltroDescrizioneProdotto,
                    Qs_Mode, w_cau_mov, get_data("inDataEmissione"), xPuaRegolamento, w_LottoAccettazione, false, Flag_QtaNoZero,
                    xTipoPuaRegolamento, filtroSpecie, null, xFiltroAggiuntivoMateriePrime, false, -1, true, FiltroCodiceProdotto);
            } else {
                elencoProdottiCompleto = RicercaElencoCompletoProdottiMultiCategoria(objP_super_server, objP_server, objP_utenti, $(cIdPiva).val(),
                    xSa_Cod, xFabbricato_Cod, xTipoDestinazione,
                    elemCodArray, filtroSpecie, null,
                    bloccaPerSottoGiacenza, options.data.filter == undefined ? null : JSON.stringify(options.data.filter.filters),
                    Qs_Mode, w_cau_mov, get_data("inDataEmissione"), xPuaRegolamento, w_LottoAccettazione, false, Flag_QtaNoZero,
                    xTipoPuaRegolamento, xFiltroAggiuntivoMateriePrime, true);
            }

            //----------------------------------------------------------------------------------------------------

            let elencoProdottiLettoJSon = JSON.parse(elencoProdottiCompleto);

            // Se F&F
            if (is_FF_FormProdottoUC()) {

                let elencoProdottiSoloLinea = [];

                if (Array.isArray(elencoProdottiLettoJSon)) {
                    if (lavCodAccettazione) {

                        // In accettazione conferimento considero solo i Trasformati Vegetali / Animali legati a linea con partita iva = piva
                        // dell'azienda che sta emettendo il documento oppure prodotto pubblico
                        // più i Trasformati Vegetali / Animali non legati a linea

                        elencoProdottiSoloLinea = elencoProdottiLettoJSon.filter(function (x) {
                            return (
                                ((x.Elem_Cod === TRASFORMATI_VEGETALI || x.Elem_Cod === TRASFORMATI_ANIMALI) &&
                                    x.LegatoALinea === 1 &&
                                    ($(cIdPiva).val() === x.Piva || x.Sa_Cod === -1) &&
                                    (lavCodAccettazionePomodoro ? x.Veg_Cod === veg_cod_pomodoro : x.Veg_Cod !== 0)) ||
                                ((x.Elem_Cod === TRASFORMATI_VEGETALI || x.Elem_Cod === TRASFORMATI_ANIMALI) &&
                                    x.LegatoALinea !== 1 &&
                                    (lavCodAccettazionePomodoro ? x.Veg_Cod === veg_cod_pomodoro : x.Veg_Cod !== 0))
                            );
                        });

                    } else {

                        // In tutti gli altri casi considero solo i Trasformati Vegetali / Animali legati a linea con partita iva = piva 
                        // dell'azienda che sta emettendo il documento
                        // più i Trasformati Vegetali / Animali non legati a linea
                        // più tutti i prodotti diversi da Trasformati Vegetali / Animali

                        elencoProdottiSoloLinea = elencoProdottiLettoJSon.filter(function (x) {
                            return (
                                ((x.Elem_Cod === TRASFORMATI_VEGETALI || x.Elem_Cod === TRASFORMATI_ANIMALI) &&
                                    x.LegatoALinea === 1 && ($(cIdPiva).val() === x.Piva || x.Sa_Cod === -1)) ||
                                ((x.Elem_Cod === TRASFORMATI_VEGETALI || x.Elem_Cod === TRASFORMATI_ANIMALI) &&
                                    x.LegatoALinea !== 1) ||
                                (x.Elem_Cod !== TRASFORMATI_VEGETALI && x.Elem_Cod !== TRASFORMATI_ANIMALI)
                            );
                        });
                    }
                }

                let elencoProdottiFinale = [];

                if (lavCodAccettazionePomodoro) {

                    let objVuoto = { Prodotto_Cod: "", Prodotto_Des: "" };
                    elencoProdottiFinale.push(objVuoto);

                }

                for (let i = 0; i < elencoProdottiSoloLinea.length; i++) {
                    let found = false;
                    if (elencoProdottiSoloLinea[i].Elem_Cod === TRASFORMATI_VEGETALI &&
                        (elencoProdottiSoloLinea[i].Mat_Cod_OMNI === 0 ||
                            elencoProdottiSoloLinea[i].Mat_Cod_OMNI === elencoProdottiSoloLinea[i].Prodotto_Cod * - 1)) {
                        // Ho trovato un prodotto OMNI
                        for (let y = 0; y < elencoProdottiSoloLinea.length; y++) {
                            if (elencoProdottiSoloLinea[y].Elem_Cod === TRASFORMATI_VEGETALI &&
                                elencoProdottiSoloLinea[i].Prodotto_Cod !== elencoProdottiSoloLinea[y].Prodotto_Cod &&
                                elencoProdottiSoloLinea[i].Prodotto_Cod === elencoProdottiSoloLinea[y].Mat_Cod_OMNI * - 1) {
                                // Devo scartare il prodotto perchè c'èuna referenza con lo stesso prodotto
                                found = true;
                                break;
                            }

                        }

                    }

                    if (!found)
                        elencoProdottiFinale.push(elencoProdottiSoloLinea[i]);

                }

                options.success(elencoProdottiFinale);

            } else {

                // Non F&F
                options.success(elencoProdottiLettoJSon);

            }
        }

        if (messaggioErrore !== "") {
            MessaggioErrore_Bootstrap(messaggioErrore, "DIV_Messaggi");
            //DisabilitaSalvataggioErrore();
            options.success(JSON.parse("[]"));
        }
    }
    else {
        // Rientro qua quando l'utente riapre la ddl a seguito di aver fatto un filtro ed una selezione di prodotto,
        // invece di ricercare tutti i prodotti li rimostro quelli che avevo caricato con l'ultimo filtro.
        // Perchè kendo, dopo aver fatto un filtro ed una selezione cancella il filtro e riscatena la lettura di tutti gli elementi,
        // in quanto da per scontato che il filtro non serva più e l'utente voglia selezionare un nuovo elemento fra tutti i disponibili;
        // nel nostro caso, invece, siccome i prodotti possono essere moltissimi, ripropongo gli ultimi caricati, eventualmente l'utente
        // farà una nuova ricerca. Anche perché abbiamo impostato che l'utente debba inserire almeno tre caratteri per far partire la ricerca.
        options.success(KendoDDL("ddlProdottoDes").dataSource.data());
    }

}

function RicercaProdottiCompleto_InneschiDaTrappole(options) {

    let messaggioErrore = "";

    let w_cau_mov = CAU_CARICO;

    let ddlCategorieMagazzinoValue = INNESCHI;

    if (options.data.filter == undefined || options.data.filter.filters.length !== 0) {

        let w_LottoAccettazione = "";

        if (messaggioErrore === "") {

            let bloccaPerSottoGiacenza = false;
            let Flag_QtaNoZero = false;
            let xPuaRegolamento = 0;
            let xTipoPuaRegolamento = 0;
            let filtroSpecie = "";
            let xFiltroAggiuntivoMateriePrime = "";

            let elencoProdottiCompleto = null;

            // -----------------  Richiamo lettura prodotti ----------

            elencoProdottiCompleto = RicercaElencoCompletoProdotti(objP_super_server,
                objP_server, objP_utenti, $(cIdPiva).val(), xSa_Cod, xFabbricato_Cod,
                xTipoDestinazione, ddlCategorieMagazzinoValue, bloccaPerSottoGiacenza, 
                options.data.filter == undefined ? null : JSON.stringify(options.data.filter.filters),
                Qs_Mode, w_cau_mov, get_data("inDataEmissione"), xPuaRegolamento, w_LottoAccettazione,
                false, Flag_QtaNoZero, xTipoPuaRegolamento, filtroSpecie, null, xFiltroAggiuntivoMateriePrime,
                false, -1, true, null, propostaDatiRiga.codiceTrappola);

            // -----------------  Richiamo lettura prodotti ----------

            let elencoProdottiLettoJSon = JSON.parse(elencoProdottiCompleto);

            options.success(elencoProdottiLettoJSon);

        }

        if (messaggioErrore !== "") {
            MessaggioErrore_Bootstrap(messaggioErrore, "DIV_Messaggi");
            options.success(JSON.parse("[]"));
        }
    }
}

function RicercaAliasDaMatCod(piva, matCod, codRisUm, flagVuoto) {
    let arrAlias = [];

    let param = kendo.stringify({ piva: piva, mat_cod: matCod, cod_risum: codRisUm });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Leggi_Alias_Prodotto",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                let objVuoto = {
                    Mat_Cod_Alias: 0,
                    Mat_Des_Alias: "",
                    Filtro_Contatti_String: ""
                };
                risp.unshift(objVuoto);
            }
            arrAlias = risp;
        },
        null);

    return arrAlias;
}

function RicercaPUA_Regolamenti(flagVuoto, Regolamento_Cod, Elem_Cod) {

    elencoPUA_Regolamenti_ElemCod = Elem_Cod;
    elencoPUA_Regolamenti = {};
    let param = kendo.stringify({
        Regolamento_Cod: parseInt(Regolamento_Cod),
        DataMovimento: get_data("inDataEmissione"),
        Elem_Cod: parseInt(Elem_Cod)
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Leggi_PUA_Regolamenti",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                let objVuoto = {
                    Regolamento_Cod: 0,
                    Regolamento_DES: ""
                };
                risp.unshift(objVuoto);
            }
            elencoPUA_Regolamenti = risp;
        },
        null);

    // Essendo una variabile globale, non c'è bisogno del "return":
    // return elencoPUA_Regolamenti;

}

function RicercaUdm_Optimize(piva, flagVuoto, Flag_ProCod_Negativo, xSa_Cod, xFabbricato_Cod, Cau_Mov, Elem_Cod, Pro_Cod, isFreshAndFood, Flag_QtaNoZero, Flag_QtaMaggioreZero) {

    elencoUdmOptimized = [];
    var xFiltroAggiuntivo = "";
    var xOrderBy = "";

    let w_gest_giacenza = getGestioneGiacenza();
    if (w_gest_giacenza === enum_Gestione_Giacenze_TuttiProdotti)
        Cau_Mov = CAU_CARICO;

    var param = kendo.stringify({
        piva: piva,
        xSa_Cod: parseInt(xSa_Cod),
        xFabbricato_Cod: parseInt(xFabbricato_Cod),
        Cau_Mov: Cau_Mov,
        Elem_Cod: parseInt(Elem_Cod),
        Flag_ProCod_Negativo: Flag_ProCod_Negativo,
        Pro_Cod: parseInt(Pro_Cod),
        Mat_Cod: 0,
        Flag_Prima_Riga: flagVuoto,
        Testo_PrimaRiga: "",
        Cod_Prima_Riga: "",
        Testo_Da_Ricercare: "",
        xFiltroAggiuntivo: xFiltroAggiuntivo,
        xOrderBy: xOrderBy,
        isFreshAndFood: isFreshAndFood,
        Flag_QtaNoZero: Flag_QtaNoZero,
        Flag_QtaMaggioreZero: Flag_QtaMaggioreZero
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Udm_Optimize",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            elencoUdmOptimized = risp;
        }, null);

}

function RicercaUdm_Optimize_Lotto(piva, flagVuoto, Flag_ProCod_Negativo, xSa_Cod, xFabbricato_Cod, Cau_Mov, Elem_Cod, Pro_Cod, Lotto, isFreshAndFood, Flag_QtaNoZero, Flag_QtaMaggioreZero) {

    elencoUdmOptimized = [];
    var xFiltroAggiuntivo = "";
    var xOrderBy = "";

    let w_gest_giacenza = getGestioneGiacenza();
    if (w_gest_giacenza === enum_Gestione_Giacenze_TuttiProdotti)  
        Cau_Mov = CAU_CARICO;

    var param = kendo.stringify({
        piva: piva,
        xSa_Cod: parseInt(xSa_Cod),
        xFabbricato_Cod: parseInt(xFabbricato_Cod),
        Cau_Mov: Cau_Mov,
        Elem_Cod: parseInt(Elem_Cod),
        Flag_ProCod_Negativo: Flag_ProCod_Negativo,
        Pro_Cod: parseInt(Pro_Cod),
        Mat_Cod: 0,
        Lotto: Lotto,
        Flag_Prima_Riga: flagVuoto,
        Testo_PrimaRiga: "",
        Cod_Prima_Riga: "",
        Testo_Da_Ricercare: "",
        xFiltroAggiuntivo: xFiltroAggiuntivo,
        xOrderBy: xOrderBy,
        isFreshAndFood: isFreshAndFood,
        Flag_QtaNoZero: Flag_QtaNoZero,
        Flag_QtaMaggioreZero: Flag_QtaMaggioreZero
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Udm_Optimize_Lotto",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            elencoUdmOptimized = risp;
        }, null);

    // Variabile globale ... non c'è bisogno 
    //   return elencoUdmOptimized;
}

function RicercaUdm_Optimize_Regolamento(flagVuoto, xSa_Cod, xFabbricato_Cod, Cau_Mov, Elem_Cod, Prodotto_Cod, Regolamento_Cod, isFreshAndFood, Flag_QtaNoZero, Flag_QtaMaggioreZero) {

    elencoUdmOptimizedRegolamento = [];

    let w_gest_giacenza = getGestioneGiacenza();
    if (w_gest_giacenza === enum_Gestione_Giacenze_TuttiProdotti)
        Cau_Mov = CAU_CARICO;

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        DataMovimento: get_data("inDataEmissione"),
        xSa_Cod: parseInt(xSa_Cod),
        xFabbricato_Cod: parseInt(xFabbricato_Cod),
        Cau_Mov: Cau_Mov,
        Elem_Cod: parseInt(Elem_Cod),
        Flag_ProCod_Negativo: true,
        Prodotto_Cod: Prodotto_Cod,
        Mat_Cod: 0,
        Flag_Prima_Riga: flagVuoto,
        Testo_PrimaRiga: "",
        Cod_Prima_Riga: "",
        Testo_Da_Ricercare: "",
        RegolamentoCod: Regolamento_Cod,
        xFiltroAggiuntivo: "",
        xOrderBy: "",
        isFreshAndFood: isFreshAndFood,
        Flag_QtaNoZero: Flag_QtaNoZero,
        Flag_QtaMaggioreZero: Flag_QtaMaggioreZero
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Udm_Optimize_Regolamento",
        param, false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            elencoUdmOptimizedRegolamento = risp;
        }, null);

    // Variabile globale ... non c'è bisogno 
    //   return elencoUdmOptimizedRegolamento;
}

function Recupera_UdmFertilizzante(Fer_Cod, PUA_RegolamentoCod) {

    let udmDefaultFerti = null;

    let param = kendo.stringify({
        Fer_Cod: Fer_Cod,
        PUA_RegolamentoCod: PUA_RegolamentoCod
    });
    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Recupera_UdmFertilizzante",
        param, false,
        function (risposta) {
            udmDefaultFerti = risposta.RispostaStringa;
        },
        function (risposta) {
            //loggo solo l'errore sulla console
            let msgError = ricavaErrore(risposta, "Errore WS Recupera Udm Fertilizzante");
            console.error("Errore interno: " + msgError);
        });

    return udmDefaultFerti;
}

function Recupera_UdmsFarmaco(Farm_Cod) {    
    let params = kendo.stringify({ Farm_Cod: Farm_Cod, });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Recupera_UdmsFarmaco",
        params, false,
        function (risposta) {
            elencoUdmOptimized = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            let msgError = ricavaErrore(risposta, "Errore WS Recupera Udm Farmaco");
            console.error("Errore interno: " + msgError);
        });
}

// -----------------------------------------------------------------
// ------ Fine lettura di tutte le tabelle di base
// -----------------------------------------------------------------

// -----------------------------------------------------------------
// ------ Inizio lettura di tutte le tabelle di base ASYNC
// -----------------------------------------------------------------

function RicercaUdm_Optimize_Async(piva, flagVuoto, Flag_ProCod_Negativo, xSa_Cod, xFabbricato_Cod, Cau_Mov, Elem_Cod, Pro_Cod, isFreshAndFood, Flag_QtaNoZero, Flag_QtaMaggioreZero) {

    var xFiltroAggiuntivo = "";
    var xOrderBy = "";
     
    return new Promise(function (resolve, reject) {

        let risp = [];
        let objVuoto = null;

        if (Elem_Cod === -999) {
            objVuoto = { Udm_Cod: 0, Udm_Des: "" };
            risp.unshift(objVuoto);
            resolve(risp);
        } else {
            let param = kendo.stringify({
                piva: piva,
                xSa_Cod: parseInt(xSa_Cod),
                xFabbricato_Cod: parseInt(xFabbricato_Cod),
                Cau_Mov: Cau_Mov,
                Elem_Cod: parseInt(Elem_Cod),
                Flag_ProCod_Negativo: Flag_ProCod_Negativo,
                Pro_Cod: parseInt(Pro_Cod),
                Mat_Cod: 0,
                Flag_Prima_Riga: flagVuoto,
                Testo_PrimaRiga: "",
                Cod_Prima_Riga: "",
                Testo_Da_Ricercare: "",
                xFiltroAggiuntivo: xFiltroAggiuntivo,
                xOrderBy: xOrderBy,
                isFreshAndFood: isFreshAndFood,
                Flag_QtaNoZero: Flag_QtaNoZero,
                Flag_QtaMaggioreZero: Flag_QtaMaggioreZero
            });

            ajaxAgronica(indirizzoHttp_DocContabile_WS + "/Udm_Optimize",
                param,
                function (risposta) {
                    risp = JSON.parse(risposta.RispostaStringa);
                    resolve(risp);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Udm Optimize");
                    reject(new Error(msgError));
                },
                null,
                false);
        }
    });
}

function RicercaCausali_Riga_Async(flagVuoto) {

    return new Promise(function (resolve, reject) {

        if (elencoCausali_Riga === null) {

            var url = CaricaUrlRicercaCausaliRiga();
            var param = CaricaParametriRicercaCausaliRiga();

            ajaxAgronica(url, param,
                function (risposta) {
                    let risp = CaricaRispostaRicercaCausaliRiga(flagVuoto, risposta);
                    resolve(risp);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Causali Riga");
                    reject(new Error(msgError));
                },
                null,
                false);

        } else {

            resolve(elencoCausali_Riga);

        }

    });

}

function RicercaCausali_Riga_Sync(flagVuoto) {

    var url = CaricaUrlRicercaCausaliRiga();
    var param = CaricaParametriRicercaCausaliRiga();

    ajaxAgronicaSync(url, param,
        false,
        function (risposta) {
            elencoCausali_Riga = CaricaRispostaRicercaCausaliRiga(flagVuoto, risposta);
        },
        function (risposta) {
            MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxFormProdottoUC, "ErroreDuePunti_", "Errore: ") + risposta.Errore, "DIV_Messaggi");
        });

}

function CaricaUrlRicercaCausaliRiga() {

    return indirizzoHttp_DocContabile_WS + "/Carica_Causale_Riga"

}

function CaricaParametriRicercaCausaliRiga() {

    if (getKendoSwitch("chkAccompagnatoria")) {

        chkAccompagnatoria = 1

    } else {

        chkAccompagnatoria = 0

    }

    return kendo.stringify({ lav_cod: cIdLavCod, qs_tipo: Qs_Tipo, chkAccompagnatoria });

}

function CaricaRispostaRicercaCausaliRiga(flagVuoto, strRisposta) {

    let objRisposta = JSON.parse(strRisposta.RispostaStringa);

    if (flagVuoto && objRisposta.length > 1) {

        var objVuoto = { KeyCausaleRiga: 0, DescrCausale: "" };

        objRisposta.unshift(objVuoto);

    }

    return objRisposta;

}

function RicercaPUA_Regolamenti_Async(flagVuoto, Regolamento_Cod, Elem_Cod) {

    return new Promise(function (resolve, reject) {

        var param = kendo.stringify({
            Regolamento_Cod: parseInt(Regolamento_Cod),
            DataMovimento: get_data("inDataEmissione"),
            Elem_Cod: parseInt(Elem_Cod)
        });

        ajaxAgronica(indirizzoHttp_DocContabile_WS + "/Leggi_PUA_Regolamenti",
            param,
            function (risposta) {
                let risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    let objVuoto = {
                        Regolamento_Cod: 0,
                        Regolamento_DES: ""
                    };
                    risp.unshift(objVuoto);
                }
                resolve(risp);
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Pua Regolamento");
                reject(new Error(msgError));
            },
            null,
            false);
    });
}

// -----------------------------------------------------------------
// ------ Fine lettura di tutte le tabelle di base ASYNC
// -----------------------------------------------------------------

function Ricerca_ImballiFormProdottoUC(piva, idAgenda, id_mov_det, options) {

    var param = kendo.stringify({ piva: piva, idAgenda: idAgenda, lavCod: cIdLavCod, idMovDet: id_mov_det });
    var risp = "";

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Ricerca_ImballiFormProdottoUC",
        param, false,
        function (risposta) {
            $('input[name$="hdKendo_Imballi_formProdottoUC"]').val(risposta.RispostaStringa);
        }, null);
 
}

function Impostazione_LottoProdotto(w_gest_lotti) {

    let matCod = 0;
    if (Get_KendoDDLValue("ddlProdottoDes") !== "") {
        let codProdotto = parseInt(Get_KendoDDLValue("ddlProdottoDes"));
        if (codProdotto < 0) {
            matCod = Math.abs(codProdotto);
        }
    }
    
    let w_qual = 0;
    if (KendoDDL("ddlqualità") !== undefined)
        w_qual = Get_KendoDDLValue("ddlqualità");
    let w_cert = 0;
    if (KendoDDL("ddlcertificazioni") !== undefined)
        w_cert = Get_KendoDDLValue("ddlcertificazioni");

    let doc_numero_sin = "";
    let doc_numero = 0;
    let doc_numero_des = "";

    let doc_numero_sin_accettazione = "";
    let doc_numero_accettazione = 0;
    let doc_numero_des_accettazione = "";

    //TODO: ATTENZIONE - va sistemato per il verso e capire come fare e se serve quando non si è in accettazione
    if (lavCodAccettazione === true) {
        doc_numero_sin = $("#inNumDocSin2").val().toUpperCase();
        doc_numero = Get_KendoNumTBValue("inNumDoc2");
        doc_numero_des = $("#inNumDocDes2").val().toUpperCase();

        doc_numero_sin_accettazione = $("#inNumDocSin").val().toUpperCase();
        doc_numero_accettazione = Get_KendoNumTBValue("inNumDoc");
        doc_numero_des_accettazione = $("#inNumDocDes").val().toUpperCase();
    } else {

        //il documento di accettazione non c'è
        doc_numero_sin = $("#inNumDocSin").val().toUpperCase();
        doc_numero = Get_KendoNumTBValue("inNumDoc");
        doc_numero_des = $("#inNumDocDes").val().toUpperCase();
    }

    let w_Elem_Cod = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
    let w_modulo_anagrafe_log = moduloFromElemCod(w_Elem_Cod);

    let ubicazioneCarico = Get_KendoDDLValue("ddlUbicDestinazione");
    var tipoDestinazioneCarico = parseInt(ubicazioneCarico.split("_")[0]);
    var saCodDestCarico = parseInt(ubicazioneCarico.split("_")[1]);
    var idDestinazioneCarico = parseInt(ubicazioneCarico.split("_")[2]);



    let param = kendo.stringify({
        piva: $(cIdPiva).val(),
        sa_cod: saCodDestCarico,
        modulo_generazione: w_modulo_anagrafe_log,
        mat_cod: matCod,
        cod_contatto: KendoDDL(ddlContatto1Nome).dataItem().Cod_Contatto,
        cod_risum: parseInt(Get_KendoDDLValue(ddlContatto1Nome)),

        doc_numero_sin: doc_numero_sin,
        doc_numero: doc_numero,
        doc_numero_des: doc_numero_des,

        doc_numero_sin_accettazione: doc_numero_sin_accettazione,
        doc_numero_accettazione: doc_numero_accettazione,
        doc_numero_des_accettazione: doc_numero_des_accettazione,

        data_ingresso: kendo.parseDate($("#inDataSpedizione").val()),
        qualita_cod: w_qual,
        destinazioni_cod: tipoDestinazioneCarico & '|' & saCodDestCarico & '|' & idDestinazioneCarico,
        appezzamenti: ImpostaAppezamenti(),
        lotti_impianti: ImpostaLottiImpianti(),
        certificato_cod: w_cert,
        contatore_univoco_parametri: "",
        sigla_certificazione: ""
    });

    var lotto = null;
    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Impostazione_LottoProdotto",
        param,
        false,
        function(risposta) {
            lotto = risposta.RispostaStringa;
        },
        function (risposta) {
            if (w_gest_lotti === enum_Gestione_Lotti_Obbligatoria)
                var msgError = mostraErrore(risposta, "Errore in fase di assegnazione Lotto");
        });

    return lotto;
}

function IvaConti(piva, data_movimento, cod_risum, elem_cod, pro_cod, mat_cod, cau_mov) {

    let param = kendo.stringify({
        piva: piva,
        data_movimento: data_movimento,
        cod_risum: cod_risum,
        elem_cod: elem_cod,
        pro_cod: pro_cod,
        mat_cod: mat_cod,
        cau_mov: cau_mov
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/IvaConti",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            console.debug(risp);

            Last_Iva_Default_Prodotto = risp.Cod_Iva;

            if (risp.Cod_Iva !== 0) {
                Set_KendoDDLValue("ddlCodIva", risp.Cod_Iva);
                ddlCodIva_change();
            }

            //TODO: setta anche i conti di default
            if (gestioneContabilita === enum_Livello_GestContabilita_Bilancio) {
                if (risp.Cod_Conto_Economico !== 0) {
                    let ddlEco = KendoDDL("ddlContoEconomico");
                    if (ddlEco !== undefined && ddlEco !== null &&
                        ddlEco.dataItems() !== undefined &&
                        ddlEco.dataItems() !== null &&
                        ddlEco.dataItems().length > 0) {
                        //Se esiste il conto nell'elenco, lo imposto
                        if (ddlEco.dataItems().some(function (dataItem) { return dataItem.Cod_Conto === risp.Cod_Conto_Economico; })) {
                            ddlEco.select(function (dataItem) {
                                return dataItem.Cod_Conto === risp.Cod_Conto_Economico;
                            });
                        }
                    }
                }

                if (risp.Cod_Conto_Economico !== 0) {
                    let ddlPat = KendoDDL("ddlContoPatrimoniale");
                    if (ddlPat !== undefined && ddlPat !== null &&
                        ddlPat.dataItems() !== undefined &&
                        ddlPat.dataItems() !== null &&
                        ddlPat.dataItems().length > 0) {
                        //Se esiste il conto nell'elenco, lo imposto
                        if (ddlPat.dataItems().some(function (dataItem) { return dataItem.Cod_Conto_Pat === risp.Cod_Conto_Patrimoniale; })) {
                            ddlPat.select(function (dataItem) {
                                return dataItem.Cod_Conto_Pat === risp.Cod_Conto_Patrimoniale;
                            });
                        }
                    }
                }
            }



        }, function (rispostaErrore) {
            MessaggioErrore_Bootstrap(rispostaErrore.Errore, "DIV_Messaggi");
        });
}


function Ricerca_OrdiniFormProdottoUC(piva, cod_risum, lav_cod) {

    var param = kendo.stringify({ piva: piva, cod_risum: cod_risum, lav_cod: lav_cod});

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Ricerca_OrdiniFormProdottoUC",
        param, false,
        function (risposta) {
            $('input[name$="hdKendo_Ordini_formProdottoUC"]').val(risposta.RispostaStringa);
            popolaOrdiniFormProdottoUC("tab_ordini_cliente_formProdottoUC");
        }, null);

}

function Ricerca_DDTFormProdottoUC(piva, cod_risum, lav_cod) {

    var param = kendo.stringify({ piva: piva, cod_risum: cod_risum, lav_cod: lav_cod});

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Ricerca_DDTFormProdottoUC",
        param, false,
        function (risposta) {
            $('input[name$="hdKendo_DDT_formProdottoUC"]').val(risposta.RispostaStringa);
            popolaDDTFormProdottoUC("tab_ddt_cliente_formProdottoUC");
        }, null);

}

function Verifica_Utilizzo_CalCod() {

    let altriMovimentiStessoCalCod = "";

    if ($('input[name$="hf_key_mov_dett"]').val() !== "") {

        var id_mov_det = parseInt($('input[name$="hf_key_mov_dett"]').val().split("_")[4]);

        var param = kendo.stringify({ piva: $(cIdPiva).val(), id_mov_det: id_mov_det, cal_cod: $('input[name$="hf_Cal_Cod"]').val() });

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Verifica_Utilizzo_CalCod",
            param, false,
            function (risposta) {
                altriMovimentiStessoCalCod = risposta.RispostaStringa;
            },
            null);
    }

    return altriMovimentiStessoCalCod;
   
}

// 'i18n Contratti Pomodoro?
function Verifica_Contratto_Pomodoro(imposta_prezzo) {

    let messaggio = "";
    let conferente = null;
    let prodotto = null;
    let data = kendo.parseDate($("#inDataEmissione").val()); // inDataSpedizione

    if (KendoDDL("ddlProdottoDes") !== undefined && KendoDDL("ddlProdottoDes").dataItem() !== undefined && KendoDDL("ddlProdottoDes").dataItem() !== null) {
        prodotto = KendoDDL("ddlProdottoDes").dataItem();
    }

    if (KendoDDL(ddlContatto1Nome) !== undefined && KendoDDL(ddlContatto1Nome).dataItem() !== undefined && KendoDDL(ddlContatto1Nome).dataItem() !== null) {
        conferente = KendoDDL(ddlContatto1Nome).dataItem();
    }

    if (conferente != null) {

        let mat_cod = prodotto != null && prodotto.Prodotto_Cod != "" ? Math.abs(prodotto.Prodotto_Cod) : 0;

        var param = kendo.stringify({ piva: $(cIdPiva).val(), cod_risum: conferente.Cod_RisUm, elem_cod: TRASFORMATI_VEGETALI, mat_cod: mat_cod, data: data });

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Verifica_Contratto_Pomodoro",
            param, false,
            function (risposta) {
                if (risposta.RispostaStringa == "") {
                    contrattoPomodoro = null;
                    descrizionePomodoro = prodotto != null && prodotto.Prodotto_Des != "" ? "di <b>" + prodotto.Prodotto_Des + "</b>" : "";
                    messaggio = "Attenzione! Nessun contratto di conferimento " + descrizionePomodoro + " presente in archivio per <b>" + conferente.Rag_Soc + "</b> ";
                    messaggio += "<br>L'inserimento della bolla di accettazione non può essere salvata.";
                } else {
                    contrattoPomodoro = JSON.parse(risposta.RispostaStringa);
                    if (imposta_prezzo) Set_KendoNumTBValue("idPrezzo", contrattoPomodoro.Prezzo);
                }
            },
            null);
    }    

    return messaggio;

}

// 'i18n Contratti Pomodoro?
function Calcola_Prezzo_Pomodoro(e) {

    let peso = kendo.parseFloat(Get_KendoNumTBValue("idKgNetti"));
    let prezzo = kendo.parseFloat(Get_KendoNumTBValue("idPrezzo"));
    let data = kendo.parseDate($("#inDataEmissione").val()); // inDataSpedizione?

    // setto i parametri per il conferimento pomodoro
    parametriPomodoro.Pomodoro_BIO = current_Reg_Cod == 4;
    parametriPomodoro.Inerti = kendo.parseFloat(Get_KendoNumTBValue("txtinerti", true));
    parametriPomodoro.Verde = kendo.parseFloat(Get_KendoNumTBValue("txtverde", true));
    parametriPomodoro.Marcio = kendo.parseFloat(Get_KendoNumTBValue("txtmarcio", true));
    parametriPomodoro.Residuoottico = kendo.parseFloat(Get_KendoNumTBValue("txtgradobrix", true));
    parametriPomodoro.FruttiSchiacciati = kendo.parseFloat(Get_KendoNumTBValue("txtfruttischiacciati", true));
    parametriPomodoro.FruttiImmaturi = kendo.parseFloat(Get_KendoNumTBValue("txtfruttiimmaturi", true));
    parametriPomodoro.FruttiScottati = kendo.parseFloat(Get_KendoNumTBValue("txtfruttiscottati", true));
    parametriPomodoro.FruttiLesionati = kendo.parseFloat(Get_KendoNumTBValue("txtfruttilesionati", true));

    if (prezzo == null || prezzo <= 0) {
        kendo.alert("Prezzo non disponibile");
    } else if (peso == null || peso <= 0) {
        kendo.alert("Quantità non impostata");
    } else {

        var param = kendo.stringify({
            peso: peso,
            prezzo: prezzo,
            data: data,
            parametri: kendoEscapeOggetto(parametriPomodoro),
            contratto: kendoEscapeOggetto(contrattoPomodoro)
        });

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Calcola_Prezzo_Pomodoro",
            param,
            false,
            function(risposta) {
                riepilogoPomodoro = JSON.parse(risposta.RispostaStringa);
                if (e === undefined) {
                    Riepilogo_Dati_Pomodoro();
                }
                if (riepilogoPomodoro.Errori != "") {
                    kendo.alert(riepilogoPomodoro.Errori);
                } else {
                    Set_KendoNumTBValue("idDegradoPerc", riepilogoPomodoro.TotaleDMA);
                    idDegradoPerc_change();
                    Set_KendoNumTBValue("idPrezzo", contrattoPomodoro.Prezzo);
                    // Set_KendoNumTBValue("idPrezzoNetto", riepilogoPomodoro.Prezzo_Netto);
                    Set_KendoNumTBValue("idPrezzoNetto", riepilogoPomodoro.Prezzo_Finale);
                    Set_KendoNumTBValue("idImponibileTotale", riepilogoPomodoro.Prezzo_Finale * riepilogoPomodoro.Peso_Effettivo);
                    AggiornaDettagliEconomici();
                }
            },
            null);
    }

}

// 'i18n Contratti Pomodoro?
function Riepilogo_Dati_Pomodoro() {

    if (riepilogoPomodoro != null) {

        var param = kendo.stringify({
            parametri: kendoEscapeOggetto(parametriPomodoro),
            contratto: kendoEscapeOggetto(contrattoPomodoro),
            riepilogo: kendoEscapeOggetto(riepilogoPomodoro)
        });

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Riepilogo_Dati_Pomodoro",
            param, false,
            function (risposta) {
                $("#datiRiepilogativiPomodoroDialog").kendoDialog({
                    width: "800px",
                    height: "600px",
                    title: "Dati Riepilogativi",
                    closable: true,
                    modal: true,
                    visible: false,
                    content: risposta.RispostaStringa,
                    messages: { close: "Chiudi" },
                    actions: [{ text: "Chiudi" }]
                });
                $("#datiRiepilogativiPomodoroDialog").data("kendoDialog").open();
            },
            null);

    } else kendo.alert("Riepilogo dati non disponibile");

}

function Riepilogo_Prezzi_Listini(imposta_prezzo) {

    let contatto = Get_KendoDDLValue(ddlContatto1Nome);    
    let categoria = Get_KendoDDLValue("ddlCategorieMagazzino");
    let prodotto = Get_KendoDDLValue("ddlProdottoDes");
    let um = Get_KendoDDLValue("ddlUM");
    let tipoRapporto = GetTipoRapporto(cIdLavCod);
    let data = kendo.parseDate($("#inDataEmissione").val());

    if (contatto != null && contatto != "" && prodotto != null && prodotto != "") {

        var param = kendo.stringify({
            piva: $(cIdPiva).val(),
            tipo: tipoRapporto == enum_TipoRapporto.Clienti ? 2 : 1,
            data: data,
            sa_cod: Qs_SaCod,
            cod_risum: contatto,
            elem_cod: categoria,
            udm_cod: um != null && um != "" ? parseInt(um) : 0,
            pro_cod: prodotto < 0 ? 0 : Math.abs(prodotto),
            mat_cod: prodotto < 0 ? Math.abs(prodotto) : 0
        });

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Riepilogo_Prezzi_Listini",
            param, false,
            function (risposta) {
                var dati = JSON.parse(risposta.RispostaStringa);
                if (imposta_prezzo) {
                    let listino = null;
                    for (i = 0; i < dati.length; i++) {
                        // imposto il primo prezzo trovato
                        if (listino == null) listino = dati[i];
                        // a meno che non ce ne siano altri allo stesso livello 
                        else if (listino.Livello == dati[i].Livello) {
                            listino = null;
                            break;
                        } else break;                    
                    }
                    // imposta prezzo e sconti listino
                    if (listino != null) ImpostaPrezzoListino(listino);
                } else if (dati.length == 0) {
                    kendo.alert(TraduzioneMultiResx(resxFormProdottoUC, "NessunListinoDisponibile", "Nessun listino disponibile"));
                } else {
                    var winListini = $("#windowRiepilogoPrezziListini")
                        .kendoWindow({
                            title: TraduzioneMultiResx(resxFormProdottoUC, "RiepilogoPrezziListini", "Riepilogo Prezzi Listini"),
                            modal: true,
                            visible: false,
                            resizable: true,
                            height: "80%",
                            width: "80%",
                            actions: ["Maximize", "Close"]
                        }).data("kendoWindow");
                    winListini.content("<div id='gridPrezziListini'></div>");
                    popolaGrigliaPrezziListini("gridPrezziListini", dati);
                    winListini.center().open();
                }
            },
            null);
    } else
        if (!imposta_prezzo &&
            categoria !== RIGA_DESCRIZIONE_LIBERA && categoria !== ALTRI_BENI)
            kendo.alert(TraduzioneMultiResx(resxFormProdottoUC, "SelezionareUnProdotto", "È necessario selezionare un prodotto"));
}

function popolaGrigliaPrezziListini(IDControllo, dati) {
    var funzioniCRUD = {
        funzioneRead: function (options) { options.success(dati); }
    };
    var idModel = "KeyListinoProdotto";
    var campiKendoModel = {
        KeyListinoProdotto: { type: "string" },
        Tipo_Classe: { type: "number" },
        Listino_Classe_Des: { type: "string" },
        Listino_Cod: { type: "number" },
        Listino_Des: { type: "string" },
        Listino_Cod_Des: { type: "string" },
        Sa_Nome: { type: "string" },
        Rapporto_Des: { type: "string" },
        Rag_Soc: { type: "string" },
        ChkApplicabilita: { type: "number" },
        Validita_Inizio_Listino: { type: "date" },
        Validita_Fine_Listino: { type: "date" },
        Categoria: { type: "string" },
        Cod_Articolo: { type: "string" },
        Prodotto: { type: "string" },
        Calibro: { type: "string" },
        Qualita: { type: "string" },
        Udm_Des: { type: "string" },
        Qta: { type: "number" },
        Prezzo: { type: "number" },
        Sconto: { type: "number" },
        Validita_Inizio: { type: "date" },
        Validita_Fine: { type: "date" }
    };
    var colonneCustomKendoGrid = [
        {
            command: [
                { iconClass: "fa fa-euro fa-lg", name: "Prezzi", text: "&nbsp;", click: SelezionaPrezzoListino }
            ],
            title: TraduzioneMultiResx(resxFormProdottoUC, "Seleziona", "Seleziona"),
            width: 85
        }
    ]; 
    var colonneKendoGrid = [
        {
            field: "Tipo_Classe", title: TraduzioneMultiResx(resxFormProdottoUC, "TipoListino", "Tipo listino"), filterable: { multi: true, search: true }, hidden: true, width: 100,
            values: [{ text: TraduzioneMultiResx(resxFormProdottoUC, "Acquisto", "Acquisto"), value: "1" }, { text: TraduzioneMultiResx(resxFormProdottoUC, "Vendita", "Vendita"), value: "2" }]
        },
        { field: "Listino_Classe_Des", title: TraduzioneMultiResx(resxFormProdottoUC, "ClasseListino", "Classe listino"), filterable: { multi: true, search: true }, hidden: true, width: 100 },
        { field: "Listino_Des", title: TraduzioneMultiResx(resxFormProdottoUC, "DescrizioneListino", "Descrizione listino"), filterable: { multi: true, search: true } },
        { field: "Listino_Cod_Des", title: TraduzioneMultiResx(resxFormProdottoUC, "CodiceListino", "Codice listino"), filterable: { multi: true, search: true }, hidden: true, width: 100 },
        { field: "Sa_Nome", title: TraduzioneMultiResx(resxFormProdottoUC, "CentroAziendale", "Centro Aziendale"), filterable: { multi: true, search: true } },
        { field: "Rapporto_Des", title: TraduzioneMultiResx(resxFormProdottoUC, "RapportoContabile", "Rapporto Contabile"), filterable: { multi: true, search: true } },
        { field: "Rag_Soc", title: TraduzioneMultiResx(resxFormProdottoUC, "Contatto", "Contatto"), filterable: { multi: true, search: true } },
        // { field: "ChkApplicabilita", title: "Applicabile", filterable: { multi: true, search: true }, values: [{ text: "SI", value: "1" }, { text: "NO", value: "-1" }] },
        { field: "Validita_Inizio_Listino", title: TraduzioneMultiResx(resxFormProdottoUC, "ValiditaInizioListino", "Validità inizio listino"), format: "{0:dd/MM/yyyy}", hidden: true, width: 100 },
        { field: "Validita_Fine_Listino", title: TraduzioneMultiResx(resxFormProdottoUC, "ValiditaFineListino", "Validita fine listino"), format: "{0:dd/MM/yyyy}", hidden: true, width: 100 },
        { field: "Categoria", title: TraduzioneMultiResx(resxFormProdottoUC, "Categoria", "Categoria"), filterable: { multi: true, search: true }, hidden: true, width: 100 },
        { field: "Cod_Articolo", title: TraduzioneMultiResx(resxFormProdottoUC, "CodiceArticolo", "Codice Articolo"), filterable: { multi: true, search: true }, hidden: true, width: 100 },
        { field: "Prodotto", title: TraduzioneMultiResx(resxFormProdottoUC, "Prodotto", "Prodotto"), filterable: { multi: true, search: true }, width: 200 },
        { field: "Calibro", title: TraduzioneMultiResx(resxFormProdottoUC, "Calibro", "Calibro"), filterable: { multi: true, search: true }, hidden: true, width: 100 },
        { field: "Qualita", title: TraduzioneMultiResx(resxFormProdottoUC, "Qualità", "Qualità"), filterable: { multi: true, search: true }, hidden: true, width: 100 },
        { field: "Udm_Des", title: TraduzioneMultiResx(resxFormProdottoUC, "UnitàDiMisuraAbbr", "UdM"), filterable: { multi: true, search: true } },
        { field: "Qta", title: TraduzioneMultiResx(resxFormProdottoUC, "RisorsaQuantità", "Quantità"), format: "{0:n2}", attributes: { style: "text-align: right" }, hidden: true, width: 100 },
        { field: "Prezzo", title: TraduzioneMultiResx(resxFormProdottoUC, "Prezzo", "Prezzo"), format: "{0:n5}", attributes: { style: "text-align: right" } },
        { field: "Sconto", title: TraduzioneMultiResx(resxFormProdottoUC, "ScontoPercentuale", "Sconto %"), format: "{0:n2}", attributes: { style: "text-align: right" } },
        { field: "Sconto_Add1", title: TraduzioneMultiResx(resxFormProdottoUC, "Sconto1Percentuale", "Sconto1 %"), format: "{0:n2}", attributes: { style: "text-align: right" }, hidden: true, width: 100 },
        { field: "Sconto_Condizione_Valore1", title: TraduzioneMultiResx(resxFormProdottoUC, "CondSconto1Qta", "Qta Min 1"), format: "{0:n2}", attributes: { style: "text-align: right" }, hidden: true, width: 100 },
        { field: "Sconto_Add2", title: TraduzioneMultiResx(resxFormProdottoUC, "Sconto2Percentuale", "Sconto2 %"), format: "{0:n2}", attributes: { style: "text-align: right" }, hidden: true, width: 100 },
        { field: "Sconto_Condizione_Valore2", title: TraduzioneMultiResx(resxFormProdottoUC, "CondSconto2Qta", "Qta Min 2"), format: "{0:n2}", attributes: { style: "text-align: right" }, hidden: true, width: 100 },
        { field: "Sconto_Add3", title: TraduzioneMultiResx(resxFormProdottoUC, "Sconto2Percentuale", "Sconto3 %"), format: "{0:n2}", attributes: { style: "text-align: right" }, hidden: true, width: 100 },
        { field: "Sconto_Condizione_Valore3", title: TraduzioneMultiResx(resxFormProdottoUC, "CondSconto3Qta", "Qta Min 3"), format: "{0:n2}", attributes: { style: "text-align: right" }, hidden: true, width: 100 },
        { field: "Validita_Inizio", title: TraduzioneMultiResx(resxFormProdottoUC, "ValiditàInizio", "Validità inizio"), format: "{0:dd/MM/yyyy}", hidden: true, width: 100 },
        { field: "Validita_Fine", title: TraduzioneMultiResx(resxFormProdottoUC, "ValiditaFine", "Validita fine"), format: "{0:dd/MM/yyyy}", hidden: true, width: 100 }
    ];
    var parametriPerLettura = null;
    var parametriDataSource = {};
    var parametriKendoGrid = {
        salvaRipristinaPersonalizzazioni: { url: pathCoreWS },
        excel: true,
        pdf: false,
        reorderable: true,
        columnMenu: true,
        editable: false,
        colonneCustomKendoGrid: colonneCustomKendoGrid,
        pageable: { pageSizes: [5, 10, 20, 50, 100, "all"], buttonCount: 3 }
    };
    var funzioniPrimaDopoEventi = { };
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

function onDataBoundGrigliaPrezziListini(e) {
    var gridId = e.sender.element[0].id;
    var grid = $("#" + gridId).data("kendoGrid");
    for (var i = 0; i < grid.columns.length; i++) {
        grid.autoFitColumn(i);
    }
}

function SelezionaPrezzoListino(e) {
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    // kendo.alert(dataItem.Prezzo);
    ImpostaPrezzoListino(dataItem);
    $("#windowRiepilogoPrezziListini").data("kendoWindow").close();
}

function ImpostaPrezzoListino(listino) {
    Set_KendoNumTBValue("idPrezzo", listino.Prezzo);
    Set_KendoNumTBValue("idScontoAddiz1", listino.Sconto_Add1);
    Set_KendoNumTBValue("idScontoAddiz2", listino.Sconto_Add2);
    Set_KendoNumTBValue("idScontoAddiz3", listino.Sconto_Add3);
    AggiornaDettagliEconomici();
}

// Gestione Confezionamento Lotto

function RiempiElencoConfezionamentoLotto(options) {

    options.success(elencoConfezionamentoLotto);

}

function RicercaConfezionamentoLotto_Async(flagVuoto,piva) {

    return new Promise(function (resolve, reject) {

        let risp = [];

        let param = kendo.stringify({
            piva: piva
        });

        ajaxAgronica(indirizzoHttp_DocContabile_WS + "/RicercaConfezionamentoLotto",
            param,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = { piva: "", sa_cod: 0, cod_articolo: "", mat_des: "" };
                    risp.unshift(objVuoto);
                }
                resolve(risp);
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS RicercaConfezionamentoLotto");
                reject(new Error(msgError));
            },
            null,
            false);
        
    });

}



function LeggiParametriIndiciSalvati() {

    var valori = "";
    let w_Id_Agenda = 0;
    let w_Id_Mov_Det = 0;
    let w_KeyDet = $('input[name$="hf_key_mov_dett"]').val();
    if (w_KeyDet !== "") {
        w_Id_Agenda = parseInt(w_KeyDet.split("_")[2]);
        w_Id_Mov_Det = parseInt(w_KeyDet.split("_")[4]);
    }

    var param = kendo.stringify({ piva: $(cIdPiva).val(), id_agenda: w_Id_Agenda, id_mov_det: w_Id_Mov_Det });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/LeggiParametriIndiciSalvati",
        param, false,
        function (risposta) {
            valori = JSON.parse(risposta.RispostaStringa);
            
        }, null);

    return valori;

}
