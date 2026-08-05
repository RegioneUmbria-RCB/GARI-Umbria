var resxRicercaIndiciDocumentaliUC = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Scadenzario/UserControl/App_LocalResources/Ricerca_Indici_Documentale_UC.ascx.resx"
];


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

    if (Array.isArray(resxArrPath) && resxArrPath.length > 0) {
        // Carico i files resx per le traduzioni
        resxArrPath.forEach(function (resxSinglePath) {
            resxRicercaIndiciDocumentaliUC.unshift(readResxFile(resxSinglePath, "Ricerca_Indici_Documentale_UC_jQueryDocReady.js"));
        });
    }

    //inizializzazione della pagina la prima volta che viene caricata

    $(".preArea").show();
    

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });
    

    //// Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    //document.getElementById("panelArea").style.opacity = "1";


   //ConfiguraGrigliaIndici("tab_griglia_Indici");*/
    
    
    //fine controlli

    $('#dialogSessioneScaduta').on('show.bs.modal', function (event) {
        impostaRedirectStart();
    });

  //Anna 31/05/22 Se l'utente non ha permessi di scrittura, nascondo il pulsante di aggiunta nuovo indice
    if ($("input[name$='hf_UtenteAbilitatoScrittura']").val() === "False") {
        $("#id_nuovo").hide();
    }

    //eventi di click pulsanti
    $("#btn_Nuovo").click(
        function () {            
            Nuovo();
        });
    
    //eventi di click pulsanti
    $("#btn_esci").click(
        function () {
            //AggiornaDati(true);
        });


    

    $.logThis("DocReady: FINE");

});
