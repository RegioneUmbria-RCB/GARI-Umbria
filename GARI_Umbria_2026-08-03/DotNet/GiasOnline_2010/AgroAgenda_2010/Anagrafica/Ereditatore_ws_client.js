/*
*
*  
*
*
*/


function LeggiElencoProprieta() {
    ajaxAgronicaSync("Ereditatore.aspx/LeggiElencoProprieta", JSON.stringify({}), false,
        function (risposta) {
            if (risposta.RispostaOK) {
                jSonParsed_Kendo_ElencoProp = JSON.parse(risposta.RispostaStringa);
            }
            else {
                alert(risposta.Errore);
            }
        }, null);
}

function leggi_ImpiantiConProprieta(multiselect){
    var elencoValori = multiselect.value();
    ajaxAgronicaSync("Ereditatore.aspx/LeggiImpiantiConProprieta", JSON.stringify({valori: elencoValori,filtroDistinte: $('#filtroDistinte').find('input[name=optdistinte]:checked').val(), dataDistinta: $('#datepicker_distinta').val()}), false,
        function (risposta) {
            if (risposta.RispostaOK) {
                jSonParsed_Kendo_EredImpianti = JSON.parse(risposta.RispostaStringa);
            }
            else {
                alert(risposta.Errore);
            }
        }, null);
}