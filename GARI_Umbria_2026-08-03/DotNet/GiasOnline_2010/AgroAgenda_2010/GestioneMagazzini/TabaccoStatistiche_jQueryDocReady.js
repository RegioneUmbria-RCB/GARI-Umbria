

$(document).ready(function () {


    //imposto il click della ricerca
    $("#btn_ricerca").click(function () {

        var validato_ok = valida_ricerca();

        if(validato_ok)
            eseguiRicerca();

    });

    // Al click dei radio filtri...
    $(".filtro_report").click(function () {
        //... mostro i campi aggiuntivi
        $('#more_search').show();
    });

    $("input:not(.filtro_report)").click(function () {
        //... nascondo i campi aggiuntivi
        $('#more_search').hide();
    });


    //imposto il click dell'esportazione del report
    $("#exportRep").click(function () {
        EsportaSuReport();
    });

});