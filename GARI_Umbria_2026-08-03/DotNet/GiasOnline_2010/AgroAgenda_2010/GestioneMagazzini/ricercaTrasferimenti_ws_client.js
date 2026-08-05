

//////////////////////////////////////////////////////////
// Movimenti di Campionamento
//////////////////////////////////////////////////////////

var indirizzohttp = "./RicercaTrasferimenti.aspx";   


function RicercaTrasferimenti() {

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        idAgenda: 0,
        dataDal: $('input[name$="txt_DataDal"]').val(),
        dataAl: $('input[name$="txt_DataAl"]').val(),
        des_TestataGriglia: $('input[name$="txt_des_TestataGriglia"]').val()
    });

    ajaxAgronica(indirizzohttp + "/ElencoTrasferimenti",
        param,
        function (risposta) {
            $('input[name$="hdKendo_Trasferimenti"]').val(risposta.RispostaStringa);
            popolaTestateTrasferimenti("tab_testata_griglia_trasferimenti");
        }, null);
}

function RicercaTrasferimentiDettagli() {

    var param = kendo.stringify({
        piva: $(cIdPiva).val(),
        dataDal: $('input[name$="txt_DataDal"]').val(),
        dataAl: $('input[name$="txt_DataAl"]').val(),
        des: $('input[name$="txt_des_TestataGriglia"]').val(),
        categorieProdotti: KendoMultisel("multiselCategorie").value().join("|"),
        codiciProdotti: KendoMultisel("multiselProdotti").value().join("|"),
        specie: KendoMultisel("multiselSpecie").value().join("|"),
        varieta: KendoMultisel("multiselVarieta").value().join("|"),
    });

    ajaxAgronica(indirizzohttp + "/ElencoTrasferimentiDettagli",
        param,
        function (risposta) {
            $('input[name$="hdKendo_TrasferimentiDettagli"]').val(risposta.RispostaStringa);
            popolaTestateTrasferimentiDettagli("tab_testata_griglia_trasferimenti_dettagli");
        }, null);
}

function ModificaTrasferimento(piva, idAgenda, tipoOperazione) {

    var param = kendo.stringify({
        piva: piva,
        idAgenda: idAgenda,
        tipoOperazione: tipoOperazione
    });
    var risp = "";
    ajaxAgronica(indirizzohttp + "/ApriModificaTrasferimento",
        param,
        function (risposta) {
            risp = risposta.RispostaStringa;
            //var win = window.open(risp);
            //win.focus();
            window.location.href = risp;
        }, null);
}

function CancellaTotaleTrasferimento(piva, idAgenda) {

    var risultatoCancellazione;

    var param = kendo.stringify({ piva: piva, idAgenda: idAgenda, messaggioDettagliato: true });

    ajaxAgronicaSync(indirizzohttp + "/CancellaTotaleTrasferimento",
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

function Leggi_Specie() {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Specie",
        "{ }",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Leggi_Varieta(filtro_specie) {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Varieta",
        "{ filtro_specie: '" + filtro_specie + "'}",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}

function Leggi_Categorie_Magazzino() {

    var risultato_lettura;

    ajaxAgronicaSync(indirizzohttp + "/Leggi_Categorie_Magazzino",
        "{ }",
        false,
        function (risposta) {
            risp = JSON.parse(risposta.RispostaStringa);
            risultato_lettura = risp;
        }, null);

    return risultato_lettura;
}