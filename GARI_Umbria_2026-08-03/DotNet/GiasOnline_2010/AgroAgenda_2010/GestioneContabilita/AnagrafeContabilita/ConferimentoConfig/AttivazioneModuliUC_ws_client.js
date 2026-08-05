function Leggi_StatoAttivazioni() {
    var result = null;
    var param = "{ piva: '" + piva + "' }";
    ajaxAgronicaSync(indirizzohttp + "/LeggiStatoAttivazioni",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            result = risp;
        }, null);
    return result;
}
 
function Salva_AttivazioneModuli() {
    var trasformazioniVegetali = $("#chkTrasformazioniVegetali").data("kendoSwitch").check();
    var trasformazioniAnimali = $("#chkTrasformazioniAnimali").data("kendoSwitch").check();
    var param = "{ piva: '" + piva + "', trasformazioniVegetali: " + trasformazioniVegetali + ", trasformazioniAnimali: " + trasformazioniAnimali + " }";
    ajaxAgronicaSync(indirizzohttp + "/SalvaAttivazioneModuli",
        param,
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            statoAttivazione = risp;
            MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
            ImpostaSwitches();
        }, null);
}
