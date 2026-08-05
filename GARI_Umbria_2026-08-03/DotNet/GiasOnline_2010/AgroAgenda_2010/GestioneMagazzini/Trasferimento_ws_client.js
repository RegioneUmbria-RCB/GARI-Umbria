var indirizzoHttp = "./Trasferimento.aspx";
var indirizzoHttp_DocContabile_WS = "../GestioneContabilita/DocContabile_WS.aspx";

function InserisciScaricoTrasferimento(piva, idAgenda, idMovScarico, idMovCarico, trasf, messaggioDettagliato) {

    var risultatoInserimento;

    //se è il primo giro id_agenda è = "", ma questo dà fastidio, quindi metto zero, tanto verrà scritta da questa funzione
    if (idAgenda === "")
        idAgenda = 0;

    var param = kendo.stringify({
        piva: piva,
        idAgenda: idAgenda,
        idMovScarico: idMovScarico,
        idMovCarico: idMovCarico,
        trasferimento: trasf,
        messaggioDettagliato: messaggioDettagliato
    });

    ajaxAgronicaSync(indirizzoHttp + "/InserisciScaricoTrasferimento",
        param,
        false,
        function (risposta) {
            risultatoInserimento = risposta;
        },
        function (risposta) {
            risultatoInserimento = risposta;
        });

    return risultatoInserimento;
}

function ModificaScaricoTrasferimento(piva, trasf, messaggioDettagliato) {

    var risultatoModifica;

    var param = kendo.stringify({
        piva: piva,
        trasferimento: trasf,
        messaggioDettagliato: messaggioDettagliato
    });

    ajaxAgronicaSync(indirizzoHttp + "/ModificaScaricoTrasferimento",
        param,
        false,
        function (risposta) {
            risultatoModifica = risposta;
        },
        function (risposta) {
            risultatoModifica = risposta;
        });

    return risultatoModifica;
}

function ModificaCaricoTrasferimento(piva, trasf, messaggioDettagliato) {

    var risultatoModifica;

    var param = kendo.stringify({
        piva: piva,
        trasferimento: trasf,
        messaggioDettagliato: messaggioDettagliato
    });

    ajaxAgronicaSync(indirizzoHttp + "/ModificaCaricoTrasferimento",
        param,
        false,
        function (risposta) {
            risultatoModifica = risposta;
        },
        function (risposta) {
            risultatoModifica = risposta;
        });

    return risultatoModifica;
}

function CancellaScaricoTrasferimento(piva, trasf, messaggioDettagliato) {

    var risultatoCancellazione;

    var param = kendo.stringify({ piva: piva, trasferimento: trasf, messaggioDettagliato: messaggioDettagliato });

    ajaxAgronicaSync(indirizzoHttp + "/CancellaScaricoTrasferimento",
        param,
        false,
        function (risposta) {
            risultatoCancellazione = risposta;
        },
        function (risposta) {
            risultatoCancellazione = risposta;
        });

    return risultatoCancellazione;
}

function CancellaTotaleTrasferimento(piva, idAgenda, messaggioDettagliato) {

    var risultatoCancellazione;

    var param = kendo.stringify({ piva: piva, idAgenda: idAgenda, messaggioDettagliato: messaggioDettagliato });

    ajaxAgronicaSync(indirizzoHttp + "/CancellaTotaleTrasferimento",
        param,
        false,
        function (risposta) {
            risultatoCancellazione = risposta;
        },
        function (risposta) {
            risultatoCancellazione = risposta;
        });

    return risultatoCancellazione;
}

function LeggiCelle(options) {
    options.success(RicercaCelle(false, 20, $(cIdPiva).val(), 0));
}



function LeggiUnitaMisuraTrasporto(options) {
    options.success(RicercaMisuraTrasporto());
}



function RicercaMisuraTrasporto() {

    var elencoUdmTrasporto = "";
    var param = "";

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/Udm_Trasporto",
        param,
        false,
        function (risposta) {
            var risp = JSON.parse(risposta.RispostaStringa);
            elencoUdmTrasporto = risp;
        }, null);

    return elencoUdmTrasporto;
}
