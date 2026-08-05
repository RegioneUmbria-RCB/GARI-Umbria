

//////////////////////////////////////////////////////////
// Griglia Castelletto
//////////////////////////////////////////////////////////

function EmptyRead(options) { }
function EmptySubmit(options) { }

function CaricaGrigliaCastelletto(options) {

    if (parseInt($(cIdAgenda).val()) !== 0) {

        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/CaricaGrigliaCastelletto",
            kendo.stringify({
                piva: $(cIdPiva).val(),
                id_agenda: parseInt($(cIdAgenda).val()),
                lav_cod: cIdLavCod
            }),
            false,
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);

                var riepilogoImporti = JSON.parse(risposta.ParametroDue_stringa);

                Set_KendoNumTBValue("idImponibileTotaleLordo", riepilogoImporti.ImponibileLordo);
                Set_KendoNumTBValue("idVariazione", riepilogoImporti.Variazioni);
                Set_KendoNumTBValue("idImponibileNetto", riepilogoImporti.ImponibileNetto);
                Set_KendoNumTBValue("idImposta", riepilogoImporti.Imposta);
                Set_KendoNumTBValue("idTotale", riepilogoImporti.TotaleDocumento);

                options.success(risp);

            }, null);
    }
}
