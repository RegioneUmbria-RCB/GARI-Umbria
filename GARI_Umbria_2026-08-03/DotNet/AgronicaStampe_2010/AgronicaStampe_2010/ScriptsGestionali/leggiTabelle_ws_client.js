// *********************************************************************************************************
// * ATTENZIONE!!!!!!!!
// * Questo file è una copia creata a partire dal file leggiTabelle_ws_client.js di AgronicaAgenda_2010
// * Ogni tanto sarà necessario allinearlo fino a che non si troverà una soluzione per unificare il tutto
// *********************************************************************************************************

var indirizzohttp_ListControl = "AgronicaCoreUtility/CaricaListControl.asmx";
var indirizzohttp_Accise = "Metaschema/Accise.asmx";
var indirizzohttp_ContabHelper = "AgronicaCoreModello/ContabilitaHelper.asmx";
var indirizzohttp_Leggi_Tabelle_FF_WS = "FreshAndFood/FreshAndFood.asmx";
var indirizzohttp_Leggi_IVA_Aliquote = "Metaschema/IVA_Aliquote.asmx";
var indirizzohttp_Categorie_Magazzino = "Metaschema/Categorie_Magazzino.asmx";
var indirizzohttp_CategorieXUnitaMisura = "Metaschema/CategorieXUnitaMisura.asmx";
var indirizzohttp_ElencoCompletoProdotti = "Anagrafica/Prodotti.asmx";
var indirizzohttp_Fabbricati = "Anagrafica/Fabbricati.asmx";
var indirizzohttp_Giacenze = "Contab/Giacenze.asmx";
var indirizzohttp_Aziende = "Anagrafica/Imprese.asmx";
var indirizzohttp_Contatti = "Anagrafica/Contatti.asmx";
var indirizzohttp_Utenti_Impostazioni = "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx";
var indirizzohttp_Pagamenti_Causali = "Contab/Pagamenti_Causali.asmx";
var indirizzohttp_Listini = "Contab/Listini_Prezzi.asmx";
var indirizzohttp_Utenti = "AgronicaCoreUtentiBIZ/Utenti_R.asmx";
var indirizzohttp_Contab = "Contab/Contabilita.asmx";
var indirizzohttp_Nazioni = "Metaschema/Nazioni.asmx";
var indirizzohttp_Regioni = "Metaschema/Regioni.asmx";
var indirizzohttp_Indirizzi = "Anagrafica/Indirizzi.asmx";
var indirizzohttp_LineeMacchineLavorazione = "FreshAndFood/Lavorazioni.asmx";
var indirizzohttp_RicercaDocumenti_Con_Agenda = "AgronicaCoreScadenziario/Alert_Entita.asmx";

var elencoContatti = [];
var elencoConferentiAccettazione = [];
//Devo inizializzare, per assicurarmi che ci sia sempre
elencoConferentiAccettazione[1] = { pivaPadreGerarchia: "", ruoloAccettazione: "Conferente", elencoContatti: [] };
elencoConferentiAccettazione[2] = { pivaPadreGerarchia: "-1", ruoloAccettazione: "Coop1", elencoContatti: [] };
elencoConferentiAccettazione[3] = { pivaPadreGerarchia: "-1", ruoloAccettazione: "Coop2", elencoContatti: [] };
elencoConferentiAccettazione[4] = { pivaPadreGerarchia: "-1", ruoloAccettazione: "Produttore", elencoContatti: [] };

var elencoCedenti = null;
var elencoCessionari = null;
var elencoSezionali = null;
var elencoAspettoBeni = null;
var elencoCausaliContabilita = null;
var elencoCausaliTrasporto = null;
var elencoSigleAE = null;
var elencoGestioneVettore = null;
var elencoVettori = null;
var elencoAccUnitaTrasporto = null;
var elencoAccModalitaTrasporto = null;
var elencoOperatori = null;
var elencoContoEconomico = null;
var elencoContoPatrimoniale = null;
var elencoAnniApertiConti = null;

var elencoParametriQualitativi = null;
var valoriParamQualitativiList = [];
var elencoCausaliEntrataProdotto = null;
var elencoGruppiFatturazione = null;
var elencoSpecie = null;
var elencoVarieta = [];
var elencoTuttiProdotti = null;
var elencoImballaggi = null;
var elencoContenitori = null;
var elencoConfezioni = null;
var elencoCelle = null;
var elencoLineeMacchineLavorazione = null;
var elencoMagazzini = null;
var elencoCelleMagazzini = null;
var elencoFornitori = null;
var elencoConfigImballiProdotto = null;
var elencoIVA_Aliquote = null;
var elencoCategorie_Magazzino = null;
var elencoCategorieXUnitaMisura = null;
var elencoPUA_Regolamenti = null;
var elenco_Aziende = null;
var elencoCentriAziendali = null;
var elencoMagazziniOmni = null;
var elencoPagamenti_Causali = null;
var elencoNumeratoriDefaults = null;
var elencoNazioni = null;
var elencoRegioni = null;
var elencoDocumentiConAgenda = null;

function GetUrlLetturaTabelleGestionali() {

    var url = window.location.protocol + "//" + window.location.hostname;

    if (typeof pathCoreWS !== "undefined") {
        if (pathCoreWS !== undefined && pathCoreWS.includes(url)) {
            url = "";
        }
    } else {
        pathCoreWS = "";
    }

    return url + pathCoreWS;
}

// START GESTIONE ERRORE
function ricavaErrore(risposta, msgErrorDefault) {
    var msgError = "";
    if (risposta !== undefined && risposta !== null && risposta.RispostaOK === false) {
        msgError = risposta.RispostaStringa + "<br/>" + "Errore: " + risposta.Errore;
    } else {
        msgError = msgErrorDefault;
    }
    return msgError;
}

function mostraErrore(risposta, msgErrorDefault) {
    var msgError = ricavaErrore(risposta, msgErrorDefault);
    MessaggioErrore_Bootstrap(msgError, "DIV_Messaggi");
    console.error("Errore interno: " + msgError);
    return msgError;
}
// END GESTIONE ERRORE

function RicercaSezionali(piva, tipoValue, gestioneWaitFrame) {

    return new Promise(function (resolve, reject) {

        if (elencoSezionali === null) {
            var param = kendo.stringify({
                objP_server: objP_server,
                PrimaRiga_Flag: false,
                PrimaRiga_Text: "",
                PrimaRiga_Value: "",
                Tipo_Value: tipoValue,
                Piva: piva,
                Sezionale_Cod: 0
            });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_ListControl + "/LeggiSezionali",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    elencoSezionali = risp;
                    resolve(risp);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Sezionali");
                    reject(new Error(msgError));
                },
                null,
                gestioneWaitFrame);
        } else {
            resolve(elencoSezionali);
        }
    });

}

function RicercaAspettoBeni(moduloGias, gestioneWaitFrame) {

    return new Promise(function (resolve, reject) {

        if (elencoAspettoBeni === null) {
            var param = kendo.stringify({
                objP_server: objP_server,
                PrimaRiga_Flag: false,
                PrimaRiga_Text: "",
                PrimaRiga_Value: "",
                moduloGias: moduloGias
            });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_ListControl + "/LeggiAspettoBeni",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    elencoAspettoBeni = risp;
                    resolve(risp);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Aspetto Beni");
                    reject(new Error(msgError));
                },
                null,
                gestioneWaitFrame);
        } else {
            resolve(elencoAspettoBeni);
        }
    });
}

function RicercaCausaliContabilita(flagVuoto, piva, codice, categoria, filtroAggiuntivo, orderBy, gestioneWaitFrame) {

    return new Promise(function (resolve, reject) {

        if (elencoCausaliContabilita === null) {
            var param = kendo.stringify({
                piva: piva,
                codice: codice,
                categoria: categoria,
                xFiltroAggiuntivo: filtroAggiuntivo,
                xOrderBy: orderBy,
                objP_server: objP_server
            });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + "Contab/Causali_Fattura.asmx/LeggiCausaliFattura",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    if (flagVuoto === true) {
                        var objVuoto = { Cau_Contab_Codice: 0, Cau_Contab_Descrizione: "", Cau_Contab_Categoria: "", Piva: "" };
                        risp.unshift(objVuoto);
                    }
                    elencoCausaliContabilita = risp;
                    resolve(elencoCausaliContabilita);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Causali Contabilità");
                    reject(new Error(msgError));
                },
                null,
                gestioneWaitFrame);
        } else {
            resolve(elencoCausaliContabilita);
        }
    });
}

function RicercaCausaliTrasporto(moduloGias, lavCod, gestioneWaitFrame) {

    return new Promise(function (resolve, reject) {

        if (elencoCausaliTrasporto === null) {
            var param = kendo.stringify({
                objP_server: objP_server,
                PrimaRiga_Flag: false,
                PrimaRiga_Text: "",
                PrimaRiga_Value: "",
                moduloGias: moduloGias,
                lavCod: lavCod
            });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_ListControl + "/LeggiCausaliTrasporto",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    elencoCausaliTrasporto = risp;
                    resolve(elencoCausaliTrasporto);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Causali Trasporto");
                    reject(new Error(msgError));
                },
                null,
                gestioneWaitFrame);
        } else {
            resolve(elencoCausaliTrasporto);
        }
    });
}



function RicercaSigleAE(lavCod, gestioneWaitFrame) {

    return new Promise(function (resolve, reject) {

        if (elencoSigleAE === null) {
            var param = kendo.stringify({
                objP_server: objP_server,
                PrimaRiga_Flag: false,
                PrimaRiga_Text: "",
                PrimaRiga_Value: "",
                lavCod: lavCod
            });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_ListControl + "/LeggiSigleAE",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    elencoSigleAE = risp;
                    resolve(elencoSigleAE);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Sigle AE");
                    reject(new Error(msgError));
                },
                null,
                gestioneWaitFrame);
        } else {
            resolve(elencoSigleAE);
        }
    });
}



function RicercaGestioneVettore(gestioneWaitFrame) {

    return new Promise(function (resolve, reject) {

        if (elencoGestioneVettore === null) {
            var param = kendo.stringify({
                objP_server: objP_server,
                PrimaRiga_Flag: true,
                PrimaRiga_Text: "",
                PrimaRiga_Value: "0",
                flagEstero: true
            });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_ListControl + "/LeggiGestioneVettore",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    elencoGestioneVettore = risp;
                    resolve(risp);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Gestione Vettore");
                    reject(new Error(msgError));
                },
                null,
                gestioneWaitFrame);
        } else {
            resolve(elencoGestioneVettore);
        }

    });
}

function RicercaContatti(flagVuoto, piva, cliente, fornitore, dipendente, terzista, legale, agente, consulente, gestioneWaitFrame) {

    return new Promise(function (resolve, reject) {

        var param = kendo.stringify({
            objP_server: objP_server,
            piva: piva,
            rapportoAttivo: true,
            cliente: cliente,
            fornitore: fornitore,
            dipendente: dipendente,
            terzista: terzista,
            legale: legale,
            agente: agente,
            consulente: consulente,
            Cod_Contatto: ""
        });

        ajaxAgronica(GetUrlLetturaTabelleGestionali() + "Anagrafica/Contatti.asmx/LeggiRapportoSpecifico",
            param,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = { Rag_Soc: "", Rag_Soc_Completa: "", Cod_RisUm: 0, Cod_Contatto: "", Partita_Iva: "", IsImpresaGias: false };
                    risp.unshift(objVuoto);
                }
                resolve(risp);
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Contatti");
                reject(new Error(msgError));
            },
            null,
            gestioneWaitFrame);
    });
}

//tipoRapporto: 0 = Clienti, 1 = Fornitori, 2 = Professionisti, 3 = Dipendenti+Terzisti, 4 = Agenti, 5 = Capo Area, 
// 6 = Conferenti x Accettazione, 7 = Vettori, 8 = Clienti + Fornitori, 9 = Terzisti(no filtro), 10 = Dipendenti
function RicercaContattiDocumentoAsync(flagVuoto, piva, tipoRapporto, dataValidita, filtraDataSuDb, filtraSoloValidi, cercaSoloValidi,
    includiIndirizzo, accettazioneConGerarchia, pivaPadreGerarchia, testoRicerca, codRisUm, codContatto, gestioneWaitFrame, checkRaccolte, dataFineRaccolte) {

    return new Promise(function (resolve) {
        if (elencoContatti[tipoRapporto] === null || elencoContatti[tipoRapporto] === undefined) {

            let rapportoAttivo = true;
            let dataValiditaDb = dataValidita;

            if (filtraDataSuDb === false) {
                rapportoAttivo = false;
                dataValiditaDb = null;
            }

            if (checkRaccolte === undefined || checkRaccolte === null) {
                checkRaccolte = false;
            }

            if (dataFineRaccolte === undefined || dataFineRaccolte === null) {
                dataFineRaccolte = new Date();
            }

            var param = kendo.stringify({
                objP_server: objP_server,
                piva: piva,
                rapportoAttivo: rapportoAttivo,
                dataValidita: dataValiditaDb,
                tipoRapporto: tipoRapporto,
                filtraSoloValidi: filtraSoloValidi,
                cercaSoloValidi: cercaSoloValidi,
                includiIndirizzo: includiIndirizzo,
                accettazioneConGerarchia: accettazioneConGerarchia,
                pivaPadreGerarchia: pivaPadreGerarchia,
                testoRicerca: testoRicerca,
                codRisUm: codRisUm,
                codContatto: codContatto,
                checkRaccolte: checkRaccolte,
                dataFineRaccolte: dataFineRaccolte
            });

            //Faccio la chiamata asincrona (utile in docReady o quando non va usata in cascade)
            ajaxAgronica(GetUrlLetturaTabelleGestionali() + "Anagrafica/Contatti.asmx/LeggiRapportoDocumenti",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    if (flagVuoto === true) {
                        var objVuoto = { Rag_Soc: "", Rag_Soc_Completa: "", Cod_RisUm: 0, Cod_Contatto: "", Partita_Iva: "", IsImpresaGias: false, Validita_Inizio: AGRODATAINIZIO, Validita_Fine: AGRODATAFINE };
                        risp.unshift(objVuoto);
                    }
                    elencoContatti[tipoRapporto] = risp;

                    if (filtraDataSuDb === false && dataValidita !== null) {
                        resolve(elencoContatti[tipoRapporto].filter(function (x) {
                            return kendo.parseDate(x.Validita_Inizio) <= dataValidita && kendo.parseDate(x.Validita_Fine) >= dataValidita;
                        }));
                    } else {
                        resolve(elencoContatti[tipoRapporto]);
                    }
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Contatti");
                    reject(new Error(msgError));
                },
                null,
                gestioneWaitFrame);

        } else {
            if (filtraDataSuDb === false && dataValidita !== null) {
                resolve(elencoContatti[tipoRapporto].filter(function (x) {
                    return kendo.parseDate(x.Validita_Inizio) <= dataValidita && kendo.parseDate(x.Validita_Fine) >= dataValidita;
                }));
            } else {
                resolve(elencoContatti[tipoRapporto]);
            }
        }
    });

}

//tipoRapporto: 0 = Clienti, 1 = Fornitori, 2 = Professionisti, 3 = Dipendenti+Terzisti, 4 = Agenti, 5 = Capo Area, 
// 6 = Conferenti x Accettazione, 7 = Vettori, 8 = Clienti + Fornitori, 9 = Terzisti(no filtro), 10 = Dipendenti
function RicercaContattiDocumentoSync(flagVuoto, piva, tipoRapporto, filtraSoloValidi, cercaSoloValidi, includiIndirizzo,
    accettazioneConGerarchia, pivaPadreGerarchia, testoRicerca, codRisUm, codContatto, gestioneWaitFrame, checkRaccolte, dataFineRaccolte) {

    if (elencoContatti[tipoRapporto] === null || elencoContatti[tipoRapporto] === undefined) {

        if (checkRaccolte === undefined || checkRaccolte === null) {
            checkRaccolte = false;
        }

        if (dataFineRaccolte === undefined || dataFineRaccolte === null) {
            dataFineRaccolte = new Date();
        }

        var param = kendo.stringify({
            objP_server: objP_server,
            piva: piva,
            rapportoAttivo: true,
            dataValidita: null,
            tipoRapporto: tipoRapporto,
            filtraSoloValidi: filtraSoloValidi,
            cercaSoloValidi: cercaSoloValidi,
            includiIndirizzo: includiIndirizzo,
            accettazioneConGerarchia: accettazioneConGerarchia,
            pivaPadreGerarchia: pivaPadreGerarchia,
            testoRicerca: testoRicerca,
            codRisUm: codRisUm,
            codContatto: codContatto,
            checkRaccolte: checkRaccolte,
            dataFineRaccolte: dataFineRaccolte
        });

        //Faccio la chiamata sincrona (utile quando richiamata da un evento change di altra ddl)
        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Anagrafica/Contatti.asmx/LeggiRapportoDocumenti",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = { Rag_Soc: "", Rag_Soc_Completa: "", Cod_RisUm: 0, Cod_Contatto: "", Partita_Iva: "", IsImpresaGias: false, Validita_Inizio: AGRODATAINIZIO, Validita_Fine: AGRODATAFINE };
                    risp.unshift(objVuoto);
                }
                elencoContatti[tipoRapporto] = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Contatti");
            },
            null,
            gestioneWaitFrame);
    }

    return elencoContatti[tipoRapporto];
}

function RicercaConferentiGerarchiaDocumentoSync(flagVuoto, piva, dataValidita, filtraDataSuDb, filtraSoloValidi, cercaSoloValidi, includiIndirizzo,
    accettazioneConGerarchia, ruoloAccettazione, pivaPadreGerarchia, testoRicerca, codRisUm, codContatto, gestioneWaitFrame, checkRaccolte, dataFineRaccolte) {

    if (elencoConferentiAccettazione[accettazioneConGerarchia].elencoContatti === undefined ||
        elencoConferentiAccettazione[accettazioneConGerarchia].elencoContatti === null) {

        var objVuoto = { Rag_Soc: "", Rag_Soc_Completa: "", Cod_RisUm: 0, Cod_Contatto: "", Partita_Iva: "", IsImpresaGias: false, Validita_Inizio: AGRODATAINIZIO, Validita_Fine: AGRODATAFINE };

        if (accettazioneConGerarchia !== 0 && pivaPadreGerarchia === "-1") {
            //viene passato apposta -1 per evitare che la query restituisca risultati,
            //ma a questo punto, tanto vale risparmiare tempo e non fare la query

            var risp = new Array;
            if (flagVuoto === true) {
                risp.unshift(objVuoto);
            }
            elencoConferentiAccettazione[accettazioneConGerarchia] = { ruoloAccettazione: ruoloAccettazione, pivaPadreGerarchia: "-1", elencoContatti: risp };

        } else {

            let rapportoAttivo = true;
            let dataValiditaDb = dataValidita;

            if (filtraDataSuDb === false) {
                rapportoAttivo = false;
                dataValiditaDb = null;
            }

            if (checkRaccolte === undefined || checkRaccolte === null) {
                checkRaccolte = false;
            }

            if (dataFineRaccolte === undefined || dataFineRaccolte === null) {
                dataFineRaccolte = new Date();
            }

            var param = kendo.stringify({
                objP_server: objP_server,
                piva: piva,
                rapportoAttivo: rapportoAttivo,
                dataValidita: dataValiditaDb,
                tipoRapporto: enum_TipoRapporto.Conferenti,
                filtraSoloValidi: filtraSoloValidi,
                cercaSoloValidi: cercaSoloValidi,
                includiIndirizzo: includiIndirizzo,
                accettazioneConGerarchia: accettazioneConGerarchia,
                pivaPadreGerarchia: pivaPadreGerarchia,
                testoRicerca: testoRicerca,
                codRisUm: codRisUm,
                codContatto: codContatto,
                checkRaccolte: checkRaccolte,
                dataFineRaccolte: dataFineRaccolte
            });

            //Faccio la chiamata sincrona (utile quando richiamata da un evento change di altra ddl)
            ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Anagrafica/Contatti.asmx/LeggiRapportoDocumenti",
                param,
                false,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    if (flagVuoto === true) {
                        risp.unshift(objVuoto);
                    }
                    elencoConferentiAccettazione[accettazioneConGerarchia] = { ruoloAccettazione: ruoloAccettazione, pivaPadreGerarchia: pivaPadreGerarchia, elencoContatti: risp };
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Contatti");
                },
                null,
                gestioneWaitFrame);
        }

    }

    //Per evitare che mi ricapiti di ri-leggere immediatamente la stessa cosa, 
    //per i conferenti con gerarchia (problemi dovuti ai vari change e ricaricamenti delle ddl in cascata)
    //mi salvo la lista appena caricata e quali criteri ho usato, così se mi ricapitano gli stessi parametri, non c'è bisogno che resetto la lista.

    if (filtraDataSuDb === false && dataValidita !== null) {
        return elencoConferentiAccettazione[accettazioneConGerarchia].elencoContatti.filter(function (x) {
            return kendo.parseDate(x.Validita_Inizio) <= dataValidita && kendo.parseDate(x.Validita_Fine) >= dataValidita;
        });
    } else {
        return elencoConferentiAccettazione[accettazioneConGerarchia].elencoContatti;
    }
    //return elencoConferentiAccettazione[accettazioneConGerarchia].elencoContatti;
}

function RicercaTipoIndirizzo(flagVuoto, piva, codContatto, gestioneWaitFrame) {

    var elencoTipiIndirizzoContatto = Array();

    if (codContatto !== "") {
        var param = kendo.stringify({
            objP_server: objP_server,
            piva: piva,
            codContatto: codContatto,
            codIndirizzo: 0,
            tipoIndirizzo: 0
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Indirizzi + "/LeggiIndirizziContatto",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Tipo_Indirizzo": "",
                        "Tipo_Indirizzo_Des": "",
                        "Cod_Indirizzo": 0,
                        "Indirizzo": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoTipiIndirizzoContatto = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Indirizzi");
            },
            null,
            gestioneWaitFrame);
    }

    return elencoTipiIndirizzoContatto;
}

function RicercaIndirizziContatti_X_RappContab_X_TipiInd(flagVuoto, gestioneWaitFrame, piva, codContatto,
    flagCliente, flagFornitore, flagDipendente, flagTerzista, flagLegale, flagAgente, flagConsulente, flagConferente, arrTipiIndirizzi) {
    var elencoTipiIndirizzoContatto = Array();

    if (piva !== "") {
        var param = kendo.stringify({
            objP_server: objP_server,
            piva: piva,
            codContatto: codContatto,
            cliente: flagCliente,
            fornitore: flagFornitore,
            dipendente: flagDipendente,
            terzista: flagTerzista,
            legale: flagLegale,
            agente: flagAgente,
            consulente: flagConsulente,
            conferente: flagConferente,
            tipiIndirizzi: arrTipiIndirizzi
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Indirizzi + "/LeggiIndirizziContatti_X_RappContab_X_TipiInd",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {

                    };
                    risp.unshift(objVuoto);
                }
                elencoTipiIndirizzoContatto = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Indirizzi");
            },
            null,
            gestioneWaitFrame);
    }

    return elencoTipiIndirizzoContatto;
}

function RicercaAccUnitaTrasporto(flagVuoto, gestioneWaitFrame) {

    return new Promise(function (resolve, reject) {

        if (elencoAccUnitaTrasporto === null) {
            var param = kendo.stringify({ objP_server: objP_server, codice: -1 });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_Accise + "/LeggiUnitaTrasporto",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    if (flagVuoto === true) {
                        var objVuoto = { "UnitaTrasporto_Cod": 0, "UnitaTrasporto_Des": "" };
                        risp.unshift(objVuoto);
                    }
                    elencoAccUnitaTrasporto = risp;
                    resolve(risp);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Unita Trasporto");
                    reject(new Error(msgError));
                },
                null,
                gestioneWaitFrame);
        } else {
            resolve(elencoAccUnitaTrasporto);
        }
    });

}

function RicercaAccModalitaTrasporto(flagVuoto, gestioneWaitFrame) {

    return new Promise(function (resolve, reject) {

        if (elencoAccModalitaTrasporto === null) {
            var param = kendo.stringify({ objP_server: objP_server, codice: -1 });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_Accise + "/LeggiModalitaTrasporto",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    if (flagVuoto === true) {
                        var objVuoto = { "ModalitaTrasporto_Cod": -1, "ModalitaTrasporto_Des": "" };
                        risp.unshift(objVuoto);
                    }
                    elencoAccModalitaTrasporto = risp;
                    resolve(risp);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS modalita trasporto");
                    reject(new Error(msgError));
                },
                null,
                gestioneWaitFrame);
        } else {
            resolve(elencoAccModalitaTrasporto);
        }
    });
}

function RicercaMezzoTrasporto(flagVuoto, pivaContatto, saCodContatto, codContatto, xOrderBy, gestioneWaitFrame) {

    var elencoMezzoTrasporto = Array();

    if (codContatto !== "") {
        var param = kendo.stringify({
            objP_server: objP_server,
            piva: pivaContatto,
            codContatto: codContatto,
            xOrderBy: xOrderBy
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Anagrafica/Macchine.asmx/Leggi_Macchine_Per_Contatto",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = { "Mac_Cod": -1, "Targa": "" };
                    risp.unshift(objVuoto);
                }
                elencoMezzoTrasporto = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Macchine per contatto");
            },
            null,
            gestioneWaitFrame);
    }

    return elencoMezzoTrasporto;
}

function GetOperatore(flagVuoto, usernameUtente, escludiSuperUser, gestioneWaitFrame) {

    return new Promise(function (resolve, reject) {
        if (elencoOperatori === null) {
            var param = kendo.stringify({
                objP_utenti: objP_utenti,
                usernameUtente: usernameUtente,
                escludiSuperUser: escludiSuperUser
            });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_Utenti + "/GetOperatore",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    if (flagVuoto === true) {
                        var objVuoto = { "Codice_Fiscale": "-1", "Utente": "" };
                        risp.unshift(objVuoto);
                    }
                    elencoOperatori = risp;
                    resolve(risp);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Operatore");
                    reject(new Error(msgError));
                },
                null,
                gestioneWaitFrame);

        } else {
            resolve(elencoOperatori);
        }
    });
}

function RicercaAnniApertiConti(flagVuoto, piva, anno, gestioneWaitFrame) {

    if (elencoAnniApertiConti === null) {

        var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti, piva: piva });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Contab/Conti.asmx/WS_Leggi_AnniAperti",
            param, false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = { "Anno": 0, "Anno_Descr": "" };
                    risp.unshift(objVuoto);
                }
                elencoAnniApertiConti = risp;
            },
            null,
            null,
            gestioneWaitFrame);
    }

    return elencoAnniApertiConti;
}

function RicercaContoEconomico(flagVuoto, piva, anno, tipoDareAvere, gestioneWaitFrame, PrimaRiga_Text, PrimaRiga_Value) {

    if (elencoContoEconomico === null) {

        if (PrimaRiga_Text === undefined || PrimaRiga_Text === null) PrimaRiga_Text = "";
        if (PrimaRiga_Value === undefined || PrimaRiga_Value === null) PrimaRiga_Value = -1;

        var param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva,
            anno: anno,
            ricCod: 2,
            tipoDareAvere: tipoDareAvere,
            codConto: 0,
            imputabile: 1
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Contab/Conti.asmx/WS_Leggi_ContiEconomici",
            param, false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = { Cod_Conto: PrimaRiga_Value, Descr_Conto: PrimaRiga_Text };
                    risp.unshift(objVuoto);
                }
                elencoContoEconomico = risp;
            },
            null,
            null,
            gestioneWaitFrame);
    }

    return elencoContoEconomico;
}

function RicercaContoPatrimoniale(flagVuoto, piva, anno, tipoDareAvere, gestioneWaitFrame, PrimaRiga_Text, PrimaRiga_Value) {

    if (elencoContoPatrimoniale === null) {

        if (PrimaRiga_Text === undefined || PrimaRiga_Text === null) PrimaRiga_Text = "";
        if (PrimaRiga_Value === undefined || PrimaRiga_Value === null) PrimaRiga_Value = -1;

        var param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva,
            anno: anno,
            ricCodPat: 2,
            tipoDareAvere: tipoDareAvere,
            codContoPat: 0,
            imputabile: 1
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Contab/Conti.asmx/WS_Leggi_ContiPatrimoniali",
            param, false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = { Cod_Conto_Pat: PrimaRiga_Value, Descr_Conto_Pat: PrimaRiga_Text };
                    risp.unshift(objVuoto);
                }
                elencoContoPatrimoniale = risp;
            },
            null,
            null,
            gestioneWaitFrame);
    }

    return elencoContoPatrimoniale;
}

function RicercaParametriQualitativi(flagVuoto, piva) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = true;

    if (elencoParametriQualitativi === null) {

        var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti, piva: piva });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiParametriQualitativi",
            param, false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Tabella_ID": 0,
                        "Tabella_Des": "",
                        "Tabella_Cod_Des": "",
                        "Tipo": 0,
                        "ChkObbligatorio": 0,
                        "Tabella_Key_Rif": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoParametriQualitativi = risp;
            }, null);

    }

    return elencoParametriQualitativi;
}

function RicercaParametriQualitativiFiltroSpecieVarieta(flagVuoto, piva, modulo_generazione, veg_cod, cul_cod) {

    let parametriQualitativiFiltroSpecieVarieta = [];
    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = true;
    if (veg_cod === null)
        veg_cod = 0;
    if (cul_cod === null)
        cul_cod = 0;

    var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti, piva: piva, modulo_generazione: modulo_generazione, veg_cod: veg_cod, cul_cod: cul_cod });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiParametriQualitativiFiltroSpecieVarieta",
        param, false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                var objVuoto = {
                    "Tabella_ID": 0,
                    "Tabella_Des": "",
                    "Tabella_Cod_Des": "",
                    "Tipo": 0,
                    "ChkObbligatorio": 0,
                    "Tabella_Key_Rif": "",
                    "Valore_Maximo": "",
                    "Valore_Minimo": ""
                };
                risp.unshift(objVuoto);
            }
            parametriQualitativiFiltroSpecieVarieta = risp;
        }, null);

    return parametriQualitativiFiltroSpecieVarieta;
}

function RicercaValoriParametriQualitativi(Tabella_ID, piva, flagVuoto) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = true;

    var risultato_lettura_FF;
    var found = false;

    if (valoriParamQualitativiList !== undefined && valoriParamQualitativiList.length > 0) {
        for (var iValParam = 0; iValParam < valoriParamQualitativiList.length; iValParam++) {
            var vpq = valoriParamQualitativiList[iValParam];
            if (vpq.Tabella_ID == Tabella_ID) {
                risultato_lettura_FF = vpq.Dati;
                found = true;
                break;
            }
        }
    }

    if (!found) {
        var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti, piva: piva, Tabella_ID: Tabella_ID });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiValoriParametriQualitativi",
            param, false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto) {
                    var objVuoto = {
                        "val_tabella_cod": parseInt(Tabella_ID), "val_cod": 0, "val_sigla": "", "val_des": "", "tara": 0, "elem_cod": 0, "mat_cod": 0, "imballo_des": "", "qta_extra": 0, "ofiltro_veg_cod": "0", "ofiltro_cul_cod": "0", "valore_min": 0, "valore_max": 0
                    };
                    risp.unshift(objVuoto);
                }
                risultato_lettura_FF = risp;
                valoriParamQualitativiList.push({ Tabella_ID: Tabella_ID, Dati: risultato_lettura_FF });
            }, null);
    }

    return risultato_lettura_FF;
}

function RicercaValoriParametriQualitativiFiltratiSpecieVarieta(Tabella_ID, piva, flagVuoto, veg_cod, cul_cod) {
    let risultato_lettura_SpecieVarieta = [];
    if (veg_cod === null)
        veg_cod = 0;
    if (cul_cod === null)
        cul_cod = 0;

    let risultato_lettura_FF = RicercaValoriParametriQualitativi(Tabella_ID, piva, flagVuoto);
    if (risultato_lettura_FF !== undefined && risultato_lettura_FF !== null) {
        for (let i = 0; i < risultato_lettura_FF.length; i++) {
            let rl_FF_veg_cod = replaceAll(risultato_lettura_FF[i].ofiltro_veg_cod, " ", "");
            let rl_FF_cul_cod = replaceAll(risultato_lettura_FF[i].ofiltro_cul_cod, " ", "");

            if ((rl_FF_veg_cod !== "0" && rl_FF_veg_cod !== "" && rl_FF_veg_cod !== "|" && rl_FF_veg_cod !== "||") ||
                (rl_FF_cul_cod !== "0" && rl_FF_cul_cod !== "" && rl_FF_cul_cod !== "|" && rl_FF_cul_cod !== "||")) {

                let found = false;
                // Primo giro con specie / varietà
                if ((rl_FF_veg_cod.includes("|" + veg_cod.toString() + "|") || rl_FF_veg_cod.startsWith(veg_cod.toString() + "|") || rl_FF_veg_cod.endsWith("|" + veg_cod.toString())) &&
                    (rl_FF_cul_cod.includes("|" + cul_cod.toString() + "|") || rl_FF_cul_cod.startsWith(cul_cod.toString() + "|") || rl_FF_cul_cod.endsWith("|" + cul_cod.toString()))) {
                    risultato_lettura_SpecieVarieta.push(risultato_lettura_FF[i]);
                    found = true;
                }

                if (!found) {
                    // Secondo giro con sola specie
                    if ((rl_FF_veg_cod.includes("|" + veg_cod.toString() + "|") || rl_FF_veg_cod.startsWith(veg_cod.toString() + "|") || rl_FF_veg_cod.endsWith("|" + veg_cod.toString())) &&
                        (rl_FF_cul_cod === "0" || rl_FF_cul_cod === "" || rl_FF_cul_cod === "|" || rl_FF_cul_cod === "||")) {
                        risultato_lettura_SpecieVarieta.push(risultato_lettura_FF[i]);
                    }
                }

            } else {
                // Nessun filtro per questo valore
                risultato_lettura_SpecieVarieta.push(risultato_lettura_FF[i]);
            }
        }


    }

    return risultato_lettura_SpecieVarieta;
}

// Dato un tipo di bene confezionamento e il suo codice di OTabelle_Parametri restituisce il mat_cod corrispondente
function TrovaMatCodPerBeneConfezionamento(Tabella_ID, piva, Cod_Bene_Confez_OTabelle_Parametri) {
    var codMat = 0;
    var beniConf = RicercaValoriParametriQualitativi(Tabella_ID, piva);
    for (var i = 0; i < beniConf.length; i++) {
        var v = beniConf[i];
        if (v.val_cod == Cod_Bene_Confez_OTabelle_Parametri) {
            codMat = v.mat_cod;
            break;
        }
    }
    return codMat;
}

function RicercaCausaliEntrataProdotto(flagVuoto) {

    if (elencoCausaliEntrataProdotto === null) {

        var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiCausaliEntrataProdotto",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "LAV_COD": 0,
                        "LAV_DES": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoCausaliEntrataProdotto = risp;
            }, null);

    }

    return elencoCausaliEntrataProdotto;
}

function RicercaGruppiFatturazione(piva) {

    if (elencoGruppiFatturazione === null) {

        var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti, piva: piva });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiGruppiFatturazione",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                elencoGruppiFatturazione = risp;
            }, null);
    }

    return elencoGruppiFatturazione;
}

function RicercaSpecie(piva, leggiTerrenoNudo) {

    if (leggiTerrenoNudo === undefined || leggiTerrenoNudo === null)
        leggiTerrenoNudo = false;

    if (elencoSpecie === null) {

        var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti, piva: piva });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiSpecieFF",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (leggiTerrenoNudo)
                    elencoSpecie = risp;
                else
                    elencoSpecie = $.Enumerable.From(risp)
                        .Where(function (x) { return (x.Veg_Cod !== "0") })
                        .Select(function (x) { return x })
                        .ToArray();
            }, null);

    }

    return elencoSpecie;
}



function RicercaVarieta(piva, veg_cod) {

    var risultato_lettura_FF;
    var found = false;

    if (elencoVarieta.length > 0) {
        for (var iValVar = 0; iValVar < elencoVarieta.length; iValVar++) {
            var v = elencoVarieta[iValVar];
            if (v.Veg_Cod == veg_cod) {
                risultato_lettura_FF = v.Dati;
                found = true;
            }
        }
    }

    if (!found) {

        var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti, piva: piva, Veg_Cod: veg_cod });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiVarietaFF",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                risultato_lettura_FF = risp;
                elencoVarieta.push({ Veg_Cod: veg_cod, Dati: risultato_lettura_FF });
            }, null);

    }

    return risultato_lettura_FF;

}

function RicercaProdotti_FF(flagVuoto, piva, Mat_Cod, Elem_Cod, soloMovimentato, ricaricaProdotti, soloLegatiALinea) {

    if (soloMovimentato === undefined)
        soloMovimentato = false;

    //In caso di solo movimentato rileggo ogni volta perché potrei avere dei prodotti movimentati dopo l'ultima lettura
    if (elencoTuttiProdotti === null || soloMovimentato || ricaricaProdotti) {

        var param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva,
            Mat_Cod: Mat_Cod,
            Elem_Cod: Elem_Cod,
            soloMovimentato: soloMovimentato,
            filters: "",
            soloLegatiALinea: soloLegatiALinea
        });
        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiProdotti_FF",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);

                if (flagVuoto === true) {
                    var objVuoto = {
                        "Piva": "",
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
                    risp.unshift(objVuoto);
                }
                elencoTuttiProdotti = risp;
            }, null);
    }

    return elencoTuttiProdotti;
}

function RicercaTipoImballaggio(flagVuoto) {

    var objVuoto;

    var risultato_lettura_FF = [{
        "Tabella_Cod": 4,
        "Tipo_Imballo_Des": "Imballaggio"
    },
    {
        "Tabella_Cod": 8,
        "Tipo_Imballo_Des": "Contenitore"
    },
    {
        "Tabella_Cod": 5,
        "Tipo_Imballo_Des": "Confezione"
    }];

    if (flagVuoto === true) {
        objVuoto = {
            "Tabella_Cod": 0,
            "Tipo_Imballo_Des": ""
        };
        risultato_lettura_FF.unshift(objVuoto);
    }

    return risultato_lettura_FF;
}

function RicercaImballaggio(flagVuoto, piva, Tabella_Cod) {

    var risultato_lettura_FF;
    var bOk = false;

    switch (Tabella_Cod) {
        case 4: //Imballaggio
            if (elencoImballaggi === null)
                bOk = true;
            else
                risultato_lettura_FF = elencoImballaggi;
            break;

        case 8: //Contenitore
            if (elencoContenitori === null)
                bOk = true;
            else
                risultato_lettura_FF = elencoContenitori;
            break;

        case 5: //Confezione
            if (elencoConfezioni === null)
                bOk = true;
            else
                risultato_lettura_FF = elencoConfezioni;
            break;
    }

    if (bOk) {
        var param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva,
            Tabella_Cod: parseInt(Tabella_Cod)
        });
        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiImballaggi",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "elem_cod": 205,
                        "imballo_des": "",
                        "mat_cod": 0,
                        "tara": 0,
                        "val_cod": 0,
                        "val_des": "",
                        "val_sigla": "",
                        "val_tabella_cod": Tabella_Cod
                    };
                    risp.unshift(objVuoto);
                }
                risultato_lettura_FF = risp;
            }, null);

        switch (Tabella_Cod) {

            case 4: //Imballaggio
                elencoImballaggi = risultato_lettura_FF;
                break;
            case 8: //Contenitore
                elencoContenitori = risultato_lettura_FF;
                break;
            case 5: //Confezione
                elencoConfezioni = risultato_lettura_FF;
                break;
        }
    }

    return risultato_lettura_FF;
}

function RicercaLineeMacchineLavorazione(flagVuoto, piva) {
    if (elencoLineeMacchineLavorazione === null || elencoLineeMacchineLavorazione === undefined) {
        var param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_LineeMacchineLavorazione + "/LeggiLineeMacchineLavorazione",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "codice": "",
                        "descrizione": "",
                        "nome_classe_algoritmi": "",
                        "funzioni_agoritmi": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoLineeMacchineLavorazione = risp;
            }, null);
    }

    return elencoLineeMacchineLavorazione;
}

function RicercaCelle(flagVuoto, tipo_fabbricato, piva, sa_cod, forza_tipo_destinazione_magazzino) {

    if (forza_tipo_destinazione_magazzino === undefined || forza_tipo_destinazione_magazzino === null) {
        forza_tipo_destinazione_magazzino = false;
    }

    if (elencoCelle === null) {
        var param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva,
            sa_cod: parseInt(sa_cod),
            CelleMagazzini: "C",
            Tipo_Fabbricato: parseInt(tipo_fabbricato),
            Forza_Tipo_Destinazione_Magazzino: forza_tipo_destinazione_magazzino
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Fabbricati + "/LeggiFabbricati",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "key_Dest": "",
                        "Tipo_Destinazione": 0,
                        "Sa_Cod": 0,
                        "Id_Destinazione": 0,
                        "Ubic_Des": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoCelle = risp;
            }, null);
    }

    return elencoCelle;

}

function RicercaMagazzini(flagVuoto, tipo_fabbricato, piva, sa_cod, gestioneWaitFrame, forza_tipo_destinazione_magazzino) {

    if (elencoMagazzini === null) {

        if (gestioneWaitFrame === null || gestioneWaitFrame === undefined) {
            gestioneWaitFrame = true;
        }

        if (forza_tipo_destinazione_magazzino === undefined || forza_tipo_destinazione_magazzino === null) {
            forza_tipo_destinazione_magazzino = false;
        }

        var param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva,
            sa_cod: parseInt(sa_cod),
            CelleMagazzini: "M",
            Tipo_Fabbricato: parseInt(tipo_fabbricato),
            Forza_Tipo_Destinazione_Magazzino: forza_tipo_destinazione_magazzino
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Fabbricati + "/LeggiFabbricati",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "key_Dest": "",
                        "Tipo_Destinazione": 0,
                        "Sa_Cod": 0,
                        "Id_Destinazione": 0,
                        "Ubic_Des": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoMagazzini = risp;
            }, null, null, gestioneWaitFrame);
    }

    return elencoMagazzini;

}

function RicercaFabbricatiOmni(flagVuoto, piva, sa_cod) {

    if (elencoMagazziniOmni === null) {

        var strfiltro = "OGenerazioni_Anagrafe_Log.Codice_Generazione In (391, 47)"; //Magazzino Ortoftutta, Reparto Prodotti Enologici

        var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti, piva: piva, sa_cod: sa_cod, strfiltro: strfiltro });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Fabbricati + "/LeggiFabbricatiOmni",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Key": '0|0',  //Sa_Cod|Fabbricato_Cod
                        "Fabbricato_Cod": 0,
                        "Fabbricato_Des_Estesa": "",
                        "Sa_Cod": 0,
                        "Modulo_Generazione": 0,
                        "Codice_Generazione": 0
                    };
                    risp.unshift(objVuoto);
                }
                elencoMagazziniOmni = risp;
            }, null);
    }

    return elencoMagazziniOmni;
}

function RicercaLineeProduzione(flagVuoto, piva) {

    var risultato_lettura_FF = [];

    var param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: piva
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiLineeProduzione",
        param,
        false,
        function (risposta) {
            risultato_lettura_FF = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                var objVuoto = {
                    "Linea_Cod": 0,
                    "Linea_Des": ""
                };
                risultato_lettura_FF.unshift(objVuoto);
            }
        }, null);

    return risultato_lettura_FF;
}

function RicercaFornitori(flagVuoto, piva) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = false;

    if (elencoFornitori === null) {

        var param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Contatti + "/LeggiFornitori_FF",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Cod_RisUm": 0,
                        "Rag_Soc": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoFornitori = risp;
            }, null);

    }

    return elencoFornitori;
}

function RicercaConfigImballiProdotto(piva) {

    if (elencoConfigImballiProdotto === null) {

        var param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiConfigImballiProdotto",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                elencoConfigImballiProdotto = risp;
            }, null);
    }

    return elencoConfigImballiProdotto;
}

function RicercaIVA_Aliquote(piva) {

    if (elencoIVA_Aliquote === null) {

        var param = kendo.stringify({
            Codice: 0,
            Aliquota: -1,
            Tipologia: -1,
            Flag_Credito_Imposta_Export: -1,
            NaturaEsclusione: "99",
            objP_server: objP_server,
            objP_utenti: objP_utenti
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_IVA_Aliquote + "/LeggiIVA_Aliquote",
            param,
            false,
            function (risposta) {
                var r = risposta.RispostaStringa;
                r = replaceAll(r, "Codice", "Cod_IVA");
                r = replaceAll(r, "Aliquota", "Aliquota_IVA");
                r = replaceAll(r, "Sigla", "Sigla_IVA");
                var risp = JSON.parse(r);
                elencoIVA_Aliquote = risp;
            }, null);
    }

    return elencoIVA_Aliquote;
}

function Leggi_MateriePrimeCampionature(piva, cal_cod) {

    var risultato_lettura_FF;

    var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti, piva: piva, progressivo: parseInt(cal_cod), tipo: "", tipo_cod: 0 });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiMateriePrimeCampionature",
        param, false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura_FF = risp;
        }, null);

    return risultato_lettura_FF;
}

function Ricerca_Categorie_Magazzino(flagVuoto, PrimaRiga_Text, PrimaRiga_Value) {

    if (elencoCategorie_Magazzino === null) {

        if (PrimaRiga_Text === undefined || PrimaRiga_Text === null) PrimaRiga_Text = "";
        if (PrimaRiga_Value === undefined || PrimaRiga_Value === null) PrimaRiga_Value = 0;

        var flagCantina = false;

        var param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            Elem_Cod: 0,
            Cau_Mov: "",
            Flag_Cantina: flagCantina
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Categorie_Magazzino + "/Leggi_Categorie_Magazzino",
            param,
            false,
            function (risposta) {
                let risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    let objVuoto = {
                        Elem_Cod: PrimaRiga_Value,
                        NomeComune: PrimaRiga_Text
                    };
                    risp.unshift(objVuoto);
                }
                elencoCategorie_Magazzino = risp;
            },
            null);
    }

    return elencoCategorie_Magazzino;
}

function Ricerca_CategorieXUnitaMisura(flagVuoto, Elem_Cod, Udm_Cod, Cau_Mov) {

    var flagCantina = false;

    var param = kendo.stringify({
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        Elem_Cod: parseInt(Elem_Cod),
        Udm_Cod: parseInt(Udm_Cod),
        Cau_Mov: Cau_Mov,
        Flag_Cantina: flagCantina,
        xFiltroAggiuntivo: ""
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_CategorieXUnitaMisura + "/Leggi_CategorieXUnitaMisura",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                let objVuoto = {
                    Elem_Cod: 0,
                    Udm_Cod: 0,
                    Udm_Des: "",
                    Udm_Sim: "",
                    NomeComune: ""
                };
                risp.unshift(objVuoto);
            }
            elencoCategorieXUnitaMisura = risp;
        },
        null);

    return elencoCategorieXUnitaMisura;
}

function RicercaElencoCompletoProdotti(objP_super_server, objP_server, objP_utenti, piva,
    Sa_Cod, Fabbricato_Cod, TipoDestinazione, Elem_Cod, soloInGiacenza, FiltroDescrizioneProdotto,
    Mode, Cau_Mov, Data_Movimento, xPUARegolamento, xLottoAccettazione, leggiUMformulati, Flag_QtaNoZero,
    xTipoPUARegolamento, Specie_Array, Varieta_Array, xFiltroAggiuntivoMateriePrime, creaGriglia,
    filtroProdottiValorizzati, flagDiversificaDesFertilizzanti, FiltroCodiceProdotto, FiltroCodiceTrappola) {

    if (FiltroCodiceTrappola === undefined || FiltroCodiceTrappola === null)
        FiltroCodiceTrappola = 0;

    if (FiltroCodiceProdotto === undefined || FiltroCodiceProdotto === null)
        FiltroCodiceProdotto = 0;

    if (flagDiversificaDesFertilizzanti === undefined || flagDiversificaDesFertilizzanti === null)
        flagDiversificaDesFertilizzanti = false;

    if (filtroProdottiValorizzati === undefined || filtroProdottiValorizzati === null)
        filtroProdottiValorizzati = -1;

    if (creaGriglia === undefined || creaGriglia === null)
        creaGriglia = false;

    if (Elem_Cod === "")
        Elem_Cod = 0;

    var elencoProdottiCompleto = "[]";
    var Specie_Array_Str = "";
    var Varieta_Array_Str = "";

    if (Specie_Array !== undefined && Specie_Array !== null && Specie_Array.length !== 0) {
        for (let i = 0; i < Specie_Array.length; i++) {
            if (i !== 0)
                Specie_Array_Str += "|";
            Specie_Array_Str += Specie_Array[i];
        }
    }

    if (Varieta_Array !== undefined && Varieta_Array !== null && Varieta_Array.length !== 0) {
        for (let i = 0; i < Varieta_Array.length; i++) {
            if (i !== 0)
                Varieta_Array_Str += "|";
            Varieta_Array_Str += Varieta_Array[i];
        }
    }

    if (Elem_Cod !== 0 || FiltroDescrizioneProdotto !== "[]") {
        var param = kendo.stringify({
            objP_super_server: objP_super_server,
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva,
            xSa_Cod: parseInt(Sa_Cod),
            xFabbricato_Cod: parseInt(Fabbricato_Cod),
            xTipoDestinazione: parseInt(TipoDestinazione),
            Elem_Cod: parseInt(Elem_Cod),
            soloInGiacenza: soloInGiacenza,
            FiltroDescrizioneProdotto: FiltroDescrizioneProdotto,
            Mode: Mode,
            Cau_Mov: Cau_Mov,
            Data_Movimento_Str: Data_Movimento,
            xPUARegolamento: xPUARegolamento,
            xLottoAccettazione: xLottoAccettazione,
            leggiUMformulati: leggiUMformulati,
            metaschema: "",
            Flag_QtaNoZero: Flag_QtaNoZero,
            xTipoPUARegolamento: xTipoPUARegolamento,
            Elenco_Specie: Specie_Array_Str,
            Elenco_Varieta: Varieta_Array_Str,
            xFiltroAggiuntivoMateriePrime: xFiltroAggiuntivoMateriePrime,
            creaGriglia: creaGriglia,
            filtroProdottiValorizzati: filtroProdottiValorizzati,
            flagDiversificaDesFertilizzanti: flagDiversificaDesFertilizzanti,
            FiltroCodiceProdotto: FiltroCodiceProdotto,
            FiltroCodiceTrappola: FiltroCodiceTrappola
        });
        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_ElencoCompletoProdotti + "/LeggiElencoCompletoProdotti",
            param,
            false,
            function (risposta) {
                elencoProdottiCompleto = risposta.RispostaStringa;
            }, null);
    }

    return elencoProdottiCompleto;
}


function RicercaElencoCompletoProdottiMultiCategoria(objP_super_server, objP_server, objP_utenti, piva,
    Sa_Cod, Fabbricato_Cod, TipoDestinazione,
    Elem_Cod_Array, Specie_Array, Varieta_Array, soloInGiacenza, FiltroDescrizioneProdotto,
    Mode, Cau_Mov, Data_Movimento, xPUARegolamento, xLottoAccettazione, leggiUMformulati,
    Flag_QtaNoZero, xTipoPUARegolamento, xFiltroAggiuntivoMateriePrime, flagDiversificaDesFertilizzanti) {

    if (flagDiversificaDesFertilizzanti === undefined || flagDiversificaDesFertilizzanti === null)
        flagDiversificaDesFertilizzanti = false;

    var elencoProdottiCompleto = "[]";
    var Elem_Cod_Str = "";
    var Specie_Array_Str = "";
    var Varieta_Array_Str = "";

    if (Elem_Cod_Array !== undefined && Elem_Cod_Array !== null && Elem_Cod_Array.length !== 0) {
        for (let i = 0; i < Elem_Cod_Array.length; i++) {
            if (i !== 0)
                Elem_Cod_Str += "|";
            Elem_Cod_Str += Elem_Cod_Array[i];
        }
    } else {
        Elem_Cod_Str = "0";
    }

    if (Specie_Array !== undefined && Specie_Array !== null && Specie_Array.length !== 0) {
        for (let i = 0; i < Specie_Array.length; i++) {
            if (i !== 0)
                Specie_Array_Str += "|";
            Specie_Array_Str += Specie_Array[i];
        }
    }

    if (Varieta_Array !== undefined && Varieta_Array !== null && Varieta_Array.length !== 0) {
        for (let i = 0; i < Varieta_Array.length; i++) {
            if (i !== 0)
                Varieta_Array_Str += "|";
            Varieta_Array_Str += Varieta_Array[i];
        }
    }

    if (Elem_Cod_Str !== "|0|" || FiltroDescrizioneProdotto !== "[]") {
        var param = kendo.stringify({
            objP_super_server: objP_super_server,
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva,
            xSa_Cod: parseInt(Sa_Cod),
            xFabbricato_Cod: parseInt(Fabbricato_Cod),
            xTipoDestinazione: parseInt(TipoDestinazione),
            Elenco_Elem_Cod: Elem_Cod_Str,
            soloInGiacenza: soloInGiacenza,
            FiltroDescrizioneProdotto: FiltroDescrizioneProdotto,
            Mode: Mode,
            Cau_Mov: Cau_Mov,
            Data_Movimento_Str: Data_Movimento,
            xPUARegolamento: xPUARegolamento,
            xLottoAccettazione: xLottoAccettazione,
            leggiUMformulati: leggiUMformulati,
            metaschema: "",
            Flag_QtaNoZero: Flag_QtaNoZero,
            xTipoPUARegolamento: xTipoPUARegolamento,
            Elenco_Specie: Specie_Array_Str,
            Elenco_Varieta: Varieta_Array_Str,
            xFiltroAggiuntivoMateriePrime: xFiltroAggiuntivoMateriePrime,
            flagDiversificaDesFertilizzanti: flagDiversificaDesFertilizzanti
        });
        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_ElencoCompletoProdotti + "/LeggiElencoCompletoProdottiMultiCategoria",
            param,
            false,
            function (risposta) {
                elencoProdottiCompleto = risposta.RispostaStringa;
            }, null);
    }

    return elencoProdottiCompleto;
}

function RicercaNumeratoriPiuDefaults(piva, saCod, lavCod, dataDocumento, fatturaAccompagnatoria) {

    if (elencoNumeratoriDefaults === null) {

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
                var risp = JSON.parse(risposta.RispostaStringa);
                elencoNumeratoriDefaults = risp;
                console.debug(risp);
            },
            function (risposta) {
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            });
    }

    return elencoNumeratoriDefaults;
}

function LastNumDocumento(piva, lavCod, anno, docNumeroSin, docNumeroDes, codRisUm, cauMov) {

    if (cauMov === undefined) {
        cauMov = "";
    }

    var lastNum = 0;

    var param = kendo.stringify({
        objP_server: objP_server,
        piva: piva,
        lavCod: lavCod,
        anno: anno,
        docNumeroSin: docNumeroSin,
        docNumeroDes: docNumeroDes,
        codRisUm: codRisUm,
        cauMov: cauMov
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Contab/Contabilita.asmx/LastNumDocumento",
        param,
        false,
        function (risposta) {
            lastNum = parseInt(risposta.RispostaStringa);
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });

    return lastNum;
}

function NuovoProgressivoUpdate(piva, anno, tipoProgressivo, docNumeroSin, docNumeroDes, sezionaleCod) {

    var newProgressivo = 0;

    var param = kendo.stringify({
        objP_server: objP_server,
        piva: piva,
        anno: anno,
        tipoProgressivo: tipoProgressivo,
        docNumeroSin: docNumeroSin,
        docNumeroDes: docNumeroDes,
        sezionaleCod: sezionaleCod
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Contab/Contabilita.asmx/NuovoProgressivoUpdate",
        param,
        false,
        function (risposta) {
            newProgressivo = parseInt(risposta.RispostaStringa);
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });

    return newProgressivo;
}

function NuovoProgressivoValBase(piva, anno, tipoProgressivo, docNumeroSin, docNumeroDes, sezionaleCod, valoreInizialeDefault) {

    var newProgressivo = 0;

    var param = kendo.stringify({
        objP_server: objP_server,
        piva: piva,
        anno: anno,
        tipoProgressivo: tipoProgressivo,
        docNumeroSin: docNumeroSin,
        docNumeroDes: docNumeroDes,
        sezionaleCod: sezionaleCod,
        valoreInizialeDefault: valoreInizialeDefault
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Contab/Contabilita.asmx/NuovoProgressivoValBase",
        param,
        false,
        function (risposta) {
            newProgressivo = parseInt(risposta.RispostaStringa);
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });

    return newProgressivo;
}

function RicercaGiacenze(Data_Movimento, Tipo_Aggregazione, SoloCampiApp, piva, Sa_Cod, Tipo_Fabbricato_Cod, Fabbricato_Cod, Elem_Cod, Pro_Cod, Mat_Cod, Lotto, isFreshAndFood, flag_QtaNoZero, flagVuoto) {

    var risultato_lettura;

    var param =
        "{" +
        " piva: '" + piva + "', objP_super_server: '" + objP_super_server + "', objP_server: '" + objP_server + "', objP_utenti: '" + objP_utenti + "'," +
        " tipo_aggregazione: " + Tipo_Aggregazione + "," +
        " soloCampiApp: '" + SoloCampiChiave + "'," +
        " sa_cod: " + Sa_Cod + "," +
        " tipo_fabbricato_cod: " + Tipo_Fabbricato_Cod + "," +
        " fabbricato_cod: " + Fabbricato_Cod + "," +
        " elem_cod: " + Elem_Cod + "," +
        " pro_cod: " + Pro_Cod + "," +
        " mat_cod: " + Mat_Cod + "," +
        " lotto: '" + Lotto + "'," +
        " Data_Movimento_Str: '" + Data_Movimento + "'," +
        " isFreshAndFood: '" + isFreshAndFood + "'," +
        " flag_QtaNoZero: '" + flag_QtaNoZero + "'" +
        " } ";

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Giacenze + "/Leggi_Giacenze",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                var objVuoto = {
                    "Chiave_Giacenza": "",
                    "Prodotto_Des": "",
                    "Giacenza": 0
                };
                risp.unshift(objVuoto);
            }
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}


function RicercaImprese(gestioneWaitFrame) {

    if (elenco_Aziende === null) {

        if (gestioneWaitFrame === null || gestioneWaitFrame === undefined) {
            gestioneWaitFrame = true;
        }

        var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Aziende + "/LeggiImpreseConFiltroUtente",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                elenco_Aziende = risp;
            },
            null, null, gestioneWaitFrame);
    }

    return elenco_Aziende;
}

function RicercaCentriAziendali(piva, soloCentriAttivi, tipoValue, gestioneWaitFrame,
    PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value) {

    if (elencoCentriAziendali === null) {

        if (PrimaRiga_Flag === undefined || PrimaRiga_Flag === null) PrimaRiga_Flag = false;
        if (PrimaRiga_Text === undefined || PrimaRiga_Text === null) PrimaRiga_Text = "";
        if (PrimaRiga_Value === undefined || PrimaRiga_Value === null) PrimaRiga_Value = "";

        var param = kendo.stringify({
            objP_server: objP_server,
            PrimaRiga_Flag: PrimaRiga_Flag,
            PrimaRiga_Text: PrimaRiga_Text,
            PrimaRiga_Value: PrimaRiga_Value,
            Piva: piva,
            Flag_SoloCentriAttivi: soloCentriAttivi,
            Tipo_Value: tipoValue
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Anagrafica/CentroAziendale.asmx/LeggiCentriConFiltroUtente",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                elencoCentriAziendali = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Centri Aziendali");
            },
            null,
            gestioneWaitFrame);
    }

    return elencoCentriAziendali;
}

function RicercaUtenti_Impostazioni(codImpost, username_1Utente_o_2SuperUser) {

    var risultato_lettura;

    var param = kendo.stringify({
        objP_Utenti: objP_utenti,
        impostazione_cod: codImpost,
        Username_1Utente_o_2SuperUser: username_1Utente_o_2SuperUser
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Utenti_Impostazioni + "/Get_Impostazioni",
        param,
        false,
        function (risposta) {
            risultato_lettura = risposta.RispostaStringa;
        },
        null);

    return risultato_lettura;
}

function RicercaPagamenti_Causali(flagVuoto, piva, cau_pagamento) {

    if (elencoPagamenti_Causali === null) {

        var param = kendo.stringify({
            piva: piva,
            cau_pagamento: cau_pagamento,
            x_filtroAggiuntivo: "",
            x_OrderBy: "",
            objP_server: objP_server
        });
        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Pagamenti_Causali + "/Leggi",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Cau_Pagamento": 0,
                        "Cau_Pagamento_Sigla": "",
                        "Cau_Pagamento_Des": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoPagamenti_Causali = risp;
            }, null);
    }

    return elencoPagamenti_Causali;

}

function RicercaGestioneVettoreSync() {

    if (elencoGestioneVettore === null) {

        var param = kendo.stringify({
            objP_server: objP_server,
            PrimaRiga_Flag: true,
            PrimaRiga_Text: "",
            PrimaRiga_Value: "0",
            flagEstero: true
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_ListControl + "/LeggiGestioneVettore",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                elencoGestioneVettore = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Gestione Vettore");
            },
            null,
            false);
    }

    return elencoGestioneVettore;

}

function RicercaListini(piva, Listino_Cod, Listino_Classe_Cod, Tipo_Classe, ChkApplicabilita, TipoIva, Tipo_Provvigione, x_OrderBy) {
    var listini = null;

    var param = kendo.stringify({
        piva: piva,
        Listino_Cod: Listino_Cod,
        Listino_Classe_Cod: Listino_Classe_Cod,
        Tipo_Classe: Tipo_Classe,
        ChkApplicabilita: ChkApplicabilita,
        TipoIva: TipoIva,
        Tipo_Provvigione: Tipo_Provvigione,
        x_OrderBy: x_OrderBy,
        objP_server: objP_server,
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Listini + "/Leggi",
        param,
        false,
        function (risposta) {
            listini = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });

    return listini;
}

function RicercaMagazzinoCellaDaLineaProdotto(piva, sa_cod, modulo_generazione, mat_cod) {
    var keyUbic = null;

    var param = kendo.stringify({
        piva: piva,
        sa_cod: sa_cod,
        modulo_generazione: modulo_generazione,
        mat_cod: mat_cod,
        objP_server: objP_server,
        objP_utenti: objP_utenti
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiDestinazioneAccettazioneDefault",
        param,
        false,
        function (risposta) {
            keyUbic = risposta.RispostaStringa;
        },
        function (risposta) {
            MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
        });

    return keyUbic;
}




function LeggiListiniPrezziValidi(piva, lav_cod, tipo_classe, elem_cod, pro_cod, mat_cod, cal_cod, qualita_cod, lotto_cod1, lotto_val1, lotto_cod2, lotto_val2, chkvettore, cod_rapporto, cod_risum, chksemina, udm_cod, udm_cod_extra, livello_prezzo, chkapplicabilita, data_movimento, chkcontatto, listino_cod_default) {

    var risultato = "";

    var param = "{objP_server: '" + objP_server + "', objP_utenti: '" + objP_utenti +
        "', piva: '" + piva + "'" +
        ", lav_cod: " + lav_cod +
        ", tipo_classe: " + tipo_classe +
        ", elem_cod: " + elem_cod +
        ", pro_cod: " + pro_cod +
        ", mat_cod: " + mat_cod +
        ", cal_cod: " + cal_cod +
        ", qualita_cod: " + qualita_cod +
        ", lotto_cod1: " + lotto_cod1 +
        ", lotto_val1: '" + lotto_val1 + "' " +
        ", lotto_cod2: " + lotto_cod2 +
        ", lotto_val2: '" + lotto_val2 + "' " +
        ", chkvettore: " + chkvettore +
        ", cod_rapporto: " + cod_rapporto +
        ", cod_risum: " + cod_risum +
        ", chksemina: " + chksemina +
        ", livello_prezzo: " + livello_prezzo +
        ", udm_cod: " + udm_cod +
        ", udm_cod_extra: " + udm_cod_extra +
        ", chkapplicabilita: " + chkapplicabilita +
        ", data: '" + data_movimento + "'" +
        ", chkcontatto: " + chkcontatto +
        ", listino_cod_default: " + listino_cod_default + "}";

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Listini + "/LeggiListiniPrezziValidi",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            risultato = risp;
        }, null);

    return risultato;
}

function Ricerca_Categorie_Magazzino_Async(flagVuoto) {

    return new Promise(function (resolve, reject) {

        if (elencoCategorie_Magazzino === null) {

            var flagCantina = false;

            var param = kendo.stringify({
                objP_server: objP_server,
                objP_utenti: objP_utenti,
                Elem_Cod: 0,
                Cau_Mov: "",
                Flag_Cantina: flagCantina
            });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_Categorie_Magazzino + "/Leggi_Categorie_Magazzino",
                param,
                function (risposta) {
                    let risp = JSON.parse(risposta.RispostaStringa);
                    if (risp.length > 1 && flagVuoto) {
                        let objVuoto = {
                            "Elem_Cod": 0,
                            "NomeComune": ""
                        };
                        risp.unshift(objVuoto);
                    }
                    elencoCategorie_Magazzino = risp;
                    resolve(elencoCategorie_Magazzino);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Categorie Magazzino");
                    reject(new Error(msgError));
                },
                null,
                false);
        } else {
            resolve(elencoCategorie_Magazzino);
        }
    });
}

function RicercaCelleEMagazzini_Async(flagVuoto, tipo_fabbricato, piva, sa_cod, CoM, forza_tipo_destinazione_magazzino) {

    //C = Solo celle
    //M = Solo magazzini
    if (CoM === null)
        CoM = "";

    if (forza_tipo_destinazione_magazzino === undefined || forza_tipo_destinazione_magazzino === null) {
        forza_tipo_destinazione_magazzino = false;
    }

    return new Promise(function (resolve, reject) {

        if (elencoCelleMagazzini === null) {

            var param = kendo.stringify({
                objP_server: objP_server,
                objP_utenti: objP_utenti,
                piva: piva,
                sa_cod: parseInt(sa_cod),
                CelleMagazzini: CoM,
                Tipo_Fabbricato: parseInt(tipo_fabbricato),
                Forza_Tipo_Destinazione_Magazzino: forza_tipo_destinazione_magazzino
            });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_Fabbricati + "/LeggiFabbricati",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    if (risp.length > 1 && flagVuoto) {
                        var objVuoto = {
                            key_Dest: "",
                            Tipo_Destinazione: 0,
                            Sa_Cod: 0,
                            Id_Destinazione: 0,
                            Ubic_Des: ""
                        };
                        risp.unshift(objVuoto);
                    }
                    resolve(risp);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Celle e Magazzini");
                    reject(new Error(msgError));
                },
                null,
                false);
        } else {
            resolve(elencoCelleMagazzini);
        }
    });
}

function RicercaCelleEMagazzini_Sync(flagVuoto, tipo_fabbricato, piva, sa_cod, CoM, desVuoto, forza_tipo_destinazione_magazzino) {

    if (elencoCelleMagazzini === null) {

        //C = Solo celle
        //M = Solo magazzini
        if (CoM === null) {
            CoM = "";
        }

        if (desVuoto === undefined || desVuoto === null) {
            desVuoto = "";
        }

        if (forza_tipo_destinazione_magazzino === undefined || forza_tipo_destinazione_magazzino === null) {
            forza_tipo_destinazione_magazzino = false;
        }

        var param = kendo.stringify({
            objP_server: objP_server,
            objP_utenti: objP_utenti,
            piva: piva,
            sa_cod: parseInt(sa_cod),
            CelleMagazzini: CoM,
            Tipo_Fabbricato: parseInt(tipo_fabbricato),
            Forza_Tipo_Destinazione_Magazzino: forza_tipo_destinazione_magazzino
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Fabbricati + "/LeggiFabbricati",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (risp.length > 1 && flagVuoto) {
                    var objVuoto = {
                        key_Dest: "",
                        Tipo_Destinazione: 0,
                        Sa_Cod: 0,
                        Id_Destinazione: 0,
                        Ubic_Des: desVuoto
                    }

                    risp.unshift(objVuoto);
                }
                else if (risp.length === 0 && flagVuoto) {
                    var objVuoto = {
                        key_Dest: "",
                        Tipo_Destinazione: 0,
                        Sa_Cod: 0,
                        Id_Destinazione: 0,
                        Ubic_Des: "NESSUN MAGAZZINO"
                    };

                    risp.unshift(objVuoto);
                }
                elencoCelleMagazzini = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Celle e Magazzini");
            });
    }

    return elencoCelleMagazzini;
}

function RicercaIVA_Aliquote_Async(piva) {

    return new Promise(function (resolve, reject) {

        if (elencoIVA_Aliquote === null) {

            var param = kendo.stringify({
                Codice: 0,
                Aliquota: -1,
                Tipologia: -1,
                Flag_Credito_Imposta_Export: -1,
                NaturaEsclusione: "99",
                objP_server: objP_server,
                objP_utenti: objP_utenti
            });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_IVA_Aliquote + "/LeggiIVA_Aliquote",
                param,
                function (risposta) {
                    var r = risposta.RispostaStringa;
                    r = replaceAll(r, "Codice", "Cod_IVA");
                    r = replaceAll(r, "Aliquota", "Aliquota_IVA");
                    r = replaceAll(r, "Sigla", "Sigla_IVA");
                    var risp = JSON.parse(r);
                    resolve(risp);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Aliquote IVA");
                    reject(new Error(msgError));
                },
                null,
                false);
        } else {
            resolve(elencoIVA_Aliquote);
        }
    });
}

function RicercaImballaggio_Async(flagVuoto, piva, Tabella_Cod) {

    var risultato_lettura_FF;
    var bOk = false;

    return new Promise(function (resolve, reject) {

        switch (Tabella_Cod) {
            case 4: //Imballaggio
                if (elencoImballaggi === null)
                    bOk = true;
                else
                    risultato_lettura_FF = elencoImballaggi;
                break;

            case 8: //Contenitore
                if (elencoContenitori === null)
                    bOk = true;
                else
                    risultato_lettura_FF = elencoContenitori;
                break;

            case 5: //Confezione
                if (elencoConfezioni === null)
                    bOk = true;
                else
                    risultato_lettura_FF = elencoConfezioni;
                break;
        }

        if (bOk) {
            var param = kendo.stringify({
                objP_server: objP_server,
                objP_utenti: objP_utenti,
                piva: piva,
                Tabella_Cod: parseInt(Tabella_Cod)
            });
            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiImballaggi",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    if (risp.length > 1 && flagVuoto) {
                        var objVuoto = {
                            "elem_cod": 205,
                            "imballo_des": "",
                            "mat_cod": 0,
                            "tara": 0,
                            "val_cod": 0,
                            "val_des": "",
                            "val_sigla": "",
                            "val_tabella_cod": Tabella_Cod
                        };
                        risp.unshift(objVuoto);
                    }
                    risultato_lettura_FF = risp;
                    resolve(risultato_lettura_FF);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Imballi tipo " + Tabella_Cod);
                    reject(new Error(msgError));
                },
                null,
                false
            );


        } else {
            resolve(risultato_lettura_FF);
        }

    });
}


function RicercaParametriQualitativi_Async(flagVuoto, piva) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = true;

    return new Promise(function (resolve, reject) {

        if (elencoParametriQualitativi === null) {

            var param = kendo.stringify({ objP_server: objP_server, objP_utenti: objP_utenti, piva: piva });

            ajaxAgronica(GetUrlLetturaTabelleGestionali() + indirizzohttp_Leggi_Tabelle_FF_WS + "/LeggiParametriQualitativi",
                param,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    if (risp.length > 1 && flagVuoto) {
                        var objVuoto = {
                            "Tabella_ID": 0,
                            "Tabella_Des": "",
                            "Tabella_Cod_Des": "",
                            "Tipo": 0,
                            "ChkObbligatorio": 0,
                            "Tabella_Key_Rif": ""
                        };
                        risp.unshift(objVuoto);
                    }
                    resolve(risp);
                },
                function (risposta) {
                    var msgError = mostraErrore(risposta, "Errore WS Parametri Qualitativi gestiti");
                    reject(new Error(msgError));
                },
                null,
                false);
        } else {
            resolve(elencoParametriQualitativi);
        }
    });

}

function LeggiRapportiContabili(flagVuoto, gestioneWaitFrame, flagCliente, flagFornitore, flagDipendente, flagTerzista, flagLegale, flagAgente, flagConsulente) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = true;

    if (flagCliente === undefined || flagCliente === null)
        flagCliente = false;

    if (flagFornitore === undefined || flagFornitore === null)
        flagFornitore = false;

    if (flagDipendente === undefined || flagDipendente === null)
        flagDipendente = false;

    if (flagTerzista === undefined || flagTerzista === null)
        flagTerzista = false;

    if (flagLegale === undefined || flagLegale === null)
        flagLegale = false;

    if (flagAgente === undefined || flagAgente === null)
        flagAgente = false;

    if (flagConsulente === undefined || flagConsulente === null)
        flagConsulente = false;

    var elencoRapportiContabili = Array();

    var param = kendo.stringify({
        objP_server: objP_server,
        cliente: flagCliente,
        fornitore: flagFornitore,
        dipendente: flagDipendente,
        terzista: flagTerzista,
        legale: flagLegale,
        agente: flagAgente,
        consulente: flagConsulente
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Contab + "/LeggiRapportiContabili",
        param,
        false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            if (flagVuoto === true) {
                var objVuoto = {
                    "Cod_Rapporto": "0",
                    "Rapporto_Des": "",
                    "Sa_Cod": -1
                };
                risp.unshift(objVuoto);
            }
            elencoRapportiContabili = risp;
        },
        function (risposta) {
            var msgError = mostraErrore(risposta, "Errore WS Contabilita");
        },
        null,
        gestioneWaitFrame);

    return elencoRapportiContabili;
}

function LeggiNazioni(flagVuoto, gestioneWaitFrame) {

    if (flagVuoto === undefined || flagVuoto === null)
        flagVuoto = true;

    if (elencoNazioni === undefined || elencoNazioni === null) {
        var param = kendo.stringify({
            objP_server: objP_server
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Nazioni + "/Leggi",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                if (flagVuoto === true) {
                    var objVuoto = {
                        "Codice": "",
                        "Descrizione": ""
                    };
                    risp.unshift(objVuoto);
                }
                elencoNazioni = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Nazioni");
            },
            null,
            gestioneWaitFrame);
    }

    return elencoNazioni;
}

function LeggiRegioni(gestioneWaitFrame) {

    if (elencoRegioni === undefined || elencoRegioni === null) {
        var param = kendo.stringify({
            objP_server: objP_server
        });

        ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_Regioni + "/Leggi",
            param,
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                elencoRegioni = risp;
            },
            function (risposta) {
                var msgError = mostraErrore(risposta, "Errore WS Regioni");
            },
            null,
            gestioneWaitFrame);
    }

    return elencoRegioni;
}



function RicercaDocumentiConAgenda(piva, id_agenda) {

    var param = kendo.stringify({
        objP_server: objP_server,
        piva: piva,
        id_agenda: id_agenda
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + indirizzohttp_RicercaDocumenti_Con_Agenda + "/Leggi_documenti_con_agenda",
        param,
        false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            elencoDocumentiConAgenda = risp;
        },
        function (risposta) {
            var msgError = mostraErrore(risposta, "Errore WS Alert_Entita");
        },
        null,
        elencoDocumentiConAgenda);

    return elencoDocumentiConAgenda;
}
