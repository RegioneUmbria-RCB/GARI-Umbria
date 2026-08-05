


self.addEventListener("message", function (e) {

    let reqParams = e.data.params;
    let url = e.data.baseurl;

    console.log("Parameters ready, calling evaluate method");
    console.log(reqParams);

    evaluateOnAllStations(url, reqParams);
});



function evaluateOnAllStations(url, reqParams) {

    let params = {
        DataDa: reqParams.DataDa,
        DataA: reqParams.DataA,
        SorgentiMeteo: [],
        ModelloPrevisionale: reqParams.ModelloPrevisionale,
        Veg_Cod: reqParams.Veg_Cod,
        Av_Cod: reqParams.Av_Cod,
        Algoritmo: reqParams.Algoritmo,
        ParametriAggiuntivi: reqParams.ParametriAggiuntivi
    };

    let chunkSize = 20;
    let idx = 0;

    while (idx < reqParams.Sorgente.length) {

        const chunk = reqParams.Sorgente.slice(idx, idx + chunkSize);

        params.SorgentiMeteo = [];

        chunk.forEach(s => {

            params.SorgentiMeteo.push({
                Tipo: reqParams.TipoSorgente,
                Stazione: s.id
            });
        });

        sendRequest(url, params);

        idx += chunkSize;
    };

    self.postMessage(null);
}



function sendRequest(url, params) {

    let req = new XMLHttpRequest();

    req.open("POST", url + "/ModelliPrevisionali_Indicatore", false);

    req.setRequestHeader("Content-type", "application/json; charset=utf-8");

    req.onreadystatechange = function (e) {

        let req = e.currentTarget;
        if (req.readyState === 4 && req.status === 200) {

            let json_risp = JSON.parse(req.responseText).d;

            if (json_risp.RispostaOK) {

                let elenco = json_risp.RispostaStringa;

                elenco.forEach(r => {

                    let data = {
                        id: r.StazioneSorgente,
                        color: '#ccc',
                        value: null
                    };

                    if (r.RisultatoElaborazione.Status === 0) {

                        data.color = r.RisultatoElaborazione.Bands.find(b => r.RisultatoElaborazione.Value <= b.Value).Color;
                        data.value = r.RisultatoElaborazione.Value;
                    }

                    self.postMessage(data);
                });
            }

        } else {

            console.log(req);
        }
    };

    req.send(JSON.stringify(params));
}



