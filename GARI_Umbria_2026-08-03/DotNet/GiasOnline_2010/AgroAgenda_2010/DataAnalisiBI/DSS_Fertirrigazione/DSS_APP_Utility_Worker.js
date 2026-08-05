
self.addEventListener("message", function (e) {

    let id = e.data.id;
    if (id === 1) {

        let oStazioniAvail = {};
        let centriXstazione = [];
        let idx = 0;
        while (idx < e.data.stazioniXcentro.length) {

            let sxc = e.data.stazioniXcentro[idx++];
            let prop = sxc.sorgente + "_" + sxc.stazione;

            if (!oStazioniAvail.hasOwnProperty(prop)) {

                oStazioniAvail[prop] = centriXstazione.length;
                centriXstazione.push({ sorgente: sxc.sorgente, stazione: sxc.stazione, elem_id: [] });
            } 

            centriXstazione[oStazioniAvail[prop]].elem_id.push(sxc.elem_id)
        }

        idx = 0;
        while (idx < centriXstazione.length) {

            let cxs = centriXstazione[idx++];

            let req = new XMLHttpRequest();
            req.open("POST", e.data.baseurl + "/LeggiStazione", false);
            req.setRequestHeader("Content-type", "application/json; charset=utf-8");
            req.onreadystatechange = function (e) {

                let req = e.currentTarget;
                if (req.readyState === 4 && req.status === 200) {

                    let json_risp = JSON.parse(req.responseText).d;

                    if (json_risp.RispostaOK) {

                        self.postMessage({ elem_id: cxs.elem_id, stazione: JSON.parse(json_risp.RispostaStringa) });
                    } 
                }
            };
            req.send(JSON.stringify({ tipo_sorgente: cxs.sorgente, stazione_cod: cxs.stazione }));
        }

    } else {

        //potrei fare altro...
    }
});

