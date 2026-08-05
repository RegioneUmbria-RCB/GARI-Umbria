
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

    let url = window.location.href.split("?")[0] + "/CaricaIndicatori";

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

function GetSpecie(vegArray) {

    var rval = [];

    ajaxAgronicaSync(location.pathname + "/SpecieCaricaCombo",
        "{}",
        true,
        function (risposta) {

            rval = JSON.parse(risposta.RispostaStringa);
        },
        null
    );

    let idx0 = 0;
    let cnt1 = vegArray.length;
    let found = false;
    while (idx0 < rval.length) {
        idx1 = 0;
        found = false;
        while (!found && idx1 < cnt1) {
            found = (rval[idx0].veg_cod == vegArray[idx1]);
            idx1++;
        }
        if (!found) {
            rval.splice(idx0, 1);
        } else {
            idx0++;
        }
    }

    return rval;
}

