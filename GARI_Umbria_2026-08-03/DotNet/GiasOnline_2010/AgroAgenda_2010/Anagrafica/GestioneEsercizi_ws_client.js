//////////////////////////////////////////////////////////
// Gestione Esercizi
//////////////////////////////////////////////////////////

var indirizzohttp = "./GestioneEsercizi.aspx";

function LeggiEsercizi(options) {

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        poliennali: $('#checkPoliennali').is(":checked"),
        arboree: $('#checkArboree').is(":checked"),
        filtroTemporale: false
    });

    ajaxAgronica(indirizzohttp + "/LeggiAnagraficaEsercizi",
        param,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            if (risp.length > 0) {
                if (parent && parent.apriGestioneEsercizi) {
                    parent.apriGestioneEsercizi();
                }
            }
            options.success(risp);
        }, null);
}

function EseguiAzioneEsercizi(esercizi) {

    var azione = Get_KendoDDLValue("ddlAzioneEsercizio");
    var dataChiusura = $('input[name$="txtDataChiusura"]').val();

    var param = kendo.stringify({
        azione: azione,
        dataChiusura: dataChiusura,
        esercizi: kendoEscapeOggetto(esercizi)
    });

    ajaxAgronicaSync(indirizzohttp + "/EseguiAzioneEsercizi", param, false,
        function (risposta) {
            if (risposta.RispostaOK) {
                MessaggioTuttoOK_Bootstrap(TraduzioneMultiResx(gestioneEserciziResx, "AzioneEseguitaCorrettamente", "Azione eseguita correttamente"), "DIV_Messaggi");
                Aggiorna_Griglia_Esercizi();
            }
        }, function (risposta) {
            if (risposta.RispostaStringa !== "") {
                kendo.alert(risposta.RispostaStringa);
            } else {
                MessaggioErrore_Bootstrap(risposta.Errore, "DIV_Messaggi");
            }
        });
}
