function SportelloPCB() {
    return new Promise((resolve, reject) => {
        var parametri = kendo.stringify({
            Piva: Qs_Piva
        });

        ajaxAgronica("../PC_Bilancio/PCB_Inserimento.aspx/SportelloPCB",
            parametri,
            function (risposta) {
                resolve(JSON.parse(risposta.RispostaStringa));
            }, null, null, false);
    });
}

function CaricaPiogge() {
    let tipoSorgente = $(Meteo_TipoSorgente_Real).val();
    if (tipoSorgente == "" && $(Meteo_ChkAgenda).val() == '1')
        tipoSorgente = "0" //Il WS si aspetta un intero, con stringa vuota si schianta...

    let param = {
        PIVA: Qs_Piva,
        SaCod: $($(Cmb_Centro) + ' option:selected').val(),
        Anno: $(Txt_Anno).val(),
        leggiDaAgenda: $(Meteo_ChkAgenda).val(),
        tipoSorgente: tipoSorgente,
        sorgente: $(Meteo_Sorgente).val(),
        regolamento: $($(ddlRegolamento) + ' option:selected').val()
    };

    ajaxAgronicaSync("../PC_Bilancio/PCB_Inserimento.aspx/Carica_Piogge_WM",
        JSON.stringify(param),
        true,
        function (risposta) {
            let values = JSON.parse(risposta.RispostaStringa);

            $(Txt_Pioggia).val(values.Pioggia);
            $(Txt_Pioggia_Febbraio).val(values.Pioggia_Febbraio);
        },
        null
    );

    return false;
}

function RicaricaTipoSorgente() {
    let sorgenti = [];

    ajaxAgronicaSync("../MeteoWS.aspx/RicaricaSorgenteDati",
        JSON.stringify({}),
        false,
        function (risposta) {
            sorgenti = JSON.parse(risposta.RispostaStringa);
        },
        function (risposta) {
        }
    );
    return sorgenti;
}

function RicaricaOrigineDati() {

    let SaCod = $($(Cmb_Centro) + ' option:selected').val();

    let tipoSorgente = cmbTipoSorgente.value();

    if (tipoSorgente == "") {
        cmbOrigineDati.setDataSource([]);
        cmbOrigineDati.refresh();
        return;
    }

    let param = "{ PIVA: '" + Qs_Piva + "', SaCod: " + SaCod + ", TipoSorgenteDati: " + tipoSorgente + " }";

    ajaxAgronicaSync("../MeteoWS.aspx/RicaricaOrigineDati",
        param,
        true,
        function (risposta) {
            OrigineDati = JSON.parse(risposta.RispostaStringa);
            cmbOrigineDati.setDataSource(OrigineDati);
            cmbOrigineDati.refresh();
            cmbOrigineDati.select(-1);
        },
        null
    );
}