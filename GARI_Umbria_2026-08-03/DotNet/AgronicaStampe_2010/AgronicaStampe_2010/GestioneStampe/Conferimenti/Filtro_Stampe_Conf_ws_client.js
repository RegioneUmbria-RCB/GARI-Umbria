/* File Created: luglio 14, 2020 */

function EseguiEstrazione(objFilters) {
    ajaxAgronica(
        "Filtro_Stampe_Conf.aspx/EseguiEstrazione",
        kendo.stringify({ params: kendo.stringify(objFilters) }),
        function (risposta) {
            //console.log("ok: " + risposta.RispostaStringa);
            try {
                var rispJson = JSON.parse(risposta.RispostaStringa);

                window.open(
                    rispJson.paginaDaRichiamare + "?" + rispJson.queryString,
                    "_blank"
                );
            } catch (e) {
                // Si verifica errore nel caso in cui RispostaStringa non sia un oggetto json, ma un messaggio, come per esempio "report selezionato dismesso"
                MessaggioTuttoOK_Bootstrap(risposta.RispostaStringa, "DIV_Messaggi");
            }
        },
        function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js dovuto al codice...
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'Filtro_Stampe_Conf.aspx/EseguiEstrazione', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server, magari per la mancanza di un qualche dato che notifico all'utente
                var jsonArrErrori = JSON.parse(risposta.Errore);
                var stringArrErrori = jsonArrErrori.join("<br/>");
                MessaggioErrore_Bootstrap("Si sono verificati i seguenti errori:<br/>" + stringArrErrori, "DIV_Messaggi");
            }
        }
    );
}

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
 
/**
 * Restituisce tutti i conferenti di primo livello
 * @param {any} objP_server
 * @param {string} piva
 * @param {number} superUserAccettazioneConGerarchia Accetta due valori: 0 => gestione senza gerarchia, 1 => gestione gerarchica
 * @param {string} filtroUtente
 */
function RicercaConferentiPrimoLivello(objP_server, piva, superUserAccettazioneConGerarchia, filtroUtente) {
    var conferenti = [];
    var tipoRapporto = 6; // Il codice 6 permette di prelevare i soli soggetti che sono considerati contemporaneamente clienti e fornitori
    var codTipologiaSoggettoDaRicercare = 0;
    if (superUserAccettazioneConGerarchia === 1) {
        codTipologiaSoggettoDaRicercare = 1; 
        /* Imposto il valore fisso a 1 perché corrisponde alla ricerca per Conferenti; altri valori disponibili sono:
         * 2 => Prima Cooperativa alias Cessionario
         * 3 => Seconda/o Cooperativa/Cessionario
         * 4 => Produttore
         */
    }
    var param = kendo.stringify({
        objP_server: objP_server,
        piva: piva,
        rapportoAttivo: false, // indica se estrarre solo i soggetti attivi, alla dataValidita seguente
        dataValidita: null,
        tipoRapporto: tipoRapporto, // vedi documentazione per valori disponibili
        filtraSoloValidi: true, // serve come filtro a posteriori della query, filtra sul campo di select indicato nel punto precedente
        cercaSoloValidi: true, // imposta un filtro sulla query, per scegliere se estrarre solo i soggetti che hanno il tipo rapporto indicato oppure se estrarli tutti mantenendo in select una colonna che indica questi ultimi come validi
        includiIndirizzo: true,
        accettazioneConGerarchia: codTipologiaSoggettoDaRicercare,
        pivaPadreGerarchia: piva, 
        testoRicerca: filtroUtente === undefined ? "" : filtroUtente,
        codRisUm: 0,
        codContatto: "",
        checkRaccolte: false,
        dataFineRaccolte: new Date()
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Anagrafica/Contatti.asmx/LeggiRapportoDocumenti",
        param, false,
        function(risposta) {
            conferenti = JSON.parse(risposta.RispostaStringa);
        }, null);

    return conferenti;
}


function LeggiProdotti() {

    let xFiltroAggiuntivoMateriePrime = "";
    if (elemCod === 210 || elemCod === 310)
        xFiltroAggiuntivoMateriePrime = $(hf_filtroMateriePrimeConferimento).val();

    let specie = "";

    let varieta = "";

    if (Get_KendoDDLValue("ddlSpecie")!=="" && parseInt(Get_KendoDDLValue("ddlSpecie")) !== -1)
        specie = Get_KendoDDLValue("ddlSpecie").toString();


    if (Get_KendoDDLValue("ddlVarieta") !== "" && parseInt(Get_KendoDDLValue("ddlVarieta")) !== 0)
        varieta = Get_KendoDDLValue("ddlVarieta").toString();

    var param = kendo.stringify({
        objP_super_server: objP_super_server,
        objP_server: objP_server,
        objP_utenti: objP_utenti,
        piva: $(cIdPiva).val(),
        xSa_Cod: 0,
        xFabbricato_Cod: 0,
        xTipoDestinazione: 0,
        Elem_Cod: elemCod,
        soloInGiacenza: false,
        FiltroDescrizioneProdotto: "",
        Mode: "",
        Cau_Mov: "7300",
        Data_Movimento_Str: new Date(),
        xPUARegolamento: 0,
        xLottoAccettazione: "",
        leggiUMformulati: false,
        metaschema: "",
        Flag_QtaNoZero: false,
        xTipoPUARegolamento: 0,
        Elenco_Specie: specie,
        Elenco_Varieta: varieta,
        xFiltroAggiuntivoMateriePrime: xFiltroAggiuntivoMateriePrime,
        creaGriglia: true,
        filtroProdottiValorizzati: -1,
        flagDiversificaDesFertilizzanti: false,
        FiltroCodiceProdotto: 0,
        FiltroCodiceTrappola: 0
    });

    ajaxAgronicaSync(GetUrlLetturaTabelleGestionali() + "Anagrafica/Prodotti.asmx/LeggiElencoCompletoProdotti",
        param,
        false,
        function (risposta) {
            $('#hdgridProdotto').val(risposta.RispostaStringa);
        }, null);

}


function LeggiDocPrefissiSuffissi(_piva, _preSuf) {
    var risp = [];

    var parametri = kendo.stringify({
        piva: _piva,
        preSuf: _preSuf
    });

    ajaxAgronicaSync(
        "Filtro_Stampe_Conf.aspx/LeggiDocPrefissiSuffissi",
        parametri,
        false,
        function (risposta) {
            try {
                risp = JSON.parse(risposta.RispostaStringa);
            } catch (e) {
                MessaggioErrore_Bootstrap("Si è verificato il seguente errore:<br/>" + e, "DIV_Messaggi");
            }
        },
        function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js dovuto al codice...
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'Filtro_Stampe_Conf.aspx/LeggiDocPrefissiSuffissi', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server, magari per la mancanza di un qualche dato che notifico all'utente
                MessaggioErrore_Bootstrap("Si è verificato il seguente errore:<br/>" + risposta.Errore, "DIV_Messaggi");
            }
        }
    );

    return risp;
}

function LeggiElencoStampanti(_piva) {
    var risp = [];

    var parametri = kendo.stringify({
        piva: _piva,
    });

    ajaxAgronicaSync(
        "Filtro_Stampe_Conf.aspx/LeggiElencoStampanti",
        parametri,
        false,
        function (risposta) {
            try {
                risp = JSON.parse(risposta.RispostaStringa);
            } catch (e) {
                MessaggioErrore_Bootstrap("Si è verificato il seguente errore:<br/>" + e, "DIV_Messaggi");
            }
        },
        function (risposta) {
            if (typeof risposta === "string") {
                // In questo caso ho un errore js dovuto al codice...
                MessaggioErrore_Bootstrap(
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'Filtro_Stampe_Conf.aspx/LeggiDocPrefissiSuffissi', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server, magari per la mancanza di un qualche dato che notifico all'utente
                MessaggioErrore_Bootstrap("Si è verificato il seguente errore:<br/>" + risposta.Errore, "DIV_Messaggi");
            }
        }
    );

    return risp;
}
