
function DocContabileRicercaMovimenti(indirizzohttp, ID_Controllo, piva, idAgenda, lav_cod, options) {

    if (parseInt($(cIdAgenda).val()) !== 0) {
        var param = kendo.stringify({ piva: piva, id_agenda: idAgenda, lav_cod: lav_cod });
        var risp = "";

        ajaxAgronicaSync(indirizzohttp + "/RicercaRigheDoc",
            param, false,
            function (risposta) {

                statoEvasioneDoc = JSON.parse(risposta.ParametroDue_stringa);
                MostraStatoEvasioneOrdine();

                if (ID_Controllo !== null) {
                    $('input[name$="hdKendo_RigheDoc"]').val(risposta.RispostaStringa);
                    popolaDocContabileRighe(ID_Controllo);
                    if ($("#" + ID_Controllo).data("kendoGrid") !== undefined) {
                        maxOrdineDet = $("#" + ID_Controllo).data("kendoGrid").dataSource.aggregates().Ordine_Det.max;
                    } else {
                        maxOrdineDet = 0;
                    }
                    impostaRiepilogoPesiUC(ID_Controllo);
                }
                //////else {

                //////    // Aggiorno solo i dati senza ricreare la griglia
                //////    var data = risposta.RispostaStringa;
                //////    var jSonParsed_Kendo = JSON.parse(data);
                //////    options.success(jSonParsed_Kendo.kendo_rows);

                //////    // Aggiorno i dati memorizzati in precedenza
                //////    var data_Old = $('input[name$="hdKendo_RigheDoc"]').val();
                //////    var jSonParsed_Kendo_old = JSON.parse(data_Old);

                //////    jSonParsed_Kendo_old.kendo_rows = jSonParsed_Kendo.kendo_rows;

                //////    $('input[name$="hdKendo_RigheDoc"]').val(JSON.stringify(jSonParsed_Kendo_old));
                //////}
                    
            }, null);
    }
}

function Elimina_Riga_Doc(Piva, Id_Agenda, Id_Mov, Id_Mov_Det, Lav_Cod, Elem_Cod, IgnoraAvvisoWarning, Data_Movimento) {

    let w_modulo_anagrafe_log = moduloFromElemCod(Elem_Cod);

    if (Data_Movimento == undefined || Data_Movimento == null) {
        Data_Movimento = AGRODATAINIZIO
    }

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        Id_Agenda: Id_Agenda,
        Id_Mov: Id_Mov,
        Id_Mov_Det: Id_Mov_Det,
        Lav_Cod: Lav_Cod,
        Tipo_Operazione: enum_TipoOperazioneDB.Cancellazione.value,
        IgnoraAvvisoWarning: IgnoraAvvisoWarning,
        ModuloGiasLicenziato: w_modulo_anagrafe_log,
        flagAggiornaConteggi: true,
        dataMov: Data_Movimento
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/VerificaEliminaModificaDocumento",
        param,
        false,
        function (risposta) {

            console.log("Ho cancellato la riga!");

            let risp = JSON.parse(risposta.RispostaStringa);   

            //MessaggioTuttoOK_Bootstrap("Cancellazione riga effettuata correttamente", "DIV_Messaggi");
            $("<div></div>").kendoAlert({ title: TraduzioneMultiResx(resxContabileDettagliUC, "Cancellazione", "Cancellazione"), content: risp.RispostaStringa }).data("kendoAlert").open();

            //TODO: ricaricare anche griglia confezionamenti + setta i totali

            let testataDoc = getDatiContabTestata();
            testataDoc.DataOraUltimaLettura = kendo.parseDate(risp.Time);

            if (risp.ConteggiTotali !== undefined && risp.ConteggiTotali !== null) {

                testataDoc.Colli = parseInt(risp.ConteggiTotali.Colli);
                testataDoc.TipoPeso = 0; // fisso a peso lordo
                testataDoc.Peso = kendo.parseFloat(risp.ConteggiTotali.PesoLordoDoc);
                testataDoc.PesoNettoProd = kendo.parseFloat(risp.ConteggiTotali.PesoNettoProd);
                testataDoc.TaraImballi = kendo.parseFloat(risp.ConteggiTotali.TaraImballi);

                testataDoc.ImballiVuoti = kendo.parseFloat(risp.ConteggiTotali.ImballiVuoti);

                //In teoria tara veicolo è quella che avevo già scritto, quindi non ci sarebbe bisogno di sovrascriverla
                testataDoc.TaraVeicolo = kendo.parseFloat(risp.ConteggiTotali.TaraVeicolo);
                testataDoc.PesoTaraTrasporto = kendo.parseFloat(risp.ConteggiTotali.TaraVeicolo);

                Set_KendoNumTBValue("inNumeroColli", testataDoc.Colli);
            }

            $('input[name$="hdKendo_TestataDoc"]').val(kendo.stringify(testataDoc));

            //ricarico
            DocContabileRicercaMovimenti(indirizzoHttp_DocContabile_WS, "tab_elenco_movimenti", $(cIdPiva).val(), $(cIdAgenda).val(), cIdLavCod);

            if (risp.ConteggiTotali !== undefined && risp.ConteggiTotali !== null) {

                //Marco: Aggiornamento Riepiloghi Castelletto
                let riepilogoImporti = risp.ConteggiTotali.RiepilogoImporti;

                Set_KendoNumTBValue("idImponibileTotaleLordo", riepilogoImporti.ImponibileLordo);
                Set_KendoNumTBValue("idVariazione", riepilogoImporti.Variazioni);
                Set_KendoNumTBValue("idImponibileNetto", riepilogoImporti.ImponibileNetto);
                Set_KendoNumTBValue("idImposta", riepilogoImporti.Imposta);
                Set_KendoNumTBValue("idTotale", riepilogoImporti.TotaleDocumento);

                //Rileggo il datasource della griglia Operazioni/Attivita
                let gridCasteletto = $("#tab_griglia_castelletto").data("kendoGrid");
                if (gridCasteletto !== undefined) {
                    //Utilizzo il setDatasource per aggiornare i filtri della colonna se precedentemente erano
                    //stati selezionati.

                    let ds = new kendo.data.DataSource({ data: JSON.parse(risp.ConteggiTotali.RiepilogoCastelletto) });
                    gridCasteletto.setDataSource(ds);
                    gridCasteletto.dataSource.read();

                }

            }

            //Ricarico griglia per confezionamenti collegati e mostro TAB Beni confezionamento
            if (isGestitoTabConfezionamento()) {
                ConfiguraGrigliaCaricoEredita("tab_griglia_carico_eredita");
                $("#a_tabBeniConfezionamento").show();
            } else {
                $("#a_tabBeniConfezionamento").hide();
            }

        }, function (risposta) {

            if (risposta.RispostaConferma === true) {

                let risp = JSON.parse(risposta.RispostaStringa);   

                $("#confermaEliminazioneRigaDialog").kendoDialog({
                    width: "400px",
                    title: TraduzioneMultiResx(resxContabileDettagliUC, "GestioneCancellazioneDocumenti", "Gestione Cancellazione Documenti"),
                    closable: false,
                    modal: true,
                    visible: false,
                    content: "<p>" + risp.RispostaStringa + "<p>",
                    actions: [
                        { text: TraduzioneMultiResx(resxContabileDettagliUC, "Conferma", "Conferma"), action: function (e) { Elimina_Riga_Doc(Piva, Id_Agenda, Id_Mov, Id_Mov_Det, Lav_Cod, Elem_Cod, true); } },
                        { text: TraduzioneMultiResx(resxContabileDettagliUC, "Annulla", "Annulla"), primary: true }
                    ]
                });
                $("#confermaEliminazioneRigaDialog").data("kendoDialog").open();
            } else {
                kendo.alert(TraduzioneMultiResx(resxContabileDettagliUC, "ErroreDuePunti_", "Errore: ") + risposta.Errore);
            }
        }
    );

}

function Elimina_Intero_Doc(Id_Agenda, Lav_Cod, IgnoraAvvisoWarning, Data_Movimento) {

    if (Data_Movimento == undefined || Data_Movimento == null) {
        Data_Movimento = AGRODATAINIZIO;
    }

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        Id_Agenda: Id_Agenda,
        Id_Mov: 0,
        Id_Mov_Det: 0,
        Lav_Cod: Lav_Cod,
        Tipo_Operazione: enum_TipoOperazioneDB.Cancellazione.value,
        IgnoraAvvisoWarning: IgnoraAvvisoWarning,
        ModuloGiasLicenziato: 0,
        flagAggiornaConteggi: false,
        dataMov: Data_Movimento
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/VerificaEliminaModificaDocumento",
        param,
        false,
        function (risposta) {

            let risp = JSON.parse(risposta.RispostaStringa);    
            //sono costretta ad usare l'alert standard perché quello kendo non blocca la pagina, e sparisce subito perché c'è il redirect
            //$("<div></div>").kendoAlert({ title: "Cancellazione", content: risposta.RispostaStringa }).data("kendoAlert").open();
            alert(risp.RispostaStringa);

            if ($(hf_UrlPostDelete).val() !== "")
                window.location.href = $(hf_UrlPostDelete).val();

        }, function (risposta) {

            if (risposta.RispostaConferma === true) {

                let risp = JSON.parse(risposta.RispostaStringa);    

                $("#confermaEliminazioneRigaDialog").kendoDialog({
                    width: "400px",
                    title: TraduzioneMultiResx(resxContabileDettagliUC, "GestioneCancellazioneDocumenti", "Gestione Cancellazione Documenti"),
                    closable: false,
                    modal: true,
                    visible: false,
                    content: "<p>" + risp.RispostaStringa + "<p>",
                    actions: [
                        { text: TraduzioneMultiResx(resxContabileDettagliUC, "Conferma", "Conferma"), action: function (e) { Elimina_Intero_Doc(Id_Agenda, Lav_Cod, true); } },
                        { text: TraduzioneMultiResx(resxContabileDettagliUC, "Annulla", "Annulla"), primary: true }
                    ]
                });
                $("#confermaEliminazioneRigaDialog").data("kendoDialog").open();
            } else {
                kendo.alert(TraduzioneMultiResx(resxContabileDettagliUC, "ErroreDuePunti_", "Errore: ") + risposta.Errore);
            }

        });
}

function Stampa_BarCode(dataItem, dettaglio) {

    var Mat_Cod = "";
    var Cal_Cod = "";
    var Lotto = "";
    if (dettaglio) {
        Mat_Cod = dataItem.Mat_Cod != undefined ? dataItem.Mat_Cod : "";
        Cal_Cod = dataItem.Cal_Cod != undefined ? dataItem.Cal_Cod : "";
        Lotto = dataItem.Lotto != undefined ? dataItem.Lotto : "";
    }

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        Id_Agenda: dataItem.Id_Agenda,
        Lav_Cod: dataItem.Lav_Cod,
        Mat_Cod: Mat_Cod,
        Cal_Cod: Cal_Cod,
        Lotto: Lotto,
        Modulo: dataItem.Modulo,
        Tipo_Accettazione: dataItem.Tipo_Accettazione,
        Dettaglio: dettaglio
    });
    var risp = "";

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/StampaBarCode",
        param,
        false,
        function (risposta) {
            risp = risposta.RispostaStringa;
            var win = window.open(risp);
            win.focus();
        }, function (rispostaErrore) {
            MessaggioErrore_Bootstrap(rispostaErrore.Errore, "DIV_Messaggi");
        });
}



function WS_LeggiListaCdgProgetti(options) {
    try {
        var analisi = "";

        if (cIdLavCod === enum_LavCod.Ordine_Acquisto.value) {
            analisi = "|1|";
        }
        else if (cIdLavCod === enum_LavCod.DDT_Emesso.value || cIdLavCod === enum_LavCod.DDT_Contabilizzato_Emesso.value) {
            let causaleTrasporto = Get_KendoDDLValue("inCausaleTrasporto", 0);
            if (parseInt(causaleTrasporto) === 15) {
                //sono in un ddt emesso di reso e quindi devo proporre i progetti come se fossi in un ordine di acquisto
                analisi = "|1|";
            } else {
                analisi = "|2|";
            }
        }
        let resp = LeggiListaCdgProgetti(true,$(cIdPiva).val(), analisi);
        options.success(resp);
    } catch (err) {
        let msgErr = TraduzioneMultiResx(resxContabileDettagliUC, "ErroreDuePunti_", "Errore: ") + err;
        console.error(msgErr);
        options.error(msgErr);
    }
}



function LeggiListaCdgProgetti(flagVuoto, piva , analisi) {

    var elencoCdgProgetti = null;

    var param = kendo.stringify({
        piva: piva,
        analisi: analisi

    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/LeggiListaCdgProgetti",
        param,
        false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                var objVuoto = {
                    "Imputazione_Nome": "",
                    "Imputazione_Cod": ""
                };
                risp.unshift(objVuoto);
            }
            elencoCdgProgetti = risp;
        },
        function (risposta) {
            var msgError = mostraErrore(risposta, "Errore");
        },
        null,
        elencoCdgProgetti);

    return elencoCdgProgetti;
}



function WS_AggionaCDGImputazioniDocContabile(piva, saCod, idAgenda, idMov, idMovDet, lavCod, dataEmissione, descLib, imputazione_Cod, odaOddt) {
    var msgErrCatch = "";
    var msgError = "";
    try {
        var msgRisposta = null;
        var param = kendo.stringify({
            piva: piva,
            saCod: saCod,
            idAgenda: idAgenda,
            idMov: idMov,
            idMovDet: idMovDet,
            lavCod: lavCod,
            dataDoc: dataEmissione,
            descLib: descLib,
            imputazione_Cod: imputazione_Cod,
            odaOddt: odaOddt
        });

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/AggiornaCDGImputazioniDocContabile",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                msgRisposta = risp;
            },
            function (risposta) {
                 msgError = mostraErrore(risposta, "Errore");
            },
            null,
            msgRisposta);

        //options.success(resp);
    } catch (err) {
        msgErrCatch = TraduzioneMultiResx(resxContabileDettagliUC, "ErroreDuePunti_", "Errore: ") + err;
        console.error(msgErr);
        //options.error(msgErr);
    }

    msgError = msgErrCatch + msgError;

    return msgError;
}