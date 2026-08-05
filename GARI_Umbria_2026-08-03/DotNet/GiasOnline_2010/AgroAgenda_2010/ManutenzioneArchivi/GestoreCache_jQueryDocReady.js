
//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    var ca = ($(cacheAbilitata).val() === "True" ? true : false);
    creaKendoSwitch("chkCacheEnabled", "Si", "No", ca, abilitaCache);

    popolaGrigliaCache();

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    $("#btn_pulisci_cache").on('click', function (e) {
        pulisciCache();
    });

    $("#btn_pulisci_cache_permessi").on('click', function (e) {
        pulisciCachePermessi();
    });

    $("#btn_pulisci_cache_impostazioni").on('click', function (e) {
        pulisciCacheImpostazioni();
    });

    $("#btn_aggiorna").on('click', function (e) {
        aggiorna();
    });

    $("#dialogErrorKendo").kendoDialog({
        title: "Elenco chiamate di clear cache fallite", content: "", visible: false
    });

    $("#dialogOkKendo").kendoDialog({
        title: "Elenco chiamate di clear cache completate con successo", content: "", visible: false
    });

    $.logThis("DocReady: FINE");

});