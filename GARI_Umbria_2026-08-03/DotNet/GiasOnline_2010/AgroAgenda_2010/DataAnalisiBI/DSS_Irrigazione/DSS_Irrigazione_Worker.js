

self.addEventListener("message", function (e) {

    let req = new XMLHttpRequest();
    req.open("POST", e.data.baseurl + "/CaricaIndicatori_V2", false);
    req.setRequestHeader("Content-type", "application/json; charset=utf-8");
    req.onreadystatechange = function () {

        let indicatori = [];

        if (req.readyState === 4 && req.status === 200) {

            let json_risp = JSON.parse(req.responseText).d;
            if (json_risp.RispostaOK) {

                indicatori = JSON.parse(json_risp.RispostaStringa);
            } else {

                console.log("ERRORE DSS Irrigazione");
            }

        } else {

            console.log("ERRORE DSS Irrigazione");
        }

        self.postMessage(indicatori);
    };
    req.send();
});

