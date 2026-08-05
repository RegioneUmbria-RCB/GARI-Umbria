
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

    //inizializzazione della pagina la prima volta che viene caricata

    kendo.ui.DatePicker.fn.options.max = new Date(2100, 11, 31);
    $(".anagArea").show();

    $('.nav-tabs a').on('shown.bs.tab', function (event) {
        var currentTabName = $(event.target).text();         // active tab
        var previousTabName = $(event.relatedTarget).text();  // previous tab
        var tabId = event.target.id;
        OnTabShow(tabId);
    });

    Popola_Numeratori_Tipo("tab_numeratore_tipo");
    tabStripAperti.push("a_tabNumeratoriTipo");

    var gridNumTipi = $("#tab_numeratore_tipo").data("kendoGrid");
    if (gridNumTipi != null && gridNumTipi != undefined)
        gridNumTipi.bind("cellClose", grid_cellClose);

    var gridNumPF = $("#tab_numeratore_prefisso_suffisso").data("kendoGrid");
    if (gridNumPF != null && gridNumPF != undefined)
        gridNumPF.bind("cellClose", grid_cellClose);

    //fine controlli

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    $.logThis("DocReady: FINE");

});