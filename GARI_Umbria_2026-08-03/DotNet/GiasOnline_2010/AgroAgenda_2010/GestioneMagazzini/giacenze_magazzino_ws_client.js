var giacenzeMagazzinoWSClientResx = [];

function TraduciGiacenzeMagazzinoWSClient(chiave, testoAlternativo) {
    if (giacenzeMagazzinoWSClientResx.length === 0) {
        giacenzeMagazzinoWSClientResx.push(readResxFile("GestioneMagazzini/App_LocalResources/Giacenze_Magazzino.aspx.resx", "giacenze_magazzino_ws_client.js"));
        giacenzeMagazzinoWSClientResx.push(readResxFile("App_GlobalResources/AgronicaAgenda_2010.resx", "giacenze_magazzino_ws_client.js"));
    }
    return TraduzioneMultiResx(giacenzeMagazzinoWSClientResx, chiave, testoAlternativo);
}

// Eccezione di jQueryDocReady messo qui per utilizzo caricamento remote filtering prodotto
$(document).ready(function () {
    var indirizzohttp_Leggi_Tabelle_WS = "FreshAndFood/FreshAndFood.asmx";
    var objVuoto = {
        "Mat_Cod": 0,
        "Mat_Des": "",
        "Elem_Cod": 0,
        "Linea_Cod": 0,
        "Veg_Cod": -1,
        "Cul_Cod": 0,
        "Veg_Des": "",
        "Cul_Des": "",
        "Cal_Cod": 0
    };
    $("#idIProdotto").kendoDropDownList({
        filter: "contains",
        dataTextField: "Mat_Des",
        dataValueField: "Mat_Cod",
        autoBind: false,
        minLength: 3,
        dataSource: {
            serverFiltering: true,
            transport: {

                read: function (options) {
                    
                    var param = kendo.stringify({
                        objP_server: objP_server, objP_utenti: objP_utenti, piva: $(cIdPiva).val(),
                        Mat_Cod: 0, Elem_Cod: 210, soloMovimentato: true, filters: JSON.stringify(options.data.filter.filters), soloLegatiALinea: true
                    });
                    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_WS + "/LeggiProdotti_FF",
                        param, false,
                        function (risposta) {
                            let risp = JSON.parse(risposta.RispostaStringa);
                            risp.unshift(objVuoto);
                            options.success(risp);
                        }, null);
                }
            }
        }
    });
});

function RicercaGiacenzeMagazzino(indirizzohttp, mostraCampiInput) {

    var idMat_Cod = 0;

    if (KendoDDL("idIProdotto").value() !== "")
        idMat_Cod = KendoDDL("idIProdotto").value();

    if (parseInt(Get_KendoDDLValue("ddlSpecie")) === -1 &&
        KendoMultisel("multiselVarieta").value().length === 0 &&
        parseInt(Get_KendoDDLValue("idCentro")) === 0 &&
        $('input[name$="txt_Lotto"]').val() === "" &&
        prendiParametroQualitativo("idCalibro").Param_ID === 0 &&
        prendiParametroQualitativo("idQualita").Param_ID === 0 &&
        prendiParametroQualitativo("idCertificazione").Param_ID === 0 &&
        prendiParametroQualitativo("idImballaggio").Param_ID === 0 &&
        prendiParametroQualitativo("idContenitore").Param_ID === 0 &&
        prendiParametroQualitativo("idConfezione").Param_ID === 0 &&
        idMat_Cod === 0) {

        MessaggioErrore_Bootstrap(TraduciGiacenzeMagazzinoWSClient("InserireUnParametroDiFiltro", "Inserire almeno un parametro di filtro"), "DIV_Messaggi");

    } else {

        var param = kendo.stringify({
            piva: $(cIdPiva).val(),
            _prodotto: idMat_Cod,
            _specie: parseInt(Get_KendoDDLValue("ddlSpecie")),
            _varieta: KendoMultisel("multiselVarieta").value().join("|"),
            _sa_cod: parseInt(Get_KendoDDLValue("idCentro")),
            _dataRif: $('input[name$="txt_DataRif_GiacenzeMagazzino"]').val(),
            _lotto: $('input[name$="txt_Lotto"]').val(),
            _calibro: prendiParametroQualitativo("idCalibro").Param_ID,
            _qualita: prendiParametroQualitativo("idQualita").Param_ID,
            _certificazione: prendiParametroQualitativo("idCertificazione").Param_ID,
            //_rugginosita: prendiParametroQualitativo("idRugginosita").Param_ID,
            _rugginosita: 0,
            _imballaggio: prendiParametroQualitativo("idImballaggio").Param_ID,
            _contenitore: prendiParametroQualitativo("idContenitore").Param_ID,
            _confezione: prendiParametroQualitativo("idConfezione").Param_ID,
            _chkGiacenzePositive: getKendoSwitch("chkGiacenzePositive"),
            _mostraCampiInput: mostraCampiInput,
            _calCodEsclusi: GetElencoCalCodCarichi(indirizzohttp)
        });

        ajaxAgronica(indirizzohttp + "/CaricaGrigliaGiacenze",
            param, 
            function (risposta) {
                $('input[name$="hdKendo_Giacenze"]').val(risposta.RispostaStringa);
                popolaGrigliaGiacenzeMagazzino("tab_giacenze", mostraCampiInput);
            }, null);

    }

}

function GetElencoCalCodCarichi(indirizzohttp) {

    var elencoCalCodCarichi = null;

    if (indirizzohttp === "./Gestione_Lavorazioni.aspx" && modalita_trasferimento === false) {

        var gridCarichi = $("#tab_elenco_carichi").data("kendoGrid");

        const righeCarichi = getRigheCarichi(gridCarichi);

        if (righeCarichi.length > 0) {

            for (let iRiga = 0; iRiga < righeCarichi.length; iRiga++) {

                let calCodCarico = righeCarichi[iRiga].Cal_Cod;

                if (isNaN(calCodCarico)) continue;

                if (calCodCarico !== "0") {

                    if (elencoCalCodCarichi === null) {
                        elencoCalCodCarichi = calCodCarico;
                    } else {
                        elencoCalCodCarichi = elencoCalCodCarichi + "," + calCodCarico;
                    }

                }

            }

        }

    }

    return elencoCalCodCarichi;

}

function RiempiSpecie(options) {
    var elencoSpecie = RicercaSpecie($(cIdPiva).val());
    options.success(elencoSpecie);
}

function RiempiVarieta(options) {
    var multisel = KendoMultisel("multiselVarieta");
    var vegCod = multisel.dataSource.options.transport.data.Veg_Cod;
    var elencoVarieta = RicercaVarieta($(cIdPiva).val(), vegCod);
    options.success(elencoVarieta);
}

function LeggiCentri(options) {
    try {
        let tipoValue = 2; // Tipo_Value = 2 ---> ex CaricaCombo_CentriAziendali2

        let resp = RicercaCentriAziendali($(cIdPiva).val(), false, tipoValue, false);
        options.success(resp);

    } catch (err) {
        let msgErr = TraduciGiacenzeMagazzinoWSClient("ErroreInLetturaCentri_", "Errore in lettura centri: ") + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

var modalita_trasferimento = false;

function SubmitInserisciNuoviScarichi(options) {

    var gridGiacenze = $("#tab_giacenze").data("kendoGrid");
    var gridScarichi = $("#tab_elenco_scarichi").data("kendoGrid");
    var allOk = true;
    var rileggiScarichi = true;

    // controllo che tutte le righe siano complete
    var errMessage = controllaRigheInserisciNuoviScarichiPerSubmit(options.data.models);

    // controllo eventuali criteri di aggregazione
    if (errMessage === "" && modalita_trasferimento === false) {
        errMessage = controllaCriteriAggregazioneNuoviScarichi(options.data.models);
    }

    // controllo righe uscita spedite se criteri aggregazione
    if (errMessage === "" && modalita_trasferimento === false) {
        errMessage = controllaUsciteSpediteCriteriAggregazione();
    }

    if (errMessage !== "") {

        $("<div></div>").kendoAlert({
            title: TraduciGiacenzeMagazzinoWSClient("OperazioneNonConsentita", "Operazione non consentita"),
            content: errMessage
        }).data("kendoAlert").open();

        gridGiacenze.cancelChanges();

    } else {

        //TODO: devo distinguere se sono in trasferimento devo fare un oggetto diverso
        //(per evitare danni mi conviene rinominare le proprietà dell'oggetto vb in accordo con quelle dichiarate qui, così poi lo posso deserializzare direttamente )

        var righeModificate = kendoEscapeOggetto(options.data.models);

        if (modalita_trasferimento === false) {
            var totaleQtaPrec = getTotaleQtaScarichiPrec();
        }

        for (var i = 0; i < options.data.models.length; i++) {

            var riga = options.data.models[i];

            if (modalita_trasferimento === true) {

                var trasferimento = CreaOggettoInserisciTrasferimentoScarico(riga, true);

                var trasf = kendoEscapeOggetto(trasferimento);

                let ris = InserisciScaricoTrasferimento($(cIdPiva).val(), $(cIdAgenda).val(),
                                                        $(cIdMovScarico).val(), $(cIdMovCarico).val(),
                                                        trasf, true);

                if (ris !== undefined && ris !== null && ris.RispostaOK === true) {

                    MessaggioTuttoOK_Bootstrap(TraduciGiacenzeMagazzinoWSClient("InserimentoEffettuatoCorrettamente", "Inserimento effettuato correttamente"), "DIV_Messaggi");

                    if (ris.RispostaStringa !== "") {
                        let rispServer = JSON.parse(ris.RispostaStringa);
                        console.debug(rispServer);

                        //se è la prima riga che ho scritto devo anche riportarmi indietro l'id_agenda e gli id_mov 
                        if (rispServer.TestataSalvata === true) {
                            $(cIdAgenda).val(rispServer.IdAgenda);
                            $(cIdMovScarico).val(rispServer.IdMovS);
                            $(cIdMovCarico).val(rispServer.IdMovC);

                            ////è da cambiare anche il valore di cIdTipoOp (anche il campo nascosto), mettendo modifica
                            //cIdTipoOp = enum_TipoOperazioneDB.Modifica.value;
                            //$('input[name$="hdTipoOp"]').val(enum_TipoOperazioneDB.Modifica.value);

                            let newTitle = TraduciGiacenzeMagazzinoWSClient("Modifica", "Modifica") + " " + rispServer.DescrizioneAgenda;
                            if (document.title !== newTitle) {
                                document.title = newTitle;
                            }
                        }

                    }

                } else if (ris !== undefined && ris.RispostaOK === false) {

                    allOk = false;
                    let msgError;
                    if (modalita_trasferimento === true) {
                        let risp = JSON.parse(ris.RispostaStringa);

                        if (risp.MsgError !== "" && ris.Errore !== "") {
                            msgError = risp.MsgError + "<br/>" + TraduciGiacenzeMagazzinoWSClient("ErroreDuePunti_", "Errore: ") + ris.Errore;
                        } else if (risp.MsgError !== "") {
                            msgError = risp.MsgError;
                        } else if (risMod.Errore !== "") {
                            msgError = ris.Errore;
                        }

                    } else {

                        msgError = ris.RispostaStringa + "<br/>" + TraduciGiacenzeMagazzinoWSClient("ErroreDuePunti_", "Errore: ") + ris.Errore;

                    }

                    MessaggioErrore_Bootstrap(msgError, "DIV_Messaggi");

                    //MessaggioErrore_Bootstrap(ris.RispostaStringa + "<br/>" + "Errore: " + ris.Errore, "DIV_Messaggi");
                    //MessaggioErrore_Bootstrap("Errore: " + ris.Errore, "DIV_Messaggi");

                } else {

                    allOk = false;
                    MessaggioErrore_Bootstrap(TraduciGiacenzeMagazzinoWSClient("ErroreInserimento", "Errore inserimento"), "DIV_Messaggi");

                }

            } else {

                var lavorazione = new Object();
                lavorazione.piva = riga.chiave_giacenze.split("_")[0];
                lavorazione.saCod = parseInt(riga.Sa_Cod);
                lavorazione.tipoDestinazione = parseInt(riga.chiave_giacenze.split("_")[2]);
                lavorazione.idDestinazione = parseInt(riga.Id_Destinazione);
                lavorazione.elemCod = parseInt(riga.Cat_Cod);
                lavorazione.proCod = parseInt(riga.Pro_Cod);
                lavorazione.matCod = parseInt(riga.Mat_Cod);
                lavorazione.matDes = riga.Pro_Des;
                lavorazione.lotto = riga.Lotto;
                lavorazione.calCod = parseInt(riga.Cal_Cod);

                lavorazione.idAgenda = parseInt($(cIdAgenda).val());

                let pesoNetto = kendo.parseFloat(riga.Netto_Mov);
                let pesoLordo = kendo.parseFloat(riga.Lordo_Mov);
                let qta = kendo.parseFloat(riga.Qta_Mov);

                let numConfezioni = parseInt(riga.NrConfezioni_Mov);
                let numContenitori = parseInt(riga.NrContenitori_Mov);
                let numImballaggi = parseInt(riga.NrImballaggi_Mov);

                let modalitaUdm = getModalitaUdm(riga);

                switch (modalitaUdm) {
                    case ENUM_MOD_UDM.KG_CONF:

                        // Posso scegliere la confezione solo con udm = KG (su db è sempre scritto udm = NR)

                        //TODO: devono essere valorizzate numConfezioni e peso netto
                        if (numConfezioni <= 0)
                            throw new Error("numConfezioni <= 0");

                        //38 = numero
                        lavorazione.udmCod = 38;
                        lavorazione.qta = numConfezioni;
                        lavorazione.qtaExtra = pesoNetto / numConfezioni;
                        lavorazione.udmCodExtra = 2;

                        break;

                    case ENUM_MOD_UDM.KG:

                        //Sono nella vecchia situazione: movimento a kg + eventuali contenitori e/o imballi

                        //TODO: deve essere valorizzato peso netto
                        if (pesoNetto <= 0)
                            throw new Error("pesoNetto <= 0");

                        //2 = kg
                        lavorazione.udmCod = 2;
                        lavorazione.qta = pesoNetto;
                        lavorazione.qtaExtra = 1;

                        if (numContenitori > 0 || numImballaggi > 0) {
                            lavorazione.udmCodExtra = 2;
                        } else {
                            lavorazione.udmCodExtra = 0;
                        }

                        break;

                    case ENUM_MOD_UDM.ALTRA_UDM:
                    default:

                        //Ho movimentato a numero o altre Udm

                        lavorazione.udmCod = parseInt(riga.Udm_Cod);
                        lavorazione.qta = qta;
                        lavorazione.qtaExtra = 1;
                        lavorazione.udmCodExtra = 2;

                        //Qta_Mov diventerà di fatto il Peso Netto
                        pesoNetto = qta;

                        break;
                }
                
                lavorazione.qtaExtraTotale = pesoNetto;
                lavorazione.tara = pesoLordo - pesoNetto;

                //if (numConfezioni > 0) {
                //    //38 = numero
                //    lavorazione.udmCod = 38;
                //    lavorazione.qta = numConfezioni;
                //    lavorazione.qtaExtra = pesoNetto / numConfezioni;
                //} else {
                //    //2 = kg
                //    lavorazione.udmCod = 2;
                //    lavorazione.qta = pesoNetto;
                //    lavorazione.qtaExtra = 1;
                //}

                lavorazione.numContenitori = numContenitori;
                lavorazione.numImballaggi = numImballaggi;

                // Aggiungo all'oggetto lavorazione le parti che mi servono introdotte per la gestione algoritmi di interfacciamento
                // con linee macchine produzione
                lavorazione.codMacchinaLav = Get_KendoDDLValue("idLinea_Macchina_Lav", "");
                lavorazione.Linea_Macchina_Lavorazione = ValorizzaParametriMacchinaLavorazione();

                var lav = kendoEscapeOggetto(lavorazione);

                let ris = InserisciScaricoLavorazione($(cIdPiva).val(), lav, true);

                if (ris !== undefined && ris !== null && ris.RispostaOK === true) {

                    if (rileggiScarichi === true) {
                        RicercaScarichi(indirizzohttp, null, $(cIdPiva).val(), $(cIdAgenda).val(), options);
                        gridScarichi.dataSource.read();
                        rileggiScarichi = false;
                    }

                    let messaggio = TraduciGiacenzeMagazzinoWSClient("InserimentoEffettuatoCorrettamente", "Inserimento effettuato correttamente");

                    let objEsito = aggiornaDatiRicalcolatiCarichi(totaleQtaPrec, options);

                    visualizzaMessaggioEsito(messaggio, objEsito);

                } else if (ris !== undefined && ris.RispostaOK === false) {

                    allOk = false;
                    MessaggioErrore_Bootstrap(ris.RispostaStringa + "<br/>" + TraduciGiacenzeMagazzinoWSClient("ErroreDuePunti_", "Errore: ") + ris.Errore, "DIV_Messaggi");

                } else {

                    allOk = false;
                    MessaggioErrore_Bootstrap(TraduciGiacenzeMagazzinoWSClient("ErroreInserimento", "Errore inserimento"), "DIV_Messaggi");

                }

            }

        }

        if (allOk) {

            if (rileggiScarichi === true) {
                RicercaScarichi(indirizzohttp, null, $(cIdPiva).val(), $(cIdAgenda).val(), options);
                gridScarichi.dataSource.read();
                rileggiScarichi = false;
            }

            // Prima di ricaricare la griglia salvo le eventuali personalizzazioni alla griglia fatte dall'utente
            salvaPersonalizzazioniGrigliaKendo(pathCoreWS, location.pathname, "tab_giacenze", false);

            RicercaGiacenzeMagazzino(indirizzohttp, $("input[name$='hf_UtenteAbilitatoScrittura']").val() === "True");
            gridGiacenze.dataSource.read();

            if (modalita_trasferimento === true) {
                //visto che in un colpo solo scrivo entrambi i lati devo ricaricare anche la griglia carichi
                RicercaCarichi(indirizzohttp, null, $(cIdPiva).val(), $(cIdAgenda).val(), options);
                KendoGrid("tab_elenco_carichi").dataSource.read();
            }

        }

    }

}

function controllaRigheInserisciNuoviScarichiPerSubmit(righe) {

    let errMessage = "";

    if (modalita_trasferimento === true) {
        if (KendoDDL("cmbDestinazioneDef").dataItem().key_Dest === "") {
            errMessage += TraduciGiacenzeMagazzinoWSClient("DestinazioneObbligatoria", "Destinazione obbligatoria") + "<br/>";
        }
    }

    for (let x = 0; x < righe.length; x++) {

        let item = righe[x];
        let modalitaUdm = getModalitaUdm(item);

        if (modalita_trasferimento === true) {

            let magazzinoDestinazione = KendoDDL("cmbDestinazioneDef").dataItem();

            if (parseInt(magazzinoDestinazione.Tipo_Destinazione) === parseInt(item.chiave_giacenze.split("_")[2]) &&
                parseInt(magazzinoDestinazione.Sa_Cod) === parseInt(item.Sa_Cod) &&
                parseInt(magazzinoDestinazione.Id_Destinazione) === parseInt(item.Id_Destinazione)) {

                errMessage += TraduciGiacenzeMagazzinoWSClient("MagCellaOrigineDestinazioneCoincidono", "Magazzino/Cella di origine e Destinazione coincidono: impossibile proseguire") + "<br/>";
            }

        }
        
        //if (!FF_gest_materiale_vivaistico && (item.Lordo_Mov === undefined || item.Lordo_Mov === null || item.Lordo_Mov == 0)) {
        //    errMessage += TraduciGiacenzeMagazzinoWSClient("ChilogrammiLordiObbligatori", "Kg Lordi obbligatori") + "<br/>";
        //}

        if (!FF_gest_materiale_vivaistico &&
            (modalitaUdm === ENUM_MOD_UDM.KG_CONF || modalitaUdm === ENUM_MOD_UDM.KG) &&
            (item.Lordo_Mov === undefined || item.Lordo_Mov === null || item.Lordo_Mov == 0)) {
            errMessage += TraduciGiacenzeMagazzinoWSClient("ChilogrammiLordiObbligatori", "Kg Lordi obbligatori") + "<br/>";
        }

        //if (item.Netto_Mov === undefined || item.Netto_Mov === null || item.Netto_Mov == 0) {
        //    if (FF_gest_materiale_vivaistico)
        //        errMessage += TraduciGiacenzeMagazzinoWSClient("NumeroObbligatorio", "Numero obbligatorio") + "<br/>";
        //    else
        //        errMessage += TraduciGiacenzeMagazzinoWSClient("ChilogrammiNettiObbligatori", "Kg Netti obbligatori") + "<br/>";
        //}

        if (modalitaUdm === ENUM_MOD_UDM.KG_CONF || modalitaUdm === ENUM_MOD_UDM.KG) {
            if (item.Netto_Mov === undefined || item.Netto_Mov === null || item.Netto_Mov == 0) {
                if (FF_gest_materiale_vivaistico)
                    errMessage += TraduciGiacenzeMagazzinoWSClient("NumeroObbligatorio", "Numero obbligatorio") + "<br/>";
                else
                    errMessage += TraduciGiacenzeMagazzinoWSClient("ChilogrammiNettiObbligatori", "Kg Netti obbligatori") + "<br/>";
            }
        } else {
            if (item.Qta_Mov === undefined || item.Qta_Mov === null || item.Qta_Mov == 0) {
                errMessage += TraduciGiacenzeMagazzinoWSClient("QuantitaObbligatoria", "Quantità obbligatoria") + "<br/>";
            }
        }

        //Se udm=n ed esiste tipo confezione, devo avere sia qta(num_conf) che peso netto
        if (modalitaUdm === ENUM_MOD_UDM.KG_CONF) {
            if (item.NrConfezioni_Mov === undefined || item.NrConfezioni_Mov === null || item.NrConfezioni_Mov == 0) {
                errMessage += "Numero confezioni obbligatorio" + "<br/>";   //i18n
            }
        }

    }

    return errMessage;
}

function controllaCriteriAggregazioneNuoviScarichi(righeNuove) {

    let messaggioErrore = "";

    if (criteriAggregazionePrimoIngresso.length > 0) {

        const righeScarichi = $("#tab_elenco_scarichi").data("kendoGrid").dataSource.data();

        if (righeScarichi.length > 0) {

            messaggioErrore = confrontaNuovaRigaConRigaEsistente(righeNuove[0], righeScarichi[0]);

        }

    }

    return messaggioErrore;

}

function confrontaNuovaRigaConRigaEsistente(rigaNuova, rigaEsistente) {

    let messaggioErrore = "";

    let criteriAggregazione = criteriAggregazionePrimoIngresso[0];

    // Linea criteri aggregazione
    if (criteriAggregazione.PreparazioneCodLinea === 0) {
        var criteriLinea = getCriteriAggregazionePreparazioneLinea(criteriAggregazione.PreparazioneCod, rigaNuova.Linea_Cod)
        if (criteriLinea.length > 0 && criteriLinea[0].PreparazioneCodLinea !== 0) {
            messaggioErrore = accodaMessaggioErrore(TraduciGiacenzeMagazzinoWSClient("LineaCriteriAggregazione", "Linea criteri aggregazione"), messaggioErrore);
        }
    }

    // Linea
    if (criteriAggregazione.PreparazioneCodLinea !== 0) {
        if (valoriCriterioDiversi(rigaNuova.Linea_Cod, rigaEsistente.Linea_Cod)) {
            messaggioErrore = accodaMessaggioErrore(TraduciGiacenzeMagazzinoWSClient("Linea", "Linea"), messaggioErrore);
        }
    }

    // Fornitore
    if (criteriAggregazione.AggregaFornitore === enum_TipoAggregazione.SoloUguali) {
        const nomeParamQualFornitore = "FF_fornitore_Tipo_Cod";
        if (valoriCriterioDiversi(rigaNuova[nomeParamQualFornitore], rigaEsistente[nomeParamQualFornitore])) {
            messaggioErrore = accodaMessaggioErrore(TraduciGiacenzeMagazzinoWSClient("Fornitore", "Fornitore"), messaggioErrore);
        }
    }

    // Specie
    if (criteriAggregazione.AggregaSpecie === enum_TipoAggregazione.SoloUguali) {
        if (valoriCriterioDiversi(rigaNuova.Veg_Cod, rigaEsistente.Veg_Cod)) {
            messaggioErrore = accodaMessaggioErrore(TraduciGiacenzeMagazzinoWSClient("Specie", "Specie"), messaggioErrore);
        }
    }

    // Varietà
    if (criteriAggregazione.AggregaVarieta === enum_TipoAggregazione.SoloUguali) {
        if (valoriCriterioDiversi(rigaNuova.Cul_Cod, rigaEsistente.Cul_Cod)) {
            messaggioErrore = accodaMessaggioErrore(TraduciGiacenzeMagazzinoWSClient("Varietà", "Varietà"), messaggioErrore);
        }
    }

    // Regolamento
    if (criteriAggregazione.AggregaRegolamento === enum_TipoAggregazione.SoloUguali) {
        if (valoriCriterioDiversi(rigaNuova.Regolamento, rigaEsistente.Reg_Cod)) {
            messaggioErrore = accodaMessaggioErrore(TraduciGiacenzeMagazzinoWSClient("Regolamento", "Regolamento"), messaggioErrore);
        }
    }

    // Lotto
    if (criteriAggregazione.AggregaLotto === enum_TipoAggregazione.SoloUguali) {
        if (valoriCriterioDiversi(rigaNuova.Lotto, rigaEsistente.Lotto)) {
            messaggioErrore = accodaMessaggioErrore(TraduciGiacenzeMagazzinoWSClient("Lotto", "Lotto"), messaggioErrore);
        }
    }

    // Prodotto
    if (criteriAggregazione.AggregaProdotto === enum_TipoAggregazione.SoloUguali) {
        if (valoriCriterioDiversi(rigaNuova.Mat_Cod, rigaEsistente.Mat_Cod)) {
            messaggioErrore = accodaMessaggioErrore(TraduciGiacenzeMagazzinoWSClient("Prodotto", "Prodotto"), messaggioErrore);
        }
    }

    // Unità misura prodotto
    if (criteriAggregazione.AggregaUdm === enum_TipoAggregazione.SoloUguali) {
        if (valoriCriterioDiversi(rigaNuova.Udm_Cod, rigaEsistente.Udm_Cod)) {
            messaggioErrore = accodaMessaggioErrore(TraduciGiacenzeMagazzinoWSClient("UnitàDiMisura", "Unità di Misura"), messaggioErrore);
        }
    }

    // Cella
    if (criteriAggregazione.AggregaCella === enum_TipoAggregazione.SoloUguali) {
        let rigaNuova_Cella = TIPO_DESTINAZIONE_CELLA + "_" + rigaNuova.Sa_Cod + "_" + rigaNuova.Id_Destinazione;
        let rigaEsistente_Cella = rigaEsistente.key_Dest;
        if (valoriCriterioDiversi(rigaNuova_Cella, rigaEsistente_Cella)) {
            messaggioErrore = accodaMessaggioErrore(TraduciGiacenzeMagazzinoWSClient("Cella", "Cella"), messaggioErrore);
        }
    }

    // Parametri qualitativi

    let criteriParamQual = criteriAggregazione.CriteriParamQual;

    for (let i = 0; i < criteriParamQual.length; i++) {

        if (criteriParamQual[i].AggregaParam === enum_TipoAggregazione.SoloUguali) {

            const nomeColonnaParamQual = getNomeColonnaParamQualDaCodice(criteriParamQual[i].CodParam);

            if (valoriCriterioDiversi(rigaNuova[nomeColonnaParamQual], rigaEsistente[nomeColonnaParamQual])) {

                let nomeColonnaParamQual = "";

                const datiParamQual = getDatiParamQualByCodDes(criteriParamQual[i].CodParam);

                if (datiParamQual.length > 0) {
                    nomeColonnaParamQual = datiParamQual[0].Tabella_Des;
                } else {
                    nomeColonnaParamQual = criteriParamQual[i].CodParam;
                }  

                messaggioErrore = accodaMessaggioErrore(nomeColonnaParamQual, messaggioErrore);

            }

        }

    }

    return messaggioErrore;

}

function valoriCriterioDiversi(campoRigaNuova, campoRigaEsistente) {

    if (campoRigaNuova === null || campoRigaNuova === undefined) {
        campoRigaNuova = "";
    }

    if (campoRigaEsistente === null || campoRigaEsistente === undefined) {
        campoRigaEsistente = "";
    }

    return campoRigaNuova !== campoRigaEsistente;

}

function accodaMessaggioErrore(messaggioCriterio, messaggioErrore) {

    let messaggioErroreOutput = messaggioErrore;

    if (messaggioErroreOutput === "") {
        let messaggioIniziale = TraduciGiacenzeMagazzinoWSClient("RigaNonRispettaCriteriAggregazione", "La riga non rispetta alcuni dei criteri di aggregazione previsti");
        messaggioErroreOutput = messaggioIniziale + ": ";
    } else {
        messaggioErroreOutput += ", ";
    }

    messaggioErroreOutput += messaggioCriterio;

    return messaggioErroreOutput;

}