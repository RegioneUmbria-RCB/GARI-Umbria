//DOCUMENT READY
$(document).ready(function() {
    // Non c'è nulla, tutto spostato nella funzione impostaRifCatastaliUC,
    // in modo che sia il chiamante a gestire quando farla.
});

function impostaRifCatastaliUC() {

    $.logThis("EMBEDDED RifCatastaliUC_jQueryDocReady: INIZIO");
  
    ConfiguraGrigliaRifCatastali(idTabGrigliaRifCatastali);

    $.logThis("EMBEDDED RifCatastaliUC_jQueryDocReady: FINE");

}
