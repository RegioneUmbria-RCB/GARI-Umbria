

//////////////////////////////////////////////////////////
// Analisi Progetti
//////////////////////////////////////////////////////////

var indirizzohttp = "./FiltroStampeDAA.aspx";

function Report_DAA() {

    var risp = "";

    var param = "{ piva: '" + $(piva).val() + "'" +
        ", dataDal: '" + $('input[name$="DaData"]').val() + "'" +
        ", dataAl: '" + $('input[name$="AData"]').val() + "'" +
        ", dataStampa: '" + $('input[name$="DataRif"]').val() + "'" +
        ", protocollo: '" + $('input[name$="numProt"]').val() + "'" +
        ", ufficioDogane: '" + $('input[name$="uffDog"]').val() +  "'" +
        ", tipoReport: '" + Get_KendoDDLValue("idTipoReport") + "'" +
        " }";

    ajaxAgronica(indirizzohttp + "/Report_DAA",
        param,
        function (risposta) {
            risp = risposta.RispostaStringa;
            var win = window.open(risp);
            win.focus();
        }, null);

}

function controllaCampiObbligatori() {

    var returnValue = true;

    dataStampa = $('input[name$="DataRif"]').val();
    if (dataStampa == "") {
        returnValue = false;
        MessaggioErrore_Bootstrap("Data stampa non valida <br/>", "DIV_Messaggi");
    }

    protocollo = $('input[name$="numProt"]').val();
    if (protocollo == "") {
        returnValue = false;
        MessaggioErrore_Bootstrap("Numero protocollo è obbligatorio <br/>", "DIV_Messaggi");
    }

    uffDogane = $('input[name$="uffDog"]').val();
    if (uffDogane == "") {
        returnValue = false;
        MessaggioErrore_Bootstrap("Ufficio Dogane è obbligatorio <br/>", "DIV_Messaggi");
    }

    return returnValue    

}

function controllaDateLancio() {

    var dataValida = true;

    dataDaControllare = $('input[name$="DaData"]').val();
    if (dataDaControllare !== "")
        dataValida = isValidDate(dataDaControllare);
    if (!dataValida)
        MessaggioErrore_Bootstrap("Da data non valida <br/>", "DIV_Messaggi");

    dataDaControllare = $('input[name$="AData"]').val();
    if (dataDaControllare !== "")
        dataValida = isValidDate(dataDaControllare);
    if (!dataValida)
        MessaggioErrore_Bootstrap("A data non valida <br/>", "DIV_Messaggi");

    dataDaControllare = $('input[name$="DataRif"]').val();
    if (dataDaControllare !== "")
        dataValida = isValidDate(dataDaControllare);
    if (!dataValida)
        MessaggioErrore_Bootstrap("Data stampa non valida <br/>", "DIV_Messaggi");

    return dataValida;

}

function RicercaRiepilogoGaranzieCircolazione(options) {

    var dataDa = $('input[name$="DaData"]').val();
    var dataA = $('input[name$="AData"]').val();
    var pi = $(piva).val();

    var param = "{ piva: '" + pi +"', dataDal:'" +dataDa + "', dataAl:'" + dataA +"' }"

    ajaxAgronica(indirizzohttp + "/CaricaGrigliaLiquidazioneSocio",
                    param,
                    function (risposta) {
                        risp = JSON.parse(risposta.RispostaStringa);
                        options.success(risp);
                    },
                    function (risposta) {
                        MessaggioErrore_Bootstrap("Errore: " + risposta.Errore, "DIV_Messaggi");
                    });
}
