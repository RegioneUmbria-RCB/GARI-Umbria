
//DOCUMENT READY
$(document).ready(function () {

    // Evita l'utilizzo dell'invio
    // TODO Stefano
    //$(window).keydown(function (event) {
    //    if (event.keyCode == 13) {
    //        event.preventDefault();
    //        return false;
    //    }
    //});

    $.logThis("DocReady: INIZIO");
    RicercaSpecie($(cIdPiva).val());
    //inizializzazione della pagina la prima volta che viene caricata
    $(".anagArea").show();
    popolaParamEntrata("tab_parametri_specie_varieta");
    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);
    RicercaTipoRiferimentoPrezzi();
    $.logThis("DocReady: FINE");


});