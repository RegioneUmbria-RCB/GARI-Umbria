
//DOCUMENT READY
$(document).ready(function () {


    $.logThis("DocReady: INIZIO");

        //Controllo se ha i permessi di lettura e scrittura sulla pagina
        UtenteAbilitatoScrittura = $("input[name$='hf_UtenteAbilitatoScrittura']").val() == "True"

        //Se l'utente è abilitato alla scrittura aggiungo i pulsanti per l'inserimento di nuovi elementi
    if (UtenteAbilitatoScrittura == true) {
        $(".add").show();

        //Mostro i bottoni Autorizza e Togli Autorizzazione 
        $("#btn_autorizza_grid_tipologiexindice").show();
        $("#btn_togli_autorizza_grid_tipologiexindice").show();
    }
    else if (UtenteAbilitatoScrittura == false){
        //Nascondo i bottoni Autorizza e Togli Autorizzazione 
        $("#btn_autorizza_grid_tipologiexindice").hide();
        $("#btn_togli_autorizza_grid_tipologiexindice").hide();
    }

    // Anna 23/07/21: Aggiunta button per spostare documenti su DB 
    if ($("input[name$='hf_UtenteAbilitatoExportSuDB']").val() == "True") {
        $("#spostaCategoria").show();
        $("#spostaTipologia").show();
    } else {
        $("#spostaCategoria").hide();
        $("#spostaTipologia").hide();
    }

        //Leggo l'anagrafica delle aziende, filtrate in base alla visibilità utente
        //ddlAzienda_Load();


    //Carico la Listbox con le aree associate alla Piva della atb Categorie/Tipologie
    LsbAree_Load();

    $(".kendoCalendar").kendoDatePicker({
        footer: "#: kendo.toString(data, 'd')#",  //Template per il footer
        max: new Date(2100, 11, 31)
    });

    //Controllo Tab Default
    if ($(cTabDefault).val() == "tab_indici") {
        $("#a_tabIndici").tab("show");
    }


    // Introdotto per mostrare la pagina solo quando tutti i controlli sono caricati
    document.getElementById("panelArea").style.opacity = "1";

    $.logThis("DocReady: FINE");

});
