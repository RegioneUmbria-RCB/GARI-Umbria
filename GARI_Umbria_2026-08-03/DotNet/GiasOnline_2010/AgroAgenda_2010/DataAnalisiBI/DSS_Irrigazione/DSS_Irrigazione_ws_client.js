
function DescrizioneServizio() {

    let result = null;

    let url = window.location.href.split("?")[0] + "/MessaggioANBI";

    ajaxAgronicaSync(url,
        "",
        false,
        function (risposta) {
            result = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
        },
        null,
        false
    );

    return result;
}

function CaricaIndicatori() {

    let indicatori = [];

    let url = window.location.href.split("?")[0] + "/CaricaIndicatori_V2";

    ajaxAgronicaSync(url,
        "",
        false,
        function (risposta) {

            indicatori = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
            console.log("ERRORE DSS Irrigazione");
        },
        null,
        false
    );

    return indicatori;
}
