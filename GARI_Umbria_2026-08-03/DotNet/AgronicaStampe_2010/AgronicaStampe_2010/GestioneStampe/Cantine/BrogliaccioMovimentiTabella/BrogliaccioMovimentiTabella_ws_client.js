
var indirizzohttp = "./BrogliaccioMovimentiTabella.aspx";

function CaricaKendo(qs_piva, dataDa, dataA, mese, anno, f) {
    var paramPlan = '{ "qs_piva": "' + qs_piva + '", "dataDa": "' + dataDa + '" , "dataA" : "' + dataA + '" , "mese" : "' + mese + '" , "anno" : "' + anno + '" }';
    ajaxAgronica(indirizzohttp + "/CaricaKendoBrogliaccioMovimenti", paramPlan, function (risposta) {
        f(risposta);
    }, null);
}