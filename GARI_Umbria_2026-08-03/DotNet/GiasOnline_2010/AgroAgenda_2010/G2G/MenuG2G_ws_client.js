function ElencoConfigurazioni(options) {
    ajaxAgronica(indirizzohttp + "/ElencoConfigurazioniG2G",
        "{ }",
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
            LeggiConfigurazione();
        });
}

function LeggiConfigurazione() {
    var id_cfg = $("#"+ ddlConfigurazioni_ClientID).data("kendoDropDownList").value();
    if (id_cfg !== "") {
        ajaxAgronica(indirizzohttp + "/LeggiConfigurazioneG2G",
            "{ID_Cfg: " + id_cfg + "}",
            function (risposta) {
                $("#txtConfigurazione").val(risposta.RispostaStringa);
            });
    }
}

function EsportaDatiG2G(id_cfg, xml_cfg) {
    var parametri = kendo.stringify({
        ID_Cfg: id_cfg,
        XML_Cfg: btoa(xml_cfg)
    });
    ajaxAgronica(indirizzohttp + '/EsportaDatiG2G',
        parametri,
        function (risposta) {
            if (risposta.RispostaOK) {
                $("#txtLog").val(risposta.RispostaStringa.replace(/<br\s*[\/]?>/gi, "\n"));
                wnd.content(risposta.RispostaStringa);
                wnd.center().open();
            } else {
                kendo.alert(risposta.Errore);
            }
        });
}


function ElaboraLogG2G() {
    var parametri = kendo.stringify({ log: $("#txtLog").val() });
    ajaxAgronica(indirizzohttp + '/ElaboraLogG2G',
        parametri,
        function (risposta) {
            if (risposta.RispostaOK) {
                $("#txtLogErrori").val(risposta.RispostaStringa);
            } else {
                kendo.alert(risposta.Errore);
            }
        });
}
