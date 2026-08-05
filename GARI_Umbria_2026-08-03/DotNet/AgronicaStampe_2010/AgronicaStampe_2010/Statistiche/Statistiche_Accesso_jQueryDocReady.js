$(document).ready(function () {

    $.logThis("DocReady: INIZIO");
    docReady();


});

function docReady() {
    pathNetCoreApi = LeggiPathNetCoreApi();
    caricaRicercaRapida();
    iniziallizaTooltips();
    iniziallizaKendoDate();
    iniziallizaOnClickEvents();
    $(".btn").unbind('hover');
}