var indirizzohttp = "./Editor.aspx";   

function RicercaTestoEditor() {

    var param = JSON.stringify({
        Piva: $(cIdPiva).val(),
        Sa_Cod: $(cSa_Cod).val(),
        Cod_RisUm: $(cCod_RisUm).val(),
        Elem_Cod: $(cElem_Cod).val(),
        Pro_Cod: $(cPro_Cod).val(),
        Mat_Cod: $(cMat_Cod).val()
    });

    ajaxAgronicaSync(indirizzohttp + "/RicercaTestoEditor",
        param, false,
        function (risposta) {

            var testo = decodeURI(risposta.RispostaStringa);
            $('input[name$="hdTesto"]').val(testo);
            riempiTesto("editorID");
        },
        null
    );

}
 
function Aggiorna_Testo() {

    var editor = $("#editorID").data("kendoEditor");
    var testo = editor.value();
    testo = encodeURI(testo);
    testo = replaceAll(testo, "'", "\\'");

    var param = JSON.stringify({
        Piva: $(cIdPiva).val(),
        Sa_Cod: $(cSa_Cod).val(),
        Modulo: 0,
        Testo: testo,
        Soluzione: "",
        Stato: "0",
        Testo_Parametri: "",
        Soluzione_Parametri: "",
        Cod_RisUm: $(cCod_RisUm).val(),
        Colore: 12648447,
        Modalita: 0,
        Elem_Cod: $(cElem_Cod).val(),
        Pro_Cod: $(cPro_Cod).val(),
        Mat_Cod: $(cMat_Cod).val()
    });

    ajaxAgronica(indirizzohttp + "/AggiornaTestoEditor",
        param,
        function (risposta) {
            var risp = risposta.RispostaStringa;
            MessaggioTuttoOK_Bootstrap("Aggiornamento effettuato correttamente", "DIV_Messaggi");
            
        }, null);

}