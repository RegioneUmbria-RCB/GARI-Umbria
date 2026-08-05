var indirizzohttp = ""; //"./Lavorazione_Config.aspx";

function ImpostaIndirizzoHttp(httpPath) {
    console.log("criteri: " + httpPath);
    indirizzohttp = httpPath;
}

function RicercaCriteriAggregazione(options) {
    
    let param = kendo.stringify({piva: KendoDDL("ddlImpresa").value()});

    ajaxAgronica(indirizzohttp + "/Leggi_CriteriAggregazione",
        param,
        function (risposta) {
            options.success(JSON.parse(risposta.RispostaStringa));
        }, null);
}

function nuovoModificaCriterio(paramQual, azione) {

    if (KendoDDL("ddlAzienda").value() === '' || KendoDDL("ddlAzienda").value() === undefined ||
        KendoDDL("Lavorazione").value() === '0' || KendoDDL("Lavorazione").value() === '' || KendoDDL("Lavorazione").value() === undefined ||
        KendoDDL("LineaLav").value() === '' || KendoDDL("LineaLav").value() === undefined) {
        kendo.alert("Riempire i campi Impresa, Lavorazione e Linea Lavorazione per proseguire")
        return 0;
    }

    let param = kendo.stringify({
        piva: KendoDDL("ddlAzienda").value(),
        tipoLav: KendoDDL("Lavorazione").value(),
        tipoLavLinea: KendoDDL("LineaLav").value(),
        aggregaFornitore: KendoDDL("fornitore").value(),
        aggregaSpecie: KendoDDL("specie").value(),
        aggregaVarieta: KendoDDL("varieta").value(),
        aggregaRegolamento: KendoDDL("regolamento").value(),
        aggregaLotto: KendoDDL("lotto").value(),
        aggregaLottoUscita: KendoDDL("lottoUscita").value(),
        aggregaProdotto: KendoDDL("prodotto").value(),
        aggregaProdottoUscita: KendoDDL("prodottoUscita").value(),
        aggregaCella: KendoDDL("cella").value(),
        aggregaCellaUscita: KendoDDL("cellaUscita").value(),
        aggregaUnitaMisura: KendoDDL("unitaMisura").value(),
        paramQual: paramQual,
        azione: azione
    });
    ajaxAgronica(indirizzohttp + "/Aggiorna_CriteriAggregazione",
        param,
        function (risposta) {
            gestisciRispostaFinestra(risposta.RispostaStringa);
        }, null);
}

function RiempiDdlAzienda(options) {
    var parametri = ""
    
    parametri = kendo.stringify({ "objP_server": objP_server, "objP_utenti": objP_utenti });
    
    ajaxAgronicaSync(indirizzohttp + "/Carica_Imprese_Idonee",
        parametri,
        true,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            objVuoto = { "piva": "", "rag_soc": "TUTTE", "forma_giuridica": "0", "flagPubblica": -1 };
            if (cIdPiva != "") {
                //Potrei essere entrato con un'azienda NON conforme al tipo richiesta, quindi potrei non averla nella lista delle aziende selezionabili
                let tmp = risp.filter(x => x.piva == cIdPiva)[0];
                if (tmp !== undefined) {
                    risp.splice(risp.indexOf(tmp), 1)
                    risp.unshift(tmp);
                }
            }
            risp.unshift(objVuoto);
            //options.success(risp.slice(0,100));
            options.success(risp);
            //resolve(risp);
        }, null);
}

function RicercaPreparazioniGeneriche(options) {

    var param = "{piva: 'AAAAAAAAAAA'}";
    var risultato_lettura = [];

    ajaxAgronicaSync(indirizzohttp + "/LeggiPreparazioniGeneriche",
        param,
        false,
        function (risposta) {
            risultato_lettura = JSON.parse(risposta.RispostaStringa);

            var objVuoto = {
                "Preparazione_Des": "",
                "Preparazione_Cod": 0
            };
            risultato_lettura.unshift(objVuoto);
            options.success(risultato_lettura);
        },
        null);

    //elencoTipologieLavorazioniAssegnaLotto = risultato_lettura;

}

function RicercaLineePreparazione(options) {

    var param = kendo.stringify({ "piva": KendoDDL("ddlAzienda").value(), "codGenerazione": KendoDDL("Lavorazione").value() });

    var risultato_lettura = [];

    ajaxAgronicaSync(indirizzohttp + "/LeggiLineePreparazione",
        param,
        false,
        function (risposta) {
            risultato_lettura = JSON.parse(risposta.RispostaStringa);

            var objVuoto = {
                "descrizione": "",
                "Preparazione_Cod": 0
            };
            risultato_lettura.unshift(objVuoto);
            options.success(risultato_lettura);
            KendoDDL('LineaLav').enable(true)
        },
        null);
}

function cancellaCriterio() {
    var grid = $("#griglia_Aggregazioni").data("kendoGrid");
    var datiCancellati = grid.dataSource.destroyed();
        
    let param = kendo.stringify({ "cancellate": JSON.stringify(datiCancellati) })

    ajaxAgronicaSync(indirizzohttp + "/CancellaCriterioAggregazione",
        param,
        false,
        function (risposta) {
            risultato_lettura = JSON.parse(risposta.RispostaStringa);
        },
        null);
}
