function GeneraRiepilogoUtilizzoSuperficiMono(_piva, _saCod, _dataRif) {
    let params = {
        piva: _piva,
        saCod: _saCod,
        dataRif: _dataRif
    };

    ajaxAgronica("./Filtro_RiepilogoUtilizzoSuperfici.aspx/Stampa",
        JSON.stringify(params),
        function (risposta) {
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
                    "Si è verificato il seguente errore javascript nell'esecuzione della chiamata ajax 'Filtro_RiepilogoUtilizzoSuperfici.aspx/Stampa', " +
                    "controllare i parametri passati: " + risposta,
                    "DIV_Messaggi");
            } else {
                // ...mentre in questo c'è stato un errore lato server, magari per la mancanza di un qualche dato che notifico all'utente
                var jsonArrErrori = JSON.parse(risposta.Errore);
                var stringArrErrori = jsonArrErrori.join("<br/>");
                MessaggioErrore_Bootstrap("Si sono verificati i seguenti errori:<br/>" + stringArrErrori, "DIV_Messaggi");
            }
        }
    )
}