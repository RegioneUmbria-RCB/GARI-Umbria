var CurrentDate = new Date();
var indirizzohttp = "./DomandaIrrigua_WS.aspx";

var CurrentMod = false;

$(document).ready(function () {
    $.logThis("DocReady: INIZIO");

    let Anno = CurrentDate.getFullYear();
    if ($(cIdAnno).val() != null && $(cIdAnno).val() != "") {
        Anno = $(cIdAnno).val();
    }

    LeggiInizializzaDomanda($(cIdIDDomanda).val(), $(cIdPiva).val(), Anno);

    AbilitaModifica($(cIdEnableMod).val());

    $.logThis("DocReady: FINE");
});