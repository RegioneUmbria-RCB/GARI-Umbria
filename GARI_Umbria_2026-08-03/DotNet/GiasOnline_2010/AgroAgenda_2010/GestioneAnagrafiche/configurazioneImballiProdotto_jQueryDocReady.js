
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

    //    $('#aspnetForm').change(function () {
    //        controlla_form();
    //    });

    $.logThis("DocReady: INIZIO");

    //inizializzazione della pagina la prima volta che viene caricata

    //$(".searchArea").show();
    $(".modifyArea").show();


    //eventi di click pulsanti
    //    $("#btn_ricerca").click(function () {
    //        dataDaControllare = $('input[name$="txt_DataRif"]').val();
    //        //dataDaControllare = $("#txt_kDataRif").data("kendoDatePicker").value();
    //        dataValida = true;
    //        if (dataDaControllare != "")//(dataDaControllare != null)
    //            dataValida = isValidDate(dataDaControllare); //d.isValid();//
    //        if (!dataValida)
    //          MessaggioErrore_Bootstrap("Data non valida", "DIV_Messaggi");
    //        else {
    //          popolaTestateGrigliaCampionamento("tab_testata_griglia_campionamento");
    //          $(".modifyArea").show();
    //        }
    //    });

    Tipo_Imballaggio = RicercaTipoImballaggio(true);
    Elenco_Imballaggi = RicercaImballaggio(true, $(cIdPiva).val(), 4);
    Elenco_Contenitori = RicercaImballaggio(true, $(cIdPiva).val(), 8);
    Elenco_Confezioni = RicercaImballaggio(true, $(cIdPiva).val(), 5);
    Elenco_Specie = RicercaSpecie($(cIdPiva).val());

    // Cerco se siamo in gestione materiale vivaistico
    let imp_848 = RicercaUtenti_Impostazioni(enum_FF_gest_materiale_vivaistico, 2);
    FF_gest_materiale_vivaistico = (imp_848 === '1');

    popolaTestateGrigliaImballiProdotto("tab_griglia_configurazioneimballiprodotto");

    //fine controlli

    $('#dialogSessioneScaduta').on("show.bs.modal", function (event) {
        impostaRedirectStart();
    });



    $.logThis("DocReady: FINE");

});