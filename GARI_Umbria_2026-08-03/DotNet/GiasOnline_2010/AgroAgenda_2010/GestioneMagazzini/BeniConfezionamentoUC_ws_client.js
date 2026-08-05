

//////////////////////////////////////////////////////////
// Griglia Imputazione
//////////////////////////////////////////////////////////

function EmptyRead(options) { }
function EmptySubmit(options) { }

function CaricaGrigliaCarico(options) {

    if (parseInt($(cIdAgenda).val()) !== 0 && parseInt($(cId_Mov_BC_Principale).val()) !== 0) {

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/CaricaGrigliaBeniConfezionamento",
            kendo.stringify({
                piva: $(cIdPiva).val(),
                cau_mov: $(cCau_Mov_BC_Principale).val(),
                id_agenda: parseInt($(cIdAgenda).val()),
                id_mov: parseInt($(cId_Mov_BC_Principale).val()),
                filtro_aggiuntivo: "Movimenti_Dettagli.Ordine_Det = 30000",
                modulo_generazione: 2
            }),
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                options.success(risp);

                //Impostazione Magazzino
                if (risp.length > 0) {
                    var chiave = risp[0].Sa_Cod + "|" + risp[0].Id_Destinazione;
                    Set_KendoDDLValue("cmbDestinazione", chiave);
                }

            }, null);
    }
}


function CaricaGrigliaCaricoEredita(options) {

    if (parseInt($(cIdAgenda).val()) !== 0 && parseInt($(cId_Mov_BC_Principale).val()) !== 0) {

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/CaricaGrigliaBeniConfezionamento",
            kendo.stringify({
                piva: $(cIdPiva).val(),
                cau_mov: $(cCau_Mov_BC_Principale).val(),
                id_agenda: parseInt($(cIdAgenda).val()),
                id_mov: parseInt($(cId_Mov_BC_Principale).val()),
                filtro_aggiuntivo: "Movimenti_Dettagli.Ordine_Det = 1000",
                modulo_generazione: 2
            }),
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                options.success(risp);
            }, null);

    }

}



function CaricaGrigliaScarico(options) {

    if (parseInt($(cId_Agenda_Scarico).val()) !== 0 && parseInt($(cId_Mov_Scarico).val()) !== 0) {

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/CaricaGrigliaBeniConfezionamento",
            kendo.stringify({
                piva: $(cPiva_Scarico).val(),
                cau_mov: CAU_SCARICO,
                id_agenda: parseInt($(cId_Agenda_Scarico).val()),
                id_mov: parseInt($(cId_Mov_Scarico).val()),
                filtro_aggiuntivo: "",
                modulo_generazione: 2
            }),
            false,
            function(risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                options.success(risp);

                ImpostaTipoOpTestataScarico();

                //Impostazione Magazzino
                if (risp.length > 0) {

                    var chiave = risp[0].Sa_Cod + "|" + risp[0].Id_Destinazione;

                    Set_KendoDDLValue("cmbProvenienza", chiave);

                    $("#inNumDocSinScarico").val(risp[0].Doc_Numero_Sin);
                    Set_KendoNumTBValue("inNumDocScarico", risp[0].Doc_Numero);
                    $("#inNumDocDesScarico").val(risp[0].Doc_Numero_Des);

                    $("#inNumDocShowScarico").val(risp[0].Doc_Numero_Visualizzato);

                } else {
                    Set_KendoNumTBValue("inNumDocScarico", 0);  //se avevo una riga e poi la cancello (quindi ho eliminato il doc), devo ri-azzerare il numero
                    KendoNumTB("inNumDocScarico").trigger("change");
                }

            },
            null);

    } else {
        ImpostaTipoOpTestataScarico();
        Set_KendoNumTBValue("inNumDocScarico", 0);  //se avevo una riga e poi la cancello (quindi ho eliminato il doc), devo ri-azzerare il numero
        KendoNumTB("inNumDocScarico").trigger("change");
    }
    
        
}




////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////// FUNZIONI CONTROLLO E RESTORE //////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Controllo campi obbligatori
function controllaRigheValidePerSubmitGrid(righe) {

    var nrErr = 0;

    for (let x = 0; x < righe.length; x++) {

        var item = righe[x];

        if (item.Mat_Cod === 0 || item.Qta === 0) {
            nrErr++;
        }
    }

    return nrErr;
}



////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////     RIEMPIMENTO ELENCHI SU RICHIESTA   /////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

function Elenco_Beni_Confezionamento_Riempi(flagVuoto, tipo) {

    var risultato_lettura;
        
    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Leggi_Beni_Confezionamento",
        kendo.stringify({ piva: $(cIdPiva).val(), tipo: tipo, modulo_generazione: 2}),
        false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                var objVuoto = {
                    Elem_Cod: 0,
                    Mat_Cod: 0,
                    Mat_Cod_Scarico: 0,
                    Mat_Des: "",
                    Cod_Articolo: "",
                    Tara: 0,
                    Tipo_BC: 0,
                    Tipo_Des_BC: "",
                    Giacenza: 0,
                    Qta_Extr: 0
                };
                risp.unshift(objVuoto);
            }
            risultato_lettura = risp;
        }, null);
       
    return risultato_lettura;
}



function Leggi_Giacenza(Sa_Cod, Fabbricato_Cod, Mat_Cod) {

    var risultato_lettura = 0;

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        objP_super_server: objP_super_server,
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        tipo_aggregazione: 1,
        soloCampiApp: false,
        sa_cod: Sa_Cod,
        tipo_fabbricato_cod: MAGAZZINO,
        fabbricato_cod: Fabbricato_Cod,
        elem_cod: BENI_CONFEZ_VEGETALE,
        pro_cod: 0,
        mat_cod: Mat_Cod,
        lotto: "",
        Data_Movimento_Str: get_data("inDataEmissione"),
        isFreshAndFood: false,
        flag_QtaNoZero: false
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Giacenze + "/Leggi_Giacenze",
        param,
        false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);

            if (risp.length > 0) {
                risultato_lettura = risp[0].Giacenza;
            }
        }, null);

    return risultato_lettura;
}

    


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////// SUBMIT GENERALE  /////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//eventi di click pulsanti
function AggiornaEffettivo(flagEsci) {

    var allOk = false;

    cmbProvenienza_change();
    cmbDestinazione_change();

    Imposta_Numero_Colli();

    //Numerazione carico
    let docNumeroCarico = Get_KendoNumTBValue("inNumDoc");
    let docNumeroLunghezzaCarico = 0;
    let docNumeroCarattereFormattazioneCarico = "";
    let docNumeroLockCarico = getKendoSwitch("inNumDocLock");
    if (KendoDDL("inNumDocDDL") !== undefined &&
        KendoDDL("inNumDocDDL").dataItem() !== undefined &&
        KendoDDL("inNumDocDDL").dataItem() !== null) {

        //se è lockato, non mi passo neanche le informazioni di formattazione perché non mi servirebbero (e potrebbero essere diverse da quelle usate precedentemente)
        // a meno che non debba assegnarlo io lato server
        if (docNumeroCarico === 0 || (docNumeroCarico > 0 && docNumeroLockCarico === false)) {
            docNumeroLunghezzaCarico = parseInt(KendoDDL("inNumDocDDL").dataItem().Lunghezza_Centro);
            docNumeroCarattereFormattazioneCarico = KendoDDL("inNumDocDDL").dataItem().CarattereFormattazione;
        }

    }

    //Numerazione scarico
    let docNumeroScarico = Get_KendoNumTBValue("inNumDocScarico");
    let docNumeroLunghezzaScarico = 0;
    let docNumeroCarattereFormattazioneScarico = "";
    let docNumeroLockScarico = true; //getKendoSwitch("inNumDocLockScarico");
    if (KendoDDL("inNumDocDDLScarico") !== undefined &&
        KendoDDL("inNumDocDDLScarico").dataItem() !== undefined &&
        KendoDDL("inNumDocDDLScarico").dataItem() !== null) {

        //se è lockato, non mi passo neanche le informazioni di formattazione perché non mi servirebbero (e potrebbero essere diverse da quelle usate precedentemente)
        // a meno che non debba assegnarlo io lato server
        if (docNumeroScarico === 0 || (docNumeroScarico > 0 && docNumeroLockScarico === false)) {
            docNumeroLunghezzaScarico = parseInt(KendoDDL("inNumDocDDLScarico").dataItem().Lunghezza_Centro);
            docNumeroCarattereFormattazioneScarico = KendoDDL("inNumDocDDLScarico").dataItem().CarattereFormattazione;
        }

    }

    var param = kendo.stringify({
        piva_carico: $(cIdPiva).val(),
        id_agenda_carico: parseInt($(cIdAgenda).val()),
        id_mov_testata_carico: parseInt($(cId_Mov_Testata).val()),
        id_mov_carico: parseInt($(cId_Mov_BC_Principale).val()),
        lav_cod_carico: cIdLavCod,
        rag_soc_carico: KendoDDL(ddlContatto1Nome).dataItem().Rag_Soc,
        docnumerosin_carico: $("#inNumDocSin").val().toUpperCase(),
        docnumero_carico: docNumeroCarico,
        docnumero_lunghezza_carico: docNumeroLunghezzaCarico,
        docnumero_carattere_formattazione_carico: docNumeroCarattereFormattazioneCarico,
        docnumerodes_carico: $("#inNumDocDes").val().toUpperCase(),
        docnumero_visualizzato_carico: $("#inNumDocShow").val().toUpperCase(),
        docnumero_lock_carico: docNumeroLockCarico,
        sa_cod_carico: Sa_Cod_Carico,
        fabbricato_cod_carico: Fabbricato_Cod_Carico,
        numero_colli_carico: Numero_Colli_Carico,
        cod_risum_carico: parseInt(Get_KendoDDLValue(ddlContatto1Nome)),
        piva_scarico: $(cPiva_Scarico).val(),
        id_agenda_scarico: $(cId_Agenda_Scarico).val(),
        id_mov_testata_scarico: $(cId_Mov_Testata_Scarico).val(),
        id_mov_scarico: $(cId_Mov_Scarico).val(),
        lav_cod_scarico: $(cLav_Cod_Scarico).val(),
        rag_soc_scarico: KendoDDL(ddlContatto1Nome).dataItem().Rag_Soc,
        docnumerosin_scarico: $("#inNumDocSinScarico").val().toUpperCase(),
        docnumero_scarico: docNumeroScarico,
        docnumero_lunghezza_scarico: docNumeroLunghezzaScarico,
        docnumero_carattere_formattazione_scarico: docNumeroCarattereFormattazioneScarico,
        docnumerodes_scarico: $("#inNumDocDesScarico").val().toUpperCase(),
        docnumero_visualizzato_scarico: $("#inNumDocShowScarico").val().toUpperCase(),
        docnumero_lock_scarico: docNumeroLockScarico,
        sa_cod_scarico: Sa_Cod_Scarico,
        fabbricato_cod_scarico: Fabbricato_Cod_Scarico,
        numero_colli_scarico: Numero_Colli_Scarico,
        cod_risum_scarico: parseInt(Get_KendoDDLValue(ddlContatto1Nome)),
        data_movimento: $('input[name$="inDataEmissione"]').val(),
        data_registrazione: get_data("inDataRegistrazione"),
        cod_indirizzo_scarico: parseInt(Get_KendoDDLValue("inTipoIndirizzoCC1")),
        righeInseriteGrid_Carico: righeInseriteGrid_Carico,
        righeModificateGrid_Carico: righeModificateGrid_Carico,
        righeCancellateGrid_Carico: righeCancellateGrid_Carico,
        righeInseriteGrid_Scarico: righeInseriteGrid_Scarico,
        righeModificateGrid_Scarico: righeModificateGrid_Scarico,
        righeCancellateGrid_Scarico: righeCancellateGrid_Scarico
    });


    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/AggiornaBeniConfezionamento",
        param, false,
        function (risposta) {

            //TODO: in questo momento non sto tenendo conto della data ultima modifica di agenda/mov_dettaglio

            //Impostazione Chiavi
            var chiavi = risposta.RispostaStringa;
            var elemChiavi = chiavi.split("§");
            
            $(cIdAgenda).val(parseInt(elemChiavi[0]));
            $(cId_Mov_Testata).val(parseInt(elemChiavi[1]));
            $(cId_Mov_BC_Principale).val(parseInt(elemChiavi[2]));

            var conteggiCarico = JSON.parse(elemChiavi[3]);

            $(cId_Agenda_Scarico).val(parseInt(elemChiavi[4]));
            $(cId_Mov_Testata_Scarico).val(parseInt(elemChiavi[5]));
            $(cId_Mov_Scarico).val(parseInt(elemChiavi[6]));

            var conteggiScarico = JSON.parse(elemChiavi[7]);    //in realtà non mi serve

            if (flagEsci !== true) {

                if (conteggiCarico !== undefined && conteggiCarico !== null) {

                    console.debug(conteggiCarico);

                    let testataDoc = getDatiContabTestata();

                    testataDoc.Colli = parseInt(conteggiCarico.Colli);
                    testataDoc.TipoPeso = 0;  // fisso a peso lordo
                    testataDoc.Peso = kendo.parseFloat(conteggiCarico.PesoLordoDoc);
                    testataDoc.PesoNettoProd = kendo.parseFloat(conteggiCarico.PesoNettoProd);
                    testataDoc.TaraImballi = kendo.parseFloat(conteggiCarico.TaraImballi);

                    testataDoc.ImballiVuoti = kendo.parseFloat(conteggiCarico.ImballiVuoti);

                    //In teoria tara veicolo è quella che avevo già scritto, quindi non ci sarebbe bisogno di sovrascriverla
                    testataDoc.TaraVeicolo = kendo.parseFloat(conteggiCarico.TaraVeicolo);
                    testataDoc.PesoTaraTrasporto = kendo.parseFloat(conteggiCarico.TaraVeicolo);

                    Set_KendoNumTBValue("inNumeroColli", testataDoc.Colli);

                    $('input[name$="hdKendo_TestataDoc"]').val(kendo.stringify(testataDoc));

                    //aggiornamento interfaccia
                    impostaRiepilogoPesiUC("tab_elenco_movimenti");

                    let msgRiepilogoPesi = VerificaCongruenzaPesi();

                    if (msgRiepilogoPesi !== "")
                        alert(msgRiepilogoPesi);
                }

                //TODO: devo fare lo l'aggiornamento del riepilogo pesi

                //reload                
                ConfiguraGrigliaCarico("tab_griglia_carico");
                //  Agg. Eredita non dovrebbe servire in quanto richiamato ogni volta quando si salva la riga di dettagli
                //  In più dà errore quando è appena stato eseguito dall'aggiornamento automatico pesi  
                //     ConfiguraGrigliaCaricoEredita("tab_griglia_carico_eredita");
                ConfiguraGrigliaScarico("tab_griglia_scarico");

                BloccaMagazzinoCarico();
                BloccaMagazzinoScarico("#tab_griglia_scarico");
            }

            //Visto che ogni tanto sparisce, ma qui sono già dentro la tab, quindi se sono riuscita a fare una modifica
            //doveva per forza essere già visibile, allora forzo la sua visibilità
            $("#a_tabBeniConfezionamento").show();
            allOk = true;

        },
        function (risposta) {
            if (risposta !== undefined && risposta !== null && risposta.RispostaOK === false) {
                MessaggioErrore_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
            } else {
                MessaggioErrore_Bootstrap(TraduzioneMultiResx(resxObj, "ErroreSalvataggio", "Errore salvataggio"), "DIV_Messaggi");
            }
        });
      

    //    if (!allOk) {
    //    erroreSubmit(grid_carico);
    //    erroreSubmit(grid_scarico);
    //}
    //else {
    //    MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
    //}
}

function GetNumeratoriScarico(options) {

    let piva = $(cPiva_Scarico).val();
    let saCod = Sa_Cod_Scarico;
    let lavCod = $(cLav_Cod_Scarico).val();
    let dataDocumento = KendoDate("inDataEmissione").value();
    let fatturaAccompagnatoria = false;
    let flagVuoto = false;

    let risp = null;
    let elencoNumeratoriScarico = [];
    //let ret = RicercaNumeratoriPiuDefaults(piva, saCod, lavCod, dataDocumento, fatturaAccompagnatoria);

    //if (elencoNumeratoriDefaults === null) {

        var param = kendo.stringify({
            piva: piva,
            data_Documento: dataDocumento,
            sa_Cod: saCod,
            lav_Cod: lavCod,
            fattura_Accompagnatoria: fatturaAccompagnatoria
        });

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Numeratori_E_Defaults",
            param,
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                //elencoNumeratoriDefaults = risp;
                console.debug(risp);
            },
            function (risposta) {
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            });

    //return elencoNumeratoriDefaults;






    if (risp !== undefined &&
        risp !== null &&
        risp.Numeratori !== undefined &&
        risp.Numeratori !== null &&
        risp.Numeratori.length > 0) {

        elencoNumeratoriScarico = risp.Numeratori;

        //TODO: sono in modifica ==> potrebbe essere stata modificata la tabella di configurazione, quindi nell'elenco non ho più il prefisso/suffisso che era stato usato

    } else {

        elencoNumeratoriScarico = [];
        //flagVuoto = true;

        //devo aggiungere la voce vuota solo se sono in creazione e non ho ottenuto nessuna configurazione,
        // se sono in modifica e non ho configurazione, lascio la DDL vuota
        //if (cIdTipoOp === enum_TipoOperazioneDB.Scrittura.value &&
        //    gestioneContabilita !== enum_Livello_GestContabilita_NonGestita) {
        //    flagVuoto = true;
        //}

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
        elencoNumeratoriScarico.unshift(objVuoto);
    }

    options.success(elencoNumeratoriScarico);
}
