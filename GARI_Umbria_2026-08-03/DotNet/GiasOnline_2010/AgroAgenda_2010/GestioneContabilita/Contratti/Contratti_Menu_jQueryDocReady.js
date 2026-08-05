
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

    

    //tabstrip = $("#tabstrip_dettagli").kendoTabStrip({
    //    animation: false
    //}).data("kendoTabStrip");


    //tabstrip = $("#tabClausole").kendoTabStrip({
    //    animation: false
    //}).data("kendoTabStrip");

    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelArea").style.opacity = "1";

    Elenco_Causale = [
        { "Cau_Contratto": 9300, "Cau_Contratto_Des": "Contratto di Conferimento Colturale" }
    ];

    Elenco_Anno = Elenco_Anno_Riempi(false);
    Elenco_Conferente = Elenco_Conferente_Riempi(false);
    Elenco_Prodotto = Elenco_Prodotto_Riempi(false);
    
    creaKendoMultiselect("multiselAnno", { read: RiempiContrattoAnno, data: { Anno: 0 } }, "Anno_Des", "Anno", null, null, null, null);
    creaKendoMultiselect("multiselConferente", { read: RiempiContrattoConferente, data: { Cod_Risum: 0 } }, "Rag_Soc_Contatto", "Cod_RisUm", null, null, null, null);
    creaKendoMultiselect("multiselProdotto", { read: RiempiContrattoProdotto, data: { Mat_Cod: 0 } }, "Mat_Des", "Mat_Cod", null, null, null, null);


    ConfiguraGrigliaContrattiPomodoro("tab_griglia_contratti_pomodoro", false);
    ConfiguraGrigliaClausole("tab_griglia_clausole", false);

    //fine controlli

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

  

    //eventi di click pulsanti
    $("#btn_Ricerca").click(
        function () {
         // costi_ricavi = 0;
            Ricerca_Contratti_Pomodoro();
        });
    

    $("#btn_nuovo_contratto").click(
        function () {
            Nuovo_Contratto_Pomodoro();
        });

    $("#btn_nuovo_contratto_generico").click(
        function () {
            Nuovo_Contratto_Generico();
        });


    //eventi di click pulsanti
    $("#SalvaClausole").click(
        function () {
            SalvaClausole(true);
        });

    
    $.logThis("DocReady: FINE");

});

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}

