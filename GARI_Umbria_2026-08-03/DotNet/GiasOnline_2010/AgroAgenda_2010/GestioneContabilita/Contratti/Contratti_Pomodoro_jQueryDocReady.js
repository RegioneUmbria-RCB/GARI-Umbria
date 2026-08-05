
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

    //$(".padreImpianti").show();
    

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });

    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelArea").style.opacity = "1";

    var d = new Date();
    $('input[name$="txt_anno"]').val(d.getFullYear());

    if ($(cContratto_Cod).val() == 0 && $('input[name$="txt_anno"]').val() == "2021" && $(cVeg_Cod).val() == 52) {

       $('input[name$="txt_DMA"]').val(8);
        $('input[name$="txt_Franchigia_DMA"]').val(3);
        $('input[name$="txt_DMI"]').val(12);
        $('input[name$="txt_Coefficiente"]').val("0,5");
        $('input[name$="txt_Limite"]').val(6);

    } else {

        $('input[name$="txt_superficie"]').val(0);
        $('input[name$="txt_resa_prevista"]').val(0);
        $('input[name$="txt_premio"]').val(0);
        $('input[name$="txt_tetto"]').val(0);
        $('input[name$="txt_premio2"]').val(0);
        $('input[name$="txt_tetto2"]').val(0);
        $('input[name$="txt_DMA"]').val(0);
        $('input[name$="txt_Franchigia_DMA"]').val(0);
        $('input[name$="txt_DMI"]').val(0);
        $('input[name$="txt_Coefficiente"]').val(0);
        $('input[name$="txt_Limite"]').val(0);

    }
    
    $('input[name$="txt_Data"]').val(formattedDate(new Date(), '/'));
    $('input[name$="txt_DataPremio"]').val(formattedDate(new Date(), '/'));
    $('input[name$="txt_DataPremioFine"]').val(formattedDate(new Date(), '/'));
    $('input[name$="txt_DataPremio2"]').val(formattedDate(new Date(), '/'));
    $('input[name$="txt_DataPremioFine2"]').val(formattedDate(new Date(), '/'));

    //Sospeso
    //$('input[name$="lbl_tetto2"]').hide();
    //$('input[name$="txt_tetto2"]').hide();

    creaKendoSwitch("ChkPremio", "Sì", "No", false, function (e) {
        //if (e.checked) {

        //}
        //else {
           
        //}
    });

    creaKendoSwitch("ChkPremio2", "Sì", "No", false, function (e) {
        //if (e.checked) {

        //}
        //else {

        //}
    });

    Elenco_Causale = [
        { "Cau_Contratto": 9300, "Cau_Contratto_Des": "Contratto di Conferimento Colturale"},
        { "Cau_Contratto": 9301, "Cau_Contratto_Des": "Contratto di Conferimento Colturale Azienda Agricola"}
    ];

    creaKendoDropDownList("cmbCausale", { read: RiempicmbCausale }, "Cau_Contratto_Des", "Cau_Contratto").bind("change", cmbCausale_change);
    
    Elenco_Clausola = Elenco_Clausola_Riempi(true);
    Elenco_Specie = Elenco_Specie_Riempi(true);
    Elenco_Conferente = Elenco_Conferente_Riempi(true);
    Elenco_Listino = Elenco_Listino_Riempi(false);

    Elenco_Centri = Elenco_Centri_Riempi(true);
    //Elenco_Fabbricati = Elenco_Fabbricati_Riempi(true);
    
    creaKendoDropDownList("cmbConferente", { read: RiempicmbConferente }, "Rag_Soc_Contatto", "Cod_RisUm").bind("change", cmbConferente_change);
    creaKendoDropDownList("cmbListino", { read: RiempicmbListino }, "Listino_Des", "Listino_Cod").bind("change", cmbListino_change);
    creaKendoDropDownList("cmbCentri", { read: RiempicmbCentri }, "sa_nome", "sa_cod").bind("change", cmbCentri_change);
    creaKendoDropDownList("cmbFabbricati", { read: RiempicmbFabbricati }, "Fabbricato_Des", "Fabbricato_Cod").bind("change", cmbFabbricati_change);

    LeggiContratto();
    LeggiValori();

    ConfiguraGrigliaFasi("tab_griglia_fasi", false);
    ConfiguraGrigliaClausole("tab_griglia_clausole", false);

    //fine controlli

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

    $("#btn_salva").click(
        function () {
            AggiornaDati(true);
        });
    
    $.logThis("DocReady: FINE");

});

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}