function ws_PrenotazionePiante_Riepilogo(Data) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            Data: Data
        });

        ajaxAgronica("PrenotazionePiante_Riepilogo.aspx/PrenotazionePiante_Riepilogo",
            parametri,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, null, false);
    });
}

function ws_PrenotazionePiante_RiepilogoSintetico(Data) {
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({
            Data: Data
        });

        ajaxAgronica("PrenotazionePiante_Riepilogo.aspx/PrenotazionePiante_RiepilogoSintetico",
            parametri,
            function (risposta) {
                resolve(risposta.RispostaStringa);
            }, null, null, false);
    });
}

function Popola_objColors(){
    return new Promise((resolve, reject) => {

        var parametri = kendo.stringify({});

        ajaxAgronica("PrenotazionePiante_Riepilogo.aspx/Colori_Stati",
            parametri,
            function (risposta) {
                resolve(JSON.parse(risposta.RispostaStringa));
            }, null, null, false);
    });
}