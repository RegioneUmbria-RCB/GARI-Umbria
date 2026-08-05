function raccolteConfUC_CercaRaccolte(_piva, _dataMov,  _idMovDetConf) {
    var elenco = [];

    var parametri = kendo.stringify({
        piva: _piva,
        dataMovimento: _dataMov,
        idMovDet: _idMovDetConf
    });
    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/CercaRaccolte",
        parametri,
        false,
        function (rispServer) {
            elenco = JSON.parse(rispServer.RispostaStringa);
        },
        null);

    return elenco;
}

function raccolteConfUC_WSImpostazioneProponiPeso(_piva, _saCod, _vegCod) {
    var impostazione = null;

    var parametri = kendo.stringify({
        piva: _piva,
        saCod: _saCod,
        vegCod: _vegCod
    });
    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/LeggiImpostazioneProponiPesoRaccolta",
        parametri,
        false,
        function (rispServer) {
            impostazione = rispServer.RispostaStringa;
        },
        null);

    return impostazione;
}